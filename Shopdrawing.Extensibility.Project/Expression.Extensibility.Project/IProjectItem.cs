using System;

namespace Microsoft.Expression.Extensibility.Project
{
	public interface IProjectItem
	{
		string FullPath
		{
			get;
		}

		string Name
		{
			get;
		}
	}
}