// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Designers.CanvasLayoutDesigner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Designers
{
  internal class CanvasLayoutDesigner : LayoutDesigner
  {
    private static IPropertyId[] canvasLayoutProperties = new IPropertyId[4]
    {
      CanvasElement.LeftProperty,
      CanvasElement.TopProperty,
      CanvasElement.RightProperty,
      CanvasElement.BottomProperty
    };

    public CanvasLayoutDesigner()
      : base((IEnumerable<IPropertyId>) CanvasLayoutDesigner.canvasLayoutProperties)
    {
    }

    protected override LayoutOperation CreateLayoutOperation(BaseFrameworkElement child)
    {
      return (LayoutOperation) new CanvasLayoutOperation((ILayoutDesigner) this, child);
    }

    public override void SetChildRect(BaseFrameworkElement child, Rect rect, LayoutOverrides layoutOverrides, LayoutOverrides overridesToIgnore, LayoutOverrides nonExplicitOverrides, SetRectMode setRectMode)
    {
      overridesToIgnore |= LayoutOverrides.Margin;
      base.SetChildRect(child, rect, layoutOverrides, overridesToIgnore, nonExplicitOverrides, setRectMode);
    }
  }
}
