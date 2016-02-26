using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Project;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Versioning;
using System.Threading;

namespace Microsoft.Expression.DesignModel.Core
{
    public abstract class ProjectContext : TypeResolver, IProjectContext, IDocumentRootResolver, ITypeResolver, IMetadataResolver, IServiceProvider
    {
        public abstract IProjectDocument Application
        {
            get;
        }

        public abstract IDocumentRoot ApplicationRoot
        {
            get;
        }

        public abstract ICollection<IProjectDocument> Documents
        {
            get;
        }

        public abstract IFontResolver FontResolver
        {
            get;
        }

        public abstract IProjectDocument LocalApplication
        {
            get;
        }

        public abstract IPlatform Platform
        {
            get;
        }

        public abstract ObservableCollection<IProjectFont> ProjectFonts
        {
            get;
        }

        public abstract string ProjectName
        {
            get;
        }

        public virtual FrameworkName TargetFramework
        {
            get
            {
                return null;
            }
        }

        protected ProjectContext()
        {
        }

        public static IProjectContext FromProject(IProject project)
        {
            XamlProject xamlProject = project as XamlProject;
            if (xamlProject == null)
            {
                return null;
            }
            return xamlProject.ProjectContext;
        }

        public virtual IAssembly GetDesignAssembly(IAssembly assembly)
        {
            return null;
        }

        public abstract IProjectDocument GetDocument(IDocumentRoot documentRoot);

        public abstract IProjectDocument GetDocument(IDocumentLocator documentLocator);

        public abstract IDocumentRoot GetDocumentRoot(string path);

        public static ProjectContext GetProjectContext(IProjectContext projectContext)
        {
            if (projectContext == null)
            {
                return null;
            }
            return (ProjectContext)projectContext.GetService(typeof(ProjectContext));
        }

        public virtual object GetService(Type serviceType)
        {
            if (serviceType.IsAssignableFrom(base.GetType()))
            {
                return this;
            }
            return null;
        }

        public virtual bool InitializeContext()
        {
            return true;
        }

        public abstract bool IsTypeInSolution(IType type);

        public bool IsTypeSupported(ITypeId type)
        {
            if (!base.PlatformMetadata.IsSupported(this, type))
            {
                return false;
            }
            IType type1 = this.ResolveType(type);
            if (PlatformTypes.MediaElement.IsAssignableFrom(type1) && !this.IsCapabilitySet(PlatformCapability.SupportsMediaElementControl))
            {
                return false;
            }
            if (PlatformTypes.HyperlinkButton.IsAssignableFrom(type1) && !this.IsCapabilitySet(PlatformCapability.SupportsHyperlinkButtonControl))
            {
                return false;
            }
            if (PlatformTypes.ComboBox.IsAssignableFrom(type1) && !this.IsCapabilitySet(PlatformCapability.SupportsComboBox))
            {
                return false;
            }
            if (PlatformTypes.ComboBoxItem.IsAssignableFrom(type1) && !this.IsCapabilitySet(PlatformCapability.SupportsComboBoxItem))
            {
                return false;
            }
            if (PlatformTypes.VirtualizingStackPanel.IsAssignableFrom(type1) && !this.IsCapabilitySet(PlatformCapability.SupportsVirtualizingStackPanel))
            {
                return false;
            }
            return true;
        }

        public abstract Uri MakeDesignTimeUri(Uri uri, string documentUrl);

        public abstract string MakeResourceReference(string resourceReference, IDocumentLocator referringDocument);

        ITypeMetadataFactory Microsoft.Expression.DesignModel.Core.IProjectContext.MetadataFactory
        {
            get { return base.MetadataFactory; }
        }

        protected void OnDocumentClosed(IProjectDocument document)
        {
            if (this.DocumentClosed != null)
            {
                this.DocumentClosed(this, new ProjectDocumentEventArgs(document));
            }
        }

        protected void OnDocumentClosing(IProjectDocument document)
        {
            if (this.DocumentClosing != null)
            {
                this.DocumentClosing(this, new ProjectDocumentEventArgs(document));
            }
        }

        protected void OnDocumentOpened(IProjectDocument document)
        {
            if (this.DocumentOpened != null)
            {
                this.DocumentOpened(this, new ProjectDocumentEventArgs(document));
            }
        }

        public abstract IProjectDocument OpenDocument(string path);

        public event EventHandler<ProjectDocumentEventArgs> DocumentClosed;

        public event EventHandler<ProjectDocumentEventArgs> DocumentClosing;

        public event EventHandler<ProjectDocumentEventArgs> DocumentOpened;
    }
}