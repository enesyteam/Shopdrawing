// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.AssetTool
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Tools.Assets;
using Microsoft.Expression.DesignSurface.Tools.Text;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal sealed class AssetTool : Tool
  {
    private Asset asset;

    public Asset Asset
    {
      get
      {
        return this.asset;
      }
    }

    public override string Identifier
    {
      get
      {
        return this.asset.Name;
      }
    }

    public override string Caption
    {
      get
      {
        return this.asset.Name;
      }
    }

    public override string Description
    {
      get
      {
        return StringTable.GenericControlToolDescription;
      }
    }

    public override Key Key
    {
      get
      {
        return Key.None;
      }
    }

    public override DrawingBrush NormalIconBrush
    {
      get
      {
        return this.asset.LargeIcon;
      }
    }

    public override DrawingBrush HoverIconBrush
    {
      get
      {
        return this.asset.LargeIcon;
      }
    }

    public override ToolCategory Category
    {
      get
      {
        return ToolCategory.Asset;
      }
    }

    public override bool IsEnabled
    {
      get
      {
        if (this.asset.IsValid)
          return base.IsEnabled;
        return false;
      }
    }

    internal override bool ShowExtensibleAdorners
    {
      get
      {
        return true;
      }
    }

    public AssetTool(ToolContext toolContext, Asset asset)
      : base(toolContext)
    {
      this.asset = asset;
    }

    protected override ToolBehavior CreateToolBehavior()
    {
      if (this.Asset.SupportsTextEditing(this.ActiveSceneViewModel.ProjectContext))
        return (ToolBehavior) new TextToolBehavior(this.GetActiveViewContext(), (ToolBehavior) new AssetToolBehavior(this.GetActiveViewContext(), (Func<Asset>) (() => this.asset), true));
      return (ToolBehavior) new AssetToolBehavior(this.GetActiveViewContext(), (Func<Asset>) (() => this.asset), false);
    }

    protected override void OnDoubleClick()
    {
      ISceneInsertionPoint sceneInsertionPoint = this.ActiveSceneViewModel.ActiveSceneInsertionPoint;
      if (this.asset != null && this.asset.IsValid && this.asset.CanCreateInstance(sceneInsertionPoint))
        this.asset.CreateInstance(this.ActiveSceneViewModel.DesignerContext.LicenseManager, sceneInsertionPoint, Rect.Empty, (OnCreateInstanceAction) null);
      if (!this.asset.SupportsTextEditing(this.ActiveSceneViewModel.ProjectContext))
        return;
      this.ActiveView.TryEnterTextEditMode(true);
    }
  }
}
