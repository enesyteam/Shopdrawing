// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Actipro.ActiproSyntaxEditorRenderer
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using ActiproSoftware.Drawing;
using ActiproSoftware.SyntaxEditor;
using ActiproSoftware.WinUICore;
using Microsoft.Expression.Code;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media;

namespace Microsoft.Expression.Code.Actipro
{
  internal sealed class ActiproSyntaxEditorRenderer : VisualStudio2005SyntaxEditorRenderer
  {
    private static string pathPrefix = "Resources\\Actipro\\";
    private static int SliderGripSize = 4;
    private static int ThumbImageSize = 18;
    private System.Drawing.Color? scrollbarBackgroundColor;
    private string themeImagePrefix;

    public ActiproSyntaxEditorRenderer(string themeImagePrefix)
    {
      this.Border = (Border) new SimpleBorder(SimpleBorderStyle.None, System.Drawing.Color.White);
      this.LineNumberMarginBorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
      this.LineNumberMarginBorderColor = System.Drawing.Color.FromArgb(204, 204, 204);
      this.themeImagePrefix = themeImagePrefix != null ? themeImagePrefix : string.Empty;
      this.InitializeColors();
    }

    private void InitializeColors()
    {
      SolidColorBrush solidColorBrush = System.Windows.Application.Current.TryFindResource((object) "ScrollbarBackgroundBrush") as SolidColorBrush;
      if (solidColorBrush == null)
        return;
      System.Windows.Media.Color color = solidColorBrush.Color;
      this.scrollbarBackgroundColor = new System.Drawing.Color?(System.Drawing.Color.FromArgb((int) color.A, (int) color.R, (int) color.G, (int) color.G));
    }

    public override void DrawScrollBarButton(PaintEventArgs e, Rectangle bounds, ActiproSoftware.WinUICore.ScrollBarButton button)
    {
      Graphics graphics = e.Graphics;
      ActiproSyntaxEditorRenderer.ScrollbarStatus scrollbarStatus = this.GetScrollbarStatus(button.GetDrawState());
      Image buttonImage = this.GetButtonImage(button, scrollbarStatus);
      try
      {
        graphics.DrawImage(buttonImage, bounds);
      }
      finally
      {
        if (buttonImage != null)
          buttonImage.Dispose();
      }
    }

    public override void DrawScrollBarThumb(PaintEventArgs e, Rectangle bounds, ScrollBarThumb thumb)
    {
      Graphics graphics = e.Graphics;
      Orientation orientation = thumb.ScrollBar.Orientation;
      ActiproSyntaxEditorRenderer.ScrollbarStatus scrollbarStatus = this.GetScrollbarStatus(thumb.GetDrawState());
      if (scrollbarStatus == ActiproSyntaxEditorRenderer.ScrollbarStatus.Disabled)
        return;
      Image thumbImage1 = this.GetThumbImage(orientation, ActiproSyntaxEditorRenderer.SliderRegion.FirstRegion, scrollbarStatus);
      Image thumbImage2 = this.GetThumbImage(orientation, ActiproSyntaxEditorRenderer.SliderRegion.MiddleRegion, scrollbarStatus);
      Image thumbImage3 = this.GetThumbImage(orientation, ActiproSyntaxEditorRenderer.SliderRegion.LastRegion, scrollbarStatus);
      if (thumb.ScrollBar.Orientation == Orientation.Horizontal)
      {
        thumbImage1.RotateFlip(RotateFlipType.Rotate180FlipX);
        thumbImage2.RotateFlip(RotateFlipType.Rotate180FlipX);
        thumbImage3.RotateFlip(RotateFlipType.Rotate180FlipX);
      }
      try
      {
        Rectangle scrollbarRegionBounds1 = this.GetScrollbarRegionBounds(bounds, orientation, ActiproSyntaxEditorRenderer.SliderRegion.FirstRegion);
        graphics.DrawImage(thumbImage1, scrollbarRegionBounds1);
        if (this.ShouldDrawMiddleRegion(bounds, orientation))
        {
          Rectangle scrollbarRegionBounds2 = this.GetScrollbarRegionBounds(bounds, orientation, ActiproSyntaxEditorRenderer.SliderRegion.MiddleRegion);
          ImageAttributes imageAttr = new ImageAttributes();
          imageAttr.SetWrapMode(WrapMode.Tile);
          graphics.DrawImage(thumbImage2, scrollbarRegionBounds2, 0, 0, thumbImage2.Width, thumbImage2.Height, GraphicsUnit.Pixel, imageAttr);
        }
        Rectangle scrollbarRegionBounds3 = this.GetScrollbarRegionBounds(bounds, orientation, ActiproSyntaxEditorRenderer.SliderRegion.LastRegion);
        graphics.DrawImage(thumbImage3, scrollbarRegionBounds3);
      }
      finally
      {
        if (thumbImage1 != null)
          thumbImage1.Dispose();
        if (thumbImage2 != null)
          thumbImage2.Dispose();
        if (thumbImage3 != null)
          thumbImage3.Dispose();
      }
    }

    public override void DrawScrollBarBackground(PaintEventArgs e, Rectangle bounds, ActiproSoftware.WinUICore.ScrollBar scrollBar)
    {
      if (this.scrollbarBackgroundColor.HasValue)
      {
        using (System.Drawing.Brush brush = (System.Drawing.Brush) new SolidBrush(this.scrollbarBackgroundColor.Value))
          e.Graphics.FillRectangle(brush, bounds);
      }
      else
        base.DrawScrollBarBackground(e, bounds, scrollBar);
    }

    public override void DrawScrollBarBlockBackground(PaintEventArgs e, Rectangle bounds, EditorView view)
    {
      Graphics graphics = e.Graphics;
      Image scrollbarBlockImage = this.GetScrollbarBlockImage();
      try
      {
        graphics.DrawImage(scrollbarBlockImage, bounds);
      }
      finally
      {
        if (scrollbarBlockImage != null)
          scrollbarBlockImage.Dispose();
      }
    }

    private bool ShouldDrawMiddleRegion(Rectangle bounds, Orientation orientation)
    {
      return orientation == Orientation.Horizontal && bounds.Width > 2 * ActiproSyntaxEditorRenderer.SliderGripSize || orientation == Orientation.Vertical && bounds.Height > 2 * ActiproSyntaxEditorRenderer.SliderGripSize;
    }

    private Rectangle GetScrollbarRegionBounds(Rectangle bounds, Orientation orientation, ActiproSyntaxEditorRenderer.SliderRegion region)
    {
      int x = -1;
      int y = -1;
      int height = -1;
      int width = -1;
      if (orientation == Orientation.Horizontal)
      {
        y = bounds.Y;
        height = ActiproSyntaxEditorRenderer.ThumbImageSize;
        switch (region)
        {
          case ActiproSyntaxEditorRenderer.SliderRegion.FirstRegion:
            x = bounds.X;
            width = ActiproSyntaxEditorRenderer.SliderGripSize;
            break;
          case ActiproSyntaxEditorRenderer.SliderRegion.MiddleRegion:
            x = bounds.X + ActiproSyntaxEditorRenderer.SliderGripSize;
            width = bounds.Width - 2 * ActiproSyntaxEditorRenderer.SliderGripSize;
            break;
          case ActiproSyntaxEditorRenderer.SliderRegion.LastRegion:
            x = bounds.X + bounds.Width - ActiproSyntaxEditorRenderer.SliderGripSize;
            width = ActiproSyntaxEditorRenderer.SliderGripSize;
            break;
        }
      }
      else
      {
        x = bounds.X;
        width = ActiproSyntaxEditorRenderer.ThumbImageSize;
        switch (region)
        {
          case ActiproSyntaxEditorRenderer.SliderRegion.FirstRegion:
            y = bounds.Y;
            height = ActiproSyntaxEditorRenderer.SliderGripSize;
            break;
          case ActiproSyntaxEditorRenderer.SliderRegion.MiddleRegion:
            y = bounds.Y + ActiproSyntaxEditorRenderer.SliderGripSize;
            height = bounds.Height - 2 * ActiproSyntaxEditorRenderer.SliderGripSize;
            break;
          case ActiproSyntaxEditorRenderer.SliderRegion.LastRegion:
            y = bounds.Y + bounds.Height - ActiproSyntaxEditorRenderer.SliderGripSize;
            height = ActiproSyntaxEditorRenderer.SliderGripSize;
            break;
        }
      }
      return new Rectangle(x, y, width, height);
    }

    private ActiproSyntaxEditorRenderer.ScrollbarStatus GetScrollbarStatus(UIElementDrawState drawState)
    {
      ActiproSyntaxEditorRenderer.ScrollbarStatus scrollbarStatus = ActiproSyntaxEditorRenderer.ScrollbarStatus.Normal;
      if ((drawState & UIElementDrawState.Pressed) == UIElementDrawState.Pressed)
        scrollbarStatus = ActiproSyntaxEditorRenderer.ScrollbarStatus.Pressed;
      else if ((drawState & UIElementDrawState.Hot) == UIElementDrawState.Hot)
        scrollbarStatus = ActiproSyntaxEditorRenderer.ScrollbarStatus.Over;
      else if ((drawState & UIElementDrawState.Disabled) == UIElementDrawState.Disabled)
        scrollbarStatus = ActiproSyntaxEditorRenderer.ScrollbarStatus.Disabled;
      return scrollbarStatus;
    }

    private Image GetScrollbarBlockImage()
    {
      return this.CreateImageFromResource(ActiproSyntaxEditorRenderer.pathPrefix + "Corner_" + this.themeImagePrefix + ".png");
    }

    private Image GetThumbImage(Orientation orientation, ActiproSyntaxEditorRenderer.SliderRegion region, ActiproSyntaxEditorRenderer.ScrollbarStatus state)
    {
      string str1 = ActiproSyntaxEditorRenderer.pathPrefix;
      string orientationPrefix = this.GetOrientationPrefix(orientation);
      string statePrefix = this.GetStatePrefix(state);
      string str2 = "";
      switch (region)
      {
        case ActiproSyntaxEditorRenderer.SliderRegion.FirstRegion:
          str2 = "First";
          break;
        case ActiproSyntaxEditorRenderer.SliderRegion.MiddleRegion:
          str2 = "Middle";
          break;
        case ActiproSyntaxEditorRenderer.SliderRegion.LastRegion:
          str2 = "Last";
          break;
      }
      return this.CreateImageFromResource(str1 + orientationPrefix + statePrefix + this.themeImagePrefix + "_" + str2 + "Slider.png");
    }

    private Image GetButtonImage(ActiproSoftware.WinUICore.ScrollBarButton button, ActiproSyntaxEditorRenderer.ScrollbarStatus state)
    {
      string str1 = ActiproSyntaxEditorRenderer.pathPrefix;
      string orientationPrefix = this.GetOrientationPrefix(button.ScrollBar.Orientation);
      string statePrefix = this.GetStatePrefix(state);
      string str2 = "";
      if (button.CommandLink.Command == button.ScrollBar.DecreaseSmallCommand)
        str2 = "Decrease";
      else if (button.CommandLink.Command == button.ScrollBar.IncreaseSmallCommand)
        str2 = "Increase";
      return this.CreateImageFromResource(str1 + orientationPrefix + statePrefix + this.themeImagePrefix + "_" + str2 + ".png");
    }

    private string GetOrientationPrefix(Orientation orientation)
    {
      switch (orientation)
      {
        case Orientation.Horizontal:
          return "H_";
        case Orientation.Vertical:
          return "V_";
        default:
          return "";
      }
    }

    private string GetStatePrefix(ActiproSyntaxEditorRenderer.ScrollbarStatus state)
    {
      switch (state)
      {
        case ActiproSyntaxEditorRenderer.ScrollbarStatus.Normal:
          return "Normal_";
        case ActiproSyntaxEditorRenderer.ScrollbarStatus.Pressed:
          return "MD_";
        case ActiproSyntaxEditorRenderer.ScrollbarStatus.Over:
          return "MO_";
        case ActiproSyntaxEditorRenderer.ScrollbarStatus.Disabled:
          return "Disabled_";
        default:
          return "";
      }
    }

    private Image CreateImageFromResource(string imagePath)
    {
      byte[] byteArray = FileTable.GetByteArray(imagePath);
      if (byteArray != null)
        return (Image) new Bitmap((Stream) new MemoryStream(byteArray));
      return (Image) null;
    }

    private enum SliderRegion
    {
      FirstRegion,
      MiddleRegion,
      LastRegion,
    }

    private enum ScrollbarStatus
    {
      Normal,
      Pressed,
      Over,
      Disabled,
    }

    private enum ScrollBarButton
    {
      First,
      Last,
    }
  }
}
