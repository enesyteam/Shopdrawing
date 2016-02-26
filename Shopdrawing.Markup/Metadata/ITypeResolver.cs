// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Metadata.ITypeResolver
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.Metadata
{
  public interface ITypeResolver : IMetadataResolver
  {
    ICollection<IAssembly> AssemblyReferences { get; }

    IPlatformMetadata PlatformMetadata { get; }

    string RootNamespace { get; }

    IAssembly ProjectAssembly { get; }

    IXmlNamespaceTypeResolver ProjectNamespaces { get; }

    string ProjectPath { get; }

    event EventHandler<TypesChangedEventArgs> TypesChangedEarly;

    event EventHandler<TypesChangedEventArgs> TypesChanged;

    IType GetType(Type type);

    IType GetType(IXmlNamespace xmlNamespace, string typeName);

    IType GetType(string assemblyName, string typeName);

    IType GetType(IAssembly assembly, string typeName);

    bool InTargetAssembly(IType typeId);

    bool EnsureAssemblyReferenced(string assemblyPath);

    IAssembly GetAssembly(string assemblyName);

    bool IsCapabilitySet(PlatformCapability capability);

    object GetCapabilityValue(PlatformCapability capability);
  }
}
