// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.BaseSizeBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using System;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal abstract class BaseSizeBehavior : ScaleBehavior
  {
    public abstract override string ActionString { get; }

    protected override bool AllowScaleAroundCenter
    {
      get
      {
        return false;
      }
    }

    protected Size StartSize { get; set; }

    public BaseSizeBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    protected override void Initialize()
    {
      this.StartSize = this.StartBounds.Size;
    }

    protected abstract override void ApplyScale(Vector scale, Point center);

    protected abstract Size ComputeSizeFromScaledDimensions(double scaledWidth, double scaledHeight);

    protected Size ComputeNewSize(Vector scale)
    {
      bool flag1 = Math.Abs(this.StartBounds.Width) < 1E-06;
      bool flag2 = Math.Abs(this.StartBounds.Height) < 1E-06;
      return this.ComputeSizeFromScaledDimensions(!flag1 || !this.ActiveAdorner.TestFlags(EdgeFlags.LeftOrRight) ? this.StartSize.Width * scale.X : this.CurrentPointerPosition.X - this.StartPointerPosition.X, !flag2 || !this.ActiveAdorner.TestFlags(EdgeFlags.TopOrBottom) ? this.StartSize.Height * scale.Y : this.CurrentPointerPosition.Y - this.StartPointerPosition.Y);
    }
  }
}
