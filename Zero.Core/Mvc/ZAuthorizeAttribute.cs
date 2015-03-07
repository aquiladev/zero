using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Newtonsoft.Json;
using Zero.Core.Domain;
using Zero.Core.Security;

namespace Zero.Core.Mvc
{
	public class ZAuthorizeAttribute : AuthorizationFilterAttribute
	{
		private const string RequestNotAuthorized = "Unauthorized request";

		public override void OnAuthorization(HttpActionContext actionContext)
		{
			if (actionContext == null)
			{
				throw new ArgumentNullException("actionContext");
			}

			ZPrincipal principal = GetPrincipal(actionContext);

			if (IsAuthorized(actionContext, principal))
			{
				SetPrincipal(actionContext, principal);
			}
			else
			{
				HandleUnauthorizedRequest(actionContext);
			}
		}

		protected virtual bool IsAuthorized(HttpActionContext actionContext, ZPrincipal principal)
		{
			if (actionContext == null)
			{
				throw new ArgumentNullException("actionContext");
			}
			
			return principal != null;
		}

		protected virtual void HandleUnauthorizedRequest(HttpActionContext actionContext)
		{
			if (actionContext == null)
			{
				throw new ArgumentNullException("actionContext");
			}

			actionContext.Response = actionContext.ControllerContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, RequestNotAuthorized);
		}

		private ZPrincipal GetPrincipal(HttpActionContext actionContext)
		{
			try
			{
				string authHeader = actionContext.Request.Headers.GetValues(AuthConfiguration.AuthHeader).First();
				var result = Rijndael.Decrypt(authHeader, Rijndael.GetRandomKeyText());
				return JsonConvert.DeserializeObject<ZPrincipal>(result);
			}
			catch (Exception)
			{
				return null;
			}
		}

		private void SetPrincipal(HttpActionContext actionContext, ZPrincipal principal)
		{
			var controller = actionContext.ControllerContext.Controller as ZApiController;
			if (controller == null)
			{
				return;
			}
			controller.Principal = principal;
		}
	}
}
