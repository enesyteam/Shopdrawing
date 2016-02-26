// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Designers.ILayoutDesigner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Designers
{
  public interface ILayoutDesigner
  {
    IDisposable SuppressLayoutRounding { get; }

    Rect GetChildRect(BaseFrameworkElement child);

    void SetChildRect(BaseFrameworkElement child, Rect rect);

    void SetChildRect(BaseFrameworkElement child, Rect rect, bool setWidth, bool setHeight);

    void SetChildRect(BaseFrameworkElement child, Rect rect, LayoutOverrides layoutOverrides, LayoutOverrides overridesToIgnore, LayoutOverrides nonExplicitOverrides);

    void SetChildRect(BaseFrameworkElement child, Rect rect, LayoutOverrides layoutOverrides, LayoutOverrides overridesToIgnore, LayoutOverrides nonExplicitOverrides, SetRectMode setRectMode);

    void SetRootSize(BaseFrameworkElement root, Size size, bool setWidth, bool setHeight);

    LayoutOverrides ComputeOverrides(BaseFrameworkElement element);

    void SetHorizontalAlignment(BaseFrameworkElement element, HorizontalAlignment alignment);

    void SetVerticalAlignment(BaseFrameworkElement element, VerticalAlignment alignment);

    LayoutConstraintMode GetWidthConstraintMode(BaseFrameworkElement parent, BaseFrameworkElement child);

    LayoutConstraintMode GetWidthConstraintMode(BaseFrameworkElement child);

    LayoutConstraintMode GetHeightConstraintMode(BaseFrameworkElement parent, BaseFrameworkElement child);

    LayoutConstraintMode GetHeightConstraintMode(BaseFrameworkElement child);

    LayoutCacheRecord CacheLayout(BaseFrameworkElement element);

    void SetLayoutFromCache(BaseFrameworkElement element, LayoutCacheRecord layoutCacheRecord, Rect boundsOfAllCaches);

    List<LayoutCacheRecord> GetCurrentRects(BaseFrameworkElement parent);

    void SetCurrentRects(BaseFrameworkElement parent, List<LayoutCacheRecord> layoutCache);

    IEnumerable<IPropertyId> GetLayoutProperties();

    void ClearUnusedLayoutProperties(BaseFrameworkElement element);

    void FillChild(BaseFrameworkElement element);
  }
}
