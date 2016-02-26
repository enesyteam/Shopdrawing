// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.ServiceManager
// Assembly: Microsoft.Windows.Design.Extensibility, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4673B7C2-4EF5-4715-85F2-D8E573468337
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Extensibility\Microsoft.Windows.Design.Extensibility.dll

using MS.Internal.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Windows.Design
{
  public abstract class ServiceManager : IServiceProvider, IEnumerable<Type>, IEnumerable
  {
    public abstract bool Contains(Type serviceType);

    public bool Contains<TServiceType>()
    {
      return this.Contains(typeof (TServiceType));
    }

    public TServiceType GetRequiredService<TServiceType>()
    {
      TServiceType service = this.GetService<TServiceType>();
      if ((object) service == null)
        throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_RequiredService, new object[1]
        {
          (object) typeof (TServiceType).FullName
        }));
      return service;
    }

    public TServiceType GetService<TServiceType>()
    {
      return (TServiceType) this.GetService(typeof (TServiceType));
    }

    public abstract object GetService(Type serviceType);

    public abstract IEnumerator<Type> GetEnumerator();

    public abstract void Subscribe(Type serviceType, SubscribeServiceCallback callback);

    public void Subscribe<TServiceType>(SubscribeServiceCallback<TServiceType> callback)
    {
      if (callback == null)
        throw new ArgumentNullException("callback");
      this.Subscribe(typeof (TServiceType), new ServiceManager.SubscribeProxy<TServiceType>(callback).Callback);
    }

    public abstract void Publish(Type serviceType, PublishServiceCallback callback);

    public abstract void Publish(Type serviceType, object serviceInstance);

    public void Publish<TServiceType>(PublishServiceCallback<TServiceType> callback)
    {
      if (callback == null)
        throw new ArgumentNullException("callback");
      this.Publish(typeof (TServiceType), new ServiceManager.PublishProxy<TServiceType>(callback).Callback);
    }

    public void Publish<TServiceType>(TServiceType serviceInstance)
    {
      if ((object) serviceInstance == null)
        throw new ArgumentNullException("serviceInstance");
      this.Publish(typeof (TServiceType), (object) serviceInstance);
    }

    public void Unsubscribe<TServiceType>(SubscribeServiceCallback<TServiceType> callback)
    {
      if (callback == null)
        throw new ArgumentNullException("callback");
      this.Unsubscribe(typeof (TServiceType), new ServiceManager.SubscribeProxy<TServiceType>(callback).Callback);
    }

    public abstract void Unsubscribe(Type serviceType, SubscribeServiceCallback callback);

    protected static object GetTarget(Delegate callback)
    {
      if (callback == null)
        throw new ArgumentNullException("callback");
      ServiceManager.ICallbackProxy callbackProxy = callback.Target as ServiceManager.ICallbackProxy;
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
      ServiceManager.ICallbackProxy callbackProxy1 = toRemove.Target as ServiceManager.ICallbackProxy;
      if (callbackProxy1 == null)
        return Delegate.Remove(existing, toRemove);
      toRemove = callbackProxy1.OriginalDelegate;
      Delegate[] invocationList = existing.GetInvocationList();
      bool flag = false;
      for (int index = 0; index < invocationList.Length; ++index)
      {
        Delegate @delegate = invocationList[index];
        ServiceManager.ICallbackProxy callbackProxy2 = @delegate.Target as ServiceManager.ICallbackProxy;
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

    private class PublishProxy<TServiceType>
    {
      private PublishServiceCallback<TServiceType> _genericCallback;

      internal PublishServiceCallback Callback
      {
        get
        {
          return new PublishServiceCallback(this.PublishService);
        }
      }

      internal PublishProxy(PublishServiceCallback<TServiceType> callback)
      {
        this._genericCallback = callback;
      }

      private object PublishService(Type serviceType)
      {
        if (serviceType == null)
          throw new ArgumentNullException("serviceType");
        if (serviceType != typeof (TServiceType))
          throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_IncorrectServiceType, (object) typeof (ServiceManager).FullName, (object) typeof (TServiceType).FullName, (object) serviceType.FullName));
        object o = (object) this._genericCallback();
        if (o == null)
          throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_NullService, new object[2]
          {
            (object) this._genericCallback.Method.DeclaringType.FullName,
            (object) serviceType.FullName
          }));
        if (!serviceType.IsInstanceOfType(o))
          throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_IncorrectServiceType, (object) this._genericCallback.Method.DeclaringType.FullName, (object) serviceType.FullName, (object) o.GetType().FullName));
        return o;
      }
    }

    private interface ICallbackProxy
    {
      Delegate OriginalDelegate { get; }

      object OriginalTarget { get; }
    }

    private class SubscribeProxy<ServiceType> : ServiceManager.ICallbackProxy
    {
      private SubscribeServiceCallback<ServiceType> _genericCallback;

      internal SubscribeServiceCallback Callback
      {
        get
        {
          return new SubscribeServiceCallback(this.SubscribeService);
        }
      }

      Delegate ServiceManager.ICallbackProxy.OriginalDelegate
      {
        get
        {
          return (Delegate) this._genericCallback;
        }
      }

      object ServiceManager.ICallbackProxy.OriginalTarget
      {
        get
        {
          return this._genericCallback.Target;
        }
      }

      internal SubscribeProxy(SubscribeServiceCallback<ServiceType> callback)
      {
        this._genericCallback = callback;
      }

      private void SubscribeService(Type serviceType, object service)
      {
        if (serviceType == null)
          throw new ArgumentNullException("serviceType");
        if (service == null)
          throw new ArgumentNullException("service");
        if (!typeof (ServiceType).IsInstanceOfType(service))
          throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_IncorrectServiceType, new object[2]
          {
            (object) typeof (ServiceType).FullName,
            (object) serviceType.FullName
          }));
        this._genericCallback((ServiceType) service);
      }
    }
  }
}
