using System;

namespace Microsoft.Expression.DesignModel.ViewObjects
{
	public interface IViewItemsControl : IViewControl, IViewVisual, IViewObject
	{
		bool IsItemItsOwnContainer(IViewObject item);
	}
}