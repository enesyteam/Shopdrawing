// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.ConfigureSampleDataDialog
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.ValueEditors;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public sealed class ConfigureSampleDataDialog : Dialog
  {
    private static readonly Size dialogSize = new Size(796.0, 543.0);
    private bool isInInitialFocusState = true;
    private DataGrid sampleDataGrid;
    private NumberEditor rowsSlider;
    private Button acceptButton;

    public SampleDataEditorModel Model { get; private set; }

    public DataGrid SampleDataGrid
    {
      get
      {
        return this.sampleDataGrid;
      }
    }

    public IList<SampleDataDialogColumn> Columns { get; private set; }

    public ConfigureSampleDataDialog(DataSchemaNodePath schemaPath, IMessageDisplayService messageService)
    {
      this.Title = StringTable.SampleDataConfigurationDialogTitle;
      this.MinWidth = ConfigureSampleDataDialog.dialogSize.Width;
      this.MinHeight = ConfigureSampleDataDialog.dialogSize.Height;
      this.Height = ConfigureSampleDataDialog.dialogSize.Height;
      this.Width = ConfigureSampleDataDialog.dialogSize.Width;
      this.ResizeMode = ResizeMode.CanResize;
      this.Model = new SampleDataEditorModel(schemaPath, messageService);
      FrameworkElement element = Microsoft.Expression.DesignSurface.FileTable.GetElement("Resources\\DataPane\\ConfigureSampleDataDialog.xaml");
      this.DialogContent = (UIElement) element;
      this.sampleDataGrid = (DataGrid) LogicalTreeHelper.FindLogicalNode((DependencyObject) element, "SampleDataGrid");
      this.rowsSlider = (NumberEditor) LogicalTreeHelper.FindLogicalNode((DependencyObject) element, "RowsSlider");
      this.acceptButton = (Button) LogicalTreeHelper.FindLogicalNode((DependencyObject) element, "AcceptButton");
      this.rowsSlider.KeyDown += new KeyEventHandler(this.HandleEnterPressOnRowsSlider);
      this.sampleDataGrid.GotFocus += new RoutedEventHandler(this.sampleDataGrid_GotFocus);
      if (this.sampleDataGrid != null)
      {
        this.Columns = (IList<SampleDataDialogColumn>) new List<SampleDataDialogColumn>();
        foreach (SampleDataProperty property in (IEnumerable<SampleDataProperty>) this.Model.SampleDataProperties)
        {
          SampleDataDialogColumn column = new SampleDataDialogColumn(property, this);
          this.Columns.Add(column);
          this.StyleColumnHeader(column);
          this.sampleDataGrid.Columns.Add((DataGridColumn) column);
        }
      }
      element.DataContext = (object) this.Model;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      if ((!this.DialogResult.HasValue || !this.DialogResult.Value) && this.Model.IsModified)
      {
        MessageWindow messageWindow = new MessageWindow((Window) this, MessageIcon.Warning, StringTable.SampleDataConfigurationCloseConfirmTitle, StringTable.SampleDataConfigurationCloseConfirmMessage, MessageChoice.Yes | MessageChoice.No);
        messageWindow.ShowDialog();
        if ((messageWindow.Result & MessageChoice.No) > MessageChoice.None)
          e.Cancel = true;
      }
      base.OnClosing(e);
    }

    private void HandleEnterPressOnRowsSlider(object sender, KeyEventArgs e)
    {
      if (e.Key != Key.Return)
        return;
      e.Handled = true;
      this.acceptButton.Focus();
    }

    private void StyleColumnHeader(SampleDataDialogColumn column)
    {
      FrameworkElement frameworkElement = (FrameworkElement) this.sampleDataGrid.Resources[(object) "ColumnHeaderPrototype"];
      AutomationElement.SetId((DependencyObject) frameworkElement, "ColumnHeader_" + column.Name);
      frameworkElement.DataContext = (object) column;
      column.Header = (object) frameworkElement;
      column.Width = (DataGridLength) 150.0;
    }

    private void sampleDataGrid_GotFocus(object sender, RoutedEventArgs e)
    {
      if (!this.isInInitialFocusState)
        return;
      this.acceptButton.Focus();
      this.isInInitialFocusState = false;
    }
  }
}
