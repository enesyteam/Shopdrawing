// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.SourceControl.ISourceControlService
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework.SourceControl
{
  public interface ISourceControlService
  {
    ISourceControlProvider ActiveProvider { get; set; }

    IEnumerable<string> RegisteredProviders { get; }

    IEnumerable<ISourceControlProvider> LoadedProviders { get; }

    ISourceControlProvider this[string name] { get; }

    event EventHandler<SourceControlProviderEventArgs> ActiveProviderChanged;

    void RegisterProvider(string name, SourceControlProviderCreatorCallback callback);

    void RegisterProvider(string name, ISourceControlProvider instance);

    void UnregisterProvider(string name);

    void SetLoggingInformation(string moreInfoText);

    void LogMessage(string message);

    void SetProperty(string name, string propertyName, object propertyValue);

    object GetProperty(string name, string propertyName);
  }
}
