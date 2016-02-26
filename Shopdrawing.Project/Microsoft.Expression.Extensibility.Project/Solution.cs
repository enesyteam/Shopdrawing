using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.Build;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Extensibility.Project
{
	internal class Solution : Microsoft.Expression.Extensibility.Project.ISolution
	{
		private ISolutionManagement solution;

		private IProjectManager projectManager;

		private Dictionary<Microsoft.Expression.Project.IProject, Microsoft.Expression.Extensibility.Project.IProject> projectCache;

		public Microsoft.Expression.Extensibility.Project.IProject ActiveProject
		{
			get
			{
				return this.GetProject(this.projectManager.ItemSelectionSet.SelectedProjects.SingleOrNull<Microsoft.Expression.Project.IProject>());
			}
		}

		internal ISolutionManagement InternalSolution
		{
			get
			{
				return this.solution;
			}
		}

		public bool IsBuilding
		{
			get
			{
				if (!this.IsValid)
				{
					return false;
				}
				if (this.solution.ProjectBuildContext != null)
				{
					return BuildManager.Building;
				}
				return false;
			}
		}

		public bool IsValid
		{
			get
			{
				return this.solution != null;
			}
		}

		public IEnumerable<Microsoft.Expression.Extensibility.Project.IProject> Projects
		{
			get
			{
				if (!this.IsValid)
				{
					return null;
				}
				this.EnsureProjectsCache();
				return this.projectCache.Values;
			}
		}

		public Microsoft.Expression.Extensibility.Project.IProject StartupProject
		{
			get
			{
				if (!this.IsValid)
				{
					return null;
				}
				Microsoft.Expression.Project.IProject startupProject = this.solution.StartupProject as Microsoft.Expression.Project.IProject;
				if (startupProject == null)
				{
					return null;
				}
				return this.GetProject(startupProject);
			}
		}

		internal Solution(ISolutionManagement solution, IProjectManager projectManager)
		{
			this.solution = solution;
			this.projectManager = projectManager;
			this.projectCache = new Dictionary<Microsoft.Expression.Project.IProject, Microsoft.Expression.Extensibility.Project.IProject>();
		}

		public void Build()
		{
			if (!this.IsValid)
			{
				return;
			}
			IExecutable startupProject = this.solution.StartupProject;
			if (startupProject == null || !startupProject.IsExecuting)
			{
				this.solution.Save(false);
				IProjectBuildContext projectBuildContext = this.solution.ProjectBuildContext;
				this.projectManager.BuildManager.Build(projectBuildContext, startupProject, false);
			}
		}

		public void Close()
		{
			foreach (Microsoft.Expression.Project.IProject key in this.projectCache.Keys)
			{
				this.OnProjectClosing(key, false);
			}
			this.projectCache.Clear();
			this.solution = null;
		}

		internal void EnsureProjectsCache()
		{
			foreach (INamedProject allProject in this.solution.AllProjects)
			{
				this.GetProject(allProject as Microsoft.Expression.Project.IProject);
			}
		}

		internal Microsoft.Expression.Extensibility.Project.IProject GetProject(Microsoft.Expression.Project.IProject project)
		{
			if (project == null)
			{
				return null;
			}
			Microsoft.Expression.Extensibility.Project.IProject project1 = null;
			if (!this.projectCache.TryGetValue(project, out project1))
			{
				project1 = new Microsoft.Expression.Extensibility.Project.Project(project);
				this.projectCache[project] = project1;
			}
			return project1;
		}

		public void OnProjectClosing(Microsoft.Expression.Project.IProject closingProject, bool removeFromCache)
		{
			foreach (Microsoft.Expression.Extensibility.Project.IProject value in this.projectCache.Values)
			{
				Microsoft.Expression.Extensibility.Project.Project project = (Microsoft.Expression.Extensibility.Project.Project)value;
				if (!project.InternalProject.Equals(closingProject))
				{
					continue;
				}
				project.Close();
				break;
			}
			if (removeFromCache)
			{
				this.projectCache.Remove(closingProject);
			}
		}
	}
}