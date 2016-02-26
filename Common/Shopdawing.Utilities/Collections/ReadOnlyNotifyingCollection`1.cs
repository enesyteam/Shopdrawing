// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.Collections.ReadOnlyNotifyingCollection`1
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.Utility.Collections
{
  public class ReadOnlyNotifyingCollection<T> : INotifyingEnumerable<T>, IEnumerable<T>, IEnumerable
  {
    private INotifyingEnumerable<T> hostedNotifyingCollection;

    public event EventHandler<EnumerationChangedEventArgs<T>> EnumerationChanged;

    public ReadOnlyNotifyingCollection(INotifyingEnumerable<T> hostedNotifyingCollection)
    {
      this.hostedNotifyingCollection = hostedNotifyingCollection;
      this.hostedNotifyingCollection.EnumerationChanged += new EventHandler<EnumerationChangedEventArgs<T>>(this.HostedNotifyingCollection_EnumerationChanged);
    }

    private void HostedNotifyingCollection_EnumerationChanged(object sender, EnumerationChangedEventArgs<T> e)
    {
      if (this.EnumerationChanged == null)
        return;
      this.EnumerationChanged(sender, e);
    }

    public IEnumerator<T> GetEnumerator()
    {
      return this.hostedNotifyingCollection.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.hostedNotifyingCollection.GetEnumerator();
    }
  }
}
