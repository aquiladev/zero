using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.OData.Extensions;
using Zero.Core.Http;

namespace Zero.Api
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "{controller}/{action}/{id}",
				constraints: null,
				handler: new ContentMd5Handler
				{
					InnerHandler = new HttpControllerDispatcher(config)
				},
				defaults: new { id = RouteParameter.Optional }
			);

			config.AddODataQueryFilter();
		}
	}
}
