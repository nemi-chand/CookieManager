using Newtonsoft.Json;
using System;

namespace CookieManager
{
    public class DefaultCookieManager : ICookieManager
    {
        private ICookie _cookie;

        public bool IsHttpOnly { get ; set ; }

        public DefaultCookieManager(ICookie cookie)
        {
            this._cookie = cookie;
        }


        public bool Contains(string key)
        {
            return _cookie.Contains(key);
        }

        public T GetOrSet<T>(string key, Func<T> acquirer, int? expireTime = default(int?))
        {
            if(_cookie.Contains(key))
            {
                //get the existing value
                GetExisting<T>(key);
            }
            else
            {
                var value = acquirer();
                this.Set(key, value, expireTime);

                return value;
            }

            return GetExisting<T>(key);
        }

        private T GetExisting<T>(string key)
        {
            var value = _cookie.Get(key);

            if (string.IsNullOrEmpty(value))
                return default(T);

            return JsonConvert.DeserializeObject<T>(_cookie.Get(key));
        }

        public void Remove(string key)
        {
            _cookie.Remove(key);
        }

        public void Set(string key, object value, int? expireTime = default(int?))
        {
            _cookie.Set(key, JsonConvert.SerializeObject(value), expireTime);
        }

        public string Get(string key)
        {
            return _cookie.Get(key);
        }

        public T Get<T>(string key)
        {
            return GetExisting<T>(key);
        }
    }
}
