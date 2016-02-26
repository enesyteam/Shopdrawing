using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Data;
using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

namespace Microsoft.Expression.Framework.UserInterface
{
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public sealed class MoreWorkspacesDialog : Dialog, IComponentConnector
    {
        private WorkspaceService workspaceService;

        private ObservableCollection<MoreWorkspacesDialog.WorkspaceEntry> workspaces = new ObservableCollection<MoreWorkspacesDialog.WorkspaceEntry>();

        internal MoreWorkspacesDialog MoreWorkspacesDialogRoot;

        internal Button AcceptButton;

        internal Button CancelButton;

        private bool _contentLoaded;

        public ICommand MouseDoubleClickCommand
        {
            get
            {
                MoreWorkspacesDialog moreWorkspacesDialog = this;
                return new DelegateCommand(new DelegateCommand.SimpleEventHandler(moreWorkspacesDialog.OnAcceptButtonExecute));
            }
        }

        public string SelectedWorkspace
        {
            get
            {
                MoreWorkspacesDialog.WorkspaceEntry currentItem = (MoreWorkspacesDialog.WorkspaceEntry)CollectionViewSource.GetDefaultView(this.Workspaces).CurrentItem;
                if (currentItem == null)
                {
                    return null;
                }
                return currentItem.Name;
            }
        }

        public ObservableCollection<MoreWorkspacesDialog.WorkspaceEntry> Workspaces
        {
            get
            {
                return this.workspaces;
            }
        }

        public MoreWorkspacesDialog(WorkspaceService workspaceService)
        {
            this.workspaceService = workspaceService;
            this.InitializeComponent();
            ReadOnlyCollection<string> customWorkspaceNames = this.workspaceService.CustomWorkspaceNames;
            for (int i = 0; i < customWorkspaceNames.Count; i++)
            {
                MoreWorkspacesDialog.WorkspaceEntry workspaceEntry = new MoreWorkspacesDialog.WorkspaceEntry()
                {
                    Name = customWorkspaceNames[i],
                    IsActiveWorkspace = customWorkspaceNames[i] == this.workspaceService.ActiveWorkspace.Name
                };
                this.workspaces.Add(workspaceEntry);
            }
            base.Title = StringTable.AllWorkspacesDialogTitle;
        }

        [DebuggerNonUserCode]
        internal Delegate _CreateDelegate(Type delegateType, string handler)
        {
            return Delegate.CreateDelegate(delegateType, this, handler);
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (this._contentLoaded)
            {
                return;
            }
            this._contentLoaded = true;
            Application.LoadComponent(this, new Uri("/Microsoft.Expression.Framework;component/userinterface/moreworkspacesdialog.xaml", UriKind.Relative));
        }

        [DebuggerNonUserCode]
        [EditorBrowsable(EditorBrowsableState.Never)]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    {
                        this.MoreWorkspacesDialogRoot = (MoreWorkspacesDialog)target;
                        return;
                    }
                case 2:
                    {
                        this.AcceptButton = (Button)target;
                        return;
                    }
                case 3:
                    {
                        this.CancelButton = (Button)target;
                        return;
                    }
            }
            this._contentLoaded = true;
        }

        public class WorkspaceEntry
        {
            public bool IsActiveWorkspace
            {
                get;
                set;
            }

            public string Name
            {
                get;
                set;
            }

            public WorkspaceEntry()
            {
            }
        }
    }
}