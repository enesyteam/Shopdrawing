// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.Text.ControlTextRangeElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Tools.Text;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.ViewModel.Text
{
  internal class ControlTextRangeElement : TextBoxRangeElement
  {
    public static IPropertyId[] TextRangeControlProperties = new IPropertyId[4]
    {
      TextBoxRangeElement.FontFamilyProperty,
      TextBoxRangeElement.FontSizeProperty,
      TextBoxRangeElement.FontWeightProperty,
      TextBoxRangeElement.FontStyleProperty
    };
    public static readonly ControlTextRangeElement.ConcreteControlTextRangeElementFactory Factory = new ControlTextRangeElement.ConcreteControlTextRangeElementFactory();

    public override IEnumerable<IPropertyId> RangeProperties
    {
      get
      {
        return (IEnumerable<IPropertyId>) ControlTextRangeElement.TextRangeControlProperties;
      }
    }

    public class ConcreteControlTextRangeElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new ControlTextRangeElement();
      }

      public TextBoxRangeElement Instantiate(TextEditProxy textEditProxy, SceneViewModel viewModel, ITypeId targetType)
      {
        ControlTextRangeElement textRangeElement = (ControlTextRangeElement) this.Instantiate(viewModel, targetType);
        textRangeElement.TextEditProxy = textEditProxy;
        return (TextBoxRangeElement) textRangeElement;
      }
    }
  }
}
