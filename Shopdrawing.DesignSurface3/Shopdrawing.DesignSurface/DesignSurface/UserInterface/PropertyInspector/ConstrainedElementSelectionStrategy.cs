// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.ConstrainedElementSelectionStrategy
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public abstract class ConstrainedElementSelectionStrategy : Freezable, IElementSelectionStrategy
  {
    public static readonly DependencyProperty TypeConstraintProperty = DependencyProperty.Register("TypeConstraint", typeof (ITypeId), typeof (ConstrainedElementSelectionStrategy));

    public ITypeId TypeConstraint
    {
      get
      {
        return (ITypeId) this.GetValue(ConstrainedElementSelectionStrategy.TypeConstraintProperty);
      }
      set
      {
        this.SetValue(ConstrainedElementSelectionStrategy.TypeConstraintProperty, (object) value);
      }
    }

    public virtual bool CanSelectElement(SceneElement element)
    {
      if (this.TypeConstraint == null)
        return true;
      return this.TypeConstraint.IsAssignableFrom((ITypeId) element.Type);
    }

    public abstract void SelectElement(SceneElement element, SceneNodeProperty editingProperty);
  }
}
