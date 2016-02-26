using Microsoft.Expression.DesignModel.ViewObjects;
using System;
using System.Windows;

namespace Microsoft.Expression.DesignModel.ViewObjects.TextObjects
{
	public interface IViewBlock : IViewTextElement, IViewObject
	{
		System.Windows.TextAlignment TextAlignment
		{
			get;
			set;
		}
	}
}