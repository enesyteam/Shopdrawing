// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.IExpressionMefHostingService
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework
{
  public interface IExpressionMefHostingService
  {
    IEnumerable<Exception> CompositionExceptions { get; }

    void AddPart(object part);

    void AddInternalPart(object part);

    void AddAssembly(string assembly);

    void AddFolder(string folder);

    void Compose();

    void AddCompositionException(Exception exception);

    void ClearCompositionExceptions();
  }
}
