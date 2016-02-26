// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI.StoryboardPickerPopup
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Framework.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI
{
  internal class StoryboardPickerPopup : ResizablePopup
  {
    private static readonly Size MinimumSize = new Size(148.0, 222.0);
    private static readonly Size DefaultSize = new Size(438.0, 222.0);
    private static readonly double Offset = 1.0;
    private StoryboardPicker picker;

    protected override Size MinSize
    {
      get
      {
        return StoryboardPickerPopup.MinimumSize;
      }
    }

    public StoryboardPickerPopup(StoryboardPicker picker, FrameworkElement placementTarget, IConfigurationObject configuration)
      : base(new ContentControl(), configuration, "StoryboardPicker", StoryboardPickerPopup.DefaultSize)
    {
      this.picker = picker;
      this.ContentControl.Content = (object) picker;
      this.ContentControl.Focusable = false;
      this.ContentControl.IsTabStop = false;
      this.PlacementTarget = (UIElement) placementTarget;
      this.Placement = PlacementMode.Relative;
      this.HorizontalOffset = 0.0;
      this.VerticalOffset = placementTarget.ActualHeight + StoryboardPickerPopup.Offset;
      this.RedirectFocusOnOpen = false;
    }

    protected override void OnOpened(EventArgs e)
    {
      base.OnOpened(e);
      this.picker.OnOpened();
    }
  }
}
