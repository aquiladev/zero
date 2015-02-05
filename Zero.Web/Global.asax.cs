using Newtonsoft.Json;
using System;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Zero.Core.Domain;
using Zero.Web.Models;

namespace Zero.Web
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801
	public class MvcApplication : HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			WebApiConfig.Register(GlobalConfiguration.Configuration);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);

			ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
		}

		protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
		{
			HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

			if (authCookie != null)
			{
				FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
				var model = JsonConvert.DeserializeObject<ZeroPrincipalModel>(authTicket.UserData);
				HttpContext.Current.User = new ZeroPrincipal(model.Url, model.Login);
			}
		}
	}
}