// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DataContextAncestorWalker
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public sealed class DataContextAncestorWalker : IDataContextAncestorWalker
  {
    private int currentIndex = -1;
    private DocumentNodePath dataContextPath;
    private DocumentCompositeNode currentNode;
    private IProperty currentProperty;
    private DocumentCompositeNode moveToNode;
    private IProperty moveToProperty;

    public DocumentCompositeNode CurrentNode
    {
      get
      {
        return this.currentNode;
      }
    }

    public IProperty CurrentProperty
    {
      get
      {
        return this.currentProperty;
      }
    }

    public DataContextAncestorWalker(SceneNode targetNode, IProperty targetProperty)
    {
      this.dataContextPath = this.InitializePath(targetNode, targetProperty);
    }

    public DataContextAncestorWalker(DocumentNodePath dataContextPath)
    {
      this.dataContextPath = dataContextPath;
    }

    public void Reset()
    {
      this.currentIndex = -1;
      this.currentNode = (DocumentCompositeNode) null;
      this.currentProperty = (IProperty) null;
      this.moveToNode = (DocumentCompositeNode) null;
      this.moveToProperty = (IProperty) null;
    }

    public bool MoveNext()
    {
      if (this.moveToNode != null)
      {
        this.currentNode = this.moveToNode;
        this.currentProperty = this.moveToProperty;
        this.moveToNode = (DocumentCompositeNode) null;
        this.moveToProperty = (IProperty) null;
        return true;
      }
      if (this.MoveUp())
        return true;
      return this.ScopeOut();
    }

    private bool MoveUp()
    {
      if (this.currentNode == null || this.currentNode.Parent == null || this.currentIndex >= this.dataContextPath.Count || this.dataContextPath[this.currentIndex].Container == this.currentNode)
        return false;
      this.currentProperty = this.currentNode.SitePropertyKey;
      this.currentNode = this.currentNode.Parent;
      return true;
    }

    private bool ScopeOut()
    {
      if (this.currentIndex >= this.dataContextPath.Count - 1)
        return false;
      ++this.currentIndex;
      NodePathEntry nodePathEntry = this.dataContextPath[this.currentIndex];
      this.currentNode = (DocumentCompositeNode) nodePathEntry.Target;
      this.currentProperty = nodePathEntry.PropertyKey;
      return true;
    }

    public bool MoveTo(DocumentCompositeNode targetNode, IProperty targetProperty, bool makeCurrent)
    {
      if (targetNode == this.CurrentNode)
      {
        this.moveToNode = targetNode;
        this.moveToProperty = targetProperty;
        if (makeCurrent)
          return this.MoveNext();
        return true;
      }
      if (this.currentIndex < 0)
        this.currentIndex = 0;
      for (; this.currentIndex < this.dataContextPath.Count; ++this.currentIndex)
      {
        NodePathEntry nodePathEntry = this.dataContextPath[this.currentIndex];
        if (nodePathEntry.Container == null || nodePathEntry.Container.IsAncestorOf((DocumentNode) targetNode))
        {
          this.moveToNode = targetNode;
          this.moveToProperty = targetProperty;
          if (makeCurrent)
            return this.MoveNext();
          return true;
        }
      }
      return false;
    }

    public override string ToString()
    {
      string str1 = string.Empty;
      if (this.CurrentNode != null)
      {
        string str2 = str1 + " Current: ";
        if (this.CurrentProperty != null)
          str1 = string.Concat(new object[4]
          {
            (object) str2,
            (object) this.CurrentNode,
            (object) "+",
            (object) this.CurrentProperty.Name
          });
        else
          str1 = str2 + (object) this.CurrentNode;
      }
      return str1;
    }

    private DocumentNodePath InitializePath(SceneNode targetNode, IProperty targetProperty)
    {
      List<NodePathEntry> list = new List<NodePathEntry>();
      List<EditContext> editContextChain = this.GetEditContextChain(targetNode.ViewModel);
      if (editContextChain.Count == 0)
        return new DocumentNodePath((IEnumerable<NodePathEntry>) list);
      DocumentNodePath editingContainerPath1 = editContextChain[editContextChain.Count - 1].EditingContainerPath;
      DocumentCompositeNode documentCompositeNode = targetNode.DocumentNode as DocumentCompositeNode;
      NodePathEntry nodePathEntry;
      if (documentCompositeNode != null)
      {
        nodePathEntry = new NodePathEntry(targetProperty, editingContainerPath1.ContainerNode, (DocumentNode) documentCompositeNode);
        list.Add(nodePathEntry);
      }
      for (int index = editContextChain.Count - 2; index >= 0; --index)
      {
        EditContext editContext1 = editContextChain[index];
        EditContext editContext2 = editContextChain[index + 1];
        if (editContext2.ParentElement != null && editContext2.ParentElement.PropertyKey != null)
        {
          DocumentNodePath nodePath = editContext2.ParentElement.NodePath;
          nodePathEntry = new NodePathEntry((IProperty) editContext2.ParentElement.PropertyKey, nodePath.ContainerNode, nodePath.Node);
        }
        else
        {
          DocumentNodePath editingContainerPath2 = editContext2.EditingContainerPath;
          nodePathEntry = new NodePathEntry(editingContainerPath2.ContainerOwnerProperty, editContext1.EditingContainerPath.ContainerOwner ?? editContext1.EditingContainerPath.Node, editingContainerPath2.ContainerOwner);
        }
        list.Add(nodePathEntry);
      }
      return new DocumentNodePath((IEnumerable<NodePathEntry>) list);
    }

    private List<EditContext> GetEditContextChain(SceneViewModel viewModel)
    {
      List<EditContext> contexts = new List<EditContext>();
      viewModel.EditContextManager.MultiViewModelEditContextWalker.Walk(false, (MultiHistoryCallback) ((context, selectedElementPath, ownerPropertyKey, isGhosted) =>
      {
        if (!isGhosted)
          contexts.Add(context);
        return false;
      }));
      return contexts;
    }
  }
}
