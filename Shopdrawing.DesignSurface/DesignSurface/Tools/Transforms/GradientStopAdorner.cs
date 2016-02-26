// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.GradientStopAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal sealed class GradientStopAdorner : BrushAdorner, IClickable
  {
    public bool Hidden { get; set; }

    public int Index { get; set; }

    public GradientStopAdorner(BrushTransformAdornerSet adornerSet, int index)
      : base(adornerSet)
    {
      this.Index = index;
    }

    public override Point GetClickablePoint(Matrix matrix)
    {
      if (PlatformTypes.IsInstance(this.PlatformBrush, PlatformTypes.GradientBrush, (ITypeResolver) this.Element.ProjectContext))
      {
        GradientStopCollection gradientStopCollection = (GradientStopCollection) this.GetBrushPropertyAsWpf(GradientBrushNode.GradientStopsProperty);
        Point startPoint;
        Point endPoint;
        if (this.GetBrushEndpoints(out startPoint, out endPoint) && this.Index >= 0 && this.Index < gradientStopCollection.Count)
        {
          GradientStop gradientStop = gradientStopCollection[this.Index];
          if (gradientStop != null)
          {
            Matrix matrix1 = this.GetCompleteBrushTransformMatrix(true) * matrix;
            Point point = startPoint * matrix1;
            endPoint *= matrix1;
            return (endPoint - point) * gradientStop.Offset + point;
          }
        }
      }
      return new Point(0.0, 0.0);
    }

    public override void Draw(DrawingContext context, Matrix matrix)
    {
      if (this.Hidden || !this.ShouldDraw || !PlatformTypes.IsInstance(this.PlatformBrush, PlatformTypes.GradientBrush, (ITypeResolver) this.Element.ProjectContext))
        return;
      Matrix matrix1 = this.GetCompleteBrushTransformMatrix(true) * matrix;
      PropertyReference propertyReference1 = this.AdornerSet.BrushPropertyReference.Append(GradientBrushNode.GradientStopsProperty);
      int num1 = (int) this.Element.GetComputedValue(propertyReference1.Append(GradientStopCollectionNode.CountProperty));
      Point startPoint;
      Point endPoint;
      if (!this.GetBrushEndpoints(out startPoint, out endPoint) || this.Index < 0 || this.Index >= num1)
        return;
      PropertyReference propertyReference2 = propertyReference1.Append((ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep((IPlatformMetadata) this.Element.ViewModel.ProjectContext.Platform.Metadata, PlatformTypes.GradientStopCollection, this.Index));
      PropertyReference propertyReference3 = propertyReference2.Append(GradientStopNode.OffsetProperty);
      PropertyReference propertyReference4 = propertyReference2.Append(GradientStopNode.ColorProperty);
      double num2 = (double) this.Element.GetComputedValue(propertyReference3);
      int index = this.DesignerContext.GradientToolSelectionService.Index;
      Point point = (endPoint - startPoint) * num2 + startPoint;
      double num3 = 3.5;
      Color color = (Color) this.Element.GetComputedValueAsWpf(propertyReference4);
      color.A = byte.MaxValue;
      SolidColorBrush solidColorBrush = new SolidColorBrush(color);
      Pen pen = this.ThinPen;
      if (index == this.Index)
      {
        pen = this.ThickPen;
        num3 = 4.5;
      }
      context.DrawEllipse((Brush) Brushes.Transparent, (Pen) null, point * matrix1, num3 + 4.0, num3 + 4.0);
      context.DrawEllipse((Brush) solidColorBrush, pen, point * matrix1, num3, num3);
    }
  }
}
