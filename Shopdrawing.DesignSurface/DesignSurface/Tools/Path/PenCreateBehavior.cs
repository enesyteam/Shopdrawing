// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.PenCreateBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  internal class PenCreateBehavior : PathCreateBehavior
  {
    private PenAction action;
    private bool isConstrainingAxes;
    private PathEditorTarget pathEditorTarget;
    private PathEditContext pathEditContext;

    internal bool IsActive
    {
      get
      {
        return this.ActiveDocument != null && this.EditingElement != null && (this.EditingElement.IsAttached && this.ActiveSceneViewModel.ElementSelectionSet.IsSelected((SceneElement) this.EditingElement));
      }
    }

    protected override Cursor DefaultCursor
    {
      get
      {
        PenAction penAction = this.action ?? this.GetAction((PathPartAdorner) null);
        if (penAction is StartAction && this.IsProjectedInsertionPoint)
          return ToolCursors.NoDropCursor;
        return penAction.HoverCursor;
      }
    }

    protected override bool ShowPreviewHighlightOnHover
    {
      get
      {
        return this.action == null && this.GetAction((PathPartAdorner) null) is StartAction;
      }
    }

    public PenCreateBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    public void DeleteLastSegment()
    {
      if (this.pathEditorTarget == null)
        return;
      this.pathEditorTarget.UpdateCachedPath();
      if (this.pathEditorTarget.PathGeometry == null || this.pathEditContext.FigureIndex >= this.pathEditorTarget.PathGeometry.Figures.Count)
        return;
      this.BeginAction((PenAction) new DeleteLastSegmentAction(this.pathEditorTarget, this.ActiveSceneViewModel), (PathPartAdorner) null);
      this.EndAction();
    }

    protected override bool OnHoverOverAdorner(IAdorner adorner)
    {
      PathPartAdorner adorner1 = adorner as PathPartAdorner;
      if (adorner1 == null)
        return base.OnHoverOverAdorner(adorner);
      this.Cursor = this.GetAction(adorner1).HoverCursor;
      this.PreviewInsertionPoint = (ISceneInsertionPoint) null;
      return true;
    }

    protected override bool OnButtonDownOverAdorner(IAdorner adorner)
    {
      this.EndAction();
      if (!this.ToolBehaviorContext.SnappingEngine.IsStarted)
        this.ToolBehaviorContext.SnappingEngine.Start(this.ToolBehaviorContext, (BaseFrameworkElement) null, (IList<BaseFrameworkElement>) null);
      PathPartAdorner pathPartAdorner = adorner as PathPartAdorner;
      if (pathPartAdorner == null)
        return base.OnButtonDownOverAdorner(adorner);
      PenAction action = this.GetAction(pathPartAdorner);
      SceneElement sceneElement = (SceneElement) this.EditingElement;
      this.pathEditorTarget = pathPartAdorner.PathAdornerSet.PathEditorTarget;
      if (this.pathEditorTarget != null)
        this.EditingElement = (BaseFrameworkElement) this.pathEditorTarget.EditingElement;
      if (sceneElement != null)
        this.ActiveSceneViewModel.DefaultView.AdornerLayer.InvalidateAdornerVisuals(sceneElement);
      this.BeginAction(action, pathPartAdorner);
      return true;
    }

    protected override bool OnButtonDownOverNonAdorner(Point pointerPosition)
    {
      this.IsProjectedInsertionPoint = Adorner.NonAffineTransformInParentStack(this.ActiveSceneInsertionPoint.SceneElement);
      if ((this.action ?? this.GetAction((PathPartAdorner) null)) is StartAction && this.IsProjectedInsertionPoint)
        return true;
      this.EndAction();
      if (!this.ToolBehaviorContext.SnappingEngine.IsStarted)
        this.ToolBehaviorContext.SnappingEngine.Start(this.ToolBehaviorContext, (BaseFrameworkElement) null, (IList<BaseFrameworkElement>) null);
      FrameworkElement frameworkElement = this.MouseDevice.DirectlyOver as FrameworkElement;
      if (frameworkElement == null || frameworkElement.Parent == null)
        return true;
      ActivePathEditInformation pathEditInformation = ((PenTool) this.Tool).ActivePathEditInformation;
      if (pathEditInformation != null)
      {
        this.pathEditorTarget = pathEditInformation.ActivePathEditorTarget;
        this.pathEditContext = new PathEditContext(pathEditInformation.ActiveFigureIndex, 0);
      }
      else
        this.pathEditorTarget = (PathEditorTarget) null;
      if (this.pathEditorTarget != null)
      {
        this.EditingElement = (BaseFrameworkElement) this.pathEditorTarget.EditingElement;
        this.pathEditorTarget.UpdateCachedPath();
      }
      this.BeginAction(this.GetAction((PathPartAdorner) null), (PathPartAdorner) null);
      return true;
    }

    protected override MotionlessAutoScroller CreateMotionlessAutoScroller()
    {
      return new MotionlessAutoScroller((ToolBehavior) this, new Func<Point, Point, bool, bool>(this.HandleDragCallback));
    }

    private bool HandleDragCallback(Point dragStartPosition, Point dragCurrentPosition, bool scrollNow)
    {
      if (this.action != null && this.action.IsActive)
      {
        this.action.Drag(this.MouseDevice, this.ActiveView.Zoom);
        this.ActiveView.EnsureVisible(dragCurrentPosition, scrollNow);
      }
      this.UpdateCursor();
      return true;
    }

    protected override bool OnDrag(Point dragStartPosition, Point dragCurrentPosition, bool scrollNow)
    {
      this.HandleDragCallback(dragStartPosition, dragCurrentPosition, scrollNow);
      if (this.action == null && this.IsAltDown)
        this.PushBehavior((ToolBehavior) new ConvertAnchorBehavior(this.ToolBehaviorContext));
      return true;
    }

    protected override bool OnDragEnd(Point dragStartPosition, Point dragEndPosition)
    {
      return this.AllDone();
    }

    protected override bool OnClickEnd(Point pointerPosition, int clickCount)
    {
      return this.AllDone();
    }

    private bool AllDone()
    {
      this.ToolBehaviorContext.SnappingEngine.Stop();
      this.EndAction();
      this.UpdateCursor();
      if (this.action == null && this.IsAltDown)
        this.PushBehavior((ToolBehavior) new ConvertAnchorBehavior(this.ToolBehaviorContext));
      return true;
    }

    protected override bool OnKey(KeyEventArgs args)
    {
      if (this.action == null && args.IsDown && args.Key == Key.System && (args.SystemKey == Key.LeftAlt || args.SystemKey == Key.RightAlt))
      {
        this.PushBehavior((ToolBehavior) new ConvertAnchorBehavior(this.ToolBehaviorContext));
        return true;
      }
      if (!this.IsActive)
        return base.OnKey(args);
      bool flag = false;
      if (this.action != null)
      {
        bool isShiftDown = this.IsShiftDown;
        if (this.isConstrainingAxes != isShiftDown)
        {
          this.isConstrainingAxes = isShiftDown;
          this.action.Drag(this.MouseDevice, this.ActiveView.Zoom);
        }
        flag = true;
      }
      this.UpdateCursor();
      if (args.Key == Key.Return)
      {
        this.Tool.Deactivate();
        this.Tool.Activate();
        flag = true;
      }
      if (!flag)
        return base.OnKey(args);
      return true;
    }

    protected override void OnAttach()
    {
      base.OnAttach();
      this.ActiveSceneViewModel.EarlySceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.ActiveSceneViewModel_EarlySceneUpdatePhase);
    }

    protected override void OnDetach()
    {
      this.ActiveSceneViewModel.EarlySceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.ActiveSceneViewModel_EarlySceneUpdatePhase);
      if (this.IsActive && this.action != null)
        this.EndAction();
      if (this.EditingElement != null)
        this.FinalizePath(this.EditingElement);
      this.pathEditorTarget = (PathEditorTarget) null;
      this.pathEditContext = (PathEditContext) null;
      base.OnDetach();
    }

    private void ActiveSceneViewModel_EarlySceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      if (!args.IsDirtyViewState(SceneViewModel.ViewStateBits.ElementSelection) || this.action != null || (this.EditingElement == null || this.ActiveDocument == null) || this.EditingElement.IsAttached && this.ActiveSceneViewModel.ElementSelectionSet != null && this.ActiveSceneViewModel.ElementSelectionSet.IsSelected((SceneElement) this.EditingElement))
        return;
      this.OnDetach();
      this.OnAttach();
    }

    private void FinalizePath(BaseFrameworkElement editingElement)
    {
      if (editingElement == null || !editingElement.IsAttached || editingElement != this.EditingElement)
        return;
      this.EditingElement = (BaseFrameworkElement) null;
    }

    private void BeginAction(PenAction action, PathPartAdorner pathPartAdorner)
    {
      if (action == null)
        return;
      if (this.IsActive && action is StartAction)
        this.FinalizePath(this.EditingElement);
      PathEditContext pathEditContext = this.pathEditContext;
      if (pathPartAdorner != null)
        pathEditContext = new PathEditContext(pathPartAdorner.FigureIndex, pathPartAdorner.PartIndex);
      this.action = action;
      this.Cursor = action.DragCursor;
      this.isConstrainingAxes = this.IsShiftDown;
      action.Begin(pathEditContext, this.MouseDevice);
      this.pathEditorTarget = action.PathEditorTarget;
      this.pathEditContext = action.PathEditContext;
      if (this.pathEditorTarget != null)
        this.EditingElement = (BaseFrameworkElement) this.pathEditorTarget.EditingElement;
      if (!action.SetPathActive || this.pathEditorTarget == null || this.pathEditContext == null || this.EditingElement != null && !this.EditingElement.IsViewObjectValid)
        return;
      ((PenTool) this.Tool).ActivePathEditInformation = new ActivePathEditInformation(this.pathEditorTarget, this.pathEditContext.FigureIndex);
    }

    private void EndAction()
    {
      if (this.action == null)
        return;
      this.action.End();
      this.pathEditorTarget = this.action.PathEditorTarget;
      this.pathEditContext = this.action.PathEditContext;
      if (this.pathEditorTarget != null)
        this.EditingElement = (BaseFrameworkElement) this.pathEditorTarget.EditingElement;
      if (this.action.SetPathActive && this.pathEditorTarget != null && this.pathEditContext != null && (this.EditingElement == null || this.EditingElement.IsViewObjectValid))
        ((PenTool) this.Tool).ActivePathEditInformation = new ActivePathEditInformation(this.pathEditorTarget, this.pathEditContext.FigureIndex);
      this.action = (PenAction) null;
    }

    private PenAction GetAction(PathPartAdorner adorner)
    {
      PenAction penAction = (PenAction) null;
      PathSegmentAdorner pathSegmentAdorner = adorner as PathSegmentAdorner;
      PathEditorTarget pathEditorTarget = (PathEditorTarget) null;
      if (adorner != null)
        pathEditorTarget = adorner.PathAdornerSet.PathEditorTarget;
      if (pathSegmentAdorner != null)
      {
        penAction = (PenAction) new InsertAction(pathEditorTarget, this.ActiveSceneViewModel);
      }
      else
      {
        PathPointAdorner pathPointAdorner = adorner as PathPointAdorner;
        if (pathPointAdorner != null)
        {
          penAction = (PenAction) new DeletePointAction(pathEditorTarget, this.ActiveSceneViewModel);
          PathFigure figure1 = pathEditorTarget.PathGeometry.Figures[pathPointAdorner.FigureIndex];
          if (PathFigureUtilities.IsOpen(figure1) && !PathFigureUtilities.IsIsolatedPoint(figure1))
          {
            bool flag1 = pathPointAdorner.PointIndex == 0;
            bool flag2 = pathPointAdorner.PointIndex == PathFigureUtilities.PointCount(figure1) - 1;
            bool flag3 = pathPointAdorner.PathAdornerSet.PathEditorTarget == this.pathEditorTarget;
            bool flag4 = this.pathEditContext != null && pathPointAdorner.FigureIndex == this.pathEditContext.FigureIndex;
            bool flag5 = this.pathEditorTarget != null && pathPointAdorner.PathAdornerSet.PathEditorTarget.PathEditMode == this.pathEditorTarget.PathEditMode;
            bool flag6 = pathPointAdorner.PathAdornerSet.PathEditorTarget.PathEditMode == PathEditMode.ScenePath;
            PathFigure figure2 = this.pathEditContext == null || this.pathEditorTarget == null ? (PathFigure) null : this.pathEditContext.GetPathFigure(this.pathEditorTarget.PathGeometry);
            bool flag7 = figure2 != null && PathFigureUtilities.IsOpen(figure2);
            bool flag8 = (flag3 && !flag4 || flag5 && flag6) && flag7;
            if (flag1)
              penAction = !flag3 || !flag4 ? (!flag8 ? (PenAction) new ExtendAction(pathEditorTarget, this.ActiveSceneViewModel) : (PenAction) new JoinAction(pathEditorTarget, this.ActiveSceneViewModel, this.pathEditorTarget, this.pathEditContext)) : (PenAction) new CloseAction(pathEditorTarget, this.ActiveSceneViewModel);
            else if (flag2)
              penAction = !flag3 || !flag4 ? (!flag8 ? (PenAction) new ExtendAction(pathEditorTarget, this.ActiveSceneViewModel) : (PenAction) new JoinAction(pathEditorTarget, this.ActiveSceneViewModel, this.pathEditorTarget, this.pathEditContext)) : (PenAction) new AdjustAction(pathEditorTarget, this.ActiveSceneViewModel);
          }
          if (PathFigureUtilities.IsIsolatedPoint(figure1) && this.pathEditContext == null)
            penAction = (PenAction) new ExtendAction(pathEditorTarget, this.ActiveSceneViewModel);
        }
      }
      if (penAction == null)
        penAction = !this.IsActive || this.pathEditorTarget == null || (this.pathEditorTarget.PathGeometry.Figures.Count <= 0 || this.pathEditorTarget.PathGeometry.Figures[this.pathEditorTarget.PathGeometry.Figures.Count - 1].IsClosed) ? (PenAction) new StartAction(this, (PathEditorTarget) null, this.ActiveSceneViewModel) : (PenAction) new AppendAction(this.pathEditorTarget, this.ActiveSceneViewModel);
      return penAction;
    }
  }
}
