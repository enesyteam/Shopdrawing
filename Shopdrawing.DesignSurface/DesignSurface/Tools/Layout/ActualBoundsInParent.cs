// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Layout.ActualBoundsInParent
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Tools.Layout
{
  internal sealed class ActualBoundsInParent
  {
    private Dictionary<IViewObject, Rect> layoutBounds = new Dictionary<IViewObject, Rect>();
    private SceneElement parent;

    public Rect this[IViewVisual childVisual]
    {
      get
      {
        Rect actualBoundsInParent;
        if (!this.layoutBounds.TryGetValue((IViewObject) childVisual, out actualBoundsInParent))
          this.layoutBounds[(IViewObject) childVisual] = actualBoundsInParent = this.parent.ViewModel.DefaultView.GetActualBoundsInParent((IViewObject) childVisual);
        return actualBoundsInParent;
      }
    }

    public ActualBoundsInParent(SceneElement parent)
    {
      this.parent = parent;
    }
  }
}
