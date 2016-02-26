// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.SourceControl.SourceControlService
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.UserInterface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Windows.Threading;

namespace Microsoft.Expression.Framework.SourceControl
{
  public class SourceControlService : CommandTarget, ISourceControlService
  {
    private IDictionary<string, SourceControlService.SourceControlProviderEntry> providerTable = (IDictionary<string, SourceControlService.SourceControlProviderEntry>) new Dictionary<string, SourceControlService.SourceControlProviderEntry>();
    private IServiceProvider serviceProvider;
    private ISourceControlProvider activeProvider;
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

    public IEnumerable<ISourceControlProvider> LoadedProviders
    {
      get
      {
        List<ISourceControlProvider> list = new List<ISourceControlProvider>();
        foreach (SourceControlService.SourceControlProviderEntry controlProviderEntry in (IEnumerable<SourceControlService.SourceControlProviderEntry>) this.providerTable.Values)
        {
          if (controlProviderEntry.IsLoaded)
            list.Add(controlProviderEntry.Instance);
        }
        return (IEnumerable<ISourceControlProvider>) list;
      }
    }

    public ISourceControlProvider ActiveProvider
    {
      get
      {
        return this.activeProvider;
      }
      set
      {
        if (value == this.activeProvider)
          return;
        if (value != null && !Enumerable.Contains<ISourceControlProvider>(this.LoadedProviders, value))
          throw new InvalidOperationException("Source Control Provider is not registered and loaded.");
        ISourceControlProvider oldProvider = this.activeProvider;
        this.activeProvider = value;
        this.OnActiveProviderChanged(oldProvider);
      }
    }

    public ISourceControlProvider this[string name]
    {
      get
      {
        ISourceControlProvider sourceControlProvider = (ISourceControlProvider) null;
        if (this.providerTable.ContainsKey(name))
          sourceControlProvider = this.providerTable[name].Instance;
        return sourceControlProvider;
      }
    }

    public event EventHandler<SourceControlProviderEventArgs> ActiveProviderChanged;

    public SourceControlService(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
    }

    [Conditional("DEBUG")]
    public void AddSourceControlDebugMenuItems(ICommandBarItemCollection commandItems)
    {
      commandItems.AddButton("DebugSCC_NewWorkspaces", "Open Project");
      commandItems.AddButton("DebugSCC_GetAll", "Sync All");
      commandItems.AddButton("DebugSCC_Checkout", "Checkout all files");
      commandItems.AddButton("DebugSCC_Diff", "Diff All Files");
      commandItems.AddButton("DebugSCC_Checkin", "Checkin all edited files");
      commandItems.AddButton("DebugSCC_Undo", "Revert all checked out files");
      commandItems.AddButton("DebugSCC_History", "File History");
      commandItems.AddButton("DebugSCC_Add", "Add files");
      commandItems.AddButton("DebugSCC_Delete", "Delete files");
      commandItems.AddButton("DebugSCC_QueryInfo", "Query Info");
      commandItems.AddButton("DebugSCC_GoOnline", "Attempt to go online");
      commandItems.AddButton("DebugSCC_GoOffline", "Attempt to go offline");
    }

    public void RegisterProvider(string name, SourceControlProviderCreatorCallback callback)
    {
      this.providerTable.Add(name, new SourceControlService.SourceControlProviderEntry(this.serviceProvider, callback));
    }

    public void RegisterProvider(string name, ISourceControlProvider instance)
    {
      this.providerTable.Add(name, new SourceControlService.SourceControlProviderEntry(this.serviceProvider, instance));
    }

    public void UnregisterProvider(string name)
    {
      if (!this.providerTable.ContainsKey(name))
        return;
      if (this.providerTable[name].IsLoaded)
      {
        if (this.ActiveProvider == this.providerTable[name].Instance)
          this.ActiveProvider = (ISourceControlProvider) null;
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

    private void OnActiveProviderChanged(ISourceControlProvider oldProvider)
    {
      if (this.ActiveProviderChanged == null)
        return;
      this.ActiveProviderChanged((object) this, new SourceControlProviderEventArgs(oldProvider));
    }

    private class SourceControlProviderEntry
    {
      private IDictionary<string, object> propertyTable = (IDictionary<string, object>) new Dictionary<string, object>();
      private SourceControlProviderCreatorCallback callback;
      private ISourceControlProvider instance;
      private IServiceProvider serviceProvider;

      public ISourceControlProvider Instance
      {
        get
        {
          if (this.instance == null && this.callback != null)
            this.SetSourceControlProvider(this.callback(this.serviceProvider));
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

      public SourceControlProviderEntry(IServiceProvider serviceProvider, SourceControlProviderCreatorCallback callback)
      {
        this.serviceProvider = serviceProvider;
        this.callback = callback;
      }

      public SourceControlProviderEntry(IServiceProvider serviceProvider, ISourceControlProvider instance)
      {
        this.serviceProvider = serviceProvider;
        this.SetSourceControlProvider(instance);
      }

      public void SetProperty(string propertyName, object propertyValue)
      {
        this.propertyTable[propertyName] = propertyValue;
      }

      public object GetProperty(string propertyName)
      {
        return this.propertyTable[propertyName];
      }

      private void SetSourceControlProvider(ISourceControlProvider instance)
      {
        try
        {
          ISourceControlProvider sourceControlProvider = (ISourceControlProvider) new SourceControlProviderHost(instance);
          sourceControlProvider.Initialize();
          this.instance = sourceControlProvider;
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
