// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.ElementNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class ElementNode : INotifyPropertyChanged, ISelectable
  {
    private SceneElement element;
    private SelectionContext<ElementNode> selectionContext;
    private ElementNode parent;
    private ObservableCollectionWorkaround<ElementNode> children;
    private bool isSelectable;
    private bool isSelected;
    private bool isExpanded;
    private DelegateCommand selectCommand;
    private DelegateCommand doubleClickCommand;

    public SceneElement Element
    {
      get
      {
        return this.element;
      }
    }

    public ICommand SelectCommand
    {
      get
      {
        return (ICommand) this.selectCommand;
      }
    }

    public ICommand DoubleClickCommand
    {
      get
      {
        return (ICommand) this.doubleClickCommand;
      }
    }

    public string Name
    {
      get
      {
        return this.element.DisplayName;
      }
    }

    public ObservableCollectionWorkaround<ElementNode> Children
    {
      get
      {
        return this.children;
      }
    }

    public bool HasChildren
    {
      get
      {
        return this.children.Count > 0;
      }
    }

    public bool IsExpanded
    {
      get
      {
        return this.isExpanded;
      }
      set
      {
        this.isExpanded = value;
        this.OnPropertyChanged("IsExpanded");
      }
    }

    public bool IsSelected
    {
      get
      {
        return this.isSelected;
      }
      set
      {
        this.isSelected = value;
        this.OnPropertyChanged("IsSelected");
      }
    }

    public bool IsSelectable
    {
      get
      {
        return this.isSelectable;
      }
      set
      {
        this.isSelectable = value;
        this.selectCommand.IsEnabled = value;
        this.OnPropertyChanged("IsSelectable");
      }
    }

    public ElementNode Parent
    {
      get
      {
        return this.parent;
      }
      set
      {
        this.parent = value;
        this.OnPropertyChanged("Parent");
      }
    }

    public int TreeDepth
    {
      get
      {
        int num = 0;
        for (ElementNode parent = this.Parent; parent != null; parent = parent.Parent)
          ++num;
        return num;
      }
    }

    public string UniqueId
    {
      get
      {
        return this.Name;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public ElementNode(SceneElement element, SelectionContext<ElementNode> selectionContext)
    {
      this.element = element;
      this.selectionContext = selectionContext;
      this.children = new ObservableCollectionWorkaround<ElementNode>();
      this.selectCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.ElementNode_OnSelected));
      this.doubleClickCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.ElementNode_OnDoubleClick));
      this.isSelectable = true;
    }

    public void AddChild(ElementNode child)
    {
      this.children.Add(child);
      child.Parent = this;
      this.OnPropertyChanged("Children");
      this.OnPropertyChanged("HasChildren");
    }

    public void RemoveChild(ElementNode child)
    {
      this.children.Remove(child);
      child.Parent = (ElementNode) null;
      this.OnPropertyChanged("Children");
      this.OnPropertyChanged("HasChildren");
    }

    public ElementNode FindDescendantByName(string name)
    {
      if (string.Equals(this.element.Name, name, StringComparison.Ordinal))
        return this;
      foreach (ElementNode elementNode in (Collection<ElementNode>) this.Children)
      {
        ElementNode descendantByName = elementNode.FindDescendantByName(name);
        if (descendantByName != null)
          return descendantByName;
      }
      return (ElementNode) null;
    }

    public ElementNode FindDescendantSceneNode(SceneNode sceneNode)
    {
      if (this.element == sceneNode)
        return this;
      foreach (ElementNode elementNode in (Collection<ElementNode>) this.Children)
      {
        ElementNode descendantSceneNode = elementNode.FindDescendantSceneNode(sceneNode);
        if (descendantSceneNode != null)
          return descendantSceneNode;
      }
      return (ElementNode) null;
    }

    public void ExpandAncestors()
    {
      this.IsExpanded = true;
      if (this.Parent == null)
        return;
      this.Parent.ExpandAncestors();
    }

    protected virtual void OnSelected()
    {
      this.selectionContext.SetSelection(this);
    }

    protected virtual void OnDoubleClick()
    {
      this.IsExpanded = !this.IsExpanded;
    }

    private void ElementNode_OnSelected()
    {
      this.OnSelected();
    }

    private void ElementNode_OnDoubleClick()
    {
      this.OnDoubleClick();
    }

    protected void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
