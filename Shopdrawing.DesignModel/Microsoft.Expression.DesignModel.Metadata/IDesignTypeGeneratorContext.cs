using System;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public interface IDesignTypeGeneratorContext
	{
		Type GetDesignType(Type type);
	}
}