// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI.VisibilityToMarginConverter
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Data;
using System;
using System.Globalization;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI
{
  public class VisibilityToMarginConverter : SingleMarginConverter
  {
    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException(ExceptionStringTable.ConvertBackNotImplementedOnVisibilityToMarginConverter);
    }

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      switch ((Visibility) value)
      {
        case Visibility.Hidden:
        case Visibility.Collapsed:
          switch (this.TargetSubProperty)
          {
            case MarginSubProperty.Left:
              return (object) new Thickness(0.0, this.Top, this.Right, this.Bottom);
            case MarginSubProperty.Top:
              return (object) new Thickness(this.Left, 0.0, this.Right, this.Bottom);
            case MarginSubProperty.Right:
              return (object) new Thickness(this.Left, this.Top, 0.0, this.Bottom);
            case MarginSubProperty.Bottom:
              return (object) new Thickness(this.Left, this.Top, this.Right, 0.0);
            default:
              throw new ArgumentException(ExceptionStringTable.InvalidTargetSubProperty);
          }
        default:
          return (object) new Thickness(this.Left, this.Top, this.Right, this.Bottom);
      }
    }
  }
}
