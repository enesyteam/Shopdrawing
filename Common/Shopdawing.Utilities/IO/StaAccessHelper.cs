// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.IO.StaAccessHelper
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using System;

namespace Microsoft.Expression.Utility.IO
{
  public static class StaAccessHelper
  {
    private static IStaAccessService staAccessService = (IStaAccessService) new Microsoft.Expression.Utility.IO.StaAccessService();

    public static IStaAccessService StaAccessService
    {
      get
      {
        return StaAccessHelper.staAccessService;
      }
      set
      {
        if (value is Microsoft.Expression.Utility.IO.StaAccessService)
        {
          StaAccessHelper.staAccessService = value;
        }
        else
        {
          if (!StaAccessHelper.IsStaAccessServiceLocal)
            throw new ArgumentException("Cannot set the StaAccessHelper more than once.");
          StaAccessHelper.staAccessService = value;
        }
      }
    }

    public static bool IsStaAccessServiceLocal
    {
      get
      {
        return StaAccessHelper.staAccessService is Microsoft.Expression.Utility.IO.StaAccessService;
      }
    }
  }
}
