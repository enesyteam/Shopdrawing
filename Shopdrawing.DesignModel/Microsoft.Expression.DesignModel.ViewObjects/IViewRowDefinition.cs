using System;
using System.Windows;

namespace Microsoft.Expression.DesignModel.ViewObjects
{
	public interface IViewRowDefinition : IViewObject
	{
		double ActualHeight
		{
			get;
		}

		GridLength Height
		{
			get;
		}
	}
}