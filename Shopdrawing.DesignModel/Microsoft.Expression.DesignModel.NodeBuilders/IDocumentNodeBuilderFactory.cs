using Microsoft.Expression.DesignModel.DocumentModel;
using System;

namespace Microsoft.Expression.DesignModel.NodeBuilders
{
	public interface IDocumentNodeBuilderFactory
	{
		IDocumentNodeBuilder DefaultBuilder
		{
			get;
			set;
		}

		IDisposable ForceBuildAnimatedValue
		{
			get;
		}

		bool IsForcingBuildAnimatedValue
		{
			get;
		}

		DocumentCompositeNode BuildNode(IDocumentContext documentContext, Type instanceType);

		DocumentNode BuildNode(IDocumentContext documentContext, object instance);

		DocumentNode BuildNode(NodeBuilderContext context, object instance);

		DocumentNode BuildNode(IDocumentContext documentContext, Type instanceType, object instance);

		DocumentNode BuildNode(NodeBuilderContext context, Type instanceType, object instance);

		IDocumentNodeBuilder GetBuilder(Type instanceType);

		void Register(IDocumentNodeBuilder value);

		void Unregister(IDocumentNodeBuilder value);
	}
}