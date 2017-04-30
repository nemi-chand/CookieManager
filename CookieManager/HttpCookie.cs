using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookieManager
{
	/// <summary>
	/// Implementation of <see cref="ICookie" /> 
	/// </summary>
	public class HttpCookie : ICookie
    {
        private readonly IHttpContextAccessor _httpContext;
		private readonly IDataProtector _dataProtector;
		private static readonly string Purpose = "CookieManager.Token.v1";

		/// <summary>
		/// External depedenacy of <see cref="IHttpContextAccessor" /> 
		/// </summary>
		/// <param name="httpContext"></param>
		public HttpCookie(IHttpContextAccessor httpContext, IDataProtectionProvider dataProtectionProvider)
        {
            this._httpContext = httpContext;
			_dataProtector = dataProtectionProvider.CreateProtector(Purpose);
		}

        public ICollection<string> Keys { get { return _httpContext.HttpContext.Request.Cookies.Keys; } }

        public bool Contains(string key)
        {            
            return _httpContext.HttpContext.Request.Cookies.ContainsKey(key);
        }

		/// <summary>
		/// Get the key value
		/// </summary>
		/// <param name="key">Key</param>
		/// <returns>value</returns>
        public string Get(string key)
        {
			if (Contains(key))
			{
				var encodedValue = _httpContext.HttpContext.Request.Cookies[key];
				var protectedData = Base64TextEncoder.Decode(encodedValue);
				var unprotectedData = _dataProtector.Unprotect(protectedData);
				return unprotectedData;
			}

			return string.Empty;
        }

		/// <summary>
		/// Remove the cookie key
		/// </summary>
		/// <param name="key">Key</param>
        public void Remove(string key)
        {
            _httpContext.HttpContext.Response.Cookies.Delete(key);
        }

		/// <summary>
		/// set the cookie
		/// </summary>
		/// <param name="key">unique key</param>
		/// <param name="value">value to store</param>
		/// <param name="expireTime">Expire time (default time is 10 millisencond)</param>
        public void Set(string key, string value, int? expireTime)
        {
			//validate input TODO
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
					option.Expires = DateTime.Now.AddSeconds(10);
			}

			
			string protecetedData = _dataProtector.Protect(value);
			var encodedValue = Base64TextEncoder.Encode(protecetedData);

			_httpContext.HttpContext.Response.Cookies.Append(key, encodedValue, option);
		}
	}
}
