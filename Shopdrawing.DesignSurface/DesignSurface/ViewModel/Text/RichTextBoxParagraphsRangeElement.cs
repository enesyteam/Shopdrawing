// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.Text.RichTextBoxParagraphsRangeElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.ViewModel.Text
{
  internal class RichTextBoxParagraphsRangeElement : RichTextBoxRangeElement
  {
    public static readonly RichTextBoxParagraphsRangeElement.ConcreteRichTextBoxParagraphsRangeElementFactory Factory = new RichTextBoxParagraphsRangeElement.ConcreteRichTextBoxParagraphsRangeElementFactory();
    public static readonly IPropertyId LineHeightProperty = (IPropertyId) PlatformTypes.Block.GetMember(MemberType.LocalProperty, "LineHeight", MemberAccessTypes.Public);
    public static readonly IPropertyId AttachedLineHeightProperty = (IPropertyId) PlatformTypes.Block.GetMember(MemberType.AttachedProperty, "LineHeight", MemberAccessTypes.Public);
    public static readonly IPropertyId TextAlignmentProperty = (IPropertyId) PlatformTypes.Block.GetMember(MemberType.LocalProperty, "TextAlignment", MemberAccessTypes.Public);
    public static readonly IPropertyId AttachedTextAlignmentProperty = (IPropertyId) PlatformTypes.Block.GetMember(MemberType.AttachedProperty, "TextAlignment", MemberAccessTypes.Public);
    public static readonly IPropertyId PaddingProperty = (IPropertyId) PlatformTypes.Block.GetMember(MemberType.LocalProperty, "Padding", MemberAccessTypes.Public);
    public static readonly IPropertyId TextIndentProperty = (IPropertyId) PlatformTypes.Paragraph.GetMember(MemberType.LocalProperty, "TextIndent", MemberAccessTypes.Public);
    public static IPropertyId[] WpfTextParagraphProperties = new IPropertyId[5]
    {
      RichTextBoxParagraphsRangeElement.AttachedLineHeightProperty,
      RichTextBoxParagraphsRangeElement.PaddingProperty,
      RichTextBoxParagraphsRangeElement.TextIndentProperty,
      RichTextBoxParagraphsRangeElement.AttachedTextAlignmentProperty,
      RichTextBoxRangeElement.TextDecorationsProperty
    };
    public static IPropertyId[] SilverlightTextParagraphProperties = new IPropertyId[2]
    {
      RichTextBoxParagraphsRangeElement.TextAlignmentProperty,
      RichTextBoxRangeElement.TextDecorationsProperty
    };
    private IPropertyId[] rangeProperties;

    public IPropertyId[] TextParagraphProperties
    {
      get
      {
        if (this.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf))
          return RichTextBoxParagraphsRangeElement.WpfTextParagraphProperties;
        return RichTextBoxParagraphsRangeElement.SilverlightTextParagraphProperties;
      }
    }

    public override IEnumerable<IPropertyId> RangeProperties
    {
      get
      {
        if (this.rangeProperties == null)
        {
          List<IPropertyId> list = new List<IPropertyId>();
          foreach (IPropertyId propertyId in base.RangeProperties)
          {
            if (!RichTextBoxRangeElement.TextBlockLineHeightPropertyId.Equals((object) propertyId) && !RichTextBoxRangeElement.TextBlockTextAlignmentPropertyId.Equals((object) propertyId) && !list.Contains(propertyId))
              list.Add(propertyId);
          }
          foreach (IPropertyId propertyId in this.TextParagraphProperties)
          {
            if (!list.Contains(propertyId))
              list.Add(propertyId);
          }
          this.rangeProperties = list.ToArray();
        }
        return (IEnumerable<IPropertyId>) this.rangeProperties;
      }
    }

    private bool IsTextParagraphProperty(PropertyReference propertyReference)
    {
      if (this.TextEditProxy.SupportsParagraphProperties)
        return this.ContainsProperty((IEnumerable<IPropertyId>) this.TextParagraphProperties, propertyReference[0]);
      return false;
    }

    protected override bool ShouldForwardPropertyChangeToControl(PropertyReference propertyReference)
    {
      if (this.IsTextParagraphProperty(propertyReference))
        return false;
      return base.ShouldForwardPropertyChangeToControl(propertyReference);
    }

    protected override bool IsTextRangeProperty(PropertyReference propertyReference)
    {
      if (!base.IsTextRangeProperty(propertyReference))
        return this.IsTextParagraphProperty(propertyReference);
      return true;
    }

    public class ConcreteRichTextBoxParagraphsRangeElementFactory : RichTextBoxRangeElement.ConcreteRichTextBoxRangeElementFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new RichTextBoxParagraphsRangeElement();
      }
    }
  }
}
