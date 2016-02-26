using Microsoft.Expression.Framework.Documents;
using System;
using System.Windows.Media;

namespace Microsoft.Expression.Project
{
	public interface IDocumentType
	{
		bool AllowVisualStudioEdit
		{
			get;
		}

		bool CanCreate
		{
			get;
		}

		bool CanView
		{
			get;
		}

		BuildTaskInfo DefaultBuildTaskInfo
		{
			get;
		}

		string DefaultFileExtension
		{
			get;
		}

		string DefaultFileName
		{
			get;
		}

		string Description
		{
			get;
		}

		string[] FileExtensions
		{
			get;
		}

		ImageSource Image
		{
			get;
		}

		bool IsAsset
		{
			get;
		}

		bool IsDefaultTypeForExtension
		{
			get;
		}

		string Name
		{
			get;
		}

		Microsoft.Expression.Project.PreferredExternalEditCommand PreferredExternalEditCommand
		{
			get;
		}

		bool AddToDocument(IProjectItem projectItem, IView view);

		bool CanAddToProject(IProject project);

		bool CanInsertTo(IProjectItem projectItem, IView view);

		void CloseDocument(IProjectItem projectItem, IProject project);

		IDocument CreateDocument(IProjectItem projectItem, IProject project, string initialContents);

		IProjectItem CreateProjectItem(IProject project, DocumentReference documentReference, IServiceProvider serviceProvider);

		bool Exists(string reference);

		bool IsDocumentTypeOf(string fileName);

		void OnItemInvalidating(IProjectItem projectItem, DocumentReference oldReference);

		IDocument OpenDocument(IProjectItem projectItem, IProject project, bool isReadOnly);

		void RefreshAllInstances(DocumentReference documentReference, IDocument document);
	}
}