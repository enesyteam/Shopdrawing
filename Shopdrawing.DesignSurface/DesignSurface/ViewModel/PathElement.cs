// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.PathElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.SceneCommands;
using Microsoft.Expression.DesignSurface.Tools.Path;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class PathElement : ShapeElement
  {
    public static readonly IPropertyId DataProperty = (IPropertyId) PlatformTypes.Path.GetMember(MemberType.LocalProperty, "Data", MemberAccessTypes.Public);
    public static readonly IPropertyId FiguresProperty = (IPropertyId) PlatformTypes.PathGeometry.GetMember(MemberType.LocalProperty, "Figures", MemberAccessTypes.Public);
    public static readonly IPropertyId PathFigureCollectionCountProperty = (IPropertyId) PlatformTypes.PathFigureCollection.GetMember(MemberType.LocalProperty, "Count", MemberAccessTypes.Public);
    public static readonly IPropertyId PathGeometryFillRuleProperty = (IPropertyId) PlatformTypes.PathGeometry.GetMember(MemberType.LocalProperty, "FillRule", MemberAccessTypes.Public);
    public static readonly IPropertyId PathGeometryFiguresProperty = (IPropertyId) PlatformTypes.PathGeometry.GetMember(MemberType.LocalProperty, "Figures", MemberAccessTypes.Public);
    public static readonly IPropertyId PathFigureIsFilledProperty = (IPropertyId) PlatformTypes.PathFigure.GetMember(MemberType.LocalProperty, "IsFilled", MemberAccessTypes.Public);
    public static readonly IPropertyId PathFigureStartPointProperty = (IPropertyId) PlatformTypes.PathFigure.GetMember(MemberType.LocalProperty, "StartPoint", MemberAccessTypes.Public);
    public static readonly IPropertyId PathFigureIsClosedProperty = (IPropertyId) PlatformTypes.PathFigure.GetMember(MemberType.LocalProperty, "IsClosed", MemberAccessTypes.Public);
    public static readonly IPropertyId PathFigureSegmentsProperty = (IPropertyId) PlatformTypes.PathFigure.GetMember(MemberType.LocalProperty, "Segments", MemberAccessTypes.Public);
    public static readonly IPropertyId PathSegmentIsStrokedProperty = (IPropertyId) PlatformTypes.PathSegment.GetMember(MemberType.LocalProperty, "IsStroked", MemberAccessTypes.Public);
    public static readonly IPropertyId PathSegmentIsSmoothJoinProperty = (IPropertyId) PlatformTypes.PathSegment.GetMember(MemberType.LocalProperty, "IsSmoothJoin", MemberAccessTypes.Public);
    public static readonly IPropertyId ArcSegmentSizeProperty = (IPropertyId) PlatformTypes.ArcSegment.GetMember(MemberType.LocalProperty, "Size", MemberAccessTypes.Public);
    public static readonly IPropertyId ArcSegmentRotationAngleProperty = (IPropertyId) PlatformTypes.ArcSegment.GetMember(MemberType.LocalProperty, "RotationAngle", MemberAccessTypes.Public);
    public static readonly IPropertyId ArcSegmentIsLargeArcProperty = (IPropertyId) PlatformTypes.ArcSegment.GetMember(MemberType.LocalProperty, "IsLargeArc", MemberAccessTypes.Public);
    public static readonly IPropertyId ArcSegmentSweepDirectionProperty = (IPropertyId) PlatformTypes.ArcSegment.GetMember(MemberType.LocalProperty, "SweepDirection", MemberAccessTypes.Public);
    public static readonly IPropertyId ArcSegmentPointProperty = (IPropertyId) PlatformTypes.ArcSegment.GetMember(MemberType.LocalProperty, "Point", MemberAccessTypes.Public);
    public static readonly IPropertyId LineSegmentPointProperty = (IPropertyId) PlatformTypes.LineSegment.GetMember(MemberType.LocalProperty, "Point", MemberAccessTypes.Public);
    public static readonly IPropertyId QuadraticBezierSegmentPoint1Property = (IPropertyId) PlatformTypes.QuadraticBezierSegment.GetMember(MemberType.LocalProperty, "Point1", MemberAccessTypes.Public);
    public static readonly IPropertyId QuadraticBezierSegmentPoint2Property = (IPropertyId) PlatformTypes.QuadraticBezierSegment.GetMember(MemberType.LocalProperty, "Point2", MemberAccessTypes.Public);
    public static readonly IPropertyId BezierSegmentPoint1Property = (IPropertyId) PlatformTypes.BezierSegment.GetMember(MemberType.LocalProperty, "Point1", MemberAccessTypes.Public);
    public static readonly IPropertyId BezierSegmentPoint2Property = (IPropertyId) PlatformTypes.BezierSegment.GetMember(MemberType.LocalProperty, "Point2", MemberAccessTypes.Public);
    public static readonly IPropertyId BezierSegmentPoint3Property = (IPropertyId) PlatformTypes.BezierSegment.GetMember(MemberType.LocalProperty, "Point3", MemberAccessTypes.Public);
    public static readonly IPropertyId PolyLineSegmentPointsProperty = (IPropertyId) PlatformTypes.PolyLineSegment.GetMember(MemberType.LocalProperty, "Points", MemberAccessTypes.Public);
    public static readonly IPropertyId PolyQuadraticBezierSegmentPointsProperty = (IPropertyId) PlatformTypes.PolyQuadraticBezierSegment.GetMember(MemberType.LocalProperty, "Points", MemberAccessTypes.Public);
    public static readonly IPropertyId PolyBezierSegmentPointsProperty = (IPropertyId) PlatformTypes.PolyBezierSegment.GetMember(MemberType.LocalProperty, "Points", MemberAccessTypes.Public);
    public static readonly PathElement.ConcretePathElementFactory Factory = new PathElement.ConcretePathElementFactory();

    public PathGeometry PathGeometry
    {
      get
      {
        object defaultValueAsWpf = this.GetLocalOrDefaultValueAsWpf(PathElement.DataProperty);
        System.Windows.Media.Geometry geometry = defaultValueAsWpf as System.Windows.Media.Geometry;
        if (geometry == null && defaultValueAsWpf != null)
            geometry = (System.Windows.Media.Geometry)this.GetComputedValueAsWpf(PathElement.DataProperty);
        return PathGeometryUtilities.GetPathGeometryFromGeometry(geometry);
      }
      set
      {
        PathGeometry geometry = PathGeometryUtilities.RemoveMapping(value, true);
        if (!this.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf))
          PathGeometryUtilities.EnsureOnlySingleSegmentsInGeometry(geometry);
        this.SetValueAsWpf(PathElement.DataProperty, (object) geometry);
      }
    }

    public PathGeometry RenderedGeometry
    {
      get
      {
        return PathGeometryUtilities.GetPathGeometryFromGeometry(this.ViewModel.DefaultView.GetRenderedGeometryAsWpf((SceneElement) this));
      }
    }

    public PathGeometry TransformedGeometry
    {
      get
      {
          return PathGeometryUtilities.TransformGeometry((System.Windows.Media.Geometry)this.PathGeometry, this.GeometryTransform);
      }
    }

    private Point? StartPointOfOnePointPath
    {
      get
      {
        IViewShape viewShape = this.ViewObject as IViewShape;
        if (viewShape != null)
          return viewShape.StartPointOfOnePointPath;
        return new Point?();
      }
    }

    public Transform GeometryTransform
    {
      get
      {
        object computedValueAsWpf = this.GetComputedValueAsWpf(ShapeElement.GeometryTransformProperty);
        return !(computedValueAsWpf is Matrix) ? computedValueAsWpf as Transform ?? Transform.Identity : (Transform) new MatrixTransform((Matrix) computedValueAsWpf);
      }
    }

    public Transform RenderTransform
    {
      get
      {
        return (Transform) this.GetComputedValueAsWpf(Base2DElement.RenderTransformProperty);
      }
      set
      {
        this.SetValueAsWpf(Base2DElement.RenderTransformProperty, (object) value);
      }
    }

    public Path Path
    {
      get
      {
        return this.ConvertToWpfValue(this.ViewObject.PlatformSpecificObject) as Path;
      }
    }

    public bool HasVertexAnimations
    {
      get
      {
        foreach (StoryboardTimelineSceneNode timelineSceneNode1 in this.ViewModel.AnimationEditor.EnumerateStoryboardsForContainer(this.StoryboardContainer))
        {
          foreach (TimelineSceneNode timelineSceneNode2 in (IEnumerable<TimelineSceneNode>) timelineSceneNode1.Children)
          {
            if (timelineSceneNode2.TargetElement == this && timelineSceneNode2.TargetProperty != null && timelineSceneNode2.TargetProperty.FirstStep.Equals((object) PathElement.DataProperty))
              return true;
          }
        }
        return false;
      }
    }

    public static int GetPointIndexFromPointProperty(IPropertyId property)
    {
      if (PathElement.BezierSegmentPoint1Property.Equals((object) property))
        return 0;
      if (PathElement.BezierSegmentPoint2Property.Equals((object) property))
        return 1;
      if (PathElement.BezierSegmentPoint3Property.Equals((object) property))
        return 2;
      if (PathElement.LineSegmentPointProperty.Equals((object) property) || PathElement.QuadraticBezierSegmentPoint1Property.Equals((object) property))
        return 0;
      return PathElement.QuadraticBezierSegmentPoint2Property.Equals((object) property) ? 1 : -1;
    }

    public bool EnsureSingleSegmentsInPathGeometry()
    {
      PathGeometry pathGeometry = this.PathGeometry;
      bool flag = PathGeometryUtilities.EnsureOnlySingleSegmentsInGeometry(pathGeometry);
      this.PathGeometry = pathGeometry;
      return flag;
    }

    public override Rect GetComputedTightBounds(Base2DElement context)
    {
      PathGeometry renderedGeometry = this.RenderedGeometry;
      if (renderedGeometry == null)
        return Rect.Empty;
      Matrix transformToElement = this.GetComputedTransformToElement((SceneElement) context);
      return PathGeometryUtilities.TightExtent(renderedGeometry, transformToElement);
    }

    protected override object GetComputedValueAsWpfInternal(IPropertyId propertyKey)
    {
      object valueAsWpfInternal = base.GetComputedValueAsWpfInternal(propertyKey);
      if (!ShapeElement.GeometryTransformProperty.Equals((object) propertyKey) || !this.ProjectContext.IsCapabilitySet(PlatformCapability.WorkaroundSL14910))
        return valueAsWpfInternal;
      Point? pointOfOnePointPath = this.StartPointOfOnePointPath;
      if (pointOfOnePointPath.HasValue)
        return (object) new TranslateTransform(-pointOfOnePointPath.Value.X, -pointOfOnePointPath.Value.Y);
      return valueAsWpfInternal;
    }

    public static void EnsureStretchIsFill(SceneNode element)
    {
      if (element == null || !PlatformTypes.Path.Equals((object) element.Type) || element.IsSet(ShapeElement.StretchProperty) != PropertyState.Unset)
        return;
      using (element.ViewModel.ForceBaseValue())
        element.SetValueAsWpf(ShapeElement.StretchProperty, (object) Stretch.Fill);
    }

    public static PathElement.PathTransformHelper TryCreateTransformHelper(SceneElement element, Size startSize)
    {
      PathElement pathElement = element as PathElement;
      if (pathElement != null)
      {
        object localValueAsWpf = pathElement.GetLocalValueAsWpf(ShapeElement.StretchProperty);
        if (localValueAsWpf != null && (Stretch) localValueAsWpf == Stretch.None)
          return new PathElement.PathTransformHelper(pathElement, startSize);
      }
      return (PathElement.PathTransformHelper) null;
    }

    public override void SetValue(PropertyReference propertyReference, object valueToSet)
    {
      if (this.IsAttached && ShapeElement.StretchProperty.Equals((object) propertyReference.LastStep) && (Stretch.None.Equals(this.ConvertToWpfValue(valueToSet)) && !Stretch.None.Equals((object) (Stretch) this.GetComputedValueAsWpf(ShapeElement.StretchProperty))))
        this.NormalizePathGeometry();
      base.SetValue(propertyReference, valueToSet);
    }

    private void NormalizePathGeometry()
    {
      PathGeometry pathGeometry = this.PathGeometry;
      Rect extent = PathCommandHelper.InflateRectByStrokeWidth(PathGeometryUtilities.TightExtent(pathGeometry, Matrix.Identity), this, false);
      Rect computedBounds = this.GetComputedBounds((Base2DElement) this);
      Transform transform = ScenePathEditorTarget.NormalizePathGeometry(pathGeometry, extent, computedBounds);
      this.PathGeometry = pathGeometry;
      PathCommandHelper.TransformPointKeyframes((SceneElement) this, new PropertyReference((ReferenceStep) this.ProjectContext.ResolveProperty(PathElement.DataProperty)), transform);
    }

    public class ConcretePathElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new PathElement();
      }
    }

    public class PathTransformHelper
    {
      private PathElement pathElement;
      private PathGeometry initialGeometry;
      private Transform transform;
      private Size initialSize;
      private double halfStrokeThickness;

      public PathTransformHelper(PathElement pathElement, Size initialSize)
      {
        this.pathElement = pathElement;
        this.initialGeometry = new PathGeometry();
        this.initialGeometry.AddGeometry((System.Windows.Media.Geometry)pathElement.PathGeometry);
        this.initialSize = initialSize;
        this.halfStrokeThickness = PathCommandHelper.GetHalfStrokeWidth(pathElement);
      }

      public void Update(double scaleX, double scaleY)
      {
        double num = 2.0 * this.halfStrokeThickness;
        if (this.initialSize.Width > num)
          scaleX = (this.initialSize.Width * scaleX - num) / (this.initialSize.Width - num);
        if (this.initialSize.Height > num)
          scaleY = (this.initialSize.Height * scaleY - num) / (this.initialSize.Height - num);
        this.transform = (Transform) new ScaleTransform(scaleX, scaleY, this.halfStrokeThickness, this.halfStrokeThickness);
        this.pathElement.PathGeometry = PathGeometryUtilities.TransformGeometry((System.Windows.Media.Geometry)this.initialGeometry, this.transform);
      }

      public void OnDragEnd()
      {
        if (this.transform == null)
          return;
        PathCommandHelper.TransformPointKeyframes((SceneElement) this.pathElement, new PropertyReference((ReferenceStep) this.pathElement.ProjectContext.ResolveProperty(PathElement.DataProperty)), this.transform);
      }
    }
  }
}
