// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.AnimatableAdornerSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public abstract class AnimatableAdornerSet : AdornerSet
  {
    private List<AdornerAnimation> activeAnimations;
    private bool isShutdown;

    public IList<AdornerAnimation> ActiveAnimations
    {
      get
      {
        return (IList<AdornerAnimation>) this.activeAnimations;
      }
    }

    internal AnimatableAdornerSet(ToolBehaviorContext toolContext, SceneElement adornedElement)
      : this(toolContext, adornedElement, AdornerSetOrderTokens.MediumPriority)
    {
    }

    internal AnimatableAdornerSet(ToolBehaviorContext toolContext, AdornerElementSet adornedElementSet)
      : this(toolContext, adornedElementSet, AdornerSetOrderTokens.MediumPriority)
    {
    }

    internal AnimatableAdornerSet(ToolBehaviorContext toolContext, SceneElement adornedElement, AdornerSetOrder order)
      : this(toolContext, toolContext.View.Artboard.AdornerLayer.CreateOrGetAdornerElementSetForElement(adornedElement), order)
    {
    }

    internal AnimatableAdornerSet(ToolBehaviorContext toolContext, AdornerElementSet adornedElementSet, AdornerSetOrder order)
      : base(toolContext, adornedElementSet, order)
    {
      this.activeAnimations = new List<AdornerAnimation>();
    }

    public override void OnRemove()
    {
      this.StopAllAnimations();
      this.isShutdown = true;
      base.OnRemove();
    }

    public void StartAnimation(string name, double duration, bool reverse, Action<IAdornerAnimation, AdornerAnimationNotification> listener)
    {
      double startLerpValue = reverse ? 1.0 : 0.0;
      double endLerpValue = reverse ? 0.0 : 1.0;
      AdornerAnimation adornerAnimation1 = Enumerable.FirstOrDefault<AdornerAnimation>((IEnumerable<AdornerAnimation>) this.activeAnimations, (Func<AdornerAnimation, bool>) (item => item.Name == name));
      if (adornerAnimation1 != null)
      {
        startLerpValue = adornerAnimation1.LerpValue;
        adornerAnimation1.Stop();
        adornerAnimation1.Dispose();
        this.activeAnimations.Remove(adornerAnimation1);
      }
      AdornerAnimation adornerAnimation2 = new AdornerAnimation(this, name, duration, startLerpValue, endLerpValue);
      adornerAnimation2.AnimationEventHandlers += new Action<IAdornerAnimation, AdornerAnimationNotification>(this.OnAnimationNotify);
      if (listener != null)
        adornerAnimation2.AnimationEventHandlers += listener;
      this.activeAnimations.Add(adornerAnimation2);
      adornerAnimation2.Start();
    }

    private void OnAnimationNotify(IAdornerAnimation sender, AdornerAnimationNotification eventArg)
    {
      if (!this.ShouldNotify())
        return;
      AdornerAnimation adornerAnimation = (AdornerAnimation) sender;
      if (eventArg == AdornerAnimationNotification.AnimationComplete)
      {
        adornerAnimation.Stop();
        adornerAnimation.AnimationEventHandlers -= new Action<IAdornerAnimation, AdornerAnimationNotification>(this.OnAnimationNotify);
        this.activeAnimations.Remove(adornerAnimation);
        adornerAnimation.Dispose();
      }
      else
      {
        if (eventArg != AdornerAnimationNotification.AnimationTick)
          return;
        this.InvalidateRender();
        this.Update();
      }
    }

    private bool ShouldNotify()
    {
      return !this.isShutdown && this.ViewModel != null && (this.ViewModel.DesignerContext != null && this.ViewModel.DesignerContext.ActiveView != null) && this.ViewModel.DesignerContext.ActiveSceneViewModel != null;
    }

    private void StopAllAnimations()
    {
      foreach (AdornerAnimation adornerAnimation in (IEnumerable<AdornerAnimation>) this.ActiveAnimations)
      {
        adornerAnimation.Stop();
        adornerAnimation.Dispose();
      }
      this.ActiveAnimations.Clear();
    }
  }
}
