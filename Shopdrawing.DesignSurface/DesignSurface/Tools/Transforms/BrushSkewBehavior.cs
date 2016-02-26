// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.BrushSkewBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class BrushSkewBehavior : BrushTransformBehavior
  {
    private Matrix documentToObjectTransform;
    private Point adornerPosition;
    private Point lastPointerPosition;
    private Point currentPointerPosition;
    private Vector displacementVector;
    private Vector transformedDisplacementVector;
    private Point oppositeAdornerPosition;
    private bool skewAroundCenter;

    public override string ActionString
    {
      get
      {
        return StringTable.UndoUnitSkewBrush;
      }
    }

    private BrushRotateAdorner ActiveAdorner
    {
      get
      {
        return (BrushRotateAdorner) base.ActiveAdorner;
      }
    }

    public BrushSkewBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    protected override bool OnButtonDown(Point pointerPosition)
    {
      this.Initialize(pointerPosition);
      return true;
    }

    internal override bool ShouldMotionlessAutoScroll(Point mousePoint, Rect artboardBoundary)
    {
      return false;
    }

    protected override bool OnDrag(Point dragStartPosition, Point dragCurrentPosition, bool scrollNow)
    {
      this.currentPointerPosition = dragCurrentPosition;
      this.skewAroundCenter = this.IsAltDown;
      this.UpdateSkew();
      this.ActiveView.EnsureVisible((IAdorner) this.ActiveAdorner);
      return true;
    }

    protected override bool OnDragEnd(Point dragStartPosition, Point dragEndPosition)
    {
      this.CommitEditTransaction();
      return base.OnDragEnd(dragStartPosition, dragEndPosition);
    }

    protected override bool OnClickEnd(Point pointerPosition, int clickCount)
    {
      this.CommitEditTransaction();
      return base.OnClickEnd(pointerPosition, clickCount);
    }

    protected override bool OnKey(KeyEventArgs args)
    {
      bool isAltDown = this.IsAltDown;
      if (this.skewAroundCenter != isAltDown)
      {
        this.skewAroundCenter = isAltDown;
        this.UpdateSkew();
      }
      return true;
    }

    private void Initialize(Point pointerPosition)
    {
      this.adornerPosition = this.ActiveAdorner.GetAnchorPoint(Matrix.Identity);
      this.adornerPosition = this.ActiveAdorner.InverseTransformPoint(this.adornerPosition, false);
      this.skewAroundCenter = this.IsAltDown;
      switch (this.ActiveAdorner.EdgeFlags)
      {
        case EdgeFlags.Left:
          this.displacementVector = new Vector(0.0, 1.0);
          this.oppositeAdornerPosition = this.GetAnchorPointAdornerPosition(EdgeFlags.Right, Matrix.Identity);
          break;
        case EdgeFlags.Top:
          this.displacementVector = new Vector(1.0, 0.0);
          this.oppositeAdornerPosition = this.GetAnchorPointAdornerPosition(EdgeFlags.Bottom, Matrix.Identity);
          break;
        case EdgeFlags.Right:
          this.displacementVector = new Vector(0.0, -1.0);
          this.oppositeAdornerPosition = this.GetAnchorPointAdornerPosition(EdgeFlags.Left, Matrix.Identity);
          break;
        case EdgeFlags.Bottom:
          this.displacementVector = new Vector(-1.0, 0.0);
          this.oppositeAdornerPosition = this.GetAnchorPointAdornerPosition(EdgeFlags.Top, Matrix.Identity);
          break;
        default:
          throw new InvalidOperationException(ExceptionStringTable.CanOnlySkewOnTopLeftBottomOrRightAdorner);
      }
      this.oppositeAdornerPosition = this.ActiveAdorner.InverseTransformPoint(this.oppositeAdornerPosition, false);
      this.documentToObjectTransform = this.ActiveView.GetComputedTransformFromRoot(this.EditingElement);
      this.lastPointerPosition = pointerPosition;
      Matrix matrix = this.documentToObjectTransform;
      matrix.Invert();
      this.transformedDisplacementVector = this.ActiveAdorner.TransformVector(this.displacementVector, false) * matrix;
      this.transformedDisplacementVector /= this.transformedDisplacementVector.Length;
    }

    private void UpdateSkew()
    {
      Vector vector1 = this.transformedDisplacementVector * Vector.Multiply(this.currentPointerPosition - this.lastPointerPosition, this.transformedDisplacementVector);
      this.lastPointerPosition = this.currentPointerPosition;
      double num1 = Vector.Multiply(this.ActiveAdorner.InverseTransformVector(vector1 * this.documentToObjectTransform, false), this.displacementVector);
      Point relativeBrushCenter = this.ActiveAdorner.RelativeBrushCenter;
      Point fixedPoint = !this.skewAroundCenter ? this.oppositeAdornerPosition : relativeBrushCenter;
      Vector vector2 = fixedPoint - this.adornerPosition;
      if (vector2.LengthSquared < FloatingPointArithmetic.DistanceTolerance)
        return;
      vector2 /= vector2.Length;
      double num2 = vector2.X * this.displacementVector.X + vector2.Y * this.displacementVector.Y;
      if ((this.displacementVector - vector2 * num2).LengthSquared < FloatingPointArithmetic.DistanceTolerance)
        return;
      CanonicalTransform transform = new CanonicalTransform(this.ActiveAdorner.BrushTransform);
      transform.UpdateCenter(relativeBrushCenter);
      transform.ApplySkewScale(this.displacementVector, this.adornerPosition - fixedPoint, transform.Center, fixedPoint, this.displacementVector, this.adornerPosition - fixedPoint + this.displacementVector * num1);
      if ((Math.Abs(transform.SkewX) - 90.0) * (Math.Abs(transform.SkewX) - 90.0) < FloatingPointArithmetic.DistanceTolerance || (Math.Abs(transform.SkewY) - 90.0) * (Math.Abs(transform.SkewY) - 90.0) < FloatingPointArithmetic.DistanceTolerance)
        return;
      this.SetBrushTransform(transform);
    }

    private Point GetAnchorPointAdornerPosition(EdgeFlags edgeFlags, Matrix matrix)
    {
      return new BrushRotateAdorner(this.ActiveAdorner.AdornerSet, edgeFlags).GetAnchorPoint(matrix);
    }
  }
}
