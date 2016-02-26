// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.ClearPartAssignmentCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.UserInterface.SkinEditing;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal class ClearPartAssignmentCommand : SceneCommandBase
  {
    public override bool IsAvailable
    {
      get
      {
        if (base.IsAvailable)
          return this.DesignerContext.ActiveSceneViewModel.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsTemplateParts);
        return false;
      }
    }

    public override bool IsEnabled
    {
      get
      {
        return base.IsEnabled && this.IsAvailable && (this.SceneViewModel.PartsModel.ShowPartsList && this.SceneViewModel.ElementSelectionSet.Count == 1) && this.SceneViewModel.PartsModel.GetPartStatus((SceneNode) this.DesignerContext.SelectionManager.ElementSelectionSet.PrimarySelection) != PartStatus.Unused;
      }
    }

    public ClearPartAssignmentCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    public override void Execute()
    {
      SceneElement primarySelection = this.DesignerContext.SelectionManager.ElementSelectionSet.PrimarySelection;
      using (SceneEditTransaction editTransaction = this.DesignerContext.ActiveSceneViewModel.CreateEditTransaction(StringTable.UndoClearPartAssignment))
      {
        primarySelection.ClearValue(primarySelection.NameProperty);
        editTransaction.Commit();
      }
    }
  }
}
