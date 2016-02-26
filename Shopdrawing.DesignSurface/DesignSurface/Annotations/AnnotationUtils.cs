// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Annotations.AnnotationUtils
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.Expression.DesignSurface.Annotations
{
  internal static class AnnotationUtils
  {
    public static void SetSerialNumber(AnnotationSceneNode annotation)
    {
      ExceptionChecks.CheckNullArgument<AnnotationSceneNode>(annotation, "annotation");
      string authorInitials = annotation.AuthorInitials;
      IEnumerable<int> source = Enumerable.Select<AnnotationSceneNode, int>(Enumerable.Where<AnnotationSceneNode>(annotation.ViewModel.AnnotationEditor.GetAnnotationsWithoutCache(), (Func<AnnotationSceneNode, bool>) (anno =>
      {
        if (anno != annotation)
          return StringComparer.CurrentCultureIgnoreCase.Equals(anno.AuthorInitials, authorInitials);
        return false;
      })), (Func<AnnotationSceneNode, int>) (anno => anno.SerialNumber));
      int num = 1;
      if (Enumerable.Any<int>(source))
        num = Enumerable.Max(source) + 1;
      annotation.SerialNumber = num;
    }

    public static string SetAnnotationId(AnnotationSceneNode annotation)
    {
      ExceptionChecks.CheckNullArgument<AnnotationSceneNode>(annotation, "annotation");
      Func<string, int> ParseOrZero = (Func<string, int>) (s =>
      {
        int result;
        if (!int.TryParse(s, out result))
          return 0;
        return result;
      });
      IEnumerable<AnnotationSceneNode> annotations = annotation.ViewModel.AnnotationEditor.Annotations;
      int num = 1;
      if (Enumerable.Any<AnnotationSceneNode>(annotations))
        num = Enumerable.Max(Enumerable.Select<AnnotationSceneNode, int>(annotations, (Func<AnnotationSceneNode, int>) (anno => ParseOrZero(anno.Id)))) + 1;
      string str = num.ToString((IFormatProvider) CultureInfo.CurrentCulture);
      annotation.Id = str;
      return str;
    }

    public static IEnumerable<string> GetReferencedAnnotationIds(SceneElement element)
    {
      ExceptionChecks.CheckNullArgument<SceneElement>(element, "element");
      return AnnotationUtils.ParseAnnotationReferences(element.GetLocalOrDefaultValue(AnnotationSceneNode.ReferencesProperty) as string ?? string.Empty);
    }

    public static IEnumerable<string> ParseAnnotationReferences(string referencesString)
    {
      if (!string.IsNullOrEmpty(referencesString))
        return (IEnumerable<string>) Regex.Split(referencesString, "\\s*,\\s*");
      return Enumerable.Empty<string>();
    }

    public static bool RemoveAnnotationReference(SceneElement element, AnnotationSceneNode annotation)
    {
      ExceptionChecks.CheckNullArgument<SceneElement>(element, "element");
      ExceptionChecks.CheckNullArgument<AnnotationSceneNode>(annotation, "annotation");
      string annotationId = annotation.Id;
      if (string.IsNullOrEmpty(annotationId))
        return false;
      List<string> list = Enumerable.ToList<string>(AnnotationUtils.GetReferencedAnnotationIds(element));
      IEnumerable<string> enumerable = Enumerable.Where<string>((IEnumerable<string>) list, (Func<string, bool>) (id => id.Equals(annotationId, StringComparison.OrdinalIgnoreCase)));
      if (!Enumerable.Any<string>(enumerable))
        return false;
      AnnotationUtils.SetAnnotationReferences(element, Enumerable.Except<string>((IEnumerable<string>) list, enumerable));
      return true;
    }

    public static void AddAnnotationReference(SceneElement element, AnnotationSceneNode annotation)
    {
      ExceptionChecks.CheckNullArgument<SceneElement>(element, "element");
      ExceptionChecks.CheckNullArgument<AnnotationSceneNode>(annotation, "annotation");
      string str = annotation.Id;
      if (string.IsNullOrEmpty(str))
        str = AnnotationUtils.SetAnnotationId(annotation);
      IEnumerable<string> referencedAnnotationIds = AnnotationUtils.GetReferencedAnnotationIds(element);
      AnnotationUtils.SetAnnotationReferences(element, EnumerableExtensions.AppendItem<string>(referencedAnnotationIds, str));
    }

    private static void SetAnnotationReferences(SceneElement element, IEnumerable<string> ids)
    {
      ExceptionChecks.CheckNullArgument<SceneElement>(element, "element");
      ExceptionChecks.CheckNullArgument<IEnumerable<string>>(ids, "ids");
      if (!Enumerable.Any<string>(ids))
        element.ClearValue(AnnotationSceneNode.ReferencesProperty);
      else
        element.SetValue(AnnotationSceneNode.ReferencesProperty, (object) string.Join(", ", Enumerable.ToArray<string>(ids)));
    }
  }
}
