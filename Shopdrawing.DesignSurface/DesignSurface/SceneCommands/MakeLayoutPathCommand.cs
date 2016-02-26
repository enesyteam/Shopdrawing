// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.MakeLayoutPathCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Designers;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Project;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class MakeLayoutPathCommand : SceneCommandBase
  {
    private BaseFrameworkElement SelectedElement
    {
      get
      {
        return this.SceneViewModel.ElementSelectionSet.PrimarySelection as BaseFrameworkElement;
      }
    }

    public override bool IsEnabled
    {
      get
      {
        if (base.IsEnabled && this.SceneViewModel.ElementSelectionSet.Count == 1 && (this.SelectedElement != null && this.SelectedElement.Parent != null) && (this.SceneViewModel.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsPathLayout) && BlendSdkHelper.IsCurrentVersionSdkFrameworkName(this.SceneViewModel.ProjectContext.TargetFramework) && !ProjectNeutralTypes.PathListBox.IsAssignableFrom((ITypeId) this.SelectedElement.Type)))
          return !ProjectNeutralTypes.PathPanel.IsAssignableFrom((ITypeId) this.SelectedElement.Type);
        return false;
      }
    }

    public MakeLayoutPathCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    public override void Execute()
    {
      BaseFrameworkElement selectedElement = this.SelectedElement;
      SceneViewModel sceneViewModel1 = this.SceneViewModel;
      bool flag1 = true;
      BaseFrameworkElement frameworkElement = selectedElement;
      int num1 = flag1 ? true : false;
      ILayoutDesigner designerForChild = sceneViewModel1.GetLayoutDesignerForChild((SceneElement) frameworkElement, num1 != 0);
      SceneNode parent = selectedElement.Parent;
      IProperty propertyForChild = parent.GetPropertyForChild((SceneNode) selectedElement);
      ISceneNodeCollection<SceneNode> collectionForProperty = parent.GetCollectionForProperty((IPropertyId) propertyForChild);
      int index = collectionForProperty.IndexOf((SceneNode) selectedElement);
      if (!BehaviorHelper.EnsureBlendSDKLibraryAssemblyReferenced(this.SceneViewModel, "Microsoft.Expression.Controls") || !ProjectContext.GetProjectContext(selectedElement.ProjectContext).IsTypeSupported(ProjectNeutralTypes.PathListBox))
        return;
      using (this.SceneViewModel.DisableUpdateChildrenOnAddAndRemove())
      {
        SceneViewModel sceneViewModel2 = this.SceneViewModel;
        bool flag2 = false;
        string unitMakeLayoutPath = StringTable.UndoUnitMakeLayoutPath;
        int num2 = flag2 ? true : false;
        using (SceneEditTransaction editTransaction = sceneViewModel2.CreateEditTransaction(unitMakeLayoutPath, num2 != 0))
        {
          using (this.SceneViewModel.ForceBaseValue())
          {
            using (this.SceneViewModel.DisableDrawIntoState())
            {
              this.SceneViewModel.ElementSelectionSet.Clear();
              Rect childRect = designerForChild.GetChildRect(selectedElement);
              selectedElement.EnsureNamed();
              PathListBoxElement pathListBoxElement = (PathListBoxElement) this.SceneViewModel.CreateSceneNode(ProjectNeutralTypes.PathListBox);
              LayoutPathNode layoutPathNode = (LayoutPathNode) this.SceneViewModel.CreateSceneNode(ProjectNeutralTypes.LayoutPath);
              BindingSceneNode bindingSceneNode = (BindingSceneNode) this.SceneViewModel.CreateSceneNode(PlatformTypes.Binding);
              bindingSceneNode.ElementName = selectedElement.Name;
              layoutPathNode.SetValue(LayoutPathNode.SourceElementProperty, (object) bindingSceneNode.DocumentNode);
              pathListBoxElement.LayoutPaths.Add((SceneNode) layoutPathNode);
              if (!collectionForProperty.FixedCapacity.HasValue || collectionForProperty.Count < collectionForProperty.FixedCapacity.Value)
              {
                collectionForProperty.Insert(index, (SceneNode) pathListBoxElement);
              }
              else
              {
                GridElement gridElement = (GridElement) this.SceneViewModel.CreateSceneNode(PlatformTypes.Grid);
                collectionForProperty[index] = (SceneNode) gridElement;
                gridElement.Children.Add((SceneNode) pathListBoxElement);
                gridElement.Children.Add((SceneNode) selectedElement);
              }
              editTransaction.Update();
              designerForChild.SetChildRect((BaseFrameworkElement) pathListBoxElement, childRect);
              this.SceneViewModel.ElementSelectionSet.SetSelection((SceneElement) pathListBoxElement);
              editTransaction.Commit();
            }
          }
        }
      }
    }
  }
}
