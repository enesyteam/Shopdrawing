// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.SelectableItem`1
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Data;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public abstract class SelectableItem<T> : VirtualizingTreeItem<T>, ISelectable, INotifyPropertyChanged where T : SelectableItem<T>
  {
    private Microsoft.Expression.DesignSurface.UserInterface.DataPane.SelectionContext<T> selectionContext;

    protected Microsoft.Expression.DesignSurface.UserInterface.DataPane.SelectionContext<T> SelectionContext
    {
      get
      {
        return this.selectionContext;
      }
    }

    public ICommand SelectCommand
    {
      get
      {
        return (ICommand) new DelegateCommand((DelegateCommand.SimpleEventHandler) (() =>
        {
          if (!this.IsSelectable)
            return;
          this.selectionContext.SetSelection((T) this);
        }));
      }
    }

    public ICommand EnsureSelectedCommand
    {
      get
      {
        return (ICommand) new DelegateCommand((DelegateCommand.SimpleEventHandler) (() =>
        {
          if (!this.IsSelectable || this.IsSelected)
            return;
          this.selectionContext.SetSelection((T) this);
        }));
      }
    }

    public ICommand ToggleSelectCommand
    {
      get
      {
        return (ICommand) new DelegateCommand((DelegateCommand.SimpleEventHandler) (() =>
        {
          if (!this.IsSelectable)
            return;
          if (!this.IsSelected)
            this.selectionContext.Add((T) this);
          else
            this.selectionContext.Remove((T) this);
        }));
      }
    }

    protected SelectableItem(Microsoft.Expression.DesignSurface.UserInterface.DataPane.SelectionContext<T> selectionContext)
    {
      this.selectionContext = selectionContext;
    }

    bool ISelectable.IsSelected
    {
        get { return this.IsSelected; }
        set { base.IsSelected = value; }
    }

  }
}
