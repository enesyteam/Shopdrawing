// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.LineTool
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Tools.Path;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal sealed class LineTool : Tool
  {
    public override string Identifier
    {
      get
      {
        return "Line";
      }
    }

    public override string Caption
    {
      get
      {
        return StringTable.LineToolCaption;
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
        return Key.Oem5;
      }
    }

    public override string IconBrushKey
    {
      get
      {
        return "line";
      }
    }

    public override ToolCategory Category
    {
      get
      {
        return ToolCategory.ShapePrimitives;
      }
    }

    public LineTool(ToolContext toolContext)
      : base(toolContext)
    {
    }

    protected override ToolBehavior CreateToolBehavior()
    {
      return (ToolBehavior) new LineCreateBehavior(this.GetActiveViewContext());
    }
  }
}
