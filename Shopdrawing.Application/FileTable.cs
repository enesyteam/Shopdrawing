// Decompiled with JetBrains decompiler
// Type: Shopdrawing.App.FileTable
// Assembly: Shopdrawing.App, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: DDD2F1CF-BB6D-4068-A4D9-DDBDD16D6739
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Shopdrawing.App.dll

using Microsoft.Expression.Framework;
using System.Windows;

namespace Shopdrawing.App
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

    public static ResourceDictionary GetResourceDictionary(string name)
    {
      return FileTable.resourceManager.GetResourceDictionary(name);
    }
  }
}
