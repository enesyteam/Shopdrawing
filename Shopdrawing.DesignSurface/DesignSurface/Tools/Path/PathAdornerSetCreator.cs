// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.PathAdornerSetCreator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  internal sealed class PathAdornerSetCreator : IAdornerSetCreator
  {
    private ElementToPathEditorTargetMap pathEditorTargetMap;
    private PathEditMode pathEditMode;

    public PathAdornerSetCreator(ElementToPathEditorTargetMap pathEditorTargetMap, PathEditMode pathEditMode)
    {
      this.pathEditorTargetMap = pathEditorTargetMap;
      this.pathEditMode = pathEditMode;
    }

    public IAdornerSet CreateAdornerSet(ToolBehaviorContext toolContext, SceneElement adornedElement)
    {
      BaseFrameworkElement adornedElement1 = adornedElement as BaseFrameworkElement;
      if (adornedElement1 == null)
        return (IAdornerSet) null;
      if (this.pathEditMode == PathEditMode.ClippingPath && RectangleGeometryAdornerSetCreatorBase.ShouldApplyRectangleGeometryAdornerTo(adornedElement))
        return (IAdornerSet) null;
      PathEditorTarget pathEditorTarget = this.pathEditorTargetMap.GetPathEditorTarget((Base2DElement) adornedElement1, this.pathEditMode);
      if (pathEditorTarget == null)
        return (IAdornerSet) null;
      return (IAdornerSet) new PathAdornerSet(toolContext, adornedElement1, pathEditorTarget);
    }
  }
}
