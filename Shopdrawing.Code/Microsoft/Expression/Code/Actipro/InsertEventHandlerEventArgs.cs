// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Actipro.InsertEventHandlerEventArgs
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using ActiproSoftware.SyntaxEditor.Addons.DotNet.Dom;
using Microsoft.Expression.DesignModel.Code;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Code.Actipro
{
  internal sealed class InsertEventHandlerEventArgs : EventArgs
  {
    public string MethodName { get; private set; }

    public Type ReturnType { get; private set; }

    public IEnumerable<IParameterDeclaration> Parameters { get; private set; }

    public string EventHandlerDeclaration { get; private set; }

    public IDomType DomType { get; private set; }

    public InsertEventHandlerEventArgs(Type returnType, string methodName, IEnumerable<IParameterDeclaration> parameters, string eventHandlerDeclaration, IDomType domType)
    {
      this.ReturnType = returnType;
      this.MethodName = methodName;
      this.Parameters = parameters;
      this.EventHandlerDeclaration = eventHandlerDeclaration;
      this.DomType = domType;
    }
  }
}
