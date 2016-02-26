// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.PerformanceStatisticsWindow
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Microsoft.Expression.Framework.UserInterface
{
  public sealed class PerformanceStatisticsWindow : Window
  {
    private ObservableCollection<PerformanceEventStatistics> statistics = new ObservableCollection<PerformanceEventStatistics>();
    private static RoutedCommand refreshCommand = new RoutedCommand("Refresh", typeof (PerformanceStatisticsWindow));
    private static RoutedCommand resetCommand;
    private static RoutedCommand sortCommand;
    private GridViewColumnHeader lastHeaderClicked;
    private ListSortDirection lastDirection;

    public ObservableCollection<PerformanceEventStatistics> Statistics
    {
      get
      {
        return this.statistics;
      }
    }

    public static RoutedCommand RefreshCommand
    {
      get
      {
        return PerformanceStatisticsWindow.refreshCommand;
      }
    }

    public static RoutedCommand ResetCommand
    {
      get
      {
        return PerformanceStatisticsWindow.resetCommand;
      }
    }

    public static RoutedCommand SortCommand
    {
      get
      {
        return PerformanceStatisticsWindow.sortCommand;
      }
    }

    static PerformanceStatisticsWindow()
    {
      PerformanceStatisticsWindow.refreshCommand.InputGestures.Add((InputGesture) new KeyGesture(Key.F7));
      PerformanceStatisticsWindow.resetCommand = new RoutedCommand("Reset", typeof (PerformanceStatisticsWindow));
      PerformanceStatisticsWindow.resetCommand.InputGestures.Add((InputGesture) new KeyGesture(Key.F8));
      PerformanceStatisticsWindow.sortCommand = new RoutedCommand("Sort", typeof (GridViewColumnHeader));
    }

    public PerformanceStatisticsWindow()
    {
      this.Refresh();
      this.Width = 640.0;
      this.Height = 480.0;
    }

    protected override void OnInitialized(EventArgs e)
    {
      base.OnInitialized(e);
      this.ApplyTemplate();
    }

    public new bool ApplyTemplate()
    {
      if (this.Content == null)
      {
        FrameworkElement element = FileTable.GetElement("Resources\\PerformanceStatisticsWindow.xaml");
        element.DataContext = (object) this;
        this.Content = (object) element;
        this.CommandBindings.Add(new CommandBinding((ICommand) PerformanceStatisticsWindow.RefreshCommand, new ExecutedRoutedEventHandler(this.RefreshCommand_Execute)));
        this.CommandBindings.Add(new CommandBinding((ICommand) PerformanceStatisticsWindow.ResetCommand, new ExecutedRoutedEventHandler(this.ResetCommand_Execute)));
        this.CommandBindings.Add(new CommandBinding((ICommand) PerformanceStatisticsWindow.SortCommand, new ExecutedRoutedEventHandler(this.SortCommand_Execute)));
      }
      return base.ApplyTemplate();
    }

    private void RefreshCommand_Execute(object sender, ExecutedRoutedEventArgs args)
    {
      this.Refresh();
    }

    private void ResetCommand_Execute(object sender, ExecutedRoutedEventArgs args)
    {
      PerformanceUtility.Reset();
      this.Refresh();
    }

    private void SortCommand_Execute(object sender, ExecutedRoutedEventArgs args)
    {
      GridViewColumnHeader viewColumnHeader = (GridViewColumnHeader) args.OriginalSource;
      if (viewColumnHeader == null)
        return;
      ListSortDirection direction = ListSortDirection.Ascending;
      if (viewColumnHeader == this.lastHeaderClicked && this.lastDirection == ListSortDirection.Ascending)
        direction = ListSortDirection.Descending;
      Binding binding = viewColumnHeader.Column.DisplayMemberBinding as Binding;
      if (binding == null)
        return;
      string sortBy = binding.Path.Path;
      if (sortBy.Contains("."))
        sortBy = sortBy.Substring(0, sortBy.IndexOf("."));
      this.Sort(sortBy, direction);
      this.lastHeaderClicked = viewColumnHeader;
      this.lastDirection = direction;
    }

    private void Sort(string sortBy, ListSortDirection direction)
    {
      ICollectionView defaultView = CollectionViewSource.GetDefaultView((object) this.statistics);
      defaultView.SortDescriptions.Clear();
      SortDescription sortDescription = new SortDescription(sortBy, direction);
      defaultView.SortDescriptions.Add(sortDescription);
      defaultView.Refresh();
    }

    private void Refresh()
    {
      this.statistics.Clear();
      foreach (PerformanceEventStatistics performanceEventStatistics in PerformanceUtility.Statistics)
      {
        if (performanceEventStatistics.EventCount > 0)
          this.statistics.Add(performanceEventStatistics);
      }
    }
  }
}
