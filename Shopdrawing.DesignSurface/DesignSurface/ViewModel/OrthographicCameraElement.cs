// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.OrthographicCameraElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class OrthographicCameraElement : ProjectionCameraElement
  {
    public static readonly IPropertyId WidthProperty = (IPropertyId) PlatformTypes.OrthographicCamera.GetMember(MemberType.LocalProperty, "Width", MemberAccessTypes.Public);
    public static readonly OrthographicCameraElement.ConcreteOrthographicCameraElementFactory Factory = new OrthographicCameraElement.ConcreteOrthographicCameraElementFactory();

    public double Width
    {
      get
      {
        return (double) this.GetLocalValue(OrthographicCameraElement.WidthProperty);
      }
      set
      {
        this.SetValue(OrthographicCameraElement.WidthProperty, (object) value);
      }
    }

    public class ConcreteOrthographicCameraElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new OrthographicCameraElement();
      }

      public OrthographicCameraElement Instantiate(SceneViewModel viewModel)
      {
        return (OrthographicCameraElement) this.Instantiate(viewModel, PlatformTypes.OrthographicCamera);
      }
    }
  }
}
