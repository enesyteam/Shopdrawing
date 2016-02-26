// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Commands.ICommandTarget
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

namespace Microsoft.Expression.Framework.Commands
{
  public interface ICommandTarget
  {
    ICommandCollection Commands { get; }

    event CommandChangedEventHandler CommandChanged;

    void AddCommand(string commandName, ICommand command);

    void RemoveCommand(string commandName);

    void Execute(string commandName, CommandInvocationSource invocationSource);

    void SetCommandProperty(string commandName, string propertyName, object propertyValue);

    object GetCommandProperty(string commandName, string propertyName);
  }
}
