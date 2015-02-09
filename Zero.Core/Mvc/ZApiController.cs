using System;
using System.Net;
using System.Web.Http;
using Microsoft.TeamFoundation.Client;
using Zero.Core.Domain;

namespace Zero.Core.Mvc
{
	[ZAuthorize]
	public class ZApiController : ApiController
	{
		public ZPrincipal Principal { get; set; }

		protected TfsConfigurationServer GetServer()
		{
			ICredentials credentials = new NetworkCredential(Principal.Login, Principal.Password, Principal.Domain);

			return new TfsConfigurationServer(new Uri(Principal.Url), credentials);
		}
	}
}