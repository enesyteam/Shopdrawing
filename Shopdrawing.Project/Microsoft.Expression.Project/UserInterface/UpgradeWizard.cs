using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.Conversion;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;

namespace Microsoft.Expression.Project.UserInterface
{
	internal class UpgradeWizard
	{
		private IEnumerable<UpgradeAction> proposedUpgrades;

		private IServiceProvider serviceProvider;

		private ISolutionManagement solution;

		private Dictionary<ConversionType, ConversionType> versionMapping = new Dictionary<ConversionType, ConversionType>()
		{
			{ ConversionType.SolutionBlendV3, ConversionType.SolutionBlendV4 },
			{ ConversionType.SolutionBlendV4, ConversionType.DoNothing },
			{ ConversionType.Unknown, ConversionType.DoNothing }
		};

		private SolutionConverter converter;

		private Action solutionUpgradedAction;

		internal UpgradeWizard(ISolutionManagement solution, IEnumerable<IProjectStore> projectStoreContext, IProjectStore targetProjectStore, Action solutionUpgradedAction, IServiceProvider serviceProvider)
		{
			this.solution = solution;
			this.serviceProvider = serviceProvider;
			this.converter = new SolutionConverter(solution, projectStoreContext, targetProjectStore, this.serviceProvider);
			this.solutionUpgradedAction = solutionUpgradedAction;
		}

		private static string SafelyCreateLogFile(ProjectUpgradeLogger upgradeLogger, string pathBase)
		{
			string str;
			string str1 = ".upgrade_log.txt";
			string str2 = string.Concat(pathBase, str1);
			for (int i = 1; i < 10000 && File.Exists(str2); i++)
			{
				object[] objArray = new object[] { pathBase, ".", i, str1 };
				str2 = string.Concat(objArray);
			}
			if (File.Exists(str2))
			{
				return null;
			}
			try
			{
				using (StreamWriter streamWriter = File.CreateText(str2))
				{
					upgradeLogger.Save(streamWriter);
				}
				str = str2;
			}
			catch (Exception exception)
			{
				return null;
			}
			return str;
		}

		private static string SafelyCreateLogFileWithFallback(ProjectUpgradeLogger upgradeLogger, string basePath)
		{
			string str = UpgradeWizard.SafelyCreateLogFile(upgradeLogger, basePath);
			if (!string.IsNullOrEmpty(str))
			{
				return str;
			}
			DateTime now = DateTime.Now;
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(basePath);
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			object[] year = new object[] { fileNameWithoutExtension, now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second };
			string str1 = string.Format(invariantCulture, "{0}.{1}-{2}-{3}_{4}-{5}-{6}", year);
			str1 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), str1);
			str = UpgradeWizard.SafelyCreateLogFile(upgradeLogger, str1);
			if (!string.IsNullOrEmpty(str))
			{
				return str;
			}
			str = UpgradeWizard.SafelyCreateLogFile(upgradeLogger, Path.GetTempFileName());
			if (!string.IsNullOrEmpty(str))
			{
				return str;
			}
			return null;
		}

		public static void SaveLogAndPromptUser(ProjectUpgradeLogger upgradeLogger, IServiceProvider serviceProvider, string basePath, bool success)
		{
			if (upgradeLogger.IsEmpty)
			{
				return;
			}
			string str = UpgradeWizard.SafelyCreateLogFileWithFallback(upgradeLogger, basePath);
			if (string.IsNullOrEmpty(str))
			{
				return;
			}
			success = success & !upgradeLogger.HasErrors;
			if (!success)
			{
				IMessageDisplayService service = (IMessageDisplayService)serviceProvider.GetService(typeof(IMessageDisplayService));
				MessageBoxArgs messageBoxArg = new MessageBoxArgs();
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string upgradeErrorsMessage = StringTable.UpgradeErrorsMessage;
				object[] objArray = new object[] { basePath };
				messageBoxArg.Message = string.Format(currentCulture, upgradeErrorsMessage, objArray);
				messageBoxArg.HyperlinkMessage = StringTable.UpgradeErrorsLinkMessage;
				messageBoxArg.HyperlinkUri = new Uri(str, UriKind.Absolute);
				messageBoxArg.Button = MessageBoxButton.OK;
				messageBoxArg.Image = MessageBoxImage.Hand;
				messageBoxArg.AutomationId = "UpgradeWarningDialog";
				service.ShowMessage(messageBoxArg);
			}
		}

		public bool Upgrade()
		{
			bool? nullable;
			if (ConversionSupressor.IsSupressed)
			{
				return true;
			}
			bool hasErrors = false;
			string str = null;
			UpgradeWizard.UpgradeResponse upgrade = this.converter.PromptToUpgrade(this.versionMapping, out nullable, ref str, out this.proposedUpgrades);
			if (upgrade != UpgradeWizard.UpgradeResponse.Upgrade)
			{
				hasErrors = upgrade == UpgradeWizard.UpgradeResponse.DontUpgrade;
			}
			else
			{
				using (ProjectUpgradeLogger projectUpgradeLogger = new ProjectUpgradeLogger())
				{
					hasErrors = true;
					using (IDisposable disposable = TemporaryCursor.SetWaitCursor())
					{
						foreach (UpgradeAction proposedUpgrade in this.proposedUpgrades)
						{
							if (proposedUpgrade.DoUpgrade())
							{
								continue;
							}
							hasErrors = false;
						}
					}
					hasErrors = hasErrors & !projectUpgradeLogger.HasErrors;
					if (hasErrors && this.solutionUpgradedAction != null)
					{
						this.solutionUpgradedAction();
					}
					if (hasErrors && !string.IsNullOrEmpty(str))
					{
						MessageBoxArgs messageBoxArg = new MessageBoxArgs()
						{
							Message = StringTable.UpgradeUpgradeAndBackupSuccess,
							Button = MessageBoxButton.OK,
							Image = MessageBoxImage.Asterisk,
							AutomationId = "BackupAndUpgradeCompletedDialog",
							HyperlinkMessage = StringTable.UpgradeBackupFolderLink,
							HyperlinkUri = new Uri(str)
						};
						this.serviceProvider.MessageDisplayService().ShowMessage(messageBoxArg);
					}
					UpgradeWizard.SaveLogAndPromptUser(projectUpgradeLogger, this.serviceProvider, this.solution.DocumentReference.Path, hasErrors);
				}
			}
			if (nullable.HasValue && nullable.Value)
			{
				this.converter.IsEnabled = false;
			}
			return hasErrors;
		}

		internal enum UpgradeResponse
		{
			Cancel,
			Upgrade,
			DontUpgrade
		}
	}
}