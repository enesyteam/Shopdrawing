// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.FocusedTask
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using MS.Internal.Properties;
using System;

namespace Microsoft.Windows.Design.Interaction
{
  public sealed class FocusedTask : ContextItem
  {
    private Task _task;

    public override Type ItemType
    {
      get
      {
        return typeof (FocusedTask);
      }
    }

    public Task Task
    {
      get
      {
        return this._task;
      }
    }

    public FocusedTask()
    {
    }

    internal FocusedTask(Task task)
    {
      this._task = task;
    }

    protected override void OnItemChanged(EditingContext context, ContextItem previousItem)
    {
      if (!context.Items.GetValue<Tool>().IsUpdatingTaskItem)
        throw new InvalidOperationException(Resources.Error_IncorrectFocusedTask);
      base.OnItemChanged(context, previousItem);
    }
  }
}
