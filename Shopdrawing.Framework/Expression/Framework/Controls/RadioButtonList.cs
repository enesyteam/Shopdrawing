// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.RadioButtonList
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Microsoft.Expression.Framework.Controls
{
  public class RadioButtonList : ListBox
  {
    public RadioButtonList()
    {
      Style style = new Style(typeof (ListBoxItem));
      Setter setter1 = new Setter(FrameworkElement.MarginProperty, (object) new Thickness(2.0, 2.0, 2.0, 0.0));
      style.Setters.Add((SetterBase) setter1);
      FrameworkElementFactory frameworkElementFactory = new FrameworkElementFactory(typeof (RadioButton));
      frameworkElementFactory.SetValue(UIElement.FocusableProperty, (object) false);
      TemplateBindingExtension bindingExtension = new TemplateBindingExtension(ContentPresenter.ContentProperty);
      frameworkElementFactory.SetValue(ContentControl.ContentProperty, (object) bindingExtension);
      frameworkElementFactory.SetValue(ToggleButton.IsCheckedProperty, (object) new Binding()
      {
        Path = new PropertyPath("IsSelected", new object[0]),
        RelativeSource = RelativeSource.TemplatedParent,
        Mode = BindingMode.TwoWay
      });
      frameworkElementFactory.SetValue(UIElement.IsEnabledProperty, (object) new TemplateBindingExtension(UIElement.IsEnabledProperty));
      ControlTemplate controlTemplate = new ControlTemplate();
      controlTemplate.VisualTree = frameworkElementFactory;
      Setter setter2 = new Setter(Control.TemplateProperty, (object) controlTemplate);
      style.Setters.Add((SetterBase) setter2);
      this.Resources.Add((object) typeof (ListBoxItem), (object) style);
    }
  }
}
