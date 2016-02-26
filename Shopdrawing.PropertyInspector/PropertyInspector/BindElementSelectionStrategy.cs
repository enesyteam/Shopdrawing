// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.BindElementSelectionStrategy
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class BindElementSelectionStrategy : ConstrainedElementSelectionStrategy
  {
    public override void SelectElement(SceneElement element, SceneNodeProperty editingProperty)
    {
      if (element == null || editingProperty == null)
        return;
      BehaviorHelper.CreateAndSetElementNameBinding(editingProperty, (SceneNode) element);
    }

    public override bool CanSelectElement(SceneElement element)
    {
      if (base.CanSelectElement(element))
        return element.CanNameElement;
      return false;
    }

    protected override Freezable CreateInstanceCore()
    {
      return (Freezable) new BindElementSelectionStrategy();
    }
  }
}
