// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.DragGestureData
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Model;
using MS.Internal.Properties;
using System;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Windows.Design.Interaction
{
  public class DragGestureData : MouseGestureData
  {
    private DragDropEffects _effects;
    private DragDropEffects _allowedEffects;
    private IDataObject _data;

    public DragDropEffects Effects
    {
      get
      {
        return this._effects;
      }
      set
      {
        if ((value & this._allowedEffects) != value)
          throw new ArgumentException(Resources.Error_EffectsNotAllowed);
        this._effects = value;
      }
    }

    public DragDropEffects AllowedEffects
    {
      get
      {
        return this._allowedEffects;
      }
    }

    public IDataObject Data
    {
      get
      {
        return this._data;
      }
    }

    public DragGestureData(EditingContext context, ModelItem sourceModel, ModelItem targetModel, Visual coordinateReference, Point startPosition, Point currentPosition, DragDropEffects allowedEffects, IDataObject data)
      : this(context, sourceModel, targetModel, coordinateReference, startPosition, currentPosition, allowedEffects, data, (DependencyObject) null, (DependencyObject) null)
    {
    }

    public DragGestureData(EditingContext context, ModelItem sourceModel, ModelItem targetModel, Visual coordinateReference, Point startPosition, Point currentPosition, DragDropEffects allowedEffects, IDataObject data, DependencyObject sourceAdorner, DependencyObject targetAdorner)
      : base(context, sourceModel, targetModel, coordinateReference, startPosition, currentPosition, sourceAdorner, targetAdorner)
    {
      if (data == null)
        throw new ArgumentNullException("data");
      this._allowedEffects = allowedEffects;
      this._data = data;
      this._effects = DragDropEffects.None;
    }

    public static DragGestureData FromEventArgs(ExecutedToolEventArgs e)
    {
      if (e == null)
        throw new ArgumentNullException("e");
      return GestureData.FromParameter<DragGestureData>(e.Parameter);
    }

    public static DragGestureData FromEventArgs(CanExecuteToolEventArgs e)
    {
      if (e == null)
        throw new ArgumentNullException("e");
      return GestureData.FromParameter<DragGestureData>(e.Parameter);
    }
  }
}
