using Microsoft.Expression.DesignModel.Core;
using System;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	internal sealed class InstanceBuilderFactory : TypeHandlerFactory<IInstanceBuilder>, IInstanceBuilderFactory
	{
		public InstanceBuilderFactory(Action initializer) : base(initializer)
		{
		}

		protected override Type GetBaseType(IInstanceBuilder handler)
		{
			return handler.BaseType;
		}

		public IInstanceBuilder GetBuilder(Type type)
		{
			return base.GetHandler(type);
		}

		public void Register(IInstanceBuilder value)
		{
			base.RegisterHandler(value);
		}

		public void Unregister(IInstanceBuilder value)
		{
			base.UnregisterHandler(value);
		}
	}
}