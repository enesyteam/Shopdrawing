// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.ToggleLockInsertionPointCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class ToggleLockInsertionPointCommand : SceneCommandBase
  {
    protected override ViewState RequiredSelectionViewState
    {
      get
      {
        return ViewState.None;
      }
    }

    public override bool IsEnabled
    {
      get
      {
        return this.ElementToToggle != null;
      }
    }

    private SceneElement ElementToToggle
    {
      get
      {
        SceneElement primarySelection = this.SceneViewModel.ElementSelectionSet.PrimarySelection;
        if (primarySelection != null && primarySelection.IsContainer)
          return primarySelection;
        return (SceneElement) null;
      }
    }

    private bool ElementToToggleIsLockedInsertionPoint
    {
      get
      {
        SceneElement elementToToggle = this.ElementToToggle;
        ISceneInsertionPoint lockedInsertionPoint = this.SceneViewModel.LockedInsertionPoint;
        if (lockedInsertionPoint != null && elementToToggle != null && lockedInsertionPoint.SceneElement == elementToToggle)
          return lockedInsertionPoint.Property == elementToToggle.DefaultContentProperty;
        return false;
      }
    }

    public ToggleLockInsertionPointCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    public override object GetProperty(string propertyName)
    {
      if (propertyName == "IsChecked")
        return (object) (bool) (this.ElementToToggleIsLockedInsertionPoint ? true : false);
      return base.GetProperty(propertyName);
    }

    public override void Execute()
    {
      this.SetProperty("IsChecked", (object) (bool) (!(bool) this.GetProperty("IsChecked") ? true : false));
    }

    public override void SetProperty(string propertyName, object propertyValue)
    {
      if (propertyName == "IsChecked")
      {
        using (SceneEditTransaction editTransaction = this.SceneViewModel.CreateEditTransaction(StringTable.UndoUnitLockInsertionPoint))
        {
          if (this.ElementToToggleIsLockedInsertionPoint)
            this.SceneViewModel.SetLockedInsertionPoint((SceneElement) null);
          else
            this.SceneViewModel.SetLockedInsertionPoint(this.ElementToToggle);
          editTransaction.Commit();
        }
      }
      else
        base.SetProperty(propertyName, propertyValue);
    }
  }
}
