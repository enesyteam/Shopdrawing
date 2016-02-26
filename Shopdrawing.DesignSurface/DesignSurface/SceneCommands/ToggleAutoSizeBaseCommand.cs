// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.ToggleAutoSizeBaseCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal abstract class ToggleAutoSizeBaseCommand : AutoSizeBaseCommand
  {
    protected abstract IPropertyId SizeProperty { get; }

    protected abstract IPropertyId DesignSizeProperty { get; }

    public ToggleAutoSizeBaseCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    protected abstract double GetActualSize(BaseFrameworkElement element);

    public override void Execute()
    {
      this.SetProperty("IsChecked", (object) (bool) (!(bool) this.GetProperty("IsChecked") ? true : false));
    }

    public override void SetProperty(string propertyName, object propertyValue)
    {
      if (propertyName == "IsChecked")
      {
        ICollection<BaseFrameworkElement> selection = this.Selection;
        if (selection == null)
          return;
        using (SceneEditTransaction editTransaction = this.SceneViewModel.CreateEditTransaction(StringTable.UndoUnitToggleAutoSize))
        {
          bool flag = this.DesignerContext.ActiveDocument != null && this.DesignerContext.ActiveDocument.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsAutoAndNan);
          foreach (BaseFrameworkElement element in (IEnumerable<BaseFrameworkElement>) selection)
          {
            if ((bool) propertyValue)
            {
              if (!flag)
                element.ClearValue(this.SizeProperty);
              else
                element.SetValue(this.SizeProperty, (object) double.NaN);
              if (this.SceneViewModel.RootNode == element)
                element.SetValue(this.DesignSizeProperty, (object) this.GetActualSize(element));
            }
            else
            {
              element.SetValue(this.SizeProperty, (object) this.GetActualSize(element));
              if (this.SceneViewModel.RootNode == element)
                element.ClearValue(this.DesignSizeProperty);
            }
          }
          editTransaction.Commit();
        }
      }
      else
        base.SetProperty(propertyName, propertyValue);
    }

    public override object GetProperty(string propertyName)
    {
      if (!(propertyName == "IsChecked"))
        return base.GetProperty(propertyName);
      ICollection<BaseFrameworkElement> selection = this.Selection;
      if (selection == null || selection.Count <= 0)
        return (object) false;
      bool flag = true;
      foreach (SceneNode sceneNode in (IEnumerable<BaseFrameworkElement>) selection)
      {
        double d = (double) sceneNode.GetComputedValue(this.SizeProperty);
        flag &= double.IsNaN(d);
      }
      return (object) (bool) (flag ? true : false);
    }
  }
}
