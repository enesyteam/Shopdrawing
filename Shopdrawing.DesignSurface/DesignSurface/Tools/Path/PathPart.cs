// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.PathPart
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  public abstract class PathPart : ISceneElementSubpart, IComparable
  {
    private int figureIndex;
    private int partIndex;
    private PathPart.PartType partType;
    private SceneElement sceneElement;
    private PathEditMode pathEditMode;

    public SceneElement SceneElement
    {
      get
      {
        return this.sceneElement;
      }
    }

    public PathEditMode PathEditMode
    {
      get
      {
        return this.pathEditMode;
      }
    }

    public PathPart.PartType PathPartType
    {
      get
      {
        return this.partType;
      }
    }

    public int FigureIndex
    {
      get
      {
        return this.figureIndex;
      }
    }

    public int PartIndex
    {
      get
      {
        return this.partIndex;
      }
    }

    protected PathPart(SceneElement sceneElement, PathEditMode pathEditMode, int figureIndex, int partIndex, PathPart.PartType partType)
    {
      this.sceneElement = sceneElement;
      this.pathEditMode = pathEditMode;
      this.figureIndex = figureIndex;
      this.partIndex = partIndex;
      this.partType = partType;
    }

    public static bool operator ==(PathPart a, PathPart b)
    {
      if (a == null && b == null)
        return true;
      if (a == null || b == null)
        return false;
      return a.Equals((object) b);
    }

    public static bool operator !=(PathPart a, PathPart b)
    {
      return !(a == b);
    }

    public static bool operator <(PathPart a, PathPart b)
    {
      if (a == null && b == null)
        return false;
      if (a == null)
        return true;
      return a.CompareTo((object) b) < 0;
    }

    public static bool operator >(PathPart a, PathPart b)
    {
      return b < a;
    }

    public override bool Equals(object obj)
    {
      PathPart pathPart = obj as PathPart;
      if (pathPart != (PathPart) null && this.FigureIndex == pathPart.FigureIndex && (this.PartIndex == pathPart.PartIndex && this.sceneElement == pathPart.SceneElement) && this.pathEditMode == pathPart.PathEditMode)
        return this.partType == pathPart.partType;
      return false;
    }

    public override int GetHashCode()
    {
      return this.partIndex.GetHashCode() ^ this.partType.GetHashCode() ^ this.figureIndex.GetHashCode() ^ this.sceneElement.GetHashCode() ^ this.pathEditMode.GetHashCode();
    }

    public abstract PathPart Clone();

    public virtual int CompareTo(object obj)
    {
      if (obj == null)
        return 1;
      PathPart pathPart = obj as PathPart;
      if (pathPart == (PathPart) null)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.PathPartInvalidCompare, new object[2]
        {
          (object) this.ToString(),
          (object) obj.ToString()
        }));
      if (this.SceneElement != pathPart.SceneElement)
        return SceneNode.MarkerCompare((SceneNode) this.SceneElement, (SceneNode) pathPart.SceneElement);
      if (this.pathEditMode != pathPart.PathEditMode)
        return this.pathEditMode.CompareTo((object) pathPart.PathEditMode);
      if (this.PathPartType != pathPart.PathPartType)
        return this.PathPartType >= pathPart.PathPartType ? 1 : -1;
      if (this.FigureIndex != pathPart.FigureIndex)
        return this.FigureIndex >= pathPart.FigureIndex ? 1 : -1;
      if (this.PartIndex == pathPart.PartIndex)
        return 0;
      return this.PartIndex >= pathPart.PartIndex ? 1 : -1;
    }

    public enum PartType
    {
      PathSegment,
      PathPoint,
    }
  }
}
