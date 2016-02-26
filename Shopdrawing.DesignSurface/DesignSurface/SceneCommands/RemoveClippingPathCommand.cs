// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.RemoveClippingPathCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class RemoveClippingPathCommand : SceneCommandBase
  {
    public override bool IsEnabled
    {
      get
      {
        if (!base.IsEnabled)
          return false;
        foreach (SceneElement sceneElement in this.SceneViewModel.ElementSelectionSet.Selection)
        {
          if (PlatformTypes.UIElement.IsAssignableFrom((ITypeId) sceneElement.Type) && sceneElement.IsSet(Base2DElement.ClipProperty) == PropertyState.Set)
            return true;
        }
        return false;
      }
    }

    public RemoveClippingPathCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    public override void Execute()
    {
      using (SceneEditTransaction editTransaction = this.SceneViewModel.CreateEditTransaction(StringTable.UndoUnitRemoveClippingPath, false))
      {
        using (this.SceneViewModel.ForceBaseValue())
        {
          foreach (SceneElement sceneElement in this.SceneViewModel.ElementSelectionSet.Selection)
          {
            if (PlatformTypes.UIElement.IsAssignableFrom((ITypeId) sceneElement.Type))
            {
              this.ReleaseClippingPath(sceneElement, editTransaction);
              sceneElement.ClearValue(Base2DElement.ClipProperty);
            }
          }
        }
        editTransaction.Commit();
      }
    }

    private void ReleaseClippingPath(SceneElement sceneElement, SceneEditTransaction editTransaction)
    {
      if (sceneElement.IsSet(Base2DElement.ClipProperty) != PropertyState.Set)
        return;
      SceneNode sceneNode = (SceneNode) sceneElement;
      for (SceneNode parent = sceneElement.Parent; parent != null; parent = parent.Parent)
      {
        PanelElement panelElement = parent as PanelElement;
        if (panelElement != null)
        {
          MatrixTransform matrixTransform = new MatrixTransform(sceneElement.GetComputedTransformToElement((SceneElement) panelElement));
          PathGeometry pathGeometry1 = new PathGeometry();
          System.Windows.Media.Geometry geometry = (System.Windows.Media.Geometry)sceneElement.GetLocalOrDefaultValueAsWpf(Base2DElement.ClipProperty);
          if (geometry != null)
            pathGeometry1 = PathGeometryUtilities.TransformGeometry(geometry, (Transform) matrixTransform);
          PathElement pathElement = (PathElement) this.SceneViewModel.CreateSceneNode(PlatformTypes.Path);
          using (pathElement.ViewModel.ForceBaseValue())
          {
            this.DesignerContext.AmbientPropertyManager.ApplyAmbientProperties((SceneNode) pathElement);
            pathElement.SetValueAsWpf(ShapeElement.StrokeProperty, (object) Brushes.Black);
            pathElement.ClearValue(ShapeElement.FillProperty);
            pathElement.ClearValue(ShapeElement.StrokeThicknessProperty);
            panelElement.Children.Insert(panelElement.Children.IndexOf(sceneNode) + 1, (SceneNode) pathElement);
            ReferenceStep singleStep1 = (ReferenceStep) pathElement.ProjectContext.ResolveProperty(Base2DElement.ClipProperty);
            ReferenceStep singleStep2 = (ReferenceStep) pathElement.ProjectContext.ResolveProperty(PathElement.DataProperty);
            PathCommandHelper.MoveVertexAnimations(sceneElement, new PropertyReference(singleStep1), (SceneElement) pathElement, new PropertyReference(singleStep2), (Transform) matrixTransform);
            Rect bounds = PathCommandHelper.InflateRectByStrokeWidth(pathGeometry1.Bounds, pathElement, false);
            Rect maxAnimatedExtent = PathCommandHelper.FindMaxAnimatedExtent((SceneElement) pathElement, bounds, new PropertyReference(singleStep2));
            editTransaction.Update();
            Vector vector = new Vector(-maxAnimatedExtent.Left, -maxAnimatedExtent.Top);
            panelElement.LayoutDesigner.SetChildRect((BaseFrameworkElement) pathElement, maxAnimatedExtent);
            Transform transform = (Transform) new TranslateTransform(vector.X, vector.Y);
            PathGeometry pathGeometry2 = PathGeometryUtilities.TransformGeometry((System.Windows.Media.Geometry)pathGeometry1, transform);
            pathElement.PathGeometry = pathGeometry2;
            PathCommandHelper.TransformPointKeyframes((SceneElement) pathElement, new PropertyReference(singleStep2), transform);
            pathElement.SetValueAsWpf(ShapeElement.StretchProperty, (object) (Stretch) (pathElement.HasVertexAnimations ? 0 : 1));
            break;
          }
        }
        else
          sceneNode = parent;
      }
    }
  }
}
