// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.AsyncDataXamlProcessor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.UserInterface;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public abstract class AsyncDataXamlProcessor : AsyncProcess
  {
    private int documentIndex;
    private List<IProjectDocument> sortedDocuments;

    public IProjectContext ProjectContext { get; private set; }

    public ExpressionEvaluator ExpressionEvaluator { get; private set; }

    public bool IsKilled { get; protected set; }

    public ChangeProcessingModes ProcessingMode { get; protected set; }

    public override int CompletedCount
    {
      get
      {
        return this.documentIndex;
      }
    }

    public override int Count
    {
      get
      {
        return this.ProjectContext.Documents.Count;
      }
    }

    public override string StatusText
    {
      get
      {
        return StringTable.ResourceReferenceSearchProgressStatus;
      }
    }

    protected IProjectDocument CurrentDocument
    {
      get
      {
        return this.sortedDocuments[this.documentIndex];
      }
    }

    public virtual bool ShouldProcessCurrentDocument
    {
      get
      {
        IProjectDocument currentDocument = this.CurrentDocument;
        return currentDocument.DocumentType == ProjectDocumentType.Application || currentDocument.DocumentType == ProjectDocumentType.ResourceDictionary || currentDocument.DocumentType == ProjectDocumentType.Page;
      }
    }

    public bool IsCollectingChanges
    {
      get
      {
        return (this.ProcessingMode & ChangeProcessingModes.CollectChanges) == ChangeProcessingModes.CollectChanges;
      }
    }

    public bool IsApplyingChanges
    {
      get
      {
        return (this.ProcessingMode & ChangeProcessingModes.ApplyChanges) == ChangeProcessingModes.ApplyChanges;
      }
    }

    protected AsyncDataXamlProcessor(IAsyncMechanism asyncMechanism, IProjectContext projectContext, ChangeProcessingModes processingMode)
      : base(asyncMechanism)
    {
      this.ProjectContext = projectContext;
      this.ProcessingMode = processingMode;
      this.ExpressionEvaluator = new ExpressionEvaluator((IDocumentRootResolver) this.ProjectContext);
      this.sortedDocuments = new List<IProjectDocument>((IEnumerable<IProjectDocument>) this.ProjectContext.Documents);
      this.sortedDocuments.Sort((Comparison<IProjectDocument>) ((a, b) => AsyncDataXamlProcessor.GetDocumentTypeOrder(a) - AsyncDataXamlProcessor.GetDocumentTypeOrder(b)));
    }

    private static int GetDocumentTypeOrder(IProjectDocument document)
    {
      switch (document.DocumentType)
      {
        case ProjectDocumentType.Application:
          return 1;
        case ProjectDocumentType.ResourceDictionary:
          return 2;
        case ProjectDocumentType.Page:
          return 0;
        default:
          return 3;
      }
    }

    public void Process(IExpressionInformationService expressionInformationService)
    {
      bool flag = false;
      if ((this.ProcessingMode & ChangeProcessingModes.NoUI) == (ChangeProcessingModes) 0)
      {
        try
        {
          flag = Dialog.ActiveModalWindow.Visibility == Visibility.Visible;
        }
        catch (Exception ex)
        {
        }
      }
      if (flag)
      {
        new AsyncProcessDialog((AsyncProcess) this, expressionInformationService).ShowDialog();
      }
      else
      {
        this.Reset();
        while (this.MoveNext() && !this.IsKilled)
          this.Work();
      }
    }

    public override void Reset()
    {
      this.IsKilled = false;
      this.documentIndex = -1;
    }

    protected override bool MoveNext()
    {
      if (this.IsKilled || this.documentIndex + 1 >= this.sortedDocuments.Count)
        return false;
      ++this.documentIndex;
      return true;
    }

    public override void Kill()
    {
      this.IsKilled = true;
    }

    protected SceneDocument GetSceneDocument(IProjectDocument projectDocument, bool checkForParseErrors)
    {
      if (projectDocument.Document == null)
        this.ProjectContext.OpenDocument(projectDocument.Path);
      SceneDocument sceneDocument = projectDocument.Document as SceneDocument;
      if (checkForParseErrors && sceneDocument.XamlDocument.ParseErrorsCount > 0)
        return (SceneDocument) null;
      return sceneDocument;
    }

    protected DocumentCompositeNode GetRootNode(IProjectDocument projectDocument, bool checkForParseErrors)
    {
      SceneDocument sceneDocument = this.GetSceneDocument(projectDocument, checkForParseErrors);
      if (sceneDocument != null)
        return sceneDocument.DocumentRoot.RootNode as DocumentCompositeNode;
      return (DocumentCompositeNode) null;
    }

    protected SceneView GetSceneView(SceneDocument sceneDocument)
    {
      return ((ISceneViewHost) this.ProjectContext.GetService(typeof (ISceneViewHost))).OpenView(sceneDocument.DocumentRoot, false);
    }

    protected DocumentCompositeNode ResolveResourceReferenceIfNeeded(DocumentCompositeNode compositeNode)
    {
      if (compositeNode == null || !compositeNode.Type.IsResource)
        return compositeNode;
      return this.ExpressionEvaluator.EvaluateExpression(new DocumentNodePath(compositeNode.DocumentRoot.RootNode, (DocumentNode) compositeNode), (DocumentNode) compositeNode) as DocumentCompositeNode;
    }
  }
}
