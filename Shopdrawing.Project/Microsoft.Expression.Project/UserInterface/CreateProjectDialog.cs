using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.Licensing;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

namespace Microsoft.Expression.Project.UserInterface
{
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public sealed class CreateProjectDialog : ProjectDialog, INotifyPropertyChanged, ITreeViewItemProvider<ProjectTemplateCategoryTreeItem>, IDisposable, IComponentConnector
    {
        private readonly static double CategoryTreeWidth;

        private readonly static double LicenseStatusBarHeight;

        private bool isCategoryTreeVisible;

        private string name = string.Empty;

        private string description = string.Empty;

        private MessageBubbleValidator<string, TextChangedEventArgs> nameValidator;

        private MessageBubbleHelper projectNameMessageBubble;

        private MessageBubbleValidator<string, TextChangedEventArgs> solutionValidator;

        private MessageBubbleHelper solutionMessageBubble;

        private string defaultProjectPath;

        private string projectPath = string.Empty;

        private MessageBubbleValidator<string, TextChangedEventArgs> pathValidator;

        private MessageBubbleHelper projectPathMessageBubble;

        private IEnumerable<IProjectTemplate> creatableProjectTemplates;

        private List<string> filters;

        private SelectionTracker<ProjectPropertyValue> targetFrameworkTracker = new SelectionTracker<ProjectPropertyValue>();

        private SelectionTracker<string> filterTracker = new SelectionTracker<string>();

        private ProjectTemplateCategoryTreeItem projectTemplateCategoryTreeRoot;

        private VirtualizingTreeItemFlattener<ProjectTemplateCategoryTreeItem> projectTemplateCategoryFlattener;

        private ProjectTemplateCategoryTreeItem projectTemplateCategorySelection;

        private bool willCreateNewSolution;

        private LicenseState currentState = LicenseState.FullLicense();

        private LicenseState targetState;

        private bool licensingAllowsNewProject;

        private IServiceProvider services;

        internal Grid OuterPanel;

        internal Border LicenseStatusBarBorder;

        internal TextBlock LicenseStatusText;

        internal ListBox CategoryTree;

        internal ToggleButton CategoryTreeViewToggle;

        internal SingleSelectionListBox ProjectTemplateListBox;

        internal TextBlock DescriptionTextBlock;

        internal TextBox NameTextBox;

        internal Label PathLabel;

        internal TextBox PathTextBox;

        internal Button BrowseButton;

        internal ComboBox FilterComboBox;

        internal Label TargetFrameworkLabel;

        internal ComboBox TargetFrameworkComboBox;

        internal Button AcceptButton;

        internal Button CancelButton;

        private bool _contentLoaded;

        [CompilerGenerated]
        // CS$<>9__CachedAnonymousMethodDelegatec
        private static Func<IProjectTemplate, string> CSu0024u003cu003e9__CachedAnonymousMethodDelegatec;

        [CompilerGenerated]
        // CS$<>9__CachedAnonymousMethodDelegated
        private static Func<IProjectTemplate, string> CSu0024u003cu003e9__CachedAnonymousMethodDelegated;

        public bool CanCreateNewProject
        {
            get
            {
                if (!this.licensingAllowsNewProject)
                {
                    return false;
                }
                return this.InputIsValid;
            }
        }

        public ReadOnlyObservableCollection<ProjectTemplateCategoryTreeItem> CategoryList
        {
            get
            {
                return this.projectTemplateCategoryFlattener.ItemList;
            }
        }

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

        public Thickness DialogPadding
        {
            get
            {
                double num = (this.IsCategoryTreeVisible ? 0 : CreateProjectDialog.CategoryTreeWidth);
                return new Thickness(num, (this.targetState == null || this.targetState.FullyLicensed ? CreateProjectDialog.LicenseStatusBarHeight : 0), 0, 0);
            }
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
                    this.filterTracker.SelectedItem = value;
                }
            }
        }

        public IEnumerable<string> Filters
        {
            get
            {
                return this.filters;
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
                if (!this.willCreateNewSolution)
                {
                    return true;
                }
                return ProjectPathHelper.IsValidNewSolutionPathFileName(this.SolutionPathFileName);
            }
        }

        public bool IsCategoryTreeVisible
        {
            get
            {
                return this.isCategoryTreeVisible;
            }
            set
            {
                if (this.isCategoryTreeVisible != value)
                {
                    this.isCategoryTreeVisible = value;
                    this.OnPropertyChanged("IsCategoryTreeVisible");
                    this.OnPropertyChanged("DialogPadding");
                }
            }
        }

        public ICommand LicenseButtonCommand
        {
            get
            {
                if (this.targetState == null)
                {
                    return null;
                }
                if (!this.targetState.IsActivatable)
                {
                    return new DelegateCommand(() => this.InitializeKeyEntry());
                }
                return new DelegateCommand(() => this.InitializeActiviation());
            }
        }

        public string LicenseButtonText
        {
            get
            {
                if (this.targetState != null && !this.targetState.IsActivatable)
                {
                    return StringTable.LicenseStatusBarButtonEnterKeyText;
                }
                return StringTable.LicenseStatusBarButtonActivateText;
            }
        }

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

        public IProjectTemplate ProjectTemplate
        {
            get
            {
                return this.projectTemplateCategorySelection.FindActualTemplate(this.ProjectTemplateListBox.SelectedItem as IProjectTemplate, this.Filter);
            }
            set
            {
                if (this.ProjectTemplate != value)
                {
                    this.ProjectTemplateListBox.SelectedItem = value;
                }
            }
        }

        public IEnumerable<IProjectTemplate> ProjectTemplates
        {
            get
            {
                return this.creatableProjectTemplates;
            }
        }

        public ProjectTemplateCategoryTreeItem RootItem
        {
            get
            {
                return this.projectTemplateCategoryTreeRoot;
            }
        }

        public ICommand SelectAndCloseCommand
        {
            get
            {
                return new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.SelectAndClose));
            }
        }

        public string SelectedCategoryFullName
        {
            get
            {
                return this.projectTemplateCategorySelection.FullName;
            }
            set
            {
                if (!this.projectTemplateCategorySelection.FullName.Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    this.projectTemplateCategoryTreeRoot.SelectCategoryByFullName(value);
                }
            }
        }

        public ProjectTemplateCategoryTreeItem SelectedCategoryItem
        {
            get
            {
                return this.projectTemplateCategorySelection;
            }
            set
            {
                if (this.projectTemplateCategorySelection != value)
                {
                    this.projectTemplateCategorySelection = value;
                    this.UpdateViewFilter();
                }
            }
        }

        public string SolutionPathFileName
        {
            get
            {
                if (!this.willCreateNewSolution || this.ProjectTemplate == null || !this.ProjectTemplate.HasProjectFile)
                {
                    return null;
                }
                if (!PathHelper.IsValidPath(this.ProjectPath) || !ProjectPathHelper.IsValidProjectFileName(this.ProjectName))
                {
                    return null;
                }
                return ProjectManager.GetSolutionPathFileName(Path.Combine(this.ProjectPath, this.ProjectName), this.ProjectName);
            }
        }

        public ProjectPropertyValue TargetFrameworkVersion
        {
            get
            {
                return this.TargetFrameworkComboBox.SelectedItem as ProjectPropertyValue;
            }
            set
            {
                this.TargetFrameworkComboBox.SelectedItem = value;
                this.targetFrameworkTracker.SelectedItem = value;
            }
        }

        internal ITemplateManager TemplateManager
        {
            get;
            private set;
        }

        static CreateProjectDialog()
        {
            CreateProjectDialog.CategoryTreeWidth = 158;
            CreateProjectDialog.LicenseStatusBarHeight = 37;
        }

        public CreateProjectDialog(IEnumerable<IProjectTemplate> creatableProjectTemplates, ITemplateManager templateManager, string title, string defaultProjectPath, bool willCreateNewSolution, IServiceProvider services)
        {
            this.InitializeComponent();
            this.services = services;
            base.Style = (Style)base.Resources["CreateProjectDialogStyle"];
            this.TemplateManager = templateManager;
            this.defaultProjectPath = defaultProjectPath;
            this.projectPath = defaultProjectPath;
            this.creatableProjectTemplates = creatableProjectTemplates;
            base.Activated += new EventHandler(this.OnCreateProjectDialogActivated);
            this.willCreateNewSolution = willCreateNewSolution;
            this.pathValidator = new MessageBubbleValidator<string, TextChangedEventArgs>(() => this.PathTextBox.Text, new Func<string, string>(ProjectPathHelper.ValidatePath));
            this.projectPathMessageBubble = new MessageBubbleHelper(this.PathTextBox, this.pathValidator);
            this.PathTextBox.TextChanged += new TextChangedEventHandler(this.pathValidator.EventHook);
            this.nameValidator = new MessageBubbleValidator<string, TextChangedEventArgs>(() => this.NameTextBox.Text, new Func<string, string>(ProjectPathHelper.ValidateProjectFileName));
            this.projectNameMessageBubble = new MessageBubbleHelper(this.NameTextBox, this.nameValidator);
            this.NameTextBox.TextChanged += new TextChangedEventHandler(this.nameValidator.EventHook);
            this.solutionValidator = new MessageBubbleValidator<string, TextChangedEventArgs>(() => this.SolutionPathFileName, new Func<string, string>(ProjectPathHelper.ValidateNewSolutionPathFileName));
            this.solutionMessageBubble = new MessageBubbleHelper(this.NameTextBox, this.solutionValidator);
            this.NameTextBox.TextChanged += new TextChangedEventHandler(this.solutionValidator.EventHook);
            base.Title = title;
            base.SizeToContent = SizeToContent.Height;
            base.Width = 550;
            base.DataContext = this;
            this.projectTemplateCategoryTreeRoot = new ProjectTemplateCategoryTreeItem(string.Empty, creatableProjectTemplates, this)
            {
                IsExpanded = true
            };
            this.projectTemplateCategoryFlattener = new VirtualizingTreeItemFlattener<ProjectTemplateCategoryTreeItem>(this);
            this.projectTemplateCategorySelection = this.projectTemplateCategoryTreeRoot;
            this.IsCategoryTreeVisible = false;
            this.projectTemplateCategoryTreeRoot.IsSelected = true;
            this.UpdateLanguageList();
            this.OnPropertyChanged("LicenseButtonCommand");
            this.OnPropertyChanged("LicenseButtonText");
            this.LicenseStatusBarBorder.Visibility = (this.ProjectTemplate == null || LicensingHelper.ProjectLicense(this.ProjectTemplate, this.services).FullyLicensed ? Visibility.Collapsed : Visibility.Visible);
        }

        [DebuggerNonUserCode]
        internal Delegate _CreateDelegate(Type delegateType, string handler)
        {
            return Delegate.CreateDelegate(delegateType, this, handler);
        }

        [CompilerGenerated]
        // <.ctor>b__0
        private string u003cu002ectoru003eb__0()
        {
            return this.PathTextBox.Text;
        }

        [CompilerGenerated]
        // <.ctor>b__1
        private string u003cu002ectoru003eb__1()
        {
            return this.NameTextBox.Text;
        }

        [CompilerGenerated]
        // <.ctor>b__2
        private string u003cu002ectoru003eb__2()
        {
            return this.SolutionPathFileName;
        }

        [CompilerGenerated]
        // <get_LicenseButtonCommand>b__e
        private void u003cget_LicenseButtonCommandu003eb__e()
        {
            this.InitializeKeyEntry();
        }

        [CompilerGenerated]
        // <get_LicenseButtonCommand>b__f
        private void u003cget_LicenseButtonCommandu003eb__f()
        {
            this.InitializeActiviation();
        }

        [CompilerGenerated]
        // <UpdateLanguageList>b__8
        private bool u003cUpdateLanguageListu003eb__8(IProjectTemplate template)
        {
            if (!this.projectTemplateCategorySelection.IsTemplateAvailable(template))
            {
                return false;
            }
            return this.ProjectTemplate.TemplateID.Equals(template.TemplateID, StringComparison.OrdinalIgnoreCase);
        }

        [CompilerGenerated]
        // <UpdateLanguageList>b__9
        private static string u003cUpdateLanguageListu003eb__9(IProjectTemplate template)
        {
            return template.ProjectType;
        }

        [CompilerGenerated]
        // <UpdateLanguageList>b__a
        private static string u003cUpdateLanguageListu003eb__a(IProjectTemplate template)
        {
            return template.ProjectType;
        }

        [CompilerGenerated]
        // <UpdateViewFilter>b__6
        private bool u003cUpdateViewFilteru003eb__6(object item)
        {
            if (this.projectTemplateCategorySelection == null)
            {
                return true;
            }
            return this.projectTemplateCategorySelection.IsTemplateShown((IProjectTemplate)item, this.Filter);
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs args)
        {
            string folderPath = ProjectPathHelper.GetFolderPath(StringTable.SelectProjectFolderDialogDescription, StringTable.SelectProjectFolderDialogDescriptionVista, this.ProjectPath);
            if (!string.IsNullOrEmpty(folderPath))
            {
                this.ProjectPath = folderPath;
            }
        }

        public void Dispose()
        {
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ProjectTemplate != null)
            {
                this.Description = this.ProjectTemplate.Description;
            }
            if (e.AddedItems.Count > 0 && !string.IsNullOrEmpty(e.AddedItems[0].ToString()))
            {
                this.filterTracker.SelectedItem = e.AddedItems[0].ToString();
            }
            this.OnPropertyChanged("CanCreateNewProject");
        }

        private string GenerateLicenseBarText(LicenseState state, ProjectLicenseGroup licenseGroup)
        {
            string licenseWpfSilverlightAuthoringNeedsActivation;
            string licenseSilverlightMobileAuthoringNeedsActivation;
            string licenseSketchFlowAuthoringNeedsActivation;
            string empty = string.Empty;
            if (!state.FullyLicensed)
            {
                switch (licenseGroup)
                {
                    case ProjectLicenseGroup.WpfSilverlight:
                        {
                            if (state.IsLicensed)
                            {
                                licenseWpfSilverlightAuthoringNeedsActivation = StringTable.LicenseWpfSilverlightAuthoringNeedsActivation;
                            }
                            else if (state.IsExpired)
                            {
                                licenseWpfSilverlightAuthoringNeedsActivation = StringTable.LicenseWpfSilverlightAuthoringTrialExpired;
                            }
                            else if (state.DaysLeft == 1)
                            {
                                licenseWpfSilverlightAuthoringNeedsActivation = StringTable.LicenseWpfSilverlightAuthoringTrialDaysLeftSingular;
                            }
                            else
                            {
                                CultureInfo currentCulture = CultureInfo.CurrentCulture;
                                string licenseWpfSilverlightAuthoringTrialDaysLeftPlural = StringTable.LicenseWpfSilverlightAuthoringTrialDaysLeftPlural;
                                object[] daysLeft = new object[] { state.DaysLeft };
                                licenseWpfSilverlightAuthoringNeedsActivation = string.Format(currentCulture, licenseWpfSilverlightAuthoringTrialDaysLeftPlural, daysLeft);
                            }
                            empty = licenseWpfSilverlightAuthoringNeedsActivation;
                            break;
                        }
                    case ProjectLicenseGroup.SilverlightMobile:
                        {
                            if (state.IsLicensed)
                            {
                                licenseSilverlightMobileAuthoringNeedsActivation = StringTable.LicenseSilverlightMobileAuthoringNeedsActivation;
                            }
                            else if (state.IsExpired)
                            {
                                licenseSilverlightMobileAuthoringNeedsActivation = StringTable.LicenseSilverlightMobileAuthoringTrialExpired;
                            }
                            else if (state.DaysLeft == 1)
                            {
                                licenseSilverlightMobileAuthoringNeedsActivation = StringTable.LicenseSilverlightMobileAuthoringTrialDaysLeftSingular;
                            }
                            else
                            {
                                CultureInfo cultureInfo = CultureInfo.CurrentCulture;
                                string licenseSilverlightMobileAuthoringTrialDaysLeftPlural = StringTable.LicenseSilverlightMobileAuthoringTrialDaysLeftPlural;
                                object[] objArray = new object[] { state.DaysLeft };
                                licenseSilverlightMobileAuthoringNeedsActivation = string.Format(cultureInfo, licenseSilverlightMobileAuthoringTrialDaysLeftPlural, objArray);
                            }
                            empty = licenseSilverlightMobileAuthoringNeedsActivation;
                            break;
                        }
                    case ProjectLicenseGroup.SketchFlow:
                        {
                            if (state.IsLicensed)
                            {
                                licenseSketchFlowAuthoringNeedsActivation = StringTable.LicenseSketchFlowAuthoringNeedsActivation;
                            }
                            else if (state.IsExpired)
                            {
                                licenseSketchFlowAuthoringNeedsActivation = StringTable.LicenseSketchFlowAuthoringTrialExpired;
                            }
                            else if (state.DaysLeft == 1)
                            {
                                licenseSketchFlowAuthoringNeedsActivation = StringTable.LicenseSketchFlowAuthoringTrialDaysLeftSingular;
                            }
                            else
                            {
                                CultureInfo currentCulture1 = CultureInfo.CurrentCulture;
                                string licenseSketchFlowAuthoringTrialDaysLeftPlural = StringTable.LicenseSketchFlowAuthoringTrialDaysLeftPlural;
                                object[] daysLeft1 = new object[] { state.DaysLeft };
                                licenseSketchFlowAuthoringNeedsActivation = string.Format(currentCulture1, licenseSketchFlowAuthoringTrialDaysLeftPlural, daysLeft1);
                            }
                            empty = licenseSketchFlowAuthoringNeedsActivation;
                            break;
                        }
                }
            }
            return empty;
        }

        private void InitializeActiviation()
        {
            (new ActivateCommand((IServices)this.services)).Execute();
            this.ProjectTemplateListBox_SelectionChanged(null, null);
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (this._contentLoaded)
            {
                return;
            }
            this._contentLoaded = true;
            Application.LoadComponent(this, new Uri("/Microsoft.Expression.Project;component/resources/createprojectdialog.xaml", UriKind.Relative));
        }

        private void InitializeKeyEntry()
        {
            (new EnterProductKeyCommand((IServices)this.services)).Execute();
            this.ProjectTemplateListBox_SelectionChanged(null, null);
        }

        protected override void OnAcceptButtonExecute()
        {
            if (this.InputIsValid)
            {
                base.OnAcceptButtonExecute();
            }
        }

        private void OnCreateProjectDialogActivated(object sender, EventArgs e)
        {
            base.Activated -= new EventHandler(this.OnCreateProjectDialogActivated);
            if (!this.IsCategoryTreeVisible)
            {
                base.Left = base.Left - CreateProjectDialog.CategoryTreeWidth / 2;
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void PathTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (PathHelper.IsValidPath(this.projectPath))
            {
                if (PathHelper.IsPathRelative(this.projectPath))
                {
                    this.ProjectPath = PathHelper.ResolveRelativePath(this.defaultProjectPath, this.projectPath);
                }
                if (ProjectPathHelper.IsValidProjectFileName(this.ProjectName))
                {
                    string projectName = this.ProjectName;
                    string str = null;
                    string projectPath = this.ProjectPath;
                    this.ProjectName = PathHelper.GetAvailableFileOrDirectoryName(projectName, str, projectPath, false);
                }
            }
        }

        private void ProjectTemplateListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IProjectTemplate item;
            if (this.ProjectTemplate == null)
            {
                ICollectionView view = ((CollectionViewSource)this.OuterPanel.Resources["ProjectTemplateViewSource"]).View;
                if (view != null && !view.IsEmpty && view.CurrentItem == null)
                {
                    view.MoveCurrentToFirst();
                    return;
                }
            }
            if (this.ProjectTemplate != null)
            {
                if (e == null || e.RemovedItems.Count <= 0)
                {
                    item = null;
                }
                else
                {
                    item = e.RemovedItems[0] as IProjectTemplate;
                }
                IProjectTemplate projectTemplate = item;
                if (projectTemplate == null || this.ProjectName.StartsWith(projectTemplate.DefaultName, StringComparison.CurrentCulture))
                {
                    if (!PathHelper.IsValidPath(this.ProjectPath))
                    {
                        this.ProjectName = this.ProjectTemplate.DefaultName;
                    }
                    else
                    {
                        string defaultName = this.ProjectTemplate.DefaultName;
                        string str = null;
                        string projectPath = this.ProjectPath;
                        this.ProjectName = PathHelper.GetAvailableFileOrDirectoryName(defaultName, str, projectPath, true);
                    }
                    this.Description = this.ProjectTemplate.Description;
                }
            }
            if (this.ProjectTemplate != null)
            {
                this.UpdateStatus(LicensingHelper.GetLicenseGroup(this.ProjectTemplate));
            }
            this.OnPropertyChanged("CanCreateNewProject");
            if (this.ProjectTemplate != null)
            {
                this.TargetFrameworkVersion = this.targetFrameworkTracker.GetValidSelectedItem(this.ProjectTemplate.ValidPropertyValues("TargetFrameworkVersion"), this.ProjectTemplate.PreferredPropertyValue("TargetFrameworkVersion"));
            }
            this.UpdateLanguageList();
        }

        private void ProjectTemplateViewSource_Filter(object sender, FilterEventArgs e)
        {
            e.Accepted = (this.projectTemplateCategorySelection == null ? true : this.projectTemplateCategorySelection.IsTemplateShown((IProjectTemplate)e.Item, this.Filter));
        }

        private void SelectAndClose()
        {
            if (this.InputIsValid)
            {
                base.DialogResult = new bool?(true);
            }
        }

        [DebuggerNonUserCode]
        [EditorBrowsable(EditorBrowsableState.Never)]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    {
                        this.OuterPanel = (Grid)target;
                        return;
                    }
                case 2:
                    {
                        ((CollectionViewSource)target).Filter += new FilterEventHandler(this.ProjectTemplateViewSource_Filter);
                        return;
                    }
                case 3:
                    {
                        this.LicenseStatusBarBorder = (Border)target;
                        return;
                    }
                case 4:
                    {
                        this.LicenseStatusText = (TextBlock)target;
                        return;
                    }
                case 5:
                    {
                        this.CategoryTree = (ListBox)target;
                        return;
                    }
                case 6:
                    {
                        this.CategoryTreeViewToggle = (ToggleButton)target;
                        return;
                    }
                case 7:
                    {
                        this.ProjectTemplateListBox = (SingleSelectionListBox)target;
                        this.ProjectTemplateListBox.SelectionChanged += new SelectionChangedEventHandler(this.ProjectTemplateListBox_SelectionChanged);
                        return;
                    }
                case 8:
                    {
                        this.DescriptionTextBlock = (TextBlock)target;
                        return;
                    }
                case 9:
                    {
                        this.NameTextBox = (TextBox)target;
                        return;
                    }
                case 10:
                    {
                        this.PathLabel = (Label)target;
                        return;
                    }
                case 11:
                    {
                        this.PathTextBox = (TextBox)target;
                        this.PathTextBox.LostFocus += new RoutedEventHandler(this.PathTextBox_LostFocus);
                        return;
                    }
                case 12:
                    {
                        this.BrowseButton = (Button)target;
                        this.BrowseButton.Click += new RoutedEventHandler(this.BrowseButton_Click);
                        return;
                    }
                case 13:
                    {
                        this.FilterComboBox = (ComboBox)target;
                        this.FilterComboBox.SelectionChanged += new SelectionChangedEventHandler(this.FilterComboBox_SelectionChanged);
                        return;
                    }
                case 14:
                    {
                        this.TargetFrameworkLabel = (Label)target;
                        return;
                    }
                case 15:
                    {
                        this.TargetFrameworkComboBox = (ComboBox)target;
                        this.TargetFrameworkComboBox.SelectionChanged += new SelectionChangedEventHandler(this.TargetFrameworkComboBox_SelectionChanged);
                        return;
                    }
                case 16:
                    {
                        this.AcceptButton = (Button)target;
                        return;
                    }
                case 17:
                    {
                        this.CancelButton = (Button)target;
                        return;
                    }
            }
            this._contentLoaded = true;
        }

        private void TargetFrameworkComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.TargetFrameworkComboBox.IsKeyboardFocusWithin || this.TargetFrameworkComboBox.IsMouseCaptureWithin)
            {
                this.TargetFrameworkVersion = this.TargetFrameworkVersion;
            }
        }

        private void UpdateLanguageList()
        {
            if (this.ProjectTemplate == null)
            {
                this.filters = null;
                this.OnPropertyChanged("Filters");
                return;
            }
            List<string> list = this.ProjectTemplates.Where<IProjectTemplate>((IProjectTemplate template) =>
            {
                if (!this.projectTemplateCategorySelection.IsTemplateAvailable(template))
                {
                    return false;
                }
                return this.ProjectTemplate.TemplateID.Equals(template.TemplateID, StringComparison.OrdinalIgnoreCase);
            }).OrderByDescending<IProjectTemplate, string>((IProjectTemplate template) => template.ProjectType).Select<IProjectTemplate, string>((IProjectTemplate template) => template.ProjectType).Distinct<string>().ToList<string>();
            if (this.filters == null || !this.filters.SequenceEqual<string>(list))
            {
                this.filters = list;
                this.OnPropertyChanged("Filters");
            }
            this.Filter = this.filterTracker.GetValidSelectedItem((List<string>)this.Filters);
        }

        private void UpdateStatus(ProjectLicenseGroup licenseGroup)
        {
            LicenseState licenseState = LicensingHelper.ProjectTypeLicense(licenseGroup, this.services);
            this.currentState = this.targetState;
            this.targetState = licenseState;
            this.licensingAllowsNewProject = !licenseState.IsExpired;
            if (this.currentState != null || this.targetState != null)
            {
                this.LicenseStatusText.Text = this.GenerateLicenseBarText(licenseState, licenseGroup);
            }
            bool flag = (this.currentState == null ? false : !this.currentState.FullyLicensed);
            if (flag != (this.targetState == null ? false : !this.targetState.FullyLicensed))
            {
                if (!flag)
                {
                    this.LicenseStatusBarBorder.Visibility = Visibility.Visible;
                }
                else
                {
                    this.LicenseStatusBarBorder.Visibility = Visibility.Collapsed;
                }
            }
            this.OnPropertyChanged("CanCreateNewProject");
            this.OnPropertyChanged("DialogPadding");
            this.OnPropertyChanged("LicenseButtonText");
            this.OnPropertyChanged("LicenseButtonCommand");
        }

        private void UpdateViewFilter()
        {
            ICollectionView view = ((CollectionViewSource)this.OuterPanel.Resources["ProjectTemplateViewSource"]).View;
            if (view != null)
            {
                view.Filter = (object item) =>
                {
                    if (this.projectTemplateCategorySelection == null)
                    {
                        return true;
                    }
                    return this.projectTemplateCategorySelection.IsTemplateShown((IProjectTemplate)item, this.Filter);
                };
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}