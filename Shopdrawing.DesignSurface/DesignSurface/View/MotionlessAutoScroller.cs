// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.MotionlessAutoScroller
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using System;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.View
{
  public class MotionlessAutoScroller : AutoScroller
  {
    private const double expectedFullscreenTime = 2.0;
    private EdgeFlags scrollDirection;
    private Point startPosition;
    private Point currentPosition;
    private Point mousePosition;
    private Rect artboardBoundary;
    private Func<Point, Point, bool, bool> handleDragCallback;

    public ToolBehavior Behavior { get; private set; }

    public SceneView SceneView
    {
      get
      {
        return this.Behavior.ActiveView;
      }
    }

    public MotionlessAutoScroller(ToolBehavior behavior, Func<Point, Point, bool, bool> handleDragCallback)
    {
      this.Behavior = behavior;
      this.handleDragCallback = handleDragCallback;
    }

    public void StartScroll(Point startPoint, Point currentPoint)
    {
      this.startPosition = startPoint;
      this.currentPosition = currentPoint;
      Artboard artboard = this.SceneView.Artboard;
      this.mousePosition = Mouse.GetPosition((IInputElement) artboard);
      this.artboardBoundary = new Rect(0.0, 0.0, artboard.ActualWidth, artboard.ActualHeight);
      if (this.Behavior.ShouldMotionlessAutoScroll(this.mousePosition, this.artboardBoundary))
      {
        this.scrollDirection = this.ComputeScrollDirection();
        this.ScrollTimer.Start();
      }
      else
        this.ScrollTimer.Stop();
    }

    protected override bool DoScroll()
    {
      if (!(this.mousePosition == Mouse.GetPosition((IInputElement) this.SceneView.Artboard)) || !this.Behavior.ShouldMotionlessAutoScroll(this.mousePosition, this.artboardBoundary))
        return false;
      this.currentPosition += this.ComputeIncrementalDelta(this.scrollDirection);
      int num = this.handleDragCallback(this.startPosition, this.currentPosition, true) ? true : false;
      return true;
    }

    private Vector ComputeIncrementalDelta(EdgeFlags edgeFlags)
    {
      Vector vector = new Vector();
      double num1 = this.ScrollTimer.Interval.TotalSeconds / 2.0 / this.SceneView.Zoom;
      double num2 = this.SceneView.Artboard.ActualWidth * num1;
      double num3 = this.SceneView.Artboard.ActualHeight * num1;
      if ((edgeFlags & EdgeFlags.Left) != EdgeFlags.None)
        vector.X -= num2;
      else if ((edgeFlags & EdgeFlags.Right) != EdgeFlags.None)
        vector.X += num2;
      if ((edgeFlags & EdgeFlags.Top) != EdgeFlags.None)
        vector.Y -= num3;
      else if ((edgeFlags & EdgeFlags.Bottom) != EdgeFlags.None)
        vector.Y += num3;
      return vector;
    }

    private EdgeFlags ComputeScrollDirection()
    {
      if (this.artboardBoundary.Width <= 2.0 || this.artboardBoundary.Height <= 2.0)
        return EdgeFlags.None;
      double num1 = (this.mousePosition.X - (this.artboardBoundary.Left + this.artboardBoundary.Right) / 2.0) / this.artboardBoundary.Width;
      double num2 = (this.mousePosition.Y - (this.artboardBoundary.Top + this.artboardBoundary.Bottom) / 2.0) / this.artboardBoundary.Height;
      if (Math.Abs(num1) > Math.Abs(num2))
        return num1 <= 0.0 ? EdgeFlags.Left : EdgeFlags.Right;
      return num2 <= 0.0 ? EdgeFlags.Top : EdgeFlags.Bottom;
    }
  }
}
