using Microsoft.Expression.DesignModel.DocumentModel;
using System;

namespace Microsoft.Expression.DesignModel.NodeBuilders
{
	public interface IDocumentNodeChildBuilder
	{
		Type BaseType
		{
			get;
		}

		void BuildNodeChildren(NodeBuilderContext context, Type instanceType, object instance, DocumentCompositeNode compositeNode);

		Type GetChildType(Type instanceType);
	}
}