// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.IssueTracking.IssueTrackingProviderBase
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework.IssueTracking
{
  public abstract class IssueTrackingProviderBase : IIssueTrackingProvider
  {
    private IServiceProvider serviceProvider;
    private IIssueTrackingService issueTrackingService;

    protected IServiceProvider ServiceProvider
    {
      get
      {
        return this.serviceProvider;
      }
    }

    protected IIssueTrackingService IssueTrackingService
    {
      get
      {
        return this.issueTrackingService;
      }
    }

    public abstract string Identifier { get; }

    public event EventHandler<TrackedItemCreatedEventArgs> TrackedItemCreated;

    protected IssueTrackingProviderBase(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
      this.issueTrackingService = (IIssueTrackingService) serviceProvider.GetService(typeof (IIssueTrackingService));
    }

    protected void WorkItemCreated(ITrackedItem item)
    {
      if (this.TrackedItemCreated == null)
        return;
      this.TrackedItemCreated((object) this, new TrackedItemCreatedEventArgs(item));
    }

    public abstract void Initialize();

    public abstract void Uninitialize();

    public abstract IList<ITrackedItemType> GetItemTypes(string sourceControlFileName);

    public abstract bool SaveWorkItem(TrackedItemFormBase form);
  }
}
