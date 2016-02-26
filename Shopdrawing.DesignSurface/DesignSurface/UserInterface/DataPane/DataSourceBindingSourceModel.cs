// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DataSourceBindingSourceModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class DataSourceBindingSourceModel : NotifyingObject, IBindingSourceModel, INotifyPropertyChanged
  {
    private DataSchemaItemFilter dataSchemaFilter;
    private DataPanelModel model;
    private ObservableCollection<DataSourceItem> dataSources;
    private DataSourceItem currentDataSource;
    private ICollectionView dataSourcesView;

    public DataPanelModel Model
    {
      get
      {
        return this.model;
      }
    }

    public string DisplayName
    {
      get
      {
        return StringTable.DataBindingDialogDataSourceDescription;
      }
    }

    public bool IsEnabled
    {
      get
      {
        return true;
      }
    }

    public string AutomationName
    {
      get
      {
        return "DataSourceBindingSource";
      }
    }

    public ISchema Schema
    {
      get
      {
        if (this.currentDataSource != null)
          return this.currentDataSource.SchemaItem.Schema;
        return (ISchema) null;
      }
    }

    public SchemaItem SchemaItem
    {
      get
      {
        if (this.currentDataSource != null)
          return this.currentDataSource.SchemaItem;
        return (SchemaItem) null;
      }
    }

    public ObservableCollection<DataSourceItem> DataSources
    {
      get
      {
        return this.dataSources;
      }
    }

    public string Path
    {
      get
      {
        if (this.currentDataSource != null)
          return this.currentDataSource.SchemaItem.SelectionPath;
        return string.Empty;
      }
    }

    public string PathDescription
    {
      get
      {
        if (this.currentDataSource != null)
          return this.currentDataSource.SchemaItem.Schema.PathDescription;
        return StringTable.UseCustomPropertyPathDescription;
      }
    }

    public DataSourceBindingSourceModel(DataPanelModel model, DataSchemaItemFilter dataSchemaFilter)
    {
      this.model = model;
      this.Model.PropertyChanged += new PropertyChangedEventHandler(this.Model_PropertyChanged);
      this.dataSchemaFilter = dataSchemaFilter;
      this.dataSources = new ObservableCollection<DataSourceItem>();
      this.dataSourcesView = CollectionViewSource.GetDefaultView((object) this.dataSources);
      this.dataSourcesView.CurrentChanged += new EventHandler(this.DataSourcesView_CurrentChanged);
      this.RefreshDataSources();
    }

    public SceneNode CreateBindingOrData(SceneViewModel viewModel, SceneNode targetNode, IProperty targetProperty)
    {
      if (this.currentDataSource != null && this.currentDataSource.DataSourceNode != null)
        return this.currentDataSource.DataSourceNode.CreateBindingOrData(viewModel, this.currentDataSource.SchemaItem.SelectionPath, targetNode, targetProperty);
      return (SceneNode) null;
    }

    public SceneNode CreateBindingOrData(SceneViewModel viewModel, string bindingPath, SceneNode targetNode, IProperty targetProperty)
    {
      if (this.currentDataSource != null && this.currentDataSource.DataSourceNode != null)
        return this.currentDataSource.DataSourceNode.CreateBindingOrData(viewModel, bindingPath, targetNode, targetProperty);
      return (SceneNode) null;
    }

    public void Unhook()
    {
      if (this.model != null)
      {
        this.model.PropertyChanged -= new PropertyChangedEventHandler(this.Model_PropertyChanged);
        this.model = (DataPanelModel) null;
      }
      this.UnhookCurrentDataSource();
    }

    private void UnhookCurrentDataSource()
    {
      if (this.currentDataSource == null)
        return;
      this.currentDataSource.SchemaItem.PropertyChanged -= new PropertyChangedEventHandler(this.SchemaItem_PropertyChanged);
      if (this.dataSchemaFilter != null)
        this.currentDataSource.SchemaItem.ClearDataSchemaItemFilter();
      this.currentDataSource = (DataSourceItem) null;
    }

    private void RefreshDataSources()
    {
      DataSourceItem dataSourceItem1 = this.currentDataSource;
      this.UnhookCurrentDataSource();
      this.dataSources.Clear();
      foreach (DataSourceItem dataSourceItem2 in this.model.DataSources)
        this.dataSources.Add(dataSourceItem2);
      if (dataSourceItem1 == null)
        return;
      foreach (DataSourceItem dataSourceItem2 in (Collection<DataSourceItem>) this.dataSources)
      {
        if (dataSourceItem2.DataSourceNode.DocumentNode == dataSourceItem1.DataSourceNode.DocumentNode)
        {
          this.UpdateCurrentDataSource(dataSourceItem2);
          break;
        }
      }
    }

    private void UpdateCurrentDataSource(DataSourceItem dataSourceItem)
    {
      this.UnhookCurrentDataSource();
      this.currentDataSource = dataSourceItem;
      if (this.dataSourcesView.CurrentItem != dataSourceItem)
        this.dataSourcesView.MoveCurrentTo((object) dataSourceItem);
      this.OnPropertyChanged("Path");
      this.OnPropertyChanged("PathDescription");
      this.OnPropertyChanged("Schema");
      this.OnPropertyChanged("SchemaItem");
      if (this.currentDataSource == null)
        return;
      this.currentDataSource.SchemaItem.PropertyChanged += new PropertyChangedEventHandler(this.SchemaItem_PropertyChanged);
      if (this.dataSchemaFilter == null)
        return;
      this.currentDataSource.SchemaItem.SetDataSchemaItemFilter(this.dataSchemaFilter);
    }

    private void SchemaItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "SelectionPath"))
        return;
      this.OnPropertyChanged("Path");
    }

    private void DataSourcesView_CurrentChanged(object sender, EventArgs e)
    {
      this.UpdateCurrentDataSource(this.dataSourcesView.CurrentItem as DataSourceItem);
    }

    private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "DataSources"))
        return;
      this.RefreshDataSources();
    }
  }
}
