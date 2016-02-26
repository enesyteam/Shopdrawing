// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.DocumentModel.IDocumentRoot
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.Markup;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.Text;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.DocumentModel
{
  public interface IDocumentRoot : IDisposable
  {
    IEnumerable<IDocumentRoot> DesignTimeResources { get; }

    IDocumentContext DocumentContext { get; }

    ITypeResolver TypeResolver { get; }

    ClassAttributes RootClassAttributes { get; }

    IType CodeBehindClass { get; }

    DocumentNode RootNode { get; set; }

    bool IsEditable { get; }

    ITextBuffer TextBuffer { get; }

    PersistenceSettings PersistenceSettings { get; }

    uint ChangeStamp { get; }

    event EventHandler RootNodeChanging;

    event EventHandler RootNodeChanged;

    event EventHandler TypesChanged;

    void OnNodeChange(DocumentNode node, DocumentNodeChange args);

    void OnNodeSetDocumentRoot(DocumentNode node);

    void OnNodeUnsetDocumentRoot(DocumentNode node);

    void SetSourceContext(DocumentNode node, INodeSourceContext sourceContext);

    void SetContainerSourceContext(DocumentNode node, INodeSourceContext sourceContext);

    void ApplyPropertyChange(DocumentCompositeNode node, IProperty propertyKey, SourceContextContainer<DocumentNode> oldValue, SourceContextContainer<DocumentNode> newValue);

    void ApplyChildrenChange(DocumentCompositeNode node, int index, DocumentNode oldChildNode, DocumentNode newChildNode);
  }
}
