using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Data;
using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Expression.Framework.ValueEditors
{
    public class InlineStringEditor : StringEditor
    {
        public readonly static DependencyProperty IsSelectedProperty;

        public readonly static DependencyProperty ModeProperty;

        private bool cancelSlowClick;

        private bool selectedOnFirstDown;

        private bool receivedMouseDown;

        private int mouseDownCount;

        private Point mouseDownPoint;

        private DispatcherTimer timer;

        private DelegateCommand editCommand;

        public ICommand EditCommand
        {
            get
            {
                if (this.editCommand == null)
                {
                    this.editCommand = new DelegateCommand(() => base.IsEditing = true)
                    {
                        IsEnabled = !base.IsReadOnly
                    };
                }
                return this.editCommand;
            }
        }

        public bool IsSelected
        {
            get
            {
                return (bool)base.GetValue(InlineStringEditor.IsSelectedProperty);
            }
            set
            {
                base.SetValue(InlineStringEditor.IsSelectedProperty, value);
            }
        }

        public InlineStringEditorMode Mode
        {
            get
            {
                return (InlineStringEditorMode)base.GetValue(InlineStringEditor.ModeProperty);
            }
            set
            {
                base.SetValue(InlineStringEditor.ModeProperty, value);
            }
        }

        static InlineStringEditor()
        {
            InlineStringEditor.IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(InlineStringEditor), new FrameworkPropertyMetadata(false));
            InlineStringEditor.ModeProperty = DependencyProperty.Register("Mode", typeof(InlineStringEditorMode), typeof(InlineStringEditor), new FrameworkPropertyMetadata((object)InlineStringEditorMode.DoubleClick));
        }

        public InlineStringEditor()
        {
        }

        [CompilerGenerated]
        // <DelayedStartEditing>b__2
        private void u003cDelayedStartEditingu003eb__2(object sender, EventArgs args)
        {
            if (!this.cancelSlowClick && this.IsSelected)
            {
                base.IsEditing = true;
            }
            this.timer.Stop();
        }

        [CompilerGenerated]
        // <get_EditCommand>b__0
        private void u003cget_EditCommandu003eb__0()
        {
            base.IsEditing = true;
        }

        private bool AllowEnteringEditingMode(MouseButtonEventArgs e)
        {
            if (!this.IsSelected || e.ChangedButton != MouseButton.Left)
            {
                return false;
            }
            return Keyboard.Modifiers == ModifierKeys.None;
        }

        private bool AllowSlowClick()
        {
            if (this.Mode == InlineStringEditorMode.SlowClick)
            {
                return true;
            }
            return this.Mode == InlineStringEditorMode.DoubleClick;
        }

        private bool ClickCloseEnough(MouseButtonEventArgs e)
        {
            Vector position = this.mouseDownPoint - e.GetPosition(this);
            if (Math.Abs(position.X) < SystemParameters.MinimumHorizontalDragDistance && Math.Abs(position.Y) < SystemParameters.MinimumVerticalDragDistance)
            {
                return true;
            }
            return false;
        }

        private void DelayedStartEditing()
        {
            if (this.timer == null)
            {
                this.timer = new DispatcherTimer()
                {
                    Interval = TimeSpan.FromMilliseconds((double)NativeMethods.GetDoubleClickTime())
                };
                this.timer.Tick += new EventHandler((object sender, EventArgs args) =>
                {
                    if (!this.cancelSlowClick && this.IsSelected)
                    {
                        base.IsEditing = true;
                    }
                    this.timer.Stop();
                });
            }
            this.timer.Start();
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            if (base.IsEditing)
            {
                base.OnDragEnter(e);
            }
        }

        protected override void OnDragLeave(DragEventArgs e)
        {
            if (base.IsEditing)
            {
                base.OnDragLeave(e);
            }
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            if (base.IsEditing)
            {
                base.OnDragOver(e);
            }
        }

        protected override void OnDrop(DragEventArgs e)
        {
            if (base.IsEditing)
            {
                base.OnDrop(e);
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            base.Focusable = base.IsEditing;
        }

        protected override void OnIsEditingChanged(DependencyPropertyChangedEventArgs e)
        {
            base.Focusable = base.IsEditing;
            base.OnIsEditingChanged(e);
        }

        protected override void OnIsReadOnlyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsReadOnlyChanged(e);
            if (this.editCommand != null)
            {
                this.editCommand.IsEnabled = !base.IsReadOnly;
            }
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);
            if (base.IsEditing)
            {
                ContextMenu newFocus = e.NewFocus as ContextMenu;
                if (newFocus == null || newFocus.PlacementTarget == null || !base.IsAncestorOf(newFocus.PlacementTarget))
                {
                    base.IsEditing = false;
                }
            }
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            if (this.Mode == InlineStringEditorMode.DoubleClick && this.AllowEnteringEditingMode(e))
            {
                base.IsEditing = true;
                e.Handled = true;
            }
            base.OnMouseDoubleClick(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.receivedMouseDown = true;
                if (!base.IsEditing)
                {
                    this.mouseDownCount = e.ClickCount;
                    if (this.mouseDownCount == 1)
                    {
                        this.selectedOnFirstDown = this.IsSelected;
                        this.mouseDownPoint = e.GetPosition(this);
                    }
                    if (this.AllowSlowClick())
                    {
                        this.cancelSlowClick = (this.mouseDownCount > 1 ? true : !this.AllowEnteringEditingMode(e));
                    }
                }
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (!base.IsEditing && !base.IsReadOnly && this.receivedMouseDown)
                {
                    if (this.Mode == InlineStringEditorMode.SingleClick && this.mouseDownCount == 1 && this.selectedOnFirstDown && this.ClickCloseEnough(e) && this.AllowEnteringEditingMode(e))
                    {
                        base.IsEditing = true;
                    }
                    else if (this.AllowSlowClick() && !this.cancelSlowClick)
                    {
                        this.DelayedStartEditing();
                    }
                }
                this.receivedMouseDown = false;
            }
            base.OnMouseUp(e);
        }
    }
}