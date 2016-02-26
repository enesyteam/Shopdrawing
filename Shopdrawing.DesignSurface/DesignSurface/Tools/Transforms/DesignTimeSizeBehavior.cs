// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.DesignTimeSizeBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class DesignTimeSizeBehavior : BaseSizeBehavior
  {
    private double minWidth;
    private double minHeight;
    private double maxWidth;
    private double maxHeight;

    public override string ActionString
    {
      get
      {
        return StringTable.UndoUnitDesignTimeResize;
      }
    }

    public DesignTimeSizeBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    public static bool IsWidthEnabled(BaseFrameworkElement element)
    {
      return double.IsNaN(element.Width);
    }

    public static bool IsHeightEnabled(BaseFrameworkElement element)
    {
      return double.IsNaN(element.Height);
    }

    protected override void Initialize()
    {
      base.Initialize();
      this.minWidth = Math.Max(0.0, (double) this.BaseEditingElement.GetComputedValue(BaseFrameworkElement.MinWidthProperty));
      this.minHeight = Math.Max(0.0, (double) this.BaseEditingElement.GetComputedValue(BaseFrameworkElement.MinHeightProperty));
      this.maxWidth = (double) this.BaseEditingElement.GetComputedValue(BaseFrameworkElement.MaxWidthProperty);
      this.maxHeight = (double) this.BaseEditingElement.GetComputedValue(BaseFrameworkElement.MaxHeightProperty);
      this.ActiveView.AdornerLayer.FirePropertyChanged("HandlingDrag", (object) true);
    }

    protected override void AllDone()
    {
      base.AllDone();
      this.ActiveView.AdornerLayer.FirePropertyChanged("HandlingDrag", (object) false);
    }

    protected override Size ComputeSizeFromScaledDimensions(double scaledWidth, double scaledHeight)
    {
      return new Size(Math.Min(Math.Max(scaledWidth, this.minWidth), this.maxWidth), Math.Min(Math.Max(scaledHeight, this.minHeight), this.maxHeight));
    }

    protected override void ApplyScale(Vector scale, Point center)
    {
      Size newSize = this.ComputeNewSize(scale);
      if (!object.Equals((object) newSize.Width, (object) this.StartSize.Width) && DesignTimeSizeBehavior.IsWidthEnabled(this.BaseEditingElement))
        this.BaseEditingElement.SetValue(DesignTimeProperties.DesignWidthProperty, (object) RoundingHelper.RoundLength(newSize.Width));
      if (object.Equals((object) newSize.Height, (object) this.StartSize.Height) || !DesignTimeSizeBehavior.IsHeightEnabled(this.BaseEditingElement))
        return;
      this.BaseEditingElement.SetValue(DesignTimeProperties.DesignHeightProperty, (object) RoundingHelper.RoundLength(newSize.Height));
    }
  }
}
