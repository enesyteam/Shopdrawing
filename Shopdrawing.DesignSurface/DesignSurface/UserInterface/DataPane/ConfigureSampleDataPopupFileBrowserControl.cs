// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.ConfigureSampleDataPopupFileBrowserControl
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class ConfigureSampleDataPopupFileBrowserControl : TextBox
  {
    public static readonly DependencyProperty CommitCommandProperty = DependencyProperty.Register("CommitCommand", typeof (ICommand), typeof (ConfigureSampleDataPopupFileBrowserControl));

    public ICommand CommitCommand
    {
      get
      {
        return (ICommand) this.GetValue(ConfigureSampleDataPopupFileBrowserControl.CommitCommandProperty);
      }
      set
      {
        this.SetValue(ConfigureSampleDataPopupFileBrowserControl.CommitCommandProperty, (object) value);
      }
    }

    public ConfigureSampleDataPopupFileBrowserControl()
    {
      this.KeyDown += new KeyEventHandler(this.ConfigureSampleDataPopupFileBrowserControl_KeyDown);
    }

    private void ConfigureSampleDataPopupFileBrowserControl_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key != Key.Return || this.CommitCommand == null)
        return;
      this.CommitCommand.Execute((object) this.Text);
    }

    internal class SetNewFolderCommand : ICommand
    {
      private ConfigureSampleDataPopupFileBrowserControl.SetNewFolderCommand.StringDelegateHandler stringDelegate;

      public event EventHandler CanExecuteChanged
      {
        add
        {
        }
        remove
        {
        }
      }

      public SetNewFolderCommand(ConfigureSampleDataPopupFileBrowserControl.SetNewFolderCommand.StringDelegateHandler stringDelegate)
      {
        this.stringDelegate = stringDelegate;
      }

      public bool CanExecute(object parameter)
      {
        return true;
      }

      public void Execute(object parameter)
      {
        this.stringDelegate(parameter != null ? parameter.ToString() : string.Empty);
      }

      internal delegate void StringDelegateHandler(string path);
    }
  }
}
