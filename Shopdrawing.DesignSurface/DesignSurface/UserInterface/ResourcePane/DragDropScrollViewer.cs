// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourcePane.DragDropScrollViewer
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.UserInterface.ResourcePane
{
  public class DragDropScrollViewer : ScrollViewer
  {
    private static readonly double DragInterval = 10.0;
    private static readonly double DragAcceleration = 0.0005;
    private static readonly double DragMaxVelocity = 2.0;
    private static readonly double DragInitialVelocity = 0.05;
    private static readonly double DragMargin = 40.0;
    private DispatcherTimer dragScrollTimer;
    private double dragVelocity;

    protected override void OnPreviewQueryContinueDrag(QueryContinueDragEventArgs e)
    {
      base.OnPreviewQueryContinueDrag(e);
      if (e.Action == DragAction.Cancel || e.Action == DragAction.Drop)
      {
        this.CancelDrag();
      }
      else
      {
        if (e.Action != DragAction.Continue)
          return;
        Point mousePosition = DragDropScrollViewer.MouseUtilities.GetMousePosition((Visual) this);
        if (mousePosition.Y >= DragDropScrollViewer.DragMargin && mousePosition.Y <= this.RenderSize.Height - DragDropScrollViewer.DragMargin || this.dragScrollTimer != null)
          return;
        this.dragVelocity = DragDropScrollViewer.DragInitialVelocity;
        this.dragScrollTimer = new DispatcherTimer();
        this.dragScrollTimer.Tick += new EventHandler(this.TickDragScroll);
        this.dragScrollTimer.Interval = new TimeSpan(0, 0, 0, 0, (int) DragDropScrollViewer.DragInterval);
        this.dragScrollTimer.Start();
      }
    }

    private void TickDragScroll(object sender, EventArgs e)
    {
      bool flag = true;
      if (this.IsLoaded)
      {
        Rect rect = new Rect(this.RenderSize);
        Point mousePosition = DragDropScrollViewer.MouseUtilities.GetMousePosition((Visual) this);
        if (rect.Contains(mousePosition))
        {
          if (mousePosition.Y < DragDropScrollViewer.DragMargin)
          {
            this.DragScroll(DragDropScrollViewer.DragDirection.Up);
            flag = false;
          }
          else if (mousePosition.Y > this.RenderSize.Height - DragDropScrollViewer.DragMargin)
          {
            this.DragScroll(DragDropScrollViewer.DragDirection.Down);
            flag = false;
          }
        }
      }
      if (!flag)
        return;
      this.CancelDrag();
    }

    private void CancelDrag()
    {
      if (this.dragScrollTimer == null)
        return;
      this.dragScrollTimer.Tick -= new EventHandler(this.TickDragScroll);
      this.dragScrollTimer.Stop();
      this.dragScrollTimer = (DispatcherTimer) null;
    }

    private void DragScroll(DragDropScrollViewer.DragDirection direction)
    {
      this.ScrollToVerticalOffset(Math.Max(0.0, this.VerticalOffset + (direction == DragDropScrollViewer.DragDirection.Up ? -(this.dragVelocity * DragDropScrollViewer.DragInterval) : this.dragVelocity * DragDropScrollViewer.DragInterval)));
      this.dragVelocity = Math.Min(DragDropScrollViewer.DragMaxVelocity, this.dragVelocity + DragDropScrollViewer.DragAcceleration * DragDropScrollViewer.DragInterval);
    }

    private enum DragDirection
    {
      Down,
      Up,
    }

    private static class MouseUtilities
    {
      internal static Point GetMousePosition(Visual relativeTo)
      {
        DragDropScrollViewer.MouseUtilities.NativeMethods.Win32Point pt = new DragDropScrollViewer.MouseUtilities.NativeMethods.Win32Point();
        DragDropScrollViewer.MouseUtilities.NativeMethods.GetCursorPos(ref pt);
        HwndSource hwndSource = (HwndSource) PresentationSource.FromVisual(relativeTo);
        DragDropScrollViewer.MouseUtilities.NativeMethods.ScreenToClient(hwndSource.Handle, ref pt);
        Point point = relativeTo.TransformToAncestor(hwndSource.RootVisual).Transform(new Point(0.0, 0.0));
        return new Point((double) pt.X - point.X, (double) pt.Y - point.Y);
      }

      internal static class NativeMethods
      {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref DragDropScrollViewer.MouseUtilities.NativeMethods.Win32Point pt);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ScreenToClient(IntPtr hwnd, ref DragDropScrollViewer.MouseUtilities.NativeMethods.Win32Point pt);

        internal struct Win32Point
        {
          public int X;
          public int Y;
        }
      }
    }
  }
}
