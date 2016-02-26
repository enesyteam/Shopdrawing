// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.GestureData
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Model;
using MS.Internal.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;

namespace Microsoft.Windows.Design.Interaction
{
  public class GestureData
  {
    private EditingContext _context;
    private ModelItem _sourceModel;
    private ModelItem _targetModel;
    private ModelItem _impliedSource;
    private ModelItem _impliedTarget;
    private DependencyObject _sourceAdorner;
    private DependencyObject _targetAdorner;
    private Task _sourceTask;

    public EditingContext Context
    {
      get
      {
        return this._context;
      }
    }

    public ModelItem ImpliedSource
    {
      get
      {
        if (this._impliedSource == null)
          this._impliedSource = GestureData.GetImpliedModel(this._sourceAdorner, this._sourceModel);
        return this._impliedSource;
      }
    }

    public ModelItem ImpliedTarget
    {
      get
      {
        if (this._impliedTarget == null)
          this._impliedTarget = GestureData.GetImpliedModel(this._targetAdorner, this._targetModel);
        return this._impliedTarget;
      }
    }

    public DependencyObject SourceAdorner
    {
      get
      {
        return this._sourceAdorner;
      }
    }

    public ModelItem SourceModel
    {
      get
      {
        return this._sourceModel;
      }
    }

    internal Task SourceTask
    {
      get
      {
        return this._sourceTask;
      }
      set
      {
        this._sourceTask = value;
      }
    }

    public DependencyObject TargetAdorner
    {
      get
      {
        return this._targetAdorner;
      }
    }

    public ModelItem TargetModel
    {
      get
      {
        return this._targetModel;
      }
    }

    public ICollection<UIElement> Adorners
    {
      get
      {
        DesignerView designerView = DesignerView.FromContext(this.Context);
        if (designerView == null)
          throw new NotSupportedException(Resources.Error_NoDesignerView);
        return designerView.Adorners;
      }
    }

    public GestureData(EditingContext context, ModelItem sourceModel, ModelItem targetModel)
      : this(context, sourceModel, targetModel, (DependencyObject) null, (DependencyObject) null)
    {
    }

    public GestureData(EditingContext context, ModelItem sourceModel, ModelItem targetModel, DependencyObject sourceAdorner, DependencyObject targetAdorner)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      if (sourceModel == null)
        throw new ArgumentNullException("sourceModel");
      if (targetModel == null)
        throw new ArgumentNullException("targetModel");
      this._context = context;
      this._sourceModel = sourceModel;
      this._targetModel = targetModel;
      this._sourceAdorner = sourceAdorner;
      this._targetAdorner = targetAdorner;
    }

    public static GestureData FromEventArgs(ExecutedToolEventArgs e)
    {
      if (e == null)
        throw new ArgumentNullException("e");
      return GestureData.FromParameter<GestureData>(e.Parameter);
    }

    public static GestureData FromEventArgs(CanExecuteToolEventArgs e)
    {
      if (e == null)
        throw new ArgumentNullException("e");
      return GestureData.FromParameter<GestureData>(e.Parameter);
    }

    internal static GestureDataType FromParameter<GestureDataType>(object parameter) where GestureDataType : GestureData
    {
      GestureDataType gestureDataType = parameter as GestureDataType;
      if ((object) gestureDataType == null)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_NoGestureData, new object[1]
        {
          (object) typeof (GestureDataType).Name
        }));
      return gestureDataType;
    }

    private static ModelItem GetImpliedModel(DependencyObject adorner, ModelItem model)
    {
      ModelItem modelItem = (ModelItem) null;
      if (adorner != null)
        modelItem = AdornerProperties.GetModel(adorner);
      if (modelItem == null)
        modelItem = model;
      return modelItem;
    }
  }
}
