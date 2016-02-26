// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Metadata.MemberType
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using System;

namespace Microsoft.Expression.DesignModel.Metadata
{
  [Flags]
  public enum MemberType
  {
    None = 0,
    Type = 1,
    LocalProperty = 2,
    AttachedProperty = 4,
    DependencyProperty = 8,
    Property = DependencyProperty | AttachedProperty | LocalProperty,
    DesignTimeProperty = 16,
    Field = 32,
    LocalEvent = 64,
    AttachedEvent = 128,
    ClrEvent = AttachedEvent | LocalEvent,
    RoutedEvent = 256,
    Event = RoutedEvent | ClrEvent,
    Method = 512,
    Constructor = 1024,
    Methods = Constructor | Method,
    IncompleteAttachedProperty = 2048,
    Other = 4096,
  }
}
