using System;
using System.Runtime.Versioning;

namespace Microsoft.Expression.Project
{
	public class Blend3SdkInfo : BlendSdkInfoBase
	{
		private FrameworkName supportedFramework;

		protected override string FontTargetsFileFormatString
		{
			get
			{
				return "$(MSBuildExtensionsPath)\\Microsoft\\Expression\\Blend\\3.0\\{0}\\Microsoft.Expression.Blend.{0}.targets";
			}
		}

		protected override string HelpFileNameFormatString
		{
			get
			{
				return "BlendSDK.chm";
			}
		}

		protected override string HelpRelativePathFormatString
		{
			get
			{
				return "Help";
			}
		}

		protected override string InteractivityRelativePathFormatString
		{
			get
			{
				return "Interactivity\\Libraries\\{0}";
			}
		}

		protected override string PrototypingRelativePathFormatString
		{
			get
			{
				return "Prototyping\\Libraries\\{0}";
			}
		}

		protected override string RootToolboxRegistryPath
		{
			get
			{
				return "SOFTWARE\\Microsoft\\Expression\\Blend\\3.0";
			}
		}

		protected override string SdkRegistryPath
		{
			get
			{
				return "SOFTWARE\\Microsoft\\Expression\\BlendSDK\\1.0";
			}
		}

		protected override string TemplateRelativePathFormatString
		{
			get
			{
				return "Templates";
			}
		}

		public Blend3SdkInfo(FrameworkName supportedFramework)
		{
			this.supportedFramework = supportedFramework;
		}

		protected override string GetCustomIdentifierFromFrameworkName(FrameworkName frameworkName)
		{
			string empty = string.Empty;
			empty = (frameworkName.Identifier != BlendSdkHelper.Wpf35.Identifier ? "Silverlight" : "WPF");
			return empty;
		}

		public override bool SupportsFramework(FrameworkName frameworkName)
		{
			return FrameworkNameEqualityComparer.AreEquivalent(frameworkName, this.supportedFramework);
		}
	}
}