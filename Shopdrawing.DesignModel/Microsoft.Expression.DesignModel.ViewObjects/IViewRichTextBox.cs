using Microsoft.Expression.DesignModel.ViewObjects.TextObjects;
using System.Windows;

namespace Microsoft.Expression.DesignModel.ViewObjects
{
	public interface IViewRichTextBox : IViewTextBoxBase, IViewControl, IViewVisual, IViewObject
	{
		IViewBlockContainer BlockContainer
		{
			get;
		}

		IViewTextRange EntireRange
		{
			get;
		}

		IViewTextSelection Selection
		{
			get;
		}

		IViewTextPointer GetPositionFromPoint(Point point);
	}
}