// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Services.ValueTranslationService
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using System;
using System.Collections.Generic;

namespace Microsoft.Windows.Design.Services
{
  public abstract class ValueTranslationService
  {
    public abstract event EventHandler<PropertyInvalidatedEventArgs> PropertyInvalidated;

    public abstract IEnumerable<PropertyIdentifier> GetProperties(Type itemType);

    public abstract bool HasValueTranslation(Type itemType, PropertyIdentifier identifier);

    public abstract object TranslatePropertyValue(Type itemType, ModelItem item, PropertyIdentifier identifier, object value);

    public abstract void InvalidateProperty(ModelItem item, PropertyIdentifier identifier);
  }
}
