// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.CreationTool
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;
using System.Windows.Input;

namespace Microsoft.Windows.Design.Interaction
{
  public class CreationTool : Tool
  {
    private Type _creationType;

    public Type CreationType
    {
      get
      {
        return this._creationType;
      }
      set
      {
        this._creationType = value;
      }
    }

    public event EventHandler CreationComplete;

    public CreationTool()
    {
      this.Cursor = Cursors.Cross;
      this.Tasks.Add(new Task()
      {
        InputBindings = {
          new InputBinding((ICommand) DesignerCommands.Cancel, (InputGesture) new KeyGesture(Key.Escape))
        },
        CommandBindings = {
          new CommandBinding((ICommand) DesignerCommands.Cancel, new ExecutedRoutedEventHandler(this.OnCancel))
        }
      });
    }

    private void OnCancel(object sender, ExecutedRoutedEventArgs e)
    {
      if (this.FocusedTask != null)
        this.FocusedTask.Revert();
      this.CreationComplete(sender, (EventArgs) e);
    }

    protected virtual void OnCreationComplete(EventArgs e)
    {
      if (e == null)
        throw new ArgumentNullException("e");
      if (this.CreationComplete == null)
        return;
      this.CreationComplete((object) this, e);
    }

    public void PerformCreationComplete()
    {
      this.OnCreationComplete(EventArgs.Empty);
    }
  }
}
