// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.ClrNamespaceUriParseCache
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal sealed class ClrNamespaceUriParseCache : IXmlNamespaceTypeResolver
  {
    private ITypeResolver typeResolver;
    private Dictionary<IXmlNamespace, ClrNamespaceUriParseCache.ClrNamespaceDeclaration> dictionary;

    public ClrNamespaceUriParseCache(ITypeResolver typeResolver)
    {
      this.typeResolver = typeResolver;
      this.dictionary = new Dictionary<IXmlNamespace, ClrNamespaceUriParseCache.ClrNamespaceDeclaration>();
    }

    public AssemblyNamespace GetNamespace(IXmlNamespace xmlNamespace)
    {
      AssemblyNamespace assemblyNamespace;
      this.GetNamespace(xmlNamespace, out assemblyNamespace);
      return assemblyNamespace;
    }

    public bool GetNamespace(IXmlNamespace xmlNamespace, out AssemblyNamespace assemblyNamespace)
    {
      ClrNamespaceUriParseCache.ClrNamespaceDeclaration namespaceDeclaration;
      if (!this.dictionary.TryGetValue(xmlNamespace, out namespaceDeclaration))
      {
        string clrNamespace;
        string assemblyName;
        if (!XamlParser.TryParseClrNamespaceUri(xmlNamespace.Value, out clrNamespace, out assemblyName))
        {
          assemblyNamespace = (AssemblyNamespace) null;
          return false;
        }
        namespaceDeclaration = new ClrNamespaceUriParseCache.ClrNamespaceDeclaration(this, assemblyName, clrNamespace);
        this.dictionary.Add(xmlNamespace, namespaceDeclaration);
      }
      assemblyNamespace = namespaceDeclaration.GetAssemblyNamespace();
      return true;
    }

    public bool Contains(IXmlNamespace xmlNamespace)
    {
      return this.GetNamespace(xmlNamespace) != null;
    }

    public IType GetType(IXmlNamespace xmlNamespace, string typeName)
    {
      IType type = this.typeResolver.GetType(xmlNamespace, typeName);
      if (type == null)
      {
        AssemblyNamespace @namespace = this.GetNamespace(xmlNamespace);
        if (@namespace != null)
          type = @namespace.GetType(this.typeResolver, typeName);
      }
      return type;
    }

    public IXmlNamespace GetNamespace(IAssembly assembly, Type type)
    {
      throw new InvalidOperationException();
    }

    public IXmlNamespace GetNamespace(IAssembly assembly, string clrNamespace)
    {
      throw new InvalidOperationException();
    }

    public string GetDefaultPrefix(IXmlNamespace xmlNamespace)
    {
      throw new InvalidOperationException();
    }

    public string GetClrNamespacePrefixWorkaround(IAssembly assemblyReference, string clrNamespace)
    {
      throw new InvalidOperationException();
    }

    public AssemblyNamespace Resolve(string assemblyName, string clrNamespace)
    {
      IAssembly clrAssembly;
      if (string.IsNullOrEmpty(assemblyName))
      {
        clrAssembly = this.typeResolver.ProjectAssembly;
      }
      else
      {
        clrAssembly = (IAssembly) null;
        foreach (IAssembly assembly in (IEnumerable<IAssembly>) this.typeResolver.AssemblyReferences)
        {
          if (string.Compare(assemblyName, assembly.Name, StringComparison.OrdinalIgnoreCase) == 0)
          {
            clrAssembly = assembly;
            break;
          }
        }
      }
      if (clrAssembly != null)
        return new AssemblyNamespace(clrAssembly, clrNamespace);
      return (AssemblyNamespace) null;
    }

    private sealed class ClrNamespaceDeclaration
    {
      private readonly ClrNamespaceUriParseCache namespaces;
      private readonly string assemblyName;
      private readonly string clrNamespace;
      private bool resolved;
      private AssemblyNamespace assemblyNamespace;

      public ClrNamespaceDeclaration(ClrNamespaceUriParseCache namespaces, string assemblyName, string clrNamespace)
      {
        this.namespaces = namespaces;
        this.assemblyName = ClrNamespaceUriParseCache.ClrNamespaceDeclaration.Normalize(assemblyName);
        this.clrNamespace = ClrNamespaceUriParseCache.ClrNamespaceDeclaration.Normalize(clrNamespace);
        if (this.assemblyName == null || !this.namespaces.typeResolver.IsCapabilitySet(PlatformCapability.AllowExtensionInClrNamespace))
          return;
        string extension = Path.GetExtension(this.assemblyName);
        if (string.IsNullOrEmpty(extension))
          return;
        switch (extension.ToUpperInvariant())
        {
          case ".DLL":
          case ".EXE":
            this.assemblyName = Path.GetFileNameWithoutExtension(this.assemblyName);
            break;
        }
      }

      public AssemblyNamespace GetAssemblyNamespace()
      {
        if (!this.resolved)
        {
          this.resolved = true;
          this.assemblyNamespace = this.namespaces.Resolve(this.assemblyName, this.clrNamespace);
        }
        return this.assemblyNamespace;
      }

      private static string Normalize(string value)
      {
        if (!string.IsNullOrEmpty(value))
          return value;
        return (string) null;
      }
    }
  }
}
