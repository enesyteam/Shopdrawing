// Decompiled with JetBrains decompiler
// Type: MS.Internal.Features.TaskProviderFeatureConnector
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using System.Collections.Generic;

namespace MS.Internal.Features
{
  [RequiresContextItem(typeof (Tool))]
  internal class TaskProviderFeatureConnector : PolicyDrivenToolFeatureConnector<TaskProvider>
  {
    private List<Task> _currentTasks;

    public TaskProviderFeatureConnector(FeatureManager manager)
      : base(manager)
    {
    }

    protected override bool IsValidProvider(FeatureProvider featureProvider)
    {
      TaskProvider taskProvider = featureProvider as TaskProvider;
      if (taskProvider != null)
        return taskProvider.IsToolSupported(this.CurrentTool);
      return false;
    }

    protected override void FeatureProvidersAdded(ModelItem item, IEnumerable<TaskProvider> extensions)
    {
      if (this._currentTasks == null)
        this._currentTasks = new List<Task>();
      Tool currentTool = this.CurrentTool;
      foreach (TaskProvider taskProvider in extensions)
      {
        taskProvider.InvokeActivate(this.Context, item);
        foreach (Task task in (IEnumerable<Task>) taskProvider.Tasks)
        {
          if (new RequirementValidator(this.Manager, task.GetType()).MeetsRequirements)
          {
            currentTool.Tasks.Add(task);
            this._currentTasks.Add(task);
          }
        }
      }
    }

    protected override void FeatureProvidersRemoved(ModelItem item, IEnumerable<TaskProvider> extensions)
    {
      Tool currentTool = this.CurrentTool;
      foreach (TaskProvider taskProvider in extensions)
      {
        foreach (Task task in (IEnumerable<Task>) taskProvider.Tasks)
        {
          currentTool.Tasks.Remove(task);
          if (this._currentTasks != null)
            this._currentTasks.Remove(task);
        }
        taskProvider.InvokeDeactivate();
      }
    }

    protected override void UpdateCurrentTool(Tool newTool)
    {
      Tool currentTool = this.CurrentTool;
      if (currentTool == newTool)
        return;
      if (this._currentTasks != null)
      {
        foreach (Task task in this._currentTasks)
        {
          if (currentTool != null)
            currentTool.Tasks.Remove(task);
          if (newTool != null)
            newTool.Tasks.Add(task);
        }
      }
      base.UpdateCurrentTool(newTool);
    }
  }
}
