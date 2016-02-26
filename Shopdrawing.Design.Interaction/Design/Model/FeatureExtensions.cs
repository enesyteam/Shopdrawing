// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Model.FeatureExtensions
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design.Features;
using System;
using System.Collections.Generic;

namespace Microsoft.Windows.Design.Model
{
  public static class FeatureExtensions
  {
    public static IEnumerable<FeatureProvider> CreateFeatureProviders(this FeatureManager source, Type featureProviderType, ModelItem item)
    {
      if (source == null)
        throw new ArgumentNullException("source");
      if (featureProviderType == null)
        throw new ArgumentNullException("featureProviderType");
      if (item == null)
        throw new ArgumentNullException("item");
      return source.CreateFeatureProviders(featureProviderType, item.ItemType);
    }

    public static IEnumerable<FeatureProvider> CreateFeatureProviders(this FeatureManager source, Type featureProviderType, ModelItem item, Predicate<Type> match)
    {
      if (source == null)
        throw new ArgumentNullException("source");
      if (featureProviderType == null)
        throw new ArgumentNullException("featureProviderType");
      if (item == null)
        throw new ArgumentNullException("item");
      if (match == null)
        throw new ArgumentNullException("match");
      return source.CreateFeatureProviders(featureProviderType, item.ItemType, match);
    }
  }
}
