// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Layout.DockPanelLayoutAdornerSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.Tools.Layout
{
  internal class DockPanelLayoutAdornerSet : AdornerSet
  {
    public DockPanelLayoutAdornerSet(ToolBehaviorContext toolContext, BaseFrameworkElement adornedElement)
      : base(toolContext, (SceneElement) adornedElement)
    {
    }

    protected override void CreateAdorners()
    {
      if (!ProjectNeutralTypes.DockPanel.IsAssignableFrom((ITypeId) this.Element.Type))
        return;
      this.CreateDockPanelAdorners();
    }

    private void CreateDockPanelAdorners()
    {
      this.AddAdorner((Adorner) new DockPanelAdorner((AdornerSet) this, Dock.Left));
      this.AddAdorner((Adorner) new DockPanelAdorner((AdornerSet) this, Dock.Right));
      this.AddAdorner((Adorner) new DockPanelAdorner((AdornerSet) this, Dock.Top));
      this.AddAdorner((Adorner) new DockPanelAdorner((AdornerSet) this, Dock.Bottom));
    }
  }
}
