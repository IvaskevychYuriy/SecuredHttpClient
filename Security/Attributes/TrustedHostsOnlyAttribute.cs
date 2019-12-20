using System;

namespace Security.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class TrustedHostsOnlyAttribute : Attribute
	{
	}
}
