using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Microsoft.Expression.DesignModel.Core
{
	public static class DocumentNodeUtilities
	{
		public static IType GetStyleOrTemplateTargetType(DocumentNode node)
		{
			return DocumentNodeHelper.GetStyleOrTemplateTargetType(node);
		}

		public static ITypeId GetStyleOrTemplateTypeAndTargetType(DocumentNode node, out IType styleOrTemplateType)
		{
			return DocumentNodeHelper.GetStyleOrTemplateTypeAndTargetType(node, out styleOrTemplateType);
		}

		public static DocumentCompositeNode GetStyleSetter(DocumentCompositeNode styleNode, IPropertyId property)
		{
			DocumentCompositeNode documentCompositeNode;
			if (styleNode != null && property != null)
			{
				IPlatformMetadata platformMetadata = styleNode.Context.TypeResolver.PlatformMetadata;
				IProperty property1 = styleNode.Context.TypeResolver.ResolveProperty(property);
				DocumentCompositeNode item = styleNode.Properties[platformMetadata.KnownProperties.StyleSetters] as DocumentCompositeNode;
				if (item != null && item.SupportsChildren && property1 != null)
				{
					using (IEnumerator<DocumentNode> enumerator = item.Children.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							DocumentCompositeNode current = enumerator.Current as DocumentCompositeNode;
							if (current == null)
							{
								continue;
							}
							IMemberId valueAsMember = DocumentNodeHelper.GetValueAsMember(current, KnownProperties.SetterPropertyProperty);
							if (valueAsMember == null || !property1.Equals(valueAsMember))
							{
								continue;
							}
							documentCompositeNode = current;
							return documentCompositeNode;
						}
						return null;
					}
					return documentCompositeNode;
				}
			}
			return null;
		}

		public static DocumentNode GetStyleSetterValueAsDocumentNode(DocumentCompositeNode styleNode, IPropertyId property)
		{
			DocumentCompositeNode styleSetter = DocumentNodeUtilities.GetStyleSetter(styleNode, property);
			if (styleSetter == null)
			{
				return null;
			}
			return styleSetter.Properties[KnownProperties.SetterValueProperty];
		}

		public static bool IsBinding(DocumentNode node)
		{
			return node.Type.IsBinding;
		}

		public static bool IsDynamicResource(DocumentNode node)
		{
			return node.Type.Equals(PlatformTypes.DynamicResource);
		}

		public static bool IsMarkupExtension(DocumentNode node)
		{
			return DocumentNodeHelper.IsMarkupExtension(node);
		}

		public static bool IsResource(DocumentNode node)
		{
			if (DocumentNodeUtilities.IsStaticResource(node))
			{
				return true;
			}
			return DocumentNodeUtilities.IsDynamicResource(node);
		}

		public static bool IsStaticExtension(DocumentNode node)
		{
			return PlatformTypes.StaticExtension.IsAssignableFrom(node.Type);
		}

		public static bool IsStaticResource(DocumentNode node)
		{
			return node.Type.Equals(PlatformTypes.StaticResource);
		}

		public static bool IsStyleOrTemplate(IType type)
		{
			return DocumentNodeHelper.IsStyleOrTemplate(type);
		}

		public static bool IsTemplateBinding(DocumentNode node)
		{
			return PlatformTypes.TemplateBinding.IsAssignableFrom(node.Type);
		}

		public static DocumentCompositeNode NewBindingNode(IDocumentContext documentContext)
		{
			return documentContext.CreateNode(PlatformTypes.Binding);
		}

		public static DocumentCompositeNode NewBindingNode(IDocumentContext documentContext, DocumentNode pathNode)
		{
			DocumentCompositeNode documentCompositeNode = DocumentNodeUtilities.NewBindingNode(documentContext);
			documentCompositeNode.Properties[KnownProperties.BindingPathProperty] = pathNode;
			return documentCompositeNode;
		}

		public static DocumentCompositeNode NewDynamicResourceNode(IDocumentContext documentContext, DocumentNode keyNode)
		{
			DocumentCompositeNode documentCompositeNode = documentContext.CreateNode(documentContext.TypeResolver.ResolveType(PlatformTypes.DynamicResource));
			documentCompositeNode.Properties[KnownProperties.DynamicResourceResourceKeyProperty] = keyNode;
			return documentCompositeNode;
		}

		public static DocumentPrimitiveNode NewRoutedEventNode(IDocumentContext documentContext, RoutedEvent routedEvent)
		{
			IEvent @event = PlatformTypeHelper.GetEvent(documentContext.TypeResolver, routedEvent);
			if (@event == null)
			{
				return null;
			}
			return documentContext.CreateNode(@event.MemberTypeId, new DocumentNodeMemberValue(@event));
		}

		public static DocumentCompositeNode NewStaticNode(IDocumentContext documentContext, IMember memberId)
		{
			return DocumentNodeHelper.NewStaticNode(documentContext, memberId);
		}

		public static DocumentCompositeNode NewStaticResourceNode(IDocumentContext documentContext, DocumentNode keyNode)
		{
			DocumentCompositeNode documentCompositeNode = documentContext.CreateNode(documentContext.TypeResolver.ResolveType(PlatformTypes.StaticResource));
			documentCompositeNode.Properties[KnownProperties.StaticResourceResourceKeyProperty] = keyNode;
			return documentCompositeNode;
		}

		public static DocumentCompositeNode NewTemplateBindingNode(DocumentNode targetNode, IPropertyId sourceProperty)
		{
			return DocumentNodeUtilities.NewTemplateBindingNode(targetNode, sourceProperty, null);
		}

		public static DocumentCompositeNode NewTemplateBindingNode(DocumentNode targetNode, IPropertyId sourceProperty, PropertyReference targetProperty)
		{
			DocumentCompositeNode documentCompositeNode = null;
			IDocumentContext context = targetNode.Context;
			if (!DocumentNodeUtilities.ShouldUseRelativeSourceTemplateBinding(targetNode, targetProperty))
			{
				IProperty property = targetNode.TypeResolver.ResolveProperty(sourceProperty);
				DocumentNode documentNode = targetNode.Context.CreateNode(PlatformTypes.DependencyProperty, new DocumentNodeMemberValue(property));
				documentCompositeNode = targetNode.Context.CreateNode(PlatformTypes.TemplateBinding);
				documentCompositeNode.Properties[KnownProperties.TemplateBindingPropertyProperty] = documentNode;
			}
			else
			{
				DocumentCompositeNode documentCompositeNode1 = targetNode.Context.CreateNode(PlatformTypes.RelativeSource);
				DocumentPrimitiveNode documentPrimitiveNode = targetNode.Context.CreateNode(PlatformTypes.RelativeSourceMode, new DocumentNodeStringValue("TemplatedParent"));
				documentCompositeNode1.Properties[KnownProperties.RelativeSourceModeProperty] = documentPrimitiveNode;
				IPlatformTypes platformMetadata = (IPlatformTypes)targetNode.PlatformMetadata;
				object obj = platformMetadata.MakePropertyPath(sourceProperty.Name, new object[0]);
				DocumentNode documentNode1 = targetNode.Context.CreateNode(obj.GetType(), obj);
				documentCompositeNode = targetNode.Context.CreateNode(PlatformTypes.Binding);
				documentCompositeNode.Properties[KnownProperties.BindingRelativeSourceProperty] = documentCompositeNode1;
				documentCompositeNode.Properties[KnownProperties.BindingPathProperty] = documentNode1;
			}
			return documentCompositeNode;
		}

		public static DocumentNode NewUriDocumentNode(IDocumentContext documentContext, Uri uri)
		{
			return documentContext.CreateNode(documentContext.TypeResolver.ResolveType(PlatformTypes.Uri), new DocumentNodeStringValue(uri.OriginalString));
		}

		private static bool ShouldUseRelativeSourceTemplateBinding(DocumentNode targetNode, PropertyReference targetProperty)
		{
			bool flag;
			if (!targetNode.PlatformMetadata.IsCapabilitySet(PlatformCapability.UseRelativeSourceTemplateBinding))
			{
				return false;
			}
			if (!PlatformTypes.FrameworkElement.IsAssignableFrom(targetNode.Type) && !PlatformTypes.FrameworkContentElement.IsAssignableFrom(targetNode.Type))
			{
				return true;
			}
			if (targetProperty != null)
			{
				using (IEnumerator<ReferenceStep> enumerator = targetProperty.ReferenceSteps.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ReferenceStep current = enumerator.Current;
						if (!PlatformTypes.Freezable.IsAssignableFrom(current.DeclaringType) || PlatformTypes.FrameworkElement.IsAssignableFrom(current.DeclaringType) || PlatformTypes.FrameworkContentElement.IsAssignableFrom(current.DeclaringType))
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
			return false;
		}
	}
}