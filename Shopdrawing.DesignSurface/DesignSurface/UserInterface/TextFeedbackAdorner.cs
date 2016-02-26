// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.TextFeedbackAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.View;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public class TextFeedbackAdorner
  {
    private string text = string.Empty;
    private SceneView view;
    private DrawingContext feedbackContext;
    private TranslateTransform translateToPosition;
    private DispatcherTimer closeTimer;
    private TimeSpan closeDelay;
    private Vector offset;
    private Typeface defaultTypeface;
    private Brush defaultTextBrush;
    private Brush backgroundBrush;
    private FormattedText formattedText;

    public string Text
    {
      get
      {
        return this.text;
      }
      set
      {
        if (!(this.text != value))
          return;
        this.text = value;
        this.formattedText = new FormattedText(this.text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, this.defaultTypeface, 10.0, this.defaultTextBrush);
      }
    }

    public FormattedText FormattedText
    {
      get
      {
        return this.formattedText;
      }
      set
      {
        this.formattedText = value;
        if (this.formattedText == null)
          return;
        this.text = this.formattedText.Text;
      }
    }

    public Brush Background
    {
      get
      {
        return this.backgroundBrush;
      }
      set
      {
        this.backgroundBrush = value;
      }
    }

    public Vector Offset
    {
      get
      {
        return this.offset;
      }
      set
      {
        this.offset = value;
      }
    }

    public TimeSpan CloseDelay
    {
      get
      {
        return this.closeDelay;
      }
      set
      {
        this.closeDelay = value;
      }
    }

    public TextFeedbackAdorner(SceneView view, string text, Point position, Vector offset, TimeSpan closeDelay)
    {
      if (view == null)
        throw new ArgumentNullException("view");
      this.view = view;
      this.offset = offset;
      this.closeDelay = closeDelay;
      this.SetPosition(position);
      this.defaultTypeface = new Typeface((FontFamily) this.view.FeedbackLayer.FindResource((object) SystemFonts.IconFontFamilyKey), (FontStyle) this.view.FeedbackLayer.FindResource((object) SystemFonts.IconFontStyleKey), (FontWeight) this.view.FeedbackLayer.FindResource((object) SystemFonts.IconFontWeightKey), FontStretches.Normal);
      this.defaultTextBrush = (Brush) this.view.FeedbackLayer.FindResource((object) SystemColors.HighlightTextBrushKey);
      this.backgroundBrush = (Brush) this.view.FeedbackLayer.FindResource((object) SystemColors.HighlightBrushKey);
      this.Text = text;
    }

    public TextFeedbackAdorner(SceneView view)
      : this(view, string.Empty, new Point(0.0, 0.0), new Vector(0.0, 0.0), TimeSpan.FromSeconds(5.0))
    {
    }

    public void SetPosition(Point position)
    {
      if (this.translateToPosition == null)
      {
        this.translateToPosition = new TranslateTransform(position.X, position.Y);
      }
      else
      {
        if (this.translateToPosition.X == position.X && this.translateToPosition.Y == position.Y)
          return;
        this.translateToPosition.X = position.X;
        this.translateToPosition.Y = position.Y;
      }
    }

    public void DrawAdorner(DrawingContext context)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      if (this.formattedText == null)
        return;
      context.PushTransform((Transform) this.translateToPosition);
      Matrix matrix = new Matrix();
      matrix.ScaleAt(1.0 / this.view.Zoom, 1.0 / this.view.Zoom, 0.0, 0.0);
      MatrixTransform matrixTransform = new MatrixTransform(matrix);
      matrixTransform.Freeze();
      context.PushTransform((Transform) matrixTransform);
      TranslateTransform translateTransform = new TranslateTransform(this.offset.X, this.offset.Y);
      translateTransform.Freeze();
      context.PushTransform((Transform) translateTransform);
      Rect rectangle = new Rect(0.0, 0.0, this.formattedText.Width + 8.0, this.formattedText.Height + 2.0);
      context.DrawRoundedRectangle(this.backgroundBrush, (Pen) null, rectangle, 3.0, 3.0);
      context.DrawText(this.formattedText, new Point(4.0, 0.0));
      context.Pop();
      context.Pop();
      context.Pop();
      if (!(this.closeDelay != TimeSpan.MaxValue) || !(this.closeDelay != TimeSpan.MinValue) || !(this.closeDelay != TimeSpan.Zero))
        return;
      this.closeTimer = new DispatcherTimer(DispatcherPriority.Normal);
      this.closeTimer.Interval = this.closeDelay;
      this.closeTimer.Tick += new EventHandler(this.CloseAdornerTimer_Tick);
      this.closeTimer.Start();
    }

    public void DrawAdorner()
    {
      if (this.feedbackContext != null)
        this.CloseAdorner();
      this.OpenFeedback();
      this.DrawAdorner(this.feedbackContext);
      this.CloseFeedback();
    }

    public void CloseAdorner()
    {
      this.StopCloseTimer();
      this.ClearFeedback();
    }

    private void CloseAdornerTimer_Tick(object sender, EventArgs e)
    {
      this.CloseAdorner();
    }

    private void StopCloseTimer()
    {
      if (this.closeTimer == null)
        return;
      this.closeTimer.Stop();
      this.closeTimer = (DispatcherTimer) null;
    }

    private void OpenFeedback()
    {
      this.feedbackContext = this.view.FeedbackLayer.RenderOpen();
    }

    private void CloseFeedback()
    {
      if (this.feedbackContext == null)
        return;
      this.feedbackContext.Close();
    }

    private void ClearFeedback()
    {
      if (this.view.FeedbackLayer == null)
        return;
      this.view.FeedbackLayer.Clear();
    }
  }
}
