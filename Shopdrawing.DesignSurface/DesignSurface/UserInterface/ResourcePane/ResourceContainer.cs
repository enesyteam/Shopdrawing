// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourcePane.ResourceContainer
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.UserInterface.ResourcePane
{
  public abstract class ResourceContainer : ResourceEntryBase
  {
    private ObservableCollection<ResourceItem> resourceItems = new ObservableCollection<ResourceItem>();
    private bool isInScope;
    private ResourceManager manager;
    private DispatcherTimer timer;

    protected override ResourceManager ResourceManager
    {
      get
      {
        return this.manager;
      }
    }

    protected override ResourceContainer DragDropTargetContainer
    {
      get
      {
        return this;
      }
    }

    public bool DocumentHasErrors
    {
      get
      {
        if (this.DocumentNode != null && this.DocumentNode.DocumentRoot != null)
          return !this.DocumentNode.DocumentRoot.IsEditable;
        return true;
      }
    }

    public bool IsInScope
    {
      get
      {
        return this.isInScope;
      }
      internal set
      {
        if (this.isInScope == value)
          return;
        this.isInScope = value;
        this.OnPropertyChanged("IsInScope");
      }
    }

    public virtual ObservableCollection<ResourceItem> ResourceItems
    {
      get
      {
        return this.resourceItems;
      }
    }

    public abstract string Name { get; }

    public abstract ISupportsResources ResourcesCollection { get; }

    public abstract DocumentReference DocumentReference { get; }

    public abstract SceneDocument Document { get; }

    public abstract SceneViewModel ViewModel { get; }

    public virtual IDocumentContext DocumentContext
    {
      get
      {
        return this.ViewModel.Document.DocumentContext;
      }
    }

    public IProjectContext ProjectContext
    {
      get
      {
        if (this.Document == null)
          return (IProjectContext) null;
        return this.Document.ProjectContext;
      }
    }

    public abstract ResourceDictionaryNode ResourceDictionaryNode { get; }

    public abstract SceneNode Node { get; }

    public virtual bool IsEditable
    {
      get
      {
        return true;
      }
    }

    protected ResourceContainer(ResourceManager manager)
    {
      this.manager = manager;
    }

    public virtual void EnsureResourceDictionaryNode()
    {
    }

    public virtual void EnsureEditable()
    {
    }

    public override string ToString()
    {
      return this.Name;
    }

    public virtual void RefreshName()
    {
      this.OnPropertyChanged("Name");
    }

    public override void OnDragEnter(DragEventArgs e)
    {
      base.OnDragEnter(e);
      e.Handled = true;
      if (this.timer == null)
      {
        this.timer = new DispatcherTimer();
        this.timer.Interval = SystemParameters.MouseHoverTime.Add(SystemParameters.MouseHoverTime);
        this.timer.Tick += new EventHandler(this.Timer_Tick);
      }
      else
        this.timer.Stop();
      this.timer.Start();
    }

    public override void OnDrop(DragEventArgs e)
    {
      this.DoDrop(e, 0);
      base.OnDrop(e);
      this.StopTimer();
    }

    public override void OnDragLeave(DragEventArgs e)
    {
      base.OnDragLeave(e);
      this.StopTimer();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
      if (!this.IsExpanded)
        this.IsExpanded = true;
      this.StopTimer();
    }

    private void StopTimer()
    {
      if (this.timer == null)
        return;
      this.timer.Stop();
      this.timer.Tick -= new EventHandler(this.Timer_Tick);
      this.timer = (DispatcherTimer) null;
    }

    internal void MoveItem(ResourceEntryItem resourceEntry, int destinationIndex)
    {
      using (SceneEditTransaction editTransaction = this.ViewModel.CreateEditTransaction(StringTable.UndoUnitMoveResource))
      {
        DictionaryEntryNode dictionaryEntryNode = (DictionaryEntryNode) resourceEntry.Resource.ResourceNode.SceneNode;
        int num = this.ResourceDictionaryNode.IndexOf(dictionaryEntryNode);
        if (num == -1)
          return;
        if (destinationIndex > num)
          --destinationIndex;
        if (!this.ResourceDictionaryNode.Remove(dictionaryEntryNode))
          return;
        if (destinationIndex >= 0 && destinationIndex < this.ResourceDictionaryNode.Count)
          this.ResourceDictionaryNode.Insert(destinationIndex, dictionaryEntryNode);
        else
          this.ResourceDictionaryNode.Add(dictionaryEntryNode);
        editTransaction.Commit();
      }
    }

    public virtual void Close()
    {
    }
  }
}
