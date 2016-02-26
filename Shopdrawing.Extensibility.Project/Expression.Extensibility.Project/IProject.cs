using System;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace Microsoft.Expression.Extensibility.Project
{
	public interface IProject
	{
		IEnumerable<IProjectItem> Items
		{
			get;
		}

		string Path
		{
			get;
		}

		IProjectStore ProjectStore
		{
			get;
		}

		FrameworkName TargetFramework
		{
			get;
		}

		IProjectItem AddItem(string documentItemPath, string itemType);

		void AddReference(string reference);
	}
}