// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.GroupCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class GroupCommand : SceneCommandBase
  {
    public override bool IsEnabled
    {
      get
      {
        if (!base.IsEnabled)
          return false;
        SceneElementSelectionSet elementSelectionSet = this.SceneViewModel.ElementSelectionSet;
        if (elementSelectionSet.IsEmpty)
          return false;
        SceneElement sceneElement1 = this.SceneViewModel.RootNode as SceneElement;
        if (sceneElement1 != null && elementSelectionSet.Selection.Contains(sceneElement1))
          return false;
        SceneElement parentElement = elementSelectionSet.PrimarySelection.ParentElement;
        bool flag = true;
        if (elementSelectionSet.PrimarySelection is BaseFrameworkElement || !(elementSelectionSet.PrimarySelection is Base3DElement))
          return false;
        foreach (SceneElement sceneElement2 in elementSelectionSet.Selection)
        {
          if (sceneElement2 is CameraElement || sceneElement2 is Model3DElement && !(elementSelectionSet.PrimarySelection is Model3DElement) || sceneElement2 is Visual3DElement && !(elementSelectionSet.PrimarySelection is Visual3DElement))
            return false;
          if (sceneElement2.ParentElement != parentElement)
            flag = false;
        }
        return flag;
      }
    }

    public GroupCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    public override object GetProperty(string propertyName)
    {
      if (propertyName == "IsVisible")
        return (object) (bool) (!GroupCommandHelpers.IsGrouping2D(this.SceneViewModel) ? true : false);
      return base.GetProperty(propertyName);
    }

    public override void Execute()
    {
      using (SceneEditTransaction editTransaction = this.SceneViewModel.CreateEditTransaction(StringTable.UndoUnitCreateGroup))
      {
        SceneElement selectionToSet = this.GroupElements();
        if (selectionToSet != null)
          this.SceneViewModel.ElementSelectionSet.SetSelection(selectionToSet);
        editTransaction.Commit();
      }
    }

    private SceneElement GroupElements()
    {
      ReadOnlyCollection<SceneElement> selection = this.SceneViewModel.ElementSelectionSet.Selection;
      if (selection[0] is BaseFrameworkElement)
        return (SceneElement) null;
      if (!(selection[0] is Base3DElement))
        return (SceneElement) null;
      SceneElement primarySelection = this.SceneViewModel.ElementSelectionSet.PrimarySelection;
      this.SceneViewModel.ElementSelectionSet.Clear();
      List<SceneElement> list = new List<SceneElement>((IEnumerable<SceneElement>) selection);
      Base3DElement base3Delement1 = (Base3DElement) null;
      int num = int.MaxValue;
      Viewport3DElement viewport = ((Base3DElement) list[0]).Viewport;
      foreach (Base3DElement base3Delement2 in list)
      {
        if (base3Delement2.Viewport == viewport)
        {
          int depthFromViewport3D = base3Delement2.DepthFromViewport3D;
          if (depthFromViewport3D < num)
          {
            base3Delement1 = base3Delement2;
            num = depthFromViewport3D;
          }
        }
      }
      SceneElement parentElement = base3Delement1.ParentElement;
      Matrix3D matrix3D = Matrix3D.Identity;
      if (parentElement is Base3DElement)
      {
        matrix3D = ((Base3DElement) parentElement).GetComputedTransformFromViewport3DToElement();
        matrix3D.Invert();
      }
      Base3DElement parent = (Base3DElement) null;
      if (list[0] is Model3DElement)
      {
        Model3DGroup model3Dgroup = new Model3DGroup();
        Model3DCollection model3Dcollection = new Model3DCollection();
        model3Dgroup.Children = model3Dcollection;
        parent = (Base3DElement) this.SceneViewModel.CreateSceneNode((object) model3Dgroup);
      }
      else if (list[0] is Visual3DElement)
        parent = (Base3DElement) this.SceneViewModel.CreateSceneNode((object) new ModelVisual3D());
      SceneNodeIDHelper sceneNodeIdHelper = new SceneNodeIDHelper(this.SceneViewModel, (SceneNode) this.SceneViewModel.ActiveSceneInsertionPoint.SceneElement.StoryboardContainer);
      string validElementId = sceneNodeIdHelper.GetValidElementID((SceneNode) parent, "group3D");
      sceneNodeIdHelper.SetLocalName((SceneNode) parent, validElementId);
      using (this.SceneViewModel.ForceBaseValue())
      {
        Matrix3D[] matrix3DArray = new Matrix3D[list.Count];
        for (int index = 0; index < list.Count; ++index)
          matrix3DArray[index] = ((Base3DElement) list[index]).GetComputedTransformFromViewport3DToElement();
        foreach (Base3DElement base3Delement2 in list)
        {
          if (base3Delement2 != primarySelection)
            base3Delement2.Remove();
        }
        int insertAt = primarySelection.GetCollectionContainer().IndexOf((SceneNode) primarySelection);
        primarySelection.Remove();
        if (parentElement is Viewport3DElement)
          GroupCommandHelpers.InsertElement(parentElement, (SceneNode) parent, insertAt);
        else
          GroupCommandHelpers.InsertElement(this.SceneViewModel, (Base3DElement) parentElement, (SceneNode) parent, new int?(insertAt));
        for (int index = 0; index < list.Count; ++index)
        {
          GroupCommandHelpers.InsertElement(this.SceneViewModel, parent, (SceneNode) list[index], new int?());
          ((Base3DElement) list[index]).Transform = (Transform3D) new CanonicalTransform3D(matrix3DArray[index] * matrix3D);
        }
      }
      return (SceneElement) parent;
    }
  }
}
