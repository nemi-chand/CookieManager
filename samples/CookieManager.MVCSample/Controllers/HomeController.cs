using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace CookieManager.MVCSample.Controllers
{
	public class HomeController : Controller
    {
		private readonly ICookieManager _cookieManager;
		private readonly ICookie _cookie;

		public HomeController(ICookieManager cookieManager,ICookie cookie)
		{
			this._cookieManager = cookieManager;
			this._cookie = cookie;
		}

        public IActionResult Index()
        {

			string keystr = _cookie.Get("Key");

			_cookie.Set("Key", "value here", new CookieOptions() { HttpOnly = true, Expires = DateTime.Now.AddDays(1) });

			// get <T> object from cookie
			//MyCookie myCookie = _cookieManager.Get<MyCookie>("Key");
			MyCookie objFromCookie = _cookieManager.Get<MyCookie>("Key1");
			MyCookie cooObj= new MyCookie()
			{
				Id = Guid.NewGuid().ToString(),
				Indentifier = "valueasgrsdgdf66514sdfgsd51d65s31g5dsg1rs5dg",
				Date = DateTime.Now
			};
			_cookieManager.Set("Key1", cooObj, 100000);


			//Get or set <T>
			//Cookieoption example
			MyCookie myCook = _cookieManager.GetOrSet<MyCookie>("Key2", () =>
			{
				   //write fucntion to store  output in cookie
				   return new MyCookie()
				   {
					   Id = Guid.NewGuid().ToString(),
					   Indentifier = "valueasgrsdgdf66514sdfgsd51d65s31g5dsg1rs5dg",
					   Date = DateTime.Now
				   };

			}, new CookieOptions() { HttpOnly = true, Expires = DateTime.Now.AddDays(1) });

			// expire time example
			//MyCookie myCookWithExpireTime = _cookieManager.GetOrSet<MyCookie>("Key", () =>
			//{
			//	//write fucntion to store  output in cookie
			//	return new MyCookie()
			//	{
			//		Id = Guid.NewGuid().ToString(),
			//		Indentifier = "value here",
			//		Date = DateTime.Now
			//	};

			//}, 60);




			return View();
        }

		
		public class MyCookie
		{
			public string Id { get; set; }

			public DateTime Date { get; set; }

			public string Indentifier { get; set; }
		}

		
	}
}
