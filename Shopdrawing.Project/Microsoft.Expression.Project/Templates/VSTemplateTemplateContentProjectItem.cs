using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace Microsoft.Expression.Project.Templates
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[Serializable]
	[XmlType(AnonymousType=true, Namespace="http://schemas.microsoft.com/developer/vstemplate/2005")]
	public class VSTemplateTemplateContentProjectItem
	{
		private string subTypeField;

		private bool replaceParametersField;

		private bool replaceParametersFieldSpecified;

		private string targetFileNameField;

		private string valueField;

		[XmlAttribute]
		public bool OpenInEditor
		{
			get;
			set;
		}

		[XmlIgnore]
		public bool OpenInEditorSpecified
		{
			get;
			set;
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
		public string SubType
		{
			get
			{
				return this.subTypeField;
			}
			set
			{
				this.subTypeField = value;
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

		public VSTemplateTemplateContentProjectItem()
		{
		}
	}
}