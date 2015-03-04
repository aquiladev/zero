using System.Web.Http;
using Newtonsoft.Json;
using Zero.Core.Domain;
using Zero.Core.Security;

namespace Zero.Api.Controllers
{
	public class AuthController : ApiController
	{
		public string Get(string url, string login, string password, string domain = "")
		{
			var principal = new ZPrincipal
			{
				Url = url,
				Login = login,
				Domain = domain,
				Password = password
			};

			var value = JsonConvert.SerializeObject(principal);
			var result = Rijndael.Encrypt(value);
			return result;
		}
	}
}
