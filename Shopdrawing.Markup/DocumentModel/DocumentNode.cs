// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.DocumentModel.DocumentNode
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Expression.DesignModel.DocumentModel
{
  public abstract class DocumentNode
  {
    private static EmptyList<DocumentNode> emptyChildNodes = new EmptyList<DocumentNode>();
    private readonly IDocumentContext context;
    private IDocumentRoot documentRoot;
    private INodeSourceContext sourceContext;
    private INodeSourceContext oldContainerSourceContext;
    private IType type;
    private DocumentNodeMarker marker;
    private object sceneNode;
    private DocumentCompositeNode parent;
    private IProperty propertyKey;
    private int childIndex;

    public IDocumentContext Context
    {
      get
      {
        return this.context;
      }
    }

    public ITypeResolver TypeResolver
    {
      get
      {
        return this.context.TypeResolver;
      }
    }

    public IPlatformMetadata PlatformMetadata
    {
      get
      {
        return this.context.TypeResolver.PlatformMetadata;
      }
    }

    public IType Type
    {
      get
      {
        return this.type;
      }
      set
      {
        this.type = value;
      }
    }

    public System.Type TargetType
    {
      get
      {
        return this.type.NearestResolvedType.RuntimeType;
      }
    }

    public IDocumentRoot DocumentRoot
    {
      get
      {
        return this.documentRoot;
      }
    }

    public DocumentCompositeNode Parent
    {
      get
      {
        return this.parent;
      }
    }

    public bool IsChild
    {
      get
      {
        return this.propertyKey == null;
      }
    }

    public bool IsProperty
    {
      get
      {
        return this.propertyKey != null;
      }
    }

    public IProperty SitePropertyKey
    {
      get
      {
        return this.propertyKey;
      }
    }

    public int SiteChildIndex
    {
      get
      {
        this.parent.ValidateChildIndices(this.childIndex);
        return this.childIndex;
      }
      internal set
      {
        this.childIndex = value;
      }
    }

    public object SceneNode
    {
      get
      {
        return this.sceneNode;
      }
      set
      {
        this.sceneNode = value;
      }
    }

    public INodeSourceContext SourceContext
    {
      get
      {
        return this.sourceContext;
      }
      set
      {
        this.sourceContext = value;
      }
    }

    public INodeSourceContext ContainerSourceContext
    {
      get
      {
        INodeSourceContext nodeSourceContext = (INodeSourceContext) null;
        if (this.Parent != null && this.IsProperty)
          nodeSourceContext = this.Parent.GetContainerContext((IPropertyId) this.SitePropertyKey);
        if (this.oldContainerSourceContext != null)
          return this.oldContainerSourceContext;
        return nodeSourceContext;
      }
      set
      {
        if (this.Parent == null || !this.IsProperty)
          return;
        this.Parent.SetContainerContext((IPropertyId) this.SitePropertyKey, value);
      }
    }

    public IProperty NameProperty
    {
      get
      {
        return this.Type.Metadata.NameProperty;
      }
    }

    public virtual string Name
    {
      get
      {
        return (string) null;
      }
      set
      {
      }
    }

    public virtual DocumentNodeNameScope NameScope
    {
      get
      {
        return (DocumentNodeNameScope) null;
      }
      set
      {
      }
    }

    public bool IsSubclassDefinition
    {
      get
      {
        if (this.documentRoot != null && this == this.documentRoot.RootNode)
          return this.documentRoot.RootClassAttributes != null;
        return false;
      }
    }

    public virtual IEnumerable<DocumentNode> ChildNodes
    {
      get
      {
        return (IEnumerable<DocumentNode>) DocumentNode.emptyChildNodes;
      }
    }

    public virtual int ChildNodesCount
    {
      get
      {
        return 0;
      }
    }

    public virtual IEnumerable<DocumentNode> DescendantNodes
    {
      get
      {
        return (IEnumerable<DocumentNode>) DocumentNode.emptyChildNodes;
      }
    }

    public IEnumerable<DocumentNode> AncestorNodes
    {
      get
      {
        for (DocumentNode cur = (DocumentNode) this.Parent; cur != null; cur = (DocumentNode) cur.Parent)
          yield return cur;
      }
    }

    public DocumentNodeMarker Marker
    {
      get
      {
        if (this.marker == null && this.DocumentRoot != null)
        {
          this.marker = new DocumentNodeMarker(this);
          this.marker.InitializeChildIndex();
        }
        return this.marker;
      }
    }

    public bool IsInDocument
    {
      get
      {
        return this.documentRoot != null;
      }
    }

    protected DocumentNode(IDocumentContext context, ITypeId type)
    {
      this.context = context;
      if (this.context == null)
        throw new ArgumentNullException("context");
      this.type = context.TypeResolver.ResolveType(type);
      if (this.type == null)
        throw new ArgumentNullException("type");
    }

    internal void SetSitePropertyKey(DocumentCompositeNode parent, IProperty propertyKey)
    {
      if (parent == null)
        throw new ArgumentNullException("parent");
      if (propertyKey == null)
        throw new ArgumentNullException("propertyKey");
      this.SetSite(parent, propertyKey, -1);
    }

    internal void SetSiteChildIndex(DocumentCompositeNode parent, int childIndex)
    {
      if (parent == null)
        throw new ArgumentNullException("parent");
      if (childIndex < 0)
        throw new ArgumentOutOfRangeException("childIndex", (object) childIndex, "must be >= 0");
      this.SetSite(parent, (IProperty) null, childIndex);
    }

    internal void ClearSite()
    {
      this.SetSite((DocumentCompositeNode) null, (IProperty) null, -1);
    }

    internal virtual void SetSite(DocumentCompositeNode parent, IProperty propertyKey, int childIndex)
    {
      if (parent == null)
      {
        this.oldContainerSourceContext = this.ContainerSourceContext;
        DocumentNode.UpdateNameScopes((DocumentNode) this.Parent, this.SitePropertyKey, this, false);
        this.parent = (DocumentCompositeNode) null;
        this.propertyKey = (IProperty) null;
        this.childIndex = -1;
        this.SetDocumentRoot((IDocumentRoot) null);
      }
      else
      {
        if (this.parent != null)
          throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.DocumentNodeCannotReparentNode, (object) this, (object) this.Parent, (object) parent));
        this.parent = parent;
        this.propertyKey = propertyKey;
        this.childIndex = childIndex;
        this.SetDocumentRoot(parent.DocumentRoot);
        DocumentNode.UpdateNameScopes((DocumentNode) parent, propertyKey, this, true);
      }
    }

    public void ClearOldSourceContainerContext()
    {
      this.oldContainerSourceContext = (INodeSourceContext) null;
    }

    public bool IsNameProperty(IPropertyId propertyKey)
    {
      return this.Type.Metadata.IsNameProperty(propertyKey);
    }

    public DocumentNodeNameScope FindContainingNameScope()
    {
      if (this.Parent != null)
        return this.Parent.FindNameScopeForChildren();
      if (this.NameScope != null)
        return this.NameScope;
      return (DocumentNodeNameScope) null;
    }

    public DocumentNodeNameScope FindNameScopeForChildren()
    {
      if (this.NameScope != null)
        return this.NameScope;
      return this.FindContainingNameScope();
    }

    private static void UpdateNameScopes(DocumentNode parent, IProperty propertyKey, DocumentNode node, bool adding)
    {
      if (parent == null)
        return;
      if (node is DocumentPrimitiveNode)
      {
        if (propertyKey == null || !parent.IsNameProperty((IPropertyId) propertyKey))
          return;
        DocumentNodeNameScope containingNameScope = parent.FindContainingNameScope();
        if (containingNameScope == null)
          return;
        string valueAsString = DocumentPrimitiveNode.GetValueAsString(node);
        if (valueAsString == null)
          return;
        if (adding)
          containingNameScope.AddNode(valueAsString, parent);
        else
          containingNameScope.RemoveNode(valueAsString);
      }
      else
      {
        DocumentNodeNameScope containingNameScope = node.FindContainingNameScope();
        if (containingNameScope == null)
          return;
        DocumentNode.UpdateNameScopesRecursive(containingNameScope, node, adding);
      }
    }

    private static void UpdateNameScopesRecursive(DocumentNodeNameScope nameScope, DocumentNode node, bool adding)
    {
      if (node.Name != null)
      {
        if (adding)
          nameScope.AddNode(node.Name, node);
        else
          nameScope.RemoveNode(node.Name);
      }
      if (node.NameScope != null)
        return;
      DocumentCompositeNode documentCompositeNode = node as DocumentCompositeNode;
      if (documentCompositeNode == null)
        return;
      if (documentCompositeNode.SupportsChildren)
      {
        for (int index = 0; index < documentCompositeNode.Children.Count; ++index)
          DocumentNode.UpdateNameScopesRecursive(nameScope, documentCompositeNode.Children[index], adding);
      }
      for (int index = 0; index < documentCompositeNode.Properties.Count; ++index)
        DocumentNode.UpdateNameScopesRecursive(nameScope, documentCompositeNode.Properties[index], adding);
    }

    public abstract DocumentNode Clone(IDocumentContext context);

    public abstract bool Equals(DocumentNode other);

    public abstract int GetHashCodeInternal();

    public IProperty GetValueProperty()
    {
      if (this.Parent != null)
      {
        DocumentCompositeNode parent = this.Parent;
        IType type = parent.Type;
        IProperty sitePropertyKey = this.SitePropertyKey;
        if (sitePropertyKey != null)
        {
          IPropertyValueTypeMetadata valueTypeMetadata = type.Metadata as IPropertyValueTypeMetadata;
          if (valueTypeMetadata != null && sitePropertyKey.Equals((object) valueTypeMetadata.ValueProperty))
            return DocumentPrimitiveNode.GetValueAsMember(parent.Properties[valueTypeMetadata.PropertyProperty]) as IProperty;
          return sitePropertyKey;
        }
      }
      return (IProperty) null;
    }

    public bool IsAncestorOf(DocumentNode other)
    {
      for (; other != null; other = (DocumentNode) other.Parent)
      {
        if (this == other)
          return true;
      }
      return false;
    }

    public IEnumerable<DocumentNode> SelectChildNodes(System.Type targetType)
    {
      foreach (DocumentNode documentNode in this.ChildNodes)
      {
        if (targetType.IsAssignableFrom(documentNode.TargetType))
          yield return documentNode;
      }
    }

    public IEnumerable<DocumentNode> SelectChildNodes(Predicate<DocumentNode> predicate)
    {
      foreach (DocumentNode documentNode in this.ChildNodes)
      {
        if (predicate(documentNode))
          yield return documentNode;
      }
    }

    public IEnumerable<DocumentNode> SelectDescendantNodes(ITypeId targetTypeId)
    {
      foreach (DocumentNode documentNode in this.DescendantNodes)
      {
        if (targetTypeId.IsAssignableFrom((ITypeId) documentNode.Type))
          yield return documentNode;
      }
    }

    public IEnumerable<DocumentNode> SelectDescendantNodes(System.Type targetType)
    {
      foreach (DocumentNode documentNode in this.DescendantNodes)
      {
        if (targetType.IsAssignableFrom(documentNode.TargetType))
          yield return documentNode;
      }
    }

    public IEnumerable<DocumentNode> SelectDescendantNodes(Predicate<DocumentNode> predicate)
    {
      foreach (DocumentNode documentNode in this.DescendantNodes)
      {
        if (predicate(documentNode))
          yield return documentNode;
      }
    }

    public DocumentNode SelectFirstDescendantNode(System.Type targetType)
    {
      IEnumerator<DocumentNode> enumerator = this.SelectDescendantNodes(targetType).GetEnumerator();
      if (enumerator.MoveNext())
        return enumerator.Current;
      return (DocumentNode) null;
    }

    public DocumentNode SelectFirstDescendantNode(Predicate<DocumentNode> predicate)
    {
      IEnumerator<DocumentNode> enumerator = this.SelectDescendantNodes(predicate).GetEnumerator();
      if (enumerator.MoveNext())
        return enumerator.Current;
      return (DocumentNode) null;
    }

    public IEnumerable<DocumentNode> SelectAncestorNodes(System.Type targetType)
    {
      foreach (DocumentNode documentNode in this.AncestorNodes)
      {
        if (targetType.IsAssignableFrom(documentNode.TargetType))
          yield return documentNode;
      }
    }

    public IEnumerable<DocumentNode> SelectAncestorNodes(Predicate<DocumentNode> predicate)
    {
      foreach (DocumentNode documentNode in this.AncestorNodes)
      {
        if (predicate(documentNode))
          yield return documentNode;
      }
    }

    public DocumentNode SelectFirstAncestorNode(System.Type targetType)
    {
      IEnumerator<DocumentNode> enumerator = this.SelectAncestorNodes(targetType).GetEnumerator();
      if (enumerator.MoveNext())
        return enumerator.Current;
      return (DocumentNode) null;
    }

    public DocumentNode SelectFirstAncestorNode(Predicate<DocumentNode> predicate)
    {
      IEnumerator<DocumentNode> enumerator = this.SelectAncestorNodes(predicate).GetEnumerator();
      if (enumerator.MoveNext())
        return enumerator.Current;
      return (DocumentNode) null;
    }

    public DocumentNode FindFirst(Predicate<DocumentNode> predicate)
    {
      if (predicate(this))
        return this;
      foreach (DocumentNode documentNode in this.ChildNodes)
      {
        DocumentNode first = documentNode.FindFirst(predicate);
        if (first != null)
          return first;
      }
      return (DocumentNode) null;
    }

    public override string ToString()
    {
      return this.type.Name;
    }

    public virtual void SetDocumentRoot(IDocumentRoot documentRoot)
    {
      this.SetDocumentRootRecursively(documentRoot);
    }

    private void SetDocumentRootRecursively(IDocumentRoot documentRoot)
    {
      if (this.documentRoot == documentRoot)
        return;
      if (this.documentRoot != null)
        this.documentRoot.OnNodeUnsetDocumentRoot(this);
      this.documentRoot = documentRoot;
      if (this.documentRoot != null)
        this.documentRoot.OnNodeSetDocumentRoot(this);
      else if (this.marker != null)
      {
        this.marker.SetDeleted();
        this.marker = (DocumentNodeMarker) null;
      }
      foreach (DocumentNode documentNode in this.ChildNodes)
        documentNode.SetDocumentRootRecursively(documentRoot);
    }

    internal void FixupMarkerChildIndex(int newIndex, DocumentNode previousSibling)
    {
      if (this.marker == null)
        return;
      this.marker.FixupChildIndex(newIndex, previousSibling != null ? previousSibling.Marker : (DocumentNodeMarker) null);
    }

    internal static CanAssignResult CanAssignTo(ITypeResolver typeResolver, IType destinationTypeId, DocumentNode valueNode)
    {
      if (!DocumentPrimitiveNode.IsNull(valueNode))
        return DocumentNode.CanAssignTo(typeResolver, (ITypeId) destinationTypeId, (ITypeId) valueNode.Type);
      return !destinationTypeId.SupportsNullValues ? CanAssignResult.NotNullable : CanAssignResult.CanAssign;
    }

    internal static CanAssignResult CanAssignTo(ITypeResolver typeResolver, ITypeId destinationTypeId, ITypeId sourceTypeId)
    {
      IPlatformMetadata platformMetadata = typeResolver.PlatformMetadata;
      IType type1 = typeResolver.ResolveType(destinationTypeId);
      IType type2 = typeResolver.ResolveType(sourceTypeId);
      if (platformMetadata.KnownTypes.NullExtension.IsAssignableFrom((ITypeId) type2))
        return !type1.SupportsNullValues ? CanAssignResult.NotNullable : CanAssignResult.CanAssign;
      if (type2 == type1)
        return CanAssignResult.CanAssign;
      if (platformMetadata.KnownTypes.ArrayExtension.IsAssignableFrom((ITypeId) type2))
      {
        if (type1.IsArray)
          return CanAssignResult.CanAssign;
        System.Type runtimeType = type1.RuntimeType;
        if (runtimeType == (System.Type) null)
          return CanAssignResult.Unknown;
        if (runtimeType.IsAssignableFrom(typeof (Array)))
          return CanAssignResult.CanAssign;
      }
      else
      {
        if (type2.IsExpression)
          return CanAssignResult.Expression;
        if (platformMetadata.KnownTypes.XData.Equals((object) type2))
          return type1.Metadata.SupportsInlineXml ? CanAssignResult.CanAssign : CanAssignResult.NotXmlSerializable;
        System.Type runtimeType1 = type2.RuntimeType;
        System.Type runtimeType2 = type1.RuntimeType;
        if (runtimeType2 == (System.Type) null || runtimeType1 == (System.Type) null)
          return CanAssignResult.Unknown;
        if (platformMetadata.KnownTypes.Expression.IsAssignableFrom((ITypeId) type2))
          return CanAssignResult.Expression;
        if (runtimeType2.IsAssignableFrom(runtimeType1))
          return CanAssignResult.CanAssign;
      }
      return CanAssignResult.CannotAssign;
    }
  }
}
