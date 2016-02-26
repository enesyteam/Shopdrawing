// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.DocumentModel.ClassAttributes
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using System;
using System.Globalization;

namespace Microsoft.Expression.DesignModel.DocumentModel
{
  public sealed class ClassAttributes
  {
    private string codeBehindClassName;
    private string generatedClassName;
    private string generatedClassModifier;
    private string qualifiedClassName;

    public bool SeparateGeneratedAndCodeBehindClasses
    {
      get
      {
        return this.codeBehindClassName != this.generatedClassName;
      }
    }

    public string CodeBehindClassName
    {
      get
      {
        return this.codeBehindClassName;
      }
    }

    public string QualifiedClassName
    {
      get
      {
        return this.qualifiedClassName;
      }
    }

    public string GeneratedClassName
    {
      get
      {
        return this.generatedClassName;
      }
    }

    public string GeneratedClassModifier
    {
      get
      {
        return this.generatedClassModifier;
      }
    }

    public ClassAttributes(string codeBehindClassName, string generatedClassName, string generatedClassModifier, string qualifiedClassName)
    {
      if (codeBehindClassName == null)
        throw new ArgumentNullException("codeBehindClassName");
      this.codeBehindClassName = codeBehindClassName;
      this.generatedClassName = generatedClassName == null ? codeBehindClassName : generatedClassName;
      this.generatedClassModifier = generatedClassModifier;
      this.qualifiedClassName = qualifiedClassName;
    }

    public override string ToString()
    {
      if (!this.SeparateGeneratedAndCodeBehindClasses)
        return this.codeBehindClassName;
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} : {1}", new object[2]
      {
        (object) this.codeBehindClassName,
        (object) this.generatedClassName
      });
    }
  }
}
