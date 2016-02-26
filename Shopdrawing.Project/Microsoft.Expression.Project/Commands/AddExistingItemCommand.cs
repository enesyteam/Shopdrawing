using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Commands
{
	internal class AddExistingItemCommand : AddProjectItemCommand
	{
		public override string DisplayName
		{
			get
			{
				return StringTable.CommandAddExistingItemName;
			}
		}

		protected virtual CreationOptions Options
		{
			get
			{
				return CreationOptions.None;
			}
		}

		public AddExistingItemCommand(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		protected override bool CreateProjectItem()
		{
			string[] filesToImport = this.GetFilesToImport(base.GetImportFolder());
			if (filesToImport != null && (int)filesToImport.Length > 0)
			{
				IProject project = this.SelectedProjectOrNull();
				if (project != null)
				{
					IEnumerable<IProjectItem> projectItems = project.AddItems(
						from file in filesToImport
						select new DocumentCreationInfo()
						{
							SourcePath = file,
							CreationOptions = this.Options
						});
					if (projectItems == null)
					{
						return false;
					}
					return projectItems.CountIsMoreThan<IProjectItem>(0);
				}
			}
			return false;
		}
	}
}