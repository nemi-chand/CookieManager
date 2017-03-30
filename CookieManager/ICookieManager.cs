using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookieManager
{
    public interface ICookieManager
    {        
        string Get(string key);

        T Get<T>(string key);

        T GetOrSet<T>(string key, Func<T> acquirer, int? expireTime = null);

        void Set(string key, object value, int? expireTime = null);

        bool Contains(string key);

        void Remove(string key);

        bool IsHttpOnly { get; set; }
    }
}
