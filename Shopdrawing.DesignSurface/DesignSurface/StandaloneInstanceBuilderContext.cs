// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.StandaloneInstanceBuilderContext
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.InstanceBuilders;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.Text;
using Microsoft.Expression.DesignModel.ViewObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface
{
  internal class StandaloneInstanceBuilderContext : InstanceBuilderContextBase
  {
    private IViewPanel overlayLayer;
    private IProjectContext projectContext;

    public override bool IsStandalone
    {
      get
      {
        return true;
      }
    }

    public override IViewPanel OverlayLayer
    {
      get
      {
        return this.overlayLayer;
      }
    }

    public StandaloneInstanceBuilderContext(IDocumentContext documentContext, DesignerContext designerContext)
      : this(documentContext, (IViewRootResolver) designerContext.ViewRootResolver, designerContext.TextBufferService)
    {
    }

    public StandaloneInstanceBuilderContext(IDocumentContext documentContext, IInstanceBuilderContext context)
      : this(documentContext, (IViewRootResolver) null, context.TextBufferService)
    {
      InstanceBuilderContextBase.TransferViewRootResolver((InstanceBuilderContextBase) context, (InstanceBuilderContextBase) this);
    }

    public StandaloneInstanceBuilderContext(IDocumentContext documentContext, IViewRootResolver viewRootResolver, ITextBufferService textBufferService)
      : base(((IProjectContext) documentContext.TypeResolver).Platform, documentContext, (IDocumentRootResolver) documentContext.TypeResolver, viewRootResolver, ((IProjectContext) documentContext.TypeResolver).MetadataFactory, (INameScope) new NameScope(), (IInstanceBuilderContext) null, true, textBufferService, (DocumentNode) null)
    {
      this.overlayLayer = ((IProjectContext) documentContext.TypeResolver).Platform.CreateOverlayLayer();
      this.projectContext = (IProjectContext) documentContext.TypeResolver;
    }

    public override ICollection<IProperty> GetProperties(ViewNode viewNode)
    {
      return (ICollection<IProperty>) ((DocumentCompositeNode) viewNode.DocumentNode).Properties.Keys;
    }

    public override DocumentNode GetPropertyValue(ViewNode viewNode, IPropertyId propertyKey)
    {
      return ((DocumentCompositeNode) viewNode.DocumentNode).Properties[propertyKey];
    }

    public override bool ShouldDisableVisualStateManagerFor(ViewNode viewNode)
    {
      return false;
    }

    public override object EvaluateSystemResource(object resourceKey)
    {
      FrameworkElement frameworkElement = this.overlayLayer.PlatformSpecificObject as FrameworkElement;
      if (frameworkElement != null)
        return frameworkElement.FindResource(resourceKey);
      return (object) null;
    }

    public override bool HasFont(string fontFamilyName)
    {
      return Enumerable.FirstOrDefault<IProjectFont>((IEnumerable<IProjectFont>) this.projectContext.ProjectFonts, (Func<IProjectFont, bool>) (font => font.FontFamilyName == fontFamilyName)) != null;
    }

    public override string ResolveFont(string fontFamilySource, object fontStretch, object fontStyle, object fontWeight, IDocumentContext documentContext)
    {
      return this.projectContext.FontResolver.ResolveFont(fontFamilySource, (object) fontStretch.ToString(), (object) fontStyle.ToString(), (object) fontWeight.ToString(), documentContext);
    }

    public override string ConvertToWpfFontName(string gdiFontName)
    {
      return this.projectContext.FontResolver.ConvertToWpfFontName(gdiFontName);
    }
  }
}
