namespace System.Windows.Controls
{
   using System;
   using System.Diagnostics;
   using System.Windows.Input;

   public class DropRelayCommand
    {
        #region Fields

        readonly Action<string, object> execute;
        readonly Func<string, object, bool> canExecute;

        #endregion // Fields

        #region Constructors

        public DropRelayCommand(Action<string, object> execute)
            : this(execute, null)
        {
        }

        public DropRelayCommand(Action<string, object> execute, Func<string, object, bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            this.execute = execute;
            this.canExecute = canExecute;
        }
        #endregion // Constructors

        public bool CanExecute(string format, object data)
        {
            return canExecute == null || canExecute(format, data);
        }

        public void Execute(string format, object data)
        {
            execute(format, data);
        }
    }
}
