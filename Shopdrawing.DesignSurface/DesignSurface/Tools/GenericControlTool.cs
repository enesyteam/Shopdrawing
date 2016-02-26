// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.GenericControlTool
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Tools.Assets;
using Microsoft.Expression.DesignSurface.Tools.Text;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal class GenericControlTool : GenericShapeTool
  {
    private IAssetLibrary assetLibrary;

    public Asset Asset
    {
      get
      {
        return (Asset) this.assetLibrary.FindActiveUserThemeAsset(this.ShapeType);
      }
    }

    public ITypeId ControlType
    {
      get
      {
        return this.ShapeType;
      }
    }

    public override string Caption
    {
      get
      {
        Asset asset = this.Asset;
        if (asset == null)
          return base.Caption;
        return asset.Name;
      }
    }

    public override string Description
    {
      get
      {
        return StringTable.GenericControlToolDescription;
      }
    }

    public GenericControlTool(ToolContext toolContext, ITypeId controlType, ToolCategory category)
      : base(toolContext, controlType, category)
    {
      this.assetLibrary = toolContext.AssetLibrary;
    }

    protected override ToolBehavior CreateToolBehavior()
    {
      Asset asset = this.Asset;
      if (asset == null)
        return base.CreateToolBehavior();
      if (asset.SupportsTextEditing(this.ActiveSceneViewModel.ProjectContext))
        return (ToolBehavior) new TextToolBehavior(this.GetActiveViewContext(), (ToolBehavior) new AssetToolBehavior(this.GetActiveViewContext(), (Func<Asset>) (() => this.Asset), true));
      return (ToolBehavior) new AssetToolBehavior(this.GetActiveViewContext(), (Func<Asset>) (() => this.Asset), false);
    }

    protected override void OnDoubleClick()
    {
      Asset asset = this.Asset;
      if (asset != null)
      {
        ISceneInsertionPoint sceneInsertionPoint = this.ActiveSceneViewModel.ActiveSceneInsertionPoint;
        if (!asset.IsValid || !asset.CanCreateInstance(sceneInsertionPoint))
          return;
        asset.CreateInstance(this.ActiveSceneViewModel.DesignerContext.LicenseManager, sceneInsertionPoint, Rect.Empty, (OnCreateInstanceAction) null);
      }
      else
        base.OnDoubleClick();
    }
  }
}
