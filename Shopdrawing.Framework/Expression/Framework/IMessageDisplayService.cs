// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.IMessageDisplayService
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Controls;
using System;
using System.Windows;

namespace Microsoft.Expression.Framework
{
  public interface IMessageDisplayService
  {
    MessageBoxResult ShowMessage(string message);

    MessageBoxResult ShowMessage(string message, string caption);

    MessageBoxResult ShowMessage(string message, string caption, MessageBoxButton button);

    MessageBoxResult ShowMessage(string message, string caption, MessageBoxButton button, MessageBoxImage image);

    MessageBoxResult ShowMessage(string message, string caption, MessageBoxButton button, MessageBoxImage image, MessageChoice defaultChoice);

    MessageBoxResult ShowMessage(Window owner, string message, string caption, MessageBoxButton button, MessageBoxImage image);

    MessageBoxResult ShowMessage(Window owner, string message, string caption, MessageBoxButton button, MessageBoxImage image, MessageChoice defaultChoice);

    MessageBoxResult ShowMessage(MessageBoxArgs args);

    MessageBoxResult ShowMessage(MessageBoxArgs args, out bool doNotAskAgain);

    void ShowError(string message);

    void ShowError(string message, string caption);

    void ShowError(string message, Exception exception, string caption);

    void ShowError(ErrorArgs args);
  }
}
