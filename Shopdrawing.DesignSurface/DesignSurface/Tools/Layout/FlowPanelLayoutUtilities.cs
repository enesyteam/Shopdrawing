// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Layout.FlowPanelLayoutUtilities
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Designers;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.Tools.Layout
{
  internal static class FlowPanelLayoutUtilities
  {
    public static int GetInsertionIndex(SceneElement container, Point position, out bool isCursorAtEnd)
    {
      isCursorAtEnd = false;
      IViewPanel viewPanel = MoveStrategy.GetContainerHost(container) as IViewPanel;
      if (viewPanel == null)
        return 0;
      Orientation orientation = viewPanel.Orientation;
      if (viewPanel.ChildrenCount == 0)
        return 0;
      bool flag = PlatformTypes.StackPanel.IsAssignableFrom((ITypeId) container.Type) || PlatformTypes.VirtualizingStackPanel.IsAssignableFrom((ITypeId) container.Type);
      ActualBoundsInParent actualBoundsInParent = new ActualBoundsInParent(container);
      foreach (FlowPanelLayoutUtilities.LineInfo line in FlowPanelLayoutUtilities.ExtractLines(container))
      {
        if (flag || FlowPanelLayoutUtilities.IsPointInLine(position, line, orientation))
        {
          if (line.StartElementIndex == line.EndElementIndex)
          {
            IViewVisual child = viewPanel.GetChild(line.StartElementIndex);
            Rect elementLayoutBounds = actualBoundsInParent[child];
            if (FlowPanelLayoutUtilities.DoesPointLieBeforeElement(position, elementLayoutBounds, orientation))
            {
              isCursorAtEnd = false;
              return line.StartElementIndex;
            }
            isCursorAtEnd = true;
            return line.StartElementIndex + 1;
          }
          IViewVisual child1 = viewPanel.GetChild(line.StartElementIndex);
          IViewVisual child2 = viewPanel.GetChild(line.EndElementIndex);
          Rect elementLayoutBounds1 = actualBoundsInParent[child1];
          Rect elementLayoutBounds2 = actualBoundsInParent[child2];
          if (FlowPanelLayoutUtilities.DoesPointLieBeforeElement(position, elementLayoutBounds1, orientation))
          {
            isCursorAtEnd = false;
            return line.StartElementIndex;
          }
          if (FlowPanelLayoutUtilities.DoesPointLieAfterElement(position, elementLayoutBounds2, orientation))
          {
            isCursorAtEnd = true;
            return line.EndElementIndex + 1;
          }
          for (int startElementIndex = line.StartElementIndex; startElementIndex < line.EndElementIndex; ++startElementIndex)
          {
            IViewVisual child3 = viewPanel.GetChild(startElementIndex);
            IViewVisual child4 = viewPanel.GetChild(startElementIndex + 1);
            Rect currentElementLayoutBounds = actualBoundsInParent[child3];
            Rect nextElementLayoutBounds = actualBoundsInParent[child4];
            if (FlowPanelLayoutUtilities.DoesPointLieBetweenElements(position, currentElementLayoutBounds, nextElementLayoutBounds, orientation))
              return startElementIndex + 1;
          }
          break;
        }
      }
      if (flag)
        return -1;
      isCursorAtEnd = true;
      return viewPanel.ChildrenCount;
    }

    private static bool DoesPointLieBetweenElements(Point point, Rect currentElementLayoutBounds, Rect nextElementLayoutBounds, Orientation orientation)
    {
      switch (orientation)
      {
        case Orientation.Horizontal:
          if ((currentElementLayoutBounds.Right + currentElementLayoutBounds.Left) / 2.0 < point.X)
            return point.X < (nextElementLayoutBounds.Right + nextElementLayoutBounds.Left) / 2.0;
          return false;
        case Orientation.Vertical:
          if ((currentElementLayoutBounds.Bottom + currentElementLayoutBounds.Top) / 2.0 < point.Y)
            return point.Y < (nextElementLayoutBounds.Bottom + nextElementLayoutBounds.Top) / 2.0;
          return false;
        default:
          throw new NotImplementedException(ExceptionStringTable.StackPanelUnrecognizedOrientation);
      }
    }

    private static bool DoesPointLieBeforeElement(Point point, Rect elementLayoutBounds, Orientation orientation)
    {
      switch (orientation)
      {
        case Orientation.Horizontal:
          return point.X < elementLayoutBounds.Left + elementLayoutBounds.Width / 2.0;
        case Orientation.Vertical:
          return point.Y < elementLayoutBounds.Top + elementLayoutBounds.Height / 2.0;
        default:
          throw new NotImplementedException(ExceptionStringTable.StackPanelUnrecognizedOrientation);
      }
    }

    private static bool DoesPointLieAfterElement(Point point, Rect elementLayoutBounds, Orientation orientation)
    {
      switch (orientation)
      {
        case Orientation.Horizontal:
          return point.X > elementLayoutBounds.Right - elementLayoutBounds.Width / 2.0;
        case Orientation.Vertical:
          return point.Y > elementLayoutBounds.Bottom - elementLayoutBounds.Height / 2.0;
        default:
          throw new NotImplementedException(ExceptionStringTable.StackPanelUnrecognizedOrientation);
      }
    }

    private static bool IsPointInLine(Point point, FlowPanelLayoutUtilities.LineInfo line, Orientation orientation)
    {
      switch (orientation)
      {
        case Orientation.Horizontal:
          if (line.StartPoint.Y < point.Y)
            return point.Y < line.StartPoint.Y + line.LineLength;
          return false;
        case Orientation.Vertical:
          if (line.StartPoint.X < point.X)
            return point.X < line.StartPoint.X + line.LineLength;
          return false;
        default:
          throw new NotImplementedException(ExceptionStringTable.StackPanelUnrecognizedOrientation);
      }
    }

    internal static List<FlowPanelLayoutUtilities.LineInfo> ExtractLines(SceneElement container)
    {
      List<FlowPanelLayoutUtilities.LineInfo> list = new List<FlowPanelLayoutUtilities.LineInfo>();
      IViewPanel flowPanel = MoveStrategy.GetContainerHost(container) as IViewPanel;
      Orientation orientation = flowPanel.Orientation;
      double num = -1.0;
      Point startPoint = new Point(0.0, 0.0);
      FlowPanelLayoutUtilities.InitializeStartPoint(ref startPoint, flowPanel);
      int startElementIndex = 0;
      int endElementIndex = 0;
      ActualBoundsInParent actualBoundsInParent = new ActualBoundsInParent(container);
      for (int index = 0; index < flowPanel.ChildrenCount; ++index)
      {
        IViewVisual child1 = flowPanel.GetChild(index);
        Rect rect = actualBoundsInParent[child1];
        num = FlowPanelLayoutUtilities.UpdateLineLength(num, startPoint, rect, orientation);
        if (endElementIndex + 1 < flowPanel.ChildrenCount)
        {
          IViewVisual child2 = flowPanel.GetChild(endElementIndex + 1);
          Rect nextElementLayoutBounds = actualBoundsInParent[child2];
          if (FlowPanelLayoutUtilities.IsNextElementStartOfNewLine(rect, nextElementLayoutBounds, orientation))
          {
            list.Add(new FlowPanelLayoutUtilities.LineInfo(startPoint, num, startElementIndex, endElementIndex));
            startElementIndex = endElementIndex + 1;
            FlowPanelLayoutUtilities.UpdateStartPoint(ref startPoint, num, orientation);
            num = -1.0;
          }
          ++endElementIndex;
        }
        else
        {
          list.Add(new FlowPanelLayoutUtilities.LineInfo(startPoint, num, startElementIndex, endElementIndex));
          break;
        }
      }
      return list;
    }

    private static bool IsNextElementStartOfNewLine(Rect currentElementLayoutBounds, Rect nextElementLayoutBounds, Orientation orientation)
    {
      switch (orientation)
      {
        case Orientation.Horizontal:
          return Tolerances.LessThan(nextElementLayoutBounds.Left, currentElementLayoutBounds.Right);
        case Orientation.Vertical:
          return Tolerances.LessThan(nextElementLayoutBounds.Top, currentElementLayoutBounds.Bottom);
        default:
          throw new NotImplementedException(ExceptionStringTable.StackPanelUnrecognizedOrientation);
      }
    }

    private static void InitializeStartPoint(ref Point startPoint, IViewPanel flowPanel)
    {
      switch (flowPanel.Orientation)
      {
        case Orientation.Horizontal:
        case Orientation.Vertical:
          startPoint = new Point(0.0, 0.0);
          break;
        default:
          throw new NotImplementedException(ExceptionStringTable.StackPanelUnrecognizedOrientation);
      }
    }

    private static void UpdateStartPoint(ref Point startPoint, double precedinglineLength, Orientation orientation)
    {
      switch (orientation)
      {
        case Orientation.Horizontal:
          startPoint.Y += precedinglineLength;
          break;
        case Orientation.Vertical:
          startPoint.X += precedinglineLength;
          break;
        default:
          throw new NotImplementedException(ExceptionStringTable.StackPanelUnrecognizedOrientation);
      }
    }

    private static double UpdateLineLength(double lineLength, Point startPoint, Rect layoutBounds, Orientation orientation)
    {
      double num = lineLength;
      switch (orientation)
      {
        case Orientation.Horizontal:
          if (lineLength < layoutBounds.Bottom - startPoint.Y)
          {
            num = layoutBounds.Bottom - startPoint.Y;
            break;
          }
          break;
        case Orientation.Vertical:
          if (lineLength < layoutBounds.Right - startPoint.X)
          {
            num = layoutBounds.Right - startPoint.X;
            break;
          }
          break;
        default:
          throw new NotImplementedException(ExceptionStringTable.StackPanelUnrecognizedOrientation);
      }
      return num;
    }

    internal class LineInfo
    {
      private Point startPoint;
      private double lineLength;
      private int startElementIndex;
      private int endElementIndex;

      public Point StartPoint
      {
        get
        {
          return this.startPoint;
        }
      }

      public double LineLength
      {
        get
        {
          return this.lineLength;
        }
      }

      public int StartElementIndex
      {
        get
        {
          return this.startElementIndex;
        }
      }

      public int EndElementIndex
      {
        get
        {
          return this.endElementIndex;
        }
      }

      public LineInfo(Point startPoint, double lineLength, int startElementIndex, int endElementIndex)
      {
        this.startPoint = new Point(startPoint.X, startPoint.Y);
        this.lineLength = lineLength;
        this.startElementIndex = startElementIndex;
        this.endElementIndex = endElementIndex;
      }
    }
  }
}
