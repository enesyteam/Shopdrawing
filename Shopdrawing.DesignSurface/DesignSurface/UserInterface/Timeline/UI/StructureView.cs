// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI.StructureView
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Timeline.DragDrop;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.Timeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Controls;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class StructureView : Grid, INotifyPropertyChanged, IComponentConnector
  {
    public static readonly DependencyProperty TimelinePaneProperty = DependencyProperty.Register("TimelinePane", typeof (TimelinePane), typeof (StructureView));
    public static readonly DependencyProperty ElementAreaHeightProperty = DependencyProperty.Register("ElementAreaHeight", typeof (double), typeof (StructureView), new PropertyMetadata((object) 0.0));
    private DateTime lastScrollTime;
    private DesignerContext designerContext;
    private DispatcherTimer timer;
    internal StructureView Root;
    internal Grid StructureRowTwo;
    internal Button ScopeToRootButton;
    internal TextBlock FocusedElementText;
    internal Button ToggleShowAllButton;
    internal Button ToggleLockAllButton;
    internal ScrollViewer ElementTreeViewer;
    internal ListBox ElementTree;
    internal ToggleButton ToggleSortOrder;
    internal ScrollBar treeHorizontalScrollBar;
    internal ExpressionPopup DragTip;
    internal Control DropSplitAdorner;
    private bool _contentLoaded;

    public TimelinePane TimelinePane
    {
      get
      {
        return (TimelinePane) this.GetValue(StructureView.TimelinePaneProperty);
      }
      set
      {
        this.SetValue(StructureView.TimelinePaneProperty, (object) value);
      }
    }

    public double ElementAreaHeight
    {
      get
      {
        return (double) this.GetValue(StructureView.ElementAreaHeightProperty);
      }
      set
      {
        this.SetValue(StructureView.ElementAreaHeightProperty, (object) value);
      }
    }

    public bool ShowPartsList
    {
      get
      {
        if (this.ActiveSceneViewModel != null)
          return this.ActiveSceneViewModel.PartsModel.ShowPartsList;
        return false;
      }
    }

    public TimelineItemManager TimelineItemManager
    {
      get
      {
        if (this.TimelinePane != null)
          return this.TimelinePane.TimelineItemManager;
        return (TimelineItemManager) null;
      }
    }

    public DragTipModel DragTipModel { get; private set; }

    public TimelineItem DragTargetParent { get; private set; }

    public SceneViewModel ActiveSceneViewModel
    {
      get
      {
        if (this.TimelinePane != null)
          return this.TimelinePane.ActiveSceneViewModel;
        return (SceneViewModel) null;
      }
    }

    public ScrollBar TreeHorizontalScrollBar
    {
      get
      {
        return this.treeHorizontalScrollBar;
      }
    }

    public bool SortByZOrder
    {
      get
      {
        if (this.TimelineItemManager != null)
          return this.TimelineItemManager.SortByZOrder;
        return false;
      }
      set
      {
        if (this.TimelineItemManager == null)
          return;
        this.TimelineItemManager.SortByZOrder = value;
      }
    }

    public ScrollViewer ElementTreeScroller
    {
      get
      {
        return (ScrollViewer) this.ElementTree.Template.FindName("Scroller", (FrameworkElement) this.ElementTree);
      }
    }

    public StructureViewDragDropHandler StructureViewDragDropHandler
    {
      get
      {
        return new StructureViewDragDropHandler(this);
      }
    }

    internal double IndentMultiplier
    {
      get
      {
        return (double) this.Resources[(object) "IndentMultiplier"];
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public StructureView()
    {
      this.InitializeComponent();
    }

    protected override void OnInitialized(EventArgs e)
    {
      this.Loaded += new RoutedEventHandler(this.HandleLoaded);
      this.Unloaded += new RoutedEventHandler(this.HandleUnloaded);
      base.OnInitialized(e);
    }

    private void HandleLoaded(object sender, EventArgs e)
    {
      if (this.TimelinePane == null)
        return;
      this.designerContext = this.TimelinePane.DesignerContext;
      this.designerContext.SelectionManager.LateActiveSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.SelectionManager_LateActiveSceneUpdatePhase);
      this.TimelinePane.VerticalScrollManager.RegisterViewer(this.ElementTreeScroller);
      this.SetBinding(StructureView.ElementAreaHeightProperty, (BindingBase) new Binding("ActualHeight")
      {
        Source = (object) this.ElementTreeViewer
      });
      this.ElementTreeViewer.ScrollChanged += new ScrollChangedEventHandler(this.ElementTreeViewer_ScrollChanged);
      if (this.TimelineItemManager == null)
        return;
      this.TimelineItemManager.UpdateViewport(this.ElementTreeViewer.ViewportWidth, this.ElementTreeViewer.HorizontalOffset);
    }

    private void HandleUnloaded(object sender, EventArgs e)
    {
      if (this.designerContext != null && this.designerContext.SelectionManager != null)
        this.designerContext.SelectionManager.LateActiveSceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.SelectionManager_LateActiveSceneUpdatePhase);
      this.ElementTreeViewer.ScrollChanged -= new ScrollChangedEventHandler(this.ElementTreeViewer_ScrollChanged);
    }

    private void OnMouseLeave(object e, MouseEventArgs args)
    {
      TimelineTreeRow timelineTreeRow = e as TimelineTreeRow;
      if (timelineTreeRow == null)
        return;
      ToolTip toolTip = timelineTreeRow.ToolTip as ToolTip;
      if (toolTip == null)
        return;
      toolTip.IsOpen = false;
    }

    private void ElementTreeViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
      if (e.OriginalSource != this.ElementTreeViewer)
        return;
      if (this.TimelineItemManager != null)
        this.TimelineItemManager.UpdateViewport(e.ViewportWidth, e.HorizontalOffset);
      this.TreeHorizontalScrollBar.Value = this.ElementTreeViewer.HorizontalOffset;
    }

    private void SelectionManager_LateActiveSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      if (args.IsRadicalChange || args.IsDirtyViewState(SceneViewModel.ViewStateBits.IsEditable))
        this.OnActiveViewChanged();
      if (this.ActiveSceneViewModel == null)
        return;
      if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.ElementSelection))
      {
        SceneElement primarySelection = this.ActiveSceneViewModel.ElementSelectionSet.PrimarySelection;
        if (primarySelection != null)
        {
          TimelineItem timelineItem = this.TimelineItemManager.FindTimelineItem((SceneNode) primarySelection);
          if (timelineItem != null)
            this.ElementTree.ScrollIntoView((object) timelineItem);
        }
      }
      if (!args.IsDirtyPipelineCalcState(SceneViewModel.PipelineCalcBits.TimelineSortZOrder))
        return;
      this.OnTimelineSortByZOrderChanged();
    }

    private void OnActiveViewChanged()
    {
      if (this.TimelineItemManager != null && this.ElementTreeViewer != null)
        this.TimelineItemManager.UpdateViewport(this.ElementTreeViewer.ViewportWidth, this.ElementTreeViewer.HorizontalOffset);
      this.OnPropertyChanged("SortByZOrder");
    }

    private void OnTimelineSortByZOrderChanged()
    {
      this.OnPropertyChanged("SortByZOrder");
    }

    public void ScrollIfNeeded(FrameworkElement treeItem)
    {
      if (!(DateTime.Now - this.lastScrollTime > TimeSpan.FromSeconds(0.15)))
        return;
      this.lastScrollTime = DateTime.Now;
      Transform transform = (Transform) treeItem.TransformToAncestor((Visual) this.ElementTreeViewer);
      if (transform.Value.OffsetY < 10.0)
      {
        ScrollViewer scrollViewer = (ScrollViewer) this.ElementTree.Template.FindName("Scroller", (FrameworkElement) this.ElementTree);
        scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - 1.0);
      }
      else
      {
        if (transform.Value.OffsetY <= this.ElementTreeViewer.ViewportHeight - treeItem.ActualHeight - 10.0)
          return;
        ScrollViewer scrollViewer = (ScrollViewer) this.ElementTree.Template.FindName("Scroller", (FrameworkElement) this.ElementTree);
        scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + 1.0);
      }
    }

    private void TreeHorizontalScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      if (this.ElementTreeViewer.HorizontalOffset == this.TreeHorizontalScrollBar.Value)
        return;
      this.ElementTreeViewer.ScrollToHorizontalOffset(this.TreeHorizontalScrollBar.Value);
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    internal void ShowDragFeedback(bool showDragTip, Point mouseOffset, TimelineDragDescriptor timelineDragDescriptor)
    {
      DragTipModel dragTipModel = (DragTipModel) null;
      if (showDragTip && timelineDragDescriptor != null)
        dragTipModel = DragTipModel.FromDescriptor(timelineDragDescriptor);
      if (dragTipModel != null)
      {
        if (this.DragTipModel != dragTipModel)
        {
          this.DragTipModel = dragTipModel;
          this.DragTip.IsOpen = true;
          this.OnPropertyChanged("DragTipModel");
        }
        if (this.timer != null)
          this.timer.Stop();
      }
      else
      {
        if (this.timer == null)
        {
          this.timer = new DispatcherTimer(DispatcherPriority.Input);
          this.timer.Interval = TimeSpan.FromSeconds(0.1);
          this.timer.Tick += (EventHandler) ((sender, e) =>
          {
            this.DragTip.IsOpen = false;
            this.DragTipModel = (DragTipModel) null;
            this.OnPropertyChanged("DragTipModel");
            this.timer.Stop();
          });
        }
        this.timer.Start();
      }
      if (showDragTip && this.DragTip.IsOpen)
      {
        this.DragTip.HorizontalOffset = mouseOffset.X + 10.0;
        this.DragTip.VerticalOffset = mouseOffset.Y + 20.0;
      }
      this.DragTargetParent = !showDragTip || timelineDragDescriptor == null ? (TimelineItem) null : timelineDragDescriptor.TargetParent;
      this.OnPropertyChanged("DragTargetParent");
    }

    internal void ShowSplitAdorner(bool showFeedback, TimelineDragDescriptor descriptor)
    {
      if (showFeedback && descriptor != null && (descriptor.TargetItem != null && !descriptor.HideSplitter))
      {
        TimelineItem timelineItem = (TimelineItem) null;
        bool flag = false;
        double num1 = 0.0;
        if (descriptor.AllowBetween)
        {
          timelineItem = descriptor.TargetItem;
          flag = (descriptor.ResultDropEffects & TimelineDropEffects.Before) != TimelineDropEffects.None;
          num1 = (double) (timelineItem.Depth + descriptor.RelativeDepth);
        }
        else if (descriptor.TargetItem.IsExpanded && descriptor.DropInDefaultContent)
        {
          ElementTimelineItem elementTimelineItem = descriptor.TargetItem as ElementTimelineItem;
          if (elementTimelineItem != null)
          {
            ISceneNodeCollection<SceneNode> defaultContent = elementTimelineItem.Element.DefaultContent;
            if (defaultContent != null)
            {
              if (defaultContent.Count > 0 && !this.TimelineItemManager.SortByZOrder)
              {
                flag = true;
                num1 = (double) (descriptor.TargetItem.Depth + 1);
                int index = this.TimelineItemManager.ItemList.IndexOf((TimelineItem) elementTimelineItem);
                int count = this.TimelineItemManager.ItemList.Count;
                while (index < count - 1 && (double) this.TimelineItemManager.ItemList[index + 1].Depth >= num1)
                  ++index;
                timelineItem = this.TimelineItemManager.ItemList[index];
              }
              else
              {
                timelineItem = descriptor.TargetItem;
                flag = true;
                num1 = (double) (timelineItem.Depth + 1);
              }
            }
          }
        }
        if (timelineItem != null)
        {
          Point point = ((Visual) this.ElementTree.ItemContainerGenerator.ContainerFromItem((object) timelineItem)).TransformToVisual((Visual) this).Transform(new Point(0.0, 0.0));
          bool sortByZorder = this.TimelineItemManager.SortByZOrder;
          double indentMultiplier = this.IndentMultiplier;
          double left = num1 * this.IndentMultiplier + point.X + indentMultiplier;
          double num2 = (double) ((int) (this.DropSplitAdorner.ActualHeight - 1.0) / 2);
          double num3 = (double) this.Resources[(object) "RowMinHeight"];
          double top = point.Y - num2 + (sortByZorder ^ flag ? num3 : 0.0);
          this.DropSplitAdorner.Margin = new Thickness(left, top, 0.0, 0.0);
          this.DropSplitAdorner.Visibility = Visibility.Visible;
          return;
        }
      }
      this.DropSplitAdorner.Visibility = Visibility.Collapsed;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/resources/timeline/structureview.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
      return Delegate.CreateDelegate(delegateType, (object) this, handler);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.Root = (StructureView) target;
          break;
        case 2:
          this.StructureRowTwo = (Grid) target;
          break;
        case 3:
          this.ScopeToRootButton = (Button) target;
          break;
        case 4:
          this.FocusedElementText = (TextBlock) target;
          break;
        case 5:
          this.ToggleShowAllButton = (Button) target;
          break;
        case 6:
          this.ToggleLockAllButton = (Button) target;
          break;
        case 7:
          this.ElementTreeViewer = (ScrollViewer) target;
          break;
        case 8:
          this.ElementTree = (ListBox) target;
          break;
        case 9:
          this.ToggleSortOrder = (ToggleButton) target;
          break;
        case 10:
          this.treeHorizontalScrollBar = (ScrollBar) target;
          this.treeHorizontalScrollBar.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.TreeHorizontalScrollBar_ValueChanged);
          break;
        case 11:
          this.DragTip = (ExpressionPopup) target;
          break;
        case 12:
          this.DropSplitAdorner = (Control) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
