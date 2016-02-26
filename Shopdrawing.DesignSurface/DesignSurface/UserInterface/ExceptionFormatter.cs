// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ExceptionFormatter
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public class ExceptionFormatter
  {
    private IProjectContext projectContext;

    public ExceptionFormatter(IProjectContext projectContext)
    {
      this.projectContext = projectContext;
    }

    public FormattedException Format(Exception exception)
    {
      List<IAssembly> list = new List<IAssembly>();
      foreach (IAssembly assembly in (IEnumerable<IAssembly>) this.projectContext.AssemblyReferences)
        list.Add(assembly);
      return new FormattedException((ICollection<IAssembly>) list, exception);
    }
  }
}
