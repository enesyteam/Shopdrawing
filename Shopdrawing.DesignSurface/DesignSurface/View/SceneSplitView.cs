// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.SceneSplitView
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Code;
using Microsoft.Expression.DesignSurface.UserInterface;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Microsoft.Expression.DesignSurface.View
{
  internal sealed class SceneSplitView : Grid
  {
    private const double SplitViewRowMinHeight = 150.0;
    private SceneView sceneView;
    private ViewOptionsModel viewOptionsModel;
    private SceneScrollViewer scrollViewer;
    private ITextEditor codeEditor;
    private GridSplitter splitViewSplitter;
    private RowDefinition topRow;
    private RowDefinition bottomRow;
    private RowDefinition autoRow;
    private ColumnDefinition leftColumn;
    private ColumnDefinition rightColumn;
    private ColumnDefinition autoColumn;

    internal SceneSplitView(SceneView sceneView, ViewOptionsModel viewOptionsModel, SceneScrollViewer scrollViewer, ITextEditor codeEditor)
    {
      this.sceneView = sceneView;
      this.viewOptionsModel = viewOptionsModel;
      this.scrollViewer = scrollViewer;
      this.codeEditor = codeEditor;
      this.topRow = new RowDefinition();
      this.topRow.MinHeight = 150.0;
      this.RowDefinitions.Add(this.topRow);
      this.autoRow = new RowDefinition();
      this.autoRow.Height = GridLength.Auto;
      this.RowDefinitions.Add(this.autoRow);
      this.bottomRow = new RowDefinition();
      this.bottomRow.MinHeight = 150.0;
      this.RowDefinitions.Add(this.bottomRow);
      this.SizeChanged += new SizeChangedEventHandler(this.SceneSplitView_SizeChanged);
      this.leftColumn = new ColumnDefinition();
      this.ColumnDefinitions.Add(this.leftColumn);
      this.autoColumn = new ColumnDefinition();
      this.autoColumn.Width = GridLength.Auto;
      this.ColumnDefinitions.Add(this.autoColumn);
      this.rightColumn = new ColumnDefinition();
      this.ColumnDefinitions.Add(this.rightColumn);
      this.Children.Add((UIElement) this.scrollViewer);
      this.splitViewSplitter = new GridSplitter();
      this.splitViewSplitter.HorizontalAlignment = HorizontalAlignment.Stretch;
      this.splitViewSplitter.VerticalAlignment = VerticalAlignment.Stretch;
      this.splitViewSplitter.DragCompleted += new DragCompletedEventHandler(this.OnSplitViewSplitterDragCompleted);
      this.Children.Add((UIElement) this.splitViewSplitter);
      if (this.codeEditor == null)
        return;
      this.Children.Add((UIElement) this.codeEditor.Element);
    }

    private void SceneSplitView_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (!this.viewOptionsModel.IsVerticalSplit)
        return;
      double num1 = (e.NewSize.Height - this.autoRow.ActualHeight) * this.viewOptionsModel.SplitRatio;
      double num2 = (e.NewSize.Height - this.autoRow.ActualHeight) * (1.0 - this.viewOptionsModel.SplitRatio);
      bool flag = false;
      if (num1 < 150.0)
      {
        double num3 = 150.0 - num1;
        num1 = 150.0;
        num2 -= num3;
        flag = true;
      }
      if (num2 < 150.0)
      {
        double num3 = 150.0 - num2;
        num2 = 150.0;
        num1 -= num3;
        flag = true;
      }
      if (!flag)
        return;
      this.topRow.Height = new GridLength(num1, this.topRow.Height.GridUnitType);
      this.bottomRow.Height = new GridLength(num2, this.bottomRow.Height.GridUnitType);
    }

    public void Dispose()
    {
      this.sceneView = (SceneView) null;
      this.codeEditor = (ITextEditor) null;
      this.viewOptionsModel = (ViewOptionsModel) null;
      this.scrollViewer = (SceneScrollViewer) null;
      this.Children.Clear();
    }

    public void UpdateSplitViewState(Thickness verticalScrollBarMargin)
    {
      bool flag1 = this.viewOptionsModel == null || this.viewOptionsModel.IsVerticalSplit;
      bool flag2 = this.viewOptionsModel == null || this.viewOptionsModel.IsDesignOnTop;
      double num = this.viewOptionsModel == null ? 0.8 : this.viewOptionsModel.SplitRatio;
      switch (this.sceneView.ViewMode)
      {
        case ViewMode.Design:
          this.scrollViewer.VerticalScrollBarMargin = verticalScrollBarMargin;
          Grid.SetRow((UIElement) this.scrollViewer, 0);
          Grid.SetRowSpan((UIElement) this.scrollViewer, 3);
          Grid.SetColumn((UIElement) this.scrollViewer, 0);
          Grid.SetColumnSpan((UIElement) this.scrollViewer, 3);
          this.scrollViewer.Visibility = Visibility.Visible;
          if (this.codeEditor != null)
            this.codeEditor.Element.Visibility = Visibility.Collapsed;
          this.splitViewSplitter.Visibility = Visibility.Collapsed;
          break;
        case ViewMode.Code:
          this.codeEditor.VerticalScrollBarMargin = verticalScrollBarMargin;
          Grid.SetRow((UIElement) this.codeEditor.Element, 0);
          Grid.SetRowSpan((UIElement) this.codeEditor.Element, 3);
          Grid.SetColumn((UIElement) this.codeEditor.Element, 0);
          Grid.SetColumnSpan((UIElement) this.codeEditor.Element, 3);
          this.scrollViewer.Visibility = Visibility.Collapsed;
          this.codeEditor.Element.Visibility = Visibility.Visible;
          this.splitViewSplitter.Visibility = Visibility.Collapsed;
          break;
        case ViewMode.Split:
          if (flag1)
          {
            if (flag2)
            {
              this.scrollViewer.VerticalScrollBarMargin = verticalScrollBarMargin;
              this.codeEditor.VerticalScrollBarMargin = new Thickness();
            }
            else
            {
              this.scrollViewer.VerticalScrollBarMargin = new Thickness();
              this.codeEditor.VerticalScrollBarMargin = verticalScrollBarMargin;
            }
            Grid.SetRow((UIElement) this.scrollViewer, flag2 ? 0 : 2);
            Grid.SetRowSpan((UIElement) this.scrollViewer, 1);
            Grid.SetColumn((UIElement) this.scrollViewer, 0);
            Grid.SetColumnSpan((UIElement) this.scrollViewer, 3);
            Grid.SetRow((UIElement) this.codeEditor.Element, flag2 ? 2 : 0);
            Grid.SetRowSpan((UIElement) this.codeEditor.Element, 1);
            Grid.SetColumn((UIElement) this.codeEditor.Element, 0);
            Grid.SetColumnSpan((UIElement) this.codeEditor.Element, 3);
            this.splitViewSplitter.ResizeDirection = GridResizeDirection.Rows;
            Grid.SetRow((UIElement) this.splitViewSplitter, 1);
            Grid.SetRowSpan((UIElement) this.splitViewSplitter, 1);
            Grid.SetColumn((UIElement) this.splitViewSplitter, 0);
            Grid.SetColumnSpan((UIElement) this.splitViewSplitter, 3);
            this.topRow.Height = new GridLength(num, GridUnitType.Star);
            this.bottomRow.Height = new GridLength(1.0 - num, GridUnitType.Star);
          }
          else
          {
            if (!flag2)
            {
              this.scrollViewer.VerticalScrollBarMargin = verticalScrollBarMargin;
              this.codeEditor.VerticalScrollBarMargin = new Thickness();
            }
            else
            {
              this.scrollViewer.VerticalScrollBarMargin = new Thickness();
              this.codeEditor.VerticalScrollBarMargin = verticalScrollBarMargin;
            }
            Grid.SetRow((UIElement) this.scrollViewer, 0);
            Grid.SetRowSpan((UIElement) this.scrollViewer, 3);
            Grid.SetColumn((UIElement) this.scrollViewer, flag2 ? 0 : 2);
            Grid.SetColumnSpan((UIElement) this.scrollViewer, 1);
            Grid.SetRow((UIElement) this.codeEditor.Element, 0);
            Grid.SetRowSpan((UIElement) this.codeEditor.Element, 3);
            Grid.SetColumn((UIElement) this.codeEditor.Element, flag2 ? 2 : 0);
            Grid.SetColumnSpan((UIElement) this.codeEditor.Element, 1);
            this.splitViewSplitter.ResizeDirection = GridResizeDirection.Columns;
            Grid.SetRow((UIElement) this.splitViewSplitter, 0);
            Grid.SetRowSpan((UIElement) this.splitViewSplitter, 3);
            Grid.SetColumn((UIElement) this.splitViewSplitter, 1);
            Grid.SetColumnSpan((UIElement) this.splitViewSplitter, 1);
            this.leftColumn.Width = new GridLength(num, GridUnitType.Star);
            this.rightColumn.Width = new GridLength(1.0 - num, GridUnitType.Star);
          }
          this.scrollViewer.Visibility = Visibility.Visible;
          this.codeEditor.Element.Visibility = Visibility.Visible;
          this.splitViewSplitter.Visibility = Visibility.Visible;
          break;
      }
    }

    private void OnSplitViewSplitterDragCompleted(object sender, DragCompletedEventArgs e)
    {
      if (this.viewOptionsModel == null)
        return;
      if (this.viewOptionsModel.IsVerticalSplit)
        this.viewOptionsModel.SplitRatio = this.topRow.Height.Value / (this.bottomRow.Height.Value + this.topRow.Height.Value);
      else
        this.viewOptionsModel.SplitRatio = this.leftColumn.Width.Value / (this.leftColumn.Width.Value + this.rightColumn.Width.Value);
    }
  }
}
