// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Layout.FlowPanelInsertionPointAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Designers;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Layout
{
  internal class FlowPanelInsertionPointAdorner : BaseFlowInsertionPointAdorner
  {
    public FlowPanelInsertionPointAdorner(AdornerSet adornerSet, BaseFlowInsertionPoint baseFlowInsertionPoint)
      : base(adornerSet, baseFlowInsertionPoint)
    {
    }

    protected override bool GetInsertionInfo(SceneElement container, int insertionIndex, bool isCursorAtEnd, out Point position, out double length, out Orientation orientation)
    {
      position = new Point();
      length = 0.0;
      orientation = Orientation.Horizontal;
      IViewPanel viewPanel = MoveStrategy.GetContainerHost(container) as IViewPanel;
      if (container == null || viewPanel == null)
        return false;
      orientation = viewPanel.Orientation;
      if (viewPanel.ChildrenCount == 0)
      {
        position = new Point(0.0, 0.0);
        length = orientation == Orientation.Horizontal ? viewPanel.RenderSize.Height : viewPanel.RenderSize.Width;
        return true;
      }
      List<FlowPanelLayoutUtilities.LineInfo> lines = FlowPanelLayoutUtilities.ExtractLines(container);
      if (lines.Count <= 0)
        return false;
      FlowPanelLayoutUtilities.LineInfo lineInfo1 = (FlowPanelLayoutUtilities.LineInfo) null;
      IViewVisual index = (IViewVisual) null;
      ActualBoundsInParent actualBoundsInParent = new ActualBoundsInParent(container);
      foreach (FlowPanelLayoutUtilities.LineInfo lineInfo2 in lines)
      {
        if (lineInfo2.EndElementIndex == insertionIndex - 1 && isCursorAtEnd && insertionIndex < viewPanel.ChildrenCount)
        {
          index = viewPanel.GetChild(lineInfo2.EndElementIndex);
          lineInfo1 = lineInfo2;
          length = lineInfo2.LineLength;
          break;
        }
        if (lineInfo2.StartElementIndex <= insertionIndex && insertionIndex <= lineInfo2.EndElementIndex)
        {
          index = viewPanel.GetChild(insertionIndex);
          lineInfo1 = lineInfo2;
          length = lineInfo2.LineLength;
          break;
        }
      }
      Matrix matrixFromTransform = VectorUtilities.GetMatrixFromTransform(this.AdornerSet.ViewModel.DefaultView.ComputeTransformToVisual((IViewObject) viewPanel, this.Element.Visual));
      Point point = new Point(matrixFromTransform.OffsetX, matrixFromTransform.OffsetY);
      if (index != null && lineInfo1 != null)
      {
        Rect rect = actualBoundsInParent[index];
        switch (orientation)
        {
          case Orientation.Horizontal:
            double num1 = isCursorAtEnd ? rect.Right : rect.Left;
            position = new Point(num1 + point.X, lineInfo1.StartPoint.Y + point.Y);
            break;
          case Orientation.Vertical:
            double num2 = isCursorAtEnd ? rect.Bottom : rect.Top;
            position = new Point(lineInfo1.StartPoint.X + point.X, num2 + point.Y);
            break;
          default:
            throw new NotImplementedException(ExceptionStringTable.StackPanelUnrecognizedOrientation);
        }
        return true;
      }
      FlowPanelLayoutUtilities.LineInfo lineInfo3 = lines[lines.Count - 1];
      if (insertionIndex <= lineInfo3.EndElementIndex)
        return false;
      IViewVisual child = viewPanel.GetChild(lineInfo3.EndElementIndex);
      Rect rect1 = actualBoundsInParent[child];
      switch (orientation)
      {
        case Orientation.Horizontal:
          position = new Point(rect1.Right + point.X, lineInfo3.StartPoint.Y + point.Y);
          break;
        case Orientation.Vertical:
          position = new Point(lineInfo3.StartPoint.X + point.X, rect1.Bottom + point.Y);
          break;
        default:
          throw new NotImplementedException(ExceptionStringTable.StackPanelUnrecognizedOrientation);
      }
      length = lineInfo3.LineLength;
      return true;
    }
  }
}
