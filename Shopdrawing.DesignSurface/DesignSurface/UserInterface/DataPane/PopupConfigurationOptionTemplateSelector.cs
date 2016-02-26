// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.PopupConfigurationOptionTemplateSelector
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class PopupConfigurationOptionTemplateSelector : DataTemplateSelector
  {
    public DataTemplate ComboBoxTemplate { get; set; }

    public DataTemplate NumberSliderTemplate { get; set; }

    public DataTemplate FolderBrowserTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      IConfigurationOptionData configurationOptionData = item as IConfigurationOptionData;
      if (configurationOptionData is ComboBoxData)
        return this.ComboBoxTemplate;
      if (configurationOptionData is NumberSliderData)
        return this.NumberSliderTemplate;
      if (configurationOptionData is FileBrowserData)
        return this.FolderBrowserTemplate;
      return (DataTemplate) null;
    }
  }
}
