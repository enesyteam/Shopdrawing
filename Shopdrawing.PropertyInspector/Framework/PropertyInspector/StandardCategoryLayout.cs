// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.PropertyInspector.StandardCategoryLayout
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Windows.Design.PropertyEditing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Microsoft.Expression.Framework.PropertyInspector
{
  public class StandardCategoryLayout : ItemsControl
  {
    protected override DependencyObject GetContainerForItemOverride()
    {
      return (DependencyObject) new PropertyContainer();
    }

    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
      PropertyContainer propertyContainer = element as PropertyContainer;
      if (propertyContainer != null)
      {
        ((FrameworkElement) propertyContainer).DataContext = item;
        ((FrameworkElement) propertyContainer).SetBinding((DependencyProperty) PropertyContainer.PropertyEntryProperty, (BindingBase) new Binding());
      }
      base.PrepareContainerForItemOverride(element, item);
    }
  }
}
