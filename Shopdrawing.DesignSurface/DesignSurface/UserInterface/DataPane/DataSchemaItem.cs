// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DataSchemaItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.SampleData;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.UserInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class DataSchemaItem : DataModelItemBase, IDragDropHandler
  {
    private bool childrenExpanded;
    private ICommand doubleClickCommand;
    private DataSchemaNodePath nodePath;
    private SchemaItem schemaItem;
    private CollectionViewSource childrenViewSource;
    private DataSchemaItemFilter filter;
    private DataPanelModel model;
    private SampleType pendingSampleType;
    private string pendingName;
    private bool isPendingRename;

    public DataSchemaNode DataSchemaNode
    {
      get
      {
        return this.nodePath.Node;
      }
    }

    public DataSourceNode DataSourceNode
    {
      get
      {
        return this.schemaItem.Schema.DataSource;
      }
    }

    public DataSchemaNodePath DataSchemaNodePath
    {
      get
      {
        return this.nodePath;
      }
    }

    public bool HasLoadedChildren
    {
      get
      {
        return this.childrenExpanded;
      }
    }

    public ICollectionView ChildrenView
    {
      get
      {
        return this.ChildrenViewSource.View;
      }
    }

    public CollectionViewSource ChildrenViewSource
    {
      get
      {
        if (this.childrenViewSource == null)
        {
          this.childrenViewSource = new CollectionViewSource();
          this.childrenViewSource.Source = (object) this.Children;
        }
        return this.childrenViewSource;
      }
    }

    public bool IsSampleDataType
    {
      get
      {
        return this.SampleType != null;
      }
    }

    public SampleDataSet SampleDataSet
    {
      get
      {
        return this.schemaItem.Schema.DataSource.SampleData;
      }
    }

    public bool IsDataSetContextSampleData
    {
      get
      {
        if (this.SampleDataSet != null)
          return this.SampleDataSet.Context == DataSetContext.SampleData;
        return false;
      }
    }

    public bool IsDataSetContextDataStore
    {
      get
      {
        if (this.SampleDataSet != null)
          return this.SampleDataSet.Context == DataSetContext.DataStore;
        return false;
      }
    }

    public SampleType SampleType
    {
      get
      {
        if (this.pendingSampleType != null)
          return this.pendingSampleType;
        return this.DataSchemaNode.SampleType;
      }
    }

    public bool IsSampleBasicType
    {
      get
      {
        SampleType sampleType = this.SampleType;
        if (sampleType != null)
          return sampleType.IsBasicType;
        return false;
      }
    }

    public bool IsSampleCollectionType
    {
      get
      {
        SampleType sampleType = this.SampleType;
        if (sampleType != null)
          return sampleType.IsCollection;
        return false;
      }
    }

    public bool HasChildren
    {
      get
      {
        if (this.HasLoadedChildren)
          return this.Children.Count != 0;
        return this.nodePath.Node.Children.Count != 0;
      }
    }

    public string UniqueId
    {
      get
      {
        DataSchemaItem dataSchemaItem = (DataSchemaItem) this.Parent;
        if (dataSchemaItem == null)
          return this.PathName;
        return dataSchemaItem.UniqueId + "/" + this.PathName;
      }
    }

    public override string DisplayName
    {
      get
      {
        return this.DataSchemaNode.DisplayName;
      }
      set
      {
      }
    }

    public bool IsRenameEnabled
    {
      get
      {
        if (this.DataSchemaNode.Parent != null && this.IsSampleDataType)
          return this.HasNoErrors;
        return false;
      }
    }

    public string PathName
    {
      get
      {
        return this.pendingName ?? this.DataSchemaNode.PathName;
      }
      set
      {
        if (!this.IsRenameEnabled || this.pendingName != null || string.IsNullOrEmpty(value))
          return;
        this.RenameProperty(value);
      }
    }

    public SceneViewModel ViewModel
    {
      get
      {
        return this.model.ViewModel;
      }
    }

    public bool IsPendingRename
    {
      get
      {
        return this.isPendingRename;
      }
      set
      {
        if (this.isPendingRename == value)
          return;
        this.isPendingRename = value;
        this.OnPropertyChanged("IsPendingRename");
      }
    }

    public bool Filtered
    {
      get
      {
        if (this.filter == null)
          return false;
        return !this.filter(this);
      }
    }

    public ICommand DoubleClickCommand
    {
      get
      {
        return this.doubleClickCommand;
      }
    }

    public ICommand RangeSelectCommand
    {
      get
      {
        return (ICommand) new DelegateCommand((DelegateCommand.SimpleEventHandler) (() =>
        {
          if (!this.IsSelectable)
            return;
          DataSchemaItem last = this.SelectionContext.PrimarySelection as DataSchemaItem;
          DataModelItemBase parent = this.Parent;
          if (last != null && parent != null && last.Parent == parent)
          {
            this.SelectionContext.SetSelection((IEnumerable<DataModelItemBase>) this.GenerateSiblingSchemaItemRange(this, last, parent));
          }
          else
          {
            if (this.SelectionContext.Count != 0)
              return;
            this.SelectionContext.Add((DataModelItemBase) this);
          }
        }));
      }
    }

    public ICommand ExtendRangeSelectCommand
    {
      get
      {
        return (ICommand) new DelegateCommand((DelegateCommand.SimpleEventHandler) (() =>
        {
          if (!this.IsSelectable)
            return;
          DataSchemaItem last = this.SelectionContext.PrimarySelection as DataSchemaItem;
          DataModelItemBase parent = this.Parent;
          if (last != null && parent != null && last.Parent == parent)
          {
            foreach (DataModelItemBase dataModelItemBase in this.GenerateSiblingSchemaItemRange(this, last, parent))
            {
              if (!dataModelItemBase.IsSelected)
                this.SelectionContext.Add(dataModelItemBase);
            }
            this.SelectionContext.PrimarySelection = (DataModelItemBase) last;
          }
          else
          {
            if (this.SelectionContext.Count != 0)
              return;
            this.SelectionContext.Add((DataModelItemBase) this);
          }
        }));
      }
    }

    public bool IsRemoveEnabled
    {
      get
      {
        if (this.Parent != null)
          return this.HasNoErrors;
        return false;
      }
    }

    public ICommand RemoveCommand
    {
      get
      {
        if (this.IsRemoveEnabled)
          return (ICommand) new DelegateCommand((DelegateCommand.SimpleEventHandler) (() => this.ChangeEffectivePropertyIfValid((DataSchemaItem.ModifySamplePropertyOperation) ((parentType, property) => property.Delete()))));
        return (ICommand) null;
      }
    }

    public bool IsAddPropertyEnabled
    {
      get
      {
        if (!this.HasNoErrors)
          return false;
        return this.DataSchemaNode.EffectiveType != null;
      }
    }

    public bool IsAddBasicPropertyEnabled
    {
      get
      {
        return this.IsAddPropertyEnabled;
      }
    }

    public bool IsAddCollectionPropertyEnabled
    {
      get
      {
        if (this.DataSourceNode.IsDataStoreSource)
          return false;
        return this.IsAddPropertyEnabled;
      }
    }

    public bool IsAddCompositePropertyEnabled
    {
      get
      {
        if (this.DataSourceNode.IsDataStoreSource)
          return false;
        return this.IsAddPropertyEnabled;
      }
    }

    public bool IsEditCollectionValuesEnabled
    {
      get
      {
        if (!this.HasNoErrors)
          return false;
        DataSchemaNode collectionItem = this.DataSchemaNode.CollectionItem;
        if (collectionItem != null && collectionItem.Children != null)
        {
          foreach (DataSchemaNode dataSchemaNode in collectionItem.Children)
          {
            if (dataSchemaNode.SampleType is SampleBasicType)
              return true;
          }
        }
        return false;
      }
    }

    public bool HasNoErrors
    {
      get
      {
        return string.IsNullOrEmpty(this.DataSourceNode.ErrorMessage);
      }
    }

    public ICommand EditCollectionValuesCommand
    {
      get
      {
        if (this.IsEditCollectionValuesEnabled)
          return (ICommand) new DelegateCommand((DelegateCommand.SimpleEventHandler) (() =>
          {
            ConfigureSampleDataDialog sampleDataDialog = new ConfigureSampleDataDialog(this.DataSchemaNodePath, this.ViewModel.DesignerContext.MessageDisplayService);
            bool? nullable = sampleDataDialog.ShowDialog();
            if (nullable.HasValue && nullable.Value)
              sampleDataDialog.Model.CommitChanges();
            else
              sampleDataDialog.Model.CancelChanges();
          }));
        return (ICommand) null;
      }
    }

    public ICommand AddBasicPropertyCommand
    {
      get
      {
        return (ICommand) new DelegateCommand((DelegateCommand.SimpleEventHandler) (() => this.AddProperty(false)));
      }
    }

    public ICommand AddCompositePropertyCommand
    {
      get
      {
        return (ICommand) new DelegateCommand((DelegateCommand.SimpleEventHandler) (() => this.AddProperty(true)));
      }
    }

    public ICommand AddCollectionCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.AddCollection));
      }
    }

    public bool CanMakeCompositeProperty
    {
      get
      {
        return !this.IsDataSetContextDataStore;
      }
    }

    public bool CanMakeCollectionProperty
    {
      get
      {
        return !this.IsDataSetContextDataStore;
      }
    }

    public bool CanMakeHierarchicalCollection
    {
      get
      {
        if (this.SampleDataSet != null && this.SampleDataSet.Context == DataSetContext.DataStore || !this.HasNoErrors)
          return false;
        SampleCollectionType collectionType = this.DataSchemaNode.SampleType as SampleCollectionType;
        if (collectionType == null)
          return false;
        SampleCompositeType sampleCompositeType = collectionType.ItemSampleType as SampleCompositeType;
        return sampleCompositeType != null && Enumerable.FirstOrDefault<SampleProperty>((IEnumerable<SampleProperty>) sampleCompositeType.SampleProperties, (Func<SampleProperty, bool>) (prop => prop.PropertySampleType == collectionType)) == null;
      }
    }

    public bool IsHierarchicalCollection
    {
      get
      {
        return this.DataSchemaNodePath.IsHierarchicalCollection;
      }
    }

    public ICommand MakeHierarchicalCollectionCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.MakeHierarchicalCollection));
      }
    }

    public DataSchemaItem(DataSchemaNodePath nodePath, SchemaItem schemaItem, DataPanelModel model, SelectionContext<DataModelItemBase> selectionContext)
      : base(selectionContext)
    {
      this.nodePath = nodePath;
      this.schemaItem = schemaItem;
      this.model = model;
      this.doubleClickCommand = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnDoubleClicked));
      this.PropertyChanged += new PropertyChangedEventHandler(this.DataSchemaItem_PropertyChanged);
    }

    private bool RenameProperty(string rawName)
    {
      SampleCompositeType effectiveParentType1 = this.DataSchemaNode.EffectiveParentType;
      SampleProperty sampleProperty1 = effectiveParentType1.GetSampleProperty(this.DataSchemaNode.PathName);
      SampleProperty propertyToRename1 = DataSchemaItem.GetHierarchicalPropertyToRename(sampleProperty1);
      if (propertyToRename1 == null || propertyToRename1 != sampleProperty1)
        return this.RenamePropertyInternal(sampleProperty1, propertyToRename1, rawName);
      for (DataSchemaItem dataSchemaItem = this.Parent as DataSchemaItem; dataSchemaItem != null; dataSchemaItem = dataSchemaItem.Parent as DataSchemaItem)
      {
        SampleCompositeType effectiveParentType2 = dataSchemaItem.DataSchemaNode.EffectiveParentType;
        if (effectiveParentType2 != effectiveParentType1)
        {
          SampleProperty sampleProperty2 = effectiveParentType2 != null ? effectiveParentType2.GetSampleProperty(dataSchemaItem.DataSchemaNode.PathName) : (SampleProperty) null;
          if (sampleProperty2 == null)
            return this.RenamePropertyInternal(sampleProperty1, (SampleProperty) null, rawName);
          SampleProperty propertyToRename2 = DataSchemaItem.GetHierarchicalPropertyToRename(sampleProperty2);
          if (propertyToRename2 != sampleProperty1)
            return this.RenamePropertyInternal(sampleProperty1, (SampleProperty) null, rawName);
          return dataSchemaItem.RenamePropertyInternal(sampleProperty2, propertyToRename2, rawName);
        }
      }
      return this.RenamePropertyInternal(sampleProperty1, (SampleProperty) null, rawName);
    }

    private bool RenamePropertyInternal(SampleProperty sampleProperty, SampleProperty hierarchicalProperty, string rawName)
    {
      string uniquePropertyName = DataSchemaItem.GetUniquePropertyName(sampleProperty, hierarchicalProperty, rawName);
      if (string.IsNullOrEmpty(uniquePropertyName) || uniquePropertyName == sampleProperty.Name && (hierarchicalProperty == null || hierarchicalProperty.Name == uniquePropertyName))
        return false;
      this.pendingName = uniquePropertyName;
      Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (Delegate) (() => this.ChangeEffectivePropertyIfValid((DataSchemaItem.ModifySamplePropertyOperation) ((parentType, property) =>
      {
        string schemaNodePath = (string) null;
        if (this.IsSelected)
        {
          this.model.SelectionContext.Remove((DataModelItemBase) this);
          schemaNodePath = DataSchemaItem.ProvideNodePathForPendingEdit(this.Parent as DataSchemaItem, this.pendingName, sampleProperty, hierarchicalProperty);
        }
        string typeName = rawName.Trim();
        DataSchemaItem.RenamePropertyAndType(sampleProperty, this.pendingName, typeName);
        if (hierarchicalProperty != null)
          DataSchemaItem.RenamePropertyAndType(hierarchicalProperty, this.pendingName, typeName);
        if (schemaNodePath != null)
          this.model.ExtendSelectionUponRebuild(this.DataSourceNode, schemaNodePath);
        this.pendingName = (string) null;
      }))));
      return true;
    }

    private static string GetUniquePropertyName(SampleProperty sampleProperty, SampleProperty hierarchicalProperty, string rawName)
    {
      string uniqueNameForRename1 = sampleProperty.GetUniqueNameForRename(rawName);
      if (string.IsNullOrEmpty(uniqueNameForRename1) || hierarchicalProperty == null || hierarchicalProperty.GetUniqueNameForRename(uniqueNameForRename1) == uniqueNameForRename1)
        return uniqueNameForRename1;
      NumberedName numberedName = new NumberedName(uniqueNameForRename1);
      while (numberedName.Increment())
      {
        if (!(sampleProperty.GetUniqueNameForRename(numberedName.CurrentName) != numberedName.CurrentName))
        {
          string uniqueNameForRename2 = hierarchicalProperty.GetUniqueNameForRename(numberedName.CurrentName);
          if (!(uniqueNameForRename2 != numberedName.CurrentName))
            return uniqueNameForRename2;
        }
      }
      return (string) null;
    }

    private static SampleProperty GetHierarchicalPropertyToRename(SampleProperty sampleProperty)
    {
      SampleCollectionType collectionType = sampleProperty.PropertySampleType as SampleCollectionType;
      if (collectionType == null)
        return (SampleProperty) null;
      SampleCompositeType sampleCompositeType = collectionType.ItemSampleType as SampleCompositeType;
      if (sampleCompositeType == null)
        return (SampleProperty) null;
      SampleProperty hierarchicalProperty = Enumerable.FirstOrDefault<SampleProperty>((IEnumerable<SampleProperty>) sampleCompositeType.SampleProperties, (Func<SampleProperty, bool>) (p => p.PropertySampleType == collectionType));
      if (hierarchicalProperty == null)
        return (SampleProperty) null;
      if (Enumerable.FirstOrDefault<SampleProperty>((IEnumerable<SampleProperty>) sampleCompositeType.SampleProperties, (Func<SampleProperty, bool>) (p =>
      {
        if (p != hierarchicalProperty)
          return p.PropertySampleType == collectionType;
        return false;
      })) != null)
        return (SampleProperty) null;
      return hierarchicalProperty;
    }

    private static void RenamePropertyAndType(SampleProperty property, string propertyName, string typeName)
    {
      if (propertyName == property.Name)
        return;
      property.Rename(propertyName);
      SampleNonBasicType sampleNonBasicType1 = property.PropertySampleType as SampleNonBasicType;
      if (sampleNonBasicType1 == null)
        return;
      string uniqueTypeName1 = sampleNonBasicType1.DeclaringDataSet.GetUniqueTypeName(typeName, sampleNonBasicType1.Name);
      if (string.IsNullOrEmpty(uniqueTypeName1))
        return;
      sampleNonBasicType1.Rename(uniqueTypeName1);
      SampleCollectionType sampleCollectionType = sampleNonBasicType1 as SampleCollectionType;
      SampleNonBasicType sampleNonBasicType2 = sampleCollectionType != null ? sampleCollectionType.ItemSampleType as SampleNonBasicType : (SampleNonBasicType) null;
      if (sampleNonBasicType2 == null || sampleNonBasicType2 == sampleNonBasicType1)
        return;
      string uniqueTypeName2 = sampleNonBasicType2.DeclaringDataSet.GetUniqueTypeName(typeName + "Item", sampleNonBasicType2.Name);
      if (string.IsNullOrEmpty(uniqueTypeName2))
        return;
      sampleNonBasicType2.Rename(uniqueTypeName2);
    }

    private void ChangeEffectivePropertyIfValid(DataSchemaItem.ModifySamplePropertyOperation operation)
    {
      SampleCompositeType effectiveParentType = this.DataSchemaNode.EffectiveParentType;
      if (effectiveParentType == null)
        return;
      SampleDataSet declaringDataSet = effectiveParentType.DeclaringDataSet;
      SampleProperty sampleProperty = effectiveParentType.GetSampleProperty(this.DataSchemaNode.PathName);
      if (sampleProperty == null)
        return;
      operation(effectiveParentType, sampleProperty);
      using (TemporaryCursor.SetWaitCursor())
        declaringDataSet.CommitChanges(this.ViewModel.DesignerContext.MessageDisplayService);
    }

    private List<DataModelItemBase> GenerateSiblingSchemaItemRange(DataSchemaItem first, DataSchemaItem last, DataModelItemBase parent)
    {
      int num1 = parent.Children.IndexOf((DataModelItemBase) last);
      int num2 = parent.Children.IndexOf((DataModelItemBase) first);
      int num3 = num2 < num1 ? 1 : -1;
      List<DataModelItemBase> list = new List<DataModelItemBase>();
      int index = num2;
      while (index != num1 + num3)
      {
        DataSchemaItem dataSchemaItem = parent.Children[index] as DataSchemaItem;
        if (dataSchemaItem != null)
          list.Add((DataModelItemBase) dataSchemaItem);
        index += num3;
      }
      return list;
    }

    public void AddProperty(bool generateTypeForProperty)
    {
      if (!this.schemaItem.Schema.DataSource.IsSampleDataSource)
        return;
      SampleDataSet sampleData = this.schemaItem.Schema.DataSource.SampleData;
      SampleCompositeType effectiveType = this.DataSchemaNode.EffectiveType;
      if (effectiveType == null)
        return;
      this.model.SelectionContext.Clear();
      string uniquePropertyName = effectiveType.GetUniquePropertyName(generateTypeForProperty ? "ComplexProperty" : "Property1");
      if (generateTypeForProperty)
      {
        string uniqueTypeName = sampleData.GetUniqueTypeName(uniquePropertyName + "Type");
        SampleCompositeType compositeType = sampleData.CreateCompositeType(uniqueTypeName);
        effectiveType.AddProperty(uniquePropertyName, (SampleType) compositeType);
      }
      else
        effectiveType.AddProperty(uniquePropertyName, (SampleType) SampleBasicType.String);
      string schemaNodePath = DataSchemaItem.ProvideNodePathForPendingEdit(this, uniquePropertyName);
      this.model.RenameSampleDataSchemaItemUponRebuild(this.schemaItem.Schema.DataSource, schemaNodePath);
      this.model.ExtendSelectionUponRebuild(this.schemaItem.Schema.DataSource, schemaNodePath);
      this.IsExpanded = true;
      using (TemporaryCursor.SetWaitCursor())
        sampleData.CommitChanges(this.ViewModel.DesignerContext.MessageDisplayService);
    }

    public void AddCollection()
    {
      if (!this.schemaItem.Schema.DataSource.IsSampleDataSource)
        return;
      SampleDataSet sampleData = this.schemaItem.Schema.DataSource.SampleData;
      SampleCompositeType effectiveType = this.DataSchemaNode.EffectiveType;
      if (effectiveType == null)
        return;
      this.model.SelectionContext.Clear();
      string uniquePropertyName = effectiveType.GetUniquePropertyName("Collection");
      string uniqueTypeName = sampleData.GetUniqueTypeName(uniquePropertyName + "Item");
      SampleCompositeType compositeType = sampleData.CreateCompositeType(uniqueTypeName);
      SampleCollectionType collectionType = sampleData.CreateCollectionType((SampleNonBasicType) compositeType);
      effectiveType.AddProperty(uniquePropertyName, (SampleType) collectionType);
      string schemaNodePath = DataSchemaItem.ProvideNodePathForPendingEdit(this, uniquePropertyName);
      this.model.RenameSampleDataSchemaItemUponRebuild(this.schemaItem.Schema.DataSource, schemaNodePath);
      this.model.ExtendSelectionUponRebuild(this.schemaItem.Schema.DataSource, schemaNodePath);
      this.IsExpanded = true;
      using (TemporaryCursor.SetWaitCursor())
        sampleData.CommitChanges(this.ViewModel.DesignerContext.MessageDisplayService);
    }

    public void MakeHierarchicalCollection()
    {
      if (!this.schemaItem.Schema.DataSource.IsSampleDataSource)
        return;
      SampleDataSet sampleData = this.schemaItem.Schema.DataSource.SampleData;
      SampleCompositeType effectiveType = this.DataSchemaNode.EffectiveType;
      if (effectiveType == null)
        return;
      this.model.SelectionContext.Clear();
      SampleNonBasicType sampleNonBasicType = (SampleNonBasicType) this.DataSchemaNode.SampleType;
      string uniquePropertyName1 = effectiveType.GetUniquePropertyName(this.DataSchemaNode.PathName);
      SampleProperty hierarchicalProperty = effectiveType.AddProperty(uniquePropertyName1, (SampleType) sampleNonBasicType);
      SampleProperty sampleProperty = this.DataSchemaNode.EffectiveParentType.GetSampleProperty(this.DataSchemaNode.PathName);
      string uniquePropertyName2 = DataSchemaItem.GetUniquePropertyName(sampleProperty, hierarchicalProperty, uniquePropertyName1);
      string schemaNodePath = DataSchemaItem.ProvideNodePathForPendingEdit(this, uniquePropertyName2, sampleProperty, hierarchicalProperty);
      sampleProperty.Rename(uniquePropertyName2);
      hierarchicalProperty.Rename(uniquePropertyName2);
      this.model.RenameSampleDataSchemaItemUponRebuild(this.schemaItem.Schema.DataSource, schemaNodePath);
      this.model.ExtendSelectionUponRebuild(this.schemaItem.Schema.DataSource, schemaNodePath);
      this.IsExpanded = true;
      using (TemporaryCursor.SetWaitCursor())
        sampleData.CommitChanges(this.ViewModel.DesignerContext.MessageDisplayService);
    }

    public void EnsureChildrenExpanded()
    {
      if (this.childrenExpanded)
        return;
      this.childrenExpanded = true;
      this.schemaItem.ProcessChildren(this);
    }

    public void RefreshFilter()
    {
      this.OnPropertyChanged("Filtered");
    }

    public void SetFilter(DataSchemaItemFilter filter)
    {
      this.filter = filter;
      this.RefreshFilter();
    }

    public override int CompareTo(DataModelItemBase treeItem)
    {
      DataSchemaItem dataSchemaItem = (DataSchemaItem) treeItem;
      return StringLogicalComparer.Instance.Compare(this.PathName, dataSchemaItem.PathName);
    }

    public void ExpandAncestors()
    {
      if (this.Parent != null)
        ((DataSchemaItem) this.Parent).ExpandAncestors();
      this.IsExpanded = true;
    }

    public void Dispose()
    {
      this.nodePath = (DataSchemaNodePath) null;
    }

    private static string ProvideNodePathForPendingEdit(DataSchemaItem parentItem, string newPropertyName, params SampleProperty[] renamedProperties)
    {
      DataSchemaNodePath dataSchemaNodePath = parentItem.DataSchemaNodePath;
      DataSchemaNode endNode = (DataSchemaNode) null;
      if (dataSchemaNodePath.IsCollection)
        endNode = dataSchemaNodePath.EffectiveCollectionItemNode;
      if (endNode == null)
        endNode = dataSchemaNodePath.Node;
      if (endNode == null)
        return newPropertyName;
      string inheritedPath = (string) null;
      if (renamedProperties.Length == 0)
      {
        if (endNode != dataSchemaNodePath.Node)
          dataSchemaNodePath = new DataSchemaNodePath(dataSchemaNodePath.Schema, endNode);
        return ClrObjectSchema.CombinePaths(dataSchemaNodePath.Path, newPropertyName);
      }
      List<string> list = new List<string>()
      {
        newPropertyName
      };
      string pathName = endNode.PathName;
      for (DataSchemaNode parent = endNode.Parent; parent != null; parent = parent.Parent)
      {
        SampleCompositeType sampleCompositeType = parent.SampleType as SampleCompositeType;
        if (sampleCompositeType == null)
        {
          list.Add(pathName);
        }
        else
        {
          string name = pathName;
          SampleProperty sampleProperty = sampleCompositeType.GetSampleProperty(name);
          if (Enumerable.FirstOrDefault<SampleProperty>((IEnumerable<SampleProperty>) renamedProperties, (Func<SampleProperty, bool>) (p => p == sampleProperty)) != null)
            name = newPropertyName;
          list.Add(name);
        }
        if (parent != dataSchemaNodePath.Schema.Root)
          pathName = parent.PathName;
        else
          break;
      }
      for (int index = list.Count - 1; index >= 0; --index)
        inheritedPath = ClrObjectSchema.CombinePaths(inheritedPath, list[index]);
      return inheritedPath;
    }

    private void OnDoubleClicked()
    {
      if (this.IsSampleBasicType)
        return;
      this.IsExpanded = !this.IsExpanded;
    }

    private void DataSchemaItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "IsExpanded"))
        return;
      foreach (DataSchemaItem dataSchemaItem in Enumerable.OfType<DataSchemaItem>((IEnumerable) this.Children))
        dataSchemaItem.EnsureChildrenExpanded();
    }

    public void OnDragBegin(DragBeginEventArgs e)
    {
      if (this.nodePath == null)
        return;
      List<DataSchemaItem> list1 = new List<DataSchemaItem>(Enumerable.OfType<DataSchemaItem>((IEnumerable) this.SelectionContext));
      if (list1.Count <= 0 || list1.Count != this.SelectionContext.Count || !list1.Contains(this))
        return;
      DataSchemaNode parent = list1[0].DataSchemaNode.Parent;
      DataSourceNode dataSourceNode = list1[0].DataSourceNode;
      List<DataSchemaNodePath> list2 = new List<DataSchemaNodePath>();
      list2.Add(list1[0].nodePath.AbsolutePath);
      for (int index = 1; index < list1.Count; ++index)
      {
        if (parent != list1[index].DataSchemaNode.Parent || dataSourceNode != list1[index].DataSourceNode)
          return;
        list2.Add(list1[index].nodePath.AbsolutePath);
      }
      DataSchemaNodePathCollection nodePathCollection = new DataSchemaNodePathCollection((IEnumerable<DataSchemaNodePath>) list2);
      using (DataBindingDragDropManager.GetDragDropToken())
      {
        int num = (int) DragSourceHelper.DoDragDrop(e.DragSource, (object) nodePathCollection, DragDropEffects.Copy | DragDropEffects.Move);
      }
    }

    public void OnDragOver(DragEventArgs e)
    {
    }

    public void OnDragEnter(DragEventArgs e)
    {
    }

    public void OnDragLeave(DragEventArgs e)
    {
    }

    public void OnDrop(DragEventArgs e)
    {
    }

    public void OnGiveFeedback(GiveFeedbackEventArgs e)
    {
    }

    public void OnQueryContinueDrag(QueryContinueDragEventArgs e)
    {
    }

    private delegate void ModifySamplePropertyOperation(SampleCompositeType parentType, SampleProperty property);
  }
}
