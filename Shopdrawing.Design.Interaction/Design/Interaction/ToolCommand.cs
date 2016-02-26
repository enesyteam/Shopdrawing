// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.ToolCommand
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using MS.Internal.Properties;
using System;
using System.Globalization;
using System.Windows.Input;

namespace Microsoft.Windows.Design.Interaction
{
  public class ToolCommand : ICommand
  {
    private string _name;

    public string Name
    {
      get
      {
        if (this._name != null)
          return this._name;
        return string.Empty;
      }
    }

    public event EventHandler CanExecuteChanged
    {
      add
      {
        throw new NotSupportedException();
      }
      remove
      {
        throw new NotSupportedException();
      }
    }

    public ToolCommand()
    {
    }

    public ToolCommand(string commandName)
    {
      if (commandName == null)
        throw new ArgumentNullException("commandName");
      this._name = commandName;
    }

    public void Execute(GestureData data)
    {
      if (data == null)
        throw new ArgumentNullException("data");
      if (data.Context == null)
        throw new ArgumentException(Resources.Error_MissingContext);
      ToolCommandBinding commandBinding = this.GetCommandBinding(data);
      if (commandBinding == null)
        return;
      ExecutedToolEventArgs e = new ExecutedToolEventArgs((ICommand) this, (object) data);
      commandBinding.OnExecute(data.Context, e);
    }

    private ToolCommandBinding GetCommandBinding(GestureData data)
    {
      return data.Context.Items.GetValue<Tool>().GetToolCommandBinding((ICommand) this, data);
    }

    public bool CanExecute(GestureData data)
    {
      if (data == null)
        throw new ArgumentNullException("data");
      if (data.Context == null)
        throw new ArgumentException(Resources.Error_MissingContext);
      ToolCommandBinding commandBinding = this.GetCommandBinding(data);
      if (commandBinding == null)
        return false;
      CanExecuteToolEventArgs e = new CanExecuteToolEventArgs((ICommand) this, (object) data);
      return commandBinding.OnCanExecute(data.Context, e);
    }

    private static GestureData GetGestureData(object parameter, bool throwIfMissing)
    {
      GestureData gestureData = parameter as GestureData;
      if (gestureData == null && throwIfMissing)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ArgIncorrectType, new object[2]
        {
          (object) "parameter",
          (object) typeof (GestureData).Name
        }));
      return gestureData;
    }

    public override string ToString()
    {
      if (this._name != null)
        return this._name;
      return base.ToString();
    }

    void ICommand.Execute(object parameter)
    {
      if (parameter == null)
        throw new ArgumentNullException("parameter");
      this.Execute(ToolCommand.GetGestureData(parameter, true));
    }

    bool ICommand.CanExecute(object parameter)
    {
      GestureData gestureData = ToolCommand.GetGestureData(parameter, false);
      if (gestureData == null)
        return false;
      return this.CanExecute(gestureData);
    }
  }
}
