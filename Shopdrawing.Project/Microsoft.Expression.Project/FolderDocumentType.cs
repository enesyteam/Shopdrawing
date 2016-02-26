using Microsoft.Expression.Framework.Documents;
using System;
using System.IO;

namespace Microsoft.Expression.Project
{
	internal class FolderDocumentType : DocumentType
	{
		private const string BuildTask = "Folder";

		protected override string DefaultBuildTask
		{
			get
			{
				return "Folder";
			}
		}

		public override string DefaultFileName
		{
			get
			{
				return string.Concat(this.FileNameBase, Path.DirectorySeparatorChar);
			}
		}

		public override string Description
		{
			get
			{
				return StringTable.FolderDocumentTypeDescription;
			}
		}

		public override string[] FileExtensions
		{
			get
			{
				return new string[] { "." };
			}
		}

		protected override string FileNameBase
		{
			get
			{
				return StringTable.FolderDocumentTypeFileNameBase;
			}
		}

		protected override string ImagePath
		{
			get
			{
				return "Resources\\file_folder_on_16x16.png";
			}
		}

		public override string Name
		{
			get
			{
				return DocumentTypeNamesHelper.Folder;
			}
		}

		public override Microsoft.Expression.Project.PreferredExternalEditCommand PreferredExternalEditCommand
		{
			get
			{
				return Microsoft.Expression.Project.PreferredExternalEditCommand.None;
			}
		}

		public FolderDocumentType()
		{
		}

		public override bool CanAddToProject(IProject project)
		{
			return false;
		}

		public override IProjectItem CreateProjectItem(IProject project, DocumentReference documentReference, IServiceProvider serviceProvider)
		{
			return new FolderProjectItem(project, documentReference, this, serviceProvider);
		}

		public override bool Exists(string name)
		{
			return Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(name);
		}

		protected override bool HasExpectedFileExtension(string fileName)
		{
			string extension;
			bool flag;
			try
			{
				extension = Path.GetExtension(fileName);
			}
			catch (ArgumentException argumentException)
			{
				flag = false;
				return flag;
			}
			if (string.IsNullOrEmpty(extension))
			{
				return true;
			}
			string[] fileExtensions = this.FileExtensions;
			int num = 0;
			while (num < (int)fileExtensions.Length)
			{
				string str = fileExtensions[num];
				if (StringComparer.OrdinalIgnoreCase.Compare(str, extension) != 0)
				{
					num++;
				}
				else
				{
					flag = true;
					return flag;
				}
			}
			return false;
			return flag;
		}
	}
}