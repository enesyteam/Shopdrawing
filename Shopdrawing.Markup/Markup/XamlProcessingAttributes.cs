// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.XamlProcessingAttributes
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.Metadata;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal static class XamlProcessingAttributes
  {
    private static readonly Dictionary<string, IPropertyId> properties = new Dictionary<string, IPropertyId>();
    private static bool isInitialized;

    public static IProperty GetProperty(string name, IPlatformMetadata platformMetadata)
    {
      XamlProcessingAttributes.Initialize(platformMetadata);
      IPropertyId propertyId;
      if (!XamlProcessingAttributes.properties.TryGetValue(name, out propertyId))
        return (IProperty) null;
      return platformMetadata.ResolveProperty(propertyId);
    }

    public static bool IsProcessingAttribute(IProperty propertyKey)
    {
      XamlProcessingAttributes.Initialize(propertyKey.DeclaringType.PlatformMetadata);
      foreach (IPropertyId propertyId in XamlProcessingAttributes.properties.Values)
      {
        if (propertyId == propertyKey || propertyId.Equals((object) propertyKey))
          return true;
      }
      return false;
    }

    private static void Initialize(IPlatformMetadata platformMetadata)
    {
      if (XamlProcessingAttributes.isInitialized)
        return;
      XamlProcessingAttributes.isInitialized = true;
      XamlProcessingAttributes.AddProperty(platformMetadata.KnownProperties.DesignTimeXName);
      XamlProcessingAttributes.AddProperty(platformMetadata.KnownProperties.DictionaryEntryKey);
      XamlProcessingAttributes.AddProperty(platformMetadata.KnownProperties.DesignTimeShared);
      XamlProcessingAttributes.AddProperty(platformMetadata.KnownProperties.DesignTimeUid);
      XamlProcessingAttributes.AddProperty(platformMetadata.KnownProperties.DesignTimeClass);
      XamlProcessingAttributes.AddProperty(platformMetadata.KnownProperties.DesignTimeSubclass);
      XamlProcessingAttributes.AddProperty(platformMetadata.KnownProperties.DesignTimeClassModifier);
      XamlProcessingAttributes.AddProperty(platformMetadata.KnownProperties.DesignTimeFieldModifier);
    }

    private static void AddProperty(IPropertyId propertyKey)
    {
      XamlProcessingAttributes.properties.Add(propertyKey.Name, propertyKey);
    }
  }
}
