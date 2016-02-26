using System;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public static class ProjectNeutralTypes
	{
		public readonly static ITypeId VisualStateManager;

		public readonly static ITypeId VisualStateGroup;

		public readonly static ITypeId VisualState;

		public readonly static ITypeId VisualTransition;

		public readonly static ITypeId IEasingFunction;

		public readonly static ITypeId TemplateVisualStateAttribute;

		public readonly static ITypeId AutoCompleteBox;

		public readonly static ITypeId ValidationSummary;

		public readonly static ITypeId Calendar;

		public readonly static ITypeId CalendarButton;

		public readonly static ITypeId CalendarDayButton;

		public readonly static ITypeId CalendarItem;

		public readonly static ITypeId DatePicker;

		public readonly static ITypeId GridSplitter;

		public readonly static ITypeId HeaderedItemsControl;

		public readonly static ITypeId HierarchicalDataTemplate;

		public readonly static ITypeId TabControl;

		public readonly static ITypeId TabItem;

		public readonly static ITypeId TabPanel;

		public readonly static ITypeId TreeView;

		public readonly static ITypeId TreeViewItem;

		public readonly static ITypeId DataGrid;

		public readonly static ITypeId DataGridBoundColumn;

		public readonly static ITypeId DataGridCell;

		public readonly static ITypeId DataGridCellsPresenter;

		public readonly static ITypeId DataGridCheckBoxColumn;

		public readonly static ITypeId DataGridColumn;

		public readonly static ITypeId DataGridColumnCollection;

		public readonly static ITypeId DataGridColumnHeader;

		public readonly static ITypeId DataGridColumnHeadersPresenter;

		public readonly static ITypeId DataGridDetailsPresenter;

		public readonly static ITypeId DataGridFrozenGrid;

		public readonly static ITypeId DataGridRow;

		public readonly static ITypeId DataGridRowGroupHeader;

		public readonly static ITypeId DataGridRowHeader;

		public readonly static ITypeId DataGridRowsPresenter;

		public readonly static ITypeId DataGridTextColumn;

		public readonly static ITypeId DataGridTemplateColumn;

		public readonly static ITypeId DataGridLength;

		public readonly static ITypeId Frame;

		public readonly static ITypeId NumericUpDown;

		public readonly static ITypeId TimePicker;

		public readonly static ITypeId Picker;

		public readonly static ITypeId TimeUpDown;

		public readonly static ITypeId HeaderedContentControl;

		public readonly static ITypeId ListBoxDragDropTarget;

		public readonly static ITypeId TreeViewDragDropTarget;

		public readonly static ITypeId DataGridDragDropTarget;

		public readonly static ITypeId DataGridCellsPanel;

		public readonly static ITypeId DataGridHeaderBorder;

		public readonly static ITypeId DataGridHyperlinkColumn;

		public readonly static ITypeId DockPanel;

		public readonly static ITypeId Expander;

		public readonly static ITypeId Label;

		public readonly static ITypeId Viewbox;

		public readonly static ITypeId WrapPanel;

		public readonly static ITypeId Interaction;

		public readonly static ITypeId Behavior;

		public readonly static ITypeId BehaviorCollection;

		public readonly static ITypeId BehaviorTriggerBase;

		public readonly static ITypeId BehaviorEventTriggerBase;

		public readonly static ITypeId BehaviorTriggerCollection;

		public readonly static ITypeId BehaviorTriggerAction;

		public readonly static ITypeId BehaviorTargetedTriggerAction;

		public readonly static ITypeId BehaviorTriggerActionCollection;

		public readonly static ITypeId BehaviorEventTrigger;

		public readonly static ITypeId InvokeCommandAction;

		public readonly static ITypeId DefaultTriggerAttribute;

		public readonly static ITypeId TypeConstraintAttribute;

		public readonly static ITypeId CustomPropertyValueEditorAttribute;

		public readonly static ITypeId CustomPropertyValueEditor;

		public readonly static ITypeId ConditionBehavior;

		public readonly static ITypeId ConditionalExpression;

		public readonly static ITypeId ConditionCollection;

		public readonly static ITypeId ComparisonCondition;

		public readonly static ITypeId ForwardChaining;

		public readonly static ITypeId SetDataStoreValueAction;

		public readonly static ITypeId DataStoreChangedTrigger;

		public readonly static ITypeId PropertyChangedTrigger;

		public readonly static ITypeId ChangePropertyAction;

		public readonly static ITypeId CallMethodAction;

		public readonly static ITypeId TimerTrigger;

		public readonly static ITypeId FluidMoveBehavior;

		public readonly static ITypeId FluidMoveBehaviorBase;

		public readonly static ITypeId FluidMoveSetTagBehavior;

		public readonly static ITypeId StoryboardAction;

		public readonly static ITypeId ControlStoryboardAction;

		public readonly static ITypeId StoryboardTrigger;

		public readonly static ITypeId StoryboardCompletedTrigger;

		public readonly static ITypeId KeyTrigger;

		public readonly static ITypeId GoToStateAction;

		public readonly static ITypeId HyperlinkAction;

		public readonly static ITypeId LaunchUriOrFileAction;

		public readonly static ITypeId RemoveElementAction;

		public readonly static ITypeId PlaySoundAction;

		public readonly static ITypeId MouseDragElementBehavior;

		public readonly static ITypeId DataTrigger;

		public readonly static ITypeId DataStateBehavior;

		public readonly static ITypeId TranslateZoomRotateBehavior;

		public readonly static ITypeId ExtendedVisualStateManager;

		public readonly static ITypeId TransitionEffect;

		public readonly static ITypeId PrimitiveShape;

		public readonly static ITypeId CompositeShape;

		public readonly static ITypeId CompositeContentShape;

		public readonly static ITypeId GeometryEffect;

		public readonly static ITypeId RegularPolygon;

		public readonly static ITypeId Arc;

		public readonly static ITypeId BlockArrow;

		public readonly static ITypeId Callout;

		public readonly static ITypeId LineArrow;

		public readonly static ITypeId LayoutPath;

		public readonly static ITypeId Orientation;

		public readonly static ITypeId PathListBox;

		public readonly static ITypeId PathListBoxItem;

		public readonly static ITypeId PathPanel;

		public readonly static ITypeId LayoutPathCollection;

		public readonly static ITypeId ActivateStateAction;

		public readonly static ITypeId NavigateBackAction;

		public readonly static ITypeId NavigateForwardAction;

		public readonly static ITypeId NavigateToScreenAction;

		public readonly static ITypeId PlaySketchFlowAnimationAction;

		public readonly static ITypeId NavigationMenuAction;

		public readonly static ITypeId SketchFlowAnimationTrigger;

		public readonly static ITypeId RemoveItemInListBoxAction;

		public readonly static ITypeId SketchBaseSL;

		public readonly static ITypeId SketchBorderSL;

		public readonly static ITypeId SketchCircleSL;

		public readonly static ITypeId SketchRectangleSL;

		public readonly static ITypeId SketchBase;

		public readonly static ITypeId SketchBorderUC;

		public readonly static ITypeId SketchCircleUC;

		public readonly static ITypeId SketchPath;

		public readonly static ITypeId SketchRectangleUC;

		static ProjectNeutralTypes()
		{
			ProjectNeutralTypes.VisualStateManager = new ProjectNeutralTypeId("System.Windows.VisualStateManager", AssemblyGroup.VisualStateManager, new string[0]);
			ProjectNeutralTypes.VisualStateGroup = new ProjectNeutralTypeId("System.Windows.VisualStateGroup", AssemblyGroup.VisualStateManager, new string[0]);
			ProjectNeutralTypes.VisualState = new ProjectNeutralTypeId("System.Windows.VisualState", AssemblyGroup.VisualStateManager, new string[0]);
			ProjectNeutralTypes.VisualTransition = new ProjectNeutralTypeId("System.Windows.VisualTransition", AssemblyGroup.VisualStateManager, new string[0]);
			ProjectNeutralTypes.IEasingFunction = new ProjectNeutralTypeId("System.Windows.Media.Animation.IEasingFunction", AssemblyGroup.VisualStateManager, new string[0]);
			ProjectNeutralTypes.TemplateVisualStateAttribute = new ProjectNeutralTypeId("System.Windows.TemplateVisualStateAttribute", AssemblyGroup.VisualStateManager, new string[0]);
			string[] strArrays = new string[] { "Microsoft.Windows.Controls.AutoCompleteBox" };
			ProjectNeutralTypes.AutoCompleteBox = new ProjectNeutralTypeId("System.Windows.Controls.AutoCompleteBox", AssemblyGroup.ExtendedControlsInput, strArrays);
			string[] strArrays1 = new string[] { "System.Windows.Controls.ValidationSummary" };
			ProjectNeutralTypes.ValidationSummary = new ProjectNeutralTypeId("System.Windows.Controls.ValidationSummary", AssemblyGroup.ExtendedControlsDataInput, strArrays1);
			string[] strArrays2 = new string[] { "Microsoft.Windows.Controls.Calendar" };
			ProjectNeutralTypes.Calendar = new ProjectNeutralTypeId("System.Windows.Controls.Calendar", AssemblyGroup.ExtendedControls, strArrays2);
			string[] strArrays3 = new string[] { "Microsoft.Windows.Controls.CalendarButton" };
			ProjectNeutralTypes.CalendarButton = new ProjectNeutralTypeId("System.Windows.Controls.Primitives.CalendarButton", AssemblyGroup.ExtendedControls, strArrays3);
			string[] strArrays4 = new string[] { "Microsoft.Windows.Controls.CalendarDayButton" };
			ProjectNeutralTypes.CalendarDayButton = new ProjectNeutralTypeId("System.Windows.Controls.Primitives.CalendarDayButton", AssemblyGroup.ExtendedControls, strArrays4);
			string[] strArrays5 = new string[] { "Microsoft.Windows.Controls.CalendarItem" };
			ProjectNeutralTypes.CalendarItem = new ProjectNeutralTypeId("System.Windows.Controls.Primitives.CalendarItem", AssemblyGroup.ExtendedControls, strArrays5);
			string[] strArrays6 = new string[] { "Microsoft.Windows.Controls.DatePicker" };
			ProjectNeutralTypes.DatePicker = new ProjectNeutralTypeId("System.Windows.Controls.DatePicker", AssemblyGroup.ExtendedControls, strArrays6);
			ProjectNeutralTypes.GridSplitter = new ProjectNeutralTypeId("System.Windows.Controls.GridSplitter", AssemblyGroup.ExtendedControls, new string[0]);
			ProjectNeutralTypes.HeaderedItemsControl = new ProjectNeutralTypeId("System.Windows.Controls.HeaderedItemsControl", AssemblyGroup.ExtendedControls, new string[0]);
			ProjectNeutralTypes.HierarchicalDataTemplate = new ProjectNeutralTypeId("System.Windows.HierarchicalDataTemplate", AssemblyGroup.ExtendedControls, new string[0]);
			ProjectNeutralTypes.TabControl = new ProjectNeutralTypeId("System.Windows.Controls.TabControl", AssemblyGroup.ExtendedControls, new string[0]);
			ProjectNeutralTypes.TabItem = new ProjectNeutralTypeId("System.Windows.Controls.TabItem", AssemblyGroup.ExtendedControls, new string[0]);
			ProjectNeutralTypes.TabPanel = new ProjectNeutralTypeId("System.Windows.Controls.Primitives.TabPanel", AssemblyGroup.ExtendedControls, new string[0]);
			ProjectNeutralTypes.TreeView = new ProjectNeutralTypeId("System.Windows.Controls.TreeView", AssemblyGroup.ExtendedControls, new string[0]);
			ProjectNeutralTypes.TreeViewItem = new ProjectNeutralTypeId("System.Windows.Controls.TreeViewItem", AssemblyGroup.ExtendedControls, new string[0]);
			string[] strArrays7 = new string[] { "Microsoft.Windows.Controls.DataGrid" };
			ProjectNeutralTypes.DataGrid = new ProjectNeutralTypeId("System.Windows.Controls.DataGrid", AssemblyGroup.ExtendedControlsData, strArrays7);
			string[] strArrays8 = new string[] { "Microsoft.Windows.Controls.DataGridBoundColumn" };
			ProjectNeutralTypes.DataGridBoundColumn = new ProjectNeutralTypeId("System.Windows.Controls.DataGridBoundColumn", AssemblyGroup.ExtendedControlsData, strArrays8);
			string[] strArrays9 = new string[] { "Microsoft.Windows.Controls.DataGridCell" };
			ProjectNeutralTypes.DataGridCell = new ProjectNeutralTypeId("System.Windows.Controls.DataGridCell", AssemblyGroup.ExtendedControlsData, strArrays9);
			string[] strArrays10 = new string[] { "Microsoft.Windows.Controls.Primitives.DataGridCellsPresenter" };
			ProjectNeutralTypes.DataGridCellsPresenter = new ProjectNeutralTypeId("System.Windows.Controls.Primitives.DataGridCellsPresenter", AssemblyGroup.ExtendedControlsData, strArrays10);
			string[] strArrays11 = new string[] { "Microsoft.Windows.Controls.DataGridCheckBoxColumn" };
			ProjectNeutralTypes.DataGridCheckBoxColumn = new ProjectNeutralTypeId("System.Windows.Controls.DataGridCheckBoxColumn", AssemblyGroup.ExtendedControlsData, strArrays11);
			string[] strArrays12 = new string[] { "Microsoft.Windows.Controls.DataGridColumn" };
			ProjectNeutralTypes.DataGridColumn = new ProjectNeutralTypeId("System.Windows.Controls.DataGridColumn", AssemblyGroup.ExtendedControlsData, strArrays12);
			string[] strArrays13 = new string[] { "Microsoft.Windows.Controls.DataGridColumnCollection" };
			ProjectNeutralTypes.DataGridColumnCollection = new ProjectNeutralTypeId("System.Windows.Controls.DataGridColumnCollection", AssemblyGroup.ExtendedControlsData, strArrays13);
			string[] strArrays14 = new string[] { "Microsoft.Windows.Controls.Primitives.DataGridColumnHeader" };
			ProjectNeutralTypes.DataGridColumnHeader = new ProjectNeutralTypeId("System.Windows.Controls.Primitives.DataGridColumnHeader", AssemblyGroup.ExtendedControlsData, strArrays14);
			string[] strArrays15 = new string[] { "Microsoft.Windows.Controls.Primitives.DataGridColumnHeadersPresenter" };
			ProjectNeutralTypes.DataGridColumnHeadersPresenter = new ProjectNeutralTypeId("System.Windows.Controls.Primitives.DataGridColumnHeadersPresenter", AssemblyGroup.ExtendedControlsData, strArrays15);
			string[] strArrays16 = new string[] { "Microsoft.Windows.Controls.Primitives.DataGridDetailsPresenter" };
			ProjectNeutralTypes.DataGridDetailsPresenter = new ProjectNeutralTypeId("System.Windows.Controls.Primitives.DataGridDetailsPresenter", AssemblyGroup.ExtendedControlsData, strArrays16);
			string[] strArrays17 = new string[] { "Microsoft.Windows.Controls.Primitives.DataGridFrozenGrid" };
			ProjectNeutralTypes.DataGridFrozenGrid = new ProjectNeutralTypeId("System.Windows.Controls.Primitives.DataGridFrozenGrid", AssemblyGroup.ExtendedControlsData, strArrays17);
			string[] strArrays18 = new string[] { "Microsoft.Windows.Controls.DataGridRow" };
			ProjectNeutralTypes.DataGridRow = new ProjectNeutralTypeId("System.Windows.Controls.DataGridRow", AssemblyGroup.ExtendedControlsData, strArrays18);
			string[] strArrays19 = new string[] { "Microsoft.Windows.Controls.DataGridRowGroupHeader" };
			ProjectNeutralTypes.DataGridRowGroupHeader = new ProjectNeutralTypeId("System.Windows.Controls.DataGridRowGroupHeader", AssemblyGroup.ExtendedControlsData, strArrays19);
			string[] strArrays20 = new string[] { "Microsoft.Windows.Controls.Primitives.DataGridRowHeader" };
			ProjectNeutralTypes.DataGridRowHeader = new ProjectNeutralTypeId("System.Windows.Controls.Primitives.DataGridRowHeader", AssemblyGroup.ExtendedControlsData, strArrays20);
			string[] strArrays21 = new string[] { "Microsoft.Windows.Controls.Primitives.DataGridRowsPresenter" };
			ProjectNeutralTypes.DataGridRowsPresenter = new ProjectNeutralTypeId("System.Windows.Controls.Primitives.DataGridRowsPresenter", AssemblyGroup.ExtendedControlsData, strArrays21);
			string[] strArrays22 = new string[] { "Microsoft.Windows.Controls.DataGridTextColumn" };
			ProjectNeutralTypes.DataGridTextColumn = new ProjectNeutralTypeId("System.Windows.Controls.DataGridTextColumn", AssemblyGroup.ExtendedControlsData, strArrays22);
			string[] strArrays23 = new string[] { "Microsoft.Windows.Controls.DataGridTemplateColumn" };
			ProjectNeutralTypes.DataGridTemplateColumn = new ProjectNeutralTypeId("System.Windows.Controls.DataGridTemplateColumn", AssemblyGroup.ExtendedControlsData, strArrays23);
			string[] strArrays24 = new string[] { "Microsoft.Windows.Controls.DataGridLength" };
			ProjectNeutralTypes.DataGridLength = new ProjectNeutralTypeId("System.Windows.Controls.DataGridLength", AssemblyGroup.ExtendedControlsData, strArrays24);
			ProjectNeutralTypes.Frame = new ProjectNeutralTypeId("System.Windows.Controls.Frame", AssemblyGroup.ExtendedControlsNavigation, new string[0]);
			string[] strArrays25 = new string[] { "System.Windows.Controls.NumericUpDown" };
			ProjectNeutralTypes.NumericUpDown = new ProjectNeutralTypeId("System.Windows.Controls.NumericUpDown", AssemblyGroup.ExtendedControlsToolkit, strArrays25);
			string[] strArrays26 = new string[] { "System.Windows.Controls.TimePicker" };
			ProjectNeutralTypes.TimePicker = new ProjectNeutralTypeId("System.Windows.Controls.TimePicker", AssemblyGroup.ExtendedControlsToolkit, strArrays26);
			string[] strArrays27 = new string[] { "System.Windows.Controls.Picker" };
			ProjectNeutralTypes.Picker = new ProjectNeutralTypeId("System.Windows.Controls.Picker", AssemblyGroup.ExtendedControlsToolkit, strArrays27);
			string[] strArrays28 = new string[] { "System.Windows.Controls.TimeUpDown" };
			ProjectNeutralTypes.TimeUpDown = new ProjectNeutralTypeId("System.Windows.Controls.TimeUpDown", AssemblyGroup.ExtendedControlsToolkit, strArrays28);
			ProjectNeutralTypes.HeaderedContentControl = new ProjectNeutralTypeId("System.Windows.Controls.HeaderedContentControl", AssemblyGroup.ExtendedControlsToolkit, new string[0]);
			ProjectNeutralTypes.ListBoxDragDropTarget = new ProjectNeutralTypeId("System.Windows.Controls.ListBoxDragDropTarget", AssemblyGroup.ExtendedControlsToolkit, new string[0]);
			ProjectNeutralTypes.TreeViewDragDropTarget = new ProjectNeutralTypeId("System.Windows.Controls.TreeViewDragDropTarget", AssemblyGroup.ExtendedControlsToolkit, new string[0]);
			ProjectNeutralTypes.DataGridDragDropTarget = new ProjectNeutralTypeId("System.Windows.Controls.DataGridDragDropTarget", AssemblyGroup.ExtendedControlsToolkit, new string[0]);
			ProjectNeutralTypes.DataGridCellsPanel = new ProjectNeutralTypeId("Microsoft.Windows.Controls.DataGridCellsPanel", AssemblyGroup.ExtendedControlsToolkit, new string[0]);
			ProjectNeutralTypes.DataGridHeaderBorder = new ProjectNeutralTypeId("Microsoft.Windows.Controls.DataGridHeaderBorder", AssemblyGroup.ExtendedControlsToolkit, new string[0]);
			ProjectNeutralTypes.DataGridHyperlinkColumn = new ProjectNeutralTypeId("Microsoft.Windows.Controls.DataGridHyperlinkColumn", AssemblyGroup.ExtendedControlsToolkit, new string[0]);
			ProjectNeutralTypes.DockPanel = new ProjectNeutralTypeId("System.Windows.Controls.DockPanel", AssemblyGroup.ExtendedControlsToolkit, new string[0]);
			ProjectNeutralTypes.Expander = new ProjectNeutralTypeId("System.Windows.Controls.Expander", AssemblyGroup.ExtendedControlsToolkit, new string[0]);
			ProjectNeutralTypes.Label = new ProjectNeutralTypeId("System.Windows.Controls.Label", AssemblyGroup.ExtendedControlsDataInput, new string[0]);
			ProjectNeutralTypes.Viewbox = new ProjectNeutralTypeId("System.Windows.Controls.Viewbox", AssemblyGroup.ExtendedControlsToolkit, new string[0]);
			ProjectNeutralTypes.WrapPanel = new ProjectNeutralTypeId("System.Windows.Controls.WrapPanel", AssemblyGroup.ExtendedControlsToolkit, new string[0]);
			ProjectNeutralTypes.Interaction = new ProjectNeutralTypeId("System.Windows.Interactivity.Interaction", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.Behavior = new ProjectNeutralTypeId("System.Windows.Interactivity.Behavior", AssemblyGroup.Interactivity, KnownUnreferencedType.Behavior, new string[0]);
			ProjectNeutralTypes.BehaviorCollection = new ProjectNeutralTypeId("System.Windows.Interactivity.BehaviorCollection", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.BehaviorTriggerBase = new ProjectNeutralTypeId("System.Windows.Interactivity.TriggerBase", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.BehaviorEventTriggerBase = new ProjectNeutralTypeId("System.Windows.Interactivity.EventTriggerBase", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.BehaviorTriggerCollection = new ProjectNeutralTypeId("System.Windows.Interactivity.TriggerCollection", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.BehaviorTriggerAction = new ProjectNeutralTypeId("System.Windows.Interactivity.TriggerAction", AssemblyGroup.Interactivity, KnownUnreferencedType.TriggerAction, new string[0]);
			ProjectNeutralTypes.BehaviorTargetedTriggerAction = new ProjectNeutralTypeId("System.Windows.Interactivity.TargetedTriggerAction", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.BehaviorTriggerActionCollection = new ProjectNeutralTypeId("System.Windows.Interactivity.TriggerActionCollection", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.BehaviorEventTrigger = new ProjectNeutralTypeId("System.Windows.Interactivity.EventTrigger", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.InvokeCommandAction = new ProjectNeutralTypeId("System.Windows.Interactivity.InvokeCommandAction", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.DefaultTriggerAttribute = new ProjectNeutralTypeId("System.Windows.Interactivity.DefaultTriggerAttribute", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.TypeConstraintAttribute = new ProjectNeutralTypeId("System.Windows.Interactivity.TypeConstraintAttribute", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.CustomPropertyValueEditorAttribute = new ProjectNeutralTypeId("System.Windows.Interactivity.CustomPropertyValueEditorAttribute", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.CustomPropertyValueEditor = new ProjectNeutralTypeId("System.Windows.Interactivity.CustomPropertyValueEditor", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.ConditionBehavior = new ProjectNeutralTypeId("Microsoft.Expression.Interactivity.Core.ConditionBehavior", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.ConditionalExpression = new ProjectNeutralTypeId("Microsoft.Expression.Interactivity.Core.ConditionalExpression", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.ConditionCollection = new ProjectNeutralTypeId("Microsoft.Expression.Interactivity.Core.ConditionCollection", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.ComparisonCondition = new ProjectNeutralTypeId("Microsoft.Expression.Interactivity.Core.ComparisonCondition", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.ForwardChaining = new ProjectNeutralTypeId("Microsoft.Expression.Interactivity.Core.ForwardChaining", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.SetDataStoreValueAction = new ProjectNeutralTypeId("Microsoft.Expression.Interactivity.Core.SetDataStoreValueAction", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.DataStoreChangedTrigger = new ProjectNeutralTypeId("Microsoft.Expression.Interactivity.Core.DataStoreChangedTrigger", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.PropertyChangedTrigger = new ProjectNeutralTypeId("Microsoft.Expression.Interactivity.Core.PropertyChangedTrigger", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.ChangePropertyAction = new ProjectNeutralTypeId("Microsoft.Expression.Interactivity.Core.ChangePropertyAction", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.CallMethodAction = new ProjectNeutralTypeId("Microsoft.Expression.Interactivity.Core.CallMethodAction", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.TimerTrigger = new ProjectNeutralTypeId("Microsoft.Expression.Interactivity.Core.TimerTrigger", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.FluidMoveBehavior = new ProjectNeutralTypeId("Microsoft.Expression.Interactivity.Layout.FluidMoveBehavior", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.FluidMoveBehaviorBase = new ProjectNeutralTypeId("Microsoft.Expression.Interactivity.Layout.FluidMoveBehaviorBase", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.FluidMoveSetTagBehavior = new ProjectNeutralTypeId("Microsoft.Expression.Interactivity.Layout.FluidMoveSetTagBehavior", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.StoryboardAction = new ProjectNeutralTypeId("Microsoft.Expression.Interactivity.Media.StoryboardAction", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.ControlStoryboardAction = new ProjectNeutralTypeId("Microsoft.Expression.Interactivity.Media.ControlStoryboardAction", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.StoryboardTrigger = new ProjectNeutralTypeId("Microsoft.Expression.Interactivity.Media.StoryboardTrigger", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.StoryboardCompletedTrigger = new ProjectNeutralTypeId("Microsoft.Expression.Interactivity.Media.StoryboardCompletedTrigger", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.KeyTrigger = new ProjectNeutralTypeId("Microsoft.Expression.Interactivity.Input.KeyTrigger", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.GoToStateAction = new ProjectNeutralTypeId("Microsoft.Expression.Interactivity.Core.GoToStateAction", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.HyperlinkAction = new ProjectNeutralTypeId("Microsoft.Expression.Interactivity.Core.HyperlinkAction", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.LaunchUriOrFileAction = new ProjectNeutralTypeId("Microsoft.Expression.Interactivity.Core.LaunchUriOrFileAction", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.RemoveElementAction = new ProjectNeutralTypeId("Microsoft.Expression.Interactivity.Core.RemoveElementAction", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.PlaySoundAction = new ProjectNeutralTypeId("Microsoft.Expression.Interactivity.Media.PlaySoundAction", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.MouseDragElementBehavior = new ProjectNeutralTypeId("Microsoft.Expression.Interactivity.Layout.MouseDragElementBehavior", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.DataTrigger = new ProjectNeutralTypeId("Microsoft.Expression.Interactivity.Core.DataTrigger", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.DataStateBehavior = new ProjectNeutralTypeId("Microsoft.Expression.Interactivity.Core.DataStateBehavior", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.TranslateZoomRotateBehavior = new ProjectNeutralTypeId("Microsoft.Expression.Interactivity.Input.TranslateZoomRotateBehavior", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.ExtendedVisualStateManager = new ProjectNeutralTypeId("Microsoft.Expression.Interactivity.Core.ExtendedVisualStateManager", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.TransitionEffect = new ProjectNeutralTypeId("Microsoft.Expression.Media.Effects.TransitionEffect", AssemblyGroup.Interactivity, KnownUnreferencedType.TransitionEffect, new string[0]);
			ProjectNeutralTypes.PrimitiveShape = new ProjectNeutralTypeId("Microsoft.Expression.Shapes.PrimitiveShape", AssemblyGroup.Interactivity, KnownUnreferencedType.PrimitiveShape, new string[0]);
			ProjectNeutralTypes.CompositeShape = new ProjectNeutralTypeId("Microsoft.Expression.Controls.CompositeShape", AssemblyGroup.Interactivity, KnownUnreferencedType.CompositeShape, new string[0]);
			ProjectNeutralTypes.CompositeContentShape = new ProjectNeutralTypeId("Microsoft.Expression.Controls.CompositeContentShape", AssemblyGroup.Interactivity, KnownUnreferencedType.CompositeContentShape, new string[0]);
			ProjectNeutralTypes.GeometryEffect = new ProjectNeutralTypeId("Microsoft.Expression.Media.GeometryEffect", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.RegularPolygon = new ProjectNeutralTypeId("Microsoft.Expression.Shapes.RegularPolygon", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.Arc = new ProjectNeutralTypeId("Microsoft.Expression.Shapes.Arc", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.BlockArrow = new ProjectNeutralTypeId("Microsoft.Expression.Shapes.BlockArrow", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.Callout = new ProjectNeutralTypeId("Microsoft.Expression.Controls.Callout", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.LineArrow = new ProjectNeutralTypeId("Microsoft.Expression.Controls.LineArrow", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.LayoutPath = new ProjectNeutralTypeId("Microsoft.Expression.Controls.LayoutPath", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.Orientation = new ProjectNeutralTypeId("Microsoft.Expression.Controls.Orientation", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.PathListBox = new ProjectNeutralTypeId("Microsoft.Expression.Controls.PathListBox", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.PathListBoxItem = new ProjectNeutralTypeId("Microsoft.Expression.Controls.PathListBoxItem", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.PathPanel = new ProjectNeutralTypeId("Microsoft.Expression.Controls.PathPanel", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.LayoutPathCollection = new ProjectNeutralTypeId("Microsoft.Expression.Controls.LayoutPathCollection", AssemblyGroup.Interactivity, new string[0]);
			ProjectNeutralTypes.ActivateStateAction = new ProjectNeutralTypeId("Microsoft.Expression.Prototyping.Behavior.ActivateStateAction", AssemblyGroup.SketchFlowExtensions, new string[0]);
			ProjectNeutralTypes.NavigateBackAction = new ProjectNeutralTypeId("Microsoft.Expression.Prototyping.Behavior.NavigateBackAction", AssemblyGroup.SketchFlowExtensions, new string[0]);
			ProjectNeutralTypes.NavigateForwardAction = new ProjectNeutralTypeId("Microsoft.Expression.Prototyping.Behavior.NavigateForwardAction", AssemblyGroup.SketchFlowExtensions, new string[0]);
			ProjectNeutralTypes.NavigateToScreenAction = new ProjectNeutralTypeId("Microsoft.Expression.Prototyping.Behavior.NavigateToScreenAction", AssemblyGroup.SketchFlowExtensions, new string[0]);
			ProjectNeutralTypes.PlaySketchFlowAnimationAction = new ProjectNeutralTypeId("Microsoft.Expression.Prototyping.Behavior.PlaySketchFlowAnimationAction", AssemblyGroup.SketchFlowExtensions, new string[0]);
			ProjectNeutralTypes.NavigationMenuAction = new ProjectNeutralTypeId("Microsoft.Expression.Prototyping.Behavior.NavigationMenuAction", AssemblyGroup.SketchFlowExtensions, new string[0]);
			ProjectNeutralTypes.SketchFlowAnimationTrigger = new ProjectNeutralTypeId("Microsoft.Expression.Prototyping.Behavior.SketchFlowAnimationTrigger", AssemblyGroup.SketchFlowExtensions, new string[0]);
			ProjectNeutralTypes.RemoveItemInListBoxAction = new ProjectNeutralTypeId("Microsoft.Expression.Prototyping.Behavior.RemoveItemInListBoxAction", AssemblyGroup.SketchFlowExtensions, new string[0]);
			ProjectNeutralTypes.SketchBaseSL = new ProjectNeutralTypeId("Microsoft.Expression.Prototyping.SketchControls.SketchBaseSL", AssemblyGroup.SketchFlowExtensions, KnownUnreferencedType.SketchControl, new string[0]);
			ProjectNeutralTypes.SketchBorderSL = new ProjectNeutralTypeId("Microsoft.Expression.Prototyping.SketchControls.SketchBorderSL", AssemblyGroup.SketchFlowExtensions, new string[0]);
			ProjectNeutralTypes.SketchCircleSL = new ProjectNeutralTypeId("Microsoft.Expression.Prototyping.SketchControls.SketchCircleSL", AssemblyGroup.SketchFlowExtensions, new string[0]);
			ProjectNeutralTypes.SketchRectangleSL = new ProjectNeutralTypeId("Microsoft.Expression.Prototyping.SketchControls.SketchRectangleSL", AssemblyGroup.SketchFlowExtensions, new string[0]);
			ProjectNeutralTypes.SketchBase = new ProjectNeutralTypeId("Microsoft.Expression.Prototyping.SketchControls.SketchBase", AssemblyGroup.SketchFlowExtensions, KnownUnreferencedType.SketchControl, new string[0]);
			ProjectNeutralTypes.SketchBorderUC = new ProjectNeutralTypeId("Microsoft.Expression.Prototyping.SketchControls.SketchBorderUC", AssemblyGroup.SketchFlowExtensions, new string[0]);
			ProjectNeutralTypes.SketchCircleUC = new ProjectNeutralTypeId("Microsoft.Expression.Prototyping.SketchControls.SketchCircleUC", AssemblyGroup.SketchFlowExtensions, new string[0]);
			ProjectNeutralTypes.SketchPath = new ProjectNeutralTypeId("Microsoft.Expression.Prototyping.SketchControls.SketchPath", AssemblyGroup.SketchFlowExtensions, new string[0]);
			ProjectNeutralTypes.SketchRectangleUC = new ProjectNeutralTypeId("Microsoft.Expression.Prototyping.SketchControls.SketchRectangleUC", AssemblyGroup.SketchFlowExtensions, new string[0]);
		}
	}
}