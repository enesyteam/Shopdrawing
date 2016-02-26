using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public class BindingInstanceBuilderBase : MarkupExtensionInstanceBuilderBase
	{
		public BindingInstanceBuilderBase()
		{
		}

		public override bool AllowPostponedResourceUpdate(IInstanceBuilderContext context, ViewNode viewNode, IProperty propertyKey, DocumentNodePath evaluatedResource)
		{
			return false;
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
				if (DocumentNodeUtilities.IsBinding(documentNode))
				{
					DocumentNode item = documentNode.Properties[KnownProperties.BindingPathProperty];
					DocumentNode item1 = documentNode.Properties[KnownProperties.BindingRelativeSourceProperty];
					DocumentNode documentNode1 = documentNode.Properties[KnownProperties.BindingElementNameProperty];
					if (item == null && (item1 != null || documentNode1 != null))
					{
						CultureInfo currentCulture = CultureInfo.CurrentCulture;
						string invalidBindingToVisualTreeElement = ExceptionStringTable.InvalidBindingToVisualTreeElement;
						object[] name = new object[] { sitePropertyKey.Name };
						throw new InvalidOperationException(string.Format(currentCulture, invalidBindingToVisualTreeElement, name));
					}
				}
			}
		}

		public override bool Instantiate(IInstanceBuilderContext context, ViewNode viewNode)
		{
			if (viewNode != null && viewNode.DocumentNode != null && viewNode.DocumentNode.Parent != null)
			{
				BindingInstanceBuilderBase.CheckForInvalidBinding(viewNode);
			}
			return base.Instantiate(context, viewNode);
		}

		public override void OnViewNodeInvalidating(IInstanceBuilderContext context, ViewNode target, ViewNode child, ref bool doesInvalidRootsContainTarget, List<ViewNode> invalidRoots)
		{
			InstanceBuilderOperations.SetInvalid(context, target, ref doesInvalidRootsContainTarget, invalidRoots);
			base.OnViewNodeInvalidating(context, target, child, ref doesInvalidRootsContainTarget, invalidRoots);
		}
	}
}