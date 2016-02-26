// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Selection.PathPartSelectionSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools.Path;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.Selection
{
  public class PathPartSelectionSet : SubpartSelectionSet<PathPart, OrderedList<PathPart>>
  {
    public ICollection<SceneElement> SelectedPaths
    {
      get
      {
        List<SceneElement> list = new List<SceneElement>();
        foreach (PathPart pathPart in this.Selection)
        {
          if (!list.Contains(pathPart.SceneElement))
            list.Add(pathPart.SceneElement);
        }
        return (ICollection<SceneElement>) list;
      }
    }

    public PathPartSelectionSet(SceneElementSelectionSet elementSelectionSet)
      : base(elementSelectionSet.ViewModel, (ISelectionSetNamingHelper) new PathPartSelectionSet.PathPartNamingHelper(), elementSelectionSet, (SelectionSet<PathPart, OrderedList<PathPart>>.IStorageProvider) new BasicSelectionSetStorageProvider<PathPart>((IComparer<PathPart>) new SubpartSelectionSet<PathPart, OrderedList<PathPart>>.SceneElementSubpartComparer()))
    {
    }

    public ICollection<PathPart> GetSelectionByElement(SceneElement element)
    {
      List<PathPart> list = new List<PathPart>();
      foreach (PathPart pathPart in this.Selection)
      {
        if (pathPart.SceneElement == element)
          list.Add(pathPart);
      }
      return (ICollection<PathPart>) list;
    }

    public ICollection<PathPart> GetSelectionByElement(SceneElement element, PathEditMode pathEditMode)
    {
      List<PathPart> list = new List<PathPart>();
      foreach (PathPart pathPart in this.Selection)
      {
        if (pathPart.SceneElement == element && pathPart.PathEditMode == pathEditMode)
          list.Add(pathPart);
      }
      return (ICollection<PathPart>) list;
    }

    private class PathPartNamingHelper : ISelectionSetNamingHelper
    {
      public string Name
      {
        get
        {
          return StringTable.UndoUnitPathPartName;
        }
      }

      public string GetUndoString(object obj)
      {
        PathPart pathPart = obj as PathPart;
        if (!(pathPart != (PathPart) null))
          return "";
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} {1} {2}", (object) pathPart.SceneElement.Name, (object) pathPart.PathPartType, (object) pathPart.PartIndex);
      }
    }
  }
}
