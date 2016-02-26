// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.PencilCreateBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Diagnostics;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  internal class PencilCreateBehavior : PathCreateBehavior
  {
    private List<Point> pointList = new List<Point>();
    private IncrementalFitter vpFitter = new IncrementalFitter();
    private const float DotRadius = 1f;

    public override string ActionString
    {
      get
      {
        return StringTable.UndoUnitPencil;
      }
    }

    protected override Cursor DefaultCursor
    {
      get
      {
        if (!this.IsProjectedInsertionPoint)
          return ToolCursors.PencilCursor;
        return ToolCursors.NoDropCursor;
      }
    }

    public PencilCreateBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    protected override bool OnButtonDownOverNonAdorner(Point pointerPosition)
    {
      this.IsProjectedInsertionPoint = Adorner.NonAffineTransformInParentStack(this.ActiveSceneInsertionPoint.SceneElement);
      if (!this.IsProjectedInsertionPoint)
      {
        this.pointList.Clear();
        this.pointList.Add(pointerPosition);
        this.vpFitter.Clear();
        this.vpFitter.Add(pointerPosition, 0L);
        this.vpFitter.CurveTolerance = Tolerances.CurveFittingDistanceTolerance(this.ActiveView.Zoom) * Tolerances.CurveFittingDistanceTolerance(this.ActiveView.Zoom);
      }
      return true;
    }

    internal override bool ShouldMotionlessAutoScroll(Point mousePoint, Rect artboardBoundary)
    {
      return false;
    }

    protected override bool OnDrag(Point dragStartPosition, Point dragCurrentPosition, bool scrollNow)
    {
      if (!this.IsProjectedInsertionPoint)
      {
        if (VectorUtilities.Distance(this.pointList[this.pointList.Count - 1], dragCurrentPosition) > 1.2 / this.ActiveView.Zoom)
        {
          this.pointList.Add(dragCurrentPosition);
          if (DebugVariables.Instance.EnableRealTimeFitting)
            this.vpFitter.Add(dragCurrentPosition, 0L);
        }
        DrawingContext dc = this.OpenFeedback();
        if (DebugVariables.Instance.EnableRealTimeFitting)
          this.DrawCurve(dc);
        else
          this.DrawLines(dc);
        this.CloseFeedback();
      }
      return true;
    }

    protected override bool OnDragEnd(Point dragStartPosition, Point dragEndPosition)
    {
      if (!this.IsProjectedInsertionPoint)
      {
        SceneView activeView = this.ActiveView;
        SceneViewModel viewModel = activeView.ViewModel;
        bool flag = false;
        if (this.pointList.Count >= 4)
        {
          Point point1 = this.pointList[0];
          Point point2 = this.pointList[this.pointList.Count - 1];
          if (Tolerances.DoPointsHit(point1, point2, activeView.Zoom))
          {
            this.pointList[this.pointList.Count - 1] = point1;
            flag = true;
          }
        }
        PropertyManager propertyManager = (PropertyManager) this.ToolBehaviorContext.PropertyManager;
        BezierCurveFitter bezierCurveFitter = new BezierCurveFitter();
        PathGeometry pathGeometry = !DebugVariables.Instance.EnableRealTimeFitting ? (!flag ? bezierCurveFitter.OpenFit(this.pointList, true, Tolerances.CurveFitCornerThreshold, Tolerances.CurveFittingDistanceTolerance(activeView.Zoom), true) : bezierCurveFitter.ClosedFit(this.pointList, true, Tolerances.CurveFitCornerThreshold, Tolerances.CurveFittingDistanceTolerance(activeView.Zoom), true)) : this.vpFitter.Path;
        try
        {
          this.EnsureEditTransaction();
          Matrix transformFromRoot = activeView.GetComputedTransformFromRoot(this.ActiveSceneInsertionPoint.SceneElement);
          new PathFigureEditor(pathGeometry.Figures[0]).ApplyTransform(transformFromRoot);
          viewModel.ElementSelectionSet.Clear();
          this.CreatePathElement();
          this.UpdateEditTransaction();
          if (this.EditingElement.IsViewObjectValid)
          {
            using (ScenePathEditorTarget pathEditorTarget = new ScenePathEditorTarget((PathElement) this.EditingElement))
            {
              pathEditorTarget.BeginEditing();
              pathEditorTarget.PathGeometry = pathGeometry;
              pathEditorTarget.EndEditing(true);
            }
          }
          this.CommitEditTransaction();
          this.Tool.RebuildAdornerSets();
        }
        finally
        {
          this.CancelEditTransaction();
          this.ClearFeedback();
          this.PreviewInsertionPoint = (ISceneInsertionPoint) null;
        }
      }
      return true;
    }

    protected override bool OnKey(KeyEventArgs args)
    {
      if (this.IsDragging)
        return true;
      return base.OnKey(args);
    }

    private void DrawLines(DrawingContext dc)
    {
      if (this.pointList.Count < 2)
        return;
      Pen thinPen = FeedbackHelper.GetThinPen(this.ActiveView.Zoom);
      Point point0 = this.pointList[0];
      for (int index = 1; index < this.pointList.Count; ++index)
      {
        Point point1 = this.pointList[index];
        dc.DrawLine(thinPen, point0, point1);
        point0 = point1;
      }
    }

    private void DrawCurve(DrawingContext dc)
    {
      PathGeometry path = this.vpFitter.Path;
      Pen thinPen = FeedbackHelper.GetThinPen(this.ActiveView.Zoom);
      dc.DrawGeometry((Brush)null, thinPen, (System.Windows.Media.Geometry)path);
    }
  }
}
