// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.SampleDataFlags
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;

namespace Microsoft.Expression.DesignSurface.SampleData
{
  [Flags]
  public enum SampleDataFlags
  {
    None = 0,
    DirtyValues = 1,
    DirtyTypes = 2,
    DirtyTypeMetadata = 4,
    DirtyDesignTimeTypes = 8,
    TypesLoadFailed = 16,
    TypesSaveFailed = 32,
    ValuesLoadFailed = 64,
    ValuesSaveFailed = 128,
    UpdateProjectItemsFailed = 256,
    CodeGenFailed = 512,
    RuntimeDisabled = 1024,
    Offline = 4096,
    SaveInProgress = 8192,
  }
}
