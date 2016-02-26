namespace System.Windows.Controls
{
   using System.Windows.Input;
   using System.Windows.Media;

   /// <summary>
   /// Base for a class, which wants to be provided with mouse events.
   /// </summary>
   internal class InputSubscriberBase
   {
      protected internal bool IsLeftButtonDown { get; set; }

      protected internal Point LastPositionOfMouseDown { get; set; }

      internal virtual void OnMouseDown(MouseButtonEventArgs e)
      {
      }

      internal virtual void OnMouseUp(MouseButtonEventArgs e)
      {
      }

      internal virtual void OnMouseMove(MouseEventArgs e)
      {
      }

      protected TreeViewExItem GetTreeViewItemUnderMouse(TreeViewEx treeView, Point positionRelativeToTree)
      {
         HitTestResult hitTestResult = VisualTreeHelper.HitTest(treeView, positionRelativeToTree);
         if (hitTestResult == null || hitTestResult.VisualHit == null) return null;

         TreeViewExItem item = null;
         DependencyObject currentObject = hitTestResult.VisualHit;

         while (item == null && currentObject != null)
         {
            item = currentObject as TreeViewExItem;
            currentObject = VisualTreeHelper.GetParent(currentObject);
         }

         return item;
      }
   }
}
