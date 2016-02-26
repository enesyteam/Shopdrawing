// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.MessageBubble
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.UserInterface
{
  public class MessageBubble : Popup
  {
    private MessageBubble.Bubble bubble;
    private UIElement placementTarget;
    private MessageBubbleContent content;

    public MessageBubbleContent Content
    {
      get
      {
        return this.content;
      }
    }

    public MessageBubble(UIElement placementTarget, MessageBubbleContent content)
    {
      this.placementTarget = placementTarget;
      this.content = content;
    }

    public void Initialize()
    {
      this.bubble = new MessageBubble.Bubble(this.content.Message);
      Popup.CreateRootPopup((Popup) this, (UIElement) this.bubble);
      this.SnapsToDevicePixels = true;
      this.PlacementTarget = this.placementTarget;
      this.Placement = PlacementMode.Bottom;
      this.AllowsTransparency = true;
    }

    protected override void OnOpened(EventArgs e)
    {
      this.MouseDown += new MouseButtonEventHandler(this.MessageBubble_MouseDown);
      this.placementTarget.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(this.TargetElement_LostKeyboardFocus);
      base.OnOpened(e);
    }

    protected override void OnClosed(EventArgs e)
    {
      this.MouseDown -= new MouseButtonEventHandler(this.MessageBubble_MouseDown);
      this.placementTarget.LostKeyboardFocus -= new KeyboardFocusChangedEventHandler(this.TargetElement_LostKeyboardFocus);
      base.OnClosed(e);
    }

    private void MessageBubble_MouseDown(object sender, MouseButtonEventArgs e)
    {
      this.IsOpen = false;
      e.Handled = false;
    }

    private void TargetElement_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
      this.IsOpen = false;
      e.Handled = false;
    }

    private sealed class Bubble : Decorator
    {
      private static int TextMargin = 4;
      private static int TriangleHeight = MessageBubble.Bubble.TextMargin * 3;
      private static double PenThickness = 1.0;
      private static ResourceDictionary resources;
      private Pen border;
      private Brush background;
      private Point bubblePoint;

      private Point BubblePoint
      {
        get
        {
          return this.bubblePoint;
        }
        set
        {
          this.bubblePoint = value;
          FrameworkElement frameworkElement = this.Child as FrameworkElement;
          if (frameworkElement == null)
            return;
          frameworkElement.Margin = new Thickness((double) MessageBubble.Bubble.TextMargin, (double) MessageBubble.Bubble.TextMargin - value.Y, (double) MessageBubble.Bubble.TextMargin, (double) MessageBubble.Bubble.TextMargin);
        }
      }

      private double ActualBubbleHeight
      {
        get
        {
          return this.ActualHeight + this.bubblePoint.Y;
        }
      }

      public Bubble(string text)
      {
        Brush brush1 = (Brush) Brushes.White;
        Brush brush2 = (Brush) Brushes.DarkGray;
        if (MessageBubble.Bubble.resources == null)
          MessageBubble.Bubble.resources = Application.Current.Resources;
        if (MessageBubble.Bubble.resources != null)
        {
          if (MessageBubble.Bubble.resources.Contains((object) "Text1Brush"))
            brush1 = (Brush) MessageBubble.Bubble.resources[(object) "Text1Brush"];
          if (MessageBubble.Bubble.resources.Contains((object) "ToolTipBackgroundBrush"))
            brush2 = (Brush) MessageBubble.Bubble.resources[(object) "ToolTipBackgroundBrush"];
        }
        TextBlock textBlock = new TextBlock();
        textBlock.Foreground = brush1;
        textBlock.Text = text;
        this.Child = (UIElement) textBlock;
        this.background = brush2;
        this.border = new Pen(textBlock.Foreground, MessageBubble.Bubble.PenThickness);
        this.SnapsToDevicePixels = true;
      }

      protected override void OnInitialized(EventArgs e)
      {
        this.BubblePoint = new Point(0.0, (double) -MessageBubble.Bubble.TriangleHeight);
        base.OnInitialized(e);
      }

      protected override void OnRender(DrawingContext dc)
      {
        StreamGeometry streamGeometry = new StreamGeometry();
        using (StreamGeometryContext streamGeometryContext = streamGeometry.Open())
        {
          double y = this.bubblePoint.Y;
          MessageBubble.Bubble.OffsetPoint offsetPoint = new MessageBubble.Bubble.OffsetPoint(this.bubblePoint.Y);
          Rect rect = new Rect(MessageBubble.Bubble.PenThickness, MessageBubble.Bubble.PenThickness, this.ActualWidth - MessageBubble.Bubble.PenThickness - 1.0, this.ActualBubbleHeight - MessageBubble.Bubble.PenThickness - 1.0);
          streamGeometryContext.BeginFigure(offsetPoint.Get(rect.TopLeft), true, true);
          streamGeometryContext.LineTo(offsetPoint.Get((double) MessageBubble.Bubble.TriangleHeight * 0.5, rect.Top), true, true);
          streamGeometryContext.LineTo(offsetPoint.Get((double) MessageBubble.Bubble.TriangleHeight * 0.5 + this.BubblePoint.X, this.BubblePoint.Y), true, true);
          streamGeometryContext.LineTo(offsetPoint.Get((double) MessageBubble.Bubble.TriangleHeight * 1.5, rect.Top), true, true);
          streamGeometryContext.LineTo(offsetPoint.Get(rect.TopRight), true, true);
          streamGeometryContext.LineTo(offsetPoint.Get(rect.BottomRight), true, true);
          streamGeometryContext.LineTo(offsetPoint.Get(rect.BottomLeft), true, true);
        }
        dc.DrawGeometry(this.background, this.border, (Geometry) streamGeometry);
      }

      private sealed class OffsetPoint
      {
        private double offset;

        internal OffsetPoint(double offset)
        {
          this.offset = offset;
        }

        internal Point Get(double x, double y)
        {
          return new Point(x, y - this.offset);
        }

        internal Point Get(Point point)
        {
          return this.Get(point.X, point.Y);
        }
      }
    }
  }
}
