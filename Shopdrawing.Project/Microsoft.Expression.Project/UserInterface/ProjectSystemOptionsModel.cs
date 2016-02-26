using Microsoft.Expression.Framework.Configuration;
using System;
using System.ComponentModel;
using System.Threading;

namespace Microsoft.Expression.Project.UserInterface
{
	public sealed class ProjectSystemOptionsModel : INotifyPropertyChanged
	{
		public readonly static string NameInteractiveElementsByDefaultProperty;

		public readonly static string ShowSecurityWarningProperty;

		public readonly static string UseVisualStudioEventHandlerSupportProperty;

		public readonly static string LogAssemblyLoadingProperty;

		public readonly static string LargeImageWarningThresholdProperty;

		private bool nameInteractiveElementsByDefault;

		private bool showSecurityWarning;

		private bool useVisualStudioEventHandlerSupport;

		private bool logAssemblyLoading;

		private int largeImageWarningThreshold;

		public int LargeImageWarningThreshold
		{
			get
			{
				return this.largeImageWarningThreshold;
			}
			set
			{
				this.SetProperty<int>(ProjectSystemOptionsModel.LargeImageWarningThresholdProperty, ref this.largeImageWarningThreshold, value);
			}
		}

		public bool LogAssemblyLoading
		{
			get
			{
				return this.logAssemblyLoading;
			}
			set
			{
				this.SetProperty<bool>(ProjectSystemOptionsModel.LogAssemblyLoadingProperty, ref this.logAssemblyLoading, value);
			}
		}

		public bool NameInteractiveElementsByDefault
		{
			get
			{
				return this.nameInteractiveElementsByDefault;
			}
			set
			{
				this.SetProperty<bool>(ProjectSystemOptionsModel.NameInteractiveElementsByDefaultProperty, ref this.nameInteractiveElementsByDefault, value);
			}
		}

		public bool ShowSecurityWarning
		{
			get
			{
				return this.showSecurityWarning;
			}
			set
			{
				this.SetProperty<bool>(ProjectSystemOptionsModel.ShowSecurityWarningProperty, ref this.showSecurityWarning, value);
			}
		}

		public bool UseVisualStudioEventHandlerSupport
		{
			get
			{
				return this.useVisualStudioEventHandlerSupport;
			}
			set
			{
				this.SetProperty<bool>(ProjectSystemOptionsModel.UseVisualStudioEventHandlerSupportProperty, ref this.useVisualStudioEventHandlerSupport, value);
			}
		}

		static ProjectSystemOptionsModel()
		{
			ProjectSystemOptionsModel.NameInteractiveElementsByDefaultProperty = "NameInteractiveElementsByDefault";
			ProjectSystemOptionsModel.ShowSecurityWarningProperty = "ShowSecurityWarning";
			ProjectSystemOptionsModel.UseVisualStudioEventHandlerSupportProperty = "UseVisualStudioEventHandlerSupport";
			ProjectSystemOptionsModel.LogAssemblyLoadingProperty = "LogAssemblyLoading";
			ProjectSystemOptionsModel.LargeImageWarningThresholdProperty = "LargeImageWarningThreshold";
		}

		public ProjectSystemOptionsModel()
		{
		}

		public void Load(IConfigurationObject value)
		{
			if (value != null)
			{
				this.NameInteractiveElementsByDefault = (bool)value.GetProperty(ProjectSystemOptionsModel.NameInteractiveElementsByDefaultProperty, false);
				this.ShowSecurityWarning = (bool)value.GetProperty(ProjectSystemOptionsModel.ShowSecurityWarningProperty, true);
				this.UseVisualStudioEventHandlerSupport = (bool)value.GetProperty(ProjectSystemOptionsModel.UseVisualStudioEventHandlerSupportProperty, false);
				this.LargeImageWarningThreshold = (int)value.GetProperty(ProjectSystemOptionsModel.LargeImageWarningThresholdProperty, 250);
			}
		}

		public void Save(IConfigurationObject value)
		{
			if (value != null)
			{
				value.SetProperty(ProjectSystemOptionsModel.NameInteractiveElementsByDefaultProperty, this.NameInteractiveElementsByDefault);
				value.SetProperty(ProjectSystemOptionsModel.ShowSecurityWarningProperty, this.ShowSecurityWarning);
				value.SetProperty(ProjectSystemOptionsModel.UseVisualStudioEventHandlerSupportProperty, this.UseVisualStudioEventHandlerSupport);
				value.SetProperty(ProjectSystemOptionsModel.LargeImageWarningThresholdProperty, this.LargeImageWarningThreshold);
			}
		}

		private void SetProperty<T>(string propertyName, ref T propertyStorage, T value)
		{
			if (!object.Equals(propertyStorage, value))
			{
				propertyStorage = value;
				if (this.PropertyChanged != null)
				{
					this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}