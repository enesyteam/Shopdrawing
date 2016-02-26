// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.Collections.EnumerationChangedEventArgs`1
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Utility.Collections
{
  public class EnumerationChangedEventArgs<T> : EventArgs
  {
    public IEnumerable<EnumerationChange<T>> Changes { get; private set; }

    public EnumerationChangedEventArgs(IEnumerable<EnumerationChange<T>> changes)
    {
      this.Changes = changes;
    }

    public EnumerationChangedEventArgs(T changedItem, EnumerationChangeType changeType, string oldName = null)
    {
      this.Changes = (IEnumerable<EnumerationChange<T>>) new EnumerationChange<T>[1]
      {
        new EnumerationChange<T>()
        {
          ChangedItem = changedItem,
          ChangeType = changeType,
          OldName = oldName
        }
      };
    }

    public static EnumerationChangedEventArgs<T> Aggregator(IEnumerable<EnumerationChangedEventArgs<T>> eventArguments)
    {
      List<EnumerationChange<T>> list = new List<EnumerationChange<T>>();
      foreach (EnumerationChangedEventArgs<T> changedEventArgs in eventArguments)
        list.AddRange(changedEventArgs.Changes);
      return new EnumerationChangedEventArgs<T>((IEnumerable<EnumerationChange<T>>) list);
    }
  }
}
