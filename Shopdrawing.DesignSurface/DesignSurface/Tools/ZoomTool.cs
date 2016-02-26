// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.ZoomTool
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.View;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal sealed class ZoomTool : Tool
  {
    public override string Identifier
    {
      get
      {
        return "Zoom";
      }
    }

    public override string Caption
    {
      get
      {
        return StringTable.ZoomToolCaption;
      }
    }

    public override string Description
    {
      get
      {
        return StringTable.ZoomToolDescription;
      }
    }

    public override Key Key
    {
      get
      {
        return Key.Z;
      }
    }

    public override string IconBrushKey
    {
      get
      {
        return "zoom";
      }
    }

    public override ToolCategory Category
    {
      get
      {
        return ToolCategory.Zoom;
      }
    }

    public override bool IsEnabled
    {
      get
      {
        return this.IsActiveViewValid;
      }
    }

    protected override bool UseDefaultEditingAdorners
    {
      get
      {
        return false;
      }
    }

    protected override ViewState RequiredActiveElementViewState
    {
      get
      {
        return ViewState.None;
      }
    }

    public override bool KeepTextSelectionSet
    {
      get
      {
        return true;
      }
    }

    public ZoomTool(ToolContext toolContext)
      : base(toolContext)
    {
    }

    protected override ToolBehavior CreateToolBehavior()
    {
      return (ToolBehavior) new ZoomToolBehavior(this.GetActiveViewContext());
    }

    protected override void OnDoubleClick()
    {
      if (this.ActiveView == null)
        return;
      this.ActiveView.Zoom = 1.0;
    }
  }
}
