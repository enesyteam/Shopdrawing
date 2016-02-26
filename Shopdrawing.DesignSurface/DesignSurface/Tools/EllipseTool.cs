// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.EllipseTool
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Tools.Path;
using Microsoft.Expression.DesignSurface.UserInterface;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal sealed class EllipseTool : Tool
  {
    public override string Identifier
    {
      get
      {
        return "Ellipse";
      }
    }

    public override DrawingBrush NormalIconBrush
    {
      get
      {
        return IconMapper.GetDrawingBrushForType(PlatformTypes.Ellipse, false, 24, 24);
      }
    }

    public override DrawingBrush HoverIconBrush
    {
      get
      {
        return IconMapper.GetDrawingBrushForType(PlatformTypes.Ellipse, true, 24, 24);
      }
    }

    public override string Caption
    {
      get
      {
        return StringTable.EllipseToolCaption;
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
        return Key.L;
      }
    }

    public override string IconBrushKey
    {
      get
      {
        return "ellipse";
      }
    }

    public override ToolCategory Category
    {
      get
      {
        return ToolCategory.ShapePrimitives;
      }
    }

    public EllipseTool(ToolContext toolContext)
      : base(toolContext)
    {
    }

    protected override ToolBehavior CreateToolBehavior()
    {
      return (ToolBehavior) new ShapeCreateBehavior(this.GetActiveViewContext(), PlatformTypes.Ellipse);
    }

    protected override void OnDoubleClick()
    {
      this.CreateDefaultElement(PlatformTypes.Ellipse);
    }
  }
}
