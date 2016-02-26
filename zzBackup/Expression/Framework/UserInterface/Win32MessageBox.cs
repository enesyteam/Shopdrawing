// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.Win32MessageBox
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Windows;

namespace Microsoft.Expression.Framework.UserInterface
{
  internal class Win32MessageBox
  {
    public static MessageBoxResult Show(string message, string caption, MessageBoxButton button, MessageBoxImage image)
    {
      uint uType = 65536U;
      switch (button)
      {
        case MessageBoxButton.OK:
          uType = uType;
          break;
        case MessageBoxButton.OKCancel:
          uType |= 1U;
          break;
        case MessageBoxButton.YesNoCancel:
          uType |= 3U;
          break;
        case MessageBoxButton.YesNo:
          uType |= 4U;
          break;
      }
      switch (image)
      {
        case MessageBoxImage.Exclamation:
          uType |= 48U;
          break;
        case MessageBoxImage.Asterisk:
          uType |= 64U;
          break;
        case MessageBoxImage.Hand:
          uType |= 16U;
          break;
        case MessageBoxImage.Question:
          uType |= 32U;
          break;
      }
      MessageBoxResult messageBoxResult;
      switch (Microsoft.Expression.Framework.UnsafeNativeMethods.MessageBox(IntPtr.Zero, message, caption, uType))
      {
        case 1:
          messageBoxResult = MessageBoxResult.OK;
          break;
        case 2:
          messageBoxResult = MessageBoxResult.Cancel;
          break;
        case 6:
          messageBoxResult = MessageBoxResult.Yes;
          break;
        default:
          messageBoxResult = MessageBoxResult.No;
          break;
      }
      return messageBoxResult;
    }
  }
}
