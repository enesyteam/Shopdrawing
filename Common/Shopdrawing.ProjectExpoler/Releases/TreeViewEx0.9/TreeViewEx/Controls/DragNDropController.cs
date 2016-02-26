using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;

namespace System.Windows.Controls
{
   using System.Linq;

   class DragNDropController : InputSubscriberBase, IDisposable
   {
      private TreeViewEx treeViewEx;
      private bool isDraggingInProcess;

      private IList<object> draggableItems;

      public DragNDropController(TreeViewEx treeViewEx)
      {
         this.treeViewEx = treeViewEx;
         treeViewEx.AllowDrop = true;

         treeViewEx.Drop += OnDrop;
         treeViewEx.DragOver += OnDragOver;
      }

      public bool Enabled { get; set; }

      internal override void OnMouseDown(MouseButtonEventArgs e)
      {
         base.OnMouseDown(e);

         // initalize draggable items on click. Doing that in mouse move results in drag operations,
         // when the border is visible.
         draggableItems = GetDraggableItems(e.GetPosition(treeViewEx));
      }

      internal override void OnMouseMove(MouseEventArgs e)
      {
         if (!IsLeftButtonDown) return;
         
         if (draggableItems.Count > 0)
         {
            Debug.WriteLine("DragNDropController.InitializeDrag ");
            isDraggingInProcess = true;

            try
            {
               DragDrop.AddGiveFeedbackHandler(treeViewEx, OnGiveFeedBack);
               // ToDo add data provided by all nodes selected
               const DragDropEffects allowedEffects = DragDropEffects.Move | DragDropEffects.None;
               var d = DragDrop.DoDragDrop(treeViewEx, new DataObject(draggableItems), allowedEffects);
               DragDrop.RemoveGiveFeedbackHandler(treeViewEx, OnGiveFeedBack);
            }
            finally
            {
               isDraggingInProcess = false;
            }
         }

         e.Handled = isDraggingInProcess;
      }

      private void OnGiveFeedBack(object sender, GiveFeedbackEventArgs e)
      {
         e.UseDefaultCursors = false;
      }

      private void OnDrop(object sender, DragEventArgs e)
      {
         TreeViewExItem item = GetTreeViewItemUnderMouse(treeViewEx, e.GetPosition(treeViewEx));

         if (item == null || item.DropCommand == null) return;

         string format = null;
         foreach (string f in e.Data.GetFormats())
         {
            if (item.DropCommand.CanExecute(f))
            {
               format = f;
               break;
            }
         }

         if (format == null) return;

         item.DropCommand.Execute(e.Data.GetData(format));
      }

      void OnDragOver(object sender, DragEventArgs e)
      {
         // Debug.Write("OnDragOver ");
         //TreeViewExItem item = GetTreeViewItemUnderMouse(treeViewEx, e.GetPosition(treeViewEx));


         //if (item == null || item.DropCommand == null || !item.DropCommand.CanExecute(null))
         //{
         //   Debug.WriteLine("None");
         //   e.Effects = DragDropEffects.None;
         //   return;
         //}

         //Debug.WriteLine("Move");
         //e.Effects = DragDropEffects.Move;
      }

      private IList<object> GetDraggableItems(Point mousePositionRelativeToTree)
      {
         var items = treeViewEx.GetTreeViewItemsFor(treeViewEx.SelectedItems).ToList();

         foreach (var item in items)
         {
            if (item.DragCommand == null || !item.DragCommand.CanExecute(null))
            {
               // if one item is not draggable, nothing can be dragged
               return new List<object>();
            }
         }

         TreeViewExItem itemUnderMouse = GetTreeViewItemUnderMouse(treeViewEx, mousePositionRelativeToTree);

         // if no selected item is under mouse
         if (!items.Contains(itemUnderMouse)) return new List<object>();

         return treeViewEx.SelectedItems.Cast<object>().ToList();
      }

      public void Dispose()
      {
         if (treeViewEx != null)
         {
            treeViewEx.Drop -= OnDrop;
            treeViewEx.DragOver -= OnDragOver;

            treeViewEx = null;
         }
      }
   }
}
