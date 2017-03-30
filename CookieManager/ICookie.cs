using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookieManager
{

    /// <summary>
    /// ICookie is absstraction layer on top of ASP.Net Core Cookie API
    /// </summary>
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
        /// Sets the cookie 
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value of the key</param>
        /// <param name="expireTime">cookie expire time</param>
        void Set(string key, string value, int? expireTime);

        /// <summary>
        /// Contain the key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>bool</returns>
        bool Contains(string key);

        /// <summary>
        /// delete the key from cookie 
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);

        /// <summary>
        /// 
        /// </summary>
        bool IsHttpOnly { get; }


    }
}
