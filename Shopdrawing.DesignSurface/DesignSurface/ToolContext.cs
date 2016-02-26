// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ToolContext
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.Tools.Assets;
using Microsoft.Expression.DesignSurface.Tools.Path;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.Framework;

namespace Microsoft.Expression.DesignSurface
{
  internal sealed class ToolContext
  {
    private DesignerContext designerContext;
    private AssetMruList assetMruList;

    public SceneView ActiveView
    {
      get
      {
        if (this.designerContext.ViewService == null)
          return (SceneView) null;
        return this.designerContext.ViewService.ActiveView as SceneView;
      }
    }

    public IPropertyManager PropertyManager
    {
      get
      {
        return this.designerContext.PropertyManager;
      }
    }

    public IAmbientPropertyManager AmbientPropertyManager
    {
      get
      {
        return this.designerContext.AmbientPropertyManager;
      }
    }

    public ToolManager ToolManager
    {
      get
      {
        return this.designerContext.ToolManager;
      }
    }

    public IAssetLibrary AssetLibrary
    {
      get
      {
        return this.designerContext.AssetLibrary;
      }
    }

    public SnappingEngine SnappingEngine
    {
      get
      {
        return this.designerContext.SnappingEngine;
      }
    }

    public IMessageDisplayService MessageDisplayService
    {
      get
      {
        return this.designerContext.MessageDisplayService;
      }
    }

    public ElementToPathEditorTargetMap PathEditorTargetMap
    {
      get
      {
        return this.designerContext.PathEditorTargetMap;
      }
    }

    public IPlatform DesignerDefaultPlatform
    {
      get
      {
        return this.designerContext.DesignerDefaultPlatformService.DefaultPlatform;
      }
    }

    public AssetMruList AssetMruList
    {
      get
      {
        return this.assetMruList;
      }
    }

    public ToolContext(DesignerContext designerContext)
    {
      this.designerContext = designerContext;
      this.assetMruList = new AssetMruList(designerContext, this);
    }
  }
}
