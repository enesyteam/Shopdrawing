// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Documents.CodeView
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using ActiproSoftware.SyntaxEditor.Addons.DotNet.Dom;
using Microsoft.Expression.Code;
using Microsoft.Expression.Code.Actipro;
using Microsoft.Expression.Code.UserInterface;
using Microsoft.Expression.DesignModel.Code;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project.Documents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace Microsoft.Expression.Code.Documents
{
  internal sealed class CodeView : DocumentView, IElementProvider, ISetCaretPosition
  {
    private CodeDocument codeDocument;
    private ActiproEditor actiproEditor;
    private IMessageDisplayService messageDisplayService;
    private IViewService viewService;
    private CodeOptionsModel codeOptionsModel;

    public FrameworkElement Element
    {
      get
      {
        return this.actiproEditor.Element;
      }
    }

    public override object ActiveEditor
    {
      get
      {
        return (object) this.actiproEditor.Element;
      }
    }

    internal CodeView(IDocument document, ICodeProject codeProject, IMessageDisplayService messageDisplayService, IViewService viewService, CodeOptionsModel codeOptionsModel, Microsoft.Expression.Framework.UserInterface.IWindowService windowService)
      : base(document)
    {
      Application.Current.Activated += new EventHandler(this.Application_Activated);
      this.codeDocument = (CodeDocument) document;
      this.messageDisplayService = messageDisplayService;
      this.viewService = viewService;
      this.codeOptionsModel = codeOptionsModel;
      this.actiproEditor = new ActiproEditor(this.codeDocument.Document, this.codeOptionsModel, this.messageDisplayService, this.viewService, codeProject, windowService);
      this.actiproEditor.CommandExecutionRequested += new EventHandler<CommandExecutionRequestedEventArgs>(this.ActiproEditor_CommandExecutionRequested);
      this.actiproEditor.EventInserted += new EventHandler<InsertEventHandlerEventArgs>(this.Editor_EventInserted);
      this.AddCommands();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        Application.Current.Activated -= new EventHandler(this.Application_Activated);
        if (this.actiproEditor != null)
        {
          this.actiproEditor.CommandExecutionRequested -= new EventHandler<CommandExecutionRequestedEventArgs>(this.ActiproEditor_CommandExecutionRequested);
          this.actiproEditor.EventInserted -= new EventHandler<InsertEventHandlerEventArgs>(this.Editor_EventInserted);
          this.actiproEditor.Dispose();
          this.actiproEditor = (ActiproEditor) null;
        }
      }
      base.Dispose(disposing);
    }

    public override void Activated()
    {
      base.Activated();
      this.codeDocument.ActivateEditing();
    }

    public override void Deactivated()
    {
      base.Deactivated();
      this.codeDocument.DeactivateEditing();
      this.actiproEditor.Deactivated();
    }

    public void SetCaretPosition(int line, int column)
    {
      --line;
      --column;
      this.ChangeCaretPosition(this.actiproEditor.GetStartOfLineFromLineNumber(line) + Math.Max(0, Math.Min(column, this.actiproEditor.GetLengthOfLineFromLineNumber(line))));
      this.actiproEditor.Focus();
    }

    public bool CreateMethod(IDomType domType, Type returnType, string methodName, IEnumerable<IParameterDeclaration> parameters)
    {
      return this.codeDocument.CreateMethod(domType, returnType, methodName, parameters, new CodeDocument.CaretPositionChanger(this.ChangeCaretPosition));
    }

    private bool ChangeCaretPosition(int offset)
    {
      this.actiproEditor.CaretPosition = offset;
      return true;
    }

    private void AddCommands()
    {
      foreach (DictionaryEntry dictionaryEntry in this.actiproEditor.GetEditCommands())
        this.AddCommand(dictionaryEntry.Key as string, (ICommand) (dictionaryEntry.Value as Command));
    }

    private void ActiproEditor_CommandExecutionRequested(object sender, CommandExecutionRequestedEventArgs e)
    {
      this.Execute(e.CommandName, CommandInvocationSource.ContextMenu);
    }

    private void Application_Activated(object sender, EventArgs e)
    {
      if (this.viewService == null || this.actiproEditor == null || this.viewService.ActiveView != this)
        return;
      this.actiproEditor.Focus();
    }

    private void Editor_EventInserted(object sender, InsertEventHandlerEventArgs e)
    {
      int caretPosition1 = this.actiproEditor.CaretPosition;
      this.CreateMethod(e.DomType, e.ReturnType, e.MethodName, e.Parameters);
      int caretPosition2 = this.actiproEditor.CaretPosition;
      if (caretPosition2 > caretPosition1)
        caretPosition2 += e.EventHandlerDeclaration.Length;
      this.actiproEditor.AutoInsertText(caretPosition1, e.EventHandlerDeclaration);
      this.actiproEditor.CaretPosition = caretPosition2;
    }

    public override void ReturnFocus()
    {
      UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.ApplicationIdle, (Action) (() =>
      {
        if (this.actiproEditor == null)
          return;
        this.actiproEditor.Focus();
      }));
    }
  }
}
