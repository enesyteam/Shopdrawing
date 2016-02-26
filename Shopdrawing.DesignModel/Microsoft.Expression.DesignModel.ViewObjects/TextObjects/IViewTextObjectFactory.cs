using Microsoft.Expression.DesignModel.ViewObjects;

namespace Microsoft.Expression.DesignModel.ViewObjects.TextObjects
{
	public interface IViewTextObjectFactory
	{
		IViewParagraph CreateParagraph();

		IViewRichTextBox CreateRichTextBox();

		IViewRun CreateRun();

		IViewTextBox CreateTextBox();

		IViewTextRange CreateTextRange(IViewBlockContainer blockContainer, IViewTextPointer startPointer, IViewTextPointer endPointer);
	}
}