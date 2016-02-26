namespace System.Windows.Controls
{
   using System;
   using System.Collections.Generic;
   using System.Diagnostics;
   using System.Reflection;
   using System.Windows.Input;

   internal class InputEventRouter : IDisposable
   {
      private TreeViewEx treeView;

      private List<InputSubscriberBase> inputSubscribers;

      private bool isLeftMouseButtonDown;

      private Point mouseDownPoint;

      public InputEventRouter(TreeViewEx treeView)
      {
         inputSubscribers = new List<InputSubscriberBase>(2);
         this.treeView = treeView;

         treeView.MouseDown += OnMouseDown;
         treeView.MouseMove += OnMouseMove;
         treeView.MouseUp += OnMouseUp;
      }

      internal void Add(InputSubscriberBase inputSubscriber)
      {
         inputSubscribers.Add(inputSubscriber);
      }

      private void OnMouseDown(object sender, MouseButtonEventArgs e)
      {
         isLeftMouseButtonDown = true;
         mouseDownPoint = e.GetPosition(treeView);
         Call("OnMouseDown", e);
      }

      private void Call(string methodName, MouseEventArgs e)
      {
         foreach (var inputSubscriber in inputSubscribers)
         {
            // initialize provider
            inputSubscriber.IsLeftButtonDown = isLeftMouseButtonDown;
            inputSubscriber.LastPositionOfMouseDown = mouseDownPoint;

            MethodInfo methodInfo = typeof(InputSubscriberBase).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            methodInfo.Invoke(inputSubscriber, new object[] { e });

            if (e.Handled) break;
         }
      }

      private void OnMouseMove(object sender, MouseEventArgs e)
      {
         isLeftMouseButtonDown = e.LeftButton == MouseButtonState.Pressed;
         Call("OnMouseMove", e);
      }

      private void OnMouseUp(object sender, MouseButtonEventArgs e)
      {
         Call("OnMouseUp", e);
         isLeftMouseButtonDown = false;
      }

      public void Dispose()
      {
         if (treeView != null)
         {
            treeView.MouseDown -= OnMouseDown;
            treeView.MouseMove -= OnMouseMove;
            treeView.MouseUp -= OnMouseUp;

            treeView = null;
         }

         if (inputSubscribers != null)
         {
            inputSubscribers.Clear();
            inputSubscribers = null;
         }

         GC.SuppressFinalize(this);
      }
   }
}
