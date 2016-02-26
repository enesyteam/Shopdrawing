// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.ToggleAutoSizeBothCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class ToggleAutoSizeBothCommand : AutoSizeBaseCommand
  {
    public ToggleAutoSizeBothCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    public override void Execute()
    {
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
          foreach (BaseFrameworkElement frameworkElement in (IEnumerable<BaseFrameworkElement>) selection)
          {
            if ((bool) propertyValue)
            {
              if (!flag)
              {
                frameworkElement.ClearValue(BaseFrameworkElement.WidthProperty);
                frameworkElement.ClearValue(BaseFrameworkElement.HeightProperty);
              }
              else
              {
                frameworkElement.SetValue(BaseFrameworkElement.WidthProperty, (object) double.NaN);
                frameworkElement.SetValue(BaseFrameworkElement.HeightProperty, (object) double.NaN);
              }
              if (this.SceneViewModel.RootNode == frameworkElement)
              {
                Rect computedTightBounds = frameworkElement.GetComputedTightBounds();
                frameworkElement.SetValue(DesignTimeProperties.DesignWidthProperty, (object) computedTightBounds.Width);
                frameworkElement.SetValue(DesignTimeProperties.DesignHeightProperty, (object) computedTightBounds.Height);
              }
            }
            else
            {
              Rect computedTightBounds = frameworkElement.GetComputedTightBounds();
              frameworkElement.SetValue(BaseFrameworkElement.WidthProperty, (object) computedTightBounds.Width);
              frameworkElement.SetValue(BaseFrameworkElement.HeightProperty, (object) computedTightBounds.Height);
              if (this.SceneViewModel.RootNode == frameworkElement)
              {
                frameworkElement.ClearValue(DesignTimeProperties.DesignWidthProperty);
                frameworkElement.ClearValue(DesignTimeProperties.DesignHeightProperty);
              }
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
      foreach (BaseFrameworkElement frameworkElement in (IEnumerable<BaseFrameworkElement>) selection)
      {
        double d1 = (double) frameworkElement.GetComputedValue(BaseFrameworkElement.WidthProperty);
        flag &= double.IsNaN(d1);
        double d2 = (double) frameworkElement.GetComputedValue(BaseFrameworkElement.HeightProperty);
        flag &= double.IsNaN(d2);
        if (!flag)
          return (object) (bool) (flag ? true : false);
      }
      return (object) (bool) (flag ? true : false);
    }
  }
}
