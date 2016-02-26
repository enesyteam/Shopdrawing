using Microsoft.Expression.Project.Build;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Expression.Project
{
	internal interface ISolutionManagement : ISolution, IDocumentItem, IDisposable, INotifyPropertyChanged
	{
		IEnumerable<INamedProject> AllProjects
		{
			get;
		}

		IExecutable StartupProject
		{
			get;
			set;
		}

		INamedProject AddProject(IProjectStore projectStore);

		void AddProjectOutputReferences(IEnumerable<INamedProject> createdProjects);

		void CheckDelayedChange();

		void Close(bool isUserInitiated);

		void CloseAllProjects();

		void CloseProject(INamedProject project);

		void DeactivateWatchers();

		bool Load();

		void OpenInitialViews();

		void ReactivateWatchers();

		bool Save(bool promptBeforeSaving, bool saveActiveDocument);

		void SaveCopy(string newRootPath, string newSolutionName);

		event EventHandler<NamedProjectEventArgs> AnyProjectClosed;

		event EventHandler<NamedProjectEventArgs> AnyProjectOpened;
	}
}