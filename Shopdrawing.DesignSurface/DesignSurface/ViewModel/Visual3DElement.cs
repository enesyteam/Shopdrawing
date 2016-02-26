// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.Visual3DElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public abstract class Visual3DElement : Base3DElement
  {
    public static readonly IPropertyId TransformProperty = (IPropertyId) PlatformTypes.Visual3D.GetMember(MemberType.LocalProperty, "Transform", MemberAccessTypes.Public);

    public override Transform3D Transform
    {
      get
      {
        return (Transform3D) this.GetComputedValue(Visual3DElement.TransformProperty);
      }
      set
      {
        this.SetValue(Visual3DElement.TransformProperty, (object) value);
      }
    }

    public override IPropertyId TransformPropertyId
    {
      get
      {
        return Visual3DElement.TransformProperty;
      }
    }
  }
}
