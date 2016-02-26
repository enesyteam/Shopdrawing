// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.SelectionCommands
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System.Windows.Input;

namespace Microsoft.Windows.Design.Interaction
{
  public static class SelectionCommands
  {
    private static RoutedCommand _clear;
    private static RoutedCommand _selectNext;
    private static RoutedCommand _selectPrevious;
    private static ToolCommand _selectTarget;
    private static ToolCommand _onlySelectTarget;
    private static ToolCommand _toggleSelectTarget;
    private static ToolCommand _unionSelectTarget;
    private static ToolCommand _showEvent;

    public static RoutedCommand Clear
    {
      get
      {
        return SelectionCommands.EnsureCommand(ref SelectionCommands._clear, "ClearSelection");
      }
    }

    public static RoutedCommand SelectAll
    {
      get
      {
        return (RoutedCommand) ApplicationCommands.SelectAll;
      }
    }

    public static RoutedCommand SelectNext
    {
      get
      {
        return SelectionCommands.EnsureCommand(ref SelectionCommands._selectNext, "SelectNext");
      }
    }

    public static RoutedCommand SelectPrevious
    {
      get
      {
        return SelectionCommands.EnsureCommand(ref SelectionCommands._selectPrevious, "SelectPrevious");
      }
    }

    public static ToolCommand SelectTarget
    {
      get
      {
        return SelectionCommands.EnsureCommand(ref SelectionCommands._selectTarget, "SelectTarget");
      }
    }

    public static ToolCommand SelectOnlyTarget
    {
      get
      {
        return SelectionCommands.EnsureCommand(ref SelectionCommands._onlySelectTarget, "SelectOnlyTarget");
      }
    }

    public static ToolCommand ToggleSelectTarget
    {
      get
      {
        return SelectionCommands.EnsureCommand(ref SelectionCommands._toggleSelectTarget, "ToggleSelectTarget");
      }
    }

    public static ToolCommand UnionSelectTarget
    {
      get
      {
        return SelectionCommands.EnsureCommand(ref SelectionCommands._unionSelectTarget, "UnionSelectTarget");
      }
    }

    public static ToolCommand ShowEvent
    {
      get
      {
        return SelectionCommands.EnsureCommand(ref SelectionCommands._showEvent, "ShowEvent");
      }
    }

    private static ToolCommand EnsureCommand(ref ToolCommand command, string name)
    {
      if (command == null)
        command = new ToolCommand(name);
      return command;
    }

    private static RoutedCommand EnsureCommand(ref RoutedCommand command, string name)
    {
      if (command == null)
        command = new RoutedCommand(name, typeof (SelectionCommands));
      return command;
    }
  }
}
