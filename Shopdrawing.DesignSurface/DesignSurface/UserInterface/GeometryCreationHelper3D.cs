// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.GeometryCreationHelper3D
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public static class GeometryCreationHelper3D
  {
    private const double quadSize = 20.0;

    public static Viewport3DElement GetEnclosingViewportForModelVisual3D(SceneViewModel sceneViewModel, ModelVisual3DElement modelVisual3D)
    {
      Viewport3DElement viewport3Delement = (Viewport3DElement) sceneViewModel.CreateSceneNode(PlatformTypes.Viewport3D);
      viewport3Delement.SetValueAsWpf(Viewport3DElement.ClipToBoundsProperty, (object) true);
      Camera perspectiveCamera = Helper3D.CreateEnclosingPerspectiveCamera(45.0, 4.0 / 3.0, modelVisual3D.DesignTimeBounds, 0.8);
      viewport3Delement.Camera = (CameraElement) sceneViewModel.CreateSceneNode((object) perspectiveCamera);
      ModelVisual3DElement modelVisual3Delement = (ModelVisual3DElement) sceneViewModel.CreateSceneNode(PlatformTypes.ModelVisual3D);
      modelVisual3Delement.Name = "World";
      viewport3Delement.Children.Add((Visual3DElement) modelVisual3Delement);
      AmbientLightElement ambientLightElement = (AmbientLightElement) sceneViewModel.CreateSceneNode(PlatformTypes.AmbientLight);
      ambientLightElement.SetValueAsWpf(LightElement.ColorProperty, (object) Color.FromRgb((byte) 127, (byte) 127, (byte) 127));
      ambientLightElement.Name = "AmbientLight";
      modelVisual3Delement.Children.Add((Visual3DElement) GeometryCreationHelper3D.CreateModelVisual3DContainer((Model3DElement) ambientLightElement, sceneViewModel));
      DirectionalLightElement directionalLightElement = (DirectionalLightElement) sceneViewModel.CreateSceneNode(PlatformTypes.DirectionalLight);
      directionalLightElement.SetValueAsWpf(LightElement.ColorProperty, (object) Color.FromRgb((byte) 63, (byte) 63, (byte) 63));
      directionalLightElement.SetValueAsWpf(DirectionalLightElement.DirectionProperty, (object) new Vector3D(0.0, 0.0, -1.0));
      directionalLightElement.SetValueAsWpf(Model3DElement.TransformProperty, (object) new TranslateTransform3D(0.0, 0.0, 3.0));
      directionalLightElement.Name = "DirectionalLight";
      modelVisual3Delement.Children.Add((Visual3DElement) GeometryCreationHelper3D.CreateModelVisual3DContainer((Model3DElement) directionalLightElement, sceneViewModel));
      modelVisual3Delement.Children.Add((Visual3DElement) modelVisual3D);
      return viewport3Delement;
    }

    private static ModelVisual3DElement CreateModelVisual3DContainer(Model3DElement model3DElement, SceneViewModel sceneViewModel)
    {
      ModelVisual3DElement modelVisual3Delement = (ModelVisual3DElement) sceneViewModel.CreateSceneNode(PlatformTypes.ModelVisual3D);
      modelVisual3Delement.Content = (SceneNode) model3DElement;
      modelVisual3Delement.Name = (model3DElement.Name ?? "") + "Container";
      return modelVisual3Delement;
    }

    public static GeometryModel3DElement ConvertImageTo3D(SceneViewModel viewModel, BaseFrameworkElement imageElement, string newName)
    {
      ImageBrushNode imageBrush;
      return GeometryCreationHelper3D.ConvertImageTo3D(viewModel, imageElement, newName, out imageBrush);
    }

    public static GeometryModel3DElement ConvertImageTo3D(SceneViewModel viewModel, BaseFrameworkElement imageElement, string newName, out ImageBrushNode imageBrush)
    {
      imageBrush = (ImageBrushNode) viewModel.CreateSceneNode(typeof (ImageBrush));
      string uri = ((ImageElement) imageElement).Uri;
      SceneNode valueAsSceneNode = imageElement.GetLocalValueAsSceneNode(ImageElement.SourceProperty);
      if (valueAsSceneNode != null)
        imageBrush.ImageSource = viewModel.GetSceneNode(valueAsSceneNode.DocumentNode.Clone(viewModel.Document.DocumentContext));
      double num = 1.0;
      double imageWidth = 1.0;
      double imageHeight = 1.0;
      double width = 1.0;
      double height = 1.0;
      Stretch stretch = Stretch.Fill;
      ImageSource imageSource;
      if (imageElement.Visual != null)
      {
        Rect computedTightBounds = imageElement.GetComputedTightBounds();
        num = computedTightBounds.Width / computedTightBounds.Height;
        imageWidth = computedTightBounds.Width;
        imageHeight = computedTightBounds.Height;
        imageSource = imageElement.GetComputedValue(ImageElement.SourceProperty) as ImageSource;
        stretch = (Stretch) imageElement.GetComputedValue(ImageElement.StretchProperty);
      }
      else
      {
        imageSource = imageElement.GetLocalOrDefaultValue(ImageElement.SourceProperty) as ImageSource;
        object localOrDefaultValue = imageElement.GetLocalOrDefaultValue(ImageElement.StretchProperty);
        if (localOrDefaultValue is Stretch)
          stretch = (Stretch) localOrDefaultValue;
      }
      if (imageSource != null)
      {
        width = imageSource.Width;
        height = imageSource.Height;
      }
      imageBrush.SetValue(TileBrushNode.StretchProperty, (object) stretch);
      if (stretch == Stretch.None)
      {
        double x = 0.0;
        double y = 0.0;
        if (imageWidth > width)
          x = (imageWidth - width) / 2.0;
        if (imageHeight > height)
          y = (imageHeight - height) / 2.0;
        imageBrush.SetValue(TileBrushNode.ViewportUnitsProperty, (object) BrushMappingMode.Absolute);
        imageBrush.SetValue(TileBrushNode.ViewportProperty, (object) new Rect(x, y, width, height));
      }
      GeometryModel3D geometryModel3D = new GeometryModel3D();
      geometryModel3D.Geometry = (Geometry3D) GeometryCreationHelper3D.CreateSubdividedQuad(9, 20.0 * num, 20.0, imageWidth, imageHeight);
      GeometryModel3DElement geometryModel3Delement = (GeometryModel3DElement) viewModel.CreateSceneNode((object) geometryModel3D);
      geometryModel3Delement.Name = SceneNodeIDHelper.ToCSharpID(newName);
      geometryModel3Delement.DesignTimeBounds = geometryModel3D.Bounds;
      DiffuseMaterialNode diffuseMaterialNode = (DiffuseMaterialNode) viewModel.CreateSceneNode(typeof (DiffuseMaterial));
      diffuseMaterialNode.Brush = (SceneNode) imageBrush;
      geometryModel3Delement.Material = (MaterialNode) diffuseMaterialNode;
      BitmapImageNode bitmapImageNode = imageBrush.ImageSource as BitmapImageNode;
      if (bitmapImageNode != null && uri != null)
        bitmapImageNode.SetValue(BitmapImageNode.UriSourceProperty, (object) new Uri(uri, UriKind.RelativeOrAbsolute));
      return geometryModel3Delement;
    }

    private static MeshGeometry3D CreateSubdividedQuad(int subdivisions, double width, double height, double imageWidth, double imageHeight)
    {
      MeshGeometry3D meshGeometry3D = new MeshGeometry3D();
      double num1 = width / (double) subdivisions;
      double num2 = height / (double) subdivisions;
      double num3 = (double) subdivisions;
      double num4 = width / 2.0;
      double num5 = height / 2.0;
      int num6 = subdivisions + 1;
      for (int index1 = 0; index1 < num6; ++index1)
      {
        for (int index2 = 0; index2 < num6; ++index2)
        {
          meshGeometry3D.Positions.Add(new Point3D(-num4 + (double) index2 * num1, -num5 + (double) index1 * num2, 0.0));
          meshGeometry3D.Normals.Add(new Vector3D(0.0, 0.0, 1.0));
          int num7 = subdivisions - index1;
          meshGeometry3D.TextureCoordinates.Add(new Point((double) index2 / num3 * imageWidth, (double) num7 / num3 * imageHeight));
          if (index2 < subdivisions && index1 < subdivisions)
          {
            int num8 = index1 * num6 + index2;
            meshGeometry3D.TriangleIndices.Add(num8);
            meshGeometry3D.TriangleIndices.Add(num8 + 1);
            meshGeometry3D.TriangleIndices.Add(num8 + num6);
            meshGeometry3D.TriangleIndices.Add(num8 + 1);
            meshGeometry3D.TriangleIndices.Add(num8 + num6 + 1);
            meshGeometry3D.TriangleIndices.Add(num8 + num6);
          }
        }
      }
      return meshGeometry3D;
    }
  }
}
