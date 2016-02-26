// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.BooleanDivideCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class BooleanDivideCommand : BooleanCommand
  {
      private List<System.Windows.Media.Geometry> fragments;
      private System.Windows.Media.Geometry mostRecentGeometry;
      private System.Windows.Media.Geometry unionOfPreviousGeometries;

    public BooleanDivideCommand(SceneViewModel viewModel)
      : base(viewModel, StringTable.UndoUnitBooleanDivide)
    {
    }

    protected override void Initialize(System.Windows.Media.Geometry primaryGeometry)
    {
        this.fragments = new List<System.Windows.Media.Geometry>(1);
      this.fragments.Add(primaryGeometry);
      this.mostRecentGeometry = primaryGeometry;
      this.unionOfPreviousGeometries = (System.Windows.Media.Geometry)null;
    }

    protected override void Combine(System.Windows.Media.Geometry secondaryGeometry)
    {
        this.unionOfPreviousGeometries = this.unionOfPreviousGeometries != null ? (System.Windows.Media.Geometry)System.Windows.Media.Geometry.Combine(this.unionOfPreviousGeometries, this.mostRecentGeometry, GeometryCombineMode.Union, (Transform)null) : this.mostRecentGeometry;
      this.mostRecentGeometry = secondaryGeometry;
      List<System.Windows.Media.Geometry> list = new List<System.Windows.Media.Geometry>(2 * this.fragments.Count + 1);
      foreach (System.Windows.Media.Geometry geometry1 in this.fragments)
      {
          System.Windows.Media.Geometry geometry2 = (System.Windows.Media.Geometry)System.Windows.Media.Geometry.Combine(geometry1, secondaryGeometry, GeometryCombineMode.Exclude, (Transform)null);
          System.Windows.Media.Geometry geometry3 = (System.Windows.Media.Geometry)System.Windows.Media.Geometry.Combine(geometry1, secondaryGeometry, GeometryCombineMode.Intersect, (Transform)null);
        if (!geometry2.Bounds.IsEmpty)
          list.Add(geometry2);
        if (!geometry3.Bounds.IsEmpty)
          list.Add(geometry3);
      }
      System.Windows.Media.Geometry geometry = (System.Windows.Media.Geometry)System.Windows.Media.Geometry.Combine(secondaryGeometry, this.unionOfPreviousGeometries, GeometryCombineMode.Exclude, (Transform)null);
      if (!geometry.Bounds.IsEmpty)
        list.Add(geometry);
      this.fragments = list;
    }

    protected override PathGeometry GetResult()
    {
      PathGeometry pathGeometry = new PathGeometry();
      foreach (System.Windows.Media.Geometry geometry in this.fragments)
        pathGeometry.AddGeometry(geometry);
      this.fragments = (List<System.Windows.Media.Geometry>)null;
      this.mostRecentGeometry = (System.Windows.Media.Geometry)null;
      this.unionOfPreviousGeometries = (System.Windows.Media.Geometry)null;
      return pathGeometry;
    }
  }
}
