// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.ModalCommands.UndoCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.SceneCommands.ModalCommands
{
  internal sealed class UndoCommand : ModalCommandBase
  {
    public override bool IsEnabled
    {
      get
      {
        return this.SceneViewModel.Document.CanUndo;
      }
    }

    public override string Description
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.UndoContextualDescription, new object[1]
        {
          (object) this.SceneViewModel.Document.UndoDescription
        });
      }
    }

    public UndoCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    public override void Execute()
    {
      this.SceneViewModel.Document.Undo();
    }
  }
}
