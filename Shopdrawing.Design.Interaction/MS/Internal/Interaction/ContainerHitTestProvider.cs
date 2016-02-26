// Decompiled with JetBrains decompiler
// Type: MS.Internal.Interaction.ContainerHitTestProvider
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using MS.Internal.Transforms;
using System.Windows;
using System.Windows.Media;

namespace MS.Internal.Interaction
{
  internal class ContainerHitTestProvider : HitTestProvider
  {
    public override PointHitTestResult HitTestPoint(VisualHitTestArgs args)
    {
      if (args.ChildLayoutBounds.Contains(args.ChildPoint))
        return new PointHitTestResult(args.ChildVisual, args.ChildPoint);
      return (PointHitTestResult) null;
    }

    public override GeometryHitTestResult HitTestGeometry(VisualHitTestArgs args)
    {
      Rect rect = TransformUtil.GetTransformToAncestor((DependencyObject) args.ChildVisual, args.SourceAncestor).TransformBounds(args.ChildLayoutBounds);
      Rect bounds = args.SourceHitGeometry.Bounds;
      if (args.SourceHitGeometry.FillContains((Geometry) new RectangleGeometry(rect)))
        return new GeometryHitTestResult(args.ChildVisual, IntersectionDetail.FullyInside);
      if (rect.Contains(bounds))
        return new GeometryHitTestResult(args.ChildVisual, IntersectionDetail.FullyContains);
      if (bounds.IntersectsWith(rect))
        return new GeometryHitTestResult(args.ChildVisual, IntersectionDetail.Intersects);
      return new GeometryHitTestResult(args.ChildVisual, IntersectionDetail.Empty);
    }
  }
}
