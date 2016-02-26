// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.ElementLayoutBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Designers;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class ElementLayoutBehavior : AdornedToolBehavior
  {
    public ElementLayoutBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    protected override bool OnClickEnd(Point pointerPosition, int clickCount)
    {
      ElementLayoutLockAdorner layoutLockAdorner = (ElementLayoutLockAdorner) this.ActiveAdorner;
      SceneDocument activeDocument = this.ActiveDocument;
      BaseFrameworkElement element = layoutLockAdorner.Element;
      ILayoutDesigner designerForChild = this.ActiveSceneViewModel.GetLayoutDesignerForChild((SceneElement) element, true);
      HorizontalAlignment horizontalAlignment = (HorizontalAlignment) element.GetComputedValue(BaseFrameworkElement.HorizontalAlignmentProperty);
      VerticalAlignment verticalAlignment = (VerticalAlignment) element.GetComputedValue(BaseFrameworkElement.VerticalAlignmentProperty);
      using (SceneEditTransaction editTransaction = activeDocument.CreateEditTransaction(StringTable.UndoUnitToggleLayoutAdorner))
      {
        bool flag1 = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
        if (layoutLockAdorner.IsX)
        {
          bool flag2 = horizontalAlignment == HorizontalAlignment.Left || horizontalAlignment == HorizontalAlignment.Stretch;
          bool flag3 = horizontalAlignment == HorizontalAlignment.Right || horizontalAlignment == HorizontalAlignment.Stretch;
          if (layoutLockAdorner.Type == ElementLayoutAdornerType.Left)
          {
            flag2 = !flag2;
            if (!flag2 && !flag3 && !flag1)
              flag3 = true;
          }
          else
          {
            flag3 = !flag3;
            if (!flag2 && !flag3 && !flag1)
              flag2 = true;
          }
          HorizontalAlignment alignment = !flag2 ? (flag3 ? HorizontalAlignment.Right : HorizontalAlignment.Center) : (flag3 ? HorizontalAlignment.Stretch : HorizontalAlignment.Left);
          if (alignment != horizontalAlignment)
            designerForChild.SetHorizontalAlignment(element, alignment);
        }
        else
        {
          bool flag2 = verticalAlignment == VerticalAlignment.Top || verticalAlignment == VerticalAlignment.Stretch;
          bool flag3 = verticalAlignment == VerticalAlignment.Bottom || verticalAlignment == VerticalAlignment.Stretch;
          if (layoutLockAdorner.Type == ElementLayoutAdornerType.Top)
          {
            flag2 = !flag2;
            if (!flag2 && !flag3 && !flag1)
              flag3 = true;
          }
          else
          {
            flag3 = !flag3;
            if (!flag2 && !flag3 && !flag1)
              flag2 = true;
          }
          VerticalAlignment alignment = !flag2 ? (flag3 ? VerticalAlignment.Bottom : VerticalAlignment.Center) : (flag3 ? VerticalAlignment.Stretch : VerticalAlignment.Top);
          if (alignment != verticalAlignment)
            designerForChild.SetVerticalAlignment(element, alignment);
        }
        editTransaction.Commit();
      }
      return base.OnClickEnd(pointerPosition, clickCount);
    }
  }
}
