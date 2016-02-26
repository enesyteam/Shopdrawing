// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.Base2DElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.Tools.Transforms;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class Base2DElement : SceneElement
  {
    public static readonly IPropertyId RenderTransformProperty = (IPropertyId) PlatformTypes.UIElement.GetMember(MemberType.LocalProperty, "RenderTransform", MemberAccessTypes.Public);
    public static readonly IPropertyId RenderTransformOriginProperty = (IPropertyId) PlatformTypes.UIElement.GetMember(MemberType.LocalProperty, "RenderTransformOrigin", MemberAccessTypes.Public);
    public static readonly IPropertyId OpacityProperty = (IPropertyId) PlatformTypes.UIElement.GetMember(MemberType.LocalProperty, "Opacity", MemberAccessTypes.Public);
    public static readonly IPropertyId OpacityMaskProperty = (IPropertyId) PlatformTypes.UIElement.GetMember(MemberType.LocalProperty, "OpacityMask", MemberAccessTypes.Public);
    public static readonly IPropertyId VisibilityProperty = (IPropertyId) PlatformTypes.UIElement.GetMember(MemberType.LocalProperty, "Visibility", MemberAccessTypes.Public);
    public static readonly IPropertyId EffectProperty = (IPropertyId) PlatformTypes.UIElement.GetMember(MemberType.LocalProperty, "Effect", MemberAccessTypes.Public);
    public static readonly IPropertyId ClipProperty = (IPropertyId) PlatformTypes.UIElement.GetMember(MemberType.LocalProperty, "Clip", MemberAccessTypes.Public);
    public static readonly IPropertyId SnapsToDevicePixelsProperty = (IPropertyId) PlatformTypes.UIElement.GetMember(MemberType.LocalProperty, "SnapsToDevicePixels", MemberAccessTypes.Public);
    public static readonly IPropertyId BitmapEffectProperty = (IPropertyId) PlatformTypes.UIElement.GetMember(MemberType.LocalProperty, "BitmapEffect", MemberAccessTypes.Public);
    public static readonly IPropertyId BitmapEffectInputProperty = (IPropertyId) PlatformTypes.UIElement.GetMember(MemberType.LocalProperty, "BitmapEffectInput", MemberAccessTypes.Public);
    public static readonly IPropertyId AllowDropProperty = (IPropertyId) PlatformTypes.UIElement.GetMember(MemberType.LocalProperty, "AllowDrop", MemberAccessTypes.Public);
    public static readonly IPropertyId UseLayoutRoundingProperty = (IPropertyId) PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "UseLayoutRounding", MemberAccessTypes.Public);
    public static readonly IPropertyId ProjectionProperty = (IPropertyId) PlatformTypes.UIElement.GetMember(MemberType.LocalProperty, "Projection", MemberAccessTypes.Public);
    public static readonly Base2DElement.ConcreteBase2DElementFactory Factory = new Base2DElement.ConcreteBase2DElementFactory();

    public Point RenderTransformOrigin
    {
      get
      {
        if (this.IsSet(Base2DElement.RenderTransformOriginProperty) == PropertyState.Set || this.ViewModel.AnimationEditor.ActiveStoryboardTimeline != null && this.ViewModel.AnimationEditor.IsCurrentlyAnimated((SceneNode) this, new PropertyReference(this.ProjectContext.ResolveProperty(Base2DElement.RenderTransformOriginProperty) as ReferenceStep), 1))
          return (Point) this.GetComputedValueAsWpf(Base2DElement.RenderTransformOriginProperty);
        if (this.IsSet(Base2DElement.RenderTransformProperty) == PropertyState.Set)
          return new Point(0.0, 0.0);
        return new Point(0.5, 0.5);
      }
      set
      {
        this.SetValueAsWpf(Base2DElement.RenderTransformOriginProperty, (object) value);
      }
    }

    public Point RenderTransformOriginInElementCoordinates
    {
      get
      {
        return this.GetRenderTransformOriginInElementCoordinates(false);
      }
      set
      {
        Rect computedBounds = this.GetComputedBounds(this);
        this.RenderTransformOrigin = new Point((value.X - computedBounds.Left) / (computedBounds.Width == 0.0 ? 1.0 : computedBounds.Width), (value.Y - computedBounds.Top) / (computedBounds.Height == 0.0 ? 1.0 : computedBounds.Height));
      }
    }

    public bool IsSnapsToDevicePixelsSupported
    {
      get
      {
        return this.ProjectContext.ResolveProperty(Base2DElement.SnapsToDevicePixelsProperty) != null;
      }
    }

    public bool IsUseLayoutRoundingSupported
    {
      get
      {
        return this.ProjectContext.ResolveProperty(Base2DElement.UseLayoutRoundingProperty) != null;
      }
    }

    public Point GetRenderTransformOriginInElementCoordinates(bool forceAvalonValue)
    {
      Point point = !forceAvalonValue ? this.RenderTransformOrigin : (Point) this.GetComputedValueAsWpf(Base2DElement.RenderTransformOriginProperty);
      Rect computedTightBounds = this.GetComputedTightBounds();
      return new Point(computedTightBounds.Left + point.X * computedTightBounds.Width, computedTightBounds.Top + point.Y * computedTightBounds.Height);
    }

    public Matrix GetEffectiveRenderTransform(bool forceAvalonValue)
    {
      Point elementCoordinates = this.GetRenderTransformOriginInElementCoordinates(forceAvalonValue);
      Matrix identity = Matrix.Identity;
      Matrix renderTransform = this.GetRenderTransform();
      identity.Translate(-elementCoordinates.X, -elementCoordinates.Y);
      Matrix matrix = identity * renderTransform;
      matrix.Translate(elementCoordinates.X, elementCoordinates.Y);
      return matrix;
    }

    public Matrix GetRenderTransform()
    {
      Matrix identity = Matrix.Identity;
      Transform transform = (Transform) this.GetComputedValueAsWpf(Base2DElement.RenderTransformProperty);
      if (transform != null)
        identity = transform.Value;
      return identity;
    }

    public Point GetComputedCenter()
    {
      Rect computedTightBounds = this.GetComputedTightBounds();
      return computedTightBounds.IsEmpty ? new Point(0.0, 0.0) : new Point((computedTightBounds.Left + computedTightBounds.Right) / 2.0, (computedTightBounds.Top + computedTightBounds.Bottom) / 2.0);
    }

    public virtual Rect GetComputedTightBounds()
    {
      if (this.Visual != null)
        return this.ViewModel.DefaultView.GetActualBounds(this.Visual);
      return Rect.Empty;
    }

    public virtual Rect GetComputedTightBounds(Base2DElement context)
    {
      return this.GetComputedBounds(context);
    }

    public Rect GetComputedBounds(Base2DElement context)
    {
      Rect actualBounds = this.ViewModel.DefaultView.GetActualBounds(this.Visual);
      return this.ViewModel.DefaultView.TransformBounds(this.Visual, context.Visual, actualBounds);
    }

    public void EnsureRenderTransform()
    {
      this.EnsureTransform(new PropertyReference(this.ProjectContext.ResolveProperty(Base2DElement.RenderTransformProperty) as ReferenceStep));
    }

    protected override void ModifyValue(PropertyReference propertyReference, object valueToSet, SceneNode.Modification modification, int index)
    {
      if (propertyReference.LastStep.Equals((object) GradientBrushNode.MappingModeProperty))
      {
        object obj = this.ConvertToWpfValue(valueToSet);
        if (obj is BrushMappingMode)
        {
          BrushMappingMode mappingMode = (BrushMappingMode) obj;
          this.UpdateBrushForMappingModeChange(propertyReference.Subreference(0, propertyReference.Count - 2), mappingMode);
        }
      }
      base.ModifyValue(propertyReference, valueToSet, modification, index);
    }

    private void UpdateBrushForMappingModeChange(PropertyReference brushPropertyReference, BrushMappingMode mappingMode)
    {
      GradientBrush gradientBrush = this.GetComputedValueAsWpf(brushPropertyReference) as GradientBrush;
      if (gradientBrush == null || gradientBrush.MappingMode == mappingMode)
        return;
      Rect computedTightBounds = this.GetComputedTightBounds();
      if (computedTightBounds.IsEmpty)
        return;
      Matrix identity = Matrix.Identity;
      Matrix matrix = Matrix.Identity;
      if (BrushAdorner.IsAdorningFillProperty((SceneElement) this))
        matrix = BrushAdorner.GetStrokeTransform((SceneElement) this);
      if (mappingMode == BrushMappingMode.Absolute)
      {
        identity *= matrix;
        identity.Scale(computedTightBounds.Width, computedTightBounds.Height);
        identity.Translate(computedTightBounds.X, computedTightBounds.Y);
      }
      else if (mappingMode == BrushMappingMode.RelativeToBoundingBox)
      {
        identity.Translate(-computedTightBounds.X, -computedTightBounds.Y);
        identity.Scale(computedTightBounds.Width == 0.0 ? 1.0 : 1.0 / computedTightBounds.Width, computedTightBounds.Height == 0.0 ? 1.0 : 1.0 / computedTightBounds.Height);
        matrix.Invert();
        identity *= matrix;
      }
      if (identity.IsIdentity)
        return;
      LinearGradientBrush linearGradientBrush;
      if ((linearGradientBrush = gradientBrush as LinearGradientBrush) != null)
      {
        Point point1 = RoundingHelper.RoundPosition(linearGradientBrush.StartPoint * identity);
        Point point2 = RoundingHelper.RoundPosition(linearGradientBrush.EndPoint * identity);
        this.SetValueAsWpf(brushPropertyReference.Append(LinearGradientBrushNode.StartPointProperty), (object) point1);
        this.SetValueAsWpf(brushPropertyReference.Append(LinearGradientBrushNode.EndPointProperty), (object) point2);
      }
      else
      {
        RadialGradientBrush radialGradientBrush;
        if ((radialGradientBrush = gradientBrush as RadialGradientBrush) == null)
          return;
        Point point1 = RoundingHelper.RoundPosition(radialGradientBrush.Center * identity);
        Point point2 = RoundingHelper.RoundPosition(radialGradientBrush.GradientOrigin * identity);
        double num1 = RoundingHelper.RoundLength(radialGradientBrush.RadiusX * identity.M11);
        double num2 = RoundingHelper.RoundLength(radialGradientBrush.RadiusY * identity.M22);
        this.SetValueAsWpf(brushPropertyReference.Append(RadialGradientBrushNode.CenterProperty), (object) point1);
        this.SetValueAsWpf(brushPropertyReference.Append(RadialGradientBrushNode.GradientOriginProperty), (object) point2);
        this.SetValue(brushPropertyReference.Append(RadialGradientBrushNode.RadiusXProperty), (object) num1);
        this.SetValue(brushPropertyReference.Append(RadialGradientBrushNode.RadiusYProperty), (object) num2);
      }
    }

    public class ConcreteBase2DElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new Base2DElement();
      }
    }
  }
}
