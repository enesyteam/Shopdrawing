using Microsoft.Expression.Framework;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Expression.Framework.ValueEditors
{
    public class StringEditor : TextBox
    {
        public readonly static DependencyProperty ValueProperty;

        public readonly static DependencyProperty IsNinchedProperty;

        public readonly static DependencyProperty IsEditingProperty;

        public readonly static DependencyProperty CornerRadiusProperty;

        public readonly static DependencyProperty BorderWidthProperty;

        public readonly static DependencyProperty BeginCommandProperty;

        public readonly static DependencyProperty CommitCommandProperty;

        public readonly static DependencyProperty UpdateCommandProperty;

        public readonly static DependencyProperty CancelCommandProperty;

        public readonly static DependencyProperty FinishEditingCommandProperty;

        public readonly static DependencyProperty LostFocusCommandProperty;

        public readonly static DependencyProperty TextChangedCommandProperty;

        private StringEditor.LostFocusAction lostFocusAction;

        private bool ignoreTextChanges;

        private bool transactionOpen;

        public ICommand BeginCommand
        {
            get
            {
                return (ICommand)base.GetValue(StringEditor.BeginCommandProperty);
            }
            set
            {
                base.SetValue(StringEditor.BeginCommandProperty, value);
            }
        }

        public double BorderWidth
        {
            get
            {
                return (double)base.GetValue(StringEditor.BorderWidthProperty);
            }
            set
            {
                base.SetValue(StringEditor.BorderWidthProperty, value);
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return (ICommand)base.GetValue(StringEditor.CancelCommandProperty);
            }
            set
            {
                base.SetValue(StringEditor.CancelCommandProperty, value);
            }
        }

        public ICommand CommitCommand
        {
            get
            {
                return (ICommand)base.GetValue(StringEditor.CommitCommandProperty);
            }
            set
            {
                base.SetValue(StringEditor.CommitCommandProperty, value);
            }
        }

        public double CornerRadius
        {
            get
            {
                return (double)base.GetValue(StringEditor.CornerRadiusProperty);
            }
            set
            {
                base.SetValue(StringEditor.CornerRadiusProperty, value);
            }
        }

        public ICommand FinishEditingCommand
        {
            get
            {
                return (ICommand)base.GetValue(StringEditor.FinishEditingCommandProperty);
            }
            set
            {
                base.SetValue(StringEditor.FinishEditingCommandProperty, value);
            }
        }

        public bool IsEditing
        {
            get
            {
                return (bool)base.GetValue(StringEditor.IsEditingProperty);
            }
            set
            {
                base.SetValue(StringEditor.IsEditingProperty, value);
            }
        }

        public bool IsNinched
        {
            get
            {
                return (bool)base.GetValue(StringEditor.IsNinchedProperty);
            }
            set
            {
                base.SetValue(StringEditor.IsNinchedProperty, value);
            }
        }

        public ICommand LostFocusCommand
        {
            get
            {
                return (ICommand)base.GetValue(StringEditor.LostFocusCommandProperty);
            }
            set
            {
                base.SetValue(StringEditor.LostFocusCommandProperty, value);
            }
        }

        public ICommand TextChangedCommand
        {
            get
            {
                return (ICommand)base.GetValue(StringEditor.TextChangedCommandProperty);
            }
            set
            {
                base.SetValue(StringEditor.TextChangedCommandProperty, value);
            }
        }

        public ICommand UpdateCommand
        {
            get
            {
                return (ICommand)base.GetValue(StringEditor.UpdateCommandProperty);
            }
            set
            {
                base.SetValue(StringEditor.UpdateCommandProperty, value);
            }
        }

        public string Value
        {
            get
            {
                return (string)base.GetValue(StringEditor.ValueProperty);
            }
            set
            {
                base.SetValue(StringEditor.ValueProperty, value);
            }
        }

        static StringEditor()
        {
            StringEditor.ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(StringEditor), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(StringEditor.ValueChanged), null, false, UpdateSourceTrigger.PropertyChanged));
            StringEditor.IsNinchedProperty = DependencyProperty.Register("IsNinched", typeof(bool), typeof(StringEditor), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(StringEditor.IsNinchedChanged)));
            StringEditor.IsEditingProperty = DependencyProperty.Register("IsEditing", typeof(bool), typeof(StringEditor), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(StringEditor.IsEditingChanged), new CoerceValueCallback(StringEditor.CoerceIsEditing)));
            StringEditor.CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(double), typeof(StringEditor), new FrameworkPropertyMetadata((object)0, FrameworkPropertyMetadataOptions.None));
            StringEditor.BorderWidthProperty = DependencyProperty.Register("BorderWidth", typeof(double), typeof(StringEditor), new FrameworkPropertyMetadata((object)1, FrameworkPropertyMetadataOptions.None));
            StringEditor.BeginCommandProperty = DependencyProperty.Register("BeginCommand", typeof(ICommand), typeof(StringEditor), new PropertyMetadata(null));
            StringEditor.CommitCommandProperty = DependencyProperty.Register("CommitCommand", typeof(ICommand), typeof(StringEditor), new PropertyMetadata(null));
            StringEditor.UpdateCommandProperty = DependencyProperty.Register("UpdateCommand", typeof(ICommand), typeof(StringEditor), new PropertyMetadata(null));
            StringEditor.CancelCommandProperty = DependencyProperty.Register("CancelCommand", typeof(ICommand), typeof(StringEditor), new PropertyMetadata(null));
            StringEditor.FinishEditingCommandProperty = DependencyProperty.Register("FinishEditingCommand", typeof(ICommand), typeof(StringEditor), new PropertyMetadata(null));
            StringEditor.LostFocusCommandProperty = DependencyProperty.Register("LostFocusCommand", typeof(ICommand), typeof(StringEditor), new PropertyMetadata(null));
            StringEditor.TextChangedCommandProperty = DependencyProperty.Register("TextChangedCommand", typeof(ICommand), typeof(StringEditor), new PropertyMetadata(null));
            TextBoxBase.IsReadOnlyProperty.OverrideMetadata(typeof(StringEditor), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(StringEditor.IsReadOnlyChanged)));
        }

        public StringEditor()
        {
        }

        private void CancelChange()
        {
            if (!this.transactionOpen)
            {
                ValueEditorUtils.ExecuteCommand(this.BeginCommand, this, null);
            }
            ValueEditorUtils.UpdateBinding(this, StringEditor.ValueProperty, false);
            ValueEditorUtils.ExecuteCommand(this.CancelCommand, this, null);
            this.transactionOpen = false;
            ValueEditorUtils.UpdateBinding(this, StringEditor.ValueProperty, UpdateBindingType.Target);
            this.UpdateTextFromValue();
        }

        private static object CoerceIsEditing(DependencyObject target, object value)
        {
            StringEditor stringEditor = target as StringEditor;
            if (stringEditor == null || !(bool)value || !stringEditor.IsReadOnly)
            {
                return value;
            }
            return false;
        }

        private void CommitChange()
        {
            if (!this.transactionOpen)
            {
                ValueEditorUtils.ExecuteCommand(this.BeginCommand, this, null);
            }
            this.Value = base.Text;
            ValueEditorUtils.ExecuteCommand(this.CommitCommand, this, null);
            this.transactionOpen = false;
            ValueEditorUtils.UpdateBinding(this, StringEditor.ValueProperty, UpdateBindingType.Target);
            this.UpdateTextFromValue();
        }

        private void InternalLostFocus()
        {
            StringEditor.LostFocusAction lostFocusAction = this.lostFocusAction;
            this.lostFocusAction = StringEditor.LostFocusAction.None;
            if (lostFocusAction == StringEditor.LostFocusAction.Commit)
            {
                this.CommitChange();
                return;
            }
            if (lostFocusAction == StringEditor.LostFocusAction.Cancel)
            {
                this.CancelChange();
            }
        }

        private static void IsEditingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StringEditor stringEditor = d as StringEditor;
            if (stringEditor != null)
            {
                stringEditor.OnIsEditingChanged(e);
            }
        }

        private static void IsNinchedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StringEditor stringEditor = d as StringEditor;
            if (stringEditor != null)
            {
                stringEditor.UpdateTextFromValue();
            }
        }

        private static void IsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StringEditor stringEditor = d as StringEditor;
            if (stringEditor != null)
            {
                stringEditor.OnIsReadOnlyChanged(e);
            }
        }

        private void OnFinishEditing()
        {
            ICommand finishEditingCommand = this.FinishEditingCommand;
            if (finishEditingCommand == null)
            {
                Keyboard.Focus(null);
                return;
            }
            ValueEditorUtils.ExecuteCommand(finishEditingCommand, this, null);
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (!base.IsReadOnly)
            {
                this.IsEditing = true;
                base.SelectAll();
            }
            base.OnGotKeyboardFocus(e);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        protected virtual void OnIsEditingChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                if (base.IsInitialized)
                {
                    base.Focus();
                    return;
                }
                this.PostFocusCallback();
            }
        }

        protected virtual void OnIsReadOnlyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.CoerceValue(StringEditor.IsEditingProperty);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            bool handlesCommitKeys = ValueEditorUtils.GetHandlesCommitKeys(this);
            if (e.Key == Key.Return || e.Key == Key.Return)
            {
                StringEditor.LostFocusAction lostFocusAction = this.lostFocusAction;
                this.lostFocusAction = StringEditor.LostFocusAction.None;
                bool modifiers = (e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.None;
                if (lostFocusAction == StringEditor.LostFocusAction.Commit)
                {
                    if (!modifiers)
                    {
                        this.UpdateChange();
                    }
                    else
                    {
                        this.CommitChange();
                    }
                }
                if (modifiers)
                {
                    this.OnFinishEditing();
                }
                KeyEventArgs handled = e;
                handled.Handled = handled.Handled | handlesCommitKeys;
            }
            else if (e.Key == Key.Escape && this.IsEditing)
            {
                StringEditor.LostFocusAction lostFocusAction1 = this.lostFocusAction;
                this.lostFocusAction = StringEditor.LostFocusAction.None;
                if (lostFocusAction1 != StringEditor.LostFocusAction.None)
                {
                    this.CancelChange();
                }
                this.OnFinishEditing();
                KeyEventArgs keyEventArg = e;
                keyEventArg.Handled = keyEventArg.Handled | handlesCommitKeys;
            }
            base.OnKeyDown(e);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            this.IsEditing = false;
            ValueEditorUtils.ExecuteCommand(this.LostFocusCommand, this, null);
            e.Handled = true;
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);
            this.InternalLostFocus();
        }

        protected override void OnPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnPreviewLostKeyboardFocus(e);
            this.InternalLostFocus();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Pen pen = null;
            double borderWidth = this.BorderWidth;
            Brush borderBrush = base.BorderBrush;
            if (borderWidth > 0 && borderBrush != null)
            {
                pen = new Pen(borderBrush, borderWidth);
            }
            RenderUtils.DrawInscribedRoundedRect(drawingContext, base.Background, pen, new Rect(0, 0, base.ActualWidth, base.ActualHeight), this.CornerRadius);
            base.OnRender(drawingContext);
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            if (this.ignoreTextChanges)
            {
                return;
            }
            ValueEditorUtils.ExecuteCommand(this.TextChangedCommand, this, base.Text);
            if (this.IsEditing)
            {
                this.lostFocusAction = StringEditor.LostFocusAction.Commit;
            }
        }

        private void PostFocusCallback()
        {
            UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.Input, new Action(this.SetFocus));
        }

        private void SetFocus()
        {
            if (base.Visibility == Visibility.Visible)
            {
                base.Focus();
            }
        }

        private void UpdateChange()
        {
            if (!this.transactionOpen)
            {
                ValueEditorUtils.ExecuteCommand(this.BeginCommand, this, null);
            }
            this.Value = base.Text;
            ValueEditorUtils.ExecuteCommand(this.UpdateCommand, this, null);
            this.transactionOpen = true;
            ValueEditorUtils.UpdateBinding(this, StringEditor.ValueProperty, UpdateBindingType.Target);
            this.UpdateTextFromValue();
        }

        private void UpdateTextFromValue()
        {
            this.ignoreTextChanges = true;
            if (this.IsNinched)
            {
                base.Text = null;
            }
            else
            {
                base.Text = this.Value;
            }
            this.ignoreTextChanges = false;
        }

        private static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StringEditor stringEditor = d as StringEditor;
            if (stringEditor != null)
            {
                stringEditor.UpdateTextFromValue();
            }
        }

        private enum LostFocusAction
        {
            None,
            Commit,
            Cancel
        }
    }
}