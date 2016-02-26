// Decompiled with JetBrains decompiler
// Type: MS.Internal.DesignerUtilities
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using MS.Internal.Transforms;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace MS.Internal
{
  internal static class DesignerUtilities
  {
    private static int _designerRoundingPrecision = 0;
    private static DateTime _toolboxTextDropTimeStamp = DateTime.Now;
    private const int DOUBLEPRECISION = 3;
    internal const int ROUNDINGPRECISION = 0;
    private static int s_dpi;

    internal static DateTime ToolboxTextDropTimeStamp
    {
      get
      {
        return DesignerUtilities._toolboxTextDropTimeStamp;
      }
      set
      {
        DesignerUtilities._toolboxTextDropTimeStamp = value;
      }
    }

    internal static int DesignerRoundingPrecision
    {
      get
      {
        return DesignerUtilities._designerRoundingPrecision;
      }
      set
      {
        DesignerUtilities._designerRoundingPrecision = value;
      }
    }

    public static int CapsDpi
    {
      get
      {
        if (DesignerUtilities.s_dpi == 0)
          DesignerUtilities.s_dpi = SharedUnsafeNativeMethods.GetDeviceCaps(SharedUnsafeNativeMethods.GetDC(IntPtr.Zero), 88);
        return DesignerUtilities.s_dpi;
      }
    }

    internal static void SetUseLayoutRounding(ModelItem selectedItem)
    {
      if (DesignerUtilities.UseRounding(selectedItem, (Dictionary<ModelItem, bool>) null))
        DesignerUtilities.DesignerRoundingPrecision = 0;
      else
        DesignerUtilities.DesignerRoundingPrecision = 3;
    }

    private static bool UseRounding(ModelItem selectedItem, Dictionary<ModelItem, bool> cache)
    {
      bool flag1;
      if (cache != null && cache.TryGetValue(selectedItem, out flag1))
        return flag1;
      flag1 = true;
      for (ModelProperty modelProperty = selectedItem.Properties.Find("UseLayoutRounding"); modelProperty != (ModelProperty) null; modelProperty = selectedItem != null ? selectedItem.Properties.Find("UseLayoutRounding") : (ModelProperty) null)
      {
        if (modelProperty.IsSet)
        {
          if (!(bool) modelProperty.ComputedValue)
          {
            flag1 = false;
            break;
          }
          break;
        }
        if (cache != null)
          cache.Add(selectedItem, flag1);
        selectedItem = selectedItem.Parent;
        bool flag2;
        if (selectedItem != null && cache != null && cache.TryGetValue(selectedItem, out flag2))
          return flag2;
      }
      return flag1;
    }

    internal static void SetUseLayoutRounding(IEnumerable<ModelItem> selectedItems)
    {
      bool flag = true;
      Dictionary<ModelItem, bool> cache = new Dictionary<ModelItem, bool>();
      foreach (ModelItem selectedItem in selectedItems)
      {
        flag = DesignerUtilities.UseRounding(selectedItem, cache);
        if (!flag)
          break;
      }
      if (flag)
        DesignerUtilities.DesignerRoundingPrecision = 0;
      else
        DesignerUtilities.DesignerRoundingPrecision = 3;
    }

    internal static double GetInvertZoom(EditingContext context)
    {
      return DesignerUtilities.GetInvertZoom(DesignerView.FromContext(context));
    }

    internal static double GetInvertZoom(DesignerView dview)
    {
      Vector scaleFromTransform = TransformUtil.GetScaleFromTransform(dview.GetZoomTransform());
      if (scaleFromTransform.X == 0.0)
        return 0.0;
      return 1.0 / scaleFromTransform.X;
    }

    internal static Vector GetZoomFactor(EditingContext context)
    {
      DesignerView designerView = DesignerView.FromContext(context);
      if (designerView == null)
        return new Vector(1.0, 1.0);
      return TransformUtil.GetScaleFromTransform(designerView.GetZoomTransform());
    }

    internal static Vector GetInvertZoomFactor(DesignerView dview)
    {
      return VectorUtilities.InvertScale(TransformUtil.GetScaleFromTransform(dview.GetZoomTransform()));
    }

    internal static Vector GetZoomRounding(EditingContext context)
    {
      return DesignerUtilities.GetZoomRounding(DesignerView.FromContext(context));
    }

    internal static double Round(double dimension)
    {
      return Math.Round(dimension, DesignerUtilities.DesignerRoundingPrecision);
    }

    internal static Vector GetZoomRounding(DesignerView dview)
    {
      Vector invertZoomFactor = DesignerUtilities.GetInvertZoomFactor(dview);
      invertZoomFactor.X = Math.Round(invertZoomFactor.X, 3);
      invertZoomFactor.Y = Math.Round(invertZoomFactor.Y, 3);
      return invertZoomFactor;
    }

    public static Popup FindPopupRoot(DependencyObject element)
    {
      DependencyObject reference = element;
      DependencyObject dependencyObject = (DependencyObject) null;
      FrameworkContentElement frameworkContentElement;
      for (; reference != null; reference = frameworkContentElement == null ? (reference is Visual || reference is Visual3D ? VisualTreeHelper.GetParent(reference) : (DependencyObject) null) : frameworkContentElement.Parent)
      {
        dependencyObject = reference;
        frameworkContentElement = reference as FrameworkContentElement;
      }
      DependencyObject current = dependencyObject;
      if (current == null)
        return (Popup) null;
      return LogicalTreeHelper.GetParent(current) as Popup;
    }
  }
}
