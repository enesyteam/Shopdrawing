// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SilverlightChecker
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework;
using Microsoft.Win32;
using System;
using System.IO;

namespace Microsoft.Expression.DesignSurface
{
  internal sealed class SilverlightChecker
  {
    private IServiceProvider serviceProvider;

    public bool IsAvailable
    {
      get
      {
        string referenceAssembliesPath = this.ReferenceAssembliesPath;
        if (!string.IsNullOrEmpty(referenceAssembliesPath))
          return File.Exists(Path.Combine(referenceAssembliesPath, "npctrl.dll"));
        return false;
      }
    }

    public string ReferenceAssembliesPath
    {
      get
      {
        return this.GetPathFromRegistry("SilverlightReferenceAssembliesPath", "SOFTWARE\\Microsoft\\Microsoft SDKs\\Silverlight\\v4.0\\ReferenceAssemblies", "SLRuntimeInstallPath");
      }
    }

    public string ClientLibrariesPath
    {
      get
      {
        return this.GetPathFromRegistry("SilverlightClientLibrariesPath", "SOFTWARE\\Microsoft\\Microsoft SDKs\\Silverlight\\V4.0\\AssemblyFoldersEx\\Silverlight SDK Client Libraries", (string) null);
      }
    }

    public SilverlightChecker(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
    }

    private string GetPathFromRegistry(string blendValueName, string defaultKey, string defaultValueName)
    {
      string str = string.Empty;
      IExpressionInformationService informationService = (IExpressionInformationService) this.serviceProvider.GetService(typeof (IExpressionInformationService));
      if (informationService != null)
        str = RegistryHelper.RetrieveRegistryValue<string>(Registry.LocalMachine, informationService.VersionedRegistryPath, blendValueName);
      if (string.IsNullOrEmpty(str))
        str = RegistryHelper.RetrieveRegistryValue<string>(Registry.LocalMachine, defaultKey, defaultValueName);
      return str;
    }
  }
}
