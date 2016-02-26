// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.AdornerSet3D
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.Tools.Transforms;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public abstract class AdornerSet3D : IAdornerSet
  {
    public static readonly double PixelsPerInch = 96.0;
    private List<Adorner3D> adornerList = new List<Adorner3D>();
    private bool needsRebuild = true;
    private bool needsRedraw = true;
    private AdornerSetOrder order;
    private ToolBehaviorContext toolContext;
    private AdornerElementSet adornedElementSet;
    private ModelVisual3D adornerSetVisual;
    private AdornerSet3DContainer adornerSet3DContainer;
    private bool doScaleToScreen;
    private bool doRemoveObjectScale;
    private bool doCenterOnSpecifiedCenter;
    private AdornerSet3D.Location adornmentLayer;

    public AdornerSetOrder Order
    {
      get
      {
        return this.order;
      }
    }

    public SceneView View
    {
      get
      {
        return this.toolContext.View;
      }
    }

    public ModelVisual3D AdornerVisual
    {
      get
      {
        return this.adornerSetVisual;
      }
    }

    AdornerElementSet IAdornerSet.ElementSet
    {
      get
      {
        return this.adornedElementSet;
      }
    }

    public Base3DElement Element
    {
      get
      {
        return this.adornedElementSet.PrimaryElement as Base3DElement;
      }
    }

    internal ToolBehaviorContext ToolContext
    {
      get
      {
        return this.toolContext;
      }
    }

    public IAdornerCollection Adorners
    {
      get
      {
        return (IAdornerCollection) new AdornerCollection((IList) this.adornerList);
      }
    }

    public virtual ToolBehavior Behavior
    {
      get
      {
        return (ToolBehavior) null;
      }
    }

    protected virtual bool NeedsRebuild
    {
      get
      {
        return this.needsRebuild;
      }
    }

    public bool DoScaleToScreen
    {
      get
      {
        return this.doScaleToScreen;
      }
      set
      {
        this.doScaleToScreen = value;
      }
    }

    public bool DoRemoveObjectScale
    {
      get
      {
        return this.doRemoveObjectScale;
      }
      set
      {
        this.doRemoveObjectScale = value;
      }
    }

    public bool DoCenterOnSpecifiedCenter
    {
      get
      {
        return this.doCenterOnSpecifiedCenter;
      }
      set
      {
        this.doCenterOnSpecifiedCenter = value;
      }
    }

    public AdornerSet3D.Location Placement
    {
      get
      {
        return this.adornmentLayer;
      }
      protected set
      {
        this.adornmentLayer = value;
      }
    }

    internal AdornerSet3DContainer AdornerSet3DContainer
    {
      get
      {
        return this.adornerSet3DContainer;
      }
      set
      {
        this.adornerSet3DContainer = value;
      }
    }

    internal AdornerSet3D(ToolBehaviorContext toolContext, Base3DElement adornedElement)
      : this(toolContext, adornedElement, AdornerSetOrderTokens.MediumPriority)
    {
    }

    internal AdornerSet3D(ToolBehaviorContext toolContext, Base3DElement adornedElement, AdornerSetOrder order)
    {
      if (toolContext == null)
        throw new ArgumentNullException("toolContext");
      if (adornedElement == null)
        throw new ArgumentNullException("adornedElement");
      this.toolContext = toolContext;
      this.adornedElementSet = toolContext.View.Artboard.AdornerLayer.CreateOrGetAdornerElementSetForElement((SceneElement) adornedElement);
      this.order = order;
      this.adornerSetVisual = new ModelVisual3D();
      this.adornerSetVisual.SetValue(FrameworkElement.NameProperty, (object) "adornerSet3DVisual");
      this.adornerSetVisual.Transform = (Transform3D) new MatrixTransform3D();
    }

    public void Update()
    {
      if (this.NeedsRebuild)
      {
        this.needsRebuild = false;
        this.needsRedraw = true;
        this.RemoveAllAdorners();
        this.CreateAdorners();
      }
      if (!this.needsRedraw)
        return;
      this.needsRedraw = false;
      if (((UIElement) this.Element.Viewport.ViewObject.PlatformSpecificObject).IsArrangeValid)
        this.RecomputeTransform();
      foreach (Adorner3D adorner3D in this.adornerList)
        adorner3D.PositionAndOrientGeometry();
    }

    public void InvalidateRender()
    {
      this.needsRedraw = true;
    }

    public Adorner3D GetAssociatedAdorner(ModelVisual3D modelVisual3D)
    {
      foreach (Adorner3D adorner3D in this.adornerList)
      {
        if (adorner3D.AdornerVisual == modelVisual3D)
          return adorner3D;
      }
      return (Adorner3D) null;
    }

    protected void RecomputeTransform()
    {
      AdornerSet3DContainer adornerSet3Dcontainer = this.adornerSet3DContainer;
      Viewport3D viewport = (Viewport3D) this.Element.Viewport.ViewObject.PlatformSpecificObject;
      Matrix3D viewport3DtoElement = this.Element.GetComputedTransformFromViewport3DToElement();
      Point3D point = new Point3D();
      if (this.DoCenterOnSpecifiedCenter)
        point = (CanonicalTransform3D) this.Element.Transform.Center;
      Matrix3D source = viewport3DtoElement;
      source.TranslatePrepend((Vector3D) point);
      if (this.DoRemoveObjectScale)
      {
        Matrix3D rotation = new Matrix3D();
        Vector3D scale = new Vector3D();
        Matrix3DOperations.DecomposeIntoRotationAndScale(source, out rotation, out scale);
        rotation.OffsetX = source.OffsetX;
        rotation.OffsetY = source.OffsetY;
        rotation.OffsetZ = source.OffsetZ;
        source = rotation;
      }
      if (this.adornmentLayer == AdornerSet3D.Location.OrthographicLayer && adornerSet3Dcontainer.ShadowAdorningViewport3D.Camera is ProjectionCamera)
      {
        OrthographicCamera ortho = (OrthographicCamera) adornerSet3Dcontainer.OrthographicAdorningViewport3D.Camera;
        Point3D point3D = AdornedToolBehavior3D.ProjectionPoint3DTranslatedToMatchingOrthographicPosition(viewport, viewport3DtoElement, ortho, point);
        source.OffsetX = point3D.X;
        source.OffsetY = point3D.Y;
        source.OffsetZ = point3D.Z;
      }
      Point3D targetPoint = new Point3D(source.OffsetX, source.OffsetY, source.OffsetZ);
      if (this.DoScaleToScreen)
      {
        double num1 = this.adornmentLayer != AdornerSet3D.Location.OrthographicLayer || !(adornerSet3Dcontainer.ShadowAdorningViewport3D.Camera is ProjectionCamera) ? Helper3D.UnitsPerPixel(viewport, targetPoint) * AdornerSet3D.PixelsPerInch : 6.0 / (this.AdornerSet3DContainer.AdornedViewport.GetComputedTightBounds().Width / 96.0);
        Matrix rotation;
        Vector scale;
        Matrix3DOperations.DecomposeIntoRotationAndScale(ElementUtilities.GetComputedTransform((Visual) viewport, (Visual) (this.View.ViewModel.DefaultView.ViewRoot.PlatformSpecificObject as FrameworkElement)), out rotation, out scale);
        double x = scale.X;
        double zoom = this.View.Zoom;
        double num2 = num1 / zoom * x;
        double offsetX = -targetPoint.X * num2 + targetPoint.X;
        double offsetY = -targetPoint.Y * num2 + targetPoint.Y;
        double offsetZ = -targetPoint.Z * num2 + targetPoint.Z;
        Matrix3D matrix3D = new Matrix3D(num2, 0.0, 0.0, 0.0, 0.0, num2, 0.0, 0.0, 0.0, 0.0, num2, 0.0, offsetX, offsetY, offsetZ, 1.0);
        ((MatrixTransform3D) this.adornerSetVisual.Transform).Matrix = source * matrix3D;
      }
      else
        ((MatrixTransform3D) this.adornerSetVisual.Transform).Matrix = source;
    }

    public virtual Cursor GetCursor(IAdorner adorner)
    {
      return Cursors.Arrow;
    }

    protected virtual void CreateAdorners()
    {
    }

    protected void AddAdorner(Adorner3D adorner)
    {
      if (adorner == null)
        throw new ArgumentNullException("adorner");
      if (this.adornerList == null)
        throw new InvalidOperationException(ExceptionStringTable.AdornerSetAddAdornerCalledWithNullList);
      if (adorner.AdornerModel == null)
        return;
      this.adornerList.Add(adorner);
      this.adornerSetVisual.Children.Add((Visual3D) adorner.AdornerVisual);
    }

    public void RemoveAdorner(Adorner3D adorner)
    {
      if (this.adornerList == null)
        throw new InvalidOperationException(ExceptionStringTable.AdornerSetAddAdornerCalledWithNullList);
      this.adornerList.Remove(adorner);
      this.adornerSetVisual.Children.Remove((Visual3D) adorner.AdornerVisual);
    }

    public void RemoveAllAdorners()
    {
      for (int index = this.adornerList.Count - 1; index >= 0; --index)
        this.RemoveAdorner(this.adornerList[index]);
    }

    public enum Location
    {
      ShadowLayer,
      OrthographicLayer,
    }
  }
}
