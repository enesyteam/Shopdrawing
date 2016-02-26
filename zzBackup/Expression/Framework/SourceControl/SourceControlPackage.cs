// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.SourceControl.SourceControlPackage
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework.IssueTracking;
using System;
using System.IO;
using System.Reflection;
using System.Security;

namespace Microsoft.Expression.Framework.SourceControl
{
  public class SourceControlPackage : IPackage
  {
    private static readonly string SourceControlProviderType = "Microsoft.Expression.SourceControl.TFS.TeamFoundationServerProvider";
    private static readonly string IssueTrackingProviderType = "Microsoft.Expression.SourceControl.TFS.TeamFoundationIssueTrackingProvider";
    private Assembly tfsAssembly;
    private IServices services;
    private ISourceControlService sourceControlService;
    private IIssueTrackingService issueTrackingService;

    public void Load(IServices services)
    {
      if (services == null)
        throw new ArgumentNullException("services");
      this.services = services;
      this.sourceControlService = (ISourceControlService) new SourceControlService((IServiceProvider) services);
      this.issueTrackingService = (IIssueTrackingService) new IssueTrackingService((IServiceProvider) services);
      this.services.AddService(typeof (ISourceControlService), (object) this.sourceControlService);
      this.services.AddService(typeof (IIssueTrackingService), (object) this.issueTrackingService);
      this.sourceControlService.RegisterProvider("TeamFoundationVersionControl", new SourceControlProviderCreatorCallback(this.LoadTeamFoundationServiceSourceControlProviderCallback));
      this.issueTrackingService.RegisterProvider("TeamFoundationVersionControl", new IssueTrackingProviderCreatorCallback(this.LoadTeamFoundationServiceIssueTrackingProviderCallback));
      this.sourceControlService.SetProperty("TeamFoundationVersionControl", "Description", (object) "Team Foundation Server");
      this.issueTrackingService.SetProperty("TeamFoundationVersionControl", "Description", (object) "Team Foundation Server");
    }

    public void Unload()
    {
      this.sourceControlService.UnregisterProvider("TeamFoundationVersionControl");
      this.issueTrackingService.UnregisterProvider("TeamFoundationVersionControl");
      this.services.RemoveService(typeof (ISourceControlService));
      this.services.RemoveService(typeof (IIssueTrackingService));
    }

    public ISourceControlProvider LoadTeamFoundationServiceSourceControlProviderCallback(IServiceProvider serviceProvider)
    {
      return (ISourceControlProvider) this.LoadTeamFoundationServiceProviderCallback(SourceControlPackage.SourceControlProviderType, serviceProvider);
    }

    public IIssueTrackingProvider LoadTeamFoundationServiceIssueTrackingProviderCallback(IServiceProvider serviceProvider)
    {
      return (IIssueTrackingProvider) this.LoadTeamFoundationServiceProviderCallback(SourceControlPackage.IssueTrackingProviderType, serviceProvider);
    }

    private object LoadTeamFoundationServiceProviderCallback(string typeName, IServiceProvider serviceProvider)
    {
      string str = Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), "Microsoft.Expression.SourceControl.TFS.dll");
      if (Microsoft.Expression.Framework.Documents.PathHelper.FileExists(str))
      {
        try
        {
          this.tfsAssembly = this.tfsAssembly ?? Assembly.LoadFrom(str);
          Type type = this.tfsAssembly.GetType(typeName);
          if (type != (Type) null)
            return Activator.CreateInstance(type, new object[1]
            {
              (object) serviceProvider
            });
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
      }
      return (object) null;
    }
  }
}
