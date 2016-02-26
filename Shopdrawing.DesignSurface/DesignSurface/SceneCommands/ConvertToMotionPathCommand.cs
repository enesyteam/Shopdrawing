// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.ConvertToMotionPathCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Geometry;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class ConvertToMotionPathCommand : SceneCommandBase
  {
    public override bool IsAvailable
    {
      get
      {
        if (JoltHelper.TypeSupported((ITypeResolver) this.SceneViewModel.ProjectContext, PlatformTypes.DoubleAnimationUsingPath) && JoltHelper.TypeSupported((ITypeResolver) this.SceneViewModel.ProjectContext, PlatformTypes.MatrixAnimationUsingPath))
          return JoltHelper.TypeSupported((ITypeResolver) this.SceneViewModel.ProjectContext, PlatformTypes.PointAnimationUsingPath);
        return false;
      }
    }

    public override bool IsEnabled
    {
      get
      {
        if (!base.IsEnabled || this.SceneViewModel.ElementSelectionSet.Count < 1 || this.SceneViewModel.ElementSelectionSet.Count > 2)
          return false;
        SceneElement pathTarget = this.GetPathTarget(this.SceneViewModel.ElementSelectionSet);
        SceneElement animationTarget = this.GetAnimationTarget(this.SceneViewModel.ElementSelectionSet);
        return pathTarget != null && pathTarget.IsViewObjectValid && (animationTarget == null || animationTarget is BaseFrameworkElement && animationTarget != this.SceneViewModel.ActiveEditingContainer && animationTarget.IsViewObjectValid);
      }
    }

    public ConvertToMotionPathCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    public override void Execute()
    {
      AnimationEditor animationEditor = this.SceneViewModel.AnimationEditor;
      SceneElement pathTarget = this.GetPathTarget(this.SceneViewModel.ElementSelectionSet);
      BaseFrameworkElement frameworkElement = (BaseFrameworkElement) this.GetAnimationTarget(this.SceneViewModel.ElementSelectionSet);
      if (frameworkElement == null)
      {
        SceneElement root = this.SceneViewModel.ActiveEditingContainer as SceneElement;
        if (root != null)
          frameworkElement = PathTargetDialog.ChooseMotionPathTarget(root, pathTarget);
      }
      if (frameworkElement == null)
        return;
      PathGeometry geometry = PathConversionHelper.ConvertToPathGeometry(pathTarget);
      if (PathGeometryUtilities.TotalSegmentCount(geometry) == 0)
      {
        this.DesignerContext.MessageDisplayService.ShowError(StringTable.ConvertToMotionPathNoSegmentsMessage);
      }
      else
      {
        using (SceneEditTransaction editTransaction = this.SceneViewModel.CreateEditTransaction(StringTable.ConvertToMotionPathUndo, false))
        {
          StoryboardTimelineSceneNode targetTimeline = this.GetTargetTimeline();
          if (targetTimeline == null)
            this.SceneViewModel.AnimationEditor.CreateNewTimeline(TriggerCreateBehavior.Default);
          else if (targetTimeline != this.SceneViewModel.ActiveStoryboardTimeline)
            this.SceneViewModel.SetActiveStoryboardTimeline(targetTimeline.StoryboardContainer, targetTimeline, (TriggerBaseNode) null);
          this.SceneViewModel.ElementSelectionSet.Clear();
          Point elementCoordinates = frameworkElement.RenderTransformOriginInElementCoordinates;
          Matrix matrix = new TranslateTransform(-elementCoordinates.X, -elementCoordinates.Y).Value;
          Transform transform = (Transform) new MatrixTransform(pathTarget.GetComputedTransformToElement((SceneElement) frameworkElement) * frameworkElement.GetEffectiveRenderTransform(false) * matrix);
          PathGeometry path = PathGeometryUtilities.TransformGeometry((System.Windows.Media.Geometry)geometry, transform);
          double animationTime = animationEditor.AnimationTime;
          animationEditor.SetMotionPath((SceneElement) frameworkElement, path, new double?(animationTime), new double?(animationTime + 2.0));
          this.SceneViewModel.ElementSelectionSet.SetSelection((SceneElement) frameworkElement);
          editTransaction.Commit();
        }
      }
    }

    private StoryboardTimelineSceneNode GetTargetTimeline()
    {
      if (this.SceneViewModel.ActiveStoryboardTimeline != null)
        return this.SceneViewModel.ActiveStoryboardTimeline;
      return this.SceneViewModel.AnimationEditor.FirstActiveStoryboard;
    }

    private SceneElement GetPathTarget(SceneElementSelectionSet selection)
    {
      SceneElement primarySelection = selection.PrimarySelection;
      SceneElement sceneElement = (SceneElement) null;
      if (selection.Count == 1 && PathConversionHelper.CanConvert(selection.PrimarySelection))
        sceneElement = selection.PrimarySelection;
      if (selection.Count == 2)
      {
        SceneElement element = selection.Selection[0] == primarySelection ? selection.Selection[1] : selection.Selection[0];
        if (PathConversionHelper.CanConvert(element))
          sceneElement = element;
        else if (PathConversionHelper.CanConvert(primarySelection))
          sceneElement = primarySelection;
      }
      return sceneElement;
    }

    private SceneElement GetAnimationTarget(SceneElementSelectionSet selection)
    {
      SceneElement sceneElement = (SceneElement) null;
      if (selection.Count == 2)
      {
        SceneElement primarySelection = selection.PrimarySelection;
        SceneElement element = selection.Selection[0] == primarySelection ? selection.Selection[1] : selection.Selection[0];
        if (PathConversionHelper.CanConvert(element))
          sceneElement = primarySelection;
        else if (PathConversionHelper.CanConvert(primarySelection))
          sceneElement = element;
      }
      return sceneElement;
    }
  }
}
