// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.WheelGestureData
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Model;
using System;
using System.Windows;

namespace Microsoft.Windows.Design.Interaction
{
  public class WheelGestureData : GestureData
  {
    private int _delta;

    public int Delta
    {
      get
      {
        return this._delta;
      }
    }

    public WheelGestureData(EditingContext context, ModelItem sourceModel, ModelItem targetModel, int delta)
      : this(context, sourceModel, targetModel, delta, (DependencyObject) null, (DependencyObject) null)
    {
    }

    public WheelGestureData(EditingContext context, ModelItem sourceModel, ModelItem targetModel, int delta, DependencyObject sourceAdorner, DependencyObject targetAdorner)
      : base(context, sourceModel, targetModel, sourceAdorner, targetAdorner)
    {
      this._delta = delta;
    }

    public static WheelGestureData FromEventArgs(ExecutedToolEventArgs e)
    {
      if (e == null)
        throw new ArgumentNullException("e");
      return GestureData.FromParameter<WheelGestureData>(e.Parameter);
    }

    public static WheelGestureData FromEventArgs(CanExecuteToolEventArgs e)
    {
      if (e == null)
        throw new ArgumentNullException("e");
      return GestureData.FromParameter<WheelGestureData>(e.Parameter);
    }
  }
}
