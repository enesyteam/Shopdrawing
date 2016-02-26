// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Documents.CodeEditor
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Code.Classifiers;
using Microsoft.Expression.Code.Nautilus;
using Microsoft.Expression.Code.UserInterface;
using Microsoft.Expression.DesignModel.Code;
using Microsoft.Expression.Framework;
using Microsoft.VisualStudio.ApplicationModel.Environments;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.UI.Text.AdornmentLibrary.Squiggles;
using Microsoft.VisualStudio.UI.Undo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.Code.Documents
{
  internal sealed class CodeEditor : ITextEditor, IDisposable
  {
    private IWpfTextView textView;
    private IWpfTextViewHost textViewHost;
    private ITextBufferUndoManager undoService;
    private IEditorOperations editorCommands;
    private CodeEditorOperations codeEditorOperations;
    private IFindLogic findLogic;
    private IClassificationTypeRegistry classificationRegistry;
    private IClassificationFormatMap classificationFormatMap;
    private CodeOptionsModel codeOptionsModel;
    private EditorSpecificOptionsModel editorSpecificOptionsModel;
    private IWpfTextViewMargin verticalScollBar;
    private IWpfLineNumberMargin lineNumberMargin;
    private ISquiggleProvider squiggleProvider;
    private IEnvironment environment;
    private IMessageDisplayService messageDisplayService;
    private string fontName;
    private double fontSize;

    public FrameworkElement Element
    {
      get
      {
        return (FrameworkElement) this.textViewHost.HostControl;
      }
    }

    public Thickness VerticalScrollBarMargin
    {
      get
      {
        return this.verticalScollBar.VisualElement.Margin;
      }
      set
      {
        this.verticalScollBar.VisualElement.Margin = value;
      }
    }

    public bool ReadOnly
    {
      get
      {
        return this.textViewHost.IsReadOnly;
      }
      set
      {
        this.textViewHost.IsReadOnly = value;
      }
    }

    public string Text
    {
      get
      {
        return this.TextSnapshot.GetText(0, this.TextSnapshot.Length);
      }
      set
      {
        this.TextBuffer.Replace(new Span(0, this.TextSnapshot.Length), value);
      }
    }

    public int LineCount
    {
      get
      {
        return this.TextSnapshot.LineCount;
      }
    }

    internal ITextView TextView
    {
      get
      {
        return (ITextView) this.textView;
      }
    }

    internal IEditorOperations EditorCommands
    {
      get
      {
        return this.editorCommands;
      }
    }

    public ITextBuffer TextBuffer
    {
      get
      {
        return this.textView.TextBuffer;
      }
    }

    public ITextSnapshot TextSnapshot
    {
      get
      {
        return this.textView.TextSnapshot;
      }
    }

    internal UndoHistory UndoHistory
    {
      get
      {
        return this.undoService.TextBufferUndoHistory;
      }
    }

    internal IEnvironment Environment
    {
      get
      {
        return this.environment;
      }
    }

    public int CaretPosition
    {
      get
      {
        return this.TextView.Caret.Position.Index;
      }
      set
      {
        this.TextView.Caret.MoveTo(value, CaretAffinity.Successor);
      }
    }

    public bool IsSelectionEmpty
    {
      get
      {
        return this.TextView.Selection.SelectionSpan.IsEmpty;
      }
    }

    public int SelectionStart
    {
      get
      {
        return this.TextView.Selection.SelectionSpan.Start;
      }
    }

    public int SelectionLength
    {
      get
      {
        return this.TextView.Selection.SelectionSpan.Length;
      }
    }

    public bool CanUndo
    {
      get
      {
        return this.undoService.TextBufferUndoHistory.CanUndo;
      }
    }

    public bool CanRedo
    {
      get
      {
        return this.undoService.TextBufferUndoHistory.CanRedo;
      }
    }

    public int SquiggleCount
    {
      get
      {
        return this.squiggleProvider.SquiggleList.Count;
      }
    }

    public string FontName
    {
      get
      {
        return this.fontName;
      }
    }

    public double FontSize
    {
      get
      {
        return this.fontSize;
      }
    }

    public bool ConvertTabsToSpace
    {
      get
      {
        return this.editorSpecificOptionsModel.ConvertTabsToSpace;
      }
    }

    public int TabSize
    {
      get
      {
        return this.editorSpecificOptionsModel.TabSize;
      }
    }

    public bool WordWrap
    {
      get
      {
        return this.editorSpecificOptionsModel.WordWrap;
      }
    }

    public event EventHandler LostFocus;

    public event EventHandler GotFocus;

    public event EventHandler CaretPositionChanged;

    internal CodeEditor(IWpfTextView textView, IWpfTextViewHost textViewHost, IEditorOperations editorCommands, ITextBufferUndoManager undoService, IFindLogic findLogic, IClassificationTypeRegistry classificationRegistry, IClassificationFormatMap classificationFormatMap, ISquiggleProvider squiggleProvider, ICompletionBroker completionBroker, CodeOptionsModel codeOptionsModel, IMessageDisplayService messageDisplayService, ICodeAidProvider codeAidProvider, IEnvironment environment)
    {
      this.textView = textView;
      this.textViewHost = textViewHost;
      this.findLogic = findLogic;
      this.classificationRegistry = classificationRegistry;
      this.classificationFormatMap = classificationFormatMap;
      this.editorCommands = editorCommands;
      this.undoService = undoService;
      this.squiggleProvider = squiggleProvider;
      this.environment = environment;
      this.messageDisplayService = messageDisplayService;
      this.codeEditorOperations = new CodeEditorOperations(this, completionBroker, codeAidProvider);
      FrameworkElement element1 = this.Element;
      element1.PreviewLostKeyboardFocus += new KeyboardFocusChangedEventHandler(this.Editor_LostFocus);
      element1.LostFocus += new RoutedEventHandler(this.Editor_LostFocus);
      element1.GotFocus += new RoutedEventHandler(this.Editor_GotFocus);
      this.textView.Caret.PositionChanged += new EventHandler<CaretPositionChangedEventArgs>(this.Caret_PositionChanged);
      this.textView.Background = (Brush) Brushes.White;
      this.textView.Selection.UnfocusedBrush = (Brush) Brushes.LightGray;
      this.verticalScollBar = (IWpfTextViewMargin) this.textViewHost.GetTextViewMargin("Wpf Vertical Scrollbar");
      UIElement element2 = ((IWpfTextViewMargin) this.textViewHost.GetTextViewMargin("Wpf Horizontal Scrollbar")).VisualElement.Parent as UIElement;
      if (element2 != null)
      {
        Grid.SetColumn(element2, 0);
        Grid.SetColumnSpan(element2, 2);
      }
      this.lineNumberMargin = (IWpfLineNumberMargin) this.textViewHost.GetTextViewMargin("Wpf Line Number Margin");
      this.lineNumberMargin.BackgroundBrush = (Brush) new SolidColorBrush(Colors.White);
      Border border = this.textViewHost.GetTextViewMargin("Spacer Margin") as Border;
      if (border != null)
      {
        border.BorderBrush = (Brush) new SolidColorBrush(Color.FromRgb((byte) 204, (byte) 204, (byte) 204));
        border.Margin = new Thickness(2.0, 0.0, 0.0, 0.0);
        border.BorderThickness = new Thickness(1.0, 0.0, 0.0, 0.0);
      }
      this.textViewHost.BottomRightMarginCorner = (UIElement) new CodeEditorCorner();
      this.codeOptionsModel = codeOptionsModel;
      this.codeOptionsModel.PropertyChanged += new PropertyChangedEventHandler(this.CodeOptionsModel_PropertyChanged);
      this.editorSpecificOptionsModel = this.codeOptionsModel.GetEditorModel(this.codeOptionsModel.GetEditorType(this.TextBuffer));
      this.editorSpecificOptionsModel.PropertyChanged += new PropertyChangedEventHandler(this.CodeOptionsModel_PropertyChanged);
      this.UpdateOptions();
    }

    public void Dispose()
    {
      FrameworkElement element = this.Element;
      element.PreviewLostKeyboardFocus -= new KeyboardFocusChangedEventHandler(this.Editor_LostFocus);
      element.LostFocus -= new RoutedEventHandler(this.Editor_LostFocus);
      element.GotFocus -= new RoutedEventHandler(this.Editor_GotFocus);
      this.textView.Caret.PositionChanged -= new EventHandler<CaretPositionChangedEventArgs>(this.Caret_PositionChanged);
      this.codeOptionsModel.PropertyChanged -= new PropertyChangedEventHandler(this.CodeOptionsModel_PropertyChanged);
      this.editorSpecificOptionsModel.PropertyChanged -= new PropertyChangedEventHandler(this.CodeOptionsModel_PropertyChanged);
      this.textViewHost.Close();
      this.textViewHost = (IWpfTextViewHost) null;
      this.textView = (IWpfTextView) null;
      if (this.codeEditorOperations == null)
        return;
      this.codeEditorOperations.Dispose();
      this.codeEditorOperations = (CodeEditorOperations) null;
    }

    public string GetText(int offset, int length)
    {
      return this.TextSnapshot.GetText(offset, length);
    }

    public void Activated()
    {
      this.SetDefaultClassifications();
    }

    public void ClearUndoStack()
    {
      this.undoService.UnregisterUndoHistory();
    }

    public void UpdateErrorRanges(List<TextEditorErrorInformation> errorRanges)
    {
      this.squiggleProvider.ClearAllSquiggles();
      errorRanges.Sort((Comparison<TextEditorErrorInformation>) ((left, right) => left.Start.CompareTo(right.Start)));
      for (int index = 0; index < errorRanges.Count; ++index)
      {
        int start = errorRanges[index].Start;
        int num = errorRanges[index].Length;
        if (index + 1 < errorRanges.Count)
          num = Math.Min(num, errorRanges[index + 1].Start - start);
        ITextSnapshot currentSnapshot = this.TextBuffer.CurrentSnapshot;
        if (start >= 0 && start <= currentSnapshot.Length)
        {
          Span span = new Span(start, Math.Min(currentSnapshot.Length - start, num));
          this.squiggleProvider.AddSquiggle(currentSnapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeExclusive), (object) errorRanges[index].Description);
        }
      }
    }

    public void Focus()
    {
      if (this.textView == null)
        return;
      this.textView.VisualElement.Focus();
    }

    public int GetLengthOfLineFromLineNumber(int value)
    {
      if (this.ClampToLineCount(ref value))
        return 0;
      return this.TextSnapshot.GetLineFromLineNumber(value).Length;
    }

    public int GetLineNumberFromPosition(int value)
    {
      if (this.ClampToLength(ref value))
        return this.TextSnapshot.LineCount;
      return this.TextSnapshot.GetLineNumberFromPosition(value);
    }

    public int GetLineOffsetFromPosition(int value)
    {
      this.ClampToLength(ref value);
      return value - this.TextSnapshot.GetLineFromPosition(value).Start;
    }

    public int GetStartOfLineFromLineNumber(int value)
    {
      this.ClampToLineCount(ref value);
      return this.TextSnapshot.GetLineFromLineNumber(value).Start;
    }

    public int GetStartOfNextLineFromPosition(int value)
    {
      this.ClampToLength(ref value);
      return this.TextSnapshot.GetLineFromPosition(value).EndIncludingLineBreak + 1;
    }

    public void Select(int start, int length)
    {
      this.TextView.Selection.Select(new Span(start, length), false);
    }

    public void ClearSelection()
    {
      this.TextView.Selection.Clear();
    }

    public void EnsureSpanVisible(int start, int length)
    {
      this.TextView.ViewScroller.EnsureSpanVisible(new Span(start, length), 0.0, 0.0, EnsureSpanVisibleFallback.SpanStart);
    }

    public void EnsureCaretVisible()
    {
      this.TextView.Caret.EnsureVisible();
    }

    public void MoveLineToCenterOfView(int lineNumber)
    {
      this.ClampToLineCount(ref lineNumber);
      ITextView textView = this.TextView;
      double verticalDistance;
      if (textView.RenderedTextLines != null && textView.RenderedTextLines.Count >= 1)
      {
        double height = textView.RenderedTextLines[0].Height;
        verticalDistance = (double) (int) (textView.ViewportHeight / 2.0 / height) * height;
      }
      else
        verticalDistance = textView.ViewportHeight / 2.0;
      textView.DisplayTextLineContainingCharacter(this.TextSnapshot.GetLineFromLineNumber(lineNumber).Start, verticalDistance, ViewRelativePosition.Top);
    }

    public void GotoLine(int lineNumber)
    {
      this.ClampToLineCount(ref lineNumber);
      this.EditorCommands.GotoLine(lineNumber);
      this.Focus();
    }

    public void SelectCurrentWord()
    {
      this.EditorCommands.SelectCurrentWord();
    }

    public int Find(string findText, bool matchCase, bool searchReverse)
    {
      if (this.textView != null)
      {
        int startIndex = this.TextView.Caret.Position.Index;
        if (!this.TextView.Selection.IsEmpty)
        {
          SnapshotSpan selectionSpan1 = this.TextView.Selection.SelectionSpan;
          SnapshotSpan selectionSpan2 = this.TextView.Selection.SelectionSpan;
          if (selectionSpan2.Length > 0)
            startIndex = !searchReverse ? selectionSpan2.End : selectionSpan2.End - 1;
        }
        this.findLogic.TextSnapshotToSearch = this.TextSnapshot;
        this.findLogic.MatchCase = matchCase;
        this.findLogic.SearchString = findText;
        this.findLogic.SearchReverse = searchReverse;
        SnapshotSpan? next = this.findLogic.FindNext(startIndex, true);
        if (next.HasValue)
        {
          ITextEditor textEditor = (ITextEditor) this;
          textEditor.ClearSelection();
          textEditor.Select(next.Value.Start, next.Value.Length);
          textEditor.EnsureSpanVisible(next.Value.Start, next.Value.Length);
          textEditor.CaretPosition = next.Value.End;
          textEditor.EnsureCaretVisible();
          return next.Value.Start;
        }
      }
      return -1;
    }

    public int Replace(string findText, string replaceText, bool matchCase)
    {
      if (this.textView == null)
        return -1;
      if (!this.TextView.Selection.IsEmpty)
      {
        int start = this.TextView.Selection.SelectionSpan.Start;
        this.findLogic.TextSnapshotToSearch = this.TextSnapshot;
        this.findLogic.MatchCase = matchCase;
        this.findLogic.SearchString = findText;
        this.findLogic.SearchReverse = false;
        SnapshotSpan? next = this.findLogic.FindNext(start, true);
        if (next.HasValue && this.TextView.Selection.SelectionSpan.Equals((object) next))
          this.EditorCommands.ReplaceSelection(replaceText, this.undoService.TextBufferUndoHistory);
      }
      return this.Find(findText, matchCase, false);
    }

    public bool ReplaceAll(string findText, string replaceText, bool matchCase)
    {
      if (this.TextView == null)
        return false;
      return this.EditorCommands.ReplaceAllMatches(findText, replaceText, matchCase, false, false, this.undoService.TextBufferUndoHistory) > 0;
    }

    public TextEditorErrorInformation GetSquiggleInformation(int squiggleIndex)
    {
      ISquiggleAdornment squiggleAdornment = this.squiggleProvider.SquiggleList[squiggleIndex];
      Span span = (Span) squiggleAdornment.Span.GetSpan(this.TextSnapshot);
      return new TextEditorErrorInformation(span.Start, span.Length, (string) squiggleAdornment.ToolTipContent);
    }

    public IDictionary GetEditCommands()
    {
      return CodeEditorStandardCommands.GetEditCommands(this, this.messageDisplayService);
    }

    public IDictionary GetUndoCommands()
    {
      return CodeEditorStandardCommands.GetUndoCommands(this);
    }

    internal void DismissCompletionSession()
    {
      this.codeEditorOperations.DismissCompletionSession();
    }

    private bool ClampToLineCount(ref int line)
    {
      return CodeEditor.Clamp(ref line, Math.Max(0, this.TextSnapshot.LineCount - 1));
    }

    private bool ClampToLength(ref int offset)
    {
      return CodeEditor.Clamp(ref offset, Math.Max(0, this.TextSnapshot.Length - 1));
    }

    private static bool Clamp(ref int value, int maxValue)
    {
      if (value <= maxValue)
        return false;
      value = maxValue;
      return true;
    }

    private void SetDefaultClassifications()
    {
      List<KeyValuePair<string, Color>> list = new List<KeyValuePair<string, Color>>();
      if (this.codeOptionsModel.GetEditorType(this.TextBuffer) == EditorType.XamlEditor)
      {
        list.Add(new KeyValuePair<string, Color>(XamlTokens.Tag.Classification, Colors.Blue));
        list.Add(new KeyValuePair<string, Color>(XamlTokens.QuotedString.Classification, Colors.Blue));
        list.Add(new KeyValuePair<string, Color>(XamlTokens.CommentDelimiter.Classification, Colors.Blue));
        list.Add(new KeyValuePair<string, Color>(XamlTokens.ElementName.Classification, Colors.DarkRed));
        list.Add(new KeyValuePair<string, Color>(XamlTokens.Comment.Classification, Colors.Green));
        list.Add(new KeyValuePair<string, Color>(XamlTokens.Attribute.Classification, Colors.Red));
        list.Add(new KeyValuePair<string, Color>(XamlTokens.Text.Classification, Colors.Black));
        list.Add(new KeyValuePair<string, Color>(XamlTokens.Quote.Classification, Colors.Black));
        list.Add(new KeyValuePair<string, Color>(XamlTokens.MarkupExtension.Classification, Colors.DarkGoldenrod));
      }
      else
      {
        list.Add(new KeyValuePair<string, Color>(CodeClassifications.Comment.Classification, Color.FromRgb((byte) 0, (byte) 127, (byte) 0)));
        list.Add(new KeyValuePair<string, Color>(CodeClassifications.Quote.Classification, Color.FromRgb((byte) 143, (byte) 0, (byte) 0)));
        list.Add(new KeyValuePair<string, Color>(CodeClassifications.Keyword.Classification, Color.FromRgb((byte) 0, (byte) 0, byte.MaxValue)));
        list.Add(new KeyValuePair<string, Color>(CodeClassifications.Type.Classification, Color.FromRgb((byte) 0, (byte) 63, (byte) 127)));
      }
      this.classificationFormatMap.SetTextProperties(this.classificationRegistry.GetClassificationType("text"), TextFormattingRunProperties.CreateTextFormattingRunProperties(new Typeface(this.fontName), this.fontSize * 4.0 / 3.0, Colors.Black));
      foreach (KeyValuePair<string, Color> keyValuePair in list)
        this.classificationFormatMap.SetTextProperties(TokenClassificationStore.GetTokenType(keyValuePair.Key), TextFormattingRunProperties.CreateTextFormattingRunProperties(new Typeface(this.fontName), this.fontSize * 4.0 / 3.0, keyValuePair.Value));
    }


    private void UpdateOptions()
    {
        this.EditorCommands.ConvertTabsToSpace = this.editorSpecificOptionsModel.ConvertTabsToSpace;
        this.textView.TabSize = this.editorSpecificOptionsModel.TabSize;
        this.fontName = this.editorSpecificOptionsModel.FontFamily;
        this.fontSize = this.editorSpecificOptionsModel.FontSize;
        this.lineNumberMargin.LineNumberFont = TextFormattingRunProperties.CreateTextFormattingRunProperties(new Typeface(this.fontName), this.fontSize * 4 / 3, Color.FromRgb(0, 128, 128));
        this.textViewHost.WordWrapStyle = (this.editorSpecificOptionsModel.WordWrap ? WordWrapStyles.WordWrap : WordWrapStyles.None);
        this.SetDefaultClassifications();
    }


    private void Editor_GotFocus(object sender, EventArgs e)
    {
      if (this.GotFocus == null)
        return;
      this.GotFocus((object) this, e);
    }

    private void Editor_LostFocus(object sender, EventArgs e)
    {
      if (this.LostFocus == null)
        return;
      this.LostFocus((object) this, e);
    }

    private void Caret_PositionChanged(object sender, CaretPositionChangedEventArgs e)
    {
      if (this.CaretPositionChanged == null)
        return;
      this.CaretPositionChanged((object) this, (EventArgs) e);
    }

    private void CodeOptionsModel_PropertyChanged(object sender, EventArgs e)
    {
      this.UpdateOptions();
    }
  }
}
