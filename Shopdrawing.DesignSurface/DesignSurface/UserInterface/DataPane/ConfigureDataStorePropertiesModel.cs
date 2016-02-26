// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.ConfigureDataStorePropertiesModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Design.UserInterface.Dialogs;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.SampleData;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class ConfigureDataStorePropertiesModel : IGenericDialogModel
  {
    private static readonly string[] columnNames = new string[3]
    {
      "PropertyName",
      "PropertyValue",
      "PropertyType"
    };
    private static readonly Size dialogSize = new Size(796.0, 543.0);
    private IList<DataStorePropertyEntry> properties = (IList<DataStorePropertyEntry>) new List<DataStorePropertyEntry>();
    private SampleDataSet dataStore;
    private DataGrid dataGrid;

    public IEnumerable<DataStorePropertyEntry> Properties
    {
      get
      {
        return (IEnumerable<DataStorePropertyEntry>) this.properties;
      }
    }

    public ConfigureDataStorePropertiesModel(SampleDataSet dataStore)
    {
      this.dataStore = dataStore;
    }

    public void Initialize(UIElement dialogContent)
    {
      this.GeneratePropertyList();
      this.dataGrid = LogicalTreeHelper.FindLogicalNode((DependencyObject) dialogContent, "DataStoreGrid") as DataGrid;
      this.dataGrid.Loaded += new RoutedEventHandler(this.dataGrid_Loaded);
      this.dataGrid.Unloaded += new RoutedEventHandler(this.dataGrid_Unloaded);
    }

    private void dataGrid_Unloaded(object sender, RoutedEventArgs e)
    {
      this.dataGrid.Loaded -= new RoutedEventHandler(this.dataGrid_Loaded);
      this.dataGrid.Unloaded -= new RoutedEventHandler(this.dataGrid_Unloaded);
    }

    private void dataGrid_Loaded(object sender, RoutedEventArgs e)
    {
      this.GenerateAutomationElementIdForCells(this.dataGrid);
    }

    private void GenerateAutomationElementIdForCells(DataGrid dataGrid)
    {
      if (dataGrid == null)
        return;
      for (int index = 0; index < dataGrid.Columns.Count; ++index)
      {
        DataGridColumn dataGridColumn = dataGrid.Columns[index];
        foreach (DataStorePropertyEntry storePropertyEntry in (IEnumerable<DataStorePropertyEntry>) this.properties)
        {
          FrameworkElement cellContent = dataGridColumn.GetCellContent((object) storePropertyEntry);
          if (cellContent != null)
          {
            DataGridCell dataGridCell = cellContent.Parent as DataGridCell;
            if (dataGridCell != null)
              AutomationElement.SetId((DependencyObject) dataGridCell, "Cell_" + storePropertyEntry.PropertyName + "_" + ConfigureDataStorePropertiesModel.columnNames[index]);
          }
        }
      }
    }

    public Dialog CreateDialog()
    {
      GenericDialog genericDialog = new GenericDialog("Resources\\DataPane\\ConfigureDataStorePropertiesDialog.xaml", StringTable.ConfigureDataStoreDialogTitle, (IGenericDialogModel) this);
      genericDialog.ResizeMode = ResizeMode.CanResize;
      genericDialog.MinWidth = ConfigureDataStorePropertiesModel.dialogSize.Width;
      genericDialog.MinHeight = ConfigureDataStorePropertiesModel.dialogSize.Height;
      genericDialog.Height = ConfigureDataStorePropertiesModel.dialogSize.Height;
      genericDialog.Width = ConfigureDataStorePropertiesModel.dialogSize.Width;
      genericDialog.SizeToContent = SizeToContent.Manual;
      return (Dialog) genericDialog;
    }

    public void Commit()
    {
      IEnumerable<DataStorePropertyEntry> enumerable = Enumerable.Where<DataStorePropertyEntry>((IEnumerable<DataStorePropertyEntry>) this.properties, (Func<DataStorePropertyEntry, bool>) (item => item.IsDirty));
      if (enumerable == null || Enumerable.FirstOrDefault<DataStorePropertyEntry>(enumerable) == null)
        return;
      DocumentCompositeNode documentCompositeNode = (DocumentCompositeNode) this.dataStore.RootNode.Clone(this.dataStore.RootNode.Context);
      if (!(false | this.ProcessChangedPropertyName(enumerable) | this.ProcessChangedTypes(enumerable) | this.ProcessChangedPropertyValue(documentCompositeNode, enumerable)))
        return;
      this.dataStore.CommitChanges(documentCompositeNode, (IMessageDisplayService) null);
    }

    private bool ProcessChangedPropertyValue(DocumentCompositeNode rootNode, IEnumerable<DataStorePropertyEntry> dirtyEntries)
    {
      bool flag = false;
      foreach (DataStorePropertyEntry entry in dirtyEntries)
      {
        if (entry.IsValueDirty)
        {
          this.ChangePropertyValue(rootNode, entry);
          flag = true;
        }
      }
      return flag;
    }

    private bool ProcessChangedPropertyName(IEnumerable<DataStorePropertyEntry> dirtyEntries)
    {
      bool flag = false;
      foreach (DataStorePropertyEntry entry in dirtyEntries)
      {
        if (entry.IsNameDirty)
        {
          this.ChangePropertyName(entry);
          flag = true;
        }
      }
      return flag;
    }

    private bool ProcessChangedTypes(IEnumerable<DataStorePropertyEntry> dirtyEntries)
    {
      bool flag = false;
      foreach (DataStorePropertyEntry entry in dirtyEntries)
      {
        if (entry.IsTypeDirty)
        {
          this.ChangePropertyType(entry);
          flag = true;
        }
      }
      return flag;
    }

    private void ChangePropertyName(DataStorePropertyEntry entry)
    {
      SampleProperty dataSetProperty = entry.DataSetProperty;
      entry.ValidatePropertyName();
      if (string.IsNullOrEmpty(entry.PropertyName))
        return;
      dataSetProperty.Rename(entry.PropertyName);
    }

    private void ChangePropertyValue(DocumentCompositeNode rootNode, DataStorePropertyEntry entry)
    {
      switch (entry.PropertyType)
      {
        case DataStoreType.String:
          rootNode.Properties[(IPropertyId) entry.Property] = (DocumentNode) rootNode.Context.CreateNode(entry.PropertyValue);
          break;
        case DataStoreType.Number:
          rootNode.Properties[(IPropertyId) entry.Property] = rootNode.Context.CreateNode(typeof (double), (object) double.Parse(entry.PropertyValue, (IFormatProvider) CultureInfo.CurrentCulture));
          break;
        case DataStoreType.Boolean:
          rootNode.Properties[(IPropertyId) entry.Property] = rootNode.Context.CreateNode(typeof (bool), (object) (bool) (bool.Parse(entry.PropertyValue) ? true : false));
          break;
      }
    }

    private void ChangePropertyType(DataStorePropertyEntry entry)
    {
      entry.DataSetProperty.ChangeType(DataStorePropertyEntry.SampleTypeFromDataStoreType(entry.PropertyType));
    }

    private void GeneratePropertyList()
    {
      foreach (IProperty property in this.dataStore.RootType.Properties)
        this.properties.Add(new DataStorePropertyEntry(this.dataStore, property, this.properties));
    }
  }
}
