// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.PropertyInspector.PropertyInspectorHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Windows;

namespace Microsoft.Expression.Framework.PropertyInspector
{
  public class PropertyInspectorHelper
  {
    public static readonly DependencyProperty OwningPropertyInspectorModelProperty = DependencyProperty.RegisterAttached("OwningPropertyInspectorModel", typeof (IPropertyInspector), typeof (PropertyInspectorHelper), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.Inherits));
    public static readonly DependencyProperty OwningPropertyInspectorElementProperty = DependencyProperty.RegisterAttached("OwningPropertyInspectorElement", typeof (UIElement), typeof (PropertyInspectorHelper), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.Inherits));

    public static void SetOwningPropertyInspectorModel(DependencyObject dependencyObject, IPropertyInspector value)
    {
      if (dependencyObject == null)
        throw new ArgumentNullException("dependencyObject");
      dependencyObject.SetValue(PropertyInspectorHelper.OwningPropertyInspectorModelProperty, (object) value);
    }

    public static IPropertyInspector GetOwningPropertyInspectorModel(DependencyObject dependencyObject)
    {
      if (dependencyObject == null)
        throw new ArgumentNullException("dependencyObject");
      return (IPropertyInspector) dependencyObject.GetValue(PropertyInspectorHelper.OwningPropertyInspectorModelProperty);
    }

    public static void SetOwningPropertyInspectorElement(DependencyObject dependencyObject, UIElement value)
    {
      if (dependencyObject == null)
        throw new ArgumentNullException("dependencyObject");
      dependencyObject.SetValue(PropertyInspectorHelper.OwningPropertyInspectorElementProperty, (object) value);
    }

    public static UIElement GetOwningPropertyInspectorElement(DependencyObject dependencyObject)
    {
      if (dependencyObject == null)
        throw new ArgumentNullException("dependencyObject");
      return (UIElement) dependencyObject.GetValue(PropertyInspectorHelper.OwningPropertyInspectorElementProperty);
    }
  }
}
