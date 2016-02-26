// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Documents.MultiplePropertyReferencesChangedEventArgs
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.Documents
{
  public sealed class MultiplePropertyReferencesChangedEventArgs : EventArgs
  {
    private PropertyReference[] propertyReferences;
    private SceneViewModel.ViewStateBits dirtyViewState;
    private bool forceUpdate;

    public IEnumerable<PropertyReference> PropertyReferences
    {
      get
      {
        return (IEnumerable<PropertyReference>) this.propertyReferences;
      }
    }

    public bool AllPropertiesChanged
    {
      get
      {
        return this.propertyReferences == null;
      }
    }

    public bool IsForceUpdate
    {
      get
      {
        return this.forceUpdate;
      }
    }

    public MultiplePropertyReferencesChangedEventArgs(IList<PropertyReference> propertyReferenceList, SceneViewModel.ViewStateBits dirtyViewState, bool forceUpdate)
    {
      this.dirtyViewState = dirtyViewState;
      if (propertyReferenceList != null)
      {
        this.propertyReferences = new PropertyReference[propertyReferenceList.Count];
        propertyReferenceList.CopyTo(this.propertyReferences, 0);
      }
      this.forceUpdate = forceUpdate;
    }

    public MultiplePropertyReferencesChangedEventArgs(PropertyReference propertyReference)
    {
      this.propertyReferences = new PropertyReference[1];
      this.propertyReferences[0] = propertyReference;
    }

    public bool IsDirtyViewState(SceneViewModel.ViewStateBits bits)
    {
      return (this.dirtyViewState & bits) != SceneViewModel.ViewStateBits.None;
    }
  }
}
