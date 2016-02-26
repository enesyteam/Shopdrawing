using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Extensibility.Project
{
	public interface IProjectStore
	{
		IEnumerable<string> ProjectImports
		{
			get;
		}

		bool AddImport(string value);
	}
}