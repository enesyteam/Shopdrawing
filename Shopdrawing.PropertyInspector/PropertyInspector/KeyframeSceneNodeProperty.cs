// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.KeyframeSceneNodeProperty
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class KeyframeSceneNodeProperty : SceneNodeProperty
  {
    public override bool IsEnabledClearValue
    {
      get
      {
        if (!(base.get_PropertyName() == "Value"))
          return base.IsEnabledClearValue;
        return false;
      }
    }

    public override bool IsEnabledLocalResource
    {
      get
      {
        if (PlatformTypes.KeySpline.IsAssignableFrom((ITypeId) this.PropertyTypeId))
          return base.IsEnabledLocalResource;
        return false;
      }
    }

    public override bool IsEnabledTemplateBinding
    {
      get
      {
        return false;
      }
    }

    public override bool IsEnabledMakeNewResource
    {
      get
      {
        if (PlatformTypes.KeySpline.IsAssignableFrom((ITypeId) this.PropertyTypeId))
          return base.IsEnabledMakeNewResource;
        return false;
      }
    }

    protected override bool CanSetDynamicExpression
    {
      get
      {
        if (!(base.get_PropertyName() == "Value"))
          return base.CanSetDynamicExpression;
        return true;
      }
    }

    public KeyframeSceneNodeProperty(SceneNodeObjectSet objectSet, PropertyReference propertyReference, AttributeCollection attributes, Type proxyType)
      : base(objectSet, propertyReference, attributes, null, proxyType)
    {
    }

    public override void SetValue(object value)
    {
      bool? nullable = new bool?(false);
      if (this.ValueEditorParameters != null && this.ValueEditorParameters["NumberRanges"] != null)
        nullable = ((NumberRangesAttribute) this.ValueEditorParameters["NumberRanges"]).get_CanBeAuto();
      if (nullable.HasValue && nullable.Value && (value is double && double.IsNaN((double) value)) && !this.SceneNodeObjectSet.ViewModel.AnimationEditor.CanAnimateLayout)
        this.SceneNodeObjectSet.ViewModel.AnimationEditor.PostAutoErrorDialog(AutoDialogType.KeyFrame);
      else
        base.SetValue(value);
    }
  }
}
