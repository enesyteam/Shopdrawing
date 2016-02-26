// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.BindingModeInfo
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Windows.Data;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class BindingModeInfo
  {
    public BindingMode Mode { get; private set; }

    public bool IsOptional { get; private set; }

    public BindingModeInfo(BindingMode mode, bool isOptional)
    {
      this.Mode = mode;
      this.IsOptional = isOptional;
    }

    public override string ToString()
    {
      return this.Mode.ToString() + " - " + (this.IsOptional ? "optional" : "mandatory");
    }
  }
}
