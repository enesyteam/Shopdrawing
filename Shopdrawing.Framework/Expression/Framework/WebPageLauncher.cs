// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.WebPageLauncher
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Expression.Framework
{
  public static class WebPageLauncher
  {
    public static bool Navigate(Uri uri, IMessageDisplayService messageDisplayService)
    {
      try
      {
        Process.Start(uri.OriginalString);
      }
      catch (Exception ex)
      {
        if (ex is ObjectDisposedException || ex is Win32Exception || (ex is ArgumentException || ex is InvalidOperationException))
        {
          if (messageDisplayService != null)
            messageDisplayService.ShowError(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.WebNavigationFailedMessage, new object[1]
            {
              (object) uri.OriginalString
            }));
          return false;
        }
        throw;
      }
      return true;
    }
  }
}
