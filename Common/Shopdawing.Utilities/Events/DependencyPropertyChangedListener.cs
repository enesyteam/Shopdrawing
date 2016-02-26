// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.Events.DependencyPropertyChangedListener
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.Expression.Utility.Events
{
  public sealed class DependencyPropertyChangedListener : DependencyObject, IDisposable
  {
    public static readonly DependencyProperty DependencyPropertyBindingTargetProperty = DependencyProperty.Register("DependencyPropertyBindingTarget", typeof (object), typeof (DependencyPropertyChangedListener), new PropertyMetadata(new PropertyChangedCallback(DependencyPropertyChangedListener.DependencyPropertyBindingTargetChanged)));
    private List<DependencyPropertyChangedEventHandler> subscribedHandlers = new List<DependencyPropertyChangedEventHandler>();
    private Binding binding;
    private DependencyObject target;
    private string propertyPath;

    public object DependencyPropertyBindingTarget
    {
      get
      {
        return this.GetValue(DependencyPropertyChangedListener.DependencyPropertyBindingTargetProperty);
      }
      set
      {
        this.SetValue(DependencyPropertyChangedListener.DependencyPropertyBindingTargetProperty, value);
      }
    }

    public event DependencyPropertyChangedEventHandler DependencyPropertyChanged
    {
      add
      {
        if (this.subscribedHandlers.Count == 0)
          this.RegisterBinding();
        this.subscribedHandlers.Add(value);
      }
      remove
      {
        this.subscribedHandlers.Remove(value);
        if (this.subscribedHandlers.Count != 0)
          return;
        this.UnregisterBinding();
      }
    }

    public DependencyPropertyChangedListener(DependencyObject target, string propertyPath)
    {
      this.target = target;
      this.propertyPath = propertyPath;
    }

    public static void DependencyPropertyBindingTargetChanged(object sender, DependencyPropertyChangedEventArgs args)
    {
      ((DependencyPropertyChangedListener) sender).FireDependencyPropertyChanged(args);
    }

    private void FireDependencyPropertyChanged(DependencyPropertyChangedEventArgs args)
    {
      foreach (DependencyPropertyChangedEventHandler changedEventHandler in this.subscribedHandlers)
        changedEventHandler((object) this.target, args);
    }

    private void RegisterBinding()
    {
      if (this.binding != null)
        return;
      this.binding = new Binding(this.propertyPath);
      this.binding.Source = (object) this.target;
      BindingOperations.SetBinding((DependencyObject) this, DependencyPropertyChangedListener.DependencyPropertyBindingTargetProperty, (BindingBase) this.binding);
    }

    private void UnregisterBinding()
    {
      if (this.binding == null)
        return;
      this.subscribedHandlers.Clear();
      BindingOperations.ClearBinding((DependencyObject) this, DependencyPropertyChangedListener.DependencyPropertyBindingTargetProperty);
    }

    public void Dispose()
    {
      this.UnregisterBinding();
      GC.SuppressFinalize((object) this);
    }
  }
}
