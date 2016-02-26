using System;

namespace Microsoft.VisualStudio.Silverlight
{
	internal class AssemblyNameVersion
	{
		public string Name;

		public System.Version Version;

		public byte[] PublicKeyToken;

		public AssemblyNameVersion(string name, System.Version version, byte[] publicKeyToken)
		{
			this.Name = name;
			this.Version = version;
			this.PublicKeyToken = publicKeyToken;
		}
	}
}