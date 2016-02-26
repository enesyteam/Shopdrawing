using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public abstract class SetterBaseInstanceBuilderHelper
	{
		protected SetterBaseInstanceBuilderHelper()
		{
		}

		public static DocumentNode GetShadowPropertyNode(IInstanceBuilderContext context, ViewNode viewNode, IProperty propertyKey, DocumentNode valueNode)
		{
			IType valueAsType;
			DocumentNodeMemberValue value;
			ReferenceStep member;
			if (!context.UseShadowProperties || propertyKey == null || !KnownProperties.SetterPropertyProperty.Equals(propertyKey))
			{
				return null;
			}
			if (context.ContainerRoot == null || !PlatformTypes.Style.IsAssignableFrom(context.ContainerRoot.Type))
			{
				return null;
			}
			DocumentCompositeNode documentNode = (DocumentCompositeNode)context.ContainerRoot.DocumentNode;
			DocumentPrimitiveNode item = documentNode.Properties[KnownProperties.StyleTargetTypeProperty] as DocumentPrimitiveNode;
			if (item != null)
			{
				valueAsType = DocumentPrimitiveNode.GetValueAsType(item);
			}
			else
			{
				valueAsType = null;
			}
			IType type = valueAsType;
			if (type == null)
			{
				return null;
			}
			DocumentPrimitiveNode documentPrimitiveNode = valueNode as DocumentPrimitiveNode;
			if (documentPrimitiveNode != null)
			{
				value = documentPrimitiveNode.Value as DocumentNodeMemberValue;
			}
			else
			{
				value = null;
			}
			DocumentNodeMemberValue documentNodeMemberValue = value;
			if (documentNodeMemberValue != null)
			{
				member = documentNodeMemberValue.Member as ReferenceStep;
			}
			else
			{
				member = null;
			}
			ReferenceStep referenceStep = member;
			if (referenceStep == null)
			{
				return null;
			}
			if (context.RootTargetTypeReplacement != null)
			{
				ViewNode viewNode1 = StyleControlTemplateHelper.FindStyleTemplateOwningViewNode(viewNode);
				if (viewNode1 == null || viewNode1 == viewNode.ViewNodeManager.Root)
				{
					IProperty replacementProperty = context.RootTargetTypeReplacement.GetReplacementProperty(referenceStep);
					if (replacementProperty != null && replacementProperty != referenceStep)
					{
						return context.DocumentContext.CreateNode(valueNode.Type, new DocumentNodeMemberValue(replacementProperty));
					}
				}
			}
			IProperty shadowProperty = DesignTimeProperties.GetShadowProperty(referenceStep, type);
			if (shadowProperty == null)
			{
				return null;
			}
			if (!DesignTimeProperties.UseShadowPropertyForInstanceBuilding(context.DocumentContext.TypeResolver, shadowProperty))
			{
				return null;
			}
			DocumentPrimitiveNode documentPrimitiveNode1 = context.DocumentContext.CreateNode(valueNode.Type, new DocumentNodeMemberValue(shadowProperty));
			return documentPrimitiveNode1;
		}
	}
}