using Microsoft.Expression.DesignModel.DocumentModel;
using System;

namespace Microsoft.Expression.DesignModel.NodeBuilders
{
	public interface IDocumentNodeBuilder
	{
		Type BaseType
		{
			get;
		}

		DocumentCompositeNode BuildNode(IDocumentContext documentContext, Type instanceType);

		DocumentNode BuildNode(NodeBuilderContext context, Type instanceType, object instance);
	}
}