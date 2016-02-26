using Microsoft.Expression.DesignModel.Metadata;
using System;

namespace Microsoft.Expression.DesignModel.Core
{
	internal static class PathMetadata
	{
		public readonly static IPropertyId DataProperty;

		public readonly static IPropertyId GeometryTransformProperty;

		public readonly static IPropertyId FiguresProperty;

		public readonly static IPropertyId PathFigureCollectionCountProperty;

		public readonly static IPropertyId PathGeometryFillRuleProperty;

		public readonly static IPropertyId PathGeometryFiguresProperty;

		public readonly static IPropertyId PathFigureIsFilledProperty;

		public readonly static IPropertyId PathFigureStartPointProperty;

		public readonly static IPropertyId PathFigureIsClosedProperty;

		public readonly static IPropertyId PathFigureSegmentsProperty;

		public readonly static IPropertyId PathSegmentIsStrokedProperty;

		public readonly static IPropertyId PathSegmentIsSmoothJoinProperty;

		public readonly static IPropertyId ArcSegmentSizeProperty;

		public readonly static IPropertyId ArcSegmentRotationAngleProperty;

		public readonly static IPropertyId ArcSegmentIsLargeArcProperty;

		public readonly static IPropertyId ArcSegmentSweepDirectionProperty;

		public readonly static IPropertyId ArcSegmentPointProperty;

		public readonly static IPropertyId LineSegmentPointProperty;

		public readonly static IPropertyId QuadraticBezierSegmentPoint1Property;

		public readonly static IPropertyId QuadraticBezierSegmentPoint2Property;

		public readonly static IPropertyId BezierSegmentPoint1Property;

		public readonly static IPropertyId BezierSegmentPoint2Property;

		public readonly static IPropertyId BezierSegmentPoint3Property;

		public readonly static IPropertyId PolyLineSegmentPointsProperty;

		public readonly static IPropertyId PolyQuadraticBezierSegmentPointsProperty;

		public readonly static IPropertyId PolyBezierSegmentPointsProperty;

		public readonly static IPropertyId ClipProperty;

		static PathMetadata()
		{
			PathMetadata.DataProperty = (IPropertyId)PlatformTypes.Path.GetMember(MemberType.LocalProperty, "Data", MemberAccessTypes.Public);
			PathMetadata.GeometryTransformProperty = (IPropertyId)PlatformTypes.Geometry.GetMember(MemberType.LocalProperty, "Transform", MemberAccessTypes.Public);
			PathMetadata.FiguresProperty = (IPropertyId)PlatformTypes.PathGeometry.GetMember(MemberType.LocalProperty, "Figures", MemberAccessTypes.Public);
			PathMetadata.PathFigureCollectionCountProperty = (IPropertyId)PlatformTypes.PathFigureCollection.GetMember(MemberType.LocalProperty, "Count", MemberAccessTypes.Public);
			PathMetadata.PathGeometryFillRuleProperty = (IPropertyId)PlatformTypes.PathGeometry.GetMember(MemberType.LocalProperty, "FillRule", MemberAccessTypes.Public);
			PathMetadata.PathGeometryFiguresProperty = (IPropertyId)PlatformTypes.PathGeometry.GetMember(MemberType.LocalProperty, "Figures", MemberAccessTypes.Public);
			PathMetadata.PathFigureIsFilledProperty = (IPropertyId)PlatformTypes.PathFigure.GetMember(MemberType.LocalProperty, "IsFilled", MemberAccessTypes.Public);
			PathMetadata.PathFigureStartPointProperty = (IPropertyId)PlatformTypes.PathFigure.GetMember(MemberType.LocalProperty, "StartPoint", MemberAccessTypes.Public);
			PathMetadata.PathFigureIsClosedProperty = (IPropertyId)PlatformTypes.PathFigure.GetMember(MemberType.LocalProperty, "IsClosed", MemberAccessTypes.Public);
			PathMetadata.PathFigureSegmentsProperty = (IPropertyId)PlatformTypes.PathFigure.GetMember(MemberType.LocalProperty, "Segments", MemberAccessTypes.Public);
			PathMetadata.PathSegmentIsStrokedProperty = (IPropertyId)PlatformTypes.PathSegment.GetMember(MemberType.LocalProperty, "IsStroked", MemberAccessTypes.Public);
			PathMetadata.PathSegmentIsSmoothJoinProperty = (IPropertyId)PlatformTypes.PathSegment.GetMember(MemberType.LocalProperty, "IsSmoothJoin", MemberAccessTypes.Public);
			PathMetadata.ArcSegmentSizeProperty = (IPropertyId)PlatformTypes.ArcSegment.GetMember(MemberType.LocalProperty, "Size", MemberAccessTypes.Public);
			PathMetadata.ArcSegmentRotationAngleProperty = (IPropertyId)PlatformTypes.ArcSegment.GetMember(MemberType.LocalProperty, "RotationAngle", MemberAccessTypes.Public);
			PathMetadata.ArcSegmentIsLargeArcProperty = (IPropertyId)PlatformTypes.ArcSegment.GetMember(MemberType.LocalProperty, "IsLargeArc", MemberAccessTypes.Public);
			PathMetadata.ArcSegmentSweepDirectionProperty = (IPropertyId)PlatformTypes.ArcSegment.GetMember(MemberType.LocalProperty, "SweepDirection", MemberAccessTypes.Public);
			PathMetadata.ArcSegmentPointProperty = (IPropertyId)PlatformTypes.ArcSegment.GetMember(MemberType.LocalProperty, "Point", MemberAccessTypes.Public);
			PathMetadata.LineSegmentPointProperty = (IPropertyId)PlatformTypes.LineSegment.GetMember(MemberType.LocalProperty, "Point", MemberAccessTypes.Public);
			PathMetadata.QuadraticBezierSegmentPoint1Property = (IPropertyId)PlatformTypes.QuadraticBezierSegment.GetMember(MemberType.LocalProperty, "Point1", MemberAccessTypes.Public);
			PathMetadata.QuadraticBezierSegmentPoint2Property = (IPropertyId)PlatformTypes.QuadraticBezierSegment.GetMember(MemberType.LocalProperty, "Point2", MemberAccessTypes.Public);
			PathMetadata.BezierSegmentPoint1Property = (IPropertyId)PlatformTypes.BezierSegment.GetMember(MemberType.LocalProperty, "Point1", MemberAccessTypes.Public);
			PathMetadata.BezierSegmentPoint2Property = (IPropertyId)PlatformTypes.BezierSegment.GetMember(MemberType.LocalProperty, "Point2", MemberAccessTypes.Public);
			PathMetadata.BezierSegmentPoint3Property = (IPropertyId)PlatformTypes.BezierSegment.GetMember(MemberType.LocalProperty, "Point3", MemberAccessTypes.Public);
			PathMetadata.PolyLineSegmentPointsProperty = (IPropertyId)PlatformTypes.PolyLineSegment.GetMember(MemberType.LocalProperty, "Points", MemberAccessTypes.Public);
			PathMetadata.PolyQuadraticBezierSegmentPointsProperty = (IPropertyId)PlatformTypes.PolyQuadraticBezierSegment.GetMember(MemberType.LocalProperty, "Points", MemberAccessTypes.Public);
			PathMetadata.PolyBezierSegmentPointsProperty = (IPropertyId)PlatformTypes.PolyBezierSegment.GetMember(MemberType.LocalProperty, "Points", MemberAccessTypes.Public);
			PathMetadata.ClipProperty = (IPropertyId)PlatformTypes.UIElement.GetMember(MemberType.LocalProperty, "Clip", MemberAccessTypes.Public);
		}
	}
}