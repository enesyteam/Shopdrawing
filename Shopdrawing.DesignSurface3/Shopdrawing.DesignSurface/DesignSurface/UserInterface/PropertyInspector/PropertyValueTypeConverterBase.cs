// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.PropertyValueTypeConverterBase
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public abstract class PropertyValueTypeConverterBase : IValueConverter
  {
    protected abstract object Convert(Type type);

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      PropertyValue propertyValue = (PropertyValue) value;
      try
      {
        if (propertyValue.Value != null)
          return this.Convert(propertyValue.Value.GetType());
      }
      catch (TargetInvocationException ex)
      {
        if (ex.InnerException != null)
          typeof (ArgumentOutOfRangeException).IsAssignableFrom(ex.InnerException.GetType());
      }
      return (object) string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
