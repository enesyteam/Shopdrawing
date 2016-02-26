using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Project.Build;
using Microsoft.Expression.Project.UserInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project
{
	internal abstract class SingleProjectSolution : SolutionBase
	{
		private SingleProjectSolution.TemporaryConfigurationService configurationService;

		private Microsoft.Expression.Project.SolutionSettingsManager solutionSettingsManager;

		public override IProjectBuildContext ProjectBuildContext
		{
			get
			{
				MSBuildBasedProject mSBuildBasedProject = base.Projects.OfType<MSBuildBasedProject>().FirstOrDefault<MSBuildBasedProject>();
				if (mSBuildBasedProject == null)
				{
					return null;
				}
				return mSBuildBasedProject.ProjectBuildContext;
			}
		}

		public override Microsoft.Expression.Project.SolutionSettingsManager SolutionSettingsManager
		{
			get
			{
				return this.solutionSettingsManager;
			}
		}

		internal SingleProjectSolution(IServiceProvider serviceProvider, Microsoft.Expression.Framework.Documents.DocumentReference documentReference) : base(serviceProvider, documentReference)
		{
			this.configurationService = new SingleProjectSolution.TemporaryConfigurationService();
			this.solutionSettingsManager = new Microsoft.Expression.Project.SolutionSettingsManager(this, this.configurationService["BaseSettings"]);
		}

		public override void CheckForChangedOrDeletedItems()
		{
		}

		protected override bool LoadInternal()
		{
			IProjectStore projectStore = ProjectStoreHelper.CreateProjectStore(base.DocumentReference, base.Services, ProjectStoreHelper.ResilientProjectCreationChain);
			if (projectStore == null)
			{
				return false;
			}
			if (!(new UpgradeWizard(this, EnumerableExtensions.AsEnumerable<IProjectStore>(projectStore), null, () => {
				projectStore.Dispose();
				projectStore = ProjectStoreHelper.CreateProjectStore(this.DocumentReference, this.Services, ProjectStoreHelper.DefaultProjectCreationChain);
			}, base.Services)).Upgrade())
			{
				return false;
			}
			if (projectStore is MigratingMSBuildStore)
			{
				projectStore.Dispose();
				projectStore = ProjectStoreHelper.CreateProjectStore(base.DocumentReference, base.Services, ProjectStoreHelper.DefaultProjectCreationChain);
			}
			INamedProject namedProject = null;
			try
			{
				namedProject = base.OpenProject(projectStore);
				if (namedProject != null)
				{
					base.OpenInitialScene();
				}
			}
			finally
			{
				if (namedProject == null && projectStore != null)
				{
					projectStore.Dispose();
				}
			}
			return namedProject != null;
		}

		private class TemporaryConfigurationService : ConfigurationServiceBase
		{
			public TemporaryConfigurationService()
			{
			}

			public override void Load()
			{
			}

			public override void Save()
			{
			}
		}
	}
}