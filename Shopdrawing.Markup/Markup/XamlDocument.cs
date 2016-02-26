// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.XamlDocument
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Markup.Xml;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.Expression.DesignModel.Markup
{
  public class XamlDocument : IDocumentRoot, IDisposable, INotifyPropertyChanged
  {
    private List<XamlDocument.SourceContextUndo> sourceContextUndo = new List<XamlDocument.SourceContextUndo>();
    private IDocumentContext documentContext;
    private XmlDocumentReference xmlDocumentReference;
    private PersistenceSettings persistenceSettings;
    private IList<XamlParseError> parseErrorsAndWarnings;
    private ITypeId expectedRootType;
    private ITextBuffer textBuffer;
    private Encoding documentEncoding;
    private IXamlSerializerFilter defaultXamlSerializerFilter;
    private DocumentNode rootNode;
    private uint changeStamp;
    private TypeReferences typeReferences;
    private List<DocumentNodeChangeList> changeLists;
    private bool isUndoingTextChange;
    private DocumentNodeChangeList documentChangesSinceSerialize;
    private ITextChangesTracker textChanges;
    private bool isSerializing;
    private bool isParsing;
    private bool isSettingTextDuringUndo;

    public virtual bool IsEditable
    {
      get
      {
        if (this.ParseErrorsCount == 0)
          return this.typeReferences.IsEditable;
        return false;
      }
    }

    public IDocumentContext DocumentContext
    {
      get
      {
        return this.documentContext;
      }
      private set
      {
        if (this.documentContext == value)
          return;
        this.OnDocumentContextChanging(this.documentContext, value);
        this.documentContext = value;
      }
    }

    public virtual IEnumerable<IDocumentRoot> DesignTimeResources
    {
      get
      {
        yield break;
      }
    }

    public ITypeResolver TypeResolver
    {
      get
      {
        return this.documentContext.TypeResolver;
      }
    }

    public ITypeId ExpectedRootType
    {
      get
      {
        return this.expectedRootType;
      }
    }

    public bool HasTextEdits
    {
      get
      {
        return this.textChanges.HasChanged;
      }
    }

    public bool IsParsing
    {
      get
      {
        return this.isParsing;
      }
    }

    public bool IsSerializing
    {
      get
      {
        return this.isSerializing;
      }
    }

    public bool IsSettingTextDuringUndo
    {
      get
      {
        return this.isSettingTextDuringUndo;
      }
    }

    public ITextBuffer TextBuffer
    {
      get
      {
        return this.textBuffer;
      }
    }

    public virtual string Text
    {
      get
      {
        return this.TextBuffer.GetText(0, this.TextBuffer.Length);
      }
      set
      {
        this.OnRootNodeChangingOutsideUndo();
        new XamlDocument.SetTextUndoable(this, value).Invoke();
        this.OnRootNodeChangedOutsideUndo();
      }
    }

    public virtual bool IsUndoingTextChange
    {
      get
      {
        return this.isUndoingTextChange;
      }
      set
      {
        this.isUndoingTextChange = value;
      }
    }

    protected virtual bool IsUndoing
    {
      get
      {
        return false;
      }
    }

    public Encoding SourceEncoding
    {
      get
      {
        string encoding1 = this.xmlDocumentReference.Encoding;
        if (!string.IsNullOrEmpty(encoding1))
        {
          Encoding encoding2;
          try
          {
            encoding2 = Encoding.GetEncoding(encoding1);
          }
          catch (ArgumentException ex)
          {
            encoding2 = (Encoding) null;
          }
          if (encoding2 != null)
            return encoding2;
        }
        return this.documentEncoding;
      }
    }

    public Encoding TargetEncoding
    {
      get
      {
        return DocumentEncodingHelper.GetTargetEncoding(this.SourceEncoding);
      }
    }

    public ICollection<IType> UnresolvedTypes
    {
      get
      {
        return this.typeReferences.UnresolvedTypes;
      }
    }

    internal XmlDocumentReference XmlDocumentReference
    {
      get
      {
        return this.xmlDocumentReference;
      }
    }

    public PersistenceSettings PersistenceSettings
    {
      get
      {
        return this.persistenceSettings;
      }
    }

    public IList<XamlParseError> ParseErrorsAndWarnings
    {
      get
      {
        return this.parseErrorsAndWarnings;
      }
      protected set
      {
        this.parseErrorsAndWarnings = value;
      }
    }

    public int ParseErrorsCount
    {
      get
      {
        int num = 0;
        for (int index = 0; index < this.parseErrorsAndWarnings.Count; ++index)
        {
          if (this.parseErrorsAndWarnings[index].Severity == XamlErrorSeverity.Error)
            ++num;
        }
        return num;
      }
    }

    public ClassAttributes RootClassAttributes
    {
      get
      {
        DocumentCompositeNode documentCompositeNode = this.RootNode as DocumentCompositeNode;
        if (documentCompositeNode != null)
        {
          string valueAsString1 = DocumentPrimitiveNode.GetValueAsString(documentCompositeNode.Properties[this.TypeResolver.PlatformMetadata.KnownProperties.DesignTimeClass]);
          if (valueAsString1 != null)
          {
            string codeBehindClassName = DocumentPrimitiveNode.GetValueAsString(documentCompositeNode.Properties[this.TypeResolver.PlatformMetadata.KnownProperties.DesignTimeSubclass]) ?? valueAsString1;
            string rootNamespace = this.TypeResolver.RootNamespace;
            string qualifiedClassName = string.IsNullOrEmpty(rootNamespace) || !this.TypeResolver.IsCapabilitySet(PlatformCapability.SupportsXClassRootNamespace) ? codeBehindClassName : rootNamespace + "." + codeBehindClassName;
            string valueAsString2 = DocumentPrimitiveNode.GetValueAsString(documentCompositeNode.Properties[this.TypeResolver.PlatformMetadata.KnownProperties.DesignTimeClassModifier]);
            return new ClassAttributes(codeBehindClassName, valueAsString1, valueAsString2, qualifiedClassName);
          }
        }
        return (ClassAttributes) null;
      }
    }

    protected IXamlSerializerFilter DefaultXamlSerializerFilter
    {
      get
      {
        return this.defaultXamlSerializerFilter;
      }
    }

    public bool IsTextUpToDate
    {
      get
      {
        return this.documentChangesSinceSerialize.Count == 0;
      }
    }

    public DocumentNode RootNode
    {
      get
      {
        return this.rootNode;
      }
      set
      {
        if (this.rootNode == value)
          return;
        this.OnRootNodeChangingOutsideUndo();
        this.SetRootNode(value);
        this.OnRootNodeChangedOutsideUndo();
      }
    }

    public IAssembly Assembly
    {
      get
      {
        return this.TypeResolver.ProjectAssembly;
      }
    }

    public ICollection<IAssembly> AssemblyReferences
    {
      get
      {
        return this.TypeResolver.AssemblyReferences;
      }
    }

    public uint ChangeStamp
    {
      get
      {
        return this.changeStamp;
      }
    }

    public IType CodeBehindClass
    {
      get
      {
        ClassAttributes rootClassAttributes = this.RootClassAttributes;
        if (rootClassAttributes != null && !string.IsNullOrEmpty(rootClassAttributes.QualifiedClassName))
        {
          string qualifiedClassName = rootClassAttributes.QualifiedClassName;
          IType type = this.TypeResolver.GetType((string) null, qualifiedClassName);
          if (type != null && type.IsBuilt)
            return type;
          IAssembly assembly = this.Assembly;
          if (assembly != null)
            return this.TypeResolver.PlatformMetadata.CreateUnknownType(this.TypeResolver, assembly, TypeHelper.GetClrNamespacePart(qualifiedClassName), TypeHelper.GetTypeNamePart(qualifiedClassName));
        }
        return (IType) null;
      }
    }

    public event EventHandler TextChanged;

    public event EventHandler ParseCompleted;

    public event EventHandler HasTextEditsChanged;

    public static event EventHandler IncrementalSerializing;

    public static event EventHandler IncrementalSerialized;

    public event EventHandler RootNodeChangingOutsideUndo;

    public event EventHandler RootNodeChangedOutsideUndo;

    public event EventHandler RootNodeChanging;

    public event EventHandler RootNodeChanged;

    public event EventHandler TypesChanged;

    public event PropertyChangedEventHandler PropertyChanged;

    public XamlDocument(IDocumentContext documentContext, ITypeId expectedRootType, ITextBuffer textBuffer, Encoding documentEncoding, IXamlSerializerFilter defaultXamlSerializerFilter)
    {
      if (documentContext == null)
        throw new ArgumentNullException("documentContext");
      if (expectedRootType == null)
        throw new ArgumentNullException("expectedRootType");
      if (textBuffer == null)
        throw new ArgumentNullException("textBuffer");
      if (documentEncoding == null)
        throw new ArgumentNullException("documentEncoding");
      XamlParserResults xamlParserResults = new XamlParser(documentContext, (IReadableSelectableTextBuffer) textBuffer, expectedRootType).Parse();
      this.DocumentContext = documentContext;
      this.xmlDocumentReference = xamlParserResults.XmlDocumentReference;
      this.parseErrorsAndWarnings = xamlParserResults.Errors;
      this.rootNode = xamlParserResults.RootNode;
      this.persistenceSettings = new PersistenceSettings();
      this.expectedRootType = expectedRootType;
      this.textBuffer = textBuffer;
      this.textBuffer.TextChanged += new EventHandler<TextChangedEventArgs>(this.OnTextChanged);
      this.documentEncoding = documentEncoding;
      this.defaultXamlSerializerFilter = defaultXamlSerializerFilter;
      this.ListenToTextChanges();
      this.documentChangesSinceSerialize = new DocumentNodeChangeList();
      this.typeReferences = new TypeReferences();
      this.typeReferences.IsEditableChanged += new EventHandler(this.TypeReferences_IsEditableChanged);
      this.changeLists = new List<DocumentNodeChangeList>();
      if (this.RootNode == null)
        return;
      this.RootNode.SetDocumentRoot((IDocumentRoot) this);
    }

    public void RegisterChangeList(DocumentNodeChangeList changeList)
    {
      this.changeLists.Add(changeList);
    }

    public void UnregisterChangeList(DocumentNodeChangeList changeList)
    {
      this.changeLists.Remove(changeList);
    }

    protected void AddDocumentChange(DocumentNodeMarker marker, DocumentNodeChange change)
    {
      this.documentChangesSinceSerialize.AddCoalesced(marker, change);
    }

    public bool StripExtraNamespaces(params string[] namespaces)
    {
      DocumentNode rootNode = this.RootNode;
      if (rootNode == null)
        return false;
      XmlElementReference elementReference = rootNode.SourceContext as XmlElementReference;
      if (elementReference == null)
        return false;
      bool removed = false;
      elementReference.RemoveMatchingAttributes((Func<XmlElementReference.Attribute, bool>) (attribute =>
      {
        if (attribute.Type != XmlElementReference.AttributeType.Xmlns && attribute.Type != XmlElementReference.AttributeType.SerializerAddedXmlns)
          return false;
        if (namespaces == null || namespaces.Length == 0)
        {
          removed = true;
          return true;
        }
        if (Enumerable.FirstOrDefault<string>((IEnumerable<string>) namespaces, (Func<string, bool>) (ns => ns == attribute.XmlAttribute.Value.Value)) == null)
          return false;
        removed = true;
        return true;
      }));
      if (!removed)
        return false;
      this.AddDocumentChange(rootNode.Marker, new DocumentNodeChange(rootNode, rootNode));
      return true;
    }

    public bool CommitTextEdits()
    {
      if (this.isSerializing)
        return false;
      bool flag = false;
      this.isParsing = true;
      try
      {
        ITextRange textRange = this.textChanges.GetChanges();
        IList<XamlParseError> list = this.parseErrorsAndWarnings;
        IList<XamlParseError> errors = (IList<XamlParseError>) null;
        foreach (XamlParseError xamlParseError in (IEnumerable<XamlParseError>) list)
        {
          int originalReference = this.textChanges.GetOffsetInOriginalReference(xamlParseError.Line, xamlParseError.Column);
          textRange = TextRange.Union(textRange, (ITextRange) new TextRange(originalReference, originalReference));
        }
        if (!TextRange.IsNull(textRange))
        {
          ITextRange nodeRange;
          DocumentNode documentNode = this.GetLowestNodeContainingRange(textRange, true, true, out nodeRange);
          if (this.textChanges.ContainsRangeDeletions && documentNode != null)
          {
            XmlElementReference elementReference1 = (XmlElementReference) documentNode.SourceContext;
            if (elementReference1 != null)
            {
              int start = elementReference1.TextRange.Offset + 1;
              int end = elementReference1.TextRange.Offset + elementReference1.TextRange.Length;
              if (end <= start || !TextRange.Contains((ITextRange) new TextRange(start, end), textRange))
              {
                while (documentNode != null && documentNode.Parent != null)
                {
                  documentNode = (DocumentNode) documentNode.Parent;
                  XmlElementReference elementReference2 = documentNode.SourceContext as XmlElementReference;
                  if (elementReference2 != null && elementReference2.StartTagRange != null && elementReference2.StartTagRange.Length != 0)
                  {
                    nodeRange = elementReference2.TextRange;
                    if (documentNode.Parent == null || documentNode.Parent.SourceContext == null || (documentNode.Parent.SourceContext.TextRange.Offset != nodeRange.Offset || documentNode.Parent.SourceContext.TextRange.Length != nodeRange.Length))
                      break;
                  }
                }
              }
            }
          }
          if (documentNode != null && documentNode != this.RootNode)
          {
            DocumentNode newlyParsedNode = XamlParser.ParseRegion((IReadableSelectableTextBuffer) this.textBuffer, (DocumentNode) documentNode.Parent, documentNode, nodeRange, out errors);
            if (errors.Count == 0)
            {
              flag = true;
              new XamlDocument.IncrementalTreeChange(this).Apply(documentNode, newlyParsedNode);
            }
          }
          else
          {
            XamlParserResults xamlParserResults = new XamlParser(this.documentContext, (IReadableSelectableTextBuffer) this.textBuffer, this.expectedRootType).Parse();
            if (xamlParserResults.Errors.Count == 0)
            {
              if (this.RootNode == null || xamlParserResults.RootNode == null || !this.RootNode.Type.Equals((object) xamlParserResults.RootNode.Type))
              {
                this.OnRootNodeChangingOutsideUndo();
                this.SetRootNode(xamlParserResults.RootNode);
                this.OnRootNodeChangedOutsideUndo();
              }
              else
                new XamlDocument.IncrementalTreeChange(this).Apply(this.RootNode, xamlParserResults.RootNode);
              this.xmlDocumentReference = xamlParserResults.XmlDocumentReference;
              flag = true;
            }
            errors = xamlParserResults.Errors;
          }
        }
        this.SetParseErrors(errors);
        if (flag)
        {
          this.ListenToTextChanges();
          if (this.ParseCompleted != null)
            this.ParseCompleted((object) this, EventArgs.Empty);
        }
        if (this.HasTextEditsChanged != null)
          this.HasTextEditsChanged((object) this, EventArgs.Empty);
      }
      finally
      {
        this.isParsing = false;
      }
      return flag;
    }

    protected virtual void PushTransaction()
    {
      this.sourceContextUndo.Clear();
    }

    protected virtual void PopTransaction(bool commit)
    {
      if (commit)
        return;
      for (int index = this.sourceContextUndo.Count - 1; index >= 0; --index)
      {
        XamlDocument.SourceContextUndo sourceContextUndo = this.sourceContextUndo[index];
        if (sourceContextUndo.SourceContext != null)
          sourceContextUndo.DocumentNode.SourceContext = sourceContextUndo.SourceContext;
        if (sourceContextUndo.ContainerSourceContext != null)
          sourceContextUndo.DocumentNode.ContainerSourceContext = sourceContextUndo.ContainerSourceContext;
      }
    }

    protected virtual void ClearUndoStack()
    {
      this.typeReferences.Clear();
    }

    protected void ReparseDocument()
    {
      bool flag = this.isParsing;
      this.isParsing = true;
      try
      {
        this.OnRootNodeChangingOutsideUndo();
        this.OnRootNodeChanging();
        this.ClearUndoStack();
        XamlParserResults xamlParserResults = new XamlParser(this.documentContext, (IReadableSelectableTextBuffer) this.textBuffer, this.expectedRootType).Parse();
        this.xmlDocumentReference = xamlParserResults.XmlDocumentReference;
        this.parseErrorsAndWarnings = xamlParserResults.Errors;
        this.SetDocumentRoot(xamlParserResults.RootNode);
        this.FireRootChangedNotifications();
        this.OnRootNodeChanged();
        this.OnRootNodeChangedOutsideUndo();
      }
      finally
      {
        this.isParsing = flag;
      }
    }

    protected virtual void FireRootChangedNotifications()
    {
      this.OnPropertyChanged("ParseErrorsAndWarnings");
      this.OnPropertyChanged("ParseErrorsCount");
      this.OnPropertyChanged("IsEditable");
    }

    private bool AnyReferencesToAssemblies(ICollection<TypeChangedInfo> assemblyInfos)
    {
      if (assemblyInfos.Count > 0)
      {
        IEnumerable<IAssembly> source = Enumerable.Select<TypeChangedInfo, IAssembly>(Enumerable.Where<TypeChangedInfo>((IEnumerable<TypeChangedInfo>) assemblyInfos, (Func<TypeChangedInfo, bool>) (info => info.ModificationAction != ModificationType.Added)), (Func<TypeChangedInfo, IAssembly>) (assemblyInfo => assemblyInfo.Assembly));
        foreach (KeyValuePair<IType, int> keyValuePair in this.typeReferences.References)
        {
          IAssembly runtimeAssembly = keyValuePair.Key.RuntimeAssembly;
          if (Enumerable.Contains<IAssembly>(source, runtimeAssembly))
            return true;
        }
      }
      return false;
    }

    private void UpdateTypeReferences(DocumentNode node, bool addReference)
    {
      this.UpdateTypeReference(node.Type, addReference);
      IMember valueAsMember = DocumentPrimitiveNode.GetValueAsMember(node);
      if (valueAsMember == null)
        return;
      IType typeId = valueAsMember as IType;
      if (typeId != null)
      {
        this.UpdateTypeReference(typeId, addReference);
      }
      else
      {
        IType declaringType = valueAsMember.DeclaringType;
        if (declaringType == null)
          return;
        this.UpdateTypeReference(declaringType, addReference);
      }
    }

    private void UpdateTypeReference(IType typeId, bool addReference)
    {
      if (addReference)
        this.typeReferences.AddReference(typeId);
      else
        this.typeReferences.RemoveReference(typeId);
    }

    protected virtual void SetDocumentRoot(DocumentNode node)
    {
      foreach (DocumentNodeMarkerSortedListBase markerSortedListBase in this.changeLists)
        markerSortedListBase.Clear();
      if (this.RootNode != null)
      {
        this.OnNodeChange(this.RootNode, new DocumentNodeChange(this.RootNode, node));
        this.RootNode.SetDocumentRoot((IDocumentRoot) null);
      }
      DocumentNode rootNode = this.RootNode;
      this.SetRootNodeRaw(node);
      ++this.changeStamp;
      if (this.RootNode == null)
        return;
      this.RootNode.SetDocumentRoot((IDocumentRoot) this);
      this.OnNodeChange(node, new DocumentNodeChange(rootNode, this.RootNode));
    }

    protected virtual void SetParseErrors(IList<XamlParseError> errors)
    {
      this.ParseErrorsAndWarnings = errors;
      if (this.ParseErrorsAndWarnings.Count == 0)
        return;
      this.FireRootChangedNotifications();
    }

    protected virtual void SetRootNode(DocumentNode rootNode)
    {
      this.OnRootNodeChanging();
      this.SetDocumentRoot(rootNode);
      this.OnRootNodeChanged();
    }

    private void SetRootNodeRaw(DocumentNode rootNode)
    {
      this.rootNode = rootNode;
    }

    public bool SynchronizeText()
    {
      if (this.IsTextUpToDate || this.isParsing || this.isSerializing)
        return false;
      if (XamlDocument.IncrementalSerializing != null)
        XamlDocument.IncrementalSerializing((object) this, EventArgs.Empty);
      bool flag1 = this.isSerializing;
      this.isSerializing = true;
      try
      {
        XamlSerializer serializer = new XamlSerializer((IDocumentRoot) this, this.DefaultXamlSerializerFilter, true);
        bool flag2 = false;
        DocumentNodeMarkerSortedList serializedNodes = new DocumentNodeMarkerSortedList(5);
        DocumentNodeMarkerSortedList removedNodes = new DocumentNodeMarkerSortedList(2);
        List<DocumentNodeChange> list = new List<DocumentNodeChange>(this.documentChangesSinceSerialize.CollapsedChangeList);
        XamlDocument.StableSortHelper.StableSort<DocumentNodeChange>(list, new Comparison<DocumentNodeChange>(this.DocumentNodeChangeComparer));
        foreach (DocumentNodeChange documentNodeChange in list)
        {
          if (!documentNodeChange.IsRootNodeChange)
          {
            if (documentNodeChange.OldChildNode != null || documentNodeChange.NewChildNode != null && documentNodeChange.NewChildNode.Marker != null)
            {
              DocumentNodeChangeAction action = documentNodeChange.Action;
              DocumentNode parentNode = (DocumentNode) documentNodeChange.ParentNode;
              ITextRange nodeRange = TextRange.Null;
              DocumentNode documentNode1;
              if (documentNodeChange.OldChildNode == null)
              {
                documentNode1 = XamlSerializerUtilities.GetClosestAncestorSupportedForIncrementalSerialize(documentNodeChange.NewChildNode);
                action = documentNode1 != documentNodeChange.NewChildNode ? DocumentNodeChangeAction.Replace : DocumentNodeChangeAction.Add;
              }
              else
              {
                documentNode1 = XamlSerializerUtilities.GetClosestAncestorSupportedForIncrementalSerialize(documentNodeChange.OldChildNode);
                if (documentNode1 != documentNodeChange.OldChildNode)
                  action = DocumentNodeChangeAction.Replace;
              }
              if (action != DocumentNodeChangeAction.Add && documentNode1 != null)
              {
                XamlSourceContext xamlSourceContext = documentNode1.SourceContext as XamlSourceContext;
                if (xamlSourceContext == null || xamlSourceContext.TextRange == null)
                {
                  XmlAttributeReference attributeReference = documentNode1.ContainerSourceContext as XmlAttributeReference;
                  if (attributeReference == null || attributeReference.TextRange == null)
                  {
                    documentNode1 = (DocumentNode) documentNodeChange.ParentNode;
                    while (documentNode1 != null && documentNode1.SourceContext == null && !(documentNode1.ContainerSourceContext is XmlAttributeReference))
                      documentNode1 = (DocumentNode) documentNode1.Parent;
                    action = DocumentNodeChangeAction.Replace;
                  }
                }
                if (action == DocumentNodeChangeAction.Remove && parentNode != null && (parentNode.ChildNodesCount == 0 && parentNode.SourceContext == null) && (parentNode.ContainerSourceContext != null && parentNode.ContainerSourceContext.TextRange != null))
                {
                  documentNode1 = parentNode;
                  parentNode = (DocumentNode) parentNode.Parent;
                }
                nodeRange = XamlSerializerUtilities.GetNodeSpan(this.textBuffer, documentNode1, true);
              }
              if (documentNode1 == documentNodeChange.OldChildNode)
                documentNode1 = documentNodeChange.NewChildNode;
              if (documentNode1 == null || documentNode1.Marker == null || documentNode1.Marker.IsDeleted)
              {
                action = DocumentNodeChangeAction.Remove;
                documentNode1 = documentNodeChange.OldChildNode;
              }
              if (action == DocumentNodeChangeAction.Remove)
              {
                this.UpdateSerializationForRemove(documentNode1, parentNode, nodeRange, removedNodes);
              }
              else
              {
                DocumentNode documentNode2 = this.RootNode;
                try
                {
                  this.PushTransaction();
                  documentNode2 = this.UpdateSerializationForChange(serializer, serializedNodes, documentNode1, action, nodeRange);
                }
                finally
                {
                  this.PopTransaction(documentNode2 == null);
                }
                while (documentNode2 != null)
                {
                  if (documentNode2 == this.RootNode)
                  {
                    flag2 = true;
                    break;
                  }
                  ITextRange nodeSpan = XamlSerializerUtilities.GetNodeSpan(this.textBuffer, documentNode2, true, documentNode1);
                  bool flag3 = TextRange.IsNull(nodeSpan) || nodeSpan.Length == 0;
                  try
                  {
                    this.PushTransaction();
                    documentNode2 = this.UpdateSerializationForChange(serializer, serializedNodes, documentNode2, flag3 ? DocumentNodeChangeAction.Add : DocumentNodeChangeAction.Replace, nodeSpan);
                  }
                  finally
                  {
                    this.PopTransaction(documentNode2 == null);
                  }
                }
              }
            }
            else
              continue;
          }
          else
            flag2 = true;
          if (flag2)
            break;
        }
        if (flag2)
        {
          XamlSerializerUtilities.ClearSourceContextInitialRanges(this.RootNode);
          this.textBuffer.SetText(0, this.textBuffer.Length, serializer.Serialize());
          XamlSerializerUtilities.UpdateSourceContextRangeFromInitialRange((IReadableSelectableTextBuffer) this.textBuffer, this.RootNode, 0);
          this.xmlDocumentReference = (XmlDocumentReference) ((XamlSourceContext) this.RootNode.SourceContext).Parent;
          this.xmlDocumentReference.TextBuffer = (IReadableSelectableTextBuffer) this.textBuffer;
          if (serializer.RootAttributesModified != null)
          {
            XmlElementReference element = this.RootNode.SourceContext as XmlElementReference;
            if (element != null)
              this.ModifyRootXmlAttributes(new XamlDocument.ModifyRootXmlAttributesUndoable(element, (IEnumerable<KeyValuePair<XmlAttribute, XmlAttribute>>) serializer.RootAttributesModified));
          }
        }
      }
      finally
      {
        this.isSerializing = flag1;
      }
      this.documentChangesSinceSerialize.Clear();
      this.ListenToTextChanges();
      if (XamlDocument.IncrementalSerialized != null)
        XamlDocument.IncrementalSerialized((object) this, EventArgs.Empty);
      return true;
    }

    public void ClearDocumentChanges()
    {
      this.documentChangesSinceSerialize.Clear();
    }

    private void CollapseParentTagIfEmpty(DocumentNode parentNode)
    {
      while (parentNode != null && (parentNode.SourceContext == null || parentNode.SourceContext.TextBuffer != this.textBuffer))
        parentNode = (DocumentNode) parentNode.Parent;
      if (parentNode == null)
        return;
      XmlElementReference elementReference = parentNode.SourceContext as XmlElementReference;
      if (elementReference == null || elementReference.StartTagRange == null || elementReference.TextRange == null)
        return;
      int num1 = elementReference.StartTagRange.Offset + elementReference.StartTagRange.Length;
      int num2 = elementReference.TextRange.Offset + elementReference.TextRange.Length;
      if (num1 == num2)
        return;
      ITextRange textRange = TextBufferHelper.ExpandSpanLeftToFillWhitespace((IReadableTextBuffer) this.textBuffer, (ITextRange) new TextRange(XamlSerializerUtilities.GetStartOfXmlCloseTag((IReadableSelectableTextBuffer) this.textBuffer, num2), num2));
      if (textRange.Offset != num1)
        return;
      int num3 = elementReference.StartTagRange.Length + 1;
      this.textBuffer.SetText(textRange.Offset - 1, textRange.Length + 1, "/>");
      XamlSerializerUtilities.UpdateNodeSourceContext((IReadableSelectableTextBuffer) this.textBuffer, parentNode, num3, num3);
    }

    private void UpdateSerializationForRemove(DocumentNode textDeletedNode, DocumentNode parentNode, ITextRange nodeRange, DocumentNodeMarkerSortedList removedNodes)
    {
      if (parentNode != null && parentNode.Marker == null)
        return;
      ITextRange range = TextBufferHelper.ExpandSpanLeftToFillWhitespace((IReadableTextBuffer) this.textBuffer, (ITextRange) new TextRange(nodeRange.Offset, nodeRange.Offset + nodeRange.Length));
      if (textDeletedNode != null)
      {
        if (removedNodes != null && textDeletedNode.Marker != null)
        {
          if (removedNodes.FindPosition(textDeletedNode.Marker) >= 0 || removedNodes.FindFarthestAncestor(textDeletedNode.Marker) >= 0)
            return;
          removedNodes.Add(textDeletedNode.Marker);
        }
        XamlSerializerUtilities.PrepareNodeForTextDeletion((IDocumentRoot) this, textDeletedNode);
      }
      if (TextRange.IsNull(range))
        return;
      this.textBuffer.SetText(range.Offset, range.Length, string.Empty);
      this.CollapseParentTagIfEmpty(parentNode);
    }

    private DocumentNode UpdateSerializationForChange(XamlSerializer serializer, DocumentNodeMarkerSortedList serializedNodes, DocumentNode changedNode, DocumentNodeChangeAction action, ITextRange nodeRange)
    {
      DocumentNode ancestorMatchingRange = this.GetFurthestAncestorMatchingRange(changedNode, nodeRange);
      if (ancestorMatchingRange != changedNode)
      {
        changedNode = ancestorMatchingRange;
        action = DocumentNodeChangeAction.Replace;
        nodeRange = TextRange.Union(nodeRange, XamlSerializerUtilities.GetNodeSpan(this.textBuffer, changedNode, true));
      }
      if (action != DocumentNodeChangeAction.Add && TextRange.IsNull(nodeRange))
        action = DocumentNodeChangeAction.Add;
      if (changedNode.Parent == null)
        return this.RootNode;
      if (changedNode.Marker == null || serializedNodes.FindPosition(changedNode.Marker) >= 0 || serializedNodes.FindFarthestAncestor(changedNode.Marker) >= 0)
        return (DocumentNode) null;
      serializedNodes.Add(changedNode.Marker);
      XamlSerializerUtilities.ClearSourceContextInitialRanges(changedNode);
      List<KeyValuePair<XmlAttribute, XmlAttribute>> rootAttributesModified;
      StringBuilder stringBuilder1 = serializer.SerializeRegion(changedNode, out rootAttributesModified);
      if (rootAttributesModified != null && Enumerable.Any<KeyValuePair<XmlAttribute, XmlAttribute>>((IEnumerable<KeyValuePair<XmlAttribute, XmlAttribute>>) rootAttributesModified, (Func<KeyValuePair<XmlAttribute, XmlAttribute>, bool>) (d => d.Value != null)))
        return this.RootNode;
      if (stringBuilder1 == null)
      {
        if (action == DocumentNodeChangeAction.Replace)
          this.UpdateSerializationForRemove(changedNode, (DocumentNode) changedNode.Parent, nodeRange, (DocumentNodeMarkerSortedList) null);
        return (DocumentNode) null;
      }
      int startIndex1 = 0;
      int index = stringBuilder1.Length - 1;
      while (startIndex1 < stringBuilder1.Length && char.IsWhiteSpace(stringBuilder1[startIndex1]))
        ++startIndex1;
      while (index > startIndex1 && char.IsWhiteSpace(stringBuilder1[index]))
        --index;
      bool flag1 = false;
      if (action != DocumentNodeChangeAction.Add)
        flag1 = (int) this.textBuffer.GetText(nodeRange.Offset, 1)[0] == 60;
      bool flag2 = startIndex1 < stringBuilder1.Length && (int) stringBuilder1[startIndex1] == 60;
      DocumentNode documentNode = (DocumentNode) changedNode.Parent;
      XmlElementReference elementReference = documentNode.SourceContext as XmlElementReference;
      if (elementReference != null && elementReference.TextRange == null)
        elementReference = (XmlElementReference) null;
      int offset1 = -1;
      int offset2;
      if ((action == DocumentNodeChangeAction.Add || !flag1) && flag2)
      {
        if (action != DocumentNodeChangeAction.Add)
        {
          ITextRange textRange = TextBufferHelper.ExpandSpanLeftToFillWhitespace((IReadableTextBuffer) this.textBuffer, (ITextRange) new TextRange(nodeRange.Offset, nodeRange.Offset + nodeRange.Length));
          this.textBuffer.SetText(textRange.Offset, textRange.Length, string.Empty);
        }
        StringBuilder stringBuilder2 = new StringBuilder(stringBuilder1.Length + 150);
        stringBuilder2.AppendLine();
        int length = stringBuilder2.Length;
        while (startIndex1 > 0 && (int) stringBuilder1[startIndex1 - 1] != 10)
          --startIndex1;
        stringBuilder2.Append(stringBuilder1.ToString(startIndex1, stringBuilder1.Length - startIndex1));
        int num = -1;
        int startTagEnd = -2;
        if (elementReference != null)
        {
          num = elementReference.TextRange.Offset + elementReference.TextRange.Length;
          startTagEnd = elementReference.StartTagRange == null ? num : elementReference.StartTagRange.Offset + elementReference.StartTagRange.Length;
        }
        if (num != startTagEnd)
        {
          int siteChildIndex = changedNode.SiteChildIndex;
          if (!changedNode.IsChild)
          {
            if (elementReference == null)
              return documentNode;
            offset1 = this.FindInsertionLocation(changedNode, startTagEnd);
          }
          else
          {
            bool flag3 = false;
            DocumentNode node;
            for (; siteChildIndex > 0 && (node = changedNode.Parent.Children[siteChildIndex - 1]) != null; --siteChildIndex)
            {
              ITextRange nodeSpan = XamlSerializerUtilities.GetNodeSpan(this.textBuffer, node, false);
              if (!TextRange.IsNull(nodeSpan))
              {
                offset1 = nodeSpan.Offset + nodeSpan.Length;
                flag3 = true;
                break;
              }
            }
            if (!flag3)
            {
              if (elementReference == null)
                return documentNode;
              offset1 = this.FindInsertionLocation(changedNode, startTagEnd);
            }
          }
          this.textBuffer.SetText(offset1, 0, stringBuilder2.ToString());
          offset2 = offset1 + (length - startIndex1);
        }
        else
        {
          string fullName = this.ParseStartTag(this.textBuffer.GetText(elementReference.TextRange.Offset, elementReference.TextRange.Length)).Name.FullName;
          StringBuilder stringBuilder3 = new StringBuilder(stringBuilder2.Length + 50);
          stringBuilder3.Append(">");
          stringBuilder3.Append(stringBuilder2.ToString());
          stringBuilder3.AppendLine();
          ITextRange textRange = TextBufferHelper.ExpandSpanLeftToFillWhitespace((IReadableTextBuffer) this.textBuffer, (ITextRange) new TextRange(elementReference.TextRange.Offset, elementReference.TextRange.Offset));
          if (textRange.Length > 0)
          {
            string text = this.textBuffer.GetText(textRange.Offset, textRange.Length);
            int startIndex2 = text.LastIndexOf('\n') + 1;
            stringBuilder3.Append(text, startIndex2, text.Length - startIndex2);
          }
          stringBuilder3.Append("</");
          stringBuilder3.Append(fullName);
          stringBuilder3.Append(">");
          this.textBuffer.SetText(startTagEnd - 2, 2, stringBuilder3.ToString());
          offset2 = startTagEnd - 1 + length - startIndex1;
          XamlSerializerUtilities.UpdateNodeSourceContext((IReadableSelectableTextBuffer) this.textBuffer, documentNode, startTagEnd - elementReference.TextRange.Offset - 1, num + stringBuilder3.Length - 2 - elementReference.TextRange.Offset);
        }
      }
      else if ((action == DocumentNodeChangeAction.Add || flag1) && !flag2)
      {
        if (elementReference == null)
          return documentNode;
        if (action != DocumentNodeChangeAction.Add)
        {
          ITextRange textRange = TextBufferHelper.ExpandSpanLeftToFillWhitespace((IReadableTextBuffer) this.textBuffer, (ITextRange) new TextRange(nodeRange.Offset, nodeRange.Offset + nodeRange.Length));
          this.textBuffer.SetText(textRange.Offset, textRange.Length, "");
        }
        int insertionLocation = this.FindAttributeInsertionLocation(this.textBuffer, elementReference.StartTagRange ?? elementReference.TextRange, changedNode);
        this.textBuffer.SetText(insertionLocation, 0, " " + stringBuilder1.ToString(startIndex1, index - startIndex1 + 1));
        offset2 = insertionLocation + 1 - startIndex1;
        if (action != DocumentNodeChangeAction.Add)
          this.CollapseParentTagIfEmpty(documentNode);
      }
      else
      {
        this.textBuffer.SetText(nodeRange.Offset, nodeRange.Length, stringBuilder1.ToString(startIndex1, index - startIndex1 + 1));
        offset2 = nodeRange.Offset - startIndex1;
      }
      XamlSerializerUtilities.UpdateSourceContextRangeFromInitialRange((IReadableSelectableTextBuffer) this.textBuffer, changedNode, offset2);
      return (DocumentNode) null;
    }

    private int DocumentNodeChangeComparer(DocumentNodeChange changeA, DocumentNodeChange changeB)
    {
      if (changeA.Action != changeB.Action)
      {
        if (changeA.Action == DocumentNodeChangeAction.Remove)
          return -1;
        if (changeB.Action == DocumentNodeChangeAction.Remove)
          return 1;
      }
      if (changeA.IsPropertyChange && changeB.IsPropertyChange)
      {
        IPropertyId timeShouldSerialize = this.DocumentContext.TypeResolver.PlatformMetadata.KnownProperties.DesignTimeShouldSerialize;
        if (timeShouldSerialize.Equals((object) changeA.PropertyKey) && timeShouldSerialize.Equals((object) changeB.PropertyKey))
          return !changeA.ParentNode.Marker.Contains(changeB.ParentNode.Marker) ? 1 : -1;
        if (timeShouldSerialize.Equals((object) changeA.PropertyKey))
          return -1;
        return timeShouldSerialize.Equals((object) changeB.PropertyKey) ? 1 : 0;
      }
      if (changeA.IsChildChange != changeB.IsChildChange)
        return changeA.IsChildChange.CompareTo(changeB.IsChildChange);
      if (!changeA.IsChildChange || !changeB.IsChildChange || changeA.NewChildNode == null && changeB.NewChildNode == null)
        return 0;
      if (changeA.NewChildNode == null)
        return -1;
      if (changeB.NewChildNode == null)
        return 1;
      if (changeA.NewChildNode.Parent == null && changeB.NewChildNode.Parent == null)
        return 0;
      if (changeA.NewChildNode.Parent == null)
        return -1;
      if (changeB.NewChildNode.Parent == null)
        return 1;
      return changeA.NewChildNode.SiteChildIndex.CompareTo(changeB.NewChildNode.SiteChildIndex);
    }

    private int FindAttributeInsertionLocation(ITextBuffer textBuffer, ITextRange startTagRange, DocumentNode node)
    {
      int num = startTagRange.Offset + startTagRange.Length - 1;
      if (num - 1 >= 0 && (int) textBuffer.GetText(num - 1, 1)[0] == 47)
        --num;
      if (node.IsProperty && (node.Parent.IsNameProperty((IPropertyId) node.SitePropertyKey) || this.TypeResolver.PlatformMetadata.KnownProperties.DictionaryEntryKey.Equals((object) node.SitePropertyKey)))
      {
        XmlAttribute xmlAttribute = Enumerable.FirstOrDefault<XmlAttribute>((IEnumerable<XmlAttribute>) XmlAttributeEnumerable.NotXmlnsNorDirective(this.ParseStartTag(textBuffer.GetText(startTagRange.Offset, startTagRange.Length))));
        if (xmlAttribute != null)
          num = startTagRange.Offset + xmlAttribute.SourceContext.StartCol;
      }
      return TextBufferHelper.ExpandSpanLeftToFillWhitespace((IReadableTextBuffer) this.textBuffer, (ITextRange) new TextRange(num, num)).Offset;
    }

    private int FindInsertionLocation(DocumentNode node, int startTagEnd)
    {
      int num = startTagEnd;
      DocumentCompositeNode parent = node.Parent;
      while (parent != null && parent.SourceContext == null)
        parent = parent.Parent;
      if (parent == null)
        return startTagEnd;
      IPropertyId resourcesProperty = parent.Type.Metadata.ResourcesProperty;
      if (resourcesProperty != null)
      {
        if (node.IsProperty && resourcesProperty.Equals((object) node.SitePropertyKey))
          return startTagEnd;
        DocumentNode node1 = parent.Properties[resourcesProperty];
        if (node1 != null && (node1.SourceContext is XmlElementReference || node1.ContainerSourceContext is XmlElementReference))
        {
          ITextRange nodeSpan = XamlSerializerUtilities.GetNodeSpan(this.textBuffer, node1, true, node);
          if (!TextRange.IsNull(nodeSpan))
            num = nodeSpan.Offset + nodeSpan.Length;
        }
      }
      if (DocumentNodeHelper.IsStyleOrTemplate(parent.Type))
      {
        if (node.IsProperty && this.TypeResolver.PlatformMetadata.KnownProperties.FrameworkTemplateVisualTree.Equals((object) node.SitePropertyKey))
          return num;
        DocumentNode node1 = parent.Properties[this.TypeResolver.PlatformMetadata.KnownProperties.FrameworkTemplateVisualTree];
        if (node1 != null && (node1.SourceContext is XmlElementReference || node1.ContainerSourceContext is XmlElementReference))
        {
          ITextRange nodeSpan = XamlSerializerUtilities.GetNodeSpan(this.textBuffer, node1, true, node);
          if (!TextRange.IsNull(nodeSpan))
            num = nodeSpan.Offset + nodeSpan.Length;
        }
      }
      int val1 = num;
      for (int index = 0; index < parent.Properties.Count; ++index)
      {
        DocumentNode node1 = parent.Properties[index];
        if (node1.SourceContext is XmlElementReference || node1.ContainerSourceContext is XmlElementReference)
        {
          ITextRange nodeSpan = XamlSerializerUtilities.GetNodeSpan(this.textBuffer, node1, true, node);
          if (!TextRange.IsNull(nodeSpan))
            val1 = Math.Max(val1, nodeSpan.Offset + nodeSpan.Length);
        }
      }
      IPropertyId propertyId = (IPropertyId) parent.Type.Metadata.DefaultContentProperty;
      if (propertyId != null && node.IsProperty && propertyId.Equals((object) node.SitePropertyKey) || node.IsChild)
        return val1;
      return num;
    }

    private DocumentNode GetFurthestAncestorMatchingRange(DocumentNode documentNode, ITextRange range)
    {
      DocumentNode documentNode1 = documentNode;
      for (DocumentNode documentNode2 = (DocumentNode) documentNode.Parent; documentNode2 != null; documentNode2 = (DocumentNode) documentNode2.Parent)
      {
        XamlSourceContext xamlSourceContext = documentNode2.SourceContext as XamlSourceContext;
        DocumentCompositeNode node = documentNode2 as DocumentCompositeNode;
        if (xamlSourceContext == null && node != null && (!node.SupportsChildren || XamlSerializer.HasAnyNonContentPropertiesSet(node)) || xamlSourceContext != null && xamlSourceContext.TextRange != null && (xamlSourceContext.TextRange.Offset == range.Offset && xamlSourceContext.TextRange.Length == range.Length))
          documentNode1 = documentNode2;
        else
          break;
      }
      return documentNode1;
    }

    private XmlElement ParseStartTag(string startTag)
    {
      return new Parser(new Document("InMemoryString", new DocumentText(startTag)), new ErrorNodeList()).ParsePartialElement((XmlElement) null);
    }

    public void Save(Stream stream)
    {
      this.SynchronizeText();
      Encoding targetEncoding = this.TargetEncoding;
      using (StreamWriter streamWriter = new StreamWriter(stream, targetEncoding))
      {
        int offset = 0;
        int length = 1000;
        while (offset < this.textBuffer.Length)
        {
          if (this.textBuffer.Length - offset < length)
            length = this.textBuffer.Length - offset;
          string text = this.textBuffer.GetText(offset, length);
          streamWriter.Write(text);
          offset += length;
        }
      }
    }

    public void OnNodeChange(DocumentNode node, DocumentNodeChange args)
    {
      ++this.changeStamp;
      foreach (DocumentNodeMarkerSortedListOf<DocumentNodeChange> markerSortedListOf in this.changeLists)
        markerSortedListOf.AddCoalesced(node.Marker, args);
      if (this.IsParsing || this.IsUndoing || args.IsPropertyChange && !args.PropertyKey.ShouldSerialize && args.PropertyKey.MemberType == MemberType.DesignTimeProperty)
        return;
      this.AddDocumentChange(node.Marker, args);
    }

    public void OnNodeSetDocumentRoot(DocumentNode node)
    {
      this.UpdateTypeReferences(node, true);
    }

    public void OnNodeUnsetDocumentRoot(DocumentNode node)
    {
      this.UpdateTypeReferences(node, false);
    }

    protected virtual void ModifyRootXmlAttributes(XamlDocument.ModifyRootXmlAttributesUndoable undoUnit)
    {
      undoUnit.Invoke();
    }

    public virtual void SetSourceContext(DocumentNode node, INodeSourceContext sourceContext)
    {
      this.sourceContextUndo.Add(new XamlDocument.SourceContextUndo(node, node.SourceContext, (INodeSourceContext) null));
      node.SourceContext = sourceContext;
    }

    public virtual void SetContainerSourceContext(DocumentNode node, INodeSourceContext sourceContext)
    {
      this.sourceContextUndo.Add(new XamlDocument.SourceContextUndo(node, (INodeSourceContext) null, node.ContainerSourceContext));
      node.ContainerSourceContext = sourceContext;
    }

    public virtual void ApplyPropertyChange(DocumentCompositeNode node, IProperty propertyKey, SourceContextContainer<DocumentNode> oldValue, SourceContextContainer<DocumentNode> newValue)
    {
      node.ApplyPropertyChange(propertyKey, oldValue, newValue);
    }

    public virtual void ApplyChildrenChange(DocumentCompositeNode node, int index, DocumentNode oldChildNode, DocumentNode newChildNode)
    {
      node.ApplyChildrenChange(index, oldChildNode, newChildNode);
    }

    protected virtual void ApplyNamescopeUpdate(Dictionary<DocumentNodeNameScope, List<DocumentNode>> actions, bool isRemoving)
    {
      new XamlDocument.ApplyNamescopeUpdateUndoable(actions, isRemoving).Invoke();
    }

    public virtual void Dispose()
    {
      if (this.textChanges != null)
      {
        this.textChanges.Dispose();
        this.textChanges = (ITextChangesTracker) null;
      }
      if (this.textBuffer != null)
      {
        this.textBuffer.TextChanged -= new EventHandler<TextChangedEventArgs>(this.OnTextChanged);
        IDisposable disposable = this.textBuffer as IDisposable;
        if (disposable != null)
          disposable.Dispose();
        this.textBuffer = (ITextBuffer) null;
      }
      this.DocumentContext = (IDocumentContext) null;
      this.xmlDocumentReference = (XmlDocumentReference) null;
    }

    protected void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    protected void OnRootNodeChangingOutsideUndo()
    {
      if (this.RootNodeChangingOutsideUndo == null)
        return;
      this.RootNodeChangingOutsideUndo((object) this, EventArgs.Empty);
    }

    protected void OnRootNodeChangedOutsideUndo()
    {
      if (this.RootNodeChangedOutsideUndo == null)
        return;
      this.RootNodeChangedOutsideUndo((object) this, EventArgs.Empty);
    }

    protected void OnRootNodeChanging()
    {
      if (this.RootNodeChanging == null)
        return;
      this.RootNodeChanging((object) this, EventArgs.Empty);
    }

    protected void OnRootNodeChanged()
    {
      if (this.RootNodeChanged == null)
        return;
      this.RootNodeChanged((object) this, EventArgs.Empty);
    }

    protected void OnTypesChanged()
    {
      if (this.TypesChanged == null)
        return;
      this.TypesChanged((object) this, EventArgs.Empty);
    }

    public override string ToString()
    {
      return this.textBuffer.ToString();
    }

    protected virtual void OnDocumentContextChanging(IDocumentContext oldContext, IDocumentContext newContext)
    {
      if (oldContext != null)
      {
        oldContext.TypeResolver.TypesChangedEarly -= new EventHandler<TypesChangedEventArgs>(this.TypeResolver_TypesChanging);
        oldContext.TypeResolver.TypesChanged -= new EventHandler<TypesChangedEventArgs>(this.TypeResolver_TypesChanged);
      }
      if (newContext == null)
        return;
      newContext.TypeResolver.TypesChangedEarly += new EventHandler<TypesChangedEventArgs>(this.TypeResolver_TypesChanging);
      newContext.TypeResolver.TypesChanged += new EventHandler<TypesChangedEventArgs>(this.TypeResolver_TypesChanged);
    }

    protected virtual bool ShouldTrackTextChange()
    {
      if (!this.isSerializing)
        return !this.IsSettingTextDuringUndo;
      return false;
    }

    private void OnTextChanged(object sender, TextChangedEventArgs args)
    {
      if (this.ShouldTrackTextChange())
      {
        if (this.TextChanged != null)
          this.TextChanged((object) this, (EventArgs) null);
      }
      else
        this.ListenToTextChanges();
      if (this.HasTextEditsChanged == null)
        return;
      this.HasTextEditsChanged((object) this, EventArgs.Empty);
    }

    private void ListenToTextChanges()
    {
      if (this.textChanges != null)
        this.textChanges.Dispose();
      this.textChanges = this.textBuffer.RecordChanges(new ShouldTrackChange(this.ShouldTrackTextChange));
    }

    public DocumentNode GetLowestNodeContainingRange(ITextRange range, bool xmlElementsOnly, bool matchHighestPotentialNode, out ITextRange nodeRange)
    {
      if (this.RootNode != null && !TextRange.IsNull(range))
        return this.GetLowestNodeContainingRange(this.RootNode, range, xmlElementsOnly, matchHighestPotentialNode, out nodeRange) ?? this.RootNode;
      nodeRange = TextRange.Null;
      return (DocumentNode) null;
    }

    private DocumentNode GetLowestNodeContainingRange(DocumentNode node, ITextRange range, bool xmlElementsOnly, bool matchHighestPotentialNode, out ITextRange nodeRange)
    {
      XamlSourceContext xamlSourceContext = node.SourceContext as XamlSourceContext;
      ITextRange range1 = TextRange.Null;
      if (xamlSourceContext != null && xamlSourceContext.TextRange != null)
      {
        if (xmlElementsOnly && !(xamlSourceContext is XmlElementReference))
        {
          nodeRange = TextRange.Null;
          return (DocumentNode) null;
        }
        range1 = xamlSourceContext.TextRange;
        if (!TextRange.Contains(range1, range))
        {
          nodeRange = TextRange.Null;
          return (DocumentNode) null;
        }
      }
      foreach (DocumentNode node1 in node.ChildNodes)
      {
        ITextRange nodeRange1;
        DocumentNode documentNode = this.GetLowestNodeContainingRange(node1, range, xmlElementsOnly, matchHighestPotentialNode, out nodeRange1);
        if (documentNode != null)
        {
          if (matchHighestPotentialNode && range1.Offset == nodeRange1.Offset && range1.Length == nodeRange1.Length)
            documentNode = node;
          nodeRange = nodeRange1;
          return documentNode;
        }
      }
      if (!TextRange.IsNull(range1))
      {
        nodeRange = range1;
        return node;
      }
      nodeRange = TextRange.Null;
      return (DocumentNode) null;
    }

    private void TypeReferences_IsEditableChanged(object sender, EventArgs e)
    {
      this.OnPropertyChanged("UnresolvedTypes");
      this.OnPropertyChanged("IsEditable");
    }

    private bool ReparseNeeded(TypesChangedEventArgs e)
    {
      if (this.ParseErrorsCount > 0 || this.AnyReferencesToAssemblies(e.InvalidatedAssemblies))
        return true;
      IDocumentContext documentContext = this.DocumentContext;
      if (documentContext != null)
        return documentContext.IsUsingDesignTimeTypes;
      return false;
    }

    private void TypeResolver_TypesChanging(object sender, TypesChangedEventArgs e)
    {
      if (!this.ReparseNeeded(e))
        return;
      this.ReparseDocument();
    }

    private void TypeResolver_TypesChanged(object sender, TypesChangedEventArgs e)
    {
      if (this.ReparseNeeded(e))
        return;
      int count1 = this.typeReferences.UnresolvedTypes.Count;
      this.typeReferences.ResolveAllTypes();
      int count2 = this.typeReferences.UnresolvedTypes.Count;
      if (count1 != count2 && count2 == 0)
      {
        this.OnRootNodeChangingOutsideUndo();
        this.OnRootNodeChanging();
        this.SetDocumentRoot(this.RootNode);
        this.OnRootNodeChanged();
        this.OnRootNodeChangedOutsideUndo();
      }
      else
        this.OnTypesChanged();
    }

    private static bool RemoveAttribute(XmlElementReference element, XmlAttribute attribute)
    {
      return element.RemoveMatchingAttributes((Func<XmlElementReference.Attribute, bool>) (target =>
      {
        if (string.Equals(target.XmlAttribute.Value.Value, attribute.Value.Value, StringComparison.Ordinal) && string.Equals(target.XmlAttribute.LocalName, attribute.LocalName, StringComparison.Ordinal))
          return string.Equals(target.XmlAttribute.Prefix, attribute.Prefix, StringComparison.Ordinal);
        return false;
      }));
    }

    private class IncrementalTreeChange
    {
      private List<DocumentNode> names;
      private XamlDocument document;
      private DocumentNode rootNode;

      public IncrementalTreeChange(XamlDocument document)
      {
        this.document = document;
      }

      public void Apply(DocumentNode treeNode, DocumentNode newlyParsedNode)
      {
        this.names = new List<DocumentNode>();
        Dictionary<DocumentNodeNameScope, List<DocumentNode>> actions1 = new Dictionary<DocumentNodeNameScope, List<DocumentNode>>();
        this.BuildNamesToRemoveFromNameScope(treeNode.FindContainingNameScope(), treeNode, actions1);
        this.document.ApplyNamescopeUpdate(actions1, true);
        this.rootNode = treeNode;
        this.ApplyMinimalChange(treeNode, newlyParsedNode);
        Dictionary<DocumentNodeNameScope, List<DocumentNode>> actions2 = new Dictionary<DocumentNodeNameScope, List<DocumentNode>>();
        foreach (DocumentNode documentNode in this.names)
        {
          DocumentNodeNameScope containingNameScope = documentNode.FindContainingNameScope();
          if (containingNameScope != null)
          {
            List<DocumentNode> list;
            if (!actions2.TryGetValue(containingNameScope, out list))
            {
              list = new List<DocumentNode>();
              actions2.Add(containingNameScope, list);
            }
            list.Add(documentNode);
          }
        }
        this.document.ApplyNamescopeUpdate(actions2, false);
      }

      private void BuildNamesToRemoveFromNameScope(DocumentNodeNameScope nameScope, DocumentNode node, Dictionary<DocumentNodeNameScope, List<DocumentNode>> actions)
      {
        if (nameScope != null && node.Name != null)
        {
          List<DocumentNode> list;
          if (!actions.TryGetValue(nameScope, out list))
          {
            list = new List<DocumentNode>();
            actions.Add(nameScope, list);
          }
          list.Add(node);
        }
        if (node.NameScope != null)
          nameScope = node.NameScope;
        foreach (DocumentNode node1 in node.ChildNodes)
          this.BuildNamesToRemoveFromNameScope(nameScope, node1, actions);
      }

      private bool ApplyMinimalChange(DocumentNode oldNode, DocumentNode newNode)
      {
        DocumentCompositeNode documentCompositeNode1 = oldNode as DocumentCompositeNode;
        DocumentCompositeNode documentCompositeNode2 = newNode as DocumentCompositeNode;
        bool flag1 = false;
        if (documentCompositeNode1 != null && documentCompositeNode2 != null)
        {
          if (!documentCompositeNode1.Type.Equals((object) documentCompositeNode2.Type))
          {
            flag1 = true;
          }
          else
          {
            int index1 = 0;
            int index2 = 0;
            while (index1 < documentCompositeNode1.Properties.Count && index2 < documentCompositeNode2.Properties.Count)
            {
              switch (documentCompositeNode1.Properties.Keys[index1].SortValue.CompareTo(documentCompositeNode2.Properties.Keys[index2].SortValue))
              {
                case 0:
                  bool flag2 = this.ApplyMinimalChange(documentCompositeNode1.Properties[index1], documentCompositeNode2.Properties[index2]);
                  ++index1;
                  if (!flag2)
                  {
                    ++index2;
                    continue;
                  }
                  continue;
                case -1:
                  this.ApplyMinimalChange(documentCompositeNode1.Properties[index1], (DocumentNode) null);
                  continue;
                case 1:
                  DocumentNode node1 = documentCompositeNode2.Properties[index2];
                  IProperty sitePropertyKey1 = node1.SitePropertyKey;
                  INodeSourceContext containerSourceContext1 = node1.ContainerSourceContext;
                  documentCompositeNode2.Properties[(IPropertyId) sitePropertyKey1] = (DocumentNode) null;
                  documentCompositeNode1.Properties[(IPropertyId) sitePropertyKey1] = node1;
                  this.document.SetContainerSourceContext(node1, containerSourceContext1);
                  ++index1;
                  continue;
                default:
                  continue;
              }
            }
            while (index1 < documentCompositeNode1.Properties.Count)
              this.ApplyMinimalChange(documentCompositeNode1.Properties[index1], (DocumentNode) null);
            while (index2 < documentCompositeNode2.Properties.Count)
            {
              DocumentNode node2 = documentCompositeNode2.Properties[index2];
              IProperty sitePropertyKey2 = node2.SitePropertyKey;
              INodeSourceContext containerSourceContext2 = node2.ContainerSourceContext;
              documentCompositeNode2.Properties[(IPropertyId) sitePropertyKey2] = (DocumentNode) null;
              documentCompositeNode1.Properties[(IPropertyId) sitePropertyKey2] = node2;
              this.document.SetContainerSourceContext(node2, containerSourceContext2);
            }
            if (documentCompositeNode1.Children != null)
            {
              int index3 = 0;
              int index4 = 0;
              while (index3 < documentCompositeNode1.Children.Count && index4 < documentCompositeNode2.Children.Count)
              {
                DocumentNode d1 = documentCompositeNode1.Children[index3];
                DocumentNode d2 = documentCompositeNode2.Children[index4];
                if (this.AreEquivalentForDiff(d1, d2))
                {
                  bool flag3 = this.ApplyMinimalChange(documentCompositeNode1.Children[index3], documentCompositeNode2.Children[index4]);
                  ++index3;
                  if (!flag3)
                    ++index4;
                }
                else if (index3 + 1 < documentCompositeNode1.Children.Count && this.AreEquivalentForDiff(documentCompositeNode1.Children[index3 + 1], d2))
                {
                  this.ApplyMinimalChange(documentCompositeNode1.Children[index3], (DocumentNode) null);
                }
                else
                {
                  DocumentNode documentNode = documentCompositeNode2.Children[index4];
                  documentCompositeNode2.Children.RemoveAt(index4);
                  documentCompositeNode1.Children.Insert(index3, documentNode);
                  ++index3;
                }
              }
              while (index3 < documentCompositeNode1.Children.Count)
                this.ApplyMinimalChange(documentCompositeNode1.Children[index3], (DocumentNode) null);
              while (index4 < documentCompositeNode2.Children.Count)
              {
                DocumentNode documentNode = documentCompositeNode2.Children[index4];
                documentCompositeNode2.Children.RemoveAt(index4);
                documentCompositeNode1.Children.Add(documentNode);
              }
            }
            XmlElementReference elementReference = documentCompositeNode2.SourceContext as XmlElementReference;
            this.document.SetSourceContext((DocumentNode) documentCompositeNode1, (INodeSourceContext) elementReference);
            if (documentCompositeNode1 != this.rootNode)
              this.document.SetContainerSourceContext((DocumentNode) documentCompositeNode1, documentCompositeNode2.ContainerSourceContext);
            if (documentCompositeNode1.Name != null)
              this.names.Add((DocumentNode) documentCompositeNode1);
          }
        }
        else if (oldNode != null && newNode != null && !oldNode.Equals(newNode) || (oldNode == null || newNode == null))
          flag1 = true;
        if (flag1)
        {
          if (oldNode.IsProperty)
          {
            if (newNode != null && newNode.Parent != null)
            {
              this.document.SetContainerSourceContext(oldNode, newNode.ContainerSourceContext);
              newNode.Parent.Properties[(IPropertyId) newNode.SitePropertyKey] = (DocumentNode) null;
            }
            oldNode.Parent.Properties[(IPropertyId) oldNode.SitePropertyKey] = newNode;
          }
          else if (newNode != null)
          {
            if (newNode.Parent != null)
              newNode.Parent.Children.RemoveAt(newNode.SiteChildIndex);
            oldNode.Parent.Children[oldNode.SiteChildIndex] = newNode;
          }
          else
            oldNode.Parent.Children.RemoveAt(oldNode.SiteChildIndex);
        }
        else if ((documentCompositeNode1 == null || documentCompositeNode2 == null) && (oldNode != null && newNode != null))
        {
          this.document.SetSourceContext(oldNode, newNode.SourceContext);
          this.document.SetContainerSourceContext(oldNode, newNode.ContainerSourceContext);
        }
        return flag1;
      }

      private bool AreEquivalentForDiff(DocumentNode d1, DocumentNode d2)
      {
        DocumentCompositeNode documentCompositeNode1 = d1 as DocumentCompositeNode;
        DocumentCompositeNode documentCompositeNode2 = d2 as DocumentCompositeNode;
        if (documentCompositeNode1 == null || documentCompositeNode2 == null || !documentCompositeNode1.Type.Equals((object) documentCompositeNode2.Type))
          return false;
        if (this.document.TypeResolver.PlatformMetadata.KnownTypes.DictionaryEntry.Equals((object) documentCompositeNode1.Type))
        {
          DocumentNode documentNode1 = documentCompositeNode1.Properties[this.document.TypeResolver.PlatformMetadata.KnownProperties.DictionaryEntryValue];
          DocumentNode documentNode2 = documentCompositeNode2.Properties[this.document.TypeResolver.PlatformMetadata.KnownProperties.DictionaryEntryValue];
          if (documentNode1 == null || documentNode2 == null || !documentNode1.Type.Equals((object) documentNode2.Type))
            return false;
        }
        if (documentCompositeNode1.Children != null && documentCompositeNode2.Children != null && documentCompositeNode1.Children.Count == documentCompositeNode2.Children.Count)
          return true;
        if (documentCompositeNode1.Children == null)
          return documentCompositeNode2.Children == null;
        return false;
      }
    }

    private static class StableSortHelper
    {
      public static void StableSort<T>(List<T> list, Comparison<T> comparison)
      {
        for (int index1 = 1; index1 < list.Count; ++index1)
        {
          T y = list[index1];
          int index2;
          for (index2 = index1 - 1; index2 >= 0 && comparison(list[index2], y) > 0; --index2)
            list[index2 + 1] = list[index2];
          list[index2 + 1] = y;
        }
      }
    }

    protected sealed class ModifyRootXmlAttributesUndoable
    {
      private bool isUndoing = true;
      private XmlElementReference element;
      private IEnumerable<KeyValuePair<XmlAttribute, XmlAttribute>> attributes;

      internal ModifyRootXmlAttributesUndoable(XmlElementReference element, IEnumerable<KeyValuePair<XmlAttribute, XmlAttribute>> attributes)
      {
        this.element = element;
        this.attributes = attributes;
      }

      public void Invoke()
      {
        this.isUndoing = !this.isUndoing;
        if (this.isUndoing)
        {
          foreach (KeyValuePair<XmlAttribute, XmlAttribute> keyValuePair in this.attributes)
          {
            if (keyValuePair.Value != null)
              XamlDocument.RemoveAttribute(this.element, keyValuePair.Value);
            if (keyValuePair.Key != null)
              this.element.AddAttributeToPreserve(new XmlElementReference.Attribute(XmlElementReference.AttributeType.SerializerAddedXmlns, keyValuePair.Key, (XamlSourceContext) null));
          }
        }
        else
        {
          foreach (KeyValuePair<XmlAttribute, XmlAttribute> keyValuePair in this.attributes)
          {
            if (keyValuePair.Key != null)
              XamlDocument.RemoveAttribute(this.element, keyValuePair.Key);
            if (keyValuePair.Value != null)
              this.element.AddAttributeToPreserve(new XmlElementReference.Attribute(XmlElementReference.AttributeType.SerializerAddedXmlns, keyValuePair.Value, (XamlSourceContext) null));
          }
        }
      }
    }

    protected sealed class ApplyNamescopeUpdateUndoable
    {
      private Dictionary<DocumentNodeNameScope, List<DocumentNode>> nodes;
      private bool isRemoving;

      public ApplyNamescopeUpdateUndoable(Dictionary<DocumentNodeNameScope, List<DocumentNode>> nodes, bool isRemoving)
      {
        this.nodes = nodes;
        this.isRemoving = !isRemoving;
      }

      public void Invoke()
      {
        this.isRemoving = !this.isRemoving;
        foreach (KeyValuePair<DocumentNodeNameScope, List<DocumentNode>> keyValuePair in this.nodes)
        {
          foreach (DocumentNode node in keyValuePair.Value)
          {
            if (this.isRemoving)
            {
              keyValuePair.Key.RemoveNode(node.Name);
            }
            else
            {
              string name = node.Name;
              if (keyValuePair.Key.FindNode(name) == null)
                keyValuePair.Key.AddNode(name, node);
            }
          }
        }
      }
    }

    protected sealed class SetTextUndoable
    {
      private bool isUndoing = true;
      private XamlDocument document;
      private string text;
      private DocumentNode rootNode;
      private XmlDocumentReference xmlDocumentReference;
      private IList<XamlParseError> errorsAndWarnings;
      private bool isParsed;

      public SetTextUndoable(XamlDocument document, string text)
      {
        this.document = document;
        this.text = text;
      }

      public void Invoke()
      {
        this.isUndoing = !this.isUndoing;
        if (!this.isParsed)
        {
          this.isParsed = true;
          this.document.isSettingTextDuringUndo = true;
          this.document.TextBuffer.SetText(0, this.document.TextBuffer.Length, this.text);
          this.document.isSettingTextDuringUndo = false;
          XamlParserResults xamlParserResults = new XamlParser(this.document.DocumentContext, (IReadableSelectableTextBuffer) this.document.TextBuffer, this.document.ExpectedRootType).Parse();
          this.rootNode = xamlParserResults.RootNode;
          this.errorsAndWarnings = xamlParserResults.Errors;
          this.xmlDocumentReference = xamlParserResults.XmlDocumentReference;
        }
        else
        {
          this.document.IsUndoingTextChange = true;
          if (this.isUndoing)
            this.document.TextBuffer.Undo();
          else
            this.document.TextBuffer.Redo();
          this.document.IsUndoingTextChange = false;
        }
        XmlDocumentReference documentReference = this.document.XmlDocumentReference;
        this.document.xmlDocumentReference = this.xmlDocumentReference;
        this.xmlDocumentReference = documentReference;
        IList<XamlParseError> list = this.document.parseErrorsAndWarnings;
        this.document.parseErrorsAndWarnings = this.errorsAndWarnings;
        this.errorsAndWarnings = list;
        DocumentNode rootNode = this.document.RootNode;
        this.document.OnRootNodeChanging();
        this.document.SetDocumentRoot(this.rootNode);
        this.document.OnRootNodeChanged();
        this.rootNode = rootNode;
        this.document.ListenToTextChanges();
        this.document.documentChangesSinceSerialize.Clear();
        this.document.FireRootChangedNotifications();
      }
    }

    private class SourceContextUndo
    {
      public DocumentNode DocumentNode { get; private set; }

      public INodeSourceContext SourceContext { get; private set; }

      public INodeSourceContext ContainerSourceContext { get; private set; }

      public SourceContextUndo(DocumentNode node, INodeSourceContext sourceContext, INodeSourceContext containerSourceContext)
      {
        this.DocumentNode = node;
        this.SourceContext = sourceContext;
        this.ContainerSourceContext = containerSourceContext;
      }
    }
  }
}
