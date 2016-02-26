// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.DataStorePropertiesValueProvider
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Design.UserInterface.Dialogs;
using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.SampleData;
using Microsoft.Expression.DesignSurface.UserInterface.DataPane;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class DataStorePropertiesValueProvider
  {
    private static List<DataStorePropertiesValueProvider> Providers = new List<DataStorePropertiesValueProvider>();
    private FrameworkElement hostControl;
    private DataStorePropertyEntry editingProperty;
    private DataStorePropertyEntry lastEditingProperty;
    private List<DataStorePropertyEntry> properties;

    public string PropertyNameEntry { get; set; }

    public DataStorePropertyEntry SelectedProperty
    {
      get
      {
        return this.editingProperty;
      }
      set
      {
        this.lastEditingProperty = this.editingProperty;
        this.editingProperty = value;
        if (this.editingProperty == null)
          return;
        this.CreateBinding(this.editingProperty);
      }
    }

    public IList<DataStorePropertyEntry> Properties
    {
      get
      {
        return (IList<DataStorePropertyEntry>) this.properties;
      }
    }

    public DataStorePropertyEntry EditingProperty
    {
      get
      {
        return this.editingProperty;
      }
      set
      {
        this.editingProperty = value;
      }
    }

    private SceneDocument ActiveDocument
    {
      get
      {
        return this.ObjectSet.Document;
      }
    }

    private SceneNodeProperty SceneNodeProperty
    {
      get
      {
        SceneNodeProperty sceneNodeProperty = this.hostControl.DataContext as SceneNodeProperty;
        if (sceneNodeProperty != null)
          return sceneNodeProperty;
        SceneNodePropertyValue nodePropertyValue = this.hostControl.DataContext as SceneNodePropertyValue;
        if (nodePropertyValue != null)
          return (SceneNodeProperty) nodePropertyValue.get_ParentProperty();
        return (SceneNodeProperty) null;
      }
    }

    private SceneNodeObjectSetBase ObjectSet
    {
      get
      {
        return (SceneNodeObjectSetBase) this.SceneNodeProperty.ObjectSet;
      }
    }

    private ProjectXamlContext ProjectXamlContext
    {
      get
      {
        return ProjectXamlContext.FromProjectContext(this.ObjectSet.ProjectContext);
      }
    }

    private SceneNode OwnerObject
    {
      get
      {
        return this.ObjectSet.Objects[0];
      }
    }

    public event EventHandler Rebuilt;

    public DataStorePropertiesValueProvider(FrameworkElement hostControl)
    {
      this.hostControl = hostControl;
      this.hostControl.Loaded += new RoutedEventHandler(this.OnPropertyPickerEditorLoaded);
      this.hostControl.Unloaded += new RoutedEventHandler(this.OnPropertyPickerEditorUnloaded);
      this.hostControl.DataContextChanged += new DependencyPropertyChangedEventHandler(this.OnPropertyPickerEditorDataContextChanged);
    }

    private static void FireRebuildForAllProviders()
    {
      foreach (DataStorePropertiesValueProvider propertiesValueProvider in DataStorePropertiesValueProvider.Providers)
        propertiesValueProvider.Rebuild();
    }

    private void OnPropertyPickerEditorLoaded(object sender, RoutedEventArgs e)
    {
      DataStorePropertiesValueProvider.Providers.Add(this);
      this.UpdateFromDataContext();
    }

    private void OnPropertyPickerEditorUnloaded(object sender, RoutedEventArgs e)
    {
      DataStorePropertiesValueProvider.Providers.Remove(this);
      DataStorePropertiesValueProvider.FireRebuildForAllProviders();
    }

    private void OnPropertyPickerEditorDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
    }

    private void UpdateFromDataContext()
    {
      this.Rebuild();
    }

    public void Rebuild()
    {
      if (this.SceneNodeProperty == null)
        return;
      ProjectXamlContext projectXamlContext = this.ProjectXamlContext;
      if (this.ProjectXamlContext != null)
      {
        SampleDataCollection sampleData = projectXamlContext.SampleData;
        IEnumerable<DataStoreReferenceEntry> dataStoreDataSet = this.FindDataStoreDataSet((Predicate<SampleDataSet>) (dataSet => true));
        HashSet<string> hashSet = new HashSet<string>();
        List<DataStorePropertyEntry> list = new List<DataStorePropertyEntry>();
        list.Add(new DataStorePropertyEntry((SampleDataSet) null, StringTable.CreateNewPropertyItem, true));
        this.properties = list;
        if (dataStoreDataSet != null)
        {
          foreach (DataStoreReferenceEntry storeReferenceEntry in dataStoreDataSet)
          {
            SampleDataSet dataStore = storeReferenceEntry.DataStore;
            if (!hashSet.Contains(dataStore.Name))
            {
              hashSet.Add(dataStore.Name);
              for (int index = 0; index < storeReferenceEntry.DataStore.RootType.Properties.Count; ++index)
                list.Add(new DataStorePropertyEntry(storeReferenceEntry.DataStore, storeReferenceEntry.DataStore.RootType.Properties[index].Name, false));
            }
          }
          if (this.editingProperty != null)
            this.editingProperty = Enumerable.FirstOrDefault<DataStorePropertyEntry>(Enumerable.Where<DataStorePropertyEntry>((IEnumerable<DataStorePropertyEntry>) this.properties, (Func<DataStorePropertyEntry, bool>) (entry => entry.Name == this.SelectedProperty.Name)));
        }
      }
      if (this.Rebuilt == null)
        return;
      this.Rebuilt(this, (EventArgs) null);
    }

    public DataStorePropertyEntry FindMatchDataStorePropertyEntry(SceneNodeProperty property, string propertyName)
    {
      if (property == null)
        return (DataStorePropertyEntry) null;
      bool isMixed = false;
      DocumentNode valueAsDocumentNode = property.GetLocalValueAsDocumentNode(false, out isMixed);
      if (valueAsDocumentNode != null && property.SceneNodeObjectSet.ViewModel.IsExternal(valueAsDocumentNode))
        return (DataStorePropertyEntry) null;
      BindingSceneNode bindingSceneNode = property.SceneNodeObjectSet.ViewModel.GetSceneNode(valueAsDocumentNode) as BindingSceneNode;
      if (bindingSceneNode == null)
        return (DataStorePropertyEntry) null;
      string path = propertyName ?? bindingSceneNode.Path;
      string dataStore = (string) null;
      DocumentCompositeNode node = bindingSceneNode.Source as DocumentCompositeNode;
      if (node != null && PlatformTypes.StaticResource.IsAssignableFrom((ITypeId) node.Type))
      {
        DocumentPrimitiveNode documentPrimitiveNode = ResourceNodeHelper.GetResourceKey(node) as DocumentPrimitiveNode;
        if (documentPrimitiveNode != null)
          dataStore = documentPrimitiveNode.GetValue<string>();
      }
      return Enumerable.FirstOrDefault<DataStorePropertyEntry>(Enumerable.Where<DataStorePropertyEntry>((IEnumerable<DataStorePropertyEntry>) this.Properties, (Func<DataStorePropertyEntry, bool>) (entry =>
      {
        if (entry.Name == path)
          return entry.DataSetName == dataStore;
        return false;
      })));
    }

    private ResourceDictionaryContentProvider GetResourceDictionaryContentProvider(SceneDocument document)
    {
      return this.ObjectSet.DesignerContext.ResourceManager.FindContentProviderForResourceDictionary(document);
    }

    private IEnumerable<DataStoreReferenceEntry> FindDataStoreDataSet(Predicate<SampleDataSet> predicate)
    {
      IEnumerable<DataStoreReferenceEntry> referenceDictionary = this.FindDataStoreReferenceDictionary(this.ActiveDocument);
      IEnumerable<DataStoreReferenceEntry> source1 = referenceDictionary != null ? Enumerable.Where<DataStoreReferenceEntry>(referenceDictionary, (Func<DataStoreReferenceEntry, bool>) (entry => predicate(entry.DataStore))) : (IEnumerable<DataStoreReferenceEntry>) null;
      if (source1 != null && Enumerable.FirstOrDefault<DataStoreReferenceEntry>(source1) != null)
        return source1;
      if (this.ObjectSet.ViewModel != null && this.ObjectSet.ViewModel.DataPanelModel.SharedDocument != null)
        source1 = this.FindDataStoreReferenceDictionary(this.ObjectSet.ViewModel.DataPanelModel.SharedDocument);
      IEnumerable<DataStoreReferenceEntry> source2 = source1 != null ? Enumerable.Where<DataStoreReferenceEntry>(source1, (Func<DataStoreReferenceEntry, bool>) (entry => predicate(entry.DataStore))) : (IEnumerable<DataStoreReferenceEntry>) null;
      if (source2 != null && Enumerable.FirstOrDefault<DataStoreReferenceEntry>(source2) != null)
        return source2;
      return (IEnumerable<DataStoreReferenceEntry>) null;
    }

    private IEnumerable<DataStoreReferenceEntry> FindDataStoreReferenceDictionary(SceneDocument document)
    {
      List<DataStoreReferenceEntry> list = (List<DataStoreReferenceEntry>) null;
      ResourceDictionaryContentProvider dictionaryContentProvider = this.GetResourceDictionaryContentProvider(document);
      if (dictionaryContentProvider != null)
      {
        foreach (DocumentNode documentNode1 in dictionaryContentProvider.Items)
        {
          DocumentCompositeNode documentCompositeNode = documentNode1 as DocumentCompositeNode;
          if (documentCompositeNode != null)
          {
            DocumentNode documentNode2 = documentCompositeNode.Properties[DictionaryEntryNode.ValueProperty];
            if (documentNode2 != null)
            {
              SampleDataSet sampleDataSet = SampleDataSet.SampleDataSetFromType(documentNode2.Type.RuntimeType);
              if (sampleDataSet != null && sampleDataSet.Context == DataSetContext.DataStore)
              {
                if (list == null)
                  list = new List<DataStoreReferenceEntry>();
                DataStoreReferenceEntry storeReferenceEntry = new DataStoreReferenceEntry()
                {
                  DataStore = sampleDataSet,
                  DictionaryEntryNode = documentCompositeNode
                };
                list.Add(storeReferenceEntry);
              }
            }
          }
        }
      }
      return (IEnumerable<DataStoreReferenceEntry>) list;
    }

    private void CreateBinding(DataStorePropertyEntry propertyEntry)
    {
      string path = (string) null;
      bool flag = false;
      DataStoreReferenceEntry storeReferenceEntry = (DataStoreReferenceEntry) null;
      using (SceneEditTransaction editTransaction = this.ObjectSet.ViewModel.CreateEditTransaction(StringTable.ConditionChangeUndo))
      {
        if (propertyEntry.Renamed || propertyEntry.IsCreateNewPropertyEntry)
        {
          IEnumerable<DataStoreReferenceEntry> dataStoreDataSet = this.FindDataStoreDataSet((Predicate<SampleDataSet>) (dataSet => true));
          if (dataStoreDataSet != null)
          {
            IEnumerable<DataStoreReferenceEntry> source = (IEnumerable<DataStoreReferenceEntry>) null;
            if (propertyEntry.Renamed)
              source = Enumerable.Where<DataStoreReferenceEntry>(dataStoreDataSet, (Func<DataStoreReferenceEntry, bool>) (entry => Enumerable.Count<SampleProperty>(Enumerable.Where<SampleProperty>((IEnumerable<SampleProperty>) entry.DataStore.RootType.SampleProperties, (Func<SampleProperty, bool>) (property => property.Name == propertyEntry.Name))) > 0));
            if (source != null && Enumerable.FirstOrDefault<DataStoreReferenceEntry>(source) != null)
            {
              storeReferenceEntry = Enumerable.FirstOrDefault<DataStoreReferenceEntry>(source);
              path = propertyEntry.Name;
            }
            else
            {
              storeReferenceEntry = Enumerable.FirstOrDefault<DataStoreReferenceEntry>(dataStoreDataSet);
              if (propertyEntry.IsCreateNewPropertyEntry)
              {
                AddDataStorePropertyDialogModel model = new AddDataStorePropertyDialogModel(dataStoreDataSet);
                bool? nullable = new GenericDialog("Resources\\DataPane\\AddDataStorePropertyDialog.xaml", StringTable.AddDataStorePropertyDialogTitle, (IGenericDialogModel) model).ShowDialog();
                if ((!nullable.GetValueOrDefault() ? 0 : (nullable.HasValue ? 1 : 0)) != 0)
                {
                  propertyEntry.Name = model.PropertyName;
                  storeReferenceEntry = Enumerable.FirstOrDefault<DataStoreReferenceEntry>(Enumerable.Where<DataStoreReferenceEntry>(dataStoreDataSet, (Func<DataStoreReferenceEntry, bool>) (entry => entry.DataStore.Name == model.SelectedDataStore)));
                }
                else
                {
                  this.editingProperty = this.lastEditingProperty;
                  editTransaction.Cancel();
                  this.Rebuild();
                  return;
                }
              }
              path = this.CreateNewProperty(storeReferenceEntry.DataStore, propertyEntry.Name);
              flag = true;
            }
          }
          if (storeReferenceEntry == null)
          {
            storeReferenceEntry = this.CreateNewDataStore(editTransaction);
            if (storeReferenceEntry != null)
            {
              if (propertyEntry.IsCreateNewPropertyEntry)
              {
                IList<DataStoreReferenceEntry> list = (IList<DataStoreReferenceEntry>) new List<DataStoreReferenceEntry>();
                list.Add(storeReferenceEntry);
                AddDataStorePropertyDialogModel propertyDialogModel = new AddDataStorePropertyDialogModel((IEnumerable<DataStoreReferenceEntry>) list);
                bool? nullable = new GenericDialog("Resources\\DataPane\\AddDataStorePropertyDialog.xaml", StringTable.AddDataStorePropertyDialogTitle, (IGenericDialogModel) propertyDialogModel).ShowDialog();
                if ((!nullable.GetValueOrDefault() ? 0 : (nullable.HasValue ? 1 : 0)) != 0)
                {
                  propertyEntry.Name = propertyDialogModel.PropertyName;
                }
                else
                {
                  this.editingProperty = this.lastEditingProperty;
                  editTransaction.Cancel();
                  this.Rebuild();
                  return;
                }
              }
              path = this.CreateNewProperty(storeReferenceEntry.DataStore, propertyEntry.Name);
            }
            flag = true;
          }
        }
        else
        {
          storeReferenceEntry = Enumerable.FirstOrDefault<DataStoreReferenceEntry>(this.FindDataStoreDataSet((Predicate<SampleDataSet>) (dataSet =>
          {
            if (dataSet.Name != propertyEntry.DataSetName)
              return false;
            return Enumerable.Count<SampleProperty>(Enumerable.Where<SampleProperty>((IEnumerable<SampleProperty>) dataSet.RootType.SampleProperties, (Func<SampleProperty, bool>) (property => property.Name == propertyEntry.Name))) > 0;
          })));
          path = propertyEntry.Name;
        }
        if (storeReferenceEntry != null && path != null)
        {
          ISchema schemaForDataSource = SchemaManager.GetSchemaForDataSource(storeReferenceEntry.DictionaryEntryNode.Properties[DictionaryEntryNode.ValueProperty]);
          this.ObjectSet.ViewModel.BindingEditor.CreateAndSetBindingOrData(this.OwnerObject, (IPropertyId) this.SceneNodeProperty.Reference.LastStep, this.PropertyNameEntry != null ? schemaForDataSource.CreateNodePath() : schemaForDataSource.GetNodePathFromPath(path), false);
          if (this.PropertyNameEntry != null)
          {
            IType type = this.ObjectSet.RepresentativeSceneNode.Type;
            IProperty property;
            for (property = (IProperty) null; property == null && type != null; type = type.BaseType)
              property = type.GetMember(MemberType.LocalProperty, this.PropertyNameEntry, MemberAccessTypes.All) as IProperty;
            if (property != null)
              this.ObjectSet.RepresentativeSceneNode.SetValue((IPropertyId) property, (object) path);
          }
          propertyEntry.Name = path;
        }
        editTransaction.Commit();
        if (!flag)
          return;
        DataStorePropertiesValueProvider.FireRebuildForAllProviders();
      }
    }

    private DataStoreReferenceEntry CreateNewDataStore(SceneEditTransaction transation)
    {
      SampleDataSet sampleDataSet = this.ProjectXamlContext.SampleData.CreateSampleDataSet(DataSetContext.DataStore, "DataStore", true);
      sampleDataSet.Save();
      this.ObjectSet.ViewModel.DataPanelModel.AddDataStoreDataSource(sampleDataSet, true);
      transation.Update();
      IEnumerable<DataStoreReferenceEntry> dataStoreDataSet = this.FindDataStoreDataSet((Predicate<SampleDataSet>) (dataSet => true));
      if (dataStoreDataSet != null)
        return Enumerable.FirstOrDefault<DataStoreReferenceEntry>(dataStoreDataSet);
      return (DataStoreReferenceEntry) null;
    }

    private string CreateNewProperty(SampleDataSet dataSet, string newProperyName)
    {
      string uniqueTypeName = dataSet.GetUniqueTypeName(newProperyName);
      SampleProperty sampleProperty = dataSet.RootType.AddProperty(uniqueTypeName, (SampleType) SampleBasicType.String);
      DocumentCompositeNode newRootNode = (DocumentCompositeNode) dataSet.RootNode.Clone(dataSet.RootNode.Context);
      newRootNode.Properties[(IPropertyId) sampleProperty] = (DocumentNode) newRootNode.Context.CreateNode(StringTable.DefaultValueDataStore);
      dataSet.CommitChanges(newRootNode, this.ObjectSet.ViewModel.DesignerContext.MessageDisplayService);
      return uniqueTypeName;
    }
  }
}
