using Microsoft.TeamFoundation.Client;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Zero.Web.Models;

namespace Zero.Web.Controllers
{
	[Authorize]
	public class AccountController : Controller
	{
		[AllowAnonymous]
		public ActionResult Login(string returnUrl)
		{
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult Login(LoginViewModel model, string returnUrl)
		{
			if (true) //if (ModelState.IsValid)
			{
				ICredentials credentials = new NetworkCredential(model.Login, model.Password, model.Domain);

				using (var server = new TfsConfigurationServer(new Uri(model.Url), credentials))
				{
					server.EnsureAuthenticated();
					server.Authenticate();

					if (server.HasAuthenticated)
					{
						SignIn(model);
						return RedirectToAction("Index", "Home");
					}
				}
			}
			return View(model);
		}

		public ActionResult Logout()
		{
			FormsAuthentication.SignOut();
			Session.Abandon();

			// Clear authentication cookie
			HttpCookie rFormsCookie = new HttpCookie(FormsAuthentication.FormsCookieName, "")
			{
				Expires = DateTime.Now.AddYears(-1)
			};
			Response.Cookies.Add(rFormsCookie);

			// Clear session cookie 
			HttpCookie rSessionCookie = new HttpCookie("ASP.NET_SessionId", "")
			{
				Expires = DateTime.Now.AddYears(-1)
			};
			Response.Cookies.Add(rSessionCookie);

			return RedirectToAction("Index", "Home");
		}

		private void SignIn(LoginViewModel model)
		{
			string userData = JsonConvert.SerializeObject(
				new ZeroPrincipalModel
				{
					Login = model.Login,
					Url = model.Url
				});

			var authTicket = new FormsAuthenticationTicket(1, model.Login,
				DateTime.Now, DateTime.Now.AddMinutes(30),
				false, userData);

			string encTicket = FormsAuthentication.Encrypt(authTicket);
			var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
			Response.Cookies.Add(cookie);
		}
	}
}
