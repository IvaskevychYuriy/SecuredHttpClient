using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Security.Attributes;
using Security.Configuration;
using Security.Constants;
using Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Security.Filters
{
	public class TrustedHostsFilter : IAsyncAuthorizationFilter
	{
		private readonly ILogger _logger;
		private readonly RemoteHostsConfiguration _remoteHostsConfiguration;

		public TrustedHostsFilter(
			ILogger<TrustedHostsFilter> logger, 
			RemoteHostsConfiguration remoteHostsConfiguration)
		{
			_logger = logger;
			_remoteHostsConfiguration = remoteHostsConfiguration;
		}

		public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
		{
			if (!ShouldCheckSecurity(context.ActionDescriptor as ControllerActionDescriptor))
			{
				return;
			}

			string path = context.HttpContext.Request.Path.Value;
			try
			{
				bool isCallTrusted = await IsCallTrusted(context);
				if (!isCallTrusted)
				{
					_logger.LogWarning("Detected untrusted remote request for {Path}", path);
					context.Result = new UnauthorizedResult();
				}
			}
			catch (Exception e)
			{
				_logger.LogWarning(e, "Checking if the remote request is trusted failed for {Path}", path);
				context.Result = new UnauthorizedResult();
			}
		}

		private bool ShouldCheckSecurity(ControllerActionDescriptor descriptor)
			=> descriptor.ControllerTypeInfo.CustomAttributes.Any(a => a.AttributeType == typeof(TrustedHostsOnlyAttribute))
			|| descriptor.MethodInfo.CustomAttributes.Any(a => a.AttributeType == typeof(TrustedHostsOnlyAttribute));

		private async Task<bool> IsCallTrusted(AuthorizationFilterContext context)
		{
			var headers = context.HttpContext.Request.Headers;
			string hostName = headers[HeaderNames.Host];
			var salt = Convert.FromBase64String(headers[HeaderNames.Salt]);
			var signature = Convert.FromBase64String(headers[HeaderNames.Signature]);

			string hostPublicKey = _remoteHostsConfiguration.Find(hostName).PublicKey;
			var decryptedHash = CryptographyHelper.Decrypt(signature, hostPublicKey);

			var data = await GetData(context);
			var hash = CryptographyHelper.ComputeHash(data, salt);
			return CryptographyHelper.AreHashesEqual(decryptedHash, hash);
		}

		private static async Task<IEnumerable<byte>> GetData(AuthorizationFilterContext context)
		{
			var request = context.HttpContext.Request;
			request.EnableRewind();
			request.Body.Seek(0, SeekOrigin.Begin);

			var data = await HttpRequestHelper.GetPayloadBytesAsync(request);

			request.Body.Position = 0;
			return data;
		}
	}
}
