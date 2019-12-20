using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Security.Infrastructure
{
	internal static class HttpRequestHelper
	{
		public static async Task<IEnumerable<byte>> GetPayloadBytesAsync(HttpRequestMessage request)
		{
			IEnumerable<byte> data = Encoding.UTF8.GetBytes(request.RequestUri.AbsolutePath);
			if (request.Content != null)
			{
				var content = await request.Content.ReadAsByteArrayAsync();
				data = data.Concat(content);
			}

			return data;
		}

		public static async Task<IEnumerable<byte>> GetPayloadBytesAsync(HttpRequest request)
		{
			IEnumerable<byte> data = Encoding.UTF8.GetBytes(request.Path.Value);
			if (request.ContentLength > 0)
			{
				var ms = new MemoryStream();
				await request.Body.CopyToAsync(ms);
				var content = ms.ToArray();
				data = data.Concat(content);
			}

			return data;
		}
	}
}
