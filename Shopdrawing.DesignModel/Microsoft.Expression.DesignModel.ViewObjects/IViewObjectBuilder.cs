using System;

namespace Microsoft.Expression.DesignModel.ViewObjects
{
	public interface IViewObjectBuilder
	{
		IViewObject Create(object arg);

		Type GetBaseType();
	}
}