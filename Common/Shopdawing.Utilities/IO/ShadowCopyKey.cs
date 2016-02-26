// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.IO.ShadowCopyKey
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Expression.Utility.IO
{
  [Serializable]
  public struct ShadowCopyKey
  {
    private static Dictionary<string, int> prefixMap = new Dictionary<string, int>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static int serialIndex = 0;
    public static readonly ShadowCopyKey Empty = new ShadowCopyKey();
    private string key;

    public string ModuleName
    {
      get
      {
        if (this.key != null)
          return this.key.Substring(this.key.IndexOf('>') + 1);
        return (string) null;
      }
    }

    public bool IsNative
    {
      get
      {
        if (this.key != null)
          return this.key.StartsWith("<nb", StringComparison.Ordinal);
        return false;
      }
    }

    public string OriginalKey
    {
      get
      {
        if (this.key == null)
          return (string) null;
        if (this.key.StartsWith("<nb", StringComparison.Ordinal))
          return "native" + this.ModuleName;
        return this.ModuleName;
      }
    }

    public bool IsBinaryShadowCopy
    {
      get
      {
        if (this.key == null)
          return false;
        if (!this.key.StartsWith("<nb", StringComparison.Ordinal))
          return this.key.StartsWith("<b", StringComparison.Ordinal);
        return true;
      }
    }

    public bool IsResourceShadowCopy
    {
      get
      {
        if (this.key != null)
          return this.key.StartsWith("<r", StringComparison.Ordinal);
        return false;
      }
    }

    private ShadowCopyKey(string key)
    {
      this.key = key;
    }

    public static bool operator ==(ShadowCopyKey left, ShadowCopyKey right)
    {
      return string.Equals(left.key, right.key, StringComparison.OrdinalIgnoreCase);
    }

    public static bool operator !=(ShadowCopyKey left, ShadowCopyKey right)
    {
      return !(left == right);
    }

    public override bool Equals(object obj)
    {
      if (obj is ShadowCopyKey)
        return this == (ShadowCopyKey) obj;
      return false;
    }

    public override int GetHashCode()
    {
      return this.key.GetHashCode();
    }

    public override string ToString()
    {
      return this.key;
    }

    public static ShadowCopyKey CreateBinaryKey(string assemblyName, string fullPath, bool isNativeModule = false)
    {
      return new ShadowCopyKey(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<{1}b{0}>{2}", (object) ShadowCopyKey.GetPrefix(fullPath), isNativeModule ? (object) "n" : (object) string.Empty, (object) assemblyName));
    }

    public static ShadowCopyKey CreateResourceKey(string assemblyName, string fullPath)
    {
      return new ShadowCopyKey("<r" + ShadowCopyKey.GetPrefix(fullPath) + ">" + assemblyName);
    }

    private static string GetPrefix(string fullPath)
    {
      int num;
      if (!ShadowCopyKey.prefixMap.TryGetValue(fullPath, out num))
      {
        string index = PathHelper.NormalizePath(fullPath);
        ShadowCopyKey.prefixMap[index] = num = ShadowCopyKey.serialIndex++;
      }
      return num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
    }
  }
}
