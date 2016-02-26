using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public abstract class DesignTimeProperties
	{
		public const string DesignWidthPropertyName = "DesignWidth";

		public const string DesignHeightPropertyName = "DesignHeight";

		private readonly static ITypeId LayoutOverridesType;

		public readonly static IPropertyId IsPrototypingCompositionProperty;

		public readonly static IPropertyId HandoffStoryboardProperty;

		public readonly static IPropertyId IsLayerProperty;

		public readonly static IPropertyId LayoutOverridesProperty;

		public readonly static IPropertyId IsGroupProperty;

		public readonly static IPropertyId LayoutRectProperty;

		public readonly static IPropertyId SlotOriginProperty;

		public readonly static IPropertyId CopyTokenProperty;

		public readonly static IPropertyId TestNameProperty;

		public readonly static IPropertyId IsControlPartProperty;

		public readonly static IPropertyId IsAnimatedProperty;

		public readonly static IPropertyId LayoutRoundingProperty;

		public readonly static IPropertyId ShouldSerializeProperty;

		public readonly static IPropertyId BrushKeyProperty;

		public readonly static IPropertyId BrushDocumentReferenceProperty;

		public readonly static IPropertyId UseSampleDataProperty;

		public readonly static IPropertyId SharedProperty;

		public readonly static IPropertyId IsDefaultStyleProperty;

		public readonly static IPropertyId IsDataSourceProperty;

		public readonly static IPropertyId SampleDataTagProperty;

		public readonly static IPropertyId SchemaUriProperty;

		public readonly static IPropertyId ClassProperty;

		public readonly static IPropertyId SubclassProperty;

		public readonly static IPropertyId ClassModifierProperty;

		public readonly static IPropertyId FieldModifierProperty;

		public readonly static IPropertyId UidProperty;

		public readonly static IPropertyId InlineXmlProperty;

		public readonly static IPropertyId FreezeProperty;

		public readonly static IPropertyId XNameProperty;

		public readonly static IPropertyId VisualTreeProperty;

		public readonly static IPropertyId IsLockedProperty;

		public readonly static IPropertyId IsTextEditingProperty;

		public readonly static IPropertyId StoryboardNameProperty;

		public readonly static IPropertyId RuntimeVisibilityProperty;

		public readonly static IPropertyId IsHiddenProperty;

		public readonly static IPropertyId RuntimeOpacityProperty;

		public readonly static IPropertyId RuntimeIsHitTestVisibleProperty;

		public readonly static IPropertyId IsEffectDisabledProperty;

		public readonly static IPropertyId RuntimeEffectProperty;

		public readonly static IPropertyId RuntimeProjectionProperty;

		public readonly static IPropertyId RuntimeWidthProperty;

		public readonly static IPropertyId DesignWidthProperty;

		public readonly static IPropertyId RuntimeHeightProperty;

		public readonly static IPropertyId DesignHeightProperty;

		public readonly static IPropertyId IsStaticTextProperty;

		public readonly static IPropertyId RuntimeDataContextProperty;

		public readonly static IPropertyId RuntimeContentDataContextProperty;

		public readonly static IPropertyId DesignDataContextProperty;

		public readonly static IPropertyId RuntimeCustomVisualStateManagerProperty;

		public readonly static IPropertyId DesignTimeBlockVisualStateManagerProperty;

		public readonly static string ExplicitAnimationPropertyName;

		public readonly static IPropertyId ExplicitAnimationProperty;

		public readonly static IPropertyId RuntimeFlowDirectionProperty;

		public readonly static IPropertyId OwningTimelineSourceProperty;

		public readonly static IPropertyId RuntimeLoadedBehaviorProperty;

		public readonly static IPropertyId RuntimeScrubbingEnabledProperty;

		public readonly static IPropertyId DesignTimeNaturalDurationProperty;

		public readonly static IPropertyId RuntimeAutoPlayProperty;

		public readonly static IPropertyId IsPopupOpenProperty;

		public readonly static IPropertyId DefaultSelectedIndexProperty;

		public readonly static IPropertyId RuntimeSelectedIndexProperty;

		public readonly static IPropertyId DesignTimeSelectedIndexProperty;

		public readonly static IPropertyId RuntimeSelectedItemProperty;

		public readonly static IPropertyId RuntimeSelectedValueProperty;

		public readonly static IPropertyId ViewNodeProperty;

		public readonly static IPropertyId ViewNodeIdProperty;

		public readonly static IPropertyId InstantiatedElementViewNodeProperty;

		public readonly static IPropertyId InstantiatedElementStyleViewNodeIdProperty;

		public readonly static IPropertyId InstantiatedElementTemplateViewNodeIdProperty;

		public readonly static IPropertyId InstantiatedElementTemplateTriggerProperty;

		public readonly static IPropertyId ChildProperty;

		public readonly static IPropertyId IsOpenProperty;

		public readonly static IPropertyId PlacementRectangleProperty;

		public readonly static IPropertyId PlacementTargetProperty;

		public readonly static IPropertyId AllowsTransparencyProperty;

		public readonly static IPropertyId CustomPopupPlacementCallbackProperty;

		public readonly static IPropertyId HorizontalOffsetProperty;

		public readonly static IPropertyId PlacementProperty;

		public readonly static IPropertyId PopupAnimationProperty;

		public readonly static IPropertyId StaysOpenProperty;

		public readonly static IPropertyId VerticalOffsetProperty;

		public readonly static IPropertyId RuntimePageTemplateProperty;

		public readonly static IPropertyId RuntimeWindowTemplateProperty;

		public readonly static IPropertyId RuntimeExpanderIsExpandedProperty;

		public readonly static IPropertyId DesignTimeExpanderIsExpandedProperty;

		public readonly static IPropertyId RuntimeTreeViewItemIsExpandedProperty;

		public readonly static IPropertyId DesignTimeTreeViewItemIsExpandedProperty;

		public readonly static IPropertyId RuntimeRepeatBehaviorProperty;

		public readonly static IPropertyId RuntimeCollectionViewSourceProperty;

		public readonly static IPropertyId DesignSourceProperty;

		public readonly static IPropertyId RuntimeFirstColumnProperty;

		public readonly static IPropertyId DesignTimeNameScopeProperty;

		public readonly static IPropertyId BoundsProperty;

		public readonly static IPropertyId EulerAnglesProperty;

		public readonly static IPropertyId RuntimeIsDropDownOpenProperty;

		public readonly static IPropertyId IsExpandedProperty;

		public readonly static IPropertyId RuntimeSourceProperty;

		public readonly static IPropertyId RuntimeFontUriProperty;

		public readonly static IPropertyId RuntimeFontFamilyProperty;

		public readonly static IPropertyId RuntimeFontStyleProperty;

		public readonly static IPropertyId RuntimeFontStretchProperty;

		public readonly static IPropertyId RuntimeFontWeightProperty;

		public readonly static IPropertyId RuntimeIsUndoEnabledProperty;

		public readonly static IPropertyId IsTextEditProxyProperty;

		public readonly static IPropertyId InstanceBuilderContextProperty;

		public readonly static IPropertyId SourceDictionaryNodeProperty;

		public readonly static IPropertyId SourceDocumentRootProperty;

		public readonly static IPropertyId IsEnhancedOutOfPlaceRootProperty;

		public readonly static IPropertyId IsVisualTriggerActiveProperty;

		public readonly static IPropertyId LastTangentProperty;

		public readonly static IPropertyId UpdateContextProperty;

		public readonly static IPropertyId IsSketchFlowAnimationProperty;

		public readonly static IPropertyId IsSketchFlowStyleProperty;

		public readonly static IPropertyId IsSketchFlowDefaultStyleProperty;

		public readonly static IPropertyId AutoTransitionProperty;

		public readonly static IPropertyId IsOptimizedProperty;

		public readonly static IPropertyId IsAnimationProxyProperty;

		public readonly static IPropertyId IsPlaceholderProperty;

		public readonly static IPropertyId IsDesignTimeCreatableProperty;

		public readonly static IPropertyId StyleDefaultContentProperty;

		public readonly static IPropertyId ExplicitWidthProperty;

		public readonly static IPropertyId ExplicitHeightProperty;

		public readonly static IPropertyId IsRuntimeVisibilityTimelineProperty;

		public readonly static IPropertyId TextEditProxyReferenceProperty;

		private readonly static List<int> preferredValueShadowProperties;

		private static HashSet<IPropertyId> useShadowPropertyForInstanceBuilding;

		private static HashSet<IPropertyId> shadowPropertiesUsingDesignerCoercionCallback;

		private readonly static List<IPropertyId> propertiesShadowedInSilverlightAnimation;

		private static Dictionary<string, DesignTimeProperties.DesignTimePropertyId> shadowSourceNameToDesignProperties;

		private static Dictionary<string, DesignTimeProperties.DesignTimePropertyId> neutralDesignPropertiesByName;

		private static Dictionary<int, DesignTimeProperties.DesignTimePropertyId> neutralDesignPropertiesBySortValue;

		internal static Dictionary<string, bool> shadowSourceNameExclusionSet;

		private static Dictionary<DesignTimeProperties.DesignTimePropertyId, DesignTimeProperties.DesignTimePropertyId> neutralShadowPeerProperties;

		private Dictionary<DesignTimeProperties.DesignTimePropertyId, IProperty> neutralToResolvedProperties = new Dictionary<DesignTimeProperties.DesignTimePropertyId, IProperty>();

		private Dictionary<IPropertyId, DesignTimeProperties.DesignTimePropertyId> resolvedToNeutralProperties = new Dictionary<IPropertyId, DesignTimeProperties.DesignTimePropertyId>();

		internal Microsoft.Expression.DesignModel.Metadata.PlatformTypes PlatformTypes
		{
			get;
			private set;
		}

		public static IEnumerable<IPropertyId> ShadowPropertiesUsingDesignerCoercionCallback
		{
			get
			{
				return DesignTimeProperties.shadowPropertiesUsingDesignerCoercionCallback;
			}
		}

		static DesignTimeProperties()
		{
			DesignTimeProperties.LayoutOverridesType = new PlatformNeutralTypeId(typeof(LayoutOverrides).FullName);
			DesignTimeProperties.IsPrototypingCompositionProperty = DesignTimeProperties.Register("IsPrototypingComposition", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Boolean, "false", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.UserControl);
			DesignTimeProperties.HandoffStoryboardProperty = DesignTimeProperties.Register("HandoffStoryboard", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Object, null, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Storyboard, DesignerSerializationVisibility.Hidden);
			DesignTimeProperties.IsLayerProperty = DesignTimeProperties.Register("IsLayer", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Boolean, "false", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.DependencyObject);
			DesignTimeProperties.LayoutOverridesProperty = DesignTimeProperties.Register("LayoutOverrides", DesignTimeProperties.LayoutOverridesType, "None", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.DependencyObject);
			DesignTimeProperties.IsGroupProperty = DesignTimeProperties.Register("IsGroup", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Boolean, "false", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.DependencyObject);
			DesignTimeProperties.LayoutRectProperty = DesignTimeProperties.Register("LayoutRect", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Rect, "0,0,0,0", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.DependencyObject);
			DesignTimeProperties.SlotOriginProperty = DesignTimeProperties.Register("SlotOrigin", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Point, "0,0", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.DependencyObject);
			DesignTimeProperties.CopyTokenProperty = DesignTimeProperties.Register("CopyToken", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.String, "", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.DependencyObject);
			DesignTimeProperties.TestNameProperty = DesignTimeProperties.Register("TestName", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.String, "", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.DependencyObject);
			DesignTimeProperties.IsControlPartProperty = DesignTimeProperties.RegisterDocumentOnly("IsControlPart", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Boolean, "false", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Style);
			DesignTimeProperties.IsAnimatedProperty = DesignTimeProperties.Register("IsAnimated", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Boolean, "false", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.PathGeometry, DesignerSerializationVisibility.Hidden);
			DesignTimeProperties.LayoutRoundingProperty = DesignTimeProperties.Register("LayoutRounding", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.String, string.Empty, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.FrameworkElement);
			DesignTimeProperties.ShouldSerializeProperty = DesignTimeProperties.Register("ShouldSerialize", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Boolean, "true", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.DependencyObject);
			DesignTimeProperties.BrushKeyProperty = DesignTimeProperties.Register("BrushKey", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.String, null, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Canvas);
			DesignTimeProperties.BrushDocumentReferenceProperty = DesignTimeProperties.Register("BrushDocumentReference", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.String, null, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Canvas);
			DesignTimeProperties.UseSampleDataProperty = DesignTimeProperties.RegisterDocumentOnly("UseSampleData", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Boolean, "false", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Object);
			DesignTimeProperties.SharedProperty = DesignTimeProperties.RegisterDocumentOnly("Shared", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Boolean, "false", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Object);
			DesignTimeProperties.IsDefaultStyleProperty = DesignTimeProperties.RegisterDocumentOnly("IsDefaultStyle", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Boolean, "false", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Style);
			DesignTimeProperties.IsDataSourceProperty = DesignTimeProperties.RegisterDocumentOnly("IsDataSource", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Boolean, "false", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Object);
			DesignTimeProperties.SampleDataTagProperty = DesignTimeProperties.RegisterDocumentOnly("SampleDataTag", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Object, null, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Object, DesignerSerializationVisibility.Hidden);
			DesignTimeProperties.SchemaUriProperty = DesignTimeProperties.RegisterDocumentOnly("SchemaUri", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Uri, null, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.XmlDataProvider);
			DesignTimeProperties.ClassProperty = DesignTimeProperties.RegisterDocumentOnly("Class", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.String, null, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Object);
			DesignTimeProperties.SubclassProperty = DesignTimeProperties.RegisterDocumentOnly("Subclass", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.String, null, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Object);
			DesignTimeProperties.ClassModifierProperty = DesignTimeProperties.RegisterDocumentOnly("ClassModifier", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.String, null, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Object);
			DesignTimeProperties.FieldModifierProperty = DesignTimeProperties.RegisterDocumentOnly("FieldModifier", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.String, null, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Object);
			DesignTimeProperties.UidProperty = DesignTimeProperties.RegisterDocumentOnly("Uid", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.String, string.Empty, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Object);
			DesignTimeProperties.InlineXmlProperty = DesignTimeProperties.RegisterDocumentOnly("InlineXml", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.IXmlSerializable, null, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.XData);
			DesignTimeProperties.FreezeProperty = DesignTimeProperties.RegisterDocumentOnly("Freeze", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Boolean, "false", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Object);
			DesignTimeProperties.XNameProperty = DesignTimeProperties.Register("Name", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.String, null, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Object);
			DesignTimeProperties.VisualTreeProperty = DesignTimeProperties.Register("VisualTree", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.DependencyObject, null, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.FrameworkTemplate);
			DesignTimeProperties.IsLockedProperty = DesignTimeProperties.RegisterDocumentOnly("IsLocked", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Boolean, "false", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Object);
			DesignTimeProperties.IsTextEditingProperty = DesignTimeProperties.Register("IsTextEditing", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Boolean, "false", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.UIElement);
			DesignTimeProperties.StoryboardNameProperty = DesignTimeProperties.Register("StoryboardName", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.String, string.Empty, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Timeline, DesignerSerializationVisibility.Hidden);
			DesignTimeProperties.RuntimeVisibilityProperty = DesignTimeProperties.RegisterShadow("RuntimeVisibility", "Visibility", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.UIElement, true);
			ITypeId flag = Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Boolean;
			ITypeId uIElement = Microsoft.Expression.DesignModel.Metadata.PlatformTypes.UIElement;
			IPropertyId[] runtimeVisibilityProperty = new IPropertyId[] { DesignTimeProperties.RuntimeVisibilityProperty };
			DesignTimeProperties.IsHiddenProperty = DesignTimeProperties.RegisterShadowPeer("IsHidden", flag, "false", uIElement, runtimeVisibilityProperty);
			DesignTimeProperties.RuntimeOpacityProperty = DesignTimeProperties.RegisterShadow("RuntimeOpacity", "Opacity", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.UIElement, true);
			DesignTimeProperties.RuntimeIsHitTestVisibleProperty = DesignTimeProperties.RegisterShadow("RuntimeIsHitTestVisible", "IsHitTestVisible", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.UIElement, false);
			DesignTimeProperties.IsEffectDisabledProperty = DesignTimeProperties.Register("IsEffectDisabled", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Boolean, "false", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.UIElement);
			DesignTimeProperties.RuntimeEffectProperty = DesignTimeProperties.RegisterShadow("RuntimeEffect", "Effect", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.UIElement, false);
			DesignTimeProperties.RuntimeProjectionProperty = DesignTimeProperties.RegisterShadow("RuntimeProjection", "Projection", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.UIElement, false);
			DesignTimeProperties.RuntimeWidthProperty = DesignTimeProperties.RegisterShadow("RuntimeWidth", "Width", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.FrameworkElement, true);
			ITypeId num = Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Double;
			ITypeId frameworkElement = Microsoft.Expression.DesignModel.Metadata.PlatformTypes.FrameworkElement;
			IPropertyId[] runtimeWidthProperty = new IPropertyId[] { DesignTimeProperties.RuntimeWidthProperty };
			DesignTimeProperties.DesignWidthProperty = DesignTimeProperties.RegisterShadowPeer("DesignWidth", num, "NaN", frameworkElement, runtimeWidthProperty);
			DesignTimeProperties.RuntimeHeightProperty = DesignTimeProperties.RegisterShadow("RuntimeHeight", "Height", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.FrameworkElement, true);
			ITypeId typeId = Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Double;
			ITypeId frameworkElement1 = Microsoft.Expression.DesignModel.Metadata.PlatformTypes.FrameworkElement;
			IPropertyId[] runtimeHeightProperty = new IPropertyId[] { DesignTimeProperties.RuntimeHeightProperty };
			DesignTimeProperties.DesignHeightProperty = DesignTimeProperties.RegisterShadowPeer("DesignHeight", typeId, "NaN", frameworkElement1, runtimeHeightProperty);
			DesignTimeProperties.IsStaticTextProperty = DesignTimeProperties.Register("IsStaticText", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Boolean, "true", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.DependencyObject);
			DesignTimeProperties.RuntimeDataContextProperty = DesignTimeProperties.RegisterShadow("RuntimeDataContext", "DataContext", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.FrameworkElement, false);
			DesignTimeProperties.RuntimeContentDataContextProperty = DesignTimeProperties.RegisterShadow("RuntimeContentDataContext", "DataContext", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.FrameworkContentElement, false);
			ITypeId obj = Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Object;
			ITypeId dependencyObject = Microsoft.Expression.DesignModel.Metadata.PlatformTypes.DependencyObject;
			IPropertyId[] runtimeDataContextProperty = new IPropertyId[] { DesignTimeProperties.RuntimeDataContextProperty, DesignTimeProperties.RuntimeContentDataContextProperty };
			DesignTimeProperties.DesignDataContextProperty = DesignTimeProperties.RegisterShadowPeer("DataContext", obj, null, dependencyObject, runtimeDataContextProperty);
			DesignTimeProperties.RuntimeCustomVisualStateManagerProperty = DesignTimeProperties.RegisterProjectNeutralShadow("RuntimeCustomVisualStateManager", "CustomVisualStateManager", ProjectNeutralTypes.VisualStateManager, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.FrameworkElement, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Object, null);
			ITypeId obj1 = Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Object;
			ITypeId typeId1 = Microsoft.Expression.DesignModel.Metadata.PlatformTypes.FrameworkElement;
			IPropertyId[] runtimeCustomVisualStateManagerProperty = new IPropertyId[] { DesignTimeProperties.RuntimeCustomVisualStateManagerProperty };
			DesignTimeProperties.DesignTimeBlockVisualStateManagerProperty = DesignTimeProperties.RegisterShadowPeer("DesignTimeBlockVisualStateManager", obj1, null, typeId1, runtimeCustomVisualStateManagerProperty);
			DesignTimeProperties.ExplicitAnimationPropertyName = "ExplicitAnimation";
			DesignTimeProperties.ExplicitAnimationProperty = DesignTimeProperties.Register(DesignTimeProperties.ExplicitAnimationPropertyName, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Double, "0", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.DependencyObject, DesignerSerializationVisibility.Hidden);
			DesignTimeProperties.RuntimeFlowDirectionProperty = DesignTimeProperties.RegisterShadow("RuntimeFlowDirection", "FlowDirection", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.FrameworkElement, false);
			DesignTimeProperties.OwningTimelineSourceProperty = DesignTimeProperties.Register("OwningTimelineSource", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Uri, null, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.MediaElement);
			DesignTimeProperties.RuntimeLoadedBehaviorProperty = DesignTimeProperties.RegisterShadow("RuntimeLoadedBehavior", "LoadedBehavior", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.MediaElement, false);
			DesignTimeProperties.RuntimeScrubbingEnabledProperty = DesignTimeProperties.RegisterShadow("RuntimeScrubbingEnabled", "ScrubbingEnabled", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.MediaElement, false);
			DesignTimeProperties.DesignTimeNaturalDurationProperty = DesignTimeProperties.Register("DesignTimeNaturalDuration", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Double, "0", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.DependencyObject);
			DesignTimeProperties.RuntimeAutoPlayProperty = DesignTimeProperties.RegisterShadow("RuntimeAutoPlay", "AutoPlay", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.MediaElement, false);
			DesignTimeProperties.IsPopupOpenProperty = DesignTimeProperties.Register("IsPopupOpen", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Boolean, "false", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.UIElement, DesignerSerializationVisibility.Hidden);
			DesignTimeProperties.DefaultSelectedIndexProperty = DesignTimeProperties.Register("DefaultSelectedIndex", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Int32, "-1", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Selector, DesignerSerializationVisibility.Hidden);
			DesignTimeProperties.RuntimeSelectedIndexProperty = DesignTimeProperties.RegisterShadow("RuntimeSelectedIndex", "SelectedIndex", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Selector, false);
			ITypeId num1 = Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Int32;
			ITypeId selector = Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Selector;
			IPropertyId[] runtimeSelectedIndexProperty = new IPropertyId[] { DesignTimeProperties.RuntimeSelectedIndexProperty };
			DesignTimeProperties.DesignTimeSelectedIndexProperty = DesignTimeProperties.RegisterShadowPeer("DesignTimeSelectedIndex", num1, "-1", selector, DesignerSerializationVisibility.Hidden, runtimeSelectedIndexProperty);
			DesignTimeProperties.RuntimeSelectedItemProperty = DesignTimeProperties.RegisterShadow("RuntimeSelectedItem", "SelectedItem", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Selector, false);
			DesignTimeProperties.RuntimeSelectedValueProperty = DesignTimeProperties.RegisterShadow("RuntimeSelectedValue", "SelectedValue", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Selector, false);
			DesignTimeProperties.ViewNodeProperty = DesignTimeProperties.Register("ViewNode", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Object, null, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.DependencyObject, DesignerSerializationVisibility.Hidden);
			DesignTimeProperties.ViewNodeIdProperty = DesignTimeProperties.Register("ViewNodeId", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.String, null, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Object, DesignerSerializationVisibility.Hidden);
			DesignTimeProperties.InstantiatedElementViewNodeProperty = DesignTimeProperties.Register("InstantiatedElementViewNode", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Object, null, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.DependencyObject, DesignerSerializationVisibility.Hidden);
			DesignTimeProperties.InstantiatedElementStyleViewNodeIdProperty = DesignTimeProperties.Register("InstantiatedElementStyleViewNodeId", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.String, null, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Object);
			DesignTimeProperties.InstantiatedElementTemplateViewNodeIdProperty = DesignTimeProperties.Register("InstantiatedElementTemplateViewNodeId", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.String, null, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Object);
			DesignTimeProperties.InstantiatedElementTemplateTriggerProperty = DesignTimeProperties.Register("InstantiatedElementTemplateTrigger", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Object, null, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.DependencyObject);
			DesignTimeProperties.ChildProperty = DesignTimeProperties.RegisterShadow("Child", "Child", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Popup, false);
			DesignTimeProperties.IsOpenProperty = DesignTimeProperties.RegisterShadow("IsOpen", "IsOpen", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Popup, false);
			DesignTimeProperties.PlacementRectangleProperty = DesignTimeProperties.RegisterShadow("PlacementRectangle", "PlacementRectangle", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Popup, false);
			DesignTimeProperties.PlacementTargetProperty = DesignTimeProperties.RegisterShadow("PlacementTarget", "PlacementTarget", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Popup, false);
			DesignTimeProperties.AllowsTransparencyProperty = DesignTimeProperties.RegisterShadow("AllowsTransparency", "AllowsTransparency", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Popup, false);
			DesignTimeProperties.CustomPopupPlacementCallbackProperty = DesignTimeProperties.RegisterShadow("CustomPopupPlacementCallback", "CustomPopupPlacementCallback", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Popup, false);
			DesignTimeProperties.HorizontalOffsetProperty = DesignTimeProperties.RegisterShadow("HorizontalOffset", "HorizontalOffset", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Popup, false);
			DesignTimeProperties.PlacementProperty = DesignTimeProperties.RegisterShadow("Placement", "Placement", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Popup, false);
			DesignTimeProperties.PopupAnimationProperty = DesignTimeProperties.RegisterShadow("PopupAnimation", "PopupAnimation", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Popup, false);
			DesignTimeProperties.StaysOpenProperty = DesignTimeProperties.RegisterShadow("StaysOpen", "StaysOpen", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Popup, false);
			DesignTimeProperties.VerticalOffsetProperty = DesignTimeProperties.RegisterShadow("VerticalOffset", "VerticalOffset", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Popup, false);
			DesignTimeProperties.RuntimePageTemplateProperty = DesignTimeProperties.RegisterShadow("RuntimePageTemplate", "Template", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Page, false);
			DesignTimeProperties.RuntimeWindowTemplateProperty = DesignTimeProperties.RegisterShadow("RuntimeWindowTemplate", "Template", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Control, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Window, DesignerSerializationVisibility.Visible, true, false);
			DesignTimeProperties.RuntimeExpanderIsExpandedProperty = DesignTimeProperties.RegisterProjectNeutralShadow("RuntimeExpanderIsExpanded", "IsExpanded", ProjectNeutralTypes.Expander, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.ContentControl, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Boolean, "false");
			ITypeId flag1 = Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Boolean;
			ITypeId contentControl = Microsoft.Expression.DesignModel.Metadata.PlatformTypes.ContentControl;
			IPropertyId[] runtimeExpanderIsExpandedProperty = new IPropertyId[] { DesignTimeProperties.RuntimeExpanderIsExpandedProperty };
			DesignTimeProperties.DesignTimeExpanderIsExpandedProperty = DesignTimeProperties.RegisterShadowPeer("DesignTimeExpanderIsExpanded", flag1, "false", contentControl, DesignerSerializationVisibility.Hidden, runtimeExpanderIsExpandedProperty);
			DesignTimeProperties.RuntimeTreeViewItemIsExpandedProperty = DesignTimeProperties.RegisterProjectNeutralShadow("RuntimeTreeViewItemIsExpanded", "IsExpanded", ProjectNeutralTypes.TreeViewItem, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.ItemsControl, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Boolean, "false");
			ITypeId flag2 = Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Boolean;
			ITypeId itemsControl = Microsoft.Expression.DesignModel.Metadata.PlatformTypes.ItemsControl;
			IPropertyId[] runtimeTreeViewItemIsExpandedProperty = new IPropertyId[] { DesignTimeProperties.RuntimeTreeViewItemIsExpandedProperty };
			DesignTimeProperties.DesignTimeTreeViewItemIsExpandedProperty = DesignTimeProperties.RegisterShadowPeer("DesignTimeTreeViewItemIsExpanded", flag2, "false", itemsControl, DesignerSerializationVisibility.Hidden, runtimeTreeViewItemIsExpandedProperty);
			DesignTimeProperties.RuntimeRepeatBehaviorProperty = DesignTimeProperties.RegisterShadow("RuntimeRepeatBehavior", "RepeatBehavior", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Timeline, false);
			DesignTimeProperties.RuntimeCollectionViewSourceProperty = DesignTimeProperties.RegisterShadow("RuntimeCollectionViewSource", "Source", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.CollectionViewSource, true);
			ITypeId obj2 = Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Object;
			ITypeId collectionViewSource = Microsoft.Expression.DesignModel.Metadata.PlatformTypes.CollectionViewSource;
			IPropertyId[] runtimeCollectionViewSourceProperty = new IPropertyId[] { DesignTimeProperties.RuntimeCollectionViewSourceProperty };
			DesignTimeProperties.DesignSourceProperty = DesignTimeProperties.RegisterShadowPeer("DesignSource", obj2, null, collectionViewSource, runtimeCollectionViewSourceProperty);
			DesignTimeProperties.RuntimeFirstColumnProperty = DesignTimeProperties.RegisterShadow("RuntimeFirstColumn", "FirstColumn", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.UniformGrid, false);
			DesignTimeProperties.DesignTimeNameScopeProperty = DesignTimeProperties.Register("DesignTimeNameScope", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.INameScope, null, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.DependencyObject, DesignerSerializationVisibility.Hidden);
			DesignTimeProperties.BoundsProperty = DesignTimeProperties.RegisterDocumentOnly("Bounds", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Rect3D, "NaN,NaN,NaN,NaN,NaN,NaN", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.DependencyObject);
			DesignTimeProperties.EulerAnglesProperty = DesignTimeProperties.Register("EulerAngles", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Vector3D, "0,0,0", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.DependencyObject);
			DesignTimeProperties.RuntimeIsDropDownOpenProperty = DesignTimeProperties.RegisterShadow("RuntimeIsDropDownOpen", "IsDropDownOpen", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.ComboBox, false);
			DesignTimeProperties.IsExpandedProperty = DesignTimeProperties.Register("IsExpanded", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Boolean, "false", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.ComboBox, DesignerSerializationVisibility.Hidden);
			DesignTimeProperties.RuntimeSourceProperty = DesignTimeProperties.RegisterShadow("RuntimeSource", "Source", ProjectNeutralTypes.Frame, false);
			DesignTimeProperties.RuntimeFontUriProperty = DesignTimeProperties.RegisterShadow("RuntimeFontUri", "FontUri", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Glyphs, true);
			DesignTimeProperties.RuntimeFontFamilyProperty = DesignTimeProperties.RegisterShadow("RuntimeFontFamily", "FontFamily", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Inline, true);
			DesignTimeProperties.RuntimeFontStyleProperty = DesignTimeProperties.RegisterShadow("RuntimeFontStyle", "FontStyle", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Inline, true);
			DesignTimeProperties.RuntimeFontStretchProperty = DesignTimeProperties.RegisterShadow("RuntimeFontStretch", "FontStretch", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Inline, true);
			DesignTimeProperties.RuntimeFontWeightProperty = DesignTimeProperties.RegisterShadow("RuntimeFontWeight", "FontWeight", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Inline, true);
			DesignTimeProperties.RuntimeIsUndoEnabledProperty = DesignTimeProperties.RegisterShadow("RuntimeIsUndoEnabled", "IsUndoEnabled", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.TextBoxBase, true);
			DesignTimeProperties.IsTextEditProxyProperty = DesignTimeProperties.Register("IsTextEditProxy", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Boolean, "false", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.TextBoxBase);
			DesignTimeProperties.InstanceBuilderContextProperty = DesignTimeProperties.Register("InstanceBuilderContext", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Object, null, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.DependencyObject, DesignerSerializationVisibility.Hidden);
			DesignTimeProperties.SourceDictionaryNodeProperty = DesignTimeProperties.RegisterDocumentOnly("SourceDictionaryNode", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Object, null, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.ResourceDictionary, DesignerSerializationVisibility.Hidden);
			DesignTimeProperties.SourceDocumentRootProperty = DesignTimeProperties.RegisterDocumentOnly("SourceDocumentRoot", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Object, null, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.ResourceDictionary, DesignerSerializationVisibility.Hidden);
			DesignTimeProperties.IsEnhancedOutOfPlaceRootProperty = DesignTimeProperties.Register("IsEnhancedOutOfPlaceRoot", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Boolean, "false", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.DependencyObject, DesignerSerializationVisibility.Hidden);
			DesignTimeProperties.IsVisualTriggerActiveProperty = DesignTimeProperties.Register("IsVisualTriggerActive", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Boolean, "true", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.DependencyObject);
			DesignTimeProperties.LastTangentProperty = DesignTimeProperties.RegisterDocumentOnly("LastTangent", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Point, "0,0", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.FrameworkElement, DesignerSerializationVisibility.Hidden);
			DesignTimeProperties.UpdateContextProperty = DesignTimeProperties.Register("UpdateContext", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Object, null, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.RichTextBox);
			DesignTimeProperties.IsSketchFlowAnimationProperty = DesignTimeProperties.Register("IsSketchFlowAnimation", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Boolean, "false", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.DependencyObject);
			DesignTimeProperties.IsSketchFlowStyleProperty = DesignTimeProperties.Register("IsSketchFlowStyle", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Boolean, "false", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.ResourceDictionary);
			DesignTimeProperties.IsSketchFlowDefaultStyleProperty = DesignTimeProperties.Register("IsSketchFlowDefaultStyle", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Boolean, "false", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.ResourceDictionary);
			DesignTimeProperties.AutoTransitionProperty = DesignTimeProperties.Register("AutoTransition", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Boolean, "false", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Timeline, DesignerSerializationVisibility.Visible);
			DesignTimeProperties.IsOptimizedProperty = DesignTimeProperties.Register("IsOptimized", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Boolean, "false", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Timeline);
			DesignTimeProperties.IsAnimationProxyProperty = DesignTimeProperties.Register("IsAnimationProxy", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Boolean, "false", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Timeline);
			DesignTimeProperties.IsPlaceholderProperty = DesignTimeProperties.RegisterDocumentOnly("IsPlaceholder", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Boolean, "false", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.UIElement, DesignerSerializationVisibility.Hidden);
			DesignTimeProperties.IsDesignTimeCreatableProperty = DesignTimeProperties.RegisterDocumentOnly("IsDesignTimeCreatable", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Boolean, "false", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.DataTemplate);
			DesignTimeProperties.StyleDefaultContentProperty = DesignTimeProperties.RegisterDocumentOnly("StyleDefaultContent", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.String, null, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Style);
			DesignTimeProperties.ExplicitWidthProperty = DesignTimeProperties.RegisterDocumentOnly("ExplicitWidth", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Double, "-1", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Style);
			DesignTimeProperties.ExplicitHeightProperty = DesignTimeProperties.RegisterDocumentOnly("ExplicitHeight", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Double, "-1", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Style);
			DesignTimeProperties.IsRuntimeVisibilityTimelineProperty = DesignTimeProperties.Register("IsRuntimeVisibilityTimeline", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Boolean, "false", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Timeline);
			DesignTimeProperties.TextEditProxyReferenceProperty = DesignTimeProperties.Register("TextEditProxyReference", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Object, null, Microsoft.Expression.DesignModel.Metadata.PlatformTypes.DependencyObject, DesignerSerializationVisibility.Hidden);
			DesignTimeProperties.preferredValueShadowProperties = new List<int>();
			List<IPropertyId> propertyIds = new List<IPropertyId>()
			{
				(IPropertyId)Microsoft.Expression.DesignModel.Metadata.PlatformTypes.UIElement.GetMember(MemberType.LocalProperty, "Opacity", MemberAccessTypes.Public),
				(IPropertyId)Microsoft.Expression.DesignModel.Metadata.PlatformTypes.UIElement.GetMember(MemberType.LocalProperty, "Visibility", MemberAccessTypes.Public),
				(IPropertyId)Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Inline.GetMember(MemberType.LocalProperty, "FontStretch", MemberAccessTypes.Public),
				(IPropertyId)Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Control.GetMember(MemberType.LocalProperty, "FontStretch", MemberAccessTypes.Public),
				(IPropertyId)Microsoft.Expression.DesignModel.Metadata.PlatformTypes.TextBlock.GetMember(MemberType.LocalProperty, "FontStretch", MemberAccessTypes.Public),
				(IPropertyId)Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Inline.GetMember(MemberType.LocalProperty, "FontStyle", MemberAccessTypes.Public),
				(IPropertyId)Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Control.GetMember(MemberType.LocalProperty, "FontStyle", MemberAccessTypes.Public),
				(IPropertyId)Microsoft.Expression.DesignModel.Metadata.PlatformTypes.TextBlock.GetMember(MemberType.LocalProperty, "FontStyle", MemberAccessTypes.Public),
				(IPropertyId)Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Inline.GetMember(MemberType.LocalProperty, "FontWeight", MemberAccessTypes.Public),
				(IPropertyId)Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Control.GetMember(MemberType.LocalProperty, "FontWeight", MemberAccessTypes.Public),
				(IPropertyId)Microsoft.Expression.DesignModel.Metadata.PlatformTypes.TextBlock.GetMember(MemberType.LocalProperty, "FontWeight", MemberAccessTypes.Public)
			};
			DesignTimeProperties.propertiesShadowedInSilverlightAnimation = propertyIds;
			DesignTimeProperties.AddOwnerShadow("FontFamily", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.AccessText, DesignTimeProperties.RuntimeFontFamilyProperty);
			DesignTimeProperties.AddOwnerShadow("FontFamily", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Control, DesignTimeProperties.RuntimeFontFamilyProperty);
			DesignTimeProperties.AddOwnerShadow("FontFamily", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.FlowDocument, DesignTimeProperties.RuntimeFontFamilyProperty);
			DesignTimeProperties.AddOwnerShadow("FontFamily", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.TextBlock, DesignTimeProperties.RuntimeFontFamilyProperty);
			DesignTimeProperties.AddOwnerShadow("FontFamily", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.TextElement, DesignTimeProperties.RuntimeFontFamilyProperty);
			DesignTimeProperties.AddOwnerShadow("FontFamily", ProjectNeutralTypes.DataGridTextColumn, DesignTimeProperties.RuntimeFontFamilyProperty);
			DesignTimeProperties.AddOwnerShadow("FontStretch", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.AccessText, DesignTimeProperties.RuntimeFontStretchProperty);
			DesignTimeProperties.AddOwnerShadow("FontStretch", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Control, DesignTimeProperties.RuntimeFontStretchProperty);
			DesignTimeProperties.AddOwnerShadow("FontStretch", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.FlowDocument, DesignTimeProperties.RuntimeFontStretchProperty);
			DesignTimeProperties.AddOwnerShadow("FontStretch", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.TextBlock, DesignTimeProperties.RuntimeFontStretchProperty);
			DesignTimeProperties.AddOwnerShadow("FontStretch", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.TextElement, DesignTimeProperties.RuntimeFontStretchProperty);
			DesignTimeProperties.AddOwnerShadow("FontStyle", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.AccessText, DesignTimeProperties.RuntimeFontStyleProperty);
			DesignTimeProperties.AddOwnerShadow("FontStyle", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Control, DesignTimeProperties.RuntimeFontStyleProperty);
			DesignTimeProperties.AddOwnerShadow("FontStyle", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.FlowDocument, DesignTimeProperties.RuntimeFontStyleProperty);
			DesignTimeProperties.AddOwnerShadow("FontStyle", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.TextBlock, DesignTimeProperties.RuntimeFontStyleProperty);
			DesignTimeProperties.AddOwnerShadow("FontStyle", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.TextElement, DesignTimeProperties.RuntimeFontStyleProperty);
			DesignTimeProperties.AddOwnerShadow("FontWeight", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.AccessText, DesignTimeProperties.RuntimeFontWeightProperty);
			DesignTimeProperties.AddOwnerShadow("FontWeight", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Control, DesignTimeProperties.RuntimeFontWeightProperty);
			DesignTimeProperties.AddOwnerShadow("FontWeight", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.FlowDocument, DesignTimeProperties.RuntimeFontWeightProperty);
			DesignTimeProperties.AddOwnerShadow("FontWeight", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.TextBlock, DesignTimeProperties.RuntimeFontWeightProperty);
			DesignTimeProperties.AddOwnerShadow("FontWeight", Microsoft.Expression.DesignModel.Metadata.PlatformTypes.TextElement, DesignTimeProperties.RuntimeFontWeightProperty);
			DesignTimeProperties.preferredValueShadowProperties.Add(DesignTimeProperties.RuntimeIsHitTestVisibleProperty.SortValue);
			DesignTimeProperties.preferredValueShadowProperties.Add(DesignTimeProperties.RuntimeLoadedBehaviorProperty.SortValue);
			DesignTimeProperties.preferredValueShadowProperties.Add(DesignTimeProperties.RuntimeScrubbingEnabledProperty.SortValue);
			DesignTimeProperties.preferredValueShadowProperties.Add(DesignTimeProperties.RuntimeAutoPlayProperty.SortValue);
			DesignTimeProperties.preferredValueShadowProperties.Add(DesignTimeProperties.RuntimeRepeatBehaviorProperty.SortValue);
			DesignTimeProperties.preferredValueShadowProperties.Add(DesignTimeProperties.RuntimeIsDropDownOpenProperty.SortValue);
			DesignTimeProperties.preferredValueShadowProperties.Add(DesignTimeProperties.RuntimeOpacityProperty.SortValue);
			DesignTimeProperties.preferredValueShadowProperties.Add(DesignTimeProperties.RuntimeVisibilityProperty.SortValue);
			DesignTimeProperties.preferredValueShadowProperties.Add(DesignTimeProperties.IsOpenProperty.SortValue);
			DesignTimeProperties.preferredValueShadowProperties.Sort();
		}

		protected DesignTimeProperties(Microsoft.Expression.DesignModel.Metadata.PlatformTypes platformTypes)
		{
			this.PlatformTypes = platformTypes;
			IType type = this.PlatformTypes.GetType(typeof(DocumentOnlyDesignTimePropertiesHost));
			foreach (DesignTimeProperties.DesignTimePropertyId value in DesignTimeProperties.neutralDesignPropertiesBySortValue.Values)
			{
				if (!value.IsDocumentOnly)
				{
					continue;
				}
				IProperty property = DesignTimeProperties.PlatformDocumentOnlyDesignTimeProperty.Create(value, type, this.PlatformTypes);
				if (property == null)
				{
					continue;
				}
				this.RegisterResolvedProperty(value, property);
			}
		}

		private static void AddOwnerShadow(string shadowSourcePropertyName, ITypeId ownerType, IPropertyId designTimePropertyKey)
		{
			DesignTimeProperties.DesignTimePropertyId designTimePropertyId = (DesignTimeProperties.DesignTimePropertyId)designTimePropertyKey;
			IPropertyId member = (IPropertyId)ownerType.GetMember(MemberType.LocalProperty, shadowSourcePropertyName, MemberAccessTypes.Public);
			DesignTimeProperties.shadowSourceNameToDesignProperties.Add(member.FullName, designTimePropertyId);
		}

		private static void AddUseShadowPropertyForInstanceBuilding(bool usesDesignerCoercionCallback, DesignTimeProperties.DesignTimePropertyId property)
		{
			if (!usesDesignerCoercionCallback)
			{
				if (DesignTimeProperties.useShadowPropertyForInstanceBuilding == null)
				{
					DesignTimeProperties.useShadowPropertyForInstanceBuilding = new HashSet<IPropertyId>();
				}
				DesignTimeProperties.useShadowPropertyForInstanceBuilding.Add(property);
				return;
			}
			if (DesignTimeProperties.shadowPropertiesUsingDesignerCoercionCallback == null)
			{
				DesignTimeProperties.shadowPropertiesUsingDesignerCoercionCallback = new HashSet<IPropertyId>();
			}
			DesignTimeProperties.shadowPropertiesUsingDesignerCoercionCallback.Add(property);
		}

		public static IProperty Clone(IProperty resolvedDesignProperty, ITypeResolver typeResolver)
		{
			DesignTimeProperties.DesignTimePropertyId designTimePropertyId;
			IProperty property;
			Microsoft.Expression.DesignModel.Metadata.PlatformTypes platformMetadata = (Microsoft.Expression.DesignModel.Metadata.PlatformTypes)typeResolver.PlatformMetadata;
			if (DesignTimeProperties.neutralDesignPropertiesBySortValue.TryGetValue(resolvedDesignProperty.SortValue, out designTimePropertyId) && platformMetadata.DesignTimeProperties.neutralToResolvedProperties.TryGetValue(designTimePropertyId, out property))
			{
				return property;
			}
			return null;
		}

		public object ExternalRegisterShadow(string propertyName, IType shadowSourceDeclaringType, IProperty shadowedProperty, DesignerSerializationVisibility serializationVisibility, bool enforceTypeCheck, DesignTimeProperties.PropertyChangeCallback propertyChangeCallback)
		{
			DesignTimeProperties.DesignTimePropertyId designTimePropertyId = new DesignTimeProperties.DesignTimePropertyId(propertyName, shadowedProperty, serializationVisibility)
			{
				EnforceTypeCheck = enforceTypeCheck
			};
			DesignTimeProperties.RegisterInternal(designTimePropertyId);
			if (DesignTimeProperties.shadowSourceNameToDesignProperties == null)
			{
				DesignTimeProperties.shadowSourceNameToDesignProperties = new Dictionary<string, DesignTimeProperties.DesignTimePropertyId>();
			}
			DesignTimeProperties.shadowSourceNameToDesignProperties[shadowedProperty.FullName] = designTimePropertyId;
			DesignTimeProperties.AddUseShadowPropertyForInstanceBuilding(false, designTimePropertyId);
			return this.RegisterPlatformSpecificDocumentOnlyDesignTimeProperty(designTimePropertyId, shadowSourceDeclaringType, propertyChangeCallback);
		}

		public static IProperty FromName(string name, Microsoft.Expression.DesignModel.Metadata.PlatformTypes platformTypes, IType targetType)
		{
			DesignTimeProperties.DesignTimePropertyId designTimePropertyId = null;
			if (!DesignTimeProperties.neutralDesignPropertiesByName.TryGetValue(name, out designTimePropertyId))
			{
				return null;
			}
			IProperty property = DesignTimeProperties.ResolveDesignTimePropertyKey(designTimePropertyId, platformTypes);
			if (property == null || targetType == null)
			{
				return property;
			}
			if (designTimePropertyId == DesignTimeProperties.IsLockedProperty)
			{
				if (Microsoft.Expression.DesignModel.Metadata.PlatformTypes.DependencyObject.IsAssignableFrom(targetType))
				{
					return property;
				}
				if (Microsoft.Expression.DesignModel.Metadata.PlatformTypes.FrameworkTemplate.IsAssignableFrom(targetType))
				{
					return property;
				}
				if (Microsoft.Expression.DesignModel.Metadata.PlatformTypes.Style.IsAssignableFrom(targetType))
				{
					return property;
				}
			}
			else if (designTimePropertyId == DesignTimeProperties.DesignDataContextProperty)
			{
				if (Microsoft.Expression.DesignModel.Metadata.PlatformTypes.FrameworkElement.IsAssignableFrom(targetType))
				{
					return property;
				}
				if (Microsoft.Expression.DesignModel.Metadata.PlatformTypes.FrameworkContentElement.IsAssignableFrom(targetType))
				{
					return property;
				}
			}
			else if (property.TargetType.IsAssignableFrom(targetType.RuntimeType))
			{
				return property;
			}
			return null;
		}

		public static PropertyReference GetAppliedShadowPropertyReference(PropertyReference propertyReference, ITypeId targetType)
		{
			return DesignTimeProperties.GetShadowPropertyReference(propertyReference, targetType, false);
		}

		public static object GetDocumentOnlyDefaultValue(IProperty resolvedProperty)
		{
			DesignTimeProperties.PlatformDocumentOnlyDesignTimeProperty platformDocumentOnlyDesignTimeProperty = resolvedProperty as DesignTimeProperties.PlatformDocumentOnlyDesignTimeProperty;
			if (platformDocumentOnlyDesignTimeProperty == null)
			{
				return null;
			}
			return platformDocumentOnlyDesignTimeProperty.DefaultValue;
		}

		public static DependencyPropertyReferenceStep GetShadowPeerProperty(IProperty resolvedShadowProperty)
		{
			DesignTimeProperties.DesignTimePropertyId designTimePropertyId;
			DesignTimeProperties.DesignTimePropertyId designTimePropertyId1;
			if (!DesignTimeProperties.neutralDesignPropertiesBySortValue.TryGetValue(resolvedShadowProperty.SortValue, out designTimePropertyId))
			{
				return null;
			}
			if (!DesignTimeProperties.neutralShadowPeerProperties.TryGetValue(designTimePropertyId, out designTimePropertyId1))
			{
				return null;
			}
			return DesignTimeProperties.ResolveDesignTimeReferenceStep(designTimePropertyId1, (Microsoft.Expression.DesignModel.Metadata.PlatformTypes)resolvedShadowProperty.DeclaringType.PlatformMetadata);
		}

		public static DependencyPropertyReferenceStep GetShadowProperty(IProperty shadowSourceProperty, ITypeId targetType)
		{
			ITypeId typeId;
			DesignTimeProperties.DesignTimePropertyId designTimePropertyId = null;
			if (!DesignTimeProperties.shadowSourceNameToDesignProperties.TryGetValue(shadowSourceProperty.FullName, out designTimePropertyId))
			{
				return null;
			}
			if (DesignTimeProperties.shadowSourceNameExclusionSet != null && DesignTimeProperties.shadowSourceNameExclusionSet.ContainsKey(shadowSourceProperty.FullName))
			{
				return null;
			}
			DependencyPropertyReferenceStep dependencyPropertyReferenceStep = DesignTimeProperties.ResolveDesignTimeReferenceStep(designTimePropertyId, (Microsoft.Expression.DesignModel.Metadata.PlatformTypes)shadowSourceProperty.DeclaringType.PlatformMetadata);
			if (dependencyPropertyReferenceStep == null)
			{
				return null;
			}
			if (targetType == null)
			{
				return dependencyPropertyReferenceStep;
			}
			if (!designTimePropertyId.EnforceTypeCheck)
			{
				return dependencyPropertyReferenceStep;
			}
			typeId = (!designTimePropertyId.IsShadow ? designTimePropertyId.OwnerTypeId : designTimePropertyId.ShadowSourceProperty.DeclaringTypeId);
			if (!shadowSourceProperty.DeclaringType.PlatformMetadata.ResolveType(typeId).IsAssignableFrom(targetType))
			{
				return null;
			}
			return dependencyPropertyReferenceStep;
		}

		private static PropertyReference GetShadowPropertyReference(PropertyReference propertyReference, ITypeId targetType, bool returnNullOnFailure)
		{
			DependencyPropertyReferenceStep shadowProperty = DesignTimeProperties.GetShadowProperty(propertyReference[0], targetType);
			if (shadowProperty == null)
			{
				if (!returnNullOnFailure)
				{
					return propertyReference;
				}
				return null;
			}
			PropertyReference propertyReference1 = new PropertyReference(shadowProperty);
			if (propertyReference.Count > 1)
			{
				propertyReference1 = propertyReference1.Append(propertyReference.Subreference(1));
			}
			return propertyReference1;
		}

		public static PropertyReference GetShadowPropertyReference(PropertyReference propertyReference, ITypeId targetType)
		{
			return DesignTimeProperties.GetShadowPropertyReference(propertyReference, targetType, true);
		}

		public static DependencyPropertyReferenceStep GetShadowSourceProperty(IProperty resolvedDesignTimeProperty)
		{
			DependencyPropertyReferenceStep dependencyPropertyReferenceStep = resolvedDesignTimeProperty as DependencyPropertyReferenceStep;
			DesignTimeProperties.DesignTimePropertyId designTimePropertyId = null;
			if (!dependencyPropertyReferenceStep.PlatformTypes.DesignTimeProperties.resolvedToNeutralProperties.TryGetValue(dependencyPropertyReferenceStep, out designTimePropertyId) || !designTimePropertyId.IsShadow)
			{
				return null;
			}
			return dependencyPropertyReferenceStep.PlatformTypes.ResolveProperty(designTimePropertyId.ShadowSourceProperty) as DependencyPropertyReferenceStep;
		}

		public static bool IsDocumentOnlyDesignTimeProperty(IPropertyId resolvedDesignTimeProperty)
		{
			DesignTimeProperties.DesignTimePropertyId designTimePropertyId;
			if (!PropertySortValue.IsDesignTimeProperty(resolvedDesignTimeProperty.SortValue) || !DesignTimeProperties.neutralDesignPropertiesBySortValue.TryGetValue(resolvedDesignTimeProperty.SortValue, out designTimePropertyId))
			{
				return false;
			}
			return designTimePropertyId.IsDocumentOnly;
		}

		public static bool IsShadowedInSilverlightAnimation(IProperty property)
		{
			return DesignTimeProperties.propertiesShadowedInSilverlightAnimation.Contains(property);
		}

		public static bool IsShadowValuePreferred(IProperty resolvedShadowProperty)
		{
			if (DesignTimeProperties.preferredValueShadowProperties.BinarySearch(resolvedShadowProperty.SortValue) >= 0)
			{
				return true;
			}
			return false;
		}

		protected static DesignTimeProperties.DesignTimePropertyId Register(string propertyName, ITypeId valueTypeId, string defaultValue, ITypeId targetTypeId, DesignerSerializationVisibility serializationVisibility)
		{
			DesignTimeProperties.DesignTimePropertyId designTimePropertyId = new DesignTimeProperties.DesignTimePropertyId(propertyName, valueTypeId, defaultValue, targetTypeId, serializationVisibility, false);
			return DesignTimeProperties.RegisterInternal(designTimePropertyId);
		}

		protected static DesignTimeProperties.DesignTimePropertyId Register(string propertyName, ITypeId valueTypeId, string defaultValue, ITypeId targetTypeId)
		{
			return DesignTimeProperties.Register(propertyName, valueTypeId, defaultValue, targetTypeId, DesignerSerializationVisibility.Visible);
		}

		private static DesignTimeProperties.DesignTimePropertyId RegisterDocumentOnly(string propertyName, ITypeId valueTypeId, string defaultValue, ITypeId targetTypeId, DesignerSerializationVisibility visibility)
		{
			DesignTimeProperties.DesignTimePropertyId designTimePropertyId = new DesignTimeProperties.DesignTimePropertyId(propertyName, valueTypeId, defaultValue, targetTypeId, visibility, true);
			return DesignTimeProperties.RegisterInternal(designTimePropertyId);
		}

		private static DesignTimeProperties.DesignTimePropertyId RegisterDocumentOnly(string propertyName, ITypeId valueTypeId, string defaultValue, ITypeId targetTypeId)
		{
			return DesignTimeProperties.RegisterDocumentOnly(propertyName, valueTypeId, defaultValue, targetTypeId, DesignerSerializationVisibility.Visible);
		}

		private static DesignTimeProperties.DesignTimePropertyId RegisterInternal(DesignTimeProperties.DesignTimePropertyId property)
		{
			if (DesignTimeProperties.neutralDesignPropertiesByName == null)
			{
				DesignTimeProperties.neutralDesignPropertiesByName = new Dictionary<string, DesignTimeProperties.DesignTimePropertyId>();
			}
			DesignTimeProperties.neutralDesignPropertiesByName.Add(property.Name, property);
			if (DesignTimeProperties.neutralDesignPropertiesBySortValue == null)
			{
				DesignTimeProperties.neutralDesignPropertiesBySortValue = new Dictionary<int, DesignTimeProperties.DesignTimePropertyId>();
			}
			DesignTimeProperties.neutralDesignPropertiesBySortValue.Add(property.SortValue, property);
			return property;
		}

		protected abstract object RegisterPlatformSpecificDocumentOnlyDesignTimeProperty(IPropertyId neutralProperty, IType declaringType, DesignTimeProperties.PropertyChangeCallback propertyChangeCallback);

		private static DesignTimeProperties.DesignTimePropertyId RegisterProjectNeutralShadow(string shadowPropertyName, string sourcePropertyName, ITypeId sourceProjectNeutralTypeId, ITypeId commonPlatformTypeId, ITypeId valueTypeId, string defaultValue)
		{
			DesignTimeProperties.DesignTimePropertyId designTimePropertyId = DesignTimeProperties.Register(shadowPropertyName, valueTypeId, defaultValue, commonPlatformTypeId, DesignerSerializationVisibility.Visible);
			DesignTimeProperties.AddOwnerShadow(sourcePropertyName, sourceProjectNeutralTypeId, designTimePropertyId);
			DesignTimeProperties.AddUseShadowPropertyForInstanceBuilding(false, designTimePropertyId);
			return designTimePropertyId;
		}

		protected void RegisterResolvedProperty(IPropertyId neutralProperty, IProperty resolvedProperty)
		{
			DesignTimeProperties.DesignTimePropertyId designTimePropertyId = neutralProperty as DesignTimeProperties.DesignTimePropertyId;
			if (designTimePropertyId == null)
			{
				return;
			}
			this.neutralToResolvedProperties.Add(designTimePropertyId, resolvedProperty);
			if (resolvedProperty != null)
			{
				this.resolvedToNeutralProperties.Add(resolvedProperty, designTimePropertyId);
			}
			IType declaringType = resolvedProperty.DeclaringType;
			PropertyReference.RegisterAssemblyNamespace(PlatformTypeHelper.GetRuntimeAssembly(declaringType), declaringType.Namespace);
		}

		private static DesignTimeProperties.DesignTimePropertyId RegisterShadow(string propertyName, string shadowSourcePropertyName, ITypeId shadowSourceDeclaringTypeId, ITypeId shadowTargetTypeId, DesignerSerializationVisibility serializationVisibility, bool enforceTypeCheck, bool usesDesignerCoercionCallback)
		{
			IPropertyId member = (IPropertyId)shadowTargetTypeId.GetMember(MemberType.LocalProperty, shadowSourcePropertyName, MemberAccessTypes.Public);
			IPropertyId propertyId = (IPropertyId)shadowSourceDeclaringTypeId.GetMember(MemberType.LocalProperty, shadowSourcePropertyName, MemberAccessTypes.Public);
			DesignTimeProperties.DesignTimePropertyId designTimePropertyId = new DesignTimeProperties.DesignTimePropertyId(propertyName, member, serializationVisibility)
			{
				EnforceTypeCheck = enforceTypeCheck
			};
			DesignTimeProperties.RegisterInternal(designTimePropertyId);
			if (DesignTimeProperties.shadowSourceNameToDesignProperties == null)
			{
				DesignTimeProperties.shadowSourceNameToDesignProperties = new Dictionary<string, DesignTimeProperties.DesignTimePropertyId>();
			}
			DesignTimeProperties.shadowSourceNameToDesignProperties[propertyId.FullName] = designTimePropertyId;
			DesignTimeProperties.AddUseShadowPropertyForInstanceBuilding(usesDesignerCoercionCallback, designTimePropertyId);
			return designTimePropertyId;
		}

		protected static DesignTimeProperties.DesignTimePropertyId RegisterShadow(string propertyName, string shadowSourcePropertyName, ITypeId shadowSourceDeclaringTypeId, bool usesDesignerCoercionCallback)
		{
			return DesignTimeProperties.RegisterShadow(propertyName, shadowSourcePropertyName, shadowSourceDeclaringTypeId, shadowSourceDeclaringTypeId, DesignerSerializationVisibility.Visible, false, usesDesignerCoercionCallback);
		}

		private static DesignTimeProperties.DesignTimePropertyId RegisterShadowPeer(string propertyName, ITypeId valueTypeId, string defaultValue, ITypeId targetTypeId, params IPropertyId[] shadowProperties)
		{
			return DesignTimeProperties.RegisterShadowPeer(propertyName, valueTypeId, defaultValue, targetTypeId, DesignerSerializationVisibility.Visible, shadowProperties);
		}

		private static DesignTimeProperties.DesignTimePropertyId RegisterShadowPeer(string propertyName, ITypeId valueTypeId, string defaultValue, ITypeId targetTypeId, DesignerSerializationVisibility visibility, params IPropertyId[] shadowProperties)
		{
			DesignTimeProperties.DesignTimePropertyId designTimePropertyId = DesignTimeProperties.Register(propertyName, valueTypeId, defaultValue, targetTypeId, visibility);
			if (DesignTimeProperties.neutralShadowPeerProperties == null)
			{
				DesignTimeProperties.neutralShadowPeerProperties = new Dictionary<DesignTimeProperties.DesignTimePropertyId, DesignTimeProperties.DesignTimePropertyId>();
			}
			IPropertyId[] propertyIdArray = shadowProperties;
			for (int i = 0; i < (int)propertyIdArray.Length; i++)
			{
				IPropertyId propertyId = propertyIdArray[i];
				DesignTimeProperties.neutralShadowPeerProperties[(DesignTimeProperties.DesignTimePropertyId)propertyId] = designTimePropertyId;
			}
			return designTimePropertyId;
		}

		public static IProperty ResolveDesignTimePropertyKey(IPropertyId propertyKey, IPlatformMetadata platformMetadata)
		{
			Microsoft.Expression.DesignModel.Metadata.PlatformTypes platformType = (Microsoft.Expression.DesignModel.Metadata.PlatformTypes)platformMetadata;
			DesignTimeProperties.DesignTimePropertyId designTimePropertyId = propertyKey as DesignTimeProperties.DesignTimePropertyId;
			if (designTimePropertyId == null)
			{
				return (IProperty)propertyKey;
			}
			IProperty property = null;
			if (!platformType.DesignTimeProperties.neutralToResolvedProperties.TryGetValue(designTimePropertyId, out property))
			{
				return null;
			}
			return property;
		}

		public static DependencyPropertyReferenceStep ResolveDesignTimeReferenceStep(IPropertyId propertyKey, IPlatformMetadata platformMetadata)
		{
			return DesignTimeProperties.ResolveDesignTimePropertyKey(propertyKey, platformMetadata) as DependencyPropertyReferenceStep;
		}

		public static bool UseShadowPropertyForInstanceBuilding(ITypeResolver typeResolver, IPropertyId property)
		{
			DesignTimeProperties.DesignTimePropertyId designTimePropertyId;
			if (!DesignTimeProperties.neutralDesignPropertiesByName.TryGetValue(property.Name, out designTimePropertyId))
			{
				return false;
			}
			if (!typeResolver.IsCapabilitySet(PlatformCapability.SupportsDesignerCoercionCallback))
			{
				return true;
			}
			return DesignTimeProperties.useShadowPropertyForInstanceBuilding.Contains(designTimePropertyId);
		}

		[Conditional("DEBUG")]
		protected static void VerifyDeclaringTypeAccessors(IProperty resolvedProperty)
		{
			if (resolvedProperty.Name == "OwningTimelineSource")
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			Type declaringType = PlatformTypeHelper.GetDeclaringType(resolvedProperty);
			string name = resolvedProperty.Name;
			if (!DesignTimeProperties.IsDocumentOnlyDesignTimeProperty(resolvedProperty))
			{
				Type targetType = resolvedProperty.TargetType;
				Type runtimeType = resolvedProperty.PropertyType.RuntimeType;
				MethodInfo method = declaringType.GetMethod(string.Concat("Get", name), BindingFlags.Static | BindingFlags.Public);
				if (method != null)
				{
					if (!method.ReturnType.Equals(runtimeType))
					{
						CultureInfo invariantCulture = CultureInfo.InvariantCulture;
						object[] str = new object[] { method.ToString(), runtimeType.ToString() };
						stringBuilder.AppendFormat(invariantCulture, "Design-time property getter has invalid type: '{0}'. Expected: '{1}'.", str);
					}
					ParameterInfo[] parameters = method.GetParameters();
					if (parameters == null || (int)parameters.Length != 1)
					{
						CultureInfo cultureInfo = CultureInfo.InvariantCulture;
						object[] fullName = new object[] { resolvedProperty.DeclaringType.FullName, name };
						stringBuilder.AppendFormat(cultureInfo, "Wrong method signature: public static {0}.Get{1}", fullName);
					}
					else
					{
						Type parameterType = parameters[0].ParameterType;
						if (targetType != null && !parameterType.IsAssignableFrom(targetType))
						{
							CultureInfo invariantCulture1 = CultureInfo.InvariantCulture;
							object[] objArray = new object[] { resolvedProperty.DeclaringType.FullName, method.Name, parameterType.Name, targetType.Name };
							stringBuilder.AppendFormat(invariantCulture1, "Method public static {0}.{1} has parameter type {2} which is not assignable from property's target type {3}\r\n", objArray);
						}
					}
					if (!resolvedProperty.ShouldSerialize)
					{
						bool flag = true;
						object[] customAttributes = method.GetCustomAttributes(typeof(DesignerSerializationVisibilityAttribute), false);
						if (customAttributes != null && (int)customAttributes.Length == 1)
						{
							DesignerSerializationVisibilityAttribute designerSerializationVisibilityAttribute = customAttributes[0] as DesignerSerializationVisibilityAttribute;
							if (designerSerializationVisibilityAttribute != null && designerSerializationVisibilityAttribute.Visibility == DesignerSerializationVisibility.Hidden)
							{
								flag = false;
							}
						}
						if (flag)
						{
							CultureInfo cultureInfo1 = CultureInfo.InvariantCulture;
							object[] fullName1 = new object[] { resolvedProperty.DeclaringType.FullName, method.Name };
							stringBuilder.AppendFormat(cultureInfo1, "Method public static {0}.{1} has incorrect DesignerSerializationVisibility attribute\r\n", fullName1);
						}
					}
				}
				else
				{
					CultureInfo invariantCulture2 = CultureInfo.InvariantCulture;
					object[] objArray1 = new object[] { resolvedProperty.DeclaringType.FullName, name };
					stringBuilder.AppendFormat(invariantCulture2, "Missing method declaration: public static {0}.Get{1}\r\n", objArray1);
				}
				MethodInfo methodInfo = declaringType.GetMethod(string.Concat("Set", name), BindingFlags.Static | BindingFlags.Public);
				if (methodInfo != null)
				{
					ParameterInfo[] parameterInfoArray = methodInfo.GetParameters();
					if (parameterInfoArray == null || (int)parameterInfoArray.Length != 2)
					{
						CultureInfo cultureInfo2 = CultureInfo.InvariantCulture;
						object[] fullName2 = new object[] { resolvedProperty.DeclaringType.FullName, name };
						stringBuilder.AppendFormat(cultureInfo2, "Wrong method signature: public static {0}.Set{1}", fullName2);
					}
					else
					{
						if (!parameterInfoArray[1].ParameterType.Equals(runtimeType))
						{
							CultureInfo invariantCulture3 = CultureInfo.InvariantCulture;
							object[] str1 = new object[] { methodInfo.ToString(), runtimeType.ToString() };
							stringBuilder.AppendFormat(invariantCulture3, "Design-time property setter has invalid type: '{0}'. Expected: '{1}'.", str1);
						}
						Type type = parameterInfoArray[0].ParameterType;
						if (targetType != null && !type.IsAssignableFrom(targetType))
						{
							CultureInfo cultureInfo3 = CultureInfo.InvariantCulture;
							object[] objArray2 = new object[] { resolvedProperty.DeclaringType.FullName, methodInfo.Name, type.Name, targetType.Name };
							stringBuilder.AppendFormat(cultureInfo3, "Method public static {0}.{1} has first param type {2} which is not assignable from property's target type {3}\r\n", objArray2);
						}
					}
				}
				else
				{
					CultureInfo invariantCulture4 = CultureInfo.InvariantCulture;
					object[] fullName3 = new object[] { resolvedProperty.DeclaringType.FullName, name };
					stringBuilder.AppendFormat(invariantCulture4, "Missing method declaration: public static {0}.Set{1}", fullName3);
				}
			}
			else
			{
				BindingFlags bindingFlag = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
				if (declaringType.GetField(string.Concat(name, "Property"), bindingFlag) != null)
				{
					CultureInfo cultureInfo4 = CultureInfo.InvariantCulture;
					object[] objArray3 = new object[] { name };
					stringBuilder.AppendFormat(cultureInfo4, "Document only property should not be backed up by static DP field: {0}\r\n", objArray3);
				}
				if (declaringType.GetProperty(string.Concat(name, "Property"), bindingFlag) != null)
				{
					CultureInfo invariantCulture5 = CultureInfo.InvariantCulture;
					object[] objArray4 = new object[] { name };
					stringBuilder.AppendFormat(invariantCulture5, "Document only property should not be backed up by static DP property: {0}\r\n", objArray4);
				}
				if (declaringType.GetMethod(string.Concat("Get", name), bindingFlag) != null)
				{
					CultureInfo cultureInfo5 = CultureInfo.InvariantCulture;
					object[] objArray5 = new object[] { name };
					stringBuilder.AppendFormat(cultureInfo5, "Document only property should not be backed up by static Get{0}: {0}\r\n", objArray5);
				}
				if (declaringType.GetMethod(string.Concat("Set", name), bindingFlag) != null)
				{
					CultureInfo invariantCulture6 = CultureInfo.InvariantCulture;
					object[] objArray6 = new object[] { name };
					stringBuilder.AppendFormat(invariantCulture6, "Document only property should not be backed up by static Get{0}: {0}\r\n", objArray6);
				}
			}
			string.IsNullOrEmpty(stringBuilder.ToString());
		}

		protected sealed class DesignTimePropertyId : IPropertyId, IMemberId
		{
			private string propertyName;

			private ITypeId propertyTypeId;

			private ITypeId ownerTypeId;

			private string defaultValue;

			private IPropertyId shadowSourceProperty;

			private DesignerSerializationVisibility serializationVisibility;

			private int sortValue;

			private bool isDocumentOnly;

			public MemberAccessType Access
			{
				get
				{
					return MemberAccessType.Private;
				}
			}

			public ITypeId DeclaringTypeId
			{
				get
				{
					throw new NotSupportedException();
				}
			}

			public string DefaultValue
			{
				get
				{
					return this.defaultValue;
				}
			}

			public bool EnforceTypeCheck
			{
				get;
				set;
			}

			public string FullName
			{
				get
				{
					if (this.shadowSourceProperty != null)
					{
						throw new NotSupportedException();
					}
					return string.Concat(this.ownerTypeId.FullName, ".", this.propertyName);
				}
			}

			public bool IsDocumentOnly
			{
				get
				{
					return this.isDocumentOnly;
				}
			}

			public bool IsResolvable
			{
				get
				{
					return true;
				}
			}

			public bool IsShadow
			{
				get
				{
					return this.shadowSourceProperty != null;
				}
			}

			public MemberType MemberType
			{
				get
				{
					return MemberType.DesignTimeProperty;
				}
			}

			public string Name
			{
				get
				{
					return this.propertyName;
				}
			}

			public ITypeId OwnerTypeId
			{
				get
				{
					return this.ownerTypeId;
				}
			}

			public ITypeId PropertyTypeId
			{
				get
				{
					if (this.shadowSourceProperty != null)
					{
						throw new NotSupportedException(ExceptionStringTable.PlatformNeutralShadowSourcePropertyDoesNotHaveType);
					}
					return this.propertyTypeId;
				}
			}

			public DesignerSerializationVisibility SerializationVisibility
			{
				get
				{
					return this.serializationVisibility;
				}
			}

			public IPropertyId ShadowSourceProperty
			{
				get
				{
					return this.shadowSourceProperty;
				}
			}

			public int SortValue
			{
				get
				{
					return this.sortValue;
				}
			}

			public Type TargetType
			{
				get
				{
					throw new NotSupportedException();
				}
			}

			public string UniqueName
			{
				get
				{
					return this.FullName;
				}
			}

			internal DesignTimePropertyId(string propertyName, ITypeId valueTypeId, string defaultValue, ITypeId ownerTypeId, DesignerSerializationVisibility serializationVisibility, bool isDocumentOnly)
			{
				if (propertyName == null)
				{
					throw new ArgumentNullException("propertyName");
				}
				if (valueTypeId == null)
				{
					throw new ArgumentNullException("valueTypeId");
				}
				if (ownerTypeId == null)
				{
					throw new ArgumentNullException("ownerTypeId");
				}
				this.propertyName = propertyName;
				this.propertyTypeId = valueTypeId;
				this.defaultValue = defaultValue;
				this.ownerTypeId = ownerTypeId;
				this.serializationVisibility = serializationVisibility;
				this.sortValue = PropertySortValue.RegisterDesignTimeProperty(this);
				this.isDocumentOnly = isDocumentOnly;
			}

			internal DesignTimePropertyId(string propertyName, IPropertyId shadowSourceProperty, DesignerSerializationVisibility serializationVisibility)
			{
				if (propertyName == null)
				{
					throw new ArgumentNullException("propertyName");
				}
				if (shadowSourceProperty == null)
				{
					throw new ArgumentNullException("shadowSourceProperty");
				}
				this.propertyName = propertyName;
				this.shadowSourceProperty = shadowSourceProperty;
				this.serializationVisibility = serializationVisibility;
				this.sortValue = PropertySortValue.RegisterDesignTimeProperty(this);
			}

			public override bool Equals(object obj)
			{
				if (this == obj)
				{
					return true;
				}
				IPropertyId propertyId = obj as IPropertyId;
				if (propertyId == null)
				{
					return false;
				}
				return propertyId.SortValue == this.SortValue;
			}

			public override int GetHashCode()
			{
				return this.propertyName.GetHashCode();
			}

			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder(this.FullName);
				if (this.propertyTypeId != null)
				{
					CultureInfo invariantCulture = CultureInfo.InvariantCulture;
					object[] name = new object[] { this.propertyTypeId.Name };
					stringBuilder.AppendFormat(invariantCulture, ": {0}", name);
				}
				else if (this.shadowSourceProperty != null)
				{
					CultureInfo cultureInfo = CultureInfo.InvariantCulture;
					object[] fullName = new object[] { this.shadowSourceProperty.FullName };
					stringBuilder.AppendFormat(cultureInfo, " -> {0}", fullName);
				}
				if (this.serializationVisibility == DesignerSerializationVisibility.Hidden)
				{
					stringBuilder.Append(" - hidden");
				}
				return stringBuilder.ToString();
			}
		}

		protected sealed class PlatformDocumentOnlyDesignTimeProperty : IProperty, IMember, IPropertyId, IMemberId
		{
			private IType declaringType;

			private IType targetType;

			private IType propertyType;

			private object defaultValue;

			private string propertyName;

			private int sortValue;

			private DesignerSerializationVisibility serializationVisibility;

			public MemberAccessType Access
			{
				get
				{
					return MemberAccessType.Private;
				}
			}

			public AttributeCollection Attributes
			{
				get
				{
					return AttributeCollection.Empty;
				}
			}

			public IType DeclaringType
			{
				get
				{
					return this.declaringType;
				}
			}

			public ITypeId DeclaringTypeId
			{
				get
				{
					return this.declaringType;
				}
			}

			public object DefaultValue
			{
				get
				{
					return this.defaultValue;
				}
			}

			public string FullName
			{
				get
				{
					return string.Concat(this.declaringType.FullName, ".", this.propertyName);
				}
			}

			public bool IsAttachable
			{
				get
				{
					return true;
				}
			}

			public bool IsProxy
			{
				get
				{
					return false;
				}
			}

			public bool IsResolvable
			{
				get
				{
					return true;
				}
			}

			public MemberType MemberType
			{
				get
				{
					return MemberType.DesignTimeProperty;
				}
			}

			public ITypeId MemberTypeId
			{
				get
				{
					return Microsoft.Expression.DesignModel.Metadata.PlatformTypes.PropertyInfo;
				}
			}

			public string Name
			{
				get
				{
					return this.propertyName;
				}
			}

			public IType PropertyType
			{
				get
				{
					return this.propertyType;
				}
			}

			public MemberAccessType ReadAccess
			{
				get
				{
					return MemberAccessType.Public;
				}
			}

			public bool ShouldSerialize
			{
				get
				{
					return this.serializationVisibility == DesignerSerializationVisibility.Visible;
				}
			}

			public int SortValue
			{
				get
				{
					return this.sortValue;
				}
			}

			public Type TargetType
			{
				get
				{
					return this.targetType.RuntimeType;
				}
			}

			public TypeConverter TypeConverter
			{
				get
				{
					return MetadataStore.GetTypeConverter(this.propertyType.RuntimeType);
				}
			}

			public string UniqueName
			{
				get
				{
					return this.FullName;
				}
			}

			public MemberAccessType WriteAccess
			{
				get
				{
					return MemberAccessType.Public;
				}
			}

			private PlatformDocumentOnlyDesignTimeProperty(DesignTimeProperties.DesignTimePropertyId neutralProperty, IType declaringType, IType targetType, IType propertyType)
			{
				this.declaringType = declaringType;
				this.targetType = targetType;
				this.propertyType = propertyType;
				this.propertyName = neutralProperty.Name;
				this.sortValue = neutralProperty.SortValue;
				this.serializationVisibility = neutralProperty.SerializationVisibility;
				if (neutralProperty.DefaultValue != null)
				{
					TypeConverter typeConverter = MetadataStore.GetTypeConverter(this.propertyType.RuntimeType);
					this.defaultValue = typeConverter.ConvertFromString(null, CultureInfo.InvariantCulture, neutralProperty.DefaultValue);
				}
			}

			public IMember Clone(ITypeResolver typeResolver)
			{
				if (this.DeclaringType.PlatformMetadata == typeResolver.PlatformMetadata)
				{
					return this;
				}
				return DesignTimeProperties.Clone(this, typeResolver);
			}

			public static IProperty Create(DesignTimeProperties.DesignTimePropertyId neutralProperty, IType declaringType, IPlatformMetadata platformTypes)
			{
				IType type = platformTypes.ResolveType(neutralProperty.OwnerTypeId);
				if (platformTypes.IsNullType(type))
				{
					return null;
				}
				IType type1 = platformTypes.ResolveType(neutralProperty.PropertyTypeId);
				if (platformTypes.IsNullType(type1))
				{
					return null;
				}
				return new DesignTimeProperties.PlatformDocumentOnlyDesignTimeProperty(neutralProperty, declaringType, type, type1);
			}

			public object GetDefaultValue(Type targetType)
			{
				return null;
			}

			public override int GetHashCode()
			{
				return this.propertyName.GetHashCode();
			}

			public bool HasDefaultValue(Type targetType)
			{
				return false;
			}

			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder(this.FullName);
				if (this.propertyType != null)
				{
					CultureInfo invariantCulture = CultureInfo.InvariantCulture;
					object[] name = new object[] { this.propertyType.Name };
					stringBuilder.AppendFormat(invariantCulture, ": {0}", name);
				}
				return stringBuilder.ToString();
			}
		}

		public delegate void PropertyChangeCallback(object changedObject);
	}
}