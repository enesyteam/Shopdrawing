// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Diagnostics.TreeStatisticsViewer
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;

namespace Microsoft.Expression.Framework.Diagnostics
{
  public class TreeStatisticsViewer
  {
    private ObservableCollection<DumpNode> filtered = new ObservableCollection<DumpNode>();
    private string filter = string.Empty;
    private const string XamlString = "\r\n<Window xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' \r\n\t\txmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' \r\n\t\tTitle='Tree Statistics' \r\n\t\txmlns:d='clr-namespace:Microsoft.Expression.Framework.Diagnostics;assembly=Microsoft.Expression.Framework' SnapsToDevicePixels='true' Width='640' Height='480'>\r\n\t<Grid>\r\n\t\t<Grid.Resources>\r\n\t\t\t<d:DumpNodeToBrushConverter x:Key='DumpNodeToBrushConverter' />\r\n\t\t\t<d:DumpNodeToTypeListConverter x:Key='DumpNodeToTypeListConverter' />\r\n\t\t\t<d:DumpNodeToTemplateListConverter x:Key='DumpNodeToTemplateListConverter' />\r\n\t\t</Grid.Resources>\r\n\r\n\t\t<Grid.ColumnDefinitions>\t\r\n\t\t\t<ColumnDefinition Width='2*'/>\r\n\t\t\t<ColumnDefinition/>\r\n\t\t\t<ColumnDefinition/>\r\n\t\t</Grid.ColumnDefinitions>\r\n\t\t<Grid.RowDefinitions>\r\n\t\t\t<RowDefinition Height='24'/>\r\n\t\t\t<RowDefinition/>\r\n\t\t\t<RowDefinition/>\r\n\t\t</Grid.RowDefinitions>\r\n\t\t<TextBox Text='{Binding Path=Filter, UpdateSourceTrigger=PropertyChanged}'/>\r\n\t\t<TextBlock Grid.Column='1' VerticalAlignment='Center' Text='Types:'/>\r\n\t\t<TextBlock Grid.Column='2' VerticalAlignment='Center' Text='Template Costs:'/>\r\n\t\t<TreeView Name='Tree' Grid.Row='1' Grid.RowSpan='2' ItemsSource='{Binding Filtered}' HorizontalAlignment='Stretch'>\r\n\t\t\t<TreeView.Resources>\r\n\t\t\t\t<HierarchicalDataTemplate DataType='{x:Type d:DumpNode}' ItemsSource='{Binding Children}'>\r\n\t\t\t\t\t<TextBlock Text='{Binding}' HorizontalAlignment='Stretch' Background='Transparent'/>\r\n\t\t\t\t</HierarchicalDataTemplate>\r\n\t\t\t</TreeView.Resources>\r\n\t\t</TreeView>\r\n\t\t<ListBox Grid.Row='1' Grid.Column='1' ItemsSource='{Binding ElementName=Tree, Path=SelectedItem, Converter={StaticResource DumpNodeToTypeListConverter}}' />\r\n\t\t<ListBox Grid.Row='1' Grid.Column='2' ItemsSource='{Binding ElementName=Tree, Path=SelectedItem, Converter={StaticResource DumpNodeToTemplateListConverter}}' />\r\n\t\t<Rectangle Grid.Column='1' Grid.ColumnSpan='2' Grid.Row='2' Fill='{Binding ElementName=Tree, Path=SelectedItem, Converter={StaticResource DumpNodeToBrushConverter}}' />\r\n\t</Grid>\r\n</Window>\r\n";
    private Window window;
    private DumpNode root;

    public ObservableCollection<DumpNode> Filtered
    {
      get
      {
        return this.filtered;
      }
    }

    public string Filter
    {
      get
      {
        return this.filter;
      }
      set
      {
        this.filter = value;
        this.filtered.Clear();
        if (this.filter.Length == 0)
          this.filtered.Add(this.root);
        else
          this.FilterTree(this.root, value.ToLower());
      }
    }

    private TreeStatisticsViewer()
    {
      this.window = XamlReader.Load((XmlReader) new XmlTextReader((TextReader) new StringReader("\r\n<Window xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' \r\n\t\txmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' \r\n\t\tTitle='Tree Statistics' \r\n\t\txmlns:d='clr-namespace:Microsoft.Expression.Framework.Diagnostics;assembly=Microsoft.Expression.Framework' SnapsToDevicePixels='true' Width='640' Height='480'>\r\n\t<Grid>\r\n\t\t<Grid.Resources>\r\n\t\t\t<d:DumpNodeToBrushConverter x:Key='DumpNodeToBrushConverter' />\r\n\t\t\t<d:DumpNodeToTypeListConverter x:Key='DumpNodeToTypeListConverter' />\r\n\t\t\t<d:DumpNodeToTemplateListConverter x:Key='DumpNodeToTemplateListConverter' />\r\n\t\t</Grid.Resources>\r\n\r\n\t\t<Grid.ColumnDefinitions>\t\r\n\t\t\t<ColumnDefinition Width='2*'/>\r\n\t\t\t<ColumnDefinition/>\r\n\t\t\t<ColumnDefinition/>\r\n\t\t</Grid.ColumnDefinitions>\r\n\t\t<Grid.RowDefinitions>\r\n\t\t\t<RowDefinition Height='24'/>\r\n\t\t\t<RowDefinition/>\r\n\t\t\t<RowDefinition/>\r\n\t\t</Grid.RowDefinitions>\r\n\t\t<TextBox Text='{Binding Path=Filter, UpdateSourceTrigger=PropertyChanged}'/>\r\n\t\t<TextBlock Grid.Column='1' VerticalAlignment='Center' Text='Types:'/>\r\n\t\t<TextBlock Grid.Column='2' VerticalAlignment='Center' Text='Template Costs:'/>\r\n\t\t<TreeView Name='Tree' Grid.Row='1' Grid.RowSpan='2' ItemsSource='{Binding Filtered}' HorizontalAlignment='Stretch'>\r\n\t\t\t<TreeView.Resources>\r\n\t\t\t\t<HierarchicalDataTemplate DataType='{x:Type d:DumpNode}' ItemsSource='{Binding Children}'>\r\n\t\t\t\t\t<TextBlock Text='{Binding}' HorizontalAlignment='Stretch' Background='Transparent'/>\r\n\t\t\t\t</HierarchicalDataTemplate>\r\n\t\t\t</TreeView.Resources>\r\n\t\t</TreeView>\r\n\t\t<ListBox Grid.Row='1' Grid.Column='1' ItemsSource='{Binding ElementName=Tree, Path=SelectedItem, Converter={StaticResource DumpNodeToTypeListConverter}}' />\r\n\t\t<ListBox Grid.Row='1' Grid.Column='2' ItemsSource='{Binding ElementName=Tree, Path=SelectedItem, Converter={StaticResource DumpNodeToTemplateListConverter}}' />\r\n\t\t<Rectangle Grid.Column='1' Grid.ColumnSpan='2' Grid.Row='2' Fill='{Binding ElementName=Tree, Path=SelectedItem, Converter={StaticResource DumpNodeToBrushConverter}}' />\r\n\t</Grid>\r\n</Window>\r\n"))) as Window;
      this.window.DataContext = (object) this;
      this.window.Activated += new EventHandler(this.window_Activated);
      this.Filter = "";
      this.window.Show();
    }

    public static void Show()
    {
      TreeStatisticsViewer statisticsViewer = new TreeStatisticsViewer();
    }

    private void window_Activated(object sender, EventArgs e)
    {
      this.Load((Visual) Application.Current.MainWindow);
    }

    private void FilterTree(DumpNode node, string filter)
    {
      foreach (DumpNode node1 in (Collection<DumpNode>) node.Children)
      {
        if (node1.Filter(filter))
          this.filtered.Add(node1);
        else
          this.FilterTree(node1, filter);
      }
    }

    private void Load(Visual rootVisual)
    {
      this.filtered.Clear();
      this.root = new DumpNode(rootVisual);
      this.root.UpdateChildrenCount();
      this.Filter = this.filter;
    }
  }
}
