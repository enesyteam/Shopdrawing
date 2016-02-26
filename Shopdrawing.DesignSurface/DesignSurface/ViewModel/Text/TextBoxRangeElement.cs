// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.Text.TextBoxRangeElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.Tools.Text;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Expression.DesignSurface.ViewModel.Text
{
  internal class TextBoxRangeElement : TextRangeElement
  {
    public static readonly IPropertyId FontFamilyProperty = (IPropertyId) PlatformTypes.TextBox.GetMember(MemberType.LocalProperty, "FontFamily", MemberAccessTypes.Public);
    public static readonly IPropertyId FontSizeProperty = (IPropertyId) PlatformTypes.TextBox.GetMember(MemberType.LocalProperty, "FontSize", MemberAccessTypes.Public);
    public static readonly IPropertyId FontWeightProperty = (IPropertyId) PlatformTypes.TextBox.GetMember(MemberType.LocalProperty, "FontWeight", MemberAccessTypes.Public);
    public static readonly IPropertyId FontStyleProperty = (IPropertyId) PlatformTypes.TextBox.GetMember(MemberType.LocalProperty, "FontStyle", MemberAccessTypes.Public);
    public static readonly IPropertyId TextAlignmentProperty = (IPropertyId) PlatformTypes.TextBox.GetMember(MemberType.LocalProperty, "TextAlignment", MemberAccessTypes.Public);
    public static readonly IPropertyId TextDecorationsProperty = (IPropertyId) PlatformTypes.TextBox.GetMember(MemberType.LocalProperty, "TextDecorations", MemberAccessTypes.Public);
    public static IPropertyId[] TextRangeProperties = new IPropertyId[6]
    {
      TextBoxRangeElement.FontFamilyProperty,
      TextBoxRangeElement.FontSizeProperty,
      TextBoxRangeElement.FontWeightProperty,
      TextBoxRangeElement.FontStyleProperty,
      TextBoxRangeElement.TextDecorationsProperty,
      TextBoxRangeElement.TextAlignmentProperty
    };
    public static readonly TextBoxRangeElement.ConcreteTextBoxRangeElementFactory Factory = new TextBoxRangeElement.ConcreteTextBoxRangeElementFactory();

    public override IEnumerable<IPropertyId> RangeProperties
    {
      get
      {
        return (IEnumerable<IPropertyId>) TextBoxRangeElement.TextRangeProperties;
      }
    }

    public override void ClearValue(PropertyReference propertyReference)
    {
      this.TextEditProxy.TextSource.ClearValue(propertyReference);
    }

    public override void InsertValue(PropertyReference propertyReference, int index, object valueToAdd)
    {
      this.TextEditProxy.TextSource.InsertValue(propertyReference, index, valueToAdd);
    }

    public override void SetValue(PropertyReference propertyReference, object valueToSet)
    {
      this.TextEditProxy.TextSource.SetValue(propertyReference, valueToSet);
    }

    public override void RemoveValueAt(PropertyReference propertyReference, int index)
    {
      this.TextEditProxy.TextSource.RemoveValueAt(propertyReference, index);
    }

    public override PropertyState IsSet(PropertyReference propertyReference)
    {
      return this.TextEditProxy.TextSource.IsSet(propertyReference);
    }

    protected override object GetComputedValueInternal(PropertyReference propertyReference)
    {
      return this.TextEditProxy.TextSource.GetComputedValue(propertyReference);
    }

    public override ReadOnlyCollection<IProperty> GetProperties()
    {
      return this.TextEditProxy.TextSource.GetProperties();
    }

    public class ConcreteTextBoxRangeElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new TextBoxRangeElement();
      }

      public TextBoxRangeElement Instantiate(TextEditProxy textEditProxy, SceneViewModel viewModel, ITypeId targetType)
      {
        TextBoxRangeElement textBoxRangeElement = (TextBoxRangeElement) this.Instantiate(viewModel, targetType);
        textBoxRangeElement.TextEditProxy = textEditProxy;
        return textBoxRangeElement;
      }
    }
  }
}
