using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public interface IInstanceBuilder
	{
		Type BaseType
		{
			get;
		}

		Type ReplacementType
		{
			get;
		}

		bool AllowPostponedResourceUpdate(IInstanceBuilderContext context, ViewNode viewNode, IProperty propertyKey, DocumentNodePath evaluatedResource);

		AttachmentOrder GetAttachmentOrder(IInstanceBuilderContext context, ViewNode viewNode);

		ViewNode GetViewNode(IInstanceBuilderContext context, DocumentNode documentNode);

		void Initialize(IInstanceBuilderContext context, ViewNode viewNode, bool isNewInstance);

		bool Instantiate(IInstanceBuilderContext context, ViewNode viewNode);

		void ModifyValue(IInstanceBuilderContext context, ViewNode target, object onlyThisInstance, IProperty propertyKey, object value, PropertyModification modification);

		void OnChildRemoving(IInstanceBuilderContext context, ViewNode parent, ViewNode child);

		void OnDescendantUpdated(IInstanceBuilderContext context, ViewNode viewNode, ViewNode child, InstanceState childState);

		void OnInitialized(IInstanceBuilderContext context, ViewNode target, object instance);

		void OnViewNodeInvalidating(IInstanceBuilderContext context, ViewNode target, ViewNode child, ref bool doesInvalidRootsContainTarget, List<ViewNode> invalidRoots);

		bool ShouldTryExpandExpression(IInstanceBuilderContext context, ViewNode viewNode, IPropertyId propertyKey, DocumentNode expressionNode);

		void UpdateChild(IInstanceBuilderContext context, ViewNode viewNode, int childIndex, DocumentNodeChangeAction action, DocumentNode childNode);

		void UpdateInstance(IInstanceBuilderContext context, ViewNode viewNode);

		void UpdateProperty(IInstanceBuilderContext context, ViewNode viewNode, IProperty propertyKey, DocumentNode valueNode);
	}
}