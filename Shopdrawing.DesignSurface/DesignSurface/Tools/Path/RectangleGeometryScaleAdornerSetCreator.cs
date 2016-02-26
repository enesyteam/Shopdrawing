// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.RectangleGeometryScaleAdornerSetCreator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  internal sealed class RectangleGeometryScaleAdornerSetCreator : RectangleGeometryAdornerSetCreatorBase
  {
    protected override IAdornerSet CreateRectangleGeometryAdornerSet(ToolBehaviorContext toolContext, BaseFrameworkElement element)
    {
      return (IAdornerSet) new RectangleGeometryScaleAdornerSet(toolContext, element);
    }
  }
}
