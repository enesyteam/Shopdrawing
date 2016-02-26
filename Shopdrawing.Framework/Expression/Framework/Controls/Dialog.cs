using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Framework.ValueEditors;
using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Expression.Framework.Controls
{
    public class Dialog : ExpressionWindow
    {
        public readonly static DependencyProperty IsModalProperty;

        private bool closeOnAccept = true;

        private bool isClosed;

        private bool overrideFocus = true;

        [CompilerGenerated]
        // CS$<>9__CachedAnonymousMethodDelegate1
        private static DispatcherOperationCallback CSu0024u003cu003e9__CachedAnonymousMethodDelegate1;

        private Button AcceptButton
        {
            get
            {
                return LogicalTreeHelper.FindLogicalNode(this, "AcceptButton") as Button;
            }
        }

        public static Window ActiveModalWindow
        {
            get
            {
                if (Application.Current == null)
                {
                    return null;
                }
                Window window = Dialog.FindFirstModalDialog(Application.Current.MainWindow);
                if (window != null)
                {
                    return window;
                }
                return Application.Current.MainWindow;
            }
        }

        private Button CancelButton
        {
            get
            {
                return LogicalTreeHelper.FindLogicalNode(this, "CancelButton") as Button;
            }
        }

        public bool CloseOnAccept
        {
            get
            {
                return this.closeOnAccept;
            }
            set
            {
                this.closeOnAccept = value;
            }
        }

        public UIElement DialogContent
        {
            get
            {
                return (UIElement)base.Content;
            }
            set
            {
                base.Content = value;
                if (!this.IsOverridingWindowsChrome)
                {
                    base.ResizeMode = ResizeMode.NoResize;
                    base.WindowStyle = WindowStyle.SingleBorderWindow;
                }
            }
        }

        public bool OverrideFocus
        {
            get
            {
                return this.overrideFocus;
            }
            set
            {
                this.overrideFocus = value;
            }
        }

        public static IServiceProvider ServiceProvider
        {
            get;
            set;
        }

        static Dialog()
        {
            Dialog.IsModalProperty = DependencyProperty.RegisterAttached("IsModalProperty", typeof(bool), typeof(Window), new PropertyMetadata(false));
        }

        public Dialog()
            : this(Dialog.ActiveModalWindow)
        {
            base.Loaded += new RoutedEventHandler(this.OnLoad);
        }

        internal Dialog(Window parent)
        {
            base.WindowStartupLocation = (parent.WindowState != WindowState.Maximized ? WindowStartupLocation.CenterOwner : WindowStartupLocation.CenterScreen);
            base.ShowInTaskbar = false;
            if (parent != null)
            {
                base.Owner = parent;
            }
            base.SnapsToDevicePixels = true;
            TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);
            ValueEditorUtils.SetHandlesCommitKeys(this, false);
        }

        [CompilerGenerated]
        // <OnLoad>b__2
        private void u003cOnLoadu003eb__2()
        {
            try
            {
                if (PresentationSource.FromVisual(this) != null && this.OverrideFocus)
                {
                    this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
                }
            }
            catch (Win32Exception win32Exception)
            {
            }
        }

        [CompilerGenerated]
        // <ShowDialog>b__0
        private static object u003cShowDialogu003eb__0(object arg)
        {
            return null;
        }

        public void Close(bool? result)
        {
            if (this.isClosed)
            {
                return;
            }
            bool? dialogResult = base.DialogResult;
            bool? nullable = result;
            if ((dialogResult.GetValueOrDefault() != nullable.GetValueOrDefault() ? false : dialogResult.HasValue == nullable.HasValue))
            {
                base.Close();
                return;
            }
            base.DialogResult = result;
        }

        private static Window FindFirstModalDialog(Window ownerWindow)
        {
            Window window;
            if (ownerWindow == null)
            {
                return null;
            }
            if (ownerWindow.OwnedWindows != null && ownerWindow.OwnedWindows.Count != 0)
            {
                Window window1 = null;
                IEnumerator enumerator = ownerWindow.OwnedWindows.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        window1 = Dialog.FindFirstModalDialog((Window)enumerator.Current);
                        if (window1 == null)
                        {
                            continue;
                        }
                        window = window1;
                        return window;
                    }
                    if ((bool)ownerWindow.GetValue(Dialog.IsModalProperty) && ownerWindow.IsEnabled && ownerWindow.IsVisible && ownerWindow.IsActive)
                    {
                        return ownerWindow;
                    }
                    return null;
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
                return window;
            }
            if ((bool)ownerWindow.GetValue(Dialog.IsModalProperty) && ownerWindow.IsEnabled && ownerWindow.IsVisible && ownerWindow.IsActive)
            {
                return ownerWindow;
            }
            return null;
        }

        public static bool GetIsModalProperty(Window window)
        {
            return (bool)window.GetValue(Dialog.IsModalProperty);
        }

        protected virtual void OnAcceptButtonExecute()
        {
            if (this.closeOnAccept)
            {
                FocusManager.SetFocusedElement(this, null);
                this.Close(new bool?(true));
            }
        }

        protected virtual void OnCancelButtonExecute()
        {
            this.Close(new bool?(false));
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (!e.Cancel)
            {
                this.isClosed = true;
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            Button acceptButton = this.AcceptButton;
            if (acceptButton != null)
            {
                acceptButton.IsDefault = true;
                Dialog dialog = this;
                acceptButton.Command = new DelegateCommand(new DelegateCommand.SimpleEventHandler(dialog.OnAcceptButtonExecute));
            }
            Button cancelButton = this.CancelButton;
            if (cancelButton != null)
            {
                cancelButton.IsCancel = true;
                Dialog dialog1 = this;
                cancelButton.Command = new DelegateCommand(new DelegateCommand.SimpleEventHandler(dialog1.OnCancelButtonExecute));
            }
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            //UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.Loaded, () =>
            //{
            //    try
            //    {
            //        if (PresentationSource.FromVisual(this) != null && this.OverrideFocus)
            //        {
            //            this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            //        }
            //    }
            //    catch (Win32Exception win32Exception)
            //    {
            //    }
            //});
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            WindowHelper.UpdateWindowPlacement(this);
        }

        public static void SetIsModalProperty(Window window, bool value)
        {
            window.SetValue(Dialog.IsModalProperty, value);
        }

        public new bool? ShowDialog()
        {
            bool? dialogResult = null;
            using (IDisposable disposable = TemporaryCursor.SetCursor(Cursors.Arrow))
            {
                try
                {
                    try
                    {
                        base.SetValue(Dialog.IsModalProperty, (object)true);
                        IExternalChanges service = null;
                        if (Dialog.ServiceProvider != null)
                        {
                            service = (IExternalChanges)Dialog.ServiceProvider.GetService(typeof(IExternalChanges));
                        }
                        if (service == null)
                        {
                            dialogResult = base.ShowDialog();
                        }
                        else
                        {
                            using (IDisposable disposable1 = service.DelayNotification())
                            {
                                dialogResult = base.ShowDialog();
                            }
                        }
                    }
                    catch (Win32Exception win32Exception1)
                    {
                        Win32Exception win32Exception = win32Exception1;
                        if (win32Exception.ErrorCode != -2147467259 || (win32Exception.NativeErrorCode != 1400 || !(win32Exception.TargetSite.Name == "PostMessage")) && (win32Exception.NativeErrorCode != 87 || !(win32Exception.TargetSite.Name == "SetFocus")))
                        {
                            throw;
                        }
                        else
                        {
                            dialogResult = base.DialogResult;
                        }
                    }
                }
                finally
                {
                    base.SetValue(Dialog.IsModalProperty, (object)false);
                    if (Application.Current != null && Application.Current.MainWindow != null)
                    {
                        Application.Current.MainWindow.Dispatcher.Invoke(DispatcherPriority.Render, new TimeSpan(0, 0, 3), new DispatcherOperationCallback((object arg) => null), null);
                    }
                }
            }
            return dialogResult;
        }
    }
}