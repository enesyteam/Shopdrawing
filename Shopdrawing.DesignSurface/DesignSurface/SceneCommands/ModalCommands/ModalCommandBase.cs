// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.ModalCommands.ModalCommandBase
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.SceneCommands;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.SceneCommands.ModalCommands
{
  internal abstract class ModalCommandBase : SceneCommandBase, ICommand
  {
    public abstract string Description { get; }

    protected override ViewState RequiredSelectionViewState
    {
      get
      {
        return ViewState.None;
      }
    }

    event EventHandler ICommand.CanExecuteChanged
    {
      add
      {
      }
      remove
      {
      }
    }

    protected ModalCommandBase(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    bool ICommand.CanExecute(object parameter)
    {
      return this.IsEnabled;
    }

    void ICommand.Execute(object parameter)
    {
      if (!this.IsEnabled)
        return;
      this.Execute();
    }
  }
}
