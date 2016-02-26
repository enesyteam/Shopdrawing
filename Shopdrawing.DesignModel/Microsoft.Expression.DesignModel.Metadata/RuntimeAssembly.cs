using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal sealed class RuntimeAssembly : ReferenceAssembly, IReflectionAssembly
	{
		public Microsoft.Expression.DesignModel.Metadata.AssemblySource AssemblySource
		{
			get;
			private set;
		}

		public bool IsPlatformAssembly
		{
			get
			{
				if (this.AssemblySource == Microsoft.Expression.DesignModel.Metadata.AssemblySource.Platform)
				{
					return true;
				}
				return this.AssemblySource == Microsoft.Expression.DesignModel.Metadata.AssemblySource.PlatformExtension;
			}
		}

		public Assembly ReflectionAssembly
		{
			get
			{
				return JustDecompileGenerated_get_ReflectionAssembly();
			}
			set
			{
				JustDecompileGenerated_set_ReflectionAssembly(value);
			}
		}

		public Assembly JustDecompileGenerated_get_ReflectionAssembly()
		{
			return base.InternalAssembly;
		}

		public void JustDecompileGenerated_set_ReflectionAssembly(Assembly value)
		{
			base.InternalAssembly = value;
		}

		public RuntimeAssembly(string name) : this(name, Microsoft.Expression.DesignModel.Metadata.AssemblySource.Unknown)
		{
		}

		public RuntimeAssembly(string name, Microsoft.Expression.DesignModel.Metadata.AssemblySource assemblySource)
		{
			base.Name = name;
			this.AssemblySource = assemblySource;
			base.IsResolvedImplicitAssembly = false;
		}

		public RuntimeAssembly(Assembly assembly, Microsoft.Expression.DesignModel.Metadata.AssemblySource assemblySource, bool isResolvedImplicitAssembly) : this(AssemblyHelper.GetAssemblyName(assembly).Name, assemblySource)
		{
			base.InternalAssembly = assembly;
			base.IsResolvedImplicitAssembly = isResolvedImplicitAssembly;
		}
	}
}