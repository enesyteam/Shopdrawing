// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.AdornerAnimation
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public sealed class AdornerAnimation : IAdornerAnimation, IDisposable
  {
    private double lerpValue = -1.0;
    private AnimationClock animationClock;
    private DoubleAnimation proceduralAnimation;
    private double startLerpValue;
    private double endLerpValue;
    private Action<IAdornerAnimation, AdornerAnimationNotification> animationEventHandlers;
    private string name;
    private AnimatableAdornerSet adornerSet;
    private object clientData;

    public AnimatableAdornerSet AdornerSet
    {
      get
      {
        return this.adornerSet;
      }
    }

    public double LerpValue
    {
      get
      {
        return this.lerpValue;
      }
    }

    public string Name
    {
      get
      {
        return this.name;
      }
    }

    public object ClientData
    {
      get
      {
        return this.clientData;
      }
      set
      {
        this.clientData = value;
      }
    }

    public event Action<IAdornerAnimation, AdornerAnimationNotification> AnimationEventHandlers
    {
      add
      {
        this.animationEventHandlers += value;
      }
      remove
      {
        this.animationEventHandlers -= value;
      }
    }

    public AdornerAnimation(AnimatableAdornerSet adornerSet, string name, double duration, double startLerpValue, double endLerpValue)
    {
      this.name = name;
      this.proceduralAnimation = new DoubleAnimation();
      this.startLerpValue = startLerpValue;
      this.endLerpValue = endLerpValue;
      this.proceduralAnimation.From = new double?(this.startLerpValue);
      this.proceduralAnimation.To = new double?(this.endLerpValue);
      this.proceduralAnimation.Duration = new Duration(TimeSpan.FromSeconds(duration));
      this.adornerSet = adornerSet;
    }

    public void Dispose()
    {
      this.animationEventHandlers = (Action<IAdornerAnimation, AdornerAnimationNotification>) null;
      GC.SuppressFinalize((object) this);
    }

    public void Start()
    {
      this.animationClock = (AnimationClock) this.proceduralAnimation.CreateClock(true);
      this.animationClock.CurrentTimeInvalidated += new EventHandler(this.animationClock_CurrentTimeInvalidated);
      this.animationClock.Completed += new EventHandler(this.animationClock_Completed);
      this.Notify(AdornerAnimationNotification.AnimationStart);
    }

    public void Stop()
    {
      if (this.animationClock == null)
        return;
      this.animationClock.Controller.Stop();
      this.animationClock.CurrentTimeInvalidated -= new EventHandler(this.animationClock_CurrentTimeInvalidated);
      this.animationClock.Completed -= new EventHandler(this.animationClock_Completed);
      this.proceduralAnimation = (DoubleAnimation) null;
      this.animationClock = (AnimationClock) null;
      this.Notify(AdornerAnimationNotification.AnimationStop);
    }

    public void Notify(AdornerAnimationNotification notification)
    {
      if (this.animationEventHandlers == null)
        return;
      this.animationEventHandlers((IAdornerAnimation) this, notification);
    }

    private void animationClock_Completed(object sender, EventArgs e)
    {
      this.Notify(AdornerAnimationNotification.AnimationComplete);
    }

    private void animationClock_CurrentTimeInvalidated(object sender, EventArgs e)
    {
      this.lerpValue = this.proceduralAnimation.GetCurrentValue(this.startLerpValue, this.endLerpValue, this.animationClock);
      this.Notify(AdornerAnimationNotification.AnimationTick);
    }
  }
}
