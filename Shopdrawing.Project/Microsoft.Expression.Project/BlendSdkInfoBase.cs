using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Versioning;
using System.Security;

namespace Microsoft.Expression.Project
{
	public abstract class BlendSdkInfoBase : IBlendSdkInfo
	{
		private string interactivityPath;

		private string prototypingPath;

		private string helpPath;

		private string helpFileName;

		private string templatePath;

		private string fontTargetsFile;

		private string rootSdkInstallPath;

		private FrameworkNameDictionary<IEnumerable<string>> extensionDirectories;

		protected abstract string FontTargetsFileFormatString
		{
			get;
		}

		protected abstract string HelpFileNameFormatString
		{
			get;
		}

		protected abstract string HelpRelativePathFormatString
		{
			get;
		}

		protected abstract string InteractivityRelativePathFormatString
		{
			get;
		}

		public bool IsInstalled
		{
			get
			{
				if (!string.IsNullOrEmpty(this.RootSdkInstallPath))
				{
					return true;
				}
				return false;
			}
		}

		protected abstract string PrototypingRelativePathFormatString
		{
			get;
		}

		public string RootSdkInstallPath
		{
			get
			{
				if (string.IsNullOrEmpty(this.rootSdkInstallPath))
				{
					this.rootSdkInstallPath = RegistryHelper.RetrieveRegistryValue<string>(Registry.LocalMachine, this.SdkRegistryPath, "InstallDir");
				}
				return this.rootSdkInstallPath;
			}
		}

		protected abstract string RootToolboxRegistryPath
		{
			get;
		}

		protected abstract string SdkRegistryPath
		{
			get;
		}

		protected abstract string TemplateRelativePathFormatString
		{
			get;
		}

		protected BlendSdkInfoBase()
		{
		}

		protected virtual string GetCustomIdentifierFromFrameworkName(FrameworkName frameworkName)
		{
			return frameworkName.Identifier;
		}

		protected virtual Version GetCustomVersionFromFrameworkName(FrameworkName frameworkName)
		{
			return frameworkName.Version;
		}

		public IEnumerable<string> GetExtensionDirectories(FrameworkName frameworkName)
		{
			IEnumerable<string> extensionDirectories;
			if (this.extensionDirectories == null)
			{
				this.extensionDirectories = new FrameworkNameDictionary<IEnumerable<string>>();
			}
			if (!this.extensionDirectories.TryGetValue(frameworkName, out extensionDirectories))
			{
				string str = string.Concat(this.RootToolboxRegistryPath, "\\Toolbox\\{0}\\v{1}");
				string customIdentifierFromFrameworkName = this.GetCustomIdentifierFromFrameworkName(frameworkName);
				string str1 = this.GetCustomVersionFromFrameworkName(frameworkName).ToString(2);
				CultureInfo invariantCulture = CultureInfo.InvariantCulture;
				object[] objArray = new object[] { customIdentifierFromFrameworkName, str1 };
				string str2 = string.Format(invariantCulture, str, objArray);
				extensionDirectories = BlendSdkInfoBase.GetExtensionDirectories(str2);
				this.extensionDirectories.Add(frameworkName, extensionDirectories);
			}
			return extensionDirectories;
		}

		private static IEnumerable<string> GetExtensionDirectories(string subkeyName)
		{
			List<string> strs = new List<string>();
			try
			{
				RegistryKey[] localMachine = new RegistryKey[] { Registry.LocalMachine, Registry.CurrentUser };
				for (int i = 0; i < (int)localMachine.Length; i++)
				{
					IEnumerable<string> strs1 = RegistryHelper.RetrieveAllRegistrySubkeyValues<string>(localMachine[i], subkeyName);
					if (strs1 != null)
					{
						foreach (string str in strs1)
						{
							if (string.IsNullOrEmpty(str))
							{
								continue;
							}
							string str1 = PathHelper.ResolvePath(str);
							if (!PathHelper.DirectoryExists(str1))
							{
								continue;
							}
							strs.Add(str1);
						}
					}
				}
			}
			catch (SecurityException securityException)
			{
			}
			return strs;
		}

		public string GetFontTargetsFile(FrameworkName frameworkName)
		{
			if (!this.SupportsFramework(frameworkName))
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string unknownFrameworkEncountered = ExceptionStringTable.UnknownFrameworkEncountered;
				object[] str = new object[] { frameworkName.ToString() };
				throw new ArgumentException(string.Format(currentCulture, unknownFrameworkEncountered, str));
			}
			if (string.IsNullOrEmpty(this.fontTargetsFile))
			{
				CultureInfo invariantCulture = CultureInfo.InvariantCulture;
				string fontTargetsFileFormatString = this.FontTargetsFileFormatString;
				object[] customIdentifierFromFrameworkName = new object[] { this.GetCustomIdentifierFromFrameworkName(frameworkName), frameworkName.Identifier, this.GetCustomVersionFromFrameworkName(frameworkName), frameworkName.Profile };
				this.fontTargetsFile = string.Format(invariantCulture, fontTargetsFileFormatString, customIdentifierFromFrameworkName);
			}
			return this.fontTargetsFile;
		}

		public string GetHelpFileName(FrameworkName frameworkName)
		{
			if (!this.SupportsFramework(frameworkName))
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string unknownFrameworkEncountered = ExceptionStringTable.UnknownFrameworkEncountered;
				object[] str = new object[] { frameworkName.ToString() };
				throw new ArgumentException(string.Format(currentCulture, unknownFrameworkEncountered, str));
			}
			if (string.IsNullOrEmpty(this.helpFileName))
			{
				Version customVersionFromFrameworkName = this.GetCustomVersionFromFrameworkName(frameworkName);
				CultureInfo invariantCulture = CultureInfo.InvariantCulture;
				string helpFileNameFormatString = this.HelpFileNameFormatString;
				object[] identifier = new object[] { frameworkName.Identifier, null, null, null };
				int major = customVersionFromFrameworkName.Major;
				identifier[1] = major.ToString(CultureInfo.InvariantCulture);
				int minor = customVersionFromFrameworkName.Minor;
				identifier[2] = minor.ToString(CultureInfo.InvariantCulture);
				identifier[3] = frameworkName.Profile;
				this.helpFileName = string.Format(invariantCulture, helpFileNameFormatString, identifier);
			}
			return this.helpFileName;
		}

		public string GetHelpPath(FrameworkName frameworkName)
		{
			if (!this.SupportsFramework(frameworkName))
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string unknownFrameworkEncountered = ExceptionStringTable.UnknownFrameworkEncountered;
				object[] str = new object[] { frameworkName.ToString() };
				throw new ArgumentException(string.Format(currentCulture, unknownFrameworkEncountered, str));
			}
			if (string.IsNullOrEmpty(this.helpPath))
			{
				this.helpPath = this.GetRelativePath(frameworkName, this.HelpRelativePathFormatString);
			}
			return this.helpPath;
		}

		public string GetInteractivityPath(FrameworkName frameworkName)
		{
			if (!this.SupportsFramework(frameworkName))
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string unknownFrameworkEncountered = ExceptionStringTable.UnknownFrameworkEncountered;
				object[] str = new object[] { frameworkName.ToString() };
				throw new ArgumentException(string.Format(currentCulture, unknownFrameworkEncountered, str));
			}
			if (string.IsNullOrEmpty(this.interactivityPath))
			{
				this.interactivityPath = this.GetRelativePath(frameworkName, this.InteractivityRelativePathFormatString);
			}
			return this.interactivityPath;
		}

		public string GetPrototypingPath(FrameworkName frameworkName)
		{
			if (!this.SupportsFramework(frameworkName))
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string unknownFrameworkEncountered = ExceptionStringTable.UnknownFrameworkEncountered;
				object[] str = new object[] { frameworkName.ToString() };
				throw new ArgumentException(string.Format(currentCulture, unknownFrameworkEncountered, str));
			}
			if (string.IsNullOrEmpty(this.prototypingPath))
			{
				this.prototypingPath = this.GetRelativePath(frameworkName, this.PrototypingRelativePathFormatString);
			}
			return this.prototypingPath;
		}

		private string GetRelativePath(FrameworkName frameworkName, string relativePathFormatString)
		{
			string rootSdkInstallPath = this.RootSdkInstallPath;
			string customIdentifierFromFrameworkName = this.GetCustomIdentifierFromFrameworkName(frameworkName);
			string str = this.GetCustomVersionFromFrameworkName(frameworkName).ToString(2);
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			object[] objArray = new object[] { customIdentifierFromFrameworkName, str };
			string str1 = string.Format(invariantCulture, relativePathFormatString, objArray);
			return PathHelper.ResolveCombinedPath(rootSdkInstallPath, str1);
		}

		public string GetTemplatePath(FrameworkName frameworkName)
		{
			if (!this.SupportsFramework(frameworkName))
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string unknownFrameworkEncountered = ExceptionStringTable.UnknownFrameworkEncountered;
				object[] str = new object[] { frameworkName.ToString() };
				throw new ArgumentException(string.Format(currentCulture, unknownFrameworkEncountered, str));
			}
			if (string.IsNullOrEmpty(this.templatePath))
			{
				this.templatePath = this.GetRelativePath(frameworkName, this.TemplateRelativePathFormatString);
			}
			return this.templatePath;
		}

		public abstract bool SupportsFramework(FrameworkName frameworkName);
	}
}