// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.AdornerLayout
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design.Model;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Windows.Design.Interaction
{
  public abstract class AdornerLayout
  {
    public abstract void AdornerPropertyChanged(DependencyObject adorner, DependencyPropertyChangedEventArgs args);

    public abstract bool EvaluateLayout(DesignerView view, UIElement adorner);

    public abstract void Measure(UIElement adorner, Size constraint);

    public abstract void Arrange(UIElement adorner);

    public abstract bool IsAssociated(UIElement adorner, ModelItem item);

    public virtual Size ArrangeChildren(FrameworkElement parent, UIElementCollection internalChildren, Size finalSize)
    {
      return finalSize;
    }
  }
}
