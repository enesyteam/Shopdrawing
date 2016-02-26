// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Core.DefaultProjectContext
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Expression.DesignModel.Core
{
  public sealed class DefaultProjectContext : ProjectContext
  {
    private ObservableCollection<IProjectDocument> documents;
    private XmlnsDefinitionMap namespaces;
    private IPlatform platform;

    public override IPlatform Platform
    {
      get
      {
        return this.platform;
      }
    }

    public override IAssembly ProjectAssembly
    {
      get
      {
        return (IAssembly) null;
      }
    }

    public override ICollection<IAssembly> AssemblyReferences
    {
      get
      {
        return (ICollection<IAssembly>) this.Platform.Metadata.DefaultAssemblyReferences;
      }
    }

    public override ICollection<IProjectDocument> Documents
    {
      get
      {
        return (ICollection<IProjectDocument>) this.documents;
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

    public override IXmlNamespaceTypeResolver ProjectNamespaces
    {
      get
      {
        return (IXmlNamespaceTypeResolver) this.namespaces;
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

    public override IDocumentRoot ApplicationRoot
    {
      get
      {
        return (IDocumentRoot) null;
      }
    }

    public DefaultProjectContext(IPlatform platform)
    {
      this.platform = platform;
      this.Initialize(platform.Metadata);
      this.documents = new ObservableCollection<IProjectDocument>();
      this.namespaces = platform.Metadata.CreateXmlnsDefinitionMap(platform.Metadata.DefaultTypeResolver, (IEnumerable<IAssembly>) platform.Metadata.DefaultAssemblies, (IAssembly) null);
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

    public override bool IsTypeInSolution(IType type)
    {
      return false;
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
