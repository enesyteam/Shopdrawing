// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.Text.RichTextBoxRangeElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignModel.ViewObjects.TextObjects;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.Tools.Text;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.ViewModel.Text
{
  internal class RichTextBoxRangeElement : TextRangeElement
  {
    public static readonly IPropertyId LineHeightPropertyId = (IPropertyId) PlatformTypes.Block.GetMember(MemberType.LocalProperty, "LineHeight", MemberAccessTypes.Public);
    public static readonly IPropertyId TextAlignmentPropertyId = (IPropertyId) PlatformTypes.Block.GetMember(MemberType.LocalProperty, "TextAlignment", MemberAccessTypes.Public);
    public static readonly IPropertyId PaddingPropertyId = (IPropertyId) PlatformTypes.Block.GetMember(MemberType.LocalProperty, "Padding", MemberAccessTypes.Public);
    public static readonly IPropertyId TextDecorationsPropertyId = (IPropertyId) PlatformTypes.Paragraph.GetMember(MemberType.LocalProperty, "TextDecorations", MemberAccessTypes.Public);
    public static readonly IPropertyId TextIndentPropertyId = (IPropertyId) PlatformTypes.Paragraph.GetMember(MemberType.LocalProperty, "TextIndent", MemberAccessTypes.Public);
    private static readonly IPropertyId TextElementForegroundProperty = (IPropertyId) PlatformTypes.TextElement.GetMember(MemberType.LocalProperty, "Foreground", MemberAccessTypes.Public);
    private static readonly IPropertyId TextElementBackgroundProperty = (IPropertyId) PlatformTypes.TextElement.GetMember(MemberType.LocalProperty, "Background", MemberAccessTypes.Public);
    public static readonly IPropertyId ForegroundProperty = (IPropertyId) PlatformTypes.Inline.GetMember(MemberType.LocalProperty, "Foreground", MemberAccessTypes.Public);
    public static readonly IPropertyId BackgroundProperty = (IPropertyId) PlatformTypes.Inline.GetMember(MemberType.LocalProperty, "Background", MemberAccessTypes.Public);
    public static readonly IPropertyId FontFamilyProperty = (IPropertyId) PlatformTypes.Inline.GetMember(MemberType.LocalProperty, "FontFamily", MemberAccessTypes.Public);
    public static readonly IPropertyId FontSizeProperty = (IPropertyId) PlatformTypes.Inline.GetMember(MemberType.LocalProperty, "FontSize", MemberAccessTypes.Public);
    public static readonly IPropertyId FontWeightProperty = (IPropertyId) PlatformTypes.Inline.GetMember(MemberType.LocalProperty, "FontWeight", MemberAccessTypes.Public);
    public static readonly IPropertyId FontStyleProperty = (IPropertyId) PlatformTypes.Inline.GetMember(MemberType.LocalProperty, "FontStyle", MemberAccessTypes.Public);
    public static readonly IPropertyId FontStretchProperty = (IPropertyId) PlatformTypes.Inline.GetMember(MemberType.LocalProperty, "FontStretch", MemberAccessTypes.Public);
    public static readonly IPropertyId TextDecorationsProperty = (IPropertyId) PlatformTypes.Inline.GetMember(MemberType.LocalProperty, "TextDecorations", MemberAccessTypes.Public);
    public static readonly IPropertyId TextBlockLineHeightPropertyId = (IPropertyId) PlatformTypes.TextBlock.GetMember(MemberType.LocalProperty, "LineHeight", MemberAccessTypes.Public);
    public static readonly IPropertyId TextBlockTextAlignmentPropertyId = (IPropertyId) PlatformTypes.TextBlock.GetMember(MemberType.LocalProperty, "TextAlignment", MemberAccessTypes.Public);
    public static readonly IPropertyId AttachedTextBlockTextAlignmentPropertyId = (IPropertyId) PlatformTypes.TextBlock.GetMember(MemberType.AttachedProperty, "TextAlignment", MemberAccessTypes.Public);
    private static IPropertyId[] WpfTextRangeProperties = new IPropertyId[10]
    {
      RichTextBoxRangeElement.FontFamilyProperty,
      RichTextBoxRangeElement.FontSizeProperty,
      RichTextBoxRangeElement.ForegroundProperty,
      RichTextBoxRangeElement.BackgroundProperty,
      RichTextBoxRangeElement.FontWeightProperty,
      RichTextBoxRangeElement.FontStyleProperty,
      RichTextBoxRangeElement.FontStretchProperty,
      RichTextBoxRangeElement.TextDecorationsProperty,
      RichTextBoxRangeElement.TextBlockLineHeightPropertyId,
      RichTextBoxRangeElement.TextBlockTextAlignmentPropertyId
    };
    private static IPropertyId[] SilverlightTextRangeProperties = new IPropertyId[9]
    {
      RichTextBoxRangeElement.FontFamilyProperty,
      RichTextBoxRangeElement.FontSizeProperty,
      RichTextBoxRangeElement.ForegroundProperty,
      RichTextBoxRangeElement.BackgroundProperty,
      RichTextBoxRangeElement.FontWeightProperty,
      RichTextBoxRangeElement.FontStyleProperty,
      RichTextBoxRangeElement.FontStretchProperty,
      RichTextBoxRangeElement.TextDecorationsProperty,
      RichTextBoxRangeElement.TextBlockLineHeightPropertyId
    };
    public static readonly RichTextBoxRangeElement.ConcreteRichTextBoxRangeElementFactory Factory = new RichTextBoxRangeElement.ConcreteRichTextBoxRangeElementFactory();
    private ReadOnlyCollection<IProperty> textRangeElementPropertySet;
    private IViewTextRange textRange;

    public IEnumerable<IPropertyId> TextRangeProperties
    {
      get
      {
        return RichTextBoxRangeElement.GetTextRangeProperties(this.ProjectContext);
      }
    }

    public RichTextBoxEditProxyBase RichTextEditProxy
    {
      get
      {
        return (RichTextBoxEditProxyBase) this.TextEditProxy;
      }
    }

    protected bool IsEntireRangeSelected
    {
      get
      {
        IViewRichTextBox viewRichTextBox = this.TextEditProxy.EditingElement as IViewRichTextBox;
        if (viewRichTextBox != null && this.TextRange.Contains(viewRichTextBox.BlockContainer.ContentStart))
          return this.TextRange.Contains(viewRichTextBox.BlockContainer.ContentEnd);
        return false;
      }
    }

    public override IEnumerable<IPropertyId> RangeProperties
    {
      get
      {
        return this.TextRangeProperties;
      }
    }

    public override IViewObject ViewObject
    {
      get
      {
        return (IViewObject) this.TextRange;
      }
    }

    protected IViewTextRange TextRange
    {
      get
      {
        if (this.textRange != null)
          return this.textRange;
        return (IViewTextRange) ((IViewRichTextBox) this.TextEditProxy.EditingElement).Selection;
      }
    }

    private static IEnumerable<IPropertyId> GetTextRangeProperties(IProjectContext projectContext)
    {
      if (projectContext.IsCapabilitySet(PlatformCapability.IsWpf))
        return (IEnumerable<IPropertyId>) RichTextBoxRangeElement.WpfTextRangeProperties;
      IPropertyId[] propertyIdArray = new IPropertyId[1]
      {
        projectContext.IsCapabilitySet(PlatformCapability.UsesTextBlockAlignmentInTextEditMode) ? RichTextBoxRangeElement.TextBlockTextAlignmentPropertyId : RichTextBoxRangeElement.TextAlignmentPropertyId
      };
      return Enumerable.Concat<IPropertyId>((IEnumerable<IPropertyId>) RichTextBoxRangeElement.SilverlightTextRangeProperties, (IEnumerable<IPropertyId>) propertyIdArray);
    }

    public static bool IsTextProperty(SceneNode targetNode, PropertyReference propertyReference)
    {
      bool flag = false;
      foreach (IPropertyId propertyId in RichTextBoxRangeElement.GetTextRangeProperties(targetNode.ProjectContext))
      {
        ReferenceStep referenceStep1 = (ReferenceStep) targetNode.Platform.Metadata.ResolveProperty(propertyId);
        if (referenceStep1 != null)
        {
          ReferenceStep referenceStep2 = targetNode.DesignerContext.PropertyManager.FilterProperty(targetNode, referenceStep1);
          if (propertyReference.FirstStep.Equals((object) referenceStep2))
            flag = true;
        }
      }
      return flag;
    }

    public static bool ShouldClearPropertyOnTextRuns(SceneNode targetNode, PropertyReference propertyReference)
    {
      ReferenceStep referenceStep1 = (ReferenceStep) targetNode.Platform.Metadata.ResolveProperty(RichTextBoxRangeElement.BackgroundProperty);
      bool flag = false;
      if (referenceStep1 != null)
      {
        ReferenceStep referenceStep2 = targetNode.DesignerContext.PropertyManager.FilterProperty(targetNode, referenceStep1);
        if (referenceStep2 != null)
          flag = propertyReference.FirstStep.Equals((object) referenceStep2);
      }
      if (!flag)
        return RichTextBoxRangeElement.IsTextProperty(targetNode, propertyReference);
      return false;
    }

    protected virtual bool ShouldForwardPropertyChangeToControl(PropertyReference propertyReference)
    {
      return this.IsEntireRangeSelected && !propertyReference[0].Equals((object) this.Platform.Metadata.ResolveProperty(RichTextBoxRangeElement.BackgroundProperty)) || (propertyReference[0].Equals((object) this.Platform.Metadata.ResolveProperty(RichTextBoxRangeElement.LineHeightPropertyId)) || propertyReference[0].Equals((object) this.Platform.Metadata.ResolveProperty(RichTextBoxRangeElement.TextBlockLineHeightPropertyId))) || (propertyReference[0].Equals((object) this.Platform.Metadata.ResolveProperty(RichTextBoxRangeElement.TextAlignmentPropertyId)) || propertyReference[0].Equals((object) this.Platform.Metadata.ResolveProperty(RichTextBoxRangeElement.TextBlockTextAlignmentPropertyId)) || propertyReference[0].Equals((object) this.Platform.Metadata.ResolveProperty(RichTextBoxRangeElement.AttachedTextBlockTextAlignmentPropertyId)));
    }

    protected virtual bool IsTextRangeProperty(PropertyReference propertyReference)
    {
      bool flag1 = propertyReference[0].Equals((object) this.Platform.Metadata.ResolveProperty(RichTextBoxRangeElement.LineHeightPropertyId)) || propertyReference[0].Equals((object) this.Platform.Metadata.ResolveProperty(RichTextBoxRangeElement.TextBlockLineHeightPropertyId));
      bool flag2 = propertyReference[0].Equals((object) this.Platform.Metadata.ResolveProperty(RichTextBoxRangeElement.TextAlignmentPropertyId)) || propertyReference[0].Equals((object) this.Platform.Metadata.ResolveProperty(RichTextBoxRangeElement.TextBlockTextAlignmentPropertyId)) || propertyReference[0].Equals((object) this.Platform.Metadata.ResolveProperty(RichTextBoxRangeElement.AttachedTextBlockTextAlignmentPropertyId));
      if (!this.ContainsProperty(this.TextRangeProperties, propertyReference[0]) && !flag2)
        return flag1;
      return true;
    }

    protected bool ContainsProperty(IEnumerable<IPropertyId> properties, ReferenceStep targetProperty)
    {
      foreach (IPropertyId propertyId in properties)
      {
        if (targetProperty.Equals((object) this.Platform.Metadata.ResolveProperty(propertyId)))
          return true;
      }
      return false;
    }

    private PropertyReference ConvertToPlatformPropertyReference(PropertyReference propertyReference)
    {
      IPlatformMetadata destinationPlatformMetadata = (IPlatformMetadata) this.TextRange.Platform.Metadata;
      return this.DesignerContext.PlatformConverter.ConvertFromWpfPropertyReference(this.ConvertToWpfPropertyReference(propertyReference), destinationPlatformMetadata);
    }

    public override void ClearValue(PropertyReference propertyReference)
    {
      if (this.IsTextRangeProperty(propertyReference))
      {
        if (!this.ShouldForwardPropertyChangeToControl(propertyReference))
        {
          this.ClearTextValue(this.TextRange, this.ConvertToPlatformPropertyReference(propertyReference));
          return;
        }
        this.RichTextEditProxy.ClearTextProperty((IProperty) propertyReference[0]);
        propertyReference = this.DesignerContext.PropertyManager.FilterProperty((SceneNode) this.TextEditProxy.TextSource, propertyReference);
      }
      this.TextEditProxy.TextSource.ClearValue(propertyReference);
    }

    public override void InsertValue(PropertyReference propertyReference, int index, object valueToAdd)
    {
      if (this.IsTextRangeProperty(propertyReference))
      {
        if (!this.ShouldForwardPropertyChangeToControl(propertyReference))
        {
          this.InsertTextValue(this.TextRange, this.ConvertToPlatformPropertyReference(propertyReference), index, this.ConvertToWpfValue(valueToAdd));
          return;
        }
        this.RichTextEditProxy.ClearTextProperty((IProperty) propertyReference[0]);
        propertyReference = this.DesignerContext.PropertyManager.FilterProperty((SceneNode) this.TextEditProxy.TextSource, propertyReference);
      }
      this.TextEditProxy.TextSource.InsertValue(propertyReference, index, valueToAdd);
    }

    public override void SetValue(PropertyReference propertyReference, object valueToSet)
    {
      if (this.IsTextRangeProperty(propertyReference))
      {
        if (!this.ShouldForwardPropertyChangeToControl(propertyReference))
        {
          this.SetTextValue(this.TextRange, this.ConvertToPlatformPropertyReference(propertyReference), this.ConvertToWpfValue(valueToSet));
          return;
        }
        this.RichTextEditProxy.ClearTextProperty((IProperty) propertyReference[0]);
        propertyReference = this.DesignerContext.PropertyManager.FilterProperty((SceneNode) this.TextEditProxy.TextSource, propertyReference);
      }
      this.TextEditProxy.TextSource.SetValue(propertyReference, valueToSet);
    }

    public override void RemoveValueAt(PropertyReference propertyReference, int index)
    {
      if (this.IsTextRangeProperty(propertyReference))
      {
        if (!this.ShouldForwardPropertyChangeToControl(propertyReference))
        {
          this.RemoveTextValueAt(this.TextRange, this.ConvertToPlatformPropertyReference(propertyReference), index);
          return;
        }
        this.RichTextEditProxy.ClearTextProperty((IProperty) propertyReference[0]);
        propertyReference = this.DesignerContext.PropertyManager.FilterProperty((SceneNode) this.TextEditProxy.TextSource, propertyReference);
      }
      this.TextEditProxy.TextSource.RemoveValueAt(propertyReference, index);
    }

    public override PropertyState IsSet(PropertyReference propertyReference)
    {
      if (this.IsTextRangeProperty(propertyReference))
      {
        if (!this.ShouldForwardPropertyChangeToControl(propertyReference))
          return this.IsValueSetOnTextRange(this.TextRange, this.ConvertToPlatformPropertyReference(propertyReference));
        propertyReference = this.DesignerContext.PropertyManager.FilterProperty((SceneNode) this.TextEditProxy.TextSource, propertyReference);
      }
      return this.TextEditProxy.TextSource.IsSet(propertyReference);
    }

    protected override object GetComputedValueInternal(PropertyReference propertyReference)
    {
      if (this.IsTextRangeProperty(propertyReference))
      {
        if (!this.ShouldForwardPropertyChangeToControl(propertyReference))
        {
          PropertyReference propertyReference1 = this.ConvertToPlatformPropertyReference(propertyReference);
          object textValue = this.GetTextValue(this.TextRange, propertyReference1);
          object obj = textValue;
          if (textValue != MixedProperty.Mixed)
          {
            PropertyReference propertyReference2 = DesignTimeProperties.GetShadowPropertyReference(propertyReference1, (ITypeId) null);
            if (propertyReference2 != null)
            {
              IViewObject parent = this.TextRange.Start.Parent;
              obj = DesignTimeProperties.UseShadowPropertyForInstanceBuilding(this.DocumentContext.TypeResolver, (IPropertyId) propertyReference[0]) ? parent.GetCurrentValue(propertyReference2) : SceneNode.GetComputedValueWithShadowCoercion(propertyReference1, propertyReference2, parent.PlatformSpecificObject);
            }
          }
          return this.ConvertFromWpfValue(obj);
        }
        propertyReference = this.DesignerContext.PropertyManager.FilterProperty((SceneNode) this.TextEditProxy.TextSource, propertyReference);
      }
      if (propertyReference != null)
        return this.TextEditProxy.TextSource.GetComputedValue(propertyReference);
      return (object) null;
    }

    public object GetTextValue(IViewTextRange textRange, PropertyReference propertyReference)
    {
      object textRangeValue = RichTextBoxRangeElement.GetTextRangeValue(textRange, propertyReference);
      if (DependencyProperty.UnsetValue == textRangeValue)
        return MixedProperty.Mixed;
      return textRangeValue;
    }

    public PropertyState IsValueSetOnTextRange(IViewTextRange textRange, PropertyReference propertyReference)
    {
      return RichTextBoxRangeElement.GetTextRangeValue(textRange, propertyReference) == null ? PropertyState.Unset : PropertyState.Set;
    }

    public static object GetTextPointerValue(IViewTextPointer textPointer, PropertyReference propertyReference)
    {
      return textPointer.Parent.GetValue(propertyReference);
    }

    public void SetTextValue(IViewTextRange textRange, PropertyReference propertyReference, object value)
    {
      this.ModifyTextValue(textRange, propertyReference, -1, value, SceneNode.Modification.SetValue);
    }

    public void InsertTextValue(IViewTextRange textRange, PropertyReference propertyReference, int index, object value)
    {
      this.ModifyTextValue(textRange, propertyReference, index, value, SceneNode.Modification.InsertValue);
    }

    public void RemoveTextValueAt(IViewTextRange textRange, PropertyReference propertyReference, int index)
    {
      this.ModifyTextValue(textRange, propertyReference, index, (object) null, SceneNode.Modification.RemoveValue);
    }

    public void ClearTextValue(IViewTextRange textRange, PropertyReference propertyReference)
    {
      this.ModifyTextValue(textRange, propertyReference, -1, (object) null, SceneNode.Modification.ClearValue);
    }

    private void ModifyTextValue(IViewTextRange textRange, PropertyReference propertyReference, int index, object value, SceneNode.Modification modification)
    {
      if (value is Microsoft.Expression.DesignModel.DocumentModel.DocumentNode || value is MarkupExtension)
        return;
      PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.TextSetProperty);
      value = RichTextBoxRangeElement.GetUpdatedComplexProperty(textRange, propertyReference, index, value, modification);
      ReferenceStep firstStep = propertyReference.FirstStep;
      IProperty property = textRange.Platform.Metadata.ResolveProperty((IPropertyId) firstStep);
      if (property != null)
      {
        if (modification == SceneNode.Modification.ClearValue)
          value = this.ViewModel.DefaultView.ConvertToWpfValue(this.ViewModel.DefaultView.ConvertFromWpfPropertyReference(propertyReference).LastStep.GetDefaultValue(typeof (object)));
        if (property.Equals((object) RichTextBoxRangeElement.TextIndentPropertyId) && (double) value < 0.0)
          value = (object) 0.0;
        textRange.ApplyPropertyValue((IPropertyId) property, value);
        bool flag = RichTextBoxElement.IsParagraphProperty(propertyReference);
        PropertyReference propertyReference1 = DesignTimeProperties.GetShadowPropertyReference(propertyReference, (ITypeId) this.Type);
        if (propertyReference1 != null && !DesignTimeProperties.UseShadowPropertyForInstanceBuilding(this.ViewModel.Document.DocumentContext.TypeResolver, (IPropertyId) propertyReference1.FirstStep))
          propertyReference1 = (PropertyReference) null;
        if (modification == SceneNode.Modification.ClearValue && propertyReference1 == null)
          propertyReference1 = propertyReference;
        if (propertyReference1 != null)
        {
          IViewTextElement viewTextElement1 = (IViewTextElement) null;
          if (textRange.Start.CompareTo(textRange.End) != 0 || textRange.Start.CompareTo(((IViewTextElement) textRange.Start.Parent).ContentStart) != 0 && textRange.Start.CompareTo(((IViewTextElement) textRange.Start.Parent).ContentEnd) != 0)
          {
            for (IViewTextPointer viewTextPointer = textRange.Start; viewTextPointer != null && viewTextPointer.CompareTo(textRange.End) <= 0; viewTextPointer = viewTextPointer.GetNextInsertionPosition(LogicalDirection.Forward))
            {
              IViewTextElement viewTextElement2 = viewTextPointer.Parent as IViewTextElement;
              if (viewTextElement2 != null && (viewTextElement1 == null || viewTextElement1.PlatformSpecificObject != viewTextElement2.PlatformSpecificObject))
              {
                viewTextElement1 = viewTextElement2;
                if (viewTextElement1 != null && (flag && viewTextElement1 is Paragraph || !flag && viewTextElement1 is Run))
                {
                  if (modification == SceneNode.Modification.ClearValue)
                    propertyReference1.ClearValue((object) viewTextElement1);
                  else
                    propertyReference1.SetValue((object) viewTextElement1, value);
                }
              }
            }
          }
        }
      }
      if (property.Equals((object) RichTextBoxRangeElement.ForegroundProperty) || property.Equals((object) RichTextBoxRangeElement.TextElementForegroundProperty))
      {
        TextBlockEditProxy textBlockEditProxy = this.TextEditProxy as TextBlockEditProxy;
        if (textBlockEditProxy != null)
          textBlockEditProxy.UpdateCaretBrush();
      }
      if (this.TextEditProxy == null)
        return;
      this.TextEditProxy.Serialize();
      this.TextEditProxy.UpdateDocumentModel();
    }

    private static PropertyReference CreateSubReference(PropertyReference propertyReference)
    {
      List<ReferenceStep> steps = new List<ReferenceStep>(propertyReference.Count);
      for (int index = 1; index < propertyReference.Count; ++index)
        steps.Add(propertyReference[index]);
      return new PropertyReference(steps);
    }

    private static object GetTextRangeValue(IViewTextRange textRange, PropertyReference propertyReference)
    {
      object target = (object) null;
      ReferenceStep firstStep = propertyReference.FirstStep;
      IProperty property = textRange.Platform.Metadata.ResolveProperty((IPropertyId) firstStep);
      if (property != null)
      {
        target = textRange.GetPropertyValue((IPropertyId) property, true);
        if (target == DependencyProperty.UnsetValue || propertyReference.Count <= 1)
          return target;
        target = RichTextBoxRangeElement.CreateSubReference(propertyReference).GetValue(target);
      }
      return target;
    }

    private static bool IsComplexProperty(IPlatformMetadata platformMetadata, IProperty property)
    {
      return property.Equals((object) RichTextBoxRangeElement.TextElementForegroundProperty) || property.Equals((object) RichTextBoxRangeElement.TextElementBackgroundProperty) || property.Equals((object) RichTextBoxRangeElement.PaddingPropertyId) || !platformMetadata.IsCapabilitySet(PlatformCapability.SupportsTextElementProperties) && (property.Equals((object) RichTextBoxRangeElement.ForegroundProperty) || property.Equals((object) RichTextBoxRangeElement.BackgroundProperty));
    }

    private static object GetUpdatedComplexProperty(IViewTextRange target, PropertyReference propertyReference, int index, object value, SceneNode.Modification modification)
    {
      ReferenceStep firstStep = propertyReference.FirstStep;
      IProperty property = target.Platform.Metadata.ResolveProperty((IPropertyId) firstStep);
      if (property != null && RichTextBoxRangeElement.IsComplexProperty((IPlatformMetadata) target.Platform.Metadata, property) && propertyReference.Count > 1)
      {
        PropertyReference subReference = RichTextBoxRangeElement.CreateSubReference(propertyReference);
        object target1;
        if (target.IsEmpty)
        {
          target1 = target.GetPropertyValue((IPropertyId) property);
        }
        else
        {
          IViewTextPointer viewTextPointer = target.Start;
          if (!viewTextPointer.IsAtInsertionPosition)
            viewTextPointer = viewTextPointer.GetNextInsertionPosition(LogicalDirection.Forward);
          target1 = !RichTextBoxElement.IsParagraphProperty(propertyReference) ? viewTextPointer.Parent.GetValue(property) : viewTextPointer.Paragraph.GetValue(property);
        }
        Freezable freezable = target1 as Freezable;
        if (freezable != null)
          target1 = (object) freezable.Clone();
        if (target1 != null)
        {
          switch (modification)
          {
            case SceneNode.Modification.SetValue:
              target1 = subReference.SetValue(target1, value);
              break;
            case SceneNode.Modification.InsertValue:
              subReference.Insert(target1, index, value);
              break;
            case SceneNode.Modification.RemoveValue:
              subReference.RemoveAt(target1, index);
              break;
            default:
              throw new InvalidEnumArgumentException("modification", (int) modification, typeof (SceneNode.Modification));
          }
        }
        value = target1;
      }
      return value;
    }

    public override ReadOnlyCollection<IProperty> GetProperties()
    {
      if (this.textRangeElementPropertySet == null)
      {
        List<IProperty> list = new List<IProperty>();
        foreach (IProperty property in this.TextEditProxy.TextSource.GetProperties())
          list.Add(property);
        foreach (ReferenceStep referenceStep in this.TextRangeProperties)
        {
          if (!list.Contains((IProperty) referenceStep))
            list.Add((IProperty) referenceStep);
        }
        this.textRangeElementPropertySet = new ReadOnlyCollection<IProperty>((IList<IProperty>) list);
      }
      return this.textRangeElementPropertySet;
    }

    private void SetTextRange(IViewTextRange textRange)
    {
      this.textRange = textRange;
    }

    public class ConcreteRichTextBoxRangeElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new RichTextBoxRangeElement();
      }

      public RichTextBoxRangeElement Instantiate(IViewTextRange textRange, SceneViewModel viewModel, ITypeId targetType)
      {
        RichTextBoxRangeElement textBoxRangeElement = (RichTextBoxRangeElement) this.Instantiate(viewModel, targetType);
        textBoxRangeElement.SetTextRange(textRange);
        return textBoxRangeElement;
      }

      public RichTextBoxRangeElement Instantiate(TextEditProxy textEditProxy, SceneViewModel viewModel, ITypeId targetType)
      {
        RichTextBoxRangeElement textBoxRangeElement = (RichTextBoxRangeElement) this.Instantiate(viewModel, targetType);
        textBoxRangeElement.TextEditProxy = textEditProxy;
        return textBoxRangeElement;
      }
    }
  }
}
