using Microsoft.Expression.Project;
using System;
using System.ComponentModel.Composition;

namespace Microsoft.Expression.Extensibility.Project
{
	[Export(typeof(ISolutionService))]
	internal class SolutionService : ISolutionService
	{
		private IProjectManager projectManager;

		private Microsoft.Expression.Extensibility.Project.ISolution solution;

		public Microsoft.Expression.Extensibility.Project.ISolution Solution
		{
			get
			{
				if (this.solution == null)
				{
					this.solution = this.MakeSolutionForExtension();
				}
				return this.solution;
			}
		}

		public SolutionService(IProjectManager projectManager)
		{
			this.projectManager = projectManager;
		}

		private Microsoft.Expression.Extensibility.Project.ISolution MakeSolutionForExtension()
		{
			ISolutionManagement currentSolution = this.projectManager.CurrentSolution as ISolutionManagement;
			if (currentSolution != null)
			{
				this.solution = new Microsoft.Expression.Extensibility.Project.Solution(currentSolution, this.projectManager);
				this.projectManager.SolutionClosing += new EventHandler<SolutionEventArgs>(this.projectManager_SolutionClosing);
				this.projectManager.ProjectClosing += new EventHandler<ProjectEventArgs>(this.projectManager_ProjectClosing);
			}
			return this.solution;
		}

		private void projectManager_ProjectClosing(object sender, ProjectEventArgs e)
		{
			Microsoft.Expression.Extensibility.Project.Solution solution = this.Solution as Microsoft.Expression.Extensibility.Project.Solution;
			if (solution != null)
			{
				solution.OnProjectClosing(e.Project, true);
			}
		}

		private void projectManager_SolutionClosing(object sender, SolutionEventArgs e)
		{
			if (this.solution != null)
			{
				((Microsoft.Expression.Extensibility.Project.Solution)this.solution).Close();
				this.solution = null;
			}
			this.projectManager.SolutionClosing -= new EventHandler<SolutionEventArgs>(this.projectManager_SolutionClosing);
			this.projectManager.ProjectClosing -= new EventHandler<ProjectEventArgs>(this.projectManager_ProjectClosing);
		}
	}
}