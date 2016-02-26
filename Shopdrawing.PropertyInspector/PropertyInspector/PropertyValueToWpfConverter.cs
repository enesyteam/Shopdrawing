// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.PropertyValueToWpfConverter
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class PropertyValueToWpfConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      PropertyValue propertyValue = values[0] as PropertyValue;
      object obj = values[1];
      if (propertyValue != null && obj != MixedProperty.Mixed)
      {
        SceneNodeProperty sceneNodeProperty = propertyValue.get_ParentProperty() as SceneNodeProperty;
        if (sceneNodeProperty != null)
        {
          if (obj is GradientBrush)
            obj = (object) ((GradientBrush) obj).Clone();
          obj = sceneNodeProperty.SceneNodeObjectSet.DesignerContext.PlatformConverter.ConvertToWpf(sceneNodeProperty.SceneNodeObjectSet.DocumentContext, obj);
        }
      }
      return obj;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
