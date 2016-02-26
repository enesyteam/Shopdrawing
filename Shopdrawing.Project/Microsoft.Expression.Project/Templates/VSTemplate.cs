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
	public class VSTemplate
	{
		private VSTemplateTemplateData templateDataField;

		private VSTemplateTemplateContent templateContentField;

		private VSTemplateWizardExtension[] wizardExtensionField;

		private VSTemplateWizardData[] wizardDataField;

		private string typeField;

		private string versionField;

		private VSTemplateExtensibilityWrapper vsTemplateExtensibilityWrapper;

		public VSTemplateTemplateContent TemplateContent
		{
			get
			{
				return this.templateContentField;
			}
			set
			{
				this.templateContentField = value;
			}
		}

		public VSTemplateTemplateData TemplateData
		{
			get
			{
				return this.templateDataField;
			}
			set
			{
				this.templateDataField = value;
			}
		}

		internal VSTemplateExtensibilityWrapper TemplateExtensibilityWrapper
		{
			get
			{
				if (this.vsTemplateExtensibilityWrapper == null)
				{
					this.vsTemplateExtensibilityWrapper = new VSTemplateExtensibilityWrapper(this);
				}
				return this.vsTemplateExtensibilityWrapper;
			}
		}

		[XmlAttribute]
		public string Type
		{
			get
			{
				return this.typeField;
			}
			set
			{
				this.typeField = value;
			}
		}

		[XmlAttribute]
		public string Version
		{
			get
			{
				return this.versionField;
			}
			set
			{
				this.versionField = value;
			}
		}

		[XmlElement("WizardData")]
		public VSTemplateWizardData[] WizardData
		{
			get
			{
				return this.wizardDataField;
			}
			set
			{
				this.wizardDataField = value;
			}
		}

		[XmlElement("WizardExtension")]
		public VSTemplateWizardExtension[] WizardExtension
		{
			get
			{
				return this.wizardExtensionField;
			}
			set
			{
				this.wizardExtensionField = value;
			}
		}

		public VSTemplate()
		{
		}
	}
}