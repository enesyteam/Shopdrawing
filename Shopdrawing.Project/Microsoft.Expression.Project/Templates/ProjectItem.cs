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
	public class ProjectItem
	{
		private string targetFileNameField;

		private bool replaceParametersField;

		private bool replaceParametersFieldSpecified;

		private bool openInEditorField;

		private bool openInEditorFieldSpecified;

		private int openOrderField;

		private bool openOrderFieldSpecified;

		private bool openInWebBrowserField;

		private bool openInWebBrowserFieldSpecified;

		private bool openInHelpBrowserField;

		private bool openInHelpBrowserFieldSpecified;

		private string valueField;

		[XmlAttribute]
		public bool OpenInEditor
		{
			get
			{
				return this.openInEditorField;
			}
			set
			{
				this.openInEditorField = value;
			}
		}

		[XmlIgnore]
		public bool OpenInEditorSpecified
		{
			get
			{
				return this.openInEditorFieldSpecified;
			}
			set
			{
				this.openInEditorFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public bool OpenInHelpBrowser
		{
			get
			{
				return this.openInHelpBrowserField;
			}
			set
			{
				this.openInHelpBrowserField = value;
			}
		}

		[XmlIgnore]
		public bool OpenInHelpBrowserSpecified
		{
			get
			{
				return this.openInHelpBrowserFieldSpecified;
			}
			set
			{
				this.openInHelpBrowserFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public bool OpenInWebBrowser
		{
			get
			{
				return this.openInWebBrowserField;
			}
			set
			{
				this.openInWebBrowserField = value;
			}
		}

		[XmlIgnore]
		public bool OpenInWebBrowserSpecified
		{
			get
			{
				return this.openInWebBrowserFieldSpecified;
			}
			set
			{
				this.openInWebBrowserFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public int OpenOrder
		{
			get
			{
				return this.openOrderField;
			}
			set
			{
				this.openOrderField = value;
			}
		}

		[XmlIgnore]
		public bool OpenOrderSpecified
		{
			get
			{
				return this.openOrderFieldSpecified;
			}
			set
			{
				this.openOrderFieldSpecified = value;
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

		public ProjectItem()
		{
		}
	}
}