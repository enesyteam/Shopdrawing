// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.EditNewStyleWithTemplateCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal class EditNewStyleWithTemplateCommand : ReplaceStyleTemplateCommand
  {
    protected override string UndoString
    {
      get
      {
        return StringTable.UndoUnitCreateStyle;
      }
    }

    public EditNewStyleWithTemplateCommand(ISceneViewHost viewHost, SceneViewModel viewModel, IPropertyId targetProperty)
      : base(viewHost, viewModel, targetProperty)
    {
    }

    protected override DocumentNode ProvideValue(out IList<DocumentCompositeNode> auxillaryResources)
    {
      auxillaryResources = (IList<DocumentCompositeNode>) null;
      StyleNode emptyStyle = StyleNode.CreateEmptyStyle(this.SceneViewModel, (IPropertyId) this.TargetProperty, (ITypeId) this.Type);
      ControlTemplateElement controlTemplateElement = (ControlTemplateElement) this.SceneViewModel.CreateSceneNode(PlatformTypes.ControlTemplate);
      controlTemplateElement.ControlTemplateTargetTypeId = (ITypeId) emptyStyle.StyleTargetTypeId;
      BaseFrameworkElement frameworkElement = (BaseFrameworkElement) this.SceneViewModel.CreateSceneNode(PlatformTypes.Grid);
      controlTemplateElement.DefaultInsertionPoint.Insert((SceneNode) frameworkElement);
      emptyStyle.SetValue(ControlElement.TemplateProperty, (object) controlTemplateElement.DocumentNode);
      return emptyStyle.DocumentNode;
    }

    protected override DocumentNodePath ProvideEditingContainer(SceneElement targetElement, PropertyReference targetProperty, DocumentNode resourceNode)
    {
      SceneViewModel viewModel = targetElement.ViewModel.GetViewModel(resourceNode.DocumentRoot, false);
      DocumentNodePath documentNodePath = targetElement.GetLocalValueAsDocumentNode(targetProperty) ?? new DocumentNodePath(resourceNode.DocumentRoot.RootNode, resourceNode);
      DocumentNode documentNode = (viewModel.GetSceneNode(resourceNode) as StyleNode).GetLocalValueAsSceneNode(ControlElement.TemplateProperty).DocumentNode;
      return documentNodePath.GetPathInContainer((DocumentNode) documentNode.Parent).GetPathInSubContainer(documentNode.SitePropertyKey, documentNode);
    }
  }
}
