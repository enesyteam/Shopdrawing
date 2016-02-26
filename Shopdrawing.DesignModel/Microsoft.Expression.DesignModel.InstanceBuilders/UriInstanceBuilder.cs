using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public sealed class UriInstanceBuilder : ClrObjectInstanceBuilder
	{
		public override Type BaseType
		{
			get
			{
				return typeof(Uri);
			}
		}

		public UriInstanceBuilder()
		{
		}

		public override bool Instantiate(IInstanceBuilderContext context, ViewNode viewNode)
		{
			IPropertyId sitePropertyKey;
			DocumentPrimitiveNode documentNode = viewNode.DocumentNode as DocumentPrimitiveNode;
			if (documentNode != null)
			{
				DocumentNodeStringValue value = documentNode.Value as DocumentNodeStringValue;
				if (value != null)
				{
					if (viewNode.Parent == null || !viewNode.IsProperty)
					{
						sitePropertyKey = null;
					}
					else
					{
						sitePropertyKey = viewNode.SitePropertyKey;
					}
					IPropertyId propertyProperty = sitePropertyKey;
					if (viewNode.Parent != null)
					{
						IPropertyValueTypeMetadata metadata = viewNode.Parent.DocumentNode.Type.Metadata as IPropertyValueTypeMetadata;
						IPropertyValueTypeMetadata propertyValueTypeMetadatum = metadata;
						if (metadata != null)
						{
							propertyProperty = propertyValueTypeMetadatum.PropertyProperty;
						}
					}
					if (this.ShouldUseDesignTimeUri(propertyProperty))
					{
						Uri uri = new Uri(value.Value, UriKind.RelativeOrAbsolute);
						Uri uri1 = viewNode.DocumentNode.Context.MakeDesignTimeUri(uri);
						if (!context.IsSerializationScope)
						{
							viewNode.Instance = uri1;
						}
						else
						{
							viewNode.Instance = DocumentNodeUtilities.NewUriDocumentNode(context.DocumentContext, uri1);
						}
						viewNode.InstanceState = InstanceState.Valid;
					}
				}
			}
			return base.Instantiate(context, viewNode);
		}

		public override void OnViewNodeInvalidating(IInstanceBuilderContext context, ViewNode target, ViewNode child, ref bool doesInvalidRootsContainTarget, List<ViewNode> invalidRoots)
		{
			if (target.InstanceState.IsPropertyOrChildInvalid)
			{
				InstanceBuilderOperations.SetInvalid(context, target, ref doesInvalidRootsContainTarget, invalidRoots);
			}
			base.OnViewNodeInvalidating(context, target, child, ref doesInvalidRootsContainTarget, invalidRoots);
		}

		private bool ShouldUseDesignTimeUri(IPropertyId parentProperty)
		{
			if (KnownProperties.FrameSourceProperty.Equals(parentProperty))
			{
				return false;
			}
			return !DesignTimeProperties.RuntimeFontUriProperty.Equals(parentProperty);
		}

		public override void UpdateProperty(IInstanceBuilderContext context, ViewNode viewNode, IProperty propertyKey, DocumentNode valueNode)
		{
			InstanceBuilderOperations.UpdatePropertyWithoutApply(context, viewNode, propertyKey, valueNode);
		}
	}
}