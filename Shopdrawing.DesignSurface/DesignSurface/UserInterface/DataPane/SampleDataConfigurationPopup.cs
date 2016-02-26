// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.SampleDataConfigurationPopup
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.SampleData;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.UserInterface;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class SampleDataConfigurationPopup : DataConfigurationPopup
  {
    public SampleDataConfigurationPopup(SampleDataDialogColumn column)
      : base((IPopupControlCallback) new SampleDataConfigurationPopup.ValueEditorDialogCallback(column), ConfigurationPlaceholder.Type)
    {
    }

    public SampleDataConfigurationPopup(DataSchemaItem dataSchemaItem)
      : base((IPopupControlCallback) new SampleDataConfigurationPopup.DataPaneCallback(dataSchemaItem), ConfigurationPlaceholder.Type)
    {
    }

    internal override void MouseCaptureWorkaround(UIElement element)
    {
      if (!(this.PopupCallback is SampleDataConfigurationPopup.ValueEditorDialogCallback))
        return;
      element.ReleaseMouseCapture();
      if (element is ComboBox || element is TextBlock)
        return;
      int childrenCount = VisualTreeHelper.GetChildrenCount((DependencyObject) element);
      for (int childIndex = 0; childIndex < childrenCount; ++childIndex)
      {
        UIElement element1 = VisualTreeHelper.GetChild((DependencyObject) element, childIndex) as UIElement;
        if (element1 != null)
          this.MouseCaptureWorkaround(element1);
      }
    }

    private class ValueEditorDialogCallback : IPopupControlCallback
    {
      private SampleDataDialogColumn column;

      public ValueEditorDialogCallback(SampleDataDialogColumn column)
      {
        this.column = column;
      }

      public object GetValue(ConfigurationPlaceholder control)
      {
        return this.column.SampleDataProperty.SampleTypeConfiguration.GetSelectedValue(control);
      }

      public void SetValue(ConfigurationPlaceholder control, object value)
      {
        this.column.SetConfigurationValue(control, value);
      }
    }

    private class DataPaneCallback : IPopupControlCallback
    {
      private SampleProperty property;
      private SampleDataPropertyConfiguration configuration;
      private IMessageDisplayService messageService;

      public DataPaneCallback(DataSchemaItem dataSchemaItem)
      {
        DataSchemaNode node = dataSchemaItem.DataSchemaNodePath.Node;
        this.property = ((SampleCompositeType) node.Parent.SampleType).GetSampleProperty(node.PathName);
        this.configuration = new SampleDataPropertyConfiguration(this.property);
        this.messageService = dataSchemaItem.ViewModel.DesignerContext.MessageDisplayService;
      }

      public object GetValue(ConfigurationPlaceholder control)
      {
        return this.configuration.GetSelectedValue(control);
      }

      public void SetValue(ConfigurationPlaceholder control, object value)
      {
        this.configuration.SetConfigurationValue(control, value);
        using (TemporaryCursor.SetWaitCursor())
        {
          this.property.ChangeTypeAndFormat((SampleType) this.configuration.SampleType, this.configuration.Format, this.configuration.FormatParameters);
          if (this.property.PropertySampleType == SampleBasicType.Image)
            this.property.DeclaringDataSet.EnsureSampleImages();
          this.property.DeclaringDataSet.CommitChanges(this.messageService);
        }
      }
    }
  }
}
