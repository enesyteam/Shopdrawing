// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.CameraOrbitTool
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal sealed class CameraOrbitTool : Tool
  {
    public override bool IsVisible
    {
      get
      {
        if (this.ToolContext.ActiveView != null)
          return JoltHelper.TypeSupported((ITypeResolver) this.ToolContext.ActiveView.ViewModel.ProjectContext, PlatformTypes.Viewport3D);
        return true;
      }
    }

    public override bool IsEnabled
    {
      get
      {
        return this.IsActiveViewValid;
      }
    }

    public override string Identifier
    {
      get
      {
        return "CameraOrbitTool";
      }
    }

    public override string Caption
    {
      get
      {
        return StringTable.CameraOrbitToolCaption;
      }
    }

    public override Key Key
    {
      get
      {
        return Key.O;
      }
    }

    public override string IconBrushKey
    {
      get
      {
        return "cameraorbit";
      }
    }

    public override ToolCategory Category
    {
      get
      {
        return ToolCategory.ThreeD;
      }
    }

    protected override bool UseDefaultEditingAdorners
    {
      get
      {
        return false;
      }
    }

    public CameraOrbitTool(ToolContext toolContext)
      : base(toolContext)
    {
    }

    protected override ToolBehavior CreateToolBehavior()
    {
      return (ToolBehavior) new CameraOrbitToolBehavior(this.GetActiveViewContext());
    }
  }
}
