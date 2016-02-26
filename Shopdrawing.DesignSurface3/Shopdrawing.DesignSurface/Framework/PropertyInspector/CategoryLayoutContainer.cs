// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.PropertyInspector.CategoryLayoutContainer
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Windows.Design.PropertyEditing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Microsoft.Expression.Framework.PropertyInspector
{
  public class CategoryLayoutContainer : ItemsControl
  {
    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
      ContentPresenter contentPresenter = element as ContentPresenter;
      CategoryEditor categoryEditor = item as CategoryEditor;
      if (contentPresenter != null && categoryEditor != null)
      {
        contentPresenter.SetBinding(ContentPresenter.ContentProperty, (BindingBase) new Binding("DataContext.Category")
        {
          RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof (CategoryLayoutContainer), 1)
        });
        contentPresenter.ContentTemplate = categoryEditor.EditorTemplate;
      }
      base.PrepareContainerForItemOverride(element, item);
    }
  }
}
