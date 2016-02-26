// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.BooleanCombineCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal abstract class BooleanCombineCommand : BooleanCommand
  {
    private GeometryCombineMode geometryCombineMode;
    private System.Windows.Media.Geometry primaryGeometry;
    private PathGeometry resultGeometry;

    public BooleanCombineCommand(SceneViewModel viewModel, string undoDescription, GeometryCombineMode geometryCombineMode)
      : base(viewModel, undoDescription)
    {
      this.geometryCombineMode = geometryCombineMode;
    }

    protected override void Initialize(System.Windows.Media.Geometry primaryGeometry)
    {
      this.primaryGeometry = primaryGeometry;
      this.resultGeometry = (PathGeometry) null;
    }

    protected override void Combine(System.Windows.Media.Geometry secondaryGeometry)
    {
        this.resultGeometry = System.Windows.Media.Geometry.Combine(this.primaryGeometry, secondaryGeometry, this.geometryCombineMode, (Transform)null, 0.0001, ToleranceType.Relative);
      this.primaryGeometry = (System.Windows.Media.Geometry)this.resultGeometry;
    }

    protected override PathGeometry GetResult()
    {
      PathGeometry pathGeometry = this.resultGeometry;
      this.primaryGeometry = (System.Windows.Media.Geometry)null;
      this.resultGeometry = (PathGeometry) null;
      return pathGeometry;
    }
  }
}
