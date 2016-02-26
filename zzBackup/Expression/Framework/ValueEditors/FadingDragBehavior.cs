// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ValueEditors.FadingDragBehavior
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Windows;

namespace Microsoft.Expression.Framework.ValueEditors
{
  public class FadingDragBehavior : INumberEditorDragBehavior
  {
    private double thresholdOffset = 5.0;
    private const double minimumInterval = 60.0;
    private const double defaultOffset = 5.0;
    private const double fadingOffset = 0.08;
    private DateTime timeLast;
    private NumberEditor activeNumberEditor;

    public FadingDragBehavior()
    {
      this.timeLast = DateTime.Now;
    }

    public void BeginDrag(NumberEditor sender)
    {
      this.activeNumberEditor = sender;
      this.timeLast = DateTime.Now;
    }

    public double GetDragOffsetAmount(NumberEditor sender, Vector offset)
    {
      return this.CalculateDeltaFixedSpeed(offset);
    }

    public void EndDrag(NumberEditor sender)
    {
      this.activeNumberEditor = (NumberEditor) null;
    }

    private void UpdateDistanceUnitFixedSpeed()
    {
      DateTime now = DateTime.Now;
      if ((now - this.timeLast).TotalMilliseconds > 60.0)
        this.thresholdOffset = 5.0;
      else if (this.thresholdOffset > 1.0)
        this.thresholdOffset -= 0.08;
      this.timeLast = now;
    }

    private double CalculateDeltaFixedSpeed(Vector offset)
    {
      this.UpdateDistanceUnitFixedSpeed();
      double num1 = Math.Abs(offset.X);
      double num2 = Math.Abs(offset.Y);
      if (num1 >= this.thresholdOffset || num2 >= this.thresholdOffset)
        return Math.Round((num1 > num2 ? num1 : num2) / this.thresholdOffset);
      return 0.0;
    }
  }
}
