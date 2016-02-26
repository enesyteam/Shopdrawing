// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.EyedropperTool
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal sealed class EyedropperTool : Tool
  {
    public override string Identifier
    {
      get
      {
        return "Eyedropper";
      }
    }

    public override string Caption
    {
      get
      {
        return StringTable.EyedropperToolCaption;
      }
    }

    public override Key Key
    {
      get
      {
        return Key.I;
      }
    }

    public override string IconBrushKey
    {
      get
      {
        return "eyedropper";
      }
    }

    public override ToolCategory Category
    {
      get
      {
        return ToolCategory.Eyedropper;
      }
    }

    protected override bool UseDefaultEditingAdorners
    {
      get
      {
        return false;
      }
    }

    public override bool KeepTextSelectionSet
    {
      get
      {
        return true;
      }
    }

    public EyedropperTool(ToolContext toolContext)
      : base(toolContext)
    {
    }

    protected override ToolBehavior CreateToolBehavior()
    {
      return (ToolBehavior) new PropertyToolBehavior(this.GetActiveViewContext(), PropertyToolAction.Eyedropper);
    }
  }
}
