// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Core.DocumentContext
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.NodeBuilders;
using System;
using System.IO;

namespace Microsoft.Expression.DesignModel.Core
{
  [System.Diagnostics.DebuggerDisplay("{DebuggerDisplay}")]
  public sealed class DocumentContext : IDocumentContext, IDocumentRootResolver
  {
    private readonly IProjectContext projectContext;
    private IDocumentLocator documentLocator;
    private readonly bool isLooseXaml;

    public ITypeResolver TypeResolver
    {
      get
      {
        return (ITypeResolver) this.projectContext;
      }
    }

    public string DocumentUrl
    {
      get
      {
        if (this.DocumentLocator == null)
          return string.Empty;
        return this.DocumentLocator.Path;
      }
    }

    public bool IsLooseXaml
    {
      get
      {
        return this.isLooseXaml;
      }
    }

    public bool IsUsingDesignTimeTypes
    {
      get
      {
        return this.projectContext is TypeReflectingProjectContext;
      }
    }

    public IDocumentRoot ApplicationRoot
    {
      get
      {
        return this.projectContext.ApplicationRoot;
      }
    }

    public IDocumentLocator DocumentLocator
    {
      get
      {
        return this.documentLocator;
      }
      set
      {
        this.documentLocator = value;
      }
    }

    private IDocumentNodeBuilderFactory DocumentNodeBuilderFactory
    {
      get
      {
        return this.projectContext.Platform.DocumentNodeBuilderFactory;
      }
    }

    private string DebuggerDisplay
    {
      get
      {
        string documentUrl = this.DocumentUrl;
        if (string.IsNullOrEmpty(documentUrl))
          return documentUrl;
        return Uri.UnescapeDataString(new Uri(Path.GetDirectoryName(this.projectContext.ProjectPath), UriKind.Absolute).MakeRelativeUri(new Uri(documentUrl, UriKind.Absolute)).OriginalString).TrimStart('.', '\\', '/');
      }
    }

    public DocumentContext(IProjectContext projectContext, IDocumentLocator documentLocator)
      : this(projectContext, documentLocator, false)
    {
    }

    public DocumentContext(IProjectContext projectContext, IDocumentLocator documentLocator, bool isLooseXaml)
    {
      this.documentLocator = documentLocator;
      this.isLooseXaml = isLooseXaml;
      this.projectContext = projectContext;
    }

    public Type GetChildNodeType(Type type)
    {
      IDocumentNodeChildBuilder childBuilder = this.projectContext.Platform.DocumentNodeChildBuilderFactory.GetChildBuilder(type);
      return childBuilder != null ? childBuilder.GetChildType(type) : (Type) null;
    }

    public DocumentPrimitiveNode CreateNode(ITypeId typeId, IDocumentNodeValue value)
    {
      typeId = (ITypeId) this.projectContext.ResolveType(typeId);
      DocumentNodeMemberValue documentNodeMemberValue = value as DocumentNodeMemberValue;
      if (documentNodeMemberValue != null)
      {
        IType type = documentNodeMemberValue.Member as IType;
        IPropertyId propertyId = documentNodeMemberValue.Member as IPropertyId;
        if (type != null)
          return new DocumentPrimitiveNode((IDocumentContext) this, typeId, (IDocumentNodeValue) new DocumentNodeMemberValue((IMember) this.projectContext.ResolveType((ITypeId) type)));
        if (propertyId != null)
          return new DocumentPrimitiveNode((IDocumentContext) this, typeId, (IDocumentNodeValue) new DocumentNodeMemberValue((IMember) this.projectContext.ResolveProperty(propertyId)));
      }
      return new DocumentPrimitiveNode((IDocumentContext) this, typeId, value);
    }

    public DocumentPrimitiveNode CreateNode(string value)
    {
      return this.CreateNode((ITypeId) this.projectContext.ResolveType(PlatformTypes.String), (IDocumentNodeValue) new DocumentNodeStringValue(value));
    }

    public DocumentCompositeNode CreateNode(ITypeId typeId)
    {
      IType type = this.projectContext.ResolveType(typeId);
      DocumentCompositeNode documentCompositeNode = this.DocumentNodeBuilderFactory.BuildNode((IDocumentContext) this, type.NearestResolvedType.RuntimeType);
      documentCompositeNode.Type = type;
      return documentCompositeNode;
    }

    public DocumentCompositeNode CreateNode(Type type)
    {
      return this.DocumentNodeBuilderFactory.BuildNode((IDocumentContext) this, type);
    }

    public DocumentNode CreateNode(Type type, object value)
    {
      return this.DocumentNodeBuilderFactory.BuildNode((IDocumentContext) this, type, value);
    }

    public Uri MakeDesignTimeUri(Uri uri)
    {
      return this.projectContext.MakeDesignTimeUri(uri, this.DocumentUrl);
    }

    public string MakeResourceReference(string resourceReference)
    {
      return this.MakeResourceReference(resourceReference, false);
    }

    public string MakeResourceReference(string resourceReference, bool useProjectRelativeSyntax)
    {
      IDocumentLocator referringDocument = !useProjectRelativeSyntax ? this.DocumentLocator : (IDocumentLocator) null;
      return this.projectContext.MakeResourceReference(resourceReference, referringDocument);
    }

    public IDocumentRoot GetDocumentRoot(string path)
    {
      return this.projectContext.GetDocumentRoot(path);
    }
  }
}
