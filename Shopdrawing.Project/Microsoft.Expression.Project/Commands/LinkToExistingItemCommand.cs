using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Commands
{
	internal class LinkToExistingItemCommand : AddProjectItemCommand
	{
		public override string DisplayName
		{
			get
			{
				return StringTable.CommandLinkToExistingItemName;
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
				IProject project = this.SelectedProjects().SingleOrNull<IProject>();
				if (!base.IsEnabled || project == null)
				{
					return false;
				}
				return project.GetCapability<bool>("CanAddAssemblyReference");
			}
		}

		public LinkToExistingItemCommand(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		protected override bool CreateProjectItem()
		{
			bool flag;
			string[] filesToImport = this.GetFilesToImport(base.GetImportFolder());
			if (filesToImport == null || (int)filesToImport.Length <= 0)
			{
				return false;
			}
			IProject project = this.SelectedProjects().SingleOrNull<IProject>();
			string[] strArrays = filesToImport;
			int num = 0;
			while (true)
			{
				if (num >= (int)strArrays.Length)
				{
					IEnumerable<IProjectItem> projectItems = project.AddItems(
						from file in filesToImport
						select new DocumentCreationInfo()
						{
							SourcePath = file,
							CreationOptions = CreationOptions.LinkSourceFile
						});
					if (projectItems == null)
					{
						return false;
					}
					return projectItems.CountIsMoreThan<IProjectItem>(0);
				}
				string str = strArrays[num];
				string fileName = Path.GetFileName(str);
				if (project.Items.FindMatchByUrl<IProjectItem>(str) != null)
				{
					CultureInfo currentCulture = CultureInfo.CurrentCulture;
					string linkToExistingItemCommandLinkedFileExistsDialogMessage = StringTable.LinkToExistingItemCommandLinkedFileExistsDialogMessage;
					object[] objArray = new object[] { fileName };
					this.DisplayCommandFailedMessage(string.Format(currentCulture, linkToExistingItemCommandLinkedFileExistsDialogMessage, objArray));
					flag = false;
					break;
				}
				else if (project.Items.FindMatchByUrl<IProjectItem>(Path.Combine(this.ProjectManager().TargetFolderForProject(project), fileName)) == null)
				{
					num++;
				}
				else
				{
					CultureInfo cultureInfo = CultureInfo.CurrentCulture;
					string linkToExistingItemCommandFileExistsDialogMessage = StringTable.LinkToExistingItemCommandFileExistsDialogMessage;
					object[] objArray1 = new object[] { fileName };
					this.DisplayCommandFailedMessage(string.Format(cultureInfo, linkToExistingItemCommandFileExistsDialogMessage, objArray1));
					flag = false;
					break;
				}
			}
			return flag;
		}
	}
}