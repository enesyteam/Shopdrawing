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
	public class VSTemplateTemplateContentReferences
	{
		private VSTemplateTemplateContentReferencesReference[] references;

		[XmlElement("Reference", typeof(VSTemplateTemplateContentReferencesReference))]
		public VSTemplateTemplateContentReferencesReference[] References
		{
			get
			{
				return this.references;
			}
			set
			{
				this.references = value;
			}
		}

		public VSTemplateTemplateContentReferences()
		{
		}
	}
}