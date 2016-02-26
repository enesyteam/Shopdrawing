// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.CodeAid.CodeAidInfoBase
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Code;

namespace Microsoft.Expression.DesignSurface.CodeAid
{
  internal class CodeAidInfoBase : ICodeAidMemberInfo
  {
    protected CodeAidProvider owner;

    public virtual string Name { get; private set; }

    public virtual string DescriptionText { get; private set; }

    public virtual string DescriptionTitle
    {
      get
      {
        return this.Name;
      }
    }

    public virtual string DescriptionSubtitle
    {
      get
      {
        return string.Empty;
      }
    }

    internal CodeAidInfoBase(CodeAidProvider owner, string name, string description)
    {
      this.Name = name;
      this.DescriptionText = description;
      this.owner = owner;
    }

    internal CodeAidInfoBase(CodeAidProvider owner, string name)
      : this(owner, name, (string) null)
    {
    }

    public override string ToString()
    {
      return (this.GetType().Name.Length > 7 ? this.GetType().Name.Substring(7) : this.GetType().Name) + ":" + this.Name;
    }
  }
}
