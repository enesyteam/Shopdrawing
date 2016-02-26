using System;

namespace Microsoft.Expression.DesignModel.ViewObjects
{
	public interface IParentChainProvider
	{
		IViewVisual GetParent(int index);
	}
}