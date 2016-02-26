// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.GroupControl
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.PlatformUI.Shell;
using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  public class GroupControl : LayoutSynchronizedTabControl
  {
    public GroupControl()
    {
      this.Loaded += (RoutedEventHandler) delegate
      {
        this.ClearValue(Selector.SelectedItemProperty);
      };
      UtilityMethods.AddPresentationSourceCleanupAction((UIElement) this, (Action) (() =>
      {
        BindingOperations.SetBinding((DependencyObject) this, Selector.SelectedItemProperty, (BindingBase) new Binding()
        {
          Mode = BindingMode.OneTime
        });
        BindingOperations.SetBinding((DependencyObject) this, ItemsControl.ItemsSourceProperty, (BindingBase) new Binding()
        {
          Mode = BindingMode.OneTime
        });
        this.DataContext = (object) null;
      }));
    }

    protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
    {
      base.OnItemsChanged(e);
      this.OnApplyTemplate();
    }
  }
}
