using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;

namespace Microsoft.Expression.Project
{
	public interface IProjectItem : IDocumentItem, IDisposable
	{
		bool CanAddChildren
		{
			get;
		}

		IEnumerable<IProjectItem> Children
		{
			get;
		}

		IProjectItem CodeBehindItem
		{
			get;
		}

		bool ContainsDesignTimeResources
		{
			get;
			set;
		}

		IDocument Document
		{
			get;
		}

		IDocumentType DocumentType
		{
			get;
		}

		bool FileExists
		{
			get;
		}

		ImageSource Image
		{
			get;
		}

		bool IsCodeBehindItem
		{
			get;
		}

		bool IsCut
		{
			get;
			set;
		}

		bool IsDirty
		{
			get;
		}

		bool IsLinkedFile
		{
			get;
		}

		bool IsOpen
		{
			get;
		}

		bool IsReadOnly
		{
			get;
		}

		IProjectItem Parent
		{
			get;
			set;
		}

		IProject Project
		{
			get;
		}

		string ProjectRelativeDocumentReference
		{
			get;
		}

		IPropertiesCollection Properties
		{
			get;
		}

		IEnumerable<IProjectItem> ReferencingProjectItems
		{
			get;
		}

		bool Visible
		{
			get;
		}

		void AddChild(IProjectItem child);

		void CloseDocument();

		bool CreateDocument(string initialContents);

		ICommandBar GetContextMenu(ICommandBarCollection commandBarCollection);

		string GetResourceReference(Microsoft.Expression.Framework.Documents.DocumentReference referencingDocument);

		bool IsComponentUri(string resourceReference);

		bool OpenDocument(bool isReadOnly);

		bool OpenDocument(bool isReadOnly, bool suppressUI);

		IDocumentView OpenView(bool makeActive);

		void RemoveChild(IProjectItem child);

		bool SaveDocumentFile();

		event FileSystemEventHandler FileInformationChanged;

		event EventHandler IsCutChanged;

		event EventHandler IsDirtyChanged;

		event EventHandler IsReadOnlyChanged;

		event EventHandler ParentChanged;
	}
}