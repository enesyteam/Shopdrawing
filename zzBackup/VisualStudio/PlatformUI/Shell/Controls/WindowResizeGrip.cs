// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.WindowResizeGrip
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  public class WindowResizeGrip : Thumb
  {
    public static readonly DependencyProperty ResizeGripDirectionProperty = DependencyProperty.Register("ResizeGripDirection", typeof (WindowResizeGripDirection), typeof (WindowResizeGrip));
    public static readonly DependencyProperty ResizeTargetProperty = DependencyProperty.Register("ResizeTarget", typeof (IResizable), typeof (WindowResizeGrip));

    public WindowResizeGripDirection ResizeGripDirection
    {
      get
      {
        return (WindowResizeGripDirection) this.GetValue(WindowResizeGrip.ResizeGripDirectionProperty);
      }
      set
      {
        this.SetValue(WindowResizeGrip.ResizeGripDirectionProperty, (object) value);
      }
    }

    public IResizable ResizeTarget
    {
      get
      {
        return (IResizable) this.GetValue(WindowResizeGrip.ResizeTargetProperty);
      }
      set
      {
        this.SetValue(WindowResizeGrip.ResizeTargetProperty, (object) value);
      }
    }

    static WindowResizeGrip()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (WindowResizeGrip), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (WindowResizeGrip)));
    }

    public WindowResizeGrip()
    {
      this.AddHandler(Thumb.DragDeltaEvent, (Delegate) new DragDeltaEventHandler(this.OnDragDelta));
    }

    private void OnDragDelta(object sender, DragDeltaEventArgs args)
    {
      if (this.ResizeTarget == null)
        return;
      switch (this.ResizeGripDirection)
      {
        case WindowResizeGripDirection.Left:
          this.ResizeTarget.UpdateBounds(args.HorizontalChange, 0.0, -args.HorizontalChange, 0.0);
          break;
        case WindowResizeGripDirection.Right:
          this.ResizeTarget.UpdateBounds(0.0, 0.0, args.HorizontalChange, 0.0);
          break;
        case WindowResizeGripDirection.Top:
          this.ResizeTarget.UpdateBounds(0.0, args.VerticalChange, 0.0, -args.VerticalChange);
          break;
        case WindowResizeGripDirection.TopLeft:
          this.ResizeTarget.UpdateBounds(args.HorizontalChange, args.VerticalChange, -args.HorizontalChange, -args.VerticalChange);
          break;
        case WindowResizeGripDirection.TopRight:
          this.ResizeTarget.UpdateBounds(0.0, args.VerticalChange, args.HorizontalChange, -args.VerticalChange);
          break;
        case WindowResizeGripDirection.Bottom:
          this.ResizeTarget.UpdateBounds(0.0, 0.0, 0.0, args.VerticalChange);
          break;
        case WindowResizeGripDirection.BottomLeft:
          this.ResizeTarget.UpdateBounds(args.HorizontalChange, 0.0, -args.HorizontalChange, args.VerticalChange);
          break;
        case WindowResizeGripDirection.BottomRight:
          this.ResizeTarget.UpdateBounds(0.0, 0.0, args.HorizontalChange, args.VerticalChange);
          break;
      }
    }
  }
}
