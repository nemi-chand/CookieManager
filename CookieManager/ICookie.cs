using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookieManager
{
    public interface ICookie
    {

        /// <summary>
        /// 
        /// </summary>
        ICollection<string> Keys { get; }

        /// <summary>
        /// Gets a cookie item associated with key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string Get(string key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireTime"></param>
        void Set(string key, string value, int? expireTime);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Contains(string key);

        void Remove(string key);

        bool IsHttpOnly { get; }


    }
}
