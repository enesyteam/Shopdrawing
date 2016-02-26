// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.SampleDataDialogColumn
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.SampleData;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class SampleDataDialogColumn : DataGridTemplateColumn, INotifyPropertyChanged
  {
    private SampleDataRow editingRow;
    private SampleDataProperty sampleDataProperty;
    private ConfigureSampleDataDialog dialog;

    public string Name
    {
      get
      {
        return this.sampleDataProperty.Name;
      }
    }

    public ConfigureSampleDataDialog Dialog
    {
      get
      {
        return this.dialog;
      }
    }

    public SampleDataProperty SampleDataProperty
    {
      get
      {
        return this.sampleDataProperty;
      }
    }

    public SampleBasicType SampleType
    {
      get
      {
        return this.SampleDataProperty.SampleType;
      }
    }

    public ICommand ConfigureColumnType
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnConfigureColumnType));
      }
    }

    public ImageSource ButtonTypeIcon
    {
      get
      {
        return ((Image) this.Dialog.SampleDataGrid.Resources[(object) (this.sampleDataProperty.SampleTypeConfiguration.SampleType.Name + "MouseOut")]).Source;
      }
    }

    public ImageSource ButtonTypeIconMouseOver
    {
      get
      {
        return ((Image) this.Dialog.SampleDataGrid.Resources[(object) (this.sampleDataProperty.SampleTypeConfiguration.SampleType.Name + "MouseOver")]).Source;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public SampleDataDialogColumn(SampleDataProperty property, ConfigureSampleDataDialog dialog)
    {
      this.dialog = dialog;
      this.sampleDataProperty = property;
      this.SetTemplateForColumn();
    }

    public void SetConfigurationValue(ConfigurationPlaceholder placeholder, object value)
    {
      this.sampleDataProperty.SampleTypeConfiguration.SetConfigurationValue(placeholder, value);
      this.sampleDataProperty.UpdateSampleProperty();
      if (placeholder != ConfigurationPlaceholder.Type)
        return;
      if (this.PropertyChanged != null)
      {
        this.PropertyChanged((object) this, new PropertyChangedEventArgs("ButtonTypeIcon"));
        this.PropertyChanged((object) this, new PropertyChangedEventArgs("ButtonTypeIconMouseOver"));
      }
      this.SetTemplateForColumn();
    }

    private DataTemplate GetTemplateForType(SampleBasicType type, bool editingMode)
    {
      if (editingMode)
        return (DataTemplate) this.dialog.SampleDataGrid.Resources[(object) (type.Name + "CellEditingTemplate")];
      return (DataTemplate) this.dialog.SampleDataGrid.Resources[(object) (type.Name + "CellTemplate")];
    }

    private void SetTemplateForColumn()
    {
      this.CellTemplate = this.GetTemplateForType(this.sampleDataProperty.SampleTypeConfiguration.SampleType, false);
      this.CellEditingTemplate = this.GetTemplateForType(this.sampleDataProperty.SampleTypeConfiguration.SampleType, true);
    }

    private void OnConfigureColumnType()
    {
      SampleDataConfigurationPopup configurationPopup = new SampleDataConfigurationPopup(this);
      FrameworkElement frameworkElement = (FrameworkElement) this.Header;
      configurationPopup.PlacementTarget = (UIElement) frameworkElement;
      configurationPopup.VerticalOffset = frameworkElement.ActualHeight;
      configurationPopup.HorizontalOffset = frameworkElement.ActualWidth;
      configurationPopup.Placement = PlacementMode.Left;
      configurationPopup.IsOpen = true;
    }

    private FrameworkElement GenerateTextEditor(string value)
    {
      return (FrameworkElement) new TextBox()
      {
        Text = value
      };
    }

    private void BrowseForNewImage()
    {
      ExpressionOpenFileDialog expressionOpenFileDialog = new ExpressionOpenFileDialog();
      expressionOpenFileDialog.Title = StringTable.SampleDataConfigurationImageBrowserDialog;
      expressionOpenFileDialog.Filter = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}({1})|{1}|{2}|{3}|{4}|{5}|{6}({7})|{7}", (object) StringTable.AllImageFilesDescription, (object) "*.jpg;*.png", (object) StringTable.JpgImageDocumentTypeDescription, (object) "*.jpg", (object) StringTable.PngImageDocumentTypeDescription, (object) "*.png", (object) StringTable.SampleDataAllFilesDescription, (object) "*.*");
      bool? nullable = expressionOpenFileDialog.ShowDialog();
      if (!nullable.HasValue || !nullable.Value)
        return;
      this.editingRow.GetCell(this.sampleDataProperty).Value = (object) expressionOpenFileDialog.FileName;
    }

    private void SetAutomationID(FrameworkElement element, SampleDataRow row)
    {
      string str = (row.RowNumber + 1).ToString((IFormatProvider) NumberFormatInfo.InvariantInfo);
      AutomationElement.SetId((DependencyObject) element, "Cell_" + this.Name + "_" + str);
    }

    protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
    {
      SampleDataRow row = (SampleDataRow) dataItem;
      FrameworkElement frameworkElement = base.GenerateElement(cell, dataItem);
      this.SetAutomationID((FrameworkElement) cell, row);
      cell.DataContext = (object) row.GetCell(this.SampleDataProperty);
      cell.KeyDown += new KeyEventHandler(this.cell_KeyDown);
      return frameworkElement;
    }

    private void cell_KeyDown(object sender, KeyEventArgs e)
    {
      DataGridCell dataGridCell = sender as DataGridCell;
      FrameworkElement frameworkElement = dataGridCell.Content as FrameworkElement;
      bool flag1 = e.Key == Key.Escape;
      bool flag2 = flag1;
      bool flag3 = frameworkElement is TextBox || frameworkElement is ContentPresenter && ((ContentPresenter) frameworkElement).Content == null;
      SampleBasicType sampleType = ((SampleDataDialogColumn) dataGridCell.Column).SampleType;
      if (sampleType == SampleBasicType.Number || sampleType == SampleBasicType.String)
      {
        if (flag3)
        {
          if (flag1)
          {
            this.dialog.SampleDataGrid.CancelEdit();
            flag2 = false;
          }
        }
        else if (!flag1)
        {
          this.dialog.SampleDataGrid.BeginEdit();
          TextBox textBox = dataGridCell.Content as TextBox;
          if (textBox != null)
            textBox.Focus();
        }
      }
      if (!flag2)
        return;
      this.dialog.Close();
    }

    protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
    {
      this.editingRow = (SampleDataRow) dataItem;
      object obj = this.editingRow.GetCell(this.sampleDataProperty).Value;
      FrameworkElement element = (FrameworkElement) null;
      if (this.sampleDataProperty.SampleTypeConfiguration.SampleType == SampleBasicType.Boolean)
        element = this.GenerateElement(cell, dataItem);
      if (this.sampleDataProperty.SampleTypeConfiguration.SampleType == SampleBasicType.Image)
      {
        this.BrowseForNewImage();
        element = this.GenerateElement(cell, dataItem);
      }
      if (this.sampleDataProperty.SampleTypeConfiguration.SampleType == SampleBasicType.Number || this.sampleDataProperty.SampleTypeConfiguration.SampleType == SampleBasicType.String)
        element = this.GenerateTextEditor(obj.ToString());
      this.SetAutomationID(element, this.editingRow);
      return element;
    }

    protected override bool CommitCellEdit(FrameworkElement editingElement)
    {
      if (this.SampleDataProperty.SampleType == SampleBasicType.String || this.SampleDataProperty.SampleType == SampleBasicType.Number)
        this.editingRow.GetCell(this.SampleDataProperty).Value = (object) ((TextBox) editingElement).Text;
      return true;
    }
  }
}
