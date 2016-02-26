using System;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public interface IUnreferencedTypeId
	{
		bool IsKnownUnreferencedType(KnownUnreferencedType knownUnreferencedType);
	}
}