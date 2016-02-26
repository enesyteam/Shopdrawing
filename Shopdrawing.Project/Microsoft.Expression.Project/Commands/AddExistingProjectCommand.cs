using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.UserInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Commands
{
	internal sealed class AddExistingProjectCommand : BaseOpenCommand
	{
		public override string DisplayName
		{
			get
			{
				return StringTable.CommandAddExistingProjectName;
			}
		}

		public override bool IsAvailable
		{
			get
			{
				if (!base.IsAvailable)
				{
					return false;
				}
				return !(this.Solution() is WebProjectSolution);
			}
		}

		public override bool IsEnabled
		{
			get
			{
				ISolution solution = this.Solution();
				if (!base.IsEnabled || solution == null)
				{
					return false;
				}
				return solution.CanAddProjects;
			}
		}

		public AddExistingProjectCommand(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		public override void Execute()
		{
			this.HandleBasicExceptions(() => {
				string str = base.SelectProject(this.DisplayName);
				if (!string.IsNullOrEmpty(str))
				{
					IProjectStore projectStore = ProjectStoreHelper.CreateProjectStore(DocumentReference.Create(str), base.Services, ProjectStoreHelper.ResilientProjectCreationChain);
					if (projectStore == null)
					{
						return;
					}
					UpgradeWizard upgradeWizard = new UpgradeWizard(this.Solution() as ISolutionManagement, 
						from project in this.Solution().Projects.OfType<ProjectBase>()
						select project.ProjectStore, projectStore, null, base.Services);
					try
					{
						if (!upgradeWizard.Upgrade())
						{
							return;
						}
					}
					finally
					{
						projectStore.Dispose();
						projectStore = null;
					}
					projectStore = ProjectStoreHelper.CreateProjectStore(DocumentReference.Create(str), base.Services, ProjectStoreHelper.DefaultProjectCreationChain);
					if (projectStore == null)
					{
						return;
					}
					INamedProject namedProject = null;
					try
					{
						namedProject = this.ProjectManager().AddProject(projectStore);
						if (namedProject != null)
						{
							base.UpdateSourceControl(EnumerableExtensions.AsEnumerable<INamedProject>(namedProject));
							base.ActivateProjectPane();
						}
					}
					finally
					{
						if (namedProject == null && projectStore != null)
						{
							projectStore.Dispose();
							projectStore = null;
						}
					}
				}
			});
		}
	}
}