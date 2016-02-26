// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DataSourceItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.SampleData;
using Microsoft.Expression.Framework.Data;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  [DebuggerDisplay("{DisplayName}")]
  public class DataSourceItem : DataModelItemBase
  {
    private DataHostItem host;
    private SchemaItem schemaItem;
    private DataPanelModel model;
    private DataSchemaItem activeChildItem;

    public virtual ContextMenu ContextMenu
    {
      get
      {
        ContextMenu contextMenu = new ContextMenu();
        contextMenu.Name = "DataSourceContextMenu";
        if (this.DataSourceNode != null && this.DataSourceNode.IsSampleDataSource && !this.DataSourceNode.IsDataStoreSource)
        {
          bool hasErrors = this.HasErrors;
          MenuItem menuItem1 = new MenuItem();
          menuItem1.Header = (object) StringTable.ReImportSampleDataFromXml;
          menuItem1.Name = "ReImportSampleDataFromXml";
          menuItem1.Command = (ICommand) new DelegateCommand((DelegateCommand.SimpleEventHandler) (() =>
          {
            this.model.ReimportSampleDataFromXml(this.DataSourceNode.SampleData);
            this.OnPropertyChanged("ContextMenu");
          }));
          menuItem1.IsEnabled = !hasErrors;
          contextMenu.Items.Add((object) menuItem1);
          MenuItem menuItem2 = new MenuItem();
          menuItem2.Header = (object) StringTable.EnableSampleDataAtRuntime;
          menuItem2.Name = "EnableSampleDataSourceAtRuntime";
          menuItem2.IsChecked = this.DataSourceNode.SampleData.IsEnabledAtRuntime;
          menuItem2.IsEnabled = !hasErrors;
          menuItem2.Command = (ICommand) new DelegateCommand((DelegateCommand.SimpleEventHandler) (() =>
          {
            this.DataSourceNode.SampleData.EnableAtRuntime(!this.DataSourceNode.SampleData.IsEnabledAtRuntime);
            this.OnPropertyChanged("ContextMenu");
          }));
          contextMenu.Items.Add((object) menuItem2);
          contextMenu.Items.Add((object) new Separator());
        }
        MenuItem menuItem = new MenuItem();
        menuItem.Header = (object) string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.RemoveDataSourceContextMenuItem, new object[1]
        {
          (object) this.DisplayName
        });
        menuItem.Name = "RemoveDataSource";
        menuItem.Command = this.DeleteCommand;
        contextMenu.Items.Add((object) menuItem);
        return contextMenu;
      }
    }

    public virtual bool CanEditData
    {
      get
      {
        return false;
      }
    }

    public virtual bool HasErrors
    {
      get
      {
        return !string.IsNullOrEmpty(this.DataSourceNode.ErrorMessage);
      }
    }

    public virtual string ErrorMessage
    {
      get
      {
        return this.DataSourceNode.ErrorMessage;
      }
    }

    public ICommand DeleteCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.RemoveDataSource));
      }
    }

    public DataHostItem Host
    {
      get
      {
        return this.host;
      }
      set
      {
        if (this.host == value)
          return;
        this.host = value;
        this.OnPropertyChanged("Host");
      }
    }

    public bool IsActive
    {
      get
      {
        if (this.activeChildItem != null)
          return this.activeChildItem.DataSchemaNode == this.DataSourceNode.Schema.Root;
        return false;
      }
    }

    public bool IsChildItemActive
    {
      get
      {
        if (this.activeChildItem != null)
          return !this.IsActive;
        return false;
      }
    }

    public DataSchemaItem ActiveChildItem
    {
      get
      {
        return this.activeChildItem;
      }
      set
      {
        if (this.activeChildItem == value)
          return;
        this.activeChildItem = value;
        this.OnPropertyChanged("ActiveChildItem");
        this.OnPropertyChanged("IsChildItemActive");
        this.OnPropertyChanged("IsActive");
      }
    }

    public DataSourceNode DataSourceNode
    {
      get
      {
        return this.schemaItem.Schema.DataSource;
      }
    }

    public SchemaItem SchemaItem
    {
      get
      {
        return this.schemaItem;
      }
    }

    public virtual string UniqueId
    {
      get
      {
        if (this.host != null)
          return this.host.UniqueId + "/" + this.DataSourceNode.Name;
        return this.DataSourceNode.Name;
      }
    }

    public override string DisplayName
    {
      get
      {
        return this.DataSourceNode.Name;
      }
      set
      {
      }
    }

    public virtual string ToolTip
    {
      get
      {
        SampleDataSet sampleData = this.DataSourceNode.SampleData;
        if (sampleData != null)
        {
          if (sampleData.DataSetType == DataSetType.SampleDataSet)
            return StringTable.SampleDataSourceTooltip;
          if (sampleData.DataSetType == DataSetType.DataStoreSet)
            return StringTable.DataStoreSourceTooltip;
          return (string) null;
        }
        if (this.DataSourceNode.Schema is XmlSchema)
          return StringTable.XmlDataSourceTooltip;
        return StringTable.ObjectDataSourceTooltip;
      }
    }

    public ICommand ConfigureDataStoreCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.ConfigureDataStore));
      }
    }

    public DataSourceItem(ISchema schema, DataPanelModel model)
      : base(model.SelectionContext)
    {
      this.model = model;
      this.schemaItem = new SchemaItem(schema, model, model.SelectionContext);
    }

    internal void RefreshSchema(ISchema schema)
    {
      this.schemaItem = new SchemaItem(schema, this.model, this.model.SelectionContext);
      this.activeChildItem = (DataSchemaItem) null;
      this.OnPropertyChanged((string) null);
    }

    public override int CompareTo(DataModelItemBase treeItem)
    {
      return this.DisplayName.CompareTo(treeItem.DisplayName);
    }

    private void RemoveDataSource()
    {
      this.model.RemoveDataSource(this, true);
    }

    public void OnDisplayNameChanged()
    {
      if (this.DataSourceNode != null)
        this.DataSourceNode.OnNameChanged();
      this.OnPropertyChanged("DisplayName");
    }

    private void ConfigureDataStore()
    {
      ConfigureDataStorePropertiesModel storePropertiesModel = new ConfigureDataStorePropertiesModel(this.DataSourceNode.SampleData);
      bool? nullable = storePropertiesModel.CreateDialog().ShowDialog();
      if ((!nullable.GetValueOrDefault() ? 0 : (nullable.HasValue ? true : false)) == 0)
        return;
      storePropertiesModel.Commit();
    }

    public class Comparer : System.Collections.Generic.Comparer<DataSourceItem>
    {
      public override int Compare(DataSourceItem x, DataSourceItem y)
      {
        bool flag1 = y == null || y.DataSourceNode == null || (y.DataSourceNode.DocumentNode == null || y.DataSourceNode.DocumentNode.Context == null) || y.DataSourceNode.DocumentNode.Context.DocumentUrl == null;
        bool flag2 = x == null || x.DataSourceNode == null || (x.DataSourceNode.DocumentNode == null || x.DataSourceNode.DocumentNode.Context == null) || x.DataSourceNode.DocumentNode.Context.DocumentUrl == null;
        if (flag1)
          return !flag2 ? true : false;
        if (flag2)
          return -1;
        if (x.DataSourceNode.DocumentNode.Context != y.DataSourceNode.DocumentNode.Context)
          return x.DataSourceNode.DocumentNode.Context.DocumentUrl.CompareTo(y.DataSourceNode.DocumentNode.Context.DocumentUrl);
        return x.DataSourceNode.DocumentNode.Marker.CompareTo((object) y.DataSourceNode.DocumentNode.Marker);
      }
    }
  }
}
