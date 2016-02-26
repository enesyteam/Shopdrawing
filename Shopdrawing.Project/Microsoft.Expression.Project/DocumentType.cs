using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.Project
{
	public abstract class DocumentType : IDocumentType
	{
		private const string BuildTask = "None";

		private ImageSource cachedImage;

		public virtual bool AllowVisualStudioEdit
		{
			get
			{
				return false;
			}
		}

		protected ImageSource CachedImage
		{
			get
			{
				return this.cachedImage;
			}
			set
			{
				this.cachedImage = value;
			}
		}

		public virtual bool CanCreate
		{
			get
			{
				return false;
			}
		}

		public virtual bool CanView
		{
			get
			{
				return false;
			}
		}

		protected virtual string DefaultBuildTask
		{
			get
			{
				return "None";
			}
		}

		public BuildTaskInfo DefaultBuildTaskInfo
		{
			get
			{
				return new BuildTaskInfo(this.DefaultBuildTask, this.MetadataInformation);
			}
		}

		public virtual string DefaultFileExtension
		{
			get
			{
				if ((int)this.FileExtensions.Length <= 0)
				{
					return null;
				}
				return this.FileExtensions[0];
			}
		}

		public virtual string DefaultFileName
		{
			get
			{
				return Path.ChangeExtension(this.FileNameBase, this.DefaultFileExtension);
			}
		}

		public abstract string Description
		{
			get;
		}

		public abstract string[] FileExtensions
		{
			get;
		}

		protected abstract string FileNameBase
		{
			get;
		}

		public virtual ImageSource Image
		{
			get
			{
				if (this.cachedImage == null)
				{
					this.LoadImageIcon(this.ImagePath);
					this.cachedImage.Freeze();
				}
				return this.cachedImage;
			}
		}

		protected virtual string ImagePath
		{
			get
			{
				return string.Empty;
			}
		}

		public virtual bool IsAsset
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsDefaultTypeForExtension
		{
			get
			{
				return true;
			}
		}

		public virtual bool IsVirtual
		{
			get
			{
				return false;
			}
		}

		protected virtual IDictionary<string, string> MetadataInformation
		{
			get
			{
				return new Dictionary<string, string>();
			}
		}

		public abstract string Name
		{
			get;
		}

		public virtual Microsoft.Expression.Project.PreferredExternalEditCommand PreferredExternalEditCommand
		{
			get
			{
				return Microsoft.Expression.Project.PreferredExternalEditCommand.ShellEdit;
			}
		}

		protected DocumentType()
		{
		}

		public virtual bool AddToDocument(IProjectItem projectItem, IView view)
		{
			return false;
		}

		public virtual bool CanAddToProject(IProject project)
		{
			return true;
		}

		public virtual bool CanInsertTo(IProjectItem projectItem, IView view)
		{
			return false;
		}

		public virtual void CloseDocument(IProjectItem projectItem, IProject project)
		{
		}

		public virtual IDocument CreateDocument(IProjectItem projectItem, IProject project, string initialContents)
		{
			throw new NotImplementedException();
		}

		public virtual IProjectItem CreateProjectItem(IProject project, DocumentReference documentReference, IServiceProvider serviceProvider)
		{
			return new ProjectItem(project, documentReference, this, serviceProvider);
		}

		public virtual bool Exists(string name)
		{
			return Microsoft.Expression.Framework.Documents.PathHelper.FileOrDirectoryExists(name);
		}

		protected virtual bool HasExpectedFileExtension(string fileName)
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
			if (!string.IsNullOrEmpty(extension))
			{
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
			}
			return false;
		}

		public virtual bool IsDocumentTypeOf(string fileName)
		{
			return this.HasExpectedFileExtension(fileName);
		}

		protected virtual void LoadImageIcon(string path)
		{
			this.CachedImage = FileTable.GetImageSource(path);
		}

		public virtual void OnItemInvalidating(IProjectItem projectItem, DocumentReference oldReference)
		{
		}

		public virtual IDocument OpenDocument(IProjectItem projectItem, IProject project, bool isReadOnly)
		{
			throw new NotImplementedException();
		}

		public virtual void RefreshAllInstances(DocumentReference documentReference, IDocument document)
		{
		}
	}
}