using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project
{
	internal class MSBuildProjectItemData : IProjectItemData
	{
		public string ItemType
		{
			get
			{
				return this.ProjectItem.Xml.ItemType;
			}
		}

		public Microsoft.Build.Evaluation.ProjectItem ProjectItem
		{
			get;
			private set;
		}

		public string Value
		{
			get
			{
				return this.ProjectItem.EvaluatedInclude;
			}
			set
			{
				this.ProjectItem.UnevaluatedInclude = value;
			}
		}

		public MSBuildProjectItemData(Microsoft.Build.Evaluation.ProjectItem projectItem)
		{
			this.ProjectItem = projectItem;
		}

		public string GetMetadata(string metadataName)
		{
			return this.ProjectItem.GetMetadataValue(metadataName);
		}

		public bool SetItemMetadata(string metadataName, string metadataValue)
		{
			bool flag;
			string str;
			string metadata = this.GetMetadata(metadataName);
			if (string.Equals(metadata, metadataValue))
			{
				return true;
			}
			flag = (metadataValue != null ? this.ProjectItem.SetMetadataValue(metadataName, metadataValue) != null : this.ProjectItem.RemoveMetadata(metadataName));
			if (!string.IsNullOrEmpty(metadata))
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string updateItemMetadataAction = StringTable.UpdateItemMetadataAction;
				object[] objArray = new object[] { metadataName, this.ItemType, this.Value, metadata, metadataValue };
				str = string.Format(currentCulture, updateItemMetadataAction, objArray);
			}
			else
			{
				CultureInfo cultureInfo = CultureInfo.CurrentCulture;
				string setItemMetadataAction = StringTable.SetItemMetadataAction;
				object[] objArray1 = new object[] { metadataName, this.ItemType, this.Value, metadataValue };
				str = string.Format(cultureInfo, setItemMetadataAction, objArray1);
			}
			if (!flag)
			{
				ProjectLog.LogError(this.ProjectItem.Project.FullPath, StringTable.UnknownError, str, new object[0]);
			}
			else
			{
				ProjectLog.LogSuccess(this.ProjectItem.Project.FullPath, str, new object[0]);
			}
			return flag;
		}
	}
}