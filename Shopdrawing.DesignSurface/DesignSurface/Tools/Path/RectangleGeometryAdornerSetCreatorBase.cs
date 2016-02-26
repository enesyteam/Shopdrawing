// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.RectangleGeometryAdornerSetCreatorBase
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  internal abstract class RectangleGeometryAdornerSetCreatorBase : IAdornerSetCreator
  {
    public static bool ShouldApplyRectangleGeometryAdornerTo(SceneElement adornedElement)
    {
      DocumentNodePath valueAsDocumentNode = adornedElement.GetLocalValueAsDocumentNode(Base2DElement.ClipProperty);
      return valueAsDocumentNode != null && valueAsDocumentNode.Node != null && PlatformTypes.RectangleGeometry.IsAssignableFrom((ITypeId) valueAsDocumentNode.Node.Type);
    }

    public IAdornerSet CreateAdornerSet(ToolBehaviorContext toolContext, SceneElement adornedElement)
    {
      BaseFrameworkElement element = adornedElement as BaseFrameworkElement;
      if (element == null)
        return (IAdornerSet) null;
      if (RectangleGeometryAdornerSetCreatorBase.ShouldApplyRectangleGeometryAdornerTo(adornedElement))
        return this.CreateRectangleGeometryAdornerSet(toolContext, element);
      return (IAdornerSet) null;
    }

    protected abstract IAdornerSet CreateRectangleGeometryAdornerSet(ToolBehaviorContext toolContext, BaseFrameworkElement element);
  }
}
