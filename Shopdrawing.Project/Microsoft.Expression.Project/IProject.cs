using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Versioning;

namespace Microsoft.Expression.Project
{
	public interface IProject : INamedProject, IDocumentItem, IDisposable, INotifyPropertyChanged
	{
		ICodeDocumentType CodeDocumentType
		{
			get;
		}

		string Configuration
		{
			get;
		}

		string DefaultNamespaceName
		{
			get;
		}

		string FullTargetPath
		{
			get;
		}

		bool IsReadOnly
		{
			get;
		}

		Microsoft.Expression.Framework.IReadOnlyCollection<IProjectItem> Items
		{
			get;
		}

		Guid ProjectGuid
		{
			get;
		}

		Microsoft.Expression.Framework.Documents.DocumentReference ProjectRoot
		{
			get;
		}

		IProjectStore ProjectStore
		{
			get;
		}

		IProjectType ProjectType
		{
			get;
		}

		string PropertiesPath
		{
			get;
		}

		IAssemblyCollection ReferencedAssemblies
		{
			get;
		}

		IEnumerable<IProject> ReferencedProjects
		{
			get;
		}

		bool ShouldProduceProjectOutputReferences
		{
			get;
		}

		bool ShouldReceiveProjectOutputReferences
		{
			get;
		}

		IProjectItem StartupItem
		{
			get;
			set;
		}

		ProjectAssembly TargetAssembly
		{
			get;
		}

		FrameworkName TargetFramework
		{
			get;
		}

		ICollection<string> TemplateProjectSubtypes
		{
			get;
		}

		string TemplateProjectType
		{
			get;
		}

		string UICulture
		{
			get;
			set;
		}

		IProjectItem AddAssemblyReference(string path, bool verifyFileExists);

		void AddAssemblyReferenceDeferred(string path);

		void AddImport(string path);

		IProjectItem AddItem(DocumentCreationInfo creationInfo);

		IEnumerable<IProjectItem> AddItems(IEnumerable<DocumentCreationInfo> creationInfo);

		IProjectItem AddProjectReference(IProject project);

		IProjectItem FindItem(Microsoft.Expression.Framework.Documents.DocumentReference documentReference);

		T GetCapability<T>(string name);

		IDocumentType GetDocumentType(string fileName);

		bool IsValidItemTemplate(IProjectItemTemplate itemTemplate);

		bool IsValidStartupItem(IProjectItem projectItem);

		Uri MakeDesignTimeUri(Uri uri, string documentPath);

		bool RemoveItems(bool deleteFiles, params IProjectItem[] projectItems);

		bool RenameProjectItem(IProjectItem oldProjectItem, Microsoft.Expression.Framework.Documents.DocumentReference newDocumentReference);

		bool SetCapability<T>(string name, T value);

		void UpdateProjectItemDisplayName(IProjectItem projectItem, string newDisplayName);

		bool UpdateProjectItemFileName(IProjectItem oldProjectItem, Microsoft.Expression.Framework.Documents.DocumentReference newDocumentReference);

		event EventHandler<ProjectItemEventArgs> ItemAdded;

		event EventHandler<ProjectItemEventArgs> ItemChanged;

		event EventHandler<ProjectItemEventArgs> ItemClosed;

		event EventHandler<ProjectItemEventArgs> ItemClosing;

		event EventHandler<ProjectItemEventArgs> ItemDeleted;

		event EventHandler<ProjectItemEventArgs> ItemOpened;

		event EventHandler<ProjectItemEventArgs> ItemRemoved;

		event EventHandler<ProjectItemRenamedEventArgs> ItemRenamed;

		event EventHandler<ProjectEventArgs> ProcessingProjectChanges;

		event EventHandler<ProjectEventArgs> ProcessingProjectChangesComplete;

		event EventHandler<ProjectEventArgs> ProjectChanged;

		event EventHandler<ProjectItemChangedEventArgs> StartupItemChanged;
	}
}