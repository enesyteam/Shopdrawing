// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.PointLightProxy3D
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal sealed class PointLightProxy3D : Adorner3D
  {
    private static Model3DGroup adornerGeometry = FileTable.GetModel3DGroup("Resources\\Adorners3D\\PointLightProxy.xaml");
    private Model3DGroup positionPropertyGeometry;

    public PointLightProxy3D(AdornerSet3D adornerSet)
      : base(adornerSet)
    {
      this.positionPropertyGeometry = PointLightProxy3D.adornerGeometry.Clone();
      this.AdornerModel = new Model3DGroup();
      this.AdornerModel.Children.Add((Model3D) this.positionPropertyGeometry);
      this.SetName((DependencyObject) this.AdornerModel, "PointLightProxy3D");
      this.IsProxyGeometry = true;
    }

    public override void PositionAndOrientGeometry()
    {
      this.positionPropertyGeometry.Transform = (Transform3D) new TranslateTransform3D((Vector3D) (this.Element as PointLightElement).Position);
    }
  }
}
