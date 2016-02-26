// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ParameterInformation
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Code;
using System;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  internal class ParameterInformation : IParameterDeclaration
  {
    private string name;
    private Type parameterType;

    public string Name
    {
      get
      {
        return this.name;
      }
    }

    public Type ParameterType
    {
      get
      {
        return this.parameterType;
      }
    }

    public ParameterInformation(Type parameterType, string name)
    {
      this.name = name;
      this.parameterType = parameterType;
    }
  }
}
