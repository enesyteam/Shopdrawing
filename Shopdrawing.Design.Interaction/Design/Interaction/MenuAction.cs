// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.MenuAction
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;
using System.Collections.Specialized;
using System.Windows.Input;

namespace Microsoft.Windows.Design.Interaction
{
  public class MenuAction : MenuBase
  {
    private static int stateIsCheckable = BitVector32.CreateMask();
    private static int stateIsChecked = BitVector32.CreateMask(MenuAction.stateIsCheckable);
    private static int stateIsEnabled = BitVector32.CreateMask(MenuAction.stateIsChecked);
    private static int stateIsVisible = BitVector32.CreateMask(MenuAction.stateIsEnabled);
    private BitVector32 _state = new BitVector32();
    private Uri _imageUri;
    private MenuAction.MenuActionCommand _command;

    public ICommand Command
    {
      get
      {
        if (this._command == null)
          this._command = new MenuAction.MenuActionCommand(this);
        return (ICommand) this._command;
      }
    }

    public bool Checkable
    {
      get
      {
        return this._state[MenuAction.stateIsCheckable];
      }
      set
      {
        if (this._state[MenuAction.stateIsCheckable] == value)
          return;
        this._state[MenuAction.stateIsCheckable] = value;
        this.OnPropertyChanged("Checkable");
      }
    }

    public bool Checked
    {
      get
      {
        return this._state[MenuAction.stateIsChecked];
      }
      set
      {
        if (this._state[MenuAction.stateIsChecked] == value)
          return;
        this._state[MenuAction.stateIsChecked] = value;
        this.OnPropertyChanged("Checked");
      }
    }

    public bool Enabled
    {
      get
      {
        return this._state[MenuAction.stateIsEnabled];
      }
      set
      {
        if (this._state[MenuAction.stateIsEnabled] == value)
          return;
        this._state[MenuAction.stateIsEnabled] = value;
        this.OnPropertyChanged("Enabled");
        if (this._command == null)
          return;
        this._command.RaiseCanExecuteChanged();
      }
    }

    public bool Visible
    {
      get
      {
        return this._state[MenuAction.stateIsVisible];
      }
      set
      {
        if (this._state[MenuAction.stateIsVisible] == value)
          return;
        this._state[MenuAction.stateIsVisible] = value;
        this.OnPropertyChanged("Visible");
        if (this._command == null)
          return;
        this._command.RaiseCanExecuteChanged();
      }
    }

    public Uri ImageUri
    {
      get
      {
        return this._imageUri;
      }
      set
      {
        if (!(this._imageUri != value))
          return;
        this._imageUri = value;
        this.OnPropertyChanged("ImageUri");
      }
    }

    public event EventHandler<MenuActionEventArgs> Execute;

    public MenuAction(string displayName)
    {
      this.DisplayName = displayName;
      this.Enabled = true;
      this.Visible = true;
    }

    private class MenuActionCommand : ICommand
    {
      private MenuAction _owner;

      public event EventHandler CanExecuteChanged;

      public MenuActionCommand(MenuAction owner)
      {
        this._owner = owner;
      }

      internal void RaiseCanExecuteChanged()
      {
        if (this.CanExecuteChanged == null)
          return;
        this.CanExecuteChanged((object) this._owner, EventArgs.Empty);
      }

      bool ICommand.CanExecute(object parameter)
      {
        if (this._owner.Enabled)
          return this._owner.Visible;
        return false;
      }

      public void Execute(object parameter)
      {
        if (this._owner.Execute == null)
          return;
        this._owner.Execute((object) this._owner, new MenuActionEventArgs(this._owner.Context));
      }
    }
  }
}
