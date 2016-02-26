// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.ExpandoList
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Framework.Diagnostics;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Microsoft.Expression.Framework.UserInterface
{
  [ContentProperty("Items")]
  public class ExpandoList : ItemsControl, IConfigurationSite
  {
    public static readonly DependencyProperty ExpandoListItemStyleProperty = DependencyProperty.Register("ExpandoListItemStyle", typeof (Style), typeof (ExpandoList), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsMeasure));
    public static readonly DependencyProperty ExpandoDefinitionStyleProperty = DependencyProperty.Register("ExpandoDefinitionStyle", typeof (Style), typeof (ExpandoList), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsMeasure));
    public static readonly DependencyProperty ExpandoSplitterStyleProperty = DependencyProperty.Register("ExpandoSplitterStyle", typeof (Style), typeof (ExpandoList), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsMeasure));
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof (Orientation), typeof (ExpandoList), (PropertyMetadata) new FrameworkPropertyMetadata((object) Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsMeasure));
    private Grid previousGrid;

    public Style ExpandoListItemStyle
    {
      get
      {
        return (Style) this.GetValue(ExpandoList.ExpandoListItemStyleProperty);
      }
      set
      {
        this.SetValue(ExpandoList.ExpandoListItemStyleProperty, (object) value);
      }
    }

    public Style ExpandoDefinitionStyle
    {
      get
      {
        return (Style) this.GetValue(ExpandoList.ExpandoDefinitionStyleProperty);
      }
      set
      {
        this.SetValue(ExpandoList.ExpandoDefinitionStyleProperty, (object) value);
      }
    }

    public Style ExpandoSplitterStyle
    {
      get
      {
        return (Style) this.GetValue(ExpandoList.ExpandoSplitterStyleProperty);
      }
      set
      {
        this.SetValue(ExpandoList.ExpandoSplitterStyleProperty, (object) value);
      }
    }

    public Orientation Orientation
    {
      get
      {
        return (Orientation) this.GetValue(ExpandoList.OrientationProperty);
      }
      set
      {
        this.SetValue(ExpandoList.OrientationProperty, (object) value);
      }
    }

    protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
    {
      if (e.OldItems != null)
      {
        foreach (ExpandoListItemModel expandoListItemModel in (IEnumerable) e.OldItems)
          expandoListItemModel.PropertyChanged -= new PropertyChangedEventHandler(this.Model_PropertyChanged);
      }
      if (e.NewItems != null)
      {
        foreach (ExpandoListItemModel expandoListItemModel in (IEnumerable) e.NewItems)
          expandoListItemModel.PropertyChanged += new PropertyChangedEventHandler(this.Model_PropertyChanged);
      }
      this.UpdateGrid();
      base.OnItemsChanged(e);
    }

    public override void OnApplyTemplate()
    {
      this.UpdateGrid();
      base.OnApplyTemplate();
    }

    protected override void OnTemplateChanged(ControlTemplate oldTemplate, ControlTemplate newTemplate)
    {
      if (this.previousGrid == null)
        return;
      foreach (FrameworkElement frameworkElement in this.previousGrid.Children)
      {
        ExpandoListItem expandoListItem = frameworkElement as ExpandoListItem;
        if (expandoListItem != null)
          expandoListItem.DataContext = (object) null;
      }
      this.previousGrid = (Grid) null;
      base.OnTemplateChanged(oldTemplate, newTemplate);
    }

    private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "IsForcedInvisible"))
        return;
      this.UpdateGrid();
    }

    private void UpdateGrid()
    {
      Grid grid = this.GetTemplateChild("PART_MainGrid") as Grid;
      bool flag = this.Orientation == Orientation.Vertical;
      if (grid == null)
        return;
      this.previousGrid = grid;
      foreach (FrameworkElement frameworkElement in grid.Children)
      {
        ExpandoListItem expandoListItem = frameworkElement as ExpandoListItem;
        if (expandoListItem != null)
          expandoListItem.Content = (object) null;
      }
      grid.Children.Clear();
      grid.ColumnDefinitions.Clear();
      grid.RowDefinitions.Clear();
      int num = 0;
      for (int index = 0; index < this.Items.Count; ++index)
      {
        ExpandoListItemModel expandoListItemModel = this.Items[index] as ExpandoListItemModel;
        if (expandoListItemModel == null || !expandoListItemModel.IsForcedInvisible)
        {
          ExpandoListItem expandoListItem = new ExpandoListItem();
          expandoListItem.SetValue(AutomationElement.IdProperty, (object) ("Expando_Item_" + (object) index));
          expandoListItem.DataContext = (object) expandoListItemModel;
          expandoListItem.Style = this.ExpandoListItemStyle;
          if (flag)
            Grid.SetRow((UIElement) expandoListItem, num);
          else
            Grid.SetColumn((UIElement) expandoListItem, num);
          grid.Children.Add((UIElement) expandoListItem);
          if (flag)
          {
            RowDefinition rowDefinition = new RowDefinition();
            rowDefinition.DataContext = (object) expandoListItemModel;
            rowDefinition.Style = this.ExpandoDefinitionStyle;
            grid.RowDefinitions.Add(rowDefinition);
          }
          else
          {
            ColumnDefinition columnDefinition = new ColumnDefinition();
            columnDefinition.DataContext = (object) expandoListItemModel;
            columnDefinition.Style = this.ExpandoDefinitionStyle;
            grid.ColumnDefinitions.Add(columnDefinition);
          }
          if (grid.Children.Count > 1)
          {
            ExpandoSplitter expandoSplitter = new ExpandoSplitter();
            expandoSplitter.SetValue(AutomationElement.IdProperty, (object) ("Expando_Splitter_" + (object) index));
            expandoSplitter.Style = this.ExpandoSplitterStyle;
            if (flag)
            {
              expandoSplitter.VerticalAlignment = VerticalAlignment.Top;
              Grid.SetRow((UIElement) expandoSplitter, num);
            }
            else
            {
              expandoSplitter.HorizontalAlignment = HorizontalAlignment.Left;
              Grid.SetColumn((UIElement) expandoSplitter, num);
            }
            grid.Children.Add((UIElement) expandoSplitter);
          }
          ++num;
        }
      }
    }

    public void ReadFromConfiguration(IConfigurationObject configurationObject)
    {
      for (int index = 0; index < this.Items.Count; ++index)
      {
        ExpandoListItemModel expandoListItemModel = this.Items[index] as ExpandoListItemModel;
        if (expandoListItemModel != null)
        {
          IConfigurationObject configurationObject1 = (IConfigurationObject) configurationObject.GetProperty(expandoListItemModel.Identifier);
          if (configurationObject1 != null)
            expandoListItemModel.ReadFromConfiguration(configurationObject1);
        }
      }
    }

    public void WriteToConfiguration(IConfigurationObject configurationObject)
    {
      for (int index = 0; index < this.Items.Count; ++index)
      {
        ExpandoListItemModel expandoListItemModel = this.Items[index] as ExpandoListItemModel;
        if (expandoListItemModel != null)
        {
          IConfigurationObject configurationObject1 = (IConfigurationObject) configurationObject.GetProperty(expandoListItemModel.Identifier);
          if (configurationObject1 == null)
          {
            configurationObject1 = configurationObject.CreateConfigurationObject();
            configurationObject.SetProperty(expandoListItemModel.Identifier, (object) configurationObject1);
          }
          expandoListItemModel.WriteToConfiguration(configurationObject1);
        }
      }
    }
  }
}
