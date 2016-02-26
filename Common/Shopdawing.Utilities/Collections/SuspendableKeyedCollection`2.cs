// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.Collections.SuspendableKeyedCollection`2
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using Microsoft.Expression.Utility.Events;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Expression.Utility.Collections
{
  public class SuspendableKeyedCollection<TKey, TValue> : IDisposable
  {
    private object suspenderLock = new object();
    private ConcurrentDictionary<TKey, IDisposable> notificationSuspenders = new ConcurrentDictionary<TKey, IDisposable>();
    private ConcurrentDictionary<TKey, TValue> collection = new ConcurrentDictionary<TKey, TValue>();
    private Suspender suspender;
    private Func<TKey, TValue> itemCreator;
    private Func<TValue, IDisposable> suspenderCreator;

    public IEnumerable<TValue> Items
    {
      get
      {
        return (IEnumerable<TValue>) Enumerable.ToArray<TValue>((IEnumerable<TValue>) this.collection.Values);
      }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public IDisposable Suspend
    {
      get
      {
        return this.suspender.Suspend;
      }
    }

    public SuspendableKeyedCollection(Func<TKey, TValue> itemCreator, Func<TValue, IDisposable> suspenderCreator)
    {
      if (itemCreator == null)
        throw new ArgumentNullException("itemCreator");
      if (suspenderCreator == null)
        throw new ArgumentNullException("suspenderCreator");
      this.itemCreator = itemCreator;
      this.suspenderCreator = suspenderCreator;
      this.suspender = new Suspender((Action) (() =>
      {
        KeyValuePair<TKey, IDisposable>[] keyValuePairArray = (KeyValuePair<TKey, IDisposable>[]) null;
        lock (this.suspenderLock)
        {
          keyValuePairArray = this.notificationSuspenders.ToArray();
          this.notificationSuspenders.Clear();
        }
        if (keyValuePairArray == null)
          return;
        foreach (KeyValuePair<TKey, IDisposable> keyValuePair in keyValuePairArray)
          keyValuePair.Value.Dispose();
      }), (Action) (() =>
      {
        lock (this.suspenderLock)
        {
          foreach (KeyValuePair<TKey, TValue> item_1 in this.collection.ToArray())
          {
            if ((object) item_1.Value != null && !this.notificationSuspenders.ContainsKey(item_1.Key))
              this.notificationSuspenders.TryAdd(item_1.Key, this.suspenderCreator(item_1.Value));
          }
        }
      }));
    }

    private TValue CreateItems(TKey key)
    {
      TValue obj = this.itemCreator(key);
      lock (this.suspenderLock)
      {
        if (this.suspender.Suspended)
        {
          if ((object) obj != null)
          {
            if (!this.notificationSuspenders.ContainsKey(key))
              this.notificationSuspenders.TryAdd(key, this.suspenderCreator(obj));
          }
        }
      }
      return obj;
    }

    public TValue GetItem(TKey key)
    {
      return this.collection.GetOrAdd(key, new Func<TKey, TValue>(this.CreateItems));
    }

    public void RemoveItem(TKey key)
    {
      TValue obj;
      if (!this.collection.TryRemove(key, out obj))
        return;
      IDisposable disposable = (object) obj as IDisposable;
      if (disposable == null)
        return;
      disposable.Dispose();
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public void ClearItems()
    {
      TValue[] objArray = Enumerable.ToArray<TValue>((IEnumerable<TValue>) this.collection.Values);
      this.collection.Clear();
      foreach (IDisposable disposable in Enumerable.OfType<IDisposable>((IEnumerable) objArray))
        disposable.Dispose();
    }

    private void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      this.ClearItems();
      IDisposable[] disposableArray = Enumerable.ToArray<IDisposable>((IEnumerable<IDisposable>) this.notificationSuspenders.Values);
      this.notificationSuspenders.Clear();
      foreach (IDisposable disposable in disposableArray)
        disposable.Dispose();
    }
  }
}
