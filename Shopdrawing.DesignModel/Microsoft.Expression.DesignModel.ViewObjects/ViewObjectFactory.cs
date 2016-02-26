using Microsoft.Expression.DesignModel.Core;
using System;

namespace Microsoft.Expression.DesignModel.ViewObjects
{
	internal class ViewObjectFactory : TypeHandlerFactory<IViewObjectBuilder>, IViewObjectFactory
	{
		public ViewObjectFactory(Action initializer) : base(initializer)
		{
		}

		protected override Type GetBaseType(IViewObjectBuilder handler)
		{
			return handler.GetBaseType();
		}

		public IViewObject Instantiate(object platformObject)
		{
			if (platformObject == null)
			{
				return null;
			}
			return base.GetHandler(platformObject.GetType()).Create(platformObject);
		}

		public void Register(IViewObjectBuilder value)
		{
			base.RegisterHandler(value);
		}

		public void Unregister(IViewObjectBuilder value)
		{
			base.UnregisterHandler(value);
		}
	}
}