// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DataHostItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  [DebuggerDisplay("{DisplayName}")]
  public class DataHostItem : DataModelItemBase
  {
    private DataSourceHost dataSourceHost;
    private ICommand doubleClickCommand;
    private string name;
    private string automationId;
    private bool documentHasErrors;

    public DataSourceHost DataSourceHost
    {
      get
      {
        return this.dataSourceHost;
      }
    }

    public ObservableCollectionAggregator DataSources { get; private set; }

    internal IDocumentContext DocumentContext { get; set; }

    internal DataSourcesCollection<DataSourceItem> ResourceDataSources { get; private set; }

    internal DataSourcesCollection<FileBasedDataSourceItem> FileDataSources { get; private set; }

    public bool HasChildren
    {
      get
      {
        return this.Children.Count != 0;
      }
    }

    public bool DocumentHasErrors
    {
      get
      {
        return this.documentHasErrors;
      }
      private set
      {
        this.documentHasErrors = value;
        this.OnPropertyChanged("DocumentHasErrors");
        this.OnPropertyChanged("ErrorMessage");
      }
    }

    public string ErrorMessage
    {
      get
      {
        string str = (string) null;
        if (this.documentHasErrors && this.DocumentContext != null)
          str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.DataHostError, new object[1]
          {
            (object) DocumentReference.Create(this.DocumentContext.TypeResolver.ProjectPath).GetRelativePath(DocumentReference.Create(this.DocumentContext.DocumentUrl))
          });
        return str;
      }
    }

    public bool HasDataSources
    {
      get
      {
        if (this.ResourceDataSources.Count <= 0)
          return this.FileDataSources.Count > 0;
        return true;
      }
    }

    public ICommand DoubleClickCommand
    {
      get
      {
        return this.doubleClickCommand;
      }
    }

    public string UniqueId
    {
      get
      {
        return this.automationId;
      }
    }

    public bool DocumentRequired { get; set; }

    public override string DisplayName
    {
      get
      {
        return this.name;
      }
      set
      {
      }
    }

    public DataHostItem(DataSourceHost dataSourceHost, DataPanelModel model, string name, string automationId)
      : base(model.SelectionContext)
    {
      this.dataSourceHost = dataSourceHost;
      this.DataSources = new ObservableCollectionAggregator();
      this.ResourceDataSources = new DataSourcesCollection<DataSourceItem>(this);
      this.FileDataSources = new DataSourcesCollection<FileBasedDataSourceItem>(this);
      this.DataSources.AddCollection((IList) this.ResourceDataSources);
      this.DataSources.AddCollection((IList) this.FileDataSources);
      this.doubleClickCommand = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnDoubleClick));
      this.name = name;
      this.automationId = automationId;
      this.documentHasErrors = this.CalculateDocumentHasErrors();
    }

    public void ExpandAncestors()
    {
      this.IsExpanded = true;
      if (this.Parent == null)
        return;
      ((DataHostItem) this.Parent).ExpandAncestors();
    }

    public void UpdateDocumentHasErrors()
    {
      this.DocumentHasErrors = this.CalculateDocumentHasErrors();
    }

    internal void ClearDataSourceHost()
    {
      this.dataSourceHost = (DataSourceHost) null;
      this.ResourceDataSources.Clear();
      this.FileDataSources.Clear();
    }

    internal void RefreshDataSource(DataSourceHost dataSourceHost)
    {
      this.ClearDataSourceHost();
      this.dataSourceHost = dataSourceHost;
      this.DocumentHasErrors = this.CalculateDocumentHasErrors();
    }

    internal void OnDataSourceAdded(DataSourceItem dataSource)
    {
      dataSource.Host = this;
      this.OnPropertyChanged("HasDataSources");
    }

    internal void OnDataSourceRemoved(DataSourceItem dataSource, bool shouldNotify)
    {
      dataSource.Host = (DataHostItem) null;
      if (!shouldNotify)
        return;
      this.OnPropertyChanged("HasDataSources");
    }

    internal void OnDataSourcesCleared()
    {
      this.OnPropertyChanged("HasDataSources");
    }

    public override int CompareTo(DataModelItemBase treeItem)
    {
      return this.DataSourceHost.DocumentNode.Marker.CompareTo((object) ((DataHostItem) treeItem).DataSourceHost.DocumentNode.Marker);
    }

    private void OnDoubleClick()
    {
      this.IsExpanded = !this.IsExpanded;
    }

    private bool CalculateDocumentHasErrors()
    {
      if (this.dataSourceHost == null)
        return true;
      if (this.dataSourceHost.DocumentNode == null)
        return this.DocumentRequired;
      if (this.dataSourceHost.DocumentNode.DocumentRoot != null)
        return !this.dataSourceHost.DocumentNode.DocumentRoot.IsEditable;
      return true;
    }
  }
}
