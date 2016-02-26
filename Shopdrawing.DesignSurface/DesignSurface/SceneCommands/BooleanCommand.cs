// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.BooleanCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Geometry;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal abstract class BooleanCommand : SceneCommandBase
  {
    private string undoDescription;

    public override bool IsEnabled
    {
      get
      {
        if (!base.IsEnabled)
          return false;
        SceneElementSelectionSet elementSelectionSet = this.SceneViewModel.ElementSelectionSet;
        if (elementSelectionSet.Count < 2)
          return false;
        foreach (SceneElement element in elementSelectionSet.Selection)
        {
          if (!PlatformTypes.Shape.IsAssignableFrom((ITypeId) element.Type) && !PathConversionHelper.CanConvert(element))
            return false;
        }
        return true;
      }
    }

    public BooleanCommand(SceneViewModel viewModel, string undoDescription)
      : base(viewModel)
    {
      this.undoDescription = undoDescription;
    }

    public override void Execute()
    {
      using (SceneEditTransaction editTransaction = this.SceneViewModel.CreateEditTransaction(this.undoDescription))
      {
        this.ConvertSelectedElementsToPathIfNecessary();
        editTransaction.Update();
        this.SceneViewModel.DefaultView.UpdateLayout();
        SceneElementSelectionSet elementSelectionSet = this.SceneViewModel.ElementSelectionSet;
        SceneElement primarySelection = elementSelectionSet.PrimarySelection;
        System.Windows.Media.Geometry renderedGeometryAsWpf = this.SceneView.GetRenderedGeometryAsWpf(primarySelection);
        if (renderedGeometryAsWpf == null)
          return;
        this.Initialize(renderedGeometryAsWpf);
        SceneElementCollection elementCollection = new SceneElementCollection();
        foreach (SceneElement shapeElement in elementSelectionSet.Selection)
        {
          if (shapeElement != primarySelection)
          {
            elementCollection.Add(shapeElement);
            System.Windows.Media.Geometry secondaryGeometry = this.SceneView.GetRenderedGeometryAsWpf(shapeElement);
            if (secondaryGeometry == null)
              return;
            Matrix transformToElement = shapeElement.GetComputedTransformToElement(primarySelection);
            if (!transformToElement.IsIdentity)
            {
              MatrixTransform matrixTransform = new MatrixTransform(transformToElement);
              matrixTransform.Freeze();
              GeometryGroup geometryGroup = new GeometryGroup();
              geometryGroup.Children.Add(secondaryGeometry);
              geometryGroup.Transform = (Transform) matrixTransform;
              secondaryGeometry = (System.Windows.Media.Geometry)geometryGroup;
            }
            this.Combine(secondaryGeometry);
          }
        }
        PathGeometry result = this.GetResult();
        BooleanCommand.CleanUpPathGeometry(ref result);
        PathGeometry pathGeometry = PathConversionHelper.RemoveDegeneratePoints((System.Windows.Media.Geometry)result);
        elementSelectionSet.Clear();
        PathGeometryUtilities.CollapseSingleSegmentsToPolySegments(pathGeometry);
        PathElement pathElement = (PathElement) this.SceneViewModel.CreateSceneNode(PlatformTypes.Path);
        Dictionary<IPropertyId, SceneNode> properties = (Dictionary<IPropertyId, SceneNode>) null;
        using (this.SceneViewModel.DisableUpdateChildrenOnAddAndRemove())
        {
          this.SceneViewModel.AnimationEditor.DeleteAllAnimationsInSubtree(primarySelection);
          properties = SceneElementHelper.StoreProperties((SceneNode) primarySelection);
          ISceneNodeCollection<SceneNode> collectionContainer = primarySelection.GetCollectionContainer();
          int index = collectionContainer.IndexOf((SceneNode) primarySelection);
          collectionContainer[index] = (SceneNode) pathElement;
        }
        foreach (SceneElement element in elementCollection)
        {
          this.SceneViewModel.AnimationEditor.DeleteAllAnimationsInSubtree(element);
          element.Remove();
        }
        using (this.SceneViewModel.ForceBaseValue())
        {
          if (properties != null)
            SceneElementHelper.ApplyProperties((SceneNode) pathElement, properties);
          pathElement.SetValueAsWpf(ShapeElement.StretchProperty, (object) Stretch.Fill);
          PathCommandHelper.ReplacePathGeometry(pathElement, pathGeometry, editTransaction);
        }
        elementSelectionSet.SetSelection((SceneElement) pathElement);
        editTransaction.Commit();
      }
    }

    protected abstract void Initialize(System.Windows.Media.Geometry primaryGeometry);

    protected abstract void Combine(System.Windows.Media.Geometry secondaryGeometry);

    protected abstract PathGeometry GetResult();

    public static void CleanUpPathGeometry(ref PathGeometry geometry)
    {
      if (geometry == null)
        return;
      if (geometry.Figures.Count == 0)
      {
        geometry = (PathGeometry) null;
      }
      else
      {
        geometry = PathGeometryUtilities.Copy(geometry, false);
        foreach (PathFigure pathFigure in geometry.Figures)
        {
          foreach (PathSegment pathSegment in pathFigure.Segments)
          {
            pathSegment.ClearValue(PathSegment.IsStrokedProperty);
            pathSegment.ClearValue(PathSegment.IsSmoothJoinProperty);
          }
        }
      }
    }

    private void ConvertSelectedElementsToPathIfNecessary()
    {
      using (this.SceneViewModel.DisableUpdateChildrenOnAddAndRemove())
      {
        using (SceneEditTransaction editTransaction = this.SceneViewModel.CreateEditTransaction(StringTable.UndoUnitConvertToPath, true))
        {
          PathCommandHelper.ConvertSelectionToPathIfNeeded(this.SceneViewModel.ElementSelectionSet, new SceneElementFilter(BooleanCommand.IsElementToPathConversionNecessary));
          editTransaction.Commit();
        }
      }
    }

    private static bool IsElementToPathConversionNecessary(SceneElement element)
    {
      if (PathConversionHelper.CanConvert(element))
        return !PlatformTypes.Shape.IsAssignableFrom((ITypeId) element.Type);
      return false;
    }
  }
}
