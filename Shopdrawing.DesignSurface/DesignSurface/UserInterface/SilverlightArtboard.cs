// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.SilverlightArtboard
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public sealed class SilverlightArtboard : Artboard
  {
    private IPlatformSpecificView platformView;
    private ImageHost silverlightImageHost;
    private ViewExceptionCallback viewExceptionCallback;

    public override IViewObject EditableContentObject
    {
      get
      {
        object rootInstance = this.silverlightImageHost.RootInstance;
        if (rootInstance != null)
          return this.ViewObjectFactory.Instantiate(rootInstance);
        return (IViewObject) null;
      }
    }

    public override Rect DocumentBounds
    {
      get
      {
        return this.silverlightImageHost.GetDocumentBounds(this.EditableContentObject);
      }
    }

    public override bool IsLiveControlLayerActive
    {
      get
      {
        return this.silverlightImageHost.IsLive;
      }
    }

    public ImageHost SilverlightImageHost
    {
      get
      {
        return this.silverlightImageHost;
      }
    }

    public override Vector ViewRootToArtboardScale
    {
      get
      {
        Vector scaleToDevice = DeviceUtilities.ScaleToDevice;
        return new Vector(1.0 / scaleToDevice.X, 1.0 / scaleToDevice.Y);
      }
    }

    public GeneralTransform SilverlightRootTransform { get; private set; }

    public SilverlightArtboard(IPlatformSpecificView platformView, IViewObjectFactory viewObjectFactory, ViewExceptionCallback viewExceptionCallback, ImageHost silverlightImageHost)
      : base(viewObjectFactory, viewExceptionCallback)
    {
      this.platformView = platformView;
      this.viewExceptionCallback = viewExceptionCallback;
      this.silverlightImageHost = silverlightImageHost;
      this.SilverlightRootTransform = (GeneralTransform) new MatrixTransform(Matrix.Identity);
      ((Panel) this.ResourcesHost).Children.Insert(1, (UIElement) this.silverlightImageHost);
    }

    public override bool IsInArtboard(IViewVisual visual)
    {
      bool flag = false;
      if (this.silverlightImageHost.RenderRoot != null)
        flag = this.platformView.IsAncestorOf(this.ViewObjectFactory.Instantiate(this.silverlightImageHost.RenderRoot), (IViewObject) visual);
      return flag;
    }

    public override void AddLiveControl(IViewControl control)
    {
      if (control.PlatformSpecificObject is Control)
        base.AddLiveControl(control);
      else
        this.silverlightImageHost.AddLiveControl(control);
    }

    public override void RemoveLiveControl(IViewControl control)
    {
      if (control.PlatformSpecificObject is Control)
        base.RemoveLiveControl(control);
      else
        this.silverlightImageHost.RemoveLiveControl(control);
    }

    public override void FocusLiveControlLayer()
    {
      this.silverlightImageHost.Focus();
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      Size size = base.ArrangeOverride(finalSize);
      this.silverlightImageHost.Width = finalSize.Width;
      this.silverlightImageHost.Height = finalSize.Height;
      return size;
    }

    protected override void EnsureValidContentBounds()
    {
      if (this.ValidContentBounds)
        return;
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.GetArtboardContentBounds);
      if (this.EditableContentObject != null)
      {
        try
        {
          this.ContentBounds = this.platformView.GetDescendantBounds(this.EditableContentObject);
          if (!double.IsInfinity(this.ContentBounds.Left) && !double.IsInfinity(this.ContentBounds.Top) && !double.IsInfinity(this.ContentBounds.Width))
          {
            if (!double.IsInfinity(this.ContentBounds.Height))
              goto label_7;
          }
          this.ContentBounds = new Rect(((IViewVisual) this.EditableContentObject).RenderSize);
        }
        catch (Exception ex)
        {
          this.viewExceptionCallback(ex);
        }
      }
label_7:
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.GetArtboardContentBounds);
    }

    public override void UpdateImageHostLocation()
    {
      this.UpdateChildren();
    }

    protected override void UpdateChildren()
    {
      base.UpdateChildren();
      Vector scaleToDevice = DeviceUtilities.ScaleToDevice;
      Matrix matrix1 = this.Transform.Value;
      matrix1.Scale(scaleToDevice.X, scaleToDevice.Y);
      this.SilverlightRootTransform = (GeneralTransform) new MatrixTransform(matrix1);
      this.silverlightImageHost.SetTransformMatrix(matrix1, scaleToDevice);
      this.silverlightImageHost.Redraw(false);
      Matrix matrix2 = this.Transform.Value;
      if (matrix2.HasInverse)
        matrix2.Invert();
      this.silverlightImageHost.RenderTransform = (Transform) new MatrixTransform(matrix2);
    }
  }
}
