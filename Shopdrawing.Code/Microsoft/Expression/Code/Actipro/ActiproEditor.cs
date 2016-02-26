// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Actipro.ActiproEditor
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using ActiproSoftware.SyntaxEditor;
using ActiproSoftware.SyntaxEditor.Addons.CSharp;
using Microsoft.Expression.Code;
using Microsoft.Expression.Code.Documents;
using Microsoft.Expression.Code.UserInterface;
using Microsoft.Expression.DesignModel.Code;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace Microsoft.Expression.Code.Actipro
{
  internal class ActiproEditor : ITextEditor, IDisposable
  {
    private WindowsFormsHost host;
    private SyntaxEditor editor;
    private CSharpExtendedSyntaxLanguage cSharpLanguage;
    private CodeOptionsModel codeOptionsModel;
    private EditorSpecificOptionsModel editorSpecificOptionsModel;
    private IMessageDisplayService messageDisplayService;
    private IViewService viewService;
    private Microsoft.Expression.Framework.UserInterface.IWindowService windowService;
    private IDictionary editCommandDictionary;

    internal SyntaxEditor Editor
    {
      get
      {
        return this.editor;
      }
    }

    public int CaretPosition
    {
      get
      {
        return this.editor.Caret.Offset;
      }
      set
      {
        this.editor.Caret.Offset = value;
        if (this.CaretPositionChanged == null)
          return;
        this.CaretPositionChanged((object) this, EventArgs.Empty);
      }
    }

    public string Text
    {
      get
      {
        return this.editor.Document.GetText(LineTerminator.Newline);
      }
    }

    public int LineCount
    {
      get
      {
        return this.editor.Document.Lines.Count;
      }
    }

    public bool IsSelectionEmpty
    {
      get
      {
        return this.editor.SelectedView.Selection.IsZeroLength;
      }
    }

    public int SelectionStart
    {
      get
      {
        return this.editor.SelectedView.Selection.FirstOffset;
      }
    }

    public int SelectionLength
    {
      get
      {
        return this.editor.SelectedView.Selection.Length;
      }
    }

    public bool CanUndo
    {
      get
      {
        return this.editor.Document.UndoRedo.CanUndo;
      }
    }

    public bool CanRedo
    {
      get
      {
        return this.editor.Document.UndoRedo.CanRedo;
      }
    }

    public int SquiggleCount
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public FrameworkElement Element
    {
      get
      {
        return (FrameworkElement) this.host;
      }
    }

    public Thickness VerticalScrollBarMargin
    {
      get
      {
        throw new NotImplementedException();
      }
      set
      {
        throw new NotImplementedException();
      }
    }

    public string FontName
    {
      get
      {
        return this.editorSpecificOptionsModel.FontFamily;
      }
    }

    public double FontSize
    {
      get
      {
        return this.editorSpecificOptionsModel.FontSize;
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

    public event EventHandler<CommandExecutionRequestedEventArgs> CommandExecutionRequested;

    public event EventHandler<InsertEventHandlerEventArgs> EventInserted;

    public event EventHandler GotFocus;

    public event EventHandler LostFocus;

    public event EventHandler CaretPositionChanged;

    public ActiproEditor(ActiproSoftware.SyntaxEditor.Document document, CodeOptionsModel codeOptionsModel, IMessageDisplayService messageDisplayService, IViewService viewService, ICodeProject codeProject, Microsoft.Expression.Framework.UserInterface.IWindowService windowService)
    {
      this.editor = new SyntaxEditor();
      this.InitializeEditor(document, codeProject);
      this.host = new WindowsFormsHost();
      this.host.Background = (System.Windows.Media.Brush) System.Windows.Media.Brushes.White;
      this.editor.IndicatorMarginVisible = false;
      this.host.Child = (Control) this.editor;
      this.codeOptionsModel = codeOptionsModel;
      this.codeOptionsModel.PropertyChanged += new PropertyChangedEventHandler(this.Actipro_PropertyChanged);
      this.editorSpecificOptionsModel = this.codeOptionsModel.GetEditorModel(EditorType.CodeEditor);
      this.editorSpecificOptionsModel.PropertyChanged += new PropertyChangedEventHandler(this.Actipro_PropertyChanged);
      this.messageDisplayService = messageDisplayService;
      this.viewService = viewService;
      this.windowService = windowService;
      this.windowService.ThemeChanged += new EventHandler(this.WindowService_ThemeChanged);
      this.UpdateOptions();
    }

    private void OnEventInserted(object sender, InsertEventHandlerEventArgs e)
    {
      if (this.EventInserted == null)
        return;
      this.EventInserted((object) this, e);
    }

    public void Deactivated()
    {
      this.HideIntellisensePrompt();
    }

    private void HideIntellisensePrompt()
    {
      this.editor.IntelliPrompt.ParameterInfo.Hide();
      if (!this.editor.IntelliPrompt.MemberList.Visible)
        return;
      this.editor.IntelliPrompt.MemberList.Abort();
    }

    internal void AutoInsertText(int caretPosition, string text)
    {
      this.editor.Document.InsertText(DocumentModificationType.AutoReplace, caretPosition, text);
    }

    public void Activated()
    {
      throw new NotImplementedException();
    }

    public void Focus()
    {
      if (this.host == null)
        return;
      this.host.Focus();
      if (this.editor == null)
        return;
      this.editor.Focus();
    }

    public int GetLengthOfLineFromLineNumber(int value)
    {
      this.ClampToLineCount(ref value);
      return this.editor.Document.Lines[value].Length;
    }

    public int GetLineNumberFromPosition(int value)
    {
      this.ClampToLength(ref value);
      return this.editor.Document.Lines.IndexOf(value);
    }

    public int GetLineOffsetFromPosition(int value)
    {
      this.ClampToLength(ref value);
      return value - this.editor.Document.Lines[this.editor.Document.Lines.IndexOf(value)].StartOffset;
    }

    public int GetStartOfLineFromLineNumber(int value)
    {
      this.ClampToLineCount(ref value);
      return this.editor.Document.Lines[value].StartOffset;
    }

    public int GetStartOfNextLineFromPosition(int value)
    {
      this.ClampToLength(ref value);
      return this.editor.Document.Lines[this.editor.Document.Lines.IndexOf(value)].EndOffset + 1;
    }

    public string GetText(int offset, int length)
    {
      return this.editor.Document.GetSubstring(offset, length, LineTerminator.Newline);
    }

    public void Select(int start, int length)
    {
      this.editor.SelectedView.Selection.SelectRange(start, length);
    }

    public void ClearSelection()
    {
      this.editor.SelectedView.Selection.Collapse();
    }

    public void EnsureSpanVisible(int start, int length)
    {
      this.editor.SelectedView.EnsureVisible(start, true);
    }

    public void EnsureCaretVisible()
    {
      this.editor.SelectedView.EnsureVisible(this.editor.Caret.Offset, true);
    }

    public void MoveLineToCenterOfView(int lineNumber)
    {
      this.ClampToLineCount(ref lineNumber);
      this.editor.SelectedView.FirstVisibleDisplayLineIndex = Math.Max(0, lineNumber - this.editor.SelectedView.DisplayLines.Count / 2);
    }

    public void GotoLine(int lineNumber)
    {
      this.editor.SelectedView.GoToLine(lineNumber);
    }

    public void SelectCurrentWord()
    {
      this.editor.SelectedView.Selection.SelectWord();
    }

    public int Find(string findText, bool matchCase, bool searchReverse)
    {
      if (this.editor == null)
        return -1;
      FindReplaceResultSet replaceResultSet = this.editor.SelectedView.FindReplace.Find(new FindReplaceOptions()
      {
        ChangeSelection = true,
        FindText = findText,
        MatchCase = matchCase,
        SearchUp = searchReverse
      });
      if (replaceResultSet.Count == 0)
        return -1;
      return replaceResultSet[0].StartOffset;
    }

    public int Replace(string findText, string replaceText, bool matchCase)
    {
      if (this.editor == null)
        return -1;
      FindReplaceResultSet replaceResultSet = this.editor.SelectedView.FindReplace.Replace(new FindReplaceOptions()
      {
        ChangeSelection = true,
        FindText = findText,
        MatchCase = matchCase,
        ReplaceText = replaceText
      });
      if (replaceResultSet.Count == 0)
        return -1;
      return replaceResultSet[0].StartOffset;
    }

    public bool ReplaceAll(string findText, string replaceText, bool matchCase)
    {
      if (this.editor == null)
        return false;
      return this.editor.SelectedView.FindReplace.ReplaceAll(new FindReplaceOptions()
      {
        FindText = findText,
        MatchCase = matchCase,
        ReplaceText = replaceText
      }).Count != 0;
    }

    public TextEditorErrorInformation GetSquiggleInformation(int squiggleIndex)
    {
      throw new NotImplementedException();
    }

    public void UpdateErrorRanges(List<TextEditorErrorInformation> errorRanges)
    {
      throw new NotImplementedException();
    }

    public IDictionary GetEditCommands()
    {
      if (this.editCommandDictionary == null)
        this.InitializeEditCommands();
      return this.editCommandDictionary;
    }

    public IDictionary GetUndoCommands()
    {
      throw new NotImplementedException();
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void Dispose(bool isDispoing)
    {
      if (!isDispoing)
        return;
      this.codeOptionsModel.PropertyChanged -= new PropertyChangedEventHandler(this.Actipro_PropertyChanged);
      this.editorSpecificOptionsModel.PropertyChanged -= new PropertyChangedEventHandler(this.Actipro_PropertyChanged);
      this.windowService.ThemeChanged -= new EventHandler(this.WindowService_ThemeChanged);
      if (this.cSharpLanguage != null)
      {
        this.cSharpLanguage.EventInserted -= new EventHandler<InsertEventHandlerEventArgs>(this.OnEventInserted);
        this.cSharpLanguage.Dispose();
      }
      if (this.editor != null)
      {
        this.editor.GotFocus -= new EventHandler(this.OnEditorGotFocus);
        this.editor.LostFocus -= new EventHandler(this.OnEditorLostFocus);
        this.editor.ContextMenuRequested -= new ContextMenuRequestEventHandler(this.Editor_ContextMenuRequested);
        if (this.editor.Renderer != null)
          this.editor.Renderer.Dispose();
        this.editor.Dispose();
        this.editor = (SyntaxEditor) null;
      }
      if (this.host == null)
        return;
      this.host.Dispose();
      this.host = (WindowsFormsHost) null;
    }

    private void WindowService_ThemeChanged(object sender, EventArgs e)
    {
      this.SetEditorTheme();
    }

    private void SetEditorTheme()
    {
      string themeImagePrefix = ((StringWrapper) System.Windows.Application.Current.TryFindResource((object) "ThemeImagePrefix")).Value;
      if (this.editor == null)
        return;
      if (this.editor.Renderer != null)
        this.editor.Renderer.Dispose();
      this.editor.Renderer = (ISyntaxEditorRenderer) new ActiproSyntaxEditorRenderer(themeImagePrefix);
    }

    private void InitializeEditor(ActiproSoftware.SyntaxEditor.Document document, ICodeProject codeProject)
    {
      this.editor.LineNumberMarginVisible = true;
      this.editor.BracketHighlightingVisible = true;
      this.editor.IndentationGuidesVisible = true;
      this.editor.AllowDrag = true;
      this.editor.AllowDrop = true;
      this.editor.SplitType = SyntaxEditorSplitType.None;
      this.editor.IndentType = IndentType.Smart;
      this.editor.Document = document;
      if (document != null)
      {
        this.editor.Document.SemanticParsingEnabled = true;
        this.editor.Document.LexicalParsingEnabled = true;
      }
      else
        this.editor.Enabled = false;
      this.SetEditorTheme();
      this.editor.ContextMenuRequested += new ContextMenuRequestEventHandler(this.Editor_ContextMenuRequested);
      this.editor.GotFocus += new EventHandler(this.OnEditorGotFocus);
      this.editor.LostFocus += new EventHandler(this.OnEditorLostFocus);
      if (this.editor.Document.Language is CSharpSyntaxLanguage)
      {
        this.cSharpLanguage = new CSharpExtendedSyntaxLanguage(codeProject);
        this.editor.Document.Language = (SyntaxLanguage) this.cSharpLanguage;
        this.cSharpLanguage.EventInserted += new EventHandler<InsertEventHandlerEventArgs>(this.OnEventInserted);
        this.editor.IntelliPrompt.QuickInfo.HideOnMouseMove = false;
      }
      else if (this.editor.Document.Language.Key.Equals("JScript", StringComparison.OrdinalIgnoreCase))
        this.editor.Document.Language.LineCommentDelimiter = "//";
      this.editor.KeyTyping += new KeyTypingEventHandler(this.Editor_KeyTyping);
    }

    private void Editor_KeyTyping(object sender, KeyTypingEventArgs e)
    {
      if ((e.KeyData & Keys.KeyCode) == Keys.Tab && ((e.KeyData & Keys.Modifiers) == Keys.Control || (e.KeyData & Keys.Modifiers) == (Keys.Shift | Keys.Control)))
      {
        e.Cancel = true;
        this.windowService.ShowSwitchToDialog();
      }
      if ((e.KeyData & Keys.Modifiers & Keys.Control) != Keys.Control)
        return;
      this.HideIntellisensePrompt();
    }

    private void UpdateOptions()
    {
      try
      {
        this.editor.Font = new Font(this.editorSpecificOptionsModel.FontFamily, (float) this.editorSpecificOptionsModel.FontSize);
      }
      catch
      {
        this.editor.Font = new Font(EditorSpecificOptionsModel.GetDefaultFont(), (float) this.editorSpecificOptionsModel.FontSize);
      }
      this.editor.Document.AutoConvertTabsToSpaces = this.editorSpecificOptionsModel.ConvertTabsToSpace;
      this.editor.Document.TabSize = this.editorSpecificOptionsModel.TabSize;
      this.editor.WordWrap = this.editorSpecificOptionsModel.WordWrap ? WordWrapType.Token : WordWrapType.None;
    }

    private bool ClampToLineCount(ref int line)
    {
      return ActiproEditor.Clamp(ref line, Math.Max(0, this.editor.Document.Lines.Count - 1));
    }

    private bool ClampToLength(ref int offset)
    {
      return ActiproEditor.Clamp(ref offset, Math.Max(0, this.editor.Document.Length - 1));
    }

    private static bool Clamp(ref int value, int maxValue)
    {
      if (value <= maxValue)
        return false;
      value = maxValue;
      return true;
    }

    private void OnEditorGotFocus(object sender, EventArgs e)
    {
      if (this.GotFocus == null)
        return;
      this.GotFocus((object) this, EventArgs.Empty);
    }

    private void OnEditorLostFocus(object sender, EventArgs e)
    {
      if (this.LostFocus == null)
        return;
      this.LostFocus((object) this, EventArgs.Empty);
    }

    private void Actipro_PropertyChanged(object sender, EventArgs e)
    {
      this.UpdateOptions();
    }

    private void Editor_ContextMenuRequested(object sender, ContextMenuRequestEventArgs e)
    {
      if (this.editor.ContextMenu == null)
        this.editor.ContextMenu = this.CreateContextMenuSkeleton();
      this.UpdateContextMenu();
    }

    private ContextMenu CreateContextMenuSkeleton()
    {
      ContextMenu contextMenu = new ContextMenu();
      MenuItem menuItem1 = new MenuItem(StringTable.ContextMenuCut, new EventHandler(this.ContextMenuClick));
      menuItem1.Name = "Edit_Cut";
      contextMenu.MenuItems.Add(menuItem1);
      MenuItem menuItem2 = new MenuItem(StringTable.ContextMenuCopy, new EventHandler(this.ContextMenuClick));
      menuItem2.Name = "Edit_Copy";
      contextMenu.MenuItems.Add(menuItem2);
      MenuItem menuItem3 = new MenuItem(StringTable.ContextMenuPaste, new EventHandler(this.ContextMenuClick));
      menuItem3.Name = "Edit_Paste";
      contextMenu.MenuItems.Add(menuItem3);
      MenuItem menuItem4 = new MenuItem(StringTable.ContextMenuDelete, new EventHandler(this.ContextMenuClick));
      menuItem4.Name = "Edit_Delete";
      contextMenu.MenuItems.Add(menuItem4);
      contextMenu.MenuItems.Add(new MenuItem("-"));
      MenuItem menuItem5 = new MenuItem(StringTable.ContextMenuSelectAll, new EventHandler(this.ContextMenuClick));
      menuItem5.Name = "Edit_SelectAll";
      contextMenu.MenuItems.Add(menuItem5);
      MenuItem menuItem6 = new MenuItem(StringTable.ContextMenuCommentSelection, new EventHandler(this.ContextMenuClick));
      menuItem6.Name = "Edit_CommentLines";
      contextMenu.MenuItems.Add(menuItem6);
      MenuItem menuItem7 = new MenuItem(StringTable.ContextMenuUncommentSelection, new EventHandler(this.ContextMenuClick));
      menuItem7.Name = "Edit_UncommentLines";
      contextMenu.MenuItems.Add(menuItem7);
      return contextMenu;
    }

    private void ContextMenuClick(object sender, EventArgs e)
    {
      MenuItem menuItem = sender as MenuItem;
      if (menuItem == null || this.CommandExecutionRequested == null)
        return;
      this.CommandExecutionRequested((object) this, new CommandExecutionRequestedEventArgs(menuItem.Name));
    }

    private void UpdateContextMenu()
    {
      foreach (MenuItem menuItem in this.editor.ContextMenu.MenuItems)
      {
        if (this.editCommandDictionary != null)
        {
          Command command = this.editCommandDictionary[(object) menuItem.Name] as Command;
          if (command != null)
          {
            menuItem.Enabled = command.IsEnabled;
            continue;
          }
        }
        menuItem.Enabled = false;
      }
    }

    private void InitializeEditCommands()
    {
      Hashtable hashtable = new Hashtable();
      hashtable.Add((object) "Edit_Cut", (object) new CutCommand(this));
      hashtable.Add((object) "Edit_Copy", (object) new CopyCommand(this));
      hashtable.Add((object) "Edit_Paste", (object) new PasteCommand(this));
      hashtable.Add((object) "Edit_Delete", (object) new DeleteCommand(this));
      hashtable.Add((object) "Edit_SelectAll", (object) new SelectAllCommand(this));
      hashtable.Add((object) "Edit_SelectNone", (object) new SelectNoneCommand(this));
      hashtable.Add((object) "Edit_Undo", (object) new UndoCommand(this));
      hashtable.Add((object) "Edit_Redo", (object) new RedoCommand(this));
      hashtable.Add((object) "Edit_GoToLine", (object) new GoToLineCommand(this));
      hashtable.Add((object) "Edit_CommentLines", (object) new CommentLinesCommand(this, this.viewService));
      hashtable.Add((object) "Edit_UncommentLines", (object) new UncommentLinesCommand(this, this.viewService));
      if (this.messageDisplayService != null)
      {
        hashtable.Add((object) "Edit_Find", (object) new FindCommand((ITextEditor) this, this.messageDisplayService));
        hashtable.Add((object) "Edit_FindNext", (object) new FindNextCommand((ITextEditor) this, this.messageDisplayService));
        hashtable.Add((object) "Edit_Replace", (object) new ReplaceCommand((ITextEditor) this, this.messageDisplayService));
      }
      this.editCommandDictionary = (IDictionary) hashtable;
    }
  }
}
