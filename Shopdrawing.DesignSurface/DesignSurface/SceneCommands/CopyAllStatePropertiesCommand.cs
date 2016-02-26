// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.CopyAllStatePropertiesCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal class CopyAllStatePropertiesCommand : CopyStatePropertiesCommand
  {
    protected override string UndoString
    {
      get
      {
        return StringTable.CopyAllStatePropertiesCommandUndoUnit;
      }
    }

    public override string CommandName
    {
      get
      {
        return StringTable.CopyAllStatePropertiesCommandName;
      }
    }

    public CopyAllStatePropertiesCommand(SceneViewModel viewModel, VisualStateSceneNode sourceState)
      : base(viewModel, sourceState)
    {
    }

    protected override List<TimelineSceneNode> GetAnimationsToCopy()
    {
      List<TimelineSceneNode> list = new List<TimelineSceneNode>();
      list.AddRange((IEnumerable<TimelineSceneNode>) this.SourceState.Storyboard.Children);
      return list;
    }
  }
}
