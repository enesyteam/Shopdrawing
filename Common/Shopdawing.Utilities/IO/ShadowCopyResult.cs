// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.IO.ShadowCopyResult
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using System;
using System.Diagnostics;

namespace Microsoft.Expression.Utility.IO
{
  [DebuggerDisplay("{Key} {Locations.Length} {Root} {HasChanged}")]
  [Serializable]
  public sealed class ShadowCopyResult : ShadowCopyCacheResult
  {
    public bool HasChanged { get; set; }

    public ShadowCopyResult(ShadowCopyCacheResult shadowCopyCacheResult, bool hasChanged)
    {
      this.Key = shadowCopyCacheResult.Key;
      this.Root = shadowCopyCacheResult.Root;
      this.IsPackageRoot = shadowCopyCacheResult.IsPackageRoot;
      this.Locations = shadowCopyCacheResult.Locations;
      this.HasChanged = hasChanged;
    }
  }
}
