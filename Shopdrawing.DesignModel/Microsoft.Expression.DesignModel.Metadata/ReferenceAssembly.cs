using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal class ReferenceAssembly : IAssembly, IAssemblyId
	{
		public string FullName
		{
			get
			{
				if (this.InternalAssembly == null)
				{
					return null;
				}
				return this.InternalAssembly.FullName;
			}
		}

		public bool GlobalAssemblyCache
		{
			get
			{
				if (this.InternalAssembly == null)
				{
					return false;
				}
				return this.InternalAssembly.GlobalAssemblyCache;
			}
		}

		protected Assembly InternalAssembly
		{
			get;
			set;
		}

		public bool IsDynamic
		{
			get
			{
				if (this.InternalAssembly == null)
				{
					return false;
				}
				return this.InternalAssembly.IsDynamic;
			}
		}

		public bool IsLoaded
		{
			get
			{
				return this.InternalAssembly != null;
			}
		}

		public bool IsResolvedImplicitAssembly
		{
			get
			{
				return JustDecompileGenerated_get_IsResolvedImplicitAssembly();
			}
			set
			{
				JustDecompileGenerated_set_IsResolvedImplicitAssembly(value);
			}
		}

		private bool JustDecompileGenerated_IsResolvedImplicitAssembly_k__BackingField;

		public bool JustDecompileGenerated_get_IsResolvedImplicitAssembly()
		{
			return this.JustDecompileGenerated_IsResolvedImplicitAssembly_k__BackingField;
		}

		protected void JustDecompileGenerated_set_IsResolvedImplicitAssembly(bool value)
		{
			this.JustDecompileGenerated_IsResolvedImplicitAssembly_k__BackingField = value;
		}

		public string Location
		{
			get
			{
				if (this.InternalAssembly == null)
				{
					return string.Empty;
				}
				return this.InternalAssembly.Location;
			}
		}

		public string ManifestModule
		{
			get
			{
				if (this.InternalAssembly == null)
				{
					return string.Empty;
				}
				return this.InternalAssembly.ManifestModule.Name;
			}
		}

		public string Name
		{
			get
			{
				return JustDecompileGenerated_get_Name();
			}
			set
			{
				JustDecompileGenerated_set_Name(value);
			}
		}

		private string JustDecompileGenerated_Name_k__BackingField;

		public string JustDecompileGenerated_get_Name()
		{
			return this.JustDecompileGenerated_Name_k__BackingField;
		}

		protected void JustDecompileGenerated_set_Name(string value)
		{
			this.JustDecompileGenerated_Name_k__BackingField = value;
		}

		public System.Version Version
		{
			get
			{
				if (this.InternalAssembly == null)
				{
					return null;
				}
				return this.InternalAssembly.GetName().Version;
			}
		}

		public ReferenceAssembly()
		{
		}

		public ReferenceAssembly(Assembly assembly)
		{
			this.InternalAssembly = assembly;
			this.Name = (assembly != null ? assembly.GetName().Name : string.Empty);
		}

		public bool CompareTo(IAssembly assembly)
		{
			IReflectionAssembly reflectionAssembly = assembly as IReflectionAssembly;
			if (reflectionAssembly == null)
			{
				return false;
			}
			return this.InternalAssembly == reflectionAssembly.ReflectionAssembly;
		}

		public bool CompareTo(Assembly assembly)
		{
			return this.InternalAssembly == assembly;
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			IAssembly assembly = obj as IAssembly;
			if (assembly == null)
			{
				return false;
			}
			return assembly.Name == this.Name;
		}

		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}

		public Stream GetManifestResourceStream(string name)
		{
			if (this.InternalAssembly == null)
			{
				return null;
			}
			return this.InternalAssembly.GetManifestResourceStream(name);
		}

		public byte[] GetPublicKeyToken()
		{
			if (this.InternalAssembly == null)
			{
				return null;
			}
			return AssemblyHelper.GetAssemblyName(this.InternalAssembly).GetPublicKeyToken();
		}

		public override string ToString()
		{
			return this.Name;
		}
	}
}