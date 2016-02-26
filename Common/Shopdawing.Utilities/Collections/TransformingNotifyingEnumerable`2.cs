// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.Collections.TransformingNotifyingEnumerable`2
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.Utility.Collections
{
  public class TransformingNotifyingEnumerable<T, THost> : INotifyingEnumerable<T>, IEnumerable<T>, IEnumerable
  {
    private EventHandler<EnumerationChangedEventArgs<T>> enumerationChanged;
    private INotifyingEnumerable<THost> host;
    private Func<THost, T> converter;

    public event EventHandler<EnumerationChangedEventArgs<T>> EnumerationChanged
    {
      add
      {
        if (this.enumerationChanged == null)
          this.host.EnumerationChanged += new EventHandler<EnumerationChangedEventArgs<THost>>(this.OnHostEnumerationChanged);
        this.enumerationChanged += value;
      }
      remove
      {
        this.enumerationChanged -= value;
        if (this.enumerationChanged != null)
          return;
        this.host.EnumerationChanged -= new EventHandler<EnumerationChangedEventArgs<THost>>(this.OnHostEnumerationChanged);
      }
    }

    public TransformingNotifyingEnumerable(INotifyingEnumerable<THost> host, Func<THost, T> converter)
    {
      this.host = host;
      this.converter = converter;
    }

    public IEnumerator<T> GetEnumerator()
    {
      foreach (THost host in (IEnumerable<THost>) this.host)
        yield return this.converter(host);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }

    private IEnumerable<EnumerationChange<T>> ConvertChanges(IEnumerable<EnumerationChange<THost>> changes)
    {
      if (changes != null)
      {
        foreach (EnumerationChange<THost> enumerationChange in changes)
          yield return new EnumerationChange<T>()
          {
            ChangedItem = this.converter(enumerationChange.ChangedItem),
            ChangeType = enumerationChange.ChangeType,
            OldName = enumerationChange.OldName
          };
      }
    }

    private void OnHostEnumerationChanged(object sender, EnumerationChangedEventArgs<THost> e)
    {
      if (this.enumerationChanged == null)
        return;
      this.enumerationChanged((object) this, new EnumerationChangedEventArgs<T>(this.ConvertChanges(e.Changes)));
    }
  }
}
