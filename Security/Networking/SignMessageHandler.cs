using Security.Configuration;
using Security.Constants;
using Security.Infrastructure;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Security.Networking
{
	internal class SignMessageHandler : DelegatingHandler
	{
		private const int SaltBytesLength = 512 / 8;

		private readonly HostOptions _options;

		public SignMessageHandler(HostOptions options)
		{
			_options = options;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var data = await HttpRequestHelper.GetPayloadBytesAsync(request);
			var salt = CryptographyHelper.GenerateSalt(SaltBytesLength);

			var hash = CryptographyHelper.ComputeHash(data, salt);
			var signature = CryptographyHelper.Encrypt(hash, _options.PrivateKey);

			request.Headers.Add(HeaderNames.Host, _options.Name);
			request.Headers.Add(HeaderNames.Salt, Convert.ToBase64String(salt));
			request.Headers.Add(HeaderNames.Signature, Convert.ToBase64String(signature));

			return await base.SendAsync(request, cancellationToken);
		}
	}
}
