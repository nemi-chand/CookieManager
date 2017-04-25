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
        private IHttpContextAccessor _httpContext;

		/// <summary>
		/// External depedenacy of <see cref="IHttpContextAccessor" /> 
		/// </summary>
		/// <param name="httpContext"></param>
		public HttpCookie(IHttpContextAccessor httpContext)
        {
            this._httpContext = httpContext;
        }

        public ICollection<string> Keys { get { return _httpContext.HttpContext.Request.Cookies.Keys; } }

        public bool IsHttpOnly { get; set ; }

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
            return _httpContext.HttpContext.Request.Cookies[key];
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
            CookieOptions option = new CookieOptions();
            option.HttpOnly = IsHttpOnly;

            if (expireTime.HasValue)
                option.Expires = DateTime.Now.AddMinutes(expireTime.Value);
            else
                option.Expires = DateTime.Now.AddMilliseconds(10);


            _httpContext.HttpContext.Response.Cookies.Append(key, value, option);
        }

		/// <summary>
		/// set the cookie 
		/// </summary>
		/// <param name="key">key</param>
		/// <param name="value">value of the specified key</param>
		/// <param name="option">CookieOption</param>
		public void Set(string key, string value, CookieOptions option)
		{			
			option.HttpOnly = IsHttpOnly;

			_httpContext.HttpContext.Response.Cookies.Append(key, value, option);
		}
	}
}
