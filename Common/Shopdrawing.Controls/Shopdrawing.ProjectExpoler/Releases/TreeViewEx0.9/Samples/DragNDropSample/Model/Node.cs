namespace W7StyleSample.Model
{
   #region

   using System.Collections.ObjectModel;
   using System.Windows.Input;

   using TreeViewEx.SimpleSample;

   #endregion

   /// <summary>
   /// Model for testing
   /// </summary>
   public class Node
   {
      #region Constructors and Destructors

      public Node()
      {
         Children = new ObservableCollection<Node>();

         Drag = new RelayCommand(OnDrag, CanDrag);
         Drop = new RelayCommand(OnDrop, CanDrop);
      }

      public bool AllowDrop { get; set; }

      public bool AllowDrag { get; set; }

      private bool CanDrop(object obj)
      {
         return AllowDrop;
      }

      private void OnDrop(object obj)
      {

      }

      private bool CanDrag(object obj)
      {
         return AllowDrag;
      }

      private void OnDrag(object obj)
      {

      }

      #endregion

      #region Public Properties

      public ObservableCollection<Node> Children { get; set; }

      public string Name { get; set; }

      public ICommand Drag { get; private set; }

      public ICommand Drop { get; private set; }
      #endregion

      #region Public Methods

      public override string ToString()
      {
         return Name;
      }

      #endregion
   }
}