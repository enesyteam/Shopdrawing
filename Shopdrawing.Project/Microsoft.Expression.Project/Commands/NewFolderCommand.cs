using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.ServiceExtensions;
using Microsoft.Expression.Project.ServiceExtensions.Documents;
using Microsoft.Expression.Project.ServiceExtensions.Selection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.Expression.Project.Commands
{
	internal sealed class NewFolderCommand : ProjectItemCreationCommand
	{
		public override string DisplayName
		{
			get
			{
				return StringTable.CommandNewFolderName;
			}
		}

		public NewFolderCommand(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		protected override bool CreateProjectItem()
		{
			IProjectItem projectItem;
			IProject project = this.SelectedProjectOrNull();
			if (project == null)
			{
				return false;
			}
			IDocumentType item = base.Services.DocumentTypes()[DocumentTypeNamesHelper.Folder];
			string str = this.ProjectManager().TargetFolderForProject(project);
			string availableFilePath = ProjectPathHelper.GetAvailableFilePath(item.DefaultFileName, str, project);
			Directory.CreateDirectory(availableFilePath);
			try
			{
				List<DocumentCreationInfo> documentCreationInfos = new List<DocumentCreationInfo>();
				DocumentCreationInfo documentCreationInfo = new DocumentCreationInfo()
				{
					DocumentType = item,
					TargetPath = availableFilePath
				};
				documentCreationInfos.Add(documentCreationInfo);
				projectItem = project.AddItems(documentCreationInfos).FirstOrDefault<IProjectItem>();
			}
			catch
			{
				Directory.Delete(availableFilePath);
				throw;
			}
			if (projectItem != null)
			{
				base.Services.SetSelection(projectItem);
				base.Services.CommandService().Execute("Project_RenameProjectItem", CommandInvocationSource.Internally);
			}
			return projectItem != null;
		}
	}
}