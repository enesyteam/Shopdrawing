// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.MakeSameWidthCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Designers;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal class MakeSameWidthCommand : MakeSameSizeCommandBase
  {
    public override string UndoDescription
    {
      get
      {
        return StringTable.MakeSameWidthDescription;
      }
    }

    public MakeSameWidthCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    public override void SetSize(Rect sourceRect, BaseFrameworkElement target, ILayoutDesigner targetLayoutDesigner)
    {
      Rect childRect = targetLayoutDesigner.GetChildRect(target);
      childRect.Width = sourceRect.Width;
      targetLayoutDesigner.SetChildRect(target, childRect, true, false);
    }
  }
}
