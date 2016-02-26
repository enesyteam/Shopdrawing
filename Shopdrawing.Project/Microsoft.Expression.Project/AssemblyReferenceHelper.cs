using Microsoft.Build.Execution;
using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project
{
	internal static class AssemblyReferenceHelper
	{
		public const string ReferenceItemType = "Reference";

		public const string ProjectReferenceItemType = "ProjectReference";

		public const string ComReferenceItemType = "COMReference";

		internal static IEnumerable<ItemResolutionPair> GetAssemblyPaths(IProjectStore projectStore, ProjectBuildContext buildContext)
		{
			IEnumerable<IProjectItemData> projectItemDatas = projectStore.GetItems("Reference").Concat<IProjectItemData>(projectStore.GetItems("ProjectReference").Concat<IProjectItemData>(projectStore.GetItems("COMReference")));
			IEnumerable<ProjectItemInstance> referencePaths = AssemblyReferenceHelper.GetReferencePaths(buildContext);
			IEnumerable<ItemResolutionPair> value = 
				from assemblyReference in projectItemDatas
				join resolvedAssemblyReference in referencePaths on assemblyReference.Value equals resolvedAssemblyReference.GetMetadata("OriginalItemSpec").EvaluatedValue into groupJoin
				from resolvedReference in groupJoin.DefaultIfEmpty<ProjectItemInstance>()
				select new ItemResolutionPair(assemblyReference, resolvedReference);
			return value;
		}

		private static IEnumerable<ProjectItemInstance> GetReferencePaths(ProjectBuildContext buildContext)
		{
			IEnumerable<ProjectItemInstance> items = buildContext.ProjectInstance.GetItems("ReferencePath");
			if (items.Any<ProjectItemInstance>())
			{
				return items;
			}
			AssemblyReferenceHelper.RebuildAssemblyReferencePaths(buildContext);
			return buildContext.ProjectInstance.GetItems("ReferencePath");
		}

		private static void RebuildAssemblyReferencePaths(ProjectBuildContext buildContext)
		{
			string[] strArrays = new string[] { "ResolveComReferences", "ResolveAssemblyReferences" };
			buildContext.Build(strArrays);
		}

		internal static void RepairAssemblyReferences(IProjectStore projectStore)
		{
			MSBuildBasedProjectStore mSBuildBasedProjectStore = projectStore as MSBuildBasedProjectStore;
			if (mSBuildBasedProjectStore == null)
			{
				return;
			}
			ProjectBuildContext projectBuildContext = mSBuildBasedProjectStore.GenerateNewBuildContext(null);
			AssemblyReferenceHelper.RebuildAssemblyReferencePaths(projectBuildContext);
			foreach (ItemResolutionPair itemResolutionPair in 
				from assemblyPath in AssemblyReferenceHelper.GetAssemblyPaths(projectStore, projectBuildContext)
				where assemblyPath.ResolvedItem == null
				select assemblyPath)
			{
				string value = itemResolutionPair.SourceItem.Value;
				if (string.IsNullOrEmpty(value))
				{
					continue;
				}
				int num = value.IndexOf(',');
				string fileNameWithoutExtension = null;
				if (num <= 0)
				{
					try
					{
						fileNameWithoutExtension = Path.GetFileNameWithoutExtension(value);
					}
					catch (IOException oException)
					{
						continue;
					}
					catch (ArgumentException argumentException)
					{
						continue;
					}
					if (string.Equals(fileNameWithoutExtension, value, StringComparison.OrdinalIgnoreCase))
					{
						continue;
					}
				}
				else
				{
					fileNameWithoutExtension = value.Substring(0, num);
				}
				itemResolutionPair.SourceItem.Value = fileNameWithoutExtension;
				itemResolutionPair.SourceItem.SetItemMetadata("UpgraderOriginalValue", value);
			}
			projectBuildContext = mSBuildBasedProjectStore.GenerateNewBuildContext(null);
			AssemblyReferenceHelper.RebuildAssemblyReferencePaths(projectBuildContext);
			foreach (ItemResolutionPair itemResolutionPair1 in AssemblyReferenceHelper.GetAssemblyPaths(projectStore, projectBuildContext))
			{
				string metadata = itemResolutionPair1.SourceItem.GetMetadata("UpgraderOriginalValue");
				if (string.IsNullOrEmpty(metadata))
				{
					continue;
				}
				if (itemResolutionPair1.ResolvedItem != null)
				{
					string path = projectStore.DocumentReference.Path;
					string updateItemMetadataAction = StringTable.UpdateItemMetadataAction;
					object[] itemType = new object[] { "Include", itemResolutionPair1.SourceItem.ItemType, metadata, metadata, itemResolutionPair1.SourceItem.Value };
					ProjectLog.LogSuccess(path, updateItemMetadataAction, itemType);
					string metadataValue = itemResolutionPair1.ResolvedItem.GetMetadataValue("RequiredTargetFramework");
					if (!string.IsNullOrEmpty(metadataValue))
					{
						itemResolutionPair1.SourceItem.SetItemMetadata("RequiredTargetFramework", metadataValue);
					}
				}
				else
				{
					itemResolutionPair1.SourceItem.Value = metadata;
				}
				itemResolutionPair1.SourceItem.SetItemMetadata("UpgraderOriginalValue", null);
			}
		}
	}
}