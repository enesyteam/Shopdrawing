// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.PreviewElementHighlighter
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal class PreviewElementHighlighter
  {
    private AnimatableAdornerSet adornerSet;
    private AdornerLayer adornerLayer;
    private PreviewElementHighlighter.CreateAdornerSet createAdornerSet;
    private PreviewElementHighlighter.VerifyIsEnabled verifyIsEnabled;

    public SceneElement PreviewElement
    {
      get
      {
        if (this.adornerSet != null)
          return this.adornerSet.Element;
        return (SceneElement) null;
      }
      set
      {
        bool flag = false;
        if (this.PreviewElement == value)
          return;
        if (this.adornerSet != null)
        {
          if (this.adornerSet.ViewModel == null || this.adornerSet.ViewModel.DefaultView == null || this.adornerSet.ViewModel.DefaultView.IsClosing)
            return;
          this.adornerSet.StartAnimation("PreviewBox", 0.2, true, (Action<IAdornerAnimation, AdornerAnimationNotification>) ((sender, eventarg) =>
          {
            if (eventarg != AdornerAnimationNotification.AnimationComplete || sender.AdornerSet.ViewModel == null || (sender.AdornerSet.ViewModel.DesignerContext == null || sender.AdornerSet.ViewModel.DesignerContext.ActiveView == null))
              return;
            this.adornerLayer.Remove((IAdornerSet) sender.AdornerSet);
          }));
          this.adornerSet = (AnimatableAdornerSet) null;
          flag = true;
        }
        if (!this.verifyIsEnabled())
        {
          if (!flag)
            return;
          this.adornerLayer.Update2D();
        }
        else
        {
          if (value != null && value.IsViewObjectValid)
          {
            this.adornerSet = this.createAdornerSet(value);
            this.adornerLayer.Add((IAdornerSet) this.adornerSet);
            this.adornerSet.Update();
            this.adornerSet.StartAnimation("PreviewBox", 0.2, false, (Action<IAdornerAnimation, AdornerAnimationNotification>) null);
          }
          if (!flag)
            return;
          this.adornerLayer.Update2D();
        }
      }
    }

    public PreviewElementHighlighter(AdornerLayer adornerLayer, PreviewElementHighlighter.CreateAdornerSet createAdornerSet, PreviewElementHighlighter.VerifyIsEnabled verifyIsEnabled)
    {
      this.adornerLayer = adornerLayer;
      this.createAdornerSet = createAdornerSet;
      this.verifyIsEnabled = verifyIsEnabled;
    }

    public delegate AnimatableAdornerSet CreateAdornerSet(SceneElement adornedElement);

    public delegate bool VerifyIsEnabled();
  }
}
