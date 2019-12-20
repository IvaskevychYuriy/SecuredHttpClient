using System.Collections.Generic;
using System.Linq;

namespace Security.Configuration
{
	public class RemoteHostsConfiguration
	{
		public IEnumerable<RemoteHost> Hosts { get; set; }

		public RemoteHost Find(string name)
			=> Hosts?.FirstOrDefault(h => h.Name == name);
	}
}
