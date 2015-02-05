using System.Security.Principal;

namespace Zero.Core.Domain
{
	public class ZeroPrincipal : IZeroPrincipal
	{
		public IIdentity Identity { get; private set; }
		public bool IsInRole(string role) { return false; }

		public ZeroPrincipal(string url, string login)
		{
			Url = url;
			Login = login;
			Identity = new GenericIdentity(login);
		}

		public string Url { get; private set; }
		public string Login { get; private set; }
	}
}
