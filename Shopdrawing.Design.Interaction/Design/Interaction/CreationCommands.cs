// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.CreationCommands
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System.Windows.Input;

namespace Microsoft.Windows.Design.Interaction
{
  public static class CreationCommands
  {
    private static RoutedCommand _create;
    private static ToolCommand _createAt;
    private static ToolCommand _createWithin;

    public static RoutedCommand Create
    {
      get
      {
        return CreationCommands.EnsureCommand(ref CreationCommands._create, "Create");
      }
    }

    public static ToolCommand CreateAt
    {
      get
      {
        return CreationCommands.EnsureCommand(ref CreationCommands._createAt, "CreateAt");
      }
    }

    public static ToolCommand CreateWithin
    {
      get
      {
        return CreationCommands.EnsureCommand(ref CreationCommands._createWithin, "CreateWithin");
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
        command = new RoutedCommand(name, typeof (CreationCommands));
      return command;
    }
  }
}
