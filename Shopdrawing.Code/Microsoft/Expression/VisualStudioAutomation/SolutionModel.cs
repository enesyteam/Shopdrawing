// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.VisualStudioAutomation.SolutionModel
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.DesignModel.Code;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.VisualStudioAutomation
{
  public abstract class SolutionModel
  {
    public bool MemberNamesAreCaseSensitive
    {
      get
      {
        return true;
      }
    }

    public StringComparison MemberNameComparison
    {
      get
      {
        return !this.MemberNamesAreCaseSensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
      }
    }

    public abstract void OpenVisualStudio(string projectItemName);

    public abstract ITypeDeclaration GetClass(IEnumerable<string> locations, string className);
  }
}
