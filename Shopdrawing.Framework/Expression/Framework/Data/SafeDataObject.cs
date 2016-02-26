// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Data.SafeDataObject
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Clipboard;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

namespace Microsoft.Expression.Framework.Data
{
  public class SafeDataObject
  {
    private IDataObject dataObject;

    public SafeDataObject(IDataObject dataObject)
    {
      this.dataObject = dataObject;
    }

    public static SafeDataObject FromClipboard()
    {
      try
      {
        return new SafeDataObject(ClipboardService.GetDataObject());
      }
      catch (ExternalException ex)
      {
      }
      return (SafeDataObject) null;
    }

    public bool GetDataPresent(Type format)
    {
      return this.dataObject.GetDataPresent(format);
    }

    public bool GetDataPresent(string format)
    {
      return this.dataObject.GetDataPresent(format);
    }

    public object GetData(Type format)
    {
      try
      {
        return this.dataObject.GetData(format);
      }
      catch (FileLoadException ex)
      {
      }
      catch (Exception ex)
      {
      }
      return (object) null;
    }

    public object GetData(string format)
    {
      try
      {
        return this.dataObject.GetData(format);
      }
      catch (Exception ex)
      {
      }
      return (object) null;
    }

    public string[] GetFormats()
    {
      try
      {
        return this.dataObject.GetFormats();
      }
      catch (Exception ex)
      {
      }
      return new string[0];
    }
  }
}
