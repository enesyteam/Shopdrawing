// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Clipboard.ClipboardService
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Windows;

namespace Microsoft.Expression.Framework.Clipboard
{
  public static class ClipboardService
  {
    private static IClipboard clipboard;

    public static IClipboard Clipboard
    {
      get
      {
        return ClipboardService.clipboard;
      }
      set
      {
        ClipboardService.clipboard = value;
      }
    }

    static ClipboardService()
    {
      ClipboardService.ResetClipboard();
    }

    public static void ResetClipboard()
    {
      ClipboardService.Clipboard = (IClipboard) new ClipboardService.OleClipboardService();
    }

    public static void SetDataObject(object data)
    {
      ClipboardService.clipboard.SetDataObject(data);
    }

    public static void SetDataObject(object data, bool copy)
    {
      ClipboardService.clipboard.SetDataObject(data, copy);
    }

    public static bool IsCurrent(IDataObject data)
    {
      return ClipboardService.clipboard.IsCurrent(data);
    }

    public static IDataObject GetDataObject()
    {
      return ClipboardService.clipboard.GetDataObject();
    }

    private class OleClipboardService : IClipboard
    {
      public void SetDataObject(object data)
      {
        System.Windows.Clipboard.SetDataObject(data);
      }

      public void SetDataObject(object data, bool copy)
      {
        System.Windows.Clipboard.SetDataObject(data, copy);
      }

      public bool IsCurrent(IDataObject data)
      {
        return System.Windows.Clipboard.IsCurrent(data);
      }

      public IDataObject GetDataObject()
      {
        return System.Windows.Clipboard.GetDataObject();
      }
    }
  }
}
