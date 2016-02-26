// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.MatchGestureEventArgs
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using MS.Internal.Properties;
using System;
using System.Globalization;
using System.Windows.Input;

namespace Microsoft.Windows.Design.Interaction
{
  public class MatchGestureEventArgs : EventArgs
  {
    private InputEventArgs _inputEvent;
    private InputBinding _binding;
    private GestureData _data;
    private Task _sourceTask;
    private MatchGestureEventArgs.GetGestureData _dataCallback;

    public InputEventArgs InputEvent
    {
      get
      {
        return this._inputEvent;
      }
    }

    public InputBinding Binding
    {
      get
      {
        return this._binding;
      }
      set
      {
        this._binding = value;
      }
    }

    public GestureData Data
    {
      get
      {
        if (this._data == null)
          this._data = this._dataCallback(this._sourceTask, this._inputEvent);
        return this._data;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException("value");
        if (!this.Data.GetType().IsInstanceOfType((object) value))
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_IncompatibleGestureData, new object[2]
          {
            (object) value.GetType().Name,
            (object) this._data.GetType().Name
          }));
        this._data = value;
      }
    }

    internal MatchGestureEventArgs(InputEventArgs inputEvent, InputBinding binding, Task sourceTask, MatchGestureEventArgs.GetGestureData dataCallback)
    {
      this._inputEvent = inputEvent;
      this._dataCallback = dataCallback;
      this._sourceTask = sourceTask;
      this._binding = binding;
    }

    internal delegate GestureData GetGestureData(Task sourceTask, InputEventArgs inputEvent);
  }
}
