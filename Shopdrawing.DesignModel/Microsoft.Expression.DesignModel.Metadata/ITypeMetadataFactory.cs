using System;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public interface ITypeMetadataFactory
	{
		ITypeMetadata GetMetadata(Type type);

		void Register(Type type, MetadataFactoryCallback factory);

		void Reset();
	}
}