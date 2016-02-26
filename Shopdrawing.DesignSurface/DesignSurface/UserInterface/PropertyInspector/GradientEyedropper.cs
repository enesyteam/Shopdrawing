// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.GradientEyedropper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.Framework.ValueEditors;
using Microsoft.Expression.Framework.ValueEditors.ColorEditor;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class GradientEyedropper : ColorEyedropperBase
  {
    public static readonly DependencyProperty RawGradientProperty = DependencyProperty.Register("RawGradient", typeof (LinearGradientBrush), typeof (GradientEyedropper), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null));
    private UnsafeNativeMethods.Win32Point startPoint = new UnsafeNativeMethods.Win32Point();
    private const double ErrorThreshold = 15.0;
    private bool isEditing;
    private bool isDragging;
    private bool isButtonDown;
    private bool aboutToCommit;
    private UnsafeNativeMethods.Win32Point lastPoint;
    private UnsafeNativeMethods.Win32Point endPoint;
    private BitmapSource cachedImage;

    public LinearGradientBrush RawGradient
    {
      get
      {
        return (LinearGradientBrush) this.GetValue(GradientEyedropper.RawGradientProperty);
      }
      set
      {
        this.SetValue(GradientEyedropper.RawGradientProperty, (object) value);
      }
    }

    protected override bool CenterFeedbackWindow
    {
      get
      {
        return !this.isDragging;
      }
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
      if (e.ChangedButton == MouseButton.Left)
      {
        if (this.isEditing)
        {
          this.isButtonDown = true;
          this.startPoint = this.GetPosition();
        }
        else
          this.BeginEditing();
        e.Handled = true;
      }
      else if (e.ChangedButton == MouseButton.Right && this.isEditing)
      {
        this.CancelEditing();
        e.Handled = true;
      }
      base.OnMouseDown(e);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      if (this.isButtonDown && !this.isDragging)
      {
        this.GetPosition();
        if ((double) Math.Abs(this.startPoint.x - this.endPoint.x) > SystemParameters.MinimumHorizontalDragDistance || (double) Math.Abs(this.startPoint.y - this.endPoint.y) > SystemParameters.MinimumVerticalDragDistance)
        {
          this.isDragging = true;
          this.CancelUpdateColor();
          ValueEditorUtils.ExecuteCommand(this.CancelEditCommand, (IInputElement) this, (object) null);
          ValueEditorUtils.ExecuteCommand(this.BeginEditCommand, (IInputElement) this, (object) null);
        }
      }
      base.OnMouseMove(e);
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
      if (this.isEditing && this.isButtonDown)
      {
        this.CompleteGradient();
        e.Handled = true;
      }
      base.OnMouseUp(e);
    }

    protected override void OnLostMouseCapture(MouseEventArgs e)
    {
      this.CancelEditing();
      base.OnLostMouseCapture(e);
    }

    private void CompleteGradient()
    {
      this.isButtonDown = false;
      this.aboutToCommit = true;
      this.GetPosition();
      this.ClearFeedback();
      Mouse.OverrideCursor = Cursors.Wait;
      Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Loaded, (Delegate) (o =>
      {
        if (this.isDragging)
          this.UpdateGradient(this.startPoint, this.endPoint);
        ValueEditorUtils.ExecuteCommand(this.EndEditCommand, (IInputElement) this, (object) null);
        this.FinishEditing();
        Mouse.OverrideCursor = (Cursor) null;
        return (object) null;
      }), (object) null);
    }

    private void BeginEditing()
    {
      this.isEditing = this.CaptureMouse();
      if (!this.isEditing)
        return;
      ValueEditorUtils.ExecuteCommand(this.BeginEditCommand, (IInputElement) this, (object) null);
      Mouse.OverrideCursor = ValueEditorCursors.EyedropperCursor;
      this.ShowColorEditorTrack(false);
      InputManager.Current.PostNotifyInput += new NotifyInputEventHandler(this.Current_PostNotifyInput);
      this.OpenFeedbackWindow();
    }

    protected override void CancelEditing()
    {
      if (this.aboutToCommit || !this.isEditing)
        return;
      this.isEditing = false;
      this.ClearFeedback();
      ValueEditorUtils.ExecuteCommand(this.CancelEditCommand, (IInputElement) this, (object) null);
      this.FinishEditing();
    }

    private void ClearFeedback()
    {
      if (this.FeedbackWindow != null)
        this.FeedbackWindow.Hide();
      Mouse.OverrideCursor = (Cursor) null;
      InputManager.Current.PostNotifyInput -= new NotifyInputEventHandler(this.Current_PostNotifyInput);
    }

    private void FinishEditing()
    {
      this.isButtonDown = false;
      this.isEditing = false;
      this.isDragging = false;
      this.aboutToCommit = false;
      this.ShowColorEditorTrack(true);
      this.CloseFeedbackWindow();
      this.ReleaseMouseCapture();
      ValueEditorUtils.ExecuteCommand(this.FinishEditingCommand, (IInputElement) this, (object) null);
    }

    private void Current_PostNotifyInput(object sender, NotifyInputEventArgs e)
    {
      KeyEventArgs keyEventArgs = e.StagingItem.Input as KeyEventArgs;
      if (keyEventArgs != null && keyEventArgs.Key == Key.Escape && this.isEditing)
      {
        this.CancelEditing();
      }
      else
      {
        if (this.aboutToCommit || !(e.StagingItem.Input is MouseEventArgs) || !this.isEditing)
          return;
        this.UpdateOnMouseEvent();
      }
    }

    private void UpdateOnMouseEvent()
    {
      this.endPoint = this.GetPosition();
      if (this.lastPoint.x == this.endPoint.x && this.lastPoint.y == this.endPoint.y)
        return;
      this.lastPoint = this.endPoint;
      if (!this.isDragging)
      {
        this.StartUpdateColor(this.endPoint.x, this.endPoint.y);
      }
      else
      {
        if (this.FeedbackWindow == null)
          return;
        this.SetFeedbackLine(this.startPoint, this.endPoint);
      }
    }

    private Line GetOrCreateFeedbackLine()
    {
      if (this.FeedbackWindow == null || !this.isDragging)
        return (Line) null;
      Canvas canvas = this.FeedbackWindow.Content as Canvas;
      if (canvas == null)
      {
        canvas = new Canvas();
        this.FeedbackWindow.Content = (object) canvas;
      }
      Line line = canvas.Children.Count == 0 ? (Line) null : canvas.Children[0] as Line;
      if (line == null)
      {
        line = new Line();
        line.Stroke = (Brush) new SolidColorBrush(Color.FromArgb((byte) sbyte.MinValue, (byte) 129, (byte) 169, (byte) 232));
        line.Fill = (Brush) null;
        line.StrokeThickness = 2.0;
        canvas.Children.Clear();
        canvas.Children.Add((UIElement) line);
      }
      return line;
    }

    private void SetFeedbackLine(UnsafeNativeMethods.Win32Point p1, UnsafeNativeMethods.Win32Point p2)
    {
      Line createFeedbackLine = this.GetOrCreateFeedbackLine();
      if (createFeedbackLine == null)
        return;
      double num1 = this.FeedbackWindow.Left = SystemParameters.VirtualScreenLeft;
      double num2 = this.FeedbackWindow.Top = SystemParameters.VirtualScreenTop;
      createFeedbackLine.X1 = (double) p1.x * DpiHelper.DeviceToLogicalUnitsScalingFactorX - num1;
      createFeedbackLine.Y1 = (double) p1.y * DpiHelper.DeviceToLogicalUnitsScalingFactorY - num2;
      createFeedbackLine.X2 = (double) p2.x * DpiHelper.DeviceToLogicalUnitsScalingFactorX - num1;
      createFeedbackLine.Y2 = (double) p2.y * DpiHelper.DeviceToLogicalUnitsScalingFactorY - num2;
      double num3 = Math.Max(createFeedbackLine.X1, createFeedbackLine.X2);
      double num4 = Math.Max(createFeedbackLine.Y1, createFeedbackLine.Y2);
      double num5 = 64.0;
      double num6 = Math.Ceiling(num3 / num5 + 1.0) * num5;
      double num7 = Math.Ceiling(num4 / num5 + 1.0) * num5;
      if (this.FeedbackWindow.Width < num6)
        this.FeedbackWindow.Width = num6;
      if (this.FeedbackWindow.Height >= num7)
        return;
      this.FeedbackWindow.Height = num7;
    }

    private void UpdateGradient(UnsafeNativeMethods.Win32Point start, UnsafeNativeMethods.Win32Point end)
    {
      if (start.x == end.x && start.y == end.y)
        return;
      List<UnsafeNativeMethods.Win32Point> list = this.GeneratePoints(start, end);
      List<Color> colors = new List<Color>(list.Count);
      List<double> accumulatedDistances = new List<double>(list.Count);
      double num1 = 0.0;
      this.CacheImage(start, end);
      if (this.cachedImage == null)
        return;
      int num2 = Math.Min(start.x, end.x);
      int num3 = Math.Min(start.y, end.y);
      for (int index = 0; index < list.Count; ++index)
      {
        UnsafeNativeMethods.Win32Point point = list[index];
        point.x -= num2;
        point.y -= num3;
        colors.Add(this.GetPixelColorInternal(point, this.cachedImage));
        if (index > 0)
          num1 += this.L2(list[index], list[index - 1]);
        accumulatedDistances.Add(num1);
      }
      this.cachedImage = (BitmapSource) null;
      ICollection<GradientEyedropper.GradientInterval> intervals = this.GenerateMinimalIntervalSet(colors, accumulatedDistances);
      this.RawGradient = this.ReconstructGradient(colors, accumulatedDistances, intervals);
    }

    private ICollection<GradientEyedropper.GradientInterval> GenerateMinimalIntervalSet(List<Color> colors, List<double> accumulatedDistances)
    {
      SortedDictionary<GradientEyedropper.GradientInterval, bool> intervals = new SortedDictionary<GradientEyedropper.GradientInterval, bool>((IComparer<GradientEyedropper.GradientInterval>) new GradientEyedropper.IntervalComparer(colors, accumulatedDistances));
      GradientEyedropper.GradientInterval gradientInterval = (GradientEyedropper.GradientInterval) null;
      for (int l = -1; l < colors.Count; ++l)
      {
        GradientEyedropper.GradientInterval key = new GradientEyedropper.GradientInterval(l, l + 1);
        if (gradientInterval != null)
        {
          key.Prev = gradientInterval;
          gradientInterval.Next = key;
        }
        intervals.Add(key, true);
        gradientInterval = key;
      }
      while (intervals.Count > 2)
      {
        GradientEyedropper.GradientInterval best = this.FindBest(intervals, colors, accumulatedDistances);
        if (best != null)
        {
          intervals.Remove(best);
          GradientEyedropper.GradientInterval key1 = best.Prev;
          intervals.Remove(key1);
          GradientEyedropper.GradientInterval key2 = best.Next;
          if (key2 != null)
            intervals.Remove(key2);
          GradientEyedropper.GradientInterval key3 = new GradientEyedropper.GradientInterval(key1.L, best.R);
          if (key1.Prev != null)
          {
            key3.Prev = key1.Prev;
            key1.Prev.Next = key3;
          }
          if (best.Next != null)
          {
            key3.Next = best.Next;
            best.Next.Prev = key3;
            intervals.Add(key2, true);
          }
          intervals.Add(key3, true);
        }
        else
          break;
      }
      return (ICollection<GradientEyedropper.GradientInterval>) intervals.Keys;
    }

    private LinearGradientBrush ReconstructGradient(List<Color> colors, List<double> accumulatedDistances, ICollection<GradientEyedropper.GradientInterval> intervals)
    {
      LinearGradientBrush brush = new LinearGradientBrush();
      GradientEyedropper.GradientInterval gradientInterval = (GradientEyedropper.GradientInterval) null;
      using (IEnumerator<GradientEyedropper.GradientInterval> enumerator = intervals.GetEnumerator())
      {
        if (enumerator.MoveNext())
          gradientInterval = enumerator.Current;
      }
      while (gradientInterval.Prev != null)
        gradientInterval = gradientInterval.Prev;
      if (intervals.Count == 2)
      {
        this.AddGradientStop(brush, colors, 0, accumulatedDistances);
        if (gradientInterval.R != 0 && gradientInterval.R != colors.Count - 1)
          this.AddGradientStop(brush, colors, gradientInterval.R, accumulatedDistances);
        this.AddGradientStop(brush, colors, colors.Count - 1, accumulatedDistances);
      }
      else
      {
        for (; gradientInterval != null; gradientInterval = gradientInterval.Next)
        {
          if (gradientInterval.L >= 0)
            this.AddGradientStop(brush, colors, gradientInterval.L, accumulatedDistances);
        }
      }
      return brush;
    }

    private GradientEyedropper.GradientInterval FindBest(SortedDictionary<GradientEyedropper.GradientInterval, bool> intervals, List<Color> colors, List<double> accumulatedDistances)
    {
      foreach (GradientEyedropper.GradientInterval gradientInterval in intervals.Keys)
      {
        if (gradientInterval.Prev != null && gradientInterval.GetAbsoluteError(colors, accumulatedDistances) < 15.0)
          return gradientInterval;
      }
      return (GradientEyedropper.GradientInterval) null;
    }

    [Conditional("DEBUG")]
    private void AssertSorted(SortedDictionary<GradientEyedropper.GradientInterval, bool> intervals, List<Color> colors, List<double> accumulatedDistances)
    {
      foreach (GradientEyedropper.GradientInterval gradientInterval in intervals.Keys)
        ;
    }

    private UnsafeNativeMethods.Win32Point GetPosition()
    {
      UnsafeNativeMethods.Win32Point pt = new UnsafeNativeMethods.Win32Point(0, 0);
      UnsafeNativeMethods.GetCursorPos(ref pt);
      return pt;
    }

    private void CacheImage(UnsafeNativeMethods.Win32Point start, UnsafeNativeMethods.Win32Point end)
    {
      int xSrc = Math.Min(start.x, end.x);
      int ySrc = Math.Min(start.y, end.y);
      int num1 = Math.Abs(start.x - end.x) + 1;
      int num2 = Math.Abs(start.y - end.y) + 1;
      this.cachedImage = (BitmapSource) null;
      if (num1 <= 0 || num2 <= 0)
        return;
      IntPtr dc = UnsafeNativeMethods.CreateDC("Display", (string) null, (string) null, IntPtr.Zero);
      if (!(dc != IntPtr.Zero))
        return;
      IntPtr compatibleDc = UnsafeNativeMethods.CreateCompatibleDC(dc);
      if (compatibleDc != IntPtr.Zero)
      {
        IntPtr compatibleBitmap = UnsafeNativeMethods.CreateCompatibleBitmap(dc, num1, num2);
        if (compatibleBitmap != IntPtr.Zero)
        {
          IntPtr hgdiobj = UnsafeNativeMethods.SelectObject(compatibleDc, compatibleBitmap);
          UnsafeNativeMethods.BitBlt(compatibleDc, 0, 0, num1, num2, dc, xSrc, ySrc, 13369376);
          this.cachedImage = Imaging.CreateBitmapSourceFromHBitmap(compatibleBitmap, IntPtr.Zero, new Int32Rect(0, 0, num1, num2), (BitmapSizeOptions) null);
          UnsafeNativeMethods.SelectObject(compatibleDc, hgdiobj);
          UnsafeNativeMethods.DeleteObject(compatibleBitmap);
        }
        UnsafeNativeMethods.DeleteDC(compatibleDc);
      }
      UnsafeNativeMethods.DeleteDC(dc);
    }

    private Color GetPixelColorInternal(UnsafeNativeMethods.Win32Point point, BitmapSource cachedImage)
    {
      uint[] numArray = new uint[1];
      int x = point.x;
      int y = point.y;
      cachedImage.CopyPixels(new Int32Rect(x, y, 1, 1), (Array) numArray, 4, 0);
      uint num = numArray[0];
      return Color.FromArgb(byte.MaxValue, (byte) ((num & 16711680U) >> 16), (byte) ((num & 65280U) >> 8), (byte) (num & (uint) byte.MaxValue));
    }

    private void AddGradientStop(LinearGradientBrush brush, List<Color> colors, int stopIndex, List<double> accumulatedDistances)
    {
      brush.GradientStops.Add(new GradientStop(colors[stopIndex], RoundingHelper.RoundScale(accumulatedDistances[stopIndex] / accumulatedDistances[accumulatedDistances.Count - 1])));
    }

    private static Vector3D Subtract(Color a, Color b)
    {
      return new Vector3D((double) b.R - (double) a.R, (double) b.G - (double) a.G, (double) b.B - (double) a.B);
    }

    private double L2(UnsafeNativeMethods.Win32Point a, UnsafeNativeMethods.Win32Point b)
    {
      return Math.Sqrt((double) ((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y)));
    }

    private List<UnsafeNativeMethods.Win32Point> GeneratePoints(UnsafeNativeMethods.Win32Point start, UnsafeNativeMethods.Win32Point end)
    {
      int num1 = Math.Abs(end.x - start.x);
      int num2 = Math.Abs(end.y - start.y);
      List<UnsafeNativeMethods.Win32Point> list;
      if (num2 <= num1)
      {
        int num3 = Math.Sign(end.x - start.x);
        double num4 = (double) num3 * ((double) (end.y - start.y) / (double) (end.x - start.x));
        double a = (double) start.y;
        list = new List<UnsafeNativeMethods.Win32Point>(num1 + 1);
        int x = start.x;
        while (x != end.x)
        {
          list.Add(new UnsafeNativeMethods.Win32Point(x, (int) Math.Round(a)));
          a += num4;
          x += num3;
        }
        list.Add(end);
      }
      else
      {
        int num3 = Math.Sign(end.y - start.y);
        double num4 = (double) num3 * ((double) (end.x - start.x) / (double) (end.y - start.y));
        double a = (double) start.x;
        list = new List<UnsafeNativeMethods.Win32Point>(num2 + 1);
        int y = start.y;
        while (y != end.y)
        {
          list.Add(new UnsafeNativeMethods.Win32Point((int) Math.Round(a), y));
          a += num4;
          y += num3;
        }
        list.Add(end);
      }
      return list;
    }

    private class IntervalComparer : IComparer<GradientEyedropper.GradientInterval>
    {
      private List<Color> colors;
      private List<double> accumulatedDistances;

      public IntervalComparer(List<Color> colors, List<double> accumulatedDistances)
      {
        this.colors = colors;
        this.accumulatedDistances = accumulatedDistances;
      }

      public int Compare(GradientEyedropper.GradientInterval x, GradientEyedropper.GradientInterval y)
      {
        int num = x.GetRelativeError(this.colors, this.accumulatedDistances).CompareTo(y.GetRelativeError(this.colors, this.accumulatedDistances));
        if (num == 0)
          return x.L.CompareTo(y.L);
        return num;
      }
    }

    private class GradientInterval
    {
      public int L;
      public int R;
      public GradientEyedropper.GradientInterval Prev;
      public GradientEyedropper.GradientInterval Next;

      public GradientInterval(int l, int r)
      {
        this.L = l;
        this.R = r;
      }

      public override string ToString()
      {
        return "(" + (object) this.L + ", " + (string) (object) this.R + ")";
      }

      public double GetRelativeError(List<Color> colors, List<double> accumulatedDistances)
      {
        GradientEyedropper.GradientInterval gradientInterval = this.Prev;
        if (gradientInterval == null)
          return double.MaxValue;
        bool flag1 = this.R == colors.Count;
        bool flag2 = gradientInterval.L == -1;
        double num1 = accumulatedDistances[flag1 ? colors.Count - 1 : this.R];
        double num2 = accumulatedDistances[flag2 ? 0 : gradientInterval.L];
        Vector3D vector3D1 = GradientEyedropper.Subtract(colors[flag2 ? this.L : gradientInterval.L], colors[this.L]);
        double x1 = accumulatedDistances[this.L] - num2;
        Vector3D vector3D2 = GradientEyedropper.Subtract(colors[this.L], colors[flag1 ? this.L : this.R]);
        double x2 = num1 - accumulatedDistances[this.L];
        if (flag2)
          return Math.Max(2.0 * x1 * Math.Abs(vector3D2.X) + Math.Abs(Vector.CrossProduct(new Vector(0.0, vector3D2.X), new Vector(x2, vector3D2.X))), Math.Max(2.0 * x1 * Math.Abs(vector3D2.Y) + Math.Abs(Vector.CrossProduct(new Vector(0.0, vector3D2.Y), new Vector(x2, vector3D2.Y))), 2.0 * x1 * Math.Abs(vector3D2.Z) + Math.Abs(Vector.CrossProduct(new Vector(0.0, vector3D2.Z), new Vector(x2, vector3D2.Z)))));
        if (flag1)
          return Math.Max(2.0 * x2 * Math.Abs(vector3D1.X) + Math.Abs(Vector.CrossProduct(new Vector(0.0, vector3D1.X), new Vector(x1, vector3D1.X))), Math.Max(2.0 * x2 * Math.Abs(vector3D1.Y) + Math.Abs(Vector.CrossProduct(new Vector(0.0, vector3D1.Y), new Vector(x1, vector3D1.Y))), 2.0 * x2 * Math.Abs(vector3D1.Z) + Math.Abs(Vector.CrossProduct(new Vector(0.0, vector3D1.Z), new Vector(x1, vector3D1.Z)))));
        return Math.Max(Math.Abs(Vector.CrossProduct(new Vector(-x1, -vector3D1.X), new Vector(x2, vector3D2.X))), Math.Max(Math.Abs(Vector.CrossProduct(new Vector(-x1, -vector3D1.Y), new Vector(x2, vector3D2.Y))), Math.Abs(Vector.CrossProduct(new Vector(-x1, -vector3D1.Z), new Vector(x2, vector3D2.Z)))));
      }

      public double GetAbsoluteError(List<Color> colors, List<double> accumulatedDistances)
      {
        GradientEyedropper.GradientInterval gradientInterval = this.Prev;
        double num1 = 0.0;
        bool flag1 = this.R == colors.Count;
        bool flag2 = gradientInterval.L == -1;
        Color a1 = colors[flag2 ? this.R : gradientInterval.L];
        Color b = colors[flag1 ? gradientInterval.L : this.R];
        double num2 = accumulatedDistances[flag1 ? colors.Count - 1 : this.R];
        double num3 = accumulatedDistances[flag2 ? 0 : gradientInterval.L];
        double num4 = num2 - num3;
        Vector3D vector3D1 = GradientEyedropper.Subtract(a1, b);
        for (int index = gradientInterval.L + 1; index < this.R; ++index)
        {
          Vector3D vector3D2 = vector3D1 * ((accumulatedDistances[index] - num3) / num4);
          Color a2 = Color.FromArgb(byte.MaxValue, (byte) ((double) a1.R + vector3D2.X), (byte) ((double) a1.G + vector3D2.Y), (byte) ((double) a1.B + vector3D2.Z));
          num1 = Math.Max(num1, GradientEyedropper.Subtract(a2, colors[index]).LengthSquared);
        }
        return Math.Sqrt(num1);
      }
    }
  }
}
