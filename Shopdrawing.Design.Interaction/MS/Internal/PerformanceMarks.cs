// Decompiled with JetBrains decompiler
// Type: MS.Internal.PerformanceMarks
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Internal.Performance;

namespace MS.Internal
{
  internal static class PerformanceMarks
  {
    public static readonly PerformanceMark SelectAll = new PerformanceMark("Cider_SelectAll", CodeMarkerEvent.perfCiderSelectAllBegin, CodeMarkerEvent.perfCiderSelectAllEnd);
    public static readonly PerformanceMark SelectionChange = new PerformanceMark("Cider_SelectionChange", CodeMarkerEvent.perfCiderSelectionChangeBegin, CodeMarkerEvent.perfCiderSelectionChangeEnd);
    public static readonly PerformanceMark SelectNext = new PerformanceMark("Cider_SelectNext", CodeMarkerEvent.perfCiderSelectNextBegin, CodeMarkerEvent.perfCiderSelectNextEnd);
    public static readonly PerformanceMark SelectPrevious = new PerformanceMark("Cider_SelectPrevious", CodeMarkerEvent.perfCiderSelectPreviousBegin, CodeMarkerEvent.perfCiderSelectPreviousEnd);
    public static readonly PerformanceMark MarqueeSelect = new PerformanceMark("Cider_MarqueeSelect", CodeMarkerEvent.perfCiderMarqueeSelectBegin, CodeMarkerEvent.perfCiderMarqueeSelectEnd);
    public static readonly PerformanceMark AdornerArrange = new PerformanceMark("Cider_AdornerArrange", CodeMarkerEvent.perfCiderAdornerArrangeOverrideBegin, CodeMarkerEvent.perfCiderAdornerArrangeOverrideEnd);
    public static readonly PerformanceMark AdornerMeasure = new PerformanceMark("Cider_AdornerMeasure", CodeMarkerEvent.perfCiderAdornerMeasureOverrideBegin, CodeMarkerEvent.perfCiderAdornerMeasureOverrideEnd);
    public static readonly PerformanceMark DesignerReload = new PerformanceMark("Cider_DesignerReload", CodeMarkerEvent.perfCiderDesignerReloadBegin, CodeMarkerEvent.perfCiderDesignerReloadEnd);
    public static readonly PerformanceMark OpenXAML = new PerformanceMark("Cider_OpenXAML", CodeMarkerEvent.perfCiderOpenXAMLBegin, CodeMarkerEvent.perfCiderOpenXAMLEnd);
    public static readonly PerformanceMark AppDomainReload = new PerformanceMark("Cider_AppDomainReload", CodeMarkerEvent.perfCiderAppDomainReloadBegin, CodeMarkerEvent.perfCiderAppDomainReloadEnd);
    public static readonly PerformanceMark AppDomainUnload = new PerformanceMark("Cider_AppDomainUnload", CodeMarkerEvent.perfCiderAppDomainUnloadBegin, CodeMarkerEvent.perfCiderAppDomainUnloadEnd);
    public static readonly PerformanceMark PropertyChange = new PerformanceMark("Cider_PropertyChange", CodeMarkerEvent.perfCiderPropertyChangeBegin, CodeMarkerEvent.perfCiderPropertyChangeEnd);
    public static readonly PerformanceMark CreateControl = new PerformanceMark("Cider_CreateControl", CodeMarkerEvent.perfCiderCreateControlBegin, CodeMarkerEvent.perfCiderCreateControlEnd);
    public static readonly PerformanceMark CutControl = new PerformanceMark("Cider_CutControl", CodeMarkerEvent.perfCiderCutBegin, CodeMarkerEvent.perfCiderCutEnd);
    public static readonly PerformanceMark CopyControl = new PerformanceMark("Cider_CopyControl", CodeMarkerEvent.perfCiderCopyBegin, CodeMarkerEvent.perfCiderCopyEnd);
    public static readonly PerformanceMark PasteControl = new PerformanceMark("Cider_PasteControl", CodeMarkerEvent.perfCiderPasteBegin, CodeMarkerEvent.perfCiderPasteEnd);
    public static readonly PerformanceMark ReparentControl = new PerformanceMark("Cider_ReparentControl", CodeMarkerEvent.perfCiderReparentBegin, CodeMarkerEvent.perfCiderReparentEnd);
    public static readonly PerformanceMark ResizeControl = new PerformanceMark("Cider_ResizeControl", CodeMarkerEvent.perfCiderResizeStart, CodeMarkerEvent.perfCiderResizeEnd);
    public static readonly PerformanceMark PropertyInspectorPopupOpen = new PerformanceMark("Cider_PropertyInspectorPopupOpen", CodeMarkerEvent.perfCiderPropertyInspectorPopupOpen, CodeMarkerEvent.perfCiderPropertyInspectorPopupOpen);
    public static readonly PerformanceMark PropertyInspectorPopupClose = new PerformanceMark("Cider_PropertyInspectorPopupClose", CodeMarkerEvent.perfCiderPropertyInspectorPopupClose, CodeMarkerEvent.perfCiderPropertyInspectorPopupClose);
    public static readonly PerformanceMark PropertyMarkerConvert = new PerformanceMark("Cider_PropertyMarkerConvert", CodeMarkerEvent.perfCiderPropertyMarkerConvertBegin, CodeMarkerEvent.perfCiderPropertyMarkerConvertEnd);
    public static readonly PerformanceMark PropertyValueGet = new PerformanceMark("Cider_PropertyValueGet", CodeMarkerEvent.perfCiderModelPropertyValueGetBegin, CodeMarkerEvent.perfCiderModelPropertyValueGetEnd);
    public static readonly PerformanceMark PropertyInspectorSelectionChange = new PerformanceMark("Cider_PropertyInspectorSelectionChange", CodeMarkerEvent.perfCiderPropertyInspectorSelectionChangeBegin, CodeMarkerEvent.perfCiderPropertyInspectorSelectionChangeEnd);
    public static readonly PerformanceMark PropertyInspectorInitialization = new PerformanceMark("Cider_PropertyInspectorInitialization", CodeMarkerEvent.perfCiderPropertyInspectorInitializeBegin, CodeMarkerEvent.perfCiderPropertyInspectorInitializeEnd);
    public static readonly PerformanceMark SwitchToXAMLView = new PerformanceMark("Cider_SwitchToXAMLView", CodeMarkerEvent.perfCiderSwitchToXAMLViewBegin, CodeMarkerEvent.perfCiderSwitchToXAMLViewEnd);
    public static readonly PerformanceMark SwitchToDesignView = new PerformanceMark("Cider_SwitchToDesignView", CodeMarkerEvent.perfCiderSwitchToDesignViewBegin, CodeMarkerEvent.perfCiderSwitchToDesignViewEnd);
    public static readonly PerformanceMark ActivateView = new PerformanceMark("Cider_ActivateView", CodeMarkerEvent.perfCiderActivateViewBegin, CodeMarkerEvent.perfCiderActivateViewEnd);
    public static readonly PerformanceMark ToolboxAutoUpdate = new PerformanceMark("Cider_ToolboxAutoUpdate", CodeMarkerEvent.perfCiderAutoToolboxUpdateBegin, CodeMarkerEvent.perfCiderAutoToolboxUpdateEnd);
    public static readonly PerformanceMark ListMembers = new PerformanceMark("Cider_ListMembers", CodeMarkerEvent.perfCiderListMembersBegin, CodeMarkerEvent.perfCiderListMembersEnd);
    public static readonly PerformanceMark StatementCompletion = new PerformanceMark("Cider_StatementCompletion", CodeMarkerEvent.perfCiderStatementCompletionBegin, CodeMarkerEvent.perfCiderStatementCompletionEnd);
  }
}
