// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.ArcShapeElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Properties;
using System;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class ArcShapeElement : PrimitiveShapeElement
  {
    public static readonly IPropertyId ArcThicknessProperty = (IPropertyId) ProjectNeutralTypes.Arc.GetMember(MemberType.LocalProperty, "ArcThickness", MemberAccessTypes.Public);
    public static readonly IPropertyId ArcThicknessUnitProperty = (IPropertyId) ProjectNeutralTypes.Arc.GetMember(MemberType.LocalProperty, "ArcThicknessUnit", MemberAccessTypes.Public);
    public new static readonly SceneNode.ConcreteSceneNodeFactory Factory = (SceneNode.ConcreteSceneNodeFactory) new ArcShapeElement.ArcShapeElementFactory();

    protected override void ModifyValue(PropertyReference propertyReference, object valueToSet, SceneNode.Modification modification, int index)
    {
      if (propertyReference.LastStep.Equals((object) ArcShapeElement.ArcThicknessUnitProperty) && valueToSet != null)
      {
        string str = valueToSet.ToString();
        if (str == "Pixel")
          this.UpdateArcThicknessToPixel();
        else if (str == "Percent")
          this.UpdateArcThicknessToPercent();
      }
      base.ModifyValue(propertyReference, valueToSet, modification, index);
    }

    private void UpdateArcThicknessToPercent()
    {
      if (this.IsSet(ArcShapeElement.ArcThicknessProperty) != PropertyState.Set || this.IsValueExpression(ArcShapeElement.ArcThicknessProperty))
        return;
      Rect computedTightBounds = this.GetComputedTightBounds();
      if (computedTightBounds.Width <= 0.0 || computedTightBounds.Height <= 0.0)
        return;
      double num1 = (double) this.GetLocalValueAsWpf(ArcShapeElement.ArcThicknessProperty);
      double num2 = Math.Min(computedTightBounds.Width, computedTightBounds.Height) / 2.0;
      double num3 = num1 < 0.0 ? 0.0 : (num1 > num2 ? 1.0 : num1 / num2);
      this.SetLocalValue(ArcShapeElement.ArcThicknessProperty, (object) RoundingHelper.RoundLength(num3));
    }

    private void UpdateArcThicknessToPixel()
    {
      if (this.IsSet(ArcShapeElement.ArcThicknessProperty) != PropertyState.Set || this.IsValueExpression(ArcShapeElement.ArcThicknessProperty))
        return;
      Rect computedTightBounds = this.GetComputedTightBounds();
      if (computedTightBounds.Width <= 0.0 || computedTightBounds.Height <= 0.0)
        return;
      double num1 = (double) this.GetLocalValueAsWpf(ArcShapeElement.ArcThicknessProperty);
      double num2 = Math.Min(computedTightBounds.Width, computedTightBounds.Height) / 2.0;
      this.SetLocalValue(ArcShapeElement.ArcThicknessProperty, (object) RoundingHelper.RoundLength(num2 * num1));
    }

    private class ArcShapeElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new ArcShapeElement();
      }
    }
  }
}
