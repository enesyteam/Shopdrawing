// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Actipro.CSharpExtendedSyntaxLanguage
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using ActiproSoftware.SyntaxEditor;
using ActiproSoftware.SyntaxEditor.Addons.CSharp;
using ActiproSoftware.SyntaxEditor.Addons.DotNet.Context;
using ActiproSoftware.SyntaxEditor.Addons.DotNet.Dom;
using ActiproSoftware.SyntaxEditor.Commands;
using Microsoft.Expression.Code;
using Microsoft.Expression.DesignModel.Code;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Expression.Code.Actipro
{
  internal sealed class CSharpExtendedSyntaxLanguage : CSharpSyntaxLanguage
  {
    internal static readonly object SemanticParserSyncLock = new object();
    private ICodeProject codeProject;
    private bool isPotentiallyAddingEvent;
    private DotNetContext context;
    private IDomMember domMember;
    private DotNetProjectResolver resolver;
    private string methodName;
    private string eventHandlerTypeName;
    private IEnumerable<IParameterDeclaration> parameters;
    private Type returnType;

    public event EventHandler<InsertEventHandlerEventArgs> EventInserted;

    internal CSharpExtendedSyntaxLanguage(ICodeProject codeProject)
    {
      this.codeProject = codeProject;
    }

    protected override void OnSyntaxEditorKeyTyped(SyntaxEditor syntaxEditor, KeyTypedEventArgs e)
    {
      if (e.Command is IndentCommand)
        this.ProcessTabKey(syntaxEditor);
      else if (e.Command is TypingCommand)
      {
        switch (e.KeyChar)
        {
          case ' ':
            break;
          case '=':
            this.ProcessEqualsKey(syntaxEditor);
            break;
          default:
            this.isPotentiallyAddingEvent = false;
            break;
        }
      }
      else
        this.isPotentiallyAddingEvent = false;
      this.UpdateInternalState(syntaxEditor);
      base.OnSyntaxEditorKeyTyped(syntaxEditor, e);
    }

    protected override void OnSyntaxEditorKeyTyping(SyntaxEditor syntaxEditor, KeyTypingEventArgs e)
    {
      if (e.Command is IndentCommand && this.isPotentiallyAddingEvent)
      {
        e.Cancel = true;
        this.ProcessTabKey(syntaxEditor);
        this.UpdateInternalState(syntaxEditor);
      }
      else
      {
        this.UpdateInternalState(syntaxEditor);
        base.OnSyntaxEditorKeyTyping(syntaxEditor, e);
      }
    }

    internal static void ForceSynchronousReparse(Document document)
    {
      lock (CSharpExtendedSyntaxLanguage.SemanticParserSyncLock)
      {
        if (document == null)
          return;
        SemanticParserService.Stop();
        SemanticParserService.Parse(new SemanticParserServiceRequest(0, document, new ActiproSoftware.SyntaxEditor.TextRange(0, document.Length + 1), SemanticParseFlags.None, (ISemanticParserServiceProcessor) document.Language, (ISemanticParseDataTarget) document));
        SemanticParserService.Start();
      }
    }

    private void ProcessTabKey(SyntaxEditor syntaxEditor)
    {
      if (!this.isPotentiallyAddingEvent)
        return;
      if (this.EventInserted != null)
      {
        CSharpExtendedSyntaxLanguage.ForceSynchronousReparse(syntaxEditor.Document);
        DotNetContext context = this.GetContext(syntaxEditor, syntaxEditor.Caret.Offset, false, false);
        if (context != null)
          this.EventInserted((object) this, new InsertEventHandlerEventArgs(this.returnType, this.methodName, this.parameters, this.GetEventConstructor(), context.MemberDeclarationNode.DeclaringType.Resolve(this.resolver)));
      }
      this.isPotentiallyAddingEvent = false;
    }

    private void ProcessEqualsKey(SyntaxEditor syntaxEditor)
    {
      this.resolver = syntaxEditor.Document.LanguageData as DotNetProjectResolver;
      CSharpExtendedSyntaxLanguage.ForceSynchronousReparse(syntaxEditor.Document);
      if (!this.IsEventInsertion(syntaxEditor))
        return;
      this.isPotentiallyAddingEvent = this.InitializeEventData();
    }

    private bool IsEventInsertion(SyntaxEditor editor)
    {
      this.context = this.GetAdditionAssignmentContext(editor);
      if (!this.IsValidContext(this.context))
        return false;
      DotNetContextItem eventContextItem = this.GetEventContextItem(this.context);
      this.domMember = eventContextItem == null ? (IDomMember) null : eventContextItem.ResolvedInfo as IDomMember;
      return this.IsEventType(this.domMember);
    }

    private DotNetContext GetAdditionAssignmentContext(SyntaxEditor syntaxEditor)
    {
      if (syntaxEditor.Document.Tokens.Count == 0)
        return (DotNetContext) null;
      int additionAssignmentTokenIndex = syntaxEditor.Document.Tokens.IndexOf(syntaxEditor.Caret.Offset - 1);
      if (!syntaxEditor.Document.Tokens[additionAssignmentTokenIndex].Key.Equals("AdditionAssignment", StringComparison.OrdinalIgnoreCase))
        return (DotNetContext) null;
      int identifierOffset = this.GetEventIdentifierOffset(syntaxEditor.Document.Tokens, additionAssignmentTokenIndex);
      if (identifierOffset == -1)
        return (DotNetContext) null;
      return this.GetContext(syntaxEditor, identifierOffset, false, false);
    }

    private bool IsValidContext(DotNetContext context)
    {
      if (context != null)
        return context.MemberDeclarationNode != null;
      return false;
    }

    private int GetEventIdentifierOffset(TokenCollection tokens, int additionAssignmentTokenIndex)
    {
      if (this.IsIndexValid(tokens.Count, additionAssignmentTokenIndex - 2) && tokens[additionAssignmentTokenIndex - 1].Key.Equals("Identifier", StringComparison.OrdinalIgnoreCase))
        return tokens[additionAssignmentTokenIndex - 2].EndOffset;
      if (this.IsIndexValid(tokens.Count, additionAssignmentTokenIndex - 3) && tokens[additionAssignmentTokenIndex - 1].Key.Equals("Whitespace", StringComparison.OrdinalIgnoreCase) && tokens[additionAssignmentTokenIndex - 2].Key.Equals("Identifier", StringComparison.OrdinalIgnoreCase))
        return tokens[additionAssignmentTokenIndex - 3].EndOffset;
      return -1;
    }

    private DotNetContextItem GetEventContextItem(DotNetContext context)
    {
      if (this.IsIndexValid(context.Items.Length, context.Items.Length - 1))
        return context.Items[context.Items.Length - 1];
      return (DotNetContextItem) null;
    }

    private DotNetContextItem GetIdentifierContextItem(DotNetContext context)
    {
      if (context != null && this.IsIndexValid(context.Items.Length, context.Items.Length - 2))
        return context.Items[context.Items.Length - 2];
      return (DotNetContextItem) null;
    }

    private bool IsEventType(IDomMember domMember)
    {
      if (domMember != null)
        return domMember.MemberType == DomMemberType.Event;
      return false;
    }

    private bool InitializeEventData()
    {
      Type eventHandlerType = this.GetEventHandlerType(this.domMember);
      if (!(eventHandlerType != (Type) null))
        return false;
      this.methodName = this.CreateMethodName(this.context);
      this.eventHandlerTypeName = this.CreateEventHandlerTypeName(eventHandlerType);
      MethodInfo invokeMethod = this.FindInvokeMethod(eventHandlerType);
      this.parameters = this.CreateParameterList(invokeMethod);
      this.returnType = invokeMethod != (MethodInfo) null ? invokeMethod.ReturnType : (Type) null;
      return true;
    }

    private void UpdateInternalState(SyntaxEditor editor)
    {
      if (!this.isPotentiallyAddingEvent)
      {
        this.context = (DotNetContext) null;
        this.domMember = (IDomMember) null;
        this.resolver = (DotNetProjectResolver) null;
        this.methodName = (string) null;
        this.eventHandlerTypeName = (string) null;
        this.parameters = (IEnumerable<IParameterDeclaration>) null;
        this.returnType = (Type) null;
      }
      else
        this.ShowQuickInfo(editor);
    }

    private void ShowQuickInfo(SyntaxEditor syntaxEditor)
    {
      syntaxEditor.IntelliPrompt.QuickInfo.Show(syntaxEditor.Caret.Offset, this.CreateIntellipromptQuickInfoString());
    }

    private string CreateIntellipromptQuickInfoString()
    {
      return "<b>" + this.EscapeGenericArguments(this.GetEventConstructor()) + "</b> " + StringTable.AutoInsertEventHandlerText;
    }

    private string GetEventConstructor()
    {
      return "new " + this.eventHandlerTypeName + "(" + this.methodName + ");";
    }

    private string EscapeGenericArguments(string typeString)
    {
      return typeString.Replace("<", "&lt;").Replace(">", "&gt;");
    }

    private IEnumerable<IParameterDeclaration> CreateParameterList(MethodInfo invokeMethod)
    {
      IList<IParameterDeclaration> list = (IList<IParameterDeclaration>) new List<IParameterDeclaration>();
      if (invokeMethod != (MethodInfo) null)
      {
        foreach (ParameterInfo parameterInfo in invokeMethod.GetParameters())
          list.Add((IParameterDeclaration) new CSharpExtendedSyntaxLanguage.ParameterDeclaration(parameterInfo.ParameterType, parameterInfo.Name));
      }
      return (IEnumerable<IParameterDeclaration>) list;
    }

    private string CreateMethodName(DotNetContext context)
    {
      string str1 = "";
      DotNetContextItem identifierContextItem = this.GetIdentifierContextItem(context);
      string str2 = (identifierContextItem == null || identifierContextItem.Type == DotNetContextItemType.This ? str1 + this.context.TypeDeclarationNode.Name : str1 + identifierContextItem.Text) + "_";
      DotNetContextItem dotNetContextItem = context.Items[context.Items.Length - 1];
      if (dotNetContextItem != null)
        str2 += dotNetContextItem.Text;
      return str2;
    }

    private string CreateEventHandlerTypeName(Type eventHandlerType)
    {
      return TypeNameFormatter.FormatTypeForCSharp(eventHandlerType, true);
    }

    private Type GetEventHandlerType(IDomMember domMember)
    {
      if (domMember == null)
        return (Type) null;
      string typeName = this.BuildAssemblyQualifiedName(domMember.ReturnType);
      try
      {
        return Type.GetType(typeName);
      }
      catch (ArgumentException ex)
      {
      }
      return (Type) null;
    }

    private string BuildAssemblyQualifiedName(IDomTypeReference typeReference)
    {
      if (typeReference == null)
        return string.Empty;
      string fullName = typeReference.FullName;
      string str1 = typeReference.AssemblyHint != null ? typeReference.AssemblyHint : this.codeProject.FullyQualifiedAssemblyName;
      string str2 = "";
      if (typeReference.IsGenericType)
      {
        string str3 = str2 + "[";
        foreach (IDomTypeReference typeReference1 in (IEnumerable) typeReference.GenericTypeArguments)
        {
          str3 += "[";
          str3 += this.BuildAssemblyQualifiedName(typeReference1);
          str3 += "],";
        }
        str2 = str3.Substring(0, str3.Length - 1) + "]";
      }
      return fullName + str2 + "," + str1;
    }

    private MethodInfo FindInvokeMethod(Type eventHandlerType)
    {
      if (!(eventHandlerType != (Type) null))
        return (MethodInfo) null;
      return eventHandlerType.GetMethod("Invoke");
    }

    private bool IsIndexValid(int length, int index)
    {
      if (index >= 0)
        return index < length;
      return false;
    }

    private class ParameterDeclaration : IParameterDeclaration
    {
      public Type ParameterType { get; private set; }

      public string Name { get; private set; }

      public ParameterDeclaration(Type parameterType, string paramName)
      {
        this.ParameterType = parameterType;
        this.Name = paramName;
      }
    }
  }
}
