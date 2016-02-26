// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.BindingFilterModeToStringConverter
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class BindingFilterModeToStringConverter : IValueConverter
  {
    public object ConvertBack(object o, Type targetType, object parameter, CultureInfo culture)
    {
      return (object) null;
    }

    public object Convert(object o, Type targetType, object parameter, CultureInfo culture)
    {
      BindingFilterMode bindingFilterMode = BindingFilterMode.None;
      if (o is BindingFilterMode)
        bindingFilterMode = (BindingFilterMode) o;
      switch (bindingFilterMode)
      {
        case BindingFilterMode.None:
          return (object) StringTable.BindingFilterModeNone;
        case BindingFilterMode.FilterByType:
          return (object) StringTable.BindingFilterModeFilterByType;
        default:
          return (object) StringTable.BindingFilterModeNone;
      }
    }
  }
}
