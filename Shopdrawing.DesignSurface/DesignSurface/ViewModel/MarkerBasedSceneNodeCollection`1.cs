// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.MarkerBasedSceneNodeCollection`1
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.Framework.Data;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class MarkerBasedSceneNodeCollection<T> : ISceneNodeCollection<T>, IOrderedList<T>, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable where T : SceneNode
  {
    private DocumentNodeMarkerSortedListOf<DocumentNode> markers;
    private SceneViewModel viewModel;

    public DocumentNodeMarkerSortedListBase Markers
    {
      get
      {
        return (DocumentNodeMarkerSortedListBase) this.markers;
      }
    }

    public T this[int index]
    {
      get
      {
        return SceneNode.FromMarker<T>(this.markers.MarkerAt(index), this.viewModel);
      }
      set
      {
        throw new NotSupportedException(ExceptionStringTable.UseAddInsteadToPreserveSorting);
      }
    }

    public int Count
    {
      get
      {
        return this.markers.Count;
      }
    }

    public bool IsReadOnly
    {
      get
      {
        return false;
      }
    }

    public int? FixedCapacity
    {
      get
      {
        return new int?();
      }
    }

    public MarkerBasedSceneNodeCollection(SceneViewModel viewModel)
    {
      this.viewModel = viewModel;
      this.markers = new DocumentNodeMarkerSortedListOf<DocumentNode>();
    }

    public MarkerBasedSceneNodeCollection(SceneViewModel viewModel, DocumentNodeMarkerSortedListOf<DocumentNode> other)
    {
      this.viewModel = viewModel;
      this.markers = new DocumentNodeMarkerSortedListOf<DocumentNode>(other);
    }

    public MarkerBasedSceneNodeCollection(SceneViewModel viewModel, ICollection<T> otherList)
    {
      this.viewModel = viewModel;
      MarkerBasedSceneNodeCollection<T> sceneNodeCollection = otherList as MarkerBasedSceneNodeCollection<T>;
      if (sceneNodeCollection != null)
      {
        this.markers = new DocumentNodeMarkerSortedListOf<DocumentNode>(sceneNodeCollection.markers);
      }
      else
      {
        this.markers = new DocumentNodeMarkerSortedListOf<DocumentNode>();
        foreach (T node in (IEnumerable<T>) otherList)
        {
          if ((object) node != null && this.IsElementValid(node))
            this.markers.Add(node.DocumentNode.Marker, node.DocumentNode);
        }
      }
    }

    public MarkerBasedSceneNodeCollection<T> FilterOutDeletedMarkers()
    {
      for (int index = 0; index < this.markers.Count; ++index)
      {
        if (this.markers.MarkerAt(index).IsDeleted)
        {
          MarkerBasedSceneNodeCollection<T> sceneNodeCollection = new MarkerBasedSceneNodeCollection<T>(this.viewModel, this.markers);
          sceneNodeCollection.markers.CullDeletedMarkers();
          return sceneNodeCollection;
        }
      }
      return (MarkerBasedSceneNodeCollection<T>) null;
    }

    public void RestoreDeletedMarkers()
    {
      for (int index = 0; index < this.markers.Count; ++index)
      {
        if (this.markers.MarkerAt(index).IsDeleted)
        {
          DocumentNode documentNode = this.markers.ValueAt(index);
          if (documentNode.Marker != null)
          {
            this.markers.RemoveAt(index);
            this.markers.InsertAt(index, documentNode.Marker, documentNode);
          }
          else
          {
            DocumentNodeMarker marker = this.markers.MarkerAt(index);
            this.markers.RemoveAt(index);
            if (this.markers.Add(marker, documentNode) > index)
              --index;
          }
        }
      }
    }

    public int BinarySearch(T item)
    {
      return this.markers.FindPosition(item.DocumentNode.Marker);
    }

    public int IndexOf(T item)
    {
      int num = (object) item != null ? this.markers.FindPosition(item.DocumentNode.Marker) : -1;
      if (num >= 0)
        return num;
      return -1;
    }

    public void Insert(int index, T item)
    {
      if (!this.IsElementValid(item))
        return;
      this.markers.InsertAt(index, item.DocumentNode.Marker, item.DocumentNode);
    }

    public void RemoveAt(int index)
    {
      this.markers.RemoveAt(index);
    }

    public void Add(T item)
    {
      if (!this.IsElementValid(item))
        return;
      this.markers.Add(item.DocumentNode.Marker, item.DocumentNode);
    }

    public void Clear()
    {
      this.markers.Clear();
    }

    public bool Contains(T item)
    {
      return this.IndexOf(item) >= 0;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      for (int index = 0; index < this.markers.Count; ++index)
      {
        SceneNode sceneNode = (SceneNode) null;
        DocumentNode node = this.markers.MarkerAt(index).Node;
        if (node != null)
          sceneNode = this.viewModel.GetSceneNode(node);
        array[arrayIndex + index] = (T) sceneNode;
      }
    }

    public bool Remove(T item)
    {
      int position = this.markers.FindPosition(item.DocumentNode.Marker);
      if (position < 0)
        return false;
      this.markers.RemoveAt(position);
      return true;
    }

    public IEnumerator<T> GetEnumerator()
    {
      for (int i = 0; i < this.markers.Count; ++i)
      {
        DocumentNode node = this.markers.MarkerAt(i).Node;
        if (node != null)
          yield return (T) this.viewModel.GetSceneNode(node);
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }

    private bool IsElementValid(T node)
    {
      return node.IsInDocument && node.ViewModel == this.viewModel;
    }
  }
}
