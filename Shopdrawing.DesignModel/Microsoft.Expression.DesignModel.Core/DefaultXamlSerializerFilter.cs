using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Markup;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignModel.Core
{
	public class DefaultXamlSerializerFilter : IXamlSerializerFilter
	{
		public DefaultXamlSerializerFilter()
		{
		}

		public virtual IAssembly GetClrAssembly(IType type)
		{
			return type.RuntimeAssembly;
		}

		public virtual string GetClrNamespace(IType type)
		{
			return type.Namespace;
		}

		public virtual IXmlNamespace GetReplacementNamespace(IXmlNamespace xmlNamespace)
		{
			return xmlNamespace;
		}

		public virtual string GetValueAsString(DocumentNode node)
		{
			ITypeId type = node.Type;
			Type targetType = node.TargetType;
			DocumentPrimitiveNode documentPrimitiveNode = node as DocumentPrimitiveNode;
			DocumentCompositeNode documentCompositeNode = node as DocumentCompositeNode;
			if (documentPrimitiveNode != null)
			{
				DocumentNodeStringValue value = documentPrimitiveNode.Value as DocumentNodeStringValue;
				TypeConverter valueConverter = documentPrimitiveNode.ValueConverter;
				if (value != null && valueConverter != null && valueConverter.CanConvertFrom(typeof(string)))
				{
					if (typeof(Vector3DCollection) == targetType)
					{
						Vector3DCollection vector3DCollections = (Vector3DCollection)valueConverter.ConvertFromInvariantString(value.Value);
						return PointSerializationHelper.GetVector3DCollectionAsString(vector3DCollections);
					}
					if (typeof(Point3DCollection) == targetType)
					{
						Point3DCollection point3DCollections = (Point3DCollection)valueConverter.ConvertFromInvariantString(value.Value);
						return PointSerializationHelper.GetPoint3DCollectionAsString(point3DCollections);
					}
					if (typeof(PointCollection) == targetType)
					{
						PointCollection pointCollections = (PointCollection)valueConverter.ConvertFromInvariantString(value.Value);
						return PointSerializationHelper.GetPointCollectionAsString(pointCollections);
					}
				}
			}
			else if (documentCompositeNode != null)
			{
				if (PlatformTypes.PathGeometry.IsAssignableFrom(type))
				{
					return PathGeometrySerializationHelper.SerializeAsAttribute(documentCompositeNode);
				}
				if (PlatformTypes.PathFigureCollection.IsAssignableFrom(type))
				{
					return PathGeometrySerializationHelper.SerializeAsAttribute(documentCompositeNode);
				}
				if (PlatformTypes.DoubleCollection.IsAssignableFrom(type))
				{
					return PointSerializationHelper.SerializeDoubleCollectionAsAttribute(documentCompositeNode);
				}
			}
			return null;
		}

		public virtual bool IsDesignTimeProperty(IPropertyId property)
		{
			return property.MemberType == MemberType.DesignTimeProperty;
		}

		private bool IsExplicitAnimationProperty(DocumentNode node)
		{
			DocumentPrimitiveNode documentPrimitiveNode = node as DocumentPrimitiveNode;
			if (documentPrimitiveNode != null && documentPrimitiveNode.Type.Equals(PlatformTypes.DependencyProperty))
			{
				IMember valueAsMember = DocumentPrimitiveNode.GetValueAsMember(documentPrimitiveNode);
				IProperty property = node.TypeResolver.ResolveProperty(DesignTimeProperties.ExplicitAnimationProperty);
				if (valueAsMember != null && valueAsMember.DeclaringType.FullName == property.DeclaringType.FullName && valueAsMember.Name == DesignTimeProperties.ExplicitAnimationPropertyName)
				{
					return true;
				}
			}
			return false;
		}

		protected bool IsExplicitKeyframe(DocumentCompositeNode compositeNode)
		{
			bool flag;
			DocumentCompositeNode item;
			if (PlatformTypes.DoubleAnimationUsingKeyFrames.IsAssignableFrom(compositeNode.Type))
			{
				DocumentCompositeNode documentCompositeNode = compositeNode.Properties[KnownProperties.StoryboardTargetPropertyProperty] as DocumentCompositeNode;
				if (documentCompositeNode != null && PlatformTypes.PropertyPath.IsAssignableFrom(documentCompositeNode.Type))
				{
					IProperty property = compositeNode.TypeResolver.ResolveProperty(compositeNode.TypeResolver.PlatformMetadata.KnownProperties.PropertyPathPathParameters);
					if (property != null)
					{
						item = documentCompositeNode.Properties[property] as DocumentCompositeNode;
					}
					else
					{
						item = null;
					}
					DocumentCompositeNode documentCompositeNode1 = item;
					if (documentCompositeNode1 == null || documentCompositeNode1.Children.Count <= 0)
					{
						string valueAsString = documentCompositeNode.GetValueAsString(documentCompositeNode.TypeResolver.PlatformMetadata.KnownProperties.PropertyPathPath);
						if (valueAsString != null)
						{
							return valueAsString.Contains(DesignTimeProperties.ExplicitAnimationPropertyName);
						}
					}
					else
					{
						using (IEnumerator<DocumentNode> enumerator = documentCompositeNode1.Children.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								if (!this.IsExplicitAnimationProperty(enumerator.Current))
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
			}
			return false;
		}

		private static bool IsTypeSerializable(ITypeResolver typeResolver, IType type)
		{
			if (type.IsInProject(typeResolver) || PlatformTypes.IsDesignTimeType(type.NearestResolvedType.RuntimeType))
			{
				return true;
			}
			return !PlatformTypes.IsExpressionInteractiveType(type.NearestResolvedType.RuntimeType);
		}

		public virtual bool IsXmlSpacePreserveIgnored(IType typeId)
		{
			Type runtimeType = typeId.RuntimeType;
			if (!(typeof(Vector3DCollection) == runtimeType) && !(typeof(Point3DCollection) == runtimeType) && !(typeof(PointCollection) == runtimeType))
			{
				return false;
			}
			return true;
		}

		private bool PropertyValueRequiresElement(IProperty valueProperty, IType valueTypeId)
		{
			IType propertyType;
			TypeConverter typeConverter;
			if (valueProperty == null)
			{
				propertyType = valueTypeId.PlatformMetadata.ResolveType(PlatformTypes.Object);
			}
			else
			{
				propertyType = valueProperty.PropertyType;
				ITypeId nullableType = propertyType.NullableType;
				if (nullableType != null && nullableType == valueTypeId)
				{
					return false;
				}
			}
			if (valueTypeId != valueTypeId.PlatformMetadata.ResolveType(PlatformTypes.String) && valueTypeId != propertyType && propertyType.IsAssignableFrom(valueTypeId))
			{
				if (valueProperty != null)
				{
					typeConverter = valueProperty.TypeConverter;
				}
				else
				{
					typeConverter = null;
				}
				TypeConverter typeConverter1 = typeConverter ?? propertyType.TypeConverter;
				TypeConverter typeConverter2 = valueTypeId.TypeConverter;
				if (typeConverter1 == null || typeConverter2.GetType() != typeConverter1.GetType())
				{
					return true;
				}
			}
			return false;
		}

		public virtual bool ShouldOverrideNamespaceForType(XamlSerializerContext serializerContext, ITypeId typeId)
		{
			return false;
		}

		protected virtual bool ShouldSerialize(XamlSerializerContext serializerContext, DocumentNode node)
		{
			if (node.Type != null)
			{
				DocumentCompositeNode documentCompositeNode = node as DocumentCompositeNode;
				if (documentCompositeNode != null && this.IsExplicitKeyframe(documentCompositeNode))
				{
					return false;
				}
				if (documentCompositeNode != null && documentCompositeNode.Properties[DesignTimeProperties.ShouldSerializeProperty] != null)
				{
					return false;
				}
				if (PlatformTypes.Setter.IsAssignableFrom(node.Type) && documentCompositeNode != null)
				{
					DocumentPrimitiveNode item = documentCompositeNode.Properties[KnownProperties.SetterPropertyProperty] as DocumentPrimitiveNode;
					if (item != null)
					{
						IPropertyId valueAsMember = DocumentPrimitiveNode.GetValueAsMember(item) as IPropertyId;
						if (valueAsMember != null && valueAsMember.MemberType == MemberType.DesignTimeProperty)
						{
							return false;
						}
					}
				}
				if (node.Parent != null && node.Parent.Properties[DesignTimeProperties.ShouldSerializeProperty] != null)
				{
					return false;
				}
				if (!DefaultXamlSerializerFilter.IsTypeSerializable(serializerContext.TypeResolver, node.Type))
				{
					return false;
				}
			}
			return true;
		}

		public virtual SerializedFormat ShouldSerializeChild(XamlSerializerContext serializerContext, DocumentCompositeNode parentNode, DocumentNode childNode)
		{
			if (!this.ShouldSerialize(serializerContext, childNode))
			{
				return SerializedFormat.DoNotSerialize;
			}
			return SerializedFormat.Element;
		}

		protected virtual bool ShouldSerializeDesignTimeProperty(IPropertyId propertyKey)
		{
			return false;
		}

		public virtual SerializedFormat ShouldSerializeNode(XamlSerializerContext serializerContext, DocumentNode node)
		{
			if (this.ShouldSerialize(serializerContext, node))
			{
				return SerializedFormat.Element;
			}
			return SerializedFormat.DoNotSerialize;
		}

		public virtual SerializedFormat ShouldSerializeProperty(XamlSerializerContext serializerContext, DocumentCompositeNode parentNode, IPropertyId propertyKey, DocumentNode valueNode)
		{
			bool flag;
			SerializedFormat serializedFormat = this.ShouldSerializeProperty(serializerContext, parentNode, propertyKey, valueNode, out flag);
			if (flag && !parentNode.Type.IsExpression)
			{
				return SerializedFormat.Element;
			}
			return serializedFormat;
		}

		private SerializedFormat ShouldSerializeProperty(XamlSerializerContext serializerContext, DocumentCompositeNode parentNode, IPropertyId propertyKey, DocumentNode valueNode, out bool useElementForOuterMostMarkupExtension)
		{
			bool flag;
			SerializedFormat serializedFormat;
			useElementForOuterMostMarkupExtension = false;
			DocumentCompositeNode documentCompositeNode = valueNode as DocumentCompositeNode;
			DocumentPrimitiveNode documentPrimitiveNode = valueNode as DocumentPrimitiveNode;
			IProperty valueProperty = valueNode.GetValueProperty();
			if (valueProperty != null)
			{
				if (valueProperty.MemberType != MemberType.DesignTimeProperty && !this.ShouldSerialize(serializerContext, valueNode))
				{
					return SerializedFormat.DoNotSerialize;
				}
				if (valueProperty.MemberType == MemberType.DesignTimeProperty)
				{
					bool flag1 = true;
					if (!valueProperty.ShouldSerialize)
					{
						flag1 = this.ShouldSerializeDesignTimeProperty(valueProperty);
					}
					if (!flag1)
					{
						return SerializedFormat.DoNotSerialize;
					}
				}
				if (!KnownProperties.SetterTargetNameProperty.Equals(valueProperty))
				{
					if (parentNode.Type.Metadata.IsNameProperty(valueProperty))
					{
						string valueAsString = DocumentPrimitiveNode.GetValueAsString(valueNode);
						if (valueAsString != null)
						{
							if (valueAsString.StartsWith("~", StringComparison.Ordinal) && serializerContext.InStyleOrTemplate)
							{
								return SerializedFormat.DoNotSerialize;
							}
							if (valueAsString.Length == 0)
							{
								return SerializedFormat.DoNotSerialize;
							}
						}
						return SerializedFormat.ComplexString;
					}
					if (DesignTimeProperties.UidProperty.Equals(valueProperty))
					{
						return SerializedFormat.ComplexString;
					}
					if (KnownProperties.FrameworkContentElementResourcesProperty.Equals(valueProperty))
					{
						if (documentCompositeNode != null && PlatformTypes.ResourceDictionary.IsAssignableFrom(documentCompositeNode.Type) && documentCompositeNode.SupportsChildren && documentCompositeNode.Children.Count == 0)
						{
							return SerializedFormat.DoNotSerialize;
						}
					}
					else if (!PlatformTypes.Style.IsAssignableFrom(valueProperty.PropertyType))
					{
						DependencyPropertyReferenceStep dependencyPropertyReferenceStep = valueProperty as DependencyPropertyReferenceStep;
						if (dependencyPropertyReferenceStep != null && dependencyPropertyReferenceStep.IsAttachable && !DefaultXamlSerializerFilter.IsTypeSerializable(serializerContext.TypeResolver, valueProperty.DeclaringType) && dependencyPropertyReferenceStep.MemberType != MemberType.DesignTimeProperty)
						{
							return SerializedFormat.DoNotSerialize;
						}
					}
					else if (documentCompositeNode != null)
					{
						if (documentCompositeNode.GetValue<bool>(DesignTimeProperties.IsDefaultStyleProperty))
						{
							return SerializedFormat.DoNotSerialize;
						}
					}
					else if (documentPrimitiveNode != null && documentPrimitiveNode.Value is DocumentNodeReferenceValue)
					{
						return SerializedFormat.DoNotSerialize;
					}
				}
				else
				{
					string str = DocumentPrimitiveNode.GetValueAsString(valueNode);
					if (str != null && str == "~Self")
					{
						return SerializedFormat.DoNotSerialize;
					}
				}
			}
			IType type = valueNode.Type;
			if (PlatformTypes.XData.Equals(type))
			{
				return SerializedFormat.Element;
			}
			if (PlatformTypes.StaticResource.IsAssignableFrom(type) && documentCompositeNode != null && ResourceNodeHelper.GetResourceKey(documentCompositeNode) != null)
			{
				DocumentCompositeNode parent = documentCompositeNode.Parent;
				while (parent != null && parent.Type.IsExpression)
				{
					parent = parent.Parent;
				}
				if (parent != null)
				{
					IPropertyId resourcesProperty = parent.Type.Metadata.ResourcesProperty;
					if (resourcesProperty != null)
					{
						DocumentCompositeNode item = parent.Properties[resourcesProperty] as DocumentCompositeNode;
						if (item != null && PlatformTypes.ResourceDictionary.IsAssignableFrom(item.Type))
						{
							useElementForOuterMostMarkupExtension = true;
							return SerializedFormat.ComplexString;
						}
					}
				}
			}
			if (type.IsExpression)
			{
				if (documentCompositeNode != null)
				{
					using (IEnumerator<KeyValuePair<IProperty, DocumentNode>> enumerator = documentCompositeNode.Properties.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<IProperty, DocumentNode> current = enumerator.Current;
							switch (this.ShouldSerializeProperty(serializerContext, documentCompositeNode, current.Key, current.Value, out flag))
							{
								case SerializedFormat.SimpleString:
								{
									continue;
								}
								case SerializedFormat.ComplexString:
								{
									if (current.Value.Type.IsExpression)
									{
										if (!flag)
										{
											continue;
										}
										useElementForOuterMostMarkupExtension = true;
										continue;
									}
									else
									{
										serializedFormat = SerializedFormat.Element;
										return serializedFormat;
									}
								}
							}
							serializedFormat = SerializedFormat.Element;
							return serializedFormat;
						}
						return SerializedFormat.ComplexString;
					}
					return serializedFormat;
				}
				return SerializedFormat.ComplexString;
			}
			if (this.PropertyValueRequiresElement(valueProperty, valueNode.Type))
			{
				return SerializedFormat.Element;
			}
			if (PlatformTypes.Uri.IsAssignableFrom(type))
			{
				return SerializedFormat.ComplexString;
			}
			if (PlatformTypes.PropertyPath.IsAssignableFrom(type))
			{
				if (documentCompositeNode != null)
				{
					if (DocumentPrimitiveNode.GetValueAsString(documentCompositeNode.Properties[KnownProperties.PropertyPathPathProperty]) != null)
					{
						return SerializedFormat.SimpleString;
					}
					return SerializedFormat.Element;
				}
			}
			else if (PlatformTypes.SolidColorBrush.IsAssignableFrom(type))
			{
				if (documentCompositeNode != null)
				{
					if (documentCompositeNode.Properties.Count == 0)
					{
						return SerializedFormat.SimpleString;
					}
					if (documentCompositeNode.Properties.Count == 1)
					{
						DocumentPrimitiveNode item1 = documentCompositeNode.Properties[KnownProperties.SolidColorBrushColorProperty] as DocumentPrimitiveNode;
						if (item1 != null && PlatformTypes.Color.Equals(item1.Type))
						{
							return SerializedFormat.SimpleString;
						}
					}
					return SerializedFormat.Element;
				}
			}
			else if (!PlatformTypes.Color.IsAssignableFrom(type))
			{
				if (PlatformTypes.PathGeometry.IsAssignableFrom(type))
				{
					if (valueProperty == null || PlatformTypes.PathGeometry.IsAssignableFrom(valueProperty.PropertyType))
					{
						return SerializedFormat.Element;
					}
					if (documentCompositeNode != null && !PathGeometrySerializationHelper.CanSerializeAsAttribute(documentCompositeNode))
					{
						return SerializedFormat.Element;
					}
					return SerializedFormat.ComplexString;
				}
				if (PlatformTypes.PathFigureCollection.IsAssignableFrom(type))
				{
					if (documentCompositeNode != null && !PathGeometrySerializationHelper.CanSerializeAsAttribute(documentCompositeNode))
					{
						return SerializedFormat.Element;
					}
					return SerializedFormat.ComplexString;
				}
				if (PlatformTypes.DoubleCollection.IsAssignableFrom(type))
				{
					if (documentCompositeNode != null && PointSerializationHelper.SerializeDoubleCollectionAsAttribute(documentCompositeNode) == null)
					{
						return SerializedFormat.Element;
					}
					return SerializedFormat.ComplexString;
				}
			}
			else if (documentCompositeNode == null && valueNode.Parent != null && valueNode.IsProperty)
			{
				return SerializedFormat.SimpleString;
			}
			if (documentPrimitiveNode != null && documentPrimitiveNode.Value == null)
			{
				return SerializedFormat.SimpleString;
			}
			if (XamlSerializerContext.IsXmlElement(valueNode))
			{
				return SerializedFormat.Element;
			}
			if (documentPrimitiveNode != null)
			{
				if (documentPrimitiveNode.Value is DocumentNodeStringValue)
				{
					return SerializedFormat.SimpleString;
				}
				if (documentPrimitiveNode.Value is DocumentNodeMemberValue)
				{
					return SerializedFormat.SimpleString;
				}
			}
			if (documentCompositeNode == null)
			{
				return SerializedFormat.ComplexString;
			}
			return SerializedFormat.Element;
		}
	}
}