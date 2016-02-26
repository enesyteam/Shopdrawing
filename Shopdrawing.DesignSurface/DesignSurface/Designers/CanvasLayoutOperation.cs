// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Designers.CanvasLayoutOperation
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Designers
{
  public class CanvasLayoutOperation : LayoutOperation
  {
    public CanvasLayoutOperation(ILayoutDesigner designer, BaseFrameworkElement child)
      : base(designer, child)
    {
    }

    protected override void ComputeIdealMargin()
    {
      this.Settings.Margin = (Thickness) this.Child.GetDefaultValueAsWpf(BaseFrameworkElement.MarginProperty);
    }

    protected override void ComputeMarginOverrides()
    {
    }

    protected override void SetMarginChanges()
    {
      double num1 = (double) this.Child.GetComputedValue(CanvasElement.LeftProperty);
      double num2 = (double) this.Child.GetComputedValue(CanvasElement.TopProperty);
      if (num1 != this.ChildRect.Left)
      {
        IPropertyId propertyKey = (IPropertyId) this.Child.ViewModel.ProjectContext.ResolveProperty(CanvasElement.RightProperty);
        if (this.ChildRect.Left == 0.0 && (propertyKey == null || double.IsNaN((double) this.Child.GetComputedValue(propertyKey))))
          this.Child.ClearValue(CanvasElement.LeftProperty);
        else
          this.Child.SetValue(CanvasElement.LeftProperty, (object) this.ChildRect.Left);
      }
      if (num2 != this.ChildRect.Top)
      {
        IPropertyId propertyKey = (IPropertyId) this.Child.ViewModel.ProjectContext.ResolveProperty(CanvasElement.BottomProperty);
        if (this.ChildRect.Top == 0.0 && (propertyKey == null || double.IsNaN((double) this.Child.GetComputedValue(propertyKey))))
          this.Child.ClearValue(CanvasElement.TopProperty);
        else
          this.Child.SetValue(CanvasElement.TopProperty, (object) this.ChildRect.Top);
      }
      base.SetMarginChanges();
    }
  }
}
