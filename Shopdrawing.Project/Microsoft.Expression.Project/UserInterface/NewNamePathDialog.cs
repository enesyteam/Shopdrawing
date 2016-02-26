using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Microsoft.Expression.Project.UserInterface
{
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public sealed class NewNamePathDialog : ProjectDialog, INotifyPropertyChanged, IComponentConnector
    {
        private bool showDiscardButton;

        private bool showName;

        private string newName;

        private string newPath;

        private string browseDialogTitle;

        private string browseDialogTitleVista;

        private MessageBubbleHelper projectNameMessageBubble;

        private MessageBubbleHelper projectPathMessageBubble;

        private MessageBubbleValidator<string, TextChangedEventArgs> nameValidator;

        private MessageBubbleValidator<string, TextChangedEventArgs> pathValidator;

        internal TextBox NameTextBox;

        internal TextBox PathTextBox;

        internal Button BrowseButton;

        internal Button AcceptButton;

        internal Button DiscardButton;

        internal Button CancelButton;

        private bool _contentLoaded;

        public bool InputIsValid
        {
            get
            {
                bool flag = (this.showName ? ProjectPathHelper.IsValidProjectFileName(this.NewName) : true);
                bool flag1 = PathHelper.IsValidPath(this.NewPath);
                if (flag)
                {
                    return flag1;
                }
                return false;
            }
        }

        public string NewName
        {
            get
            {
                return this.newName;
            }
            set
            {
                if (this.newName != value)
                {
                    this.newName = value;
                    this.OnPropertyChanged("NewName");
                    this.OnPropertyChanged("InputIsValid");
                }
            }
        }

        public string NewPath
        {
            get
            {
                return this.newPath;
            }
            set
            {
                if (this.newPath != value)
                {
                    this.newPath = value;
                    this.OnPropertyChanged("NewPath");
                    this.OnPropertyChanged("InputIsValid");
                }
            }
        }

        public bool ShowDiscardButton
        {
            get
            {
                return this.showDiscardButton;
            }
            set
            {
                if (value != this.showDiscardButton)
                {
                    this.showDiscardButton = value;
                    this.OnPropertyChanged("ShowDiscardButton");
                }
            }
        }

        public bool ShowName
        {
            get
            {
                return this.showName;
            }
            set
            {
                if (value != this.showName)
                {
                    this.showName = value;
                    this.OnPropertyChanged("ShowName");
                }
            }
        }

        public NewNamePathDialog()
        {
            this.InitializeComponent();
            this.showName = true;
            this.pathValidator = new MessageBubbleValidator<string, TextChangedEventArgs>(() => this.PathTextBox.Text, new Func<string, string>(PathHelper.ValidatePath));
            this.projectPathMessageBubble = new MessageBubbleHelper(this.PathTextBox, this.pathValidator);
            this.PathTextBox.TextChanged += new TextChangedEventHandler(this.pathValidator.EventHook);
            this.nameValidator = new MessageBubbleValidator<string, TextChangedEventArgs>(() => this.NameTextBox.Text, new Func<string, string>(ProjectPathHelper.ValidateProjectFileName));
            this.projectNameMessageBubble = new MessageBubbleHelper(this.NameTextBox, this.nameValidator);
            this.NameTextBox.TextChanged += new TextChangedEventHandler(this.nameValidator.EventHook);
        }

        public NewNamePathDialog(string dialogTitle, string path, string name, string browseDialogTitle, string browseDialogTitleVista)
            : this()
        {
            base.Title = dialogTitle;
            this.NewPath = path;
            this.NewName = name;
            this.browseDialogTitle = browseDialogTitle;
            this.browseDialogTitleVista = browseDialogTitleVista;
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

        internal void BrowseButton_Click(object sender, RoutedEventArgs args)
        {
            string folderPath = ProjectPathHelper.GetFolderPath(this.browseDialogTitle, this.browseDialogTitleVista, this.NewPath);
            if (!string.IsNullOrEmpty(folderPath))
            {
                this.NewPath = folderPath;
            }
        }

        internal void DiscardButton_Click(object sender, RoutedEventArgs args)
        {
            base.Result = ProjectDialog.ProjectDialogResult.Discard;
            base.Close(null);
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (this._contentLoaded)
            {
                return;
            }
            this._contentLoaded = true;
            Application.LoadComponent(this, new Uri("/Microsoft.Expression.Project;component/resources/saveprojectdialog.xaml", UriKind.Relative));
        }

        protected override void OnAcceptButtonExecute()
        {
            if (this.InputIsValid)
            {
                base.Result = ProjectDialog.ProjectDialogResult.Ok;
                base.OnAcceptButtonExecute();
            }
        }

        protected override void OnCancelButtonExecute()
        {
            base.Result = ProjectDialog.ProjectDialogResult.Cancel;
            base.OnCancelButtonExecute();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.DataContext = this;
            base.OnInitialized(e);
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
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
                        this.NameTextBox = (TextBox)target;
                        return;
                    }
                case 2:
                    {
                        this.PathTextBox = (TextBox)target;
                        return;
                    }
                case 3:
                    {
                        this.BrowseButton = (Button)target;
                        this.BrowseButton.Click += new RoutedEventHandler(this.BrowseButton_Click);
                        return;
                    }
                case 4:
                    {
                        this.AcceptButton = (Button)target;
                        return;
                    }
                case 5:
                    {
                        this.DiscardButton = (Button)target;
                        this.DiscardButton.Click += new RoutedEventHandler(this.DiscardButton_Click);
                        return;
                    }
                case 6:
                    {
                        this.CancelButton = (Button)target;
                        return;
                    }
            }
            this._contentLoaded = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}