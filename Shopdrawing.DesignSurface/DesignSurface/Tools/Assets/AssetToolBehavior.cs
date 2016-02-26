// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.AssetToolBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.Tools.Transforms;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  internal sealed class AssetToolBehavior : ElementDragCreateBehavior
  {
    private Func<Asset> queryAsset;

    private Asset Asset
    {
      get
      {
        return this.queryAsset();
      }
    }

    private Size AssetSize
    {
      get
      {
        Asset asset = this.Asset;
        if (asset == null)
          return Size.Empty;
        return asset.SourcePixelSize;
      }
    }

    protected override ITypeId InstanceType
    {
      get
      {
        TypeAsset typeAsset = this.Asset as TypeAsset;
        if (typeAsset != null)
          return (ITypeId) typeAsset.Type;
        return (ITypeId) null;
      }
    }

    protected override bool CanInsert
    {
      get
      {
        Asset asset = this.Asset;
        if (asset != null)
          return asset.CanCreateInstance(this.ActiveSceneInsertionPoint);
        return false;
      }
    }

    private bool HasNonZeroAssetSize
    {
      get
      {
        if (!this.AssetSize.IsEmpty && this.AssetSize.Width != 0.0)
          return this.AssetSize.Height != 0.0;
        return false;
      }
    }

    protected override double AspectRatio
    {
      get
      {
        if (this.HasNonZeroAssetSize)
          return this.AssetSize.Width / this.AssetSize.Height;
        return base.AspectRatio;
      }
    }

    protected override bool ShouldConstrainAspectRatio
    {
      get
      {
        if (this.HasNonZeroAssetSize)
          return !this.IsShiftDown;
        return this.IsShiftDown;
      }
    }

    internal AssetToolBehavior(ToolBehaviorContext toolContext, Func<Asset> queryAsset, bool popAfterCreation)
      : base(toolContext, popAfterCreation)
    {
      this.queryAsset = queryAsset;
    }

    protected override SceneNode CreateInstance(Rect position)
    {
      return this.Asset.CreateInstance(this.ActiveSceneViewModel.DesignerContext.LicenseManager, this.ActiveSceneInsertionPoint, position, (OnCreateInstanceAction) null);
    }

    protected override BaseFrameworkElement CreateElementOnStartDrag()
    {
      bool flag = true;
      Asset asset = this.Asset;
      TypeAsset typeAsset = asset as TypeAsset;
      StyleAsset styleAsset = asset as StyleAsset;
      if (typeAsset != null && typeAsset.Type is IUnreferencedType)
        flag = false;
      else if (styleAsset != null)
      {
        UserThemeAssetProvider themeAssetProvider = styleAsset.Provider as UserThemeAssetProvider;
        if (themeAssetProvider != null && !themeAssetProvider.IsLocal)
          flag = false;
      }
      if (flag && asset.CanCreateInstance(this.ActiveSceneInsertionPoint))
        return this.CreateInstance(Rect.Empty) as BaseFrameworkElement;
      return (BaseFrameworkElement) null;
    }

    protected override void DoUpdateElementPosition(Point pointBegin, Point pointEnd)
    {
      if (this.EditingElement != null)
        base.DoUpdateElementPosition(pointBegin, pointEnd);
      else
        this.DrawFeedback(pointBegin, pointEnd);
    }

    protected override void DoFinishElement(Point pointBegin, Point pointEnd)
    {
      Asset asset = this.Asset;
      if (this.EditingElement != null || !asset.CanCreateInstance(this.ActiveSceneInsertionPoint))
        return;
      this.ClearFeedback();
      Rect rect = !(pointBegin != pointEnd) ? new Rect(pointBegin.X, pointBegin.Y, double.PositiveInfinity, double.PositiveInfinity) : new Rect(pointBegin, pointEnd);
      asset.CreateInstance(this.ActiveSceneViewModel.DesignerContext.LicenseManager, this.ActiveSceneInsertionPoint, rect, (OnCreateInstanceAction) null);
    }

    protected override sealed bool OnDragOver(DragEventArgs args)
    {
      return base.OnDragOver(args);
    }

    protected override sealed bool OnDragEnter(DragEventArgs args)
    {
      return base.OnDragEnter(args);
    }

    protected override sealed bool OnDragLeave(DragEventArgs args)
    {
      return base.OnDragLeave(args);
    }

    protected override sealed bool OnDrop(DragEventArgs args)
    {
      return base.OnDrop(args);
    }

    protected override void OnSuspend()
    {
      base.OnSuspend();
      this.ClearFeedback();
    }

    private void DrawFeedback(Point pointBegin, Point pointEnd)
    {
      Rect rect = new Rect(pointBegin, pointEnd);
      this.ToolBehaviorContext.SnappingEngine.UpdateTargetBounds(rect);
      DrawingContext drawingContext = this.OpenFeedback();
      Pen thinPen = FeedbackHelper.GetThinPen(this.ActiveView.Zoom);
      System.Windows.Media.Geometry rectangleGeometry = Adorner.GetTransformedRectangleGeometry(this.ActiveView, this.ActiveSceneInsertionPoint.SceneElement, rect, thinPen.Thickness, false);
      drawingContext.DrawGeometry((Brush) null, thinPen, rectangleGeometry);
      if (!Adorner.NonAffineTransformInParentStack(this.ActiveSceneInsertionPoint.SceneElement))
      {
        Matrix computedTransformToRoot = this.ActiveView.GetComputedTransformToRoot(this.ActiveSceneInsertionPoint.SceneElement);
        double scale = 1.0 / this.ActiveView.Zoom;
        SizeAdorner.DrawDimension(drawingContext, ElementLayoutAdornerType.Right, computedTransformToRoot, computedTransformToRoot, thinPen, rect, scale);
        SizeAdorner.DrawDimension(drawingContext, ElementLayoutAdornerType.Bottom, computedTransformToRoot, computedTransformToRoot, thinPen, rect, scale);
      }
      this.CloseFeedback();
    }
  }
}
