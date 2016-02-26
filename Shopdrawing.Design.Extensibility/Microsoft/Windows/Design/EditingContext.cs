// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.EditingContext
// Assembly: Microsoft.Windows.Design.Extensibility, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4673B7C2-4EF5-4715-85F2-D8E573468337
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Extensibility\Microsoft.Windows.Design.Extensibility.dll

using MS.Internal.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Windows.Design
{
  public class EditingContext : IDisposable
  {
    private ContextItemManager _contextItems;
    private ServiceManager _services;

    public ContextItemManager Items
    {
      get
      {
        if (this._contextItems == null)
        {
          this._contextItems = this.CreateContextItemManager();
          if (this._contextItems == null)
            throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_NullImplementation, new object[1]
            {
              (object) "CreateContextItemManager"
            }));
        }
        return this._contextItems;
      }
    }

    public ServiceManager Services
    {
      get
      {
        if (this._services == null)
        {
          this._services = this.CreateServiceManager();
          if (this._services == null)
            throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_NullImplementation, new object[1]
            {
              (object) "CreateServiceManager"
            }));
        }
        return this._services;
      }
    }

    public event EventHandler Disposing;

    ~EditingContext()
    {
      this.Dispose(false);
    }

    protected virtual ContextItemManager CreateContextItemManager()
    {
      return (ContextItemManager) new EditingContext.DefaultContextItemManager(this);
    }

    protected virtual ServiceManager CreateServiceManager()
    {
      return (ServiceManager) new EditingContext.DefaultServiceManager();
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (this.Disposing != null)
        this.Disposing((object) this, EventArgs.Empty);
      IDisposable disposable1 = this._services as IDisposable;
      if (disposable1 != null)
        disposable1.Dispose();
      IDisposable disposable2 = this._contextItems as IDisposable;
      if (disposable2 == null)
        return;
      disposable2.Dispose();
    }

    private sealed class DefaultContextItemManager : ContextItemManager
    {
      private EditingContext _context;
      private EditingContext.DefaultContextItemManager.DefaultContextLayer _currentLayer;
      private Dictionary<Type, SubscribeContextCallback> _subscriptions;

      internal DefaultContextItemManager(EditingContext context)
      {
        this._context = context;
        this._currentLayer = new EditingContext.DefaultContextItemManager.DefaultContextLayer((EditingContext.DefaultContextItemManager.DefaultContextLayer) null);
      }

      public override void SetValue(ContextItem value)
      {
        if (value == null)
          throw new ArgumentNullException("value");
        ContextItem valueNull;
        ContextItem previousItem = (valueNull = this.GetValueNull(value.ItemType)) ?? this.GetValue(value.ItemType);
        bool flag = false;
        try
        {
          this._currentLayer.Items[value.ItemType] = value;
          ContextItemManager.NotifyItemChanged(this._context, value, previousItem);
          flag = true;
        }
        finally
        {
          if (flag)
          {
            this.OnItemChanged(value);
          }
          else
          {
            this._currentLayer.Items.Remove(value.ItemType);
            if (valueNull != null)
              this.SetValue(valueNull);
          }
        }
      }

      public override bool Contains(Type itemType)
      {
        if (itemType == null)
          throw new ArgumentNullException("itemType");
        if (!typeof (ContextItem).IsAssignableFrom(itemType))
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ArgIncorrectType, new object[2]
          {
            (object) "itemType",
            (object) typeof (ContextItem).FullName
          }));
        return this._currentLayer.Items.ContainsKey(itemType);
      }

      public override ContextItem GetValue(Type itemType)
      {
        ContextItem contextItem = this.GetValueNull(itemType);
        if (contextItem == null && !this._currentLayer.DefaultItems.TryGetValue(itemType, out contextItem))
        {
          contextItem = (ContextItem) Activator.CreateInstance(itemType);
          if (contextItem.ItemType != itemType)
            throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_DerivedContextItem, new object[2]
            {
              (object) itemType.FullName,
              (object) contextItem.ItemType.FullName
            }));
          this._currentLayer.DefaultItems.Add(contextItem.ItemType, contextItem);
        }
        return contextItem;
      }

      private ContextItem GetValueNull(Type itemType)
      {
        if (itemType == null)
          throw new ArgumentNullException("itemType");
        if (!typeof (ContextItem).IsAssignableFrom(itemType))
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ArgIncorrectType, new object[2]
          {
            (object) "itemType",
            (object) typeof (ContextItem).FullName
          }));
        ContextItem contextItem = (ContextItem) null;
        EditingContext.DefaultContextItemManager.DefaultContextLayer defaultContextLayer = this._currentLayer;
        while (defaultContextLayer != null && !defaultContextLayer.Items.TryGetValue(itemType, out contextItem))
          defaultContextLayer = defaultContextLayer.ParentLayer;
        return contextItem;
      }

      public override IEnumerator<ContextItem> GetEnumerator()
      {
        return (IEnumerator<ContextItem>) this._currentLayer.Items.Values.GetEnumerator();
      }

      private void OnItemChanged(ContextItem item)
      {
        SubscribeContextCallback subscribeContextCallback;
        if (this._subscriptions == null || !this._subscriptions.TryGetValue(item.ItemType, out subscribeContextCallback))
          return;
        subscribeContextCallback(item);
      }

      public override void Subscribe(Type contextItemType, SubscribeContextCallback callback)
      {
        if (contextItemType == null)
          throw new ArgumentNullException("contextItemType");
        if (callback == null)
          throw new ArgumentNullException("callback");
        if (!typeof (ContextItem).IsAssignableFrom(contextItemType))
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ArgIncorrectType, new object[2]
          {
            (object) "contextItemType",
            (object) typeof (ContextItem).FullName
          }));
        if (this._subscriptions == null)
          this._subscriptions = new Dictionary<Type, SubscribeContextCallback>();
        SubscribeContextCallback subscribeContextCallback1 = (SubscribeContextCallback) null;
        this._subscriptions.TryGetValue(contextItemType, out subscribeContextCallback1);
        SubscribeContextCallback subscribeContextCallback2 = subscribeContextCallback1 + callback;
        this._subscriptions[contextItemType] = subscribeContextCallback2;
        ContextItem valueNull = this.GetValueNull(contextItemType);
        if (valueNull == null)
          return;
        callback(valueNull);
      }

      public override void Unsubscribe(Type contextItemType, SubscribeContextCallback callback)
      {
        if (contextItemType == null)
          throw new ArgumentNullException("contextItemType");
        if (callback == null)
          throw new ArgumentNullException("callback");
        if (!typeof (ContextItem).IsAssignableFrom(contextItemType))
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ArgIncorrectType, new object[2]
          {
            (object) "contextItemType",
            (object) typeof (ContextItem).FullName
          }));
        SubscribeContextCallback subscribeContextCallback1;
        if (this._subscriptions == null || !this._subscriptions.TryGetValue(contextItemType, out subscribeContextCallback1))
          return;
        SubscribeContextCallback subscribeContextCallback2 = (SubscribeContextCallback) ContextItemManager.RemoveCallback((Delegate) subscribeContextCallback1, (Delegate) callback);
        if (subscribeContextCallback2 == null)
          this._subscriptions.Remove(contextItemType);
        else
          this._subscriptions[contextItemType] = subscribeContextCallback2;
      }

      private class DefaultContextLayer
      {
        private EditingContext.DefaultContextItemManager.DefaultContextLayer _parentLayer;
        private Dictionary<Type, ContextItem> _items;
        private Dictionary<Type, ContextItem> _defaultItems;

        internal Dictionary<Type, ContextItem> DefaultItems
        {
          get
          {
            if (this._defaultItems == null)
              this._defaultItems = new Dictionary<Type, ContextItem>();
            return this._defaultItems;
          }
        }

        internal Dictionary<Type, ContextItem> Items
        {
          get
          {
            if (this._items == null)
              this._items = new Dictionary<Type, ContextItem>();
            return this._items;
          }
        }

        internal EditingContext.DefaultContextItemManager.DefaultContextLayer ParentLayer
        {
          get
          {
            return this._parentLayer;
          }
        }

        internal DefaultContextLayer(EditingContext.DefaultContextItemManager.DefaultContextLayer parentLayer)
        {
          this._parentLayer = parentLayer;
        }
      }
    }

    private sealed class DefaultServiceManager : ServiceManager, IDisposable
    {
      private static readonly object _recursionSentinel = new object();
      private Dictionary<Type, object> _services;
      private Dictionary<Type, SubscribeServiceCallback> _subscriptions;

      internal DefaultServiceManager()
      {
      }

      public override bool Contains(Type serviceType)
      {
        if (serviceType == null)
          throw new ArgumentNullException("serviceType");
        if (this._services != null)
          return this._services.ContainsKey(serviceType);
        return false;
      }

      public override object GetService(Type serviceType)
      {
        object o = (object) null;
        if (serviceType == null)
          throw new ArgumentNullException("serviceType");
        if (this._services != null && this._services.TryGetValue(serviceType, out o))
        {
          if (o == EditingContext.DefaultServiceManager._recursionSentinel)
            throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_RecursionResolvingService, new object[1]
            {
              (object) serviceType.FullName
            }));
          PublishServiceCallback publishServiceCallback = o as PublishServiceCallback;
          if (publishServiceCallback != null)
          {
            this._services[serviceType] = EditingContext.DefaultServiceManager._recursionSentinel;
            try
            {
              o = publishServiceCallback(serviceType);
              if (o == null)
                throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_NullService, new object[2]
                {
                  (object) publishServiceCallback.Method.DeclaringType.FullName,
                  (object) serviceType.FullName
                }));
              if (!serviceType.IsInstanceOfType(o))
                throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_IncorrectServiceType, (object) publishServiceCallback.Method.DeclaringType.FullName, (object) serviceType.FullName, (object) o.GetType().FullName));
            }
            finally
            {
              this._services[serviceType] = o;
            }
          }
        }
        return o;
      }

      public override IEnumerator<Type> GetEnumerator()
      {
        if (this._services == null)
          this._services = new Dictionary<Type, object>();
        return (IEnumerator<Type>) this._services.Keys.GetEnumerator();
      }

      public override void Subscribe(Type serviceType, SubscribeServiceCallback callback)
      {
        if (serviceType == null)
          throw new ArgumentNullException("serviceType");
        if (callback == null)
          throw new ArgumentNullException("callback");
        object service = this.GetService(serviceType);
        if (service != null)
        {
          callback(serviceType, service);
        }
        else
        {
          if (this._subscriptions == null)
            this._subscriptions = new Dictionary<Type, SubscribeServiceCallback>();
          SubscribeServiceCallback subscribeServiceCallback1 = (SubscribeServiceCallback) null;
          this._subscriptions.TryGetValue(serviceType, out subscribeServiceCallback1);
          SubscribeServiceCallback subscribeServiceCallback2 = subscribeServiceCallback1 + callback;
          this._subscriptions[serviceType] = subscribeServiceCallback2;
        }
      }

      public override void Publish(Type serviceType, PublishServiceCallback callback)
      {
        if (serviceType == null)
          throw new ArgumentNullException("serviceType");
        if (callback == null)
          throw new ArgumentNullException("callback");
        this.Publish(serviceType, (object) callback);
      }

      public override void Publish(Type serviceType, object serviceInstance)
      {
        if (serviceType == null)
          throw new ArgumentNullException("serviceType");
        if (serviceInstance == null)
          throw new ArgumentNullException("serviceInstance");
        if (!(serviceInstance is PublishServiceCallback) && !serviceType.IsInstanceOfType(serviceInstance))
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_IncorrectServiceType, (object) typeof (ServiceManager).Name, (object) serviceType.FullName, (object) serviceInstance.GetType().FullName));
        if (this._services == null)
          this._services = new Dictionary<Type, object>();
        try
        {
          this._services.Add(serviceType, serviceInstance);
        }
        catch (ArgumentException ex)
        {
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_DuplicateService, new object[1]
          {
            (object) serviceType.FullName
          }), (Exception) ex);
        }
        SubscribeServiceCallback subscribeServiceCallback;
        if (this._subscriptions == null || !this._subscriptions.TryGetValue(serviceType, out subscribeServiceCallback))
          return;
        subscribeServiceCallback(serviceType, this.GetService(serviceType));
        this._subscriptions.Remove(serviceType);
      }

      public override void Unsubscribe(Type serviceType, SubscribeServiceCallback callback)
      {
        if (serviceType == null)
          throw new ArgumentNullException("serviceType");
        if (callback == null)
          throw new ArgumentNullException("callback");
        SubscribeServiceCallback subscribeServiceCallback1;
        if (this._subscriptions == null || !this._subscriptions.TryGetValue(serviceType, out subscribeServiceCallback1))
          return;
        SubscribeServiceCallback subscribeServiceCallback2 = (SubscribeServiceCallback) ServiceManager.RemoveCallback((Delegate) subscribeServiceCallback1, (Delegate) callback);
        if (subscribeServiceCallback2 == null)
          this._subscriptions.Remove(serviceType);
        else
          this._subscriptions[serviceType] = subscribeServiceCallback2;
      }

      void IDisposable.Dispose()
      {
        if (this._services == null)
          return;
        Dictionary<Type, object> dictionary = this._services;
        try
        {
          foreach (object obj in dictionary.Values)
          {
            IDisposable disposable = obj as IDisposable;
            if (disposable != null)
              disposable.Dispose();
          }
        }
        finally
        {
          this._services = (Dictionary<Type, object>) null;
        }
      }
    }
  }
}
