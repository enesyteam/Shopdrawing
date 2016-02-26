// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.InstanceBuilderContext
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.InstanceBuilders;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.Text;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.ViewModel.Extensibility;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class InstanceBuilderContext : InstanceBuilderContextBase, IDesignModeValueProviderContext
  {
    private SceneViewModel viewModel;

    public override IViewPanel OverlayLayer
    {
      get
      {
        if (this.viewModel != null && this.viewModel.DefaultView != null)
          return this.viewModel.DefaultView.OverlayLayer;
        return (IViewPanel) null;
      }
    }

    public override IAttachViewRoot AttachedViewRoot
    {
      get
      {
        return (IAttachViewRoot) this.viewModel.DefaultView;
      }
    }

    public override IPlatform DesignerDefaultPlatform
    {
      get
      {
        return this.viewModel.DesignerContext.DesignerDefaultPlatformService.DefaultPlatform;
      }
    }

    public ValueTranslationService ValueTranslationService
    {
      get
      {
        return this.viewModel.ExtensibilityManager.EditingContext.Services.GetRequiredService<ValueTranslationService>();
      }
    }

    public InstanceBuilderContext(IProjectContext projectContext, SceneViewModel viewModel, bool useShadowProperties, DocumentNode alternateSiteNode)
      : this(projectContext.Platform, (IDocumentRootResolver) projectContext, projectContext.MetadataFactory, viewModel, (INameScope) null, (InstanceBuilderContext) null, useShadowProperties, viewModel.DesignerContext.TextBufferService, alternateSiteNode)
    {
    }

    public InstanceBuilderContext(IPlatform platform, IDocumentRootResolver documentRootResolver, ITypeMetadataFactory metadataFactory, SceneViewModel viewModel, INameScope nameScope, InstanceBuilderContext parentContext, bool useShadowProperties, ITextBufferService textBufferService, DocumentNode alternateSiteNode)
      : base(platform, viewModel.DocumentRoot.DocumentContext, documentRootResolver, (IViewRootResolver) viewModel.DesignerContext.ViewRootResolver, metadataFactory, nameScope, (IInstanceBuilderContext) parentContext, useShadowProperties, textBufferService, alternateSiteNode)
    {
      this.viewModel = viewModel;
    }

    public override ICollection<IProperty> GetProperties(ViewNode viewNode)
    {
      return this.viewModel.GetProperties((IInstanceBuilderContext) this, viewNode);
    }

    public override DocumentNode GetPropertyValue(ViewNode viewNode, IPropertyId propertyKey)
    {
      return this.viewModel.GetPropertyValue((IInstanceBuilderContext) this, viewNode, propertyKey);
    }

    public override object EvaluateSystemResource(object resourceKey)
    {
      return this.viewModel.FindResource(resourceKey);
    }

    public override bool ShouldDisableVisualStateManagerFor(ViewNode viewNode)
    {
      return this.viewModel.ShouldDisableVisualStateManagerFor((IInstanceBuilderContext) this, viewNode);
    }

    public void UpdateDocumentRootResolver(IDocumentRootResolver resolver)
    {
      this.DocumentRootResolver = resolver;
    }

    public override bool ShouldInstantiatePreviewControl(IDocumentRoot documentRoot)
    {
      if (base.ShouldInstantiatePreviewControl(documentRoot))
        return true;
      IProjectDocument projectDocument = this.viewModel.ProjectContext.OpenDocument(documentRoot.DocumentContext.DocumentUrl);
      if (projectDocument != null && projectDocument.IsDirty)
        return true;
      IAssembly projectAssembly = documentRoot.TypeResolver.ProjectAssembly;
      if (projectAssembly.IsLoaded)
      {
        FileInfo fileInfo = new FileInfo(projectAssembly.Location);
        if (fileInfo.Exists && projectDocument != null)
          return new FileInfo(projectDocument.Path).LastWriteTimeUtc > fileInfo.LastWriteTimeUtc;
      }
      return false;
    }

    public override bool HasFont(string fontFamilyName)
    {
      IProjectFont projectFont = (IProjectFont) null;
      if (this.viewModel != null)
        Enumerable.FirstOrDefault<IProjectFont>((IEnumerable<IProjectFont>) this.viewModel.ProjectContext.ProjectFonts, (Func<IProjectFont, bool>) (font => font.FontFamilyName == fontFamilyName));
      return projectFont != null;
    }

    public override string ResolveFont(string fontFamilySource, object fontStretch, object fontStyle, object fontWeight, IDocumentContext documentContext)
    {
      if (this.viewModel != null)
        return this.viewModel.ProjectContext.FontResolver.ResolveFont(fontFamilySource, (object) fontStretch.ToString(), (object) fontStyle.ToString(), (object) fontWeight.ToString(), documentContext);
      return fontFamilySource;
    }

    public override string ConvertToWpfFontName(string gdiFontName)
    {
      if (this.viewModel != null)
        return this.viewModel.ProjectContext.FontResolver.ConvertToWpfFontName(gdiFontName);
      return gdiFontName;
    }

    public override IDocumentContext CreateErrorDocumentContext(IDocumentContext sourceDocumentContext)
    {
      IProjectContext sourceContext = (IProjectContext) sourceDocumentContext.TypeResolver;
      List<IAssembly> list = new List<IAssembly>(PlatformTypes.DesignToolAssemblies.Count);
      foreach (Assembly assembly in (IEnumerable<Assembly>) PlatformTypes.DesignToolAssemblies)
        list.Add(sourceContext.Platform.Metadata.GetDesignToolAssembly(assembly));
      return (IDocumentContext) new DocumentContext((IProjectContext) new CustomAssemblyReferencesProjectContext(sourceContext, (ICollection<IAssembly>) list), ((DocumentContext) sourceDocumentContext).DocumentLocator);
    }

    public override IInstanceBuilderContext CreateStandaloneChildContext(IDocumentContext documentContext)
    {
      return (IInstanceBuilderContext) new StandaloneInstanceBuilderContext(documentContext, this.ViewRootResolver, this.TextBufferService);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        this.viewModel = (SceneViewModel) null;
      base.Dispose(disposing);
    }

    public override void UpdateTextEditProxyIfEditing()
    {
      if (this.viewModel == null || this.viewModel.TextSelectionSet == null || this.viewModel.TextSelectionSet.TextEditProxy == null)
        return;
      this.viewModel.TextSelectionSet.TextEditProxy.UpdateEditingElementLayout();
    }

    public ModelItem GetModelItemForViewObject(IViewObject viewObject)
    {
      if (this.viewModel != null)
      {
        DocumentNode correspondingDocumentNode = this.viewModel.DefaultView.GetCorrespondingDocumentNode(viewObject, false);
        if (correspondingDocumentNode != null)
          return (ModelItem) this.viewModel.GetSceneNode(correspondingDocumentNode).ModelItem;
      }
      return (ModelItem) null;
    }
  }
}
