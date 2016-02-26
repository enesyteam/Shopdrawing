using System;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public interface IInstanceBuilderFactory
	{
		IInstanceBuilder GetBuilder(Type type);

		void Register(IInstanceBuilder value);

		void Unregister(IInstanceBuilder value);
	}
}