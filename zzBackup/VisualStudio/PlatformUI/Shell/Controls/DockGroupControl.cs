// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.DockGroupControl
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  public class DockGroupControl : SplitterItemsControl
  {
    static DockGroupControl()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (DockGroupControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (DockGroupControl)));
    }

    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
      base.PrepareContainerForItemOverride(element, item);
      if (this.Orientation == Orientation.Horizontal)
      {
        BindingOperations.SetBinding(element, SplitterPanel.SplitterLengthProperty, (BindingBase) new Binding()
        {
          Source = item,
          Path = new PropertyPath("DockedWidth", new object[0]),
          Mode = BindingMode.TwoWay
        });
        BindingOperations.SetBinding(element, SplitterPanel.MinimumLengthProperty, (BindingBase) new MultiBinding()
        {
          Converter = (IMultiValueConverter) new DockGroupControl.EffectiveMinimumSizeConverter(),
          Bindings = {
            (BindingBase) new Binding()
            {
              Source = item,
              Path = new PropertyPath("MinimumWidth", new object[0]),
              Mode = BindingMode.OneWay
            },
            (BindingBase) new Binding()
            {
              Source = (object) element,
              Path = new PropertyPath((object) SplitterItemsControl.SplitterGripSizeProperty),
              Mode = BindingMode.OneWay
            },
            (BindingBase) new Binding()
            {
              Source = (object) element,
              Path = new PropertyPath((object) SplitterPanel.IsLastProperty),
              Mode = BindingMode.OneWay
            }
          }
        });
      }
      else
      {
        BindingOperations.SetBinding(element, SplitterPanel.SplitterLengthProperty, (BindingBase) new Binding()
        {
          Source = item,
          Path = new PropertyPath("DockedHeight", new object[0]),
          Mode = BindingMode.TwoWay
        });
        BindingOperations.SetBinding(element, SplitterPanel.MinimumLengthProperty, (BindingBase) new MultiBinding()
        {
          Converter = (IMultiValueConverter) new DockGroupControl.EffectiveMinimumSizeConverter(),
          Bindings = {
            (BindingBase) new Binding()
            {
              Source = item,
              Path = new PropertyPath("MinimumHeight", new object[0]),
              Mode = BindingMode.OneWay
            },
            (BindingBase) new Binding()
            {
              Source = (object) element,
              Path = new PropertyPath((object) SplitterItemsControl.SplitterGripSizeProperty),
              Mode = BindingMode.OneWay
            },
            (BindingBase) new Binding()
            {
              Source = (object) element,
              Path = new PropertyPath((object) SplitterPanel.IsLastProperty),
              Mode = BindingMode.OneWay
            }
          }
        });
      }
    }

    private class EffectiveMinimumSizeConverter : MultiValueConverter<double, double, bool, double>
    {
      protected override double Convert(double contentSize, double splitterSize, bool isLast, object parameter, CultureInfo culture)
      {
        double num = contentSize;
        if (!isLast)
          num += splitterSize;
        return num;
      }
    }
  }
}
