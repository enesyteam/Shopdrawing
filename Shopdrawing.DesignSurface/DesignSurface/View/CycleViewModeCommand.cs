// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.CycleViewModeCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Commands;
using System;

namespace Microsoft.Expression.DesignSurface.View
{
  internal sealed class CycleViewModeCommand : Command
  {
    private static readonly int NumberOfViewModes = Enum.GetValues(typeof (ViewMode)).Length;
    private SceneView sceneView;

    public CycleViewModeCommand(SceneView sceneView)
    {
      this.sceneView = sceneView;
    }

    private ViewMode GetNextViewMode(ViewMode viewMode)
    {
      return (ViewMode) ((int) (viewMode + 1) % CycleViewModeCommand.NumberOfViewModes);
    }

    public override void Execute()
    {
      this.sceneView.ViewMode = this.GetNextViewMode(this.sceneView.ViewMode);
    }
  }
}
