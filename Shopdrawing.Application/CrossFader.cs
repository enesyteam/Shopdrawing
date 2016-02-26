// Decompiled with JetBrains decompiler
// Type: Shopdrawing.App.CrossFader
// Assembly: Shopdrawing.App, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: DDD2F1CF-BB6D-4068-A4D9-DDBDD16D6739
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Shopdrawing.App.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Shopdrawing.App
{
  public class CrossFader : Grid
  {
    public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof (object), typeof (CrossFader), new PropertyMetadata(new PropertyChangedCallback(CrossFader.ContentChanged)));
    public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.Register("ContentTemplate", typeof (DataTemplate), typeof (CrossFader));
    public static readonly DependencyProperty EnterStoryboardProperty = DependencyProperty.Register("EnterStoryboardProperty", typeof (Storyboard), typeof (CrossFader));
    public static readonly DependencyProperty ExitStoryboardProperty = DependencyProperty.Register("ExitStoryboardProperty", typeof (Storyboard), typeof (CrossFader));
    private FrameworkElement toBeStopped;

    public object Content
    {
      get
      {
        return this.GetValue(CrossFader.ContentProperty);
      }
      set
      {
        this.SetValue(CrossFader.ContentProperty, value);
      }
    }

    public DataTemplate ContentTemplate
    {
      get
      {
        return (DataTemplate) this.GetValue(CrossFader.ContentTemplateProperty);
      }
      set
      {
        this.SetValue(CrossFader.ContentTemplateProperty, (object) value);
      }
    }

    public Storyboard EnterStoryboard
    {
      get
      {
        return (Storyboard) this.GetValue(CrossFader.EnterStoryboardProperty);
      }
      set
      {
        this.SetValue(CrossFader.EnterStoryboardProperty, (object) value);
      }
    }

    public Storyboard ExitStoryboard
    {
      get
      {
        return (Storyboard) this.GetValue(CrossFader.ExitStoryboardProperty);
      }
      set
      {
        this.SetValue(CrossFader.ExitStoryboardProperty, (object) value);
      }
    }

    static CrossFader()
    {
      Storyboard storyboard1 = new Storyboard();
      DoubleAnimation doubleAnimation1 = new DoubleAnimation(0.0, 1.0, new Duration(TimeSpan.FromSeconds(0.5)));
      Storyboard.SetTargetProperty((DependencyObject) doubleAnimation1, new PropertyPath("(0)", new object[1]
      {
        (object) UIElement.OpacityProperty
      }));
      storyboard1.Children.Add((Timeline) doubleAnimation1);
      CrossFader.EnterStoryboardProperty.OverrideMetadata(typeof (CrossFader), new PropertyMetadata((object) storyboard1));
      Storyboard storyboard2 = new Storyboard();
      DoubleAnimation doubleAnimation2 = new DoubleAnimation(0.0, new Duration(TimeSpan.FromSeconds(0.5)));
      Storyboard.SetTargetProperty((DependencyObject) doubleAnimation2, new PropertyPath("(0)", new object[1]
      {
        (object) UIElement.OpacityProperty
      }));
      storyboard2.Children.Add((Timeline) doubleAnimation2);
      CrossFader.ExitStoryboardProperty.OverrideMetadata(typeof (CrossFader), new PropertyMetadata((object) storyboard2));
    }

    protected virtual void OnContentChanged(object oldContent, object newContent)
    {
      this.StopPreviousVisual();
      Storyboard enterStoryboard = this.EnterStoryboard;
      ContentPresenter contentPresenter = new ContentPresenter();
      contentPresenter.Content = newContent;
      contentPresenter.ContentTemplate = this.ContentTemplate;
      this.Children.Add((UIElement) contentPresenter);
      if (enterStoryboard == null)
        return;
      contentPresenter.ApplyTemplate();
      if (this.IsLoaded)
      {
        if (this.ContentTemplate != null)
          enterStoryboard.Begin((FrameworkElement) contentPresenter, (FrameworkTemplate) this.ContentTemplate);
        else
          enterStoryboard.Begin((FrameworkElement) contentPresenter);
      }
      this.toBeStopped = (FrameworkElement) contentPresenter;
    }

    private void StopPreviousVisual()
    {
      if (this.toBeStopped == null)
        return;
      Storyboard exitStoryboard = this.ExitStoryboard;
      if (exitStoryboard != null && this.IsLoaded)
      {
        Storyboard outroClone = exitStoryboard.Clone();
        FrameworkElement stopTarget = this.toBeStopped;
        EventHandler completedHandler = (EventHandler) null;
        completedHandler = (EventHandler) ((sender, e) =>
        {
          if (outroClone.GetCurrentState(stopTarget) == ClockState.Active)
            return;
          this.Children.Remove((UIElement) stopTarget);
          outroClone.CurrentStateInvalidated -= completedHandler;
        });
        outroClone.CurrentStateInvalidated += completedHandler;
        outroClone.Begin(this.toBeStopped, (FrameworkTemplate) this.ContentTemplate, HandoffBehavior.SnapshotAndReplace, true);
      }
      else
        this.Children.Remove((UIElement) this.toBeStopped);
      this.toBeStopped = (FrameworkElement) null;
    }

    private static void ContentChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
      ((CrossFader) obj).OnContentChanged(e.OldValue, e.NewValue);
    }
  }
}
