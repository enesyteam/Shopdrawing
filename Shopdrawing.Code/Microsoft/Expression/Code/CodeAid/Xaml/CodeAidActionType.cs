// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.CodeAid.Xaml.CodeAidActionType
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using System;

namespace Microsoft.Expression.Code.CodeAid.Xaml
{
  [Flags]
  public enum CodeAidActionType
  {
    None = 0,
    StartSession = 2,
    CommitSession = 4,
    DismissSession = 8,
    MatchTracking = 16,
    EatInput = 32,
    MoveSelUp = 64,
    MoveSelDown = 128,
    MoveSelPageUp = 256,
    MoveSelPageDown = 512,
    MoveSelHome = 1024,
    MoveSelEnd = 2048,
    CreateCloseTag = 4096,
    AutoGenClosingBracket = 8192,
    AutoGenAttrQuotes = 16384,
    AutoGenDot = 32768,
    AutoGenIndent = 65536,
    AutoGenNewLineBelow = 131072,
    AutoGenCloseComment = 262144,
    CommitOnHollowSelection = 524288,
  }
}
