// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.MotionPathAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  internal class MotionPathAdorner : Adorner
  {
    private static readonly double TimeSnapResolution = 0.125;
    private static readonly double HoldKeyframeArrowTipLength = 3.0;
    private static readonly double HoldKeyframeArrowEndSpacing = 4.5;
    private static readonly double HoldKeyframeArrowStartSpacing = 4.0;
    private const int MaxSegmentDots = 50;
    private MotionPath motionPath;
    private PropertyReference positionXAnimationProperty;
    private PropertyReference positionYAnimationProperty;
    private PropertyReference leftAnimationProperty;
    private PropertyReference topAnimationProperty;
    private bool hasValidTranslateAnimationProperty;

    public MotionPath MotionPath
    {
      get
      {
        return this.motionPath;
      }
    }

    public MotionPathAdorner(AdornerSet adornerSet)
      : base(adornerSet)
    {
      this.motionPath = new MotionPath();
      DesignerContext designerContext = this.DesignerContext;
      this.TryGetTranslateAnimationProperties();
      this.leftAnimationProperty = designerContext.ActiveSceneViewModel.AnimationEditor.GetAnimationProperty((SceneNode) this.Element, new PropertyReference((ReferenceStep) this.Element.Platform.Metadata.ResolveProperty(CanvasElement.LeftProperty)));
      this.topAnimationProperty = designerContext.ActiveSceneViewModel.AnimationEditor.GetAnimationProperty((SceneNode) this.Element, new PropertyReference((ReferenceStep) this.Element.Platform.Metadata.ResolveProperty(CanvasElement.TopProperty)));
    }

    public override void Draw(DrawingContext context, Matrix matrix)
    {
      if (!this.hasValidTranslateAnimationProperty)
        this.TryGetTranslateAnimationProperties();
      if (this.positionXAnimationProperty == null || this.positionYAnimationProperty == null || (this.leftAnimationProperty == null || this.topAnimationProperty == null))
        return;
      StoryboardTimelineSceneNode storyboardTimeline = this.DesignerContext.ActiveSceneViewModel.AnimationEditor.ActiveStoryboardTimeline;
      if (storyboardTimeline == null)
        return;
      KeyFrameAnimationSceneNode animationX = storyboardTimeline.GetAnimation((SceneNode) this.Element, this.positionXAnimationProperty) as KeyFrameAnimationSceneNode;
      KeyFrameAnimationSceneNode animationY = storyboardTimeline.GetAnimation((SceneNode) this.Element, this.positionYAnimationProperty) as KeyFrameAnimationSceneNode;
      KeyFrameAnimationSceneNode animationLeft = storyboardTimeline.GetAnimation((SceneNode) this.Element, this.leftAnimationProperty) as KeyFrameAnimationSceneNode;
      KeyFrameAnimationSceneNode animationTop = storyboardTimeline.GetAnimation((SceneNode) this.Element, this.topAnimationProperty) as KeyFrameAnimationSceneNode;
      if ((animationX == null || animationX.KeyFrameCount == 0) && (animationY == null || animationY.KeyFrameCount == 0) && ((animationLeft == null || animationLeft.KeyFrameCount == 0) && (animationTop == null || animationTop.KeyFrameCount == 0)))
        return;
      this.UpdateMotionPath(animationX, animationY, animationLeft, animationTop);
      this.DrawMotionPath(context, matrix);
    }

    private void TryGetTranslateAnimationProperties()
    {
      if (!this.Element.IsViewObjectValid || !CanonicalTransform.IsCanonical((Transform) this.Element.GetComputedValueAsWpf(Base2DElement.RenderTransformProperty)))
        return;
      this.positionXAnimationProperty = this.AdornerSet.ViewModel.AnimationEditor.GetAnimationProperty((SceneNode) this.Element, this.Element.ViewModel.DefaultView.ConvertFromWpfPropertyReference(this.Element.Platform.Metadata.CommonProperties.RenderTransformTranslationX));
      this.positionYAnimationProperty = this.AdornerSet.ViewModel.AnimationEditor.GetAnimationProperty((SceneNode) this.Element, this.Element.ViewModel.DefaultView.ConvertFromWpfPropertyReference(this.Element.Platform.Metadata.CommonProperties.RenderTransformTranslationY));
      this.hasValidTranslateAnimationProperty = true;
    }

    public static Matrix ComputeBaseValueMatrix(SceneElement element, IViewObject ancestorViewObject)
    {
      if (!element.IsViewObjectValid || ancestorViewObject == null)
        return Matrix.Identity;
      Matrix matrixFromTransform = VectorUtilities.GetMatrixFromTransform(element.ViewModel.DefaultView.ComputeTransformToVisual(element.ViewTargetElement, ancestorViewObject));
      Matrix matrix = Matrix.Identity;
      Matrix identity = Matrix.Identity;
      BaseFrameworkElement frameworkElement = element as BaseFrameworkElement;
      if (frameworkElement != null)
      {
        matrix = ElementUtilities.GetInverseMatrix(frameworkElement.GetRenderTransform());
        Point elementCoordinates = frameworkElement.RenderTransformOriginInElementCoordinates;
        identity.Translate(elementCoordinates.X, elementCoordinates.Y);
      }
      return matrix * identity * matrixFromTransform;
    }

    public static Matrix RefineTransformToAdornerLayer(SceneElement element, Matrix matrix)
    {
      Matrix matrix1 = Matrix.Identity;
      Matrix identity = Matrix.Identity;
      BaseFrameworkElement frameworkElement = element as BaseFrameworkElement;
      if (frameworkElement != null)
      {
        matrix1 = ElementUtilities.GetInverseMatrix(frameworkElement.GetRenderTransform());
        Point elementCoordinates = frameworkElement.RenderTransformOriginInElementCoordinates;
        identity.Translate(elementCoordinates.X, elementCoordinates.Y);
      }
      return matrix1 * identity * matrix;
    }

    private void UpdateMotionPath(KeyFrameAnimationSceneNode animationX, KeyFrameAnimationSceneNode animationY, KeyFrameAnimationSceneNode animationLeft, KeyFrameAnimationSceneNode animationTop)
    {
      BaseFrameworkElement frameworkElement = (BaseFrameworkElement) this.Element;
      double x = 0.0;
      double y = 0.0;
      if (animationX == null)
        animationX = (KeyFrameAnimationSceneNode) KeyFrameAnimationSceneNode.Factory.Instantiate(this.Element.ViewModel, KeyFrameAnimationSceneNode.GetKeyFrameAnimationForType(PlatformTypes.Double, frameworkElement.ProjectContext));
      if (animationLeft == null)
        animationLeft = (KeyFrameAnimationSceneNode) KeyFrameAnimationSceneNode.Factory.Instantiate(this.Element.ViewModel, KeyFrameAnimationSceneNode.GetKeyFrameAnimationForType(PlatformTypes.Double, frameworkElement.ProjectContext));
      else
        x = (double) frameworkElement.GetComputedValue(CanvasElement.LeftProperty);
      if (animationY == null)
        animationY = (KeyFrameAnimationSceneNode) KeyFrameAnimationSceneNode.Factory.Instantiate(this.Element.ViewModel, KeyFrameAnimationSceneNode.GetKeyFrameAnimationForType(PlatformTypes.Double, frameworkElement.ProjectContext));
      if (animationTop == null)
        animationTop = (KeyFrameAnimationSceneNode) KeyFrameAnimationSceneNode.Factory.Instantiate(this.Element.ViewModel, KeyFrameAnimationSceneNode.GetKeyFrameAnimationForType(PlatformTypes.Double, frameworkElement.ProjectContext));
      else
        y = (double) frameworkElement.GetComputedValue(CanvasElement.TopProperty);
      this.motionPath.AnimationX = animationX;
      this.motionPath.AnimationY = animationY;
      this.motionPath.AnimationLeft = animationLeft;
      this.motionPath.AnimationTop = animationTop;
      this.motionPath.TopLeftOffset = new Vector(x, y);
      this.motionPath.UpdatePath();
    }

    private void DrawMotionPath(DrawingContext context, Matrix matrix)
    {
      for (int index1 = 1; index1 < this.motionPath.Times.Count; ++index1)
      {
        double time = this.motionPath.Times[index1 - 1];
        double num1 = this.motionPath.Times[index1];
        int num2 = Math.Min((int) ((num1 - time) / MotionPathAdorner.TimeSnapResolution + 1.0), 50);
        this.motionPath.MoveToTime(time);
        Point point1 = new Point(this.motionPath.XPosition, this.motionPath.YPosition);
        point1 *= matrix;
        this.DrawKeyframeDot(context, point1);
        this.motionPath.MoveToTime(Math.Min(time + MotionPathAdorner.TimeSnapResolution, num1));
        bool xisHolding = this.motionPath.XIsHolding;
        bool yisHolding = this.motionPath.YIsHolding;
        if (xisHolding && yisHolding)
        {
          this.motionPath.MoveToTime(num1);
          Point endPoint = new Point(this.motionPath.XPosition, this.motionPath.YPosition);
          endPoint *= matrix;
          this.DrawArrow(context, point1, endPoint);
        }
        else
        {
          double num3 = num1 - time;
          for (int index2 = 1; index2 < num2 - 1; ++index2)
          {
            double num4 = (double) index2 / (double) (num2 - 1) * num3;
            this.motionPath.MoveToTime(time + num4);
            Point position = new Point(this.motionPath.XPosition, this.motionPath.YPosition);
            position *= matrix;
            this.DrawMotionPathDot(context, position);
          }
        }
        if (xisHolding ^ yisHolding)
        {
          this.motionPath.MoveToTime(num1);
          Point endPoint = new Point(this.motionPath.XPosition, this.motionPath.YPosition);
          endPoint *= matrix;
          Point point2 = !xisHolding ? new Point(endPoint.X, point1.Y) : new Point(point1.X, endPoint.Y);
          this.DrawMotionPathDot(context, point2);
          this.DrawArrow(context, point2, endPoint);
        }
      }
      this.motionPath.MoveToTime(this.motionPath.Times[this.motionPath.Times.Count - 1]);
      Point position1 = new Point(this.motionPath.XPosition, this.motionPath.YPosition) * matrix;
      this.DrawKeyframeDot(context, position1);
    }

    private void DrawArrow(DrawingContext context, Point startPoint, Point endPoint)
    {
      Vector vector1 = endPoint - startPoint;
      if (vector1.Length >= MotionPathAdorner.HoldKeyframeArrowEndSpacing + MotionPathAdorner.HoldKeyframeArrowTipLength)
      {
        Point point0 = startPoint + vector1 * (MotionPathAdorner.HoldKeyframeArrowStartSpacing / vector1.Length);
        Point point = startPoint + vector1 * (1.0 - MotionPathAdorner.HoldKeyframeArrowEndSpacing / vector1.Length);
        vector1 /= vector1.Length;
        Vector vector2 = new Vector(-vector1.Y, vector1.X);
        Vector vector3 = new Vector(vector1.Y, -vector1.X);
        Vector vector4 = vector2 - vector1;
        vector4 /= vector4.Length;
        Vector vector5 = vector3 - vector1;
        vector5 /= vector5.Length;
        Point point1_1 = point + vector4 * MotionPathAdorner.HoldKeyframeArrowTipLength;
        Point point1_2 = point + vector5 * MotionPathAdorner.HoldKeyframeArrowTipLength;
        context.DrawLine(this.ThinPen, point0, point);
        context.DrawLine(this.ThinPen, point, point1_1);
        context.DrawLine(this.ThinPen, point, point1_2);
      }
      else
        context.DrawLine(this.ThinPen, startPoint, endPoint);
    }

    private void DrawKeyframeDot(DrawingContext context, Point position)
    {
      context.DrawRoundedRectangle(this.InactiveBrush, this.ThinPen, new Rect(position - new Vector(1.5, 1.5), position + new Vector(1.5, 1.5)), 1.5, 1.5);
    }

    private void DrawMotionPathDot(DrawingContext context, Point position)
    {
      context.DrawRoundedRectangle(this.ActiveBrush, this.ThinPen, new Rect(position - new Vector(0.5, 0.5), position + new Vector(0.5, 0.5)), 0.5, 0.5);
    }
  }
}
