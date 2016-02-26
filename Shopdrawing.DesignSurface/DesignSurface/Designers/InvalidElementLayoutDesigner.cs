// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Designers.InvalidElementLayoutDesigner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Designers
{
  public class InvalidElementLayoutDesigner : LayoutDesigner
  {
    public override Rect GetChildRect(BaseFrameworkElement child)
    {
      return new Rect();
    }

    public override void SetChildRect(BaseFrameworkElement child, Rect rect, LayoutOverrides layoutOverrides, LayoutOverrides overridesToIgnore, LayoutOverrides nonExplicitOverrides, SetRectMode setRectMode)
    {
    }
  }
}
