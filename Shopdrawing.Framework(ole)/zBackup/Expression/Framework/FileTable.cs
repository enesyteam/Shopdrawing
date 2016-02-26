// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.FileTable
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.Framework
{
  internal static class FileTable
  {
    private static FileResourceManager resourceManager = new FileResourceManager(typeof (FileTable).Assembly);

    public static FrameworkElement GetElement(string name)
    {
      return FileTable.resourceManager.GetElement(name);
    }

    public static Style GetStyle(string name)
    {
      return FileTable.resourceManager.GetStyle(name);
    }

    public static ResourceDictionary GetResourceDictionary(string name)
    {
      return FileTable.resourceManager.GetResourceDictionary(name);
    }

    public static byte[] GetByteArray(string name)
    {
      return FileTable.resourceManager.GetByteArray(name);
    }

    public static Cursor GetCursor(string name)
    {
      return FileTable.resourceManager.GetCursor(name);
    }

    public static ImageSource GetImageSource(string name)
    {
      return FileTable.resourceManager.GetImageSource(name);
    }
  }
}
