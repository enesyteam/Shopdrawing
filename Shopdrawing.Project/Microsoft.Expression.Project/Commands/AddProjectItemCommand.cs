using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.ServiceExtensions.Documents;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.Expression.Project.Commands
{
	public abstract class AddProjectItemCommand : ProjectItemCreationCommand
	{
		protected AddProjectItemCommand(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		protected virtual string[] GetFilesToImport(string defaultImportFolder)
		{
			ExpressionOpenFileDialog expressionOpenFileDialog = new ExpressionOpenFileDialog()
			{
				RestoreDirectory = true,
				InitialDirectory = defaultImportFolder,
				Title = this.DisplayName,
				Multiselect = true
			};
			AddProjectItemCommand.FileTypeDescription[] fileTypeFilter = this.GetFileTypeFilter();
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			AddProjectItemCommand.FileTypeDescription[] fileTypeDescriptionArray = fileTypeFilter;
			for (int i = 0; i < (int)fileTypeDescriptionArray.Length; i++)
			{
				AddProjectItemCommand.FileTypeDescription fileTypeDescription = fileTypeDescriptionArray[i];
				if (!flag)
				{
					stringBuilder.Append("|");
				}
				else
				{
					flag = false;
				}
				stringBuilder.Append(fileTypeDescription.Description);
				stringBuilder.Append("|");
				stringBuilder.Append(fileTypeDescription.Extensions);
			}
			expressionOpenFileDialog.Filter = stringBuilder.ToString();
			bool? nullable = expressionOpenFileDialog.ShowDialog();
			if (!nullable.HasValue || !nullable.Value)
			{
				return null;
			}
			return expressionOpenFileDialog.FileNames;
		}

		protected virtual AddProjectItemCommand.FileTypeDescription[] GetFileTypeFilter()
		{
			List<AddProjectItemCommand.FileTypeDescription> fileTypeDescriptions = new List<AddProjectItemCommand.FileTypeDescription>();
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			IProject project = this.SelectedProjects().First<IProject>();
			foreach (IDocumentType documentType in base.Services.DocumentTypes())
			{
				if (!documentType.CanAddToProject(project))
				{
					continue;
				}
				string str = "";
				string str1 = "";
				string[] fileExtensions = documentType.FileExtensions;
				for (int i = 0; i < (int)fileExtensions.Length; i++)
				{
					string str2 = fileExtensions[i];
					if (str.Length > 0)
					{
						str = string.Concat(str, ";");
						str1 = string.Concat(str1, ", ");
					}
					string str3 = string.Concat("*", str2);
					str = string.Concat(str, str3);
					str1 = string.Concat(str1, str3);
				}
				if (flag)
				{
					flag = false;
				}
				else
				{
					stringBuilder.Append(";");
				}
				stringBuilder.Append(str);
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string addProjectItemCommandFileFilterTextFormat = StringTable.AddProjectItemCommandFileFilterTextFormat;
				object[] description = new object[] { documentType.Description, str1 };
				fileTypeDescriptions.Add(new AddProjectItemCommand.FileTypeDescription(string.Format(currentCulture, addProjectItemCommandFileFilterTextFormat, description), str));
			}
			string str4 = stringBuilder.ToString();
			fileTypeDescriptions.Insert(0, new AddProjectItemCommand.FileTypeDescription(StringTable.AddProjectItemCommandAllAssetFiles, str4));
			fileTypeDescriptions.Insert(0, new AddProjectItemCommand.FileTypeDescription(StringTable.AddProjectItemCommandAllFiles, "*.*"));
			return fileTypeDescriptions.ToArray();
		}

		protected string GetImportFolder()
		{
			string defaultImportPath = this.ProjectManager().DefaultImportPath;
			if (!PathHelper.DirectoryExists(defaultImportPath))
			{
				defaultImportPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				if (!PathHelper.DirectoryExists(defaultImportPath))
				{
					defaultImportPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
				}
			}
			return defaultImportPath;
		}

		protected class FileTypeDescription
		{
			private string description;

			private string extensions;

			public string Description
			{
				get
				{
					return this.description;
				}
			}

			public string Extensions
			{
				get
				{
					return this.extensions;
				}
			}

			public FileTypeDescription(string description, string extensions)
			{
				this.description = description;
				this.extensions = extensions;
			}
		}
	}
}