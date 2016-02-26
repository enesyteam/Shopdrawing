// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.SnappingEngine
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools
{
  public sealed class SnappingEngine
  {
    private SnapLineEngine snapLineEngine = new SnapLineEngine(8.0);
    private const double SnapThreshold = 8.0;
    private ToolBehaviorContext toolContext;

    internal DesignerContext DesignerContext { get; private set; }

    public bool IsStarted
    {
      get
      {
        return this.toolContext != null;
      }
    }

    public bool IsEnabled
    {
      get
      {
        bool flag = Keyboard.IsKeyUp(Key.S);
        this.snapLineEngine.IsEnabled = flag;
        return flag;
      }
    }

    private double ScaledSnapThreshold
    {
      get
      {
        if (this.toolContext != null)
          return 8.0 / this.toolContext.View.Zoom;
        return double.MaxValue;
      }
    }

    internal SnappingEngine(DesignerContext designerContext)
    {
      this.DesignerContext = designerContext;
    }

    public void Start(ToolBehaviorContext toolContext, BaseFrameworkElement targetElement, IList<BaseFrameworkElement> ignoredElements)
    {
      this.toolContext = toolContext;
      if (this.DesignerContext.ArtboardOptionsModel.SnapToSnapLines)
      {
        this.snapLineEngine.DefaultMargin = this.DesignerContext.ArtboardOptionsModel.SnapLineMargin;
        this.snapLineEngine.DefaultPadding = this.DesignerContext.ArtboardOptionsModel.SnapLinePadding;
        this.snapLineEngine.Start(toolContext, targetElement, ignoredElements);
      }
      SceneView view = this.toolContext.View;
      if (view == null)
        return;
      view.Artboard.SnapToGridRenderer.IsOriginOffsetLocked = true;
    }

    public void Stop()
    {
      if (this.toolContext != null)
      {
        SceneView view = this.toolContext.View;
        if (view != null)
          view.Artboard.SnapToGridRenderer.IsOriginOffsetLocked = false;
      }
      this.toolContext = (ToolBehaviorContext) null;
      if (!this.DesignerContext.ArtboardOptionsModel.SnapToSnapLines)
        return;
      this.snapLineEngine.Stop();
    }

    public Vector SnapRect(Rect rect, SceneElement container, Vector offset, EdgeFlags edgeFlags)
    {
      return this.SnapRect(rect, container, offset, edgeFlags, double.NaN);
    }

    public Vector SnapRect(Rect rect, SceneElement container, Vector offset, EdgeFlags edgeFlags, double baselineOffset)
    {
      if (!this.IsEnabled || !this.IsStarted)
        return new Vector();
      Vector vector1 = new Vector();
      Vector constraintDirection = new Vector();
      if (this.DesignerContext.ArtboardOptionsModel.SnapToSnapLines)
        vector1 = this.snapLineEngine.SnapRect(rect, offset, edgeFlags, baselineOffset, out constraintDirection);
      if (this.DesignerContext.ArtboardOptionsModel.SnapToGrid && (constraintDirection.X == 0.0 || constraintDirection.Y == 0.0))
      {
        Point[] pointArray = new Point[4]
        {
          rect.TopLeft,
          rect.TopRight,
          rect.BottomRight,
          rect.BottomLeft
        };
        EdgeFlags[] edgeFlagsArray = new EdgeFlags[4]
        {
          EdgeFlags.TopLeft,
          EdgeFlags.TopRight,
          EdgeFlags.BottomRight,
          EdgeFlags.BottomLeft
        };
        EdgeFlags edgeFlags1 = edgeFlags;
        if (edgeFlags == EdgeFlags.Left || edgeFlags == EdgeFlags.Right)
          edgeFlags1 |= EdgeFlags.TopOrBottom;
        else if (edgeFlags == EdgeFlags.Top || edgeFlags == EdgeFlags.Bottom)
          edgeFlags1 |= EdgeFlags.LeftOrRight;
        double num1 = double.PositiveInfinity;
        double num2 = double.PositiveInfinity;
        Vector vector2 = new Vector();
        for (int index = 0; index < 4; ++index)
        {
          if ((edgeFlagsArray[index] & edgeFlags1) == edgeFlagsArray[index])
          {
            Point point = this.toolContext.View.TransformPointToRoot(container, pointArray[index]) + offset;
            Point snappedPoint;
            if (this.SnapPointToGrid(point, out snappedPoint))
            {
              Vector vector3 = snappedPoint - point;
              if (constraintDirection.Y != 0.0)
                vector3.X = 0.0;
              if (constraintDirection.X != 0.0)
                vector3.Y = 0.0;
              double num3 = Math.Abs(vector3.X);
              if (num3 > 0.0 && num3 < num1)
              {
                num1 = num3;
                vector2.X = vector3.X;
              }
              double num4 = Math.Abs(vector3.Y);
              if (num4 > 0.0 && num4 < num2)
              {
                num2 = num4;
                vector2.Y = vector3.Y;
              }
            }
          }
        }
        vector1 += vector2;
      }
      return vector1;
    }

    public Point SnapPoint(Point point)
    {
      return this.SnapPoint(point, EdgeFlags.None);
    }

    public Point SnapPoint(Point point, EdgeFlags edgeFlags)
    {
      if (!this.IsEnabled)
        return point;
      Point snappedPoint = point;
      if (this.IsStarted && this.DesignerContext.ArtboardOptionsModel.SnapToSnapLines && edgeFlags != EdgeFlags.None)
      {
        if (this.toolContext.View.ViewModel.ActiveSceneInsertionPoint == null)
          return point;
        SceneElement sceneElement = this.toolContext.View.ViewModel.ActiveSceneInsertionPoint.SceneElement;
        if (sceneElement.IsViewObjectValid)
        {
          Vector vector = this.SnapRect(new Rect(this.toolContext.View.TransformPointFromRoot(sceneElement, point), new Size(0.0, 0.0)), sceneElement, new Vector(0.0, 0.0), edgeFlags);
          snappedPoint = point + vector;
        }
      }
      else if (this.DesignerContext.ArtboardOptionsModel.SnapToGrid)
        this.SnapPointToGrid(point, out snappedPoint);
      return snappedPoint;
    }

    public void UpdateTargetBounds(Rect targetBounds)
    {
      this.snapLineEngine.UpdateTargetBounds(targetBounds);
    }

    private bool SnapPointToGrid(Point point, out Point snappedPoint)
    {
      Vector originOffset = this.toolContext.View.Artboard.SnapToGridRenderer.OriginOffset;
      double gridSpacing = this.DesignerContext.ArtboardOptionsModel.GridSpacing;
      double scaledSnapThreshold = this.ScaledSnapThreshold;
      snappedPoint = point - originOffset;
      snappedPoint = new Point(gridSpacing * Math.Round(snappedPoint.X / gridSpacing), gridSpacing * Math.Round(snappedPoint.Y / gridSpacing));
      snappedPoint += originOffset;
      if (Math.Abs(snappedPoint.X - point.X) > scaledSnapThreshold)
        snappedPoint.X = point.X;
      if (Math.Abs(snappedPoint.Y - point.Y) > scaledSnapThreshold)
        snappedPoint.Y = point.Y;
      return snappedPoint != point;
    }
  }
}
