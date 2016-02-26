// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Properties.TransformProperties
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;

namespace Microsoft.Expression.DesignSurface.Properties
{
  public static class TransformProperties
  {
    public static ReferenceStep GetCenterXStep(IPlatformTypes platformTypes)
    {
      return (ReferenceStep) platformTypes.GetProperty(platformTypes.DefaultTypeResolver, typeof (CanonicalTransform), MemberType.LocalProperty, "CenterX");
    }

    public static ReferenceStep GetCenterYStep(IPlatformTypes platformTypes)
    {
      return (ReferenceStep) platformTypes.GetProperty(platformTypes.DefaultTypeResolver, typeof (CanonicalTransform), MemberType.LocalProperty, "CenterY");
    }
  }
}
