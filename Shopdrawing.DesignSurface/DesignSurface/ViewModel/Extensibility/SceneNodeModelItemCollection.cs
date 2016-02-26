// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.Extensibility.SceneNodeModelItemCollection
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Microsoft.Expression.DesignSurface.ViewModel.Extensibility
{
  public class SceneNodeModelItemCollection : ModelItemCollection, ISceneNodeModelItem
  {
    private SceneNode parent;
    private IProperty property;
    private SceneViewModel viewModel;
    private ISceneNodeCollection<SceneNode> sceneNodeCollection;

    public SceneNode SceneNode
    {
      get
      {
        return this.parent;
      }
    }

    public override EditingContext Context
    {
      get
      {
        return this.viewModel.ExtensibilityManager.EditingContext;
      }
    }

    public override ModelProperty Content
    {
      get
      {
        return (ModelProperty) null;
      }
    }

    public override int Count
    {
      get
      {
        return this.sceneNodeCollection.Count;
      }
    }

    public override ModelEventCollection Events
    {
      get
      {
        return (ModelEventCollection) null;
      }
    }

    protected override bool IsFixedSize
    {
      get
      {
        return this.sceneNodeCollection.FixedCapacity.HasValue;
      }
    }

    public override bool IsReadOnly
    {
      get
      {
        return this.sceneNodeCollection.IsReadOnly;
      }
    }

    protected override bool IsSynchronized
    {
      get
      {
        return false;
      }
    }

    public override Type ItemType
    {
      get
      {
        return this.property.PropertyType.ItemType.RuntimeType;
      }
    }

    public override string Name
    {
      get
      {
        return (string) null;
      }
      set
      {
      }
    }

    public override ModelItem Parent
    {
      get
      {
        return (ModelItem) this.parent.ModelItem;
      }
    }

    public override ModelPropertyCollection Properties
    {
      get
      {
        return (ModelPropertyCollection) null;
      }
    }

    public override ModelProperty Source
    {
      get
      {
        return (ModelProperty) new SceneNodeModelProperty(this.parent.ModelItem, this.property);
      }
    }

    public override ModelItem this[int index]
    {
      get
      {
        return (ModelItem) this.sceneNodeCollection[index].ModelItem;
      }
      set
      {
        using (SceneEditTransaction editTransaction = this.viewModel.CreateEditTransaction(StringTable.UndoUnitAddItemDefault))
        {
          this.sceneNodeCollection[index] = ((SceneNodeModelItem) value).SceneNode;
          editTransaction.Commit();
        }
      }
    }

    public override ModelItem Root
    {
      get
      {
        return (ModelItem) this.parent.ViewModel.RootNode.ModelItem;
      }
    }

    public override ViewItem View
    {
      get
      {
        return (ViewItem) new SceneNodeViewItem(this.parent.GetLocalValueAsSceneNode((IPropertyId) this.property));
      }
    }

    public override event NotifyCollectionChangedEventHandler CollectionChanged;

    public override event PropertyChangedEventHandler PropertyChanged;

    public SceneNodeModelItemCollection(ISceneNodeCollection<SceneNode> collection, SceneViewModel viewModel, SceneNode parent, IProperty property)
    {
      this.sceneNodeCollection = collection;
      this.viewModel = viewModel;
      this.parent = parent;
      this.property = property;
    }

    public override void Add(ModelItem item)
    {
      using (SceneEditTransaction editTransaction = this.viewModel.CreateEditTransaction(StringTable.UndoUnitAddItemDefault))
      {
        this.sceneNodeCollection.Add(((ISceneNodeModelItem) item).SceneNode);
        editTransaction.Commit();
      }
    }

    public override ModelItem Add(object value)
    {
      ModelItem modelItem = this.CreateModelItem(value);
      this.Add(modelItem);
      return modelItem;
    }

    public override IEnumerable<object> GetAttributes(Type attributeType)
    {
      foreach (Attribute attribute in TypeUtilities.GetAttributes((MemberInfo) this.ItemType, attributeType, true))
        yield return (object) attribute;
    }

    public override ModelEditingScope BeginEdit()
    {
      return this.BeginEdit(StringTable.ExtensibilityEditTransactionDescription);
    }

    public override ModelEditingScope BeginEdit(string description)
    {
      return this.viewModel.ExtensibilityManager.CreateEditingScope(description);
    }

    public override void Clear()
    {
      using (SceneEditTransaction editTransaction = this.viewModel.CreateEditTransaction(StringTable.UndoUnitDelete))
      {
        for (int index = this.sceneNodeCollection.Count - 1; index >= 0; --index)
          this.sceneNodeCollection.RemoveAt(index);
        editTransaction.Commit();
      }
    }

    public void OnCollectionChanged()
    {
      if (this.CollectionChanged == null)
        return;
      this.CollectionChanged((object) this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    public override bool Contains(ModelItem item)
    {
      return this.sceneNodeCollection.Contains(((ISceneNodeModelItem) item).SceneNode);
    }

    public override bool Contains(object value)
    {
      foreach (SceneNode sceneNode in (IEnumerable<SceneNode>) this.sceneNodeCollection)
      {
        if (sceneNode.ViewObject != null && object.Equals(value, sceneNode.ViewObject.PlatformSpecificObject))
          return true;
      }
      return false;
    }

    public override void CopyTo(ModelItem[] array, int arrayIndex)
    {
      for (int index = 0; index < this.sceneNodeCollection.Count; ++index)
        array[index + arrayIndex] = (ModelItem) this.sceneNodeCollection[index].ModelItem;
    }

    public override object GetCurrentValue()
    {
      return this.parent.GetComputedValue((IPropertyId) this.property);
    }

    public override IEnumerator<ModelItem> GetEnumerator()
    {
      foreach (SceneNode sceneNode in (IEnumerable<SceneNode>) this.sceneNodeCollection)
        yield return (ModelItem) sceneNode.ModelItem;
    }

    public override int IndexOf(ModelItem item)
    {
      return this.sceneNodeCollection.IndexOf(((ISceneNodeModelItem) item).SceneNode);
    }

    public override void Insert(int index, ModelItem item)
    {
      using (SceneEditTransaction editTransaction = this.viewModel.CreateEditTransaction(StringTable.UndoUnitAddItemDefault))
      {
        this.sceneNodeCollection.Insert(index, ((ISceneNodeModelItem) item).SceneNode);
        editTransaction.Commit();
      }
    }

    public override ModelItem Insert(int index, object value)
    {
      ModelItem modelItem = this.CreateModelItem(value);
      this.Insert(index, modelItem);
      return modelItem;
    }

    private ModelItem CreateModelItem(object value)
    {
      ModelItem modelItem = value as ModelItem;
      if (modelItem != null)
        return modelItem;
      if (value != null && !(value is DocumentNode) && this.SceneNode.ProjectContext.GetType(value.GetType()) == null)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.TypeIsNotResolveableWithinProject, new object[1]
        {
          (object) value.GetType().FullName
        }));
      return (ModelItem) this.viewModel.CreateSceneNode(value).ModelItem;
    }

    public override void Move(int fromIndex, int toIndex)
    {
      using (SceneEditTransaction editTransaction = this.viewModel.CreateEditTransaction(StringTable.UndoUnitMove))
      {
        SceneNode sceneNode = this.sceneNodeCollection[fromIndex];
        this.sceneNodeCollection.RemoveAt(fromIndex);
        this.Insert(toIndex, (ModelItem) sceneNode.ModelItem);
        editTransaction.Commit();
      }
    }

    public void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    public override bool Remove(ModelItem item)
    {
      bool flag = false;
      using (SceneEditTransaction editTransaction = this.viewModel.CreateEditTransaction(StringTable.UndoUnitDelete))
      {
        flag = this.sceneNodeCollection.Remove(((ISceneNodeModelItem) item).SceneNode);
        editTransaction.Commit();
      }
      return flag;
    }

    public override bool Remove(object value)
    {
      ModelItem modelItem = value as ModelItem;
      if (modelItem == null)
      {
        foreach (SceneNode sceneNode in (IEnumerable<SceneNode>) this.sceneNodeCollection)
        {
          if (object.Equals(sceneNode.ViewObject.PlatformSpecificObject, value))
          {
            modelItem = (ModelItem) sceneNode.ModelItem;
            break;
          }
        }
        if (modelItem == null)
          return false;
      }
      return this.Remove(modelItem);
    }

    public override void RemoveAt(int index)
    {
      using (SceneEditTransaction editTransaction = this.viewModel.CreateEditTransaction(StringTable.UndoUnitDelete))
      {
        this.sceneNodeCollection.RemoveAt(index);
        editTransaction.Commit();
      }
    }
  }
}
