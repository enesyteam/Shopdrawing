// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Commands.CommandChangedEventArgs
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;

namespace Microsoft.Expression.Framework.Commands
{
  public class CommandChangedEventArgs : EventArgs
  {
    private ICommandCollection added;
    private ICommandCollection removed;

    public ICommandCollection Added
    {
      get
      {
        return this.added;
      }
    }

    public ICommandCollection Removed
    {
      get
      {
        return this.removed;
      }
    }

    public CommandChangedEventArgs(ICommandCollection added, ICommandCollection removed)
    {
      this.added = added;
      this.removed = removed;
    }
  }
}
