using System;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace Microsoft.Expression.Extensibility.Designer.Platform
{
	public interface IPlatformCapabilitySettings
	{
		IDictionary<string, object> Capabilities
		{
			get;
		}

		System.Version MaxFrameworkVersion
		{
			get;
		}

		FrameworkName RequiredTargetFramework
		{
			get;
		}

		int Version
		{
			get;
		}
	}
}