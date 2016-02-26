// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.ElementToPathEditorTargetMap
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  public sealed class ElementToPathEditorTargetMap
  {
    private Dictionary<Base2DElement, List<PathEditorTarget>> map = new Dictionary<Base2DElement, List<PathEditorTarget>>();

    public IEnumerable<PathEditorTarget> PathEditorTargets
    {
      get
      {
        foreach (List<PathEditorTarget> list in this.map.Values)
        {
          foreach (PathEditorTarget pathEditorTarget in list)
            yield return pathEditorTarget;
        }
      }
    }

    public void Clear()
    {
      foreach (List<PathEditorTarget> list in this.map.Values)
      {
        foreach (PathEditorTarget pathEditorTarget in list)
          pathEditorTarget.Dispose();
      }
      this.map.Clear();
    }

    public PathEditorTarget GetPathEditorTarget(Base2DElement element, PathEditMode pathEditMode)
    {
      if (element == null)
        return (PathEditorTarget) null;
      List<PathEditorTarget> list;
      if (this.map.TryGetValue(element, out list))
      {
        foreach (PathEditorTarget pathEditorTarget in list)
        {
          if (pathEditorTarget.PathEditMode == pathEditMode)
          {
            pathEditorTarget.RefreshSubscription();
            return pathEditorTarget;
          }
        }
      }
      else
      {
        list = new List<PathEditorTarget>();
        this.map.Add(element, list);
      }
      PathEditorTarget pathEditorTarget1 = (PathEditorTarget) null;
      if (pathEditMode == PathEditMode.ScenePath)
      {
        PathElement pathElement = element as PathElement;
        if (pathElement == null)
          return (PathEditorTarget) null;
        ReferenceStep property = PlatformTypeHelper.GetProperty((ITypeResolver) element.ProjectContext, (ITypeId) element.Type, MemberType.Property, "Data");
        if (property == null || !property.ShouldSerialize)
          return (PathEditorTarget) null;
        pathEditorTarget1 = (PathEditorTarget) new ScenePathEditorTarget(pathElement);
      }
      else if (pathEditMode == PathEditMode.MotionPath)
        pathEditorTarget1 = (PathEditorTarget) new MotionPathEditorTarget(element);
      else if (pathEditMode == PathEditMode.ClippingPath)
        pathEditorTarget1 = (PathEditorTarget) new ClippingPathEditorTarget(element);
      list.Add(pathEditorTarget1);
      return pathEditorTarget1;
    }

    public IEnumerable<PathEditorTarget> GetAllPathEditorTargets(SceneElement element)
    {
      Base2DElement base2DElement = element as Base2DElement;
      if (base2DElement != null)
      {
        PathEditorTarget target = this.GetPathEditorTarget(base2DElement, PathEditMode.ScenePath);
        if (target != null)
          yield return target;
        target = this.GetPathEditorTarget(base2DElement, PathEditMode.ClippingPath);
        if (target != null)
          yield return target;
      }
    }
  }
}
