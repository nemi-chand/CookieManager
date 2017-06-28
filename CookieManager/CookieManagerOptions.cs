using System;
using System.Collections.Generic;
using System.Text;

namespace CookieManager
{
    public class CookieManagerOptions
    {
		/// <summary>
		/// allow cookie to encrypt by default it allow encryption
		/// </summary>
		public bool AllowEncryption { get; set; } = true;

		public int DefaultExpireTimeInDays { get; set; } = 1;

		/// <summary>
		/// The maximum size of cookie to send back to the client. If a cookie exceeds this size it will be broken down into multiple
		/// cookies. Set this value to null to disable this behavior. The default is 4090 characters, which is supported by all
		/// common browsers.
		///
		/// Note that browsers may also have limits on the total size of all cookies per domain, and on the number of cookies per domain.
		/// </summary>
		public int? ChunkSize { get; set; } = 4050;

		/// <summary>
		/// Throw if not all chunks of a cookie are available on a request for re-assembly.
		/// </summary>
		public bool ThrowForPartialCookies { get; set; } = true;
	}
}
