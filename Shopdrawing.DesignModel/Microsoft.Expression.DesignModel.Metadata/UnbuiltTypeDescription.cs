using System;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public class UnbuiltTypeDescription
	{
		private IAssembly assembly;

		private string clrNamespace;

		private string typeName;

		private IXmlNamespace xmlNamespace;

		private string xamlSourcePath;

		private ITypeId baseType;

		public IAssembly AssemblyReference
		{
			get
			{
				return this.assembly;
			}
		}

		public ITypeId BaseType
		{
			get
			{
				return this.baseType;
			}
		}

		public string ClrNamespace
		{
			get
			{
				return this.clrNamespace;
			}
		}

		public string TypeName
		{
			get
			{
				return this.typeName;
			}
		}

		public string XamlSourcePath
		{
			get
			{
				return this.xamlSourcePath;
			}
			set
			{
				this.xamlSourcePath = value;
			}
		}

		public IXmlNamespace XmlNamespace
		{
			get
			{
				return this.xmlNamespace;
			}
		}

		public UnbuiltTypeDescription(IAssembly assemblyReference, ITypeId baseType, string clrNamespace, string typeName, IXmlNamespace xmlNamespace)
		{
			this.assembly = assemblyReference;
			this.clrNamespace = clrNamespace;
			this.typeName = typeName;
			this.xmlNamespace = xmlNamespace;
			this.baseType = baseType;
		}
	}
}