using BridgeProject;
using BridgeProject.Model;
using DaisleyHarrison.WPF.ComplexDataTemplates.UnitTest;
using ICSharpCode.TreeView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using TreeViewItemChangedMvvm.ViewModelUtils;

namespace BridgeFS.ViewModel
{
    public class ProjectTreeViewModel : ViewModelBase<ProjectTreeViewModel>
    {
        public ProjectTreeViewModel()
        {
            MySelItemChgCmd = new RelayCommand<TreeViewHelper.DependencyPropertyEventArgs>(TreeViewItemSelectedChangedCallBack);
            CurrSelItem = new object();
        }

        public List<SharpTreeNode> ItemsSource { get; set; }

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
