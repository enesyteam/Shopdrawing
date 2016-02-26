// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Metadata.CustomAssemblyReferencesProjectContext
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Expression.DesignModel.Metadata
{
  public class CustomAssemblyReferencesProjectContext : ProjectContext
  {
    private IProjectContext sourceContext;
    private ICollection<IAssembly> references;
    private ICollection<IAssembly> customAssemblyReferences;

    public override IPlatform Platform
    {
      get
      {
        return this.sourceContext.Platform;
      }
    }

    public override ICollection<IAssembly> AssemblyReferences
    {
      get
      {
        if (this.References == null)
        {
          this.References = (ICollection<IAssembly>) new List<IAssembly>(this.SourceContext.AssemblyReferences.Count + this.customAssemblyReferences.Count);
          foreach (IAssembly assembly in (IEnumerable<IAssembly>) this.SourceContext.AssemblyReferences)
            this.References.Add(assembly);
          foreach (IAssembly assembly in (IEnumerable<IAssembly>) this.customAssemblyReferences)
            this.References.Add(assembly);
        }
        return this.References;
      }
    }

    protected ICollection<IAssembly> CustomAssemblyReferences
    {
      get
      {
        return this.customAssemblyReferences;
      }
      set
      {
        this.customAssemblyReferences = value;
      }
    }

    protected IProjectContext SourceContext
    {
      get
      {
        return this.sourceContext;
      }
    }

    public override IAssembly ProjectAssembly
    {
      get
      {
        return this.sourceContext.ProjectAssembly;
      }
    }

    public override ICollection<IProjectDocument> Documents
    {
      get
      {
        return this.sourceContext.Documents;
      }
    }

    public override IDocumentRoot ApplicationRoot
    {
      get
      {
        return this.sourceContext.ApplicationRoot;
      }
    }

    public override IProjectDocument Application
    {
      get
      {
        return this.sourceContext.Application;
      }
    }

    public override IProjectDocument LocalApplication
    {
      get
      {
        return this.sourceContext.LocalApplication;
      }
    }

    public override string ProjectName
    {
      get
      {
        return this.sourceContext.ProjectName;
      }
    }

    public override string ProjectPath
    {
      get
      {
        return this.sourceContext.ProjectPath;
      }
    }

    public override IXmlNamespaceTypeResolver ProjectNamespaces
    {
      get
      {
        return this.sourceContext.ProjectNamespaces;
      }
    }

    public override ObservableCollection<IProjectFont> ProjectFonts
    {
      get
      {
        return this.sourceContext.ProjectFonts;
      }
    }

    public override IFontResolver FontResolver
    {
      get
      {
        return this.sourceContext.FontResolver;
      }
    }

    protected ICollection<IAssembly> References
    {
      get
      {
        return this.references;
      }
      private set
      {
        this.references = value;
      }
    }

    public CustomAssemblyReferencesProjectContext(IProjectContext sourceContext, ICollection<IAssembly> customAssemblyReferences)
    {
      this.Initialize(sourceContext.Platform.Metadata);
      this.customAssemblyReferences = customAssemblyReferences;
      this.sourceContext = sourceContext;
    }

    public override bool IsTypeInSolution(IType type)
    {
      return this.sourceContext.IsTypeInSolution(type);
    }

    public override IProjectDocument GetDocument(IDocumentRoot documentRoot)
    {
      return this.sourceContext.GetDocument(documentRoot);
    }

    public override IProjectDocument GetDocument(IDocumentLocator documentLocator)
    {
      return this.sourceContext.GetDocument(documentLocator);
    }

    public override IProjectDocument OpenDocument(string path)
    {
      return this.sourceContext.OpenDocument(path);
    }

    public override string MakeResourceReference(string resourceReference, IDocumentLocator referringDocument)
    {
      return this.sourceContext.MakeResourceReference(resourceReference, referringDocument);
    }

    public override Uri MakeDesignTimeUri(Uri uri, string documentUrl)
    {
      return this.sourceContext.MakeDesignTimeUri(uri, documentUrl);
    }

    public override IDocumentRoot GetDocumentRoot(string path)
    {
      return this.sourceContext.GetDocumentRoot(path);
    }
  }
}
