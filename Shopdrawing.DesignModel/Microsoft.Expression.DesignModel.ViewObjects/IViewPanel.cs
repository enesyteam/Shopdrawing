using System;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignModel.ViewObjects
{
	public interface IViewPanel : IViewVisual, IViewObject
	{
		int ChildrenCount
		{
			get;
		}

		System.Windows.Controls.Orientation Orientation
		{
			get;
		}

		IViewVisual GetChild(int index);
	}
}