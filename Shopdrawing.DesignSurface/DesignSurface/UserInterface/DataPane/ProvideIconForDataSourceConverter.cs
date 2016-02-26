// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.ProvideIconForDataSourceConverter
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class ProvideIconForDataSourceConverter : IValueConverter
  {
    private ImageSource liveDataSourceImage = FileTable.GetImageSource("Resources\\Icons\\Data\\LiveIcon_20x16_Off.PNG");
    private ImageSource sampleDataSourceImage = FileTable.GetImageSource("Resources\\Icons\\Data\\SampleIcon_20x16_Off.PNG");
    private ImageSource designDataSourceImage = FileTable.GetImageSource("Resources\\Icons\\Data\\DesignDataIcon_Off_20x16.PNG");

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is FileBasedDataSourceItem)
        return (object) this.designDataSourceImage;
      DataSourceItem dataSourceItem = value as DataSourceItem;
      if (dataSourceItem != null)
      {
        DataSourceNode dataSourceNode = dataSourceItem.DataSourceNode;
        if (dataSourceNode is ClrObjectDataSourceNode)
        {
          if (!dataSourceNode.IsSampleDataSource)
            return (object) this.liveDataSourceImage;
          return (object) this.sampleDataSourceImage;
        }
        if (dataSourceNode is XmlDataSourceNode)
          return (object) this.liveDataSourceImage;
      }
      return (object) null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }
}
