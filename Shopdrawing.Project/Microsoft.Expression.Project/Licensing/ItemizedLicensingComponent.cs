using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.Project.Licensing
{
	public class ItemizedLicensingComponent : ItemizedLicensingComponentBase
	{
		private ProjectLicenseGroup licenseGroup;

		private LicenseState license;

		private IServiceProvider services;

		private readonly static Dictionary<ProjectLicenseGroup, string> skuNames;

		private readonly static Dictionary<ProjectLicenseGroup, ImageSource> icons;

		public override ImageSource Icon
		{
			get
			{
				ImageSource imageSource;
				if (ItemizedLicensingComponent.icons.TryGetValue(this.licenseGroup, out imageSource))
				{
					return imageSource;
				}
				return null;
			}
		}

		public override ICommand LicenseButtonCommand
		{
			get
			{
				if (base.IsActivatable)
				{
					return new DelegateCommand(() => this.InitializeActiviation());
				}
				return new DelegateCommand(() => this.InitializeKeyEntry());
			}
		}

		public override string Name
		{
			get
			{
				string str;
				if (ItemizedLicensingComponent.skuNames.TryGetValue(this.licenseGroup, out str))
				{
					return str;
				}
				return string.Empty;
			}
		}

		static ItemizedLicensingComponent()
		{
			Dictionary<ProjectLicenseGroup, string> projectLicenseGroups = new Dictionary<ProjectLicenseGroup, string>()
			{
				{ ProjectLicenseGroup.WpfSilverlight, StringTable.LicensingGroupWpfSilverlight },
				{ ProjectLicenseGroup.SketchFlow, StringTable.LicensingGroupSketchFlow }
			};
			ItemizedLicensingComponent.skuNames = projectLicenseGroups;
			Dictionary<ProjectLicenseGroup, ImageSource> projectLicenseGroups1 = new Dictionary<ProjectLicenseGroup, ImageSource>()
			{
				{ ProjectLicenseGroup.WpfSilverlight, Microsoft.Expression.Project.FileTable.GetImageSource("Resources\\BlendLarge.png") },
				{ ProjectLicenseGroup.SketchFlow, Microsoft.Expression.Project.FileTable.GetImageSource("Resources\\SketchFlowLarge.png") }
			};
			ItemizedLicensingComponent.icons = projectLicenseGroups1;
		}

		public ItemizedLicensingComponent(ProjectLicenseGroup licenseGroup, IServiceProvider services)
		{
			this.services = services;
			this.licenseGroup = licenseGroup;
			this.RefreshLicenseValuesInternal();
		}

		private void InitializeActiviation()
		{
			(new ActivateCommand((IServices)this.services)).Execute();
			this.RefreshLicenseValues();
			base.OnStatusChanged();
		}

		private void InitializeKeyEntry()
		{
			(new EnterProductKeyCommand((IServices)this.services)).Execute();
			this.RefreshLicenseValues();
			base.OnStatusChanged();
		}

		protected override void RefreshLicenseValues()
		{
			this.RefreshLicenseValuesInternal();
		}

		private void RefreshLicenseValuesInternal()
		{
			this.license = LicensingHelper.ProjectTypeLicense(this.licenseGroup, this.services);
			base.IsExpired = this.license.IsExpired;
			base.RequiresActivation = !this.license.FullyLicensed;
			base.IsActivatable = this.license.IsActivatable;
			if (!base.IsExpired)
			{
				base.IsTrial = this.license.IsTrial;
				if (!base.IsTrial)
				{
					base.IsLicensed = true;
				}
				else
				{
					base.DaysRemaining = this.license.DaysLeft;
				}
			}
			base.OnPropertyChanged("TrialStatus");
			base.OnPropertyChanged("LicenseButtonCommand");
			base.OnPropertyChanged("ButtonText");
			base.OnPropertyChanged("ActionAvailable");
			base.OnPropertyChanged("IsVisible");
		}
	}
}