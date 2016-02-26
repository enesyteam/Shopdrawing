// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Designers.LayoutDesigner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.Tools.Layout;
using Microsoft.Expression.DesignSurface.Tools.Transforms;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.Designers
{
  public class LayoutDesigner : ILayoutDesigner
  {
    private static readonly IEnumerable<IPropertyId> CommonLayoutProperties = (IEnumerable<IPropertyId>) new List<IPropertyId>()
    {
      BaseFrameworkElement.WidthProperty,
      BaseFrameworkElement.MinWidthProperty,
      BaseFrameworkElement.MaxWidthProperty,
      BaseFrameworkElement.HeightProperty,
      BaseFrameworkElement.MinHeightProperty,
      BaseFrameworkElement.MaxHeightProperty,
      BaseFrameworkElement.MarginProperty,
      BaseFrameworkElement.HorizontalAlignmentProperty,
      BaseFrameworkElement.VerticalAlignmentProperty,
      DesignTimeProperties.LayoutOverridesProperty
    };
    private static List<IPropertyId> AllSpecializedLayoutProperties = new List<IPropertyId>()
    {
      GridElement.RowProperty,
      GridElement.ColumnProperty,
      GridElement.RowSpanProperty,
      GridElement.ColumnSpanProperty,
      CanvasElement.LeftProperty,
      CanvasElement.TopProperty,
      CanvasElement.RightProperty,
      CanvasElement.BottomProperty,
      DockPanelElement.DockProperty
    };
    private IEnumerable<IPropertyId> specializedLayoutProperties;
    private int suppressLayoutRoundingCount;

    public IDisposable SuppressLayoutRounding
    {
      get
      {
        return (IDisposable) new LayoutDesigner.SuppressLayoutRoundingToken(this);
      }
    }

    public LayoutDesigner()
    {
    }

    public LayoutDesigner(IEnumerable<IPropertyId> specializedLayoutProperties)
    {
      this.specializedLayoutProperties = specializedLayoutProperties;
    }

    public virtual Rect GetChildRect(BaseFrameworkElement child)
    {
      return child.ViewModel.DefaultView.GetActualBoundsInParent(child.Visual);
    }

    public void SetChildRect(BaseFrameworkElement child, Rect rect)
    {
      this.SetChildRect(child, this.PrepareLayoutRect(rect), LayoutOverrides.RecomputeDefault, LayoutOverrides.Width | LayoutOverrides.Height, LayoutOverrides.None, SetRectMode.Default);
    }

    public void SetChildRect(BaseFrameworkElement child, Rect rect, bool setWidth, bool setHeight)
    {
      LayoutOverrides overridesToIgnore = (LayoutOverrides) (0 | (setWidth ? 16 : 0) | (setHeight ? 32 : 0));
      this.SetChildRect(child, this.PrepareLayoutRect(rect), LayoutOverrides.RecomputeDefault, overridesToIgnore, LayoutOverrides.None, SetRectMode.Default);
    }

    public void SetChildRect(BaseFrameworkElement child, Rect rect, LayoutOverrides layoutOverrides, LayoutOverrides overridesToIgnore, LayoutOverrides nonExplicitOverrides)
    {
      this.SetChildRect(child, rect, layoutOverrides, overridesToIgnore, nonExplicitOverrides, SetRectMode.Default);
    }

    protected virtual LayoutOperation CreateLayoutOperation(BaseFrameworkElement child)
    {
      return new LayoutOperation((ILayoutDesigner) this, child);
    }

    private LayoutOverrides AdjustOverrideToIgnore(BaseFrameworkElement child, LayoutOverrides overridesToIgnore)
    {
      if (!child.IsAttached)
        overridesToIgnore = LayoutOverrides.All;
      if (LayoutRoundingHelper.UpdateLayoutRounding((SceneElement) child))
        child.ViewModel.Document.OnUpdatedEditTransaction();
      BaseFrameworkElement frameworkElement = this.GetParentFrameworkElement(child);
      LayoutConstraintMode widthConstraintMode = this.GetWidthConstraintMode(child);
      if (frameworkElement is CanvasElement || (widthConstraintMode & LayoutConstraintMode.CanvasLike) == LayoutConstraintMode.NonOverlappingGridlike)
        overridesToIgnore |= LayoutOverrides.HorizontalMargin;
      LayoutConstraintMode heightConstraintMode = this.GetHeightConstraintMode(child);
      if (frameworkElement is CanvasElement || (heightConstraintMode & LayoutConstraintMode.CanvasLike) == LayoutConstraintMode.NonOverlappingGridlike)
        overridesToIgnore |= LayoutOverrides.VerticalMargin;
      return overridesToIgnore;
    }

    public virtual void SetChildRect(BaseFrameworkElement child, Rect rect, LayoutOverrides layoutOverrides, LayoutOverrides overridesToIgnore, LayoutOverrides nonExplicitOverrides, SetRectMode setRectMode)
    {
      overridesToIgnore = this.AdjustOverrideToIgnore(child, overridesToIgnore);
      if (this.suppressLayoutRoundingCount == 0 && (LayoutRoundingHelper.GetLayoutRoundingStatus((SceneElement) child) & LayoutRoundingStatus.ShouldSnapToPixel) != LayoutRoundingStatus.Off)
        rect = LayoutRoundingHelper.RoundRect(child.Platform.GeometryHelper, rect);
      using (child.ViewModel.ScopeViewObjectCache())
      {
        using (GridLayoutDesigner.TryCanvasDesignMode(child, rect.Size, true, true))
        {
          LayoutUtilities.EnterLayoutMode();
          LayoutOperation layoutOperation = this.CreateLayoutOperation(child);
          if (layoutOverrides == LayoutOverrides.RecomputeDefault)
            layoutOverrides = this.InternalComputeOverrides(child, layoutOperation);
          layoutOperation.SetRect(rect, layoutOverrides, overridesToIgnore, nonExplicitOverrides, setRectMode);
          LayoutUtilities.ExitLayoutMode();
        }
      }
    }

    public virtual void SetRootSize(BaseFrameworkElement root, Size size, bool setWidth, bool setHeight)
    {
      root = this.GetSizeElement(root);
      size = RoundingHelper.RoundSize(size);
      if (setWidth)
        root.Width = size.Width;
      if (!setHeight)
        return;
      root.Height = size.Height;
    }

    public virtual void FillChild(BaseFrameworkElement element)
    {
      IViewVisual viewVisual1 = element.Visual as IViewVisual;
      IViewVisual viewVisual2 = viewVisual1 != null ? viewVisual1.VisualParent as IViewVisual : (IViewVisual) null;
      Type runtimeType = element.ProjectContext.ResolveType(PlatformTypes.UIElement).RuntimeType;
      if (viewVisual2 == null || !runtimeType.IsAssignableFrom(viewVisual2.TargetType))
        viewVisual2 = element.ParentElement.Visual as IViewVisual;
      if (viewVisual2 == null)
        return;
      Rect rect = new Rect(new Point(), viewVisual2.RenderSize);
      this.SetChildRect(element, this.PrepareLayoutRect(rect), LayoutOverrides.None, LayoutOverrides.All, LayoutOverrides.All);
    }

    public LayoutOverrides ComputeOverrides(BaseFrameworkElement element)
    {
      LayoutOperation layoutOperation = this.CreateLayoutOperation(element);
      return this.InternalComputeOverrides(element, layoutOperation);
    }

    protected virtual LayoutOverrides InternalComputeOverrides(BaseFrameworkElement element, LayoutOperation layoutOperation)
    {
      if (!element.IsAttached)
        return LayoutOverrides.None;
      Rect childRect = this.GetChildRect(element);
      return layoutOperation.ComputeOverrides(childRect);
    }

    public void SetHorizontalAlignment(BaseFrameworkElement element, HorizontalAlignment alignment)
    {
      using (element.ViewModel.EnforceGridDesignMode)
      {
        LayoutOverrides overrides = this.ComputeOverrides(element);
        Rect childRect = this.GetChildRect(element);
        element.SetValueAsWpf(BaseFrameworkElement.HorizontalAlignmentProperty, (object) alignment);
        element.ViewModel.Document.OnUpdatedEditTransaction();
        this.SetChildRect(element, childRect, overrides | LayoutOverrides.HorizontalAlignment, LayoutOverrides.HorizontalMargin | LayoutOverrides.CenterHorizontalMargin, LayoutOverrides.HorizontalAlignment);
      }
    }

    public void SetVerticalAlignment(BaseFrameworkElement element, VerticalAlignment alignment)
    {
      using (element.ViewModel.EnforceGridDesignMode)
      {
        LayoutOverrides overrides = this.ComputeOverrides(element);
        Rect childRect = this.GetChildRect(element);
        element.SetValueAsWpf(BaseFrameworkElement.VerticalAlignmentProperty, (object) alignment);
        element.ViewModel.Document.OnUpdatedEditTransaction();
        this.SetChildRect(element, childRect, overrides | LayoutOverrides.VerticalAlignment, LayoutOverrides.VerticalMargin | LayoutOverrides.CenterVerticalMargin, LayoutOverrides.VerticalAlignment);
      }
    }

    public LayoutConstraintMode GetWidthConstraintMode(BaseFrameworkElement child)
    {
      IViewVisual child1 = child == null ? (IViewVisual) null : child.Visual as IViewVisual;
      IViewVisual parent = child1 == null ? (IViewVisual) null : child1.VisualParent as IViewVisual;
      SceneView view = child == null ? (SceneView) null : child.ViewModel.DefaultView;
      return this.GetWidthConstraintModeCore(parent, child1, view, false);
    }

    public LayoutConstraintMode GetWidthConstraintMode(BaseFrameworkElement parent, BaseFrameworkElement child)
    {
      return this.GetWidthConstraintModeCore(parent == null ? (IViewVisual) null : parent.Visual as IViewVisual, child == null ? (IViewVisual) null : child.Visual as IViewVisual, child == null ? (parent == null ? (SceneView) null : parent.ViewModel.DefaultView) : child.ViewModel.DefaultView, false);
    }

    private LayoutConstraintMode GetWidthConstraintModeCore(IViewVisual parent, IViewVisual child, SceneView view, bool allowGridAutoChecks)
    {
      if (parent == null)
        return LayoutConstraintMode.CanvasLike;
      LayoutConstraintMode layoutConstraintMode = LayoutConstraintMode.NonOverlappingGridlike;
      IType itype = parent.GetIType((ITypeResolver) view.ProjectContext);
      if (PlatformTypes.Grid.IsAssignableFrom((ITypeId) itype) || PlatformTypes.Canvas.IsAssignableFrom((ITypeId) itype))
        layoutConstraintMode |= LayoutConstraintMode.Overlapping;
      IViewVisual viewVisual = parent.VisualParent as IViewVisual;
      if (viewVisual != null && PlatformTypes.IsInstance(viewVisual.PlatformSpecificObject, ProjectNeutralTypes.Viewbox, (ITypeResolver) view.ProjectContext))
        layoutConstraintMode |= LayoutConstraintMode.CanvasLike;
      if (PlatformTypes.Canvas.IsAssignableFrom((ITypeId) itype) || PlatformTypes.TextBlock.IsAssignableFrom((ITypeId) itype) || (PlatformTypes.RichTextBox.IsAssignableFrom((ITypeId) itype) || PlatformTypes.FlowDocumentScrollViewer.IsAssignableFrom((ITypeId) itype)) || (PlatformTypes.ScrollContentPresenter.IsAssignableFrom((ITypeId) itype) || ProjectNeutralTypes.Viewbox.IsAssignableFrom((ITypeId) itype) || (PlatformTypes.Popup.IsAssignableFrom((ITypeId) itype) || ProjectNeutralTypes.PathPanel.IsAssignableFrom((ITypeId) itype))))
        layoutConstraintMode |= LayoutConstraintMode.CanvasLike;
      if (PlatformTypes.StackPanel.IsAssignableFrom((ITypeId) itype) && ((IViewPanel) parent).Orientation == Orientation.Horizontal)
        layoutConstraintMode |= LayoutConstraintMode.CanvasLike;
      if (ProjectNeutralTypes.DockPanel.IsAssignableFrom((ITypeId) itype) && child != null)
      {
        switch ((Dock) view.ConvertToWpfValue(child.GetCurrentValue(view.ProjectContext.ResolveProperty(DockPanelElement.DockProperty))))
        {
          case Dock.Left:
          case Dock.Right:
            layoutConstraintMode |= LayoutConstraintMode.CanvasLike;
            break;
        }
      }
      if (ProjectNeutralTypes.WrapPanel.IsAssignableFrom((ITypeId) itype) && double.IsNaN((double) parent.GetCurrentValue(view.ProjectContext.ResolveProperty(WrapPanelElement.ItemWidthProperty))))
        layoutConstraintMode |= LayoutConstraintMode.CanvasLike;
      if ((layoutConstraintMode & LayoutConstraintMode.CanvasLike) == LayoutConstraintMode.NonOverlappingGridlike && double.IsNaN((double) parent.GetCurrentValue(view.ProjectContext.ResolveProperty(BaseFrameworkElement.WidthProperty))) && double.IsNaN((double) parent.GetCurrentValue(view.ProjectContext.ResolveProperty(DesignTimeProperties.DesignWidthProperty))))
      {
        IViewGrid viewGrid = parent as IViewGrid;
        if (viewGrid != null)
        {
          if (!allowGridAutoChecks)
            return layoutConstraintMode;
          IType type = child == null ? (IType) null : child.GetIType((ITypeResolver) view.ProjectContext);
          if (child != null && (!PlatformTypes.Path.IsAssignableFrom((ITypeId) type) || !double.IsNaN((double) child.GetCurrentValue(view.ProjectContext.ResolveProperty(BaseFrameworkElement.WidthProperty)))))
          {
            int val2_1 = (int) child.GetCurrentValue(view.ProjectContext.ResolveProperty(GridElement.ColumnProperty));
            int val2_2 = val2_1 + (int) child.GetCurrentValue(view.ProjectContext.ResolveProperty(GridElement.ColumnSpanProperty));
            int num1 = Math.Max(0, Math.Min(viewGrid.ColumnDefinitionsCount - 1, val2_1));
            int num2 = Math.Max(0, Math.Min(viewGrid.ColumnDefinitionsCount, val2_2));
            double num3 = 0.0;
            bool flag1 = true;
            bool flag2 = true;
            if (num2 == 0)
            {
              flag1 = false;
              flag2 = false;
              num3 = viewGrid.RenderSize.Width;
            }
            else
            {
              for (int index = num1; index < num2; ++index)
              {
                IViewColumnDefinition columnDefinition = viewGrid.GetColumnDefinition(index);
                flag1 &= columnDefinition.Width.IsAuto;
                flag2 &= columnDefinition.Width.IsAbsolute;
                num3 += columnDefinition.ActualWidth;
              }
            }
            double width = child.DesiredSize.Width;
            IProperty propertyKey = view.Platform.Metadata.ResolveProperty(Base2DElement.UseLayoutRoundingProperty);
            if (propertyKey != null && child.GetCurrentValue(propertyKey) != parent.GetCurrentValue(propertyKey))
              width += 0.999;
            if (Tolerances.LessThan(width, num3) || flag2)
              return layoutConstraintMode;
            if (flag1)
              layoutConstraintMode |= LayoutConstraintMode.CanvasLike;
          }
        }
        if ((HorizontalAlignment) view.ConvertToWpfValue(parent.GetCurrentValue(view.ProjectContext.ResolveProperty(BaseFrameworkElement.HorizontalAlignmentProperty))) != HorizontalAlignment.Stretch)
        {
          layoutConstraintMode |= LayoutConstraintMode.CanvasLike;
        }
        else
        {
          if (PlatformTypes.Control.IsAssignableFrom((ITypeId) itype) || PlatformTypes.ContentPresenter.IsAssignableFrom((ITypeId) itype))
            allowGridAutoChecks = true;
          if ((this.GetWidthConstraintModeCore(parent.VisualParent as IViewVisual, parent, view, allowGridAutoChecks) & LayoutConstraintMode.CanvasLike) != LayoutConstraintMode.NonOverlappingGridlike)
            layoutConstraintMode |= LayoutConstraintMode.CanvasLike;
        }
      }
      return layoutConstraintMode;
    }

    public LayoutConstraintMode GetHeightConstraintMode(BaseFrameworkElement child)
    {
      IViewVisual child1 = child == null ? (IViewVisual) null : child.Visual as IViewVisual;
      IViewVisual parent = child1 == null ? (IViewVisual) null : child1.VisualParent as IViewVisual;
      SceneView view = child == null ? (SceneView) null : child.ViewModel.DefaultView;
      return this.GetHeightConstraintModeCore(parent, child1, view, false);
    }

    public LayoutConstraintMode GetHeightConstraintMode(BaseFrameworkElement parent, BaseFrameworkElement child)
    {
      return this.GetHeightConstraintModeCore(parent == null ? (IViewVisual) null : parent.Visual as IViewVisual, child == null ? (IViewVisual) null : child.Visual as IViewVisual, child == null ? (parent == null ? (SceneView) null : parent.ViewModel.DefaultView) : child.ViewModel.DefaultView, false);
    }

    private LayoutConstraintMode GetHeightConstraintModeCore(IViewVisual parent, IViewVisual child, SceneView view, bool allowGridAutoChecks)
    {
      if (parent == null)
        return LayoutConstraintMode.CanvasLike;
      LayoutConstraintMode layoutConstraintMode = LayoutConstraintMode.NonOverlappingGridlike;
      IType itype = parent.GetIType((ITypeResolver) view.ProjectContext);
      if (PlatformTypes.Grid.IsAssignableFrom((ITypeId) itype) || PlatformTypes.Canvas.IsAssignableFrom((ITypeId) itype))
        layoutConstraintMode |= LayoutConstraintMode.Overlapping;
      IViewVisual viewVisual = parent.VisualParent as IViewVisual;
      if (viewVisual != null && PlatformTypes.IsInstance(viewVisual.PlatformSpecificObject, ProjectNeutralTypes.Viewbox, (ITypeResolver) view.ProjectContext))
        layoutConstraintMode |= LayoutConstraintMode.CanvasLike;
      if (PlatformTypes.Canvas.IsAssignableFrom((ITypeId) itype) || PlatformTypes.TextBlock.IsAssignableFrom((ITypeId) itype) || (PlatformTypes.RichTextBox.IsAssignableFrom((ITypeId) itype) || PlatformTypes.FlowDocumentScrollViewer.IsAssignableFrom((ITypeId) itype)) || (PlatformTypes.ScrollContentPresenter.IsAssignableFrom((ITypeId) itype) || ProjectNeutralTypes.Viewbox.IsAssignableFrom((ITypeId) itype) || (PlatformTypes.Popup.IsAssignableFrom((ITypeId) itype) || ProjectNeutralTypes.PathPanel.IsAssignableFrom((ITypeId) itype))))
        layoutConstraintMode |= LayoutConstraintMode.CanvasLike;
      if (PlatformTypes.StackPanel.IsAssignableFrom((ITypeId) itype) && ((IViewPanel) parent).Orientation == Orientation.Vertical)
        layoutConstraintMode |= LayoutConstraintMode.CanvasLike;
      if (ProjectNeutralTypes.DockPanel.IsAssignableFrom((ITypeId) itype) && child != null)
      {
        switch ((Dock) view.ConvertToWpfValue(child.GetCurrentValue(view.ProjectContext.ResolveProperty(DockPanelElement.DockProperty))))
        {
          case Dock.Top:
          case Dock.Bottom:
            layoutConstraintMode |= LayoutConstraintMode.CanvasLike;
            break;
        }
      }
      if (ProjectNeutralTypes.WrapPanel.IsAssignableFrom((ITypeId) itype) && double.IsNaN((double) parent.GetCurrentValue(view.ProjectContext.ResolveProperty(WrapPanelElement.ItemHeightProperty))))
        layoutConstraintMode |= LayoutConstraintMode.CanvasLike;
      if ((layoutConstraintMode & LayoutConstraintMode.CanvasLike) == LayoutConstraintMode.NonOverlappingGridlike && double.IsNaN((double) parent.GetCurrentValue(view.ProjectContext.ResolveProperty(BaseFrameworkElement.HeightProperty))) && double.IsNaN((double) parent.GetCurrentValue(view.ProjectContext.ResolveProperty(DesignTimeProperties.DesignHeightProperty))))
      {
        IViewGrid viewGrid = parent as IViewGrid;
        if (viewGrid != null)
        {
          if (!allowGridAutoChecks)
            return layoutConstraintMode;
          IType type = child == null ? (IType) null : child.GetIType((ITypeResolver) view.ProjectContext);
          if (child != null && (!PlatformTypes.Path.IsAssignableFrom((ITypeId) type) || !double.IsNaN((double) child.GetCurrentValue(view.ProjectContext.ResolveProperty(BaseFrameworkElement.HeightProperty)))))
          {
            int val2_1 = (int) child.GetCurrentValue(view.ProjectContext.ResolveProperty(GridElement.RowProperty));
            int val2_2 = val2_1 + (int) child.GetCurrentValue(view.ProjectContext.ResolveProperty(GridElement.RowSpanProperty));
            int num1 = Math.Max(0, Math.Min(viewGrid.RowDefinitionsCount - 1, val2_1));
            int num2 = Math.Max(0, Math.Min(viewGrid.RowDefinitionsCount, val2_2));
            double num3 = 0.0;
            bool flag1 = true;
            bool flag2 = true;
            if (num2 == 0)
            {
              flag1 = false;
              flag2 = false;
              num3 = viewGrid.RenderSize.Height;
            }
            else
            {
              for (int index = num1; index < num2; ++index)
              {
                IViewRowDefinition rowDefinition = viewGrid.GetRowDefinition(index);
                flag1 &= rowDefinition.Height.IsAuto;
                flag2 &= rowDefinition.Height.IsAbsolute;
                num3 += rowDefinition.ActualHeight;
              }
            }
            double height = child.DesiredSize.Height;
            IProperty propertyKey = view.Platform.Metadata.ResolveProperty(Base2DElement.UseLayoutRoundingProperty);
            if (propertyKey != null && child.GetCurrentValue(propertyKey) != parent.GetCurrentValue(propertyKey))
              height += 0.999;
            if (Tolerances.LessThan(height, num3) || flag2)
              return layoutConstraintMode;
            if (flag1)
              layoutConstraintMode |= LayoutConstraintMode.CanvasLike;
          }
        }
        if ((VerticalAlignment) view.ConvertToWpfValue(parent.GetCurrentValue(view.ProjectContext.ResolveProperty(BaseFrameworkElement.VerticalAlignmentProperty))) != VerticalAlignment.Stretch)
        {
          layoutConstraintMode |= LayoutConstraintMode.CanvasLike;
        }
        else
        {
          if (PlatformTypes.Control.IsAssignableFrom((ITypeId) itype) || PlatformTypes.ContentPresenter.IsAssignableFrom((ITypeId) itype))
            allowGridAutoChecks = true;
          if ((this.GetHeightConstraintModeCore(parent.VisualParent as IViewVisual, parent, view, allowGridAutoChecks) & LayoutConstraintMode.CanvasLike) != LayoutConstraintMode.NonOverlappingGridlike)
            layoutConstraintMode |= LayoutConstraintMode.CanvasLike;
        }
      }
      return layoutConstraintMode;
    }

    public LayoutCacheRecord CacheLayout(BaseFrameworkElement element)
    {
      Rect childRect = this.GetChildRect(element);
      LayoutOverrides overrides = this.ComputeOverrides(element);
      LayoutConstraintMode widthConstraintMode = this.GetWidthConstraintMode(element);
      int num = (int) this.GetHeightConstraintMode(element);
      Point slotOrigin = (widthConstraintMode & LayoutConstraintMode.Overlapping) != LayoutConstraintMode.NonOverlappingGridlike || element.Visual == null ? new Point() : ((IViewVisual) element.Visual).GetLayoutSlot().TopLeft;
      return new LayoutCacheRecord(childRect, slotOrigin, overrides, (widthConstraintMode & LayoutConstraintMode.Overlapping) != LayoutConstraintMode.NonOverlappingGridlike);
    }

    public void SetLayoutFromCache(BaseFrameworkElement element, LayoutCacheRecord layoutCacheRecord, Rect boundsOfAllCaches)
    {
      LayoutConstraintMode widthConstraintMode = this.GetWidthConstraintMode(element);
      int num = (int) this.GetHeightConstraintMode(element);
      this.ClearUnusedLayoutProperties(element);
      if (layoutCacheRecord == null)
        return;
      using (element.ViewModel.EnforceGridDesignMode)
      {
        Rect rect = layoutCacheRecord.Rect;
        if ((widthConstraintMode & LayoutConstraintMode.Overlapping) != LayoutConstraintMode.NonOverlappingGridlike)
          this.SetChildRect(element, rect, layoutCacheRecord.Overrides, LayoutOverrides.None, LayoutOverrides.None, SetRectMode.Default);
        else if (layoutCacheRecord.Overlapping)
        {
          if (!boundsOfAllCaches.IsEmpty)
            rect.Offset(-boundsOfAllCaches.Left, -boundsOfAllCaches.Top);
          this.SetChildRect(element, rect, layoutCacheRecord.Overrides, LayoutOverrides.Margin, LayoutOverrides.None, SetRectMode.CreateAtSlotPosition);
        }
        else
        {
          rect.Offset(-layoutCacheRecord.SlotOrigin.X, -layoutCacheRecord.SlotOrigin.Y);
          this.SetChildRect(element, rect, layoutCacheRecord.Overrides, LayoutOverrides.None, LayoutOverrides.None, SetRectMode.CreateAtSlotPosition);
        }
      }
    }

    public void ClearUnusedLayoutProperties(BaseFrameworkElement element)
    {
      foreach (IPropertyId propertyId in LayoutDesigner.AllSpecializedLayoutProperties)
      {
        if (this.specializedLayoutProperties == null || !Enumerable.Contains<IPropertyId>(this.specializedLayoutProperties, propertyId))
        {
          IProperty property = element.ProjectContext.ResolveProperty(propertyId);
          if (property != null)
            element.ClearValue((IPropertyId) property);
        }
      }
    }

    public List<LayoutCacheRecord> GetCurrentRects(BaseFrameworkElement parent)
    {
      ISceneNodeCollection<SceneNode> defaultContent = parent.DefaultContent;
      List<LayoutCacheRecord> list = new List<LayoutCacheRecord>(defaultContent.Count);
      for (int index = 0; index < defaultContent.Count; ++index)
      {
        BaseFrameworkElement element = defaultContent[index] as BaseFrameworkElement;
        if (element != null && !PlatformTypes.ContextMenu.IsAssignableFrom((ITypeId) element.Type))
        {
          if (element.IsViewObjectValid)
            list.Add(this.CacheLayout(element));
        }
        else
          list.Add((LayoutCacheRecord) null);
      }
      return list;
    }

    public void SetCurrentRects(BaseFrameworkElement parent, List<LayoutCacheRecord> layoutCache)
    {
      ISceneNodeCollection<SceneNode> defaultContent = parent.DefaultContent;
      if (layoutCache.Count != defaultContent.Count)
        return;
      using (parent.ViewModel.EnforceGridDesignMode)
      {
        for (int index = 0; index < defaultContent.Count; ++index)
        {
          BaseFrameworkElement element = defaultContent[index] as BaseFrameworkElement;
          if (element != null && !PlatformTypes.ContextMenu.IsAssignableFrom((ITypeId) element.Type) && element.IsViewObjectValid)
          {
            LayoutCacheRecord layoutCacheRecord = layoutCache[index];
            this.SetLayoutFromCache(element, layoutCacheRecord, Rect.Empty);
          }
        }
      }
    }

    public IEnumerable<IPropertyId> GetLayoutProperties()
    {
      if (this.specializedLayoutProperties == null)
        return LayoutDesigner.CommonLayoutProperties;
      return Enumerable.Concat<IPropertyId>(LayoutDesigner.CommonLayoutProperties, this.specializedLayoutProperties);
    }

    private double PreparePosition(double pos)
    {
      if (!FloatingPointArithmetic.IsFiniteDouble(pos))
        return 0.0;
      return pos;
    }

    private double PrepareLength(double length)
    {
      if (!FloatingPointArithmetic.IsFiniteDouble(length))
        return 0.0;
      return Math.Max(length, 0.0);
    }

    protected Rect PrepareLayoutRect(Rect rect)
    {
      return new Rect(this.PreparePosition(rect.X), this.PreparePosition(rect.Y), this.PrepareLength(rect.Width), this.PrepareLength(rect.Height));
    }

    protected BaseFrameworkElement GetSizeElement(BaseFrameworkElement element)
    {
      if (element == element.ViewModel.FindPanelClosestToRoot())
        return element.ViewModel.ViewRoot as BaseFrameworkElement ?? element;
      return element;
    }

    private BaseFrameworkElement GetParentFrameworkElement(BaseFrameworkElement child)
    {
      SceneElement sceneElement = child.ParentElement;
      TextElementSceneElement elementSceneElement = sceneElement as TextElementSceneElement;
      if (elementSceneElement != null)
        sceneElement = (SceneElement) elementSceneElement.HostElement;
      return sceneElement as BaseFrameworkElement;
    }

    private class SuppressLayoutRoundingToken : IDisposable
    {
      private LayoutDesigner layoutDesigner;

      public SuppressLayoutRoundingToken(LayoutDesigner layoutDesigner)
      {
        this.layoutDesigner = layoutDesigner;
        ++this.layoutDesigner.suppressLayoutRoundingCount;
      }

      public void Dispose()
      {
        --this.layoutDesigner.suppressLayoutRoundingCount;
        GC.SuppressFinalize((object) this);
      }
    }
  }
}
