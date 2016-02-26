using System;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace Microsoft.Expression.Project
{
	public interface IBlendSdkInfo
	{
		bool IsInstalled
		{
			get;
		}

		string RootSdkInstallPath
		{
			get;
		}

		IEnumerable<string> GetExtensionDirectories(FrameworkName frameworkName);

		string GetFontTargetsFile(FrameworkName frameworkName);

		string GetHelpFileName(FrameworkName frameworkName);

		string GetHelpPath(FrameworkName frameworkName);

		string GetInteractivityPath(FrameworkName frameworkName);

		string GetPrototypingPath(FrameworkName frameworkName);

		string GetTemplatePath(FrameworkName frameworkName);

		bool SupportsFramework(FrameworkName frameworkName);
	}
}