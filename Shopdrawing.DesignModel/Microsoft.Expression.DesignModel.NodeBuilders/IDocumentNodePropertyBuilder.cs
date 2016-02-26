using Microsoft.Expression.DesignModel.DocumentModel;
using System;

namespace Microsoft.Expression.DesignModel.NodeBuilders
{
	public interface IDocumentNodePropertyBuilder
	{
		Type BaseType
		{
			get;
		}

		void BuildNodeProperties(NodeBuilderContext context, Type instanceType, object instance, DocumentCompositeNode compositeNode);
	}
}