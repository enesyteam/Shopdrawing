using System;
using System.Threading;
using System.Windows.Input;

namespace Microsoft.Expression.Framework.Data
{
    public sealed class DelegateCommand : ICommand
    {
        private DelegateCommand.SimpleEventHandler handler;

        private bool isEnabled = true;

        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }
            set
            {
                this.isEnabled = value;
                this.OnCanExecuteChanged();
            }
        }

        public DelegateCommand(DelegateCommand.SimpleEventHandler handler)
        {
            this.handler = handler;
        }

        private void OnCanExecuteChanged()
        {
            if (this.CanExecuteChanged != null)
            {
                this.CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        bool System.Windows.Input.ICommand.CanExecute(object arg)
        {
            return this.IsEnabled;
        }

        void System.Windows.Input.ICommand.Execute(object arg)
        {
            this.handler();
        }

        public event EventHandler CanExecuteChanged;

        public delegate void SimpleEventHandler();
    }
}