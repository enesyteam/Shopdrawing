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
	[XmlRoot(Namespace="http://schemas.microsoft.com/developer/vstemplate/2005", IsNullable=false)]
	[XmlType(AnonymousType=true, Namespace="http://schemas.microsoft.com/developer/vstemplate/2005")]
	public class ProjectTemplateLink
	{
		private string projectNameField;

		private string valueField;

		[XmlAttribute]
		public string ProjectName
		{
			get
			{
				return this.projectNameField;
			}
			set
			{
				this.projectNameField = value;
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

		public ProjectTemplateLink()
		{
		}
	}
}