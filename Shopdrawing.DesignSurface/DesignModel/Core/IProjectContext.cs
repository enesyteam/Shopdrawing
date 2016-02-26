// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Core.IProjectContext
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Versioning;

namespace Microsoft.Expression.DesignModel.Core
{
  public interface IProjectContext : IDocumentRootResolver, ITypeResolver, IMetadataResolver, IServiceProvider
  {
    ITypeMetadataFactory MetadataFactory { get; }

    ICollection<IProjectDocument> Documents { get; }

    IProjectDocument Application { get; }

    IProjectDocument LocalApplication { get; }

    string ProjectName { get; }

    ObservableCollection<IProjectFont> ProjectFonts { get; }

    IFontResolver FontResolver { get; }

    FrameworkName TargetFramework { get; }

    IPlatform Platform { get; }

    event EventHandler<ProjectDocumentEventArgs> DocumentOpened;

    event EventHandler<ProjectDocumentEventArgs> DocumentClosing;

    event EventHandler<ProjectDocumentEventArgs> DocumentClosed;

    IProjectDocument GetDocument(IDocumentRoot documentRoot);

    IProjectDocument GetDocument(IDocumentLocator documentLocator);

    IProjectDocument OpenDocument(string path);

    IAssembly GetDesignAssembly(IAssembly assembly);

    string MakeResourceReference(string resourceReference, IDocumentLocator referringDocument);

    Uri MakeDesignTimeUri(Uri uri, string documentUrl);

    bool IsTypeSupported(ITypeId type);

    bool IsTypeInSolution(IType type);
  }
}
