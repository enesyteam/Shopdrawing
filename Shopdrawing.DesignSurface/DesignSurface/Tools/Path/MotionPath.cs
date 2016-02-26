// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.MotionPath
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  internal class MotionPath
  {
    private List<double> times;
    private Vector topLeftOffset;
    private KeyFrameAnimationSceneNode animationX;
    private KeyFrameAnimationSceneNode animationY;
    private KeyFrameAnimationSceneNode animationLeft;
    private KeyFrameAnimationSceneNode animationTop;
    private ForwardAnimationEvaluator xEvaluator;
    private ForwardAnimationEvaluator yEvaluator;
    private ForwardAnimationEvaluator leftEvaluator;
    private ForwardAnimationEvaluator topEvaluator;

    public Vector TopLeftOffset
    {
      get
      {
        return this.topLeftOffset;
      }
      set
      {
        this.topLeftOffset = value;
      }
    }

    public KeyFrameAnimationSceneNode AnimationX
    {
      get
      {
        return this.animationX;
      }
      set
      {
        this.animationX = value;
      }
    }

    public KeyFrameAnimationSceneNode AnimationY
    {
      get
      {
        return this.animationY;
      }
      set
      {
        this.animationY = value;
      }
    }

    public KeyFrameAnimationSceneNode AnimationLeft
    {
      get
      {
        return this.animationLeft;
      }
      set
      {
        this.animationLeft = value;
      }
    }

    public KeyFrameAnimationSceneNode AnimationTop
    {
      get
      {
        return this.animationTop;
      }
      set
      {
        this.animationTop = value;
      }
    }

    public List<double> Times
    {
      get
      {
        return this.times;
      }
    }

    public double XPosition
    {
      get
      {
        return this.xEvaluator.CurrentValue + this.leftEvaluator.CurrentValue - this.TopLeftOffset.X;
      }
    }

    public double YPosition
    {
      get
      {
        return this.yEvaluator.CurrentValue + this.topEvaluator.CurrentValue - this.TopLeftOffset.Y;
      }
    }

    public bool XIsHolding
    {
      get
      {
        if (this.xEvaluator.CurrentSegmentIsHolding)
          return this.leftEvaluator.CurrentSegmentIsHolding;
        return false;
      }
    }

    public bool YIsHolding
    {
      get
      {
        if (this.yEvaluator.CurrentSegmentIsHolding)
          return this.topEvaluator.CurrentSegmentIsHolding;
        return false;
      }
    }

    public void UpdatePath()
    {
      this.GenerateTimes();
    }

    public void MoveToTime(double time)
    {
      this.xEvaluator.MoveToTime(time);
      this.yEvaluator.MoveToTime(time);
      this.leftEvaluator.MoveToTime(time);
      this.topEvaluator.MoveToTime(time);
    }

    private void GenerateTimes()
    {
      this.times = new List<double>();
      this.AddKeyTimes(this.times, this.animationX.KeyFrames);
      this.AddKeyTimes(this.times, this.animationY.KeyFrames);
      this.AddKeyTimes(this.times, this.animationLeft.KeyFrames);
      this.AddKeyTimes(this.times, this.animationTop.KeyFrames);
      this.times.Sort();
      this.xEvaluator = new ForwardAnimationEvaluator(this.animationX.KeyFrames.GetEnumerator());
      this.yEvaluator = new ForwardAnimationEvaluator(this.animationY.KeyFrames.GetEnumerator());
      this.leftEvaluator = new ForwardAnimationEvaluator(this.animationLeft.KeyFrames.GetEnumerator());
      this.topEvaluator = new ForwardAnimationEvaluator(this.animationTop.KeyFrames.GetEnumerator());
    }

    private void AddKeyTimes(List<double> keyTimes, IEnumerable<KeyFrameSceneNode> keyFrames)
    {
      foreach (KeyFrameSceneNode keyFrameSceneNode in keyFrames)
      {
        if (!keyTimes.Contains(keyFrameSceneNode.Time))
          keyTimes.Add(keyFrameSceneNode.Time);
      }
    }

    public static double GetKeyframeValue(KeyFrameSceneNode node)
    {
      if (node.Value is double)
        return (double) node.Value;
      if (node.IsValueExpression(node.ValueProperty))
      {
        object computedValue = node.GetComputedValue(node.ValueProperty);
        if (computedValue is double)
          return (double) computedValue;
      }
      return 0.0;
    }
  }
}
