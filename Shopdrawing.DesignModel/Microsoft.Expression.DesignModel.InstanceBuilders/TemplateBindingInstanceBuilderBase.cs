using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public class TemplateBindingInstanceBuilderBase : MarkupExtensionInstanceBuilderBase
	{
		public TemplateBindingInstanceBuilderBase()
		{
		}

		private static void CheckForInvalidBinding(ViewNode viewNode)
		{
			if (viewNode == null || viewNode.DocumentNode == null || viewNode.DocumentNode.Parent == null || !viewNode.DocumentNode.IsProperty)
			{
				return;
			}
			IProperty sitePropertyKey = viewNode.DocumentNode.SitePropertyKey;
			if (KnownProperties.ContentControlContentProperty.Equals(sitePropertyKey) || KnownProperties.ContentPresenterContentProperty.Equals(sitePropertyKey))
			{
				DocumentCompositeNode documentNode = (DocumentCompositeNode)viewNode.DocumentNode;
				if (DocumentNodeUtilities.IsTemplateBinding(documentNode) && documentNode.Properties[KnownProperties.TemplateBindingPropertyProperty] == null)
				{
					CultureInfo currentCulture = CultureInfo.CurrentCulture;
					string invalidBindingToVisualTreeElement = ExceptionStringTable.InvalidBindingToVisualTreeElement;
					object[] name = new object[] { sitePropertyKey.Name };
					throw new InvalidOperationException(string.Format(currentCulture, invalidBindingToVisualTreeElement, name));
				}
			}
		}

		public override bool Instantiate(IInstanceBuilderContext context, ViewNode viewNode)
		{
			TemplateBindingInstanceBuilderBase.CheckForInvalidBinding(viewNode);
			bool flag = base.Instantiate(context, viewNode);
			if (context.RootTargetTypeReplacement != null && context.IsSerializationScope)
			{
				ViewNode viewNode1 = StyleControlTemplateHelper.FindContainingControlTemplate(viewNode);
				if (viewNode1 != null)
				{
					ViewNode viewNode2 = StyleControlTemplateHelper.FindStyleTemplateOwningViewNode(viewNode1);
					if (viewNode2 == null || viewNode2 == context.ViewNodeManager.Root)
					{
						DocumentCompositeNode documentNode = viewNode.DocumentNode as DocumentCompositeNode;
						if (documentNode != null && DocumentNodeUtilities.IsTemplateBinding(documentNode))
						{
							IMemberId valueAsMember = DocumentNodeHelper.GetValueAsMember(documentNode, KnownProperties.TemplateBindingPropertyProperty);
							if (valueAsMember != null)
							{
								IProperty replacementProperty = context.RootTargetTypeReplacement.GetReplacementProperty(valueAsMember as IProperty);
								if (replacementProperty != null && replacementProperty is DependencyPropertyReferenceStep && replacementProperty != valueAsMember)
								{
									DocumentCompositeNode instance = (DocumentCompositeNode)viewNode.Instance;
									instance.Properties[KnownProperties.TemplateBindingPropertyProperty] = context.DocumentContext.CreateNode(PlatformTypes.DependencyProperty, new DocumentNodeMemberValue(replacementProperty));
								}
							}
						}
					}
				}
			}
			return flag;
		}

		public override void OnViewNodeInvalidating(IInstanceBuilderContext context, ViewNode target, ViewNode child, ref bool doesInvalidRootsContainTarget, List<ViewNode> invalidRoots)
		{
			InstanceBuilderOperations.SetInvalid(context, target, ref doesInvalidRootsContainTarget, invalidRoots);
			base.OnViewNodeInvalidating(context, target, child, ref doesInvalidRootsContainTarget, invalidRoots);
		}
	}
}