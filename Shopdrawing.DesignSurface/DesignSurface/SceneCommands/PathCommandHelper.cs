// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.PathCommandHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Designers;
using Microsoft.Expression.DesignSurface.Geometry;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  public static class PathCommandHelper
  {
    public static Transform ReplacePathGeometry(PathElement pathElement, PathGeometry pathGeometry, SceneEditTransaction editTransaction)
    {
      Transform transform1 = Transform.Identity;
      using (pathElement.ViewModel.ForceBaseValue())
      {
        if (pathGeometry == null)
        {
          pathElement.ClearLocalValue(PathElement.DataProperty);
        }
        else
        {
          editTransaction.Update();
          ILayoutDesigner designerForChild = pathElement.ViewModel.GetLayoutDesignerForChild((SceneElement) pathElement, true);
          Rect childRect = designerForChild.GetChildRect((BaseFrameworkElement) pathElement);
          Path path = pathElement.Path;
          Transform transform2 = path == null ? (Transform) null : path.RenderTransform;
          Point point1 = path == null ? new Point(0.0, 0.0) : path.RenderTransformOrigin;
          double num = path == null || path.Stroke == null ? 0.0 : path.StrokeThickness;
          Rect bounds = pathGeometry.Bounds;
          PathGeometry pathGeometry1 = new PathGeometry();
          pathGeometry1.AddGeometry((System.Windows.Media.Geometry)pathGeometry);
          Rect rect1 = PathCommandHelper.InflateRectByStrokeWidth(pathGeometry1.Bounds, pathElement, false);
          Vector vector1 = new Vector(-rect1.Left, -rect1.Top);
          transform1 = (Transform) new TranslateTransform(vector1.X, vector1.Y);
          PathGeometry pathGeometry2 = PathGeometryUtilities.TransformGeometry((System.Windows.Media.Geometry)pathGeometry1, transform1);
          pathElement.PathGeometry = pathGeometry2;
          PropertyReference propertyReference = PathCommandHelper.GetPathDataPropertyReference(pathElement.Platform);
          PathCommandHelper.TransformPointKeyframes((SceneElement) pathElement, propertyReference, transform1);
          if (!bounds.IsEmpty)
          {
            bounds.Inflate(num / 2.0, num / 2.0);
            Rect rect2 = new Rect(childRect.Left + bounds.Left, childRect.Top + bounds.Top, bounds.Width, bounds.Height);
            editTransaction.Update();
            designerForChild.SetChildRect((BaseFrameworkElement) pathElement, rect2);
            if (pathElement.IsSet(Base2DElement.RenderTransformOriginProperty) != PropertyState.Set)
            {
              if (transform2 != null)
              {
                if (transform2.Value.IsIdentity)
                  goto label_15;
              }
              else
                goto label_15;
            }
            Vector vector2 = childRect.TopLeft + new Vector(point1.X * childRect.Width, point1.Y * childRect.Height) - rect2.TopLeft;
            Point point2 = new Point(0.0, 0.0);
            if (rect2.Width != 0.0)
              point2.X = vector2.X / rect2.Width;
            if (rect2.Height != 0.0)
              point2.Y = vector2.Y / rect2.Height;
            pathElement.RenderTransformOrigin = point2;
          }
        }
      }
label_15:
      return transform1;
    }

    public static List<PathElement> ReleaseCompoundPaths(PathElement pathElement, SceneEditTransaction editTransaction)
    {
      List<PathElement> list = new List<PathElement>();
      PathGeometry pathGeometry1 = pathElement.PathGeometry;
      Transform geometryTransform = pathElement.GeometryTransform;
      ISceneNodeCollection<SceneNode> collectionContainer = pathElement.GetCollectionContainer();
      int num = collectionContainer.IndexOf((SceneNode) pathElement);
      int oldFigureIndex = 0;
      foreach (PathFigure original in pathGeometry1.Figures)
      {
        PathFigure pathFigure = PathFigureUtilities.Copy(original, geometryTransform);
        PathGeometry pathGeometry2 = new PathGeometry();
        if (pathGeometry1.Transform != null && pathGeometry1.Transform.Value != Matrix.Identity)
          pathGeometry2.Transform = pathGeometry1.Transform.Clone();
        if (pathGeometry2.FillRule != pathGeometry1.FillRule)
          pathGeometry2.FillRule = pathGeometry1.FillRule;
        pathGeometry2.Figures.Add(pathFigure);
        DocumentNode node = pathElement.DocumentNode.Clone(pathElement.DocumentContext);
        PathElement pathElement1 = (PathElement) pathElement.ViewModel.GetSceneNode(node);
        collectionContainer.Insert(num++, (SceneNode) pathElement1);
        Rect extent = pathGeometry2.Bounds;
        extent = PathCommandHelper.InflateRectByStrokeWidth(extent, pathElement1, false);
        Vector vector = new Vector(-extent.Left, -extent.Top);
        PathCommandHelper.ReplacePathGeometry(pathElement1, pathGeometry2, editTransaction);
        Matrix transformToElement = pathElement.GetComputedTransformToElement((SceneElement) pathElement1);
        transformToElement.OffsetX += vector.X;
        transformToElement.OffsetY += vector.Y;
        PropertyReference propertyReference = PathCommandHelper.GetPathDataPropertyReference(pathElement.Platform);
        PathCommandHelper.TransferPathFigureAnimations(pathElement, pathElement1, propertyReference, oldFigureIndex, 0, (Transform) new MatrixTransform(transformToElement));
        PathCommandHelper.AdjustPathForAnimations(pathElement1, editTransaction);
        list.Add(pathElement1);
        ++oldFigureIndex;
      }
      string name = pathElement.Name;
      pathElement.ViewModel.ElementSelectionSet.RemoveSelection((SceneElement) pathElement);
      pathElement.ViewModel.AnimationEditor.DeleteAllAnimationsInSubtree((SceneElement) pathElement);
      pathElement.Remove();
      foreach (SceneNode sceneNode in list)
        sceneNode.Name = name;
      return list;
    }

    public static void MakeCompoundPath(PathElement mainElement, List<PathElement> otherElements, SceneEditTransaction editTransaction)
    {
      PathElement pathElement = otherElements[otherElements.Count - 1];
      ISceneElementCollection elementCollection = (ISceneElementCollection) new SceneElementCollection();
      PathAnimationMovePackage animationMove = new PathAnimationMovePackage();
      PathGeometry pathGeometry = new PathGeometry();
      System.Windows.Media.Geometry geometry1 = (System.Windows.Media.Geometry)mainElement.TransformedGeometry;
      pathGeometry.AddGeometry(geometry1);
      foreach (PathElement oldElement in otherElements)
      {
          System.Windows.Media.Geometry geometry2 = (System.Windows.Media.Geometry)oldElement.TransformedGeometry;
        Matrix transformToElement = oldElement.GetComputedTransformToElement((SceneElement) mainElement);
        bool isIdentity = transformToElement.IsIdentity;
        Transform transform = isIdentity ? Transform.Identity : (Transform) new MatrixTransform(transformToElement);
        transform.Freeze();
        if (!isIdentity)
        {
          GeometryGroup geometryGroup = new GeometryGroup();
          geometryGroup.Children.Add(geometry2);
          geometryGroup.Transform = transform;
          geometry2 = (System.Windows.Media.Geometry)geometryGroup;
        }
        int count = pathGeometry.Figures.Count;
        pathGeometry.AddGeometry(geometry2);
        int num = pathGeometry.Figures.Count - count;
        PropertyReference propertyReference = PathCommandHelper.GetPathDataPropertyReference(oldElement.Platform);
        for (int oldFigureIndex = 0; oldFigureIndex < num; ++oldFigureIndex)
          PathCommandHelper.TransferPathFigureAnimations(oldElement, mainElement, propertyReference, oldFigureIndex, count + oldFigureIndex, transform, animationMove);
        elementCollection.Add((SceneElement) oldElement);
      }
      animationMove.Remove();
      Transform transform1 = PathCommandHelper.ReplacePathGeometry(mainElement, pathGeometry, editTransaction);
      animationMove.ApplyTransformToNewAnimations(transform1);
      animationMove.Add();
      PathCommandHelper.AdjustPathForAnimations(mainElement, editTransaction);
      foreach (SceneElement element in (IEnumerable<SceneElement>) elementCollection)
      {
        element.ViewModel.AnimationEditor.DeleteAllAnimationsInSubtree(element);
        element.ViewModel.RemoveElement((SceneNode) element);
      }
    }

    public static void TransformPointKeyframes(SceneElement element, PropertyReference animationPrefix, Transform transform)
    {
      foreach (StoryboardTimelineSceneNode timelineSceneNode1 in element.ViewModel.AnimationEditor.EnumerateStoryboardsForContainer(element.StoryboardContainer))
      {
        foreach (TimelineSceneNode timelineSceneNode2 in (IEnumerable<TimelineSceneNode>) timelineSceneNode1.Children)
        {
          if (timelineSceneNode2.TargetElement == element && timelineSceneNode2.TargetProperty != null && animationPrefix.IsPrefixOf(timelineSceneNode2.TargetProperty))
          {
            KeyFrameAnimationSceneNode animationSceneNode = timelineSceneNode2 as KeyFrameAnimationSceneNode;
            if (animationSceneNode != null)
            {
              foreach (KeyFrameSceneNode keyFrameSceneNode in animationSceneNode.KeyFrames)
              {
                if (keyFrameSceneNode.Value is Point)
                {
                  Point point = (Point) keyFrameSceneNode.Value;
                  keyFrameSceneNode.Value = (object) transform.Transform(point);
                }
              }
            }
          }
        }
      }
    }

    public static Rect FindMaxAnimatedExtent(SceneElement element, Rect bounds, PropertyReference animationPrefix)
    {
      foreach (StoryboardTimelineSceneNode timelineSceneNode1 in element.ViewModel.AnimationEditor.EnumerateStoryboardsForContainer(element.StoryboardContainer))
      {
        foreach (TimelineSceneNode timelineSceneNode2 in (IEnumerable<TimelineSceneNode>) timelineSceneNode1.Children)
        {
          if (timelineSceneNode2.TargetElement == element && timelineSceneNode2.TargetProperty != null && animationPrefix.IsPrefixOf(timelineSceneNode2.TargetProperty))
          {
            KeyFrameAnimationSceneNode animationSceneNode = timelineSceneNode2 as KeyFrameAnimationSceneNode;
            if (animationSceneNode != null)
            {
              foreach (KeyFrameSceneNode keyFrameSceneNode in animationSceneNode.KeyFrames)
              {
                if (keyFrameSceneNode.Value is Point)
                {
                  Point point = (Point) keyFrameSceneNode.Value;
                  bounds.Union(point);
                }
              }
            }
          }
        }
      }
      return bounds;
    }

    public static void AdjustPathForAnimations(PathElement pathElement, SceneEditTransaction editTransaction)
    {
      using (pathElement.ViewModel.ForceBaseValue())
      {
          System.Windows.Media.Geometry geometry1 = (System.Windows.Media.Geometry)pathElement.GetLocalOrDefaultValueAsWpf(PathElement.DataProperty);
        PathGeometry pathGeometry = new PathGeometry();
        pathGeometry.AddGeometry(geometry1);
        Rect bounds = PathCommandHelper.InflateRectByStrokeWidth(pathGeometry.Bounds, pathElement, false);
        PropertyReference propertyReference = PathCommandHelper.GetPathDataPropertyReference(pathElement.Platform);
        Rect maxAnimatedExtent = PathCommandHelper.FindMaxAnimatedExtent((SceneElement) pathElement, bounds, propertyReference);
        double num1 = Math.Min(0.0, maxAnimatedExtent.Left);
        double num2 = Math.Min(0.0, maxAnimatedExtent.Top);
        Vector vector = new Vector(-num1, -num2);
        ILayoutDesigner designerForChild = pathElement.ViewModel.GetLayoutDesignerForChild((SceneElement) pathElement, true);
        editTransaction.Update();
        Rect childRect = designerForChild.GetChildRect((BaseFrameworkElement) pathElement);
        childRect.X += num1;
        childRect.Y += num2;
        childRect.Width = Math.Max(maxAnimatedExtent.Width, childRect.Width);
        childRect.Height = Math.Max(maxAnimatedExtent.Height, childRect.Height);
        designerForChild.SetChildRect((BaseFrameworkElement) pathElement, childRect);
        Transform transform = (Transform) new TranslateTransform(vector.X, vector.Y);
        PathGeometry geometry2 = PathGeometryUtilities.TransformGeometry((System.Windows.Media.Geometry)pathGeometry, transform);
        if (!pathElement.DesignerContext.ActiveDocument.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf))
          PathGeometryUtilities.EnsureOnlySingleSegmentsInGeometry(geometry2);
        pathElement.PathGeometry = geometry2;
        PathCommandHelper.TransformPointKeyframes((SceneElement) pathElement, propertyReference, transform);
        pathElement.SetValueAsWpf(ShapeElement.StretchProperty, (object) (Stretch) (pathElement.HasVertexAnimations ? 0 : 1));
      }
    }

    public static Rect InflateRectByStrokeWidth(Rect extent, PathElement pathElement, bool justCreated)
    {
      double halfStrokeWidth = PathCommandHelper.GetHalfStrokeWidth(pathElement);
      if (halfStrokeWidth != 0.0 && !extent.IsEmpty)
      {
        if (justCreated)
        {
          double num1 = Math.Truncate(halfStrokeWidth);
          double num2 = halfStrokeWidth - num1;
          extent.Inflate(num1, num1);
          extent.Width += num2 * 2.0;
          extent.Height += num2 * 2.0;
        }
        else
          extent.Inflate(halfStrokeWidth, halfStrokeWidth);
      }
      return extent;
    }

    public static double GetHalfStrokeWidth(PathElement pathElement)
    {
      if ((Brush) pathElement.GetComputedValueAsWpf(ShapeElement.StrokeProperty) != null)
      {
        double d = (double) pathElement.GetComputedValue(ShapeElement.StrokeThicknessProperty);
        if (!double.IsNaN(d) && !double.IsInfinity(d))
          return Math.Abs(d) / 2.0;
      }
      return 0.0;
    }

    private static PropertyReference GetPathDataPropertyReference(IPlatform platform)
    {
      return new PropertyReference(platform.Metadata.ResolveProperty(PathElement.DataProperty) as ReferenceStep);
    }

    private static void TransferPathFigureAnimations(PathElement oldElement, PathElement newElement, PropertyReference pathProperty, int oldFigureIndex, int newFigureIndex, Transform transform)
    {
      PathAnimationMovePackage animationMove = new PathAnimationMovePackage();
      PathCommandHelper.TransferPathFigureAnimations(oldElement, newElement, pathProperty, oldFigureIndex, newFigureIndex, transform, animationMove);
      animationMove.AddAndRemove();
    }

    private static void TransferPathFigureAnimations(PathElement oldElement, PathElement newElement, PropertyReference pathProperty, int oldFigureIndex, int newFigureIndex, Transform transform, PathAnimationMovePackage animationMove)
    {
      IProjectContext projectContext = newElement.ProjectContext;
      IType type = projectContext.ResolveType(PlatformTypes.PathFigureCollection);
      PropertyReference propertyReference = pathProperty.Append(PathElement.FiguresProperty);
      ReferenceStep step1 = (ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep((ITypeResolver) projectContext, type.RuntimeType, oldFigureIndex);
      PropertyReference sourceReferencePrefix = propertyReference.Append(step1);
      ReferenceStep step2 = (ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep((ITypeResolver) projectContext, type.RuntimeType, newFigureIndex);
      PropertyReference destinationReferencePrefix = propertyReference.Append(step2);
      PathCommandHelper.MoveVertexAnimations((SceneElement) oldElement, sourceReferencePrefix, (SceneElement) newElement, destinationReferencePrefix, transform, animationMove);
    }

    public static void MoveVertexAnimations(SceneElement source, PropertyReference sourceReferencePrefix, SceneElement destination, PropertyReference destinationReferencePrefix, Transform transform)
    {
      PathAnimationMovePackage animationMove = new PathAnimationMovePackage();
      PathCommandHelper.MoveVertexAnimations(source, sourceReferencePrefix, destination, destinationReferencePrefix, transform, animationMove);
      animationMove.AddAndRemove();
    }

    public static void MoveVertexAnimations(SceneElement source, PropertyReference sourceReferencePrefix, SceneElement destination, PropertyReference destinationReferencePrefix, Transform transform, PathAnimationMovePackage animationMove)
    {
      foreach (StoryboardTimelineSceneNode parent in source.ViewModel.AnimationEditor.EnumerateStoryboardsForContainer(source.StoryboardContainer))
      {
        foreach (TimelineSceneNode timeline1 in (IEnumerable<TimelineSceneNode>) parent.Children)
        {
          if (timeline1.TargetElement == source && timeline1.TargetProperty != null && sourceReferencePrefix.IsPrefixOf(timeline1.TargetProperty))
          {
            PropertyReference propertyReference = destinationReferencePrefix;
            for (int count = sourceReferencePrefix.Count; count < timeline1.TargetProperty.Count; ++count)
              propertyReference = propertyReference.Append(timeline1.TargetProperty[count]);
            TimelineSceneNode timeline2 = (TimelineSceneNode) source.ViewModel.GetSceneNode(timeline1.DocumentNode.Clone(timeline1.DocumentContext));
            if (transform != null)
              PathCommandHelper.ApplyTransformToAnimation(timeline2, transform);
            timeline2.TargetProperty = propertyReference;
            timeline2.TargetElement = (SceneNode) destination;
            animationMove.Add(parent, timeline2);
            animationMove.Remove(parent, timeline1);
          }
        }
      }
    }

    public static void ApplyTransformToAnimation(TimelineSceneNode timeline, Transform transform)
    {
      KeyFrameAnimationSceneNode animationSceneNode = timeline as KeyFrameAnimationSceneNode;
      if (animationSceneNode == null)
        return;
      foreach (KeyFrameSceneNode keyFrameSceneNode in animationSceneNode.KeyFrames)
      {
        object obj = keyFrameSceneNode.Value;
        if (obj != null && PlatformTypes.Point.IsAssignableFrom((ITypeId) timeline.Platform.Metadata.GetType(obj.GetType())))
        {
          Point point1 = (Point) timeline.ViewModel.DefaultView.ConvertToWpfValue(obj);
          Point point2 = transform.Transform(point1);
          keyFrameSceneNode.Value = timeline.ViewModel.DefaultView.ConvertFromWpfValue((object) point2);
        }
      }
    }

    public static Point Round(Point point, int decimalNumber)
    {
      return new Point(PathCommandHelper.Round(point.X, decimalNumber), PathCommandHelper.Round(point.Y, decimalNumber));
    }

    public static double Round(double value, int decimalNumber)
    {
      return Math.Round(value, decimalNumber);
    }

    public static void GrokPathPointPrecision(PathGeometry geometry, int decimalNumber)
    {
      if (geometry == null)
        return;
      for (int index1 = 0; index1 < geometry.Figures.Count; ++index1)
      {
        PathFigure pathFigure = geometry.Figures[index1];
        pathFigure.StartPoint = PathCommandHelper.Round(pathFigure.StartPoint, decimalNumber);
        for (int index2 = 0; index2 < pathFigure.Segments.Count; ++index2)
        {
          PathSegment pathSegment = pathFigure.Segments[index2];
          if (pathSegment is BezierSegment)
          {
            BezierSegment bezierSegment = pathSegment as BezierSegment;
            bezierSegment.Point1 = PathCommandHelper.Round(bezierSegment.Point1, decimalNumber);
            bezierSegment.Point2 = PathCommandHelper.Round(bezierSegment.Point2, decimalNumber);
            bezierSegment.Point3 = PathCommandHelper.Round(bezierSegment.Point3, decimalNumber);
          }
          else if (pathSegment is LineSegment)
          {
            LineSegment lineSegment = pathSegment as LineSegment;
            lineSegment.Point = PathCommandHelper.Round(lineSegment.Point, decimalNumber);
          }
          else if (pathSegment is QuadraticBezierSegment)
          {
            QuadraticBezierSegment quadraticBezierSegment = pathSegment as QuadraticBezierSegment;
            quadraticBezierSegment.Point1 = PathCommandHelper.Round(quadraticBezierSegment.Point1, decimalNumber);
            quadraticBezierSegment.Point2 = PathCommandHelper.Round(quadraticBezierSegment.Point2, decimalNumber);
          }
          else if (pathSegment is PolyLineSegment)
          {
            PointCollection points = (pathSegment as PolyLineSegment).Points;
            for (int index3 = 0; index3 < points.Count; ++index3)
              points[index3] = PathCommandHelper.Round(points[index3], decimalNumber);
          }
          else if (pathSegment is PolyQuadraticBezierSegment)
          {
            PointCollection points = (pathSegment as PolyQuadraticBezierSegment).Points;
            int index3 = 0;
            while (index2 < points.Count)
            {
              points[index3] = PathCommandHelper.Round(points[index3], decimalNumber);
              ++index3;
            }
          }
          else if (pathSegment is PolyBezierSegment)
          {
            PointCollection points = (pathSegment as PolyBezierSegment).Points;
            for (int index3 = 0; index3 < points.Count; ++index3)
              points[index3] = PathCommandHelper.Round(points[index3], decimalNumber);
          }
          else if (pathSegment is ArcSegment)
          {
            ArcSegment arcSegment = pathSegment as ArcSegment;
            arcSegment.Point = PathCommandHelper.Round(arcSegment.Point, decimalNumber);
          }
        }
      }
    }

    private static Brush GetTextForeground(BaseFrameworkElement element)
    {
      ITypeId type = (ITypeId) element.ViewObject.GetIType((ITypeResolver) element.ProjectContext);
      Brush brush;
      if (PlatformTypes.RichTextBox.IsAssignableFrom(type))
        brush = (Brush) element.GetComputedValueAsWpf(ControlElement.ForegroundProperty);
      else if (PlatformTypes.TextBox.IsAssignableFrom(type))
      {
        brush = (Brush) element.GetComputedValueAsWpf(ControlElement.ForegroundProperty);
      }
      else
      {
        if (!PlatformTypes.TextBlock.IsAssignableFrom(type))
          return (Brush) null;
        brush = (Brush) element.GetComputedValueAsWpf(TextBlockElement.ForegroundProperty);
      }
      if (brush == null)
        brush = (Brush) Brushes.Black;
      return brush;
    }

    public static PathElement ConvertToPath(BaseFrameworkElement element)
    {
      SceneViewModel viewModel = element.ViewModel;
      PathGeometry pathGeometry = PathConversionHelper.ConvertToPathGeometry((SceneElement) element);
      PathElement pathElement = (PathElement) viewModel.CreateSceneNode(PlatformTypes.Path);
      ILayoutDesigner designerForChild = viewModel.GetLayoutDesignerForChild((SceneElement) element, true);
      Rect childRect = designerForChild.GetChildRect(element);
      Transform transform1 = Transform.Identity;
      if (element.IsSet(Base2DElement.RenderTransformProperty) == PropertyState.Set)
        transform1 = (Transform) element.GetComputedValueAsWpf(Base2DElement.RenderTransformProperty);
      Point point1 = new Point(0.5, 0.5);
      if (element.IsSet(Base2DElement.RenderTransformOriginProperty) == PropertyState.Set)
        point1 = (Point) element.GetComputedValueAsWpf(Base2DElement.RenderTransformOriginProperty);
      bool flag = false;
      using (SceneEditTransaction editTransaction = viewModel.CreateEditTransaction(StringTable.UndoUnitConvertToPath, true))
      {
        using (viewModel.ForceBaseValue())
        {
          pathElement.PathGeometry = pathGeometry;
          pathElement.SetValueAsWpf(ShapeElement.StretchProperty, (object) Stretch.Fill);
          Brush textForeground = PathCommandHelper.GetTextForeground(element);
          if (textForeground != null)
          {
            flag = true;
            pathElement.SetValueAsWpf(ShapeElement.FillProperty, (object) textForeground);
          }
          viewModel.AnimationEditor.DeleteAllAnimations((SceneNode) element);
          Dictionary<IPropertyId, SceneNode> properties = SceneElementHelper.StoreProperties((SceneNode) element);
          ISceneNodeCollection<SceneNode> collectionForChild = element.Parent.GetCollectionForChild((SceneNode) element);
          int index = collectionForChild.IndexOf((SceneNode) element);
          collectionForChild[index] = (SceneNode) pathElement;
          if (flag)
          {
            List<IPropertyId> list = new List<IPropertyId>();
            foreach (KeyValuePair<IPropertyId, SceneNode> keyValuePair in properties)
            {
              IPropertyId key = keyValuePair.Key;
              DependencyPropertyReferenceStep propertyReferenceStep = key as DependencyPropertyReferenceStep;
              if (propertyReferenceStep != null && !propertyReferenceStep.IsAttachable && (propertyReferenceStep.MemberType != MemberType.DesignTimeProperty && !PlatformTypeHelper.GetDeclaringType((IMember) propertyReferenceStep).IsAssignableFrom(typeof (Path))))
                list.Add(key);
            }
            foreach (IPropertyId key in list)
              properties.Remove(key);
          }
          SceneElementHelper.ApplyProperties((SceneNode) pathElement, properties);
        }
        if (ProjectNeutralTypes.PrimitiveShape.IsAssignableFrom((ITypeId) element.Type))
        {
          using (viewModel.ForceBaseValue())
          {
            Rect bounds = pathGeometry.Bounds;
            double halfStrokeWidth = PathCommandHelper.GetHalfStrokeWidth(pathElement);
            bounds.Inflate(halfStrokeWidth / 2.0, halfStrokeWidth / 2.0);
            if (transform1 != Transform.Identity && bounds.Size != childRect.Size)
            {
              Point point2 = (Point) pathElement.GetComputedValueAsWpf(Base2DElement.RenderTransformOriginProperty);
              Point point3 = new Point(point2.X * childRect.Width, point2.Y * childRect.Height);
              Point point4 = new Point((point3.X - bounds.X) / bounds.Width, (point3.Y - bounds.Y) / bounds.Height);
              pathElement.SetValueAsWpf(Base2DElement.RenderTransformOriginProperty, (object) point4);
            }
            bounds.Offset(childRect.Left, childRect.Top);
            editTransaction.Update();
            designerForChild.SetChildRect((BaseFrameworkElement) pathElement, bounds);
          }
        }
        if (flag && !pathGeometry.IsEmpty())
        {
          using (viewModel.ForceBaseValue())
          {
            Rect bounds = pathGeometry.Bounds;
            bounds.Offset(childRect.Left, childRect.Top);
            Point point2 = new Point(bounds.Left + point1.X * bounds.Width, bounds.Top + point1.Y * bounds.Height);
            Point point3 = new Point(childRect.Left + childRect.Width * point1.X, childRect.Top + childRect.Height * point1.Y);
            Point point4 = new TransformGroup()
            {
              Children = {
                (Transform) new TranslateTransform(-point3.X, -point3.Y),
                transform1,
                (Transform) new TranslateTransform(point3.X, point3.Y)
              }
            }.Transform(point2);
            Transform transform2 = (Transform) new CanonicalTransform(transform1)
            {
              TranslationX = 0.0,
              TranslationY = 0.0
            }.TransformGroup;
            Rect rect = new Rect(point4.X - bounds.Width * point1.X, point4.Y - bounds.Height * point1.Y, bounds.Width, bounds.Height);
            pathElement.SetValueAsWpf(Base2DElement.RenderTransformOriginProperty, (object) point1);
            editTransaction.Update();
            designerForChild.SetChildRect((BaseFrameworkElement) pathElement, rect);
            pathElement.RenderTransform = transform2;
          }
        }
        editTransaction.Commit();
      }
      return pathElement;
    }

    public static void ConvertSelectionToPath(SceneElementSelectionSet sceneSelectionSet)
    {
      PathCommandHelper.ConvertSelectionToPathIfNeeded(sceneSelectionSet, (SceneElementFilter) (element => true));
    }

    public static void ConvertSelectionToPathIfNeeded(SceneElementSelectionSet sceneSelectionSet, SceneElementFilter isConversionNeeded)
    {
      ReadOnlyCollection<SceneElement> selection = sceneSelectionSet.Selection;
      SceneElement primarySelection1 = sceneSelectionSet.PrimarySelection;
      SceneElement primarySelection2 = (SceneElement) null;
      sceneSelectionSet.Clear();
      List<SceneElement> list = new List<SceneElement>(selection.Count);
      foreach (SceneElement element in selection)
      {
        SceneElement sceneElement = element;
        if (isConversionNeeded(element))
          sceneElement = (SceneElement) PathCommandHelper.ConvertToPath((BaseFrameworkElement) element);
        if (element == primarySelection1)
          primarySelection2 = sceneElement;
        list.Add(sceneElement);
      }
      sceneSelectionSet.SetSelection((ICollection<SceneElement>) list, primarySelection2);
    }
  }
}
