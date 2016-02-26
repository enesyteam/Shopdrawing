using BridgeFS.Controls;
using BridgeProject;
using BridgeProject.Model;
using DaisleyHarrison.WPF.ComplexDataTemplates.UnitTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using TreeViewItemChangedMvvm.ViewModelUtils;

namespace TreeViewItemChangedMvvm.ViewModel
{
    public class MyTreeViewViewModel : ViewModelBase<MyTreeViewViewModel>
    {
        public MyTreeViewViewModel()
        {
            MySelItemChgCmd = new RelayCommand<TreeViewHelper.DependencyPropertyEventArgs>(TreeViewItemSelectedChangedCallBack);
            CurrSelItem = new object();
        }

        public List<Bridge> ItemsSource { get; set; }

        public static object CurrSelItem { get; set; }

        public RelayCommand<TreeViewHelper.DependencyPropertyEventArgs> MySelItemChgCmd { get; set; }

        private static void TreeViewItemSelectedChangedCallBack(TreeViewHelper.DependencyPropertyEventArgs e)
        {
            //if (e != null && e.DependencyPropertyChangedEventArgs.NewValue != null)
            //    MessageBox.Show(
            //        string.Format("Selected Item:  {0}", ((ProjectItem)e.DependencyPropertyChangedEventArgs.NewValue).AllowDrop.ToString()),
            //        "MVVM TreeView Selected Item Demo",
            //        MessageBoxButton.OK,
            //        MessageBoxImage.Information
            //    );
            CurrSelItem = e.DependencyPropertyChangedEventArgs.NewValue;
        }
    }
}
