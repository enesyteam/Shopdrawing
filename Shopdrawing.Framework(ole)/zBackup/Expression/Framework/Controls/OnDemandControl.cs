// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.OnDemandControl
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.Framework.Controls
{
  public class OnDemandControl : Control
  {
    public static readonly DependencyProperty OnDemandTemplateProperty = DependencyProperty.Register("OnDemandTemplate", typeof (ControlTemplate), typeof (OnDemandControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(OnDemandControl.OnDemandTemplatePropertyChanged)));
    private bool templateIsApplied;

    public ControlTemplate OnDemandTemplate
    {
      get
      {
        return (ControlTemplate) this.GetValue(OnDemandControl.OnDemandTemplateProperty);
      }
      set
      {
        this.SetValue(OnDemandControl.OnDemandTemplateProperty, (object) value);
      }
    }

    static OnDemandControl()
    {
      UIElement.VisibilityProperty.OverrideMetadata(typeof (OnDemandControl), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDemandControl.VisibilityPropertyChanged)));
    }

    private static void VisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      OnDemandControl onDemandControl = d as OnDemandControl;
      if (onDemandControl == null)
        return;
      onDemandControl.OnVisibilityChanged((Visibility) e.NewValue);
    }

    private static void OnDemandTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      OnDemandControl onDemandControl = d as OnDemandControl;
      if (onDemandControl == null || !onDemandControl.templateIsApplied && onDemandControl.Visibility != Visibility.Visible)
        return;
      onDemandControl.templateIsApplied = true;
      onDemandControl.Template = (ControlTemplate) e.NewValue;
    }

    private void OnVisibilityChanged(Visibility newValue)
    {
      if (newValue != Visibility.Visible || this.templateIsApplied)
        return;
      this.Template = this.OnDemandTemplate;
      this.templateIsApplied = true;
    }
  }
}
