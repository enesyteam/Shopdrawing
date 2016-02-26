// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Model.DesignModeValueProvider
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Services;
using MS.Internal.Features;
using System;

namespace Microsoft.Windows.Design.Model
{
  [FeatureConnector(typeof (DesignModeValueProviderConnector))]
  public class DesignModeValueProvider : FeatureProvider
  {
    private PropertyIdentifierCollection _properties;

    public PropertyIdentifierCollection Properties
    {
      get
      {
        if (this._properties == null)
          this._properties = new PropertyIdentifierCollection();
        return this._properties;
      }
    }

    public virtual object TranslatePropertyValue(ModelItem item, PropertyIdentifier identifier, object value)
    {
      if (item == null)
        throw new ArgumentNullException("item");
      if (identifier.IsEmpty)
        throw new ArgumentNullException("identifier");
      return value;
    }

    protected void InvalidateProperty(ModelItem item, PropertyIdentifier property)
    {
      if (item == null)
        throw new ArgumentNullException("item");
      item.Context.Services.GetRequiredService<ValueTranslationService>().InvalidateProperty(item, property);
    }
  }
}
