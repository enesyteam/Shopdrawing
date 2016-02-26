using Microsoft.Expression.DesignModel.ViewObjects;
using System;

namespace Microsoft.Expression.DesignModel.ViewObjects.TextObjects
{
	public interface IViewInlineUIContainer : IViewInline, IViewTextElement, IViewObject
	{
		IViewVisual UIElement
		{
			get;
			set;
		}
	}
}