// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Triggers.ValuePopup
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.Triggers
{
  public class ValuePopup : Control
  {
    public static readonly DependencyProperty IsEditingProperty = DependencyProperty.Register("IsEditing", typeof (bool), typeof (ValuePopup), new PropertyMetadata(new PropertyChangedCallback(ValuePopup.HandleIsEditingChanged)));
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof (object), typeof (ValuePopup), new PropertyMetadata(new PropertyChangedCallback(ValuePopup.HandleValueChanged)));
    private Popup popup;

    public bool IsEditing
    {
      get
      {
        return (bool) this.GetValue(ValuePopup.IsEditingProperty);
      }
      set
      {
        this.SetValue(ValuePopup.IsEditingProperty, (object) (bool) (value ? true : false));
      }
    }

    public object Value
    {
      get
      {
        return this.GetValue(ValuePopup.ValueProperty);
      }
      set
      {
        this.SetValue(ValuePopup.ValueProperty, value);
      }
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      this.popup = this.GetTemplateChild("PART_Popup") as Popup;
      if (this.popup == null)
        return;
      this.popup.Opened += new EventHandler(this.HandlePopupOpened);
    }

    private void HandlePopupOpened(object sender, EventArgs e)
    {
      this.popup.Child.Focus();
    }

    private static void HandleValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ValuePopup valuePopup = (ValuePopup) d;
      valuePopup.IsEditing = false;
      if (object.Equals(e.OldValue, e.NewValue))
        return;
      BindingExpression bindingExpression = valuePopup.GetBindingExpression(ValuePopup.ValueProperty);
      if (bindingExpression == null)
        return;
      bindingExpression.UpdateTarget();
    }

    private static void HandleIsEditingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
      base.OnMouseDown(e);
      if (!this.IsMouseCaptured)
        return;
      this.ReleaseMouseCapture();
    }

    protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
    {
      base.OnIsKeyboardFocusWithinChanged(e);
      if (this.IsKeyboardFocusWithin)
        return;
      this.IsEditing = false;
    }
  }
}
