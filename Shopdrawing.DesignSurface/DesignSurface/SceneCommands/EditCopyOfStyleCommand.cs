// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.EditCopyOfStyleCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal class EditCopyOfStyleCommand : ReplaceStyleTemplateCommand
  {
    protected override string UndoString
    {
      get
      {
        return StringTable.UndoUnitEditCopyStyle;
      }
    }

    public override bool IsEnabled
    {
      get
      {
        IList<DocumentCompositeNode> auxillaryResources;
        if (!base.IsEnabled || this.ResolveCurrentStyle(this.TargetElement, this.TargetPropertyReference, true) == null && this.ProvideCurrentStyle(this.TargetElement, this.Type, this.TargetPropertyReference, true, out auxillaryResources) == null)
          return false;
        if (this.TargetProperty.Equals((object) BaseFrameworkElement.FocusVisualStyleProperty))
          return this.TargetElement.IsSet(this.TargetPropertyReference) == PropertyState.Set;
        return true;
      }
    }

    public EditCopyOfStyleCommand(ISceneViewHost viewHost, SceneViewModel viewModel, IPropertyId targetProperty)
      : base(viewHost, viewModel, targetProperty)
    {
    }

    protected override DocumentNode ProvideValue(out IList<DocumentCompositeNode> auxillaryResources)
    {
      return this.ProvideCurrentStyle(this.TargetElement, this.Type, this.TargetPropertyReference, false, out auxillaryResources);
    }
  }
}
