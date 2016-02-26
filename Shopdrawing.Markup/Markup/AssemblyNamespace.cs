// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.AssemblyNamespace
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.Metadata;
using System;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal sealed class AssemblyNamespace
  {
    private IAssembly assembly;
    private string clrNamespace;

    public IAssembly Assembly
    {
      get
      {
        return this.assembly;
      }
    }

    public string ClrNamespace
    {
      get
      {
        return this.clrNamespace;
      }
    }

    public AssemblyNamespace(IAssembly clrAssembly, string clrNamespace)
    {
      this.assembly = clrAssembly;
      this.clrNamespace = clrNamespace;
    }

    public bool Represents(IAssembly clrAssembly, string clrNamespace)
    {
      return this.assembly.Equals((object) clrAssembly) && (this.clrNamespace == clrNamespace || string.IsNullOrEmpty(this.clrNamespace) && string.IsNullOrEmpty(clrNamespace));
    }

    public IType GetType(ITypeResolver typeResolver, string typeName)
    {
      if (this.Assembly.IsLoaded)
      {
        string typeName1 = TypeHelper.CombineNamespaceAndTypeName(this.ClrNamespace, typeName);
        try
        {
          return typeResolver.GetType(this.Assembly, typeName1);
        }
        catch (Exception ex)
        {
        }
      }
      return (IType) null;
    }

    public override string ToString()
    {
      return this.assembly.Name + ", " + (string.IsNullOrEmpty(this.clrNamespace) ? string.Empty : this.clrNamespace);
    }
  }
}
