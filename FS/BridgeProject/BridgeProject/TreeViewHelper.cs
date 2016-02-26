using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TreeViewItemChangedMvvm.ViewModelUtils
{
    public class TreeViewHelper
    {
        #region SelectedItem

        public static object GetSelectedItem(DependencyObject obj)
        {
            return (object)obj.GetValue(SelectedItemProperty);
        }

        public static void SetSelectedItem(DependencyObject obj, object value)
        {
            obj.SetValue(SelectedItemProperty, value);
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.RegisterAttached("SelectedItem", typeof(object), typeof(TreeViewHelper), new UIPropertyMetadata(null, SelectedItemChanged));

        private static void SelectedItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is TreeView) || e.NewValue == null)
                return;

            var view = obj as TreeView;

            view.SelectedItemChanged += (sender, e2) => SetSelectedItem(view, e2.NewValue);

            //view.OnSelecting += (sender, e2) => SetSelectedItem(view, e2.NewValue);
            var command = (ICommand)(view as DependencyObject).GetValue(SelectedItemChangedProperty);
            if (command != null)
            {
                if (command.CanExecute(null))
                    command.Execute(new DependencyPropertyEventArgs(e));
            }
        }

        #endregion


        #region Selected Item Changed

        public static ICommand GetSelectedItemChanged(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(SelectedItemProperty);
        }

        public static void SetSelectedItemChanged(DependencyObject obj, ICommand value)
        {
            obj.SetValue(SelectedItemProperty, value);
        }

        public static readonly DependencyProperty SelectedItemChangedProperty =
            DependencyProperty.RegisterAttached("SelectedItemChanged", typeof(ICommand), typeof(TreeViewHelper));

        #endregion

        #region Event Args

        public class DependencyPropertyEventArgs : EventArgs
        {
            public DependencyPropertyChangedEventArgs DependencyPropertyChangedEventArgs { get; private set; }

            public DependencyPropertyEventArgs(DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
            {
                this.DependencyPropertyChangedEventArgs = dependencyPropertyChangedEventArgs;
            }
        }

        #endregion

    }
}
