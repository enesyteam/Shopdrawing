// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DataSchemaItemCategoryIconConverter
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.SampleData;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class DataSchemaItemCategoryIconConverter : IMultiValueConverter
  {
    private ImageSource compositeImage = FileTable.GetImageSource("Resources\\Icons\\Data\\complex_property_16x16.png");
    private ImageSource detailsCollectionImage = FileTable.GetImageSource("Resources\\Icons\\Data\\details_mode_toggle_off_16x16.png");
    private ImageSource masterCollectionImage = FileTable.GetImageSource("Resources\\Icons\\Data\\master_mode_toggle_off_16x16.png");
    private ImageSource hierarchicalCollectionImage = FileTable.GetImageSource("Resources\\Icons\\Data\\data_tree_16x16.png");

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      DataSchemaItem dataSchemaItem = values[0] as DataSchemaItem;
      bool flag = values.Length == 2 && (bool) values[1];
      if (dataSchemaItem != null)
      {
        if (dataSchemaItem.IsHierarchicalCollection)
          return (object) this.hierarchicalCollectionImage;
        if (dataSchemaItem.DataSchemaNode.IsCollection)
        {
          if (!flag)
            return (object) this.detailsCollectionImage;
          return (object) this.masterCollectionImage;
        }
        if (dataSchemaItem.HasChildren)
          return (object) this.compositeImage;
        if (SampleDataSet.SampleDataTypeFromType(dataSchemaItem.DataSchemaNode.Type) is SampleCompositeType)
          return (object) this.compositeImage;
      }
      return (object) null;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }
}
