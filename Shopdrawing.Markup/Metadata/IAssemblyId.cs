// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Metadata.IAssemblyId
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using System;

namespace Microsoft.Expression.DesignModel.Metadata
{
  public interface IAssemblyId
  {
    string Name { get; }

    Version Version { get; }

    string FullName { get; }

    byte[] GetPublicKeyToken();
  }
}
