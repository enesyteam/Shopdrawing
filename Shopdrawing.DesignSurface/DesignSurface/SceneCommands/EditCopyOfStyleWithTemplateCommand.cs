// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.EditCopyOfStyleWithTemplateCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal class EditCopyOfStyleWithTemplateCommand : EditCopyOfStyleCommand
  {
    public EditCopyOfStyleWithTemplateCommand(ISceneViewHost viewHost, SceneViewModel viewModel, IPropertyId targetProperty)
      : base(viewHost, viewModel, targetProperty)
    {
    }

    protected override DocumentNodePath ProvideEditingContainer(SceneElement targetElement, PropertyReference targetProperty, DocumentNode resourceNode)
    {
      return ControlStylingOperations.ResolveNodePathForTemplateWithinExistingStyle(targetElement, targetProperty) ?? base.ProvideEditingContainer(targetElement, targetProperty, resourceNode);
    }
  }
}
