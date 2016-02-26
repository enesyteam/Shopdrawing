// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SetupFontEmbeddingCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.SceneCommands;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  internal class SetupFontEmbeddingCommand : SceneCommandBase
  {
    protected override ViewState RequiredSelectionViewState
    {
      get
      {
        return ViewState.None;
      }
    }

    public override bool IsEnabled
    {
      get
      {
        if (base.IsEnabled)
          return FontEmbedder.IsSubsetFontTargetInstalled((ITypeResolver) this.SceneViewModel.ProjectContext);
        return false;
      }
    }

    public SetupFontEmbeddingCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    public override void Execute()
    {
      FontEmbeddingDialog fontEmbeddingDialog = new FontEmbeddingDialog(this.SceneViewModel);
      bool? nullable = fontEmbeddingDialog.ShowDialog();
      if (!nullable.HasValue || !nullable.Value)
        return;
      fontEmbeddingDialog.Model.CommitChanges();
    }
  }
}
