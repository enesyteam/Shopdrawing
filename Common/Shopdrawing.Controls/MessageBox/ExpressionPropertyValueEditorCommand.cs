using System;
using System.Threading;
using System.Windows.Input;

namespace Microsoft.Expression.Framework.ValueEditors
{
    public sealed class ExpressionPropertyValueEditorCommand : ICommand
    {
        private ExpressionPropertyValueEditorCommandHandler handler;

        public ExpressionPropertyValueEditorCommand(ExpressionPropertyValueEditorCommandHandler handler)
        {
            this.handler = handler;
        }

        public void OnCanExecuteChanged()
        {
            if (this.CanExecuteChanged != null)
            {
                this.CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        bool System.Windows.Input.ICommand.CanExecute(object parameter)
        {
            return true;
        }

        void System.Windows.Input.ICommand.Execute(object parameter)
        {
            this.handler(parameter as ExpressionValueEditorCommandArgs);
        }

        public event EventHandler CanExecuteChanged;
    }
}