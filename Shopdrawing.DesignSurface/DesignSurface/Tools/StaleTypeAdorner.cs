// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.StaleTypeAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools
{
  public class StaleTypeAdorner : Adorner, IClickable
  {
    private static readonly Pen BorderPen = new Pen(ErrorAdorner.CreateErrorAdornerBrush(Colors.Orange), 4.0);
    private static readonly DrawingImage ContentDrawing = FileTable.GetDrawingImage("Resources\\Adorners\\StaleTypeAdorner.xaml");
    private const double Margin = 12.0;
    private const double IconSize = 24.0;
    private const double BorderThickness = 4.0;

    static StaleTypeAdorner()
    {
      StaleTypeAdorner.ContentDrawing.Freeze();
    }

    public StaleTypeAdorner(AdornerSet adornerSet)
      : base(adornerSet)
    {
    }

    public override void Draw(DrawingContext drawingContext, Matrix matrix)
    {
      Rect actualBounds = this.Element.ViewModel.DefaultView.GetActualBounds(this.Element.ViewTargetElement);
      if (actualBounds.IsEmpty)
        return;
      drawingContext.PushOpacity(0.5);
      System.Windows.Media.Geometry rectangleGeometry = Adorner.GetTransformedRectangleGeometry(actualBounds, matrix, 4.0);
      drawingContext.DrawGeometry((Brush) Brushes.Transparent, StaleTypeAdorner.BorderPen, rectangleGeometry);
      drawingContext.PushTransform((Transform) new MatrixTransform(matrix));
      double num1 = 1.0 / this.DesignerContext.ActiveView.Zoom;
      double num2 = num1 * 12.0;
      double num3 = num1 * 24.0;
      Rect rectangle = new Rect(actualBounds.Left + num2, actualBounds.Top + num2, num3, num3);
      drawingContext.DrawImage((ImageSource) StaleTypeAdorner.ContentDrawing, rectangle);
      drawingContext.Pop();
      drawingContext.Pop();
    }

    public Point GetClickablePoint(Matrix matrix)
    {
      return this.ElementBounds.TopLeft * matrix;
    }
  }
}
