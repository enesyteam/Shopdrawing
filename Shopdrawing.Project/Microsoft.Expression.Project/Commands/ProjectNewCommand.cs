using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Commands
{
	internal sealed class ProjectNewCommand : BaseNewProjectCommand
	{
		protected override bool CreateNewSolution
		{
			get
			{
				return true;
			}
		}

		public override string DisplayName
		{
			get
			{
				return StringTable.CommandNewProjectName;
			}
		}

		protected override string NewProjectPath
		{
			get
			{
				return PathHelper.EnsurePathEndsInDirectorySeparator(this.ProjectManager().DefaultNewProjectPath);
			}
		}

		public ProjectNewCommand(IServiceProvider serviceProvider, IConfigurationObject configurationObject) : base(serviceProvider, configurationObject)
		{
		}

		public override void Execute()
		{
			this.HandleBasicExceptions(() => {
				if (base.GetNewProjectData() && this.ProjectManager().CloseSolution() && base.CreateNewProject() != null)
				{
					string path = this.Solution().DocumentReference.Path;
					if (PathHelper.DirectoryExists(path) || PathHelper.FileExists(path))
					{
						this.ProjectManager().DefaultNewProjectPath = PathHelper.GetParentDirectory(PathHelper.GetDirectory(path));
					}
				}
			});
		}
	}
}