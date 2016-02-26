// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.RootElementSizeBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class RootElementSizeBehavior : BaseSizeBehavior
  {
    public override string ActionString
    {
      get
      {
        return StringTable.UndoUnitResize;
      }
    }

    public RootElementSizeBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    protected override Size ComputeSizeFromScaledDimensions(double scaledWidth, double scaledHeight)
    {
      return new Size(Math.Max(scaledWidth, 0.0), Math.Max(scaledHeight, 0.0));
    }

    protected override void ApplyScale(Vector scale, Point center)
    {
      Size newSize = this.ComputeNewSize(scale);
      BaseFrameworkElement panelClosestToRoot = this.ActiveSceneViewModel.FindPanelClosestToRoot();
      panelClosestToRoot.LayoutDesigner.SetRootSize(panelClosestToRoot, newSize, !object.Equals((object) newSize.Width, (object) this.StartSize.Width), !object.Equals((object) newSize.Height, (object) this.StartSize.Height));
      using (this.ActiveView.ViewModel.ForceBaseValue())
      {
        this.ActiveSceneViewModel.Document.OnUpdatedEditTransaction();
        if (!object.Equals((object) newSize.Width, (object) this.StartSize.Width) && !double.IsNaN(this.BaseEditingElement.Width))
          this.BaseEditingElement.ClearValue(DesignTimeProperties.DesignWidthProperty);
        if (object.Equals((object) newSize.Height, (object) this.StartSize.Height) || double.IsNaN(this.BaseEditingElement.Height))
          return;
        this.BaseEditingElement.ClearValue(DesignTimeProperties.DesignHeightProperty);
      }
    }
  }
}
