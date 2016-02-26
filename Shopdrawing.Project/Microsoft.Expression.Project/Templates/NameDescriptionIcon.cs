using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Expression.Project.Templates
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[Serializable]
	[XmlType(Namespace="http://schemas.microsoft.com/developer/vstemplate/2005")]
	public class NameDescriptionIcon
	{
		private string packageField;

		private string idField;

		private string valueField;

		[XmlAttribute]
		public string ID
		{
			get
			{
				return this.idField;
			}
			set
			{
				this.idField = value;
			}
		}

		[XmlAttribute]
		public string Package
		{
			get
			{
				return this.packageField;
			}
			set
			{
				this.packageField = value;
			}
		}

		[XmlText]
		public string Value
		{
			get
			{
				return this.valueField;
			}
			set
			{
				this.valueField = value;
			}
		}

		public NameDescriptionIcon()
		{
		}
	}
}