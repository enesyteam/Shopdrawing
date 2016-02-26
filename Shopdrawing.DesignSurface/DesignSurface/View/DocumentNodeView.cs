// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.DocumentNodeView
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Microsoft.Expression.DesignSurface.View
{
  public class DocumentNodeView
  {
    private DocumentNodeView.NodeMapping nodeMapping = new DocumentNodeView.NodeMapping();
    private const double boxWidth = 100.0;
    private const double boxHeight = 35.0;
    private const double horizontalMargin = 20.0;
    private const double verticalMargin = 40.0;
    private Window window;
    private Canvas canvas;
    private ScrollViewer scrollViewer;
    private SceneViewModel viewModel;
    private Rectangle selectedNodeAdorner;
    private Rectangle activeNodeAdorner;
    private Polyline activePropertyAdorner;

    public DocumentNodeView(SceneViewModel viewModel)
    {
      this.canvas = new Canvas();
      this.scrollViewer = new ScrollViewer();
      this.scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
      this.scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
      this.scrollViewer.Content = (object) this.canvas;
      this.window = new Window();
      this.window.Background = (Brush) Brushes.White;
      this.window.Title = viewModel.Document.DocumentReference.DisplayName + " - DocumentNode View";
      this.window.Content = (object) this.scrollViewer;
      this.window.Closing += new CancelEventHandler(this.Window_Closing);
      this.window.Show();
      this.viewModel = viewModel;
      this.viewModel.LateSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.ViewModel_LateSceneUpdatePhase);
      this.selectedNodeAdorner = new Rectangle();
      this.selectedNodeAdorner.Stroke = (Brush) Brushes.Blue;
      this.selectedNodeAdorner.StrokeThickness = 2.0;
      this.activeNodeAdorner = new Rectangle();
      this.activeNodeAdorner.Stroke = (Brush) Brushes.Yellow;
      this.activeNodeAdorner.StrokeThickness = 7.0;
      this.activePropertyAdorner = new Polyline();
      this.activePropertyAdorner.Stroke = (Brush) Brushes.Yellow;
      this.activePropertyAdorner.StrokeThickness = 5.0;
      this.RebuildTree();
    }

    private void Window_Closing(object sender, CancelEventArgs args)
    {
      this.window.Closing -= new CancelEventHandler(this.Window_Closing);
      this.viewModel.LateSceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.ViewModel_LateSceneUpdatePhase);
    }

    private void ViewModel_LateSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      if (args.DocumentChanges.Count <= 0)
        return;
      this.RebuildTree();
    }

    private void RebuildTree()
    {
      this.canvas.Children.Clear();
      Size size = this.BuildTree(this.viewModel.DocumentRoot.RootNode, 10.0, 20.0);
      this.canvas.Width = size.Width;
      this.canvas.Height = size.Height;
    }

    private Size BuildTree(DocumentNode node, double x, double y)
    {
      DocumentNodeView.NodeButton nodeButton = new DocumentNodeView.NodeButton(node);
      nodeButton.Click += new RoutedEventHandler(this.NodeButton_Click);
      Canvas.SetLeft((UIElement) nodeButton, x);
      Canvas.SetTop((UIElement) nodeButton, y);
      this.canvas.Children.Add((UIElement) nodeButton);
      this.nodeMapping[node] = (FrameworkElement) nodeButton;
      Size size = new Size(0.0, 0.0);
      if (this.nodeMapping.IsExpanded(node))
      {
        DocumentCompositeNode documentCompositeNode = node as DocumentCompositeNode;
        if (documentCompositeNode != null)
        {
          foreach (KeyValuePair<IProperty, DocumentNode> keyValuePair in (IEnumerable<KeyValuePair<IProperty, DocumentNode>>) documentCompositeNode.Properties)
            this.BuildChildTree(node, keyValuePair.Value, x, y, false, ref size);
          if (documentCompositeNode.SupportsChildren)
          {
            foreach (DocumentNode child in (IEnumerable<DocumentNode>) documentCompositeNode.Children)
              this.BuildChildTree(node, child, x, y, false, ref size);
          }
        }
      }
      size.Width = Math.Max(size.Width, 120.0);
      size.Height += 75.0;
      return size;
    }

    private void BuildChildTree(DocumentNode node, DocumentNode child, double x, double y, bool isLinked, ref Size size)
    {
      PointCollection pointCollection1 = new PointCollection();
      pointCollection1.Add(new Point(x + 50.0, y + 35.0 + 2.0));
      pointCollection1.Add(new Point(x + 50.0, y + 35.0 + 20.0));
      pointCollection1.Add(new Point(x + size.Width + 50.0, y + 35.0 + 20.0));
      pointCollection1.Add(new Point(x + size.Width + 50.0, y + 35.0 + 40.0 - 2.0));
      Polyline polyline = new Polyline();
      polyline.Points = pointCollection1;
      polyline.Stroke = (Brush) Brushes.Black;
      polyline.StrokeThickness = 1.0;
      this.canvas.Children.Add((UIElement) polyline);
      if (isLinked)
      {
        PointCollection pointCollection2 = new PointCollection();
        pointCollection2.Add(new Point(x + size.Width + 50.0, y + 35.0 + 40.0 - 2.0));
        pointCollection2.Add(new Point(x + size.Width + 50.0 - 7.0, y + 35.0 + 40.0 - 2.0 - 15.0));
        pointCollection2.Add(new Point(x + size.Width + 50.0, y + 35.0 + 40.0 - 2.0 - 10.0));
        pointCollection2.Add(new Point(x + size.Width + 50.0 + 7.0, y + 35.0 + 40.0 - 2.0 - 15.0));
        Polygon polygon = new Polygon();
        polygon.Points = pointCollection2;
        polygon.Fill = (Brush) Brushes.Black;
        this.canvas.Children.Add((UIElement) polygon);
      }
      string str = (string) null;
      if (child.IsProperty)
        str = child.SitePropertyKey.Name;
      else if (child.Parent == node)
        str = child.Parent.Children.IndexOf(child).ToString((IFormatProvider) CultureInfo.InvariantCulture);
      if (str != null)
      {
        TextBlock textBlock = new TextBlock();
        textBlock.Text = str;
        textBlock.FontSize = 10.0;
        Canvas.SetLeft((UIElement) textBlock, x + size.Width + 50.0 + 3.0);
        Canvas.SetTop((UIElement) textBlock, y + 35.0 + 20.0 + 5.0);
        this.canvas.Children.Add((UIElement) textBlock);
      }
      Size size1 = this.BuildTree(child, x + size.Width, y + 35.0 + 40.0);
      size.Width += size1.Width;
      size.Height = Math.Max(size.Height, size1.Height);
    }

    private void NodeButton_Click(object sender, RoutedEventArgs args)
    {
      DocumentNode node = ((DocumentNodeView.NodeButton) sender).Node;
      this.nodeMapping.SetIsExpanded(node, !this.nodeMapping.IsExpanded(node));
      this.RebuildTree();
    }

    private sealed class NodeMapping
    {
      private Dictionary<DocumentNode, DocumentNodeView.NodeEntry> mapping = new Dictionary<DocumentNode, DocumentNodeView.NodeEntry>();

      public FrameworkElement this[DocumentNode node]
      {
        get
        {
          DocumentNodeView.NodeEntry nodeEntry = this.GetNodeEntry(node);
          if (nodeEntry == null)
            return (FrameworkElement) null;
          return nodeEntry.Element;
        }
        set
        {
          DocumentNodeView.NodeEntry nodeEntry = this.GetNodeEntry(node);
          if (nodeEntry != null)
            nodeEntry.Element = value;
          else
            this.mapping[node] = new DocumentNodeView.NodeEntry(value);
        }
      }

      public bool IsExpanded(DocumentNode node)
      {
        DocumentNodeView.NodeEntry nodeEntry = this.GetNodeEntry(node);
        if (nodeEntry == null)
          return true;
        return nodeEntry.IsExpanded;
      }

      public void SetIsExpanded(DocumentNode node, bool value)
      {
        DocumentNodeView.NodeEntry nodeEntry = this.GetNodeEntry(node);
        if (nodeEntry == null)
          return;
        nodeEntry.IsExpanded = value;
      }

      private DocumentNodeView.NodeEntry GetNodeEntry(DocumentNode node)
      {
        DocumentNodeView.NodeEntry nodeEntry;
        if (!this.mapping.TryGetValue(node, out nodeEntry))
          nodeEntry = (DocumentNodeView.NodeEntry) null;
        return nodeEntry;
      }
    }

    private sealed class NodeEntry
    {
      public FrameworkElement Element { get; set; }

      public bool IsExpanded { get; set; }

      public NodeEntry(FrameworkElement element)
      {
        this.Element = element;
        this.IsExpanded = true;
      }
    }

    private class NodeButton : Button
    {
      private DocumentNode node;

      public DocumentNode Node
      {
        get
        {
          return this.node;
        }
      }

      static NodeButton()
      {
        Style defaultStyle = DocumentNodeView.NodeButton.GetDefaultStyle();
        FrameworkElement.StyleProperty.OverrideMetadata(typeof (DocumentNodeView.NodeButton), (PropertyMetadata) new FrameworkPropertyMetadata((object) defaultStyle));
      }

      public NodeButton(DocumentNode node)
      {
        this.node = node;
        string str = this.node.ToString();
        this.Content = (object) str;
        this.ToolTip = (object) str;
        this.Background = this.GetBackground(this.node);
        this.ContextMenu = this.CreateContextMenu();
      }

      private ContextMenu CreateContextMenu()
      {
        ContextMenu contextMenu = (ContextMenu) null;
        DocumentCompositeNode documentCompositeNode = this.node as DocumentCompositeNode;
        if (documentCompositeNode != null)
        {
          foreach (KeyValuePair<IProperty, DocumentNode> keyValuePair in (IEnumerable<KeyValuePair<IProperty, DocumentNode>>) documentCompositeNode.Properties)
          {
            IProperty key = keyValuePair.Key;
            DocumentNode documentNode = keyValuePair.Value;
          }
          if (documentCompositeNode.SupportsChildren)
          {
            foreach (DocumentNode documentNode in (IEnumerable<DocumentNode>) documentCompositeNode.Children)
              ;
          }
        }
        return contextMenu;
      }

      private Brush GetBackground(DocumentNode node)
      {
        DocumentCompositeNode documentCompositeNode = node as DocumentCompositeNode;
        if (documentCompositeNode != null)
        {
          if (!documentCompositeNode.SupportsChildren)
            return (Brush) Brushes.LightBlue;
          return (Brush) Brushes.LightCoral;
        }
        if (node is DocumentPrimitiveNode)
          return (Brush) Brushes.LightGoldenrodYellow;
        return (Brush) Brushes.LightGray;
      }

      private static Style GetDefaultStyle()
      {
        FrameworkElementFactory child = new FrameworkElementFactory(typeof (TextBlock));
        child.SetValue(FrameworkElement.MarginProperty, (object) new Thickness(2.0));
        child.SetValue(TextBlock.FontSizeProperty, (object) 10.0);
        child.SetValue(TextBlock.TextProperty, (object) new TemplateBindingExtension(ContentControl.ContentProperty));
        child.SetValue(TextBlock.ForegroundProperty, (object) Brushes.Black);
        FrameworkElementFactory frameworkElementFactory = new FrameworkElementFactory(typeof (Border));
        frameworkElementFactory.SetValue(Border.BackgroundProperty, (object) new TemplateBindingExtension(Control.BackgroundProperty));
        frameworkElementFactory.SetValue(Border.BorderBrushProperty, (object) Brushes.Black);
        frameworkElementFactory.SetValue(Border.BorderThicknessProperty, (object) new Thickness(1.0));
        frameworkElementFactory.SetValue(UIElement.ClipToBoundsProperty, (object) true);
        frameworkElementFactory.AppendChild(child);
        Style style = new Style(typeof (DocumentNodeView.NodeButton));
        ControlTemplate controlTemplate = new ControlTemplate(typeof (DocumentNodeView.NodeButton));
        controlTemplate.VisualTree = frameworkElementFactory;
        style.Setters.Add((SetterBase) new Setter(Control.TemplateProperty, (object) controlTemplate));
        style.Setters.Add((SetterBase) new Setter(FrameworkElement.WidthProperty, (object) 100.0));
        style.Setters.Add((SetterBase) new Setter(FrameworkElement.HeightProperty, (object) 35.0));
        return style;
      }
    }
  }
}
