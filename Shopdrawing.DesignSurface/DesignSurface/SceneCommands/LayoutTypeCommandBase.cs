// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.LayoutTypeCommandBase
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal abstract class LayoutTypeCommandBase : ICommand
  {
    private SceneViewModel viewModel;
    private ITypeId type;

    protected ITypeId Type
    {
      get
      {
        return this.type;
      }
    }

    protected SceneViewModel SceneViewModel
    {
      get
      {
        return this.viewModel;
      }
    }

    protected DesignerContext DesignerContext
    {
      get
      {
        return this.viewModel.DesignerContext;
      }
    }

    public event EventHandler CanExecuteChanged;

    public LayoutTypeCommandBase(SceneViewModel viewModel, ITypeId type)
    {
      this.viewModel = viewModel;
      this.type = type;
    }

    public abstract void Execute(object arg);

    public bool CanExecute(object arg)
    {
      return true;
    }

    protected void OnCanExecuteChanged()
    {
      if (this.CanExecuteChanged == null)
        return;
      this.CanExecuteChanged((object) this, EventArgs.Empty);
    }
  }
}
