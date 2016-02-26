using System;

namespace Microsoft.Expression.DesignModel.NodeBuilders
{
	public interface IDocumentNodeChildBuilderFactory
	{
		IDocumentNodeChildBuilder ArrayChildBuilder
		{
			get;
			set;
		}

		IDocumentNodeChildBuilder CollectionChildBuilder
		{
			get;
			set;
		}

		IDocumentNodeChildBuilder GetChildBuilder(Type type);

		void Register(IDocumentNodeChildBuilder builder);

		void Unregister(IDocumentNodeChildBuilder builder);
	}
}