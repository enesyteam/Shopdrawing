// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.DirectionalLightProxy3D
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal class DirectionalLightProxy3D : Adorner3D
  {
    private static Model3DGroup adornerGeometry = FileTable.GetModel3DGroup("Resources\\Adorners3D\\DirectionalLightProxy.xaml");

    public DirectionalLightProxy3D(AdornerSet3D adornerSet)
      : base(adornerSet)
    {
      this.AdornerModel = DirectionalLightProxy3D.adornerGeometry.Clone();
      this.SetName((DependencyObject) this.AdornerModel, "DirectionalLightProxy3D");
      this.IsProxyGeometry = true;
    }
  }
}
