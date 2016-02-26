// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Documents.SceneDocument
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.Text;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Tools.Text;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Commands.Undo;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.Documents
{
  [DebuggerDisplay("{DebuggerDisplayValue}")]
  public class SceneDocument : UndoDocument, ISceneEditTransactionFactory, IReadableTextBuffer, IDocumentLocator
  {
    private Stack<KeyValuePair<SceneEditTransaction, SceneEditTransactionType>> autoClosingTransactionStack = new Stack<KeyValuePair<SceneEditTransaction, SceneEditTransactionType>>();
    private DesignerContext designerContext;
    private SceneXamlDocument xamlDocument;
    private bool? isDesignDataDocument;
    private int openEditTransactionCount;

    public int Length
    {
      get
      {
        if (this.xamlDocument != null)
          return this.xamlDocument.TextBuffer.Length;
        return -1;
      }
    }

    public bool IsPreviewDocument { get; set; }

    public IDocumentRoot DocumentRoot
    {
      get
      {
        return (IDocumentRoot) this.xamlDocument;
      }
    }

    public bool IsActiveDocument
    {
      get
      {
        return this.designerContext.ActiveDocument == this;
      }
    }

    public IAssembly Assembly
    {
      get
      {
        return this.xamlDocument.Assembly;
      }
    }

    public ICollection<IAssembly> AssemblyReferences
    {
      get
      {
        return this.xamlDocument.AssemblyReferences;
      }
    }

    public IDocumentContext DocumentContext
    {
      get
      {
        return this.xamlDocument.DocumentContext;
      }
    }

    public IProjectContext ProjectContext
    {
      get
      {
        return this.xamlDocument.ProjectContext;
      }
    }

    public string Path
    {
      get
      {
        if (!(this.DocumentReference == (DocumentReference) null))
          return this.DocumentReference.Path;
        return string.Empty;
      }
    }

    public bool IsDesignDataDocument
    {
      get
      {
        if (!this.isDesignDataDocument.HasValue)
          this.isDesignDataDocument = new bool?(DocumentContextHelper.GetDesignDataMode((IProject) this.ProjectContext.GetService(typeof (IProject)), this.Path) != DesignDataMode.None);
        return this.isDesignDataDocument.Value;
      }
    }

    public ProjectDocumentType ProjectDocumentType
    {
      get
      {
        DocumentNode rootNode = this.XamlDocument.RootNode;
        if (rootNode == null)
          return ProjectDocumentType.Unknown;
        return SceneDocument.GetProjectDocumentTypeFromType((ITypeId) rootNode.Type);
      }
    }

    public SceneDocument ApplicationSceneDocument
    {
      get
      {
        return SceneDocument.GetApplicationDocument(this.ProjectContext);
      }
    }

    public IEnumerable<SceneDocument> DesignTimeResourceDocuments
    {
      get
      {
        return SceneDocument.GetDesignTimeResourceDocuments(this.ProjectContext);
      }
    }

    public Uri StartupUri
    {
      get
      {
        if (this.ProjectDocumentType == ProjectDocumentType.Application)
        {
          DocumentCompositeNode documentCompositeNode = this.XamlDocument.RootNode as DocumentCompositeNode;
          if (documentCompositeNode != null)
            return documentCompositeNode.GetUriValue(ApplicationMetadata.StartupUriProperty);
        }
        return (Uri) null;
      }
      set
      {
        if (this.ProjectDocumentType != ProjectDocumentType.Application)
          return;
        DocumentCompositeNode documentCompositeNode = this.XamlDocument.RootNode as DocumentCompositeNode;
        if (documentCompositeNode == null || !PlatformTypes.Application.IsAssignableFrom((ITypeId) documentCompositeNode.Type))
          return;
        DocumentNode documentNode = (DocumentNode) null;
        if (value != (Uri) null)
          documentNode = DocumentNodeUtilities.NewUriDocumentNode(this.DocumentContext, value);
        documentCompositeNode.Properties[ApplicationMetadata.StartupUriProperty] = documentNode;
      }
    }

    public SceneXamlDocument XamlDocument
    {
      get
      {
        return this.xamlDocument;
      }
    }

    public bool IsEditable
    {
      get
      {
        return this.xamlDocument.IsEditable;
      }
    }

    public bool HasOpenTransaction
    {
      get
      {
        return this.openEditTransactionCount > 0;
      }
    }

    public override bool IsDirty
    {
      get
      {
        if (base.IsDirty)
          return true;
        if (this.xamlDocument.HasTextEdits)
          return this.xamlDocument.ParseErrorsCount == 0;
        return false;
      }
    }

    public bool IsUndoingOrRedoing
    {
      get
      {
        if (!this.UndoService.IsUndoing)
          return this.UndoService.IsRedoing;
        return true;
      }
    }

    public bool CanUndo
    {
      get
      {
        if (this.openEditTransactionCount > 0)
          return false;
        TextEditProxy textEditProxy = this.designerContext.ActiveSceneViewModel.TextSelectionSet.TextEditProxy;
        if (textEditProxy != null && textEditProxy.EditingElement.CanUndo)
          return true;
        return this.UndoService.CanUndo;
      }
    }

    public bool CanRedo
    {
      get
      {
        if (this.openEditTransactionCount > 0)
          return false;
        TextEditProxy textEditProxy = this.designerContext.ActiveSceneViewModel.TextSelectionSet.TextEditProxy;
        if (textEditProxy != null && textEditProxy.EditingElement.CanRedo)
          return true;
        return this.UndoService.CanRedo;
      }
    }

    public string UndoDescription
    {
      get
      {
        TextEditProxy textEditProxy = this.designerContext.ActiveSceneViewModel.TextSelectionSet.TextEditProxy;
        if (textEditProxy != null && textEditProxy.EditingElement.CanUndo)
          return StringTable.TextEditUndo;
        return this.UndoService.UndoDescription;
      }
    }

    public string RedoDescription
    {
      get
      {
        TextEditProxy textEditProxy = this.designerContext.ActiveSceneViewModel.TextSelectionSet.TextEditProxy;
        if (textEditProxy != null && textEditProxy.EditingElement.CanRedo)
          return StringTable.TextEditUndo;
        return this.UndoService.RedoDescription;
      }
    }

    private string DebuggerDisplayValue
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}:{1}; Type: {{{2}}}", this.designerContext == null ? (object) "<null>" : (object) this.ProjectContext.ProjectAssembly.Name, this.designerContext == null ? (object) "<null>" : (object) DocumentReferenceLocator.GetDocumentReference(this.DocumentContext).DisplayName, (object) this.GetType());
      }
    }

    public event EventHandler EditTransactionCompleting;

    public event EventHandler EditTransactionCompleted;

    public event EventHandler PostEditTransactionCompleted;

    public event EventHandler EditTransactionUpdated;

    public event EventHandler PostEditTransactionUpdated;

    public event EventHandler EditTransactionCanceled;

    public event EventHandler EditTransactionUndoRedo;

    public event EventHandler PostEditTransactionUndoRedo;

    public event EventHandler TypesChanged;

    public event EventHandler RootNodeChanged;

    internal SceneDocument(DocumentReference documentReference, SceneXamlDocument xamlDocument, bool isReadOnly, DesignerContext designerContext)
      : base(documentReference, xamlDocument.UndoService, isReadOnly)
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.SceneDocumentConstructor);
      this.designerContext = designerContext;
      this.xamlDocument = xamlDocument;
      this.xamlDocument.TypesChanged += new EventHandler(this.XamlDocument_TypesChanged);
      this.xamlDocument.RootNodeChanged += new EventHandler(this.XamlDocument_RootNodeChanged);
      this.xamlDocument.HasTextEditsChanged += new EventHandler(this.XamlDocument_HasTextEditsChanged);
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.SceneDocumentConstructor);
    }

    public string GetText(int offset, int length)
    {
      if (this.xamlDocument != null)
        return this.xamlDocument.TextBuffer.GetText(offset, length);
      return string.Empty;
    }

    internal static ProjectDocumentType GetProjectDocumentTypeFromType(ITypeId rootType)
    {
      if (PlatformTypes.Application.IsAssignableFrom(rootType))
        return ProjectDocumentType.Application;
      if (PlatformTypes.ResourceDictionary.IsAssignableFrom(rootType))
        return ProjectDocumentType.ResourceDictionary;
      return PlatformTypes.UIElement.IsAssignableFrom(rootType) ? ProjectDocumentType.Page : ProjectDocumentType.Unknown;
    }

    internal static IEnumerable<SceneDocument> GetDesignTimeResourceDocuments(IProjectContext projectContext)
    {
      if (projectContext == null || projectContext.Documents == null)
        return Enumerable.Empty<SceneDocument>();
      List<SceneDocument> list = new List<SceneDocument>();
      IProjectContext projectContext1 = projectContext;
      IProjectDocument application = projectContext.Application;
      if (application != null)
      {
        SceneDocument sceneDocument = application.Document as SceneDocument;
        if (sceneDocument != null)
        {
          list.Add(sceneDocument);
          projectContext1 = sceneDocument.ProjectContext;
        }
      }
      foreach (IProjectDocument projectDocument in (IEnumerable<IProjectDocument>) projectContext1.Documents)
      {
        if (projectDocument.ProjectItem != null && projectDocument.ProjectItem.ContainsDesignTimeResources)
        {
          SceneDocument sceneDocument = projectDocument.Document as SceneDocument;
          if (sceneDocument != null)
            list.Add(sceneDocument);
        }
      }
      return (IEnumerable<SceneDocument>) list;
    }

    internal static SceneDocument GetApplicationDocument(IProjectContext activeContext)
    {
      if (activeContext != null)
      {
        IProjectDocument application = activeContext.Application;
        if (application != null)
          return application.Document as SceneDocument;
      }
      return (SceneDocument) null;
    }

    public SceneDocument GetSceneDocument(string path)
    {
      IProjectDocument projectDocument = this.xamlDocument.ProjectContext.OpenDocument(path);
      if (projectDocument != null)
        return projectDocument.Document as SceneDocument;
      return (SceneDocument) null;
    }

    public override bool ReferencesDocument(DocumentReference documentReference)
    {
      bool flag = false;
      DocumentNode rootNode = this.DocumentRoot.RootNode;
      IProjectDocument document = this.ProjectContext.GetDocument(DocumentReferenceLocator.GetDocumentLocator(documentReference));
      if (rootNode != null && document != null && document.DocumentType == ProjectDocumentType.ResourceDictionary)
      {
        foreach (DocumentNode node in rootNode.DescendantNodes)
        {
          if (this.DoesNodeReferenceUrl(node, documentReference.Path))
          {
            flag = true;
            break;
          }
        }
      }
      return flag;
    }

    private bool DoesNodeReferenceUrl(DocumentNode node, string url)
    {
      DocumentCompositeNode documentCompositeNode = node as DocumentCompositeNode;
      if (documentCompositeNode != null)
      {
        Uri uriValue = documentCompositeNode.GetUriValue(ResourceDictionaryNode.SourceProperty);
        string uriString = uriValue != (Uri) null ? uriValue.OriginalString : (string) null;
        if (!string.IsNullOrEmpty(uriString))
        {
          Uri uri = node.Context.MakeDesignTimeUri(new Uri(uriString, UriKind.RelativeOrAbsolute));
          if (!uri.IsAbsoluteUri)
            return false;
          string localPath = uri.LocalPath;
          if (StringComparer.OrdinalIgnoreCase.Compare(localPath, url) == 0)
            return true;
          return StringComparer.OrdinalIgnoreCase.Compare(localPath.Replace("file:///", "").Replace("/", "\\"), url) == 0;
        }
      }
      return false;
    }

    internal void OpenViewContext()
    {
      if (this.ApplicationSceneDocument != this || this.designerContext.ViewRootResolver == null)
        return;
      this.designerContext.ViewRootResolver.GetViewContext((IDocumentRoot) this.XamlDocument);
    }

    public override IDocumentView CreateDefaultView()
    {
      SceneView recycledView = this.designerContext.ViewRootResolver.GetRecycledView(this);
      if (recycledView != null)
        return (IDocumentView) recycledView;
      SceneViewModel viewModel = new SceneViewModel(this.designerContext, this);
      ISceneViewHost viewHost = (ISceneViewHost) this.ProjectContext.GetService(typeof (ISceneViewHost));
      string str = (string) this.ProjectContext.GetCapabilityValue(PlatformCapability.HostPlatformName);
      if (str == "SL")
        return (IDocumentView) new SilverlightSceneView(viewHost, viewModel);
      if (str == "WPF")
        return (IDocumentView) new WPFSceneView(viewHost, viewModel);
      throw new InvalidOperationException("Invalid host platform");
    }

    public void OnCompletingEditTransaction()
    {
      if (this.openEditTransactionCount != 1 || this.EditTransactionCompleting == null)
        return;
      this.EditTransactionCompleting((object) this, EventArgs.Empty);
    }

    public void OnCompletedEditTransaction(bool notifyListeners)
    {
      --this.openEditTransactionCount;
      if (this.openEditTransactionCount == 0 && notifyListeners)
      {
        if (this.EditTransactionCompleted != null)
          this.EditTransactionCompleted((object) this, EventArgs.Empty);
        if (this.PostEditTransactionCompleted != null)
          this.PostEditTransactionCompleted((object) this, EventArgs.Empty);
      }
      this.PopAutoClosingTransactionStack();
    }

    public void OnUpdatedEditTransaction()
    {
      if (this.EditTransactionUpdated != null)
        this.EditTransactionUpdated((object) this, EventArgs.Empty);
      if (this.PostEditTransactionUpdated == null)
        return;
      this.PostEditTransactionUpdated((object) this, EventArgs.Empty);
    }

    public void OnUndoRedoEditTransaction()
    {
      if (this.EditTransactionUndoRedo != null)
        this.EditTransactionUndoRedo((object) this, EventArgs.Empty);
      if (this.PostEditTransactionUndoRedo == null)
        return;
      this.PostEditTransactionUndoRedo((object) this, EventArgs.Empty);
    }

    public void OnCanceledEditTransaction(bool notifyListeners)
    {
      --this.openEditTransactionCount;
      if (this.openEditTransactionCount == 0 && notifyListeners && this.EditTransactionCanceled != null)
        this.EditTransactionCanceled((object) this, EventArgs.Empty);
      this.PopAutoClosingTransactionStack();
    }

    public SceneEditTransaction CreateEditTransaction(string description)
    {
      return this.CreateEditTransaction(description, false);
    }

    public SceneEditTransaction CreateEditTransaction(string description, bool hidden)
    {
      return this.CreateEditTransaction(description, hidden, SceneEditTransactionType.Normal);
    }

    public SceneEditTransaction CreateEditTransaction(string description, bool hidden, SceneEditTransactionType transactionType)
    {
      if (this.autoClosingTransactionStack.Count > 0 && transactionType != SceneEditTransactionType.NestedInAutoClosing)
      {
        bool flag = false;
        foreach (KeyValuePair<SceneEditTransaction, SceneEditTransactionType> keyValuePair in this.autoClosingTransactionStack)
        {
          if (keyValuePair.Value == SceneEditTransactionType.NestedInAutoClosing)
          {
            flag = true;
            break;
          }
        }
        if (!flag)
        {
          KeyValuePair<SceneEditTransaction, SceneEditTransactionType> keyValuePair = this.autoClosingTransactionStack.Pop();
          if (keyValuePair.Value == SceneEditTransactionType.AutoCommitting)
            keyValuePair.Key.Commit();
          else
            keyValuePair.Key.Cancel();
        }
      }
      IUndoTransaction undo = this.UndoService.CreateUndo(description, hidden);
      ++this.openEditTransactionCount;
      SceneEditTransaction key = new SceneEditTransaction(this.designerContext.ExternalChanges, this, undo);
      if (this.autoClosingTransactionStack.Count > 0 || transactionType == SceneEditTransactionType.AutoCommitting || transactionType == SceneEditTransactionType.AutoCancelling)
        this.autoClosingTransactionStack.Push(new KeyValuePair<SceneEditTransaction, SceneEditTransactionType>(key, transactionType));
      return key;
    }

    public void AddUndoUnit(IUndoUnit undoUnit)
    {
      this.UndoService.Add(undoUnit);
    }

    public void Undo()
    {
      TextEditProxy textEditProxy = this.designerContext.ActiveSceneViewModel.TextSelectionSet.TextEditProxy;
      if (textEditProxy != null)
      {
        if (textEditProxy.EditingElement.CanUndo)
        {
          if (!textEditProxy.EditingElement.CanRedo)
          {
            textEditProxy.Serialize();
            textEditProxy.UpdateDocumentModel();
            textEditProxy.EditingElement.Undo();
          }
          else
          {
            textEditProxy.EditingElement.Undo();
            return;
          }
        }
        else
          this.designerContext.ActiveView.TryExitTextEditMode();
        if (!this.UndoService.CanUndo)
          return;
      }
      this.UndoService.Undo();
      this.OnUndoRedoEditTransaction();
    }

    public void Redo()
    {
      TextEditProxy textEditProxy = this.designerContext.ActiveSceneViewModel.TextSelectionSet.TextEditProxy;
      if (textEditProxy != null)
      {
        if (textEditProxy.EditingElement.CanRedo)
        {
          textEditProxy.EditingElement.Redo();
          return;
        }
        this.designerContext.ActiveView.TryExitTextEditMode();
        if (!this.UndoService.CanRedo)
          return;
      }
      this.UndoService.Redo();
      this.OnUndoRedoEditTransaction();
    }

    internal void DumpUndoService()
    {
      Dump.Write(this.UndoService.ToString());
    }

    internal void ClearUndoService()
    {
      this.UndoService.Clear();
    }

    protected override void OnSaving(CancelEventArgs e)
    {
      if (this.designerContext.ActiveView != null && this.designerContext.ActiveView.EventRouter.ActiveBehavior != null)
        this.designerContext.ActiveView.EventRouter.ActiveBehavior.CommitCurrentEdit();
      base.OnSaving(e);
    }

    public override void ProjectItemChanged(DocumentReference documentReference)
    {
      this.RefreshAllInstances(documentReference);
    }

    public override void ProjectItemRemoved(DocumentReference documentReference)
    {
      this.RefreshAllInstances(documentReference);
    }

    public override void ProjectItemRenamed(DocumentReference oldReference, DocumentReference newReference)
    {
      base.ProjectItemRenamed(oldReference, newReference);
      this.RefreshAllInstances(oldReference);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.xamlDocument != null)
      {
        this.xamlDocument.TypesChanged -= new EventHandler(this.XamlDocument_TypesChanged);
        this.xamlDocument.RootNodeChanged -= new EventHandler(this.XamlDocument_RootNodeChanged);
        this.xamlDocument.Dispose();
        this.xamlDocument = (SceneXamlDocument) null;
      }
      this.designerContext = (DesignerContext) null;
      base.Dispose(disposing);
    }

    protected override void SaveCore(Stream stream)
    {
      using (SceneEditTransaction editTransaction = this.CreateEditTransaction(StringTable.CommitTextEditsUndo, true))
      {
        if (this.xamlDocument.HasTextEdits)
          this.xamlDocument.CommitTextEdits();
        this.xamlDocument.Save(stream);
        editTransaction.Commit();
      }
    }

    private void RefreshAllInstances(DocumentReference documentReference)
    {
      IProject project = ProjectHelper.GetProject(this.designerContext.ProjectManager, this.DocumentContext);
      if (project == null)
        return;
      project.GetDocumentType(documentReference.Path).RefreshAllInstances(documentReference, (IDocument) this);
    }

    private void XamlDocument_TypesChanged(object sender, EventArgs args)
    {
      if (this.TypesChanged == null)
        return;
      this.TypesChanged((object) this, args);
    }

    private void XamlDocument_RootNodeChanged(object sender, EventArgs args)
    {
      if (this.RootNodeChanged == null)
        return;
      this.RootNodeChanged((object) this, args);
    }

    private void XamlDocument_HasTextEditsChanged(object sender, EventArgs e)
    {
      this.OnIsDirtyChanged(EventArgs.Empty);
    }

    private void PopAutoClosingTransactionStack()
    {
      if (this.autoClosingTransactionStack.Count <= 0)
        return;
      this.autoClosingTransactionStack.Pop();
    }
  }
}
