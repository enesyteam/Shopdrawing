// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Core.ProjectContext
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Project;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

namespace Microsoft.Expression.DesignModel.Core
{
  public abstract class ProjectContext : TypeResolver, IProjectContext, IDocumentRootResolver, ITypeResolver, IMetadataResolver, IServiceProvider
  {
    public abstract ICollection<IProjectDocument> Documents { get; }

    public abstract IProjectDocument Application { get; }

    public abstract IProjectDocument LocalApplication { get; }

    public abstract string ProjectName { get; }

    public abstract ObservableCollection<IProjectFont> ProjectFonts { get; }

    public abstract IFontResolver FontResolver { get; }

    public abstract IPlatform Platform { get; }

    public virtual FrameworkName TargetFramework
    {
      get
      {
        return (FrameworkName) null;
      }
    }

    public abstract IDocumentRoot ApplicationRoot { get; }

    public event EventHandler<ProjectDocumentEventArgs> DocumentOpened;

    public event EventHandler<ProjectDocumentEventArgs> DocumentClosing;

    public event EventHandler<ProjectDocumentEventArgs> DocumentClosed;

    public static ProjectContext GetProjectContext(IProjectContext projectContext)
    {
      if (projectContext != null)
        return (ProjectContext) projectContext.GetService(typeof (ProjectContext));
      return (ProjectContext) null;
    }

    public static IProjectContext FromProject(IProject project)
    {
      XamlProject xamlProject = project as XamlProject;
      if (xamlProject != null)
        return xamlProject.ProjectContext;
      return (IProjectContext) null;
    }

    public virtual bool InitializeContext()
    {
      return true;
    }

    public abstract IProjectDocument GetDocument(IDocumentRoot documentRoot);

    public abstract IProjectDocument GetDocument(IDocumentLocator documentLocator);

    public abstract IProjectDocument OpenDocument(string path);

    public abstract bool IsTypeInSolution(IType type);

    public abstract string MakeResourceReference(string resourceReference, IDocumentLocator referringDocument);

    public virtual IAssembly GetDesignAssembly(IAssembly assembly)
    {
      return (IAssembly) null;
    }

    public abstract Uri MakeDesignTimeUri(Uri uri, string documentUrl);

    public abstract IDocumentRoot GetDocumentRoot(string path);

    protected void OnDocumentOpened(IProjectDocument document)
    {
      if (this.DocumentOpened == null)
        return;
      this.DocumentOpened((object) this, new ProjectDocumentEventArgs(document));
    }

    protected void OnDocumentClosing(IProjectDocument document)
    {
      if (this.DocumentClosing == null)
        return;
      this.DocumentClosing((object) this, new ProjectDocumentEventArgs(document));
    }

    protected void OnDocumentClosed(IProjectDocument document)
    {
      if (this.DocumentClosed == null)
        return;
      this.DocumentClosed((object) this, new ProjectDocumentEventArgs(document));
    }

    public virtual object GetService(Type serviceType)
    {
      if (serviceType.IsAssignableFrom(this.GetType()))
        return (object) this;
      return (object) null;
    }

    public bool IsTypeSupported(ITypeId type)
    {
      if (!this.PlatformMetadata.IsSupported((ITypeResolver) this, type))
        return false;
      IType type1 = this.ResolveType(type);
      return (!PlatformTypes.MediaElement.IsAssignableFrom((ITypeId) type1) || this.IsCapabilitySet(PlatformCapability.SupportsMediaElementControl)) && (!PlatformTypes.HyperlinkButton.IsAssignableFrom((ITypeId) type1) || this.IsCapabilitySet(PlatformCapability.SupportsHyperlinkButtonControl)) && ((!PlatformTypes.ComboBox.IsAssignableFrom((ITypeId) type1) || this.IsCapabilitySet(PlatformCapability.SupportsComboBox)) && (!PlatformTypes.ComboBoxItem.IsAssignableFrom((ITypeId) type1) || this.IsCapabilitySet(PlatformCapability.SupportsComboBoxItem))) && (!PlatformTypes.VirtualizingStackPanel.IsAssignableFrom((ITypeId) type1) || this.IsCapabilitySet(PlatformCapability.SupportsVirtualizingStackPanel));
    }

    [SpecialName]
    ITypeMetadataFactory IProjectContext.get_MetadataFactory()
    {
      return this.MetadataFactory;
    }
  }
}
