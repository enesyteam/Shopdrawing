using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project.Build;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Expression.Project
{
	public interface ISolution : IDocumentItem, IDisposable, INotifyPropertyChanged
	{
		bool CanAddProjects
		{
			get;
		}

		bool IsClosingAllProjects
		{
			get;
		}

		bool IsIssueTrackingAvailable
		{
			get;
		}

		bool IsSourceControlActive
		{
			get;
		}

		bool IsUnderSourceControl
		{
			get;
		}

		IProjectBuildContext ProjectBuildContext
		{
			get;
		}

		IEnumerable<IProject> Projects
		{
			get;
		}

		Microsoft.Expression.Project.SolutionSettingsManager SolutionSettingsManager
		{
			get;
		}

		string SolutionTypeDescription
		{
			get;
		}

		IExecutable StartupProject
		{
			get;
		}

		INamedProject FindProject(Microsoft.Expression.Framework.Documents.DocumentReference projectReference);

		INamedProject FindProjectByName(string projectName);

		IProject FindProjectContainingItem(Microsoft.Expression.Framework.Documents.DocumentReference documentReference);

		IProject FindProjectContainingOpenItem(Microsoft.Expression.Framework.Documents.DocumentReference documentReference);

		bool RefreshProject(IProject project);

		bool Save(bool promptBeforeSaving);
	}
}