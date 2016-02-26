using Microsoft.Expression.DesignModel.ViewObjects;
using System;
using System.Windows;

namespace Microsoft.Expression.DesignModel.ViewObjects.TextObjects
{
	public interface IViewFlowDocument : IViewBlockContainer, IViewObject
	{
		double LineHeight
		{
			get;
			set;
		}

		System.Windows.LineStackingStrategy LineStackingStrategy
		{
			get;
			set;
		}

		Thickness PagePadding
		{
			get;
			set;
		}
	}
}