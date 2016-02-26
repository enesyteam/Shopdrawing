// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.ElementDragCreateBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Designers;
using Microsoft.Expression.DesignSurface.Tools.Layout;
using Microsoft.Expression.DesignSurface.Tools.Text;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal abstract class ElementDragCreateBehavior : ElementCreateBehavior
  {
    private LayoutRoundingOverride layoutRoundingOverride = new LayoutRoundingOverride();
    private Point dragStartPosition;
    private Point dragCurrentPosition;
    private bool isConstrained;
    private bool isCentered;
    private bool popBehaviorAfterClick;

    protected BaseFrameworkElement EditingElement { get; set; }

    protected BaseFrameworkElement LayoutTargetElement
    {
      get
      {
        return DefaultTypeInstantiator.GetLayoutTarget((SceneNode) this.EditingElement) as BaseFrameworkElement ?? this.EditingElement;
      }
    }

    protected BaseFrameworkElement SelectionTargetElement
    {
      get
      {
        return DefaultTypeInstantiator.GetLayoutTarget((SceneNode) this.EditingElement) as BaseFrameworkElement ?? this.EditingElement;
      }
    }

    public override string ActionString
    {
      get
      {
        return this.Tool.Caption;
      }
    }

    public override bool UseDefaultEditingAdorners
    {
      get
      {
        return !this.IsDragging;
      }
    }

    protected virtual double AspectRatio
    {
      get
      {
        return 1.0;
      }
    }

    protected virtual bool ShouldConstrainAspectRatio
    {
      get
      {
        return this.IsShiftDown;
      }
    }

    protected virtual bool ShouldCenter
    {
      get
      {
        return this.IsAltDown;
      }
    }

    protected virtual bool CanInsert
    {
      get
      {
        if (this.ActiveSceneInsertionPoint != null && this.ActiveSceneInsertionPoint.SceneElement != null && this.ActiveSceneInsertionPoint.SceneElement.IsViewObjectValid)
          return this.ActiveSceneInsertionPoint.CanInsert(this.InstanceType);
        return false;
      }
    }

    internal ElementDragCreateBehavior(ToolBehaviorContext toolContext, bool popBehaviorAfterClick)
      : base(toolContext)
    {
      this.popBehaviorAfterClick = popBehaviorAfterClick;
    }

    protected abstract BaseFrameworkElement CreateElementOnStartDrag();

    protected override bool OnButtonDownOverNonAdorner(Point pointerPosition)
    {
      if (this.CanInsert)
      {
        this.ToolBehaviorContext.SnappingEngine.Start(this.ToolBehaviorContext, (BaseFrameworkElement) null, (IList<BaseFrameworkElement>) null);
        this.dragStartPosition = this.ToolBehaviorContext.SnappingEngine.SnapPoint(pointerPosition, EdgeFlags.All);
        this.isConstrained = this.ShouldConstrainAspectRatio;
        this.isCentered = this.ShouldCenter;
      }
      return true;
    }

    protected override sealed bool OnDrag(Point dragStartPosition, Point dragCurrentPosition, bool scrollNow)
    {
      if (this.CanInsert)
      {
        PerformanceUtility.StartPerformanceSequence(PerformanceEvent.ShapeCreateBehaviorOnDrag);
        if (!this.IsEditTransactionOpen)
        {
          GridElement gridElement = this.ActiveSceneInsertionPoint.SceneElement as GridElement;
          if (gridElement != null)
          {
            gridElement.CacheComputedColumnWidths();
            gridElement.CacheComputedRowHeights();
          }
          this.EnsureEditTransaction();
          this.Tool.ShowDimensions = true;
          this.EditingElement = this.CreateElementOnStartDrag();
          this.OnApplyAmbientProperty((SceneNode) this.EditingElement);
          this.CreateSubTransaction();
          if (!this.CanInsert)
            return false;
          if (this.EditingElement != null)
          {
            this.ToolBehaviorContext.SnappingEngine.Start(this.ToolBehaviorContext, this.EditingElement, (IList<BaseFrameworkElement>) null);
            this.layoutRoundingOverride.SetValue((IEnumerable<SceneElement>) new SceneElement[1]
            {
              (SceneElement) this.EditingElement
            }, 0 != 0);
          }
          else
            this.CancelEditTransaction();
        }
        this.SnapDragPosition(dragCurrentPosition);
        this.UpdateElementPosition(scrollNow);
        PerformanceUtility.EndPerformanceSequence(PerformanceEvent.ShapeCreateBehaviorOnDrag);
      }
      return true;
    }

    private void SnapDragPosition(Point dragCurrentPosition)
    {
      if (!this.ActiveSceneInsertionPoint.SceneNode.IsViewObjectValid || this.ActiveSceneInsertionPoint.SceneElement.Visual == null)
        return;
      SceneView activeView = this.ActiveView;
      Point location = activeView.TransformPoint((IViewObject) activeView.HitTestRoot, this.ActiveSceneInsertionPoint.SceneElement.Visual, this.dragStartPosition);
      Point point = activeView.TransformPoint((IViewObject) activeView.HitTestRoot, this.ActiveSceneInsertionPoint.SceneElement.Visual, dragCurrentPosition);
      EdgeFlags edgeFlags = (EdgeFlags) ((location.X <= point.X ? 4 : 1) | (location.Y <= point.Y ? 8 : 2));
      Rect rect = new Rect(location, new Size(0.0, 0.0));
      Vector offset = dragCurrentPosition - this.dragStartPosition;
      SceneElement container = this.EditingElement == null || !this.EditingElement.IsViewObjectValid ? this.ActiveSceneInsertionPoint.SceneElement : this.EditingElement.VisualElementAncestor ?? (SceneElement) this.EditingElement;
      Vector vector = this.ToolBehaviorContext.SnappingEngine.SnapRect(rect, container, offset, edgeFlags);
      this.dragCurrentPosition = dragCurrentPosition + vector;
    }

    protected override sealed bool OnDragEnd(Point dragStartPosition, Point dragEndPosition)
    {
      GridElement gridElement = this.ActiveSceneInsertionPoint.SceneElement as GridElement;
      if (gridElement != null)
      {
        gridElement.UncacheComputedColumnWidths();
        gridElement.UncacheComputedRowHeights();
      }
      if (this.CanInsert)
      {
        this.Tool.ShowDimensions = false;
        this.FinishElement();
        this.ToolBehaviorContext.SnappingEngine.Stop();
        this.PreviewInsertionPoint = (ISceneInsertionPoint) null;
      }
      if (this.popBehaviorAfterClick)
        this.TryPopSelf();
      return true;
    }

    private void FinishElement()
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.ShapeCreateBehaviorOnDragEnd);
      this.layoutRoundingOverride.Restore(!(this is TextCreateBehavior));
      Point pointBegin;
      Point pointEnd;
      this.GetCurrentElementPosition(out pointBegin, out pointEnd);
      this.DoFinishElement(pointBegin, pointEnd);
      this.CommitEditTransaction();
      this.Tool.RebuildAdornerSets();
      this.EditingElement = (BaseFrameworkElement) null;
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.ShapeCreateBehaviorOnDragEnd);
    }

    protected virtual void DoFinishElement(Point pointBegin, Point pointEnd)
    {
    }

    protected override bool OnClickEnd(Point pointerPosition, int clickCount)
    {
      if (this.CanInsert)
      {
        if (!this.IsEditTransactionOpen && !this.IsControlDown && (!this.IsShiftDown && clickCount == 2))
        {
          PerformanceUtility.StartPerformanceSequence(PerformanceEvent.ShapeCreateBehaviorOnDrag);
          this.EnsureEditTransaction();
          this.Tool.ShowDimensions = false;
          this.SnapDragPosition(this.dragStartPosition);
          SceneView activeView = this.ActiveView;
          Point point = activeView.TransformPoint((IViewObject) activeView.HitTestRoot, this.ActiveSceneInsertionPoint.SceneElement.Visual, this.dragStartPosition);
          Rect position = new Rect(point.X, point.Y, double.PositiveInfinity, double.PositiveInfinity);
          if (this.InstanceType != null)
          {
            this.EditingElement = this.CreateInstance(position) as BaseFrameworkElement;
            this.UpdateEditTransaction();
            this.EditingElement.ViewModel.DefaultView.UpdateLayout();
            if (this.EditingElement != null)
            {
              this.ToolBehaviorContext.SnappingEngine.Start(this.ToolBehaviorContext, this.EditingElement, (IList<BaseFrameworkElement>) null);
              this.SnapDragPosition(this.dragStartPosition);
              this.OnApplyAmbientProperty((SceneNode) this.EditingElement);
            }
            else
              this.CancelEditTransaction();
          }
          this.FinishElement();
          this.ToolBehaviorContext.SnappingEngine.Stop();
          this.PreviewInsertionPoint = (ISceneInsertionPoint) null;
          PerformanceUtility.EndPerformanceSequence(PerformanceEvent.ShapeCreateBehaviorOnDrag);
        }
        else
          this.ToolBehaviorContext.SnappingEngine.Stop();
      }
      bool flag = base.OnClickEnd(pointerPosition, clickCount);
      if (this.popBehaviorAfterClick)
        this.TryPopSelf();
      return flag;
    }

    protected virtual SceneNode CreateInstance(Rect position)
    {
      return new UserThemeTypeInstantiator(this.ActiveView).CreateInstance(this.InstanceType, this.ActiveSceneInsertionPoint, position, (OnCreateInstanceAction) null);
    }

    protected override sealed bool OnKey(KeyEventArgs args)
    {
      if (!this.IsDragging)
        return base.OnKey(args);
      bool constrainAspectRatio = this.ShouldConstrainAspectRatio;
      bool shouldCenter = this.ShouldCenter;
      if (this.isConstrained != constrainAspectRatio || this.isCentered != shouldCenter)
      {
        this.isConstrained = constrainAspectRatio;
        this.isCentered = shouldCenter;
        this.UpdateElementPosition(true);
      }
      return true;
    }

    protected override void OnDetach()
    {
      this.CommitEditTransaction();
      base.OnDetach();
    }

    protected virtual void ConstrainElementPosition(Point pointBegin, ref Point pointEnd, ref Vector diagonal)
    {
      double num = Math.Max(Math.Abs(diagonal.X) / this.AspectRatio, Math.Abs(diagonal.Y));
      diagonal = new Vector((diagonal.X < 0.0 ? -1.0 : 1.0) * num * this.AspectRatio, (diagonal.Y < 0.0 ? -1.0 : 1.0) * num);
      pointEnd = pointBegin + diagonal;
    }

    private void GetCurrentElementPosition(out Point pointBegin, out Point pointEnd)
    {
      SceneView activeView = this.ActiveView;
      IViewObject visual = this.ActiveSceneInsertionPoint.SceneElement.Visual;
      pointBegin = visual == null ? this.dragStartPosition : activeView.TransformPoint((IViewObject) activeView.HitTestRoot, visual, this.dragStartPosition);
      pointEnd = visual == null ? this.dragCurrentPosition : activeView.TransformPoint((IViewObject) activeView.HitTestRoot, visual, this.dragCurrentPosition);
      Vector diagonal = pointEnd - pointBegin;
      if (this.isConstrained)
        this.ConstrainElementPosition(pointBegin, ref pointEnd, ref diagonal);
      if (!this.isCentered)
        return;
      pointBegin -= diagonal;
    }

    private void UpdateElementPosition(bool scrollNow)
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.ShapeCreateBehaviorUpdateShapeIfNeededAfterCreation);
      Point pointBegin;
      Point pointEnd;
      this.GetCurrentElementPosition(out pointBegin, out pointEnd);
      if (this.EditingElement == null || this.EditingElement.IsViewObjectValid && this.EditingElement.Visual != null)
      {
        using (this.ActiveSceneViewModel.ForceBaseValue())
          this.DoUpdateElementPosition(pointBegin, pointEnd);
        if (this.IsEditTransactionOpen)
          this.UpdateEditTransaction();
        this.ActiveView.EnsureVisible(this.dragCurrentPosition, scrollNow);
      }
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.ShapeCreateBehaviorUpdateShapeIfNeededAfterCreation);
    }

    protected virtual void DoUpdateElementPosition(Point pointBegin, Point pointEnd)
    {
      IViewVisual viewVisual1 = this.ActiveSceneInsertionPoint.SceneElement.Visual as IViewVisual;
      IViewVisual viewVisual2 = this.LayoutTargetElement.Visual as IViewVisual;
      IViewVisual visual = viewVisual2 != null ? viewVisual2.VisualParent as IViewVisual : (IViewVisual) null;
      if (visual != null && viewVisual1 != visual && (viewVisual1 != null && this.ActiveSceneViewModel.ProjectContext.ResolveType(PlatformTypes.UIElement).RuntimeType.IsAssignableFrom(visual.TargetType)))
      {
        GeneralTransform generalTransform = viewVisual1.TransformToVisual(visual);
        pointBegin = generalTransform.Transform(pointBegin);
        pointEnd = generalTransform.Transform(pointEnd);
      }
      double width = Math.Abs(pointEnd.X - pointBegin.X);
      double height = Math.Abs(pointEnd.Y - pointBegin.Y);
      this.ReplaceSubTransaction();
      ILayoutDesigner designerForChild = this.ActiveSceneViewModel.GetLayoutDesignerForChild((SceneElement) this.LayoutTargetElement, true);
      Vector vector = new Vector(Math.Min(pointBegin.X, pointEnd.X), Math.Min(pointBegin.Y, pointEnd.Y));
      Rect rect = new Rect(vector.X, vector.Y, width, height);
      designerForChild.SetChildRect(this.LayoutTargetElement, rect, LayoutOverrides.None, LayoutOverrides.Width | LayoutOverrides.Height, LayoutOverrides.None, SetRectMode.CreateAtPosition);
    }

    protected virtual void OnApplyAmbientProperty(SceneNode node)
    {
    }
  }
}
