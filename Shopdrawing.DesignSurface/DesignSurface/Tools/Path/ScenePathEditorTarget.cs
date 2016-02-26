// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.ScenePathEditorTarget
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Designers;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.SceneCommands;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  public sealed class ScenePathEditorTarget : AnimatablePathEditorTarget
  {
    private PathElement pathElement;
    private SceneNodeSubscription<PathElement, PathGeometry> basisSubscription;

    public override PathEditMode PathEditMode
    {
      get
      {
        return PathEditMode.ScenePath;
      }
    }

    public override Base2DElement EditingElement
    {
      get
      {
        return (Base2DElement) this.pathElement;
      }
    }

    protected override bool IsAnimated
    {
      get
      {
        return this.pathElement.HasVertexAnimations;
      }
    }

    public ScenePathEditorTarget(PathElement pathElement)
      : base(pathElement.ViewModel, PathElement.DataProperty, (SceneNode) pathElement)
    {
      this.pathElement = pathElement;
      this.basisSubscription = new SceneNodeSubscription<PathElement, PathGeometry>();
      this.basisSubscription.Path = new SearchPath(new SearchStep[1]
      {
        new SearchStep(new SearchAxis((IPropertyId) pathElement.ProjectContext.ResolveProperty(PathElement.DataProperty)))
      });
      this.basisSubscription.InsertBasisNode((SceneNode) pathElement);
      this.basisSubscription.PathNodeInserted += new SceneNodeSubscription<PathElement, PathGeometry>.PathNodeInsertedListener(((PathEditorTarget) this).OnBasisSubscriptionPathNodeInserted);
      this.basisSubscription.PathNodeRemoved += new SceneNodeSubscription<PathElement, PathGeometry>.PathNodeRemovedListener(((PathEditorTarget) this).OnBasisSubscriptionPathNodeInserted);
      this.UpdateCachedPath();
    }

    public override void UpdateFromDamage(SceneUpdatePhaseEventArgs args)
    {
      base.UpdateFromDamage(args);
      this.basisSubscription.Update(this.ViewModel, args.DocumentChanges, args.DocumentChangeStamp);
    }

    public override void RefreshSubscription()
    {
      if (this.basisSubscription.BasisNodeCount != 0)
        return;
      this.basisSubscription.InsertBasisNode((SceneNode) this.pathElement);
    }

    public override Matrix GetTransformToAncestor(IViewObject ancestorViewObject)
    {
      Matrix matrix = VectorUtilities.GetMatrixFromTransform(this.pathElement.ViewModel.DefaultView.ComputeTransformToVisual(this.pathElement.ViewObject, ancestorViewObject));
      if (this.pathElement.GeometryTransform != null && (Stretch) this.pathElement.GetComputedValueAsWpf(ShapeElement.StretchProperty) != Stretch.None)
        matrix = this.pathElement.GeometryTransform.Value * matrix;
      return matrix;
    }

    public override Matrix RefineTransformToAdornerLayer(Matrix matrix)
    {
      if (this.pathElement.GeometryTransform != null && (Stretch) this.pathElement.GetComputedValueAsWpf(ShapeElement.StretchProperty) != Stretch.None)
        return this.pathElement.GeometryTransform.Value * matrix;
      return matrix;
    }

    public override void RemovePath()
    {
      this.EndEditing(false);
      this.ViewModel.ElementSelectionSet.RemoveSelection((SceneElement) this.pathElement);
      this.ViewModel.AnimationEditor.DeleteAllAnimationsInSubtree((SceneElement) this.pathElement);
      this.ViewModel.RemoveElement((SceneNode) this.pathElement);
    }

    public override void PostDeleteAction()
    {
    }

    public override void UpdateCachedPath()
    {
      PathGeometry original = (PathGeometry) null;
      if (this.pathElement.IsAttached)
          original = this.ViewModel.AnimationEditor.ActiveStoryboardTimeline == null ? this.pathElement.PathGeometry : PathGeometryUtilities.GetPathGeometryFromGeometry(this.pathElement.GetComputedValueAsWpf(PathElement.DataProperty) as System.Windows.Media.Geometry);
      if (original != null)
      {
        this.PathGeometry = PathGeometryUtilities.Copy(original, false);
        if (!this.IsCurrentlyEditing)
          return;
        this.EnsureOnlySingleSegments();
      }
      else
        this.PathGeometry = new PathGeometry();
    }

    public static Transform NormalizePathGeometry(PathGeometry geometry, Rect extent, Rect elementRect)
    {
      Vector offset = new Vector(extent.Left, extent.Top);
      double horizontalScale = extent.Width == 0.0 ? 1.0 : elementRect.Width / extent.Width;
      double verticalScale = extent.Height == 0.0 ? 1.0 : elementRect.Height / extent.Height;
      return ScenePathEditorTarget.UpdateAllPoints(geometry, offset, horizontalScale, verticalScale);
    }

    public static Transform UpdateAllPoints(PathGeometry geometry, Vector offset, double horizontalScale, double verticalScale)
    {
      TransformGroup transformGroup = new TransformGroup();
      transformGroup.Children.Add((Transform) new TranslateTransform(-offset.X, -offset.Y));
      transformGroup.Children.Add((Transform) new ScaleTransform(horizontalScale, verticalScale));
      ScenePathEditorTarget.UpdateAllPoints(geometry, (Transform) transformGroup);
      return (Transform) transformGroup;
    }

    public static void UpdateAllPoints(PathGeometry geometry, Transform transform)
    {
      for (int index1 = 0; index1 < geometry.Figures.Count; ++index1)
      {
        PathFigure figure = geometry.Figures[index1];
        PathFigureEditor pathFigureEditor = new PathFigureEditor(figure);
        int index2 = 1;
        if (!PathFigureUtilities.IsClosed(figure) || !PathFigureUtilities.IsCloseSegmentDegenerate(figure))
          pathFigureEditor.SetPoint(0, transform.Transform(figure.StartPoint));
        for (int index3 = 0; index3 < figure.Segments.Count; ++index3)
        {
          System.Windows.Media.PathSegment segment = figure.Segments[index3];
          int pointCount = PathSegmentUtilities.GetPointCount(segment);
          for (int index4 = 0; index4 < pointCount; ++index4)
          {
            Point point = PathSegmentUtilities.GetPoint(segment, index4);
            pathFigureEditor.SetPoint(index2, transform.Transform(point));
            ++index2;
          }
        }
      }
    }

    protected override void EndEditingInternal(bool pathJustCreated)
    {
      Matrix identity = Matrix.Identity;
      if (!pathJustCreated)
      {
        Transform geometryTransform = this.pathElement.GeometryTransform;
        if (geometryTransform != null)
          identity = geometryTransform.Value;
      }
      Rect extent1 = PathGeometryUtilities.TightExtent(this.PathGeometry, identity);
      Rect extent2 = extent1;
      bool flag1 = false;
      if (this.ViewModel.AnimationEditor.IsKeyFraming)
      {
        if ((Stretch) this.pathElement.GetLocalOrDefaultValueAsWpf(ShapeElement.StretchProperty) != Stretch.None)
        {
          flag1 = true;
          if (!pathJustCreated)
            extent1 = PathGeometryUtilities.TightExtent(this.pathElement.PathGeometry, Matrix.Identity);
          using (this.ViewModel.ForceBaseValue())
          {
            using (this.ViewModel.AnimationEditor.DeferKeyFraming())
            {
              this.pathElement.SetValueAsWpf(ShapeElement.StretchProperty, (object) Stretch.None);
              this.ViewModel.Document.OnUpdatedEditTransaction();
            }
          }
        }
        if (this.pathElement.EnsureSingleSegmentsInPathGeometry())
          this.ViewModel.Document.OnUpdatedEditTransaction();
      }
      Rect extent3 = PathCommandHelper.InflateRectByStrokeWidth(extent1, this.pathElement, pathJustCreated);
      Rect rect1 = PathCommandHelper.InflateRectByStrokeWidth(extent2, this.pathElement, pathJustCreated);
      Rect computedBounds = this.pathElement.GetComputedBounds((Base2DElement) this.pathElement);
      PathGeometry pathGeometry1 = this.PathGeometry;
      if (flag1 && !pathJustCreated)
        ScenePathEditorTarget.NormalizePathGeometry(pathGeometry1, extent3, computedBounds);
      if (flag1)
        this.OnMatrixChanged();
      Vector vector1 = new Vector(extent3.Left, extent3.Top);
      Transform transform = (Transform) null;
      Point centerPoint = new Point();
      bool flag2 = (Stretch) this.pathElement.GetComputedValueAsWpf(ShapeElement.StretchProperty) == Stretch.None && (this.ViewModel.AnimationEditor.IsKeyFraming || this.pathElement.HasVertexAnimations) && !pathJustCreated;
      if (flag2 && !computedBounds.Contains(rect1))
      {
        ILayoutDesigner designerForChild = this.ViewModel.GetLayoutDesignerForChild((SceneElement) this.pathElement, true);
        Rect childRect = designerForChild.GetChildRect((BaseFrameworkElement) this.pathElement);
        double x = Math.Min(0.0, rect1.Left);
        double y = Math.Min(0.0, rect1.Top);
        Vector vector2 = new Vector(x, y);
        centerPoint = this.pathElement.RenderTransformOrigin;
        transform = this.pathElement.RenderTransform;
        if (transform != null && !transform.Value.IsIdentity)
        {
          using (this.ViewModel.ForceBaseValue())
          {
            using (this.ViewModel.AnimationEditor.DeferKeyFraming())
              this.UpdateCenterPoint(new Point(0.0, 0.0));
          }
          vector2 *= transform.Value;
        }
        double num1 = Math.Max(0.0, rect1.Right - computedBounds.Right);
        double num2 = Math.Max(0.0, rect1.Bottom - computedBounds.Bottom);
        childRect.X += vector2.X;
        childRect.Y += vector2.Y;
        childRect.Width += -x + num1;
        childRect.Height += -y + num2;
        using (this.ViewModel.ForceBaseValue())
        {
          using (this.ViewModel.AnimationEditor.DeferKeyFraming())
            designerForChild.SetChildRect((BaseFrameworkElement) this.pathElement, childRect);
        }
        if (x != 0.0 || y != 0.0)
        {
          this.AdjustKeyframes(new Vector(x, y));
          ScenePathEditorTarget.UpdateAllPoints(pathGeometry1, new Vector(x, y), 1.0, 1.0);
          PathGeometry pathGeometry2 = this.pathElement.PathGeometry;
          ScenePathEditorTarget.UpdateAllPoints(pathGeometry2, new Vector(x, y), 1.0, 1.0);
          this.pathElement.PathGeometry = pathGeometry2;
        }
        this.ViewModel.Document.OnUpdatedEditTransaction();
        if (transform != null && !transform.Value.IsIdentity)
        {
          using (this.ViewModel.ForceBaseValue())
          {
            using (this.ViewModel.AnimationEditor.DeferKeyFraming())
              this.UpdateCenterPoint(centerPoint);
          }
        }
      }
      if (!flag2 && (Stretch) this.pathElement.GetComputedValueAsWpf(ShapeElement.StretchProperty) == Stretch.None)
        ScenePathEditorTarget.UpdateAllPoints(pathGeometry1, vector1, 1.0, 1.0);
      if (!flag2)
      {
        using (this.ViewModel.ForceBaseValue())
        {
          using (this.ViewModel.AnimationEditor.DeferKeyFraming())
          {
            centerPoint = this.pathElement.RenderTransformOrigin;
            transform = this.pathElement.RenderTransform;
            if (!extent3.IsEmpty)
            {
              if (transform != null && !transform.Value.IsIdentity)
              {
                this.UpdateCenterPoint(new Point(0.0, 0.0));
                vector1 *= transform.Value;
              }
              ILayoutDesigner designerForChild = this.ViewModel.GetLayoutDesignerForChild((SceneElement) this.pathElement, false);
              Rect rect2 = designerForChild.GetChildRect((BaseFrameworkElement) this.pathElement);
              if (pathJustCreated)
                rect2 = new Rect(0.0, 0.0, 0.0, 0.0);
              double width = extent3.Width;
              double height = extent3.Height;
              rect2.Offset(vector1);
              rect2.Width = width;
              rect2.Height = height;
              designerForChild.SetChildRect((BaseFrameworkElement) this.pathElement, rect2);
            }
          }
        }
      }
      if (this.pathElement.IsAttached && (this.ViewModel.AnimationEditor.IsKeyFraming || this.pathElement.HasVertexAnimations) && !pathJustCreated)
        new PathDiff((Base2DElement) this.pathElement, PathElement.DataProperty, this.PathDiffChangeList).SetPathUsingMinimalDiff(pathGeometry1);
      else
        this.pathElement.PathGeometry = pathGeometry1;
      if (flag2)
        return;
      using (this.ViewModel.ForceBaseValue())
      {
        using (this.ViewModel.AnimationEditor.DeferKeyFraming())
        {
          if (extent3.IsEmpty)
            return;
          this.ViewModel.Document.OnUpdatedEditTransaction();
          if (transform == null || transform.Value.IsIdentity)
            return;
          this.UpdateCenterPoint(centerPoint);
        }
      }
    }

    private void AdjustKeyframes(Vector offset)
    {
      foreach (StoryboardTimelineSceneNode timelineSceneNode1 in this.EditingElement.ViewModel.AnimationEditor.EnumerateStoryboardsForContainer(this.EditingElement.StoryboardContainer))
      {
        foreach (TimelineSceneNode timelineSceneNode2 in (IEnumerable<TimelineSceneNode>) timelineSceneNode1.Children)
        {
          KeyFrameAnimationSceneNode animationSceneNode = timelineSceneNode2 as KeyFrameAnimationSceneNode;
          if (animationSceneNode != null && timelineSceneNode2.TargetElement == this.EditingElement && (timelineSceneNode2.TargetProperty != null && timelineSceneNode2.TargetProperty.FirstStep.Equals((object) PathElement.DataProperty)))
          {
            foreach (KeyFrameSceneNode keyFrameSceneNode in animationSceneNode.KeyFrames)
            {
              object obj = keyFrameSceneNode.Value;
              if (obj is Point)
              {
                Point point = (Point) obj;
                keyFrameSceneNode.Value = (object) (point - offset);
              }
            }
          }
        }
      }
    }

    private void UpdateCenterPoint(Point centerPoint)
    {
      CanonicalTransform canonicalTransform = new CanonicalTransform(this.pathElement.RenderTransform);
      Point elementCoordinates = this.pathElement.RenderTransformOriginInElementCoordinates;
      Rect computedBounds = this.pathElement.GetComputedBounds((Base2DElement) this.pathElement);
      Point newOrigin = new Point(computedBounds.Left + centerPoint.X * computedBounds.Width, computedBounds.Top + centerPoint.Y * computedBounds.Height);
      canonicalTransform.UpdateForNewOrigin(elementCoordinates, newOrigin);
      this.pathElement.RenderTransformOriginInElementCoordinates = newOrigin;
      this.pathElement.SetValue(this.pathElement.Platform.Metadata.CommonProperties.RenderTransformTranslationX, (object) canonicalTransform.TranslationX);
      this.pathElement.SetValue(this.pathElement.Platform.Metadata.CommonProperties.RenderTransformTranslationY, (object) canonicalTransform.TranslationY);
    }

    protected override void Dispose(bool fromDispose)
    {
      base.Dispose(fromDispose);
      if (this.basisSubscription == null)
        return;
      this.basisSubscription.PathNodeInserted -= new SceneNodeSubscription<PathElement, PathGeometry>.PathNodeInsertedListener(((PathEditorTarget) this).OnBasisSubscriptionPathNodeInserted);
      this.basisSubscription.PathNodeRemoved -= new SceneNodeSubscription<PathElement, PathGeometry>.PathNodeRemovedListener(((PathEditorTarget) this).OnBasisSubscriptionPathNodeInserted);
      this.basisSubscription.CurrentViewModel = (SceneViewModel) null;
      this.basisSubscription = (SceneNodeSubscription<PathElement, PathGeometry>) null;
    }
  }
}
