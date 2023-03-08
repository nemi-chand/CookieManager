using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace CookieManager
{
	/// <summary>
	/// Implementation of <see cref="ICookie" /> 
	/// </summary>
	public class HttpCookie : ICookie
    {
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IDataProtector _dataProtector;
		private static readonly string Purpose = "CookieManager.Token.v1";
		private readonly CookieManagerOptions _cookieManagerOptions;
		private readonly ChunkingHttpCookie _chunkingHttpCookie;



        /// <summary>
        /// External depedenacy of <see cref="IHttpContextAccessor" /> 
        /// </summary>
        /// <param name="httpContextAccessor">IHttpAccessor</param>
        /// <param name="dataProtectionProvider">data protection provider</param>
        /// <param name="optionAccessor">cookie manager option accessor</param>
        public HttpCookie(IHttpContextAccessor httpContextAccessor, 
			IDataProtectionProvider dataProtectionProvider,
			IOptions<CookieManagerOptions> optionAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
			_dataProtector = dataProtectionProvider.CreateProtector(Purpose);
			_cookieManagerOptions = optionAccessor.Value;
			_chunkingHttpCookie = new ChunkingHttpCookie(optionAccessor);
		}

        public ICollection<string> Keys
		{
			get
			{
				if(_httpContextAccessor.HttpContext == null)
				{
					throw new ArgumentNullException(nameof(_httpContextAccessor.HttpContext));
				}

				return _httpContextAccessor.HttpContext.Request.Cookies.Keys;
			}
		}

        public bool Contains(string key)
        {            
			if(_httpContextAccessor.HttpContext == null)
			{
				throw new ArgumentNullException(nameof(_httpContextAccessor.HttpContext));
			}

			if(key == null)
			{
				throw new ArgumentNullException(nameof(key));
			}

            return _httpContextAccessor.HttpContext.Request.Cookies.ContainsKey(key);
        }

		/// <summary>
		/// Get the key value
		/// </summary>
		/// <param name="key">Key</param>
		/// <returns>value</returns>
        public string Get(string key)
        {
			if (_httpContextAccessor.HttpContext == null)
			{
				throw new ArgumentNullException(nameof(_httpContextAccessor.HttpContext));
			}

			if (key == null)
			{
				throw new ArgumentNullException(nameof(key));
			}

			if (Contains(key))
			{
				var encodedValue = _chunkingHttpCookie.GetRequestCookie(_httpContextAccessor.HttpContext, key);
				var protectedData = string.Empty;
				//allow encryption is optional
				//may change the allow encryption to avoid this first check if cookie value is able to decode than unprotect tha data
				if(Base64TextEncoder.TryDecode(encodedValue,out protectedData))
				{
					string unprotectedData;
					if(_dataProtector.TryUnprotect(protectedData, out unprotectedData))
					{
						return unprotectedData;
					}
				}
				return encodedValue;
			}

			return string.Empty;
        }

		/// <summary>
		/// Remove the cookie key
		/// </summary>
		/// <param name="key">Key</param>
        public void Remove(string key)
        {
			if (_httpContextAccessor.HttpContext == null)
			{
				throw new ArgumentNullException(nameof(_httpContextAccessor.HttpContext));
			}

			if (key == null)
			{
				throw new ArgumentNullException(nameof(key));
			}

			_chunkingHttpCookie.RemoveCookie(_httpContextAccessor.HttpContext, key);
        }

		/// <summary>
		/// set the cookie
		/// </summary>
		/// <param name="key">unique key</param>
		/// <param name="value">value to store</param>
		/// <param name="expireTime">Expire time (default time is 10 millisencond)</param>
        public void Set(string key, string value, int? expireTime)
        {
			//validate input
			if (_httpContextAccessor.HttpContext == null)
			{
				throw new ArgumentNullException(nameof(_httpContextAccessor.HttpContext));
			}

			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentException(nameof(key));
			}

			Set(key, value, null, expireTime);
        }

		/// <summary>
		/// set the cookie 
		/// </summary>
		/// <param name="key">key</param>
		/// <param name="value">value of the specified key</param>
		/// <param name="option">CookieOption</param>
		public void Set(string key, string value, CookieOptions option)
		{
			if(_httpContextAccessor.HttpContext == null)
			{
				throw new ArgumentNullException(nameof(_httpContextAccessor.HttpContext));
			}

			if(key == null)
			{
				throw new ArgumentNullException(nameof(key));
			}

			if(option == null)
			{
				throw new ArgumentNullException(nameof(option));
			}

			Set(key, value, option, null);
		}

		private void Set(string key, string value, CookieOptions option, int? expireTime)
		{
			if(option == null)
			{
				option = new CookieOptions();

				if (expireTime.HasValue)
					option.Expires = DateTime.Now.AddMinutes(expireTime.Value);
				else
					option.Expires = DateTime.Now.AddDays(_cookieManagerOptions.DefaultExpireTimeInDays);
			}

			//check for encryption 
			if(_cookieManagerOptions.AllowEncryption)
			{
				string protecetedData = _dataProtector.Protect(value);
				var encodedValue = Base64TextEncoder.Encode(protecetedData);
				_chunkingHttpCookie.AppendResponseCookie(_httpContextAccessor.HttpContext, key, encodedValue, option);
			}
			else
			{
				//just append the cookie 
				_chunkingHttpCookie.AppendResponseCookie(_httpContextAccessor.HttpContext, key, value, option);
			}
			
		}
	}
}
