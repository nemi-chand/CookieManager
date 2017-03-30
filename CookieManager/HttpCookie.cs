using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookieManager
{
    public class HttpCookie : ICookie
    {
        private IHttpContextAccessor _httpContext;

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

        public string Get(string key)
        {
            return _httpContext.HttpContext.Request.Cookies[key];
        }

        public void Remove(string key)
        {
            _httpContext.HttpContext.Response.Cookies.Delete(key);
        }

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
    }
}
