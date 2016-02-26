// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Layout.LayoutRoundingHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Designers;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Layout
{
  internal static class LayoutRoundingHelper
  {
    private static readonly string AutoLayoutRoundingValue = "Auto";

    public static Rect RoundUpLayoutRect(BaseFrameworkElement element, Rect rect)
    {
      IPlatformGeometryHelper geometryHelper = element.Platform.GeometryHelper;
      IViewVisual visual = element.ViewObject as IViewVisual;
      if (geometryHelper.NeedRoundupLayoutRect(visual))
      {
        IProperty propertyKey = element.ProjectContext.ResolveProperty(Base2DElement.UseLayoutRoundingProperty);
        if (propertyKey != null && (bool) visual.GetCurrentValue(propertyKey))
          return geometryHelper.RoundupLayoutRect(rect);
      }
      return rect;
    }

    public static IDisposable TurnOffLayoutRounding(BaseFrameworkElement element)
    {
      IViewVisual viewVisual = element.ViewObject as IViewVisual;
      IProperty propertyKey = element.ProjectContext.ResolveProperty(Base2DElement.UseLayoutRoundingProperty);
      if (propertyKey == null || viewVisual == null || !(bool) viewVisual.GetCurrentValue(propertyKey))
        return (IDisposable) null;
      SceneEditTransaction editTransaction = element.ViewModel.CreateEditTransaction("TurnOffLayoutRounding", true);
      element.SetValue((IPropertyId) propertyKey, (object) false);
      editTransaction.Update();
      viewVisual.UpdateLayout();
      return (IDisposable) editTransaction;
    }

    public static Rect GetRoundedUpChildRect(ILayoutDesigner designer, BaseFrameworkElement element)
    {
      IPlatformGeometryHelper geometryHelper = element.Platform.GeometryHelper;
      IViewVisual visual = element.ViewObject as IViewVisual;
      if (geometryHelper.NeedRoundupLayoutRect(visual))
      {
        using (IDisposable disposable = LayoutRoundingHelper.TurnOffLayoutRounding(element))
        {
          if (disposable != null)
          {
            Rect childRect = designer.GetChildRect(element);
            return geometryHelper.RoundupLayoutRect(childRect);
          }
        }
      }
      return designer.GetChildRect(element);
    }

    public static bool GetUseLayoutRounding(SceneElement element)
    {
      if (element != null && element.Type != null)
      {
        IProperty propertyKey = LayoutRoundingHelper.ResolveUseLayoutRoundingProperty((SceneNode) element);
        if (propertyKey != null)
        {
          if (propertyKey.DeclaringType.IsAssignableFrom((ITypeId) element.Type))
            return (bool) element.GetComputedValue((IPropertyId) propertyKey);
          FrameworkTemplateElement frameworkTemplateElement = element as FrameworkTemplateElement;
          if (frameworkTemplateElement != null)
          {
            IViewObject viewTargetElement = frameworkTemplateElement.ViewTargetElement;
            if (viewTargetElement != null && propertyKey.DeclaringType.IsAssignableFrom((ITypeId) viewTargetElement.GetIType((ITypeResolver) element.ProjectContext)))
              return (bool) viewTargetElement.GetCurrentValue(propertyKey);
          }
        }
      }
      return false;
    }

    public static LayoutRoundingStatus GetLayoutRoundingStatus(SceneElement element)
    {
      if (!LayoutRoundingHelper.ShouldConsiderLayoutRoundingAdjustment(element))
        return LayoutRoundingStatus.Off;
      IProperty property = LayoutRoundingHelper.ResolveUseLayoutRoundingProperty((SceneNode) element);
      if (property == null || !LayoutRoundingHelper.GetUseLayoutRounding(element))
        return LayoutRoundingStatus.Off;
      LayoutRoundingStatus layoutRoundingStatus = LayoutRoundingStatus.On;
      bool flag = element.IsSet((IPropertyId) property) == PropertyState.Set;
      if (!flag && PlatformTypes.Path.IsAssignableFrom((ITypeId) element.Type))
        layoutRoundingStatus |= LayoutRoundingStatus.ShouldTurnOff;
      else if (LayoutRoundingHelper.IsAxisAligned(element))
        layoutRoundingStatus |= LayoutRoundingStatus.ShouldSnapToPixel;
      else if (!flag || LayoutRoundingHelper.GetAutoLayoutRounding(element))
        layoutRoundingStatus |= LayoutRoundingStatus.ShouldTurnOff;
      return layoutRoundingStatus;
    }

    public static void ExplicitlyChangeLayoutRounding(SceneNode targetNode, PropertyReference filteredProperty)
    {
      Base2DElement base2Delement = targetNode as Base2DElement;
      if (base2Delement == null || !LayoutRoundingHelper.IsUseLayoutRoundingProperty(targetNode, (IProperty) filteredProperty.LastStep))
        return;
      LayoutRoundingHelper.SetAutoLayoutRounding((SceneElement) base2Delement, false);
    }

    public static bool UpdateLayoutRounding(IEnumerable<SceneElement> elements)
    {
      bool flag = false;
      if (elements != null && LayoutRoundingHelper.ShouldConsiderLayoutRoundingAdjustment(Enumerable.FirstOrDefault<SceneElement>(elements)))
      {
        foreach (SceneElement element in elements)
          flag |= LayoutRoundingHelper.UpdateLayoutRounding(element);
      }
      return flag;
    }

    public static bool UpdateLayoutRounding(SceneElement element)
    {
      if (!LayoutRoundingHelper.ShouldConsiderLayoutRoundingAdjustment(element))
        return false;
      IPropertyId propertyKey = (IPropertyId) LayoutRoundingHelper.ResolveUseLayoutRoundingProperty((SceneNode) element);
      if (propertyKey == null)
        return false;
      bool flag1 = element.IsSet(propertyKey) == PropertyState.Set;
      if (PlatformTypes.Path.IsAssignableFrom((ITypeId) element.Type))
      {
        if (!flag1 && (bool) element.GetComputedValue(propertyKey))
        {
          element.SetLocalValue(Base2DElement.UseLayoutRoundingProperty, (object) false);
          LayoutRoundingHelper.SetAutoLayoutRounding(element, false);
          return true;
        }
      }
      else if (!flag1 || LayoutRoundingHelper.GetAutoLayoutRounding(element))
      {
        bool flag2 = (bool) element.GetComputedValue(propertyKey);
        if (LayoutRoundingHelper.IsAxisAligned(element))
        {
          if (!flag2)
          {
            element.ClearValue(propertyKey);
            LayoutRoundingHelper.SetAutoLayoutRounding(element, false);
            return true;
          }
        }
        else if (flag2)
        {
          element.SetValue(propertyKey, (object) false);
          LayoutRoundingHelper.SetAutoLayoutRounding(element, true);
          return true;
        }
      }
      return false;
    }

    public static bool IsUseLayoutRoundingProperty(SceneNode node, IProperty property)
    {
      SceneElement sceneElement = node as SceneElement;
      if (sceneElement != null)
      {
        IProperty property1 = LayoutRoundingHelper.ResolveUseLayoutRoundingProperty((SceneNode) sceneElement);
        if (property1 != null && property1.Equals((object) property))
          return true;
      }
      return false;
    }

    public static IProperty ResolveUseLayoutRoundingProperty(SceneNode node)
    {
      if (node != null)
        return node.Platform.Metadata.ResolveProperty(Base2DElement.UseLayoutRoundingProperty);
      return (IProperty) null;
    }

    public static Rect RoundRect(IPlatformGeometryHelper geometryHelper, Rect rect)
    {
      double x = geometryHelper.Round(rect.Left);
      double num1 = geometryHelper.Round(rect.Right);
      double y = geometryHelper.Round(rect.Top);
      double num2 = geometryHelper.Round(rect.Bottom);
      rect = new Rect(x, y, num1 - x, num2 - y);
      return rect;
    }

    private static bool ShouldConsiderLayoutRoundingAdjustment(SceneElement element)
    {
      if (element == null)
        return false;
      if (element.ViewModel.AnimationEditor.IsRecording && !element.ViewModel.IsForcingBaseValue)
        return element.ViewModel.AnimationEditor.CanAnimateLayout;
      return true;
    }

    private static bool IsAxisAligned(SceneElement element)
    {
      if (element.IsSet(Base2DElement.RenderTransformProperty) == PropertyState.Set)
      {
        Matrix matrixFromTransform = VectorUtilities.GetMatrixFromTransform(element.GetComputedValueAsWpf(Base2DElement.RenderTransformProperty) as GeneralTransform);
        if (!matrixFromTransform.IsIdentity)
        {
          Vector vector1 = matrixFromTransform.Transform(new Vector(1.0, 0.0));
          Vector vector2 = matrixFromTransform.Transform(new Vector(0.0, 1.0));
          double num = 1E-06;
          if (Math.Abs(vector1.Y) > num || Math.Abs(vector2.X) > num)
            return false;
        }
      }
      return true;
    }

    private static bool GetAutoLayoutRounding(SceneElement element)
    {
      return LayoutRoundingHelper.AutoLayoutRoundingValue.Equals(element.GetLocalOrDefaultValue(DesignTimeProperties.LayoutRoundingProperty));
    }

    private static void SetAutoLayoutRounding(SceneElement element, bool auto)
    {
      if (auto)
        element.SetValue(DesignTimeProperties.LayoutRoundingProperty, (object) LayoutRoundingHelper.AutoLayoutRoundingValue);
      else
        element.ClearValue(DesignTimeProperties.LayoutRoundingProperty);
    }
  }
}
