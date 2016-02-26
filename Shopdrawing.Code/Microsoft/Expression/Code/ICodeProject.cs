// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.ICodeProject
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using ActiproSoftware.SyntaxEditor.Addons.DotNet.Dom;
using Microsoft.Expression.Framework.Documents;
using System;

namespace Microsoft.Expression.Code
{
  internal interface ICodeProject : IDisposable
  {
    DotNetProjectResolver ProjectResolver { get; }

    string FullyQualifiedAssemblyName { get; }

    void ActivateEditing(DocumentReference documentReference);

    void DeactivateEditing(DocumentReference documentReference);
  }
}
