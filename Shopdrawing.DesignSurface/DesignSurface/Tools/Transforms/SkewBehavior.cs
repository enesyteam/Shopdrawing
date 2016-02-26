// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.SkewBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal sealed class SkewBehavior : AdornedToolBehavior
  {
    private CanonicalTransform canonicalTransform;
    private Matrix documentToObjectTransform;
    private bool behaviorEnabled;
    private Point adornerPosition;
    private Point startPointerPosition;
    private Vector displacementVector;
    private Vector transformedDisplacementVector;
    private Point oppositeAdornerPosition;
    private bool skewAroundCenter;

    public override string ActionString
    {
      get
      {
        return StringTable.UndoUnitSkew;
      }
    }

    private RotateAdorner ActiveAdorner
    {
      get
      {
        return (RotateAdorner) base.ActiveAdorner;
      }
    }

    private BaseFrameworkElement BaseEditingElement
    {
      get
      {
        return this.EditingElement as BaseFrameworkElement;
      }
    }

    public SkewBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    protected override bool OnButtonDown(Point pointerPosition)
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.SkewElement);
      this.Initialize(pointerPosition);
      this.EnsureEditTransaction();
      return true;
    }

    internal override bool ShouldMotionlessAutoScroll(Point mousePoint, Rect artboardBoundary)
    {
      return false;
    }

    protected override bool OnDrag(Point dragStartPosition, Point dragCurrentPosition, bool scrollNow)
    {
      this.skewAroundCenter = this.IsAltDown;
      this.UpdateSkew();
      this.UpdateEditTransaction();
      this.ActiveView.EnsureVisible((IAdorner) this.ActiveAdorner);
      return true;
    }

    protected override bool OnDragEnd(Point dragStartPosition, Point dragEndPosition)
    {
      this.AllDone();
      return base.OnDragEnd(dragStartPosition, dragEndPosition);
    }

    protected override bool OnClickEnd(Point pointerPosition, int clickCount)
    {
      this.AllDone();
      return base.OnClickEnd(pointerPosition, clickCount);
    }

    private void AllDone()
    {
      this.behaviorEnabled = false;
      this.CommitEditTransaction();
    }

    protected override bool OnKey(KeyEventArgs args)
    {
      bool isAltDown = this.IsAltDown;
      if (this.skewAroundCenter != isAltDown)
      {
        this.skewAroundCenter = isAltDown;
        this.UpdateSkew();
        this.UpdateEditTransaction();
      }
      return true;
    }

    private void Initialize(Point pointerPosition)
    {
      this.canonicalTransform = new CanonicalTransform((Transform) this.EditingElement.GetComputedValueAsWpf(Base2DElement.RenderTransformProperty));
      this.adornerPosition = this.ActiveAdorner.GetOffsetAnchorPoint(Matrix.Identity, 2.0);
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
          throw new InvalidOperationException(ExceptionStringTable.SkewBehaviorInvalidAnchor);
      }
      this.startPointerPosition = pointerPosition;
      Matrix computedTransformToRoot = this.ActiveView.GetComputedTransformToRoot(this.EditingElement);
      if (computedTransformToRoot.HasInverse)
      {
        Matrix matrix = computedTransformToRoot;
        matrix.Invert();
        this.documentToObjectTransform = matrix;
        this.behaviorEnabled = true;
      }
      else
        this.behaviorEnabled = false;
      this.transformedDisplacementVector = this.displacementVector * computedTransformToRoot;
      this.transformedDisplacementVector /= this.transformedDisplacementVector.Length;
    }

    private void UpdateSkew()
    {
      if (!this.behaviorEnabled)
        return;
      Vector vector1 = this.MouseDevice.GetPosition((IInputElement) this.ActiveView.ViewRootContainer) - this.startPointerPosition;
      double num1 = vector1.X * this.transformedDisplacementVector.X + vector1.Y * this.transformedDisplacementVector.Y;
      vector1.X = this.transformedDisplacementVector.X * num1;
      vector1.Y = this.transformedDisplacementVector.Y * num1;
      Vector vector2 = vector1 * this.documentToObjectTransform;
      double num2 = vector2.X * this.displacementVector.X + vector2.Y * this.displacementVector.Y;
      CanonicalTransform newTransform = new CanonicalTransform(this.canonicalTransform);
      Point fixedPoint = !this.skewAroundCenter ? this.oppositeAdornerPosition : this.BaseEditingElement.RenderTransformOriginInElementCoordinates;
      Vector vector3 = fixedPoint - this.adornerPosition;
      if (vector3.LengthSquared < FloatingPointArithmetic.DistanceTolerance)
        return;
      Vector vector4 = vector3 / vector3.Length;
      double num3 = vector4.X * this.displacementVector.X + vector4.Y * this.displacementVector.Y;
      if ((this.displacementVector - vector4 * num3).LengthSquared < FloatingPointArithmetic.DistanceTolerance)
        return;
      newTransform.ApplySkewScale(this.displacementVector, this.adornerPosition - fixedPoint, this.BaseEditingElement.RenderTransformOriginInElementCoordinates, fixedPoint, this.displacementVector, this.adornerPosition - fixedPoint + this.displacementVector * num2);
      newTransform.ScaleX = RoundingHelper.RoundScale(newTransform.ScaleX);
      newTransform.ScaleY = RoundingHelper.RoundScale(newTransform.ScaleY);
      newTransform.SkewX = RoundingHelper.RoundAngle(newTransform.SkewX);
      newTransform.SkewY = RoundingHelper.RoundAngle(newTransform.SkewY);
      newTransform.TranslationX = RoundingHelper.RoundLength(newTransform.TranslationX);
      newTransform.TranslationY = RoundingHelper.RoundLength(newTransform.TranslationY);
      if ((Math.Abs(newTransform.Skew.X) - 90.0) * (Math.Abs(newTransform.Skew.X) - 90.0) < FloatingPointArithmetic.DistanceTolerance || (Math.Abs(newTransform.Skew.Y) - 90.0) * (Math.Abs(newTransform.Skew.Y) - 90.0) < FloatingPointArithmetic.DistanceTolerance)
        return;
      AdornedToolBehavior.UpdateElementTransform(this.EditingElement, newTransform, AdornedToolBehavior.TransformPropertyFlags.Scale | AdornedToolBehavior.TransformPropertyFlags.Skew | AdornedToolBehavior.TransformPropertyFlags.Translation);
    }

    private Point GetAnchorPointAdornerPosition(EdgeFlags edgeFlags, Matrix matrix)
    {
      return new RotateAdorner(this.ActiveAdorner.AdornerSet, edgeFlags).GetAnchorPoint(matrix);
    }
  }
}
