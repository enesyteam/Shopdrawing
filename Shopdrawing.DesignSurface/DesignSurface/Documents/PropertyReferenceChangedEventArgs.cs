// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventArgs
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;

namespace Microsoft.Expression.DesignSurface.Documents
{
  public sealed class PropertyReferenceChangedEventArgs : EventArgs
  {
    private static uint changeStampSeed = 1U;
    private PropertyReference propertyReference;
    private SceneViewModel.ViewStateBits dirtyViewState;

    public PropertyReference PropertyReference
    {
      get
      {
        return this.propertyReference;
      }
      set
      {
        this.propertyReference = value;
      }
    }

    public SceneViewModel.ViewStateBits DirtyViewState
    {
      get
      {
        return this.dirtyViewState;
      }
    }

    public uint ChangeStamp { get; private set; }

    public PropertyReferenceChangedEventArgs(SceneViewModel.ViewStateBits dirtyViewState, PropertyReference propertyReference)
    {
      this.ChangeStamp = PropertyReferenceChangedEventArgs.changeStampSeed++;
      this.dirtyViewState = dirtyViewState;
      this.propertyReference = propertyReference;
    }

    public bool IsDirtyViewState(SceneViewModel.ViewStateBits bits)
    {
      return (this.dirtyViewState & bits) != SceneViewModel.ViewStateBits.None;
    }
  }
}
