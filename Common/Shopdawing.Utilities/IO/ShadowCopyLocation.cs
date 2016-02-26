// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.IO.ShadowCopyLocation
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using System;

namespace Microsoft.Expression.Utility.IO
{
  [Serializable]
  public class ShadowCopyLocation
  {
    public string Source { get; set; }

    public string Path { get; set; }

    public static bool operator ==(ShadowCopyLocation left, ShadowCopyLocation right)
    {
      return left.Equals((object) right);
    }

    public static bool operator !=(ShadowCopyLocation left, ShadowCopyLocation right)
    {
      return !left.Equals((object) right);
    }

    public override bool Equals(object obj)
    {
      ShadowCopyLocation shadowCopyLocation = obj as ShadowCopyLocation;
      if (shadowCopyLocation == (ShadowCopyLocation) null || !string.Equals(this.Source, shadowCopyLocation.Source, StringComparison.OrdinalIgnoreCase))
        return false;
      return string.Equals(this.Path, shadowCopyLocation.Path, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
    {
      return (this.Source ?? string.Empty).GetHashCode() ^ (this.Path ?? string.Empty).GetHashCode();
    }
  }
}
