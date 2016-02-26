// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ExpressionInformationService
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Extensibility;
using System;
using System.Windows;

namespace Microsoft.Expression.Framework
{
  public sealed class ExpressionInformationService : IExpressionInformationService
  {
    public Version Version { get; private set; }

    public IServices Services { get; private set; }

    public ExpressionApplication Application { get; private set; }

    public string LongApplicationName
    {
      get
      {
        if (this.Application == null)
          return (string) null;
        return this.Application.LongApplicationName;
      }
    }

    public string ShortApplicationName
    {
      get
      {
        if (this.Application == null)
          return (string) null;
        return this.Application.ShortApplicationName;
      }
    }

    public FrameworkElement MainWindowRootElement { get; set; }

    public string DefaultDialogTitle
    {
      get
      {
        return this.ShortApplicationName;
      }
    }

    public string PoliciesRegistryPath
    {
      get
      {
        if (this.Application == null)
          return (string) null;
        return this.Application.PoliciesRegistryPath;
      }
    }

    public string VersionedPoliciesRegistryPath
    {
      get
      {
        if (this.Application == null)
          return (string) null;
        return this.Application.VersionedPoliciesRegistryPath;
      }
    }

    public string RegistryPath
    {
      get
      {
        if (this.Application == null)
          return (string) null;
        return this.Application.RegistryPath;
      }
    }

    public string VersionedRegistryPath
    {
      get
      {
        if (this.Application == null)
          return (string) null;
        return this.Application.VersionedRegistryPath;
      }
    }

    public bool IsReleaseVersion
    {
      get
      {
        if (this.Application == null)
          return false;
        return this.Application.IsReleaseVersion;
      }
    }

    public bool DiagnosticMode
    {
      get
      {
        return this.Services.GetService<ICommandLineService>().GetArgument("diagnostics") != null;
      }
    }

    public ExpressionInformationService(IServices services)
      : this(services, new Version("0.1.0.0"), (ExpressionApplication) null)
    {
    }

    public ExpressionInformationService(IServices services, Version version)
      : this(services, version, (ExpressionApplication) null)
    {
    }

    public ExpressionInformationService(IServices services, Version version, ExpressionApplication application)
    {
      this.Services = services;
      this.Version = version;
      this.Application = application;
    }
  }
}
