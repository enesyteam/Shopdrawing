using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignModel.Core
{
	internal static class PathGeometrySerializationHelper
	{
		public static bool CanSerializeAsAttribute(DocumentCompositeNode compositeNode)
		{
			if (PathGeometrySerializationHelper.SerializeAsAttribute(compositeNode) == null)
			{
				return false;
			}
			return !PathGeometrySerializationHelper.HasAnimations(compositeNode);
		}

		private static bool DoesAnimationTargetPath(DocumentNode pathNameNode, DocumentNode animationNode)
		{
			DocumentCompositeNode documentCompositeNode = animationNode as DocumentCompositeNode;
			if (documentCompositeNode == null)
			{
				return false;
			}
			DocumentNode item = documentCompositeNode.Properties[KnownProperties.StoryboardTargetNameProperty];
			string valueAsString = DocumentPrimitiveNode.GetValueAsString(pathNameNode);
			string str = DocumentPrimitiveNode.GetValueAsString(item);
			if (string.IsNullOrEmpty(valueAsString))
			{
				return false;
			}
			return valueAsString == str;
		}

		private static IEnumerable<DocumentNode> FindPointAnimationDescendantNodes(this DocumentNode root)
		{
			return root.SelectDescendantNodes((DocumentNode node) => {
				if (PlatformTypes.PointAnimationUsingKeyFrames.IsAssignableFrom(node.Type))
				{
					return true;
				}
				return PlatformTypes.PointAnimation.IsAssignableFrom(node.Type);
			});
		}

		private static bool GetArcSegmentAsString(DocumentCompositeNode arcSegmentNode, StringBuilder stringBuilder)
		{
			Size size = new Size();
			double num = 0;
			bool flag = false;
			SweepDirection sweepDirection = SweepDirection.Counterclockwise;
			Point point = new Point();
			if (PathGeometrySerializationHelper.GetPrimitiveValue<Size>(arcSegmentNode, PathMetadata.ArcSegmentSizeProperty, ref size) == PathGeometrySerializationHelper.PropertyValueKind.Composite || PathGeometrySerializationHelper.GetPrimitiveValue<double>(arcSegmentNode, PathMetadata.ArcSegmentRotationAngleProperty, ref num) == PathGeometrySerializationHelper.PropertyValueKind.Composite || PathGeometrySerializationHelper.GetPrimitiveValue<bool>(arcSegmentNode, PathMetadata.ArcSegmentIsLargeArcProperty, ref flag) == PathGeometrySerializationHelper.PropertyValueKind.Composite || PathGeometrySerializationHelper.GetPrimitiveValue<SweepDirection>(arcSegmentNode, PathMetadata.ArcSegmentSweepDirectionProperty, ref sweepDirection) == PathGeometrySerializationHelper.PropertyValueKind.Composite || PathGeometrySerializationHelper.GetPrimitiveValue<Point>(arcSegmentNode, PathMetadata.ArcSegmentPointProperty, ref point) == PathGeometrySerializationHelper.PropertyValueKind.Composite)
			{
				return false;
			}
			StringBuilder stringBuilder1 = stringBuilder;
			object[] width = new object[] { size.Width, size.Height, num, null, null };
			width[3] = (flag ? '1' : '0');
			width[4] = (sweepDirection == SweepDirection.Clockwise ? '1' : '0');
			stringBuilder1.AppendFormat(" A{0:G8},{1:G8},{2:G8},{3},{4},", width);
			PointSerializationHelper.AppendPoint(stringBuilder, point);
			return true;
		}

		private static bool GetPathFigureAsString(DocumentNode pathFigureNode, StringBuilder stringBuilder)
		{
			bool flag;
			bool flag1;
			StringBuilder stringBuilder1;
			DocumentCompositeNode documentCompositeNode = pathFigureNode as DocumentCompositeNode;
			if (documentCompositeNode == null)
			{
				return false;
			}
			if (PathGeometrySerializationHelper.IsCompositeOrNonDefaultValue<bool>(documentCompositeNode, PathMetadata.PathFigureIsFilledProperty, true))
			{
				return false;
			}
			Point point = new Point();
			if (PathGeometrySerializationHelper.GetPrimitiveValue<Point>(documentCompositeNode, PathMetadata.PathFigureStartPointProperty, ref point) == PathGeometrySerializationHelper.PropertyValueKind.Composite)
			{
				return false;
			}
			stringBuilder.Append('M');
			PointSerializationHelper.AppendPoint(stringBuilder, point);
			DocumentNode item = documentCompositeNode.Properties[PathMetadata.PathFigureSegmentsProperty];
			if (item != null)
			{
				DocumentCompositeNode documentCompositeNode1 = item as DocumentCompositeNode;
				if (documentCompositeNode1 == null)
				{
					return false;
				}
				using (IEnumerator<DocumentNode> enumerator = documentCompositeNode1.Children.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (PathGeometrySerializationHelper.GetPathSegmentAsString(enumerator.Current, stringBuilder))
						{
							continue;
						}
						flag1 = false;
						return flag1;
					}
					flag = false;
					if (PathGeometrySerializationHelper.GetPrimitiveValue<bool>(documentCompositeNode, PathMetadata.PathFigureIsClosedProperty, ref flag) == PathGeometrySerializationHelper.PropertyValueKind.Composite)
					{
						return false;
					}
					if (flag)
					{
						stringBuilder1 = stringBuilder.Append(" z");
					}
					return true;
				}
				return flag1;
			}
			flag = false;
			if (PathGeometrySerializationHelper.GetPrimitiveValue<bool>(documentCompositeNode, PathMetadata.PathFigureIsClosedProperty, ref flag) == PathGeometrySerializationHelper.PropertyValueKind.Composite)
			{
				return false;
			}
			if (flag)
			{
				stringBuilder1 = stringBuilder.Append(" z");
			}
			return true;
		}

		private static bool GetPathFigureCollectionAsString(DocumentCompositeNode pathFigureCollectionNode, StringBuilder stringBuilder)
		{
			for (int i = 0; i < pathFigureCollectionNode.Children.Count; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(' ');
				}
				if (!PathGeometrySerializationHelper.GetPathFigureAsString(pathFigureCollectionNode.Children[i], stringBuilder))
				{
					return false;
				}
			}
			return true;
		}

		private static string GetPathGeometryAsString(DocumentCompositeNode pathGeometryNode)
		{
			StringBuilder stringBuilder = new StringBuilder();
			Transform transform = null;
			PathGeometrySerializationHelper.PropertyValueKind primitiveValue = PathGeometrySerializationHelper.GetPrimitiveValue<Transform>(pathGeometryNode, PathMetadata.GeometryTransformProperty, ref transform);
			if (primitiveValue == PathGeometrySerializationHelper.PropertyValueKind.Composite || transform != null)
			{
				return null;
			}
			FillRule fillRule = FillRule.EvenOdd;
			primitiveValue = PathGeometrySerializationHelper.GetPrimitiveValue<FillRule>(pathGeometryNode, PathMetadata.PathGeometryFillRuleProperty, ref fillRule);
			if (primitiveValue == PathGeometrySerializationHelper.PropertyValueKind.Composite)
			{
				return null;
			}
			if (primitiveValue == PathGeometrySerializationHelper.PropertyValueKind.Primitive && fillRule != FillRule.EvenOdd)
			{
				stringBuilder.Append("F1 ");
			}
			DocumentNode item = pathGeometryNode.Properties[PathMetadata.PathGeometryFiguresProperty];
			if (item != null)
			{
				DocumentCompositeNode documentCompositeNode = item as DocumentCompositeNode;
				if (documentCompositeNode == null)
				{
					return null;
				}
				if (!PathGeometrySerializationHelper.GetPathFigureCollectionAsString(documentCompositeNode, stringBuilder))
				{
					return null;
				}
			}
			return stringBuilder.ToString();
		}

		private static bool GetPathSegmentAsString(DocumentNode pathSegmentNode, StringBuilder stringBuilder)
		{
			DocumentCompositeNode documentCompositeNode = pathSegmentNode as DocumentCompositeNode;
			if (documentCompositeNode == null)
			{
				return false;
			}
			if (pathSegmentNode.TypeResolver.ResolveProperty(PathMetadata.PathSegmentIsStrokedProperty) != null && PathGeometrySerializationHelper.IsCompositeOrNonDefaultValue<bool>(documentCompositeNode, PathMetadata.PathSegmentIsStrokedProperty, true))
			{
				return false;
			}
			if (pathSegmentNode.TypeResolver.ResolveProperty(PathMetadata.PathSegmentIsSmoothJoinProperty) != null && PathGeometrySerializationHelper.IsCompositeOrNonDefaultValue<bool>(documentCompositeNode, PathMetadata.PathSegmentIsSmoothJoinProperty, false))
			{
				return false;
			}
			if (PlatformTypes.ArcSegment.IsAssignableFrom(documentCompositeNode.Type))
			{
				return PathGeometrySerializationHelper.GetArcSegmentAsString(documentCompositeNode, stringBuilder);
			}
			if (PlatformTypes.LineSegment.IsAssignableFrom(documentCompositeNode.Type))
			{
				IPropertyId[] lineSegmentPointProperty = new IPropertyId[] { PathMetadata.LineSegmentPointProperty };
				return PathGeometrySerializationHelper.GetSimpleSegmentAsString(documentCompositeNode, stringBuilder, 'L', lineSegmentPointProperty);
			}
			if (PlatformTypes.QuadraticBezierSegment.IsAssignableFrom(documentCompositeNode.Type))
			{
				IPropertyId[] quadraticBezierSegmentPoint1Property = new IPropertyId[] { PathMetadata.QuadraticBezierSegmentPoint1Property, PathMetadata.QuadraticBezierSegmentPoint2Property };
				return PathGeometrySerializationHelper.GetSimpleSegmentAsString(documentCompositeNode, stringBuilder, 'Q', quadraticBezierSegmentPoint1Property);
			}
			if (PlatformTypes.BezierSegment.IsAssignableFrom(documentCompositeNode.Type))
			{
				IPropertyId[] bezierSegmentPoint1Property = new IPropertyId[] { PathMetadata.BezierSegmentPoint1Property, PathMetadata.BezierSegmentPoint2Property, PathMetadata.BezierSegmentPoint3Property };
				return PathGeometrySerializationHelper.GetSimpleSegmentAsString(documentCompositeNode, stringBuilder, 'C', bezierSegmentPoint1Property);
			}
			if (PlatformTypes.PolyLineSegment.IsAssignableFrom(documentCompositeNode.Type))
			{
				return PathGeometrySerializationHelper.GetPolySegmentAsString(documentCompositeNode, stringBuilder, 'L', PathMetadata.PolyLineSegmentPointsProperty);
			}
			if (PlatformTypes.PolyQuadraticBezierSegment.IsAssignableFrom(documentCompositeNode.Type))
			{
				return PathGeometrySerializationHelper.GetPolySegmentAsString(documentCompositeNode, stringBuilder, 'Q', PathMetadata.PolyQuadraticBezierSegmentPointsProperty);
			}
			if (!PlatformTypes.PolyBezierSegment.IsAssignableFrom(documentCompositeNode.Type))
			{
				return false;
			}
			return PathGeometrySerializationHelper.GetPolySegmentAsString(documentCompositeNode, stringBuilder, 'C', PathMetadata.PolyBezierSegmentPointsProperty);
		}

		private static bool GetPolySegmentAsString(DocumentCompositeNode polySegmentNode, StringBuilder stringBuilder, char symbol, IPropertyId pointsProperty)
		{
			DocumentPrimitiveNode item = polySegmentNode.Properties[pointsProperty] as DocumentPrimitiveNode;
			if (item == null)
			{
				return false;
			}
			PointCollection value = item.GetValue<PointCollection>();
			stringBuilder.Append(' ');
			stringBuilder.Append(symbol);
			stringBuilder.Append(PointSerializationHelper.GetPointCollectionAsString(value));
			return true;
		}

		private static PathGeometrySerializationHelper.PropertyValueKind GetPrimitiveValue<T>(DocumentCompositeNode compositeNode, IPropertyId propertyKey, ref T primitiveValue)
		{
			DocumentNode item = compositeNode.Properties[propertyKey];
			if (item == null)
			{
				return PathGeometrySerializationHelper.PropertyValueKind.Unset;
			}
			DocumentPrimitiveNode documentPrimitiveNode = item as DocumentPrimitiveNode;
			if (documentPrimitiveNode == null)
			{
				return PathGeometrySerializationHelper.PropertyValueKind.Composite;
			}
			primitiveValue = documentPrimitiveNode.GetValue<T>();
			return PathGeometrySerializationHelper.PropertyValueKind.Primitive;
		}

		private static bool GetSimpleSegmentAsString(DocumentCompositeNode simpleSegmentNode, StringBuilder stringBuilder, char symbol, params IPropertyId[] pointProperties)
		{
			stringBuilder.Append(' ');
			stringBuilder.Append(symbol);
			for (int i = 0; i < (int)pointProperties.Length; i++)
			{
				Point point = new Point();
				if (PathGeometrySerializationHelper.GetPrimitiveValue<Point>(simpleSegmentNode, pointProperties[i], ref point) == PathGeometrySerializationHelper.PropertyValueKind.Composite)
				{
					return false;
				}
				if (i > 0)
				{
					stringBuilder.Append(' ');
				}
				PointSerializationHelper.AppendPoint(stringBuilder, point);
			}
			return true;
		}

		public static bool HasAnimations(DocumentCompositeNode compositeNode)
		{
			DocumentPrimitiveNode documentPrimitiveNode;
			IPropertyId sitePropertyKey;
			documentPrimitiveNode = (!PlatformTypes.PathFigureCollection.IsAssignableFrom(compositeNode.Type) || compositeNode.Parent == null ? compositeNode.Properties[DesignTimeProperties.IsAnimatedProperty] as DocumentPrimitiveNode : compositeNode.Parent.Properties[DesignTimeProperties.IsAnimatedProperty] as DocumentPrimitiveNode);
			if (documentPrimitiveNode != null && PlatformTypes.Boolean.IsAssignableFrom(documentPrimitiveNode.Type) && documentPrimitiveNode.GetValue<bool>())
			{
				return true;
			}
			DocumentCompositeNode parent = null;
			if (PlatformTypes.PathGeometry.Equals(compositeNode.Type))
			{
				IPropertyId propertyId = compositeNode.SitePropertyKey;
				if (PathMetadata.DataProperty.Equals(propertyId) || PathMetadata.ClipProperty.Equals(propertyId))
				{
					parent = compositeNode.Parent;
				}
			}
			else if (PlatformTypes.PathFigureCollection.Equals(compositeNode.Type))
			{
				if (compositeNode.Parent != null)
				{
					sitePropertyKey = compositeNode.Parent.SitePropertyKey;
				}
				else
				{
					sitePropertyKey = null;
				}
				IPropertyId propertyId1 = sitePropertyKey;
				if (PathMetadata.DataProperty.Equals(propertyId1) || PathMetadata.ClipProperty.Equals(propertyId1))
				{
					parent = compositeNode.Parent.Parent;
				}
			}
			if (parent == null)
			{
				return false;
			}
			return PathGeometrySerializationHelper.SearchForAnimations(parent);
		}

		private static bool IsCompositeOrNonDefaultValue<T>(DocumentCompositeNode compositeNode, IPropertyId propertyKey, T defaultValue)
		{
			T t = defaultValue;
			PathGeometrySerializationHelper.PropertyValueKind primitiveValue = PathGeometrySerializationHelper.GetPrimitiveValue<T>(compositeNode, propertyKey, ref t);
			if (primitiveValue == PathGeometrySerializationHelper.PropertyValueKind.Composite)
			{
				return true;
			}
			if (primitiveValue == PathGeometrySerializationHelper.PropertyValueKind.Primitive && !object.Equals(t, defaultValue))
			{
				return true;
			}
			return false;
		}

		private static bool SearchForAnimations(DocumentCompositeNode pathElementNode)
		{
			DocumentNode item = pathElementNode.Properties[KnownProperties.FrameworkElementNameProperty];
			for (DocumentCompositeNode i = pathElementNode; i != null; i = i.Parent)
			{
				if (PathGeometrySerializationHelper.SearchForAnimationsAtElement(i, item))
				{
					return true;
				}
				if (PlatformTypes.FrameworkTemplate.IsAssignableFrom(i.Type) || PlatformTypes.Style.IsAssignableFrom(i.Type))
				{
					break;
				}
			}
			return false;
		}

		private static bool SearchForAnimationsAtElement(DocumentCompositeNode currentElement, DocumentNode pathNameNode)
		{
			if (PlatformTypes.ControlTemplate.IsAssignableFrom(currentElement.Type))
			{
				if (PathGeometrySerializationHelper.SearchForAnimationsInResources(currentElement, pathNameNode))
				{
					return true;
				}
				if (PathGeometrySerializationHelper.SearchForAnimationsInTriggers(currentElement, KnownProperties.ControlTemplateTriggersProperty, pathNameNode))
				{
					return true;
				}
			}
			else if (PlatformTypes.FrameworkElement.IsAssignableFrom(currentElement.Type))
			{
				if (PathGeometrySerializationHelper.SearchForAnimationsInResources(currentElement, pathNameNode))
				{
					return true;
				}
				if (PathGeometrySerializationHelper.SearchForAnimationsInTriggers(currentElement, KnownProperties.FrameworkElementTriggersProperty, pathNameNode))
				{
					return true;
				}
				if (PathGeometrySerializationHelper.SearchForAnimationsInVisualStateGroups(currentElement, pathNameNode))
				{
					return true;
				}
			}
			return false;
		}

		private static bool SearchForAnimationsInResources(DocumentCompositeNode elementNode, DocumentNode pathNameNode)
		{
			bool flag;
			DocumentCompositeNode resources = null;
			ISupportsResources resourcesCollection = ResourceNodeHelper.GetResourcesCollection(elementNode);
			if (resourcesCollection != null)
			{
				resources = resourcesCollection.Resources;
			}
			if (resources != null)
			{
				using (IEnumerator<DocumentNode> enumerator = resources.ChildNodes.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						DocumentCompositeNode current = enumerator.Current as DocumentCompositeNode;
						if (current == null)
						{
							continue;
						}
						DocumentNode item = current.Properties[KnownProperties.DictionaryEntryValueProperty];
						if (item == null || !PlatformTypes.Storyboard.IsAssignableFrom(item.Type))
						{
							continue;
						}
						using (IEnumerator<DocumentNode> enumerator1 = item.FindPointAnimationDescendantNodes().GetEnumerator())
						{
							while (enumerator1.MoveNext())
							{
								if (!PathGeometrySerializationHelper.DoesAnimationTargetPath(pathNameNode, enumerator1.Current))
								{
									continue;
								}
								flag = true;
								return flag;
							}
						}
					}
					return false;
				}
				return flag;
			}
			return false;
		}

		private static bool SearchForAnimationsInTriggers(DocumentCompositeNode elementNode, IPropertyId triggersProperty, DocumentNode pathNameNode)
		{
			bool flag;
			IProperty property = elementNode.Context.TypeResolver.ResolveProperty(triggersProperty);
			if (property != null)
			{
				DocumentCompositeNode item = elementNode.Properties[property] as DocumentCompositeNode;
				if (item != null && item.Children != null)
				{
					using (IEnumerator<DocumentNode> enumerator = item.FindPointAnimationDescendantNodes().GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (!PathGeometrySerializationHelper.DoesAnimationTargetPath(pathNameNode, enumerator.Current))
							{
								continue;
							}
							flag = true;
							return flag;
						}
						return false;
					}
					return flag;
				}
			}
			return false;
		}

		private static bool SearchForAnimationsInVisualStateGroups(DocumentCompositeNode elementNode, DocumentNode pathNameNode)
		{
			bool flag;
			IPropertyId member = (IPropertyId)ProjectNeutralTypes.VisualStateManager.GetMember(MemberType.AttachedProperty, "VisualStateGroups", MemberAccessTypes.Public);
			IProperty property = elementNode.Context.TypeResolver.ResolveProperty(member);
			if (property != null)
			{
				DocumentCompositeNode item = elementNode.Properties[property] as DocumentCompositeNode;
				if (item != null && item.Children != null)
				{
					using (IEnumerator<DocumentNode> enumerator = item.FindPointAnimationDescendantNodes().GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (!PathGeometrySerializationHelper.DoesAnimationTargetPath(pathNameNode, enumerator.Current))
							{
								continue;
							}
							flag = true;
							return flag;
						}
						return false;
					}
					return flag;
				}
			}
			return false;
		}

		public static string SerializeAsAttribute(DocumentCompositeNode compositeNode)
		{
			if (PlatformTypes.PathGeometry.IsAssignableFrom(compositeNode.Type))
			{
				return PathGeometrySerializationHelper.GetPathGeometryAsString(compositeNode);
			}
			if (PlatformTypes.PathFigureCollection.IsAssignableFrom(compositeNode.Type))
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (compositeNode.TypeResolver.IsCapabilitySet(PlatformCapability.SupportsPathFigureTypeConverter) && PathGeometrySerializationHelper.GetPathFigureCollectionAsString(compositeNode, stringBuilder))
				{
					return stringBuilder.ToString();
				}
			}
			return null;
		}

		private enum PropertyValueKind
		{
			Unset,
			Primitive,
			Composite
		}
	}
}