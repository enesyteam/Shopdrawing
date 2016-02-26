// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.ToolActionEventArgs
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;
using System.Windows.Input;

namespace Microsoft.Windows.Design.Interaction
{
  public class ToolActionEventArgs : InputEventArgs
  {
    private ToolAction _toolAction;
    private EventArgs _sourceEvent;

    public ToolAction ToolAction
    {
      get
      {
        return this._toolAction;
      }
    }

    public EventArgs SourceEvent
    {
      get
      {
        return this._sourceEvent;
      }
    }

    public ToolActionEventArgs(ToolAction toolAction, InputEventArgs sourceEvent)
      : base(sourceEvent == null ? (InputDevice) null : sourceEvent.Device, sourceEvent == null ? 0 : sourceEvent.Timestamp)
    {
      if (sourceEvent == null)
        throw new ArgumentNullException("sourceEvent");
      if (!EnumValidator.IsValid(toolAction))
        throw new ArgumentOutOfRangeException("toolAction");
      this._toolAction = toolAction;
      this._sourceEvent = (EventArgs) sourceEvent;
    }

    public ToolActionEventArgs(ToolAction toolAction, EventArgs sourceEvent, InputDevice inputDevice, int timestamp)
      : base(inputDevice, timestamp)
    {
      if (sourceEvent == null)
        throw new ArgumentNullException("sourceEvent");
      this._toolAction = toolAction;
      this._sourceEvent = sourceEvent;
    }

    public override string ToString()
    {
      return (string) (object) this._toolAction + (object) " : " + (string) (object) this.Timestamp + " : " + this._sourceEvent.ToString();
    }
  }
}
