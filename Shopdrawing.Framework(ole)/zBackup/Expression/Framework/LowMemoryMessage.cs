// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.LowMemoryMessage
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.UserInterface;
using System;
using System.Globalization;
using System.Threading;
using System.Windows;

namespace Microsoft.Expression.Framework
{
  public static class LowMemoryMessage
  {
    private static string messageTitle = StringTable.DefaultMessageBoxCaption;
    private static string errorMessage = StringTable.MemoryLowErrorMessage;
    private static int shown;

    internal static void SetApplicationInformation(IExpressionInformationService expressionInformationService)
    {
      LowMemoryMessage.messageTitle = expressionInformationService.DefaultDialogTitle;
      LowMemoryMessage.errorMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.MemoryLowErrorMessage, new object[1]
      {
        (object) expressionInformationService.ShortApplicationName
      });
    }

    public static void Show()
    {
      if (Interlocked.Exchange(ref LowMemoryMessage.shown, 1) != 0)
        return;
      int num = (int) Win32MessageBox.Show(LowMemoryMessage.errorMessage, LowMemoryMessage.messageTitle, MessageBoxButton.OK, MessageBoxImage.Hand);
    }
  }
}
