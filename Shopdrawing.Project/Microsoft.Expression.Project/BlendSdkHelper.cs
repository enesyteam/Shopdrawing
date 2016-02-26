using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Windows;

namespace Microsoft.Expression.Project
{
	public static class BlendSdkHelper
	{
		private static IEnumerable<FrameworkName> knownPlatforms;

		private static FrameworkNameDictionary<IBlendSdkInfo> blendSdkInfoDictionary;

		private static IEnumerable<string> templatePaths;

		private static FrameworkNameDictionary<IBlendSdkInfo> BlendSdkInfoDictionary
		{
			get
			{
				BlendSdkHelper.InitializeBlendSdkInfos();
				return BlendSdkHelper.blendSdkInfoDictionary;
			}
		}

		public static FrameworkName CurrentSilverlightVersion
		{
			get
			{
				return BlendSdkHelper.Silverlight4;
			}
		}

		private static IEnumerable<FrameworkName> CurrentVersionSdkFrameworkNames
		{
			get
			{
				return new List<FrameworkName>()
				{
					BlendSdkHelper.CurrentWpfVersion,
					BlendSdkHelper.CurrentSilverlightVersion,
					BlendSdkHelper.CurrentWindowsPhoneVersion
				};
			}
		}

		public static string CurrentVersionSilverlightTargetsFile
		{
			get
			{
				return BlendSdkHelper.GetFontTargetsFile(BlendSdkHelper.CurrentSilverlightVersion);
			}
		}

		public static string CurrentVersionWpfTargetsFile
		{
			get
			{
				return BlendSdkHelper.GetFontTargetsFile(BlendSdkHelper.CurrentWpfVersion);
			}
		}

		public static FrameworkName CurrentWindowsPhoneVersion
		{
			get
			{
				return BlendSdkHelper.WindowsPhone7;
			}
		}

		public static FrameworkName CurrentWpfVersion
		{
			get
			{
				return BlendSdkHelper.Wpf4;
			}
		}

		public static bool IsAnyCurrentVersionSdkInstalled
		{
			get
			{
				bool flag;
				using (IEnumerator<FrameworkName> enumerator = BlendSdkHelper.CurrentVersionSdkFrameworkNames.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!BlendSdkHelper.IsSdkInstalled(enumerator.Current))
						{
							continue;
						}
						flag = true;
						return flag;
					}
					return false;
				}
				return flag;
			}
		}

		public static bool IsAnySdkInstalled
		{
			get
			{
				var variable = new <>f__AnonymousType1<FrameworkName>[] { new { TargetFramework = BlendSdkHelper.Wpf4 }, new { TargetFramework = BlendSdkHelper.Silverlight4 }, new { TargetFramework = BlendSdkHelper.Wpf35 } };
				var variable1 = variable;
				for (int i = 0; i < (int)variable1.Length; i++)
				{
					if (BlendSdkHelper.IsSdkInstalled(variable1[i].TargetFramework))
					{
						return true;
					}
				}
				return false;
			}
		}

		public static IEnumerable<FrameworkName> KnownPlatforms
		{
			get
			{
				if (BlendSdkHelper.knownPlatforms == null)
				{
					FrameworkName[] wpf35 = new FrameworkName[] { BlendSdkHelper.Wpf35, BlendSdkHelper.Wpf4, BlendSdkHelper.Silverlight3, BlendSdkHelper.Silverlight4, BlendSdkHelper.WindowsPhone7 };
					BlendSdkHelper.knownPlatforms = wpf35;
				}
				return BlendSdkHelper.knownPlatforms;
			}
		}

		public static FrameworkName Silverlight3
		{
			get
			{
				return new FrameworkName("Silverlight", new Version(3, 0));
			}
		}

		public static FrameworkName Silverlight4
		{
			get
			{
				return new FrameworkName("Silverlight", new Version(4, 0));
			}
		}

		public static IEnumerable<string> TemplatePaths
		{
			get
			{
				if (BlendSdkHelper.templatePaths == null)
				{
					HashSet<string> strs = new HashSet<string>();
					foreach (FrameworkName knownPlatform in BlendSdkHelper.KnownPlatforms)
					{
						string templatePath = BlendSdkHelper.GetTemplatePath(knownPlatform);
						if (string.IsNullOrEmpty(templatePath) || strs.Contains(templatePath))
						{
							continue;
						}
						strs.Add(templatePath);
					}
					BlendSdkHelper.templatePaths = strs;
				}
				return BlendSdkHelper.templatePaths;
			}
		}

		public static FrameworkName WindowsPhone7
		{
			get
			{
				return new FrameworkName("Silverlight", new Version(4, 0), "WindowsPhone");
			}
		}

		public static FrameworkName Wpf35
		{
			get
			{
				return new FrameworkName(".NETFramework", new Version(3, 5));
			}
		}

		public static FrameworkName Wpf4
		{
			get
			{
				return new FrameworkName(".NETFramework", new Version(4, 0));
			}
		}

		public static FrameworkName GetCurrentFramework(FrameworkName targetFramework)
		{
			FrameworkName currentWpfVersion = targetFramework;
			if (targetFramework.Identifier == BlendSdkHelper.CurrentWpfVersion.Identifier)
			{
				currentWpfVersion = BlendSdkHelper.CurrentWpfVersion;
			}
			else if (targetFramework.Identifier == BlendSdkHelper.CurrentSilverlightVersion.Identifier)
			{
				currentWpfVersion = (targetFramework.Profile != BlendSdkHelper.CurrentWindowsPhoneVersion.Profile ? BlendSdkHelper.CurrentSilverlightVersion : BlendSdkHelper.CurrentWindowsPhoneVersion);
			}
			return currentWpfVersion;
		}

		public static IEnumerable<string> GetExtensionDirectories(FrameworkName frameworkName)
		{
			IEnumerable<string> strs = Enumerable.Empty<string>();
			foreach (IBlendSdkInfo sdkInfoCompatibleWithFramework in BlendSdkHelper.GetSdkInfoCompatibleWithFramework(frameworkName))
			{
				strs = strs.Concat<string>(sdkInfoCompatibleWithFramework.GetExtensionDirectories(frameworkName));
			}
			return strs;
		}

		public static string GetFontTargetsFile(FrameworkName frameworkName)
		{
			return BlendSdkHelper.GetPath(frameworkName, (FrameworkName framework, IBlendSdkInfo sdkInfo) => sdkInfo.GetFontTargetsFile(frameworkName));
		}

		private static string GetFriendlyFrameworkName(FrameworkName frameworkName)
		{
			if (frameworkName.Identifier == BlendSdkHelper.Wpf35.Identifier)
			{
				return StringTable.DotNetFrameworkShortIdentifier;
			}
			return StringTable.SilverlightShortIdentifier;
		}

		public static string GetHelpFileName(FrameworkName frameworkName)
		{
			return BlendSdkHelper.GetPath(frameworkName, (FrameworkName framework, IBlendSdkInfo sdkInfo) => sdkInfo.GetHelpFileName(frameworkName));
		}

		public static string GetHelpPath(FrameworkName frameworkName)
		{
			return BlendSdkHelper.GetPath(frameworkName, (FrameworkName framework, IBlendSdkInfo sdkInfo) => sdkInfo.GetHelpPath(frameworkName));
		}

		public static string GetInteractivityPath(FrameworkName frameworkName)
		{
			return BlendSdkHelper.GetPath(frameworkName, (FrameworkName framework, IBlendSdkInfo sdkInfo) => sdkInfo.GetInteractivityPath(frameworkName));
		}

		private static string GetPath(FrameworkName frameworkName, Func<FrameworkName, IBlendSdkInfo, string> getter)
		{
			IBlendSdkInfo blendSdkInfo;
			string str = null;
			if (BlendSdkHelper.BlendSdkInfoDictionary.TryGetValue(frameworkName, out blendSdkInfo) && blendSdkInfo.IsInstalled)
			{
				str = getter(frameworkName, blendSdkInfo);
			}
			return str;
		}

		public static string GetPrototypingPath(FrameworkName frameworkName)
		{
			return BlendSdkHelper.GetPath(frameworkName, (FrameworkName framework, IBlendSdkInfo sdkInfo) => sdkInfo.GetPrototypingPath(frameworkName));
		}

		private static IEnumerable<IBlendSdkInfo> GetSdkInfoCompatibleWithFramework(FrameworkName frameworkName)
		{
			IBlendSdkInfo blendSdkInfo;
			List<IBlendSdkInfo> blendSdkInfos = new List<IBlendSdkInfo>();
			foreach (FrameworkName knownPlatform in BlendSdkHelper.KnownPlatforms)
			{
				if (!FrameworkNameEqualityComparer.Instance.Equals(frameworkName, knownPlatform, true, false, true) || !BlendSdkHelper.BlendSdkInfoDictionary.TryGetValue(knownPlatform, out blendSdkInfo))
				{
					continue;
				}
				blendSdkInfos.Add(blendSdkInfo);
			}
			return blendSdkInfos;
		}

		public static string GetTemplatePath(FrameworkName frameworkName)
		{
			return BlendSdkHelper.GetPath(frameworkName, (FrameworkName framework, IBlendSdkInfo sdkInfo) => sdkInfo.GetTemplatePath(frameworkName));
		}

		private static void InitializeBlendSdkInfos()
		{
			if (BlendSdkHelper.blendSdkInfoDictionary == null)
			{
				BlendSdkHelper.blendSdkInfoDictionary = new FrameworkNameDictionary<IBlendSdkInfo>();
				BlendSdkHelper.BlendSdkInfoDictionary.Add(BlendSdkHelper.Wpf35, new Blend3SdkInfo(BlendSdkHelper.Wpf35));
				BlendSdkHelper.BlendSdkInfoDictionary.Add(BlendSdkHelper.Silverlight3, new Blend3SdkInfo(BlendSdkHelper.Silverlight3));
				BlendSdkHelper.BlendSdkInfoDictionary.Add(BlendSdkHelper.Wpf4, new Blend4WpfSdkInfo());
				BlendSdkHelper.BlendSdkInfoDictionary.Add(BlendSdkHelper.Silverlight4, new Blend4SilverlightSdkInfo());
				BlendSdkHelper.BlendSdkInfoDictionary.Add(BlendSdkHelper.WindowsPhone7, new Blend4MobileSdkInfo());
			}
		}

		public static bool IsCurrentVersionSdkFrameworkName(FrameworkName targetFramework)
		{
			bool flag;
			using (IEnumerator<FrameworkName> enumerator = BlendSdkHelper.CurrentVersionSdkFrameworkNames.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!FrameworkNameEqualityComparer.AreEquivalent(targetFramework, enumerator.Current))
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			return flag;
		}

		public static bool IsSdkInstalled(FrameworkName frameworkName)
		{
			IBlendSdkInfo blendSdkInfo;
			if (!BlendSdkHelper.BlendSdkInfoDictionary.TryGetValue(frameworkName, out blendSdkInfo))
			{
				return false;
			}
			return blendSdkInfo.IsInstalled;
		}

		public static void PromptUserForMissingSdk(IServices services)
		{
			IConfigurationObject item = services.GetService<IConfigurationService>()["BlendSDK"];
			var variable = new <>f__AnonymousType2<int, FrameworkName[], string, string, string, string>[3];
			FrameworkName[] wpf35 = new FrameworkName[] { BlendSdkHelper.Wpf35, BlendSdkHelper.Silverlight3 };
			variable[0] = new { SDKVersion = 3, SupportedPlatforms = wpf35, HyperlinkUri = "http://go.microsoft.com/fwlink/?LinkId=139656", SupressDialogPropertyName = "HideInstallSDKDialog", Message = StringTable.Blend3SdkUnavailable, HyperlinkMessage = StringTable.InstallBlend3SdkLink };
			FrameworkName[] wpf4 = new FrameworkName[] { BlendSdkHelper.Wpf4 };
			variable[1] = new { SDKVersion = 4, SupportedPlatforms = wpf4, HyperlinkUri = "http://go.microsoft.com/fwlink/?LinkId=183399", SupressDialogPropertyName = "HideInstall.NETFrameworkSDK4Dialog", Message = StringTable.Blend4SdkUnavailable, HyperlinkMessage = StringTable.InstallBlend4SdkLink };
			FrameworkName[] silverlight4 = new FrameworkName[] { BlendSdkHelper.Silverlight4 };
			variable[2] = new { SDKVersion = 4, SupportedPlatforms = silverlight4, HyperlinkUri = "http://go.microsoft.com/fwlink/?LinkId=183400", SupressDialogPropertyName = "HideInstallSilverlightSDK4Dialog", Message = StringTable.Blend4SdkUnavailable, HyperlinkMessage = StringTable.InstallBlend4SdkLink };
			var variable1 = variable;
			for (int i = 0; i < (int)variable1.Length; i++)
			{
				var variable2 = variable1[i];
				bool flag = true;
				FrameworkName[] supportedPlatforms = variable2.SupportedPlatforms;
				int num = 0;
				while (num < (int)supportedPlatforms.Length)
				{
					if (BlendSdkHelper.IsSdkInstalled(supportedPlatforms[num]))
					{
						num++;
					}
					else
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					item.SetProperty(variable2.SupressDialogPropertyName, false);
				}
				else if (!BlendSdkHelper.ShouldDialogBeHidden(services, variable2.SupressDialogPropertyName))
				{
					bool flag1 = false;
					string friendlyFrameworkName = BlendSdkHelper.GetFriendlyFrameworkName(variable2.SupportedPlatforms[0]);
					CultureInfo currentCulture = CultureInfo.CurrentCulture;
					string message = variable2.Message;
					object[] sDKVersion = new object[] { variable2.SDKVersion, friendlyFrameworkName };
					string str = string.Format(currentCulture, message, sDKVersion);
					CultureInfo cultureInfo = CultureInfo.CurrentCulture;
					string hyperlinkMessage = variable2.HyperlinkMessage;
					object[] objArray = new object[] { variable2.SDKVersion, friendlyFrameworkName };
					string str1 = string.Format(cultureInfo, hyperlinkMessage, objArray);
					MessageBoxArgs messageBoxArg = new MessageBoxArgs()
					{
						Message = str,
						Button = MessageBoxButton.OK,
						Image = MessageBoxImage.Exclamation,
						AutomationId = "BlendSdkDialog",
						HyperlinkMessage = str1,
						HyperlinkUri = new Uri(variable2.HyperlinkUri)
					};
					if (services.GetService<IMessageDisplayService>().ShowMessage(messageBoxArg, out flag1) == MessageBoxResult.OK)
					{
						item.SetProperty(variable2.SupressDialogPropertyName, flag1);
					}
				}
			}
		}

		public static bool ShouldDialogBeHidden(IServices services, string propertyName)
		{
			IConfigurationService service = services.GetService<IConfigurationService>();
			object property = service["BlendSDK"].GetProperty(propertyName);
			if (property != null && (bool)property)
			{
				return true;
			}
			return false;
		}
	}
}