using Microsoft.Expression.Framework;
using Microsoft.Expression.Project.Conversion;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

namespace Microsoft.Expression.Project.UserInterface
{
	public sealed class UpgradeProjectDialog : NewProjectDialog
	{
		internal Grid OuterPanel;

		internal Label SilverlightVersionLabel;

		internal ComboBox SilverlightVersionComboBox;

		internal Label DotNetVersionLabel;

		internal ComboBox DotNetVersionComboBox;

		internal Button AcceptButton;

		internal Button DiscardButton;

		public bool Backup
		{
			get;
			set;
		}

		public bool DoNotShowAgain
		{
			get;
			set;
		}

		public List<ConversionType> DotNetVersions
		{
			get;
			private set;
		}

		public string MessageText
		{
			get
			{
				CultureInfo currentUICulture = CultureInfo.CurrentUICulture;
				string upgradeMessageProject = StringTable.UpgradeMessageProject;
				object[] applicationName = new object[] { base.ApplicationName };
				return string.Format(currentUICulture, upgradeMessageProject, applicationName);
			}
		}

		public ConversionType SelectedDotNetVersion
		{
			get
			{
				if (this.DotNetVersionComboBox.SelectedItem == null)
				{
					return ConversionType.Unknown;
				}
				return this.DotNetVersions[this.DotNetVersionComboBox.SelectedIndex];
			}
		}

		public ConversionType SelectedSilverlightVersion
		{
			get
			{
				if (this.SilverlightVersionComboBox.SelectedItem == null)
				{
					return ConversionType.Unknown;
				}
				return this.SilverlightVersions[this.SilverlightVersionComboBox.SelectedIndex];
			}
		}

		public bool ShowBackupOption
		{
			get;
			private set;
		}

		public bool ShowDoNotShowAgainOption
		{
			get;
			private set;
		}

		public List<ConversionType> SilverlightVersions
		{
			get;
			private set;
		}

		public UpgradeProjectDialog(IExpressionInformationService expressionInformationService, List<ConversionType> silverlightVersions, List<ConversionType> dotNetVersions, bool showDoNotShowAgainOption, bool showBackupOption) : base("UpgradeProjectDialog", expressionInformationService)
		{
			this.InitializeComponent();
			if (silverlightVersions != null)
			{
				this.SilverlightVersions = silverlightVersions;
			}
			else
			{
				this.SilverlightVersions = new List<ConversionType>();
			}
			if (dotNetVersions != null)
			{
				this.DotNetVersions = dotNetVersions;
			}
			else
			{
				this.DotNetVersions = new List<ConversionType>();
			}
			this.DotNetVersionComboBox.SelectedIndex = 0;
			this.SilverlightVersionComboBox.SelectedIndex = 0;
			this.ShowBackupOption = showBackupOption;
			this.ShowDoNotShowAgainOption = showDoNotShowAgainOption;
		}
	}
}