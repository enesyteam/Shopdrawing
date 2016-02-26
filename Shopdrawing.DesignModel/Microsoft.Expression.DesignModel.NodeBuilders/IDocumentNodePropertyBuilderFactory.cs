using Microsoft.Expression.DesignModel.Metadata;
using System;

namespace Microsoft.Expression.DesignModel.NodeBuilders
{
	public interface IDocumentNodePropertyBuilderFactory
	{
		IDocumentNodePropertyBuilder DefaultPropertyBuilder
		{
			get;
			set;
		}

		IDocumentNodePropertyBuilder GetPropertyBuilder(ITypeResolver typeResolver, Type type);

		void Register(IDocumentNodePropertyBuilder builder);

		void Unregister(IDocumentNodePropertyBuilder builder);
	}
}