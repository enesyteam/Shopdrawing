// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.EditCopyOfTemplateCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal class EditCopyOfTemplateCommand : ReplaceStyleTemplateCommand
  {
    protected override string UndoString
    {
      get
      {
        return StringTable.UndoUnitEditCopyTemplate;
      }
    }

    public override bool IsEnabled
    {
      get
      {
        if (!base.IsEnabled)
          return false;
        Type runtimeType = this.SceneViewModel.ProjectContext.ResolveType(PlatformTypes.FrameworkTemplate).RuntimeType;
        object computedValue = this.TargetElement.GetComputedValue(this.TargetPropertyReference);
        if (computedValue != null)
          return runtimeType.IsAssignableFrom(computedValue.GetType());
        return false;
      }
    }

    public EditCopyOfTemplateCommand(ISceneViewHost viewHost, SceneViewModel viewModel, IPropertyId targetProperty)
      : base(viewHost, viewModel, targetProperty)
    {
    }

    protected override DocumentNode ProvideValue(out IList<DocumentCompositeNode> auxillaryResources)
    {
      return this.ProvideCurrentTemplate(this.TargetElement, this.TargetPropertyReference, out auxillaryResources);
    }
  }
}
