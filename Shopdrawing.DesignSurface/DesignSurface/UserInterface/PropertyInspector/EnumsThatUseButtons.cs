// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.EnumsThatUseButtons
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  internal sealed class EnumsThatUseButtons
  {
    private static List<ITypeId> enumsThatUseButtons = new List<ITypeId>();
    private static List<ITypeId> enumsThatUseToggleButtons = new List<ITypeId>();

    static EnumsThatUseButtons()
    {
      EnumsThatUseButtons.enumsThatUseToggleButtons.Add(PlatformTypes.VerticalAlignment);
      EnumsThatUseButtons.enumsThatUseToggleButtons.Add(PlatformTypes.HorizontalAlignment);
      EnumsThatUseButtons.enumsThatUseToggleButtons.Add(ProjectNeutralTypes.Orientation);
    }

    private EnumsThatUseButtons()
    {
    }

    public static bool ShouldUseButtonsForEnum(ITypeId type)
    {
      return null != EnumsThatUseButtons.enumsThatUseButtons.Find((Predicate<ITypeId>) (target => target.IsAssignableFrom(type)));
    }

    public static bool ShouldUseToggleButtonsForEnum(ITypeId type)
    {
      return null != EnumsThatUseButtons.enumsThatUseToggleButtons.Find((Predicate<ITypeId>) (target => target.IsAssignableFrom(type)));
    }
  }
}
