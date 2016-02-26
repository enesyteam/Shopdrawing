// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Annotations.AnnotationService
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;

namespace Microsoft.Expression.DesignSurface.Annotations
{
  public class AnnotationService
  {
    private bool annotationsEnabled;
    private DesignerContext designerContext;

    public IEnumerable<AnnotationSceneNode> AnnotationsInActiveView
    {
      get
      {
        SceneViewModel activeSceneViewModel = this.designerContext.ActiveSceneViewModel;
        if (activeSceneViewModel == null)
          return Enumerable.Empty<AnnotationSceneNode>();
        return activeSceneViewModel.AnnotationEditor.Annotations;
      }
    }

    public bool AnnotationsEnabled
    {
      get
      {
        this.UpdateAnnotationsEnabled();
        return this.annotationsEnabled;
      }
    }

    public bool ShowAnnotations
    {
      get
      {
        return this.designerContext.AnnotationsOptionsModel.ShowAnnotations;
      }
      set
      {
        this.designerContext.AnnotationsOptionsModel.ShowAnnotations = value;
      }
    }

    internal DesignerContext DesignerContext
    {
      get
      {
        return this.designerContext;
      }
    }

    public AnnotationSceneNode SelectedAnnotation
    {
      get
      {
        AnnotationSelectionSet annotationSelectionSet = this.designerContext.SelectionManager.AnnotationSelectionSet;
        if (annotationSelectionSet == null)
          return (AnnotationSceneNode) null;
        return annotationSelectionSet.PrimarySelection;
      }
      set
      {
        AnnotationSelectionSet annotationSelectionSet = this.designerContext.SelectionManager.AnnotationSelectionSet;
        if (annotationSelectionSet == null)
          return;
        if (value == null)
          annotationSelectionSet.Clear();
        else
          annotationSelectionSet.SetSelection(value);
      }
    }

    private AnnotationLayer ActiveAnnotationLayer
    {
      get
      {
        SceneView activeView = this.designerContext.ActiveView;
        if (activeView == null)
          return (AnnotationLayer) null;
        return activeView.Artboard.AnnotationLayer;
      }
    }

    public event EventHandler<AnnotationEventArgs> AnnotationAdded;

    public event EventHandler<AnnotationEventArgs> AnnotationDeleted;

    public event EventHandler<AnnotationEventArgs> AnnotationChanged;

    public event EventHandler ShowAnnotationsChanged;

    public event EventHandler AnnotationsEnabledChanged;

    internal AnnotationService(DesignerContext designerContext)
    {
      this.designerContext = designerContext;
      this.SubscribeEvents();
    }

    internal void Shutdown()
    {
      this.UnsubscribeEvents();
    }

    public AnnotationSceneNode Create(SceneElement owner, bool forceAnnotationsVisible)
    {
      ExceptionChecks.CheckNullArgument<SceneElement>(owner, "owner");
      return this.Create((IEnumerable<SceneElement>) new SceneElement[1]
      {
        owner
      }, forceAnnotationsVisible);
    }

    public AnnotationSceneNode Create(IEnumerable<SceneElement> targets, bool forceAnnotationsVisible)
    {
      ExceptionChecks.CheckNullArgument<IEnumerable<SceneElement>>(targets, "targets");
      ExceptionChecks.CheckEmptyListArgument<SceneElement>(targets, "targets");
      if (forceAnnotationsVisible)
        this.ShowAnnotations = true;
      AnnotationSceneNode annotation = this.designerContext.ActiveSceneViewModel.AnnotationEditor.CreateAnnotation(targets);
      if (Enumerable.Any<SceneElement>(annotation.AttachedElements) && this.ShowAnnotations)
        this.MakeVisible(annotation, targets);
      return annotation;
    }

    public void MakeVisible(AnnotationSceneNode annotation)
    {
      this.MakeVisible(annotation, Enumerable.Empty<SceneElement>());
    }

    public void MakeVisible(AnnotationSceneNode annotation, IEnumerable<SceneElement> targets)
    {
      Artboard artboard = this.designerContext.ActiveView.Artboard;
      artboard.AnnotationLayer.UpdateLayout();
      targets = Enumerable.Where<SceneElement>(targets, (Func<SceneElement, bool>) (element =>
      {
        if (element != null)
          return element != element.ViewModel.RootNode;
        return false;
      }));
      Rect rect1 = Rect.Empty;
      foreach (SceneElement sceneElement in targets)
        rect1 = Rect.Union(rect1, artboard.GetElementBounds(sceneElement));
      Size size = new Size(250.0, 200.0);
      AnnotationVisual visual = annotation.Visual;
      if (visual != null)
        size = visual.RenderSize;
      Rect rect2 = new Rect(annotation.Position, size);
      Size renderSize = artboard.RenderSize;
      double zoom = artboard.Zoom;
      Rect rect2_1 = rect2;
      rect2_1.Width /= zoom;
      rect2_1.Height /= zoom;
      Rect rect3 = Rect.Union(rect1, rect2_1);
      if (rect3.IsEmpty || artboard.ArtboardBounds.Contains(rect3))
        return;
      if (rect3.Width < artboard.ArtboardBounds.Width && rect3.Height < artboard.ArtboardBounds.Height)
      {
        Rect artboardBounds = artboard.ArtboardBounds;
        if (rect3.X < artboard.ArtboardBounds.X)
          artboardBounds.X = rect3.X;
        else if (artboard.ArtboardBounds.Right < rect3.Right)
          artboardBounds.X -= artboard.ArtboardBounds.Right - rect3.Right;
        if (rect3.Y < artboard.ArtboardBounds.Y)
          artboardBounds.Y = rect3.Y;
        else if (artboard.ArtboardBounds.Bottom < rect3.Bottom)
          artboardBounds.Y -= artboard.ArtboardBounds.Bottom - rect3.Bottom;
        artboard.ZoomToFitRectangle(artboardBounds);
      }
      else
      {
        double num = Math.Max(0.01, Math.Min((renderSize.Width - rect2.Width) / rect1.Width, (renderSize.Height - rect2.Height) / rect1.Height));
        Rect rect2_2 = rect2;
        rect2_2.Width /= num;
        rect2_2.Height /= num;
        Rect rectangle = Rect.Union(rect1, rect2_2);
        artboard.ZoomToFitRectangle(rectangle);
      }
    }

    public void Delete(AnnotationSceneNode annotation)
    {
      using (SceneEditTransaction editTransaction = annotation.ViewModel.CreateEditTransaction(StringTable.DeleteAnnotationUndoUnit))
      {
        this.DeleteCore((IEnumerable<AnnotationSceneNode>) new AnnotationSceneNode[1]
        {
          annotation
        });
        editTransaction.Commit();
      }
    }

    public void Delete(IEnumerable<AnnotationSceneNode> annotations)
    {
      ExceptionChecks.CheckNullArgument<IEnumerable<AnnotationSceneNode>>(annotations, "annotations");
      ExceptionChecks.CheckEmptyListArgument<AnnotationSceneNode>(annotations, "annotations");
      SceneViewModel viewModel = Enumerable.First<AnnotationSceneNode>(annotations).ViewModel;
      if (Enumerable.Any<AnnotationSceneNode>(annotations, (Func<AnnotationSceneNode, bool>) (anno => anno.ViewModel != viewModel)))
        throw new ArgumentException("All of the annotations must be from the same document!", "annotations");
      using (SceneEditTransaction editTransaction = viewModel.CreateEditTransaction(StringTable.DeleteAnnotationsInDocumentUndoUnit))
      {
        this.DeleteCore(annotations);
        editTransaction.Commit();
      }
    }

    public void UnlinkAllAttachments(AnnotationSceneNode annotation)
    {
      IEnumerable<SceneElement> attachedElements = annotation.AttachedElements;
      if (!Enumerable.Any<SceneElement>(attachedElements))
        return;
      SceneViewModel viewModel = annotation.ViewModel;
      using (SceneEditTransaction editTransaction = viewModel.CreateEditTransaction(StringTable.UnlinkAnnotationUndoUnit))
      {
        EnumerableExtensions.ForEach<SceneElement>(attachedElements, (Action<SceneElement>) (element => AnnotationUtils.RemoveAnnotationReference(element, annotation)));
        AnnotationManagerSceneNode.SetAnnotationParent(annotation, viewModel.RootNode);
        editTransaction.Commit();
      }
    }

    public void UnlinkAttachment(AnnotationSceneNode annotation, SceneElement target)
    {
      SceneViewModel viewModel = annotation.ViewModel;
      using (SceneEditTransaction editTransaction = viewModel.CreateEditTransaction(StringTable.UnlinkAnnotationUndoUnit))
      {
        bool flag = AnnotationUtils.RemoveAnnotationReference(target, annotation);
        if (target.Equals((object) annotation.Parent))
        {
          flag = true;
          AnnotationManagerSceneNode.SetAnnotationParent(annotation, viewModel.RootNode);
        }
        if (!flag)
          return;
        editTransaction.Commit();
      }
    }

    public void CopyToClipboardAsText(AnnotationSceneNode annotation)
    {
      ExceptionChecks.CheckNullArgument<AnnotationSceneNode>(annotation, "annotation");
      this.CopyToClipboardAsText((IEnumerable<AnnotationSceneNode>) new AnnotationSceneNode[1]
      {
        annotation
      });
    }

    public void CopyToClipboardAsText(IEnumerable<AnnotationSceneNode> annotations)
    {
      ExceptionChecks.CheckNullArgument<IEnumerable<AnnotationSceneNode>>(annotations, "annotations");
      ExceptionChecks.CheckEmptyListArgument<AnnotationSceneNode>(annotations, "annotations");
      FlowDocument flowDoc = this.GenerateFlowDocument(annotations);
      DataObject dataObj = new DataObject();
      this.SaveFlowDocIntoDataObject(flowDoc, dataObj, DataFormats.Rtf);
      this.SaveFlowDocIntoDataObject(flowDoc, dataObj, DataFormats.Text);
      Clipboard.SetDataObject((object) dataObj);
    }

    private FlowDocument GenerateFlowDocument(IEnumerable<AnnotationSceneNode> annotations)
    {
      ExceptionChecks.CheckNullArgument<IEnumerable<AnnotationSceneNode>>(annotations, "annotations");
      ExceptionChecks.CheckEmptyListArgument<AnnotationSceneNode>(annotations, "annotations");
      return this.GenerateFlowDocument(Enumerable.Select<AnnotationSceneNode, RawAnnotation>(annotations, (Func<AnnotationSceneNode, RawAnnotation>) (annotation => new RawAnnotation(annotation))));
    }

    public FlowDocument GenerateFlowDocument(IEnumerable<RawAnnotation> annotations)
    {
      ExceptionChecks.CheckNullArgument<IEnumerable<RawAnnotation>>(annotations, "annotations");
      ExceptionChecks.CheckEmptyListArgument<RawAnnotation>(annotations, "annotations");
      FlowDocument flowDocument = new FlowDocument();
      foreach (RawAnnotation annotation in annotations)
      {
        flowDocument.Blocks.Add((Block) new CopyAnnotationTextTemplate(annotation));
        flowDocument.Blocks.Add((Block) new Section((Block) new Paragraph()));
      }
      flowDocument.Blocks.Remove(Enumerable.Last<Block>((IEnumerable<Block>) flowDocument.Blocks));
      return flowDocument;
    }

    public IEnumerable<RawAnnotation> GetAnnotations(SceneDocument sceneDoc)
    {
      ExceptionChecks.CheckNullArgument<SceneDocument>(sceneDoc, "sceneDoc");
      if (sceneDoc.DocumentRoot == null || sceneDoc.DocumentRoot.RootNode == null)
        return Enumerable.Empty<RawAnnotation>();
      return (IEnumerable<RawAnnotation>) Enumerable.ToList<RawAnnotation>(Enumerable.Select<DocumentCompositeNode, RawAnnotation>(Enumerable.OfType<DocumentCompositeNode>((IEnumerable) sceneDoc.DocumentRoot.RootNode.SelectDescendantNodes(PlatformTypes.Annotation)), (Func<DocumentCompositeNode, RawAnnotation>) (anno => new RawAnnotation()
      {
        Id = this.GetPrimitiveProperty<string>(anno, AnnotationSceneNode.IdProperty, AnnotationDefaults.Id),
        Text = this.GetPrimitiveProperty<string>(anno, AnnotationSceneNode.TextProperty, AnnotationDefaults.Text),
        Top = this.GetPrimitiveProperty<double>(anno, AnnotationSceneNode.TopProperty, AnnotationDefaults.Top),
        Left = this.GetPrimitiveProperty<double>(anno, AnnotationSceneNode.LeftProperty, AnnotationDefaults.Left),
        Author = this.GetPrimitiveProperty<string>(anno, AnnotationSceneNode.AuthorProperty, AnnotationDefaults.Author),
        AuthorInitials = this.GetPrimitiveProperty<string>(anno, AnnotationSceneNode.AuthorInitialsProperty, AnnotationDefaults.AuthorInitials),
        SerialNumber = this.GetPrimitiveProperty<int>(anno, AnnotationSceneNode.SerialNumberProperty, AnnotationDefaults.SerialNumber),
        VisibleAtRuntime = this.GetPrimitiveProperty<bool>(anno, AnnotationSceneNode.VisibleAtRuntimeProperty, AnnotationDefaults.VisibleAtRuntime),
        Timestamp = this.GetPrimitiveProperty<DateTime>(anno, AnnotationSceneNode.TimestampProperty, AnnotationDefaults.Timestamp)
      })));
    }

    public bool IsSelected(AnnotationSceneNode annotation)
    {
      ExceptionChecks.CheckNullArgument<AnnotationSceneNode>(annotation, "annotation");
      return this.SelectedAnnotation == annotation;
    }

    private T GetPrimitiveProperty<T>(DocumentCompositeNode docNode, IPropertyId property, T defaultValue)
    {
      DocumentPrimitiveNode documentPrimitiveNode = docNode.Properties[property] as DocumentPrimitiveNode;
      if (documentPrimitiveNode == null)
        return defaultValue;
      return documentPrimitiveNode.GetValue<T>();
    }

    private void RefreshToolRelatedSettings()
    {
      ToolManager toolManager = this.designerContext.ToolManager;
      if (toolManager == null || toolManager.ActiveTool == null || this.ActiveAnnotationLayer == null)
        return;
      string identifier = toolManager.ActiveTool.Identifier;
      this.ActiveAnnotationLayer.IsHitTestVisible = identifier.Equals("Selection") || identifier.Equals("Subselection");
      SceneViewModel activeSceneViewModel = this.designerContext.ActiveSceneViewModel;
      if (activeSceneViewModel == null)
        return;
      foreach (AnnotationSceneNode annotationSceneNode in activeSceneViewModel.AnnotationEditor.Annotations)
      {
        AnnotationVisual visual = annotationSceneNode.Visual;
        if (visual != null)
          visual.ViewModel.RefreshToolRelatedProperties();
      }
    }

    private void DeleteCore(IEnumerable<AnnotationSceneNode> annotations)
    {
      ExceptionChecks.CheckNullArgument<IEnumerable<AnnotationSceneNode>>(annotations, "annotations");
      ExceptionChecks.CheckEmptyListArgument<AnnotationSceneNode>(annotations, "annotations");
      foreach (AnnotationSceneNode annotationSceneNode in Enumerable.ToList<AnnotationSceneNode>(annotations))
      {
        foreach (SceneElement element in Enumerable.ToList<SceneElement>(annotationSceneNode.AttachedElements))
          AnnotationUtils.RemoveAnnotationReference(element, annotationSceneNode);
        AnnotationManagerSceneNode.DeleteAnnotation(annotationSceneNode);
      }
    }

    private void SaveFlowDocIntoDataObject(FlowDocument flowDoc, DataObject dataObj, string format)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        new TextRange(flowDoc.ContentStart, flowDoc.ContentEnd).Save((Stream) memoryStream, format);
        dataObj.SetData(format, (object) Encoding.UTF8.GetString(memoryStream.ToArray()));
      }
    }

    private bool CalculateAnnotationsEnabled()
    {
      SceneViewModel activeSceneViewModel = this.designerContext.ActiveSceneViewModel;
      return activeSceneViewModel != null && activeSceneViewModel.RootNode != null && (activeSceneViewModel.IsEditable && !(activeSceneViewModel.ActiveEditingContainer is StyleNode)) && !(activeSceneViewModel.ActiveEditingContainer is FrameworkTemplateElement);
    }

    private void UpdateAnnotationsEnabled()
    {
      bool flag = this.CalculateAnnotationsEnabled();
      if (this.annotationsEnabled == flag)
        return;
      this.annotationsEnabled = flag;
      EventHandler eventHandler = this.AnnotationsEnabledChanged;
      if (eventHandler == null)
        return;
      eventHandler((object) this, EventArgs.Empty);
    }

    [Conditional("Debug")]
    private void DebugValidateAnnotationSelectionSet()
    {
    }

    internal void OnAnnotationAdded(AnnotationSceneNode annotation)
    {
      this.FireAnnotationEvent(annotation, this.AnnotationAdded);
    }

    internal void OnAnnotationDeleted(AnnotationSceneNode annotation)
    {
      this.FireAnnotationEvent(annotation, this.AnnotationDeleted);
    }

    internal void OnAnnotationChanged(AnnotationSceneNode annotation)
    {
      this.FireAnnotationEvent(annotation, this.AnnotationChanged);
    }

    private void FireAnnotationEvent(AnnotationSceneNode annotation, EventHandler<AnnotationEventArgs> e)
    {
      if (e == null)
        return;
      e((object) this, new AnnotationEventArgs(annotation));
    }

    private void SubscribeEvents()
    {
      this.designerContext.AnnotationsOptionsModel.PropertyChanged += new PropertyChangedEventHandler(this.AnnotationsOptions_PropertyChanged);
      this.designerContext.ToolManager.ActiveToolChanged += new ToolEventHandler(this.ToolManager_ActiveToolChanged);
      this.designerContext.SelectionManager.LateActiveSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.SelectionManager_LateActiveSceneUpdatePhase);
    }

    private void UnsubscribeEvents()
    {
      this.designerContext.AnnotationsOptionsModel.PropertyChanged -= new PropertyChangedEventHandler(this.AnnotationsOptions_PropertyChanged);
      this.designerContext.ToolManager.ActiveToolChanged -= new ToolEventHandler(this.ToolManager_ActiveToolChanged);
      this.designerContext.SelectionManager.LateActiveSceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.SelectionManager_LateActiveSceneUpdatePhase);
    }

    private void SelectionManager_LateActiveSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      this.UpdateAnnotationsEnabled();
      this.RefreshToolRelatedSettings();
    }

    private void AnnotationsOptions_PropertyChanged(object sender, PropertyChangedEventArgs args)
    {
      if (!(args.PropertyName == "ShowAnnotations"))
        return;
      EventHandler eventHandler = this.ShowAnnotationsChanged;
      if (eventHandler == null)
        return;
      eventHandler((object) this, EventArgs.Empty);
    }

    private void ToolManager_ActiveToolChanged(object sender, ToolEventArgs e)
    {
      this.RefreshToolRelatedSettings();
    }
  }
}
