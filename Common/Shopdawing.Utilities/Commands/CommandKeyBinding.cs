// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.Commands.CommandKeyBinding
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using System;
using System.Windows.Input;

namespace Microsoft.Expression.Utility.Commands
{
  [Serializable]
  public sealed class CommandKeyBinding
  {
    public Key Key { get; set; }

    public ModifierKeys Modifiers { get; set; }

    public CommandKeyBinding(Key key, ModifierKeys modifiers)
    {
      this.Key = key;
      this.Modifiers = modifiers;
    }
  }
}
