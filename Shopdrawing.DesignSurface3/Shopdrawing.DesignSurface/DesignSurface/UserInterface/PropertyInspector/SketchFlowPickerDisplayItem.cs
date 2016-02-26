// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SketchFlowPickerDisplayItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  internal class SketchFlowPickerDisplayItem
  {
    public string GroupName { get; private set; }

    public string DisplayName { get; private set; }

    public string XamlName { get; private set; }

    public SketchFlowPickerDisplayItem(string groupName, string displayName, string xamlName)
    {
      this.GroupName = groupName;
      this.DisplayName = displayName;
      this.XamlName = xamlName;
    }

    public override string ToString()
    {
      return this.DisplayName;
    }
  }
}
