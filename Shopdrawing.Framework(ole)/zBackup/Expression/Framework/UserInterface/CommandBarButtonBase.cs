// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.CommandBarButtonBase
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Expression.Framework.UserInterface
{
  internal abstract class CommandBarButtonBase : MenuItem
  {
    protected string command;
    protected bool displayShortcut;
    protected ICommandService commandService;

    public string CommandName
    {
      get
      {
        return this.command;
      }
    }

    public bool DisplayShortcut
    {
      get
      {
        return this.displayShortcut;
      }
      set
      {
        this.displayShortcut = value;
        this.UpdateText();
      }
    }

    public CommandBarButtonBase(ICommandService commandService, string command)
    {
      this.Focusable = true;
      this.displayShortcut = true;
      this.commandService = commandService;
      this.command = command;
    }

    public virtual void Update()
    {
      this.UpdateVisible();
      this.UpdateEnabled();
      this.UpdateText();
    }

    protected void UpdateVisible()
    {
      bool flag = true;
      object commandProperty = this.commandService.GetCommandProperty(this.command, "IsVisible");
      if (commandProperty != null)
        flag = (bool) commandProperty;
      Visibility visibility = flag ? Visibility.Visible : Visibility.Collapsed;
      if (visibility == this.Visibility)
        return;
      this.Visibility = visibility;
    }

    protected void UpdateEnabled()
    {
      bool flag = false;
      object commandProperty = this.commandService.GetCommandProperty(this.command, "IsEnabled");
      if (commandProperty != null)
        flag = (bool) commandProperty;
      this.IsEnabled = flag;
    }

    protected void UpdateChecked()
    {
      object commandProperty = this.commandService.GetCommandProperty(this.command, "IsChecked");
      if (commandProperty == null)
        return;
      this.IsChecked = (bool) commandProperty;
    }

    protected void UpdateText()
    {
      string str1 = (string) this.Header;
      object commandProperty = this.commandService.GetCommandProperty(this.command, "Text");
      if (commandProperty != null)
        str1 = (string) commandProperty;
      this.Header = (object) str1;
      string str2 = string.Empty;
      if (this.displayShortcut)
      {
        KeyBinding[] keyBindingArray = this.commandService.GetCommandProperty(this.command, "Shortcuts") as KeyBinding[];
        if (keyBindingArray != null)
          str2 = CultureManager.GetShortcutText(keyBindingArray);
      }
      this.InputGestureText = str2;
    }
  }
}
