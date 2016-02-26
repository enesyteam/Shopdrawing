using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.Licensing;
using Microsoft.Expression.Project.ServiceExtensions;
using Microsoft.Expression.Project.UserInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Threading;

namespace Microsoft.Expression.Project.Conversion
{
	internal class SolutionConverter : AggregatedConverter
	{
		private const string Key = "SolutionConverter";

		private Dictionary<ConversionType, ConversionType> sketchFlowVersionMapping = new Dictionary<ConversionType, ConversionType>()
		{
			{ ConversionType.BuildToolsVersion20, ConversionType.BuildToolsVersion40 },
			{ ConversionType.BuildToolsVersion35, ConversionType.BuildToolsVersion40 },
			{ ConversionType.BuildToolsVersion40, ConversionType.DoNothing },
			{ ConversionType.BuildToolsVersionNone, ConversionType.BuildToolsVersion40 },
			{ ConversionType.BuildToolsVersionUnknown, ConversionType.DoNothing },
			{ ConversionType.ProjectSilverlight1, ConversionType.DoNothing },
			{ ConversionType.ProjectSilverlight2, ConversionType.ProjectSilverlight4 },
			{ ConversionType.ProjectSilverlight3, ConversionType.ProjectSilverlight4 },
			{ ConversionType.ProjectSilverlight4, ConversionType.DoNothing },
			{ ConversionType.ProjectWpf30, ConversionType.DoNothing },
			{ ConversionType.ProjectWpf35, ConversionType.ProjectWpf40 },
			{ ConversionType.ProjectWpf40, ConversionType.DoNothing },
			{ ConversionType.WebAppProject9, ConversionType.WebAppProject10 },
			{ ConversionType.WebAppProject10, ConversionType.DoNothing },
			{ ConversionType.BlendSdk3, ConversionType.BlendSdk4 },
			{ ConversionType.BlendSdkFontEmbedding2, ConversionType.BlendSdkFontEmbedding4 },
			{ ConversionType.BlendSdkFontEmbedding3, ConversionType.BlendSdkFontEmbedding4 },
			{ ConversionType.BlendSdkFontEmbedding4, ConversionType.DoNothing },
			{ ConversionType.Unknown, ConversionType.DoNothing }
		};

		private Dictionary<ConversionType, ConversionType> versionMapping = new Dictionary<ConversionType, ConversionType>()
		{
			{ ConversionType.BuildToolsVersion20, ConversionType.BuildToolsVersion40 },
			{ ConversionType.BuildToolsVersion35, ConversionType.BuildToolsVersion40 },
			{ ConversionType.BuildToolsVersion40, ConversionType.DoNothing },
			{ ConversionType.BuildToolsVersionNone, ConversionType.BuildToolsVersion40 },
			{ ConversionType.BuildToolsVersionUnknown, ConversionType.DoNothing },
			{ ConversionType.ProjectSilverlight1, ConversionType.DoNothing },
			{ ConversionType.ProjectSilverlight2, ConversionType.ProjectSilverlight4 },
			{ ConversionType.ProjectSilverlight3, ConversionType.DoNothing },
			{ ConversionType.ProjectSilverlight4, ConversionType.DoNothing },
			{ ConversionType.ProjectWpf30, ConversionType.DoNothing },
			{ ConversionType.ProjectWpf35, ConversionType.DoNothing },
			{ ConversionType.ProjectWpf40, ConversionType.DoNothing },
			{ ConversionType.WebAppProject9, ConversionType.WebAppProject10 },
			{ ConversionType.WebAppProject10, ConversionType.DoNothing },
			{ ConversionType.BlendSdk3, ConversionType.BlendSdk4 },
			{ ConversionType.BlendSdkFontEmbedding2, ConversionType.BlendSdkFontEmbedding4 },
			{ ConversionType.BlendSdkFontEmbedding3, ConversionType.BlendSdkFontEmbedding4 },
			{ ConversionType.BlendSdkFontEmbedding4, ConversionType.DoNothing },
			{ ConversionType.Unknown, ConversionType.DoNothing }
		};

		private List<IProjectConverter> converters;

		private IEnumerable<IProjectStore> contextProjectStores;

		private IProjectStore targetStore;

		private bool sketchFlowSolution;

		private bool silverlightSolution;

		private bool frameworkSolution;

		protected override IList<IProjectConverter> Converters
		{
			get
			{
				return this.converters;
			}
		}

		public override string Identifier
		{
			get
			{
				return "SolutionConverter";
			}
		}

		protected override bool IgnoreEnabled
		{
			get
			{
				return !this.AllowDisabling;
			}
		}

		protected override IDictionary<ConversionType, ConversionType> VersionMapping
		{
			get
			{
				if (this.sketchFlowSolution)
				{
					return this.sketchFlowVersionMapping;
				}
				return this.versionMapping;
			}
		}

		public SolutionConverter(ISolutionManagement solution, IEnumerable<IProjectStore> contextProjectStores, IServiceProvider serviceProvider) : base(solution, serviceProvider)
		{
			this.contextProjectStores = contextProjectStores;
			List<IProjectConverter> projectConverters = new List<IProjectConverter>()
			{
				new SilverlightProjectConverter(solution, serviceProvider),
				new BlendSdkConverter(solution, serviceProvider),
				new FontEmbeddingSdkConverter(solution, serviceProvider),
				new WpfProjectConverter(solution, serviceProvider),
				new WebProjectConverter(solution, serviceProvider),
				new ToolsVersionConverter(solution, serviceProvider)
			};
			this.converters = projectConverters;
		}

		public SolutionConverter(ISolutionManagement solution, IEnumerable<IProjectStore> contextProjectStores, IProjectStore targetStore, IServiceProvider serviceProvider) : this(solution, contextProjectStores, serviceProvider)
		{
			this.targetStore = targetStore;
		}

		protected override IEnumerable<ConversionTarget> GetConversionTargets()
		{
			if (this.targetStore == null)
			{
				foreach (IProjectStore contextProjectStore in this.contextProjectStores)
				{
					yield return new ConversionTarget(contextProjectStore);
				}
			}
			else
			{
				yield return new ConversionTarget(this.targetStore);
			}
		}

		private IEnumerable<UpgradeAction> GetProposedUpgrades(Dictionary<ConversionType, ConversionType> conversionMapping)
		{
			return ConversionHelper.CheckAndAddFile(new ConversionTarget(base.Solution), EnumerableExtensions.AsEnumerable<IProjectConverter>(this), conversionMapping, false);
		}

		public override ConversionType GetVersion(ConversionTarget project)
		{
			ConversionType conversionType;
			this.silverlightSolution = false;
			this.sketchFlowSolution = false;
			this.frameworkSolution = false;
			if (!project.IsSolution)
			{
				return ConversionType.Unknown;
			}
			using (IEnumerator<IProjectStore> enumerator = this.contextProjectStores.GetEnumerator())
			{
				do
				{
					if (!enumerator.MoveNext())
					{
						break;
					}
					IProjectStore current = enumerator.Current;
					FrameworkName targetFrameworkName = ProjectStoreHelper.GetTargetFrameworkName(current);
					if (targetFrameworkName == null)
					{
						continue;
					}
					if (!this.frameworkSolution && targetFrameworkName.Identifier == ".NETFramework")
					{
						this.frameworkSolution = true;
					}
					if (!this.silverlightSolution && targetFrameworkName.Identifier == "Silverlight")
					{
						this.silverlightSolution = true;
					}
					if (this.sketchFlowSolution || !ProjectStoreHelper.IsSketchFlowProject(current))
					{
						continue;
					}
					this.sketchFlowSolution = true;
				}
				while (!this.silverlightSolution || !this.sketchFlowSolution || !this.frameworkSolution);
			}
			if (!this.silverlightSolution && !this.frameworkSolution)
			{
				return ConversionType.Unknown;
			}
			ILicenseService service = base.Services.GetService<ILicenseService>();
			if (this.sketchFlowSolution && service != null && !LicensingHelper.IsSketchFlowLicensed(service))
			{
				return ConversionType.Unknown;
			}
			using (IEnumerator<ConversionTarget> enumerator1 = this.GetConversionTargets().GetEnumerator())
			{
				while (enumerator1.MoveNext())
				{
					ConversionTarget conversionTarget = enumerator1.Current;
					if (!ConversionHelper.CheckAndAddFile(conversionTarget, this.Converters, this.VersionMapping, true).Any<UpgradeAction>())
					{
						continue;
					}
					conversionType = ConversionType.SolutionBlendV3;
					return conversionType;
				}
				return ConversionType.SolutionBlendV4;
			}
			return conversionType;
		}

		internal UpgradeWizard.UpgradeResponse PromptToUpgrade(Dictionary<ConversionType, ConversionType> conversionMapping, out bool? doNotShowAgain, ref string backupLocation, out IEnumerable<UpgradeAction> proposedUpgrades)
		{
			doNotShowAgain = null;
			proposedUpgrades = Enumerable.Empty<UpgradeAction>();
			List<UpgradeAction> list = this.GetProposedUpgrades(conversionMapping).ToList<UpgradeAction>();
			if (list.Count == 0)
			{
				return UpgradeWizard.UpgradeResponse.DontUpgrade;
			}
			List<ConversionType> conversionTypes = new List<ConversionType>();
			List<ConversionType> conversionTypes1 = new List<ConversionType>();
			bool flag = false;
			bool flag1 = false;
			if (!this.sketchFlowSolution)
			{
				using (IEnumerator<ConversionTarget> enumerator = this.GetConversionTargets().GetEnumerator())
				{
					do
					{
					Label0:
						if (!enumerator.MoveNext())
						{
							break;
						}
						ConversionTarget current = enumerator.Current;
						FrameworkName targetFrameworkName = ProjectStoreHelper.GetTargetFrameworkName(current.ProjectStore);
						if (targetFrameworkName == null || targetFrameworkName.Version == CommonVersions.Version4_0)
						{
							goto Label0;
						}
						else if (flag || !ProjectStoreHelper.UsesSilverlight(current.ProjectStore) || targetFrameworkName.Version.Major <= 1)
						{
							if (flag1 || !ProjectStoreHelper.UsesWpf(current.ProjectStore) || !(targetFrameworkName.Version >= CommonVersions.Version3_0))
							{
								continue;
							}
							flag1 = true;
						}
						else
						{
							flag = true;
						}
					}
					while (!flag1 || !flag);
				}
				if (flag1)
				{
					conversionTypes1.Add(ConversionType.ProjectWpf35);
					conversionTypes1.Add(ConversionType.ProjectWpf40);
				}
				if (flag)
				{
					conversionTypes.Add(ConversionType.ProjectSilverlight3);
					conversionTypes.Add(ConversionType.ProjectSilverlight4);
				}
			}
			bool flag2 = this.targetStore != null;
			bool flag3 = !flag2;
			bool flag4 = !flag2;
			UpgradeProjectDialog upgradeProjectDialog = new UpgradeProjectDialog(base.Services.ExpressionInformationService(), conversionTypes, conversionTypes1, flag3, flag4);
			ProjectDialogResult projectDialogResult = upgradeProjectDialog.ShowProjectDialog();
			doNotShowAgain = new bool?(upgradeProjectDialog.DoNotShowAgain);
			if (upgradeProjectDialog.Backup && projectDialogResult == ProjectDialogResult.Yes)
			{
				string parentDirectory = Microsoft.Expression.Framework.Documents.PathHelper.GetParentDirectory(Microsoft.Expression.Framework.Documents.PathHelper.GetDirectory(base.Solution.DocumentReference.Path));
				CultureInfo invariantCulture = CultureInfo.InvariantCulture;
				string newNameCopyTemplate = StringTable.NewNameCopyTemplate;
				object[] fileNameWithoutExtension = new object[] { Path.GetFileNameWithoutExtension(base.Solution.DocumentReference.DisplayName) };
				string availableFileOrDirectoryName = string.Format(invariantCulture, newNameCopyTemplate, fileNameWithoutExtension);
				availableFileOrDirectoryName = Microsoft.Expression.Framework.Documents.PathHelper.GetAvailableFileOrDirectoryName(availableFileOrDirectoryName, null, parentDirectory, false);
				backupLocation = Path.Combine(parentDirectory, availableFileOrDirectoryName);
				string str = backupLocation;
				Exception exception1 = null;
				using (IDisposable disposable = TemporaryCursor.SetWaitCursor())
				{
					ErrorHandling.HandleBasicExceptions(() => ProjectManager.CopyDirectory(Microsoft.Expression.Framework.Documents.PathHelper.GetDirectory(this.Solution.DocumentReference.Path), str), (Exception exception) => exception1 = exception);
				}
				if (exception1 != null)
				{
					ErrorArgs errorArg = new ErrorArgs()
					{
						Message = StringTable.UpgradeBackupFailed,
						Exception = exception1,
						AutomationId = "BackupProjectErrorDialog"
					};
					base.Services.MessageDisplayService().ShowError(errorArg);
					return UpgradeWizard.UpgradeResponse.Cancel;
				}
			}
			switch (projectDialogResult)
			{
				case ProjectDialogResult.Yes:
				{
					if (this.sketchFlowSolution)
					{
						proposedUpgrades = list;
					}
					else
					{
						this.VersionMapping[ConversionType.ProjectSilverlight2] = upgradeProjectDialog.SelectedSilverlightVersion;
						this.VersionMapping[ConversionType.ProjectSilverlight3] = upgradeProjectDialog.SelectedSilverlightVersion;
						this.VersionMapping[ConversionType.ProjectWpf30] = upgradeProjectDialog.SelectedDotNetVersion;
						this.VersionMapping[ConversionType.ProjectWpf35] = upgradeProjectDialog.SelectedDotNetVersion;
						proposedUpgrades = this.GetProposedUpgrades(conversionMapping);
					}
					return UpgradeWizard.UpgradeResponse.Upgrade;
				}
				case ProjectDialogResult.No:
				{
					return UpgradeWizard.UpgradeResponse.DontUpgrade;
				}
			}
			return UpgradeWizard.UpgradeResponse.Cancel;
		}
	}
}