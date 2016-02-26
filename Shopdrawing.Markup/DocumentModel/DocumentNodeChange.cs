// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.DocumentModel.DocumentNodeChange
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignModel.DocumentModel
{
  public sealed class DocumentNodeChange
  {
    private DocumentNode parentNode;
    private int childIndex;
    private IProperty propertyKey;
    private DocumentNode oldChildNode;
    private DocumentNode newChildNode;
    private object annotation;

    public DocumentCompositeNode ParentNode
    {
      get
      {
        return this.parentNode as DocumentCompositeNode;
      }
    }

    public DocumentNodeChangeAction Action
    {
      get
      {
        if (this.oldChildNode == null)
          return DocumentNodeChangeAction.Add;
        return this.newChildNode == null ? DocumentNodeChangeAction.Remove : DocumentNodeChangeAction.Replace;
      }
    }

    public bool IsPropertyChange
    {
      get
      {
        return this.propertyKey != null;
      }
    }

    public bool IsChildChange
    {
      get
      {
        return this.childIndex != -1;
      }
    }

    public bool IsRootNodeChange
    {
      get
      {
        return this.parentNode == null;
      }
    }

    public int ChildIndex
    {
      get
      {
        return this.childIndex;
      }
    }

    public IProperty PropertyKey
    {
      get
      {
        return this.propertyKey;
      }
    }

    public DocumentNode OldChildNode
    {
      get
      {
        return this.oldChildNode;
      }
    }

    public DocumentNode NewChildNode
    {
      get
      {
        return this.newChildNode;
      }
    }

    public object OldChildNodeValue
    {
      get
      {
        DocumentPrimitiveNode documentPrimitiveNode = this.OldChildNode as DocumentPrimitiveNode;
        if (documentPrimitiveNode != null)
          return (object) documentPrimitiveNode.Value;
        return (object) null;
      }
    }

    public object NewChildNodeValue
    {
      get
      {
        DocumentPrimitiveNode documentPrimitiveNode = this.NewChildNode as DocumentPrimitiveNode;
        if (documentPrimitiveNode != null)
          return (object) documentPrimitiveNode.Value;
        return (object) null;
      }
    }

    public object Annotation
    {
      get
      {
        return this.annotation;
      }
      set
      {
        this.annotation = value;
      }
    }

    public DocumentNodeChange(DocumentNode oldChildNode, DocumentNode newChildNode)
    {
      this.childIndex = -1;
      this.oldChildNode = oldChildNode;
      this.newChildNode = newChildNode;
    }

    public DocumentNodeChange(DocumentCompositeNode parentNode, int childIndex, DocumentNode oldChildNode, DocumentNode newChildNode)
    {
      this.parentNode = (DocumentNode) parentNode;
      this.childIndex = childIndex;
      this.oldChildNode = oldChildNode;
      this.newChildNode = newChildNode;
    }

    public DocumentNodeChange(DocumentCompositeNode parentNode, IProperty propertyKey, DocumentNode oldChildNode, DocumentNode newChildNode)
    {
      this.parentNode = (DocumentNode) parentNode;
      this.childIndex = -1;
      this.propertyKey = propertyKey;
      this.oldChildNode = oldChildNode;
      this.newChildNode = newChildNode;
    }
  }
}
