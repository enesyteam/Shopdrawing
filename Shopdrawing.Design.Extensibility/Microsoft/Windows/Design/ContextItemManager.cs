// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.ContextItemManager
// Assembly: Microsoft.Windows.Design.Extensibility, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4673B7C2-4EF5-4715-85F2-D8E573468337
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Extensibility\Microsoft.Windows.Design.Extensibility.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Windows.Design
{
  public abstract class ContextItemManager : IEnumerable<ContextItem>, IEnumerable
  {
    public abstract bool Contains(Type itemType);

    public bool Contains<TItemType>() where TItemType : ContextItem
    {
      return this.Contains(typeof (TItemType));
    }

    public abstract IEnumerator<ContextItem> GetEnumerator();

    public abstract ContextItem GetValue(Type itemType);

    public TItemType GetValue<TItemType>() where TItemType : ContextItem
    {
      return (TItemType) this.GetValue(typeof (TItemType));
    }

    protected static void NotifyItemChanged(EditingContext context, ContextItem item, ContextItem previousItem)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      if (item == null)
        throw new ArgumentNullException("item");
      if (previousItem == null)
        throw new ArgumentNullException("previousItem");
      item.InvokeOnItemChanged(context, previousItem);
    }

    public abstract void SetValue(ContextItem value);

    public abstract void Subscribe(Type contextItemType, SubscribeContextCallback callback);

    public void Subscribe<TContextItemType>(SubscribeContextCallback<TContextItemType> callback) where TContextItemType : ContextItem
    {
      if (callback == null)
        throw new ArgumentNullException("callback");
      this.Subscribe(typeof (TContextItemType), new ContextItemManager.SubscribeProxy<TContextItemType>(callback).Callback);
    }

    public void Unsubscribe<TContextItemType>(SubscribeContextCallback<TContextItemType> callback) where TContextItemType : ContextItem
    {
      if (callback == null)
        throw new ArgumentNullException("callback");
      this.Unsubscribe(typeof (TContextItemType), new ContextItemManager.SubscribeProxy<TContextItemType>(callback).Callback);
    }

    public abstract void Unsubscribe(Type contextItemType, SubscribeContextCallback callback);

    protected static object GetTarget(Delegate callback)
    {
      if (callback == null)
        throw new ArgumentNullException("callback");
      ContextItemManager.ICallbackProxy callbackProxy = callback.Target as ContextItemManager.ICallbackProxy;
      if (callbackProxy != null)
        return callbackProxy.OriginalTarget;
      return callback.Target;
    }

    protected static Delegate RemoveCallback(Delegate existing, Delegate toRemove)
    {
      if (existing == null)
        return (Delegate) null;
      if (toRemove == null)
        return existing;
      ContextItemManager.ICallbackProxy callbackProxy1 = toRemove.Target as ContextItemManager.ICallbackProxy;
      if (callbackProxy1 == null)
        return Delegate.Remove(existing, toRemove);
      toRemove = callbackProxy1.OriginalDelegate;
      Delegate[] invocationList = existing.GetInvocationList();
      bool flag = false;
      for (int index = 0; index < invocationList.Length; ++index)
      {
        Delegate @delegate = invocationList[index];
        ContextItemManager.ICallbackProxy callbackProxy2 = @delegate.Target as ContextItemManager.ICallbackProxy;
        if (callbackProxy2 != null)
          @delegate = callbackProxy2.OriginalDelegate;
        if (@delegate.Equals((object) toRemove))
        {
          invocationList[index] = (Delegate) null;
          flag = true;
        }
      }
      if (flag)
      {
        existing = (Delegate) null;
        foreach (Delegate b in invocationList)
        {
          if (b != null)
            existing = existing != null ? Delegate.Combine(existing, b) : b;
        }
      }
      return existing;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }

    private interface ICallbackProxy
    {
      Delegate OriginalDelegate { get; }

      object OriginalTarget { get; }
    }

    private class SubscribeProxy<ContextItemType> : ContextItemManager.ICallbackProxy where ContextItemType : ContextItem
    {
      private SubscribeContextCallback<ContextItemType> _genericCallback;

      internal SubscribeContextCallback Callback
      {
        get
        {
          return new SubscribeContextCallback(this.SubscribeContext);
        }
      }

      Delegate ContextItemManager.ICallbackProxy.OriginalDelegate
      {
        get
        {
          return (Delegate) this._genericCallback;
        }
      }

      object ContextItemManager.ICallbackProxy.OriginalTarget
      {
        get
        {
          return this._genericCallback.Target;
        }
      }

      internal SubscribeProxy(SubscribeContextCallback<ContextItemType> callback)
      {
        this._genericCallback = callback;
      }

      private void SubscribeContext(ContextItem item)
      {
        if (item == null)
          throw new ArgumentNullException("item");
        this._genericCallback((ContextItemType) item);
      }
    }
  }
}
