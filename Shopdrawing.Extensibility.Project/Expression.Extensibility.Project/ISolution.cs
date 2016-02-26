using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Extensibility.Project
{
	public interface ISolution
	{
		IProject ActiveProject
		{
			get;
		}

		bool IsBuilding
		{
			get;
		}

		IEnumerable<IProject> Projects
		{
			get;
		}

		IProject StartupProject
		{
			get;
		}

		void Build();
	}
}