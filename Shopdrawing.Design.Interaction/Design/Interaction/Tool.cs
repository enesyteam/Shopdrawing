// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.Tool
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using MS.Internal.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Windows.Design.Interaction
{
  public class Tool : ContextItem
  {
    private EditingContext _context;
    private Cursor _cursor;
    private Collection<Task> _tasks;
    private Task _focusedTask;
    private bool _isUpdatingTaskItem;

    public Task FocusedTask
    {
      get
      {
        return this._focusedTask;
      }
    }

    protected EditingContext Context
    {
      get
      {
        if (this._context == null)
          throw new InvalidOperationException(Resources.Error_ObjectNotActive);
        return this._context;
      }
    }

    public Cursor Cursor
    {
      get
      {
        Cursor cursor = this._cursor;
        if (this._focusedTask != null && this._focusedTask.Cursor != null)
          cursor = this._focusedTask.Cursor;
        return cursor;
      }
      set
      {
        this._cursor = value;
      }
    }

    internal bool IsUpdatingTaskItem
    {
      get
      {
        return this._isUpdatingTaskItem;
      }
    }

    public override sealed Type ItemType
    {
      get
      {
        return typeof (Tool);
      }
    }

    public Collection<Task> Tasks
    {
      get
      {
        if (this._tasks == null)
          this._tasks = (Collection<Task>) new Tool.TaskCollection();
        return this._tasks;
      }
    }

    internal void ClearFocusedTask()
    {
      this._focusedTask = (Task) null;
      this.UpdateTaskItem();
    }

    internal CommandBinding GetCommandBinding(ICommand command, DependencyObject sourceAdorner, DependencyObject targetAdorner)
    {
      foreach (Task task in this.GetTaskRoute((Task) null, sourceAdorner, targetAdorner))
      {
        foreach (CommandBinding commandBinding in task.CommandBindings)
        {
          if (commandBinding.Command.Equals((object) command))
            return commandBinding;
        }
      }
      return (CommandBinding) null;
    }

    private IEnumerable<Task> GetTaskRoute(Task sourceTask, DependencyObject sourceAdorner, DependencyObject targetAdorner)
    {
      if (this._focusedTask != null)
      {
        yield return this._focusedTask;
      }
      else
      {
        Task task = sourceTask;
        if (task != null)
          yield return task;
        if (sourceAdorner != null)
        {
          task = AdornerProperties.GetTask(sourceAdorner);
          if (task != null && task != sourceTask)
            yield return task;
        }
        if (targetAdorner != null && targetAdorner != sourceAdorner)
        {
          Task previousTask = task;
          task = AdornerProperties.GetTask(targetAdorner);
          if (task != null && task != previousTask && task != sourceTask)
            yield return task;
        }
        if (this._tasks != null)
        {
          foreach (Task task1 in this._tasks)
            yield return task1;
        }
      }
    }

    internal ToolCommandBinding GetToolCommandBinding(ICommand command, GestureData data)
    {
      foreach (Task task in this.GetTaskRoute(data.SourceTask, data.SourceAdorner, data.TargetAdorner))
      {
        foreach (ToolCommandBinding toolCommandBinding in (Collection<ToolCommandBinding>) task.ToolCommandBindings)
        {
          if (toolCommandBinding.Command.Equals((object) command))
            return toolCommandBinding;
        }
      }
      return (ToolCommandBinding) null;
    }

    protected virtual void OnActivate(Tool previousTool)
    {
    }

    protected virtual void OnDeactivate()
    {
    }

    protected override sealed void OnItemChanged(EditingContext context, ContextItem previousItem)
    {
      Tool previousTool = (Tool) previousItem;
      previousTool.OnDeactivate();
      if (previousTool.FocusedTask != null)
      {
        try
        {
          this._isUpdatingTaskItem = true;
          previousTool.FocusedTask.Revert();
        }
        finally
        {
          this._isUpdatingTaskItem = false;
        }
      }
      previousTool._context = (EditingContext) null;
      if (this._focusedTask != null)
      {
        this._focusedTask.Revert();
        this._focusedTask = (Task) null;
      }
      this._context = context;
      bool flag = false;
      try
      {
        this.OnActivate(previousTool);
        CommandManager.InvalidateRequerySuggested();
        flag = true;
      }
      finally
      {
        if (!flag)
          this._context = (EditingContext) null;
      }
    }

    internal void SetFocusedTask(Task task)
    {
      if (this._focusedTask != null)
        throw new InvalidOperationException(Resources.Error_TaskAlreadyFocused);
      this._focusedTask = task;
      bool flag = false;
      try
      {
        this.UpdateTaskItem();
        flag = true;
      }
      finally
      {
        if (!flag)
          this._focusedTask = (Task) null;
      }
    }

    private void UpdateTaskItem()
    {
      try
      {
        this._isUpdatingTaskItem = true;
        this.Context.Items.SetValue((ContextItem) new Microsoft.Windows.Design.Interaction.FocusedTask(this._focusedTask));
        CommandManager.InvalidateRequerySuggested();
      }
      finally
      {
        this._isUpdatingTaskItem = false;
      }
    }

    private class TaskCollection : Collection<Task>
    {
      protected override void InsertItem(int index, Task item)
      {
        this.InsertItem(index, item);
        CommandManager.InvalidateRequerySuggested();
      }

      protected override void ClearItems()
      {
        base.ClearItems();
        CommandManager.InvalidateRequerySuggested();
      }

      protected override void RemoveItem(int index)
      {
        base.RemoveItem(index);
        CommandManager.InvalidateRequerySuggested();
      }

      protected override void SetItem(int index, Task item)
      {
        this.SetItem(index, item);
        CommandManager.InvalidateRequerySuggested();
      }
    }
  }
}
