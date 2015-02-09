using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Zero.Core.Http
{
	public class ContentMd5Handler : DelegatingHandler
	{
		protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

			if (response.IsSuccessStatusCode && response.Content != null)
			{
				using (MD5 md5 = MD5.Create())
				{
					var content = await response.Content.ReadAsByteArrayAsync();
					byte[] hash = md5.ComputeHash(content);
					response.Content.Headers.ContentMD5 = hash;
				}
			}
			return response;
		}
	}
}
