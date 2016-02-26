using Microsoft.Expression.Project;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.ServiceExtensions.Solution
{
	public static class SolutionServiceExtensions
	{
		public static string FormatWithSolutionTypeSubtext(this IServiceProvider source, string template)
		{
			string str;
			ISolution solution = source.Solution();
			str = (solution != null ? solution.SolutionTypeDescription : StringTable.ProjectSolutionTypeDescription);
			CultureInfo currentCulture = CultureInfo.CurrentCulture;
			object[] objArray = new object[] { str };
			return string.Format(currentCulture, template, objArray);
		}

		public static ISolution Solution(this IServiceProvider source)
		{
			IProjectManager projectManager = source.ProjectManager();
			if (projectManager == null)
			{
				return null;
			}
			return projectManager.CurrentSolution;
		}

		internal static ISolutionManagement SolutionManagement(this IServiceProvider source)
		{
			return source.Solution() as ISolutionManagement;
		}
	}
}