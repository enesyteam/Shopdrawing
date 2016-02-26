// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourcePane.IconConverter
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.ResourcePane
{
  public class IconConverter : IValueConverter
  {
    private static ImageSource documentIcon = FileTable.GetImageSource("Resources\\Icons\\Resources\\resource_dictionary_on_12x12.png");
    private static ImageSource linkedDocumentIcon = FileTable.GetImageSource("Resources\\Icons\\Resources\\resource_externalDictionary_on_12x12.png");

    public static ImageSource DocumentIcon
    {
      get
      {
        return IconConverter.documentIcon;
      }
    }

    public static ImageSource LinkedDocumentIcon
    {
      get
      {
        return IconConverter.linkedDocumentIcon;
      }
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      ITypeId type = value as ITypeId;
      if (type != null)
        return (object) IconMapper.GetDrawingBrushForType(type, false, 12, 12);
      return (object) null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new InvalidOperationException();
    }
  }
}
