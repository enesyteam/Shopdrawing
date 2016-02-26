using System;

namespace Microsoft.Expression.DesignModel.ViewObjects
{
	public interface IViewObjectFactory
	{
		IViewObject Instantiate(object platformObject);

		void Register(IViewObjectBuilder value);

		void Unregister(IViewObjectBuilder value);
	}
}