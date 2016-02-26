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
	public class VSTemplateTemplateContentProject
	{
		private object[] itemsField;

		private string fileField;

		private string targetFileNameField;

		private bool replaceParametersField;

		private bool replaceParametersFieldSpecified;

		[XmlAttribute]
		public string File
		{
			get
			{
				return this.fileField;
			}
			set
			{
				this.fileField = value;
			}
		}

		[XmlElement("Folder", typeof(Folder))]
		[XmlElement("ProjectItem", typeof(Microsoft.Expression.Project.Templates.ProjectItem))]
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

		[XmlAttribute]
		public bool ReplaceParameters
		{
			get
			{
				return this.replaceParametersField;
			}
			set
			{
				this.replaceParametersField = value;
			}
		}

		[XmlIgnore]
		public bool ReplaceParametersSpecified
		{
			get
			{
				return this.replaceParametersFieldSpecified;
			}
			set
			{
				this.replaceParametersFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public string TargetFileName
		{
			get
			{
				return this.targetFileNameField;
			}
			set
			{
				this.targetFileNameField = value;
			}
		}

		public VSTemplateTemplateContentProject()
		{
		}
	}
}