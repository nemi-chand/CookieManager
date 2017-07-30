// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.Options;

namespace CookieManager
{
    /// <summary>
    /// This handles cookies that are limited by per cookie length. It breaks down long cookies for responses, and reassembles them
    /// from requests.
    /// </summary>
    internal class ChunkingHttpCookie
	{
		private readonly CookieManagerOptions _cookieManagerOptions;
		
        private const string ChunkKeySuffix = "C";
        private const string ChunkCountPrefix = "chunks-";

        public ChunkingHttpCookie(IOptions<CookieManagerOptions> optionAccessor)
        {
            // Lowest common denominator. Safari has the lowest known limit (4093), and we leave little extra just in case.
            // See http://browsercookielimits.x64.me/.
            // Leave at least 40 in case CookiePolicy tries to add 'secure', 'samesite=strict' and/or 'httponly'.           
			_cookieManagerOptions = optionAccessor.Value;
		}
        

        // Parse the "chunks-XX" to determine how many chunks there should be.
        private int ParseChunksCount(string value)
        {
            if (value != null && value.StartsWith(ChunkCountPrefix, StringComparison.Ordinal))
            {
                var chunksCountString = value.Substring(ChunkCountPrefix.Length);
                int chunksCount;
                if (int.TryParse(chunksCountString, NumberStyles.None, CultureInfo.InvariantCulture, out chunksCount))
                {
                    return chunksCount;
                }
            }
            return 0;
        }

        /// <summary>
        /// Get the reassembled cookie. Non chunked cookies are returned normally.
        /// Cookies with missing chunks just have their "chunks-XX" header returned.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="key"></param>
        /// <returns>The reassembled cookie, if any, or null.</returns>
        public string GetRequestCookie(HttpContext context, string key)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var requestCookies = context.Request.Cookies;
            var value = requestCookies[key];
            var chunksCount = ParseChunksCount(value);
            if (chunksCount > 0)
            {
                var chunks = new string[chunksCount];
                for (var chunkId = 1; chunkId <= chunksCount; chunkId++)
                {
                    var chunk = requestCookies[key + ChunkKeySuffix + chunkId.ToString(CultureInfo.InvariantCulture)];
                    if (string.IsNullOrEmpty(chunk))
                    {
                        if (_cookieManagerOptions.ThrowForPartialCookies)
                        {
                            var totalSize = 0;
                            for (int i = 0; i < chunkId - 1; i++)
                            {
                                totalSize += chunks[i].Length;
                            }
                            throw new FormatException(
                                string.Format(
                                    CultureInfo.CurrentCulture,
                                    "The chunked cookie is incomplete. Only {0} of the expected {1} chunks were found, totaling {2} characters. A client size limit may have been exceeded.",
                                    chunkId - 1,
                                    chunksCount,
                                    totalSize));
                        }
                        // Missing chunk, abort by returning the original cookie value. It may have been a false positive?
                        return value;
                    }

                    chunks[chunkId - 1] = chunk;
                }

                return string.Join(string.Empty, chunks);
            }
            return value;
        }

        /// <summary>
        /// Appends a new response cookie to the Set-Cookie header. If the cookie is larger than the given size limit
        /// then it will be broken down into multiple cookies as follows:
        /// Set-Cookie: CookieName=chunks-3; path=/
        /// Set-Cookie: CookieNameC1=Segment1; path=/
        /// Set-Cookie: CookieNameC2=Segment2; path=/
        /// Set-Cookie: CookieNameC3=Segment3; path=/
        /// </summary>
        /// <param name="context"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public void AppendResponseCookie(HttpContext context, string key, string value, CookieOptions options)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var template = new SetCookieHeaderValue(key)
            {
                Domain = options.Domain,
                Expires = options.Expires,
                HttpOnly = options.HttpOnly,
                Path = options.Path,
                Secure = options.Secure,
            };

            var templateLength = template.ToString().Length;

            value = value ?? string.Empty;

            // Normal cookie
            var responseCookies = context.Response.Cookies;
            if (!_cookieManagerOptions.ChunkSize.HasValue || _cookieManagerOptions.ChunkSize.Value > templateLength + value.Length)
            {
                responseCookies.Append(key, value, options);
            }
            else if (_cookieManagerOptions.ChunkSize.Value < templateLength + 10)
            {
                // 10 is the minimum data we want to put in an individual cookie, including the cookie chunk identifier "CXX".
                // No room for data, we can't chunk the options and name
                throw new InvalidOperationException("The cookie key and options are larger than ChunksSize, leaving no room for data.");
            }
            else
            {
                // Break the cookie down into multiple cookies.
                // Key = CookieName, value = "Segment1Segment2Segment2"
                // Set-Cookie: CookieName=chunks-3; path=/
                // Set-Cookie: CookieNameC1="Segment1"; path=/
                // Set-Cookie: CookieNameC2="Segment2"; path=/
                // Set-Cookie: CookieNameC3="Segment3"; path=/
                var dataSizePerCookie = _cookieManagerOptions.ChunkSize.Value - templateLength - 3; // Budget 3 chars for the chunkid.
                var cookieChunkCount = (int)Math.Ceiling(value.Length * 1.0 / dataSizePerCookie);

                responseCookies.Append(key, ChunkCountPrefix + cookieChunkCount.ToString(CultureInfo.InvariantCulture), options);

                var offset = 0;
                for (var chunkId = 1; chunkId <= cookieChunkCount; chunkId++)
                {
                    var remainingLength = value.Length - offset;
                    var length = Math.Min(dataSizePerCookie, remainingLength);
                    var segment = value.Substring(offset, length);
                    offset += length;

                    responseCookies.Append(key + ChunkKeySuffix + chunkId.ToString(CultureInfo.InvariantCulture), segment, options);
                }
            }
        }
		
		public void RemoveCookie(HttpContext context,string key)
		{
			//validate input
			if(context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			if(key == null)
			{
				throw new ArgumentNullException(nameof(key));
			}

			var value = context.Request.Cookies[key];
			var chunksCount = ParseChunksCount(value);
			if (chunksCount > 0)
			{
				//delete all the chunks
				for (int chunkId = 1; chunkId <= chunksCount; chunkId++)
				{
					context.Response.Cookies.Delete(key + ChunkKeySuffix + chunkId.ToString(CultureInfo.InvariantCulture));
				}
			}
			else
				context.Response.Cookies.Delete(key);
		}
    }
}
