using System;
using System.Runtime.Versioning;

namespace Microsoft.Expression.Project
{
	public class Blend4WpfSdkInfo : BlendSdkInfoBase
	{
		protected override string FontTargetsFileFormatString
		{
			get
			{
				return "$(MSBuildExtensionsPath)\\Microsoft\\Expression\\Blend\\{1}\\v{2}\\Microsoft.Expression.Blend.WPF.targets";
			}
		}

		protected override string HelpFileNameFormatString
		{
			get
			{
				return "{0}{1}{2}BlendSDK.chm";
			}
		}

		protected override string HelpRelativePathFormatString
		{
			get
			{
				return "{0}\\v{1}\\Help";
			}
		}

		protected override string InteractivityRelativePathFormatString
		{
			get
			{
				return "{0}\\v{1}\\Libraries";
			}
		}

		protected override string PrototypingRelativePathFormatString
		{
			get
			{
				return "{0}\\v{1}\\Prototyping\\Libraries";
			}
		}

		protected override string RootToolboxRegistryPath
		{
			get
			{
				return "SOFTWARE\\Microsoft\\Expression\\Blend\\4.0";
			}
		}

		protected override string SdkRegistryPath
		{
			get
			{
				return "SOFTWARE\\Microsoft\\Expression\\BlendWPFSDK\\2.0";
			}
		}

		protected override string TemplateRelativePathFormatString
		{
			get
			{
				return "{0}\\v{1}\\Templates";
			}
		}

		public Blend4WpfSdkInfo()
		{
		}

		public override bool SupportsFramework(FrameworkName frameworkName)
		{
			return FrameworkNameEqualityComparer.AreEquivalent(frameworkName, BlendSdkHelper.Wpf4);
		}
	}
}