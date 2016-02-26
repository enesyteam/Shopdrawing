// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.RenameElementSelectionStrategy
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Globalization;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class RenameElementSelectionStrategy : ConstrainedElementSelectionStrategy
  {
    public override bool CanSelectElement(SceneElement element)
    {
      if (!element.ViewModel.ActiveEditingContainer.Equals((object) element))
        return base.CanSelectElement(element);
      return false;
    }

    public override void SelectElement(SceneElement element, SceneNodeProperty editingProperty)
    {
      if (element == null || editingProperty == null)
        return;
      string description = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.PropertyChangeUndoDescription, new object[1]
      {
        (object) editingProperty.PropertyName
      });
      using (SceneEditTransaction editTransaction = editingProperty.SceneNodeObjectSet.ViewModel.CreateEditTransaction(description))
      {
        element.EnsureNamed();
        editingProperty.SetValue((object) element.Name);
        editTransaction.Commit();
      }
    }

    protected override Freezable CreateInstanceCore()
    {
      return (Freezable) new RenameElementSelectionStrategy();
    }
  }
}
