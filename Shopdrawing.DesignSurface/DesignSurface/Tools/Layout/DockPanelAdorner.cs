// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Layout.DockPanelAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Layout
{
  internal class DockPanelAdorner : DockAdorner
  {
    public DockPanelAdorner(AdornerSet adornerSet, Dock dock)
      : base(adornerSet, dock)
    {
    }

    public override Point GetClickablePoint(Matrix matrix)
    {
      CanonicalTransform canonicalTransform = new CanonicalTransform((Transform) new MatrixTransform(matrix));
      ScaleTransform scaleTransform = new ScaleTransform(canonicalTransform.ScaleX, canonicalTransform.ScaleY);
      canonicalTransform.Scale = new Vector(1.0, 1.0);
      Point adornerPosition = this.GetAdornerPosition(scaleTransform);
      return new Point(adornerPosition.X + this.Icon.Bounds.Width / 2.0, adornerPosition.Y + this.Icon.Bounds.Height / 2.0) * canonicalTransform.TransformGroup.Value;
    }

    public override void Draw(DrawingContext context, Matrix matrix)
    {
      if (this.Icon == null)
        return;
      if (!this.IsActive)
        context.PushOpacity(0.5);
      CanonicalTransform canonicalTransform = new CanonicalTransform((Transform) new MatrixTransform(matrix));
      ScaleTransform scaleTransform = new ScaleTransform(canonicalTransform.ScaleX, canonicalTransform.ScaleY);
      canonicalTransform.Scale = new Vector(1.0, 1.0);
      Transform transform = DockAdorner.RemoveScaleFromCanonicalTransform(canonicalTransform);
      context.PushTransform(transform);
      Point adornerPosition = this.GetAdornerPosition(scaleTransform);
      context.PushTransform((Transform) new TranslateTransform(adornerPosition.X, adornerPosition.Y));
      context.DrawDrawing(this.Icon);
      context.Pop();
      context.Pop();
      if (this.IsActive)
        return;
      context.Pop();
    }

    private Point GetAdornerPosition(ScaleTransform scaleTransform)
    {
      Rect fillRegionRectangle = DockPanelAdorner.GetFillRegionRectangle((DockPanelElement) this.Element);
      Point point1 = new Point(fillRegionRectangle.Left + fillRegionRectangle.Width / 2.0, fillRegionRectangle.Top) * scaleTransform.Value;
      Point point2 = new Point(fillRegionRectangle.Left + fillRegionRectangle.Width / 2.0, fillRegionRectangle.Bottom) * scaleTransform.Value;
      Point point3 = new Point(fillRegionRectangle.Left, fillRegionRectangle.Height / 2.0) * scaleTransform.Value;
      double num1 = ((new Point(fillRegionRectangle.Right, fillRegionRectangle.Height / 2.0) * scaleTransform.Value).X - point3.X) / 2.0;
      double num2 = (point2.Y - point1.Y) / 2.0;
      double val1_1 = Math.Max(DockAdorner.DockBottomIcon.Bounds.Width, DockAdorner.DockTopIcon.Bounds.Width);
      Math.Max(DockAdorner.DockLeftIcon.Bounds.Height, DockAdorner.DockRightIcon.Bounds.Height);
      double val1_2 = Math.Max(val1_1, DockAdorner.DockFillIcon.Bounds.Width);
      double num3 = Math.Max(val1_2, DockAdorner.DockFillIcon.Bounds.Height);
      double num4 = this.Icon.Bounds.Width / 2.0;
      double num5 = this.Icon.Bounds.Height / 2.0;
      Point point4 = fillRegionRectangle.TopLeft * scaleTransform.Value;
      switch (this.Dock)
      {
        case Dock.Left:
          return new Point(point4.X + num1 - val1_2 / 2.0 - DockAdorner.Padding - this.Icon.Bounds.Width, point4.Y + num2 - num5);
        case Dock.Top:
          return new Point(point4.X + num1 - val1_2 / 2.0, point4.Y + num2 - num3 / 2.0 - DockAdorner.Padding - this.Icon.Bounds.Height);
        case Dock.Right:
          return new Point(point4.X + num1 + val1_2 / 2.0 + DockAdorner.Padding, point4.Y + num2 - num5);
        case Dock.Bottom:
          return new Point(point4.X + num1 - val1_2 / 2.0, point4.Y + num2 + num3 / 2.0 + DockAdorner.Padding);
        default:
          throw new NotImplementedException(ExceptionStringTable.DockPanelInvalidDock);
      }
    }

    private static Rect GetFillRegionRectangle(DockPanelElement dockPanel)
    {
      Rect computedTightBounds = dockPanel.GetComputedTightBounds();
      double left = computedTightBounds.Left;
      double right = computedTightBounds.Right;
      double top = computedTightBounds.Top;
      double bottom = computedTightBounds.Bottom;
      for (int index = 0; index < DockPanelLayoutUtilities.GetFillRegionInsertionIndex(dockPanel); ++index)
      {
        BaseFrameworkElement frameworkElement = dockPanel.Children[index] as BaseFrameworkElement;
        if (frameworkElement != null)
        {
          Dock dock = (Dock) frameworkElement.GetComputedValue(DockPanelElement.DockProperty);
          Rect computedBounds = frameworkElement.GetComputedBounds((Base2DElement) dockPanel);
          switch (dock)
          {
            case Dock.Left:
              left += computedBounds.Width;
              continue;
            case Dock.Top:
              top += computedBounds.Height;
              continue;
            case Dock.Right:
              right -= computedBounds.Width;
              continue;
            case Dock.Bottom:
              bottom -= computedBounds.Height;
              continue;
            default:
              throw new NotImplementedException(ExceptionStringTable.DockPanelInvalidDock);
          }
        }
      }
      double width = Math.Max(0.0, right - left);
      double height = Math.Max(0.0, bottom - top);
      return new Rect(left, top, width, height);
    }
  }
}
