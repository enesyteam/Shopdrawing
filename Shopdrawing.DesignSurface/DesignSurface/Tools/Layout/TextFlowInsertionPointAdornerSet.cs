// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Layout.TextFlowInsertionPointAdornerSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Designers;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.Tools.Layout
{
  internal class TextFlowInsertionPointAdornerSet : AdornerSet
  {
    private TextFlowInsertionPoint insertionPoint;

    public TextFlowInsertionPointAdornerSet(ToolBehaviorContext toolContext, BaseFrameworkElement adornedElement, TextFlowInsertionPoint insertionPoint)
      : base(toolContext, (SceneElement) adornedElement)
    {
      this.insertionPoint = insertionPoint;
    }

    protected override void CreateAdorners()
    {
      this.AddAdorner((Adorner) new TextFlowInsertionPointAdorner((AdornerSet) this, (BaseFlowInsertionPoint) this.insertionPoint));
    }
  }
}
