using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Shopdrawing.Controls.Model;

namespace Shopdrawing.Controls
{
    /// <summary>
    /// Interaction logic for Project.xaml
    /// </summary>
    public partial class Project : UserControl
    {
        private Point dragStartPoint;

        public Project()
        {
            var firstNode = new ProjectItem { Name = "New Project", Color = Brushes.Red};
            var first1 = new ProjectItem { Name = "Project1", Color = Brushes.Red, ItemType = ItemType.Folder};
            var first2 = new ProjectItem { Name = "element2 (Drop Allowed)",
                AllowDrop = true, ItemType = ItemType.Note };
            var first11 = new ProjectItem { Name = "Properties", ItemType = ItemType.Properties, AllowDrag = true };
            var first12 = new ProjectItem { Name = "References", AllowDrag = true };
            var first13 = new ProjectItem { Name = "General", AllowInsert = true, ItemType = ItemType.Folder};

            firstNode.Children.Add(first1);
            firstNode.Children.Add(first2);
            first1.Children.Add(first11);
            first1.Children.Add(first12);
            first1.Children.Add(first13);

            DataContext = firstNode;
            InitializeComponent();
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            dragStartPoint = e.GetPosition(null);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(null);
            Vector diff = dragStartPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed
                && (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                var frameworkElem = ((FrameworkElement)e.OriginalSource);
                DragDrop.DoDragDrop(frameworkElem, new DataObject("Node", frameworkElem.DataContext), DragDropEffects.Move);
            }
        }
    }
}
