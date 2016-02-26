using Microsoft.Expression.Framework.SourceControl;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Commands
{
	internal abstract class SourceControlCommand : ProjectCommand
	{
		public override bool IsAvailable
		{
			get
			{
				if (this.Solution() == null)
				{
					return false;
				}
				return this.Solution().IsSourceControlActive;
			}
		}

		protected ISourceControlProvider SourceControlProvider
		{
			get
			{
				return base.Services.SourceControlProvider();
			}
		}

		public SourceControlCommand(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		public sealed override void Execute()
		{
			using (IDisposable disposable = this.SuspendWatchers())
			{
				this.HandleBasicExceptions(() => this.InternalExectute());
			}
		}

		protected abstract void InternalExectute();

		protected bool IsDirectoryBasedProjectOrFolder(IDocumentItem target)
		{
			IProject project = target as IProject;
			if (project == null)
			{
				IProjectItem projectItem = target as IProjectItem;
				if (projectItem != null && projectItem.IsDirectory)
				{
					project = projectItem.Project;
				}
			}
			if (project == null)
			{
				return false;
			}
			return project.IsDirectory;
		}
	}
}