// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Triggers.OnDemandComboBox
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Microsoft.Expression.DesignSurface.UserInterface.Triggers
{
  public class OnDemandComboBox : GroupableComboBox
  {
    public static readonly DependencyProperty SelectionProperty = DependencyProperty.Register("Selection", typeof (object), typeof (OnDemandComboBox), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnDemandComboBox.HandleSelectionChanged)));
    public static readonly DependencyProperty ItemsListProperty = DependencyProperty.Register("ItemsList", typeof (BindingBase), typeof (OnDemandComboBox));

    public object Selection
    {
      get
      {
        return this.GetValue(OnDemandComboBox.SelectionProperty);
      }
      set
      {
        this.SetValue(OnDemandComboBox.SelectionProperty, value);
      }
    }

    public BindingBase ItemsList
    {
      get
      {
        return (BindingBase) this.GetValue(OnDemandComboBox.ItemsListProperty);
      }
      set
      {
        this.SetValue(OnDemandComboBox.ItemsListProperty, (object) value);
      }
    }

    private static void HandleSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (object.Equals(e.NewValue, e.OldValue))
        return;
      OnDemandComboBox onDemandComboBox = (OnDemandComboBox) d;
      object newValue = e.NewValue;
      if (!onDemandComboBox.IsDropDownOpen)
      {
        List<object> list = new List<object>(1);
        if (newValue != null)
          list.Add(newValue);
        onDemandComboBox.ItemsSource = (IEnumerable) list;
      }
      onDemandComboBox.SelectedItem = newValue;
    }

    protected override void OnSelectionChanged(SelectionChangedEventArgs e)
    {
      base.OnSelectionChanged(e);
      if (this.SelectedIndex == -1)
        return;
      this.Selection = this.SelectedItem;
      BindingExpression bindingExpression = BindingOperations.GetBindingExpression((DependencyObject) this, OnDemandComboBox.SelectionProperty);
      if (bindingExpression == null)
        return;
      bindingExpression.UpdateTarget();
    }

    protected override void OnDropDownOpened(EventArgs e)
    {
      base.OnDropDownOpened(e);
      BindingBase bindingBase = BindingOperations.GetBindingBase((DependencyObject) this, OnDemandComboBox.ItemsListProperty);
      if (bindingBase == null)
        return;
      BindingOperations.SetBinding((DependencyObject) this, ItemsControl.ItemsSourceProperty, bindingBase);
    }
  }
}
