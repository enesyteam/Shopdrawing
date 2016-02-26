// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.IssueTracking.IssueTrackingService
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Commands;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Windows.Threading;

namespace Microsoft.Expression.Framework.IssueTracking
{
  public class IssueTrackingService : CommandTarget, IIssueTrackingService
  {
    private IDictionary<string, IssueTrackingService.IssueTrackingProviderEntry> providerTable = (IDictionary<string, IssueTrackingService.IssueTrackingProviderEntry>) new Dictionary<string, IssueTrackingService.IssueTrackingProviderEntry>();
    private IServiceProvider serviceProvider;
    private IIssueTrackingProvider activeProvider;
    private string moreInfoText;

    private IMessageLoggingService LoggingService
    {
      get
      {
        return this.serviceProvider.GetService(typeof (IMessageLoggingService)) as IMessageLoggingService;
      }
    }

    public IEnumerable<string> RegisteredProviders
    {
      get
      {
        List<string> list = new List<string>();
        list.AddRange((IEnumerable<string>) this.providerTable.Keys);
        return (IEnumerable<string>) list;
      }
    }

    public IEnumerable<IIssueTrackingProvider> LoadedProviders
    {
      get
      {
        List<IIssueTrackingProvider> list = new List<IIssueTrackingProvider>();
        foreach (IssueTrackingService.IssueTrackingProviderEntry trackingProviderEntry in (IEnumerable<IssueTrackingService.IssueTrackingProviderEntry>) this.providerTable.Values)
        {
          if (trackingProviderEntry.IsLoaded)
            list.Add(trackingProviderEntry.Instance);
        }
        return (IEnumerable<IIssueTrackingProvider>) list;
      }
    }

    public IIssueTrackingProvider ActiveProvider
    {
      get
      {
        return this.activeProvider;
      }
      set
      {
        if (value == this.activeProvider)
          return;
        if (value != null && !Enumerable.Contains<IIssueTrackingProvider>(this.LoadedProviders, value))
          throw new InvalidOperationException("Source Control Provider is not registered and loaded.");
        IIssueTrackingProvider oldProvider = this.activeProvider;
        this.activeProvider = value;
        this.OnActiveProviderChanged(oldProvider);
      }
    }

    public IIssueTrackingProvider this[string name]
    {
      get
      {
        IIssueTrackingProvider trackingProvider = (IIssueTrackingProvider) null;
        if (this.providerTable.ContainsKey(name))
          trackingProvider = this.providerTable[name].Instance;
        return trackingProvider;
      }
    }

    public event EventHandler<IssueTrackingProviderEventArgs> ActiveProviderChanged;

    public IssueTrackingService(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
    }

    public void RegisterProvider(string name, IssueTrackingProviderCreatorCallback callback)
    {
      this.providerTable.Add(name, new IssueTrackingService.IssueTrackingProviderEntry(this.serviceProvider, callback));
    }

    public void RegisterProvider(string name, IIssueTrackingProvider instance)
    {
      this.providerTable.Add(name, new IssueTrackingService.IssueTrackingProviderEntry(this.serviceProvider, instance));
    }

    public void UnregisterProvider(string name)
    {
      if (!this.providerTable.ContainsKey(name))
        return;
      if (this.providerTable[name].IsLoaded)
      {
        if (this.ActiveProvider == this.providerTable[name].Instance)
          this.ActiveProvider = (IIssueTrackingProvider) null;
        this.providerTable[name].Instance.Uninitialize();
      }
      this.providerTable.Remove(name);
    }

    public void SetProperty(string name, string propertyName, object propertyValue)
    {
      this.providerTable[name].SetProperty(propertyName, propertyValue);
    }

    public object GetProperty(string name, string propertyName)
    {
      return this.providerTable[name].GetProperty(propertyName);
    }

    public void SetLoggingInformation(string moreInfoText)
    {
      this.moreInfoText = moreInfoText;
    }

    public void LogMessage(string message)
    {
      IMessageLoggingService loggingService = this.LoggingService;
      if (loggingService == null)
        return;
      UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.Normal, (Action) (() => loggingService.WriteLine(message.TrimStart('\r', '\n'))));
    }

    internal string FormatDialogString(string message)
    {
      if (string.IsNullOrEmpty(this.moreInfoText))
        return message;
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.MoreDetailsErrorMessage, new object[2]
      {
        (object) message,
        (object) this.moreInfoText
      });
    }

    private void OnActiveProviderChanged(IIssueTrackingProvider oldProvider)
    {
      if (this.ActiveProviderChanged == null)
        return;
      this.ActiveProviderChanged((object) this, new IssueTrackingProviderEventArgs(oldProvider));
    }

    private class IssueTrackingProviderEntry
    {
      private IDictionary<string, object> propertyTable = (IDictionary<string, object>) new Dictionary<string, object>();
      private IssueTrackingProviderCreatorCallback callback;
      private IIssueTrackingProvider instance;
      private IServiceProvider serviceProvider;

      public IIssueTrackingProvider Instance
      {
        get
        {
          if (this.instance == null && this.callback != null)
            this.SetIssueTrackingProvider(this.callback(this.serviceProvider));
          return this.instance;
        }
      }

      public bool IsLoaded
      {
        get
        {
          return this.instance != null;
        }
      }

      public IssueTrackingProviderEntry(IServiceProvider serviceProvider, IssueTrackingProviderCreatorCallback callback)
      {
        this.serviceProvider = serviceProvider;
        this.callback = callback;
      }

      public IssueTrackingProviderEntry(IServiceProvider serviceProvider, IIssueTrackingProvider instance)
      {
        this.serviceProvider = serviceProvider;
        this.SetIssueTrackingProvider(instance);
      }

      public void SetProperty(string propertyName, object propertyValue)
      {
        this.propertyTable[propertyName] = propertyValue;
      }

      public object GetProperty(string propertyName)
      {
        return this.propertyTable[propertyName];
      }

      private void SetIssueTrackingProvider(IIssueTrackingProvider instance)
      {
        try
        {
          IIssueTrackingProvider trackingProvider = instance;
          trackingProvider.Initialize();
          this.instance = trackingProvider;
        }
        catch (BadImageFormatException ex)
        {
        }
        catch (SecurityException ex)
        {
        }
        catch (IOException ex)
        {
        }
        catch (TargetInvocationException ex)
        {
        }
        catch (TypeInitializationException ex)
        {
        }
        catch (TypeLoadException ex)
        {
        }
        catch (MethodAccessException ex)
        {
        }
      }
    }
  }
}
