// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.ExampleAssetInfo
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Windows.Design;
using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  [Serializable]
  internal class ExampleAssetInfo
  {
    public int Index { get; private set; }

    public string DisplayName { get; private set; }

    public string Description { get; private set; }

    public bool IsBrowable { get; private set; }

    public CustomAssetCategoryPath[] Categories { get; private set; }

    public byte[] SmallIcon { get; private set; }

    public byte[] LargeIcon { get; private set; }

    public static ExampleAssetInfo FromToolboxExample(IToolboxExample example, int index)
    {
      return new ExampleAssetInfo()
      {
        Index = index,
        DisplayName = example.DisplayName,
        Description = AssetInfoModel.GetDescription(example.GetType()),
        Categories = Enumerable.ToArray<CustomAssetCategoryPath>(AssetTypeHelper.GetCustomAssetCategoryPaths(example.GetType())),
        IsBrowable = AssetTypeHelper.IsTypeBrowsable(example.GetType()),
        SmallIcon = ExampleAssetInfo.CopyStream(example.GetImageStream(new Size(12.0, 12.0))),
        LargeIcon = ExampleAssetInfo.CopyStream(example.GetImageStream(new Size(24.0, 24.0)))
      };
    }

    private static byte[] CopyStream(Stream stream)
    {
      if (stream == null)
        return (byte[]) null;
      try
      {
        byte[] buffer = new byte[stream.Length];
        stream.Read(buffer, 0, (int) stream.Length);
        return buffer;
      }
      catch
      {
        return (byte[]) null;
      }
    }
  }
}
