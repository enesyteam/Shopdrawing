// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.ForwardAnimationEvaluator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;
using System.Windows.Media.Animation;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  internal class ForwardAnimationEvaluator
  {
    private static readonly KeySpline linearKeySpline = new KeySpline();
    private IEnumerator<KeyFrameSceneNode> keyFrameEnumerator;
    private double lastEvaluatedTime;
    private double curSegmentStartTime;
    private double curSegmentEndTime;
    private double? curSegmentStartValue;
    private double? curSegmentEndValue;
    private KeySpline keySpline;
    private IEasingFunctionDefinition easingFunction;
    private DoubleInterpolator valueInterpolator;

    public double CurrentValue
    {
      get
      {
        if (this.lastEvaluatedTime < 0.0)
          throw new InvalidOperationException("Need to move to a time before querying");
        if (!this.curSegmentStartValue.HasValue)
        {
          if (this.curSegmentEndValue.HasValue)
            return this.curSegmentEndValue.Value;
          return 0.0;
        }
        if (!this.curSegmentEndValue.HasValue)
          return this.curSegmentStartValue.Value;
        double num = (this.lastEvaluatedTime - this.curSegmentStartTime) / (this.curSegmentEndTime - this.curSegmentStartTime);
        return this.valueInterpolator.Interpolate(this.keySpline == null ? (this.easingFunction == null ? (Tolerances.AreClose(this.lastEvaluatedTime, this.curSegmentEndTime) ? 1.0 : 0.0) : this.easingFunction.Ease(num)) : this.keySpline.GetSplineProgress(num));
      }
    }

    public bool CurrentSegmentIsHolding
    {
      get
      {
        if (this.lastEvaluatedTime < 0.0)
          throw new InvalidOperationException("Need to move to a time before querying");
        if (this.easingFunction == null)
          return this.keySpline == null;
        return false;
      }
    }

    public ForwardAnimationEvaluator(IEnumerator<KeyFrameSceneNode> keyFrameEnumerator)
    {
      this.keyFrameEnumerator = keyFrameEnumerator;
      this.Reset();
    }

    public void MoveToTime(double time)
    {
      if (time < 0.0)
        throw new ArgumentException("negative times are not valid");
      if (Tolerances.LessThan(time, this.lastEvaluatedTime))
        throw new ArgumentException("Evaluating backward in time is not supported. Call Reset() first");
      this.lastEvaluatedTime = time;
      if (time >= this.curSegmentStartTime && time < this.curSegmentEndTime)
        return;
      while (time > this.curSegmentEndTime)
      {
        this.curSegmentStartTime = this.curSegmentStartTime == -1.0 ? 0.0 : this.curSegmentEndTime;
        this.curSegmentStartValue = this.curSegmentEndValue;
        if (this.keyFrameEnumerator.MoveNext())
        {
          this.easingFunction = (IEasingFunctionDefinition) null;
          this.keySpline = (KeySpline) null;
          switch (this.keyFrameEnumerator.Current.InterpolationType)
          {
            case KeyFrameInterpolationType.Discrete:
              this.keySpline = (KeySpline) null;
              break;
            case KeyFrameInterpolationType.Spline:
              this.keySpline = this.keyFrameEnumerator.Current.KeySpline;
              if (this.keySpline == null)
              {
                this.keySpline = ForwardAnimationEvaluator.linearKeySpline;
                break;
              }
              break;
            case KeyFrameInterpolationType.Linear:
              this.keySpline = ForwardAnimationEvaluator.linearKeySpline;
              break;
            case KeyFrameInterpolationType.Easing:
              this.easingFunction = this.keyFrameEnumerator.Current.EasingFunction;
              if (this.easingFunction == null)
              {
                this.keySpline = ForwardAnimationEvaluator.linearKeySpline;
                break;
              }
              break;
            default:
              this.keySpline = (KeySpline) null;
              break;
          }
          this.curSegmentEndTime = this.keyFrameEnumerator.Current.Time;
          this.curSegmentEndValue = new double?(MotionPath.GetKeyframeValue(this.keyFrameEnumerator.Current));
        }
        else
        {
          this.keySpline = (KeySpline) null;
          this.curSegmentEndTime = double.PositiveInfinity;
          this.curSegmentEndValue = new double?();
        }
      }
      if (this.curSegmentStartValue.HasValue && this.curSegmentEndValue.HasValue)
        this.valueInterpolator = (DoubleInterpolator) new LinearDoubleInterpolator(this.curSegmentStartValue.Value, this.curSegmentEndValue.Value);
      else
        this.valueInterpolator = (DoubleInterpolator) null;
    }

    public void Reset()
    {
      this.lastEvaluatedTime = -1.0;
      this.keyFrameEnumerator.Reset();
      this.curSegmentStartTime = -1.0;
      this.curSegmentEndTime = -1.0;
      this.curSegmentStartValue = new double?();
      this.curSegmentEndValue = new double?();
      this.keySpline = (KeySpline) null;
      this.easingFunction = (IEasingFunctionDefinition) null;
    }
  }
}
