// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Platform.WPF.SystemThemeContext
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Expression.Platform.WPF
{
  public class SystemThemeContext : ProjectContext
  {
    private XmlnsDefinitionMap namespaces;
    private List<IAssembly> assemblyReferences;
    private IPlatform platform;

    public override IPlatform Platform
    {
      get
      {
        return this.platform;
      }
    }

    public override ICollection<IAssembly> AssemblyReferences
    {
      get
      {
        return (ICollection<IAssembly>) this.assemblyReferences;
      }
    }

    public override IXmlNamespaceTypeResolver ProjectNamespaces
    {
      get
      {
        return (IXmlNamespaceTypeResolver) this.namespaces;
      }
    }

    public override IAssembly ProjectAssembly
    {
      get
      {
        return (IAssembly) null;
      }
    }

    public override IDocumentRoot ApplicationRoot
    {
      get
      {
        return (IDocumentRoot) null;
      }
    }

    public override ICollection<IProjectDocument> Documents
    {
      get
      {
        return (ICollection<IProjectDocument>) null;
      }
    }

    public override IProjectDocument Application
    {
      get
      {
        return (IProjectDocument) null;
      }
    }

    public override IProjectDocument LocalApplication
    {
      get
      {
        return (IProjectDocument) null;
      }
    }

    public override string ProjectName
    {
      get
      {
        return (string) null;
      }
    }

    public override string ProjectPath
    {
      get
      {
        return (string) null;
      }
    }

    public override ObservableCollection<IProjectFont> ProjectFonts
    {
      get
      {
        return new ObservableCollection<IProjectFont>();
      }
    }

    public override IFontResolver FontResolver
    {
      get
      {
        return (IFontResolver) null;
      }
    }

    public SystemThemeContext(IPlatform platform, ICollection<IAssembly> themeAssemblyReferences)
    {
      this.platform = platform;
      this.Initialize(platform.Metadata);
      SystemThemeAssemblies.LoadAssemblies();
      IPlatformTypes metadata = platform.Metadata;
      this.assemblyReferences = new List<IAssembly>(themeAssemblyReferences.Count + metadata.DefaultAssemblyReferences.Count);
      this.assemblyReferences.AddRange((IEnumerable<IAssembly>) metadata.DefaultAssemblyReferences);
      this.assemblyReferences.AddRange((IEnumerable<IAssembly>) themeAssemblyReferences);
      this.namespaces = metadata.CreateXmlnsDefinitionMap((ITypeResolver) this, (IEnumerable<IAssembly>) this.assemblyReferences, (IAssembly) null);
    }

    public override bool IsTypeInSolution(IType type)
    {
      return false;
    }

    public override IProjectDocument GetDocument(IDocumentRoot documentRoot)
    {
      return (IProjectDocument) null;
    }

    public override IProjectDocument GetDocument(IDocumentLocator documentLocator)
    {
      return (IProjectDocument) null;
    }

    public override string MakeResourceReference(string resourceReference, IDocumentLocator referringDocument)
    {
      return (string) null;
    }

    public override IProjectDocument OpenDocument(string path)
    {
      return (IProjectDocument) null;
    }

    public override Uri MakeDesignTimeUri(Uri uri, string documentUrl)
    {
      return uri;
    }

    public override IDocumentRoot GetDocumentRoot(string path)
    {
      return (IDocumentRoot) null;
    }
  }
}
