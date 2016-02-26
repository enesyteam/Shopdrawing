// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.SnapLineEngine
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Designers;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal sealed class SnapLineEngine
  {
    private bool isEnabled = true;
    private List<SnapLine> snapLines = new List<SnapLine>();
    private SnapLineRenderer snapLineRenderer = new SnapLineRenderer();
    private double defaultPadding = 8.0;
    private double defaultMargin = 4.0;
    private const double errorTolerance = 1E-06;
    private ToolBehaviorContext toolContext;
    private SceneElement targetElement;
    private Base2DElement container;
    private double snapThreshold;

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
        return this.isEnabled;
      }
      set
      {
        if (this.isEnabled == value)
          return;
        this.isEnabled = value;
        if (!this.snapLineRenderer.IsAttached && this.isEnabled && (this.toolContext != null && this.container != null))
        {
          this.snapLineRenderer.Attach(this.toolContext, (SceneElement) this.container);
        }
        else
        {
          if (!this.snapLineRenderer.IsAttached || this.isEnabled)
            return;
          this.snapLineRenderer.Detach();
        }
      }
    }

    public double DefaultPadding
    {
      get
      {
        return this.defaultPadding;
      }
      set
      {
        this.defaultPadding = value;
      }
    }

    public double DefaultMargin
    {
      get
      {
        return this.defaultMargin;
      }
      set
      {
        this.defaultMargin = value;
      }
    }

    private double ScaledSnapThreshold
    {
      get
      {
        return this.snapThreshold / this.toolContext.View.Zoom;
      }
    }

    public SnapLineEngine(double snapThreshold)
    {
      this.snapThreshold = snapThreshold;
    }

    public void Start(ToolBehaviorContext toolContext, BaseFrameworkElement targetElement, IList<BaseFrameworkElement> ignoredElements)
    {
      if (this.IsStarted)
        this.Stop();
      this.toolContext = toolContext;
      if (this.toolContext.View.ViewModel.UsingEffectDesigner)
        return;
      this.container = (Base2DElement) null;
      this.targetElement = (SceneElement) targetElement;
      if (targetElement == null)
      {
        ISceneInsertionPoint sceneInsertionPoint = this.toolContext.View.ViewModel.ActiveSceneInsertionPoint;
        if (sceneInsertionPoint != null)
          this.container = sceneInsertionPoint.SceneElement as Base2DElement;
      }
      else if (!this.IsTransformed((SceneElement) targetElement))
        this.container = targetElement.ParentElement as Base2DElement;
      ILayoutDesigner layoutDesigner = targetElement == null ? this.toolContext.View.ViewModel.GetLayoutDesignerForParent((SceneElement) this.container, true) : this.toolContext.View.ViewModel.GetLayoutDesignerForChild(this.targetElement, true);
      BaseFrameworkElement parent = this.container as BaseFrameworkElement;
      bool leftRight = layoutDesigner.GetWidthConstraintMode(parent, targetElement) != LayoutConstraintMode.CanvasLike;
      bool topBottom = layoutDesigner.GetHeightConstraintMode(parent, targetElement) != LayoutConstraintMode.CanvasLike;
      if (!leftRight && !topBottom)
        this.container = (Base2DElement) null;
      if (this.container == null)
        return;
      this.snapLineRenderer.Attach(toolContext, (SceneElement) this.container);
      Rect actualBounds = this.toolContext.View.GetActualBounds(this.container.ViewObject);
      this.AddSnapLines(actualBounds, true, leftRight, topBottom);
      GridElement gridElement = this.container as GridElement;
      if (gridElement != null)
      {
        double x = 0.0;
        foreach (ColumnDefinitionNode columnDefinitionNode in (IEnumerable<ColumnDefinitionNode>) gridElement.ColumnDefinitions)
        {
          x += columnDefinitionNode.ComputedWidth;
          Point p1 = new Point(x, actualBounds.Top);
          Point p2 = new Point(x, actualBounds.Bottom);
          this.snapLines.Add(new SnapLine(p1, p2, SnapLineOrientation.Vertical, SnapLineLocation.Minimum, true));
          this.snapLines.Add(new SnapLine(p1, p2, SnapLineOrientation.Vertical, SnapLineLocation.Maximum, true));
        }
        double y = 0.0;
        foreach (RowDefinitionNode rowDefinitionNode in (IEnumerable<RowDefinitionNode>) gridElement.RowDefinitions)
        {
          y += rowDefinitionNode.ComputedHeight;
          Point p1 = new Point(actualBounds.Left, y);
          Point p2 = new Point(actualBounds.Right, y);
          this.snapLines.Add(new SnapLine(p1, p2, SnapLineOrientation.Horizontal, SnapLineLocation.Minimum, true));
          this.snapLines.Add(new SnapLine(p1, p2, SnapLineOrientation.Horizontal, SnapLineLocation.Maximum, true));
        }
      }
      if (!(this.container is PanelElement))
        return;
      foreach (SceneNode sceneNode in (IEnumerable<SceneNode>) ((PanelElement) this.container).Children)
      {
        BaseFrameworkElement frameworkElement = sceneNode as BaseFrameworkElement;
        if (frameworkElement != null && frameworkElement != targetElement && frameworkElement.IsViewObjectValid && ((ignoredElements == null || !ignoredElements.Contains(frameworkElement)) && !this.IsTransformed((SceneElement) frameworkElement)))
        {
          Rect actualBoundsInParent = this.toolContext.View.GetActualBoundsInParent(frameworkElement.ViewObject);
          this.AddSnapLines(actualBoundsInParent, false, leftRight, topBottom);
          double baseline = this.toolContext.View.GetBaseline((SceneNode) frameworkElement);
          if (!double.IsNaN(baseline))
            this.snapLines.Add(new SnapLine(new Point(actualBoundsInParent.Left, actualBoundsInParent.Top + baseline), new Point(actualBoundsInParent.Right, actualBoundsInParent.Top + baseline), SnapLineOrientation.Horizontal, SnapLineLocation.Baseline, false));
        }
      }
    }

    public void Stop()
    {
      if (this.snapLineRenderer.IsAttached)
        this.snapLineRenderer.Detach();
      this.toolContext = (ToolBehaviorContext) null;
      this.targetElement = (SceneElement) null;
      this.container = (Base2DElement) null;
      this.snapLines.Clear();
    }

    public Vector SnapRect(Rect rect, Vector offset, EdgeFlags edgeFlags, double baselineOffset, out Vector constraintDirection)
    {
      constraintDirection = new Vector();
      if (!this.IsStarted || this.container == null)
        return new Vector();
      offset = this.toolContext.View.TransformVectorFromRoot((SceneElement) this.container, offset);
      rect = this.OffsetRect(rect, offset, edgeFlags);
      double scaledSnapThreshold = this.ScaledSnapThreshold;
      double adjustment1 = 0.0;
      List<SnapLine> snapLineList1_1 = (List<SnapLine>) null;
      if ((edgeFlags & EdgeFlags.Left) != EdgeFlags.None)
        snapLineList1_1 = this.FindNearestSnapLines(rect.TopLeft, rect.BottomLeft, SnapLineLocation.Minimum, SnapLineOrientation.Vertical, scaledSnapThreshold, out adjustment1);
      double adjustment2 = 0.0;
      List<SnapLine> snapLineList2_1 = (List<SnapLine>) null;
      if ((edgeFlags & EdgeFlags.Right) != EdgeFlags.None)
        snapLineList2_1 = this.FindNearestSnapLines(rect.TopRight, rect.BottomRight, SnapLineLocation.Maximum, SnapLineOrientation.Vertical, scaledSnapThreshold, out adjustment2);
      double chosenAdjustment1;
      List<SnapLine> list = this.ChooseSnapLines(snapLineList1_1, adjustment1, snapLineList2_1, adjustment2, out chosenAdjustment1);
      double adjustment3 = 0.0;
      List<SnapLine> snapLineList1_2 = (List<SnapLine>) null;
      if ((edgeFlags & EdgeFlags.Top) != EdgeFlags.None)
        snapLineList1_2 = this.FindNearestSnapLines(rect.TopLeft, rect.TopRight, SnapLineLocation.Minimum, SnapLineOrientation.Horizontal, scaledSnapThreshold, out adjustment3);
      double adjustment4 = 0.0;
      List<SnapLine> snapLineList2_2 = (List<SnapLine>) null;
      if ((edgeFlags & EdgeFlags.Bottom) != EdgeFlags.None)
        snapLineList2_2 = this.FindNearestSnapLines(rect.BottomLeft, rect.BottomRight, SnapLineLocation.Maximum, SnapLineOrientation.Horizontal, scaledSnapThreshold, out adjustment4);
      double chosenAdjustment2;
      List<SnapLine> snapLineList1_3 = this.ChooseSnapLines(snapLineList1_2, adjustment3, snapLineList2_2, adjustment4, out chosenAdjustment2);
      if (!double.IsNaN(baselineOffset) && (edgeFlags & EdgeFlags.TopOrBottom) == EdgeFlags.TopOrBottom)
      {
        double adjustment5 = 0.0;
        List<SnapLine> nearestSnapLines = this.FindNearestSnapLines(new Point(rect.Left, rect.Top + baselineOffset), new Point(rect.Right, rect.Top + baselineOffset), SnapLineLocation.Baseline, SnapLineOrientation.Horizontal, scaledSnapThreshold, out adjustment5);
        snapLineList1_3 = this.ChooseSnapLines(snapLineList1_3, chosenAdjustment2, nearestSnapLines, adjustment5, out chosenAdjustment2);
      }
      List<SnapLine> snapLines = new List<SnapLine>();
      if (snapLineList1_3 != null)
        snapLines.AddRange((IEnumerable<SnapLine>) snapLineList1_3);
      if (list != null)
        snapLines.AddRange((IEnumerable<SnapLine>) list);
      rect = this.OffsetRect(rect, new Vector(chosenAdjustment1, chosenAdjustment2), edgeFlags);
      this.snapLineRenderer.ReplaceSnapLines(this.targetElement, rect, snapLines);
      if (snapLineList1_3 != null && snapLineList1_3.Count > 0)
        constraintDirection = list == null || list.Count <= 0 ? this.toolContext.View.TransformVectorToRoot((SceneElement) this.container, new Vector(1.0, 0.0)) : new Vector(1.0, 1.0);
      else if (list != null && list.Count > 0)
        constraintDirection = this.toolContext.View.TransformVectorToRoot((SceneElement) this.container, new Vector(0.0, 1.0));
      return this.toolContext.View.TransformVectorToRoot((SceneElement) this.container, new Vector(chosenAdjustment1, chosenAdjustment2));
    }

    public void UpdateTargetBounds(Rect targetBounds)
    {
      this.snapLineRenderer.UpdateTargetBounds(targetBounds);
    }

    private void AddSnapLines(Rect rect, bool isContainer, bool leftRight, bool topBottom)
    {
      if (leftRight)
      {
        this.snapLines.Add(new SnapLine(rect.TopLeft, rect.BottomLeft, SnapLineOrientation.Vertical, SnapLineLocation.Minimum, isContainer));
        this.snapLines.Add(new SnapLine(rect.TopRight, rect.BottomRight, SnapLineOrientation.Vertical, SnapLineLocation.Maximum, isContainer));
      }
      if (!topBottom)
        return;
      this.snapLines.Add(new SnapLine(rect.TopLeft, rect.TopRight, SnapLineOrientation.Horizontal, SnapLineLocation.Minimum, isContainer));
      this.snapLines.Add(new SnapLine(rect.BottomLeft, rect.BottomRight, SnapLineOrientation.Horizontal, SnapLineLocation.Maximum, isContainer));
    }

    private List<SnapLine> FindNearestSnapLines(Point p1, Point p2, SnapLineLocation location, SnapLineOrientation orientation, double snapThreshold, out double adjustment)
    {
      List<SnapLine> resultSnapLines = new List<SnapLine>();
      double smallestSignedDistance = double.MaxValue;
      foreach (SnapLine snapLine in this.snapLines)
      {
        double offset;
        if (snapLine.Orientation == orientation && this.ShouldSnap(p1, p2, location, snapLine, out offset))
        {
          smallestSignedDistance = this.CheckForCloserSnapLine(p1, location, snapThreshold, resultSnapLines, smallestSignedDistance, snapLine, offset);
          if (offset != 0.0)
            smallestSignedDistance = this.CheckForCloserSnapLine(p1, location, snapThreshold, resultSnapLines, smallestSignedDistance, snapLine, 0.0);
        }
      }
      adjustment = resultSnapLines.Count > 0 ? smallestSignedDistance : 0.0;
      return resultSnapLines;
    }

    private double CheckForCloserSnapLine(Point p1, SnapLineLocation location, double snapThreshold, List<SnapLine> resultSnapLines, double smallestSignedDistance, SnapLine snapLine, double offset)
    {
      double num1 = snapLine.GetSignedDistance(p1) + offset;
      double num2 = Math.Abs(num1);
      if (num2 <= snapThreshold)
      {
        if (num2 < Math.Abs(smallestSignedDistance))
        {
          smallestSignedDistance = num1;
          resultSnapLines.Clear();
        }
        if (Math.Abs(num1 - smallestSignedDistance) < 1E-06)
        {
          snapLine.LocationRelativeToTarget = location;
          snapLine.OffsetRelativeToTarget = offset;
          resultSnapLines.Add(snapLine);
        }
      }
      return smallestSignedDistance;
    }

    private bool ShouldSnap(Point p1, Point p2, SnapLineLocation location, SnapLine snapLine, out double offset)
    {
      offset = 0.0;
      if (location == snapLine.Location)
      {
        if (!snapLine.IsContainerLine)
          return true;
        if (snapLine.RangeOverlaps(p1, p2))
        {
          offset = location == SnapLineLocation.Minimum ? this.defaultPadding : -this.defaultPadding;
          return true;
        }
      }
      else if ((location == SnapLineLocation.Minimum && snapLine.Location == SnapLineLocation.Maximum || location == SnapLineLocation.Maximum && snapLine.Location == SnapLineLocation.Minimum) && (!snapLine.IsContainerLine && snapLine.RangeOverlaps(p1, p2)))
      {
        offset = snapLine.Location == SnapLineLocation.Minimum ? -this.defaultMargin : this.defaultMargin;
        return true;
      }
      return false;
    }

    private List<SnapLine> ChooseSnapLines(List<SnapLine> snapLineList1, double adjustment1, List<SnapLine> snapLineList2, double adjustment2, out double chosenAdjustment)
    {
      if (snapLineList1 == null || snapLineList1.Count == 0)
      {
        chosenAdjustment = adjustment2;
        return snapLineList2;
      }
      if (snapLineList2 == null || snapLineList2.Count == 0)
      {
        chosenAdjustment = adjustment1;
        return snapLineList1;
      }
      if (Math.Abs(adjustment1 - adjustment2) <= 1E-06)
      {
        List<SnapLine> list = new List<SnapLine>((IEnumerable<SnapLine>) snapLineList1);
        list.AddRange((IEnumerable<SnapLine>) snapLineList2);
        chosenAdjustment = adjustment1;
        return list;
      }
      if (Math.Abs(adjustment1) <= Math.Abs(adjustment2))
      {
        chosenAdjustment = adjustment1;
        return snapLineList1;
      }
      chosenAdjustment = adjustment2;
      return snapLineList2;
    }

    private Rect OffsetRect(Rect rect, Vector offset, EdgeFlags edgeFlags)
    {
      if (!rect.IsEmpty)
      {
        switch (edgeFlags & EdgeFlags.LeftOrRight)
        {
          case EdgeFlags.Left:
            rect.X += offset.X;
            rect.Width = Math.Max(0.0, rect.Width - offset.X);
            break;
          case EdgeFlags.Right:
            rect.Width = Math.Max(0.0, rect.Width + offset.X);
            break;
          case EdgeFlags.LeftOrRight:
            rect.X += offset.X;
            break;
        }
        switch (edgeFlags & EdgeFlags.TopOrBottom)
        {
          case EdgeFlags.Top:
            rect.Y += offset.Y;
            rect.Height = Math.Max(0.0, rect.Height - offset.Y);
            break;
          case EdgeFlags.Bottom:
            rect.Height = Math.Max(0.0, rect.Height + offset.Y);
            break;
          case EdgeFlags.TopOrBottom:
            rect.Y += offset.Y;
            break;
        }
      }
      return rect;
    }

    private bool IsTransformed(SceneElement element)
    {
      IViewVisual viewVisual = element.Visual as IViewVisual;
      if (viewVisual != null)
        return viewVisual.IsTransformed;
      return false;
    }
  }
}
