// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.MouseGestureData
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Model;
using MS.Internal.Properties;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Windows.Design.Interaction
{
  public class MouseGestureData : GestureData
  {
    private Visual _coordinateReference;
    private Point _currentPosition;
    private Point _startPosition;

    public Point CurrentPosition
    {
      get
      {
        return this._currentPosition;
      }
    }

    public Point StartPosition
    {
      get
      {
        return this._startPosition;
      }
    }

    public Vector PositionDelta
    {
      get
      {
        return new Vector(this._currentPosition.X - this._startPosition.X, this._currentPosition.Y - this._startPosition.Y);
      }
    }

    public MouseGestureData(EditingContext context, ModelItem sourceModel, ModelItem targetModel, Visual coordinateReference, Point startPosition, Point currentPosition)
      : this(context, sourceModel, targetModel, coordinateReference, startPosition, currentPosition, (DependencyObject) null, (DependencyObject) null)
    {
    }

    public MouseGestureData(EditingContext context, ModelItem sourceModel, ModelItem targetModel, Visual coordinateReference, Point startPosition, Point currentPosition, DependencyObject sourceAdorner, DependencyObject targetAdorner)
      : base(context, sourceModel, targetModel, sourceAdorner, targetAdorner)
    {
      if (coordinateReference == null)
        throw new ArgumentNullException("coordinateReference");
      this._coordinateReference = coordinateReference;
      this._startPosition = startPosition;
      this._currentPosition = currentPosition;
    }

    public Point TranslatePoint(Point pt, ModelItem referenceTo)
    {
      if (referenceTo == null)
        throw new ArgumentNullException("referenceTo");
      ViewItem view = referenceTo.View;
      if (view == (ViewItem) null)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_VisualNotInDesigner, new object[1]
        {
          (object) referenceTo
        }));
      try
      {
        GeneralTransform generalTransform = view.TransformFromVisual(this._coordinateReference);
        if (generalTransform != null)
          pt = generalTransform.Transform(pt);
      }
      catch (InvalidOperationException ex)
      {
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_VisualNotInDesigner, new object[1]
        {
          (object) referenceTo
        }));
      }
      return pt;
    }

    public static MouseGestureData FromEventArgs(ExecutedToolEventArgs e)
    {
      if (e == null)
        throw new ArgumentNullException("e");
      return GestureData.FromParameter<MouseGestureData>(e.Parameter);
    }

    public static MouseGestureData FromEventArgs(CanExecuteToolEventArgs e)
    {
      if (e == null)
        throw new ArgumentNullException("e");
      return GestureData.FromParameter<MouseGestureData>(e.Parameter);
    }
  }
}
