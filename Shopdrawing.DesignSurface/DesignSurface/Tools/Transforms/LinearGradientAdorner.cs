// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.LinearGradientAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal sealed class LinearGradientAdorner : BrushAdorner, IClickable
  {
    private LinearGradientAdornerKind kind;

    public LinearGradientAdornerKind Kind
    {
      get
      {
        return this.kind;
      }
    }

    public Point StartPoint
    {
      get
      {
        Point point = new Point();
        if (PlatformTypes.IsInstance(this.PlatformBrush, PlatformTypes.LinearGradientBrush, (ITypeResolver) this.Element.ProjectContext))
          point = this.TransformPoint((Point) this.GetBrushPropertyAsWpf(LinearGradientBrushNode.StartPointProperty), true);
        return point;
      }
    }

    public Point EndPoint
    {
      get
      {
        Point point = new Point();
        if (PlatformTypes.IsInstance(this.PlatformBrush, PlatformTypes.LinearGradientBrush, (ITypeResolver) this.Element.ProjectContext))
          point = this.TransformPoint((Point) this.GetBrushPropertyAsWpf(LinearGradientBrushNode.EndPointProperty), true);
        return point;
      }
    }

    public LinearGradientAdorner(BrushTransformAdornerSet adornerSet, LinearGradientAdornerKind kind)
      : base(adornerSet)
    {
      this.kind = kind;
    }

    public override Point GetClickablePoint(Matrix matrix)
    {
      switch (this.kind)
      {
        case LinearGradientAdornerKind.StartPoint:
          return this.GetOffsetStartPoint(matrix, 13.0);
        case LinearGradientAdornerKind.EndPoint:
          return this.GetOffsetEndPoint(matrix, 11.0);
        case LinearGradientAdornerKind.StartRotation:
          return this.GetOffsetStartPoint(matrix, 23.0);
        case LinearGradientAdornerKind.EndRotation:
          return this.GetOffsetEndPoint(matrix, 19.0);
        default:
          throw new NotImplementedException(ExceptionStringTable.UnknownLinearGradientAdorner);
      }
    }

    public override void Draw(DrawingContext context, Matrix matrix)
    {
      if (!PlatformTypes.IsInstance(this.PlatformBrush, PlatformTypes.LinearGradientBrush, (ITypeResolver) this.Element.ProjectContext))
        return;
      switch (this.kind)
      {
        case LinearGradientAdornerKind.StartPoint:
          this.DrawArrowTail(context, matrix, this.StartPoint, this.EndPoint);
          break;
        case LinearGradientAdornerKind.EndPoint:
          this.DrawArrowHead(context, matrix, this.StartPoint, this.EndPoint);
          break;
        case LinearGradientAdornerKind.StartRotation:
          context.DrawEllipse((Brush) Brushes.Transparent, (Pen) null, this.GetOffsetStartPoint(matrix, RotateAdornerHelper.Radius), RotateAdornerHelper.Radius, RotateAdornerHelper.Radius);
          break;
        case LinearGradientAdornerKind.EndRotation:
          context.DrawEllipse((Brush) Brushes.Transparent, (Pen) null, this.GetOffsetEndPoint(matrix, RotateAdornerHelper.Radius), RotateAdornerHelper.Radius, RotateAdornerHelper.Radius);
          break;
        default:
          throw new NotImplementedException(ExceptionStringTable.UnknownLinearGradientAdorner);
      }
    }
  }
}
