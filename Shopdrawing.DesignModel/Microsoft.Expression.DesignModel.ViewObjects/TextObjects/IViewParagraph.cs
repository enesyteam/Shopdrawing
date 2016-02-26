using Microsoft.Expression.DesignModel.ViewObjects;
using System;

namespace Microsoft.Expression.DesignModel.ViewObjects.TextObjects
{
	public interface IViewParagraph : IViewBlock, IViewTextElement, IViewInlineContainer, IViewObject
	{
		object PlatformSpecificInlines
		{
			get;
		}
	}
}