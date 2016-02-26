using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.DesignModel.Code
{
	public interface ITextEditor : IDisposable
	{
		bool CanRedo
		{
			get;
		}

		bool CanUndo
		{
			get;
		}

		int CaretPosition
		{
			get;
			set;
		}

		bool ConvertTabsToSpace
		{
			get;
		}

		FrameworkElement Element
		{
			get;
		}

		string FontName
		{
			get;
		}

		double FontSize
		{
			get;
		}

		bool IsSelectionEmpty
		{
			get;
		}

		int LineCount
		{
			get;
		}

		int SelectionLength
		{
			get;
		}

		int SelectionStart
		{
			get;
		}

		int SquiggleCount
		{
			get;
		}

		int TabSize
		{
			get;
		}

		string Text
		{
			get;
		}

		Thickness VerticalScrollBarMargin
		{
			get;
			set;
		}

		bool WordWrap
		{
			get;
		}

		void Activated();

		void ClearSelection();

		void EnsureCaretVisible();

		void EnsureSpanVisible(int start, int length);

		int Find(string findText, bool matchCase, bool searchReverse);

		void Focus();

		IDictionary GetEditCommands();

		int GetLengthOfLineFromLineNumber(int value);

		int GetLineNumberFromPosition(int value);

		int GetLineOffsetFromPosition(int value);

		TextEditorErrorInformation GetSquiggleInformation(int squiggleIndex);

		int GetStartOfLineFromLineNumber(int value);

		int GetStartOfNextLineFromPosition(int value);

		string GetText(int offset, int length);

		IDictionary GetUndoCommands();

		void GotoLine(int lineNumber);

		void MoveLineToCenterOfView(int lineNumber);

		int Replace(string findText, string replaceText, bool matchCase);

		bool ReplaceAll(string findText, string replaceText, bool matchCase);

		void Select(int start, int length);

		void SelectCurrentWord();

		void UpdateErrorRanges(List<TextEditorErrorInformation> errorRanges);

		event EventHandler CaretPositionChanged;

		event EventHandler GotFocus;

		event EventHandler LostFocus;
	}
}