// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.TaskProvider
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Model;
using MS.Internal.Features;
using System.Collections.Generic;

namespace Microsoft.Windows.Design.Interaction
{
  [FeatureConnector(typeof (TaskProviderFeatureConnector))]
  public abstract class TaskProvider : FeatureProvider
  {
    private ICollection<Task> _tasks;
    private EditingContext _context;

    public ICollection<Task> Tasks
    {
      get
      {
        if (this._tasks == null)
          this._tasks = (ICollection<Task>) new List<Task>();
        return this._tasks;
      }
    }

    protected EditingContext Context
    {
      get
      {
        return this._context;
      }
    }

    public virtual bool IsToolSupported(Tool tool)
    {
      return tool is SelectionTool;
    }

    protected virtual void Activate(ModelItem item)
    {
    }

    protected virtual void Deactivate()
    {
    }

    internal void InvokeActivate(EditingContext context, ModelItem item)
    {
      this._context = context;
      this.Activate(item);
    }

    internal void InvokeDeactivate()
    {
      if (this._context == null)
        return;
      this.Deactivate();
      this._context = (EditingContext) null;
    }
  }
}
