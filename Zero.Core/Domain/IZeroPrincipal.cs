using System.Security.Principal;

namespace Zero.Core.Domain
{
	public interface IZeroPrincipal : IPrincipal
	{
		string Url { get; }
		string Login { get; }
	}
}
