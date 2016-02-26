// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Commands.ICommandService
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Windows.Input;

namespace Microsoft.Expression.Framework.Commands
{
  public interface ICommandService : ICommandTarget
  {
    bool ShortcutsEnabled { get; set; }

    event CommandExecutionEventHandler CommandExecuting;

    event CommandExecutionEventHandler CommandExecuted;

    void AddTarget(ICommandTarget target);

    void RemoveTarget(ICommandTarget target);

    void SetCommandPropertyDefault(string commandName, string propertyName, object propertyValue);

    bool HandleShortcut(Key shortcutKey, ModifierKeys modifiers);
  }
}
