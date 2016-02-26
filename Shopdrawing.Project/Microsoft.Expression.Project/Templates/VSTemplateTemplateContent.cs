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
	[XmlType(AnonymousType=true, Namespace="http://schemas.microsoft.com/developer/vstemplate/2005")]
	public class VSTemplateTemplateContent
	{
		private object[] itemsField;

		private VSTemplateTemplateContentCustomParameter[] customParametersField;

		[XmlArrayItem("CustomParameter", IsNullable=false)]
		public VSTemplateTemplateContentCustomParameter[] CustomParameters
		{
			get
			{
				return this.customParametersField;
			}
			set
			{
				this.customParametersField = value;
			}
		}

		[XmlElement("Project", typeof(VSTemplateTemplateContentProject))]
		[XmlElement("ProjectCollection", typeof(VSTemplateTemplateContentProjectCollection))]
		[XmlElement("ProjectItem", typeof(VSTemplateTemplateContentProjectItem))]
		[XmlElement("References", typeof(VSTemplateTemplateContentReferences))]
		public object[] Items
		{
			get
			{
				return this.itemsField;
			}
			set
			{
				this.itemsField = value;
			}
		}

		public VSTemplateTemplateContent()
		{
		}
	}
}