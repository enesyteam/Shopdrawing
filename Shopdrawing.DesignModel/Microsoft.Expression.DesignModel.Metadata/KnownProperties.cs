using System;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal class KnownProperties : IKnownProperties
	{
		public readonly static IPropertyId ArrayExtensionTypeProperty;

		public readonly static IPropertyId ArrayExtensionItemsProperty;

		public readonly static IPropertyId BeginStoryboardStoryboardProperty;

		public readonly static IPropertyId BindingElementNameProperty;

		public readonly static IPropertyId BindingPathProperty;

		public readonly static IPropertyId BindingRelativeSourceProperty;

		public readonly static IPropertyId ControlTemplateTriggersProperty;

		public readonly static IPropertyId ControlTemplateTargetTypeProperty;

		public readonly static IPropertyId DictionaryEntryKeyProperty;

		public readonly static IPropertyId DictionaryEntryValueProperty;

		public readonly static IPropertyId DynamicResourceResourceKeyProperty;

		public readonly static IPropertyId EventTriggerActionsProperty;

		public readonly static IPropertyId FrameSourceProperty;

		public readonly static IPropertyId FrameworkContentElementNameProperty;

		public readonly static IPropertyId FrameworkContentElementResourcesProperty;

		public readonly static IPropertyId FrameworkContentElementStyleProperty;

		public readonly static IPropertyId FrameworkElementDataContextProperty;

		public readonly static IPropertyId FrameworkElementActualWidthProperty;

		public readonly static IPropertyId FrameworkElementActualHeightProperty;

		public readonly static IPropertyId FrameworkElementWidthProperty;

		public readonly static IPropertyId FrameworkElementHeightProperty;

		public readonly static IPropertyId FrameworkElementMarginProperty;

		public readonly static IPropertyId FrameworkElementMinWidthProperty;

		public readonly static IPropertyId FrameworkElementMinHeightProperty;

		public readonly static IPropertyId FrameworkElementMaxWidthProperty;

		public readonly static IPropertyId FrameworkElementMaxHeightProperty;

		public readonly static IPropertyId FrameworkElementNameProperty;

		public readonly static IPropertyId FrameworkElementStyleProperty;

		public readonly static IPropertyId FrameworkElementTriggersProperty;

		public readonly static IPropertyId FrameworkElementResourcesProperty;

		public readonly static IPropertyId FrameworkTemplateVisualTreeProperty;

		public readonly static IPropertyId FrameworkTemplateResourcesProperty;

		public readonly static IPropertyId MediaElementAutoPlayProperty;

		public readonly static IPropertyId PageTemplateProperty;

		public readonly static IPropertyId PropertyPathPathProperty;

		public readonly static IPropertyId PropertyPathPathParametersProperty;

		public readonly static IPropertyId ResourceDictionaryMergedDictionariesProperty;

		public readonly static IPropertyId ResourceDictionarySourceProperty;

		public readonly static IPropertyId SetterTargetNameProperty;

		public readonly static IPropertyId SetterPropertyProperty;

		public readonly static IPropertyId SetterValueProperty;

		public readonly static IPropertyId SolidColorBrushColorProperty;

		public readonly static IPropertyId StaticExtensionMemberProperty;

		public readonly static IPropertyId StaticResourceResourceKeyProperty;

		public readonly static IPropertyId StoryboardTargetNameProperty;

		public readonly static IPropertyId StoryboardTargetPropertyProperty;

		public readonly static IPropertyId StyleBasedOnProperty;

		public readonly static IPropertyId StyleResourcesProperty;

		public readonly static IPropertyId StyleSettersProperty;

		public readonly static IPropertyId StyleTargetTypeProperty;

		public readonly static IPropertyId StyleTriggersProperty;

		public readonly static IPropertyId TemplateBindingPropertyProperty;

		public readonly static IPropertyId TriggerPropertyProperty;

		public readonly static IPropertyId TriggerSettersProperty;

		public readonly static IPropertyId TriggerSourceNameProperty;

		public readonly static IPropertyId TriggerValueProperty;

		public readonly static IPropertyId TypeExtensionTypeNameProperty;

		public readonly static IPropertyId TypeExtensionTypeProperty;

		public readonly static IPropertyId VisualBrushVisualProperty;

		public readonly static IPropertyId VisualStateGroupStatesProperty;

		public readonly static IPropertyId PopupChildProperty;

		public readonly static IPropertyId ItemContainerStyleProperty;

		public readonly static IPropertyId InputMethodIsInputMethodEnabledProperty;

		public readonly static IPropertyId InlineUIContainerChildProperty;

		public readonly static IPropertyId InlineFlowDirectionProperty;

		public readonly static IPropertyId InlineLanguageProperty;

		public readonly static IPropertyId ContentControlContentProperty;

		public readonly static IPropertyId ContentPresenterContentProperty;

		public readonly static IPropertyId ContentPresenterContentSourceProperty;

		public readonly static IPropertyId ContentPresenterContentTemplateProperty;

		public readonly static IPropertyId ContentPresenterContentTemplateSelectorProperty;

		public readonly static IPropertyId XmlDataProviderIsAsynchronousProperty;

		public readonly static IPropertyId TextBlockInlinesProperty;

		public readonly static IPropertyId TextBlockTextProperty;

		public readonly static IPropertyId ModelVisual3DContentProperty;

		public readonly static IPropertyId ItemsControlItemsProperty;

		public readonly static IPropertyId ItemsControlItemsSourceProperty;

		public readonly static IPropertyId ItemsControlItemsPanelProperty;

		public readonly static IPropertyId DataTemplateDataTypeProperty;

		public readonly static IPropertyId DataTemplateTriggersProperty;

		public readonly static IPropertyId ControlTemplateProperty;

		public readonly static IPropertyId BitmapImageUriSourceProperty;

		public readonly static IPropertyId HeaderedItemsControlHeaderProperty;

		public readonly static IPropertyId SelectorSelectedIndexProperty;

		public readonly static IPropertyId RelativeSourceModeProperty;

		public readonly static IPropertyId CenterXProperty;

		public readonly static IPropertyId CenterYProperty;

		public readonly static IPropertyId ScaleXProperty;

		public readonly static IPropertyId ScaleYProperty;

		public readonly static IPropertyId SkewXProperty;

		public readonly static IPropertyId SkewYProperty;

		public readonly static IPropertyId RotationProperty;

		public readonly static IPropertyId TranslateXProperty;

		public readonly static IPropertyId TranslateYProperty;

		public readonly static IPropertyId PathDataProperty;

		public IPropertyId ArrayExtensionItems
		{
			get
			{
				return KnownProperties.ArrayExtensionItemsProperty;
			}
		}

		public IPropertyId ArrayExtensionType
		{
			get
			{
				return KnownProperties.ArrayExtensionTypeProperty;
			}
		}

		public IPropertyId BindingPath
		{
			get
			{
				return KnownProperties.BindingPathProperty;
			}
		}

		public IPropertyId BitmapImageUriSource
		{
			get
			{
				return KnownProperties.BitmapImageUriSourceProperty;
			}
		}

		public IPropertyId DesignTimeClass
		{
			get
			{
				return DesignTimeProperties.ClassProperty;
			}
		}

		public IPropertyId DesignTimeClassModifier
		{
			get
			{
				return DesignTimeProperties.ClassModifierProperty;
			}
		}

		public IPropertyId DesignTimeFieldModifier
		{
			get
			{
				return DesignTimeProperties.FieldModifierProperty;
			}
		}

		public IPropertyId DesignTimeFreeze
		{
			get
			{
				return DesignTimeProperties.FreezeProperty;
			}
		}

		public IPropertyId DesignTimeInlineXml
		{
			get
			{
				return DesignTimeProperties.InlineXmlProperty;
			}
		}

		public IPropertyId DesignTimeShared
		{
			get
			{
				return DesignTimeProperties.SharedProperty;
			}
		}

		public IPropertyId DesignTimeShouldSerialize
		{
			get
			{
				return DesignTimeProperties.ShouldSerializeProperty;
			}
		}

		public IPropertyId DesignTimeSubclass
		{
			get
			{
				return DesignTimeProperties.SubclassProperty;
			}
		}

		public IPropertyId DesignTimeUid
		{
			get
			{
				return DesignTimeProperties.UidProperty;
			}
		}

		public IPropertyId DesignTimeVisualTree
		{
			get
			{
				return DesignTimeProperties.VisualTreeProperty;
			}
		}

		public IPropertyId DesignTimeXName
		{
			get
			{
				return DesignTimeProperties.XNameProperty;
			}
		}

		public IPropertyId DictionaryEntryKey
		{
			get
			{
				return KnownProperties.DictionaryEntryKeyProperty;
			}
		}

		public IPropertyId DictionaryEntryValue
		{
			get
			{
				return KnownProperties.DictionaryEntryValueProperty;
			}
		}

		public IPropertyId FrameworkTemplateVisualTree
		{
			get
			{
				return KnownProperties.FrameworkTemplateVisualTreeProperty;
			}
		}

		public IPropertyId PropertyPathPath
		{
			get
			{
				return KnownProperties.PropertyPathPathProperty;
			}
		}

		public IPropertyId PropertyPathPathParameters
		{
			get
			{
				return KnownProperties.PropertyPathPathParametersProperty;
			}
		}

		public IPropertyId SolidColorBrushColor
		{
			get
			{
				return KnownProperties.SolidColorBrushColorProperty;
			}
		}

		public IPropertyId StaticExtensionMember
		{
			get
			{
				return KnownProperties.StaticExtensionMemberProperty;
			}
		}

		public IPropertyId StyleSetters
		{
			get
			{
				return KnownProperties.StyleSettersProperty;
			}
		}

		public IPropertyId TemplateBindingProperty
		{
			get
			{
				return KnownProperties.TemplateBindingPropertyProperty;
			}
		}

		public IPropertyId TypeExtensionType
		{
			get
			{
				return KnownProperties.TypeExtensionTypeProperty;
			}
		}

		public IPropertyId TypeExtensionTypeName
		{
			get
			{
				return KnownProperties.TypeExtensionTypeNameProperty;
			}
		}

		static KnownProperties()
		{
			KnownProperties.ArrayExtensionTypeProperty = (IPropertyId)PlatformTypes.ArrayExtension.GetMember(MemberType.LocalProperty, "Type", MemberAccessTypes.Public);
			KnownProperties.ArrayExtensionItemsProperty = (IPropertyId)PlatformTypes.ArrayExtension.GetMember(MemberType.LocalProperty, "Items", MemberAccessTypes.Public);
			KnownProperties.BeginStoryboardStoryboardProperty = (IPropertyId)PlatformTypes.BeginStoryboard.GetMember(MemberType.LocalProperty, "Storyboard", MemberAccessTypes.Public);
			KnownProperties.BindingElementNameProperty = (IPropertyId)PlatformTypes.Binding.GetMember(MemberType.LocalProperty, "ElementName", MemberAccessTypes.Public);
			KnownProperties.BindingPathProperty = (IPropertyId)PlatformTypes.Binding.GetMember(MemberType.LocalProperty, "Path", MemberAccessTypes.Public);
			KnownProperties.BindingRelativeSourceProperty = (IPropertyId)PlatformTypes.Binding.GetMember(MemberType.LocalProperty, "RelativeSource", MemberAccessTypes.Public);
			KnownProperties.ControlTemplateTriggersProperty = (IPropertyId)PlatformTypes.ControlTemplate.GetMember(MemberType.LocalProperty, "Triggers", MemberAccessTypes.Public);
			KnownProperties.ControlTemplateTargetTypeProperty = (IPropertyId)PlatformTypes.ControlTemplate.GetMember(MemberType.LocalProperty, "TargetType", MemberAccessTypes.Public);
			KnownProperties.DictionaryEntryKeyProperty = (IPropertyId)PlatformTypes.DictionaryEntry.GetMember(MemberType.LocalProperty, "Key", MemberAccessTypes.Public);
			KnownProperties.DictionaryEntryValueProperty = (IPropertyId)PlatformTypes.DictionaryEntry.GetMember(MemberType.LocalProperty, "Value", MemberAccessTypes.Public);
			KnownProperties.DynamicResourceResourceKeyProperty = (IPropertyId)PlatformTypes.DynamicResource.GetMember(MemberType.LocalProperty, "ResourceKey", MemberAccessTypes.Public);
			KnownProperties.EventTriggerActionsProperty = (IPropertyId)PlatformTypes.EventTrigger.GetMember(MemberType.LocalProperty, "Actions", MemberAccessTypes.Public);
			KnownProperties.FrameSourceProperty = (IPropertyId)ProjectNeutralTypes.Frame.GetMember(MemberType.LocalProperty, "Source", MemberAccessTypes.Public);
			KnownProperties.FrameworkContentElementNameProperty = (IPropertyId)PlatformTypes.FrameworkContentElement.GetMember(MemberType.LocalProperty, "Name", MemberAccessTypes.Public);
			KnownProperties.FrameworkContentElementResourcesProperty = (IPropertyId)PlatformTypes.FrameworkContentElement.GetMember(MemberType.LocalProperty, "Resources", MemberAccessTypes.Public);
			KnownProperties.FrameworkContentElementStyleProperty = (IPropertyId)PlatformTypes.FrameworkContentElement.GetMember(MemberType.LocalProperty, "Style", MemberAccessTypes.Public);
			KnownProperties.FrameworkElementDataContextProperty = (IPropertyId)PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "DataContext", MemberAccessTypes.Public);
			KnownProperties.FrameworkElementActualWidthProperty = (IPropertyId)PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "ActualWidth", MemberAccessTypes.Public);
			KnownProperties.FrameworkElementActualHeightProperty = (IPropertyId)PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "ActualHeight", MemberAccessTypes.Public);
			KnownProperties.FrameworkElementWidthProperty = (IPropertyId)PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "Width", MemberAccessTypes.Public);
			KnownProperties.FrameworkElementHeightProperty = (IPropertyId)PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "Height", MemberAccessTypes.Public);
			KnownProperties.FrameworkElementMarginProperty = (IPropertyId)PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "Margin", MemberAccessTypes.Public);
			KnownProperties.FrameworkElementMinWidthProperty = (IPropertyId)PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "MinWidth", MemberAccessTypes.Public);
			KnownProperties.FrameworkElementMinHeightProperty = (IPropertyId)PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "MinHeight", MemberAccessTypes.Public);
			KnownProperties.FrameworkElementMaxWidthProperty = (IPropertyId)PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "MaxWidth", MemberAccessTypes.Public);
			KnownProperties.FrameworkElementMaxHeightProperty = (IPropertyId)PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "MaxHeight", MemberAccessTypes.Public);
			KnownProperties.FrameworkElementNameProperty = (IPropertyId)PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "Name", MemberAccessTypes.Public);
			KnownProperties.FrameworkElementStyleProperty = (IPropertyId)PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "Style", MemberAccessTypes.Public);
			KnownProperties.FrameworkElementTriggersProperty = (IPropertyId)PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "Triggers", MemberAccessTypes.Public);
			KnownProperties.FrameworkElementResourcesProperty = (IPropertyId)PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "Resources", MemberAccessTypes.Public);
			KnownProperties.FrameworkTemplateVisualTreeProperty = DesignTimeProperties.VisualTreeProperty;
			KnownProperties.FrameworkTemplateResourcesProperty = (IPropertyId)PlatformTypes.FrameworkTemplate.GetMember(MemberType.LocalProperty, "Resources", MemberAccessTypes.Public);
			KnownProperties.MediaElementAutoPlayProperty = (IPropertyId)PlatformTypes.MediaElement.GetMember(MemberType.LocalProperty, "AutoPlay", MemberAccessTypes.Public);
			KnownProperties.PageTemplateProperty = (IPropertyId)PlatformTypes.Page.GetMember(MemberType.LocalProperty, "Template", MemberAccessTypes.Public);
			KnownProperties.PropertyPathPathProperty = (IPropertyId)PlatformTypes.PropertyPath.GetMember(MemberType.LocalProperty, "Path", MemberAccessTypes.Public);
			KnownProperties.PropertyPathPathParametersProperty = (IPropertyId)PlatformTypes.PropertyPath.GetMember(MemberType.LocalProperty, "PathParameters", MemberAccessTypes.Public);
			KnownProperties.ResourceDictionaryMergedDictionariesProperty = (IPropertyId)PlatformTypes.ResourceDictionary.GetMember(MemberType.LocalProperty, "MergedDictionaries", MemberAccessTypes.Public);
			KnownProperties.ResourceDictionarySourceProperty = (IPropertyId)PlatformTypes.ResourceDictionary.GetMember(MemberType.LocalProperty, "Source", MemberAccessTypes.Public);
			KnownProperties.SetterTargetNameProperty = (IPropertyId)PlatformTypes.Setter.GetMember(MemberType.LocalProperty, "TargetName", MemberAccessTypes.Public);
			KnownProperties.SetterPropertyProperty = (IPropertyId)PlatformTypes.Setter.GetMember(MemberType.LocalProperty, "Property", MemberAccessTypes.Public);
			KnownProperties.SetterValueProperty = (IPropertyId)PlatformTypes.Setter.GetMember(MemberType.LocalProperty, "Value", MemberAccessTypes.Public);
			KnownProperties.SolidColorBrushColorProperty = (IPropertyId)PlatformTypes.SolidColorBrush.GetMember(MemberType.LocalProperty, "Color", MemberAccessTypes.Public);
			KnownProperties.StaticExtensionMemberProperty = (IPropertyId)PlatformTypes.StaticExtension.GetMember(MemberType.LocalProperty, "Member", MemberAccessTypes.Public);
			KnownProperties.StaticResourceResourceKeyProperty = (IPropertyId)PlatformTypes.StaticResource.GetMember(MemberType.LocalProperty, "ResourceKey", MemberAccessTypes.Public);
			KnownProperties.StoryboardTargetNameProperty = (IPropertyId)PlatformTypes.Storyboard.GetMember(MemberType.AttachedProperty, "TargetName", MemberAccessTypes.Public);
			KnownProperties.StoryboardTargetPropertyProperty = (IPropertyId)PlatformTypes.Storyboard.GetMember(MemberType.AttachedProperty, "TargetProperty", MemberAccessTypes.Public);
			KnownProperties.StyleBasedOnProperty = (IPropertyId)PlatformTypes.Style.GetMember(MemberType.LocalProperty, "BasedOn", MemberAccessTypes.Public);
			KnownProperties.StyleResourcesProperty = (IPropertyId)PlatformTypes.Style.GetMember(MemberType.LocalProperty, "Resources", MemberAccessTypes.Public);
			KnownProperties.StyleSettersProperty = (IPropertyId)PlatformTypes.Style.GetMember(MemberType.LocalProperty, "Setters", MemberAccessTypes.Public);
			KnownProperties.StyleTargetTypeProperty = (IPropertyId)PlatformTypes.Style.GetMember(MemberType.LocalProperty, "TargetType", MemberAccessTypes.Public);
			KnownProperties.StyleTriggersProperty = (IPropertyId)PlatformTypes.Style.GetMember(MemberType.LocalProperty, "Triggers", MemberAccessTypes.Public);
			KnownProperties.TemplateBindingPropertyProperty = (IPropertyId)PlatformTypes.TemplateBinding.GetMember(MemberType.LocalProperty, "Property", MemberAccessTypes.Public);
			KnownProperties.TriggerPropertyProperty = (IPropertyId)PlatformTypes.Trigger.GetMember(MemberType.LocalProperty, "Property", MemberAccessTypes.Public);
			KnownProperties.TriggerSettersProperty = (IPropertyId)PlatformTypes.Trigger.GetMember(MemberType.LocalProperty, "Setters", MemberAccessTypes.Public);
			KnownProperties.TriggerSourceNameProperty = (IPropertyId)PlatformTypes.Trigger.GetMember(MemberType.LocalProperty, "SourceName", MemberAccessTypes.Public);
			KnownProperties.TriggerValueProperty = (IPropertyId)PlatformTypes.Trigger.GetMember(MemberType.LocalProperty, "Value", MemberAccessTypes.Public);
			KnownProperties.TypeExtensionTypeNameProperty = (IPropertyId)PlatformTypes.TypeExtension.GetMember(MemberType.LocalProperty, "TypeName", MemberAccessTypes.Public);
			KnownProperties.TypeExtensionTypeProperty = (IPropertyId)PlatformTypes.TypeExtension.GetMember(MemberType.LocalProperty, "Type", MemberAccessTypes.Public);
			KnownProperties.VisualBrushVisualProperty = (IPropertyId)PlatformTypes.VisualBrush.GetMember(MemberType.LocalProperty, "Visual", MemberAccessTypes.Public);
			KnownProperties.VisualStateGroupStatesProperty = (IPropertyId)ProjectNeutralTypes.VisualStateGroup.GetMember(MemberType.LocalProperty, "States", MemberAccessTypes.Public);
			KnownProperties.PopupChildProperty = (IPropertyId)PlatformTypes.Popup.GetMember(MemberType.LocalProperty, "Child", MemberAccessTypes.Public);
			KnownProperties.ItemContainerStyleProperty = (IPropertyId)PlatformTypes.ItemsControl.GetMember(MemberType.LocalProperty, "ItemContainerStyle", MemberAccessTypes.Public);
			KnownProperties.InputMethodIsInputMethodEnabledProperty = (IPropertyId)PlatformTypes.InputMethod.GetMember(MemberType.AttachedProperty, "IsInputMethodEnabled", MemberAccessTypes.Public);
			KnownProperties.InlineUIContainerChildProperty = (IPropertyId)PlatformTypes.InlineUIContainer.GetMember(MemberType.LocalProperty, "Child", MemberAccessTypes.Public);
			KnownProperties.InlineFlowDirectionProperty = (IPropertyId)PlatformTypes.Inline.GetMember(MemberType.LocalProperty, "FlowDirection", MemberAccessTypes.Public);
			KnownProperties.InlineLanguageProperty = (IPropertyId)PlatformTypes.Inline.GetMember(MemberType.LocalProperty, "Language", MemberAccessTypes.Public);
			KnownProperties.ContentControlContentProperty = (IPropertyId)PlatformTypes.ContentControl.GetMember(MemberType.LocalProperty, "Content", MemberAccessTypes.Public);
			KnownProperties.ContentPresenterContentProperty = (IPropertyId)PlatformTypes.ContentPresenter.GetMember(MemberType.LocalProperty, "Content", MemberAccessTypes.Public);
			KnownProperties.ContentPresenterContentSourceProperty = (IPropertyId)PlatformTypes.ContentPresenter.GetMember(MemberType.LocalProperty, "ContentSource", MemberAccessTypes.Public);
			KnownProperties.ContentPresenterContentTemplateProperty = (IPropertyId)PlatformTypes.ContentPresenter.GetMember(MemberType.LocalProperty, "ContentTemplate", MemberAccessTypes.Public);
			KnownProperties.ContentPresenterContentTemplateSelectorProperty = (IPropertyId)PlatformTypes.ContentPresenter.GetMember(MemberType.LocalProperty, "ContentTemplateSelector", MemberAccessTypes.Public);
			KnownProperties.XmlDataProviderIsAsynchronousProperty = (IPropertyId)PlatformTypes.XmlDataProvider.GetMember(MemberType.LocalProperty, "IsAsynchronous", MemberAccessTypes.Public);
			KnownProperties.TextBlockInlinesProperty = (IPropertyId)PlatformTypes.TextBlock.GetMember(MemberType.LocalProperty, "Inlines", MemberAccessTypes.Public);
			KnownProperties.TextBlockTextProperty = (IPropertyId)PlatformTypes.TextBlock.GetMember(MemberType.LocalProperty, "Text", MemberAccessTypes.Public);
			KnownProperties.ModelVisual3DContentProperty = (IPropertyId)PlatformTypes.ModelVisual3D.GetMember(MemberType.LocalProperty, "Content", MemberAccessTypes.Public);
			KnownProperties.ItemsControlItemsProperty = (IPropertyId)PlatformTypes.ItemsControl.GetMember(MemberType.LocalProperty, "Items", MemberAccessTypes.Public);
			KnownProperties.ItemsControlItemsSourceProperty = (IPropertyId)PlatformTypes.ItemsControl.GetMember(MemberType.LocalProperty, "ItemsSource", MemberAccessTypes.Public);
			KnownProperties.ItemsControlItemsPanelProperty = (IPropertyId)PlatformTypes.ItemsControl.GetMember(MemberType.LocalProperty, "ItemsPanel", MemberAccessTypes.Public);
			KnownProperties.DataTemplateDataTypeProperty = (IPropertyId)PlatformTypes.DataTemplate.GetMember(MemberType.LocalProperty, "DataType", MemberAccessTypes.Public);
			KnownProperties.DataTemplateTriggersProperty = (IPropertyId)PlatformTypes.DataTemplate.GetMember(MemberType.LocalProperty, "Triggers", MemberAccessTypes.Public);
			KnownProperties.ControlTemplateProperty = (IPropertyId)PlatformTypes.Control.GetMember(MemberType.LocalProperty, "Template", MemberAccessTypes.Public);
			KnownProperties.BitmapImageUriSourceProperty = (IPropertyId)PlatformTypes.BitmapImage.GetMember(MemberType.LocalProperty, "UriSource", MemberAccessTypes.Public);
			KnownProperties.HeaderedItemsControlHeaderProperty = (IPropertyId)ProjectNeutralTypes.HeaderedItemsControl.GetMember(MemberType.LocalProperty, "Header", MemberAccessTypes.Public);
			KnownProperties.SelectorSelectedIndexProperty = (IPropertyId)PlatformTypes.Selector.GetMember(MemberType.LocalProperty, "SelectedIndex", MemberAccessTypes.Public);
			KnownProperties.RelativeSourceModeProperty = (IPropertyId)PlatformTypes.RelativeSource.GetMember(MemberType.LocalProperty, "Mode", MemberAccessTypes.Public);
			KnownProperties.CenterXProperty = (IPropertyId)PlatformTypes.CompositeTransform.GetMember(MemberType.Property, "CenterX", MemberAccessTypes.Public);
			KnownProperties.CenterYProperty = (IPropertyId)PlatformTypes.CompositeTransform.GetMember(MemberType.Property, "CenterY", MemberAccessTypes.Public);
			KnownProperties.ScaleXProperty = (IPropertyId)PlatformTypes.CompositeTransform.GetMember(MemberType.Property, "ScaleX", MemberAccessTypes.Public);
			KnownProperties.ScaleYProperty = (IPropertyId)PlatformTypes.CompositeTransform.GetMember(MemberType.Property, "ScaleY", MemberAccessTypes.Public);
			KnownProperties.SkewXProperty = (IPropertyId)PlatformTypes.CompositeTransform.GetMember(MemberType.Property, "SkewX", MemberAccessTypes.Public);
			KnownProperties.SkewYProperty = (IPropertyId)PlatformTypes.CompositeTransform.GetMember(MemberType.Property, "SkewY", MemberAccessTypes.Public);
			KnownProperties.RotationProperty = (IPropertyId)PlatformTypes.CompositeTransform.GetMember(MemberType.Property, "Rotation", MemberAccessTypes.Public);
			KnownProperties.TranslateXProperty = (IPropertyId)PlatformTypes.CompositeTransform.GetMember(MemberType.Property, "TranslateX", MemberAccessTypes.Public);
			KnownProperties.TranslateYProperty = (IPropertyId)PlatformTypes.CompositeTransform.GetMember(MemberType.Property, "TranslateY", MemberAccessTypes.Public);
			KnownProperties.PathDataProperty = (IPropertyId)PlatformTypes.Path.GetMember(MemberType.Property, "Data", MemberAccessTypes.Public);
		}

		public KnownProperties()
		{
		}
	}
}