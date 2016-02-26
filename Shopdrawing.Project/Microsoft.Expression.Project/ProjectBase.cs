using Microsoft.Expression.Framework.Documents;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

namespace Microsoft.Expression.Project
{
	public abstract class ProjectBase : DocumentItemBase, INamedProject, IDocumentItem, IDisposable
	{
		private Guid? generatedGuid;

		private Microsoft.Expression.Framework.Documents.DocumentReference projectRoot;

		internal bool IsDisposed
		{
			get;
			private set;
		}

		internal bool IsDisposing
		{
			get;
			private set;
		}

		public string Name
		{
			get
			{
				return base.DocumentReference.DisplayNameShort;
			}
		}

		public Microsoft.Expression.Project.ProjectFileInformation ProjectFileInformation
		{
			get;
			set;
		}

		public Guid ProjectGuid
		{
			get
			{
				Guid guid;
				if (Guid.TryParse(this.ProjectStore.GetProperty("ProjectGuid"), out guid))
				{
					return guid;
				}
				if (!this.generatedGuid.HasValue)
				{
					this.generatedGuid = new Guid?(Guid.NewGuid());
				}
				return this.generatedGuid.Value;
			}
		}

		public Microsoft.Expression.Framework.Documents.DocumentReference ProjectRoot
		{
			get
			{
				return this.projectRoot;
			}
		}

		protected internal IProjectStore ProjectStore
		{
			get;
			private set;
		}

		protected IServiceProvider Services
		{
			get;
			private set;
		}

		protected ProjectBase(IProjectStore projectStore, IServiceProvider serviceProvider) : base(projectStore.DocumentReference)
		{
			this.Services = serviceProvider;
			if (projectStore.DocumentReference.IsValidPathFormat)
			{
				this.projectRoot = Microsoft.Expression.Framework.Documents.DocumentReference.Create(PathHelper.EnsurePathEndsInDirectorySeparator(PathHelper.GetDirectory(projectStore.DocumentReference.Path)));
			}
			this.ProjectFileInformation = new Microsoft.Expression.Project.ProjectFileInformation(projectStore.DocumentReference.Path);
			this.ProjectStore = projectStore;
		}

		public void Dispose()
		{
			try
			{
				if (!this.IsDisposed)
				{
					this.IsDisposing = true;
					this.Dispose(true);
				}
			}
			finally
			{
				this.IsDisposed = true;
			}
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && !this.IsDisposed)
			{
				this.ProjectStore.Dispose();
				this.ProjectStore = null;
			}
		}

		public bool DoesErrorConditionExist(string errorCondition, object parameter)
		{
			if (errorCondition == null)
			{
				throw new ArgumentNullException("errorCondition");
			}
			return this.DoesErrorConditionExistInternal(errorCondition, parameter);
		}

		protected virtual bool DoesErrorConditionExistInternal(string errorCondition, object parameter)
		{
			return false;
		}

		public static bool IsDisposingOrDisposed(IProject project)
		{
			ProjectBase projectBase = project as ProjectBase;
			if (projectBase == null)
			{
				return false;
			}
			if (projectBase.IsDisposed)
			{
				return true;
			}
			return projectBase.IsDisposing;
		}

		protected static bool IsReloadPromptEnabled()
		{
			return SolutionBase.IsReloadPromptEnabled();
		}

		protected bool IsWithinProjectRoot(Microsoft.Expression.Framework.Documents.DocumentReference documentReference)
		{
			return documentReference.Path.StartsWith(this.ProjectRoot.Path, StringComparison.OrdinalIgnoreCase);
		}

		protected internal void ReloadProjectStore()
		{
			Type type = this.ProjectStore.GetType();
			FrameworkName targetFrameworkName = ProjectStoreHelper.GetTargetFrameworkName(this.ProjectStore);
			this.ProjectStore.Dispose();
			this.ProjectStore = ProjectStoreHelper.CreateProjectStore(base.DocumentReference, this.Services, ProjectStoreHelper.DefaultProjectCreationChain);
			if (this.ProjectStore.GetType() != type || this.ProjectStore.StoreVersion != CommonVersions.Version4_0)
			{
				throw new ProjectStoreUnsupportedException();
			}
			FrameworkName frameworkName = ProjectStoreHelper.GetTargetFrameworkName(this.ProjectStore);
			if (frameworkName != targetFrameworkName || frameworkName == null || targetFrameworkName == null || !string.Equals(targetFrameworkName.FullName, frameworkName.FullName, StringComparison.OrdinalIgnoreCase))
			{
				throw new TargetFrameworkChangedException();
			}
		}

		public virtual bool ShouldPerformOutOfBrowserDeployment()
		{
			return false;
		}
	}
}