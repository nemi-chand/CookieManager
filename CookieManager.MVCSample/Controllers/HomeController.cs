using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace CookieManager.MVCSample.Controllers
{
    public class HomeController : Controller
    {
		private readonly ICookieManager _cookieManager;

		public HomeController(ICookieManager cookieManager)
		{
			this._cookieManager = cookieManager;
		}

        public IActionResult Index()
        {

			// get <T> object from cookie
			MyCookie myCookie = _cookieManager.Get<MyCookie>("Key");


			//Get or set <T>
			MyCookie myCook =  _cookieManager.GetOrSet<MyCookie>("Key", () => 
									{
										//write fucntion to store  output in cookie
										return new MyCookie()
										{
											 Id = Guid.NewGuid().ToString(),
											 Indentifier = "value here",
											 Date = DateTime.Now
										};

									},new CookieOptions() { HttpOnly = true,Expires = DateTime.Now.AddDays(1) });
			

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
