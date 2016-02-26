// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.IO.PathComparer
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Utility.IO
{
  public sealed class PathComparer : IEqualityComparer<string>
  {
    private static readonly PathComparer instance = new PathComparer();

    public static PathComparer Instance
    {
      get
      {
        return PathComparer.instance;
      }
    }

    private PathComparer()
    {
    }

    public bool Equals(string x, string y)
    {
      return PathHelper.ArePathsEquivalent(x, y);
    }

    public int GetHashCode(string obj)
    {
      if (obj == null)
        throw new ArgumentNullException("obj");
      return PathHelper.GenerateHashFromPath(obj);
    }
  }
}
