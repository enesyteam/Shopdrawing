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

using BridgeProject.Model;
using TreeViewItemChangedMvvm.ViewModel;
using DaisleyHarrison.WPF.ComplexDataTemplates.UnitTest;
using BridgeProject;
using ICSharpCode.TreeView;
using Shopdrawing.TreeNode;

namespace BridgeFS.Controls
{
    /// <summary>
    /// Interaction logic for Project.xaml
    /// </summary>
    public partial class Project : UserControl
    {
        public MyTreeViewViewModel _myTreeViewViewModel;
        public MyTreeViewViewModel MyTreeViewViewModel
        {
            get { return _myTreeViewViewModel; }
            set { _myTreeViewViewModel = value; }
        }
        public TreeView TreeView { get; set; }
        private Point dragStartPoint;

        public Project()
        {
            //List<ClassA> listOfClassA = new List<ClassA>();
            //ClassA classA1 = new ClassA();
            //classA1.Label = "Class A-1";
            //classA1.ToolTip = "This is Class A-1";
            //ClassB classB11 = new ClassB();
            //classB11.Label = "Class B-1-1";
            //classB11.ToolTip = "This is Class B-1-1";
            //classA1.ListOfClassB.Add(classB11);
            //ClassB classB12 = new ClassB();
            //classB12.Label = "Class B-1-2";
            //classB12.ToolTip = "This is Class B-1-2";
            //classA1.ListOfClassB.Add(classB12);
            //ClassC classC11 = new ClassC();
            //classC11.Label = "Class C-1-1";
            //classC11.ToolTip = "This is Class C-1-1";
            //classA1.ListOfClassC.Add(classC11);
            //ClassC classC12 = new ClassC();
            //classC12.Label = "Class C-1-2";
            //classC12.ToolTip = "This is Class C-1-2";
            //Item item121 = new Item();
            //item121.Label = "Rocky Road";
            //item121.ToolTip = "Rocky Road";
            //classC12.IceCream.Add(item121);
            //Item item122 = new Item();
            //item122.Label = "Chocolate";
            //item122.ToolTip = "Chocolate";
            //classC12.IceCream.Add(item122);
            //classA1.ListOfClassC.Add(classC12);
            //listOfClassA.Add(classA1);

           List<Bridge> bridge = new List<Bridge>();
           Bridge b = new Bridge() { BridgeName = "Bridge No.1"};
           bridge.Add(b);
           //Bridge b2 = new Bridge() { BridgeName = "Bridge No.2" };
           //bridge.Add(b2);

            _myTreeViewViewModel = new MyTreeViewViewModel
            {
                ItemsSource = bridge
            };

            //var firstNode = new ProjectItem { Name = "Bridge No.1", Color = Brushes.Red};
            //var first1 = new ProjectItem { Name = "Bridge No.1", Color = Brushes.Red, ItemType = ItemType.Folder };
            //var first2 = new ProjectItem { Name = "element2 (Drop Allowed)",
            //    AllowDrop = true, ItemType = ItemType.Note };
            //var first11 = new ProjectItem { Name = "Properties", ItemType = ItemType.Properties, AllowDrag = true };
            //var first12 = new ProjectItem { Name = "References", AllowDrag = true };
            //var first13 = new ProjectItem { Name = "General", AllowInsert = true, ItemType = ItemType.Folder};

            //firstNode.Children.Add(first1);
            //firstNode.Children.Add(first2);
            //first1.Children.Add(first11);
            //first1.Children.Add(first12);
            //first1.Children.Add(first13);

            DataContext = MyTreeViewViewModel;
            InitializeComponent();
            this.TreeView = Tree;

            BridgeProject.Items.Bridge br = new BridgeProject.Items.Bridge();

            ICSharpCode.TreeView.SharpTreeView Tree2 = new SharpTreeView();

            //BridgeListNode n = new BridgeListNode();
            //BridgeNode n1 = new BridgeNode(br) { Text = "B1"};
            //BridgeNode n2 = new BridgeNode(br) { Text = "Abutment2" };
            //BridgeNode n3 = new BridgeNode(br) { Text = "Abutment2" };
            
            //n2.Children.Add(n3);
            //n.Children.Add(n1);
            //n.Children.Add(n2);
            //Tree2.Root = n;
            //this.Content = Tree2;
        }


        //void contentView_Loaded(object sender, EventArgs e)
        //{
        //    var contentView = (SharpTreeView)sender;
        //    var dataContext = (SharpTreeView)contentView.DataContext;
        //    // do something
        //}

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
