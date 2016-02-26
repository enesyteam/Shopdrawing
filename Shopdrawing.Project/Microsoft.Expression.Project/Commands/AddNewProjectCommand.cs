using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Commands
{
	internal sealed class AddNewProjectCommand : BaseNewProjectCommand
	{
		protected override bool CreateNewSolution
		{
			get
			{
				return false;
			}
		}

		public override string DisplayName
		{
			get
			{
				return StringTable.CommandAddNewProjectName;
			}
		}

		public override bool IsAvailable
		{
			get
			{
				if (this.Solution() is WebProjectSolution)
				{
					return false;
				}
				return base.IsAvailable;
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

		protected override string NewProjectPath
		{
			get
			{
				return Path.GetDirectoryName(this.Solution().DocumentReference.Path);
			}
		}

		public AddNewProjectCommand(IServiceProvider serviceProvider, IConfigurationObject configurationObject) : base(serviceProvider, configurationObject)
		{
		}

		public override void Execute()
		{
			this.HandleBasicExceptions(() => {
				if (base.GetNewProjectData())
				{
					base.UpdateSourceControl(base.CreateNewProject());
					base.ActivateProjectPane();
				}
			});
		}
	}
}