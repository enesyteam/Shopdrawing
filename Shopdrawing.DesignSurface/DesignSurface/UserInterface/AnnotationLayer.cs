// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.AnnotationLayer
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public class AnnotationLayer : Canvas
  {
    public static readonly DependencyProperty ChildRenderTransformProperty = DependencyProperty.Register("ChildRenderTransform", typeof (Transform), typeof (AnnotationLayer), new PropertyMetadata((object) Transform.Identity));

    public Transform ChildRenderTransform
    {
      get
      {
        return (Transform) this.GetValue(AnnotationLayer.ChildRenderTransformProperty);
      }
      set
      {
        this.SetValue(AnnotationLayer.ChildRenderTransformProperty, (object) value);
      }
    }
  }
}
