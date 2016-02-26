// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.Viewport2DVisual3DElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class Viewport2DVisual3DElement : UIElement3DElement
  {
    public static readonly Viewport2DVisual3DElement.Viewport2DVisual3DElementFactory Factory = new Viewport2DVisual3DElement.Viewport2DVisual3DElementFactory();

    public override Rect3D LocalSpaceBounds
    {
      get
      {
        Rect3D rect3D = Rect3D.Empty;
        if (this.ViewObject != null)
        {
          Viewport2DVisual3D viewport2Dvisual3D = (Viewport2DVisual3D) this.ViewObject.PlatformSpecificObject;
          if (viewport2Dvisual3D != null && viewport2Dvisual3D.Geometry != null)
            rect3D = viewport2Dvisual3D.Geometry.Bounds;
        }
        return rect3D;
      }
    }

    public class Viewport2DVisual3DElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new Viewport2DVisual3DElement();
      }

      public Viewport2DVisual3DElement Instantiate(SceneViewModel viewModel)
      {
        return (Viewport2DVisual3DElement) this.Instantiate(viewModel, PlatformTypes.Viewport2DVisual3D);
      }
    }
  }
}
