using Microsoft.Expression.DesignModel.ViewObjects.TextObjects;
using System;
using System.Windows;

namespace Microsoft.Expression.DesignModel.ViewObjects
{
	public interface IViewTextBlock : IViewVisual, IViewObject
	{
		IViewTextPointer ContentEnd
		{
			get;
		}

		IViewTextPointer ContentStart
		{
			get;
		}

		object Inlines
		{
			get;
		}

		string Text
		{
			get;
		}

		IViewTextPointer GetPositionFromPoint(Point point);
	}
}