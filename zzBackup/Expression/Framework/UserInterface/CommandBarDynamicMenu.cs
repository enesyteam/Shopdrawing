// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.CommandBarDynamicMenu
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Diagnostics;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.Framework.UserInterface
{
  internal sealed class CommandBarDynamicMenu : CommandBarButtonBase, ICommandBarDynamicMenu, ICommandBarItem
  {
    private MenuItem emptyItem;

    public string Text
    {
      get
      {
        return this.Header as string;
      }
      set
      {
        this.Header = (object) value;
      }
    }

    public CommandBarDynamicMenu(ICommandService commandService, string command)
      : base(commandService, command)
    {
      this.emptyItem = new MenuItem();
      this.emptyItem.Header = (object) (string) this.commandService.GetCommandProperty(this.command, "EmptyMenuItemText");
      this.emptyItem.IsEnabled = false;
      this.Items.Add((object) this.emptyItem);
      this.UpdateText();
      this.UpdateEnabled();
    }

    protected override void OnSubmenuOpened(RoutedEventArgs e)
    {
      base.OnSubmenuOpened(e);
      int count = this.Items.Count;
      PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.OpenCommandBarDynamicMenu);
      IEnumerable enumerable = (IEnumerable) this.commandService.GetCommandProperty(this.command, "Items");
      bool flag = false;
      foreach (object newItem in enumerable)
      {
        this.Items.Add(newItem);
        flag = true;
      }
      if (!flag)
        return;
      for (int index = 0; index < count; ++index)
        this.Items.RemoveAt(0);
    }

    protected override void OnSubmenuClosed(RoutedEventArgs e)
    {
      this.Items.Clear();
      this.Items.Add((object) this.emptyItem);
      base.OnSubmenuClosed(e);
    }
  }
}
