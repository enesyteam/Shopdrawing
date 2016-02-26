// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.SplitterItemsControl
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.VisualStudio.PlatformUI
{
  public class SplitterItemsControl : LayoutSynchronizedItemsControl
  {
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof (Orientation), typeof (SplitterItemsControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsMeasure));
    public static readonly DependencyProperty SplitterGripSizeProperty = DependencyProperty.RegisterAttached("SplitterGripSize", typeof (double), typeof (SplitterItemsControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) 5.0, FrameworkPropertyMetadataOptions.Inherits));

    public Orientation Orientation
    {
      get
      {
        return (Orientation) this.GetValue(SplitterItemsControl.OrientationProperty);
      }
      set
      {
        this.SetValue(SplitterItemsControl.OrientationProperty, (object) value);
      }
    }

    static SplitterItemsControl()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (SplitterItemsControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (SplitterItemsControl)));
    }

    public static double GetSplitterGripSize(DependencyObject element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      return (double) element.GetValue(SplitterItemsControl.SplitterGripSizeProperty);
    }

    public static void SetSplitterGripSize(DependencyObject element, double value)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      element.SetValue(SplitterItemsControl.SplitterGripSizeProperty, (object) value);
    }

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
      return item is SplitterItem;
    }

    protected override DependencyObject GetContainerForItemOverride()
    {
      return (DependencyObject) new SplitterItem();
    }
  }
}
