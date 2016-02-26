// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.DocumentModel.DocumentCompositeNode
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Microsoft.Expression.DesignModel.DocumentModel
{
  public sealed class DocumentCompositeNode : DocumentNode
  {
    private IConstructor constructor;
    private DocumentCompositeNode.ConstructorArgumentNodeCollection constructorArguments;
    private DocumentCompositeNode.PropertyDictionary properties;
    private DocumentCompositeNode.DocumentNodeCollection children;
    private DocumentNodeNameScope nameScope;
    private bool explicitCollection;

    public IConstructor Constructor
    {
      get
      {
        return this.constructor;
      }
    }

    private bool HasConstructorArguments
    {
      get
      {
        if (this.constructorArguments != null)
          return this.constructorArguments.Count > 0;
        return false;
      }
    }

    public IConstructorArgumentNodeCollection ConstructorArguments
    {
      get
      {
        if (this.constructorArguments != null)
          return (IConstructorArgumentNodeCollection) this.constructorArguments;
        return (IConstructorArgumentNodeCollection) DocumentCompositeNode.EmptyConstructorArgumentNodeCollection.Instance;
      }
    }

    public bool IsConstructorValid
    {
      get
      {
        if (this.constructorArguments != null)
        {
          int num = 0;
          foreach (DocumentCompositeNode.ConstructorArgumentNode constructorArgumentNode in (IEnumerable<DocumentCompositeNode.ConstructorArgumentNode>) this.constructorArguments.Collection)
          {
            IProperty property = constructorArgumentNode.Property;
            if (property == null || !TypeHelper.IsPropertyWritable(this.Context.TypeResolver, property, false))
              return true;
          }
          if (num == 0)
            return true;
        }
        return false;
      }
    }

    public bool IsExplicitCollection
    {
      get
      {
        return this.explicitCollection;
      }
      set
      {
        this.explicitCollection = value;
      }
    }

    public override IEnumerable<DocumentNode> ChildNodes
    {
      get
      {
        return (IEnumerable<DocumentNode>) new DocumentCompositeNode.ChildNodesEnumerable((DocumentNode) this);
      }
    }

    public override int ChildNodesCount
    {
      get
      {
        return this.properties.Count + (this.children != null ? this.children.Count : 0);
      }
    }

    public override string Name
    {
      get
      {
        IPropertyId propertyKey = (IPropertyId) this.NameProperty;
        if (propertyKey == null)
          return (string) null;
        return this.GetValueAsString(propertyKey);
      }
      set
      {
        DocumentNode documentNode = (DocumentNode) null;
        if (value != null)
          documentNode = (DocumentNode) this.Context.CreateNode(value);
        this.Properties[(IPropertyId) this.NameProperty] = documentNode;
      }
    }

    public override DocumentNodeNameScope NameScope
    {
      get
      {
        return this.nameScope;
      }
      set
      {
        this.nameScope = value;
      }
    }

    public IDocumentNodeDictionary Properties
    {
      get
      {
        return (IDocumentNodeDictionary) this.properties;
      }
    }

    public bool SupportsChildren
    {
      get
      {
        return this.children != null;
      }
    }

    public IList<DocumentNode> Children
    {
      get
      {
        return (IList<DocumentNode>) this.children;
      }
    }

    public override IEnumerable<DocumentNode> DescendantNodes
    {
      get
      {
        return (IEnumerable<DocumentNode>) new DocumentCompositeNode.DescendantNodesEnumerable((DocumentNode) this);
      }
    }

    public DocumentCompositeNode(IDocumentContext context, IType type, ITypeId childType)
      : base(context, (ITypeId) type)
    {
      this.properties = new DocumentCompositeNode.PropertyDictionary(this);
      if (childType != null)
        this.children = new DocumentCompositeNode.DocumentNodeCollection(this);
      if (!context.TypeResolver.PlatformMetadata.GetIsTypeItsOwnNameScope((ITypeId) type))
        return;
      this.nameScope = new DocumentNodeNameScope();
    }

    public void SetConstructor(IConstructor constructor, IList<DocumentNode> constructorArguments)
    {
      this.constructor = constructor;
      if (constructorArguments != null)
      {
        this.constructorArguments = new DocumentCompositeNode.ConstructorArgumentNodeCollection(this, constructor, constructorArguments);
      }
      else
      {
        if (constructor != null)
          throw new ArgumentException();
        this.constructorArguments = (DocumentCompositeNode.ConstructorArgumentNodeCollection) null;
      }
    }

    public IConstructor GetBestConstructor(out IConstructorArgumentNodeCollection constructorArguments)
    {
      if (!this.IsConstructorValid)
      {
        IType type = this.Type;
        IList<IConstructor> constructors = type.GetConstructors();
        IConstructor constructor1 = (IConstructor) null;
        IConstructor constructor2 = (IConstructor) null;
        IConstructorArgumentProperties argumentProperties = type.GetConstructorArgumentProperties();
        foreach (IConstructor constructor3 in (IEnumerable<IConstructor>) constructors)
        {
          if (constructor1 == null || constructor1.Parameters.Count < constructor3.Parameters.Count)
          {
            bool flag = true;
            foreach (IParameter parameter in (IEnumerable<IParameter>) constructor3.Parameters)
            {
              string name = parameter.Name;
              IPropertyId index = (IPropertyId) argumentProperties[name];
              if (index == null || this.properties[index] == null)
              {
                flag = false;
                break;
              }
            }
            if (flag)
              constructor1 = constructor3;
          }
          if (constructor2 == null || constructor2.Parameters.Count > constructor3.Parameters.Count)
            constructor2 = constructor3;
        }
        IConstructor constructor4 = constructor1 ?? constructor2;
        if (constructor4 != null && constructor4 != this.constructor)
        {
          constructorArguments = (IConstructorArgumentNodeCollection) new DocumentCompositeNode.PropertiesAsConstructorArgumentNodeCollection(this, argumentProperties, constructor4);
          return constructor4;
        }
      }
      if (this.constructor != null)
      {
        constructorArguments = (IConstructorArgumentNodeCollection) this.constructorArguments;
        return this.constructor;
      }
      constructorArguments = (IConstructorArgumentNodeCollection) DocumentCompositeNode.EmptyConstructorArgumentNodeCollection.Instance;
      return (IConstructor) null;
    }

    public override DocumentNode Clone(IDocumentContext context)
    {
      ITypeId typeId = (ITypeId) this.Type.Clone(context.TypeResolver);
      if (context.TypeResolver.PlatformMetadata.IsNullType(typeId))
        return (DocumentNode) null;
      DocumentCompositeNode node = context.CreateNode(typeId);
      node.explicitCollection = this.explicitCollection;
      if (this.SourceContext != null)
        node.SourceContext = this.SourceContext.FreezeText(true);
      if (this.constructor != null)
        node.constructor = (IConstructor) this.constructor.Clone(context.TypeResolver);
      if (this.constructorArguments != null)
        node.constructorArguments = this.constructorArguments.Clone(context, node, node.constructor);
      foreach (KeyValuePair<IProperty, SourceContextContainer<DocumentNode>> keyValuePair in this.properties.KeyValuePairs)
      {
        IProperty key = keyValuePair.Key;
        SourceContextContainer<DocumentNode> contextContainer = keyValuePair.Value;
        INodeSourceContext containerContext = contextContainer.ContainerContext != null ? contextContainer.ContainerContext.FreezeText(true) : (INodeSourceContext) null;
        DocumentNode content = contextContainer.Content.Clone(context);
        IProperty property = (IProperty) key.Clone(context.TypeResolver);
        if (content != null && property != null && !context.TypeResolver.PlatformMetadata.IsNullType((ITypeId) property.DeclaringType))
          node.properties[(IPropertyId) property] = new SourceContextContainer<DocumentNode>(containerContext, content);
      }
      if (this.SupportsChildren)
      {
        for (int index = 0; index < this.children.Count; ++index)
        {
          DocumentNode documentNode = this.children[index].Clone(context);
          if (documentNode != null)
            node.Children.Add(documentNode);
        }
      }
      return (DocumentNode) node;
    }

    public override bool Equals(DocumentNode other)
    {
      DocumentCompositeNode documentCompositeNode = other as DocumentCompositeNode;
      if (documentCompositeNode == null || !this.Type.Equals((object) documentCompositeNode.Type) || this.properties.Count != documentCompositeNode.properties.Count)
        return false;
      bool flag = this.PlatformMetadata != documentCompositeNode.PlatformMetadata;
      foreach (KeyValuePair<IProperty, DocumentNode> keyValuePair in this.properties)
      {
        IProperty property = keyValuePair.Key;
        if (flag)
        {
          property = property.Clone(documentCompositeNode.TypeResolver) as IProperty;
          if (property == null)
            return false;
        }
        DocumentNode documentNode = documentCompositeNode.Properties[(IPropertyId) property];
        if (documentNode == null || !documentNode.Equals(keyValuePair.Value))
          return false;
      }
      if (this.children == null)
      {
        if (documentCompositeNode.children != null)
          return false;
      }
      else
      {
        if (documentCompositeNode.children == null || this.children.Count != documentCompositeNode.children.Count)
          return false;
        for (int index = 0; index < this.children.Count; ++index)
        {
          if (!this.children[index].Equals(documentCompositeNode.children[index]))
            return false;
        }
      }
      if (!this.HasConstructorArguments)
      {
        if (documentCompositeNode.HasConstructorArguments)
          return false;
      }
      else
      {
        if (!documentCompositeNode.HasConstructorArguments || this.constructorArguments.Count != documentCompositeNode.constructorArguments.Count)
          return false;
        for (int index = 0; index < this.constructorArguments.Count; ++index)
        {
          if (!this.constructorArguments[index].Equals(documentCompositeNode.constructorArguments[index]))
            return false;
        }
      }
      return true;
    }

    public override int GetHashCodeInternal()
    {
      int hashCode = this.Type.GetHashCode();
      if (this.properties != null)
        hashCode ^= this.properties.GetHashCode();
      if (this.children != null)
        hashCode ^= this.children.GetHashCode();
      return hashCode;
    }

    public override void SetDocumentRoot(IDocumentRoot documentRoot)
    {
      if (documentRoot == null)
        this.ValidateChildIndices(-1);
      base.SetDocumentRoot(documentRoot);
    }

    public void ApplyPropertyChange(IProperty propertyKey, SourceContextContainer<DocumentNode> oldValue, SourceContextContainer<DocumentNode> newValue)
    {
      this.properties.ModifyProperty(propertyKey, oldValue, newValue);
    }

    public void ApplyChildrenChange(int index, DocumentNode oldChildNode, DocumentNode newChildNode)
    {
      this.children.ReallyApplyChange(index, oldChildNode, newChildNode);
    }

    public string GetValueAsString(IPropertyId propertyKey)
    {
      return DocumentPrimitiveNode.GetValueAsString(this.Properties[propertyKey]);
    }

    public T GetValue<T>(IPropertyId propertyKey)
    {
      DocumentNode documentNode = this.Properties[propertyKey];
      if (documentNode != null)
      {
        DocumentPrimitiveNode documentPrimitiveNode = documentNode as DocumentPrimitiveNode;
        if (documentPrimitiveNode != null)
        {
          DocumentNodeStringValue documentNodeStringValue = documentPrimitiveNode.Value as DocumentNodeStringValue;
          if (documentNodeStringValue != null && documentNodeStringValue.Value != null)
          {
            TypeConverter typeConverter = this.PlatformMetadata.GetTypeConverter((MemberInfo) typeof (T));
            try
            {
              return (T) typeConverter.ConvertFromString((ITypeDescriptorContext) null, CultureInfo.InvariantCulture, documentNodeStringValue.Value);
            }
            catch (Exception ex)
            {
              return default (T);
            }
          }
        }
      }
      return default (T);
    }

    public void SetValue<T>(IPropertyId propertyKey, T value)
    {
      this.Properties[propertyKey] = this.Context.CreateNode(typeof (T), (object) value);
    }

    public void ClearValue(IPropertyId propertyKey)
    {
      this.Properties[propertyKey] = (DocumentNode) null;
    }

    public Uri GetUriValue(IPropertyId propertyKey)
    {
      DocumentNode documentNode = this.Properties[propertyKey];
      if (documentNode != null)
      {
        DocumentPrimitiveNode documentPrimitiveNode = documentNode as DocumentPrimitiveNode;
        if (documentPrimitiveNode != null)
          return documentPrimitiveNode.GetUriValue();
      }
      return (Uri) null;
    }

    public DocumentNode GetValue(IPropertyId propertyKey, out INodeSourceContext containerContext)
    {
      SourceContextContainer<DocumentNode> contextContainer = this.properties[propertyKey];
      if (contextContainer != null)
      {
        containerContext = contextContainer.ContainerContext;
        return contextContainer.Content;
      }
      containerContext = (INodeSourceContext) null;
      return (DocumentNode) null;
    }

    public void SetValue(IPropertyId propertyKey, INodeSourceContext containerContext, DocumentNode value)
    {
      SourceContextContainer<DocumentNode> contextContainer = value != null ? new SourceContextContainer<DocumentNode>(containerContext, value) : (SourceContextContainer<DocumentNode>) null;
      this.properties[propertyKey] = contextContainer;
    }

    public void ClearContainerContext(IPropertyId propertyKey)
    {
      SourceContextContainer<DocumentNode> contextContainer = this.properties[propertyKey];
      if (contextContainer == null)
        return;
      contextContainer.ContainerContext = (INodeSourceContext) null;
    }

    public INodeSourceContext GetContainerContext(IPropertyId propertyKey)
    {
      SourceContextContainer<DocumentNode> contextContainer = this.properties[propertyKey];
      if (contextContainer != null)
        return contextContainer.ContainerContext;
      return (INodeSourceContext) null;
    }

    public void SetContainerContext(IPropertyId propertyKey, INodeSourceContext containerContext)
    {
      this.properties[propertyKey].ContainerContext = containerContext;
    }

    internal void ValidateChildIndices(int index)
    {
      if (this.children == null)
        return;
      this.children.ValidateChildIndices(index);
    }

    private class DescendantNodesEnumerable : IEnumerable<DocumentNode>, IEnumerable
    {
      private DocumentNode root;

      public DescendantNodesEnumerable(DocumentNode root)
      {
        this.root = root;
      }

      public IEnumerator<DocumentNode> GetEnumerator()
      {
        return (IEnumerator<DocumentNode>) new DocumentCompositeNode.DescendantNodesEnumerator(this.root);
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) new DocumentCompositeNode.DescendantNodesEnumerator(this.root);
      }
    }

    private class DescendantNodesEnumerator : IEnumerator<DocumentNode>, IDisposable, IEnumerator, IDescendantEnumerator
    {
      private DocumentCompositeNode rootNode;
      private DocumentCompositeNode currentParent;
      private DocumentNode currentNode;
      private int currentPosition;
      private bool pastProperties;

      public DocumentNode Current
      {
        get
        {
          return this.GetCurrent();
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return (object) this.GetCurrent();
        }
      }

      public DescendantNodesEnumerator(DocumentNode root)
      {
        this.rootNode = root as DocumentCompositeNode;
        this.currentParent = this.rootNode;
        this.DoResetForParent();
      }

      private void DoResetForParent()
      {
        this.pastProperties = this.currentParent == null || this.currentParent.Properties.Count == 0;
        this.currentPosition = 0;
      }

      private DocumentNode GetCurrent()
      {
        return this.currentNode;
      }

      public void Dispose()
      {
      }

      public bool MoveNext()
      {
        if (this.rootNode == null)
          return false;
        while (this.pastProperties)
        {
          if (this.currentParent.SupportsChildren && this.currentPosition < this.currentParent.children.Count)
          {
            this.currentNode = this.currentParent.children[this.currentPosition];
            if (this.currentNode.ChildNodesCount > 0)
            {
              this.currentParent = (DocumentCompositeNode) this.currentNode;
              this.DoResetForParent();
            }
            else
              ++this.currentPosition;
            return true;
          }
          if (this.currentParent != this.rootNode)
          {
            this.currentNode = (DocumentNode) this.currentParent;
            this.currentParent = this.currentParent.Parent;
            if (this.currentNode.IsProperty)
            {
              this.currentPosition = this.currentParent.Properties.IndexOf(this.currentNode.SitePropertyKey) + 1;
              if (this.currentPosition >= this.currentParent.Properties.Count)
              {
                this.currentPosition = 0;
                this.pastProperties = true;
              }
              else
                this.pastProperties = false;
            }
            else
            {
              this.currentPosition = this.currentNode.SiteChildIndex + 1;
              this.pastProperties = true;
            }
          }
          else
          {
            this.currentNode = (DocumentNode) null;
            return false;
          }
        }
        this.currentNode = this.currentParent.Properties[this.currentPosition];
        if (this.currentNode.ChildNodesCount > 0)
        {
          this.currentParent = (DocumentCompositeNode) this.currentNode;
          this.DoResetForParent();
        }
        else
        {
          ++this.currentPosition;
          if (this.currentPosition >= this.currentParent.Properties.Count)
          {
            this.currentPosition = 0;
            this.pastProperties = true;
          }
        }
        return true;
      }

      public void Reset()
      {
        this.currentParent = this.rootNode;
        this.currentNode = (DocumentNode) null;
        this.DoResetForParent();
      }

      public void SkipPastDescendants(DocumentNode node)
      {
        DocumentCompositeNode documentCompositeNode = node as DocumentCompositeNode;
        if (documentCompositeNode == null)
          return;
        this.currentParent = documentCompositeNode;
        this.pastProperties = true;
        this.currentPosition = this.currentParent.SupportsChildren ? this.currentParent.Children.Count : 0;
      }
    }

    private class ChildNodesEnumerable : IEnumerable<DocumentNode>, IEnumerable
    {
      private DocumentNode parent;

      public ChildNodesEnumerable(DocumentNode parent)
      {
        this.parent = parent;
      }

      public IEnumerator<DocumentNode> GetEnumerator()
      {
        return (IEnumerator<DocumentNode>) new DocumentCompositeNode.ChildNodesEnumerator(this.parent);
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) new DocumentCompositeNode.ChildNodesEnumerator(this.parent);
      }
    }

    private class ChildNodesEnumerator : IEnumerator<DocumentNode>, IDisposable, IEnumerator
    {
      private DocumentCompositeNode parentNode;
      private DocumentNode currentNode;
      private int currentPosition;
      private bool pastProperties;

      DocumentNode IEnumerator<DocumentNode>.Current
      {
        get
        {
          return this.GetCurrent();
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return (object) this.GetCurrent();
        }
      }

      public ChildNodesEnumerator(DocumentNode parent)
      {
        this.parentNode = parent as DocumentCompositeNode;
        this.DoReset();
      }

      private void DoReset()
      {
        this.pastProperties = this.parentNode == null || this.parentNode.Properties.Count == 0;
        this.currentPosition = 0;
        this.currentNode = (DocumentNode) null;
      }

      private DocumentNode GetCurrent()
      {
        return this.currentNode;
      }

      void IDisposable.Dispose()
      {
      }

      bool IEnumerator.MoveNext()
      {
        if (this.parentNode == null)
          return false;
        if (this.pastProperties)
        {
          if (this.parentNode.SupportsChildren && this.currentPosition < this.parentNode.children.Count)
          {
            this.currentNode = this.parentNode.children[this.currentPosition];
            ++this.currentPosition;
            return true;
          }
          this.currentNode = (DocumentNode) null;
          return false;
        }
        this.currentNode = this.parentNode.Properties[this.currentPosition];
        ++this.currentPosition;
        if (this.currentPosition >= this.parentNode.Properties.Count)
        {
          this.currentPosition = 0;
          this.pastProperties = true;
        }
        return true;
      }

      void IEnumerator.Reset()
      {
        this.DoReset();
      }
    }

    private sealed class EmptyConstructorArgumentNodeCollection : IConstructorArgumentNodeCollection, IEnumerable<DocumentNode>, IEnumerable
    {
      public static readonly DocumentCompositeNode.EmptyConstructorArgumentNodeCollection Instance = new DocumentCompositeNode.EmptyConstructorArgumentNodeCollection();
      private List<DocumentNode> collection;

      public int Count
      {
        get
        {
          return 0;
        }
      }

      public DocumentNode this[int index]
      {
        get
        {
          return this.collection[index];
        }
      }

      private EmptyConstructorArgumentNodeCollection()
      {
        this.collection = new List<DocumentNode>(0);
      }

      public IEnumerator<DocumentNode> GetEnumerator()
      {
        return (IEnumerator<DocumentNode>) this.collection.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) this.GetEnumerator();
      }
    }

    private abstract class ConstructorArgumentNode
    {
      public abstract DocumentNode Node { get; }

      public abstract IProperty Property { get; }

      public abstract DocumentCompositeNode.ConstructorArgumentNode Clone(IDocumentContext documentContext, DocumentCompositeNode clonedParent);

      public override string ToString()
      {
        DocumentNode node = this.Node;
        if (node != null)
          return node.ToString();
        return base.ToString();
      }
    }

    private sealed class StandaloneConstructorArgumentNode : DocumentCompositeNode.ConstructorArgumentNode
    {
      private DocumentNode node;

      public override DocumentNode Node
      {
        get
        {
          return this.node;
        }
      }

      public override IProperty Property
      {
        get
        {
          return (IProperty) null;
        }
      }

      public StandaloneConstructorArgumentNode(DocumentNode node)
      {
        this.node = node;
      }

      public override DocumentCompositeNode.ConstructorArgumentNode Clone(IDocumentContext documentContext, DocumentCompositeNode clonedParent)
      {
        return (DocumentCompositeNode.ConstructorArgumentNode) new DocumentCompositeNode.StandaloneConstructorArgumentNode(this.node.Clone(documentContext));
      }

      public override bool Equals(object obj)
      {
        DocumentCompositeNode.StandaloneConstructorArgumentNode constructorArgumentNode = obj as DocumentCompositeNode.StandaloneConstructorArgumentNode;
        if (constructorArgumentNode != null)
          return this.node.Equals(constructorArgumentNode.Node);
        return false;
      }

      public override int GetHashCode()
      {
        return this.node.GetHashCode();
      }
    }

    private sealed class PropertyBasedConstructorArgumentNode : DocumentCompositeNode.ConstructorArgumentNode
    {
      private DocumentCompositeNode parentNode;
      private IProperty propertyKey;

      public override DocumentNode Node
      {
        get
        {
          return this.parentNode.Properties[(IPropertyId) this.propertyKey];
        }
      }

      public override IProperty Property
      {
        get
        {
          return this.propertyKey;
        }
      }

      public PropertyBasedConstructorArgumentNode(DocumentCompositeNode parentNode, IProperty propertyKey)
      {
        this.parentNode = parentNode;
        this.propertyKey = propertyKey;
      }

      public override DocumentCompositeNode.ConstructorArgumentNode Clone(IDocumentContext documentContext, DocumentCompositeNode clonedParent)
      {
        IProperty propertyKey = (IProperty) this.propertyKey.Clone(documentContext.TypeResolver);
        return (DocumentCompositeNode.ConstructorArgumentNode) new DocumentCompositeNode.PropertyBasedConstructorArgumentNode(clonedParent, propertyKey);
      }

      public override bool Equals(object obj)
      {
        DocumentCompositeNode.PropertyBasedConstructorArgumentNode constructorArgumentNode = obj as DocumentCompositeNode.PropertyBasedConstructorArgumentNode;
        if (constructorArgumentNode != null)
          return this.propertyKey.Equals((object) constructorArgumentNode.propertyKey);
        return false;
      }

      public override int GetHashCode()
      {
        return this.propertyKey.GetHashCode();
      }
    }

    private sealed class PropertiesAsConstructorArgumentNodeCollection : IConstructorArgumentNodeCollection, IEnumerable<DocumentNode>, IEnumerable
    {
      private DocumentCompositeNode node;
      private IConstructorArgumentProperties argumentProperties;
      private IConstructor constructor;

      public int Count
      {
        get
        {
          return this.constructor.Parameters.Count;
        }
      }

      public DocumentNode this[int index]
      {
        get
        {
          IPropertyId index1 = (IPropertyId) this.argumentProperties[this.constructor.Parameters[index].Name];
          if (index1 != null)
            return this.node.Properties[index1];
          return (DocumentNode) null;
        }
      }

      public PropertiesAsConstructorArgumentNodeCollection(DocumentCompositeNode node, IConstructorArgumentProperties argumentProperties, IConstructor constructor)
      {
        this.node = node;
        this.argumentProperties = argumentProperties;
        this.constructor = constructor;
      }

      public IEnumerator<DocumentNode> GetEnumerator()
      {
        for (int i = 0; i < this.constructor.Parameters.Count; ++i)
          yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) this.GetEnumerator();
      }
    }

    private sealed class ConstructorArgumentNodeCollection : IConstructorArgumentNodeCollection, IEnumerable<DocumentNode>, IEnumerable
    {
      private DocumentCompositeNode node;
      private List<DocumentCompositeNode.ConstructorArgumentNode> collection;

      public IList<DocumentCompositeNode.ConstructorArgumentNode> Collection
      {
        get
        {
          return (IList<DocumentCompositeNode.ConstructorArgumentNode>) this.collection;
        }
      }

      public int Count
      {
        get
        {
          return this.collection.Count;
        }
      }

      public DocumentNode this[int index]
      {
        get
        {
          return this.collection[index].Node;
        }
      }

      public ConstructorArgumentNodeCollection(DocumentCompositeNode node, IConstructor constructor, IList<DocumentNode> constructorArgumentNodes)
        : this(node, constructorArgumentNodes.Count)
      {
        IConstructorArgumentProperties argumentProperties = this.node.Type.GetConstructorArgumentProperties();
        for (int index = 0; index < constructor.Parameters.Count; ++index)
        {
          IParameter parameter = constructor.Parameters[index];
          IProperty propertyKey = argumentProperties[parameter.Name];
          DocumentCompositeNode.ConstructorArgumentNode constructorArgumentNode;
          if (propertyKey != null)
          {
            this.node.Properties[(IPropertyId) propertyKey] = constructorArgumentNodes[index];
            constructorArgumentNode = (DocumentCompositeNode.ConstructorArgumentNode) new DocumentCompositeNode.PropertyBasedConstructorArgumentNode(node, propertyKey);
          }
          else
            constructorArgumentNode = (DocumentCompositeNode.ConstructorArgumentNode) new DocumentCompositeNode.StandaloneConstructorArgumentNode(constructorArgumentNodes[index]);
          this.collection.Add(constructorArgumentNode);
        }
      }

      private ConstructorArgumentNodeCollection(DocumentCompositeNode node, int capacity)
      {
        this.node = node;
        this.collection = new List<DocumentCompositeNode.ConstructorArgumentNode>(capacity);
      }

      public DocumentCompositeNode.ConstructorArgumentNodeCollection Clone(IDocumentContext documentContext, DocumentCompositeNode clonedParent, IConstructor constructor)
      {
        DocumentCompositeNode.ConstructorArgumentNodeCollection argumentNodeCollection = new DocumentCompositeNode.ConstructorArgumentNodeCollection(clonedParent, this.collection.Count);
        foreach (DocumentCompositeNode.ConstructorArgumentNode constructorArgumentNode in this.collection)
          argumentNodeCollection.collection.Add(constructorArgumentNode.Clone(documentContext, clonedParent));
        return argumentNodeCollection;
      }

      public IEnumerator<DocumentNode> GetEnumerator()
      {
        foreach (DocumentCompositeNode.ConstructorArgumentNode constructorArgumentNode in this.collection)
          yield return constructorArgumentNode.Node;
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) this.GetEnumerator();
      }

      public override string ToString()
      {
        return this.collection.ToString();
      }
    }

    private sealed class PropertyDictionary : IDocumentNodeDictionary, IEnumerable<KeyValuePair<IProperty, DocumentNode>>, IEnumerable
    {
      private SortedList<IProperty, SourceContextContainer<DocumentNode>> dictionary = new SortedList<IProperty, SourceContextContainer<DocumentNode>>((IComparer<IProperty>) DocumentCompositeNode.PropertyKeyComparer.Instance);
      private DocumentCompositeNode node;

      public SourceContextContainer<DocumentNode> this[IPropertyId propertyKey]
      {
        get
        {
          SourceContextContainer<DocumentNode> contextContainer;
          this.dictionary.TryGetValue(this.ResolvePropertyInternal(propertyKey), out contextContainer);
          return contextContainer;
        }
        set
        {
          IProperty propertyKey1 = this.ResolvePropertyInternal(propertyKey);
          SourceContextContainer<DocumentNode> contextContainer = this[(IPropertyId) propertyKey1];
          DocumentNode documentNode = SourceContextContainer<DocumentNode>.ToContent(contextContainer);
          DocumentNode other = SourceContextContainer<DocumentNode>.ToContent(value);
          if (documentNode == other)
            return;
          if (documentNode is DocumentPrimitiveNode)
          {
            if (documentNode.Equals(other))
              return;
            this.ApplyChange(propertyKey1, contextContainer, value);
          }
          else
            this.ApplyChange(propertyKey1, contextContainer, value);
        }
      }

      public IEnumerable<KeyValuePair<IProperty, SourceContextContainer<DocumentNode>>> KeyValuePairs
      {
        get
        {
          return (IEnumerable<KeyValuePair<IProperty, SourceContextContainer<DocumentNode>>>) this.dictionary;
        }
      }

      public int Count
      {
        get
        {
          return this.dictionary.Count;
        }
      }

      DocumentNode IDocumentNodeDictionary.this[IPropertyId propertyKey]
      {
        get
        {
          return SourceContextContainer<DocumentNode>.ToContent(this[(IPropertyId) this.node.TypeResolver.ResolveProperty(propertyKey)]);
        }
        set
        {
          IProperty property = this.node.TypeResolver.ResolveProperty(propertyKey);
          INodeSourceContext containerContext = (INodeSourceContext) null;
          SourceContextContainer<DocumentNode> contextContainer = this[(IPropertyId) property];
          if (contextContainer != null)
          {
            containerContext = contextContainer.ContainerContext;
            if (containerContext != null)
              containerContext = containerContext.Clone(true);
          }
          this[(IPropertyId) property] = value != null ? new SourceContextContainer<DocumentNode>(containerContext, value) : (SourceContextContainer<DocumentNode>) null;
        }
      }

      DocumentNode IDocumentNodeDictionary.this[int item]
      {
        get
        {
          return SourceContextContainer<DocumentNode>.ToContent(this.dictionary.Values[item]);
        }
      }

      public IList<IProperty> Keys
      {
        get
        {
          return this.dictionary.Keys;
        }
      }

      public PropertyDictionary(DocumentCompositeNode node)
      {
        this.node = node;
      }

      private IProperty ResolvePropertyInternal(IPropertyId propertyKey)
      {
        IProperty property = this.node.TypeResolver.ResolveProperty(propertyKey);
        if (property == null)
          throw new ArgumentNullException("propertyKey");
        if (this.node.PlatformMetadata != property.DeclaringType.PlatformMetadata)
        {
          property = property.Clone(this.node.TypeResolver) as IProperty;
          if (property == null)
            throw new NotSupportedException();
        }
        return property;
      }

      public bool Contains(IProperty propertyKey)
      {
        return this.dictionary.ContainsKey(propertyKey);
      }

      public int IndexOf(IProperty propertyKey)
      {
        return this.dictionary.IndexOfKey(propertyKey);
      }

      public IEnumerator<KeyValuePair<IProperty, DocumentNode>> GetEnumerator()
      {
        foreach (KeyValuePair<IProperty, SourceContextContainer<DocumentNode>> keyValuePair in this.dictionary)
          yield return new KeyValuePair<IProperty, DocumentNode>(keyValuePair.Key, keyValuePair.Value.Content);
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) this.GetEnumerator();
      }

      private void ApplyChange(IProperty propertyKey, SourceContextContainer<DocumentNode> oldValue, SourceContextContainer<DocumentNode> newValue)
      {
        IDocumentRoot documentRoot = this.node.DocumentRoot;
        if (documentRoot != null)
          documentRoot.ApplyPropertyChange(this.node, propertyKey, oldValue, newValue);
        else
          this.ModifyProperty(propertyKey, oldValue, newValue);
      }

      internal void ModifyProperty(IProperty propertyKey, SourceContextContainer<DocumentNode> oldValue, SourceContextContainer<DocumentNode> newValue)
      {
        IDocumentRoot documentRoot = this.node.DocumentRoot;
        if (oldValue == null)
        {
          newValue.Content.SetSitePropertyKey(this.node, propertyKey);
          this.dictionary.Add(propertyKey, newValue);
          if (documentRoot == null)
            return;
          documentRoot.OnNodeChange(newValue.Content, new DocumentNodeChange(this.node, propertyKey, (DocumentNode) null, newValue.Content));
        }
        else if (newValue == null)
        {
          if (documentRoot != null)
            documentRoot.OnNodeChange(oldValue.Content, new DocumentNodeChange(this.node, propertyKey, oldValue.Content, (DocumentNode) null));
          oldValue.Content.ClearSite();
          this.dictionary.Remove(propertyKey);
        }
        else
        {
          if (documentRoot != null)
            documentRoot.OnNodeChange(oldValue.Content, new DocumentNodeChange(this.node, propertyKey, oldValue.Content, newValue.Content));
          DocumentCompositeNode parent = oldValue.Content.Parent;
          IProperty sitePropertyKey = oldValue.Content.SitePropertyKey;
          int siteChildIndex = oldValue.Content.SiteChildIndex;
          oldValue.Content.ClearSite();
          try
          {
            newValue.Content.SetSitePropertyKey(this.node, propertyKey);
          }
          catch (Exception ex)
          {
            oldValue.Content.SetSite(parent, sitePropertyKey, siteChildIndex);
            throw;
          }
          this.dictionary[propertyKey] = newValue;
          if (documentRoot == null)
            return;
          documentRoot.OnNodeChange(newValue.Content, new DocumentNodeChange(this.node, propertyKey, oldValue.Content, newValue.Content));
        }
      }
    }

    private sealed class DocumentNodeCollection : IList<DocumentNode>, ICollection<DocumentNode>, IEnumerable<DocumentNode>, IEnumerable
    {
      private int lastLowestDeltaIndex = -1;
      private int lastLowestDeltaSize = -1;
      private DocumentCompositeNode node;
      private List<DocumentNode> collection;

      public DocumentNode this[int index]
      {
        get
        {
          return this.collection[index];
        }
        set
        {
          if (value == null)
            throw new ArgumentNullException("value");
          DocumentNode oldChildNode = this.collection[index];
          if (value == oldChildNode)
            return;
          this.ApplyChange(index, oldChildNode, value);
        }
      }

      public int Count
      {
        get
        {
          return this.collection.Count;
        }
      }

      public bool IsReadOnly
      {
        get
        {
          return false;
        }
      }

      public DocumentNodeCollection(DocumentCompositeNode node)
      {
        this.node = node;
        this.collection = new List<DocumentNode>();
      }

      public bool Contains(DocumentNode item)
      {
        return this.collection.Contains(item);
      }

      public bool Remove(DocumentNode item)
      {
        int index = item == null || item.Parent == null || (!item.IsChild || item.Parent.Children != this) ? -1 : item.SiteChildIndex;
        if (index < 0 || index >= this.Count)
          return false;
        this.ApplyChange(index, item, (DocumentNode) null);
        return true;
      }

      public void RemoveAt(int index)
      {
        DocumentNode oldChildNode = this[index];
        this.ApplyChange(index, oldChildNode, (DocumentNode) null);
      }

      public void Add(DocumentNode item)
      {
        if (item == null)
          throw new ArgumentNullException("item");
        this.ApplyChange(this.collection.Count, (DocumentNode) null, item);
      }

      public void Insert(int index, DocumentNode item)
      {
        if (item == null)
          throw new ArgumentNullException("item");
        if (index < 0 || index > this.collection.Count)
          throw new ArgumentOutOfRangeException("index");
        this.ApplyChange(index, (DocumentNode) null, item);
      }

      public int IndexOf(DocumentNode item)
      {
        if (item != null && item.Parent == this.node && item.IsChild)
          return item.SiteChildIndex;
        return -1;
      }

      public void CopyTo(DocumentNode[] array, int arrayIndex)
      {
        this.collection.CopyTo(array, arrayIndex);
      }

      public void Clear()
      {
        while (this.collection.Count > 0)
          this.ApplyChange(this.collection.Count - 1, this.collection[this.collection.Count - 1], (DocumentNode) null);
      }

      public IEnumerator<DocumentNode> GetEnumerator()
      {
        return (IEnumerator<DocumentNode>) this.collection.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) this.GetEnumerator();
      }

      private void ApplyChange(int index, DocumentNode oldChildNode, DocumentNode newChildNode)
      {
        IDocumentRoot documentRoot = this.node.DocumentRoot;
        if (documentRoot != null)
          documentRoot.ApplyChildrenChange(this.node, index, oldChildNode, newChildNode);
        else
          this.ReallyApplyChange(index, oldChildNode, newChildNode);
      }

      internal void ValidateChildIndices(int index)
      {
        if (this.lastLowestDeltaIndex == -1)
          return;
        int num = this.lastLowestDeltaIndex + this.lastLowestDeltaSize;
        if (index != -1 && index < this.lastLowestDeltaIndex)
          return;
        this.lastLowestDeltaIndex = -1;
        this.lastLowestDeltaSize = -1;
        DocumentNode previousSibling = num > 0 ? this.collection[num - 1] : (DocumentNode) null;
        for (int newIndex = num; newIndex < this.collection.Count; ++newIndex)
        {
          DocumentNode documentNode = this.collection[newIndex];
          documentNode.SiteChildIndex = newIndex;
          documentNode.FixupMarkerChildIndex(newIndex, previousSibling);
          previousSibling = documentNode;
        }
      }

      internal void ReallyApplyChange(int index, DocumentNode oldChildNode, DocumentNode newChildNode)
      {
        IDocumentRoot documentRoot = this.node.DocumentRoot;
        if (oldChildNode == null)
        {
          newChildNode.SetSiteChildIndex(this.node, index);
          this.collection.Insert(index, newChildNode);
          if (this.lastLowestDeltaIndex == -1 || index < this.lastLowestDeltaIndex)
          {
            this.lastLowestDeltaIndex = index;
            this.lastLowestDeltaSize = 1;
          }
          else if (index >= this.lastLowestDeltaIndex && index < this.lastLowestDeltaIndex + this.lastLowestDeltaSize)
            this.lastLowestDeltaSize = index - this.lastLowestDeltaIndex + 1;
          else if (this.lastLowestDeltaIndex + this.lastLowestDeltaSize == index)
            ++this.lastLowestDeltaSize;
          if (documentRoot == null)
            return;
          documentRoot.OnNodeChange(newChildNode, new DocumentNodeChange(this.node, index, (DocumentNode) null, newChildNode));
        }
        else if (newChildNode == null)
        {
          if (documentRoot != null)
            documentRoot.OnNodeChange(oldChildNode, new DocumentNodeChange(this.node, index, oldChildNode, (DocumentNode) null));
          oldChildNode.ClearSite();
          this.collection.RemoveAt(index);
          if (this.lastLowestDeltaIndex == -1 || index < this.lastLowestDeltaIndex)
          {
            this.lastLowestDeltaIndex = index;
            this.lastLowestDeltaSize = 0;
          }
          else
          {
            if (this.lastLowestDeltaIndex > index || index > this.lastLowestDeltaIndex + this.lastLowestDeltaSize)
              return;
            this.lastLowestDeltaSize = index - this.lastLowestDeltaIndex;
          }
        }
        else
        {
          if (documentRoot != null)
            documentRoot.OnNodeChange(oldChildNode, new DocumentNodeChange(this.node, index, oldChildNode, newChildNode));
          DocumentCompositeNode parent = oldChildNode.Parent;
          IProperty sitePropertyKey = oldChildNode.SitePropertyKey;
          int siteChildIndex = oldChildNode.SiteChildIndex;
          oldChildNode.ClearSite();
          try
          {
            newChildNode.SetSiteChildIndex(this.node, index);
          }
          catch (Exception ex)
          {
            oldChildNode.SetSite(parent, sitePropertyKey, siteChildIndex);
            throw;
          }
          this.collection[index] = newChildNode;
          if (documentRoot != null)
            documentRoot.OnNodeChange(newChildNode, new DocumentNodeChange(this.node, index, oldChildNode, newChildNode));
          if (index >= this.collection.Count - 1)
            return;
          this.collection[index + 1].FixupMarkerChildIndex(index + 1, newChildNode);
        }
      }
    }

    private class PropertyKeyComparer : IComparer<IProperty>
    {
      private static DocumentCompositeNode.PropertyKeyComparer instance = new DocumentCompositeNode.PropertyKeyComparer();

      public static DocumentCompositeNode.PropertyKeyComparer Instance
      {
        get
        {
          return DocumentCompositeNode.PropertyKeyComparer.instance;
        }
      }

      public int Compare(IProperty x, IProperty y)
      {
        return x.SortValue - y.SortValue;
      }
    }
  }
}
