// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.ResourceDictionaryNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class ResourceDictionaryNode : SceneNode, IList<DictionaryEntryNode>, ICollection<DictionaryEntryNode>, IEnumerable<DictionaryEntryNode>, IEnumerable
  {
    public static readonly IPropertyId MergedDictionariesProperty = (IPropertyId) PlatformTypes.ResourceDictionary.GetMember(MemberType.LocalProperty, "MergedDictionaries", MemberAccessTypes.Public);
    public static readonly IPropertyId SourceProperty = (IPropertyId) PlatformTypes.ResourceDictionary.GetMember(MemberType.LocalProperty, "Source", MemberAccessTypes.Public);
    public static readonly ResourceDictionaryNode.ConcreteResourceDictionaryNodeFactory Factory = new ResourceDictionaryNode.ConcreteResourceDictionaryNodeFactory();

    public Uri Source
    {
      get
      {
        return ((DocumentCompositeNode) this.DocumentNode).GetUriValue(ResourceDictionaryNode.SourceProperty);
      }
      set
      {
        DocumentNode documentNode = (DocumentNode) null;
        if (value != (Uri) null)
          documentNode = DocumentNodeUtilities.NewUriDocumentNode(this.DocumentContext, value);
        ((DocumentCompositeNode) this.DocumentNode).Properties[ResourceDictionaryNode.SourceProperty] = documentNode;
      }
    }

    public string DesignTimeSource
    {
      get
      {
        Uri source = this.Source;
        if (source != (Uri) null)
        {
          Uri uri = this.DocumentContext.MakeDesignTimeUri(source);
          if (uri != (Uri) null)
            return uri.OriginalString;
        }
        return (string) null;
      }
    }

    public IList<ResourceDictionaryNode> MergedDictionaries
    {
      get
      {
        return (IList<ResourceDictionaryNode>) new SceneNode.SceneNodeCollection<ResourceDictionaryNode>((SceneNode) this, ResourceDictionaryNode.MergedDictionariesProperty);
      }
    }

    private DocumentCompositeNode Node
    {
      get
      {
        return this.DocumentNode as DocumentCompositeNode;
      }
    }

    public bool IsReadOnly
    {
      get
      {
        return false;
      }
    }

    public bool IsFixedSize
    {
      get
      {
        return false;
      }
    }

    public DictionaryEntryNode this[int index]
    {
      get
      {
        return this.ViewModel.GetSceneNode(this.Node.Children[index]) as DictionaryEntryNode;
      }
      set
      {
        this.VerifyDetached((SceneNode) value);
        this.RemoveAt(index);
        this.Insert(index, value);
      }
    }

    public int Count
    {
      get
      {
        return this.Node.Children.Count;
      }
    }

    public string GetUniqueResourceKey(string name)
    {
      return new ResourceSite(this.DocumentNode).GetUniqueResourceKey(name);
    }

    private void VerifyDetached(SceneNode item)
    {
      if (item == null)
        throw new ArgumentNullException("item");
      if (item.IsAttached)
        throw new InvalidOperationException(ExceptionStringTable.SceneNodeCollectionCannotPerformOperationOnElementInTree);
    }

    public bool Contains(DictionaryEntryNode item)
    {
      return this.IndexOf(item) != -1;
    }

    public bool ContainsKey(object key)
    {
      foreach (DictionaryEntryNode dictionaryEntryNode in this)
      {
        if (dictionaryEntryNode.Key != null && dictionaryEntryNode.Key.Equals(key))
          return true;
      }
      return false;
    }

    public bool Remove(DictionaryEntryNode item)
    {
      int index = this.IndexOf(item);
      if (index < 0)
        return false;
      this.RemoveInternal(index);
      return true;
    }

    public bool RemoveEntryWithValue(SceneNode value)
    {
      if (value == null)
        return false;
      for (int index = 0; index < this.Count; ++index)
      {
        if (this[index].Value == value)
        {
          this.RemoveAt(index);
          return true;
        }
      }
      return false;
    }

    public void RemoveAt(int index)
    {
      this.RemoveInternal(index);
    }

    public void Clear()
    {
      for (int index = this.Count - 1; index >= 0; --index)
        this.RemoveAt(index);
    }

    public void Add(DictionaryEntryNode item)
    {
      this.VerifyDetached((SceneNode) item);
      this.OnChildAdding((SceneNode) item);
      this.Node.Children.Add(item.DocumentNode);
      this.OnChildAdded((SceneNode) item);
    }

    public void Insert(int index, DictionaryEntryNode item)
    {
      this.VerifyDetached((SceneNode) item);
      this.OnChildAdding((SceneNode) item);
      this.Node.Children.Insert(index, item.DocumentNode);
      this.OnChildAdded((SceneNode) item);
    }

    public int IndexOf(DictionaryEntryNode item)
    {
      if (item == null)
        throw new ArgumentNullException("item");
      return this.Node.Children.IndexOf(item.DocumentNode);
    }

    public void CopyTo(DictionaryEntryNode[] array, int arrayIndex)
    {
      for (int index = 0; index < this.Count; ++index)
        array[arrayIndex + index] = this[index];
    }

    public IEnumerator<DictionaryEntryNode> GetEnumerator()
    {
      for (int i = 0; i < this.Count; ++i)
        yield return this[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }

    private void RemoveInternal(int index)
    {
      DictionaryEntryNode dictionaryEntryNode = this[index];
      this.OnChildRemoving((SceneNode) dictionaryEntryNode);
      this.Node.Children.RemoveAt(index);
      this.OnChildRemoved((SceneNode) dictionaryEntryNode);
    }

    public class ConcreteResourceDictionaryNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new ResourceDictionaryNode();
      }

      public ResourceDictionaryNode Instantiate(SceneViewModel viewModel)
      {
        return (ResourceDictionaryNode) this.Instantiate(viewModel, PlatformTypes.ResourceDictionary);
      }
    }
  }
}
