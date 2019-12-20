using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Security.Configuration;
using Security.Networking;
using System;
using System.Linq;
using System.Net.Http;
using Unity;

namespace Security
{
	public static class Module
	{
		public static void Register(IUnityContainer container)
		{
			IServiceCollection services = new ServiceCollection();
			IConfiguration config = new ConfigurationBuilder()
				.AddJsonFile("remote-hosts.json")
				.Build();

			services.AddTransient(c => container.Resolve<HostOptions>());
			services.AddTransient<SignMessageHandler>();
			
			var remoteHosts = config.Get<RemoteHostsConfiguration>();
			container.RegisterInstance(remoteHosts);

			foreach (var host in remoteHosts.Hosts.Where(h => !string.IsNullOrEmpty(h.Url)))
			{
				services.AddHttpClient(host.Name, c =>
				{
					c.BaseAddress = new Uri(host.Url);
				}).AddHttpMessageHandler<SignMessageHandler>();
			}

			var serviceProvider = services.BuildServiceProvider();
			container.RegisterFactory<IHttpClientFactory>(c => serviceProvider.GetRequiredService<IHttpClientFactory>());
		}
	}
}
