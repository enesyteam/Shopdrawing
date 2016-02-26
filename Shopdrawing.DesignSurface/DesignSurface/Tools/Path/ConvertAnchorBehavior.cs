// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.ConvertAnchorBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  internal class ConvertAnchorBehavior : ElementToolBehavior
  {
    private List<PathAdornerSet> affectedAdornerSets = new List<PathAdornerSet>();
    private bool isConstrainingAxes;
    private PenAction action;

    protected override Cursor DefaultCursor
    {
      get
      {
        return ToolCursors.ConvertCursor;
      }
    }

    public ConvertAnchorBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    protected override bool OnHoverOverAdorner(IAdorner adorner)
    {
      if (this.action == null && !this.IsAltDown)
      {
        this.PopSelf();
        return true;
      }
      PathPartAdorner pathPartAdorner = adorner as PathPartAdorner;
      if (pathPartAdorner == null)
        return base.OnHoverOverAdorner(adorner);
      PenAction penAction = this.GetAction(pathPartAdorner);
      if (penAction == null)
      {
        if (this.action != null)
        {
          penAction = this.action;
        }
        else
        {
          this.PopSelf();
          return true;
        }
      }
      this.Cursor = penAction.HoverCursor;
      return true;
    }

    protected override bool OnHoverOverNonAdorner(Point pointerPosition)
    {
      if (this.action == null && !this.IsAltDown)
        this.PopSelf();
      return base.OnHoverOverNonAdorner(pointerPosition);
    }

    protected override bool OnButtonDownOverAdorner(IAdorner adorner)
    {
      if (this.action != null)
      {
        this.action.End();
        this.action = (PenAction) null;
      }
      if (!this.ToolBehaviorContext.SnappingEngine.IsStarted)
        this.ToolBehaviorContext.SnappingEngine.Start(this.ToolBehaviorContext, (BaseFrameworkElement) null, (IList<BaseFrameworkElement>) null);
      PathPartAdorner pathPartAdorner = adorner as PathPartAdorner;
      if (pathPartAdorner == null)
        return base.OnButtonDownOverAdorner(adorner);
      this.action = this.GetAction(pathPartAdorner);
      if (this.action != null)
      {
        this.action.Begin(new PathEditContext(pathPartAdorner.FigureIndex, pathPartAdorner.PartIndex), this.MouseDevice);
        if (!this.affectedAdornerSets.Contains(pathPartAdorner.PathAdornerSet))
          this.affectedAdornerSets.Add(pathPartAdorner.PathAdornerSet);
        this.isConstrainingAxes = this.IsShiftDown;
        this.UpdateCursor();
      }
      return true;
    }

    protected override bool OnButtonDownOverNonAdorner(Point pointerPosition)
    {
      if (this.action != null)
      {
        this.action.End();
        this.action = (PenAction) null;
      }
      this.UpdateCursor();
      return true;
    }

    protected override bool OnDrag(Point dragStartPosition, Point dragCurrentPosition, bool scrollNow)
    {
      if (this.action != null)
      {
        this.action.Drag(this.MouseDevice, this.ActiveView.Zoom);
        if (this.action.IsActive)
          this.ActiveView.EnsureVisible(dragCurrentPosition, scrollNow);
      }
      this.UpdateCursor();
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
      if (this.action != null)
        this.action.End();
      this.action = (PenAction) null;
      if (this.IsAltDown)
        this.UpdateCursor();
      else
        this.PopSelf();
      return true;
    }

    protected override bool OnKey(KeyEventArgs args)
    {
      if (!this.IsDragging)
      {
        if (args.IsUp && args.Key == Key.System && (args.SystemKey == Key.LeftAlt || args.SystemKey == Key.RightAlt))
          this.PopSelf();
      }
      else if (this.action != null)
      {
        bool isShiftDown = this.IsShiftDown;
        if (this.isConstrainingAxes != isShiftDown)
        {
          this.isConstrainingAxes = isShiftDown;
          this.action.Drag(this.MouseDevice, this.ActiveView.Zoom);
        }
      }
      return true;
    }

    protected override void OnSuspend()
    {
      if (this.action != null)
        this.action.End();
      foreach (PathAdornerSet pathAdornerSet in this.affectedAdornerSets)
      {
        pathAdornerSet.UpdateActiveStateFromSelection();
        pathAdornerSet.InvalidateStructure();
      }
      base.OnSuspend();
    }

    private PenAction GetAction(PathPartAdorner pathPartAdorner)
    {
      PenAction penAction = (PenAction) null;
      PathEditorTarget pathEditorTarget = pathPartAdorner.PathAdornerSet.PathEditorTarget;
      PathFigure pathFigure = pathEditorTarget.PathGeometry.Figures[pathPartAdorner.FigureIndex];
      for (int index1 = 0; index1 < pathFigure.Segments.Count; ++index1)
      {
        System.Windows.Media.PathSegment segment = pathFigure.Segments[index1];
        for (int index2 = 0; index2 < PathSegmentUtilities.GetPointCount(segment); ++index2)
        {
          if (PathSegmentUtilities.GetPointKind(segment, index2) == PathPointKind.Arc)
            return (PenAction) null;
        }
      }
      if (pathPartAdorner is PathPointAdorner)
        penAction = (PenAction) new ConvertPointAction(pathEditorTarget, this.ActiveSceneViewModel);
      else if (pathPartAdorner is PathTangentAdorner || pathPartAdorner is PenTangentAdorner)
        penAction = (PenAction) new TangentDragAction(pathEditorTarget, this.ActiveSceneViewModel, pathPartAdorner is PenTangentAdorner);
      else if (pathPartAdorner is PathSegmentAdorner)
        penAction = (PenAction) new ConvertSegmentAction(pathEditorTarget, this.ActiveSceneViewModel);
      return penAction;
    }
  }
}
