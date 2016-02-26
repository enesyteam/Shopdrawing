// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.UserInterface.IntellisenseMenu
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.VisualStudio.Language.Intellisense;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Microsoft.Expression.Code.UserInterface
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class IntellisenseMenu : UserControl, INotifyPropertyChanged, IComponentConnector
  {
    public static readonly DependencyProperty SessionProperty = DependencyProperty.Register("Session", typeof (ICompletionSession), typeof (IntellisenseMenu), new PropertyMetadata(new PropertyChangedCallback(IntellisenseMenu.SessionChanged)));
    private const double slideEaseValue = 0.45;
    private double scrollingDestination;
    private readonly double completionLineHeight;
    private bool shouldAnimate;
    private int scrollToCompletionIndex;
    private List<SelectableCompletion> selectableCompletions;
    private SelectableCompletion oldSelection;
    private ScrollViewer CompletionsScroller;
    internal Border Root;
    internal Grid SelectionWindow;
    internal TreeView CompletionsList;
    private bool _contentLoaded;

    public ICompletionSession Session
    {
      get
      {
        return (ICompletionSession) this.GetValue(IntellisenseMenu.SessionProperty);
      }
      set
      {
        this.SetValue(IntellisenseMenu.SessionProperty, (object) value);
      }
    }

    public IEnumerable<SelectableCompletion> Completions
    {
      get
      {
        return (IEnumerable<SelectableCompletion>) this.selectableCompletions;
      }
    }

    public CompletionSelectionStatus SelectionStatus
    {
      get
      {
        if (this.Session == null)
          return (CompletionSelectionStatus) null;
        return this.Session.SelectionStatus;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public IntellisenseMenu()
    {
      this.Loaded += (RoutedEventHandler) delegate
      {
        CompositionTarget.Rendering += new EventHandler(this.HandleTick);
        this.CompletionsScroller = (ScrollViewer) this.CompletionsList.Template.FindName("CompletionScroller", (FrameworkElement) this.CompletionsList);
      };
      this.Unloaded += (RoutedEventHandler) delegate
      {
        CompositionTarget.Rendering -= new EventHandler(this.HandleTick);
      };
      this.InitializeComponent();
      this.DataContext = (object) this;
      this.CompletionsList.PreviewMouseWheel += new MouseWheelEventHandler(this.CompletionsList_PreviewMouseWheel);
      this.CompletionsList.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(this.OnCompletionsListMouseLeftButtonDown);
      this.completionLineHeight = (double) this.Resources[(object) "CompletionLineHeight"];
    }

    private void CompletionsList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
      this.scrollToCompletionIndex += e.Delta > 0 ? -1 : 1;
      if (this.scrollToCompletionIndex < 0)
        this.scrollToCompletionIndex = 0;
      else if (this.scrollToCompletionIndex >= this.selectableCompletions.Count)
        this.scrollToCompletionIndex = this.selectableCompletions.Count - 1;
      this.shouldAnimate = true;
      e.Handled = true;
    }

    private static void SessionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      ((IntellisenseMenu) sender).OnSessionChanged(e);
    }

    private void OnSessionChanged(DependencyPropertyChangedEventArgs e)
    {
      ICompletionSession completionSession1 = (ICompletionSession) e.OldValue;
      ICompletionSession completionSession2 = (ICompletionSession) e.NewValue;
      if (completionSession1 != null)
      {
        completionSession1.Committed -= new EventHandler(this.OnCommitted);
        completionSession1.CompletionsChanged -= new EventHandler(this.OnCompletionsChanged);
        completionSession1.SelectionStatusChanged -= new EventHandler<SelectionStatusEventArgs>(this.OnSelectionStatusChanged);
      }
      if (completionSession2 == null)
        return;
      completionSession2.Committed += new EventHandler(this.OnCommitted);
      completionSession2.CompletionsChanged += new EventHandler(this.OnCompletionsChanged);
      completionSession2.SelectionStatusChanged += new EventHandler<SelectionStatusEventArgs>(this.OnSelectionStatusChanged);
      this.InitializeCompletions();
      this.FirePropertyChanged((string) null);
    }

    private void OnCommitted(object sender, EventArgs e)
    {
    }

    private void OnCompletionsChanged(object sender, EventArgs e)
    {
      if (this.Session == null)
        return;
      this.InitializeCompletions();
      this.FirePropertyChanged("Completions");
    }

    private void InitializeCompletions()
    {
      this.shouldAnimate = true;
      this.selectableCompletions = new List<SelectableCompletion>();
      foreach (ICompletion completion in this.Session.Completions)
        this.selectableCompletions.Add(new SelectableCompletion(this.Session, completion));
    }

    private void OnSelectionStatusChanged(object sender, EventArgs e)
    {
      if (this.Session == null)
        return;
      this.FirePropertyChanged("SelectionStatus");
      ICompletion selectedCompletion = this.Session.SelectionStatus.SelectedCompletion;
      if (this.oldSelection != null)
      {
        this.oldSelection.OnIsSelectedChanged();
        this.oldSelection = (SelectableCompletion) null;
      }
      foreach (SelectableCompletion selectableCompletion in this.selectableCompletions)
      {
        if (selectableCompletion.Completion == selectedCompletion)
        {
          selectableCompletion.OnIsSelectedChanged();
          this.oldSelection = selectableCompletion;
          break;
        }
      }
      this.ScrollIntoView(selectedCompletion);
    }

    private void ScrollIntoView(ICompletion completion)
    {
      this.scrollToCompletionIndex = this.Session.Completions.IndexOf(completion);
      this.shouldAnimate = true;
    }

    private void HandleTick(object sender, EventArgs e)
    {
      if (!this.shouldAnimate || this.CompletionsScroller == null)
        return;
      double num1 = this.CalculateCurrentScrollingPosition();
      bool flag = false;
      int num2 = -1;
      for (int index = 0; index < this.selectableCompletions.Count; ++index)
      {
        UIElement uiElement = this.CompletionsList.ItemContainerGenerator.ContainerFromIndex(index) as UIElement;
        if (uiElement != null)
        {
          if (num2 == -1)
            num2 = index;
          if (index == this.scrollToCompletionIndex)
          {
            flag = true;
            this.scrollingDestination = this.CompletionsScroller.VerticalOffset + uiElement.TransformToAncestor((Visual) this.CompletionsList).Transform(new Point()).Y;
          }
        }
      }
      if (!flag)
        this.scrollingDestination = this.CompletionsScroller.VerticalOffset + (double) (this.scrollToCompletionIndex - num2) * this.completionLineHeight;
      this.scrollingDestination -= 60.0;
      double num3 = (this.scrollingDestination - num1) * 0.45;
      double offset1 = num1 + num3;
      if (Math.Abs(offset1 - num1) < 3.0 && flag)
      {
        offset1 = this.scrollingDestination;
        this.shouldAnimate = false;
      }
      if (offset1 == num1)
        return;
      if (offset1 < 0.0)
      {
        this.CompletionsScroller.ScrollToVerticalOffset(0.0);
        this.SelectionWindow.Margin = new Thickness(1.0, offset1 * 2.0, 0.0, 0.0);
      }
      else if (offset1 > this.CompletionsScroller.ExtentHeight - this.CompletionsScroller.ViewportHeight)
      {
        double offset2 = Math.Max(0.0, this.CompletionsScroller.ExtentHeight - this.CompletionsScroller.ViewportHeight);
        this.CompletionsScroller.ScrollToVerticalOffset(offset2);
        this.SelectionWindow.Margin = new Thickness(1.0, 0.0, 0.0, -((offset1 - offset2) * 2.0));
      }
      else
      {
        this.CompletionsScroller.ScrollToVerticalOffset(offset1);
        this.SelectionWindow.Margin = new Thickness(1.0, 0.0, 0.0, 0.0);
      }
    }

    private double CalculateCurrentScrollingPosition()
    {
      if (this.SelectionWindow.Margin.Top < 0.0)
        return this.SelectionWindow.Margin.Top / 2.0;
      double num = Math.Max(0.0, this.CompletionsScroller.ExtentHeight - this.CompletionsScroller.ViewportHeight);
      if (this.CompletionsScroller.VerticalOffset == num)
        return num - this.SelectionWindow.Margin.Bottom / 2.0;
      return this.CompletionsScroller.VerticalOffset;
    }

    private void OnCompletionsListMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (this.Session == null)
        return;
      SelectableCompletion selectableCompletion = ((FrameworkElement) e.OriginalSource).DataContext as SelectableCompletion;
      if (selectableCompletion == null)
        return;
      this.Session.SetSelectionStatus(selectableCompletion.Completion, CompletionSelectionOptions.Selected | CompletionSelectionOptions.Unique);
      if (e.ClickCount > 1)
        this.Session.Commit();
      e.Handled = true;
    }

    private void FirePropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.Code;component/userinterface/intellisensemenu.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.Root = (Border) target;
          break;
        case 2:
          this.SelectionWindow = (Grid) target;
          break;
        case 3:
          this.CompletionsList = (TreeView) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
