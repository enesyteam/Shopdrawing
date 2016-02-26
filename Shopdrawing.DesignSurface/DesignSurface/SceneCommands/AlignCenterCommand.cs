// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.AlignCenterCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class AlignCenterCommand : AlignCommand
  {
    protected override string UndoDescription
    {
      get
      {
        return StringTable.UndoUnitAlignCenterHorizontally;
      }
    }

    public AlignCenterCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    protected override Vector GetOffset(Rect primaryBoundingBox, Rect elementBoundingBox)
    {
      return new Vector((primaryBoundingBox.Left + primaryBoundingBox.Right) / 2.0 - (elementBoundingBox.Left + elementBoundingBox.Right) / 2.0, 0.0);
    }
  }
}
