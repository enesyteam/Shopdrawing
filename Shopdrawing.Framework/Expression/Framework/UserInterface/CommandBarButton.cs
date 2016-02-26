// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.CommandBarButton
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Commands;
using System;
using System.Windows;
using System.Windows.Threading;

namespace Microsoft.Expression.Framework.UserInterface
{
  internal class CommandBarButton : CommandBarButtonBase, ICommandBarButton, ICommandBarItem
  {
    private CommandInvocationSource invocationSource;

    public string Text
    {
      get
      {
        return (string) this.Header;
      }
      set
      {
        this.Header = (object) value;
      }
    }

    public CommandBarButton(ICommandService commandService, string command, CommandInvocationSource invocationSource)
      : base(commandService, command)
    {
      this.invocationSource = invocationSource;
      this.Click += new RoutedEventHandler(this.Me_Click);
      this.UpdateText();
      this.UpdateEnabled();
    }

    private void Me_Click(object sender, RoutedEventArgs e)
    {
      UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.Send, (Action) (() => this.commandService.Execute(this.command, this.invocationSource)));
    }
  }
}
