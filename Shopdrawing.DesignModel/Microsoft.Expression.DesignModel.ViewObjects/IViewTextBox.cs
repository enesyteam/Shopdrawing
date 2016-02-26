using System;
using System.Windows;

namespace Microsoft.Expression.DesignModel.ViewObjects
{
	public interface IViewTextBox : IViewTextBoxBase, IViewControl, IViewVisual, IViewObject
	{
		double FontSize
		{
			get;
			set;
		}

		string SelectedText
		{
			get;
			set;
		}

		int SelectionLength
		{
			get;
			set;
		}

		int SelectionStart
		{
			get;
			set;
		}

		string Text
		{
			get;
			set;
		}

		System.Windows.TextAlignment TextAlignment
		{
			get;
			set;
		}

		System.Windows.TextWrapping TextWrapping
		{
			get;
			set;
		}

		void Select(int selectionStart, int selectionLength);
	}
}