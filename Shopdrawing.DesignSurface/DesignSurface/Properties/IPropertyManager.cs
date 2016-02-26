// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Properties.IPropertyManager
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;

namespace Microsoft.Expression.DesignSurface.Properties
{
  public interface IPropertyManager : IDisposable
  {
    event EventHandler<MultiplePropertyReferencesChangedEventArgs> MultiplePropertyReferencesChanged;

    void RegisterPropertyReferenceChangedHandler(PropertyReference propertyReference, PropertyReferenceChangedEventHandler handler, bool includeSubpropertyChanges);

    void UnregisterPropertyReferenceChangedHandler(PropertyReference propertyReference, PropertyReferenceChangedEventHandler handler);

    object GetValue(PropertyReference propertyReference);

    object GetValue(PropertyReference propertyReference, PropertyReference.GetValueFlags flags);

    void SetValue(PropertyReference propertyReference, object value);

    PropertyReference FilterProperty(SceneNode node, PropertyReference propertyReference);

    ReferenceStep FilterProperty(SceneNode node, ReferenceStep referenceStep);

    PropertyReference FilterProperty(ITypeResolver typeResolver, IType type, PropertyReference propertyReference);

    ReferenceStep FilterProperty(ITypeResolver typeResolver, IType type, ReferenceStep referenceStep);
  }
}
