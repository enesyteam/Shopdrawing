// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.EditNewTemplateCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal class EditNewTemplateCommand : ReplaceStyleTemplateCommand
  {
    protected override string UndoString
    {
      get
      {
        return StringTable.UndoUnitCreateTemplate;
      }
    }

    public EditNewTemplateCommand(ISceneViewHost viewHost, SceneViewModel viewModel, IPropertyId targetProperty)
      : base(viewHost, viewModel, targetProperty)
    {
    }

    public EditNewTemplateCommand(ISceneViewHost viewHost, SceneViewModel viewModel)
      : base(viewHost, viewModel, (IPropertyId) null, true)
    {
    }

    protected override DocumentNode ProvideValue(out IList<DocumentCompositeNode> auxillaryResources)
    {
      auxillaryResources = (IList<DocumentCompositeNode>) null;
      FrameworkTemplateElement frameworkTemplateElement = (FrameworkTemplateElement) this.SceneViewModel.CreateSceneNode((ITypeId) this.TargetProperty.PropertyType);
      if (ControlStylingOperations.DoesPropertyAffectRoot((IPropertyId) this.TargetProperty))
      {
        ControlTemplateElement controlTemplateElement = frameworkTemplateElement as ControlTemplateElement;
        if (controlTemplateElement != null)
          controlTemplateElement.ControlTemplateTargetTypeId = (ITypeId) this.Type;
      }
      BaseFrameworkElement frameworkElement = !PlatformTypes.ItemsPanelTemplate.Equals((object) frameworkTemplateElement.Type) ? (BaseFrameworkElement) this.SceneViewModel.CreateSceneNode(PlatformTypes.Grid) : (BaseFrameworkElement) this.SceneViewModel.CreateSceneNode(PlatformTypes.StackPanel);
      frameworkTemplateElement.DefaultInsertionPoint.Insert((SceneNode) frameworkElement);
      return frameworkTemplateElement.DocumentNode;
    }
  }
}
