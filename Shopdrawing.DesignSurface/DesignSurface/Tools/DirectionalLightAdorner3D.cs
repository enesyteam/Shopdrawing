// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.DirectionalLightAdorner3D
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal sealed class DirectionalLightAdorner3D : Adorner3D
  {
    private static Model3DGroup adornerGeometry = FileTable.GetModel3DGroup("Resources\\Adorners3D\\DirectionalLightAdorner.xaml");
    private Model3DGroup directionPropertyGeometry;

    public DirectionalLightAdorner3D(AdornerSet3D adornerSet)
      : base(adornerSet)
    {
      this.directionPropertyGeometry = DirectionalLightAdorner3D.adornerGeometry.Clone();
      this.AdornerModel = new Model3DGroup();
      this.AdornerModel.Children.Add((Model3D) this.directionPropertyGeometry);
      this.SetName((DependencyObject) this.AdornerModel, "DirectionalLightAdorner3D");
      this.IsProxyGeometry = true;
    }

    public override void PositionAndOrientGeometry()
    {
      this.directionPropertyGeometry.Transform = (Transform3D) new RotateTransform3D(Vector3DEditor.GetRotation3DFromDirection(((DirectionalLightElement) this.Element).Direction));
    }
  }
}
