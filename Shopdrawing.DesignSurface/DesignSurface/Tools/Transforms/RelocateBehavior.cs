// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.RelocateBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Designers;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal sealed class RelocateBehavior : DragToolBehavior
  {
    private const double autoScrollMargin = 16.0;
    private SceneElement primarySelection;
    private IList<SceneElement> selectedElements;
    private IList<BaseFrameworkElement> draggedElements;
    private BaseFrameworkElement primaryDraggedElement;
    private Rect primaryDraggedElementOriginalBounds;
    private double primaryDraggedElementBaseline;
    private Point dragStartPosition;
    private Point dragCurrentPosition;
    private bool dragCancelled;
    private bool enableReparenting;
    private MoveStrategy moveStrategy;
    private MoveStrategyContext moveStrategyContext;
    private BaseFrameworkElement dragContainer;

    internal bool DragCancelled
    {
      get
      {
        return this.dragCancelled;
      }
    }

    internal bool EnableReparenting
    {
      get
      {
        return this.enableReparenting;
      }
      set
      {
        this.enableReparenting = value;
      }
    }

    internal Vector DuplicationOffset { get; set; }

    public override string ActionString
    {
      get
      {
        return StringTable.UndoUnitMove;
      }
    }

    public RelocateBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    protected override bool OnButtonDown(Point pointerPosition)
    {
      this.dragCancelled = false;
      return true;
    }

    internal override bool ShouldMotionlessAutoScroll(Point mousePoint, Rect artboardBoundary)
    {
      return RelocateBehavior.BoundaryTest(mousePoint, artboardBoundary, 16.0) == 0;
    }

    protected override MotionlessAutoScroller CreateMotionlessAutoScroller()
    {
      return new MotionlessAutoScroller((ToolBehavior) this, new Func<Point, Point, bool, bool>(this.InternalOnDrag));
    }

    protected override bool OnDrag(Point dragStartPosition, Point dragCurrentPosition, bool scrollNow)
    {
      FrameworkElement frameworkElement = (FrameworkElement) this.ToolBehaviorContext.View.Artboard;
      if (RelocateBehavior.BoundaryTest(Mouse.GetPosition((IInputElement) frameworkElement), new Rect(0.0, 0.0, frameworkElement.ActualWidth, frameworkElement.ActualHeight), 16.0) < 0)
      {
        Mouse.OverrideCursor = ToolCursors.NoDropCursor;
      }
      else
      {
        Mouse.OverrideCursor = (Cursor) null;
        this.InternalOnDrag(dragStartPosition, dragCurrentPosition, scrollNow);
      }
      return true;
    }

    private static int BoundaryTest(Point point, Rect rect, double margin)
    {
      if (!rect.Contains(point))
        return -1;
      return margin <= 0.0 || Rect.Inflate(rect, -margin, -margin).Contains(point) ? true : false;
    }

    private bool InternalOnDrag(Point dragStartPosition, Point dragCurrentPosition, bool scrollNow)
    {
      if (this.dragCancelled)
        return true;
      this.EnsureEditTransaction();
      if (!this.HasMouseMovedAfterDown)
        this.InitializeDrag(dragStartPosition);
      if (this.dragCancelled || this.dragContainer == null)
        return true;
      Point point = dragCurrentPosition;
      dragCurrentPosition = this.SnapPrimarySelectionBounds(dragStartPosition, dragCurrentPosition);
      Point dragStartPosition1 = dragStartPosition;
      this.dragCurrentPosition = dragCurrentPosition;
      Vector vector = this.dragCurrentPosition - this.dragStartPosition;
      this.ActiveView.EnsureVisible(dragStartPosition + vector, scrollNow);
      BaseFrameworkElement hitElement = this.GetHitElement(point, this.draggedElements) ?? this.dragContainer;
      if (this.ActiveSceneViewModel.FindPanelClosestToRoot() != this.dragContainer || hitElement.GetType() == typeof (Viewport3DElement))
        hitElement = this.dragContainer;
      MoveStrategy moveStrategy = MoveStrategyFactory.Create(this.moveStrategyContext, (SceneElement) hitElement, this.IsShiftDown);
      bool flag = this.enableReparenting && this.moveStrategy != null && (moveStrategy != null && moveStrategy.LayoutContainer != null) && moveStrategy.LayoutContainer != this.moveStrategy.LayoutContainer && moveStrategy.LayoutContainer.IsViewObjectValid;
      if (flag && this.IsAltDown)
      {
        this.ProvisionalContainer = (BaseFrameworkElement) null;
        this.ToolBehaviorContext.SnappingEngine.Stop();
        this.moveStrategy.EndDrag(false);
        this.ReplaceSubTransaction();
        this.UpdateEditTransaction();
        if (moveStrategy != null)
        {
          this.moveStrategy = moveStrategy;
          this.moveStrategy.BeginDrag(dragStartPosition1);
          if (!this.moveStrategy.LayoutContainer.IsViewObjectValid)
          {
            this.CancelDrag();
            return true;
          }
          this.moveStrategy.ContinueDrag(dragCurrentPosition, hitElement);
          this.ToolBehaviorContext.SnappingEngine.Start(this.ToolBehaviorContext, this.primaryDraggedElement, this.draggedElements);
        }
      }
      else if (moveStrategy != null)
      {
        if (!flag)
        {
          this.ProvisionalContainer = (BaseFrameworkElement) null;
        }
        else
        {
          this.ProvisionalContainer = moveStrategy.LayoutContainer;
          this.SetTextCuePosition(dragCurrentPosition);
        }
        if (this.moveStrategy == null)
        {
          this.moveStrategy = moveStrategy;
          this.moveStrategy.BeginDrag(dragStartPosition);
          this.ToolBehaviorContext.SnappingEngine.Start(this.ToolBehaviorContext, this.primaryDraggedElement, this.draggedElements);
        }
        this.moveStrategy.ContinueDrag(dragCurrentPosition, hitElement);
      }
      else if (this.moveStrategy != null)
        this.moveStrategy.ContinueDrag(dragCurrentPosition, hitElement);
      this.UpdateEditTransaction();
      if (this.moveStrategy != null && !this.moveStrategy.LayoutContainer.IsViewObjectValid)
        this.CancelDrag();
      return true;
    }

    protected override bool OnDragEnd(Point dragStartPosition, Point dragEndPosition)
    {
      Mouse.OverrideCursor = (Cursor) null;
      bool flag = this.dragCancelled;
      this.ProvisionalContainer = (BaseFrameworkElement) null;
      if (this.dragContainer == null || this.dragCancelled)
      {
        this.PopSelf();
        return true;
      }
      if (this.moveStrategy != null)
      {
        dragEndPosition = this.SnapPrimarySelectionBounds(dragStartPosition, dragEndPosition);
        BaseFrameworkElement layoutContainer = this.moveStrategy.LayoutContainer;
        this.moveStrategy.ContinueDrag(dragEndPosition, layoutContainer);
        if (!this.moveStrategy.EndDrag(true))
        {
          this.CancelEditTransaction();
          flag = true;
        }
        this.ToolBehaviorContext.SnappingEngine.Stop();
        this.moveStrategy = (MoveStrategy) null;
      }
      if (!flag)
      {
        this.ActiveSceneViewModel.ElementSelectionSet.SetSelection((ICollection<SceneElement>) this.selectedElements, this.primarySelection);
        if (this.HasMouseMovedAfterDown)
          this.CommitEditTransaction();
      }
      this.PopSelf();
      return true;
    }

    protected override bool OnClickEnd(Point pointerPosition, int clickCount)
    {
      this.PopSelf();
      return false;
    }

    protected override bool OnKey(KeyEventArgs args)
    {
      if (args.IsRepeat || this.dragCancelled)
        return false;
      if (args.IsDown && args.Key == Key.Escape)
      {
        this.CancelDrag();
        return true;
      }
      Key key = args.Key == Key.System ? args.SystemKey : args.Key;
      if (this.moveStrategy != null && (key == Key.LeftShift || key == Key.RightShift))
      {
        this.moveStrategy.IsConstraining = this.IsShiftDown;
        return true;
      }
      if (!this.HasMouseMovedAfterDown || !args.IsDown || key != Key.LeftAlt && key != Key.RightAlt)
        return false;
      this.OnDrag(this.dragStartPosition, this.dragCurrentPosition, false);
      return true;
    }

    private void InitializeDrag(Point dragStartPosition)
    {
      SceneViewModel viewModel = this.ActiveView.ViewModel;
      this.CreateSubTransaction();
      this.dragStartPosition = dragStartPosition;
      this.ComputeDraggedElements();
      if (this.draggedElements == null || this.draggedElements.Count == 0)
      {
        this.moveStrategy = (MoveStrategy) null;
        this.dragContainer = (BaseFrameworkElement) null;
        this.CancelDrag();
      }
      else
      {
        this.moveStrategy = (MoveStrategy) null;
        this.moveStrategyContext = MoveStrategyContext.FromSelection((ToolBehavior) this, this.primarySelection, this.selectedElements, this.draggedElements, this.DuplicationOffset, this.dragStartPosition);
        this.dragContainer = (BaseFrameworkElement) null;
        bool flag = false;
        SceneElement sceneElement1 = (SceneElement) null;
        foreach (SceneElement sceneElement2 in (IEnumerable<SceneElement>) this.selectedElements)
        {
          BaseFrameworkElement frameworkElement = (BaseFrameworkElement) null;
          BaseFrameworkElement editingContainer = viewModel.FindPanelClosestToActiveEditingContainer();
          if (editingContainer != null && editingContainer.IsAncestorOf((SceneNode) sceneElement2))
            frameworkElement = editingContainer;
          if (!flag)
          {
            flag = true;
            this.dragContainer = frameworkElement;
            sceneElement1 = sceneElement2.ParentElement;
          }
          else if (sceneElement2.ParentElement != sceneElement1)
          {
            this.dragContainer = (BaseFrameworkElement) null;
            break;
          }
          if (frameworkElement != this.dragContainer)
          {
            this.dragContainer = (BaseFrameworkElement) null;
            break;
          }
        }
        if (this.dragContainer != null)
        {
          this.moveStrategy = MoveStrategyFactory.Create(this.moveStrategyContext, this.draggedElements[0].ParentElement, this.IsShiftDown);
          if (this.moveStrategy != null)
          {
            this.moveStrategy.BeginDrag(dragStartPosition);
            this.ToolBehaviorContext.SnappingEngine.Start(this.ToolBehaviorContext, this.primaryDraggedElement, this.draggedElements);
          }
          else
            this.CancelDrag();
        }
        else
          this.CancelDrag();
      }
    }

    private void CancelDrag()
    {
      if (this.moveStrategy != null)
      {
        this.moveStrategy.EndDrag(false);
        this.moveStrategy = (MoveStrategy) null;
      }
      this.ProvisionalContainer = (BaseFrameworkElement) null;
      this.ToolBehaviorContext.SnappingEngine.Stop();
      this.CancelEditTransaction();
      this.dragCancelled = true;
    }

    private void ComputeDraggedElements()
    {
      SceneViewModel viewModel = this.ActiveView.ViewModel;
      IList<SceneElement> list1 = (IList<SceneElement>) new List<SceneElement>((IEnumerable<SceneElement>) viewModel.ElementSelectionSet.Selection);
      SceneElement sceneElement1 = viewModel.ElementSelectionSet.PrimarySelection;
      List<BaseFrameworkElement> list2 = new List<BaseFrameworkElement>();
      foreach (SceneElement sceneElement2 in (IEnumerable<SceneElement>) list1)
      {
        BaseFrameworkElement frameworkElement1 = sceneElement2 as BaseFrameworkElement;
        if (frameworkElement1 != null && sceneElement2 != viewModel.ViewRoot && sceneElement2.IsViewObjectValid)
        {
          bool flag = true;
          for (int index = list2.Count - 1; index >= 0; --index)
          {
            BaseFrameworkElement frameworkElement2 = list2[index];
            if (frameworkElement2.IsAncestorOf((SceneNode) frameworkElement1))
            {
              if (sceneElement1 == frameworkElement1)
                sceneElement1 = (SceneElement) frameworkElement2;
              flag = false;
              break;
            }
            if (frameworkElement1.IsAncestorOf((SceneNode) frameworkElement2))
            {
              if (sceneElement1 == frameworkElement2)
                sceneElement1 = (SceneElement) frameworkElement1;
              list2.RemoveAt(index);
            }
          }
          if (flag)
            list2.Add(frameworkElement1);
        }
      }
      this.primarySelection = viewModel.ElementSelectionSet.PrimarySelection;
      this.selectedElements = list1;
      if (list2.Count == 0)
      {
        this.draggedElements = (IList<BaseFrameworkElement>) null;
      }
      else
      {
        this.draggedElements = (IList<BaseFrameworkElement>) list2;
        this.primaryDraggedElement = sceneElement1 as BaseFrameworkElement;
        if (this.primaryDraggedElement == null)
          this.primaryDraggedElement = this.draggedElements[0];
        this.primaryDraggedElementOriginalBounds = this.ActiveView.GetActualBoundsInParent(this.primaryDraggedElement.Visual);
        this.primaryDraggedElementBaseline = this.ActiveView.GetBaseline((SceneNode) this.primaryDraggedElement);
      }
    }

    private Point SnapPrimarySelectionBounds(Point dragStartPosition, Point dragCurrentPosition)
    {
      Vector vector = this.ToolBehaviorContext.SnappingEngine.SnapRect(this.primaryDraggedElementOriginalBounds, this.primaryDraggedElement.VisualElementAncestor, dragCurrentPosition - dragStartPosition, EdgeFlags.All, this.primaryDraggedElementBaseline);
      return dragCurrentPosition + vector;
    }
  }
}
