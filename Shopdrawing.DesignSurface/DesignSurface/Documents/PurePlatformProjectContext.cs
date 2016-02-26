// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Documents.PurePlatformProjectContext
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Versioning;

namespace Microsoft.Expression.DesignSurface.Documents
{
  internal class PurePlatformProjectContext : IProjectContext, IDocumentRootResolver, ITypeResolver, IMetadataResolver, IServiceProvider
  {
    private IPlatform platform;
    private IProjectContext actualProjectContext;

    public ICollection<IAssembly> AssemblyReferences
    {
      get
      {
        return (ICollection<IAssembly>) this.platform.Metadata.DefaultAssemblyReferences;
      }
    }

    public IPlatform Platform
    {
      get
      {
        return this.platform;
      }
    }

    public IPlatformMetadata PlatformMetadata
    {
      get
      {
        return (IPlatformMetadata) this.platform.Metadata;
      }
    }

    public FrameworkName TargetFramework
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public IDocumentRoot ApplicationRoot
    {
      get
      {
        return (IDocumentRoot) null;
      }
    }

    public ITypeMetadataFactory MetadataFactory
    {
      get
      {
        return this.platform.Metadata.TypeMetadataFactory;
      }
    }

    public IAssembly ProjectAssembly
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public ICollection<IProjectDocument> Documents
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public IProjectDocument Application
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public IProjectDocument LocalApplication
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public string ProjectName
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public string ProjectPath
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public IXmlNamespaceTypeResolver ProjectNamespaces
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public ObservableCollection<IProjectFont> ProjectFonts
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public IFontResolver FontResolver
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public string RootNamespace
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    event EventHandler<ProjectDocumentEventArgs> IProjectContext.DocumentOpened
    {
      add
      {
      }
      remove
      {
      }
    }

    event EventHandler<ProjectDocumentEventArgs> IProjectContext.DocumentClosing
    {
      add
      {
      }
      remove
      {
      }
    }

    event EventHandler<ProjectDocumentEventArgs> IProjectContext.DocumentClosed
    {
      add
      {
      }
      remove
      {
      }
    }

    event EventHandler<TypesChangedEventArgs> ITypeResolver.TypesChangedEarly
    {
      add
      {
      }
      remove
      {
      }
    }

    event EventHandler<TypesChangedEventArgs> ITypeResolver.TypesChanged
    {
      add
      {
      }
      remove
      {
      }
    }

    public PurePlatformProjectContext(IPlatform platform)
    {
      this.platform = platform;
    }

    public void SetActualProjectContext(IProjectContext context)
    {
      this.actualProjectContext = context;
    }

    public IType GetType(Type type)
    {
      return this.platform.Metadata.GetType(type);
    }

    public IType GetType(IXmlNamespace xmlNamespace, string typeName)
    {
      throw new NotImplementedException();
    }

    public IType GetType(string assemblyName, string typeName)
    {
      throw new NotImplementedException();
    }

    public IType GetType(IAssembly assembly, string typeName)
    {
      throw new NotImplementedException();
    }

    public IType ResolveType(ITypeId typeId)
    {
      return this.PlatformMetadata.ResolveType(typeId);
    }

    public IProperty ResolveProperty(IPropertyId propertyId)
    {
      return this.PlatformMetadata.ResolveProperty(propertyId);
    }

    public bool InTargetAssembly(IType typeId)
    {
      return false;
    }

    public bool IsTypeInSolution(IType type)
    {
      return false;
    }

    public bool IsCapabilitySet(PlatformCapability capability)
    {
      return this.PlatformMetadata.IsCapabilitySet(capability);
    }

    public object GetCapabilityValue(PlatformCapability capability)
    {
      return this.PlatformMetadata.GetCapabilityValue(capability);
    }

    public IDocumentRoot GetDocumentRoot(string path)
    {
      throw new NotImplementedException();
    }

    public IProjectDocument GetDocument(IDocumentRoot documentRoot)
    {
      throw new NotImplementedException();
    }

    public IProjectDocument GetDocument(IDocumentLocator documentReference)
    {
      throw new NotImplementedException();
    }

    public IProjectDocument OpenDocument(string path)
    {
      throw new NotImplementedException();
    }

    public IAssembly GetDesignAssembly(IAssembly assembly)
    {
      throw new NotImplementedException();
    }

    public IAssembly GetAssembly(string assemblyName)
    {
      throw new NotImplementedException();
    }

    public bool EnsureAssemblyReferenced(string assemblyName)
    {
      throw new NotImplementedException();
    }

    public string MakeResourceReference(string resourceReference, IDocumentLocator referringDocument)
    {
      throw new NotImplementedException();
    }

    public Uri MakeDesignTimeUri(Uri uri, string documentUrl)
    {
      Uri uri1 = uri;
      if (this.actualProjectContext != null)
        uri1 = this.actualProjectContext.MakeDesignTimeUri(uri, documentUrl);
      return uri1;
    }

    public object GetService(Type serviceType)
    {
      if (serviceType.IsAssignableFrom(this.GetType()))
        return (object) this;
      return this.actualProjectContext.GetService(serviceType);
    }

    public bool IsTypeSupported(ITypeId type)
    {
      if (this.actualProjectContext == null)
        return this.PlatformMetadata.IsSupported((ITypeResolver) this, type);
      return this.actualProjectContext.IsTypeSupported(type);
    }
  }
}
