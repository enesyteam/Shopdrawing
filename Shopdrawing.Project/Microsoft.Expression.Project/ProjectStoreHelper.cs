using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using System.Threading;

namespace Microsoft.Expression.Project
{
	public static class ProjectStoreHelper
	{
		private const string PropertyNameProjectTypeGuids = "ProjectTypeGuids";

		private const string PropertyNameTargetFrameworkIdentifier = "TargetFrameworkIdentifier";

		private const string PropertyNameTargetFrameworkVersion = "TargetFrameworkVersion";

		private const string PropertyNameTargetFrameworkProfile = "TargetFrameworkProfile";

		private const string PropertyNameSilverlightVersion = "SilverlightVersion";

		private const string PropertyNameSilverlightApplication = "SilverlightApplication";

		private static Regex silverlightTargetVersion;

		private static Regex msBuildVariable;

		internal static ProjectCreator[] ResilientProjectCreationChain;

		internal static ProjectCreator[] DefaultProjectCreationChain;

		static ProjectStoreHelper()
		{
			ProjectStoreHelper.silverlightTargetVersion = new Regex(".*\\\\(?<version>[^\\\\]*)\\\\Microsoft.Silverlight.(?:CSharp|VisualBasic).targets\\s*", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
			ProjectStoreHelper.msBuildVariable = new Regex("\\s*\\$\\((?<variableName>[^\\)]*)\\)\\s*", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
			ProjectCreator[] projectCreator = new ProjectCreator[] { new ProjectCreator(FileSystemProjectStore.CreateInstance), new ProjectCreator(MigratingMSBuildStore.CreateInstance) };
			ProjectStoreHelper.ResilientProjectCreationChain = projectCreator;
			ProjectCreator[] projectCreatorArray = new ProjectCreator[] { new ProjectCreator(FileSystemProjectStore.CreateInstance), new ProjectCreator(MSBuildBasedProjectStore.CreateInstance) };
			ProjectStoreHelper.DefaultProjectCreationChain = projectCreatorArray;
		}

		private static Version ConvertStringToVersion(string versionString)
		{
			string upperInvariant = versionString.ToUpperInvariant();
			string str = upperInvariant;
			if (upperInvariant != null)
			{
				if (str == "V2.0")
				{
					return CommonVersions.Version2_0;
				}
				if (str == "V3.0")
				{
					return CommonVersions.Version3_0;
				}
				if (str == "V4.0")
				{
					return CommonVersions.Version4_0;
				}
			}
			return null;
		}

		internal static IProjectStore CreateProjectStore(DocumentReference documentReference, IServiceProvider serviceProvider, IEnumerable<ProjectCreator> creatorChainOfResponsibility)
		{
			string message;
			string invalidStoreBadPath = null;
			if (!documentReference.IsValidPathFormat)
			{
				invalidStoreBadPath = StringTable.InvalidStoreBadPath;
			}
			else if (!PathHelper.FileOrDirectoryExists(documentReference.Path))
			{
				invalidStoreBadPath = StringTable.InvalidStoreMissingPath;
			}
			if (!string.IsNullOrEmpty(invalidStoreBadPath))
			{
				return InvalidProjectStore.CreateInstance(documentReference, invalidStoreBadPath, serviceProvider);
			}
			Exception exception1 = null;
			IProjectStore projectStore = ProjectStoreHelper.CreateProjectStore(documentReference, serviceProvider, creatorChainOfResponsibility, (Action action, Action<Exception> exceptionAction) => {
				bool flag;
				try
				{
					action();
					flag = true;
				}
				catch (Exception exception)
				{
					exceptionAction(exception);
					flag = false;
				}
				return flag;
			}, (Exception e) => exception1 = e);
			if (projectStore != null)
			{
				return projectStore;
			}
			DocumentReference documentReference1 = documentReference;
			if (exception1 == null)
			{
				message = null;
			}
			else
			{
				message = exception1.Message;
			}
			return InvalidProjectStore.CreateInstance(documentReference1, message, serviceProvider);
		}

		private static IProjectStore CreateProjectStore(DocumentReference documentReference, IServiceProvider serviceProvider, IEnumerable<ProjectCreator> creatorChainOfResponsibility, Func<Action, Action<Exception>, bool> exceptionHandler, Action<Exception> handledExceptionAction)
		{
			IProjectStore projectStore;
			if (documentReference == null)
			{
				throw new ArgumentNullException("documentReference");
			}
			if (serviceProvider == null)
			{
				throw new ArgumentNullException("serviceProvider");
			}
			IProjectStore projectStore1 = null;
			using (IEnumerator<ProjectCreator> enumerator = creatorChainOfResponsibility.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ProjectCreator current = enumerator.Current;
					if (!exceptionHandler(new Action(() => projectStore1 = current(documentReference, serviceProvider)), handledExceptionAction) || projectStore1 == null)
					{
						continue;
					}
					projectStore = projectStore1;
					return projectStore;
				}
				return projectStore1;
			}
			return projectStore;
		}

		public static bool DoesLanguageSupportXaml(IProjectStore projectStore)
		{
			return ProjectStoreHelper.DoesLanguageSupportXaml(ProjectStoreHelper.GetProjectLanguage(projectStore));
		}

		public static bool DoesLanguageSupportXaml(ProjectLanguage projectLanguage)
		{
			if (projectLanguage == ProjectLanguage.CSharp)
			{
				return true;
			}
			return projectLanguage == ProjectLanguage.VisualBasic;
		}

		private static FrameworkName GenerateFrameworkName(string identifier, Version targetFrameork, string profile)
		{
			FrameworkName frameworkName;
			FrameworkName frameworkName1 = null;
			try
			{
				frameworkName1 = new FrameworkName(identifier, targetFrameork, profile);
				string str = frameworkName1.Identifier;
				string str1 = str;
				if (str != null)
				{
					if (str1 != ".NETFramework")
					{
						if (str1 != "Silverlight")
						{
							return null;
						}
						if (!(frameworkName1.Version == CommonVersions.Version2_0) && !(frameworkName1.Version == CommonVersions.Version3_0) && !(frameworkName1.Version == CommonVersions.Version4_0))
						{
							return null;
						}
					}
					else if (!(frameworkName1.Version == CommonVersions.Version2_0) && !(frameworkName1.Version == CommonVersions.Version3_0) && !(frameworkName1.Version == CommonVersions.Version3_5) && !(frameworkName1.Version == CommonVersions.Version4_0))
					{
						return null;
					}
					return frameworkName1;
				}
				return null;
			}
			catch (ArgumentException argumentException)
			{
				frameworkName = null;
			}
			return frameworkName;
		}

		public static ProjectLanguage GetProjectLanguage(IProjectStore projectStore)
		{
			if (projectStore == null)
			{
				return ProjectLanguage.Unknown;
			}
			string property = projectStore.GetProperty("ProjectTypeGuids");
			if (!string.IsNullOrEmpty(property))
			{
				if (property.IndexOf("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}", StringComparison.OrdinalIgnoreCase) != -1)
				{
					return ProjectLanguage.CSharp;
				}
				if (property.IndexOf("{F184B08F-C81C-45F6-A57F-5ABD9991F28F}", StringComparison.OrdinalIgnoreCase) != -1)
				{
					return ProjectLanguage.VisualBasic;
				}
			}
			string upperInvariant = PathHelper.GetSafeExtension(projectStore.DocumentReference.Path).ToUpperInvariant();
			string str = upperInvariant;
			if (upperInvariant != null)
			{
				if (str == ".CSPROJ")
				{
					return ProjectLanguage.CSharp;
				}
				if (str == ".VBPROJ")
				{
					return ProjectLanguage.VisualBasic;
				}
				if (str == ".FSPROJ")
				{
					return ProjectLanguage.FSharp;
				}
				if (str == ".VCXPROJ")
				{
					return ProjectLanguage.CPlusPlus;
				}
			}
			return ProjectLanguage.Unknown;
		}

		internal static IEnumerable<string> GetSilverlightImports(IProjectStore projectStore)
		{
			foreach (string projectImport in projectStore.ProjectImports)
			{
				if (!ProjectStoreHelper.silverlightTargetVersion.Match(projectImport).Success)
				{
					continue;
				}
				yield return projectImport;
			}
		}

		public static Version GetSilverlightVersion(IProjectStore projectStore)
		{
			if (!ProjectStoreHelper.HasSilverlightGuids(projectStore))
			{
				return null;
			}
			return ProjectStoreHelper.ConvertStringToVersion(ProjectStoreHelper.GetSilverlightVersionFromProjectStore(projectStore));
		}

		private static string GetSilverlightVersionFromImports(IProjectStore projectStore)
		{
			string empty = string.Empty;
			foreach (string projectImport in projectStore.ProjectImports)
			{
				Match match = ProjectStoreHelper.silverlightTargetVersion.Match(projectImport);
				if (!match.Success)
				{
					continue;
				}
				empty = match.Groups["version"].Value;
			}
			return empty;
		}

		private static string GetSilverlightVersionFromProjectStore(IProjectStore projectStore)
		{
			string empty = string.Empty;
			empty = (projectStore.StoreVersion != CommonVersions.Version4_0 ? ProjectStoreHelper.GetSilverlightVersionFromImports(projectStore) : ProjectStoreHelper.GetSilverlightVersionUsingProperties(projectStore));
			return empty;
		}

		private static string GetSilverlightVersionUsingProperties(IProjectStore projectStore)
		{
			string property = projectStore.GetProperty("SilverlightVersion");
			if (ProjectStoreHelper.PropertyMatchesMSBuildVariable(property, "TargetFrameworkVersion"))
			{
				property = projectStore.GetProperty("TargetFrameworkVersion");
			}
			return property ?? string.Empty;
		}

		public static FrameworkName GetTargetFrameworkName(IProjectStore projectStore)
		{
			if (projectStore == null)
			{
				return null;
			}
			if (!ProjectStoreHelper.IsKnownLanguage(ProjectStoreHelper.GetProjectLanguage(projectStore)))
			{
				return null;
			}
			string property = projectStore.GetProperty("TargetFrameworkIdentifier");
			string str = projectStore.GetProperty("TargetFrameworkVersion");
			string property1 = projectStore.GetProperty("TargetFrameworkProfile");
			Version version30 = null;
			if (!string.IsNullOrEmpty(str))
			{
				char[] chrArray = new char[] { 'v' };
				if (!Version.TryParse(str.TrimStart(chrArray), out version30))
				{
					version30 = null;
				}
			}
			if (projectStore.StoreVersion == CommonVersions.Version2_0 && version30 == CommonVersions.Version2_0 && ProjectStoreHelper.UsesWpf(projectStore))
			{
				version30 = CommonVersions.Version3_0;
			}
			if (string.IsNullOrEmpty(property) || property.Equals("Silverlight", StringComparison.OrdinalIgnoreCase))
			{
				Version silverlightVersion = ProjectStoreHelper.GetSilverlightVersion(projectStore);
				if (silverlightVersion != null)
				{
					property = "Silverlight";
					version30 = silverlightVersion;
				}
				else
				{
					property = ".NETFramework";
				}
			}
			if (version30 == null)
			{
				return null;
			}
			return ProjectStoreHelper.GenerateFrameworkName(property, version30, property1);
		}

		private static bool HasSilverlightGuids(IProjectStore projectStore)
		{
			string property = projectStore.GetProperty("ProjectTypeGuids");
			if (property == null)
			{
				return false;
			}
			if (property.IndexOf("{A1591282-1198-4647-A2B1-27E5FF5F6F3B}", StringComparison.OrdinalIgnoreCase) != -1)
			{
				return true;
			}
			return property.IndexOf("{C089C8C0-30E0-4E22-80C0-CE093F111A43}", StringComparison.OrdinalIgnoreCase) != -1;
		}

		public static bool IsKnownLanguage(IProjectStore projectStore)
		{
			return ProjectStoreHelper.IsKnownLanguage(ProjectStoreHelper.GetProjectLanguage(projectStore));
		}

		public static bool IsKnownLanguage(ProjectLanguage projectLanguage)
		{
			switch (projectLanguage)
			{
				case ProjectLanguage.CPlusPlus:
				case ProjectLanguage.CSharp:
				case ProjectLanguage.FSharp:
				case ProjectLanguage.VisualBasic:
				{
					return true;
				}
			}
			return false;
		}

		internal static bool IsSketchFlowProject(IProjectStore projectStore)
		{
			if (!string.IsNullOrEmpty(projectStore.GetProperty("ExpressionBlendPrototypingEnabled")))
			{
				return true;
			}
			return false;
		}

		private static bool PropertyMatchesMSBuildVariable(string propertyValue, string variableToMatch)
		{
			if (propertyValue == null)
			{
				return false;
			}
			string value = null;
			Match match = ProjectStoreHelper.msBuildVariable.Match(propertyValue);
			if (match.Success)
			{
				value = match.Groups["variableName"].Value;
			}
			return string.Equals(value, variableToMatch, StringComparison.OrdinalIgnoreCase);
		}

		public static bool UsesSilverlight(IProjectStore projectStore)
		{
			return ProjectStoreHelper.GetSilverlightVersion(projectStore) != null;
		}

		public static bool UsesWpf(IProjectStore projectStore)
		{
			bool flag;
			string property = projectStore.GetProperty("ProjectTypeGuids");
			if (!string.IsNullOrEmpty(property) && property.IndexOf("{60dc8134-eba5-43b8-bcc9-bb4bc16c2548}", StringComparison.OrdinalIgnoreCase) != -1)
			{
				return true;
			}
			using (IEnumerator<IProjectItemData> enumerator = projectStore.GetItems("Reference").GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Value.IndexOf("PresentationFramework", StringComparison.OrdinalIgnoreCase) == -1)
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
}