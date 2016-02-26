using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.ServiceExtensions.Documents;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.Expression.Project.Commands
{
	internal class AddExistingItemOfTypeCommand : AddProjectItemCommand
	{
		private IDocumentType[] documentTypes;

		private IProject project;

		private IEnumerable<IProjectItem> addedProjectItems;

		private bool selectAddedItems = true;

		private string targetFolder;

		public override string DisplayName
		{
			get
			{
				return StringTable.CommandAddExistingItemName;
			}
		}

		public AddExistingItemOfTypeCommand(IServiceProvider serviceProvider, IDocumentType[] documentTypes) : base(serviceProvider)
		{
			this.documentTypes = documentTypes;
		}

		protected override bool CreateProjectItem()
		{
			this.addedProjectItems = null;
			string[] filesToImport = this.GetFilesToImport(base.GetImportFolder());
			if (filesToImport != null && (int)filesToImport.Length > 0 && this.project != null)
			{
				CreationOptions creationOption = (this.selectAddedItems ? CreationOptions.DoNotSelectCreatedItems : CreationOptions.None);
				this.addedProjectItems = this.project.AddItems(
					from file in filesToImport
					select new DocumentCreationInfo()
					{
						SourcePath = file,
						TargetFolder = this.targetFolder,
						CreationOptions = creationOption
					});
			}
			this.project = null;
			if (this.addedProjectItems == null)
			{
				return false;
			}
			return this.addedProjectItems.CountIsMoreThan<IProjectItem>(0);
		}

		protected override AddProjectItemCommand.FileTypeDescription[] GetFileTypeFilter()
		{
			List<AddProjectItemCommand.FileTypeDescription> fileTypeDescriptions = new List<AddProjectItemCommand.FileTypeDescription>();
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			if (this.documentTypes == null)
			{
				this.documentTypes = base.Services.DocumentTypes().ToArray<IDocumentType>();
			}
			IDocumentType[] documentTypeArray = this.documentTypes;
			for (int i = 0; i < (int)documentTypeArray.Length; i++)
			{
				IDocumentType documentType = documentTypeArray[i];
				if (documentType.CanAddToProject(this.project))
				{
					string str = "";
					string str1 = "";
					string[] fileExtensions = documentType.FileExtensions;
					for (int j = 0; j < (int)fileExtensions.Length; j++)
					{
						string str2 = fileExtensions[j];
						if (str.Length > 0)
						{
							str = string.Concat(str, ";");
							str1 = string.Concat(str1, ", ");
						}
						string str3 = string.Concat("*", str2);
						str = string.Concat(str, str3);
						str1 = string.Concat(str1, str3);
					}
					if (num > 0)
					{
						stringBuilder.Append(";");
					}
					num++;
					stringBuilder.Append(str);
					CultureInfo currentCulture = CultureInfo.CurrentCulture;
					string addProjectItemCommandFileFilterTextFormat = StringTable.AddProjectItemCommandFileFilterTextFormat;
					object[] description = new object[] { documentType.Description, str1 };
					fileTypeDescriptions.Add(new AddProjectItemCommand.FileTypeDescription(string.Format(currentCulture, addProjectItemCommandFileFilterTextFormat, description), str));
				}
			}
			if (num > 1)
			{
				string str4 = stringBuilder.ToString();
				fileTypeDescriptions.Insert(0, new AddProjectItemCommand.FileTypeDescription(StringTable.AddProjectItemCommandAllAssetFiles, str4));
			}
			return fileTypeDescriptions.ToArray();
		}

		public override object GetProperty(string propertyName)
		{
			if (propertyName == "AddedProjectItems")
			{
				return this.addedProjectItems;
			}
			if (propertyName == "Project")
			{
				return this.project;
			}
			if (propertyName == "TargetFolder")
			{
				return this.targetFolder;
			}
			if (propertyName == "SelectAddedItems")
			{
				return this.selectAddedItems;
			}
			return base.GetProperty(propertyName);
		}

		public override void SetProperty(string propertyName, object propertyValue)
		{
			base.SetProperty(propertyName, propertyValue);
			if (propertyName == "Project")
			{
				this.project = propertyValue as IProject;
				return;
			}
			if (propertyName != "DocumentTypes")
			{
				if (propertyName == "TargetFolder")
				{
					this.targetFolder = (string)propertyValue;
					return;
				}
				if (propertyName == "SelectAddedItems")
				{
					this.selectAddedItems = (bool)propertyValue;
				}
			}
			else
			{
				IDocumentType[] documentTypeArray = propertyValue as IDocumentType[];
				if (documentTypeArray != null)
				{
					this.documentTypes = documentTypeArray;
					return;
				}
			}
		}
	}
}