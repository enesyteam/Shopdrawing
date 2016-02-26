// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.ToolCategoryGroup
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools
{
  public abstract class ToolCategoryGroup
  {
    private ToolCategory category;

    public Tool ActiveTool { get; set; }

    public abstract Key Key { get; }

    public ToolCategory Category
    {
      get
      {
        return this.category;
      }
    }

    protected ToolCategoryGroup(ToolCategory category)
    {
      this.category = category;
    }

    public bool Activate(ToolManager toolManager)
    {
      if (!this.ActiveTool.IsEnabled || !this.ActiveTool.IsVisible)
        return false;
      toolManager.ActiveTool = this.ActiveTool;
      return true;
    }
  }
}
