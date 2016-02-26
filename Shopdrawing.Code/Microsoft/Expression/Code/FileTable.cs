// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.FileTable
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Framework;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.Code
{
  internal static class FileTable
  {
    private static FileResourceManager resourceManager = new FileResourceManager(typeof (FileTable).Assembly);

    public static FrameworkElement GetElement(string name)
    {
      return FileTable.resourceManager.GetElement(name);
    }

    public static byte[] GetByteArray(string name)
    {
      return FileTable.resourceManager.GetByteArray(name);
    }

    public static ImageSource GetImageSource(string name)
    {
      return FileTable.resourceManager.GetImageSource(name);
    }
  }
}
