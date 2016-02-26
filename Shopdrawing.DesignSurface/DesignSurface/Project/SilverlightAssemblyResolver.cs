// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Project.SilverlightAssemblyResolver
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Microsoft.Expression.DesignSurface.Project
{
  internal sealed class SilverlightAssemblyResolver : IAssemblyResolver, IDisposable
  {
    private static byte[] SilverlightPublicKeyToken = new byte[8]
    {
      (byte) 124,
      (byte) 236,
      (byte) 133,
      (byte) 215,
      (byte) 190,
      (byte) 167,
      (byte) 121,
      (byte) 142
    };
    private static byte[] SilverlightSdkPublicKeyToken = new byte[8]
    {
      (byte) 221,
      (byte) 208,
      (byte) 218,
      (byte) 77,
      (byte) 62,
      (byte) 103,
      (byte) 130,
      (byte) 23
    };
    private static byte[] SharedLibraryPublicKeyToken = new byte[8]
    {
      (byte) 49,
      (byte) 191,
      (byte) 56,
      (byte) 86,
      (byte) 173,
      (byte) 54,
      (byte) 78,
      (byte) 53
    };
    private static SilverlightAssemblyResolver.AssemblySpec MscorlibSpec = new SilverlightAssemblyResolver.AssemblySpec("mscorlib", SilverlightAssemblyResolver.SilverlightPublicKeyToken);
    private static SilverlightAssemblyResolver.AssemblySpec SystemWindowsSpec = new SilverlightAssemblyResolver.AssemblySpec("System.Windows", SilverlightAssemblyResolver.SilverlightPublicKeyToken);
    private static SilverlightAssemblyResolver.AssemblySpec[] platformAssemblies = new SilverlightAssemblyResolver.AssemblySpec[28]
    {
      new SilverlightAssemblyResolver.AssemblySpec("System", SilverlightAssemblyResolver.SilverlightPublicKeyToken),
      new SilverlightAssemblyResolver.AssemblySpec("System.Core", SilverlightAssemblyResolver.SilverlightPublicKeyToken),
      new SilverlightAssemblyResolver.AssemblySpec("System.Net", SilverlightAssemblyResolver.SilverlightPublicKeyToken),
      new SilverlightAssemblyResolver.AssemblySpec("System.Runtime.Serialization", SilverlightAssemblyResolver.SilverlightPublicKeyToken),
      new SilverlightAssemblyResolver.AssemblySpec("System.ServiceModel", SilverlightAssemblyResolver.SharedLibraryPublicKeyToken),
      new SilverlightAssemblyResolver.AssemblySpec("System.ServiceModel.Web", SilverlightAssemblyResolver.SilverlightPublicKeyToken),
      new SilverlightAssemblyResolver.AssemblySpec("System.Windows.Browser", SilverlightAssemblyResolver.SilverlightPublicKeyToken),
      SilverlightAssemblyResolver.SystemWindowsSpec,
      new SilverlightAssemblyResolver.AssemblySpec("System.Xml", SilverlightAssemblyResolver.SilverlightPublicKeyToken),
      new SilverlightAssemblyResolver.AssemblySpec("Microsoft.VisualBasic", SilverlightAssemblyResolver.SharedLibraryPublicKeyToken),
      new SilverlightAssemblyResolver.AssemblySpec("System.ComponentModel.Composition", SilverlightAssemblyResolver.SilverlightPublicKeyToken),
      new SilverlightAssemblyResolver.AssemblySpec("System.ComponentModel.Composition.Initialization", SilverlightAssemblyResolver.SilverlightPublicKeyToken),
      new SilverlightAssemblyResolver.AssemblySpec("System.ComponentModel.DataAnnotations", SilverlightAssemblyResolver.SilverlightSdkPublicKeyToken),
      new SilverlightAssemblyResolver.AssemblySpec("System.Data.Services.Client", SilverlightAssemblyResolver.SharedLibraryPublicKeyToken),
      new SilverlightAssemblyResolver.AssemblySpec("System.Json", SilverlightAssemblyResolver.SharedLibraryPublicKeyToken),
      new SilverlightAssemblyResolver.AssemblySpec("System.Runtime.Serialization.Json", SilverlightAssemblyResolver.SharedLibraryPublicKeyToken),
      new SilverlightAssemblyResolver.AssemblySpec("System.ServiceModel.PollingDuplex", SilverlightAssemblyResolver.SharedLibraryPublicKeyToken),
      new SilverlightAssemblyResolver.AssemblySpec("System.ServiceModel.Syndication", SilverlightAssemblyResolver.SharedLibraryPublicKeyToken),
      new SilverlightAssemblyResolver.AssemblySpec("System.Windows.Controls.Data", SilverlightAssemblyResolver.SharedLibraryPublicKeyToken),
      new SilverlightAssemblyResolver.AssemblySpec("System.Windows.Controls.Data.Input", SilverlightAssemblyResolver.SharedLibraryPublicKeyToken),
      new SilverlightAssemblyResolver.AssemblySpec("System.Windows.Controls", SilverlightAssemblyResolver.SharedLibraryPublicKeyToken),
      new SilverlightAssemblyResolver.AssemblySpec("System.Windows.Controls.Design", SilverlightAssemblyResolver.SharedLibraryPublicKeyToken),
      new SilverlightAssemblyResolver.AssemblySpec("System.Windows.Controls.Input", SilverlightAssemblyResolver.SharedLibraryPublicKeyToken),
      new SilverlightAssemblyResolver.AssemblySpec("System.Windows.Controls.Navigation", SilverlightAssemblyResolver.SharedLibraryPublicKeyToken),
      new SilverlightAssemblyResolver.AssemblySpec("System.Windows.Data", SilverlightAssemblyResolver.SharedLibraryPublicKeyToken),
      new SilverlightAssemblyResolver.AssemblySpec("System.Xml.Linq", SilverlightAssemblyResolver.SharedLibraryPublicKeyToken),
      new SilverlightAssemblyResolver.AssemblySpec("System.Xml.Serialization", SilverlightAssemblyResolver.SharedLibraryPublicKeyToken),
      new SilverlightAssemblyResolver.AssemblySpec("System.Xml.Utils", SilverlightAssemblyResolver.SharedLibraryPublicKeyToken)
    };
    private Dictionary<SilverlightAssemblyResolver.AssemblySpec, Assembly> resolvedAssemblies = new Dictionary<SilverlightAssemblyResolver.AssemblySpec, Assembly>();
    private const string SilverlightReferenceAssembliesPath = "SilverlightReferenceAssembliesPath";
    private const string SilverlightClientLibrariesPath = "SilverlightClientLibrariesPath";
    private IPlatformService platformService;
    private IPlatformCreator platformCreator;
    private IProjectManager projectManager;
    private IMessageDisplayService messageService;
    private ISatelliteAssemblyResolver satelliteAssemblyResolver;
    private AppDomain appDomain;
    private bool disableAssemblyRegistration;

    private IPlatformCreator PlatformCreator
    {
      get
      {
        if (this.platformCreator == null)
        {
          this.disableAssemblyRegistration = true;
          this.platformCreator = this.platformService.GetPlatformCreator("Silverlight");
          this.disableAssemblyRegistration = false;
        }
        return this.platformCreator;
      }
    }

    public SilverlightAssemblyResolver(AppDomain appDomain, IServiceProvider serviceProvider)
    {
      this.projectManager = ServiceExtensions.ProjectManager(serviceProvider);
      this.platformService = ServiceExtensions.GetService<IPlatformService>(serviceProvider);
      this.satelliteAssemblyResolver = ServiceExtensions.GetService<ISatelliteAssemblyResolver>(serviceProvider);
      this.messageService = ServiceExtensions.GetService<IMessageDisplayService>(serviceProvider);
      this.appDomain = appDomain;
      this.appDomain.AssemblyLoad += new AssemblyLoadEventHandler(this.AppDomain_AssemblyLoad);
    }

    private void AppDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
    {
      Assembly loadedAssembly = args.LoadedAssembly;
      if (!SilverlightAssemblyResolver.IsSilverlightAssembly(this.satelliteAssemblyResolver, loadedAssembly) || this.disableAssemblyRegistration)
        return;
      this.PlatformCreator.RegisterAssembly(loadedAssembly);
    }

    private static bool IsSilverlightAssembly(ISatelliteAssemblyResolver satelliteAssemblyResolver, Assembly assembly)
    {
      AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
      foreach (AssemblyName assemblyName in referencedAssemblies)
      {
        if (SilverlightAssemblyResolver.MscorlibSpec.CompareTo(assemblyName.Name, assemblyName.GetPublicKeyToken()))
          return true;
      }
      return referencedAssemblies.Length == 1 && string.Equals(referencedAssemblies[0].Name, "mscorlib", StringComparison.OrdinalIgnoreCase) && satelliteAssemblyResolver.GetCachedSatelliteAssembly(ProjectAssemblyHelper.GetAssemblyName(assembly)) != (Assembly) null;
    }

    public Assembly ResolveAssembly(AssemblyName assemblyName)
    {
      Assembly assembly1 = (Assembly) null;
      byte[] publicKeyToken = assemblyName.GetPublicKeyToken();
      if (publicKeyToken != null)
      {
        string name = assemblyName.Name;
        if (SilverlightAssemblyResolver.MscorlibSpec.CompareTo(name, publicKeyToken))
        {
          assembly1 = typeof (object).Assembly;
        }
        else
        {
          foreach (SilverlightAssemblyResolver.AssemblySpec key in SilverlightAssemblyResolver.platformAssemblies)
          {
            if (key.CompareTo(name, publicKeyToken))
            {
              Assembly assembly2 = (Assembly) null;
              if (!this.resolvedAssemblies.TryGetValue(key, out assembly2))
              {
                if (SilverlightAssemblyResolver.SystemWindowsSpec.CompareTo(name, publicKeyToken))
                {
                  try
                  {
                    this.PlatformCreator.Initialize();
                  }
                  catch (COMException ex)
                  {
                    this.messageService.ShowError(StringTable.IncompatibleSilverlightVersion);
                    throw;
                  }
                  catch (MemberAccessException ex)
                  {
                    this.messageService.ShowError(StringTable.IncompatibleSilverlightVersion);
                    throw;
                  }
                  catch (TypeLoadException ex)
                  {
                    this.messageService.ShowError(StringTable.IncompatibleSilverlightVersion);
                    throw;
                  }
                }
                if (!this.resolvedAssemblies.TryGetValue(key, out assembly2))
                {
                  assembly2 = this.LoadSilverlightPlatformAssembly(assemblyName.Name);
                  this.resolvedAssemblies[key] = assembly2;
                }
              }
              assembly1 = assembly2;
              break;
            }
          }
        }
      }
      if (this.projectManager != null && assembly1 != (Assembly) null && !assembly1.IsDynamic)
      {
        string originalAssemblyLocation = Microsoft.Expression.Framework.Documents.PathHelper.IsValidFileOrDirectoryName(assembly1.Location) ? assembly1.Location : (string) null;
        this.projectManager.AddImplicitAssemblyReference(assembly1, originalAssemblyLocation);
      }
      return assembly1;
    }

    private Assembly LoadSilverlightPlatformAssembly(string assemblyName)
    {
      foreach (string path in this.GetSilverlightPlatformPaths(assemblyName))
      {
        Assembly assembly = ProjectAssemblyHelper.LoadFrom(path);
        if (assembly != (Assembly) null)
          return assembly;
      }
      return (Assembly) null;
    }

    private IEnumerable<string> GetSilverlightPlatformPaths(string assemblyName)
    {
      string path1_1 = this.PlatformCreator.GetProperty("SilverlightReferenceAssembliesPath") as string;
      if (string.IsNullOrEmpty(path1_1))
        return (IEnumerable<string>) new string[0];
      string path1_2 = this.PlatformCreator.GetProperty("SilverlightClientLibrariesPath") as string;
      if (string.IsNullOrEmpty(path1_2))
        return (IEnumerable<string>) new string[0];
      return (IEnumerable<string>) new string[2]
      {
        Path.Combine(path1_1, assemblyName) + ".dll",
        Path.Combine(path1_2, assemblyName) + ".dll"
      };
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void Dispose(bool disposing)
    {
      if (!disposing || this.appDomain == null)
        return;
      this.appDomain.AssemblyLoad -= new AssemblyLoadEventHandler(this.AppDomain_AssemblyLoad);
    }

    private class AssemblySpec
    {
      private string name;
      private byte[] publicKeyToken;

      public AssemblySpec(string name, byte[] publicKeyToken)
      {
        this.name = name;
        this.publicKeyToken = publicKeyToken;
      }

      public bool CompareTo(string name, byte[] publicKeyToken)
      {
        if (string.Compare(this.name, name, StringComparison.OrdinalIgnoreCase) == 0)
          return ProjectAssemblyHelper.ComparePublicKeyTokens(this.publicKeyToken, publicKeyToken);
        return false;
      }
    }
  }
}
