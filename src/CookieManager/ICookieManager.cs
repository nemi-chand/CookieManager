﻿using Microsoft.AspNetCore.Http;
using System;

namespace CookieManager
{
    /// <summary>
    /// Cookie Manager is abstraction layer on top of <see cref="ICookie" />
    /// </summary>
    public interface ICookieManager
    {   
        /// <summary>
        /// Gets a cookie associated with key 
        /// </summary>
        /// <typeparam name="T">TSource</typeparam>
        /// <param name="key">Key</param>
        /// <returns>TSource object</returns>
        T Get<T>(string key);

        /// <summary>
        /// Gets and Sets the cookie object
        /// </summary>
        /// <typeparam name="T">TSource</typeparam>
        /// <param name="key">Key</param>
        /// <param name="acquirer">function</param>
        /// <param name="expireTime">cookie expire time</param>
        /// <returns></returns>
        T GetOrSet<T>(string key, Func<T> acquirer, int? expireTime = null);

		/// <summary>
		/// Gets and Sets the cookie object
		/// </summary>
		/// <typeparam name="T">TSource</typeparam>
		/// <param name="key">Key</param>
		/// <param name="acquirer">if cookie is expired than it execute to acquire function and set the cookie</param>
		/// <param name="option">cookie option</param>
		/// <returns></returns>
		T GetOrSet<T>(string key, Func<T> acquirer, CookieOptions option);

        /// <summary>
        /// Sets the cookie value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">value of the key</param>
        /// <param name="expireTime">cookie expire time</param>
        void Set(string key, object value, int? expireTime = null);

		/// <summary>
		/// Sets the cookie value
		/// </summary>
		/// <param name="key">Key</param>
		/// <param name="value">value of the key</param>
		/// <param name="expireTime">cookie expire time</param>
		void Set(string key, object value, CookieOptions option);

		/// <summary>
		/// Contains the key
		/// </summary>
		/// <param name="key">Key</param>
		/// <returns>bool</returns>
		bool Contains(string key);

        /// <summary>
        /// remove the Key
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);
    }
}
