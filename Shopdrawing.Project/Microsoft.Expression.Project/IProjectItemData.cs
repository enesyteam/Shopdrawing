using System;

namespace Microsoft.Expression.Project
{
	public interface IProjectItemData
	{
		string ItemType
		{
			get;
		}

		string Value
		{
			get;
			set;
		}

		string GetMetadata(string metadataName);

		bool SetItemMetadata(string metadataName, string metadataValue);
	}
}