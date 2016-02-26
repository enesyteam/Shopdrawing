// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.ElementCreateBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal class ElementCreateBehavior : ElementToolBehavior
  {
    private InsertionPointHighlighter previewHighlighter;
    private bool cachedNoInvisiblePanelStrokeHitTesting;
    private bool cacheInvisiblePanelContainerOfSelectionDisable;

    protected virtual ITypeId InstanceType
    {
      get
      {
        return (ITypeId) null;
      }
    }

    protected virtual bool ShowPreviewHighlightOnHover
    {
      get
      {
        return true;
      }
    }

    protected ISceneInsertionPoint PreviewInsertionPoint
    {
      get
      {
        return this.previewHighlighter.InsertionPointPreview;
      }
      set
      {
        if (value != null && this.previewHighlighter.InsertionPointPreview != null && this.previewHighlighter.InsertionPointPreview.SceneNode == value.SceneNode)
          return;
        this.previewHighlighter.InsertionPointPreview = value;
        this.OnPreviewInsertionPointChanged();
      }
    }

    internal ElementCreateBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
      this.previewHighlighter = new InsertionPointHighlighter(toolContext);
    }

    public override void SetLocalActiveSceneInsertionPoint(Point point)
    {
      this.LocalActiveSceneInsertionPoint = this.ActiveSceneViewModel.GetActiveSceneInsertionPointFromPosition(new InsertionPointContext(point, this.InstanceType));
    }

    protected virtual void OnPreviewInsertionPointChanged()
    {
    }

    protected override bool OnHoverExit()
    {
      this.PreviewInsertionPoint = (ISceneInsertionPoint) null;
      return base.OnHoverExit();
    }

    protected override bool OnHoverOverAdorner(IAdorner adorner)
    {
      this.PreviewInsertionPoint = (ISceneInsertionPoint) null;
      return base.OnHoverOverAdorner(adorner);
    }

    public static ISceneInsertionPoint GetInsertionPointToPreview(SceneViewModel viewModel, InsertionPointContext insertionPointContext)
    {
      return viewModel.GetActiveSceneInsertionPointFromPosition(insertionPointContext) ?? (ISceneInsertionPoint) null;
    }

    protected void UpdatePreviewElement(Point pointerPosition)
    {
      if (this.ShowPreviewHighlightOnHover)
        this.PreviewInsertionPoint = ElementCreateBehavior.GetInsertionPointToPreview(this.ActiveSceneViewModel, new InsertionPointContext(pointerPosition, this.InstanceType));
      else
        this.PreviewInsertionPoint = (ISceneInsertionPoint) null;
    }

    protected override bool OnHoverOverNonAdorner(Point pointerPosition)
    {
      if (!this.IsOnlyUpdatingCursor)
        this.UpdatePreviewElement(pointerPosition);
      return base.OnHoverOverNonAdorner(pointerPosition);
    }

    protected override void OnSuspend()
    {
      this.PreviewInsertionPoint = (ISceneInsertionPoint) null;
      this.ActiveView.NoInvisiblePanelStrokeHitTesting = this.cachedNoInvisiblePanelStrokeHitTesting;
      this.ActiveView.InvisiblePanelContainerOfSelectionDisable = this.cacheInvisiblePanelContainerOfSelectionDisable;
      base.OnSuspend();
    }

    protected override void OnResume()
    {
      this.ActiveView.NoInvisiblePanelStrokeHitTesting = true;
      this.ActiveView.InvisiblePanelContainerOfSelectionDisable = true;
      base.OnResume();
    }

    protected override void OnAttach()
    {
      this.cachedNoInvisiblePanelStrokeHitTesting = this.ActiveView.NoInvisiblePanelStrokeHitTesting;
      this.cacheInvisiblePanelContainerOfSelectionDisable = this.ActiveView.InvisiblePanelContainerOfSelectionDisable;
      this.ActiveView.NoInvisiblePanelStrokeHitTesting = true;
      this.ActiveView.InvisiblePanelContainerOfSelectionDisable = true;
      base.OnAttach();
    }

    protected override void OnDetach()
    {
      this.PreviewInsertionPoint = (ISceneInsertionPoint) null;
      this.ActiveView.NoInvisiblePanelStrokeHitTesting = this.cachedNoInvisiblePanelStrokeHitTesting;
      this.ActiveView.InvisiblePanelContainerOfSelectionDisable = this.cacheInvisiblePanelContainerOfSelectionDisable;
      base.OnDetach();
    }
  }
}
