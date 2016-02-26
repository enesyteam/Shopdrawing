using Microsoft.Expression.DesignModel.ViewObjects;
using System;
using System.Windows.Media;

namespace Microsoft.Expression.DesignModel.ViewObjects.TextObjects
{
	public interface IViewTextElement : IViewObject
	{
		IViewTextPointer ContentEnd
		{
			get;
		}

		IViewTextPointer ContentStart
		{
			get;
		}

		IViewTextPointer ElementEnd
		{
			get;
		}

		IViewTextPointer ElementStart
		{
			get;
		}

		System.Windows.Media.FontFamily FontFamily
		{
			get;
			set;
		}

		double FontSize
		{
			get;
			set;
		}
	}
}