using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Project
{
	public interface IProjectStore : IDisposable
	{
		Microsoft.Expression.Framework.Documents.DocumentReference DocumentReference
		{
			get;
		}

		IEnumerable<string> ProjectImports
		{
			get;
		}

		Version StoreVersion
		{
			get;
		}

		bool AddImport(string value);

		IProjectItemData AddItem(string itemType, string itemValue);

		bool ChangeImport(string oldImportValue, string newImportValue);

		IEnumerable<IProjectItemData> GetItems(string itemType);

		string GetProperty(string name);

		bool IsPropertyWritable(string name);

		bool RemoveItem(IProjectItemData item);

		void Save();

		bool SetProperty(string name, string value);

		bool SetStoreVersion(Version version);

		bool SetUnpersistedProperty(string name, string value);
	}
}