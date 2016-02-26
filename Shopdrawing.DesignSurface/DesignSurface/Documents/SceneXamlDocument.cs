// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Documents.SceneXamlDocument
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Markup;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.Text;
using Microsoft.Expression.Framework.Commands.Undo;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.Expression.DesignSurface.Documents
{
  public sealed class SceneXamlDocument : XamlDocument
  {
    private Stack<IUndoTransaction> transactions = new Stack<IUndoTransaction>();
    private readonly IUndoService undoService;

    public override string Text
    {
      get
      {
        return base.Text;
      }
      set
      {
        this.OnRootNodeChangingOutsideUndo();
        this.InvokeOrAddUndo((IUndoUnit) new SceneXamlDocument.SetTextUndoUnit(this, value));
        this.OnRootNodeChangedOutsideUndo();
      }
    }

    public IProjectContext ProjectContext { get; private set; }

    public override IEnumerable<IDocumentRoot> DesignTimeResources
    {
      get
      {
        List<IDocumentRoot> list = new List<IDocumentRoot>();
        foreach (SceneDocument sceneDocument in SceneDocument.GetDesignTimeResourceDocuments(this.ProjectContext))
          list.Add(sceneDocument.DocumentRoot);
        return (IEnumerable<IDocumentRoot>) list;
      }
    }

    public IUndoService UndoService
    {
      get
      {
        return this.undoService;
      }
    }

    protected override bool IsUndoing
    {
      get
      {
        if (this.undoService == null)
          return false;
        if (!this.undoService.IsUndoing)
          return this.undoService.IsRedoing;
        return true;
      }
    }

    public bool AllErrorsFromMissingAssemblies
    {
      get
      {
        if (this.ParseErrorsCount > 0)
        {
          for (int index = 0; index < this.ParseErrorsAndWarnings.Count; ++index)
          {
            if (!SceneXamlDocument.IsErrorFromMissingAssembly((ITypeResolver) this.ProjectContext, this.ParseErrorsAndWarnings[index]))
              return false;
          }
        }
        else if (this.UnresolvedTypes.Count > 0)
        {
          foreach (IType type in (IEnumerable<IType>) this.UnresolvedTypes)
          {
            IAssembly runtimeAssembly = type.RuntimeAssembly;
            if (runtimeAssembly == null || runtimeAssembly.IsLoaded)
              return false;
          }
        }
        return true;
      }
    }

    public SceneXamlDocument(IDocumentContext documentContext, IUndoService undoService, ITextBufferService textBufferService)
      : this(documentContext, undoService, PlatformTypes.Object, textBufferService.CreateTextBuffer(), DocumentEncodingHelper.DefaultEncoding)
    {
    }

    public SceneXamlDocument(IDocumentContext documentContext, IUndoService undoService, ITypeId expectedRootType, ITextBuffer textBuffer, Encoding documentEncoding)
      : base(documentContext, expectedRootType, textBuffer, documentEncoding, (IXamlSerializerFilter) new DefaultXamlSerializerFilter())
    {
      this.undoService = undoService;
      this.ProjectContext = (IProjectContext) documentContext.TypeResolver;
    }

    public static bool IsErrorFromMissingAssembly(ITypeResolver typeResolver, XamlParseError parseError)
    {
      if (parseError.ErrorCode == 8 && parseError.Parameters.Count == 2)
      {
        string mapping = parseError.Parameters[0];
        string clrNamespace = (string) null;
        string assemblyName = (string) null;
        if (XamlParser.TryParseClrNamespaceUri(mapping, out clrNamespace, out assemblyName))
        {
          IAssembly assembly = assemblyName != null ? typeResolver.GetAssembly(assemblyName) : typeResolver.ProjectAssembly;
          if (assembly != null && !assembly.IsLoaded)
            return true;
        }
      }
      return false;
    }

    protected override void SetParseErrors(IList<XamlParseError> errors)
    {
      this.InvokeOrAddUndo((IUndoUnit) new SceneXamlDocument.SetParseErrorsUndoUnit(this, errors));
    }

    protected override void SetRootNode(DocumentNode rootNode)
    {
      this.InvokeOrAddUndo((IUndoUnit) new SceneXamlDocument.SetRootNodeUndoUnit(this, rootNode));
    }

    protected override void ModifyRootXmlAttributes(XamlDocument.ModifyRootXmlAttributesUndoable undoUnit)
    {
      this.InvokeOrAddUndo((IUndoUnit) new SceneXamlDocument.ModifyRootXmlAttributesUndoUnit(undoUnit));
    }

    protected override void ApplyNamescopeUpdate(Dictionary<DocumentNodeNameScope, List<DocumentNode>> actions, bool isRemoving)
    {
      this.InvokeOrAddUndo((IUndoUnit) new SceneXamlDocument.ApplyNamescopeChangesUndoUnit(new XamlDocument.ApplyNamescopeUpdateUndoable(actions, isRemoving)));
    }

    public override void SetSourceContext(DocumentNode node, INodeSourceContext sourceContext)
    {
      if (this.undoService != null)
        this.undoService.Add((IUndoUnit) new SceneXamlDocument.SourceContextUndoUnit(node, sourceContext));
      else
        base.SetSourceContext(node, sourceContext);
    }

    public override void SetContainerSourceContext(DocumentNode node, INodeSourceContext sourceContext)
    {
      if (this.undoService != null)
        this.undoService.Add((IUndoUnit) new SceneXamlDocument.ContainerSourceContextUndoUnit(node, sourceContext));
      else
        base.SetContainerSourceContext(node, sourceContext);
    }

    public override void ApplyPropertyChange(DocumentCompositeNode node, IProperty propertyKey, SourceContextContainer<DocumentNode> oldValue, SourceContextContainer<DocumentNode> newValue)
    {
      if (this.undoService != null)
        this.undoService.Add((IUndoUnit) new SceneXamlDocument.PropertyDictionaryChangeUndoUnit(node, propertyKey, oldValue, newValue));
      else
        base.ApplyPropertyChange(node, propertyKey, oldValue, newValue);
    }

    public override void ApplyChildrenChange(DocumentCompositeNode node, int index, DocumentNode oldChildNode, DocumentNode newChildNode)
    {
      if (this.undoService != null)
        this.undoService.Add((IUndoUnit) new SceneXamlDocument.DocumentNodeCollectionChangeUndoUnit(node, index, oldChildNode, newChildNode));
      else
        base.ApplyChildrenChange(node, index, oldChildNode, newChildNode);
    }

    protected override bool ShouldTrackTextChange()
    {
      if (base.ShouldTrackTextChange())
        return !this.IsUndoingTextChange;
      return false;
    }

    private void InvokeOrAddUndo(IUndoUnit undo)
    {
      if (this.undoService != null)
        this.undoService.Add(undo);
      else
        undo.Redo();
    }

    protected override void FireRootChangedNotifications()
    {
      base.FireRootChangedNotifications();
      this.OnPropertyChanged("AllErrorsFromMissingAssemblies");
    }

    protected override void PushTransaction()
    {
      if (this.undoService != null)
        this.transactions.Push(this.undoService.CreateUndo("IncSerialize", true));
      else
        base.PushTransaction();
    }

    protected override void PopTransaction(bool commit)
    {
      if (this.undoService != null)
      {
        IUndoTransaction undoTransaction = this.transactions.Pop();
        if (commit)
          undoTransaction.Commit();
        else
          undoTransaction.Cancel();
      }
      else
        base.PopTransaction(commit);
    }

    protected override void ClearUndoStack()
    {
      base.ClearUndoStack();
      if (this.undoService == null)
        return;
      this.undoService.Clear();
    }

    public override string ToString()
    {
      if (this.DocumentContext == null)
        return base.ToString();
      string str = this.DocumentContext.DocumentUrl;
      if (!string.IsNullOrEmpty(this.ProjectContext.ProjectPath))
      {
        string directoryName = Path.GetDirectoryName(this.ProjectContext.ProjectPath);
        if (!string.IsNullOrEmpty(directoryName) && !string.IsNullOrEmpty(str) && str.StartsWith(directoryName, StringComparison.OrdinalIgnoreCase))
          str = str.Substring(directoryName.Length);
      }
      return str;
    }

    private abstract class DocumentUndoUnit : UndoUnit
    {
      protected SceneXamlDocument document;

      protected DocumentUndoUnit(SceneXamlDocument document)
      {
        this.document = document;
      }

      public abstract void Invoke();

      public override void Undo()
      {
        this.Invoke();
        base.Undo();
      }

      public override void Redo()
      {
        this.Invoke();
        base.Redo();
      }
    }

    private sealed class SetParseErrorsUndoUnit : SceneXamlDocument.DocumentUndoUnit
    {
      private IList<XamlParseError> parseErrors;

      public SetParseErrorsUndoUnit(SceneXamlDocument document, IList<XamlParseError> parseErrors)
        : base(document)
      {
        this.parseErrors = parseErrors;
      }

      public override void Invoke()
      {
        IList<XamlParseError> errorsAndWarnings = this.document.ParseErrorsAndWarnings;
        this.document.ParseErrorsAndWarnings = this.parseErrors;
        this.parseErrors = errorsAndWarnings;
        if (errorsAndWarnings.Count == 0 && this.document.ParseErrorsAndWarnings.Count == 0)
          return;
        this.document.FireRootChangedNotifications();
      }
    }

    private sealed class ApplyNamescopeChangesUndoUnit : UndoUnit
    {
      private XamlDocument.ApplyNamescopeUpdateUndoable undoUnit;

      public ApplyNamescopeChangesUndoUnit(XamlDocument.ApplyNamescopeUpdateUndoable undoUnit)
      {
        this.undoUnit = undoUnit;
      }

      public override void Undo()
      {
        this.undoUnit.Invoke();
      }

      public override void Redo()
      {
        this.undoUnit.Invoke();
      }
    }

    private sealed class SetRootNodeUndoUnit : SceneXamlDocument.DocumentUndoUnit
    {
      private DocumentNode node;

      public SetRootNodeUndoUnit(SceneXamlDocument document, DocumentNode node)
        : base(document)
      {
        this.node = node;
      }

      public override void Invoke()
      {
        DocumentNode rootNode = this.document.RootNode;
        SceneXamlDocument.SetRootNodeUndoUnit.ChangeDocumentRoot(this.document, this.node);
        this.node = rootNode;
      }

      public static void ChangeDocumentRoot(SceneXamlDocument document, DocumentNode node)
      {
        document.OnRootNodeChanging();
        document.SetDocumentRoot(node);
        document.OnRootNodeChanged();
      }

      public override string ToString()
      {
        return "SetRootNode: " + (this.node != null ? this.node.ToString() : "[null]");
      }
    }

    private sealed class SetTextUndoUnit : SceneXamlDocument.DocumentUndoUnit
    {
      private XamlDocument.SetTextUndoable undo;

      public SetTextUndoUnit(SceneXamlDocument document, string text)
        : base(document)
      {
        this.undo = new XamlDocument.SetTextUndoable((XamlDocument) document, text);
      }

      public override void Invoke()
      {
        this.undo.Invoke();
      }
    }

    private class SourceContextUndoUnit : UndoUnit
    {
      private INodeSourceContext sourceContext;
      private DocumentNode documentNode;

      public SourceContextUndoUnit(DocumentNode documentNode, INodeSourceContext sourceContext)
      {
        this.sourceContext = sourceContext;
        this.documentNode = documentNode;
      }

      public override void Undo()
      {
        this.Invoke();
        base.Undo();
      }

      public override void Redo()
      {
        this.Invoke();
        base.Redo();
      }

      private void Invoke()
      {
        INodeSourceContext sourceContext = this.documentNode.SourceContext;
        this.documentNode.SourceContext = this.sourceContext;
        this.sourceContext = sourceContext;
      }
    }

    private class ContainerSourceContextUndoUnit : UndoUnit
    {
      private INodeSourceContext containerSourceContext;
      private DocumentNode documentNode;

      public ContainerSourceContextUndoUnit(DocumentNode documentNode, INodeSourceContext containerSourceContext)
      {
        this.containerSourceContext = containerSourceContext;
        this.documentNode = documentNode;
      }

      public override void Undo()
      {
        this.Invoke();
        base.Undo();
      }

      public override void Redo()
      {
        this.Invoke();
        base.Redo();
      }

      private void Invoke()
      {
        this.documentNode.ClearOldSourceContainerContext();
        INodeSourceContext containerSourceContext = this.documentNode.ContainerSourceContext;
        this.documentNode.ContainerSourceContext = this.containerSourceContext;
        this.containerSourceContext = containerSourceContext;
      }
    }

    private sealed class PropertyDictionaryChangeUndoUnit : UndoUnit
    {
      private DocumentCompositeNode node;
      private IProperty propertyKey;
      private SourceContextContainer<DocumentNode> oldValue;
      private SourceContextContainer<DocumentNode> newValue;

      public override bool AllowsDeepMerge
      {
        get
        {
          return true;
        }
      }

      public override int MergeKey
      {
        get
        {
          return this.node.GetHashCode() ^ this.propertyKey.GetHashCode();
        }
      }

      public PropertyDictionaryChangeUndoUnit(DocumentCompositeNode node, IProperty propertyKey, SourceContextContainer<DocumentNode> oldValue, SourceContextContainer<DocumentNode> newValue)
      {
        this.node = node;
        this.propertyKey = propertyKey;
        this.oldValue = oldValue;
        this.newValue = newValue;
      }

      public override UndoUnitMergeResult Merge(IUndoUnit otherUnit, out IUndoUnit mergedUnit)
      {
        mergedUnit = (IUndoUnit) null;
        SceneXamlDocument.PropertyDictionaryChangeUndoUnit dictionaryChangeUndoUnit = otherUnit as SceneXamlDocument.PropertyDictionaryChangeUndoUnit;
        if (dictionaryChangeUndoUnit == null || dictionaryChangeUndoUnit.node != this.node || dictionaryChangeUndoUnit.propertyKey != this.propertyKey)
          return UndoUnitMergeResult.CouldNotMerge;
        if (SourceContextContainer<DocumentNode>.ContentReferenceEquals(dictionaryChangeUndoUnit.oldValue, this.newValue))
          return UndoUnitMergeResult.MergedIntoNothing;
        mergedUnit = (IUndoUnit) new SceneXamlDocument.PropertyDictionaryChangeUndoUnit(this.node, this.propertyKey, dictionaryChangeUndoUnit.oldValue, this.newValue);
        return UndoUnitMergeResult.MergedIntoOneUnit;
      }

      public override void Undo()
      {
        base.Undo();
        this.Toggle();
      }

      public override void Redo()
      {
        base.Redo();
        this.Toggle();
      }

      private void Toggle()
      {
        this.node.ApplyPropertyChange(this.propertyKey, this.oldValue, this.newValue);
        SourceContextContainer<DocumentNode> contextContainer = this.oldValue;
        this.oldValue = this.newValue;
        this.newValue = contextContainer;
      }

      public override string ToString()
      {
        if (this.oldValue == null)
          return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}[{1}]: add {2}", (object) this.node, (object) this.propertyKey, (object) this.newValue);
        if (this.newValue == null)
          return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}[{1}]: remove {2}", (object) this.node, (object) this.propertyKey, (object) this.oldValue);
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}[{1}]: replace {2} with {3}", (object) this.node, (object) this.propertyKey, (object) this.oldValue, (object) this.newValue);
      }
    }

    private sealed class DocumentNodeCollectionChangeUndoUnit : UndoUnit
    {
      private DocumentCompositeNode node;
      private int index;
      private DocumentNode oldChildNode;
      private DocumentNode newChildNode;

      public DocumentNodeCollectionChangeUndoUnit(DocumentCompositeNode node, int index, DocumentNode oldChildNode, DocumentNode newChildNode)
      {
        this.node = node;
        this.index = index;
        this.oldChildNode = oldChildNode;
        this.newChildNode = newChildNode;
      }

      public override void Undo()
      {
        base.Undo();
        this.Toggle();
      }

      public override void Redo()
      {
        base.Redo();
        this.Toggle();
      }

      private void Toggle()
      {
        this.node.ApplyChildrenChange(this.index, this.oldChildNode, this.newChildNode);
        DocumentNode documentNode = this.oldChildNode;
        this.oldChildNode = this.newChildNode;
        this.newChildNode = documentNode;
      }

      public override string ToString()
      {
        if (this.oldChildNode == null)
          return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}[{1}]: insert {2}", (object) this.node, (object) this.index, (object) this.newChildNode);
        if (this.newChildNode == null)
          return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}[{1}]: remove {2}", (object) this.node, (object) this.index, (object) this.oldChildNode);
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}[{1}]: replace {2} with {3}", (object) this.node, (object) this.index, (object) this.oldChildNode, (object) this.newChildNode);
      }
    }

    private sealed class ModifyRootXmlAttributesUndoUnit : UndoUnit
    {
      private XamlDocument.ModifyRootXmlAttributesUndoable undoUnit;

      public ModifyRootXmlAttributesUndoUnit(XamlDocument.ModifyRootXmlAttributesUndoable undoUnit)
      {
        this.undoUnit = undoUnit;
      }

      public override void Undo()
      {
        base.Undo();
        this.undoUnit.Invoke();
      }

      public override void Redo()
      {
        base.Redo();
        this.undoUnit.Invoke();
      }
    }
  }
}
