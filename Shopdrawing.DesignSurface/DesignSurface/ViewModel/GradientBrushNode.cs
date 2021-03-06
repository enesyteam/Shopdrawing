﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.GradientBrushNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class GradientBrushNode : BrushNode
  {
    public static readonly IPropertyId GradientStopsProperty = (IPropertyId) PlatformTypes.GradientBrush.GetMember(MemberType.LocalProperty, "GradientStops", MemberAccessTypes.Public);
    public static readonly IPropertyId MappingModeProperty = (IPropertyId) PlatformTypes.GradientBrush.GetMember(MemberType.LocalProperty, "MappingMode", MemberAccessTypes.Public);
    public static readonly IPropertyId SpreadMethodProperty = (IPropertyId) PlatformTypes.GradientBrush.GetMember(MemberType.LocalProperty, "SpreadMethod", MemberAccessTypes.Public);
    public static readonly GradientBrushNode.ConcreteGradientBrushNodeFactory Factory = new GradientBrushNode.ConcreteGradientBrushNodeFactory();

    public class ConcreteGradientBrushNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new GradientBrushNode();
      }
    }
  }
}
