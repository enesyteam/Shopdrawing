// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.GenericShapeTool
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Tools.Path;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal class GenericShapeTool : Tool
  {
    private ShapeCreateBehavior shapeCreateBehavior;
    private ITypeId shapeType;
    private ToolCategory category;

    public override string Identifier
    {
      get
      {
        return this.shapeType.Name;
      }
    }

    public override string Caption
    {
      get
      {
        return this.shapeType.Name;
      }
    }

    public override string Description
    {
      get
      {
        return StringTable.GenericControlToolDescription;
      }
    }

    public override Key Key
    {
      get
      {
        return Key.None;
      }
    }

    public override DrawingBrush NormalIconBrush
    {
      get
      {
        return this.CreateDrawingBrushFromType(this.shapeType, false);
      }
    }

    public override DrawingBrush HoverIconBrush
    {
      get
      {
        return this.CreateDrawingBrushFromType(this.shapeType, true);
      }
    }

    public override bool IsVisible
    {
      get
      {
        if (this.ToolContext.ActiveView != null)
          return JoltHelper.TypeSupported((ITypeResolver) this.ToolContext.ActiveView.ViewModel.ProjectContext, this.shapeType);
        return true;
      }
    }

    public override ToolCategory Category
    {
      get
      {
        return this.category;
      }
    }

    protected ITypeId ShapeType
    {
      get
      {
        return this.shapeType;
      }
    }

    public GenericShapeTool(ToolContext toolContext, ITypeId shapeType, ToolCategory category)
      : base(toolContext)
    {
      this.shapeType = shapeType;
      this.category = category;
    }

    protected override ToolBehavior CreateToolBehavior()
    {
      this.shapeCreateBehavior = new ShapeCreateBehavior(this.GetActiveViewContext(), this.shapeType);
      return (ToolBehavior) this.shapeCreateBehavior;
    }

    protected override void OnDoubleClick()
    {
      this.CreateDefaultElement(this.shapeType);
    }
  }
}
