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
	public class VSTemplateTemplateContentReferencesReference
	{
		private string assemblyField;

		public string Assembly
		{
			get
			{
				return this.assemblyField;
			}
			set
			{
				this.assemblyField = value;
			}
		}

		public VSTemplateTemplateContentReferencesReference()
		{
		}
	}
}