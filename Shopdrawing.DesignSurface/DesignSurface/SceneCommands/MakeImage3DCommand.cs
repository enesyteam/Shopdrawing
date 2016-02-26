// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.MakeImage3DCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class MakeImage3DCommand : ConvertElementCommand
  {
    private GeometryModel3DElement newGeometryElement;

    public override bool IsAvailable
    {
      get
      {
        return JoltHelper.TypeSupported((ITypeResolver) this.SceneViewModel.ProjectContext, PlatformTypes.ModelVisual3D);
      }
    }

    public override bool IsEnabled
    {
      get
      {
        if (!base.IsEnabled)
          return false;
        SceneElementSelectionSet elementSelectionSet = this.SceneViewModel.ElementSelectionSet;
        if (elementSelectionSet.Count == 1)
          return PlatformTypes.Image.IsAssignableFrom((ITypeId) elementSelectionSet.PrimarySelection.Type);
        return false;
      }
    }

    protected override string UndoUnitName
    {
      get
      {
        return StringTable.UndoUnitMakeImage3D;
      }
    }

    protected override bool CreateResource
    {
      get
      {
        return false;
      }
    }

    protected override bool ShouldReplaceOriginal
    {
      get
      {
        return true;
      }
    }

    protected override ITypeId Type
    {
      get
      {
        return PlatformTypes.ImageBrush;
      }
    }

    public MakeImage3DCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    protected override DocumentNode CreateValue(BaseFrameworkElement source)
    {
      ImageBrushNode imageBrush;
      this.newGeometryElement = GeometryCreationHelper3D.ConvertImageTo3D(this.DesignerContext.ActiveSceneViewModel, source, source.Name + "Model", out imageBrush);
      return imageBrush.DocumentNode;
    }

    protected override BaseFrameworkElement CreateElement(BaseFrameworkElement originalElement)
    {
      return (BaseFrameworkElement) this.DesignerContext.ActiveSceneViewModel.CreateSceneNode(typeof (Viewport3D));
    }

    protected override void Postprocess(BaseFrameworkElement originalElement, BaseFrameworkElement newElement, Dictionary<IPropertyId, SceneNode> properties, Rect layoutRect)
    {
      Viewport3DElement viewport = (Viewport3DElement) newElement;
      this.AddModel3DContainerToViewport(viewport, (Model3DElement) this.newGeometryElement);
      Camera perspectiveCamera = Helper3D.CreateEnclosingPerspectiveCamera(45.0, layoutRect.Width / layoutRect.Height, this.newGeometryElement.DesignTimeBounds, 1.0);
      viewport.Camera = (CameraElement) this.DesignerContext.ActiveSceneViewModel.CreateSceneNode((object) perspectiveCamera);
      AmbientLightElement ambientLightElement = (AmbientLightElement) this.DesignerContext.ActiveSceneViewModel.CreateSceneNode(typeof (AmbientLight));
      ambientLightElement.Name = "Ambient";
      ambientLightElement.SetValue(LightElement.ColorProperty, (object) Color.FromRgb((byte) sbyte.MinValue, (byte) sbyte.MinValue, (byte) sbyte.MinValue));
      this.AddModel3DContainerToViewport(viewport, (Model3DElement) ambientLightElement);
      DirectionalLightElement directionalLightElement = (DirectionalLightElement) this.DesignerContext.ActiveSceneViewModel.CreateSceneNode(typeof (DirectionalLight));
      directionalLightElement.Name = "Directional";
      directionalLightElement.SetValue(LightElement.ColorProperty, (object) Color.FromRgb((byte) 127, (byte) 127, (byte) 127));
      directionalLightElement.SetValue(DirectionalLightElement.DirectionProperty, (object) new Vector3D(0.0, 0.0, -1.0));
      directionalLightElement.Transform = (Transform3D) new TranslateTransform3D(new Vector3D(0.0, 0.0, 3.0));
      this.AddModel3DContainerToViewport(viewport, (Model3DElement) directionalLightElement);
    }

    private void AddModel3DContainerToViewport(Viewport3DElement viewport, Model3DElement model)
    {
      ModelVisual3DElement modelVisual3Delement = (ModelVisual3DElement) this.DesignerContext.ActiveSceneViewModel.CreateSceneNode(typeof (ModelVisual3D));
      modelVisual3Delement.Name = model.Name + "Container";
      modelVisual3Delement.Content = (SceneNode) model;
      viewport.Children.Add((Visual3DElement) modelVisual3Delement);
    }
  }
}
