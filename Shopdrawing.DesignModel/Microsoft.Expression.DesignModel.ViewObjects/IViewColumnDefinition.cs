using System;
using System.Windows;

namespace Microsoft.Expression.DesignModel.ViewObjects
{
	public interface IViewColumnDefinition : IViewObject
	{
		double ActualWidth
		{
			get;
		}

		GridLength Width
		{
			get;
		}
	}
}