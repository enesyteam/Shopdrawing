// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Documents.CodeDocument
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using ActiproSoftware.SyntaxEditor;
using ActiproSoftware.SyntaxEditor.Addons.DotNet.Ast;
using ActiproSoftware.SyntaxEditor.Addons.DotNet.Dom;
using Microsoft.Expression.Code;
using Microsoft.Expression.Code.Actipro;
using Microsoft.Expression.Code.UserInterface;
using Microsoft.Expression.DesignModel.Code;
using Microsoft.Expression.DesignModel.Markup;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.Expression.Code.Documents
{
  internal sealed class CodeDocument : UndoDocument, ICodeGeneratorHost, ICommandTarget
  {
    private static bool semanticParseEnabled;
    private CodeDomProvider codeDomProvider;
    private Encoding encoding;
    private Stream stream;
    private CodeDocumentType codeDocumentType;
    private ICodeProject codeProject;
    private IProject project;
    private ActiproSoftware.SyntaxEditor.Document document;
    private string filename;
    private CodeOptionsModel codeOptionsModel;
    private ICodeProjectService codeProjectService;
    private IViewService viewService;
    private IWindowService windowService;

    public override bool IsDirty
    {
      get
      {
        if (base.IsDirty)
          return true;
        if (this.document != null)
          return this.document.Modified;
        return false;
      }
    }

    public ActiproSoftware.SyntaxEditor.Document Document
    {
      get
      {
        if (this.document == null)
        {
          try
          {
            if (!CodeDocument.semanticParseEnabled)
            {
              lock (CSharpExtendedSyntaxLanguage.SemanticParserSyncLock)
              {
                SemanticParserService.Start();
                CodeDocument.semanticParseEnabled = true;
              }
            }
            this.document = new ActiproSoftware.SyntaxEditor.Document();
            this.document.HeaderText = this.ConstructProjectGlobalImportsHeader();
            int num = (int) this.document.LoadFile(this.stream, this.encoding);
            this.document.Language = this.codeProjectService.GetSyntaxLanguage(this.filename);
            this.document.LanguageData = (object) this.codeProject.ProjectResolver;
            this.document.Filename = this.filename;
            this.document.ModifiedChanged += new EventHandler(this.OnModifiedChanged);
          }
          catch (OutOfMemoryException ex)
          {
            this.document = (ActiproSoftware.SyntaxEditor.Document) null;
            LowMemoryMessage.Show();
          }
          finally
          {
            this.stream.Close();
            this.stream = (Stream) null;
          }
        }
        return this.document;
      }
    }

    private Encoding TargetEncoding
    {
      get
      {
        return DocumentEncodingHelper.GetTargetEncoding(this.encoding);
      }
    }

    public CodeDomProvider CodeDomProvider
    {
      get
      {
        return this.codeDomProvider;
      }
    }

    internal CodeDocument(DocumentReference documentReference, bool isReadOnly, Encoding encoding, Stream stream, CodeDomProvider codeDomProvider, CodeDocumentType codeDocumentType, ICodeProjectService codeProjectService, ICodeProject codeProject, IProject project, IViewService viewService, IWindowService windowService)
      : base(documentReference, isReadOnly)
    {
      this.filename = documentReference.Path;
      this.codeDomProvider = codeDomProvider;
      this.codeDocumentType = codeDocumentType;
      this.codeOptionsModel = this.codeDocumentType.CodeOptionsModel;
      this.codeProjectService = codeProjectService;
      this.viewService = viewService;
      this.codeProject = codeProject;
      this.project = project;
      this.windowService = windowService;
      this.encoding = encoding;
      this.stream = stream;
    }

    public void ActivateEditing()
    {
      this.codeProject.ActivateEditing(this.DocumentReference);
    }

    public void DeactivateEditing()
    {
      this.codeProject.DeactivateEditing(this.DocumentReference);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.document != null)
        {
          this.document.Dispose();
          this.document = (ActiproSoftware.SyntaxEditor.Document) null;
        }
        if (this.stream != null)
        {
          this.stream.Close();
          this.stream = (Stream) null;
        }
      }
      base.Dispose(disposing);
    }

    public override IDocumentView CreateDefaultView()
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.CreateCodeEditor);
      CodeProject.EnsureResolverCachePruned(this.codeProject.ProjectResolver);
      IDocumentView documentView = (IDocumentView) new CodeView((IDocument) this, this.codeProject, this.codeProjectService.MessageDisplayService, this.viewService, this.codeOptionsModel, this.windowService);
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.CreateCodeEditor);
      return documentView;
    }

    protected override void SaveCore(Stream stream)
    {
      if (this.document != null)
      {
        this.document.SaveFile(stream, this.TargetEncoding, LineTerminator.CarriageReturnNewline);
        this.document.Modified = false;
      }
      else
      {
        byte[] buffer = new byte[(int) this.stream.Length];
        this.stream.Read(buffer, 0, buffer.Length);
        this.stream.Position = 0L;
        stream.Write(buffer, 0, buffer.Length);
      }
    }

    private void OnModifiedChanged(object sender, EventArgs e)
    {
      this.OnIsDirtyChanged(EventArgs.Empty);
    }

    internal bool CreateMethod(IDomType domType, Type returnType, string methodName, IEnumerable<IParameterDeclaration> parameters, CodeDocument.CaretPositionChanger UpdateCaretPosition)
    {
      int num = -1;
      string str1 = string.Empty;
      string str2 = string.Empty;
      string indent = string.Empty;
      foreach (IDomMember domMember in domType.GetMembers())
      {
        IAstNode astNode1 = domMember as IAstNode;
        if (domMember.Name == methodName)
        {
          MethodDeclaration method = domMember as MethodDeclaration;
          if (method != null)
          {
            List<IParameterDeclaration> list = new List<IParameterDeclaration>(parameters);
            if (list.Count == method.Parameters.Count)
            {
              foreach (IAstNode astNode2 in (IEnumerable) method.Parameters)
              {
                ActiproSoftware.SyntaxEditor.Addons.DotNet.Ast.ParameterDeclaration parameter = astNode2 as ActiproSoftware.SyntaxEditor.Addons.DotNet.Ast.ParameterDeclaration;
                if (parameter != null)
                {
                  for (int index = 0; index < list.Count; ++index)
                  {
                    if (this.AreParameterSignaturesEquivalent(parameter, list[index]))
                    {
                      list.RemoveAt(index);
                      break;
                    }
                  }
                }
              }
              if (list.Count == 0 && (method.ReturnType != null ? this.AreTypesEquivalent(returnType, method.ReturnType) : returnType.FullName.Equals("System.Void", StringComparison.OrdinalIgnoreCase)))
              {
                int offset = astNode1 != null ? this.FindMethodBodyIndex(method) : -1;
                return UpdateCaretPosition(offset);
              }
            }
          }
        }
        if (astNode1 != null && this.IsAstNodeValid(astNode1) && astNode1.EndOffset > num)
        {
          num = astNode1.EndOffset;
          indent = this.GetLeadingWhitespaceForLine(astNode1.StartOffset);
          str1 = "\r\n\r\n";
        }
      }
      if (num == -1)
      {
        foreach (IDomType domType1 in (IEnumerable) this.codeProject.ProjectResolver.SourceProjectContent.GetTypesForSourceKey(this.filename, false))
        {
          if (string.Equals(domType1.FullName, domType.FullName, StringComparison.Ordinal))
          {
            domType = domType1;
            break;
          }
        }
        IAstNode astNode = domType as IAstNode;
        if (astNode != null && this.IsAstNodeValid(astNode))
        {
          num = Math.Max(0, astNode.EndOffset - this.codeDocumentType.ClassEndToken.Length);
          string whitespaceForLine = this.GetLeadingWhitespaceForLine(num);
          str2 = "\r\n" + whitespaceForLine;
          indent = whitespaceForLine + "\t";
          str1 = "\r\n";
        }
      }
      if (num == -1)
        return false;
      int bodyInsertionOffset;
      string text = str1 + this.codeDocumentType.CreateMethod(indent, returnType, methodName, parameters, out bodyInsertionOffset) + str2;
      this.document.InsertText(DocumentModificationType.AutoFormat, num, text);
      return UpdateCaretPosition(num + bodyInsertionOffset);
    }

    private string ConstructProjectGlobalImportsHeader()
    {
      MSBuildBasedProject buildBasedProject = this.project as MSBuildBasedProject;
      if (buildBasedProject == null || this.codeDocumentType == null || !this.codeDocumentType.SupportsGlobalImports)
        return (string) null;
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string namespaceName in (IEnumerable<string>) buildBasedProject.GlobalImportsList)
      {
        string str = this.codeDocumentType.FormatUsingDirective(namespaceName);
        if (!string.IsNullOrEmpty(str))
          stringBuilder.AppendLine(str);
      }
      if (stringBuilder.Length <= 0)
        return (string) null;
      return stringBuilder.ToString();
    }

    private bool AreTypesEquivalent(Type unmatchedType, TypeReference actiproType)
    {
      IDomType domType = actiproType.Resolve(this.codeProject.ProjectResolver);
      if (!unmatchedType.IsGenericType)
        return actiproType.GenericTypeArguments.Count == 0 && domType != null && unmatchedType.FullName == domType.FullName;
      string b = TypeNameFormatter.FormatGenericTypeName(unmatchedType, true);
      if (!string.Equals(domType != null ? domType.FullName.Substring(0, domType.FullName.IndexOf('`')) : (string) null, b, StringComparison.Ordinal))
        return false;
      List<TypeReference> list1 = Enumerable.ToList<TypeReference>(Enumerable.OfType<TypeReference>((IEnumerable) actiproType.GenericTypeArguments));
      List<Type> list2 = new List<Type>((IEnumerable<Type>) unmatchedType.GetGenericArguments());
      if (list1.Count != list2.Count)
        return false;
      for (int index = 0; index < list2.Count; ++index)
      {
        if (!this.AreTypesEquivalent(list2[index], list1[index]))
          return false;
      }
      return true;
    }

    private bool AreParameterSignaturesEquivalent(ActiproSoftware.SyntaxEditor.Addons.DotNet.Ast.ParameterDeclaration parameter, IParameterDeclaration unmatched)
    {
      if (unmatched.Name != parameter.Name)
        return false;
      return this.AreTypesEquivalent(unmatched.ParameterType, parameter.ParameterType);
    }

    private string GetLeadingWhitespaceForLine(int characterIndex)
    {
      string text = this.document.Lines[this.document.Lines.IndexOf(characterIndex)].Text;
      int length = 0;
      while (length < text.Length && char.IsWhiteSpace(text[length]))
        ++length;
      if (length != 0)
        return text.Substring(0, length);
      return string.Empty;
    }

    private int FindMethodBodyIndex(MethodDeclaration method)
    {
      int num1 = method.BlockStartOffset != -1 ? method.BlockStartOffset + 1 : method.EndOffset;
      int num2 = -1;
      if (method.Statements.Count != 0)
        num2 = method.Statements[0].StartOffset;
      if (method.Comments.Count != 0 && (num2 == -1 || method.Comments[0].StartOffset < num2))
        num2 = method.Comments[0].StartOffset;
      if (num2 == -1)
        return num1;
      return num2;
    }

    private bool IsAstNodeValid(IAstNode astNode)
    {
      CompilationUnit compilationUnit = astNode.RootNode as CompilationUnit;
      if (compilationUnit == null)
        return false;
      int num = compilationUnit.ChildNodes.Count != 0 ? compilationUnit.ChildNodes[0].StartOffset : 0;
      if (compilationUnit.Length + num == this.document.Length)
        return compilationUnit.SourceKey.Equals(this.filename, StringComparison.OrdinalIgnoreCase);
      return false;
    }

    internal delegate bool CaretPositionChanger(int offset);
  }
}
