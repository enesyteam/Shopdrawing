using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project
{
	internal class AssemblyInformation
	{
		public System.Reflection.Assembly Assembly
		{
			get;
			private set;
		}

		public bool IsPlatformAssembly
		{
			get;
			set;
		}

		public string ShadowCopyLocation
		{
			get;
			set;
		}

		public bool WasShadowCopied
		{
			get;
			set;
		}

		public AssemblyInformation(System.Reflection.Assembly assembly)
		{
			this.Assembly = assembly;
			this.WasShadowCopied = false;
			this.ShadowCopyLocation = string.Empty;
			this.IsPlatformAssembly = false;
		}
	}
}