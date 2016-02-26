using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ChristianMoser.WpfInspector.UserInterface;

using Microsoft.Expression.Framework.Controls;
using DynamicGeometry;
using Xceed.Wpf.AvalonDock.Layout;
using System.Diagnostics;
using Xceed.Wpf.AvalonDock.Layout.Serialization;
using System.IO;
using Xceed.Wpf.AvalonDock;
using System.ComponentModel;
using Shopdrawing.TreeNode;
using BridgeProject.Items;
using Microsoft.Windows.Design.Interaction;
using System.ComponentModel.Composition;

namespace TestApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Notifies the changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void NotifyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        DynamicGeometry.PropertyGridHost PropertyGridHost { get; set; }
        ChristianMoser.WpfInspector.UserInterface.Controls.PropertyGrid PropertyGrid { get; set; }

        DynamicGeometry.DrawingHost _drawingHost = null;
        DynamicGeometry.DrawingHost DrawingHost
        {
            get { return _drawingHost; }
            set
            {
                _drawingHost = value;
                NotifyChanged("DrawingHost");
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }


        private void dockManager_ActiveContentChanged(object sender, EventArgs e)
        {
            // Nếu tab chứa nội dung là Drawing
            //var firstDocumentPane = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            //if (firstDocumentPane != null)
            //{
            //    var activeContent = firstDocumentPane.SelectedContent.Content as DrawingHost;
            //    if (activeContent.CurrentDrawing != null)
            //    MessageBox.Show(activeContent.CurrentDrawing.Figures.Count.ToString());
            //}
            try
            {
                var activeContent = dockManager.ActiveContent as DrawingHost;
                if (activeContent.CurrentDrawing != null)
                {
                    this.DrawingHost = activeContent;
                    //MainToolbar.Drawing = activeContent.CurrentDrawing;
                    this.DrawingHost.CurrentDrawing.SelectionChanged += CurrentDrawing_SelectionChanged;

                }
            }
            catch { }
        }
        #region Methods
        public void NewProject()
        {
            //Drawing
            DrawingHost = new DynamicGeometry.DrawingHost();
            DrawingHost.Ribbon.Visibility = System.Windows.Visibility.Collapsed;
            // Add Behaviors
            var behaviors = Behavior.LoadBehaviors(typeof(Dragger).Assembly);
            Behavior.Default = behaviors.First(b => b is Dragger);
            foreach (var behavior in behaviors)
            {
                DrawingHost.AddToolButton(behavior);
            }
            var firstDocumentPane = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            if (firstDocumentPane != null)
            {
                LayoutDocument doc = new LayoutDocument();
                doc.Title = "Drawing" + drawingCount;
                drawingCount++;
                doc.Content = DrawingHost;
                firstDocumentPane.Children.Add(doc);
            }

            //Project Explorer
            var projectExplorer = dockManager.Layout.Descendents().OfType<LayoutAnchorablePane>().LastOrDefault();
            var rightAnchorgroup = dockManager.Layout.Descendents().OfType<LayoutAnchorablePaneGroup>().FirstOrDefault();
            if (projectExplorer != null)
            {
                // Tạo 2 LayoutAnchorablePane
                LayoutAnchorablePane l1 = new LayoutAnchorablePane();
                LayoutAnchorablePane l2 = new LayoutAnchorablePane();

                LayoutAnchorable doc = new LayoutAnchorable();

                ICSharpCode.TreeView.SharpTreeView projectTree = new ICSharpCode.TreeView.SharpTreeView() 
                { AllowDrop = true, AllowDropOrder = true };
                ICSharpCode.ILSpy.ContextMenuProvider.Add(projectTree);
                //App.CompositionContainer.ComposeParts(this);
                
                projectTree.SelectionChanged += projectTree_SelectionChanged;
                projectTree.MouseDoubleClick += projectTree_MouseDoubleClick;

                // Dữ liệu ban đầu
                BridgesList brl = new BridgesList() { Name = "Bridges List"};
                BridgeProject.Items.Bridge br = new BridgeProject.Items.Bridge() { Name = "Bridge 1"};
                //Substructures sub1 = new Substructures();
                //br.Substructures = sub1;

                BridgeProject.Items.Bridge br2 = new BridgeProject.Items.Bridge() { Name = "Bridge 2" };
                brl.Children.Add(br, br2);

                BridgeListNode n = new BridgeListNode(brl);
                foreach (Bridge b in brl.Children)
                {
                    BridgeNode n1 = new BridgeNode(b) { Text = b.Name };
                    n.Children.Add(n1);

                    SubstructuresNode subNode = new SubstructuresNode(b.Substructures);
                    n1.Children.Add(subNode);

                    AbutmentsListNode abutsNode = new AbutmentsListNode(b.Substructures.Abutments);
                    subNode.Children.Add(abutsNode);

                    PiersListNode piersNode = new PiersListNode(b.Substructures.Piers);
                    subNode.Children.Add(piersNode);

                    foreach (Abutment a in b.Substructures.Abutments.Children)
                    {
                        AbutmentNode abn = new AbutmentNode(a);
                        abutsNode.Children.Add(abn);
                    }

                    foreach (Pier p in b.Substructures.Piers.Children)
                    {
                        PierNode abn = new PierNode(p);
                        piersNode.Children.Add(abn);
                    }

                }

                projectTree.Root = n;
                //

                doc.Content = projectTree;
                doc.Title = "Project Explorer";
                doc.ContentId = "projectExplorer";

                l1.Children.Add(doc);
                
                // PropertyGrid
                PropertyGridHost = new DynamicGeometry.PropertyGridHost();
                PropertyGrid = new ChristianMoser.WpfInspector.UserInterface.Controls.PropertyGrid()
                {
                    VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                    ShowMethod = true,
                };
                PropertyGridHost.Children.Add(PropertyGrid);
                LayoutAnchorable prop = new LayoutAnchorable();
                prop.Content = PropertyGridHost;

                prop.ContentId = "propertyGrid";
                prop.Title = "Properties";
                l2.Children.Add(prop);

                rightAnchorgroup.Children.Add(l1);
                rightAnchorgroup.Children.Add(l2);

                //
                //var pe = dockManager.Layout.Descendents().OfType<LayoutAnchorable>().Single(a => a.ContentId == "projectExplorer");
                //var tree = pe.Content as BridgeFS.Controls.Project;
                //if (tree != null)
                //{
                //    //MessageBox.Show(tree.TreeView.ToString() + "xx");
                //    Binding b = new Binding();
                //    b.Source = tree.TreeView;
                //    //b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                //    b.Path = new PropertyPath(TreeViewItemChangedMvvm.ViewModelUtils.TreeViewHelper.SelectedItemProperty);
                //    this.PropertyGrid.SetBinding(ChristianMoser.WpfInspector.UserInterface.Controls.PropertyGrid.SelectedObjectProperty, b);
                //}
            }
            dockManager.UpdateLayout();
            //MainToolbar.Drawing = DrawingHost.CurrentDrawing;
            //DrawingHost.CurrentDrawing.SelectionChanged += CurrentDrawing_SelectionChanged;
            dockManager.ActiveContentChanged += dockManager_ActiveContentChanged;
        }

        void projectTree_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                ICSharpCode.TreeView.SharpTreeView t = sender as ICSharpCode.TreeView.SharpTreeView;
                if (t.SelectedItems.Count == 1)
                {
                    ShopdrawingTreeNode node = t.SelectedItem as ShopdrawingTreeNode;
                    if(node.Children.Count == 0)
                    MessageBox.Show(node.DataObject != null ? node.DataObject.ToString() : "Null");
                }
            }
        }
        //public IEnumerable<ILSpyTreeNode> SelectedNodes
        //{
        //    get
        //    {
        //        return treeView.GetTopLevelSelection().OfType<ILSpyTreeNode>();
        //    }
        //}
        void projectTree_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ICSharpCode.TreeView.SharpTreeView t = sender as ICSharpCode.TreeView.SharpTreeView;
            ShowProperties(t);
            if (SelectionChanged != null)
                SelectionChanged(sender, e);
        }
        public event SelectionChangedEventHandler SelectionChanged;
        void ShowProperties(ICSharpCode.TreeView.SharpTreeView t)
        {
            if (t.SelectedItems.Count == 1)
            {
                ShopdrawingTreeNode node = t.SelectedItem as ShopdrawingTreeNode;
                PropertyGrid.SelectedObject = node.DataObject;
            }
            else
            {
                ShopdrawingTreeNode nodes = t.SelectedItems as ShopdrawingTreeNode;
                
            }
        }
        //public IEnumerable<ShopdrawingTreeNode> SelectedNodes
        //{
        //    get
        //    {
        //        return treeView.GetTopLevelSelection().OfType<ShopdrawingTreeNode>();
        //    }
        //}
        int drawingCount = 1;
        private void NewDrawing()
        {
            //Drawing
            DynamicGeometry.DrawingHost drawingHost = new DynamicGeometry.DrawingHost();
            drawingHost.Ribbon.Visibility = System.Windows.Visibility.Collapsed;
            // Add Behaviors
            var behaviors = Behavior.LoadBehaviors(typeof(Dragger).Assembly);
            Behavior.Default = behaviors.First(b => b is Dragger);
            foreach (var behavior in behaviors)
            {
                drawingHost.AddToolButton(behavior);
            }
            var firstDocumentPane = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            if (firstDocumentPane != null)
            {
                LayoutDocument doc = new LayoutDocument();
                doc.Title = "Drawing" + drawingCount;
                drawingCount++;
                doc.Content = drawingHost;
                firstDocumentPane.Children.Add(doc);
            }
            dockManager.UpdateLayout();
            this.DrawingHost = drawingHost;
        }

        private void CurrentDrawing_SelectionChanged(object sender, DynamicGeometry.Drawing.SelectionChangedEventArgs e)
        {
            var selection = DrawingHost.CurrentDrawing.GetSelectedFigures().ToArray();
            if (selection.Length == 1)
            {
                ShowProperties(selection[0]);
            }
            else if (selection.Length > 1)
            {
                ShowProperties(selection);
            }
            else
            {
                //ShowProperties(this.DrawingHost.CurrentDrawing);
            }
        }
        public virtual void ShowProperties(object selection)
        {
            //PropertyGrid.SelectedObject = selection;
        }
        #endregion


        #region TestTimer

        /// <summary>
        /// TestTimer Dependency Property
        /// </summary>
        public static readonly DependencyProperty TestTimerProperty =
            DependencyProperty.Register("TestTimer", typeof(int), typeof(MainWindow),
                new FrameworkPropertyMetadata((int)0));

        /// <summary>
        /// Gets or sets the TestTimer property.  This dependency property 
        /// indicates a test timer that elapses evry one second (just for binding test).
        /// </summary>
        public int TestTimer
        {
            get { return (int)GetValue(TestTimerProperty); }
            set { SetValue(TestTimerProperty, value); }
        }

        #endregion

        #region TestBackground

        /// <summary>
        /// TestBackground Dependency Property
        /// </summary>
        public static readonly DependencyProperty TestBackgroundProperty =
            DependencyProperty.Register("TestBackground", typeof(Brush), typeof(MainWindow),
                new FrameworkPropertyMetadata((Brush)null));

        /// <summary>
        /// Gets or sets the TestBackground property.  This dependency property 
        /// indicates a randomly changing brush (just for testing).
        /// </summary>
        public Brush TestBackground
        {
            get { return (Brush)GetValue(TestBackgroundProperty); }
            set { SetValue(TestBackgroundProperty, value); }
        }

        #endregion

        #region FocusedElement

        /// <summary>
        /// FocusedElement Dependency Property
        /// </summary>
        public static readonly DependencyProperty FocusedElementProperty =
            DependencyProperty.Register("FocusedElement", typeof(string), typeof(MainWindow),
                new FrameworkPropertyMetadata((IInputElement)null));

        /// <summary>
        /// Gets or sets the FocusedElement property.  This dependency property 
        /// indicates ....
        /// </summary>
        public string FocusedElement
        {
            get { return (string)GetValue(FocusedElementProperty); }
            set { SetValue(FocusedElementProperty, value); }
        }

        #endregion

        private void OnLayoutRootPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var activeContent = ((LayoutRoot)sender).ActiveContent;
            if (e.PropertyName == "ActiveContent")
            {
                Debug.WriteLine(string.Format("ActiveContent-> {0}", activeContent));
                //MessageBox.Show(string.Format("ActiveContent-> {0}", activeContent));
            }
        }
        private void OnLoadLayout(object sender, RoutedEventArgs e)
        {
            var currentContentsList = dockManager.Layout.Descendents().OfType<LayoutContent>().Where(c => c.ContentId != null).ToArray();

            string fileName = (sender as MenuItem).Header.ToString();
            var serializer = new XmlLayoutSerializer(dockManager);
            //serializer.LayoutSerializationCallback += (s, args) =>
            //    {
            //        var prevContent = currentContentsList.FirstOrDefault(c => c.ContentId == args.Model.ContentId);
            //        if (prevContent != null)
            //            args.Content = prevContent.Content;
            //    };
            using (var stream = new StreamReader(string.Format(@".\AvalonDock_{0}.config", fileName)))
            {
                serializer.Deserialize(stream);
            }
        }

        private void OnSaveLayout(object sender, RoutedEventArgs e)
        {
            string fileName = (sender as MenuItem).Header.ToString();
            var serializer = new XmlLayoutSerializer(dockManager);
            using (var stream = new StreamWriter(string.Format(@".\AvalonDock_{0}.config", fileName)))
            {
                serializer.Serialize(stream);
            }
        }

        private void OnShowWinformsWindow(object sender, RoutedEventArgs e)
        {
            var winFormsWindow = dockManager.Layout.Descendents().OfType<LayoutAnchorable>().Single(a => a.ContentId == "WinFormsWindow");
            if (winFormsWindow.IsHidden)
                winFormsWindow.Show();
            else if (winFormsWindow.IsVisible)
                winFormsWindow.IsActive = true;
            else
                winFormsWindow.AddToLayout(dockManager, AnchorableShowStrategy.Bottom | AnchorableShowStrategy.Most);
        }

        private void AddTwoDocuments_click(object sender, RoutedEventArgs e)
        {
            var firstDocumentPane = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            if (firstDocumentPane != null)
            {
                LayoutDocument doc = new LayoutDocument();
                doc.Title = "Test1";
                firstDocumentPane.Children.Add(doc);

                LayoutDocument doc2 = new LayoutDocument();
                doc2.Title = "Test2";
                firstDocumentPane.Children.Add(doc2);
                firstDocumentPane.SelectedContentIndex += 1;
            }

            //var leftAnchorGroup = dockManager.Layout.LeftSide.Children.FirstOrDefault();
            //if (leftAnchorGroup == null)
            //{
            //    leftAnchorGroup = new LayoutAnchorGroup();
            //    dockManager.Layout.LeftSide.Children.Add(leftAnchorGroup);
            //}

            //leftAnchorGroup.Children.Add(new LayoutAnchorable() { Title = "New Anchorable" });

        }

        private void OnNewProjectClick(object sender, RoutedEventArgs e)
        {
            NewProject();
        }
        private void OnNewDrawingClick(object sender, RoutedEventArgs e)
        {
            NewDrawing();
        }

        private void OnNewItemClick(object sender, RoutedEventArgs e)
        {
            BridgeFS.NewProject newproject = new BridgeFS.NewProject();
            newproject.ShowDialog();
        }
        private void OnNewBridgeProjectClick(object sender, RoutedEventArgs e)
        {
            BridgeFS.CreateNewProjectDialog newproject = new BridgeFS.CreateNewProjectDialog();
            newproject.Owner = this;
            newproject.ShowDialog();
        }
        #region VIEW MENU
        private void OnShowToolWindow1(object sender, RoutedEventArgs e)
        {
            var toolWindow1 = dockManager.Layout.Descendents().OfType<LayoutAnchorable>().Single(a => a.ContentId == "toolWindow1");
            if (toolWindow1.IsHidden)
                toolWindow1.Show();
            else if (toolWindow1.IsVisible)
                toolWindow1.IsActive = true;
            else
                toolWindow1.AddToLayout(dockManager, AnchorableShowStrategy.Bottom | AnchorableShowStrategy.Most);
        }
        private void OnShowProjectExplorer(object sender, RoutedEventArgs e)
        {
            var projectExplorer = dockManager.Layout.Descendents().OfType<LayoutAnchorable>().Single(a => a.ContentId == "projectExplorer");
            if (projectExplorer.IsHidden)
                projectExplorer.Show();
            else if (projectExplorer.IsVisible)
                projectExplorer.IsActive = true;
            else
                projectExplorer.AddToLayout(dockManager, AnchorableShowStrategy.Bottom | AnchorableShowStrategy.Most);
        }
        private void OnShowPropertyGrid(object sender, RoutedEventArgs e)
        {
            var projectExplorer = dockManager.Layout.Descendents().OfType<LayoutAnchorable>().Single(a => a.ContentId == "propertyGrid");
            if (projectExplorer.IsHidden)
                projectExplorer.Show();
            else if (projectExplorer.IsVisible)
                projectExplorer.IsActive = true;
            else
                projectExplorer.AddToLayout(dockManager, AnchorableShowStrategy.Bottom | AnchorableShowStrategy.Most);
        }
        #endregion

        #region DRAW MENU
        private void OnAddCulvertClick(object sender, RoutedEventArgs e)
        {
            int row = 1;
            int column = 1;
            for (int i = 0; i < row; i++)
            {
                System.Windows.Point p = new System.Windows.Point(); // Diem chen
                for (int j = 0; j < column; j++)
                {
                    p = new System.Windows.Point((j) * 1000, i * 1000);
                    Shopdrawing.Structures.Culvert.BoxCulvertSection cs =
                new Shopdrawing.Structures.Culvert.BoxCulvertSection(this.DrawingHost.CurrentDrawing, p)
                {
                    NumberOfCells = convertCells(2),
                    B = 200,
                    H = 200,
                };
                    cs.CreateDimss(cs);
                }
            }
        }
        public Shopdrawing.Structures.Culvert.NumberOfCells convertCells(int i)
        {
            Shopdrawing.Structures.Culvert.NumberOfCells result = Shopdrawing.Structures.Culvert.NumberOfCells.OneCells;
            switch (i)
            {
                case 1:
                    result = Shopdrawing.Structures.Culvert.NumberOfCells.OneCells;
                    break;
                case 2:
                    result = Shopdrawing.Structures.Culvert.NumberOfCells.TwoCells;
                    break;
                case 3:
                    result = Shopdrawing.Structures.Culvert.NumberOfCells.ThreeCells;
                    break;
                case 4:
                    result = Shopdrawing.Structures.Culvert.NumberOfCells.FourCells;
                    break;
                case 5:
                    result = Shopdrawing.Structures.Culvert.NumberOfCells.FiveCells;
                    break;
                case 6:
                    result = Shopdrawing.Structures.Culvert.NumberOfCells.SixCells;
                    break;
                case 7:
                    result = Shopdrawing.Structures.Culvert.NumberOfCells.SevenCells;
                    break;
                case 8:
                    result = Shopdrawing.Structures.Culvert.NumberOfCells.EightCells;
                    break;
                case 9:
                    result = Shopdrawing.Structures.Culvert.NumberOfCells.NineCells;
                    break;
                case 10:
                    result = Shopdrawing.Structures.Culvert.NumberOfCells.TenCells;
                    break;

            }
            return result;
        }
        #endregion
        private void dockManager_DocumentClosing(object sender, DocumentClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to close the document?", "Shopdrawing", MessageBoxButton.YesNo) == MessageBoxResult.No)
                e.Cancel = true;
        }

        private void OnDumpToConsole(object sender, RoutedEventArgs e)
        {
            // Uncomment when TRACE is activated on AvalonDock project
            // dockManager.Layout.ConsoleDump(0);
        }

        private void OnReloadManager(object sender, RoutedEventArgs e)
        {
        }

        private void OnUnloadManager(object sender, RoutedEventArgs e)
        {
            if (layoutRoot.Children.Contains(dockManager))
                layoutRoot.Children.Remove(dockManager);
        }

        private void OnLoadManager(object sender, RoutedEventArgs e)
        {
            if (!layoutRoot.Children.Contains(dockManager))
                layoutRoot.Children.Add(dockManager);
        }

        private void OnToolWindow1Hiding(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to hide this tool?", "AvalonDock", MessageBoxButton.YesNo) == MessageBoxResult.No)
                e.Cancel = true;
        }


        #region test
        private void OnNotificationBar(object sender, RoutedEventArgs e)
        {      
        }
        private void OnAboutClick(object sender, RoutedEventArgs e)
        {
            //BridgeFS.Dialogs.AboutDialog about = new BridgeFS.Dialogs.AboutDialog();
            //about.ShowDialog();
            Microsoft.Expression.Framework.UserInterface.AboutDialog ab = new Microsoft.Expression.Framework.UserInterface.AboutDialog();
            ab.ShowDialog();
        }
        #endregion
    }
}
