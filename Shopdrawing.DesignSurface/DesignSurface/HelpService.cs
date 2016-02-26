// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.HelpService
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Expression.DesignSurface
{
  internal class HelpService : IHelpService
  {
    private List<IHelpProvider> providers = new List<IHelpProvider>();

    public bool IsHelpAvailable(object context)
    {
      return this.DetermineHelpProvider(context) != null;
    }

    public void ProvideHelp(object context)
    {
      IHelpProvider helpProvider = this.DetermineHelpProvider(context);
      if (helpProvider == null)
        return;
      helpProvider.ProvideHelp(context);
    }

    public void RegisterHelpProvider(IHelpProvider provider)
    {
      if (this.providers.Contains(provider))
        return;
      this.providers.Add(provider);
    }

    private IHelpProvider DetermineHelpProvider(object context)
    {
      return Enumerable.FirstOrDefault<IHelpProvider>((IEnumerable<IHelpProvider>) this.providers, (Func<IHelpProvider, bool>) (provider => provider.IsHelpAvailable(context)));
    }
  }
}
