// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.ToolBehaviorContext
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.View;

namespace Microsoft.Expression.DesignSurface.Tools
{
  public sealed class ToolBehaviorContext
  {
    private ToolContext toolContext;
    private Tool tool;
    private SceneView view;

    public Tool Tool
    {
      get
      {
        return this.tool;
      }
    }

    public Tool ActiveTool
    {
      get
      {
        return this.ToolManager.ActiveTool;
      }
    }

    public SceneView View
    {
      get
      {
        return this.view;
      }
    }

    public IPropertyManager PropertyManager
    {
      get
      {
        return this.toolContext.PropertyManager;
      }
    }

    public IAmbientPropertyManager AmbientPropertyManager
    {
      get
      {
        return this.toolContext.AmbientPropertyManager;
      }
    }

    public ToolManager ToolManager
    {
      get
      {
        return this.toolContext.ToolManager;
      }
    }

    internal SnappingEngine SnappingEngine
    {
      get
      {
        return this.toolContext.SnappingEngine;
      }
    }

    internal ToolBehaviorContext(ToolContext toolContext, Tool tool, SceneView view)
    {
      this.toolContext = toolContext;
      this.tool = tool;
      this.view = view;
    }
  }
}
