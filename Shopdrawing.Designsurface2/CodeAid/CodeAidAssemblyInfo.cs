// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.CodeAid.CodeAidAssemblyInfo
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Code;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.CodeAid
{
  internal class CodeAidAssemblyInfo : CodeAidInfoBase, ICodeAidAssemblyInfo, ICodeAidMemberInfo
  {
    private IAssembly assembly;

    public string ShortName
    {
      get
      {
        return this.assembly.Name;
      }
    }

    public string LongName
    {
      get
      {
        return this.assembly.Name;
      }
    }

    public IEnumerable<ICodeAidMemberInfo> Namespaces
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public CodeAidAssemblyInfo(CodeAidProvider owner, IAssembly assembly)
      : base(owner, assembly.Name, (string) null)
    {
      this.assembly = assembly;
    }
  }
}
