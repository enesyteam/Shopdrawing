// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.CodeAid.Xaml.CompletionType
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using System;

namespace Microsoft.Expression.Code.CodeAid.Xaml
{
  [Flags]
  public enum CompletionType
  {
    None = 0,
    Types = 1,
    Properties = 2,
    AttachedPropertyTypesOnly = 4,
    AttachedProperties = 8,
    EnumerationValues = 16,
    MarkupExtensions = 32,
    StartCommentMarkup = 64,
    EndCommentMarkup = 128,
    Prefixes = 256,
    ClosingTag = 512,
    ResourceNames = 1024,
    XamlNamespaceMembers = 2050,
    XmlnsMarkup = 4096,
    All = 65535,
  }
}
