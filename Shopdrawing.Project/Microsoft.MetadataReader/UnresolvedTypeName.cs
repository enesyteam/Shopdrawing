using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Reflection.Adds;

namespace Microsoft.MetadataReader
{
	[DebuggerDisplay("{m_TypeName},{m_AssemblyName}")]
	internal class UnresolvedTypeName
	{
		private readonly string m_TypeName;

		private readonly AssemblyName m_AssemblyName;

		public string TypeName
		{
			get
			{
				return this.m_TypeName;
			}
		}

		public UnresolvedTypeName(string typeName, AssemblyName assemblyName)
		{
			this.m_TypeName = typeName;
			this.m_AssemblyName = assemblyName;
		}

		public Type ConvertToType(ITypeUniverse universe, Module moduleContext)
		{
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			object[] mTypeName = new object[] { this.m_TypeName, this.m_AssemblyName.FullName };
			string str = string.Format(invariantCulture, "{0},{1}", mTypeName);
			return System.Reflection.Adds.TypeNameParser.ParseTypeName(universe, moduleContext, str);
		}
	}
}