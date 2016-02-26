// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Properties.DesignSurfaceMetadata
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Tools.Assets;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.Framework;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Properties
{
  internal class DesignSurfaceMetadata
  {
    private static HashSet<string> HandledMetadata = new HashSet<string>();
    private static Dictionary<string, DesignSurfaceMetadata.AssemblyInformation> assemblyInformation = new Dictionary<string, DesignSurfaceMetadata.AssemblyInformation>();

    static DesignSurfaceMetadata()
    {
      DesignSurfaceMetadata.AddAssemblyInformation(DesignSurfaceMetadata.CreateSystemWindowsControlsDataAssemblyInformation());
      DesignSurfaceMetadata.AddAssemblyInformation(DesignSurfaceMetadata.CreateInteractivityAssemblyInformation());
      DesignSurfaceMetadata.AddAssemblyInformation(DesignSurfaceMetadata.CreateInteractionsAssemblyInformation());
      DesignSurfaceMetadata.AddAssemblyInformation(DesignSurfaceMetadata.CreateSketchFlowInteractivityAssemblyInformation());
      DesignSurfaceMetadata.AddAssemblyInformation(DesignSurfaceMetadata.CreateSketchControlsAssemblyInformation());
      DesignSurfaceMetadata.AddAssemblyInformation(DesignSurfaceMetadata.CreatePresentationFrameworkAssemblyInformation());
      DesignSurfaceMetadata.AddAssemblyInformation(DesignSurfaceMetadata.CreateSilverlightFrameworkAssemblyInformation());
      DesignSurfaceMetadata.AddAssemblyInformation(DesignSurfaceMetadata.CreateDrawingAssemblyInformation());
    }

    private static void AddAssemblyInformation(DesignSurfaceMetadata.AssemblyInformation assemblyInformation)
    {
      DesignSurfaceMetadata.assemblyInformation.Add(assemblyInformation.AssemblyName, assemblyInformation);
    }

    private static DesignSurfaceMetadata.AssemblyInformation CreatePresentationFrameworkAssemblyInformation()
    {
      DesignSurfaceMetadata.AssemblyInformation assemblyInformation = new DesignSurfaceMetadata.AssemblyInformation("PresentationFramework");
      assemblyInformation.AddTypeAttributes(PlatformTypes.ButtonBase, (Attribute) new TemplateVisualStateAttribute()
      {
        Name = "Disabled"
      });
      assemblyInformation.AddTypeAttributes(PlatformTypes.ButtonBase, (Attribute) new TemplateVisualStateAttribute()
      {
        Name = "Pressed"
      });
      assemblyInformation.AddTypeAttributes(PlatformTypes.ButtonBase, (Attribute) new TemplateVisualStateAttribute()
      {
        Name = "MouseOver"
      });
      assemblyInformation.AddTypeAttributes(PlatformTypes.ButtonBase, (Attribute) new TemplateVisualStateAttribute()
      {
        Name = "Normal"
      });
      assemblyInformation.AddTypeAttributes(PlatformTypes.ToggleButton, (Attribute) new TemplateVisualStateAttribute()
      {
        Name = "Indeterminate"
      });
      assemblyInformation.AddTypeAttributes(PlatformTypes.ToggleButton, (Attribute) new TemplateVisualStateAttribute()
      {
        Name = "Checked"
      });
      assemblyInformation.AddTypeAttributes(PlatformTypes.ToggleButton, (Attribute) new TemplateVisualStateAttribute()
      {
        Name = "Unchecked"
      });
      assemblyInformation.AddTypeAttributes(PlatformTypes.ListBoxItem, (Attribute) new TemplateVisualStateAttribute()
      {
        Name = "MouseOver"
      });
      assemblyInformation.AddTypeAttributes(PlatformTypes.ListBoxItem, (Attribute) new TemplateVisualStateAttribute()
      {
        Name = "Normal"
      });
      assemblyInformation.AddTypeAttributes(PlatformTypes.ListBoxItem, (Attribute) new TemplateVisualStateAttribute()
      {
        Name = "Selected"
      });
      assemblyInformation.AddTypeAttributes(PlatformTypes.ListBoxItem, (Attribute) new TemplateVisualStateAttribute()
      {
        Name = "Unselected"
      });
      assemblyInformation.AddTypeAttributes(PlatformTypes.ProgressBar, (Attribute) new TemplateVisualStateAttribute()
      {
        Name = "Determinate"
      });
      assemblyInformation.AddTypeAttributes(PlatformTypes.ProgressBar, (Attribute) new TemplateVisualStateAttribute()
      {
        Name = "Indeterminate"
      });
      assemblyInformation.AddTypeAttributes(PlatformTypes.TextBoxBase, (Attribute) new TemplateVisualStateAttribute()
      {
        Name = "Disabled"
      });
      assemblyInformation.AddTypeAttributes(PlatformTypes.TextBoxBase, (Attribute) new TemplateVisualStateAttribute()
      {
        Name = "ReadOnly"
      });
      assemblyInformation.AddTypeAttributes(PlatformTypes.TextBoxBase, (Attribute) new TemplateVisualStateAttribute()
      {
        Name = "MouseOver"
      });
      assemblyInformation.AddTypeAttributes(PlatformTypes.TextBoxBase, (Attribute) new TemplateVisualStateAttribute()
      {
        Name = "Normal"
      });
      assemblyInformation.AddPropertyAttributes(PlatformTypes.WebBrowser, "Source", (Attribute) PropertyValueEditor.CreateEditorAttribute(typeof (StringViewOfObjectEditor)));
      return assemblyInformation;
    }

    private static DesignSurfaceMetadata.AssemblyInformation CreateSilverlightFrameworkAssemblyInformation()
    {
      DesignSurfaceMetadata.AssemblyInformation assemblyInformation = new DesignSurfaceMetadata.AssemblyInformation("System.Windows");
      assemblyInformation.AddPropertyAttributes(PlatformTypes.HyperlinkButton, "TargetName", (Attribute) PropertyValueEditor.CreateEditorAttribute(typeof (INavigateElementPickerPropertyValueEditor)));
      assemblyInformation.AddPropertyAttributes(PlatformTypes.WebBrowser, "Source", (Attribute) PropertyValueEditor.CreateEditorAttribute(typeof (StringViewOfObjectEditor)));
      return assemblyInformation;
    }

    private static DesignSurfaceMetadata.AssemblyInformation CreateSystemWindowsControlsDataAssemblyInformation()
    {
      DesignSurfaceMetadata.AssemblyInformation assemblyInformation = new DesignSurfaceMetadata.AssemblyInformation("System.Windows.Controls.Data");
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.DataGrid, "RowHeight", (Attribute) new NumberRangesAttribute(new double?(0.0), new double?(0.0), new double?(), new double?(), new bool?(true)));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.DataGrid, "RowHeight", (Attribute) new TypeConverterAttribute(typeof (ConditionalAutoLengthConverter)));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.DataGrid, "RowHeaderWidth", (Attribute) new NumberRangesAttribute(new double?(4.0), new double?(4.0), new double?(), new double?(), new bool?(true)));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.DataGrid, "RowHeaderWidth", (Attribute) new TypeConverterAttribute(typeof (ConditionalAutoLengthConverter)));
      return assemblyInformation;
    }

    private static DesignSurfaceMetadata.AssemblyInformation CreateInteractivityAssemblyInformation()
    {
      DesignSurfaceMetadata.AssemblyInformation assemblyInformation = new DesignSurfaceMetadata.AssemblyInformation("System.Windows.Interactivity");
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.BehaviorTriggerBase, "Actions", (Attribute) new BrowsableAttribute(false));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.Behavior, (Attribute) new KnownUnreferencedTypeAttribute(KnownUnreferencedType.Behavior));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.BehaviorTriggerAction, (Attribute) new KnownUnreferencedTypeAttribute(KnownUnreferencedType.TriggerAction));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.BehaviorTriggerAction, "IsEnabled", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_TriggerAction_IsEnabled), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties), (Attribute) new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.BehaviorTargetedTriggerAction, (Attribute) new DefaultBindingPropertyAttribute("TargetObject"));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.BehaviorTargetedTriggerAction, "TargetObject", (Attribute) new PropertyOrderAttribute(PropertyOrder.CreateBefore(PropertyOrder.Early)), (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_TargetedTriggerAction_TargetObject), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties), (Attribute) PropertyValueEditor.CreateEditorAttribute(typeof (ElementBindingPickerPropertyValueEditor)), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.BehaviorTargetedTriggerAction, "TargetName", (Attribute) new PropertyOrderAttribute(PropertyOrder.CreateBefore(PropertyOrder.Early)), (Attribute) PropertyValueEditor.CreateEditorAttribute(typeof (BehaviorElementPickerPropertyValueEditor)), (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_TargetedTriggerAction_TargetName), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties), (Attribute) DesignSurfaceMetadata.SDKVersionedAttribute.CreateMinimumVersionedAttribute((Attribute) new EditorBrowsableAttribute(EditorBrowsableState.Advanced), DesignSurfaceMetadata.SDKVersion.SDK4));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.BehaviorEventTriggerBase, (Attribute) new DefaultBindingPropertyAttribute("SourceObject"));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.BehaviorEventTriggerBase, "SourceObject", (Attribute) new PropertyOrderAttribute(PropertyOrder.CreateBefore(PropertyOrder.Early)), (Attribute) PropertyValueEditor.CreateEditorAttribute(typeof (ElementBindingPickerPropertyValueEditor)), (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_EventTriggerBase_SourceObject));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.BehaviorEventTriggerBase, "SourceName", (Attribute) new PropertyOrderAttribute(PropertyOrder.CreateBefore(PropertyOrder.Early)), (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_EventTriggerBase_SourceName), (Attribute) PropertyValueEditor.CreateEditorAttribute(typeof (BehaviorElementPickerPropertyValueEditor)), (Attribute) DesignSurfaceMetadata.SDKVersionedAttribute.CreateMinimumVersionedAttribute((Attribute) new EditorBrowsableAttribute(EditorBrowsableState.Advanced), DesignSurfaceMetadata.SDKVersion.SDK4));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.BehaviorEventTrigger, "EventName", (Attribute) PropertyValueEditor.CreateEditorAttribute(typeof (EventPickerPropertyValueEditor)), (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_EventTrigger_EventName));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.InvokeCommandAction, (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_InvokeCommandAction), (Attribute) DesignSurfaceMetadata.SDKVersionedAttribute.CreateMaxVersionedAttribute((Attribute) new ToolboxBrowsableAttribute(false), DesignSurfaceMetadata.SDKVersion.SDK3));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.InvokeCommandAction, "CommandParameter", (Attribute) new TypeConverterAttribute(typeof (StringConverter)), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties), (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_InvokeCommandAction_CommandParameter), (Attribute) new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.InvokeCommandAction, "Command", (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties), (Attribute) PropertyValueEditor.CreateEditorAttribute(typeof (PropertyBindingPickerPropertyValueEditor)), (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_InvokeCommandAction_Command));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.InvokeCommandAction, "CommandName", (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties), (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_InvokeCommandAction_CommandName), (Attribute) new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.InvokeCommandAction, (Attribute) new DefaultBindingPropertyAttribute("Command"));
      return assemblyInformation;
    }

    private static DesignSurfaceMetadata.AssemblyInformation CreateInteractionsAssemblyInformation()
    {
      DesignSurfaceMetadata.AssemblyInformation assemblyInformation = new DesignSurfaceMetadata.AssemblyInformation("Microsoft.Expression.Interactions");
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.ChangePropertyAction, (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_ChangePropertyAction));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.ChangePropertyAction, "Value", (Attribute) new BrowsableAttribute(false), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.ChangePropertyAction, "PropertyName", (Attribute) PropertyValueEditor.CreateEditorAttribute(typeof (PropertyPickerPropertyValueEditor)), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties), (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_ChangePropertyAction_PropertyName));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.ChangePropertyAction, "Duration", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_ChangePropertyAction_Duration), (Attribute) new CategoryAttribute(CategoryNames.CategoryAnimationProperties));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.ChangePropertyAction, "Ease", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_ChangePropertyAction_Ease), (Attribute) new CategoryAttribute(CategoryNames.CategoryAnimationProperties));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.ChangePropertyAction, "Increment", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_ChangePropertyAction_Increment), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.StoryboardAction, "Storyboard", (Attribute) PropertyValueEditor.CreateEditorAttribute(typeof (StoryboardPickerPropertyValueEditor)));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.StoryboardTrigger, "Storyboard", (Attribute) PropertyValueEditor.CreateEditorAttribute(typeof (StoryboardPickerPropertyValueEditor)));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.StoryboardCompletedTrigger, "Storyboard", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_StoryboardCompletedTrigger_Storyboard));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.KeyTrigger, "ActiveOnFocus", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_KeyTrigger_ActiveOnFocus));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.KeyTrigger, "FiredOn", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_KeyTrigger_FiredOn));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.KeyTrigger, "Modifiers", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_KeyTrigger_Modifiers));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.KeyTrigger, "Key", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_KeyTrigger_Key));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.ControlStoryboardAction, (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_ControlStoryboardAction));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.ControlStoryboardAction, "ControlStoryboardOption", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_ControlStoryboardAction_ControlStoryboardOption), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.ControlStoryboardAction, "Storyboard", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_ControlStoryboardAction_Storyboard), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.GoToStateAction, (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_GoToStateAction));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.GoToStateAction, "StateName", (Attribute) PropertyValueEditor.CreateEditorAttribute(typeof (StatePickerPropertyValueEditor)), (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_GoToStateAction_StateName), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.GoToStateAction, "UseTransitions", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_GoToStateAction_UseTransitions), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.HyperlinkAction, (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_HyperlinkAction));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.HyperlinkAction, "Path", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_HyperlinkAction_Path), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.HyperlinkAction, "NavigateUri", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_HyperlinkAction_NavigateURI), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.HyperlinkAction, "TargetWindow", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_HyperlinkAction_TargetWindow), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.LaunchUriOrFileAction, (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_LaunchURLOrFileAction));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.LaunchUriOrFileAction, "Path", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_LaunchURLOrFileAction_Path), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.RemoveElementAction, (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_RemoveElementAction));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.FluidMoveSetTagBehavior, (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_FluidMoveSetTagBehavior));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.FluidMoveBehaviorBase, "AppliesTo", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_FluidMoveBehavior_AppliesTo), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.FluidMoveBehaviorBase, "IsActive", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_FluidMoveBehavior_IsActive), (Attribute) new EditorBrowsableAttribute(EditorBrowsableState.Advanced), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.FluidMoveSetTagBehavior, "Tag", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_FluidMoveSetTagBehavior_Tag), (Attribute) new CategoryAttribute(CategoryNames.CategoryTagProperties));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.FluidMoveSetTagBehavior, "TagPath", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_FluidMoveSetTagBehavior_TagPath), (Attribute) new EditorBrowsableAttribute(EditorBrowsableState.Advanced), (Attribute) new CategoryAttribute(CategoryNames.CategoryTagProperties));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.FluidMoveBehavior, "Tag", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_FluidMoveBehavior_Tag), (Attribute) new CategoryAttribute(CategoryNames.CategoryTagProperties));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.FluidMoveBehavior, "TagPath", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_FluidMoveBehavior_TagPath), (Attribute) new EditorBrowsableAttribute(EditorBrowsableState.Advanced), (Attribute) new CategoryAttribute(CategoryNames.CategoryTagProperties));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.FluidMoveBehavior, (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_FluidMoveBehavior));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.FluidMoveBehavior, "Duration", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_FluidMoveBehavior_Duration), (Attribute) new CategoryAttribute(CategoryNames.CategoryAnimationProperties));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.FluidMoveBehavior, "EaseX", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_FluidMoveBehavior_EaseX), (Attribute) new CategoryAttribute(CategoryNames.CategoryAnimationProperties));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.FluidMoveBehavior, "EaseY", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_FluidMoveBehavior_EaseY), (Attribute) new CategoryAttribute(CategoryNames.CategoryAnimationProperties));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.FluidMoveBehavior, "FloatAbove", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_FluidMoveBehavior_FloatAbove), (Attribute) new CategoryAttribute(CategoryNames.CategoryAnimationProperties));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.FluidMoveBehavior, "InitialTag", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_FluidMoveBehavior_InitialTag), (Attribute) new CategoryAttribute(CategoryNames.CategoryTagProperties));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.FluidMoveBehavior, "InitialTagPath", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_FluidMoveBehavior_InitialTagPath), (Attribute) new EditorBrowsableAttribute(EditorBrowsableState.Advanced), (Attribute) new CategoryAttribute(CategoryNames.CategoryTagProperties));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.TimerTrigger, "EventName", (Attribute) new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.TimerTrigger, "TotalTicks", (Attribute) PropertyValueEditor.CreateEditorAttribute(typeof (TimerTriggerTickEditor)), (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_TimerTrigger_TotalTicks));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.TimerTrigger, "MillisecondsPerTick", (Attribute) new NumberRangesAttribute(new double?(0.0), new double?(), new double?(), new double?(), new bool?(false)), (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_TimerTrigger_MillisecondsPerTick));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.PlaySoundAction, (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_PlaySoundAction));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.PlaySoundAction, "Source", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_PlaySoundAction_Source), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.PlaySoundAction, "Volume", (Attribute) new NumberRangesAttribute(new double?(0.0), new double?(0.0), new double?(1.0), new double?(1.0), new bool?(false)), (Attribute) new NumberIncrementsAttribute(new double?(0.001), new double?(0.01), new double?(0.1)), (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_PlaySoundAction_Volume), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.MouseDragElementBehavior, (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_MouseDragElementBehavior));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.MouseDragElementBehavior, "ConstrainToParentBounds", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_MouseDragElementBehavior_ConstrainToParentBounds), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.MouseDragElementBehavior, "X", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_MouseDragElementBehavior_X), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties), (Attribute) new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.MouseDragElementBehavior, "Y", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_MouseDragElementBehavior_Y), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties), (Attribute) new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
      PropertyOrder default1 = PropertyOrder.Default;
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.CallMethodAction, (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_CallMethodAction), (Attribute) new DefaultBindingPropertyAttribute("TargetObject"));
      PropertyOrder after1;
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.CallMethodAction, "TargetObject", (Attribute) new PropertyOrderAttribute(after1 = PropertyOrder.CreateAfter(default1)), (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_CallMethodAction_Target), (Attribute) PropertyValueEditor.CreateEditorAttribute(typeof (ElementBindingPickerPropertyValueEditor)), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties));
      PropertyOrder after2;
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.CallMethodAction, "MethodName", (Attribute) new PropertyOrderAttribute(after2 = PropertyOrder.CreateAfter(after1)), (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_CallMethodAction_MethodName), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.DataTrigger, (Attribute) new DefaultBindingPropertyAttribute("Binding"));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.DataTrigger, "Binding", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_DataTrigger_Binding), (Attribute) PropertyValueEditor.CreateEditorAttribute(typeof (PropertyBindingPickerPropertyValueEditor)), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.DataTrigger, "Comparison", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_DataTrigger_Comparison), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.DataTrigger, "Value", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_DataTrigger_Value), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties), (Attribute) new TypeConverterAttribute(typeof (StringConverter)));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.SetDataStoreValueAction, "TargetObject", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_SetDataStoreValueAction_TargetObject), (Attribute) PropertyValueEditor.CreateEditorAttribute(typeof (DataStoreBindingObjectPropertyNameEditor)), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties), (Attribute) new DisplayNameAttribute(InteractivityStringTable.DisplayName_SetDataStoreValueAction_TargetObject));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.SetDataStoreValueAction, "PropertyName", (Attribute) new BrowsableAttribute(false));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.SetDataStoreValueAction, "TargetName", (Attribute) new BrowsableAttribute(false));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.SetDataStoreValueAction, "Value", (Attribute) new BrowsableAttribute(true), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties), (Attribute) PropertyValueEditor.CreateEditorAttribute(typeof (StringValueEditor)), (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_SetDataStoreValueAction_Value));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.SetDataStoreValueAction, "Increment", (Attribute) new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.SetDataStoreValueAction, "Duration", (Attribute) new BrowsableAttribute(false));
      PropertyOrder default2 = PropertyOrder.Default;
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.DataStateBehavior, (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_DataStateBehavior), (Attribute) new DefaultBindingPropertyAttribute("Binding"));
      PropertyOrder after3;
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.DataStateBehavior, "Binding", (Attribute) new PropertyOrderAttribute(after3 = PropertyOrder.CreateAfter(default2)), (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_DataStateBehavior_Binding), (Attribute) PropertyValueEditor.CreateEditorAttribute(typeof (PropertyBindingPickerPropertyValueEditor)), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties));
      PropertyOrder after4;
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.DataStateBehavior, "Value", (Attribute) new PropertyOrderAttribute(after4 = PropertyOrder.CreateAfter(after3)), (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_DataStateBehavior_Value), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties), (Attribute) new TypeConverterAttribute(typeof (StringConverter)));
      PropertyOrder after5;
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.DataStateBehavior, "TrueState", (Attribute) new PropertyOrderAttribute(after5 = PropertyOrder.CreateAfter(after4)), (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_DataStateBehavior_TrueState), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties), (Attribute) PropertyValueEditor.CreateEditorAttribute(typeof (StatePickerPropertyValueEditor)));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.DataStateBehavior, "FalseState", (Attribute) new PropertyOrderAttribute(after2 = PropertyOrder.CreateAfter(after5)), (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_DataStateBehavior_FalseState), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties), (Attribute) PropertyValueEditor.CreateEditorAttribute(typeof (StatePickerPropertyValueEditor)));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.PropertyChangedTrigger, (Attribute) new DefaultBindingPropertyAttribute("Binding"));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.PropertyChangedTrigger, "Binding", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_PropertyChangedTrigger_Binding), (Attribute) PropertyValueEditor.CreateEditorAttribute(typeof (PropertyBindingPickerPropertyValueEditor)));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.DataStoreChangedTrigger, "Binding", (Attribute) new DisplayNameAttribute(InteractivityStringTable.DisplayName_DataStoreChangeTrigger_Binding), (Attribute) PropertyValueEditor.CreateEditorAttribute(typeof (DataStoreBindingObjectEditor)), (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_DataStoreChangeTrigger_Property));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.ComparisonCondition, "LeftOperand", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_ComparisonCondition_LeftOperand));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.ComparisonCondition, "Operator", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_ComparisonCondition_Operator));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.ComparisonCondition, "Operator", (Attribute) new DisplayNameAttribute(InteractivityStringTable.DisplayName_ComparisonCondition_Operator));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.ComparisonCondition, "RightOperand", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_ComparisonCondition_RightOperand));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.ConditionBehavior, (Attribute) new ToolboxBrowsableAttribute(false));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.TranslateZoomRotateBehavior, (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_TranslateZoomRotateBehavior));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.TranslateZoomRotateBehavior, "RotationalFriction", (Attribute) new NumberIncrementsAttribute(new double?(0.001), new double?(0.01), new double?(0.1)), (Attribute) new NumberRangesAttribute(new double?(0.0), new double?(0.0), new double?(1.0), new double?(1.0), new bool?(false)));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.TranslateZoomRotateBehavior, "TranslateFriction", (Attribute) new NumberIncrementsAttribute(new double?(0.001), new double?(0.01), new double?(0.1)), (Attribute) new NumberRangesAttribute(new double?(0.0), new double?(0.0), new double?(1.0), new double?(1.0), new bool?(false)));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.TranslateZoomRotateBehavior, "MinimumScale", (Attribute) new NumberIncrementsAttribute(new double?(0.01), new double?(0.1), new double?(1.0)), (Attribute) new NumberRangesAttribute(new double?(0.0), new double?(0.0), new double?(), new double?(), new bool?(false)));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.TranslateZoomRotateBehavior, "MaximumScale", (Attribute) new NumberIncrementsAttribute(new double?(0.01), new double?(0.1), new double?(1.0)), (Attribute) new NumberRangesAttribute(new double?(0.0), new double?(0.0), new double?(), new double?(), new bool?(false)));
      PropertyOrder before = PropertyOrder.CreateBefore(PropertyOrder.Early);
      PropertyOrder after6;
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.TranslateZoomRotateBehavior, "SupportedGestures", (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties), (Attribute) new PropertyOrderAttribute(after6 = PropertyOrder.CreateAfter(before)));
      PropertyOrder after7;
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.TranslateZoomRotateBehavior, "ConstrainToParentBounds", (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties), (Attribute) new PropertyOrderAttribute(after7 = PropertyOrder.CreateAfter(after6)));
      PropertyOrder after8;
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.TranslateZoomRotateBehavior, "MaximumScale", (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties), (Attribute) new PropertyOrderAttribute(after8 = PropertyOrder.CreateAfter(after7)));
      PropertyOrder after9;
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.TranslateZoomRotateBehavior, "MinimumScale", (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties), (Attribute) new PropertyOrderAttribute(after9 = PropertyOrder.CreateAfter(after8)));
      PropertyOrder after10;
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.TranslateZoomRotateBehavior, "RotationalFriction", (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties), (Attribute) new PropertyOrderAttribute(after10 = PropertyOrder.CreateAfter(after9)));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.TranslateZoomRotateBehavior, "TranslateFriction", (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties), (Attribute) new PropertyOrderAttribute(after2 = PropertyOrder.CreateAfter(after10)));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.TranslateZoomRotateBehavior, "SupportedGestures", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_TranslateZoomRotateBehavior_SupportedGestures));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.TranslateZoomRotateBehavior, "ConstrainToParentBounds", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_TranslateZoomRotateBehavior_ConstrainToParentBounds));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.TranslateZoomRotateBehavior, "MaximumScale", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_TranslateZoomRotateBehavior_MaximumScale));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.TranslateZoomRotateBehavior, "MinimumScale", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_TranslateZoomRotateBehavior_MinimumScale));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.TranslateZoomRotateBehavior, "RotationalFriction", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_TranslateZoomRotateBehavior_RotationalFriction));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.TranslateZoomRotateBehavior, "TranslateFriction", (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_TranslateZoomRotateBehavior_TranslateFriction));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.TransitionEffect, "Progress", (Attribute) new BrowsableAttribute(false));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.TransitionEffect, "Input", (Attribute) new BrowsableAttribute(false));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.TransitionEffect, "OldImage", (Attribute) new BrowsableAttribute(false));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.TransitionEffect, (Attribute) new KnownUnreferencedTypeAttribute(KnownUnreferencedType.TransitionEffect));
      return assemblyInformation;
    }

    private static DesignSurfaceMetadata.AssemblyInformation CreateSketchFlowInteractivityAssemblyInformation()
    {
      DesignSurfaceMetadata.AssemblyInformation assemblyInformation = new DesignSurfaceMetadata.AssemblyInformation("Microsoft.Expression.Prototyping.Interactivity");
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.ActivateStateAction, (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_ActivateStateAction));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.ActivateStateAction, "TargetScreen", (Attribute) PropertyValueEditor.CreateEditorAttribute(typeof (ComposedScreenPickerPropertyValueEditor)), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties), (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_ActivateStateAction_TargetScreen));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.ActivateStateAction, "TargetState", (Attribute) PropertyValueEditor.CreateEditorAttribute(typeof (SketchFlowStatePickerPropertyValueEditor)), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties), (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_ActivateStateAction_TargetState));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.NavigateBackAction, (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_NavigateBackAction));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.NavigateForwardAction, (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_NavigateForwardAction));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.NavigateToScreenAction, (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_NavigateToScreenAction));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.NavigateToScreenAction, "TargetScreen", (Attribute) PropertyValueEditor.CreateEditorAttribute(typeof (NavigableScreenPickerPropertyValueEditor)), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties), (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_NavigateToScreenAction_TargetScreen));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.PlaySketchFlowAnimationAction, (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_PlaySketchFlowAnimationAction));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.PlaySketchFlowAnimationAction, "TargetScreen", (Attribute) PropertyValueEditor.CreateEditorAttribute(typeof (ComposedScreenPickerPropertyValueEditor)), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties), (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_PlaySketchFlowAnimationAction_TargetScreen));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.PlaySketchFlowAnimationAction, "SketchFlowAnimation", (Attribute) PropertyValueEditor.CreateEditorAttribute(typeof (SketchFlowAnimationPickerPropertyValueEditor)), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties), (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_PlaySketchFlowAnimationAction_SketchFlowAnimation));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.NavigationMenuAction, (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_NavigationMenuAction));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.NavigationMenuAction, "TargetScreen", (Attribute) PropertyValueEditor.CreateEditorAttribute(typeof (NavigableScreenPickerPropertyValueEditor)), (Attribute) new PropertyOrderAttribute(PropertyOrder.Early), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties), (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_NavigationMenuAction_TargetScreen));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.NavigationMenuAction, "ActiveState", (Attribute) PropertyValueEditor.CreateEditorAttribute(typeof (StatePickerPropertyValueEditor)), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties), (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_NavigationMenuAction_ActiveState));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.NavigationMenuAction, "InactiveState", (Attribute) PropertyValueEditor.CreateEditorAttribute(typeof (StatePickerPropertyValueEditor)), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties), (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_NavigationMenuAction_InactiveState));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.SketchFlowAnimationTrigger, (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_SketchFlowAnimationTrigger));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.SketchFlowAnimationTrigger, "SketchFlowAnimation", (Attribute) PropertyValueEditor.CreateEditorAttribute(typeof (SketchFlowAnimationPickerPropertyValueEditor)), (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties), (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_SketchFlowAnimationTrigger_SketchFlowAnimation));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.SketchFlowAnimationTrigger, "FiredOn", (Attribute) new CategoryAttribute(CategoryNames.CategoryCommonProperties), (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_SketchFlowAnimationTrigger_FiredOn));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.RemoveItemInListBoxAction, (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_RemoveItemInListBoxAction));
      return assemblyInformation;
    }

    private static DesignSurfaceMetadata.AssemblyInformation CreateSketchControlsAssemblyInformation()
    {
      DesignSurfaceMetadata.AssemblyInformation assemblyInformation = new DesignSurfaceMetadata.AssemblyInformation("Microsoft.Expression.Prototyping.SketchControls");
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.SketchRectangleSL, (Attribute) new ToolboxBrowsableAttribute(false));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.SketchCircleSL, (Attribute) new ToolboxBrowsableAttribute(false));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.SketchBorderSL, (Attribute) new ToolboxBrowsableAttribute(false));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.SketchBorderUC, (Attribute) new ToolboxBrowsableAttribute(false));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.SketchPath, (Attribute) new ToolboxBrowsableAttribute(false));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.SketchRectangleUC, (Attribute) new ToolboxBrowsableAttribute(false));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.SketchCircleUC, (Attribute) new ToolboxBrowsableAttribute(false));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.SketchBaseSL, (Attribute) new KnownUnreferencedTypeAttribute(KnownUnreferencedType.SketchControl));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.SketchBaseSL, (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_SketchBase));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.SketchBaseSL, "SegmentLength", (Attribute) new EditorBrowsableAttribute(EditorBrowsableState.Never));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.SketchBaseSL, "ExtensionLength", (Attribute) new EditorBrowsableAttribute(EditorBrowsableState.Never));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.SketchBaseSL, "ExtensionVariance", (Attribute) new EditorBrowsableAttribute(EditorBrowsableState.Never));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.SketchBaseSL, "SegmentVariance", (Attribute) new EditorBrowsableAttribute(EditorBrowsableState.Never));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.SketchBaseSL, "SegmentOffset", (Attribute) new EditorBrowsableAttribute(EditorBrowsableState.Never));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.SketchBaseSL, "StrokeWidth", (Attribute) new CategoryAttribute(CategoryNames.CategoryAppearance), (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_SketchBase_StrokeWidth));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.SketchBorderSL, (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_SketchBorder));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.SketchCircleSL, (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_SketchCircle));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.SketchCircleSL, "Segments", (Attribute) new EditorBrowsableAttribute(EditorBrowsableState.Never));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.SketchRectangleSL, (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_SketchRectangle));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.SketchBase, (Attribute) new KnownUnreferencedTypeAttribute(KnownUnreferencedType.SketchControl));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.SketchBase, (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_SketchBase));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.SketchBase, "SegmentLength", (Attribute) new EditorBrowsableAttribute(EditorBrowsableState.Never));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.SketchBase, "ExtensionLength", (Attribute) new EditorBrowsableAttribute(EditorBrowsableState.Never));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.SketchBase, "ExtensionVariance", (Attribute) new EditorBrowsableAttribute(EditorBrowsableState.Never));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.SketchBase, "SegmentVariance", (Attribute) new EditorBrowsableAttribute(EditorBrowsableState.Never));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.SketchBase, "SegmentOffset", (Attribute) new EditorBrowsableAttribute(EditorBrowsableState.Never));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.SketchBase, "StrokeWidth", (Attribute) new CategoryAttribute(CategoryNames.CategoryAppearance), (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_SketchBase_StrokeWidth));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.SketchBorderUC, (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_SketchBorder));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.SketchCircleUC, (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_SketchCircle));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.SketchCircleUC, "Segments", (Attribute) new EditorBrowsableAttribute(EditorBrowsableState.Never));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.SketchPath, (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_SketchPath));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.SketchPath, "Data", (Attribute) new EditorBrowsableAttribute(EditorBrowsableState.Never));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.SketchRectangleUC, (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_SketchRectangle));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.SketchRectangleUC, "Radius", (Attribute) new CategoryAttribute(CategoryNames.CategoryAppearance), (Attribute) new DescriptionAttribute(InteractivityStringTable.Description_SketchRectangle_Radius));
      return assemblyInformation;
    }

    private static DesignSurfaceMetadata.AssemblyInformation CreateDrawingAssemblyInformation()
    {
      DesignSurfaceMetadata.AssemblyInformation assemblyInformation = new DesignSurfaceMetadata.AssemblyInformation("Microsoft.Expression.Drawing");
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.PrimitiveShape, (Attribute) new KnownUnreferencedTypeAttribute(KnownUnreferencedType.PrimitiveShape));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.CompositeShape, (Attribute) new KnownUnreferencedTypeAttribute(KnownUnreferencedType.CompositeShape));
      assemblyInformation.AddTypeAttributes(ProjectNeutralTypes.CompositeContentShape, (Attribute) new KnownUnreferencedTypeAttribute(KnownUnreferencedType.CompositeContentShape));
      assemblyInformation.AddPropertyAttributes(ProjectNeutralTypes.Arc, "ArcThickness", (Attribute) new EditorAttribute(typeof (ArcThicknessEditor), typeof (ArcThicknessEditor)));
      return assemblyInformation;
    }

    private static AttributeTableBuilder BuildAttributeTable(Assembly assembly)
    {
      AttributeTableBuilder attributeTableBuilder = (AttributeTableBuilder) null;
      DesignSurfaceMetadata.AssemblyInformation assemblyInformation = (DesignSurfaceMetadata.AssemblyInformation) null;
      AssemblyName assemblyName = AssemblyHelper.GetAssemblyName(assembly);
      if (assembly != (Assembly) null && DesignSurfaceMetadata.assemblyInformation.TryGetValue(assemblyName.Name, out assemblyInformation) && assemblyInformation.HasTypes)
      {
        Type[] types;
        try
        {
          types = assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
          return (AttributeTableBuilder) null;
        }
        DesignSurfaceMetadata.SDKVersion assemblyVersion = DesignSurfaceMetadata.SDKVersion.GetVersionFromAssembly(assembly);
        attributeTableBuilder = new AttributeTableBuilder();
        foreach (Type type in types)
        {
          DesignSurfaceMetadata.TypeInformation typeInformation;
          if (assemblyInformation.TryGetTypeInformation(type.FullName, out typeInformation))
          {
            IReadOnlyCollection<Attribute> typeAttributes = (IReadOnlyCollection<Attribute>) typeInformation.GetAttributesForVersion(assemblyVersion);
            if (typeAttributes.Count > 0)
              attributeTableBuilder.AddCallback(type, (AttributeCallback) (builder => builder.AddCustomAttributes(Enumerable.ToArray<Attribute>((IEnumerable<Attribute>) typeAttributes))));
            if (typeInformation.HasProperties)
            {
              foreach (PropertyInfo propertyInfo1 in type.GetProperties())
              {
                DesignSurfaceMetadata.PropertyInformation propertyInfo;
                if (typeInformation.TryGetPropertyInformation(propertyInfo1.Name, out propertyInfo))
                  attributeTableBuilder.AddCallback(type, (AttributeCallback) (builder => builder.AddCustomAttributes(propertyInfo.PropertyName, Enumerable.ToArray<Attribute>((IEnumerable<Attribute>) propertyInfo.GetAttributesForVersion(assemblyVersion)))));
              }
            }
          }
        }
      }
      return attributeTableBuilder;
    }

    public static void InitializeMetadata()
    {
      foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        DesignSurfaceMetadata.InitializeMetadata(assembly);
    }

    private static void InitializeMetadata(Assembly assembly)
    {
      AssemblyName assemblyName = AssemblyHelper.GetAssemblyName(assembly);
      if (DesignSurfaceMetadata.HandledMetadata.Contains(assemblyName.FullName))
        return;
      AttributeTableBuilder attributeTableBuilder = DesignSurfaceMetadata.BuildAttributeTable(assembly);
      if (attributeTableBuilder != null)
        MetadataStore.AddAttributeTable(attributeTableBuilder.CreateTable());
      DesignSurfaceMetadata.HandledMetadata.Add(assemblyName.FullName);
    }

    public class SDKVersion : IComparable<DesignSurfaceMetadata.SDKVersion>
    {
      public static readonly DesignSurfaceMetadata.SDKVersion SDK3 = new DesignSurfaceMetadata.SDKVersion(new Version(2, 0, 5, 0), new Version(3, 5, 0, 0));
      public static readonly DesignSurfaceMetadata.SDKVersion SDK4 = new DesignSurfaceMetadata.SDKVersion(new Version(4, 0, 5, 0), new Version(4, 0, 0, 0));
      public static IEnumerable<DesignSurfaceMetadata.SDKVersion> KnownVersions = (IEnumerable<DesignSurfaceMetadata.SDKVersion>) new DesignSurfaceMetadata.SDKVersion[2]
      {
        DesignSurfaceMetadata.SDKVersion.SDK3,
        DesignSurfaceMetadata.SDKVersion.SDK4
      };
      private Version slAssemblyVersion;
      private Version wpfAssemblyVersion;

      public SDKVersion(Version slVersion, Version wpfVersion)
      {
        this.slAssemblyVersion = slVersion;
        this.wpfAssemblyVersion = wpfVersion;
      }

      public bool IsVersion(Version version)
      {
        if (!this.slAssemblyVersion.Equals(version))
          return this.wpfAssemblyVersion.Equals(version);
        return true;
      }

      public int CompareTo(DesignSurfaceMetadata.SDKVersion other)
      {
        int num1 = this.slAssemblyVersion.CompareTo(other.slAssemblyVersion);
        int num2 = this.wpfAssemblyVersion.CompareTo(other.wpfAssemblyVersion);
        if (num1 == 0 && num2 == 0)
          return 0;
        if (num1 >= 0 && num2 >= 0)
          return 1;
        if (num1 <= 0 && num2 <= 0)
          return -1;
        throw new InvalidOperationException("Unexpected SDKVersion comparison performed.");
      }

      public static DesignSurfaceMetadata.SDKVersion GetVersionFromAssembly(Assembly assembly)
      {
        AssemblyName assemblyName = AssemblyHelper.GetAssemblyName(assembly);
        foreach (DesignSurfaceMetadata.SDKVersion sdkVersion in DesignSurfaceMetadata.SDKVersion.KnownVersions)
        {
          if (sdkVersion.IsVersion(assemblyName.Version))
            return sdkVersion;
        }
        return (DesignSurfaceMetadata.SDKVersion) null;
      }
    }

    private sealed class SDKVersionedAttribute : Attribute
    {
      public Attribute Attribute { get; private set; }

      private DesignSurfaceMetadata.SDKVersion MinVersion { get; set; }

      private DesignSurfaceMetadata.SDKVersion MaxVersion { get; set; }

      private SDKVersionedAttribute(Attribute attribute, DesignSurfaceMetadata.SDKVersion minVersion, DesignSurfaceMetadata.SDKVersion maxVersion)
      {
        this.Attribute = attribute;
        this.MinVersion = minVersion;
        this.MaxVersion = maxVersion;
      }

      public static DesignSurfaceMetadata.SDKVersionedAttribute CreateMinimumVersionedAttribute(Attribute attribute, DesignSurfaceMetadata.SDKVersion minVersion)
      {
        return new DesignSurfaceMetadata.SDKVersionedAttribute(attribute, minVersion, (DesignSurfaceMetadata.SDKVersion) null);
      }

      public static DesignSurfaceMetadata.SDKVersionedAttribute CreateMaxVersionedAttribute(Attribute attribute, DesignSurfaceMetadata.SDKVersion maxVersion)
      {
        return new DesignSurfaceMetadata.SDKVersionedAttribute(attribute, (DesignSurfaceMetadata.SDKVersion) null, maxVersion);
      }

      public bool IsCompatibleWith(DesignSurfaceMetadata.SDKVersion version)
      {
        bool flag1 = true;
        bool flag2 = true;
        if (version == null)
          return true;
        if (this.MinVersion != null)
          flag1 = this.MinVersion.CompareTo(version) <= 0;
        if (this.MaxVersion != null)
          flag2 = this.MaxVersion.CompareTo(version) >= 0;
        if (flag1)
          return flag2;
        return false;
      }
    }

    private class PropertyInformation
    {
      private List<Attribute> attributes;

      public string PropertyName { get; private set; }

      public PropertyInformation(string propertyName, params Attribute[] attributes)
      {
        this.PropertyName = propertyName;
        this.attributes = new List<Attribute>();
        this.AddAttributes((IEnumerable<Attribute>) attributes);
      }

      public ReadOnlyList<Attribute> GetAttributesForVersion(DesignSurfaceMetadata.SDKVersion version)
      {
        List<Attribute> list = new List<Attribute>();
        foreach (Attribute attribute in this.attributes)
        {
          DesignSurfaceMetadata.SDKVersionedAttribute versionedAttribute = attribute as DesignSurfaceMetadata.SDKVersionedAttribute;
          if (versionedAttribute == null)
            list.Add(attribute);
          else if (versionedAttribute.IsCompatibleWith(version))
            list.Add(versionedAttribute.Attribute);
        }
        return new ReadOnlyList<Attribute>((IList<Attribute>) list);
      }

      public void AddAttributes(IEnumerable<Attribute> attributes)
      {
        this.attributes.AddRange(attributes);
      }
    }

    private class TypeInformation
    {
      private List<Attribute> attributes;

      public bool HasProperties
      {
        get
        {
          return Enumerable.Any<KeyValuePair<string, DesignSurfaceMetadata.PropertyInformation>>((IEnumerable<KeyValuePair<string, DesignSurfaceMetadata.PropertyInformation>>) this.PropertyInformation);
        }
      }

      private Dictionary<string, DesignSurfaceMetadata.PropertyInformation> PropertyInformation { get; set; }

      public TypeInformation(params Attribute[] attributes)
      {
        this.PropertyInformation = new Dictionary<string, DesignSurfaceMetadata.PropertyInformation>();
        this.attributes = new List<Attribute>();
        this.AddAttributes((IEnumerable<Attribute>) attributes);
      }

      public ReadOnlyList<Attribute> GetAttributesForVersion(DesignSurfaceMetadata.SDKVersion version)
      {
        List<Attribute> list = new List<Attribute>();
        foreach (Attribute attribute in this.attributes)
        {
          DesignSurfaceMetadata.SDKVersionedAttribute versionedAttribute = attribute as DesignSurfaceMetadata.SDKVersionedAttribute;
          if (versionedAttribute == null)
            list.Add(attribute);
          else if (versionedAttribute.IsCompatibleWith(version))
            list.Add(versionedAttribute.Attribute);
        }
        return new ReadOnlyList<Attribute>((IList<Attribute>) list);
      }

      public void AddPropertyInformation(DesignSurfaceMetadata.PropertyInformation propertyInformation)
      {
        this.PropertyInformation.Add(propertyInformation.PropertyName, propertyInformation);
      }

      public void AddAttributes(IEnumerable<Attribute> attributes)
      {
        this.attributes.AddRange(attributes);
      }

      public bool TryGetPropertyInformation(string propertyName, out DesignSurfaceMetadata.PropertyInformation propertyInformation)
      {
        return this.PropertyInformation.TryGetValue(propertyName, out propertyInformation);
      }
    }

    private class AssemblyInformation
    {
      public string AssemblyName { get; private set; }

      private Dictionary<string, DesignSurfaceMetadata.TypeInformation> TypeInformation { get; set; }

      public bool HasTypes
      {
        get
        {
          return Enumerable.Any<KeyValuePair<string, DesignSurfaceMetadata.TypeInformation>>((IEnumerable<KeyValuePair<string, DesignSurfaceMetadata.TypeInformation>>) this.TypeInformation);
        }
      }

      public AssemblyInformation(string assemblyName)
      {
        this.AssemblyName = assemblyName;
        this.TypeInformation = new Dictionary<string, DesignSurfaceMetadata.TypeInformation>();
      }

      public void AddTypeAttributes(ITypeId typeId, params Attribute[] attributes)
      {
        this.GetOrCreateTypeInformation(typeId).AddAttributes((IEnumerable<Attribute>) attributes);
      }

      public void AddPropertyAttributes(ITypeId typeId, string propertyName, params Attribute[] attributes)
      {
        DesignSurfaceMetadata.TypeInformation createTypeInformation = this.GetOrCreateTypeInformation(typeId);
        DesignSurfaceMetadata.PropertyInformation propertyInformation;
        if (!createTypeInformation.TryGetPropertyInformation(propertyName, out propertyInformation))
        {
          propertyInformation = new DesignSurfaceMetadata.PropertyInformation(propertyName, new Attribute[0]);
          createTypeInformation.AddPropertyInformation(propertyInformation);
        }
        propertyInformation.AddAttributes((IEnumerable<Attribute>) attributes);
      }

      public bool TryGetTypeInformation(string typeName, out DesignSurfaceMetadata.TypeInformation typeInformation)
      {
        return this.TypeInformation.TryGetValue(typeName, out typeInformation);
      }

      private DesignSurfaceMetadata.TypeInformation GetOrCreateTypeInformation(ITypeId typeId)
      {
        DesignSurfaceMetadata.TypeInformation typeInformation;
        if (!this.TypeInformation.TryGetValue(typeId.FullName, out typeInformation))
        {
          typeInformation = new DesignSurfaceMetadata.TypeInformation(new Attribute[0]);
          this.TypeInformation.Add(typeId.FullName, typeInformation);
        }
        return typeInformation;
      }
    }
  }
}
