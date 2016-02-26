// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.AdornerService
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.View;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public class AdornerService
  {
    private Artboard artboard;
    private PointHitTestParameters hitTestParameters3D;
    private RectangleHitTestResult hitObject;

    private AdornerLayer AdornerLayer
    {
      get
      {
        return this.artboard.AdornerLayer;
      }
    }

    private FrameworkElement ContentArea
    {
      get
      {
        return this.artboard.ContentArea;
      }
    }

    public AdornerService(Artboard artboard)
    {
      this.artboard = artboard;
    }

    public IAdorner GetHitAdorner(MouseDevice mouseDevice)
    {
      return this.GetHitAdorner(mouseDevice, new Type[1]
      {
        typeof (Adorner)
      });
    }

    public IAdorner GetHitAdorner(MouseDevice mouseDevice, Type[] adornerFilterTypes)
    {
      return this.GetHitAdorner(mouseDevice.GetPosition((IInputElement) this.ContentArea), adornerFilterTypes);
    }

    public IAdorner GetHitAdorner(Point hitPoint)
    {
      return this.GetHitAdorner(hitPoint, new Type[1]
      {
        typeof (Adorner)
      });
    }

    public IAdorner GetHitAdorner(Point hitPoint, Type[] adornerFilterTypes)
    {
      if (!this.AdornerLayer.IsVisible)
        return (IAdorner) null;
      IAdorner adorner = (IAdorner) null;
      if (this.AdornerLayer != null)
        adorner = (IAdorner) this.GetHitAdorner((Visual) this.AdornerLayer, this.ContentArea.TransformToVisual((Visual) this.AdornerLayer).Transform(hitPoint), adornerFilterTypes);
      List<AdornerSet3DContainer> adornerSet3Dcontainers = this.AdornerLayer.AdornerSet3DContainers;
      if (adorner == null && adornerSet3Dcontainers != null && adornerSet3Dcontainers.Count > 0)
      {
        foreach (AdornerSet3DContainer layer in adornerSet3Dcontainers)
        {
          Point point = this.ContentArea.TransformToVisual((Visual) layer).Transform(hitPoint);
          adorner = (IAdorner) this.GetHitAdorner3D(layer, point);
          if (adorner != null)
            break;
        }
      }
      return adorner;
    }

    public bool CanClickOnAdorner(IAdorner adorner, SceneView sceneView, IViewObject rootViewObject, ref Point clickablePoint)
    {
      if (this.AdornerLayer == null || !this.AdornerLayer.IsVisible)
        return false;
      bool flag = false;
      IClickable clickable = adorner as IClickable;
      if (clickable != null)
      {
        Matrix transformMatrix = ((AdornerSet) adorner.AdornerSet).GetTransformMatrix(rootViewObject);
        clickablePoint = clickable.GetClickablePoint(transformMatrix);
        Window mainWindow = Application.Current.MainWindow;
        Matrix matrixFromTransform = VectorUtilities.GetMatrixFromTransform(sceneView.ComputeTransformToVisual(rootViewObject, (Visual) mainWindow));
        Point point = clickablePoint * matrixFromTransform;
        flag = this.GetHitAdorner((Visual) mainWindow, point) == adorner;
      }
      return flag;
    }

    private Adorner GetHitAdorner(Visual visual, Point point)
    {
      return this.GetHitAdorner(visual, point, new Type[1]
      {
        typeof (Adorner)
      });
    }

    private Adorner GetHitAdorner(Visual visual, Point point, Type[] adornerFilterTypes)
    {
      PointHitTestResult hitTestResult = (PointHitTestResult) null;
      HitTestFilterCallback filterCallback = (HitTestFilterCallback) (testObject =>
      {
        foreach (Type type in adornerFilterTypes)
        {
          if (type.IsInstanceOfType((object) testObject))
            return HitTestFilterBehavior.Continue;
        }
        return HitTestFilterBehavior.ContinueSkipSelf;
      });
      HitTestResultCallback resultCallback = (HitTestResultCallback) (hitItemsResult =>
      {
        hitTestResult = (PointHitTestResult) hitItemsResult;
        return HitTestResultBehavior.Stop;
      });
      PointHitTestParameters hitTestParameters = new PointHitTestParameters(point);
      VisualTreeHelper.HitTest(visual, filterCallback, resultCallback, (HitTestParameters) hitTestParameters);
      Adorner adorner = (Adorner) null;
      if (hitTestResult != null)
        adorner = hitTestResult.VisualHit as Adorner;
      return adorner;
    }

    internal Adorner3D GetHitAdorner3D(AdornerSet3DContainer layer, Point point)
    {
      this.hitTestParameters3D = new PointHitTestParameters(point);
      this.hitObject = (RectangleHitTestResult) null;
      VisualTreeHelper.HitTest((Visual) layer, new HitTestFilterCallback(this.FilterPotentialHit), new HitTestResultCallback(this.ProcessHitTestResult3D), (HitTestParameters) this.hitTestParameters3D);
      if (this.hitObject != null)
      {
        ModelVisual3D modelVisual3D1 = (ModelVisual3D) null;
        foreach (DependencyObject dependencyObject in this.hitObject.HitPath)
        {
          ModelVisual3D modelVisual3D2 = dependencyObject as ModelVisual3D;
          if (modelVisual3D2 != null)
          {
            modelVisual3D1 = modelVisual3D2;
            break;
          }
        }
        Adorner3D associatedAdorner = layer.GetAssociatedAdorner(modelVisual3D1);
        if (associatedAdorner != null)
          return associatedAdorner;
      }
      return (Adorner3D) null;
    }

    private HitTestFilterBehavior FilterPotentialHit(DependencyObject testObject)
    {
      Viewport3DVisual viewportVisual = testObject as Viewport3DVisual;
      if (viewportVisual == null)
        return HitTestFilterBehavior.ContinueSkipSelf;
      RectangleHitTestResult closestHitTestResult = new Viewport3DHitTestHelper(viewportVisual, (GeneralTransform) Transform.Identity).HitTest((HitTestParameters) this.hitTestParameters3D).ClosestHitTestResult;
      if (closestHitTestResult == null)
        return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
      this.hitObject = closestHitTestResult;
      return HitTestFilterBehavior.Stop;
    }

    private HitTestResultBehavior ProcessHitTestResult3D(HitTestResult hitTestResult)
    {
      return HitTestResultBehavior.Continue;
    }
  }
}
