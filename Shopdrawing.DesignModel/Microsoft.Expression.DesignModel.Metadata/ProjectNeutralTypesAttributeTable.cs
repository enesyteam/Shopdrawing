// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Metadata.ProjectNeutralTypesAttributeTable
// Assembly: Microsoft.Expression.DesignModel, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: CEEFEC81-4FB1-4567-B694-554E1BED5C03
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignModel.dll

using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.ComponentModel;
using System.Windows;

namespace Microsoft.Expression.DesignModel.Metadata
{
    internal static class ProjectNeutralTypesAttributeTable
    {
        public static void RegisterWPFToolboxDifferenceAttributeTable(ITypeResolver typeResolver)
        {
            AttributeTableBuilder attributeTableBuilder = new AttributeTableBuilder();
            Type runtimeType1 = typeResolver.ResolveType(ProjectNeutralTypes.DataGrid).RuntimeType;
            if (runtimeType1 != (Type)null)
                attributeTableBuilder.AddCallback(runtimeType1, (AttributeCallback)(builder =>
                {
                    builder.AddCustomAttributes((Attribute)new ToolboxCategoryAttribute("Data", true));
                    builder.AddCustomAttributes((Attribute)new DescriptionAttribute(StringTable.DataGridTypeDescription));
                }));
            Type runtimeType2 = typeResolver.ResolveType(ProjectNeutralTypes.Calendar).RuntimeType;
            if (runtimeType2 != (Type)null)
                attributeTableBuilder.AddCallback(runtimeType2, (AttributeCallback)(builder =>
                {
                    builder.AddCustomAttributes((Attribute)new ToolboxCategoryAttribute("", true));
                    builder.AddCustomAttributes((Attribute)new DescriptionAttribute(StringTable.CalendarTypeDescription));
                }));
            Type runtimeType3 = typeResolver.ResolveType(ProjectNeutralTypes.CalendarButton).RuntimeType;
            if (runtimeType3 != (Type)null)
                attributeTableBuilder.AddCallback(runtimeType3, (AttributeCallback)(builder =>
                {
                    builder.AddCustomAttributes((Attribute)new ToolboxCategoryAttribute("Control Parts", false));
                    builder.AddCustomAttributes((Attribute)new DescriptionAttribute(StringTable.CalendarButtonTypeDescription));
                }));
            Type runtimeType4 = typeResolver.ResolveType(ProjectNeutralTypes.CalendarDayButton).RuntimeType;
            if (runtimeType4 != (Type)null)
                attributeTableBuilder.AddCallback(runtimeType4, (AttributeCallback)(builder =>
                {
                    builder.AddCustomAttributes((Attribute)new ToolboxCategoryAttribute("Control Parts", false));
                    builder.AddCustomAttributes((Attribute)new DescriptionAttribute(StringTable.CalendarDayButtonTypeDescription));
                }));
            Type runtimeType5 = typeResolver.ResolveType(ProjectNeutralTypes.CalendarItem).RuntimeType;
            if (runtimeType5 != (Type)null)
                attributeTableBuilder.AddCallback(runtimeType5, (AttributeCallback)(builder =>
                {
                    builder.AddCustomAttributes((Attribute)new ToolboxCategoryAttribute("Control Parts", false));
                    builder.AddCustomAttributes((Attribute)new DescriptionAttribute(StringTable.CalendarItemTypeDescription));
                }));
            MetadataStore.AddAttributeTable(attributeTableBuilder.CreateTable());
        }

        public static void RegisterDataGridAttributeTable(ITypeResolver typeResolver)
        {
            Type dataGridType = typeResolver.ResolveType(ProjectNeutralTypes.DataGrid).RuntimeType;
            if (dataGridType == (Type)null)
                return;
            AttributeTableBuilder attributeTableBuilder = new AttributeTableBuilder();
            Type dataGridCellType = typeResolver.ResolveType(ProjectNeutralTypes.DataGridCell).RuntimeType;
            Type dataGridColumnHeaderType = typeResolver.ResolveType(ProjectNeutralTypes.DataGridColumnHeader).RuntimeType;
            Type dataGridRowHeaderType = typeResolver.ResolveType(ProjectNeutralTypes.DataGridRowHeader).RuntimeType;
            Type dataGridRowType = typeResolver.ResolveType(ProjectNeutralTypes.DataGridRow).RuntimeType;
            attributeTableBuilder.AddCallback(dataGridType, (AttributeCallback)(builder =>
            {
                builder.AddCustomAttributes((Attribute)new DefaultBindingPropertyAttribute("ItemsSource"));
                builder.AddCustomAttributes((Attribute)new StyleTypedPropertyAttribute()
                {
                    Property = "CellStyle",
                    StyleTargetType = dataGridCellType
                });
                builder.AddCustomAttributes((Attribute)new StyleTypedPropertyAttribute()
                {
                    Property = "ColumnHeaderStyle",
                    StyleTargetType = dataGridColumnHeaderType
                });
                builder.AddCustomAttributes((Attribute)new StyleTypedPropertyAttribute()
                {
                    Property = "DragIndicatorStyle",
                    StyleTargetType = dataGridColumnHeaderType
                });
                builder.AddCustomAttributes((Attribute)new StyleTypedPropertyAttribute()
                {
                    Property = "DropLocationIndicatorStyle",
                    StyleTargetType = dataGridType
                });
                builder.AddCustomAttributes((Attribute)new StyleTypedPropertyAttribute()
                {
                    Property = "RowHeaderStyle",
                    StyleTargetType = dataGridRowHeaderType
                });
                builder.AddCustomAttributes((Attribute)new StyleTypedPropertyAttribute()
                {
                    Property = "RowStyle",
                    StyleTargetType = dataGridRowType
                });
                builder.AddCustomAttributes("AreRowDetailsFrozen", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("AreRowGroupHeadersFrozen", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("CanUserResizeColumns", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("CanUserResizeColumns", (Attribute)new DescriptionAttribute(PresentationFrameworkStringTable.Description_DataGrid_CanUserResizeColumns));
                builder.AddCustomAttributes("CanUserResizeColumns", (Attribute)new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                builder.AddCustomAttributes("CanUserReorderColumns", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("CanUserReorderColumns", (Attribute)new DescriptionAttribute(PresentationFrameworkStringTable.Description_DataGrid_CanUserReorderColumns));
                builder.AddCustomAttributes("CanUserReorderColumns", (Attribute)new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                builder.AddCustomAttributes("CanUserSortColumns", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("CanUserSortColumns", (Attribute)new DescriptionAttribute(PresentationFrameworkStringTable.Description_DataGrid_CanUserSortColumns));
                builder.AddCustomAttributes("CanUserSortColumns", (Attribute)new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                builder.AddCustomAttributes("RowDetailsVisibilityMode", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("RowDetailsVisibilityMode", (Attribute)new DescriptionAttribute(PresentationFrameworkStringTable.Description_DataGrid_RowDetailsVisibilityMode));
                builder.AddCustomAttributes("RowDetailsVisibilityMode", (Attribute)new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                builder.AddCustomAttributes("AutoGenerateColumns", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("AutoGenerateColumns", (Attribute)new DescriptionAttribute(PresentationFrameworkStringTable.Description_DataGrid_AutogenerateColumns));
                builder.AddCustomAttributes("Columns", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("Columns", (Attribute)new DescriptionAttribute(PresentationFrameworkStringTable.Description_DataGrid_Columns));
                builder.AddCustomAttributes("Columns", (Attribute)new AlternateContentPropertyAttribute());
                builder.AddCustomAttributes("ItemsSource", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("ItemsSource", (Attribute)new DescriptionAttribute(PresentationFrameworkStringTable.Description_DataGrid_ItemsSource));
                builder.AddCustomAttributes("SelectedIndex", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("SelectedIndex", (Attribute)new DescriptionAttribute(PresentationFrameworkStringTable.Description_DataGrid_SelectedIndex));
                builder.AddCustomAttributes("SelectionMode", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("HeadersVisibility", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("HeadersVisibility", (Attribute)new DescriptionAttribute(PresentationFrameworkStringTable.Description_DataGrid_HeadersVisibility));
                builder.AddCustomAttributes("GridLinesVisibility", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("GridLinesVisibility", (Attribute)new DescriptionAttribute(PresentationFrameworkStringTable.Description_DataGrid_GridLinesVisibility));
                builder.AddCustomAttributes("IsReadOnly", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("MaxColumnWidth", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Layout));
                builder.AddCustomAttributes("MaxColumnWidth", (Attribute)new DescriptionAttribute(PresentationFrameworkStringTable.Description_DataGrid_MaxColumnWidth));
                builder.AddCustomAttributes("MaxColumnWidth", (Attribute)new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                builder.AddCustomAttributes("MinColumnWidth", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Layout));
                builder.AddCustomAttributes("MinColumnWidth", (Attribute)new DescriptionAttribute(PresentationFrameworkStringTable.Description_DataGrid_MinColumnWidth));
                builder.AddCustomAttributes("MinColumnWidth", (Attribute)new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                builder.AddCustomAttributes("ColumnWidth", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Layout));
                builder.AddCustomAttributes("ColumnWidth", (Attribute)new DescriptionAttribute(PresentationFrameworkStringTable.Description_DataGrid_ColumnWidth));
                builder.AddCustomAttributes("RowHeight", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Layout));
                builder.AddCustomAttributes("RowHeight", (Attribute)new DescriptionAttribute(PresentationFrameworkStringTable.Description_DataGrid_RowHeight));
                builder.AddCustomAttributes("RowHeaderWidth", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Layout));
                builder.AddCustomAttributes("RowHeaderWidth", (Attribute)new DescriptionAttribute(PresentationFrameworkStringTable.Description_DataGrid_RowHeaderWidth));
                builder.AddCustomAttributes("HorizontalScrollBarVisibility", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Layout));
                builder.AddCustomAttributes("HorizontalScrollBarVisibility", (Attribute)new DescriptionAttribute(PresentationFrameworkStringTable.Description_DataGrid_HorizontalScrollBarVisibility));
                builder.AddCustomAttributes("HorizontalScrollBarVisibility", (Attribute)new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                builder.AddCustomAttributes("VerticalScrollBarVisibility", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Layout));
                builder.AddCustomAttributes("VerticalScrollBarVisibility", (Attribute)new DescriptionAttribute(PresentationFrameworkStringTable.Description_DataGrid_VerticalScrollBarVisibility));
                builder.AddCustomAttributes("VerticalScrollBarVisibility", (Attribute)new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                builder.AddCustomAttributes("RowDetailsTemplate", (Attribute)new DataContextValueSourceAttribute("ItemsSource", true));
                builder.AddCustomAttributes("SelectedItem", (Attribute)new DataContextValueSourceAttribute("ItemsSource", true));
                builder.AddCustomAttributes("SelectedItem", (Attribute)new DualDataContextAttribute(false));
                builder.AddCustomAttributes("RowStyle", (Attribute)new DataContextValueSourceAttribute("ItemsSource", true));
                builder.AddCustomAttributes("CellStyle", (Attribute)new DataContextValueSourceAttribute("ItemsSource", true));
            }));
            Type runtimeType1 = typeResolver.ResolveType(ProjectNeutralTypes.DataGridColumn).RuntimeType;
            attributeTableBuilder.AddCallback(runtimeType1, (AttributeCallback)(builder =>
            {
                builder.AddCustomAttributes((Attribute)new StyleTypedPropertyAttribute()
                {
                    Property = "CellStyle",
                    StyleTargetType = dataGridCellType
                });
                builder.AddCustomAttributes((Attribute)new StyleTypedPropertyAttribute()
                {
                    Property = "HeaderStyle",
                    StyleTargetType = dataGridColumnHeaderType
                });
                builder.AddCustomAttributes((Attribute)new StyleTypedPropertyAttribute()
                {
                    Property = "DragIndicatorStyle",
                    StyleTargetType = dataGridColumnHeaderType
                });
                builder.AddCustomAttributes("CanUserReorder", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("CanUserReorder", (Attribute)new DescriptionAttribute(PresentationFrameworkStringTable.Description_DataGridColumn_CanUserReorder));
                builder.AddCustomAttributes("CanUserResize", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("CanUserResize", (Attribute)new DescriptionAttribute(PresentationFrameworkStringTable.Description_DataGridColumn_CanUserResize));
                builder.AddCustomAttributes("CanUserSort", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("CanUserSort", (Attribute)new DescriptionAttribute(PresentationFrameworkStringTable.Description_DataGridColumn_CanUserSort));
                builder.AddCustomAttributes("Header", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("Header", (Attribute)new DescriptionAttribute(PresentationFrameworkStringTable.Description_DataGridColumn_Header));
                builder.AddCustomAttributes("DisplayIndex", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("DisplayIndex", (Attribute)new DescriptionAttribute(PresentationFrameworkStringTable.Description_DataGridColumn_DisplayIndex));
                builder.AddCustomAttributes("IsReadOnly", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("IsReadOnly", (Attribute)new DescriptionAttribute(PresentationFrameworkStringTable.Description_DataGridColumn_IsReadOnly));
                builder.AddCustomAttributes("Visibility", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Appearance));
                builder.AddCustomAttributes("Visibility", (Attribute)new DescriptionAttribute(PresentationFrameworkStringTable.Description_DataGridColumn_Visibility));
                builder.AddCustomAttributes("SortMemberPath", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Appearance));
                builder.AddCustomAttributes("SortMemberPath", (Attribute)new DescriptionAttribute(PresentationFrameworkStringTable.Description_DataGridColumn_SortMemberPath));
                builder.AddCustomAttributes("MaxWidth", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Layout));
                builder.AddCustomAttributes("MaxWidth", (Attribute)new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                builder.AddCustomAttributes("MaxWidth", (Attribute)new DescriptionAttribute(PresentationFrameworkStringTable.Description_DataGridColumn_MaxWidth));
                builder.AddCustomAttributes("MaxWidth", (Attribute)new DefaultValueAttribute(double.PositiveInfinity));
                builder.AddCustomAttributes("MinWidth", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Layout));
                builder.AddCustomAttributes("MinWidth", (Attribute)new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                builder.AddCustomAttributes("MinWidth", (Attribute)new DescriptionAttribute(PresentationFrameworkStringTable.Description_DataGridColumn_MinWidth));
                builder.AddCustomAttributes("Width", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Layout));
                builder.AddCustomAttributes("Width", (Attribute)new DescriptionAttribute(PresentationFrameworkStringTable.Description_DataGridColumn_Width));
            }));
            Type runtimeType2 = typeResolver.ResolveType(ProjectNeutralTypes.DataGridBoundColumn).RuntimeType;
            attributeTableBuilder.AddCallback(runtimeType2, (AttributeCallback)(builder =>
            {
                builder.AddCustomAttributes((Attribute)new DefaultBindingPropertyAttribute("Binding"));
                builder.AddCustomAttributes("Binding", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("Binding", (Attribute)new DescriptionAttribute(PresentationFrameworkStringTable.Description_DataGridBoundColumn_Binding));
                builder.AddCustomAttributes("Binding", (Attribute)new DataContextValueSourceAttribute("ItemsSource", "Columns\\", true));
            }));
            Type runtimeType3 = typeResolver.ResolveType(ProjectNeutralTypes.DataGridTemplateColumn).RuntimeType;
            attributeTableBuilder.AddCallback(runtimeType3, (AttributeCallback)(builder =>
            {
                builder.AddCustomAttributes("CellEditingTemplate", (Attribute)new DataContextValueSourceAttribute("ItemsSource", "Columns\\", true));
                builder.AddCustomAttributes("CellTemplate", (Attribute)new DataContextValueSourceAttribute("ItemsSource", "Columns\\", true));
            }));
            Type runtimeType4 = typeResolver.ResolveType(ProjectNeutralTypes.DataGridCheckBoxColumn).RuntimeType;
            Type checkBoxType = typeResolver.ResolveType(PlatformTypes.CheckBox).RuntimeType;
            attributeTableBuilder.AddCallback(runtimeType4, (AttributeCallback)(builder =>
            {
                builder.AddCustomAttributes((Attribute)new StyleTypedPropertyAttribute()
                {
                    Property = "EditingElementStyle",
                    StyleTargetType = checkBoxType
                });
                builder.AddCustomAttributes((Attribute)new StyleTypedPropertyAttribute()
                {
                    Property = "ElementStyle",
                    StyleTargetType = checkBoxType
                });
                builder.AddCustomAttributes("IsThreeState", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("IsThreeState", (Attribute)new DescriptionAttribute(PresentationFrameworkStringTable.Description_DataGridCheckBoxColumn_IsThreeState));
            }));
            Type runtimeType5 = typeResolver.ResolveType(ProjectNeutralTypes.DataGridTextColumn).RuntimeType;
            Type textBoxType = typeResolver.ResolveType(PlatformTypes.TextBox).RuntimeType;
            Type textBlockType = typeResolver.ResolveType(PlatformTypes.TextBlock).RuntimeType;
            attributeTableBuilder.AddCallback(runtimeType5, (AttributeCallback)(builder =>
            {
                builder.AddCustomAttributes((Attribute)new StyleTypedPropertyAttribute()
                {
                    Property = "EditingElementStyle",
                    StyleTargetType = textBoxType
                });
                builder.AddCustomAttributes((Attribute)new StyleTypedPropertyAttribute()
                {
                    Property = "ElementStyle",
                    StyleTargetType = textBlockType
                });
                builder.AddCustomAttributes("FontFamily", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Text));
                builder.AddCustomAttributes("FontFamily", (Attribute)new DescriptionAttribute(PresentationFrameworkStringTable.Description_DataGridTextColumn_FontFamily));
                builder.AddCustomAttributes("FontSize", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Text));
                builder.AddCustomAttributes("FontSize", (Attribute)new DescriptionAttribute(PresentationFrameworkStringTable.Description_DataGridTextColumn_FontSize));
                builder.AddCustomAttributes("FontStyle", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Text));
                builder.AddCustomAttributes("FontStyle", (Attribute)new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                builder.AddCustomAttributes("FontStyle", (Attribute)new DescriptionAttribute(PresentationFrameworkStringTable.Description_DataGridTextColumn_FontStyle));
                builder.AddCustomAttributes("FontWeight", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Text));
                builder.AddCustomAttributes("FontWeight", (Attribute)new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                builder.AddCustomAttributes("FontWeight", (Attribute)new DescriptionAttribute(PresentationFrameworkStringTable.Description_DataGridTextColumn_FontWeight));
                builder.AddCustomAttributes("Foreground", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Brushes));
                builder.AddCustomAttributes("Foreground", (Attribute)new DescriptionAttribute(PresentationFrameworkStringTable.Description_DataGridTextColumn_Foreground));
            }));
            Type runtimeType6 = typeResolver.ResolveType(ProjectNeutralTypes.DataGridHyperlinkColumn).RuntimeType;
            if (runtimeType6 != (Type)null)
                attributeTableBuilder.AddCallback(runtimeType6, (AttributeCallback)(builder =>
                {
                    builder.AddCustomAttributes((Attribute)new StyleTypedPropertyAttribute()
                    {
                        Property = "EditingElementStyle",
                        StyleTargetType = textBoxType
                    });
                    builder.AddCustomAttributes((Attribute)new StyleTypedPropertyAttribute()
                    {
                        Property = "ElementStyle",
                        StyleTargetType = textBlockType
                    });
                }));
            ITypeId[] typeIdArray1 = new ITypeId[10]
      {
        ProjectNeutralTypes.DataGridCell,
        ProjectNeutralTypes.DataGridCellsPresenter,
        ProjectNeutralTypes.DataGridColumnHeader,
        ProjectNeutralTypes.DataGridColumnHeadersPresenter,
        ProjectNeutralTypes.DataGridDetailsPresenter,
        ProjectNeutralTypes.DataGridFrozenGrid,
        ProjectNeutralTypes.DataGridRow,
        ProjectNeutralTypes.DataGridRowGroupHeader,
        ProjectNeutralTypes.DataGridRowHeader,
        ProjectNeutralTypes.DataGridRowsPresenter
      };
            foreach (ITypeId typeId in typeIdArray1)
            {
                Type runtimeType7 = typeResolver.ResolveType(typeId).RuntimeType;
                if (runtimeType7 != (Type)null)
                    attributeTableBuilder.AddCallback(runtimeType7, (AttributeCallback)(builder => builder.AddCustomAttributes((Attribute)new ToolboxCategoryAttribute("Data/Control Parts", false))));
            }
            ITypeId[] typeIdArray2 = new ITypeId[2]
      {
        ProjectNeutralTypes.DataGridCellsPanel,
        ProjectNeutralTypes.DataGridHeaderBorder
      };
            foreach (ITypeId typeId in typeIdArray2)
            {
                Type runtimeType7 = typeResolver.ResolveType(typeId).RuntimeType;
                if (runtimeType7 != (Type)null)
                    attributeTableBuilder.AddCallback(runtimeType7, (AttributeCallback)(builder => builder.AddCustomAttributes((Attribute)new ToolboxBrowsableAttribute(false))));
            }
            MetadataStore.AddAttributeTable(attributeTableBuilder.CreateTable());
        }

        public static void RegisterDatePickerAttributeTable(ITypeResolver typeResolver)
        {
            Type runtimeType = typeResolver.ResolveType(ProjectNeutralTypes.DatePicker).RuntimeType;
            if (runtimeType == (Type)null)
                return;
            AttributeTableBuilder attributeTableBuilder = new AttributeTableBuilder();
            Type calendarType = typeResolver.ResolveType(ProjectNeutralTypes.Calendar).RuntimeType;
            attributeTableBuilder.AddCallback(runtimeType, (AttributeCallback)(builder =>
            {
                builder.AddCustomAttributes((Attribute)new StyleTypedPropertyAttribute()
                {
                    Property = "CalendarStyle",
                    StyleTargetType = calendarType
                });
                builder.AddCustomAttributes("BlackoutDates", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("DisplayDate", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("DisplayDateEnd", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("DisplayDateStart", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("FirstDayOfWeek", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("IsDropDownOpen", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("IsTodayHighlighted", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("SelectedDateFormat", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("Text", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes((Attribute)new ToolboxCategoryAttribute("", true));
            }));
            MetadataStore.AddAttributeTable(attributeTableBuilder.CreateTable());
        }

        public static void RegisterCalendarAttributeTable(ITypeResolver typeResolver)
        {
            Type runtimeType = typeResolver.ResolveType(ProjectNeutralTypes.Calendar).RuntimeType;
            if (runtimeType == (Type)null)
                return;
            AttributeTableBuilder attributeTableBuilder = new AttributeTableBuilder();
            Type calendarButtonType = typeResolver.ResolveType(ProjectNeutralTypes.CalendarButton).RuntimeType;
            Type calendarDayButtonType = typeResolver.ResolveType(ProjectNeutralTypes.CalendarDayButton).RuntimeType;
            Type calendarItemType = typeResolver.ResolveType(ProjectNeutralTypes.CalendarItem).RuntimeType;
            attributeTableBuilder.AddCallback(runtimeType, (AttributeCallback)(builder =>
            {
                builder.AddCustomAttributes((Attribute)new StyleTypedPropertyAttribute()
                {
                    Property = "CalendarButtonStyle",
                    StyleTargetType = calendarButtonType
                });
                builder.AddCustomAttributes((Attribute)new StyleTypedPropertyAttribute()
                {
                    Property = "CalendarDayButtonStyle",
                    StyleTargetType = calendarDayButtonType
                });
                builder.AddCustomAttributes((Attribute)new StyleTypedPropertyAttribute()
                {
                    Property = "CalendarItemStyle",
                    StyleTargetType = calendarItemType
                });
                builder.AddCustomAttributes("DisplayDate", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("DisplayDateEnd", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("DisplayDateStart", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("FirstDayOfWeek", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("IsTodayHighlighted", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("DisplayMode", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("SelectionMode", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
            }));
            MetadataStore.AddAttributeTable(attributeTableBuilder.CreateTable());
        }

        public static void RegisterTabControlAttributeTable(ITypeResolver typeResolver)
        {
            Type runtimeType1 = typeResolver.ResolveType(ProjectNeutralTypes.TabControl).RuntimeType;
            if (runtimeType1 == (Type)null)
                return;
            AttributeTableBuilder attributeTableBuilder = new AttributeTableBuilder();
            Type tabItemType = typeResolver.ResolveType(ProjectNeutralTypes.TabItem).RuntimeType;
            attributeTableBuilder.AddCallback(runtimeType1, (AttributeCallback)(builder =>
            {
                builder.AddCustomAttributes((Attribute)new StyleTypedPropertyAttribute()
                {
                    Property = "ItemContainerStyle",
                    StyleTargetType = tabItemType
                });
                builder.AddCustomAttributes("SelectedIndex", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("SelectedItem", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("TabStripPlacement", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
            }));
            Type runtimeType2 = typeResolver.ResolveType(ProjectNeutralTypes.TabItem).RuntimeType;
            attributeTableBuilder.AddCallback(runtimeType2, (AttributeCallback)(builder =>
            {
                builder.AddCustomAttributes("Header", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                builder.AddCustomAttributes("IsSelected", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
            }));
            MetadataStore.AddAttributeTable(attributeTableBuilder.CreateTable());
        }

        public static void RegisterTreeViewAttributeTable(ITypeResolver typeResolver)
        {
            Type runtimeType = typeResolver.ResolveType(ProjectNeutralTypes.TreeView).RuntimeType;
            if (runtimeType == (Type)null)
                return;
            AttributeTableBuilder attributeTableBuilder = new AttributeTableBuilder();
            Type treeViewItemType = typeResolver.ResolveType(ProjectNeutralTypes.TreeViewItem).RuntimeType;
            attributeTableBuilder.AddCallback(runtimeType, (AttributeCallback)(builder =>
            {
                builder.AddCustomAttributes((Attribute)new StyleTypedPropertyAttribute()
                {
                    Property = "ItemContainerStyle",
                    StyleTargetType = treeViewItemType
                });
                builder.AddCustomAttributes("SelectedValue", (Attribute)new DataContextValueSourceAttribute("SelectedValuePath", false));
                builder.AddCustomAttributes("SelectedValue", (Attribute)new DualDataContextAttribute(true));
                builder.AddCustomAttributes("SelectedValuePath", (Attribute)new DataContextPathExtensionAttribute("ItemsSource", true));
                builder.AddCustomAttributes("SelectedItem", (Attribute)new DataContextValueSourceAttribute("ItemsSource", true));
                builder.AddCustomAttributes("SelectedItem", (Attribute)new DualDataContextAttribute(false));
            }));
            MetadataStore.AddAttributeTable(attributeTableBuilder.CreateTable());
        }

        public static void RegisterVisualTransitionAttributeTable(ITypeResolver typeResolver)
        {
            Type runtimeType = typeResolver.ResolveType(ProjectNeutralTypes.VisualTransition).RuntimeType;
            if (runtimeType == (Type)null)
                return;
            AttributeTableBuilder attributeTableBuilder = new AttributeTableBuilder();
            attributeTableBuilder.AddCallback(runtimeType, (AttributeCallback)(builder =>
            {
                builder.AddCustomAttributes("From", (Attribute)new DefaultValueAttribute((string)null));
                builder.AddCustomAttributes("To", (Attribute)new DefaultValueAttribute((string)null));
                builder.AddCustomAttributes("Storyboard", (Attribute)new DefaultValueAttribute((string)null));
            }));
            MetadataStore.AddAttributeTable(attributeTableBuilder.CreateTable());
        }

        public static void RegisterAutoCompleteBoxAttributeTable(ITypeResolver typeResolver)
        {
            AttributeTableBuilder attributeTableBuilder = new AttributeTableBuilder();
            Type runtimeType = typeResolver.ResolveType(ProjectNeutralTypes.AutoCompleteBox).RuntimeType;
            if (runtimeType != (Type)null)
                attributeTableBuilder.AddCallback(runtimeType, (AttributeCallback)(builder =>
                {
                    builder.AddCustomAttributes("IsDropDownOpen", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                    builder.AddCustomAttributes("MaxDropDownHeight", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                    builder.AddCustomAttributes("MinimumPopulateDelay", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                    builder.AddCustomAttributes("MinimumPrefixLength", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                    builder.AddCustomAttributes("ValueMemberBinding", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                    builder.AddCustomAttributes("ValueMemberPath", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                }));
            MetadataStore.AddAttributeTable(attributeTableBuilder.CreateTable());
        }

        public static void RegisterValidationSummaryAttributeTable(ITypeResolver typeResolver)
        {
            AttributeTableBuilder attributeTableBuilder = new AttributeTableBuilder();
            Type runtimeType = typeResolver.ResolveType(ProjectNeutralTypes.ValidationSummary).RuntimeType;
            if (runtimeType != (Type)null)
                attributeTableBuilder.AddCallback(runtimeType, (AttributeCallback)(builder =>
                {
                    builder.AddCustomAttributes("Errors", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                    builder.AddCustomAttributes("Filter", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                    builder.AddCustomAttributes("Target", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                    builder.AddCustomAttributes("ErrorStyle", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                }));
            MetadataStore.AddAttributeTable(attributeTableBuilder.CreateTable());
        }

        public static void RegisterTimeUpDownAttributeTable(ITypeResolver typeResolver)
        {
            AttributeTableBuilder attributeTableBuilder = new AttributeTableBuilder();
            Type runtimeType = typeResolver.ResolveType(ProjectNeutralTypes.TimeUpDown).RuntimeType;
            if (runtimeType != (Type)null)
                attributeTableBuilder.AddCallback(runtimeType, (AttributeCallback)(builder =>
                {
                    builder.AddCustomAttributes("Culture", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                    builder.AddCustomAttributes("Format", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                    builder.AddCustomAttributes("TimeGlobalizationInfo", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                    builder.AddCustomAttributes("TimeParsers", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                }));
            MetadataStore.AddAttributeTable(attributeTableBuilder.CreateTable());
        }

        public static void RegisterPickerAttributeTable(ITypeResolver typeResolver)
        {
            AttributeTableBuilder attributeTableBuilder = new AttributeTableBuilder();
            Type runtimeType = typeResolver.ResolveType(ProjectNeutralTypes.Picker).RuntimeType;
            if (runtimeType != (Type)null)
                attributeTableBuilder.AddCallback(runtimeType, (AttributeCallback)(builder =>
                {
                    builder.AddCustomAttributes("IsDropDownOpen", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                    builder.AddCustomAttributes("MaxDropDownHeight", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                    builder.AddCustomAttributes("PopupButtonMode", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                }));
            MetadataStore.AddAttributeTable(attributeTableBuilder.CreateTable());
        }

        public static void RegisterTimePickerAttributeTable(ITypeResolver typeResolver)
        {
            AttributeTableBuilder attributeTableBuilder = new AttributeTableBuilder();
            Type runtimeType = typeResolver.ResolveType(ProjectNeutralTypes.TimePicker).RuntimeType;
            if (runtimeType != (Type)null)
                attributeTableBuilder.AddCallback(runtimeType, (AttributeCallback)(builder =>
                {
                    builder.AddCustomAttributes("Culture", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                    builder.AddCustomAttributes("Format", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                    builder.AddCustomAttributes("PopupMinutesInterval", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                    builder.AddCustomAttributes("PopupSecondsInterval", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                    builder.AddCustomAttributes("PopupTimeSelectionMode", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                    builder.AddCustomAttributes("TimeGlobalizationInfo", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                    builder.AddCustomAttributes("TimeParsers", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                }));
            MetadataStore.AddAttributeTable(attributeTableBuilder.CreateTable());
        }

        public static void RegisterNumericUpDownAttributeTable(ITypeResolver typeResolver)
        {
            AttributeTableBuilder attributeTableBuilder = new AttributeTableBuilder();
            Type runtimeType = typeResolver.ResolveType(ProjectNeutralTypes.NumericUpDown).RuntimeType;
            if (runtimeType != (Type)null)
                attributeTableBuilder.AddCallback(runtimeType, (AttributeCallback)(builder =>
                {
                    builder.AddCustomAttributes("DecimalPlaces", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                    builder.AddCustomAttributes("Increment", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                }));
            MetadataStore.AddAttributeTable(attributeTableBuilder.CreateTable());
        }

        public static void RegisterDragDropTargetAttributeTable(ITypeResolver typeResolver)
        {
            AttributeTableBuilder attributeTableBuilder = new AttributeTableBuilder();
            ITypeId[] typeIdArray = new ITypeId[3]
      {
        ProjectNeutralTypes.DataGridDragDropTarget,
        ProjectNeutralTypes.ListBoxDragDropTarget,
        ProjectNeutralTypes.TreeViewDragDropTarget
      };
            foreach (ITypeId typeId in typeIdArray)
            {
                Type runtimeType = typeResolver.ResolveType(typeId).RuntimeType;
                if (runtimeType != (Type)null)
                    attributeTableBuilder.AddCallback(runtimeType, (AttributeCallback)(builder =>
                    {
                        builder.AddCustomAttributes("AllowedSourceEffects", (Attribute)new CategoryAttribute(PresentationFrameworkStringTable.Category_Common_Properties));
                        builder.AddCustomAttributes("AllowedSourceEffects", (Attribute)new DescriptionAttribute(StringTable.DragDropTargetTypeDescription));
                        builder.AddCustomAttributes("AllowDrop", (Attribute)new EditorBrowsableAttribute(EditorBrowsableState.Always));
                    }));
            }
            MetadataStore.AddAttributeTable(attributeTableBuilder.CreateTable());
        }
    }
}
