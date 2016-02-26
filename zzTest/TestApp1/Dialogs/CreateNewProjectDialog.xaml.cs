using BridgeFS.Helper;
using BridgeFS.ViewModel;
using ICSharpCode.TreeView;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace BridgeFS
{
    /// <summary>
    /// Interaction logic for CreateNewProjectDialog.xaml
    /// </summary>
    public partial class CreateNewProjectDialog : Dialog
    {
        public ProjectTreeViewModel _myTreeViewViewModel;
        public ProjectTreeViewModel MyTreeViewViewModel
        {
            get { return _myTreeViewViewModel; }
            set { _myTreeViewViewModel = value; }
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs args)
        {
            string folderPath = ProjectPathHelper.GetFolderPath("Select the location for your new project folder.", "Browse For Folder", this.ProjectPath);
            if (!string.IsNullOrEmpty(folderPath))
            {
                this.ProjectPath = folderPath;
            }
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.IsOverridingWindowsChrome)
            {
                FrameworkElement templateChild = (FrameworkElement)base.GetTemplateChild("Caption");
                if (templateChild != null)
                {
                    templateChild.MouseLeftButtonDown += new MouseButtonEventHandler(this.DoDragMove);
                }
                Button button = (Button)base.GetTemplateChild("Close");
                if (button != null)
                {
                    button.Click += new RoutedEventHandler(this.Close);
                }
            }
        }
        private void Close(object sender, EventArgs e)
        {
            base.Close();
        }

        private void DoDragMove(object sender, MouseButtonEventArgs e)
        {
            base.DragMove();
        }
        protected override void OnAcceptButtonExecute()
        {
            if (this.InputIsValid)
            {
                base.OnAcceptButtonExecute();
                var parent = this.Owner as TestApp1.MainWindow;
                parent.NewProject();
            }
        }
        private bool InputIsValid
        {
            get
            {
                if (!ProjectPathHelper.IsValidProjectFileName(this.ProjectName) || !ProjectPathHelper.IsValidPath(this.ProjectPath))
                {
                    return false;
                }
                //if (!this.willCreateNewSolution)
                //{
                //    return true;
                //}
                return ProjectPathHelper.IsValidNewSolutionPathFileName(this.SolutionPathFileName);
            }
        }
        public string SolutionPathFileName
        {
            get
            {
                //if (!this.willCreateNewSolution || this.ProjectTemplate == null || !this.ProjectTemplate.HasProjectFile)
                //{
                //    return null;
                //}
                if (!PathHelper.IsValidPath(this.ProjectPath) || !ProjectPathHelper.IsValidProjectFileName(this.ProjectName))
                {
                    return null;
                }
                return GetSolutionPathFileName(System.IO.Path.Combine(this.ProjectPath, this.ProjectName), this.ProjectName);
            }
        }
        public static string GetSolutionPathFileName(string projectPath, string projectName)
        {
            return string.Concat(System.IO.Path.Combine(projectPath, projectName), ".sln");
        }

        public ICSharpCode.TreeView.SharpTreeView ProjectTree { get; set; }
        public ICSharpCode.TreeView.SharpTreeNode ProjectNodes { get; set; }
        public CreateNewProjectDialog()
        {
            //ProjectTree = new ICSharpCode.TreeView.SharpTreeView();
            List<SharpTreeNode> nodes = new List<SharpTreeNode>();
            //ICSharpCode.TreeView.SharpTreeNode n0 = new ICSharpCode.TreeView.SharpTreeNode() { Text = "Bridges" };
            //nodes.Add(n0);

            //ProjectTree.Root = n0;
            

            //_myTreeViewViewModel = new ProjectTreeViewModel
            //{
            //    ItemsSource = nodes
            //};
            //DataContext = MyTreeViewViewModel;
            DataContext = this;
            InitializeComponent();
            this.BrowseButton.Click += new RoutedEventHandler(this.BrowseButton_Click);
        }
        public string Filter
        {
            get
            {
                return this.FilterComboBox.SelectedItem as string;
            }
            set
            {
                if (this.Filter != value)
                {
                    this.FilterComboBox.SelectedItem = value;
                }
            }
        }
        private List<string> filters;
        public IEnumerable<string> Filters
        {
            get
            {
                return this.filters;
            }
        }
        private string name = "Bridge Project";
        public string ProjectName
        {
            get
            {
                return this.name;
            }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    this.OnPropertyChanged("ProjectName");
                    this.OnPropertyChanged("CanCreateNewProject");
                }
            }
        }
        private string projectPath = @"C:\";
        public string ProjectPath
        {
            get
            {
                return this.projectPath;
            }
            set
            {
                if (this.projectPath != value)
                {
                    this.projectPath = value;
                    this.OnPropertyChanged("ProjectPath");
                    this.OnPropertyChanged("CanCreateNewProject");
                }
            }
        }
        #region PropertyChange
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region Commands
        //public ICommand SelectAndCloseCommand
        //{
        //    get
        //    {
        //        return new Microsoft.Expression.Framework.Data.DelegateCommand(new Microsoft.Expression.Framework.Data.DelegateCommand.SimpleEventHandler(this.SelectAndClose));
        //    }
        //}
        //public ICommand LicenseButtonCommand
        //{
        //    get
        //    {
        //        if (this.targetState == null)
        //        {
        //            return null;
        //        }
        //        if (!this.targetState.IsActivatable)
        //        {
        //            return new Microsoft.Expression.Framework.Data.DelegateCommand(() => this.InitializeKeyEntry());
        //        }
        //        return new Microsoft.Expression.Framework.Data.DelegateCommand(() => this.InitializeActiviation());
        //    }
        //}
        #endregion
        private ProjectDialogResult result = ProjectDialogResult.Cancel;

        internal static string[] ReservedNames;

        public ProjectDialogResult Result
        {
            get
            {
                return this.result;
            }
            protected set
            {
                this.result = value;
            }
        }

        static CreateNewProjectDialog()
        {
            string[] strArrays = new string[] { ".", "..", "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9" };
            ReservedNames = strArrays;
        }

        public enum ProjectDialogResult
        {
            Ok,
            Discard,
            Cancel
        }

        #region MyRegion
        private string description = string.Empty;
        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                if (this.name != value)
                {
                    this.description = value;
                    this.OnPropertyChanged("Description");
                }
            }
        }
        //private ProjectTemplateCategoryTreeItem projectTemplateCategorySelection;
        //public IProjectTemplate ProjectTemplate
        //{
        //    get
        //    {
        //        return this.projectTemplateCategorySelection.FindActualTemplate(this.ProjectTemplateListBox.SelectedItem as IProjectTemplate, this.Filter);
        //    }
        //    set
        //    {
        //        if (this.ProjectTemplate != value)
        //        {
        //            this.ProjectTemplateListBox.SelectedItem = value;
        //        }
        //    }
        //}
        #endregion
    }
}
