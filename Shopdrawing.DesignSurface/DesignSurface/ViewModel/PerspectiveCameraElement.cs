// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.PerspectiveCameraElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class PerspectiveCameraElement : ProjectionCameraElement
  {
    public static readonly IPropertyId FieldOfViewProperty = (IPropertyId) PlatformTypes.PerspectiveCamera.GetMember(MemberType.LocalProperty, "FieldOfView", MemberAccessTypes.Public);
    public static readonly PerspectiveCameraElement.ConcretePerspectiveCameraElementFactory Factory = new PerspectiveCameraElement.ConcretePerspectiveCameraElementFactory();

    public double FieldOfView
    {
      get
      {
        return (double) this.GetLocalValue(PerspectiveCameraElement.FieldOfViewProperty);
      }
      set
      {
        this.SetValue(PerspectiveCameraElement.FieldOfViewProperty, (object) value);
      }
    }

    public class ConcretePerspectiveCameraElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new PerspectiveCameraElement();
      }

      public PerspectiveCameraElement Instantiate(SceneViewModel viewModel)
      {
        return (PerspectiveCameraElement) this.Instantiate(viewModel, PlatformTypes.PerspectiveCamera);
      }
    }
  }
}
