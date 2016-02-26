using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project.Build;
using Microsoft.Expression.Project.UserInterface;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Expression.Project
{
	public interface IProjectManager
	{
		IProjectBuildContext ActiveBuildTarget
		{
			get;
		}

		Microsoft.Expression.Project.Build.BuildManager BuildManager
		{
			get;
		}

		ISolution CurrentSolution
		{
			get;
		}

		ICodeDocumentType DefaultCodeDocumentType
		{
			get;
		}

		string DefaultImportPath
		{
			get;
			set;
		}

		string DefaultNewProjectPath
		{
			get;
			set;
		}

		string DefaultNewSamplePath
		{
			get;
			set;
		}

		string DefaultOpenProjectPath
		{
			get;
			set;
		}

		Microsoft.Expression.Framework.IReadOnlyCollection<Assembly> ImplicitAssemblyReferences
		{
			get;
		}

		Microsoft.Expression.Project.ItemSelectionSet ItemSelectionSet
		{
			get;
		}

		ProjectSystemOptionsModel OptionsModel
		{
			get;
		}

		string[] RecentProjects
		{
			get;
		}

		ITemplateManager TemplateManager
		{
			get;
		}

		void AddImplicitAssemblyReference(Assembly assembly, string originalAssemblyLocation);

		INamedProject AddProject(IProjectStore projectStore);

		bool CloseSolution();

		bool CloseSolution(bool applicationClosing);

		IEnumerable<INamedProject> CreateProjectTemplate(string projectPath, string projectName, IProjectTemplate projectTemplate, IEnumerable<TemplateArgument> templateArguments);

		string DetermineOriginalImplicitAssemblyLocation(Assembly assembly);

		void InitializeFromKnownProjects(string[] args);

		void InitializeRecentProjects();

		ISolution OpenSolution(DocumentReference solutionOrProjectReference, bool addToRecentList, bool openInitialScene);

		string TargetFolderForProject(IProject project);

		event EventHandler ActiveBuildTargetChanged;

		event EventHandler<ProjectEventArgs> ProjectClosed;

		event EventHandler<ProjectEventArgs> ProjectClosing;

		event EventHandler<ProjectEventArgs> ProjectOpened;

		event EventHandler<SolutionEventArgs> SolutionClosed;

		event EventHandler<SolutionEventArgs> SolutionClosing;

		event EventHandler<SolutionEventArgs> SolutionMigrated;

		event EventHandler<SolutionEventArgs> SolutionOpened;

		event EventHandler StartupProjectChanged;
	}
}