using Microsoft.Expression.Project;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.ServiceExtensions.Selection
{
	public static class SelectionServiceExtensions
	{
		public static void ExtendSelection(this IServiceProvider source, IDocumentItem item)
		{
			ItemSelectionSet itemSelectionSets = source.SelectionSet();
			if (itemSelectionSets != null)
			{
				itemSelectionSets.ExtendSelection(item);
			}
		}

		public static IEnumerable<IProject> SelectedProjects(this IServiceProvider source)
		{
			ItemSelectionSet itemSelectionSets = source.SelectionSet();
			if (itemSelectionSets == null)
			{
				return Enumerable.Empty<IProject>();
			}
			return itemSelectionSets.SelectedProjects.ToArray<IProject>();
		}

		public static IEnumerable<IDocumentItem> Selection(this IServiceProvider source)
		{
			ItemSelectionSet itemSelectionSets = source.SelectionSet();
			if (itemSelectionSets == null)
			{
				return Enumerable.Empty<IDocumentItem>();
			}
			return itemSelectionSets.Selection.ToArray<IDocumentItem>();
		}

		private static ItemSelectionSet SelectionSet(this IServiceProvider source)
		{
			IProjectManager projectManager = source.ProjectManager();
			if (projectManager == null)
			{
				return null;
			}
			return projectManager.ItemSelectionSet;
		}

		public static void SetSelection(this IServiceProvider source, IDocumentItem item)
		{
			ItemSelectionSet itemSelectionSets = source.SelectionSet();
			if (itemSelectionSets != null)
			{
				itemSelectionSets.SetSelection(item);
			}
		}
	}
}