// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.PlatformPackage
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Extensibility.Designer.Platform;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace Microsoft.Expression.DesignSurface
{
  public sealed class PlatformPackage : IPackage, IPartImportsSatisfiedNotification
  {

    private IServices services;
    private MediaElementThumnailGrabber thumnailGrabber;

    [ImportMany]
    internal IEnumerable<IPlatformCapabilitySettings> Capabilities { get; set; }

    public void Load(IServices services)
    {
      this.services = services;
      IPlatformService platformService = (IPlatformService) new PlatformService();
      this.services.AddService(typeof (IPlatformService), (object) platformService);
      platformService.RegisterPlatformCreator(".NETFramework", new PlatformCreatorCallback(PlatformPackage.CreateWpfPlatformCreator));
      platformService.RegisterPlatformCreator("Silverlight", new PlatformCreatorCallback(PlatformPackage.CreateSilverlightPlatformCreator));
      this.thumnailGrabber = new MediaElementThumnailGrabber(this.services);
      platformService.SetProperty(".NETFramework", "MediaElementThumbnailGrabberDelegate", (object) new Action<Uri, Action<Uri, BitmapSource, string>>(this.thumnailGrabber.BeginGrabThumbnail));
      SilverlightChecker silverlightChecker = new SilverlightChecker((IServiceProvider) this.services);
      platformService.SetProperty("Silverlight", "SilverlightReferenceAssembliesPath", (object) silverlightChecker.ReferenceAssembliesPath);
      platformService.SetProperty("Silverlight", "SilverlightClientLibrariesPath", (object) silverlightChecker.ClientLibrariesPath);
    }

    public void Unload()
    {
      IPlatformService service = this.services.GetService<IPlatformService>();
      service.UnregisterPlatformCreator("Silverlight");
      service.UnregisterPlatformCreator(".NETFramework");
      this.services.RemoveService(typeof (IPlatformService));
    }

    private static IPlatformCreator CreateWpfPlatformCreator()
    {
      return PlatformPackage.CreatePlatformCreator("Microsoft.Expression.Platform.WPF.dll", "Microsoft.Expression.Platform.WPF.WpfPlatformCreator");
    }

    private static IPlatformCreator CreateSilverlightPlatformCreator()
    {
      return PlatformPackage.CreatePlatformCreator("Microsoft.Expression.Platform.Silverlight.dll", "Microsoft.Expression.Platform.Silverlight.SilverlightPlatformCreator");
    }

    private static IPlatformCreator CreatePlatformCreator(string assemblyFile, string typeName)
    {
      Assembly assembly = ProjectAssemblyHelper.LoadFrom(Path.Combine(Path.GetDirectoryName(typeof (PlatformService).Assembly.Location), assemblyFile));
      if (assembly == (Assembly) null)
        return (IPlatformCreator) null;
      return (IPlatformCreator) Activator.CreateInstance(assembly.GetType(typeName));
    }

    public void OnImportsSatisfied()
    {
      if (this.Capabilities == null)
        return;
      foreach (IPlatformCapabilitySettings capabilitySettings in this.Capabilities)
      {
        try
        {
          if (capabilitySettings.Version != 1)
            throw new ArgumentOutOfRangeException(typeof (IPlatformCapabilitySettings).ToString() + ".Version");
          PlatformCapabilitySettings settings = new PlatformCapabilitySettings(capabilitySettings.RequiredTargetFramework, capabilitySettings.MaxFrameworkVersion);
          foreach (string index in (IEnumerable<string>) capabilitySettings.Capabilities.Keys)
          {
            PlatformCapability result;
            if (Enum.TryParse<PlatformCapability>(index, true, out result))
              settings.AddCapability(result, capabilitySettings.Capabilities[index]);
          }
          PlatformTypes.AddCapabilitySettings(settings);
        }
        catch (Exception ex)
        {
          IExpressionMefHostingService service = this.services.GetService<IExpressionMefHostingService>();
          if (service != null)
            service.AddCompositionException(ex);
          else
            throw;
        }
      }
    }
  }
}
