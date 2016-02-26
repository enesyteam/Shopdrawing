using Microsoft.Expression.DesignModel.ViewObjects;
using System;

namespace Microsoft.Expression.DesignModel.ViewObjects.TextObjects
{
	public interface IViewRun : IViewInline, IViewTextElement, IViewObject
	{
		string Text
		{
			get;
			set;
		}
	}
}