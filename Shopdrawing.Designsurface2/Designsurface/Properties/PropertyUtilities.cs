// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Properties.PropertyUtilities
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.View;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.Properties
{
  public sealed class PropertyUtilities
  {
    private PropertyUtilities()
    {
    }

    public static bool Compare(object first, object second, SceneView view)
    {
      if (first == null && second == null)
        return true;
      if (first == null || second == null || first.GetType() != second.GetType())
        return false;
      if (first.Equals(second))
        return true;
      if (PropertyUtilities.AreEasingFunctions(first, second, view))
        return PropertyUtilities.CompareEasingFunctions(first, second, view);
      first = view.ConvertToWpfValue(first);
      second = view.ConvertToWpfValue(second);
      return PropertyUtilities.Compare(first, second, 0);
    }

    private static bool Compare(object first, object second, int depth)
    {
      if (depth > 3)
        return true;
      Brush firstBrush = first as Brush;
      Brush secondBrush = second as Brush;
      if (firstBrush != null && secondBrush != null)
        return PropertyUtilities.CompareBrushes(firstBrush, secondBrush);
      GradientStopCollection firstStops;
      GradientStopCollection secondStops;
      if ((firstStops = first as GradientStopCollection) != null && (secondStops = second as GradientStopCollection) != null)
        return PropertyUtilities.CompareGradientStops(firstStops, secondStops);
      TextDecorationCollection firstTextDecorationCollection;
      TextDecorationCollection secondTextDecorationCollection;
      if ((firstTextDecorationCollection = first as TextDecorationCollection) != null && (secondTextDecorationCollection = second as TextDecorationCollection) != null)
        return PropertyUtilities.CompareTextDecorations(firstTextDecorationCollection, secondTextDecorationCollection);
      AxisAngleRotation3D axisAngleRotation3D1;
      AxisAngleRotation3D axisAngleRotation3D2;
      if ((axisAngleRotation3D1 = first as AxisAngleRotation3D) != null && (axisAngleRotation3D2 = second as AxisAngleRotation3D) != null)
      {
        if (axisAngleRotation3D1.Angle == axisAngleRotation3D2.Angle)
          return axisAngleRotation3D1.Axis == axisAngleRotation3D2.Axis;
        return false;
      }
      Material firstMaterial;
      Material secondMaterial;
      if ((firstMaterial = first as Material) != null && (secondMaterial = second as Material) != null)
        return PropertyUtilities.CompareMaterials(firstMaterial, secondMaterial);
      Freezable firstFreezable;
      Freezable secondFreezable;
      if ((firstFreezable = first as Freezable) != null && (secondFreezable = second as Freezable) != null)
        return PropertyUtilities.CompareFreezables(firstFreezable, secondFreezable, depth);
      return object.Equals(first, second);
    }

    private static bool AreEasingFunctions(object first, object second, SceneView sceneView)
    {
      if (first == null || second == null || sceneView == null)
        return false;
      IType type1 = sceneView.ProjectContext.GetType(first.GetType());
      IType type2 = sceneView.ProjectContext.GetType(second.GetType());
      return PlatformTypes.IEasingFunction.IsAssignableFrom((ITypeId) type1) && PlatformTypes.IEasingFunction.IsAssignableFrom((ITypeId) type2);
    }

    private static bool CompareEasingFunctions(object firstEasingFunction, object secondEasingFunction, SceneView sceneView)
    {
      if (sceneView == null)
        return false;
      IEasingFunctionDefinition functionDefinition1 = sceneView.ProjectContext.Platform.ViewObjectFactory.Instantiate(firstEasingFunction) as IEasingFunctionDefinition;
      IEasingFunctionDefinition functionDefinition2 = sceneView.ProjectContext.Platform.ViewObjectFactory.Instantiate(secondEasingFunction) as IEasingFunctionDefinition;
      if (functionDefinition1 == null || functionDefinition2 == null)
        return false;
      if (functionDefinition1.EasingMode == EasingMode.None || functionDefinition2.EasingMode == EasingMode.None)
        return firstEasingFunction.GetType() == secondEasingFunction.GetType();
      if (firstEasingFunction.GetType() == secondEasingFunction.GetType())
        return functionDefinition1.EasingMode == functionDefinition2.EasingMode;
      return false;
    }

    private static bool CompareMaterials(Material firstMaterial, Material secondMaterial)
    {
      if (firstMaterial.GetType() != secondMaterial.GetType())
        return false;
      MaterialGroup materialGroup1 = firstMaterial as MaterialGroup;
      if (materialGroup1 != null)
      {
        MaterialGroup materialGroup2 = (MaterialGroup) secondMaterial;
        if (materialGroup1.Children.Count != materialGroup2.Children.Count)
          return false;
        for (int index = 0; index < materialGroup1.Children.Count; ++index)
        {
          if (!PropertyUtilities.CompareMaterials(materialGroup1.Children[index], materialGroup2.Children[index]))
            return false;
        }
        return true;
      }
      DiffuseMaterial diffuseMaterial1 = firstMaterial as DiffuseMaterial;
      if (diffuseMaterial1 != null)
      {
        DiffuseMaterial diffuseMaterial2 = (DiffuseMaterial) secondMaterial;
        return PropertyUtilities.CompareBrushes(diffuseMaterial1.Brush, diffuseMaterial2.Brush);
      }
      EmissiveMaterial emissiveMaterial1 = firstMaterial as EmissiveMaterial;
      if (emissiveMaterial1 != null)
      {
        EmissiveMaterial emissiveMaterial2 = (EmissiveMaterial) secondMaterial;
        return PropertyUtilities.CompareBrushes(emissiveMaterial1.Brush, emissiveMaterial2.Brush);
      }
      SpecularMaterial specularMaterial1 = firstMaterial as SpecularMaterial;
      if (specularMaterial1 == null)
        return object.Equals((object) firstMaterial, (object) secondMaterial);
      SpecularMaterial specularMaterial2 = (SpecularMaterial) secondMaterial;
      if (specularMaterial1.SpecularPower == specularMaterial2.SpecularPower)
        return PropertyUtilities.CompareBrushes(specularMaterial1.Brush, specularMaterial2.Brush);
      return false;
    }

    private static bool CompareBrushes(Brush firstBrush, Brush secondBrush)
    {
      if (firstBrush == null && secondBrush == null)
        return true;
      if (firstBrush == null || secondBrush == null)
        return false;
      Type type1 = firstBrush.GetType();
      Type type2 = secondBrush.GetType();
      if (type1 != type2)
        return false;
      if (type1 == typeof (SolidColorBrush))
        return ((SolidColorBrush) firstBrush).Color == ((SolidColorBrush) secondBrush).Color;
      GradientBrush gradientBrush1;
      if ((gradientBrush1 = firstBrush as GradientBrush) != null)
      {
        GradientBrush gradientBrush2 = (GradientBrush) secondBrush;
        if (gradientBrush1.MappingMode != gradientBrush2.MappingMode || gradientBrush1.SpreadMethod != gradientBrush2.SpreadMethod)
          return false;
        LinearGradientBrush linearGradientBrush1 = gradientBrush1 as LinearGradientBrush;
        if (linearGradientBrush1 != null)
        {
          LinearGradientBrush linearGradientBrush2 = (LinearGradientBrush) gradientBrush2;
          if (linearGradientBrush1.EndPoint != linearGradientBrush2.EndPoint || linearGradientBrush1.StartPoint != linearGradientBrush2.StartPoint)
            return false;
        }
        else
        {
          RadialGradientBrush radialGradientBrush1;
          if ((radialGradientBrush1 = gradientBrush1 as RadialGradientBrush) != null)
          {
            RadialGradientBrush radialGradientBrush2 = (RadialGradientBrush) gradientBrush2;
            if (radialGradientBrush1.Center != radialGradientBrush2.Center || radialGradientBrush1.RadiusX != radialGradientBrush2.RadiusX || (radialGradientBrush1.RadiusY != radialGradientBrush2.RadiusY || radialGradientBrush1.GradientOrigin != radialGradientBrush2.GradientOrigin))
              return false;
          }
        }
        Transform transform1 = gradientBrush1.Transform;
        Transform transform2 = gradientBrush2.Transform;
        if (transform1 == null || transform2 == null || new CanonicalTransform(transform1) != new CanonicalTransform(transform2))
          return false;
        return PropertyUtilities.CompareGradientStops(gradientBrush1.GradientStops, gradientBrush2.GradientStops);
      }
      TileBrush tileBrush1;
      if ((tileBrush1 = firstBrush as TileBrush) == null)
        return object.Equals((object) firstBrush, (object) secondBrush);
      TileBrush tileBrush2 = (TileBrush) secondBrush;
      if (tileBrush1.AlignmentX != tileBrush2.AlignmentX || tileBrush1.AlignmentY != tileBrush2.AlignmentY || (tileBrush1.RelativeTransform != tileBrush2.RelativeTransform || tileBrush1.Stretch != tileBrush2.Stretch) || (tileBrush1.TileMode != tileBrush2.TileMode || tileBrush1.Viewbox != tileBrush2.Viewbox || (tileBrush1.ViewboxUnits != tileBrush2.ViewboxUnits || tileBrush1.Viewport != tileBrush2.Viewport)) || tileBrush1.ViewportUnits != tileBrush2.ViewportUnits)
        return false;
      Transform transform3 = tileBrush1.Transform;
      Transform transform4 = tileBrush2.Transform;
      return (transform3 != null || transform4 == null) && (transform3 == null || transform4 != null) && !(new CanonicalTransform(transform3) != new CanonicalTransform(transform4));
    }

    public static bool IsLengthNearlyEqual(double length1, double length2)
    {
      if (!double.IsNaN(length1) && !double.IsNaN(length2))
        return Math.Abs(length1 - length2) <= 0.0001;
      return false;
    }

    private static bool CompareGradientStops(GradientStopCollection firstStops, GradientStopCollection secondStops)
    {
      if (firstStops.Count != secondStops.Count)
        return false;
      int count = firstStops.Count;
      for (int index = 0; index < count; ++index)
      {
        if (firstStops[index].Offset != secondStops[index].Offset || firstStops[index].Color != secondStops[index].Color)
          return false;
      }
      return true;
    }

    private static bool CompareTextDecorations(TextDecorationCollection firstTextDecorationCollection, TextDecorationCollection secondTextDecorationCollection)
    {
      if (firstTextDecorationCollection.Count != secondTextDecorationCollection.Count)
        return false;
      foreach (TextDecoration textDecoration in firstTextDecorationCollection)
      {
        if (!secondTextDecorationCollection.Contains(textDecoration))
          return false;
      }
      foreach (TextDecoration textDecoration in secondTextDecorationCollection)
      {
        if (!firstTextDecorationCollection.Contains(textDecoration))
          return false;
      }
      return true;
    }

    private static bool CompareFreezables(Freezable firstFreezable, Freezable secondFreezable, int depth)
    {
      if (firstFreezable == null && secondFreezable == null)
        return true;
      if (firstFreezable == null || secondFreezable == null || firstFreezable.GetType() != secondFreezable.GetType())
        return false;
      IEnumerator enumerator1 = (IEnumerator) firstFreezable.GetLocalValueEnumerator();
      IEnumerator enumerator2 = (IEnumerator) secondFreezable.GetLocalValueEnumerator();
      while (enumerator1.MoveNext())
      {
        if (!enumerator2.MoveNext() || !PropertyUtilities.Compare(enumerator1.Current, enumerator2.Current, depth + 1))
          return false;
      }
      return !enumerator2.MoveNext();
    }
  }
}
