// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.ProjectionCameraElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public abstract class ProjectionCameraElement : CameraElement
  {
    public static readonly IPropertyId PositionProperty = (IPropertyId) PlatformTypes.ProjectionCamera.GetMember(MemberType.LocalProperty, "Position", MemberAccessTypes.Public);
    public static readonly IPropertyId LookDirectionProperty = (IPropertyId) PlatformTypes.ProjectionCamera.GetMember(MemberType.LocalProperty, "LookDirection", MemberAccessTypes.Public);
    public static readonly IPropertyId UpDirectionProperty = (IPropertyId) PlatformTypes.ProjectionCamera.GetMember(MemberType.LocalProperty, "UpDirection", MemberAccessTypes.Public);

    public Vector3D LookDirection
    {
      get
      {
        return (Vector3D) this.GetLocalValue(ProjectionCameraElement.LookDirectionProperty);
      }
      set
      {
        this.SetValue(ProjectionCameraElement.LookDirectionProperty, (object) value);
      }
    }

    public Point3D Position
    {
      get
      {
        return (Point3D) this.GetLocalValue(ProjectionCameraElement.PositionProperty);
      }
      set
      {
        this.SetValue(ProjectionCameraElement.PositionProperty, (object) value);
      }
    }

    public Vector3D Up
    {
      get
      {
        return (Vector3D) this.GetLocalValue(ProjectionCameraElement.UpDirectionProperty);
      }
      set
      {
        this.SetValue(ProjectionCameraElement.UpDirectionProperty, (object) value);
      }
    }
  }
}
