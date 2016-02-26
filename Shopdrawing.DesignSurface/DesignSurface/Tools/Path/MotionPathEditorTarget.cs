// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.MotionPathEditorTarget
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  public sealed class MotionPathEditorTarget : PathEditorTarget
  {
    private Base2DElement editingElement;

    public override PathEditMode PathEditMode
    {
      get
      {
        return PathEditMode.MotionPath;
      }
    }

    public override Base2DElement EditingElement
    {
      get
      {
        return this.editingElement;
      }
    }

    public MotionPathEditorTarget(Base2DElement editingElement)
      : base(editingElement.ViewModel)
    {
      this.editingElement = editingElement;
      this.UpdateCachedPath();
    }

    public override void UpdateFromDamage(SceneUpdatePhaseEventArgs args)
    {
      this.AddCriticalEdit();
      bool flag = false;
      PathAnimationSceneNode pathAnimation = this.GetPathAnimation();
      if ((args.DirtyViewState & SceneViewModel.ViewStateBits.ActiveTimeline) != SceneViewModel.ViewStateBits.None || args.IsRadicalChange || pathAnimation != null && args.DocumentChanges.Contains(pathAnimation.DocumentNode))
        flag = true;
      if (pathAnimation != null)
      {
        SceneNode valueAsSceneNode = pathAnimation.GetLocalValueAsSceneNode(PathAnimationSceneNode.PathGeometryProperty);
        if (valueAsSceneNode != null && args.DocumentChanges.Contains(valueAsSceneNode.DocumentNode))
          flag = true;
      }
      if (!flag && this.ViewModel.AnimationEditor.ActiveStoryboardTimeline != null)
      {
        DocumentNode documentNode = this.ViewModel.AnimationEditor.ActiveStoryboardTimeline.DocumentNode;
        foreach (DocumentNodeChange documentNodeChange in args.DocumentChanges.DistinctChanges)
        {
          if (documentNodeChange.ParentNode != null && documentNodeChange.ParentNode.Parent == documentNode)
          {
            flag = true;
            break;
          }
        }
      }
      if (!flag)
        return;
      this.UpdateCachedPath();
    }

    public override void PostDeleteAction()
    {
      if (!PathGeometryUtilities.IsEmpty(this.PathGeometry))
        return;
      this.ViewModel.AnimationEditor.DeleteMotionPath((SceneNode) this.EditingElement);
    }

    public override void RefreshSubscription()
    {
    }

    public override Matrix GetTransformToAncestor(IViewObject ancestorViewObject)
    {
      return MotionPathAdorner.ComputeBaseValueMatrix((SceneElement) this.editingElement, ancestorViewObject);
    }

    public override Matrix RefineTransformToAdornerLayer(Matrix matrix)
    {
      return MotionPathAdorner.RefineTransformToAdornerLayer((SceneElement) this.editingElement, matrix);
    }

    public override void RemovePath()
    {
      this.EndEditing(false);
      this.ViewModel.AnimationEditor.DeleteMotionPath((SceneNode) this.editingElement);
    }

    public override void UpdateCachedPath()
    {
      PathAnimationSceneNode pathAnimation = this.GetPathAnimation();
      this.PathGeometry = pathAnimation == null || pathAnimation.Path == null ? new PathGeometry() : PathGeometryUtilities.Copy(pathAnimation.Path, true);
    }

    protected override void EndEditingInternal(bool pathJustCreated)
    {
      this.ViewModel.AnimationEditor.SetMotionPath((SceneElement) this.editingElement, PathGeometryUtilities.Copy(this.PathGeometry, true), new double?(), new double?());
    }

    private PathAnimationSceneNode GetPathAnimation()
    {
      PropertyReference transformTranslationX = this.EditingElement.Platform.Metadata.CommonProperties.RenderTransformTranslationX;
      if (this.ViewModel.AnimationEditor.ActiveStoryboardTimeline != null)
        return this.ViewModel.AnimationEditor.ActiveStoryboardTimeline.GetAnimation((SceneNode) this.editingElement, transformTranslationX) as PathAnimationSceneNode;
      return (PathAnimationSceneNode) null;
    }
  }
}
