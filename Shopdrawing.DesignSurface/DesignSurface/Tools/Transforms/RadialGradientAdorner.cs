// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.RadialGradientAdorner
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
  internal sealed class RadialGradientAdorner : BrushAdorner, IClickable
  {
    private RadialGradientAdornerKind kind;

    public RadialGradientAdornerKind Kind
    {
      get
      {
        return this.kind;
      }
    }

    public Point GradientOriginPoint
    {
      get
      {
        Point point = new Point();
        if (PlatformTypes.IsInstance(this.PlatformBrush, PlatformTypes.RadialGradientBrush, (ITypeResolver) this.Element.ProjectContext))
          point = this.TransformPoint((Point) this.GetBrushPropertyAsWpf(RadialGradientBrushNode.GradientOriginProperty), true);
        return point;
      }
    }

    public Point RadiusPoint
    {
      get
      {
        return this.TransformPoint(this.GetRadialGradientEndPoint(), true);
      }
    }

    public RadialGradientAdorner(BrushTransformAdornerSet adornerSet, RadialGradientAdornerKind kind)
      : base(adornerSet)
    {
      this.kind = kind;
    }

    public override Point GetClickablePoint(Matrix matrix)
    {
      switch (this.kind)
      {
        case RadialGradientAdornerKind.GradientOriginPoint:
          return this.GetOffsetStartPoint(matrix, 13.0);
        case RadialGradientAdornerKind.RadiusPoint:
          return this.GetOffsetEndPoint(matrix, 11.0);
        default:
          throw new NotImplementedException(ExceptionStringTable.UnknownRadialGradientAdorner);
      }
    }

    public override void Draw(DrawingContext context, Matrix matrix)
    {
      if (!PlatformTypes.IsInstance(this.PlatformBrush, PlatformTypes.RadialGradientBrush, (ITypeResolver) this.Element.ProjectContext))
        return;
      switch (this.kind)
      {
        case RadialGradientAdornerKind.GradientOriginPoint:
          this.DrawArrowTail(context, matrix, this.GradientOriginPoint, this.RadiusPoint);
          break;
        case RadialGradientAdornerKind.RadiusPoint:
          this.DrawArrowHead(context, matrix, this.GradientOriginPoint, this.RadiusPoint);
          break;
        default:
          throw new NotImplementedException(ExceptionStringTable.UnknownRadialGradientAdorner);
      }
    }
  }
}
