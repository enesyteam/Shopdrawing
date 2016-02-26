// Decompiled with JetBrains decompiler
// Type: MS.Internal.Interaction.VisualHitTestArgs
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using MS.Internal;
using MS.Internal.Transforms;
using System;
using System.Windows;
using System.Windows.Media;

namespace MS.Internal.Interaction
{
  internal class VisualHitTestArgs
  {
    private Visual _sourceAncestor;
    private Visual _child;
    private HitTestParameters _hitTestParameters;
    private Point? _childPoint;

    public Visual ChildVisual
    {
      get
      {
        return this._child;
      }
    }

    public Visual SourceAncestor
    {
      get
      {
        return this._sourceAncestor;
      }
    }

    public Point ChildPoint
    {
      get
      {
        if (!this._childPoint.HasValue)
        {
          PointHitTestParameters hitTestParameters = this._hitTestParameters as PointHitTestParameters;
          if (hitTestParameters != null)
            this._childPoint = new Point?(TransformUtil.SafeInvert(TransformUtil.GetSelectionFrameTransformToParentVisual((DependencyObject) this._child, this._sourceAncestor)).Transform(hitTestParameters.HitPoint));
        }
        return this._childPoint.Value;
      }
    }

    public HitTestParameters HitTestParameters
    {
      get
      {
        return this._hitTestParameters;
      }
    }

    public Rect ChildLayoutBounds
    {
      get
      {
        return new Rect(new Point(), ElementUtilities.GetSelectionFrameBounds((DependencyObject) this._child).Size);
      }
    }

    public Geometry SourceHitGeometry
    {
      get
      {
        GeometryHitTestParameters hitTestParameters = this._hitTestParameters as GeometryHitTestParameters;
        if (hitTestParameters != null)
          return hitTestParameters.HitGeometry;
        return (Geometry) null;
      }
    }

    public VisualHitTestArgs(Visual sourceAncestor, Visual child, Point parentPoint)
      : this(sourceAncestor, child, (HitTestParameters) new PointHitTestParameters(parentPoint))
    {
    }

    public VisualHitTestArgs(Visual sourceAncestor, Visual child, HitTestParameters parameters)
    {
      if (sourceAncestor == null)
        throw new ArgumentNullException("sourceAncestor");
      if (child == null)
        throw new ArgumentNullException("child");
      this._sourceAncestor = sourceAncestor;
      this._child = child;
      this._hitTestParameters = parameters;
    }

    internal void UpdateChild(DependencyObject child)
    {
      this._child = child as Visual;
      this._childPoint = new Point?();
    }
  }
}
