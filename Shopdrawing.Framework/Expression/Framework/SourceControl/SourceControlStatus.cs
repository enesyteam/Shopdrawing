// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.SourceControl.SourceControlStatus
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

namespace Microsoft.Expression.Framework.SourceControl
{
  public enum SourceControlStatus
  {
    Invalid = -1,
    None = 0,
    Add = 1,
    Delete = 2,
    Rename = 3,
    RemoteChange = 4,
    CheckedIn = 5,
    CheckedOut = 6,
    Locked = 7,
    Branched = 8,
    Merged = 9,
  }
}
