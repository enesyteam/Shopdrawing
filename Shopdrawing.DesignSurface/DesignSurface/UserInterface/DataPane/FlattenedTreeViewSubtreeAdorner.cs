// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.FlattenedTreeViewSubtreeAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Data;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class FlattenedTreeViewSubtreeAdorner : Border
  {
    public static readonly DependencyProperty TargetItemsControlNameProperty = DependencyProperty.Register("TargetItemsControlName", typeof (string), typeof (FlattenedTreeViewSubtreeAdorner), (PropertyMetadata) new FrameworkPropertyMetadata((object) string.Empty));
    private ItemsControl treeView;

    public string TargetItemsControlName
    {
      get
      {
        return (string) this.GetValue(FlattenedTreeViewSubtreeAdorner.TargetItemsControlNameProperty);
      }
      set
      {
        this.SetValue(FlattenedTreeViewSubtreeAdorner.TargetItemsControlNameProperty, (object) value);
      }
    }

    static FlattenedTreeViewSubtreeAdorner()
    {
      UIElement.IsEnabledProperty.OverrideMetadata(typeof (FlattenedTreeViewSubtreeAdorner), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(FlattenedTreeViewSubtreeAdorner.OnIsEnabledChanged)));
    }

    public FlattenedTreeViewSubtreeAdorner()
    {
      this.Loaded += new RoutedEventHandler(this.FlattenedTreeViewSubtreeAdorner_Loaded);
    }

    private void FlattenedTreeViewSubtreeAdorner_Loaded(object sender, RoutedEventArgs e)
    {
      this.treeView = this.FindName(this.TargetItemsControlName) as ItemsControl;
      if (this.treeView != null && this.treeView.ItemContainerGenerator != null)
        this.treeView.ItemContainerGenerator.ItemsChanged += new ItemsChangedEventHandler(this.ItemContainerGenerator_StatusChanged);
      this.UpdateBounds();
    }

    private static void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs args)
    {
      FlattenedTreeViewSubtreeAdorner viewSubtreeAdorner = sender as FlattenedTreeViewSubtreeAdorner;
      if (viewSubtreeAdorner == null)
        return;
      viewSubtreeAdorner.UpdateBounds();
    }

    private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
    {
      this.Dispatcher.BeginInvoke((Delegate) new DelegateCommand.SimpleEventHandler(this.UpdateBounds), DispatcherPriority.Loaded, (object[]) null);
    }

    private void UpdateBounds()
    {
      Rect empty = Rect.Empty;
      if (this.IsEnabled && this.treeView != null)
      {
        DataSchemaItem dataSchemaItem = this.DataContext as DataSchemaItem;
        DataModelItemBase parent = dataSchemaItem.Parent;
        int num1 = parent != null ? parent.Children.IndexOf((DataModelItemBase) dataSchemaItem) : -1;
        DataModelItemBase dataModelItemBase = parent == null || num1 >= parent.Children.Count - 1 ? (DataModelItemBase) null : parent.Children[num1 + 1];
        int index = this.treeView.Items.IndexOf((object) dataSchemaItem);
        int num2 = dataModelItemBase != null ? this.treeView.Items.IndexOf((object) dataModelItemBase) : this.treeView.Items.Count;
        FrameworkElement frameworkElement1 = VisualTreeHelper.GetParent((DependencyObject) this) as FrameworkElement;
        if (dataSchemaItem != null && parent != null && (index >= 0 && num2 >= 0))
        {
          FrameworkElement frameworkElement2 = this.treeView.ItemContainerGenerator.ContainerFromIndex(index) as FrameworkElement;
          if (frameworkElement2 != null)
          {
            Rect rect = frameworkElement2.TransformToVisual((Visual) frameworkElement1).TransformBounds(new Rect(0.0, 0.0, frameworkElement2.ActualWidth, frameworkElement2.ActualHeight));
            empty.Union(rect);
          }
          if (num2 > index)
          {
            FrameworkElement frameworkElement3 = this.treeView.ItemContainerGenerator.ContainerFromIndex(num2 - 1) as FrameworkElement;
            if (frameworkElement3 != null)
            {
              Rect rect = frameworkElement3.TransformToVisual((Visual) frameworkElement1).TransformBounds(new Rect(0.0, 0.0, frameworkElement3.ActualWidth, frameworkElement3.ActualHeight));
              empty.Union(rect);
            }
          }
        }
      }
      if (empty.IsEmpty)
      {
        this.Visibility = Visibility.Collapsed;
      }
      else
      {
        this.Visibility = Visibility.Visible;
        this.RenderTransform = (Transform) new TranslateTransform(empty.Left, empty.Top);
        this.Height = empty.Height;
      }
    }
  }
}
