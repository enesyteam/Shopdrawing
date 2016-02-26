using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.Reflection.Adds
{
	[DebuggerDisplay("Resolve for {Name}")]
	internal class ResolveAssemblyNameEventArgs : EventArgs
	{
		public AssemblyName Name
		{
			get;
			internal set;
		}

		public Assembly Target
		{
			get;
			set;
		}

		public ResolveAssemblyNameEventArgs(AssemblyName name)
		{
			this.Name = name;
		}
	}
}