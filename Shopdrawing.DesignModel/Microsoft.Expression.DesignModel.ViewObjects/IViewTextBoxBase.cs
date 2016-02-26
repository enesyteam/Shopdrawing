using Microsoft.Expression.DesignModel.ViewObjects.TextObjects;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignModel.ViewObjects
{
	public interface IViewTextBoxBase : IViewControl, IViewVisual, IViewObject
	{
		bool AcceptsReturn
		{
			get;
			set;
		}

		bool AcceptsTab
		{
			get;
			set;
		}

		bool CanRedo
		{
			get;
		}

		bool CanUndo
		{
			get;
		}

		double Height
		{
			get;
			set;
		}

		object InstanceBuilderContext
		{
			get;
			set;
		}

		bool IsEnabled
		{
			get;
			set;
		}

		bool IsInputMethodEnabled
		{
			get;
			set;
		}

		bool IsSpellCheckEnabled
		{
			get;
			set;
		}

		bool IsUndoEnabled
		{
			get;
			set;
		}

		double Opacity
		{
			get;
			set;
		}

		Thickness Padding
		{
			get;
			set;
		}

		object TextEditProxyObject
		{
			get;
			set;
		}

		ViewTextBoxType Type
		{
			get;
		}

		ScrollBarVisibility VerticalScrollBarVisibility
		{
			get;
			set;
		}

		double Width
		{
			get;
			set;
		}

		void Copy();

		void Cut();

		void Focus();

		void Paste();

		bool Redo();

		void SelectAll();

		void SetTransformMatrix(Matrix transformMatrix);

		bool Undo();

		event KeyboardFocusChangedEventHandler GotKeyboardFocus;

		event KeyEventHandler KeyDown;

		event RoutedEventHandler LostFocus;

		event KeyboardFocusChangedEventHandler PreviewLostKeyboardFocus;

		event TextCompositionEventHandler PreviewTextInput;

		event RoutedEventHandler SelectionChanged;

		event TextChangedEventHandler TextChanged;
	}
}