// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.PencilTool
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Tools.Path;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal sealed class PencilTool : Tool
  {
    public override string Identifier
    {
      get
      {
        return "Pencil";
      }
    }

    public override string Caption
    {
      get
      {
        return StringTable.PencilToolCaption;
      }
    }

    public override string Description
    {
      get
      {
        return StringTable.StackedToolDescription;
      }
    }

    public override Key Key
    {
      get
      {
        return Key.Y;
      }
    }

    public override string IconBrushKey
    {
      get
      {
        return "pencil";
      }
    }

    public override ToolCategory Category
    {
      get
      {
        return ToolCategory.Drawing;
      }
    }

    public PencilTool(ToolContext toolContext)
      : base(toolContext)
    {
    }

    protected override ToolBehavior CreateToolBehavior()
    {
      return (ToolBehavior) new PencilCreateBehavior(this.GetActiveViewContext());
    }
  }
}
