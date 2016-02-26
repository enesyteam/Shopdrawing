// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI.StoryboardPicker
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Data;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class StoryboardPicker : Border, INotifyPropertyChanged, IComponentConnector
  {
    public static readonly DependencyProperty CausesDismissProperty = DependencyProperty.RegisterAttached("CausesDismiss", typeof (bool), typeof (StoryboardPicker), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, FrameworkPropertyMetadataOptions.Inherits));
    private TimelinePane timelinePane;
    private string filterString;
    private ICommand clearFilterStringCommand;
    internal StoryboardPicker StoryboardPickerRoot;
    internal ClearableTextBox SearchTextBox;
    internal ListBox StoryboardsListView;
    private bool _contentLoaded;

    public TimelinePane TimelinePane
    {
      get
      {
        return this.timelinePane;
      }
    }

    public string FilterString
    {
      get
      {
        return this.filterString;
      }
      set
      {
        if (!(this.filterString != value))
          return;
        this.filterString = value;
        foreach (object obj in this.TimelinePane.StoryboardsView.SourceCollection)
        {
          TimelineHeader timelineHeader = obj as TimelineHeader;
          if (timelineHeader != null)
            timelineHeader.MatchesFilter = string.IsNullOrEmpty(this.filterString) || timelineHeader.Name.IndexOf(this.filterString, StringComparison.OrdinalIgnoreCase) >= 0;
        }
        this.SendOnPropertyChanged("FilterString");
      }
    }

    public ICommand ClearFilterStringCommand
    {
      get
      {
        if (this.clearFilterStringCommand == null)
          this.clearFilterStringCommand = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.ClearFilterString));
        return this.clearFilterStringCommand;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public StoryboardPicker(TimelinePane timelinePane)
    {
      this.timelinePane = timelinePane;
      this.DataContext = (object) this;
      this.InitializeComponent();
    }

    public static bool GetCausesDismiss(DependencyObject target)
    {
      return (bool) target.GetValue(StoryboardPicker.CausesDismissProperty);
    }

    public static void SetCausesDismiss(DependencyObject target, bool value)
    {
      target.SetValue(StoryboardPicker.CausesDismissProperty, (object) (bool) (value ? true : false));
    }

    private void ClearFilterString()
    {
      this.FilterString = (string) null;
    }

    internal void OnOpened()
    {
      this.FilterString = (string) null;
      this.StoryboardsListView.ScrollIntoView(this.TimelinePane.StoryboardsView.CurrentItem);
    }

    protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
    {
      base.OnPreviewMouseDown(e);
      DependencyObject target = e.OriginalSource as DependencyObject;
      if (target == null || !StoryboardPicker.GetCausesDismiss(target))
        return;
      this.timelinePane.CloseStoryboardPickerPopup();
    }

    private void SendOnPropertyChanged(string name)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(name));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/timeline/ui/storyboardpicker.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
      return Delegate.CreateDelegate(delegateType, (object) this, handler);
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.StoryboardPickerRoot = (StoryboardPicker) target;
          break;
        case 2:
          this.SearchTextBox = (ClearableTextBox) target;
          break;
        case 3:
          this.StoryboardsListView = (ListBox) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
