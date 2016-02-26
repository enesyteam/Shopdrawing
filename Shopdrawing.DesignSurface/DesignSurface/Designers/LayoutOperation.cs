// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Designers.LayoutOperation
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Tools.Transforms;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using System;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Designers
{
  public class LayoutOperation
  {
    protected ILayoutDesigner Designer { get; private set; }

    protected BaseFrameworkElement Child { get; private set; }

    protected LayoutSettings Settings { get; private set; }

    protected LayoutSettings SettingsFromElement { get; private set; }

    protected LayoutOverrides ExplicitOverrides { get; private set; }

    protected LayoutOverrides LayoutOverrides { get; private set; }

    protected LayoutOverrides OverridesToIgnore { get; private set; }

    protected LayoutOverrides NonExplicitOverrides { get; private set; }

    protected SetRectMode SetRectMode { get; private set; }

    protected LayoutConstraintMode WidthConstraintMode { get; private set; }

    protected LayoutConstraintMode HeightConstraintMode { get; private set; }

    protected Rect ChildRect { get; private set; }

    protected Rect SlotRect { get; set; }

    protected virtual bool RoundBeforeSettingChildRect
    {
      get
      {
        return true;
      }
    }

    public LayoutOperation(ILayoutDesigner designer, BaseFrameworkElement child)
    {
      this.Designer = designer;
      this.Child = child;
      this.Settings = new LayoutSettings();
      this.SettingsFromElement = LayoutUtilities.GetLayoutSettingsFromElement((SceneElement) child);
      this.ExplicitOverrides = LayoutUtilities.GetLayoutOverrides((SceneElement) child);
      this.WidthConstraintMode = designer.GetWidthConstraintMode(child);
      this.HeightConstraintMode = designer.GetHeightConstraintMode(child);
    }

    public LayoutOverrides ComputeOverrides(Rect rect)
    {
      if (this.Child.Visual == null)
        return LayoutOverrides.None;
      this.Settings.LayoutOverrides = this.SettingsFromElement.LayoutOverrides;
      this.ChildRect = rect;
      this.ComputeIdealSlotRect();
      this.ComputeSlotRectOverrides();
      this.UpdateChildRectWithinSlot();
      this.ComputeIdealAlignment();
      this.ComputeAlignmentOverrides();
      this.ComputeIdealSize();
      this.ComputeSizeOverrides();
      this.ComputeIdealMargin();
      this.ComputeMarginOverrides();
      return this.Settings.LayoutOverrides;
    }

    public void SetRect(Rect rect, LayoutOverrides layoutOverrides, LayoutOverrides overridesToIgnore, LayoutOverrides nonExplicitOverrides, SetRectMode setRectMode)
    {
      if (this.Child.Visual == null)
        return;
      this.LayoutOverrides = layoutOverrides;
      this.OverridesToIgnore = overridesToIgnore;
      this.NonExplicitOverrides = nonExplicitOverrides;
      this.SetRectMode = setRectMode;
      LayoutOverrides explicitOverrides = this.ExplicitOverrides;
      if (this.RoundBeforeSettingChildRect)
      {
        double num1 = (this.LayoutOverrides & LayoutOverrides.Width) == LayoutOverrides.None || (this.OverridesToIgnore & LayoutOverrides.Width) != LayoutOverrides.None ? 0.0 : 0.00045;
        double num2 = (this.LayoutOverrides & LayoutOverrides.Height) == LayoutOverrides.None || (this.OverridesToIgnore & LayoutOverrides.Height) != LayoutOverrides.None ? 0.0 : 0.00045;
        double x = RoundingHelper.RoundLength(rect.Left - num1);
        double y = RoundingHelper.RoundLength(rect.Top - num2);
        double num3 = RoundingHelper.RoundLength(rect.Right + num1);
        double num4 = RoundingHelper.RoundLength(rect.Bottom + num2);
        rect = new Rect(x, y, RoundingHelper.RoundLength(num3 - x), RoundingHelper.RoundLength(num4 - y));
      }
      this.ChildRect = rect;
      this.ComputeIdealSlotRect();
      this.SetSlotRectChanges();
      this.UpdateChildRectWithinSlot();
      this.ComputeIdealAlignment();
      this.SetAlignmentChanges();
      this.ComputeIdealSize();
      this.SetSizeChanges();
      this.ComputeIdealMargin();
      this.SetMarginChanges();
      this.ExplicitOverrides &= ~this.NonExplicitOverrides;
      if (this.ExplicitOverrides == explicitOverrides)
        return;
      LayoutUtilities.SetLayoutOverrides((SceneElement) this.Child, this.ExplicitOverrides);
    }

    protected virtual void ComputeIdealSlotRect()
    {
      if (this.Child.Visual is IViewVisual)
        this.SlotRect = ((IViewVisual) this.Child.Visual).GetLayoutSlot();
      else
        this.SlotRect = Rect.Empty;
    }

    protected virtual void ComputeSlotRectOverrides()
    {
    }

    protected virtual void SetSlotRectChanges()
    {
    }

    private void UpdateChildRectWithinSlot()
    {
      if (this.SetRectMode == SetRectMode.CreateDefault)
      {
        if (this.WidthConstraintMode == LayoutConstraintMode.NonOverlappingGridlike)
        {
          this.ChildRect = new Rect(this.SlotRect.Left, this.ChildRect.Top, this.SlotRect.Width, this.ChildRect.Height);
          this.LayoutOverrides &= ~LayoutOverrides.Width;
        }
        if (this.HeightConstraintMode != LayoutConstraintMode.NonOverlappingGridlike)
          return;
        this.ChildRect = new Rect(this.ChildRect.Left, this.SlotRect.Top, this.ChildRect.Width, this.SlotRect.Height);
        this.LayoutOverrides &= ~LayoutOverrides.Height;
      }
      else if (this.SetRectMode == SetRectMode.CreateAtPosition)
      {
        if ((this.WidthConstraintMode & LayoutConstraintMode.Overlapping) == LayoutConstraintMode.NonOverlappingGridlike)
        {
          if ((this.WidthConstraintMode & LayoutConstraintMode.CanvasLike) == LayoutConstraintMode.NonOverlappingGridlike && this.ChildRect.Right > this.SlotRect.Right)
            this.ChildRect = new Rect(this.ChildRect.Left + this.SlotRect.Right - this.ChildRect.Right, this.ChildRect.Top, this.ChildRect.Width, this.ChildRect.Height);
          if (this.ChildRect.Left < this.SlotRect.Left)
            this.ChildRect = new Rect(this.SlotRect.Left, this.ChildRect.Top, this.ChildRect.Width, this.ChildRect.Height);
        }
        if ((this.HeightConstraintMode & LayoutConstraintMode.Overlapping) != LayoutConstraintMode.NonOverlappingGridlike)
          return;
        if ((this.HeightConstraintMode & LayoutConstraintMode.CanvasLike) == LayoutConstraintMode.NonOverlappingGridlike && this.ChildRect.Bottom > this.SlotRect.Bottom)
          this.ChildRect = new Rect(this.ChildRect.Left, this.ChildRect.Top + this.SlotRect.Bottom - this.ChildRect.Bottom, this.ChildRect.Width, this.ChildRect.Height);
        if (this.ChildRect.Top >= this.SlotRect.Top)
          return;
        this.ChildRect = new Rect(this.ChildRect.Left, this.SlotRect.Top, this.ChildRect.Width, this.ChildRect.Height);
      }
      else
      {
        if (this.SetRectMode != SetRectMode.CreateAtSlotPosition)
          return;
        if ((this.WidthConstraintMode & LayoutConstraintMode.Overlapping) == LayoutConstraintMode.NonOverlappingGridlike)
          this.ChildRect = new Rect(this.SlotRect.Left + this.ChildRect.Left, this.ChildRect.Top, this.ChildRect.Width, this.ChildRect.Height);
        if ((this.HeightConstraintMode & LayoutConstraintMode.Overlapping) != LayoutConstraintMode.NonOverlappingGridlike)
          return;
        this.ChildRect = new Rect(this.ChildRect.Left, this.SlotRect.Top + this.ChildRect.Top, this.ChildRect.Width, this.ChildRect.Height);
      }
    }

    protected virtual void ComputeIdealAlignment()
    {
      this.Settings.HorizontalAlignment = (this.WidthConstraintMode & LayoutConstraintMode.CanvasLike) == LayoutConstraintMode.NonOverlappingGridlike ? (!Tolerances.LessThanOrClose(this.ChildRect.Right, (this.SlotRect.Left + this.SlotRect.Right) / 2.0) ? (!Tolerances.GreaterThanOrClose(this.ChildRect.Left, (this.SlotRect.Left + this.SlotRect.Right) / 2.0) ? HorizontalAlignment.Stretch : HorizontalAlignment.Right) : HorizontalAlignment.Left) : HorizontalAlignment.Stretch;
      if ((this.HeightConstraintMode & LayoutConstraintMode.CanvasLike) != LayoutConstraintMode.NonOverlappingGridlike)
        this.Settings.VerticalAlignment = VerticalAlignment.Stretch;
      else if (Tolerances.LessThanOrClose(this.ChildRect.Bottom, (this.SlotRect.Top + this.SlotRect.Bottom) / 2.0))
        this.Settings.VerticalAlignment = VerticalAlignment.Top;
      else if (Tolerances.GreaterThanOrClose(this.ChildRect.Top, (this.SlotRect.Top + this.SlotRect.Bottom) / 2.0))
        this.Settings.VerticalAlignment = VerticalAlignment.Bottom;
      else
        this.Settings.VerticalAlignment = VerticalAlignment.Stretch;
    }

    protected virtual void ComputeAlignmentOverrides()
    {
      if (this.SettingsFromElement.HorizontalAlignment != this.Settings.HorizontalAlignment)
      {
        this.Settings.LayoutOverrides |= LayoutOverrides.HorizontalAlignment;
        this.Settings.HorizontalAlignment = this.SettingsFromElement.HorizontalAlignment;
      }
      if (this.SettingsFromElement.VerticalAlignment == this.Settings.VerticalAlignment)
        return;
      this.Settings.LayoutOverrides |= LayoutOverrides.VerticalAlignment;
      this.Settings.VerticalAlignment = this.SettingsFromElement.VerticalAlignment;
    }

    protected virtual void SetAlignmentChanges()
    {
      if (this.SettingsFromElement.HorizontalAlignment != this.Settings.HorizontalAlignment)
      {
        if ((this.LayoutOverrides & LayoutOverrides.HorizontalAlignment) != LayoutOverrides.None && (this.OverridesToIgnore & (LayoutOverrides) (1 | (this.SettingsFromElement.HorizontalAlignment == HorizontalAlignment.Center ? 4 : 0))) == LayoutOverrides.None)
          this.Settings.HorizontalAlignment = this.SettingsFromElement.HorizontalAlignment;
        else
          this.Child.SetValueAsWpf(BaseFrameworkElement.HorizontalAlignmentProperty, (object) this.Settings.HorizontalAlignment);
        this.ExplicitOverrides &= ~LayoutOverrides.HorizontalAlignment;
      }
      else if ((this.LayoutOverrides & LayoutOverrides.HorizontalAlignment) != LayoutOverrides.None && (this.SettingsFromElement.LayoutOverrides & LayoutOverrides.HorizontalAlignment) == LayoutOverrides.None)
        this.ExplicitOverrides |= LayoutOverrides.HorizontalAlignment;
      if (this.SettingsFromElement.VerticalAlignment != this.Settings.VerticalAlignment)
      {
        if ((this.LayoutOverrides & LayoutOverrides.VerticalAlignment) != LayoutOverrides.None && (this.OverridesToIgnore & (LayoutOverrides) (2 | (this.SettingsFromElement.VerticalAlignment == VerticalAlignment.Center ? 8 : 0))) == LayoutOverrides.None)
          this.Settings.VerticalAlignment = this.SettingsFromElement.VerticalAlignment;
        else
          this.Child.SetValueAsWpf(BaseFrameworkElement.VerticalAlignmentProperty, (object) this.Settings.VerticalAlignment);
        this.ExplicitOverrides &= ~LayoutOverrides.VerticalAlignment;
      }
      else
      {
        if ((this.LayoutOverrides & LayoutOverrides.VerticalAlignment) == LayoutOverrides.None || (this.SettingsFromElement.LayoutOverrides & LayoutOverrides.VerticalAlignment) != LayoutOverrides.None)
          return;
        this.ExplicitOverrides |= LayoutOverrides.VerticalAlignment;
      }
    }

    protected virtual void ComputeIdealSize()
    {
      this.Settings.Width = this.Settings.HorizontalAlignment != HorizontalAlignment.Stretch || (this.WidthConstraintMode & LayoutConstraintMode.CanvasLike) != LayoutConstraintMode.NonOverlappingGridlike ? Math.Max(this.ChildRect.Width, 0.0) : double.NaN;
      this.Settings.Height = this.Settings.VerticalAlignment != VerticalAlignment.Stretch || (this.HeightConstraintMode & LayoutConstraintMode.CanvasLike) != LayoutConstraintMode.NonOverlappingGridlike ? Math.Max(this.ChildRect.Height, 0.0) : double.NaN;
    }

    protected virtual void ComputeSizeOverrides()
    {
      if (!object.Equals((object) RoundingHelper.RoundLength(this.SettingsFromElement.Width), (object) RoundingHelper.RoundLength(this.Settings.Width)))
      {
        this.Settings.LayoutOverrides |= LayoutOverrides.Width;
        this.Settings.Width = this.SettingsFromElement.Width;
      }
      if (object.Equals((object) RoundingHelper.RoundLength(this.SettingsFromElement.Height), (object) RoundingHelper.RoundLength(this.Settings.Height)))
        return;
      this.Settings.LayoutOverrides |= LayoutOverrides.Height;
      this.Settings.Height = this.SettingsFromElement.Height;
    }

    protected virtual void SetSizeChanges()
    {
      this.Settings.Width = this.SetPropertyChanges<double>(this.Settings.Width, this.SettingsFromElement.Width, LayoutOverrides.Width, BaseFrameworkElement.WidthProperty);
      this.Settings.Height = this.SetPropertyChanges<double>(this.Settings.Height, this.SettingsFromElement.Height, LayoutOverrides.Height, BaseFrameworkElement.HeightProperty);
    }

    protected virtual void ComputeIdealMargin()
    {
      bool flag1 = (this.WidthConstraintMode & LayoutConstraintMode.CanvasLike) == LayoutConstraintMode.NonOverlappingGridlike;
      bool flag2 = (this.HeightConstraintMode & LayoutConstraintMode.CanvasLike) == LayoutConstraintMode.NonOverlappingGridlike;
      this.Settings.Margin = new Thickness(RoundingHelper.RoundLength(!flag1 || this.Settings.HorizontalAlignment != HorizontalAlignment.Left && this.Settings.HorizontalAlignment != HorizontalAlignment.Stretch ? 0.0 : this.ChildRect.Left - this.SlotRect.Left), RoundingHelper.RoundLength(!flag2 || this.Settings.VerticalAlignment != VerticalAlignment.Top && this.Settings.VerticalAlignment != VerticalAlignment.Stretch ? 0.0 : this.ChildRect.Top - this.SlotRect.Top), RoundingHelper.RoundLength(!flag1 || this.Settings.HorizontalAlignment != HorizontalAlignment.Right && this.Settings.HorizontalAlignment != HorizontalAlignment.Stretch ? 0.0 : this.SlotRect.Right - this.ChildRect.Right), RoundingHelper.RoundLength(!flag2 || this.Settings.VerticalAlignment != VerticalAlignment.Bottom && this.Settings.VerticalAlignment != VerticalAlignment.Stretch ? 0.0 : this.SlotRect.Bottom - this.ChildRect.Bottom));
    }

    protected virtual void ComputeMarginOverrides()
    {
      bool flag = false;
      double left = this.Settings.Margin.Left;
      double right = this.Settings.Margin.Right;
      double top = this.Settings.Margin.Top;
      double bottom = this.Settings.Margin.Bottom;
      if (!Tolerances.AreClose(this.SettingsFromElement.Margin.Left, this.Settings.Margin.Left) || !Tolerances.AreClose(this.SettingsFromElement.Margin.Right, this.Settings.Margin.Right))
      {
        this.Settings.LayoutOverrides |= LayoutOverrides.HorizontalMargin;
        left = this.SettingsFromElement.Margin.Left;
        right = this.SettingsFromElement.Margin.Right;
        flag = true;
      }
      if (!Tolerances.AreClose(this.SettingsFromElement.Margin.Top, this.Settings.Margin.Top) || !Tolerances.AreClose(this.SettingsFromElement.Margin.Bottom, this.Settings.Margin.Bottom))
      {
        this.Settings.LayoutOverrides |= LayoutOverrides.VerticalMargin;
        top = this.SettingsFromElement.Margin.Top;
        bottom = this.SettingsFromElement.Margin.Bottom;
        flag = true;
      }
      if (!flag)
        return;
      this.Settings.Margin = new Thickness(left, top, right, bottom);
    }

    protected virtual void SetMarginChanges()
    {
      bool flag1 = false;
      bool flag2 = false;
      double left = this.Settings.Margin.Left;
      double right = this.Settings.Margin.Right;
      double top = this.Settings.Margin.Top;
      double bottom = this.Settings.Margin.Bottom;
      if (!Tolerances.AreClose(this.SettingsFromElement.Margin.Left, this.Settings.Margin.Left) || !Tolerances.AreClose(this.SettingsFromElement.Margin.Right, this.Settings.Margin.Right))
      {
        if ((this.LayoutOverrides & LayoutOverrides.HorizontalMargin) != LayoutOverrides.None && (this.OverridesToIgnore & (this.Settings.HorizontalAlignment == HorizontalAlignment.Center ? LayoutOverrides.CenterHorizontalMargin : LayoutOverrides.HorizontalMargin)) == LayoutOverrides.None)
        {
          left = this.SettingsFromElement.Margin.Left;
          right = this.SettingsFromElement.Margin.Right;
          flag1 = true;
        }
        else
          flag2 = true;
        this.ExplicitOverrides &= ~LayoutOverrides.HorizontalMargin;
        if (double.IsNaN(this.Settings.Width) && (this.OverridesToIgnore & LayoutOverrides.Width) != LayoutOverrides.None && !Tolerances.AreClose(this.SettingsFromElement.Margin.Left + this.SettingsFromElement.Margin.Right, this.Settings.Margin.Left + this.Settings.Margin.Right))
          this.ExplicitOverrides &= ~LayoutOverrides.Width;
      }
      else if ((this.LayoutOverrides & LayoutOverrides.HorizontalMargin) != LayoutOverrides.None && (this.SettingsFromElement.LayoutOverrides & LayoutOverrides.HorizontalMargin) == LayoutOverrides.None)
        this.ExplicitOverrides |= LayoutOverrides.HorizontalMargin;
      if (!Tolerances.AreClose(this.SettingsFromElement.Margin.Top, this.Settings.Margin.Top) || !Tolerances.AreClose(this.SettingsFromElement.Margin.Bottom, this.Settings.Margin.Bottom))
      {
        if ((this.LayoutOverrides & LayoutOverrides.VerticalMargin) != LayoutOverrides.None && (this.OverridesToIgnore & (this.Settings.VerticalAlignment == VerticalAlignment.Center ? LayoutOverrides.CenterVerticalMargin : LayoutOverrides.VerticalMargin)) == LayoutOverrides.None)
        {
          top = this.SettingsFromElement.Margin.Top;
          bottom = this.SettingsFromElement.Margin.Bottom;
          flag1 = true;
        }
        else
          flag2 = true;
        this.ExplicitOverrides &= ~LayoutOverrides.VerticalMargin;
        if (double.IsNaN(this.Settings.Height) && (this.OverridesToIgnore & LayoutOverrides.Height) != LayoutOverrides.None && !Tolerances.AreClose(this.SettingsFromElement.Margin.Top + this.SettingsFromElement.Margin.Bottom, this.Settings.Margin.Top + this.Settings.Margin.Bottom))
          this.ExplicitOverrides &= ~LayoutOverrides.Height;
      }
      else if ((this.LayoutOverrides & LayoutOverrides.VerticalMargin) != LayoutOverrides.None && (this.SettingsFromElement.LayoutOverrides & LayoutOverrides.VerticalMargin) == LayoutOverrides.None)
        this.ExplicitOverrides |= LayoutOverrides.VerticalMargin;
      if (flag1)
        this.Settings.Margin = new Thickness(left, top, right, bottom);
      if (!flag2)
        return;
      this.Child.SetValueAsWpf(BaseFrameworkElement.MarginProperty, (object) this.Settings.Margin);
    }

    protected T SetPropertyChanges<T>(T settingsValue, T settingsFromElementValue, LayoutOverrides layoutOverride, IPropertyId property)
    {
      if (!settingsFromElementValue.Equals((object) settingsValue))
      {
        if ((this.LayoutOverrides & layoutOverride) != LayoutOverrides.None && (this.OverridesToIgnore & layoutOverride) == LayoutOverrides.None)
          settingsValue = settingsFromElementValue;
        else
          this.Child.SetValueAsWpf(property, (object) settingsValue);
        this.ExplicitOverrides &= ~layoutOverride;
      }
      else if ((this.LayoutOverrides & layoutOverride) != LayoutOverrides.None && (this.SettingsFromElement.LayoutOverrides & layoutOverride) == LayoutOverrides.None)
        this.ExplicitOverrides |= layoutOverride;
      return settingsValue;
    }
  }
}
