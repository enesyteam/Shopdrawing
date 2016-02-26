// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.Collections.NotifyingEnumerableExtensions
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.Utility.Collections
{
  public static class NotifyingEnumerableExtensions
  {
    public static INotifyingEnumerable<T> Empty<T>()
    {
      return (INotifyingEnumerable<T>) NotifyingEnumerableExtensions.EmptyNotifyingEnumerable<T>.Instance;
    }

    private class EmptyNotifyingEnumerable<T> : INotifyingEnumerable<T>, IEnumerable<T>, IEnumerable
    {
      private static NotifyingEnumerableExtensions.EmptyNotifyingEnumerable<T> instance;
      private static IEnumerable<T> enumerableInstance;

      public static NotifyingEnumerableExtensions.EmptyNotifyingEnumerable<T> Instance
      {
        get
        {
          if (NotifyingEnumerableExtensions.EmptyNotifyingEnumerable<T>.instance == null)
          {
            NotifyingEnumerableExtensions.EmptyNotifyingEnumerable<T>.instance = new NotifyingEnumerableExtensions.EmptyNotifyingEnumerable<T>();
            NotifyingEnumerableExtensions.EmptyNotifyingEnumerable<T>.enumerableInstance = (IEnumerable<T>) new T[0];
          }
          return NotifyingEnumerableExtensions.EmptyNotifyingEnumerable<T>.instance;
        }
      }

      public event EventHandler<EnumerationChangedEventArgs<T>> EnumerationChanged
      {
        add
        {
        }
        remove
        {
        }
      }

      public IEnumerator<T> GetEnumerator()
      {
        return NotifyingEnumerableExtensions.EmptyNotifyingEnumerable<T>.enumerableInstance.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) this.GetEnumerator();
      }
    }
  }
}
