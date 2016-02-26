// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.ConfigureDataStoreValueTemplateSelector
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class ConfigureDataStoreValueTemplateSelector : DataTemplateSelector
  {
    public DataTemplate TextEditingTemplate { get; set; }

    public DataTemplate BoolEditingTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      DataStorePropertyEntry storePropertyEntry = item as DataStorePropertyEntry;
      if (storePropertyEntry == null)
        return (DataTemplate) null;
      if (storePropertyEntry.PropertyType != DataStoreType.Boolean)
        return this.TextEditingTemplate;
      return this.BoolEditingTemplate;
    }
  }
}
