namespace System.Windows.Controls
{
   using System;
   
   public class DragRelayCommand
   {
      #region Fields

      readonly Func<object> execute;
      readonly Func<bool> canExecute;

      #endregion // Fields

      #region Constructors

      public DragRelayCommand(Func<object> execute)
         : this(execute, null)
      {
      }

      public DragRelayCommand(Func<object> execute, Func<bool> canExecute)
      {
         if (execute == null)
            throw new ArgumentNullException("execute");

         this.execute = execute;
         this.canExecute = canExecute;
      }
      #endregion // Constructors

      public bool CanExecute()
      {
         return canExecute == null || canExecute();
      }

      public object Execute()
      {
         return execute();
      }
   }
}
