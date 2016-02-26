// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.DocumentProcessors.DocumentProcessor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Project;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.DocumentProcessors
{
  internal abstract class DocumentProcessor : AsyncProcess
  {
    private int currentViewIndex = -1;
    private DesignerContext designerContext;
    private List<IProjectDocument> documents;
    private List<IProjectDocument> solutionDocuments;

    public override sealed int Count
    {
      get
      {
        switch (this.SearchScope)
        {
          case DocumentSearchScope.ActiveDocument:
            return Math.Min(this.DesignerContext.ViewService.Views.Count, 1);
          case DocumentSearchScope.OpenDocuments:
            return this.DesignerContext.ViewService.Views.Count;
          case DocumentSearchScope.AllDocuments:
            if (this.SolutionDocuments == null)
              return 0;
            return this.SolutionDocuments.Count;
          default:
            return 0;
        }
      }
    }

    public override sealed int CompletedCount
    {
      get
      {
        return Math.Max(this.currentViewIndex, 0);
      }
    }

    private IList<IProjectDocument> SolutionDocuments
    {
      get
      {
        if (this.solutionDocuments == null && this.designerContext.ProjectManager != null && this.designerContext.ProjectManager.CurrentSolution != null)
        {
          this.solutionDocuments = new List<IProjectDocument>();
          foreach (IXamlProject xamlProject in Enumerable.OfType<IXamlProject>((IEnumerable) this.designerContext.ProjectManager.CurrentSolution.Projects))
          {
            if (xamlProject.ProjectContext != null)
              this.solutionDocuments.AddRange((IEnumerable<IProjectDocument>) xamlProject.ProjectContext.Documents);
          }
          this.solutionDocuments = Enumerable.ToList<IProjectDocument>(Enumerable.Select<IGrouping<string, IProjectDocument>, IProjectDocument>(Enumerable.GroupBy<IProjectDocument, string>((IEnumerable<IProjectDocument>) this.solutionDocuments, (Func<IProjectDocument, string>) (document => document.Path)), (Func<IGrouping<string, IProjectDocument>, IProjectDocument>) (groupedDocuments => Enumerable.First<IProjectDocument>((IEnumerable<IProjectDocument>) Enumerable.OrderByDescending<IProjectDocument, bool>((IEnumerable<IProjectDocument>) groupedDocuments, (Func<IProjectDocument, bool>) (d => d.Document != null))))));
        }
        return (IList<IProjectDocument>) this.solutionDocuments;
      }
    }

    internal DesignerContext DesignerContext
    {
      get
      {
        return this.designerContext;
      }
    }

    private IList<IProjectDocument> ProjectDocuments
    {
      get
      {
        if (this.documents == null)
        {
          this.documents = new List<IProjectDocument>();
          switch (this.SearchScope)
          {
            case DocumentSearchScope.ActiveDocument:
              SceneDocument activeDocument = this.designerContext.ActiveDocument;
              this.documents.Add(activeDocument.ProjectContext.GetDocument((IDocumentLocator) activeDocument));
              break;
            case DocumentSearchScope.OpenDocuments:
              using (IEnumerator<IView> enumerator = this.DesignerContext.ViewService.Views.GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  SceneView sceneView = enumerator.Current as SceneView;
                  if (sceneView != null)
                    this.documents.Add(sceneView.Document.ProjectContext.GetDocument((IDocumentLocator) sceneView.Document));
                }
                break;
              }
            case DocumentSearchScope.AllDocuments:
              this.documents.AddRange((IEnumerable<IProjectDocument>) this.SolutionDocuments);
              break;
          }
        }
        return (IList<IProjectDocument>) this.documents;
      }
    }

    protected abstract DocumentSearchScope SearchScope { get; }

    public DocumentProcessor(DesignerContext designerContext, DispatcherPriority priority)
      : this(designerContext, (IAsyncMechanism) new CurrentDispatcherAsyncMechanism(priority))
    {
    }

    protected DocumentProcessor(DesignerContext designerContext, IAsyncMechanism asyncMechanism)
      : base(asyncMechanism)
    {
      this.designerContext = designerContext;
    }

    public override sealed void Reset()
    {
      this.currentViewIndex = -1;
    }

    protected override sealed void Work()
    {
      IProjectDocument projectDocument = this.ProjectDocuments[this.currentViewIndex];
      if (projectDocument == null)
        return;
      bool flag = false;
      if (projectDocument.Document == null)
      {
        IXamlProject xamlProject = this.designerContext.ProjectManager.CurrentSolution.FindProjectContainingItem(DocumentReference.Create(projectDocument.Path)) as IXamlProject;
        if (xamlProject == null)
          return;
        try
        {
          projectDocument = xamlProject.ProjectContext.OpenDocument(projectDocument.Path);
        }
        catch (NotSupportedException ex)
        {
          return;
        }
        flag = true;
        if (projectDocument == null)
          return;
      }
      SceneDocument document = projectDocument.Document as SceneDocument;
      if (document == null || !document.IsEditable)
        return;
      this.ProcessDocument(document);
      if (!flag)
        return;
      this.CloseDocument(document);
    }

    protected virtual void CloseDocument(SceneDocument document)
    {
      try
      {
        this.DesignerContext.DocumentService.CloseDocument((IDocument) document);
      }
      catch (NotSupportedException ex)
      {
      }
    }

    public override sealed void Kill()
    {
      base.Kill();
    }

    protected abstract void ProcessDocument(SceneDocument document);

    protected override sealed bool MoveNext()
    {
      return ++this.currentViewIndex < this.Count;
    }
  }
}
