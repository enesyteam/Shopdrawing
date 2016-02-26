// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Documents.CodeEditorOperations
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Code.CodeAid.Xaml;
using Microsoft.Expression.DesignModel.Code;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.UI.Undo;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Expression.Code.Documents
{
  internal class CodeEditorOperations : IDisposable
  {
    private FrameworkElement editorElement;
    private IEditorOperations editorOperations;
    private UndoHistory undoHistory;
    private ITextView textView;
    private XamlCodeAidEngine codeAidEngine;
    private ICompletionBroker completionBroker;
    private ICompletionSession activeSession;
    private ITrackingPoint activeSessionStartPoint;

    public CodeEditorOperations(CodeEditor codeEditor, ICompletionBroker completionBroker, ICodeAidProvider codeAidProvider)
    {
      this.editorElement = codeEditor.Element;
      this.editorOperations = codeEditor.EditorCommands;
      this.undoHistory = codeEditor.UndoHistory;
      this.textView = codeEditor.TextView;
      this.completionBroker = completionBroker;
      this.codeAidEngine = new XamlCodeAidEngine(codeEditor.TextBuffer, codeAidProvider, codeEditor.Environment);
      this.editorElement.TextInput += new TextCompositionEventHandler(this.OnTextInput);
      this.editorElement.KeyDown += new KeyEventHandler(this.OnKeyDown);
      this.editorElement.MouseDown += new MouseButtonEventHandler(this.OnMouseDown);
      this.undoHistory.UndoRedoHappened += new EventHandler<UndoRedoEventArgs>(this.OnUndoRedoHappened);
      TextCompositionManager.AddTextInputStartHandler((DependencyObject) this.editorElement, new TextCompositionEventHandler(this.OnTextInputStart));
      TextCompositionManager.AddTextInputUpdateHandler((DependencyObject) this.editorElement, new TextCompositionEventHandler(this.OnTextInputUpdate));
    }

    ~CodeEditorOperations()
    {
      this.Dispose(false);
    }

    private void InitializeActiveCompletionSession()
    {
      PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.XamlIntellisenseCreateMenu);
      this.activeSessionStartPoint = this.textView.TextBuffer.CurrentSnapshot.CreateTrackingPoint(this.textView.Caret.Position.Index, PointTrackingMode.Negative);
      this.activeSession.Committed += new EventHandler(this.ActiveSession_Committed);
      this.activeSession.Dismissed += new EventHandler(this.ActiveSession_Dismissed);
      this.activeSession.Presenter.ReleaseKeyboard();
      if (this.activeSession.Completions.Count == 1)
      {
        CompletionSelectionOptions selectionOptions = CompletionSelectionOptions.Unique;
        if (this.activeSession.Completions[0].ApplicableTo.GetSpan(this.textView.TextBuffer.CurrentSnapshot).Length == 0)
          this.activeSession.SetSelectionStatus(this.activeSession.Completions[0], selectionOptions | CompletionSelectionOptions.Selected);
        else
          this.CompletionSessionMatchTrackingWorker();
      }
      else
        this.CompletionSessionMatchTrackingWorker();
    }

    private void OnUndoRedoHappened(object sender, UndoRedoEventArgs args)
    {
      this.DismissCompletionSession();
    }

    private void OnTextInputStart(object sender, TextCompositionEventArgs args)
    {
      if (!(args.TextComposition is ImeTextComposition))
        return;
      this.HandleProvisionalImeInput(args);
    }

    private void OnTextInputUpdate(object sender, TextCompositionEventArgs args)
    {
      if (args.TextComposition is ImeTextComposition)
        this.HandleProvisionalImeInput(args);
      else
        args.Handled = false;
    }

    private void HandleProvisionalImeInput(TextCompositionEventArgs args)
    {
      if (args.Text.Length > 0)
      {
        this.CreateAndExecuteCodeAidAction(args, true);
        this.editorOperations.InsertProvisionalText(args.Text, this.undoHistory);
        this.textView.Caret.EnsureVisible();
      }
      args.Handled = true;
    }

    private bool CreateAndExecuteCodeAidAction(TextCompositionEventArgs e, bool matchingModeOnly)
    {
      PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.TextInsertLetter);
      if (this.completionBroker == null)
        return false;
      CodeAidContext codeAidContext = this.CreateCodeAidContext();
      CodeAidAction action = this.codeAidEngine.ProcessTextInput(e.Text, codeAidContext);
      if (!action.IsNone)
        this.ExecuteCodeAidAction(action, matchingModeOnly);
      if (!action.TestFlag(CodeAidActionType.EatInput) && !matchingModeOnly)
        this.editorOperations.InsertText(e.Text, this.undoHistory);
      return true;
    }

    private void OnTextInput(object sender, TextCompositionEventArgs e)
    {
      if (e.Text.Length > 0 || this.editorOperations.ProvisionalCompositionSpan != null)
      {
        if (!this.CreateAndExecuteCodeAidAction(e, false))
          this.editorOperations.InsertText(e.Text, this.undoHistory);
        this.textView.Caret.EnsureVisible();
      }
      e.Handled = true;
    }

    private void ExecuteCodeAidAction(CodeAidAction action, bool matchingModeOnly)
    {
      if (action.TestFlag(CodeAidActionType.CommitSession) && !matchingModeOnly)
        this.CommitCompletionSession(action.TestFlag(CodeAidActionType.CommitOnHollowSelection));
      else if (action.TestFlag(CodeAidActionType.DismissSession))
        this.DismissCompletionSession();
      if (action.TestFlag(CodeAidActionType.StartSession))
        this.AsyncStartCompletionSession();
      if (action.TestFlag(CodeAidActionType.MatchTracking))
        this.AsyncMatchCompletionSession();
      if (action.TestFlag(CodeAidActionType.MoveSelDown))
        this.OffsetCompletionSelection(1);
      if (action.TestFlag(CodeAidActionType.MoveSelUp))
        this.OffsetCompletionSelection(-1);
      if (action.TestFlag(CodeAidActionType.MoveSelPageDown))
        this.OffsetCompletionSelection(7);
      if (action.TestFlag(CodeAidActionType.MoveSelPageUp))
        this.OffsetCompletionSelection(-7);
      if (action.TestFlag(CodeAidActionType.MoveSelHome) && this.activeSession != null)
        this.activeSession.SetSelectionStatus(this.activeSession.Completions[0], CompletionSelectionOptions.Selected | CompletionSelectionOptions.Unique);
      if (action.TestFlag(CodeAidActionType.MoveSelEnd) && this.activeSession != null)
        this.activeSession.SetSelectionStatus(this.activeSession.Completions[this.activeSession.Completions.Count - 1], CompletionSelectionOptions.Selected | CompletionSelectionOptions.Unique);
      if (action.TestFlag(CodeAidActionType.AutoGenDot) && !matchingModeOnly)
      {
        int num = this.LeftmostTextViewSelection();
        this.editorOperations.InsertText(".", this.undoHistory);
        this.textView.Caret.MoveTo(num + 1);
      }
      if (action.TestFlag(CodeAidActionType.AutoGenAttrQuotes) && !matchingModeOnly)
      {
        int num1 = this.LeftmostTextViewSelection();
        this.editorOperations.InsertText("=\"\"", this.undoHistory);
        int num2 = action.TestFlag(CodeAidActionType.CreateCloseTag) ? 3 : 2;
        this.textView.Caret.MoveTo(num1 + num2);
      }
      if (action.TestFlag(CodeAidActionType.CreateCloseTag) && !matchingModeOnly)
      {
        string autoCloseTag = this.codeAidEngine.GetAutoCloseTag(this.CreateCodeAidContext());
        if (!string.IsNullOrEmpty(autoCloseTag))
        {
          int caretIndex = this.LeftmostTextViewSelection();
          this.editorOperations.InsertText("</" + autoCloseTag + ">", this.undoHistory);
          this.textView.Caret.MoveTo(caretIndex);
        }
      }
      if (action.TestFlag(CodeAidActionType.AutoGenClosingBracket) && !matchingModeOnly)
      {
        int num = this.LeftmostTextViewSelection();
        this.editorOperations.InsertText(">", this.undoHistory);
        this.textView.Caret.MoveTo(num + 1);
      }
      if (!action.TestFlag(CodeAidActionType.AutoGenCloseComment) || matchingModeOnly)
        return;
      int caretIndex1 = this.LeftmostTextViewSelection();
      this.editorOperations.InsertText("-->", this.undoHistory);
      this.textView.Caret.MoveTo(caretIndex1);
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Handled)
        return;
      if (this.completionBroker != null)
      {
        CodeAidContext codeAidContext = this.CreateCodeAidContext();
        CodeAidAction action = this.codeAidEngine.ProcessKeyDown(e.Key, codeAidContext);
        if (!action.IsNone)
          this.ExecuteCodeAidAction(action, false);
        if (action.TestFlag(CodeAidActionType.EatInput))
          e.Handled = true;
        else
          this.OnKeyDownHelper(e);
        this.ExecuteFormattingAction(action);
      }
      else
        this.OnKeyDownHelper(e);
    }

    private void ExecuteFormattingAction(CodeAidAction action)
    {
      ITextSnapshot textSnapshot = this.textView.TextSnapshot;
      int index = this.textView.Caret.Position.Index;
      string forContainingTag = this.codeAidEngine.GetBeginningWhitespaceForContainingTag(textSnapshot, index);
      if (action.TestFlag(CodeAidActionType.AutoGenNewLineBelow))
      {
        this.editorOperations.InsertNewline(this.undoHistory);
        this.ReplaceTextRange(this.textView.TextSnapshot.GetLineFromPosition(index).Start, index, forContainingTag);
        this.textView.Caret.MoveTo(index);
      }
      if (!action.TestFlag(CodeAidActionType.AutoGenIndent))
        return;
      this.ReplaceTextRange(this.textView.TextSnapshot.GetLineFromPosition(index).Start, index, forContainingTag + "\t");
    }

    private void ReplaceTextRange(int beginningOffset, int endOffset, string newText)
    {
      this.textView.TextBuffer.Replace(new Span(beginningOffset, endOffset - beginningOffset), newText);
    }

    private void OnKeyDownHelper(KeyEventArgs e)
    {
      bool select = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
      bool flag1 = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
      bool flag2 = (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt;
      bool flag3 = Keyboard.Modifiers == ModifierKeys.Control;
      if (Keyboard.Modifiers == ModifierKeys.Shift)
        this.HandleOnlyShiftKeyDown(e);
      if (flag3)
        this.HandleOnlyControlKeyDown(e);
      if (flag1 && !flag2)
        this.HandleControlAndNotAltKeyDown(e, select);
      if (!flag1 && !flag2 && !select)
        this.HandleNoModifiersKeyDown(e);
      if (!flag1 && !flag2)
        this.HandleNoControlAltKeyDown(e, select);
      if (!flag1)
        return;
      this.DismissCompletionSession();
    }

    private void HandleNoControlAltKeyDown(KeyEventArgs e, bool select)
    {
      switch (e.Key)
      {
        case Key.Back:
          this.editorOperations.DeleteCharacterToLeft(this.undoHistory);
          e.Handled = true;
          break;
        case Key.Return:
          this.editorOperations.InsertNewline(this.undoHistory);
          e.Handled = true;
          break;
        case Key.Prior:
          this.editorOperations.PageUp(select);
          e.Handled = true;
          break;
        case Key.Next:
          this.editorOperations.PageDown(select);
          e.Handled = true;
          break;
        case Key.End:
          this.editorOperations.MoveToEndOfLine(select);
          e.Handled = true;
          break;
        case Key.Home:
          this.editorOperations.MoveToStartOfLine(select);
          e.Handled = true;
          break;
        case Key.Left:
          this.editorOperations.MoveToPreviousCharacter(select);
          e.Handled = true;
          break;
        case Key.Up:
          this.editorOperations.MoveLineUp(select);
          e.Handled = true;
          break;
        case Key.Right:
          this.editorOperations.MoveToNextCharacter(select);
          e.Handled = true;
          break;
        case Key.Down:
          this.editorOperations.MoveLineDown(select);
          e.Handled = true;
          break;
      }
    }

    private void HandleNoModifiersKeyDown(KeyEventArgs e)
    {
      switch (e.Key)
      {
        case Key.Tab:
          if (!this.textView.Selection.IsEmpty)
          {
            this.editorOperations.IndentSelection(this.undoHistory);
            e.Handled = true;
            break;
          }
          this.editorOperations.InsertTab(this.undoHistory);
          e.Handled = true;
          break;
        case Key.Escape:
          this.editorOperations.ResetSelection();
          e.Handled = true;
          break;
        case Key.Insert:
          this.editorOperations.OverwriteMode = !this.editorOperations.OverwriteMode;
          e.Handled = true;
          break;
        case Key.Delete:
          this.editorOperations.DeleteCharacterToRight(this.undoHistory);
          e.Handled = true;
          break;
      }
    }

    private void HandleControlAndNotAltKeyDown(KeyEventArgs e, bool select)
    {
      switch (e.Key)
      {
        case Key.End:
          this.editorOperations.MoveToEndOfDocument(select);
          e.Handled = true;
          break;
        case Key.Home:
          this.editorOperations.MoveToStartOfDocument(select);
          e.Handled = true;
          break;
        case Key.Left:
          this.editorOperations.MoveToPreviousWord(select);
          e.Handled = true;
          break;
        case Key.Right:
          this.editorOperations.MoveToNextWord(select);
          e.Handled = true;
          break;
      }
    }

    private void HandleOnlyControlKeyDown(KeyEventArgs e)
    {
      switch (e.Key)
      {
        case Key.Up:
          this.editorOperations.ScrollUpAndMoveCaretIfNecessary();
          e.Handled = true;
          break;
        case Key.Down:
          this.editorOperations.ScrollDownAndMoveCaretIfNecessary();
          e.Handled = true;
          break;
        case Key.Insert:
          this.editorOperations.CopySelection();
          e.Handled = true;
          break;
        case Key.Delete:
          this.editorOperations.DeleteWordToRight(this.undoHistory);
          e.Handled = true;
          break;
        case Key.J:
          if (this.activeSession != null || this.completionBroker == null)
            break;
          this.AsyncStartCompletionSession();
          break;
        case Key.Back:
          this.editorOperations.DeleteWordToLeft(this.undoHistory);
          e.Handled = true;
          break;
        case Key.Prior:
          this.textView.Caret.MoveTo((ITextLineMap) this.textView.RenderedTextLines.FirstFullyVisibleLine, 0.0);
          e.Handled = true;
          break;
        case Key.Next:
          this.textView.Caret.MoveTo((ITextLineMap) this.textView.RenderedTextLines.LastFullyVisibleLine, 0.0);
          e.Handled = true;
          break;
      }
    }

    private void HandleOnlyShiftKeyDown(KeyEventArgs e)
    {
      switch (e.Key)
      {
        case Key.Tab:
          if (!this.textView.Selection.IsEmpty)
            this.editorOperations.UnindentSelection(this.undoHistory);
          e.Handled = true;
          break;
        case Key.Insert:
          this.editorOperations.Paste(this.undoHistory);
          e.Handled = true;
          break;
        case Key.Delete:
          this.editorOperations.CutSelection(this.undoHistory);
          e.Handled = true;
          break;
      }
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
      this.DismissCompletionSession();
    }

    private CodeAidContext CreateCodeAidContext()
    {
      ITextSnapshot currentSnapshot = this.textView.TextBuffer.CurrentSnapshot;
      CompletionSelectionStatus status = this.activeSession != null ? this.activeSession.SelectionStatus : (CompletionSelectionStatus) null;
      ICompletion completion1 = status == null || !CompletionSessionExtensions.IsSelected(status) ? (ICompletion) null : this.activeSession.SelectionStatus.SelectedCompletion;
      ICompletion completion2 = status == null || !CompletionSessionExtensions.IsHollowSelected(status) ? (ICompletion) null : this.activeSession.SelectionStatus.SelectedCompletion;
      int position = this.LeftmostTextViewSelection();
      return new CodeAidContext()
      {
        CurrentPosition = new SnapshotPoint(currentSnapshot, position),
        SelectionSpan = this.textView.Selection.SelectionSpan,
        SessionStartingPosition = this.activeSessionStartPoint,
        SessionSelectedCompletion = completion1,
        SessionHollowSelectedCompletion = completion2
      };
    }

    private int LeftmostTextViewSelection()
    {
      if (this.textView.Selection.IsEmpty)
        return this.textView.Caret.Position.Index;
      SnapshotSpan selectionSpan = this.textView.Selection.SelectionSpan;
      return this.textView.Selection.SelectionSpan.Start;
    }

    private void AsyncStartCompletionSession()
    {
        this.editorElement.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback((object o) =>
        {
            ITrackingPoint trackingPoint = this.textView.TextBuffer.CurrentSnapshot.CreateTrackingPoint(this.textView.Caret.Position.Index, PointTrackingMode.Positive);
            if (this.activeSession == null)
            {
                ICompletionSession completionSession = this.completionBroker.TriggerCompletion(trackingPoint);
                ICompletionSession completionSession1 = completionSession;
                this.activeSession = completionSession;
                if (completionSession1 != null)
                {
                    this.InitializeActiveCompletionSession();
                }
            }
            return null;
        }), null);
    }

    private void AsyncMatchCompletionSession()
    {
        if (this.activeSession != null)
        {
            this.editorElement.Dispatcher.BeginInvoke(DispatcherPriority.Send, new DispatcherOperationCallback((object o) =>
            {
                if (this.activeSession == null || this.activeSession.IsDismissed)
                {
                    return null;
                }
                this.CompletionSessionMatchTrackingWorker();
                return null;
            }), null);
        }
    }

    private void CompletionSessionMatchTrackingWorker()
    {
      if (this.activeSession == null || this.activeSession.IsDismissed)
        return;
      this.activeSession.Match(CompletionMatchType.MatchDisplayText, false);
    }

    private void ActiveSession_Committed(object sender, EventArgs e)
    {
      ICompletionSession completionSession = sender as ICompletionSession;
      if (completionSession != null && CompletionSessionExtensions.IsSelected(completionSession.SelectionStatus))
      {
        ICodeAidCompletion codeAidCompletion = completionSession.SelectionStatus.SelectedCompletion as ICodeAidCompletion;
        if (codeAidCompletion != null)
        {
          int completionCaretDelta = codeAidCompletion.CompletionCaretDelta;
          bool flag = completionCaretDelta > 0;
          int num = Math.Abs(completionCaretDelta);
          for (int index = 0; index < num; ++index)
          {
            if (flag)
              this.editorOperations.MoveToNextCharacter(false);
            else
              this.editorOperations.MoveToPreviousCharacter(false);
          }
        }
      }
      this.OnActiveSessionClosed();
    }

    private void ActiveSession_Dismissed(object sender, EventArgs e)
    {
      this.OnActiveSessionClosed();
    }

    private void OnActiveSessionClosed()
    {
      this.activeSession.Committed -= new EventHandler(this.ActiveSession_Committed);
      this.activeSession.Dismissed -= new EventHandler(this.ActiveSession_Dismissed);
      this.activeSession = (ICompletionSession) null;
      this.activeSessionStartPoint = (ITrackingPoint) null;
    }

    private void CommitCompletionSession(bool shouldCommitHollowSelections)
    {
      if (this.activeSession == null || !CompletionSessionExtensions.IsSelected(this.activeSession.SelectionStatus) && (!shouldCommitHollowSelections || !CompletionSessionExtensions.IsHollowSelected(this.activeSession.SelectionStatus)))
        return;
      this.activeSession.Commit();
      this.textView.Selection.Clear();
      this.textView.Caret.EnsureVisible();
    }

    public void DismissCompletionSession()
    {
      if (this.activeSession == null)
        return;
      this.activeSession.Dismiss();
    }

    private void OffsetCompletionSelection(int offset)
    {
      if (this.activeSession == null)
        return;
      int count = this.activeSession.Completions.Count;
      if (count == 1)
      {
        this.activeSession.SetSelectionStatus(this.activeSession.Completions[0], CompletionSelectionOptions.Selected | CompletionSelectionOptions.Unique);
      }
      else
      {
        int num = this.activeSession.Completions.IndexOf(this.activeSession.SelectionStatus.SelectedCompletion);
        int index = num + offset;
        if (index < 0)
          index = 0;
        else if (index >= count)
          index = count - 1;
        if (index == num && !CompletionSessionExtensions.IsHollowSelected(this.activeSession.SelectionStatus))
          return;
        if (CompletionSessionExtensions.IsHollowSelected(this.activeSession.SelectionStatus))
          index = num;
        this.activeSession.SetSelectionStatus(this.activeSession.Completions[index], CompletionSelectionOptions.Selected | CompletionSelectionOptions.Unique);
      }
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool isDisposing)
    {
      if (!isDisposing)
        return;
      if (this.editorElement != null)
      {
        TextCompositionManager.RemoveTextInputStartHandler((DependencyObject) this.editorElement, new TextCompositionEventHandler(this.OnTextInputStart));
        TextCompositionManager.RemoveTextInputUpdateHandler((DependencyObject) this.editorElement, new TextCompositionEventHandler(this.OnTextInputUpdate));
        this.editorElement.TextInput -= new TextCompositionEventHandler(this.OnTextInput);
        this.editorElement.KeyDown -= new KeyEventHandler(this.OnKeyDown);
        this.editorElement.MouseDown -= new MouseButtonEventHandler(this.OnMouseDown);
        this.editorElement = (FrameworkElement) null;
      }
      if (this.codeAidEngine != null)
      {
        this.codeAidEngine.Dispose();
        this.codeAidEngine = (XamlCodeAidEngine) null;
      }
      if (this.undoHistory == null)
        return;
      this.undoHistory.UndoRedoHappened -= new EventHandler<UndoRedoEventArgs>(this.OnUndoRedoHappened);
      this.undoHistory = (UndoHistory) null;
    }
  }
}
