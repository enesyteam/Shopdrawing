// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.ElementToolBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools.Selection;
using Microsoft.Expression.DesignSurface.UserInterface;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal abstract class ElementToolBehavior : ToolBehavior
  {
    private AdornerToolTipService adornerToolTipService = new AdornerToolTipService();

    protected virtual Cursor DefaultCursor
    {
      get
      {
        return ToolCursors.CrosshairCursor;
      }
    }

    protected ElementToolBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    protected override void OnResume()
    {
      base.OnResume();
      this.UpdateCursor();
    }

    protected override void OnSuspend()
    {
      base.OnSuspend();
      this.adornerToolTipService.MouseOverAdorner = (IAdorner) null;
    }

    protected override sealed bool OnHover(Point pointerPosition)
    {
      bool flag = false;
      IAdorner hitAdorner = this.ActiveView.AdornerService.GetHitAdorner(this.MouseDevice);
      this.adornerToolTipService.MouseOverAdorner = hitAdorner;
      if (hitAdorner != null)
        flag = this.OnHoverOverAdorner(hitAdorner);
      if (!flag)
        flag = this.OnHoverOverNonAdorner(pointerPosition);
      return flag;
    }

    protected virtual bool OnHoverOverAdorner(IAdorner adorner)
    {
      if (adorner.AdornerSet.Behavior == null)
        return false;
      this.Cursor = adorner.AdornerSet.GetCursor(adorner);
      return true;
    }

    protected virtual bool OnHoverOverNonAdorner(Point pointerPosition)
    {
      this.Cursor = this.DefaultCursor;
      return true;
    }

    protected override sealed bool OnButtonDown(Point pointerPosition)
    {
      this.adornerToolTipService.MouseOverAdorner = (IAdorner) null;
      bool flag = false;
      IAdorner hitAdorner = this.ActiveView.AdornerService.GetHitAdorner(this.MouseDevice);
      if (hitAdorner != null)
        flag = this.OnButtonDownOverAdorner(hitAdorner);
      if (!flag)
        flag = this.OnButtonDownOverNonAdorner(pointerPosition);
      return flag;
    }

    protected virtual bool OnButtonDownOverAdorner(IAdorner adorner)
    {
      ToolBehavior behavior = adorner.AdornerSet.Behavior;
      if (behavior == null)
        return false;
      AdornedToolBehavior adornedToolBehavior = behavior as AdornedToolBehavior;
      if (adornedToolBehavior != null)
        adornedToolBehavior.SetActiveAdorner(adorner);
      this.PushBehavior(behavior);
      return true;
    }

    protected virtual bool OnButtonDownOverNonAdorner(Point pointerPosition)
    {
      return true;
    }

    protected override bool OnRightButtonDown(Point pointerPosition)
    {
      this.adornerToolTipService.MouseOverAdorner = (IAdorner) null;
      new ElementEditorBehavior(this.ToolBehaviorContext, false, true, true, false, false, (Cursor) null, (Cursor) null, (Cursor) null, (ToolBehavior) null).HandleRightButtonDown(pointerPosition);
      return true;
    }
  }
}
