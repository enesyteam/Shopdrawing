using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Licenses;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.ServiceExtensions;
using Microsoft.Expression.Project.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;

namespace Microsoft.Expression.Project.Licensing
{
	public static class LicensingHelper
	{
		private const string SilverlightMobileTemplateIdPrefix = "Microsoft.Blend.WindowsPhone.";

		private const string WindowsPhoneProfileId = "WindowsPhone";

		private static bool dialogShownThisSession;

		private static bool needToShowDialog;

		private static bool? isAnySketchFlowSkuEnabled;

		public static bool DialogShownThisSession
		{
			get
			{
				return LicensingHelper.dialogShownThisSession;
			}
		}

		public static bool NeedToShowDialog
		{
			get
			{
				return LicensingHelper.needToShowDialog;
			}
		}

		private static IList<Guid> CombineLists(IList<Guid> baseList, IEnumerable<Guid> listBeingAdded)
		{
			IList<Guid> guids = baseList;
			foreach (Guid guid in listBeingAdded)
			{
				if (baseList.Contains(guid))
				{
					continue;
				}
				guids.Add(guid);
			}
			return guids;
		}

		private static LicenseState DetermineLicenseState(ILicenseService licenseService, IList<Guid> skus)
		{
			LicenseState licenseState;
			Guid guid = licenseService.MostPermissiveLicenseSku(skus);
			bool flag = licenseService.FeaturesFromSku(guid).Contains(ExpressionFeatureMapper.ActivationLicense);
			if (licenseService.IsLicensed(guid))
			{
				licenseState = LicenseState.FullLicense();
			}
			else if (!licenseService.IsInGrace(guid))
			{
				bool unlicensedReason = licenseService.GetUnlicensedReason(guid) == UnlicensedReason.GraceTimeExpired;
				bool flag1 = licenseService.HasKey(guid);
				licenseState = LicenseState.Expired((!flag || !unlicensedReason ? false : flag1));
			}
			else
			{
				licenseState = LicenseState.Trial(flag, licenseService.GetRemainingGraceMinutes(guid) / 1440);
			}
			return licenseState;
		}

		public static ProjectLicenseGroup GetLicenseGroup(IProjectTemplate projectTemplate)
		{
			TemplateBase templateBase = projectTemplate as TemplateBase;
			if (templateBase != null)
			{
				VSTemplateTemplateData templateData = templateBase.Template.TemplateData;
				if (templateData.ExpressionBlendPrototypingEnabled)
				{
					return ProjectLicenseGroup.SketchFlow;
				}
				if (templateData.TemplateID != null && templateData.TemplateID.StartsWith("Microsoft.Blend.WindowsPhone.", StringComparison.Ordinal))
				{
					return ProjectLicenseGroup.SilverlightMobile;
				}
			}
			return ProjectLicenseGroup.WpfSilverlight;
		}

		public static ProjectLicenseGroup GetLicenseGroup(IProjectStore projectStore)
		{
			if (ProjectStoreHelper.IsSketchFlowProject(projectStore))
			{
				return ProjectLicenseGroup.SketchFlow;
			}
			FrameworkName targetFrameworkName = ProjectStoreHelper.GetTargetFrameworkName(projectStore);
			if (targetFrameworkName != null && !string.IsNullOrEmpty(targetFrameworkName.Profile) && targetFrameworkName.Profile.Equals("WindowsPhone", StringComparison.OrdinalIgnoreCase))
			{
				return ProjectLicenseGroup.SilverlightMobile;
			}
			return ProjectLicenseGroup.WpfSilverlight;
		}

		public static LicenseState IsProjectLicensed(IProjectStore projectStore, IServiceProvider services)
		{
			return LicensingHelper.ProjectTypeLicense(LicensingHelper.GetLicenseGroup(projectStore), services);
		}

		[CLSCompliant(false)]
		public static bool IsSketchFlowLicensed(ILicenseService licenseService)
		{
			if (LicensingHelper.isAnySketchFlowSkuEnabled.HasValue)
			{
				return LicensingHelper.isAnySketchFlowSkuEnabled.Value;
			}
			LicensingHelper.isAnySketchFlowSkuEnabled = new bool?(licenseService.IsAnySkuEnabled(licenseService.SkusFromFeature(ExpressionFeatureMapper.SketchFlowFeature)));
			return LicensingHelper.isAnySketchFlowSkuEnabled.Value;
		}

		public static LicenseState ProjectLicense(IProjectTemplate template, IServiceProvider services)
		{
			return LicensingHelper.ProjectTypeLicense(LicensingHelper.GetLicenseGroup(template), services);
		}

		internal static LicenseState ProjectTypeLicense(ProjectLicenseGroup group, IServiceProvider services)
		{
			ILicenseService service = services.GetService<ILicenseService>();
			switch (group)
			{
				case ProjectLicenseGroup.WpfSilverlight:
				{
					IList<Guid> guids = service.SkusFromFeature(ExpressionFeatureMapper.WpfFeature);
					IList<Guid> guids1 = service.SkusFromFeature(ExpressionFeatureMapper.SilverlightFeature);
					return LicensingHelper.DetermineLicenseState(service, LicensingHelper.CombineLists(guids, guids1));
				}
				case ProjectLicenseGroup.SilverlightMobile:
				{
					return LicensingHelper.DetermineLicenseState(service, service.SkusFromFeature(ExpressionFeatureMapper.MobileFeature));
				}
				case ProjectLicenseGroup.SketchFlow:
				{
					IList<Guid> list = service.SkusFromFeature(ExpressionFeatureMapper.HobbledSketchFlowFeature);
					list = list.Union<Guid>(service.SkusFromFeature(ExpressionFeatureMapper.SketchFlowFeature)).ToList<Guid>();
					return LicensingHelper.DetermineLicenseState(service, list);
				}
			}
			return LicenseState.FullLicense();
		}

		public static void ResetCache()
		{
			LicensingHelper.isAnySketchFlowSkuEnabled = null;
		}

		private static void ShowLicensingDialog(IServiceProvider services)
		{
			LicensingHelper.dialogShownThisSession = true;
			LicensingHelper.needToShowDialog = false;
			(new ItemizedLicensingDialogCreator((IServices)services)).GetInstance.ShowDialog();
		}

		public static void ShowLicensingDialogIfNecessary(IServiceProvider services)
		{
			if (LicensingHelper.needToShowDialog)
			{
				LicensingHelper.ShowLicensingDialog(services);
			}
		}

		public static void SuppressDialogForSession()
		{
			LicensingHelper.dialogShownThisSession = true;
		}

		public static void UnlicensedProjectLoadAttempted()
		{
			if (!LicensingHelper.dialogShownThisSession)
			{
				LicensingHelper.needToShowDialog = true;
			}
		}
	}
}