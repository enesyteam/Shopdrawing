// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.SkinEditing.StateModelBase
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Data;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.SkinEditing
{
  public abstract class StateModelBase : ModelBase
  {
    private bool isSelected;
    private ICommand selectCommand;
    private ICommand pinCommand;

    public bool IsSelected
    {
      get
      {
        return this.isSelected;
      }
      set
      {
        if (this.isSelected == value)
          return;
        this.isSelected = value;
        this.NotifyPropertyChanged("IsSelected");
        this.UpdateSelectedWithin();
      }
    }

    public abstract bool IsStructureEditable { get; }

    protected StateModelManager Manager
    {
      get
      {
        return this.ParentModel as StateModelManager;
      }
    }

    public ICommand SelectCommand
    {
      get
      {
        if (this.selectCommand == null)
          this.selectCommand = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.Select));
        return this.selectCommand;
      }
    }

    public ICommand PinCommand
    {
      get
      {
        if (this.pinCommand == null)
          this.pinCommand = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.Pin));
        return this.pinCommand;
      }
    }

    protected override bool GetDirectChildrenOrSelfSelected
    {
      get
      {
        return this.IsSelected;
      }
    }

    protected abstract void Select();

    protected abstract void Pin();
  }
}
