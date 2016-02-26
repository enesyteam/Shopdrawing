// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.LastSelectionModifierKeyBehaviorFactory
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

namespace Microsoft.Expression.DesignSurface.Tools
{
  public class LastSelectionModifierKeyBehaviorFactory : IModifierKeyBehaviorFactory
  {
    public bool ShouldActivate(ExtendedModifierKeys modifiers)
    {
      if (modifiers != ExtendedModifierKeys.Control && modifiers != ExtendedModifierKeys.ControlAlt && modifiers != ExtendedModifierKeys.ControlShift)
        return modifiers == ExtendedModifierKeys.ControlAltShift;
      return true;
    }

    public ToolBehavior CreateInstance(ToolBehaviorContext context)
    {
      Tool activeTool = context.ToolManager.ActiveTool;
      Tool tool = (Tool) null;
      if (activeTool is ISelectionTool)
        return (ToolBehavior) null;
      if (activeTool is CameraOrbitTool)
        return (ToolBehavior) null;
      if (context.ToolManager.LastSelectionTool != null)
        tool = context.ToolManager.LastSelectionTool;
      if (tool == null)
        return (ToolBehavior) null;
      context.ToolManager.OverrideTool = tool;
      return ((ISelectionTool) tool).CreateToolBehavior(context);
    }
  }
}
