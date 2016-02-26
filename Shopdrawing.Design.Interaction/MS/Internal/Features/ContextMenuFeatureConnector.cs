// Decompiled with JetBrains decompiler
// Type: MS.Internal.Features.ContextMenuFeatureConnector
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Policies;
using Microsoft.Windows.Design.Services;
using MS.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MS.Internal.Features
{
  internal class ContextMenuFeatureConnector : PolicyDrivenFeatureConnector<ContextMenuProvider>
  {
    public ContextMenuFeatureConnector(FeatureManager manager)
      : base(manager)
    {
      this.Context.Services.Publish<ContextMenuService>(new ContextMenuService(this));
    }

    public IEnumerable<MenuBase> GetItems()
    {
      try
      {
        return MenuUtilities.MergeMenuGroups(new MenuUtilities.MenuBaseEnumerator(this.EnumerateRootMenuItems));
      }
      catch
      {
        return (IEnumerable<MenuBase>) new List<MenuBase>();
      }
    }

    private IEnumerable<MenuBase> EnumerateRootMenuItems()
    {
      List<ContextMenuProvider> contextMenuProviders = new List<ContextMenuProvider>();
      foreach (PolicyDrivenFeatureConnector<ContextMenuProvider>.ItemFeatureProvider itemFeatureProvider in this.FeatureProviders)
      {
        itemFeatureProvider.FeatureProvider.Update(this.Context);
        contextMenuProviders.Add(itemFeatureProvider.FeatureProvider);
      }
      contextMenuProviders.Sort(new Comparison<ContextMenuProvider>(MenuUtilities.CompareContextMenuProviders));
      foreach (ContextMenuProvider contextMenuProvider in contextMenuProviders)
      {
        foreach (MenuBase menuBase in (Collection<MenuBase>) contextMenuProvider.Items)
          yield return menuBase;
      }
    }

    protected override void FeatureProvidersAdded(ModelItem item, IEnumerable<ContextMenuProvider> featureProviders)
    {
    }

    protected override void FeatureProvidersRemoved(ModelItem item, IEnumerable<ContextMenuProvider> featureProviders)
    {
    }
  }
}
