// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.AdornerSet3DContainer
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  internal class AdornerSet3DContainer : ContainerVisual
  {
    private List<AdornerSet3D> adornerSets = new List<AdornerSet3D>();
    private Viewport3DElement adornedViewport;
    private Viewport3DVisual shadowAdorningContainer;
    private Viewport3DVisual orthographicAdorningContainer;

    public int AdornerSetCount
    {
      get
      {
        return this.adornerSets.Count;
      }
    }

    public Viewport3DElement AdornedViewport
    {
      get
      {
        return this.adornedViewport;
      }
    }

    public Viewport3DVisual ShadowAdorningViewport3D
    {
      get
      {
        return this.shadowAdorningContainer;
      }
    }

    public Viewport3DVisual OrthographicAdorningViewport3D
    {
      get
      {
        return this.orthographicAdorningContainer;
      }
    }

    public List<AdornerSet3D> AdornerSets
    {
      get
      {
        return this.adornerSets;
      }
    }

    public AdornerSet3DContainer(Viewport3DElement adornedViewport)
    {
      if (adornedViewport == null)
        throw new ArgumentNullException("adornedViewport");
      this.adornedViewport = adornedViewport;
      Rect computedTightBounds = adornedViewport.GetComputedTightBounds();
      this.shadowAdorningContainer = new Viewport3DVisual();
      this.shadowAdorningContainer.Children.Add((Visual3D) this.CreateContainerWorld());
      this.shadowAdorningContainer.Viewport = computedTightBounds;
      this.orthographicAdorningContainer = new Viewport3DVisual();
      this.orthographicAdorningContainer.Children.Add((Visual3D) this.CreateContainerWorld());
      this.orthographicAdorningContainer.Viewport = computedTightBounds;
      ProjectionCamera projectionCamera1 = this.adornedViewport.Camera.ViewObject.PlatformSpecificObject as ProjectionCamera;
      ProjectionCamera projectionCamera2;
      if (projectionCamera1 != null)
      {
        projectionCamera2 = projectionCamera1.Clone();
        projectionCamera2.Transform = Transform3D.Identity;
      }
      else
        projectionCamera2 = (ProjectionCamera) new PerspectiveCamera(new Point3D(0.0, 0.0, 10.0), new Vector3D(0.0, 0.0, 1.0), new Vector3D(0.0, 1.0, 0.0), 35.0);
      this.shadowAdorningContainer.Camera = (Camera) projectionCamera2;
      OrthographicCamera orthographicCamera = new OrthographicCamera(projectionCamera2.Position, projectionCamera2.LookDirection, projectionCamera2.UpDirection, 3.0);
      orthographicCamera.NearPlaneDistance = 0.01;
      orthographicCamera.FarPlaneDistance = 1000000.0;
      this.orthographicAdorningContainer.Camera = (Camera) orthographicCamera;
      this.Children.Add((Visual) this.shadowAdorningContainer);
      this.Children.Add((Visual) this.orthographicAdorningContainer);
    }

    private ModelVisual3D CreateContainerWorld()
    {
      ModelVisual3D modelVisual3D = new ModelVisual3D();
      Model3DGroup model3Dgroup = new Model3DGroup();
      modelVisual3D.Content = (Model3D) model3Dgroup;
      Model3DCollection model3Dcollection = new Model3DCollection();
      model3Dgroup.Children = model3Dcollection;
      Color.FromScRgb(1f, 1f, 1f, 1f);
      Color color = Color.FromScRgb(1f, 0.5f, 0.5f, 0.5f);
      Vector3D direction = new Vector3D(1.0, -1.0, -1.0);
      direction.Normalize();
      DirectionalLight directionalLight = new DirectionalLight(color, direction);
      model3Dgroup.Children.Add((Model3D) directionalLight);
      AmbientLight ambientLight = new AmbientLight(color);
      model3Dgroup.Children.Add((Model3D) ambientLight);
      return modelVisual3D;
    }

    internal void SetMatrix(Matrix matrix)
    {
      MatrixTransform matrixTransform = new MatrixTransform(matrix);
      matrixTransform.Freeze();
      this.VisualTransform = (Transform) matrixTransform;
    }

    public Adorner3D GetAssociatedAdorner(ModelVisual3D modelVisual3D)
    {
      foreach (AdornerSet3D adornerSet3D in this.adornerSets)
      {
        Adorner3D associatedAdorner = adornerSet3D.GetAssociatedAdorner(modelVisual3D);
        if (associatedAdorner != null)
          return associatedAdorner;
      }
      return (Adorner3D) null;
    }

    public void AddAdornerSet(AdornerSet3D adornerSet)
    {
      adornerSet.AdornerSet3DContainer = this;
      this.adornerSets.Add(adornerSet);
      this.GetCorrespondingVisual3DCollection(adornerSet.Placement).Add((Visual3D) adornerSet.AdornerVisual);
    }

    public bool RemoveAdornerSet(AdornerSet3D adornerSet)
    {
      if (!this.adornerSets.Remove(adornerSet))
        return false;
      adornerSet.AdornerSet3DContainer = (AdornerSet3DContainer) null;
      this.GetCorrespondingVisual3DCollection(adornerSet.Placement).Remove((Visual3D) adornerSet.AdornerVisual);
      return true;
    }

    public void Update(Base3DElement optionalBase3DElement)
    {
      this.MatchCameras();
      foreach (AdornerSet3D adornerSet3D in this.adornerSets)
      {
        if (adornerSet3D.Element.IsAttached && adornerSet3D.Element.ViewObject != null && (adornerSet3D.Element.Viewport != null && adornerSet3D.Element.Viewport.ViewObject != null) && (optionalBase3DElement == null || adornerSet3D.Element == optionalBase3DElement))
          adornerSet3D.Update();
      }
    }

    public void InvalidateRender(Base3DElement optionalBase3DElement)
    {
      foreach (AdornerSet3D adornerSet3D in this.adornerSets)
      {
        if (optionalBase3DElement == null || adornerSet3D.Element == optionalBase3DElement)
          adornerSet3D.InvalidateRender();
      }
    }

    private Visual3DCollection GetCorrespondingVisual3DCollection(AdornerSet3D.Location placement)
    {
      Visual3DCollection visual3Dcollection = (Visual3DCollection) null;
      switch (placement)
      {
        case AdornerSet3D.Location.ShadowLayer:
          visual3Dcollection = this.shadowAdorningContainer.Children;
          break;
        case AdornerSet3D.Location.OrthographicLayer:
          visual3Dcollection = this.orthographicAdorningContainer.Children;
          break;
      }
      return visual3Dcollection;
    }

    private void MatchCameras()
    {
      if (!this.adornedViewport.IsAttached || this.adornedViewport.ViewObject == null)
        return;
      Rect computedTightBounds = this.adornedViewport.GetComputedTightBounds();
      if (this.shadowAdorningContainer.Viewport != computedTightBounds)
      {
        this.shadowAdorningContainer.Viewport = computedTightBounds;
        this.orthographicAdorningContainer.Viewport = computedTightBounds;
      }
      CameraElement camera = this.adornedViewport.Camera;
      if (camera == null)
        return;
      ProjectionCamera projectionCamera1 = camera.ViewObject.PlatformSpecificObject as ProjectionCamera;
      if (projectionCamera1 == null)
        return;
      ProjectionCamera projectionCamera2 = (ProjectionCamera) this.shadowAdorningContainer.Camera;
      OrthographicCamera orthographicCamera1 = (OrthographicCamera) this.orthographicAdorningContainer.Camera;
      if (projectionCamera2.FarPlaneDistance != projectionCamera1.FarPlaneDistance || projectionCamera2.NearPlaneDistance != projectionCamera1.NearPlaneDistance || (projectionCamera2.LookDirection != projectionCamera1.LookDirection || projectionCamera2.Position != projectionCamera1.Position) || (projectionCamera2.UpDirection != projectionCamera1.UpDirection || projectionCamera2.Transform != projectionCamera1.Transform))
      {
        projectionCamera2.FarPlaneDistance = projectionCamera1.FarPlaneDistance;
        projectionCamera2.NearPlaneDistance = projectionCamera1.NearPlaneDistance;
        projectionCamera2.LookDirection = projectionCamera1.LookDirection;
        orthographicCamera1.LookDirection = projectionCamera1.LookDirection;
        projectionCamera2.Position = projectionCamera1.Position;
        orthographicCamera1.Position = projectionCamera1.Position;
        projectionCamera2.UpDirection = projectionCamera1.UpDirection;
        orthographicCamera1.UpDirection = projectionCamera1.UpDirection;
      }
      PerspectiveCamera perspectiveCamera1 = projectionCamera1 as PerspectiveCamera;
      PerspectiveCamera perspectiveCamera2 = projectionCamera2 as PerspectiveCamera;
      if (perspectiveCamera1 != null && perspectiveCamera2 != null && perspectiveCamera1.FieldOfView != perspectiveCamera2.FieldOfView)
        perspectiveCamera2.FieldOfView = perspectiveCamera1.FieldOfView;
      OrthographicCamera orthographicCamera2 = projectionCamera1 as OrthographicCamera;
      OrthographicCamera orthographicCamera3 = projectionCamera2 as OrthographicCamera;
      if (orthographicCamera2 == null || orthographicCamera3 == null || orthographicCamera2.Width == orthographicCamera3.Width)
        return;
      orthographicCamera3.Width = orthographicCamera2.Width;
      orthographicCamera1.Width = orthographicCamera2.Width;
    }
  }
}
