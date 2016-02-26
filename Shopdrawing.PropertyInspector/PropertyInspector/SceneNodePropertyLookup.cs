// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SceneNodePropertyLookup
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class SceneNodePropertyLookup : INotifyPropertyChanged
  {
    private Dictionary<PropertyReference, KeyValuePair<SceneNodeProperty, bool>> cachedProperties = new Dictionary<PropertyReference, KeyValuePair<SceneNodeProperty, bool>>();
    private SceneNodeObjectSet objectSet;
    private PropertyReference baseReference;

    public virtual object this[string propertyReference]
    {
      get
      {
        int length = propertyReference.IndexOf(':');
        if (length != -1)
        {
          string str = propertyReference.Substring(0, length);
          propertyReference = propertyReference.Substring(length + 1);
          IPlatform platform = this.objectSet.ProjectContext.Platform;
          if (str != ((PlatformTypes) platform.Metadata).IdentityPrefix)
            return null;
        }
        PropertyReference index = new PropertyReference((ITypeResolver) this.objectSet.ProjectContext, propertyReference);
        KeyValuePair<SceneNodeProperty, bool> keyValuePair;
        if (!this.cachedProperties.TryGetValue(index, out keyValuePair))
        {
          PropertyReference propertyReference1 = index;
          if (this.baseReference != null)
            propertyReference1 = this.baseReference.Append(index);
          keyValuePair = new KeyValuePair<SceneNodeProperty, bool>((SceneNodeProperty) this.objectSet.CreateProperty(propertyReference1, (AttributeCollection) null), true);
          this.cachedProperties[index] = keyValuePair;
        }
        return (object) keyValuePair.Key;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public event SceneNodePropertyLookup.PropertyReferenceChangedEventHandler PropertyReferenceChanged;

    public SceneNodePropertyLookup(SceneNodeObjectSet objectSet, PropertyReference baseReference)
    {
      this.objectSet = objectSet;
      this.objectSet.SetChanged += new EventHandler(this.OnObjectSetSetChanged);
      this.baseReference = baseReference;
    }

    internal void AddProperty(PropertyReference propertyReference, SceneNodeProperty property)
    {
      this.cachedProperties[propertyReference] = new KeyValuePair<SceneNodeProperty, bool>(property, false);
    }

    internal void ClearProperties()
    {
      foreach (KeyValuePair<PropertyReference, KeyValuePair<SceneNodeProperty, bool>> keyValuePair1 in this.cachedProperties)
      {
        KeyValuePair<SceneNodeProperty, bool> keyValuePair2 = keyValuePair1.Value;
        if (keyValuePair2.Value)
          keyValuePair2.Key.OnRemoveFromCategory();
      }
      this.cachedProperties.Clear();
    }

    internal void Unhook()
    {
      if (this.objectSet == null)
        return;
      this.objectSet.SetChanged -= new EventHandler(this.OnObjectSetSetChanged);
      this.objectSet = (SceneNodeObjectSet) null;
    }

    internal void OnPropertiesUpdated()
    {
      this.OnItemsChanged();
    }

    internal void UpdateSubProperties(PropertyReference propertyReference)
    {
      if (this.PropertyReferenceChanged == null || this.objectSet.IsEmpty)
        return;
      this.PropertyReferenceChanged(propertyReference);
    }

    protected void OnItemsChanged()
    {
      if (this.PropertyChanged == null || this.objectSet.IsEmpty)
        return;
      this.PropertyChanged(this, new PropertyChangedEventArgs("Item[]"));
    }

    private void OnObjectSetSetChanged(object sender, EventArgs e)
    {
      this.OnItemsChanged();
    }

    public delegate void PropertyReferenceChangedEventHandler(PropertyReference propertyReference);
  }
}
