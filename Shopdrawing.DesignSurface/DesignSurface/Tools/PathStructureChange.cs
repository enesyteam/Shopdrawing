// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.PathStructureChange
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Windows;

namespace Microsoft.Expression.DesignSurface.Tools
{
  public class PathStructureChange
  {
    public int OldSegmentIndex { get; set; }

    public int NewSegmentIndex { get; set; }

    public int OldFigureIndex { get; set; }

    public int NewFigureIndex { get; set; }

    public DependencyProperty OldPointProperty { get; set; }

    public DependencyProperty NewPointProperty { get; set; }

    public PathChangeType PathChangeType { get; set; }

    public static int DeletedPointIndex
    {
      get
      {
        return -1;
      }
    }

    public static int StartPointIndex
    {
      get
      {
        return -2;
      }
    }

    public PathStructureChange(int oldIndex, int newIndex)
      : this(oldIndex, newIndex, -1, -1, (DependencyProperty) null, (DependencyProperty) null)
    {
    }

    public PathStructureChange(int oldIndex, int newIndex, int oldFigure, int newFigure)
      : this(oldIndex, newIndex, oldFigure, newFigure, (DependencyProperty) null, (DependencyProperty) null)
    {
    }

    public PathStructureChange(int oldIndex, int newIndex, PathChangeType pathChangeType)
      : this(oldIndex, newIndex, -1, -1, (DependencyProperty) null, (DependencyProperty) null, pathChangeType)
    {
    }

    public PathStructureChange(int oldIndex, int newIndex, DependencyProperty pointProperty)
      : this(oldIndex, newIndex, -1, -1, pointProperty, pointProperty)
    {
    }

    public PathStructureChange(int oldIndex, int newIndex, int oldFigure, int newFigure, PathChangeType pathChangeType)
      : this(oldIndex, newIndex, oldFigure, newFigure, (DependencyProperty) null, (DependencyProperty) null, pathChangeType)
    {
    }

    public PathStructureChange(int oldIndex, int newIndex, int oldFigure, int newFigure, DependencyProperty pointProperty)
      : this(oldIndex, newIndex, oldFigure, newFigure, pointProperty, pointProperty)
    {
    }

    public PathStructureChange(int oldIndex, int newIndex, DependencyProperty oldPointProperty, DependencyProperty newPointProperty)
      : this(oldIndex, newIndex, -1, -1, oldPointProperty, newPointProperty, PathChangeType.Move)
    {
    }

    public PathStructureChange(int oldIndex, int newIndex, int oldFigure, int newFigure, DependencyProperty oldPointProperty, DependencyProperty newPointProperty)
      : this(oldIndex, newIndex, oldFigure, newFigure, oldPointProperty, newPointProperty, PathChangeType.Move)
    {
    }

    public PathStructureChange(int oldIndex, int newIndex, DependencyProperty oldPointProperty, DependencyProperty newPointProperty, PathChangeType pathChangeType)
      : this(oldIndex, newIndex, -1, -1, oldPointProperty, newPointProperty, pathChangeType)
    {
    }

    public PathStructureChange(int oldIndex, int newIndex, int oldFigure, int newFigure, DependencyProperty oldPointProperty, DependencyProperty newPointProperty, PathChangeType pathChangeType)
    {
      this.OldSegmentIndex = oldIndex;
      this.NewSegmentIndex = newIndex;
      this.OldPointProperty = oldPointProperty;
      this.NewPointProperty = newPointProperty;
      this.PathChangeType = pathChangeType;
      this.OldFigureIndex = oldFigure;
      this.NewFigureIndex = newFigure;
    }
  }
}
