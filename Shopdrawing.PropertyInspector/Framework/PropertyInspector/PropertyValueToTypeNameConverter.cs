// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.PropertyInspector.PropertyValueToTypeNameConverter
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.Expression.Framework.PropertyInspector
{
  public class PropertyValueToTypeNameConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (typeof (string).IsAssignableFrom(targetType))
      {
        PropertyValue propertyValue = value as PropertyValue;
        if (propertyValue != null)
        {
          if (propertyValue.get_Value() != null)
            return (object) (propertyValue.get_ParentProperty().get_DisplayName() + " " + propertyValue.get_Value().GetType().Name);
          return (object) (propertyValue.get_ParentProperty().get_DisplayName() + (object) " " + (string) (object) propertyValue.get_ParentProperty().get_PropertyType());
        }
      }
      return (object) string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException(ExceptionStringTable.MethodOrOperationIsNotImplemented);
    }
  }
}
