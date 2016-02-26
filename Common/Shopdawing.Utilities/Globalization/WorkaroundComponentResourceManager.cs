// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.Globalization.WorkaroundComponentResourceManager
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using System;
using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace Microsoft.Expression.Utility.Globalization
{
  public class WorkaroundComponentResourceManager : ComponentResourceManager
  {
    public WorkaroundComponentResourceManager(Type type)
      : base(type)
    {
    }

    public override ResourceSet GetResourceSet(CultureInfo culture, bool createIfNotExists, bool tryParents)
    {
      try
      {
        return base.GetResourceSet(culture, createIfNotExists, tryParents);
      }
      catch (MissingSatelliteAssemblyException ex)
      {
        return (ResourceSet) null;
      }
    }
  }
}
