// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DataConfigurationButton
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public abstract class DataConfigurationButton : Button
  {
    private static DataConfigurationPopup openedPopup;
    private static string openedPopupUniqueId;
    private DataSchemaItem dataSchemaItem;

    public DataSchemaItem DataSchemaItem
    {
      get
      {
        if (this.dataSchemaItem == null && this.Parent != null)
          this.dataSchemaItem = ((FrameworkElement) this.Parent).DataContext as DataSchemaItem;
        return this.dataSchemaItem;
      }
    }

    public string UniqueId
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", new object[2]
        {
          (object) this.DataSchemaItem.DataSourceNode.UniqueId,
          (object) this.DataSchemaItem.UniqueId
        });
      }
    }

    protected DataConfigurationButton()
    {
      this.DataContext = (object) this;
      this.Loaded += new RoutedEventHandler(this.ButtonLoaded);
    }

    public abstract DataConfigurationPopup CreatePopup(DataSchemaItem dataSchemaItem);

    private void DataConfigurationButton_Click(object sender, RoutedEventArgs e)
    {
      DataConfigurationPopup popup = this.CreatePopup(this.DataSchemaItem);
      popup.PlacementTarget = (UIElement) this;
      popup.VerticalOffset = this.ActualHeight;
      popup.HorizontalOffset = this.ActualWidth - 2.0;
      popup.Placement = PlacementMode.Left;
      popup.IsOpen = true;
      this.RememberPopup(popup);
    }

    private void ButtonLoaded(object sender, RoutedEventArgs e)
    {
      this.Loaded -= new RoutedEventHandler(this.ButtonLoaded);
      if (this.DataSchemaItem == null)
        return;
      this.Click += new RoutedEventHandler(this.DataConfigurationButton_Click);
      this.RestorePreviousPopup();
    }

    private void RememberPopup(DataConfigurationPopup popup)
    {
      DataConfigurationButton.openedPopup = popup;
      DataConfigurationButton.openedPopupUniqueId = this.UniqueId;
    }

    private void RestorePreviousPopup()
    {
      if (DataConfigurationButton.openedPopup == null || !DataConfigurationButton.openedPopup.IsPerformingSchemaChange || !(DataConfigurationButton.openedPopupUniqueId == this.UniqueId))
        return;
      if (this.IsEnabled)
      {
        DataConfigurationButton.openedPopup.PlacementTarget = (UIElement) this;
        DataConfigurationButton.openedPopup.IsOpen = true;
        DataConfigurationButton.openedPopup.IsPerformingSchemaChange = false;
      }
      else
        DataConfigurationButton.openedPopup = (DataConfigurationPopup) null;
    }
  }
}
