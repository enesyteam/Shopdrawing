using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.ServiceExtensions;
using Microsoft.Expression.Project.ServiceExtensions.ErrorHandling;
using Microsoft.Expression.Project.ServiceExtensions.Selection;
using Microsoft.Expression.Project.ServiceExtensions.Solution;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Commands
{
	public static class ProjectCommandExtensions
	{
		private static string CommandErrorMessage(this IProjectCommand source)
		{
			CultureInfo currentCulture = CultureInfo.CurrentCulture;
			string commandFailedDialogMessage = StringTable.CommandFailedDialogMessage;
			object[] displayName = new object[] { source.DisplayName };
			return string.Format(currentCulture, commandFailedDialogMessage, displayName);
		}

		public static void DisplayCommandFailedExceptionMessage(this IProjectCommand source, Exception exception)
		{
			source.DisplayCommandFailedMessage(exception.Message);
		}

		public static void DisplayCommandFailedMessage(this IProjectCommand source, string failureMessage)
		{
			IMessageDisplayService messageDisplayService = source.Services.MessageDisplayService();
			if (messageDisplayService != null)
			{
				messageDisplayService.ShowError(failureMessage);
			}
		}

		public static string FormatWithSolutionTypeSubtext(this IProjectCommand source, string template)
		{
			return source.Services.FormatWithSolutionTypeSubtext(template);
		}

		public static void HandleBasicExceptions(this IProjectCommand source, Action action)
		{
			source.Services.ExceptionHandler(action, new Func<string>(source.CommandErrorMessage));
		}

		public static bool IsWebSolution(this IProjectCommand source)
		{
			return source.Solution() is WebProjectSolution;
		}

		public static IProjectManager ProjectManager(this IProjectCommand source)
		{
			return source.Services.ProjectManager();
		}

		public static bool SaveSolution(this IProjectCommand source, bool promptBeforeSaving)
		{
			ISolutionManagement solutionManagement = source.Services.SolutionManagement();
			if (solutionManagement == null)
			{
				return false;
			}
			return solutionManagement.Save(promptBeforeSaving);
		}

		public static bool SaveSolution(this IProjectCommand source, bool promptBeforeSaving, bool saveActiveDocument)
		{
			ISolutionManagement solutionManagement = source.Services.SolutionManagement();
			if (solutionManagement == null)
			{
				return false;
			}
			return solutionManagement.Save(promptBeforeSaving, saveActiveDocument);
		}

		public static IProjectItem SelectedProjectItemOrNull(this IProjectCommand source)
		{
			return source.Services.Selection().SingleOrNull<IDocumentItem>() as IProjectItem;
		}

		public static IProject SelectedProjectOrNull(this IProjectCommand source)
		{
			return source.Services.SelectedProjects().SingleOrNull<IProject>();
		}

		public static IEnumerable<IProject> SelectedProjects(this IProjectCommand source)
		{
			return source.Services.SelectedProjects();
		}

		public static IEnumerable<IDocumentItem> Selection(this IProjectCommand source)
		{
			return source.Services.Selection();
		}

		public static ISolution Solution(this IProjectCommand source)
		{
			return source.Services.Solution();
		}

		public static IDisposable SuspendWatchers(this IProjectCommand source)
		{
			return new WatcherSuspender(source.Solution() as ISolutionManagement);
		}
	}
}