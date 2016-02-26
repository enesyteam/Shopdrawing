// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.IReferenceAssemblyContext
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using System;
using System.Reflection;
using System.Runtime.Versioning;

namespace Microsoft.Expression.Utility
{
  public interface IReferenceAssemblyContext : IDisposable
  {
    Guid Identifier { get; }

    ReferenceAssemblyMode ReferenceAssemblyMode { get; }

    FrameworkName TargetFramework { get; }

    Assembly ResolveReferenceAssembly(string assemblySpec);

    Assembly UpdateReferenceAssembly(string name, string path, Assembly currentReferenceAssembly, Assembly currentRuntimeAssembly);
  }
}
