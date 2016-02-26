// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.EditTextCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools.Text;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal class EditTextCommand : SceneCommandBase
  {
    public override bool IsEnabled
    {
      get
      {
        if (base.IsEnabled && this.SceneViewModel.ElementSelectionSet.Count == 1 && TextEditProxyFactory.IsEditableElement(this.SceneViewModel.ElementSelectionSet.PrimarySelection))
          return !this.SceneView.EventRouter.IsEditingText;
        return false;
      }
    }

    public EditTextCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    public override void Execute()
    {
      if (!this.IsEnabled)
        return;
      this.SceneView.TryEnterTextEditMode(false);
    }
  }
}
