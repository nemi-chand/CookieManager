using Newtonsoft.Json;
using System;
using Microsoft.AspNetCore.Http;

namespace CookieManager
{
    /// <summary>
    /// Implementation of <see cref="ICookieManager" /> 
    /// </summary>
    public class DefaultCookieManager : ICookieManager
    {
        private readonly ICookie _cookie;

        public DefaultCookieManager(ICookie cookie)
        {
            this._cookie = cookie;
        }


        public bool Contains(string key)
        {
            return _cookie.Contains(key);
        }

        /// <summary>
        /// Get the value or set the value if specified key is expire
        /// </summary>
        /// <typeparam name="T">TSource</typeparam>
        /// <param name="key">Key</param>
        /// <param name="acquirer">action to execute</param>
        /// <param name="expireTime">Expire time</param>
        /// <returns>TSource object</returns>
        public T GetOrSet<T>(string key, Func<T> acquirer, int? expireTime = default)
        {
            try
            {
                return GetExisting<T>(key);
            }
            catch { }

            var value = acquirer();
            Set(key, value, expireTime);

            return value;
        }

        private T GetExisting<T>(string key)
        {
            var value = _cookie.Get(key);

            if (string.IsNullOrEmpty(value))
                return default(T);

            return JsonConvert.DeserializeObject<T>(value);
        }

        /// <summary>
        /// remove the key
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            _cookie.Remove(key);
        }

        /// <summary>
        /// set the value 
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="expireTime"></param>
        public void Set(string key, object value, int? expireTime = default(int?))
        {
            _cookie.Set(key, JsonConvert.SerializeObject(value), expireTime);
        }


        /// <summary>
        /// get the value of specified key
        /// </summary>
        /// <typeparam name="T">T object</typeparam>
        /// <param name="key">Key</param>
        /// <returns>T object</returns>
        public T Get<T>(string key)
        {
            return GetExisting<T>(key);
        }

        public void Set(string key, object value, CookieOptions option)
        {
            _cookie.Set(key, JsonConvert.SerializeObject(value), option);
        }

        public T GetOrSet<T>(string key, Func<T> acquirer, CookieOptions option)
        {
            try
            {
                return GetExisting<T>(key);
            }
            catch { }

            var value = acquirer();
            Set(key, value, option);

            return value;
        }
    }
}
