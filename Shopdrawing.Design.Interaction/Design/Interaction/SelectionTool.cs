// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.SelectionTool
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using MS.Internal;
using System.Windows.Input;

namespace Microsoft.Windows.Design.Interaction
{
  public class SelectionTool : Tool
  {
    private bool isLocalSelectAll = true;

    public SelectionTool()
    {
      Task task = new Task();
      task.CommandBindings.Add(new CommandBinding((ICommand) DesignerCommands.Cancel, new ExecutedRoutedEventHandler(this.OnSelectParent)));
      task.CommandBindings.Add(new CommandBinding((ICommand) SelectionCommands.SelectAll, new ExecutedRoutedEventHandler(this.OnSelectAll)));
      task.CommandBindings.Add(new CommandBinding((ICommand) SelectionCommands.SelectNext, new ExecutedRoutedEventHandler(this.OnSelectNext)));
      task.CommandBindings.Add(new CommandBinding((ICommand) SelectionCommands.SelectPrevious, new ExecutedRoutedEventHandler(this.OnSelectPrevious)));
      task.ToolCommandBindings.Add(new ToolCommandBinding(SelectionCommands.SelectTarget, new ExecutedToolEventHandler(this.OnSelectObject)));
      task.ToolCommandBindings.Add(new ToolCommandBinding(SelectionCommands.SelectOnlyTarget, new ExecutedToolEventHandler(this.OnSelectOnlyObject)));
      task.ToolCommandBindings.Add(new ToolCommandBinding(SelectionCommands.ToggleSelectTarget, new ExecutedToolEventHandler(this.OnToggleSelectObject)));
      task.ToolCommandBindings.Add(new ToolCommandBinding(SelectionCommands.UnionSelectTarget, new ExecutedToolEventHandler(this.OnUnionSelectObject)));
      task.ToolCommandBindings.Add(new ToolCommandBinding(SelectionCommands.ShowEvent, new ExecutedToolEventHandler(this.OnShowEvent)));
      task.InputBindings.Add(new InputBinding((ICommand) DesignerCommands.Cancel, (InputGesture) new KeyGesture(System.Windows.Input.Key.Escape)));
      task.InputBindings.Add(new InputBinding((ICommand) SelectionCommands.SelectAll, (InputGesture) new KeyGesture(System.Windows.Input.Key.A, ModifierKeys.Control)));
      task.InputBindings.Add(new InputBinding((ICommand) SelectionCommands.SelectNext, (InputGesture) new KeyGesture(System.Windows.Input.Key.Tab)));
      task.InputBindings.Add(new InputBinding((ICommand) SelectionCommands.SelectPrevious, (InputGesture) new KeyGesture(System.Windows.Input.Key.Tab, ModifierKeys.Shift)));
      task.InputBindings.Add(new InputBinding((ICommand) SelectionCommands.ShowEvent, (InputGesture) new KeyGesture(System.Windows.Input.Key.Return)));
      task.InputBindings.Add(new InputBinding((ICommand) SelectionCommands.ShowEvent, (InputGesture) new ToolGesture(ToolAction.DoubleClick)));
      this.Tasks.Add(task);
    }

    protected override void OnActivate(Tool previousTool)
    {
      base.OnActivate(previousTool);
      this.Context.Items.Subscribe<Selection>(new SubscribeContextCallback<Selection>(this.OnSelectionChanged));
    }

    protected override void OnDeactivate()
    {
      base.OnDeactivate();
      this.Context.Items.Unsubscribe<Selection>(new SubscribeContextCallback<Selection>(this.OnSelectionChanged));
    }

    private void OnSelectParent(object sender, ExecutedRoutedEventArgs e)
    {
      Performance.StartTiming(PerformanceMarks.SelectionChange);
      SelectionImplementation.SelectParent(this.Context);
      Performance.StopTiming(PerformanceMarks.SelectionChange);
    }

    private void OnSelectAll(object sender, ExecutedRoutedEventArgs e)
    {
      Performance.StartTiming(PerformanceMarks.SelectAll);
      SelectionImplementation.SelectAll(this.Context, this.isLocalSelectAll);
      this.isLocalSelectAll = false;
      Performance.StopTiming(PerformanceMarks.SelectAll);
    }

    private void OnSelectNext(object sender, ExecutedRoutedEventArgs e)
    {
      Performance.StartTiming(PerformanceMarks.SelectNext);
      SelectionImplementation.SelectNext(this.Context);
      Performance.StopTiming(PerformanceMarks.SelectNext);
    }

    private void OnSelectPrevious(object sender, ExecutedRoutedEventArgs e)
    {
      Performance.StartTiming(PerformanceMarks.SelectPrevious);
      SelectionImplementation.SelectPrevious(this.Context);
      Performance.StopTiming(PerformanceMarks.SelectPrevious);
    }

    private void OnSelectionChanged(Selection s)
    {
      this.isLocalSelectAll = true;
    }

    private void OnSelectObject(object sender, ExecutedToolEventArgs e)
    {
      GestureData gestureData = GestureData.FromEventArgs(e);
      if (gestureData.ImpliedSource == null)
        return;
      Performance.StartTiming(PerformanceMarks.SelectionChange);
      SelectionOperations.Select(this.Context, gestureData.ImpliedSource);
      Performance.StopTiming(PerformanceMarks.SelectionChange);
    }

    private void OnSelectOnlyObject(object sender, ExecutedToolEventArgs e)
    {
      GestureData gestureData = GestureData.FromEventArgs(e);
      if (gestureData.ImpliedSource == null)
        return;
      Performance.StartTiming(PerformanceMarks.SelectionChange);
      SelectionOperations.SelectOnly(this.Context, gestureData.ImpliedSource);
      Performance.StopTiming(PerformanceMarks.SelectionChange);
    }

    private void OnToggleSelectObject(object sender, ExecutedToolEventArgs e)
    {
      Performance.StartTiming(PerformanceMarks.SelectionChange);
      SelectionOperations.Toggle(this.Context, GestureData.FromEventArgs(e).ImpliedSource);
      Performance.StopTiming(PerformanceMarks.SelectionChange);
    }

    private void OnUnionSelectObject(object sender, ExecutedToolEventArgs e)
    {
      Performance.StartTiming(PerformanceMarks.SelectionChange);
      SelectionOperations.Union(this.Context, GestureData.FromEventArgs(e).ImpliedSource);
      Performance.StopTiming(PerformanceMarks.SelectionChange);
    }

    private void OnShowEvent(object sender, ExecutedToolEventArgs e)
    {
      SelectionImplementation.ShowDefaultEvent(this.Context);
    }
  }
}
