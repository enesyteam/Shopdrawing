// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.CodeAid.CodeAidAttachedPropertyInfo
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Code;
using Microsoft.Expression.DesignModel.Metadata;
using System;

namespace Microsoft.Expression.DesignSurface.CodeAid
{
  internal class CodeAidAttachedPropertyInfo : CodeAidInfoBase, ICodeAidPropertyInfo, ICodeAidMemberInfo
  {
    private Type propertyType;

    public ICodeAidTypeInfo PropertyType
    {
      get
      {
        IType type = this.owner.ProjectContext.GetType(this.propertyType);
        if (type != null)
          return (ICodeAidTypeInfo) new CodeAidTypeInfo(this.owner, type);
        return (ICodeAidTypeInfo) null;
      }
    }

    public CodeAidAttachedPropertyInfo(CodeAidProvider owner, string propertyName, Type propertyType)
      : base(owner, propertyName)
    {
      this.propertyType = propertyType;
    }
  }
}
