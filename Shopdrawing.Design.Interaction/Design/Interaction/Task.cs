// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.Task
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design.Model;
using MS.Internal.Properties;
using System;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Windows.Design.Interaction
{
  public class Task
  {
    private InputBindingCollection _inputBindings;
    private CommandBindingCollection _commandBindings;
    private ToolCommandBindingCollection _toolCommandBindings;
    private Cursor _cursor;
    private string _description;
    private ModelEditingScope _editingScope;
    private Tool _activeTool;
    private HitTestFilterCallback _adornerFilter;
    private ModelHitTestFilterCallback _modelFilter;

    public HitTestFilterCallback AdornerFilter
    {
      get
      {
        return this._adornerFilter;
      }
      set
      {
        this._adornerFilter = value;
      }
    }

    public CommandBindingCollection CommandBindings
    {
      get
      {
        if (this._commandBindings == null)
          this._commandBindings = new CommandBindingCollection();
        return this._commandBindings;
      }
    }

    public Cursor Cursor
    {
      get
      {
        return this._cursor;
      }
      set
      {
        this._cursor = value;
      }
    }

    public string Description
    {
      get
      {
        if (this._description == null)
          return string.Empty;
        return this._description;
      }
      set
      {
        this._description = value;
      }
    }

    public InputBindingCollection InputBindings
    {
      get
      {
        if (this._inputBindings == null)
          this._inputBindings = new InputBindingCollection();
        return this._inputBindings;
      }
    }

    public bool IsFocused
    {
      get
      {
        return this._activeTool != null;
      }
    }

    public ModelHitTestFilterCallback ModelFilter
    {
      get
      {
        return this._modelFilter;
      }
      set
      {
        this._modelFilter = value;
      }
    }

    public ToolCommandBindingCollection ToolCommandBindings
    {
      get
      {
        if (this._toolCommandBindings == null)
          this._toolCommandBindings = new ToolCommandBindingCollection();
        return this._toolCommandBindings;
      }
    }

    public event EventHandler FocusDeactivated;

    public event EventHandler Reverted;

    public event EventHandler Completed;

    public void BeginFocus(GestureData data)
    {
      if (data == null)
        throw new ArgumentNullException("data");
      if (this._activeTool != null)
        throw new InvalidOperationException(Resources.Error_TaskAlreadyFocused);
      Tool tool = data.Context.Items.GetValue<Tool>();
      tool.SetFocusedTask(this);
      this._activeTool = tool;
      this._editingScope = data.TargetModel.BeginEdit();
    }

    public void Complete()
    {
      if (this._activeTool == null)
        throw new InvalidOperationException(Resources.Error_ObjectNotActive);
      ModelEditingScope modelEditingScope = this._editingScope;
      try
      {
        this.OnCompleted(EventArgs.Empty);
        if (modelEditingScope == null)
          return;
        if (this.Description.Length > 0)
          modelEditingScope.Description = this.Description;
        modelEditingScope.Complete();
        modelEditingScope = (ModelEditingScope) null;
      }
      finally
      {
        this._activeTool.ClearFocusedTask();
        this._activeTool = (Tool) null;
        this._editingScope = (ModelEditingScope) null;
        if (modelEditingScope != null)
          modelEditingScope.Revert();
        this.OnFocusDeactivated(EventArgs.Empty);
      }
    }

    public void Revert()
    {
      if (this._activeTool == null)
        throw new InvalidOperationException(Resources.Error_ObjectNotActive);
      try
      {
        this.OnReverted(EventArgs.Empty);
        if (this._editingScope == null)
          return;
        this._editingScope.Revert();
      }
      finally
      {
        this._activeTool.ClearFocusedTask();
        this._activeTool = (Tool) null;
        this._editingScope = (ModelEditingScope) null;
        this.OnFocusDeactivated(EventArgs.Empty);
      }
    }

    protected virtual void OnCompleted(EventArgs e)
    {
      if (this.Completed == null)
        return;
      this.Completed((object) this, e);
    }

    protected virtual void OnFocusDeactivated(EventArgs e)
    {
      if (this.FocusDeactivated == null)
        return;
      this.FocusDeactivated((object) this, e);
    }

    protected virtual void OnReverted(EventArgs e)
    {
      if (this.Reverted == null)
        return;
      this.Reverted((object) this, e);
    }
  }
}
