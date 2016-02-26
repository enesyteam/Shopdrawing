// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.PanelDataHost
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class PanelDataHost : PanelDataHostBase
  {
    protected PanelDataHost(ITypeId panelType)
      : base(panelType)
    {
    }

    public static PanelDataHostBase CreatePlatformPanel(ITypeId panelTypeId, IPlatform platform)
    {
      ITypeId typeId = (ITypeId) platform.Metadata.ResolveType(panelTypeId);
      if (platform.Metadata.IsNullType(typeId))
        return (PanelDataHostBase) null;
      return !PlatformTypes.Grid.IsAssignableFrom(typeId) ? (PanelDataHostBase) new PanelDataHost(typeId) : (PanelDataHostBase) new GridDataHost(typeId);
    }
  }
}
