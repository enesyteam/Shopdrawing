// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.IO.ShadowCopyCacheResult
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using System;
using System.Diagnostics;

namespace Microsoft.Expression.Utility.IO
{
  [DebuggerDisplay("{Key} {Locations.Length} {Root}")]
  [Serializable]
  public class ShadowCopyCacheResult
  {
    public ShadowCopyKey Key { get; set; }

    public string Root { get; set; }

    public bool IsPackageRoot { get; set; }

    public ShadowCopyLocation[] Locations { get; set; }
  }
}
