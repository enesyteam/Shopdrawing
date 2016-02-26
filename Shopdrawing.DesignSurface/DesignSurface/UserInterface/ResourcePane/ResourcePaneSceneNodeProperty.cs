// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourcePane.ResourcePaneSceneNodeProperty
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Microsoft.Expression.DesignSurface.UserInterface.ResourcePane
{
  internal class ResourcePaneSceneNodeProperty : TypedSceneNodeProperty
  {
    public override PropertyValueEditor PropertyValueEditor
    {
      get
      {
        PropertyValueEditor propertyValueEditor = base.PropertyValueEditor;
        if (PlatformTypes.Transform.IsAssignableFrom((ITypeId) this.PropertyTypeId) || PlatformTypes.Transform3D.IsAssignableFrom((ITypeId) this.PropertyTypeId) || (ProjectNeutralTypes.BehaviorEventTriggerBase.IsAssignableFrom((ITypeId) this.PropertyTypeId) || ProjectNeutralTypes.BehaviorTargetedTriggerAction.IsAssignableFrom((ITypeId) this.PropertyTypeId)))
          propertyValueEditor = (PropertyValueEditor) null;
        if (PlatformTypes.Thickness.IsAssignableFrom((ITypeId) this.PropertyTypeId) || PlatformTypes.KeySpline.IsAssignableFrom((ITypeId) this.PropertyTypeId) || (PlatformTypes.EasingFunctionBase.IsAssignableFrom((ITypeId) this.PropertyTypeId) || PlatformTypes.Projection.IsAssignableFrom((ITypeId) this.PropertyTypeId)))
        {
          DataTemplate inlineEditorTemplate = (DataTemplate) null;
          if (this.Converter != null && this.Converter.CanConvertTo(typeof (string)))
            inlineEditorTemplate = this.ProvideInlineTemplate();
          propertyValueEditor = (PropertyValueEditor) new ExtendedPropertyValueEditor(propertyValueEditor.InlineEditorTemplate, inlineEditorTemplate);
        }
        return propertyValueEditor;
      }
    }

    public ResourcePaneSceneNodeProperty(SceneNodeObjectSet objectSet, PropertyReference propertyReference, AttributeCollection attributeCollection, Type valueType, ITypeResolver typeResolver)
      : base(objectSet, propertyReference, attributeCollection, valueType, typeResolver)
    {
    }

    private DataTemplate ProvideInlineTemplate()
    {
      DataTemplate dataTemplate = new DataTemplate();
      FrameworkElementFactory frameworkElementFactory = new FrameworkElementFactory(typeof (Grid));
      frameworkElementFactory.SetValue(FrameworkElement.MinHeightProperty, (object) 20.0);
      frameworkElementFactory.SetValue(ClickBehavior.MouseClickCommandProperty, (object) PropertyValueEditorCommands.ShowExtendedPopupEditor);
      FrameworkElementFactory child = new FrameworkElementFactory(typeof (TextBlock));
      child.SetValue(FrameworkElement.VerticalAlignmentProperty, (object) VerticalAlignment.Center);
      Binding binding = new Binding("StringValue");
      child.SetValue(TextBlock.TextProperty, (object) binding);
      frameworkElementFactory.AppendChild(child);
      dataTemplate.VisualTree = frameworkElementFactory;
      dataTemplate.Seal();
      return dataTemplate;
    }
  }
}
