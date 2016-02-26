using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Adds;

namespace Microsoft.MetadataReader
{
	[DebuggerDisplay("AssemblyRef: {m_name}")]
	internal class AssemblyRef : AssemblyProxy
	{
		private readonly AssemblyName m_name;

		public AssemblyRef(AssemblyName name, ITypeUniverse universe) : base(universe)
		{
			this.m_name = name;
		}

		public override AssemblyName GetName()
		{
			return this.m_name;
		}

		protected override Assembly GetResolvedAssemblyWorker()
		{
			return base.TypeUniverse.ResolveAssembly(this.m_name);
		}
	}
}