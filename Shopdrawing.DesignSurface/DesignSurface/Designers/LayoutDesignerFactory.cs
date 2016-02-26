// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Designers.LayoutDesignerFactory
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;

namespace Microsoft.Expression.DesignSurface.Designers
{
  public static class LayoutDesignerFactory
  {
    public static ILayoutDesigner Instantiate(SceneNode node)
    {
      LayoutDesignerFactory.LayoutDesignerTypeHandlerFactory orCreateCache = DesignSurfacePlatformCaches.GetOrCreateCache<LayoutDesignerFactory.LayoutDesignerTypeHandlerFactory>(node.Platform.Metadata, DesignSurfacePlatformCaches.LayoutDesignerFactory);
      ItemsControlElement itemsControlElement = node as ItemsControlElement;
      Type type = (Type) null;
      if (itemsControlElement != null && itemsControlElement.ItemsHost != null)
        type = orCreateCache.GetLayoutDesignerType((ITypeResolver) itemsControlElement.ProjectContext, itemsControlElement.ProjectContext.GetType(itemsControlElement.ItemsHost.GetType()), true);
      if (type == (Type) null)
        type = orCreateCache.GetLayoutDesignerType((ITypeResolver) node.ProjectContext, node.Type, false);
      if (!(type == (Type) null))
        return (ILayoutDesigner) Activator.CreateInstance(type, (object[]) null);
      return (ILayoutDesigner) null;
    }

    private sealed class LayoutDesignerTypeHandlerFactory : TypeIdHandlerFactory<LayoutDesignerFactory.LayoutDesignerTypeHandler>
    {
      public Type GetLayoutDesignerType(ITypeResolver typeResolver, IType type, bool appliesToItemsHost)
      {
        LayoutDesignerFactory.LayoutDesignerTypeHandler handler = this.GetHandler((IMetadataResolver) typeResolver, type);
        if (handler != null && (!appliesToItemsHost || handler.AppliesToItemsHost))
          return handler.LayoutDesignerType;
        return (Type) null;
      }

      protected override void Initialize()
      {
        base.Initialize();
        this.RegisterHandler(new LayoutDesignerFactory.LayoutDesignerTypeHandler(PlatformTypes.Object, typeof (LayoutDesigner), false));
        this.RegisterHandler(new LayoutDesignerFactory.LayoutDesignerTypeHandler(PlatformTypes.Canvas, typeof (CanvasLayoutDesigner), true));
        this.RegisterHandler(new LayoutDesignerFactory.LayoutDesignerTypeHandler(ProjectNeutralTypes.DockPanel, typeof (DockPanelLayoutDesigner), false));
        this.RegisterHandler(new LayoutDesignerFactory.LayoutDesignerTypeHandler(PlatformTypes.Grid, typeof (GridLayoutDesigner), false));
      }

      protected override ITypeId GetBaseType(LayoutDesignerFactory.LayoutDesignerTypeHandler handler)
      {
        return handler.BaseType;
      }
    }

    private sealed class LayoutDesignerTypeHandler
    {
      private ITypeId baseType;
      private Type layoutDesignerType;
      private bool appliesToItemsHost;

      public ITypeId BaseType
      {
        get
        {
          return this.baseType;
        }
      }

      public Type LayoutDesignerType
      {
        get
        {
          return this.layoutDesignerType;
        }
      }

      public bool AppliesToItemsHost
      {
        get
        {
          return this.appliesToItemsHost;
        }
      }

      public LayoutDesignerTypeHandler(ITypeId baseType, Type layoutDesignerType, bool appliesToItemsHost)
      {
        this.baseType = baseType;
        this.layoutDesignerType = layoutDesignerType;
        this.appliesToItemsHost = appliesToItemsHost;
      }
    }
  }
}
