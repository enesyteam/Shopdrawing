using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Versioning;
using System.Text;

namespace Microsoft.Expression.Project
{
	internal class ReferenceAssemblyResolver
	{
		private const string tempOutputDirectory = "Expression_Blend_CAC0CD0D_3298_4A3D_9EC0_DBFD8306D79F";

		private static ProjectCollection projectCollection;

		private static BuildManager buildManager;

		private static object syncLock;

		private FrameworkName targetFramework;

		private Microsoft.Build.Evaluation.Project project;

		private Microsoft.Build.Evaluation.Project Project
		{
			get
			{
				if (this.project == null)
				{
					if (ReferenceAssemblyResolver.projectCollection == null)
					{
						ReferenceAssemblyResolver.projectCollection = new ProjectCollection();
						ReferenceAssemblyResolver.buildManager = new BuildManager("ExpressionBlend");
					}
					ProjectRootElement projectRootElement = ProjectRootElement.Create(ReferenceAssemblyResolver.projectCollection);
					projectRootElement.DefaultTargets = "Build";
					ProjectPropertyGroupElement projectPropertyGroupElement = projectRootElement.AddPropertyGroup();
					projectPropertyGroupElement.AddProperty("OutputPath", "$(TEMP)\\Expression_Blend_CAC0CD0D_3298_4A3D_9EC0_DBFD8306D79F");
					projectPropertyGroupElement.AddProperty("TargetFrameworkIdentifier", this.targetFramework.Identifier);
					projectPropertyGroupElement.AddProperty("TargetFrameworkVersion", string.Concat("v", this.targetFramework.Version));
					projectPropertyGroupElement.AddProperty("TargetFrameworkProfile", this.targetFramework.Profile);
					if (this.targetFramework.Identifier == "Silverlight")
					{
						projectPropertyGroupElement.AddProperty("FrameworkRegistryBase", "Software\\Microsoft\\Microsoft SDKs\\Silverlight");
					}
					projectPropertyGroupElement.AddProperty("ProcessorArchitecture", "msil");
					projectRootElement.AddImport("$(MSBuildToolsPath)\\Microsoft.Common.targets");
					this.project = new Microsoft.Build.Evaluation.Project(projectRootElement)
					{
						IsBuildEnabled = true
					};
				}
				return this.project;
			}
		}

		static ReferenceAssemblyResolver()
		{
			ReferenceAssemblyResolver.syncLock = new object();
		}

		public ReferenceAssemblyResolver(FrameworkName targetFramework)
		{
			this.targetFramework = targetFramework;
		}

		private bool IsValidItem(ProjectItemInstance outputItem)
		{
			Version version = null;
			Version version1 = null;
			if (Version.TryParse(outputItem.GetMetadataValue("Version"), out version) && Version.TryParse(outputItem.GetMetadataValue("HighestVersionInRedist"), out version1) && version > version1)
			{
				return false;
			}
			if (!string.IsNullOrEmpty(outputItem.GetMetadataValue("OutOfRangeDependencies")))
			{
				return false;
			}
			return true;
		}

		public IDictionary<string, string> ResolveAssemblyPaths(string[] assemblySpecs)
		{
			IDictionary<string, string> strs = new Dictionary<string, string>();
			string str = "DesignTimeResolveAssemblyReferences";
			string str1 = "DesignTimeReferencePath";
			ProjectInstance projectInstance = this.Project.CreateProjectInstance();
			StringBuilder stringBuilder = new StringBuilder();
			string[] strArrays = assemblySpecs;
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				stringBuilder.Append(strArrays[i]);
				stringBuilder.Append(";");
			}
			projectInstance.SetProperty("DesignTimeReference", stringBuilder.ToString());
			projectInstance.SetProperty("DesignTimeFindDependencies", "false");
			projectInstance.SetProperty("DesignTimeSilentResolution", "true");
			projectInstance.SetProperty("_FullFrameworkReferenceAssemblyPaths", string.Empty);
			projectInstance.SetProperty("_TargetFrameworkDirectories", string.Empty);
			projectInstance.SetProperty("TargetFrameworkMonikerDisplayName", string.Empty);
			projectInstance.SetProperty("FrameworkPathOverride", string.Empty);
			lock (ReferenceAssemblyResolver.syncLock)
			{
				BuildParameters buildParameter = new BuildParameters(ReferenceAssemblyResolver.projectCollection)
				{
					OnlyLogCriticalEvents = true,
					MaxNodeCount = 1,
					SaveOperatingEnvironment = false
				};
				string[] strArrays1 = new string[] { str };
				BuildRequestData buildRequestDatum = new BuildRequestData(projectInstance, strArrays1, null);
				ReferenceAssemblyResolver.buildManager.Build(buildParameter, buildRequestDatum);
			}
			foreach (ProjectItemInstance item in projectInstance.GetItems(str1))
			{
				if (!this.IsValidItem(item))
				{
					continue;
				}
				strs.Add(item.GetMetadataValue("OriginalItemSpec"), item.EvaluatedInclude);
			}
			string str2 = Path.Combine(Path.GetTempPath(), "Expression_Blend_CAC0CD0D_3298_4A3D_9EC0_DBFD8306D79F");
			if (!string.IsNullOrEmpty(str2) && Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(str2))
			{
				try
				{
					Directory.Delete(str2, true);
				}
				catch (IOException oException)
				{
				}
				catch (UnauthorizedAccessException unauthorizedAccessException)
				{
				}
			}
			return strs;
		}
	}
}