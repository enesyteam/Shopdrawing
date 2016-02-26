using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.Expression.Project.Conversion
{
	internal sealed class FontEmbeddingSdkConverter : ProjectConverter
	{
		private const string Key = "FontEmbeddingLocalToSdk";

		private const string SilverlightOldTargetFileSdk2 = "SubsetFontSilverlight.targets";

		private const string SilverlightOldTargetFileSdk3 = "$(MSBuildExtensionsPath)\\Microsoft\\Expression\\Blend\\3.0\\Silverlight\\Microsoft.Expression.Blend.Silverlight.targets";

		private const string WpfOldTargetFileSdk2 = "SubsetFont.targets";

		private const string WpfOldTargetFileSdk3 = "$(MSBuildExtensionsPath)\\Microsoft\\Expression\\Blend\\3.0\\WPF\\Microsoft.Expression.Blend.WPF.targets";

		private const string SubsetTaskFile = "SubsetFontTask.dll";

		private readonly static string[] itemsToRemove;

		public override string Identifier
		{
			get
			{
				return "FontEmbeddingLocalToSdk";
			}
		}

		static FontEmbeddingSdkConverter()
		{
			string[] strArrays = new string[] { "SubsetFontSilverlight.targets", "$(MSBuildExtensionsPath)\\Microsoft\\Expression\\Blend\\3.0\\Silverlight\\Microsoft.Expression.Blend.Silverlight.targets", "SubsetFont.targets", "$(MSBuildExtensionsPath)\\Microsoft\\Expression\\Blend\\3.0\\WPF\\Microsoft.Expression.Blend.WPF.targets", "SubsetFontTask.dll" };
			FontEmbeddingSdkConverter.itemsToRemove = strArrays;
		}

		public FontEmbeddingSdkConverter(ISolutionManagement solution, IServiceProvider serviceProvider) : base(solution, serviceProvider)
		{
		}

		public override ConversionType GetVersion(ConversionTarget file)
		{
			if (file.IsProject && BlendSdkHelper.IsAnyCurrentVersionSdkInstalled)
			{
				IEnumerable<string> projectImports = file.ProjectStore.ProjectImports;
				if (projectImports != null)
				{
					if (BlendSdkHelper.IsSdkInstalled(BlendSdkHelper.CurrentWpfVersion))
					{
						if (projectImports.Contains<string>("SubsetFont.targets"))
						{
							return ConversionType.BlendSdkFontEmbedding2;
						}
						if (projectImports.Contains<string>("$(MSBuildExtensionsPath)\\Microsoft\\Expression\\Blend\\3.0\\WPF\\Microsoft.Expression.Blend.WPF.targets"))
						{
							return ConversionType.BlendSdkFontEmbedding3;
						}
						if (projectImports.Contains<string>(BlendSdkHelper.CurrentVersionWpfTargetsFile))
						{
							return ConversionType.BlendSdkFontEmbedding4;
						}
					}
					if (BlendSdkHelper.IsSdkInstalled(BlendSdkHelper.CurrentSilverlightVersion))
					{
						if (projectImports.Contains<string>("SubsetFontSilverlight.targets"))
						{
							return ConversionType.BlendSdkFontEmbedding2;
						}
						if (projectImports.Contains<string>("$(MSBuildExtensionsPath)\\Microsoft\\Expression\\Blend\\3.0\\Silverlight\\Microsoft.Expression.Blend.Silverlight.targets"))
						{
							return ConversionType.BlendSdkFontEmbedding3;
						}
						if (projectImports.Contains<string>(BlendSdkHelper.CurrentVersionSilverlightTargetsFile))
						{
							return ConversionType.BlendSdkFontEmbedding4;
						}
					}
				}
			}
			return ConversionType.Unknown;
		}

		private bool ReplaceFontTargets(IProjectStore projectStore)
		{
			projectStore.ChangeImport("SubsetFontSilverlight.targets", BlendSdkHelper.CurrentVersionSilverlightTargetsFile);
			projectStore.ChangeImport("$(MSBuildExtensionsPath)\\Microsoft\\Expression\\Blend\\3.0\\Silverlight\\Microsoft.Expression.Blend.Silverlight.targets", BlendSdkHelper.CurrentVersionSilverlightTargetsFile);
			projectStore.ChangeImport("SubsetFont.targets", BlendSdkHelper.CurrentVersionWpfTargetsFile);
			projectStore.ChangeImport("$(MSBuildExtensionsPath)\\Microsoft\\Expression\\Blend\\3.0\\WPF\\Microsoft.Expression.Blend.WPF.targets", BlendSdkHelper.CurrentVersionWpfTargetsFile);
			string[] strArrays = FontEmbeddingSdkConverter.itemsToRemove;
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				foreach (IProjectItemData item in projectStore.GetItems(strArrays[i]))
				{
					projectStore.RemoveItem(item);
				}
			}
			string[] strArrays1 = FontEmbeddingSdkConverter.itemsToRemove;
			for (int j = 0; j < (int)strArrays1.Length; j++)
			{
				this.TryDeleteFile(strArrays1[j], projectStore.DocumentReference.Path);
			}
			foreach (IProjectItemData projectItemDatum in projectStore.GetItems("BlendEmbeddedFont"))
			{
				if (!string.IsNullOrEmpty(projectItemDatum.GetMetadata("All")))
				{
					continue;
				}
				projectItemDatum.SetItemMetadata("All", "True");
				projectItemDatum.SetItemMetadata("AutoFill", "True");
			}
			return true;
		}

		private void TryDeleteFile(string fileName, string projectPath)
		{
			string str = Microsoft.Expression.Framework.Documents.PathHelper.ResolveRelativePath(Path.GetDirectoryName(projectPath), fileName);
			if (Microsoft.Expression.Framework.Documents.PathHelper.FileExists(str) && ProjectPathHelper.AttemptToMakeWritable(DocumentReference.Create(str), base.Context))
			{
				Microsoft.Expression.Framework.Documents.PathHelper.ClearFileOrDirectoryReadOnlyAttribute(str);
				File.Delete(str);
			}
		}

		protected override bool UpgradeProject(IProjectStore projectStore, ConversionType initialVersion, ConversionType targetVersion)
		{
			return this.ReplaceFontTargets(projectStore);
		}
	}
}