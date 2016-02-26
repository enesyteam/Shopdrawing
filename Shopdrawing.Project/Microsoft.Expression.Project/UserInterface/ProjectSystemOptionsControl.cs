using Microsoft.Expression.Framework.ValueEditors;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Microsoft.Expression.Project.UserInterface
{
	public sealed class ProjectSystemOptionsControl : Grid
	{
		internal ProjectSystemOptionsControl projectSystemOptionsControl;

		internal Grid projectSystemOptionsGrid;

		internal CheckBox NameInteractiveElementsByDefault;

		internal CheckBox ShowSecurityWarning;

		internal CheckBox UseVisualStudioEventHandlerSupport;

		internal CheckBox LogAssemblyLoading;

		internal NumberEditor LargeImageThresholdEditor;

		public ProjectSystemOptionsControl(ProjectSystemOptionsModel projectOptions)
		{
			base.DataContext = projectOptions;
			this.InitializeComponent();
		}
	}
}