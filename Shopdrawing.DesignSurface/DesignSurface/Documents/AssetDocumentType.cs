// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Documents.AssetDocumentType
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Designers;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.Documents
{
  internal abstract class AssetDocumentType : DocumentType
  {
    public const string BuildTask = "Resource";
    private DesignerContext designerContext;

    public override bool IsAsset
    {
      get
      {
        return true;
      }
    }

    internal DesignerContext DesignerContext
    {
      get
      {
        return this.designerContext;
      }
    }

    protected override string DefaultBuildTask
    {
      get
      {
        return "Resource";
      }
    }

    public override PreferredExternalEditCommand PreferredExternalEditCommand
    {
      get
      {
        return PreferredExternalEditCommand.ShellEdit;
      }
    }

    public AssetDocumentType(DesignerContext designerContext)
    {
      this.designerContext = designerContext;
    }

    public override sealed bool CanInsertTo(IProjectItem projectItem, IView view)
    {
      SceneView sceneView = view as SceneView;
      if (sceneView == null)
        return false;
      return this.CanInsertTo(projectItem, view, sceneView.ViewModel.ActiveSceneInsertionPoint);
    }

    public virtual bool CanInsertTo(IProjectItem projectItem, IView view, ISceneInsertionPoint insertionPoint)
    {
      if (projectItem.Project == null)
        return false;
      SceneView sceneView = view as SceneView;
      if (sceneView == null || !sceneView.IsDesignSurfaceEnabled || insertionPoint == null)
        return false;
      ViewState viewState = ViewState.ElementValid | ViewState.AncestorValid;
      if ((sceneView.GetViewState(insertionPoint.SceneNode) & viewState) != viewState)
        return false;
      ProjectContext projectContext = (ProjectContext) ProjectXamlContext.GetProjectContext(projectItem.Project);
      if (projectContext == null)
        return false;
      IProject project = ProjectHelper.GetProject(this.designerContext.ProjectManager, sceneView.Document.DocumentContext);
      return project != null && (projectContext == sceneView.Document.ProjectContext || ProjectHelper.DoesProjectReferencesContainTarget(project, (IProjectContext) projectContext)) && ((!this.IsContentOrNoneBuildItem(projectItem) || projectItem.Project == project) && this.CanAddToProject(project));
    }

    private bool IsContentOrNoneBuildItem(IProjectItem projectItem)
    {
      string str = projectItem.Properties["BuildAction"];
      if (!(str == "Content"))
        return str == "None";
      return true;
    }

    protected abstract SceneElement CreateElement(SceneViewModel viewModel, ISceneInsertionPoint insertionPoint, string resourceReference);

    public override sealed bool AddToDocument(IProjectItem projectItem, IView view)
    {
      SceneView sceneView = view as SceneView;
      if (sceneView == null)
        return false;
      return this.AddToDocument(projectItem, view, sceneView.ViewModel.ActiveSceneInsertionPoint, Rect.Empty);
    }

    public virtual bool AddToDocument(IProjectItem projectItem, IView view, ISceneInsertionPoint insertionPoint, Rect rect)
    {
      SceneView sceneView = view as SceneView;
      if (sceneView != null && sceneView.IsDesignSurfaceEnabled && insertionPoint != null)
      {
        SceneViewModel viewModel = sceneView.ViewModel;
        string str = insertionPoint.SceneNode.DocumentNode.Context.MakeResourceReference(projectItem.DocumentReference.Path);
        if (!string.IsNullOrEmpty(str))
        {
          SceneElement element = this.CreateElement(viewModel, insertionPoint, str);
          if (element != null)
          {
            try
            {
              using (SceneEditTransaction editTransaction = viewModel.CreateEditTransaction(StringTable.AssetAddAssetUndo))
              {
                IList<SceneElement> list = this.AddToDocumentInternal(str, element, insertionPoint, viewModel, editTransaction);
                editTransaction.Update();
                if (sceneView.IsValid && list.Count > 0)
                {
                  element = list[0];
                  if (element != null && element.IsViewObjectValid && (this.designerContext.ActiveView == sceneView && sceneView.ViewRoot != null) && element.Visual != null)
                    sceneView.EnsureVisible(element.Visual);
                  BaseFrameworkElement child = element as BaseFrameworkElement;
                  if (child != null)
                  {
                    SetRectMode setRectMode = SetRectMode.CreateAtPosition;
                    if (rect.IsEmpty)
                    {
                      rect = new Rect(0.0, 0.0, double.PositiveInfinity, double.PositiveInfinity);
                      setRectMode = SetRectMode.CreateDefault;
                    }
                    if (double.IsPositiveInfinity(rect.Width))
                      rect.Width = this.GetNativeWidth((SceneElement) child);
                    if (double.IsPositiveInfinity(rect.Height))
                      rect.Height = this.GetNativeHeight((SceneElement) child);
                    using (viewModel.ForceBaseValue())
                    {
                      ILayoutDesigner designerForChild = viewModel.GetLayoutDesignerForChild((SceneElement) child, true);
                      LayoutOverrides layoutOverrides = LayoutOverrides.HorizontalAlignment | LayoutOverrides.VerticalAlignment;
                      designerForChild.SetChildRect(child, rect, LayoutOverrides.None, layoutOverrides, layoutOverrides, setRectMode);
                    }
                  }
                  viewModel.ElementSelectionSet.SetSelection((ICollection<SceneElement>) list, (SceneElement) null);
                }
                editTransaction.Commit();
              }
              return true;
            }
            catch (InvalidOperationException ex)
            {
              this.DesignerContext.MessageDisplayService.ShowError(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.AssetElementInsertionFailedDialogMessage, new object[2]
              {
                (object) element.Name,
                (object) ex.Message
              }));
            }
          }
        }
      }
      return false;
    }

    protected virtual double GetNativeWidth(SceneElement element)
    {
      double val2 = double.PositiveInfinity;
      Base2DElement context = element.Parent as Base2DElement;
      if (context != null && context.IsViewObjectValid)
        val2 = context.GetComputedBounds(context).Width;
      return Math.Min(100.0, val2);
    }

    protected virtual double GetNativeHeight(SceneElement element)
    {
      double val2 = double.PositiveInfinity;
      Base2DElement context = element.Parent as Base2DElement;
      if (context != null && context.IsViewObjectValid)
        val2 = context.GetComputedBounds(context).Height;
      return Math.Min(100.0, val2);
    }

    public virtual Size GetSourcePixelSize(IProjectItem projectItem)
    {
      return Size.Empty;
    }

    protected virtual IList<SceneElement> AddToDocumentInternal(string importedFilePath, SceneElement element, ISceneInsertionPoint insertionPoint, SceneViewModel sceneViewModel, SceneEditTransaction undoTransaction)
    {
      List<SceneElement> list = new List<SceneElement>();
      SceneElement sceneElement = insertionPoint.SceneElement;
      Viewport3DElement viewport3Delement = sceneElement as Viewport3DElement;
      if (viewport3Delement != null || sceneElement is Base3DElement)
      {
        if (element.TargetType == typeof (Image))
        {
          string[] strArray = importedFilePath.Split('.');
          string newName = strArray.Length <= 1 || strArray[0] == null ? "Image" : strArray[0];
          element = (SceneElement) GeometryCreationHelper3D.ConvertImageTo3D(sceneViewModel, (BaseFrameworkElement) element, newName);
        }
        IChildContainer3D childContainer3D = sceneElement as IChildContainer3D;
        if (ModelVisual3DElement.ContentProperty.Equals((object) insertionPoint.Property))
        {
          element = (SceneElement) BaseElement3DCoercionHelper.CoerceToModel3D(sceneViewModel, element);
          if (element != null)
          {
            list.Add(element);
            insertionPoint.Insert((SceneNode) element);
          }
        }
        else if (childContainer3D != null)
        {
          Base3DElement child = (Base3DElement) element;
          list.Add(childContainer3D.AddChild(sceneViewModel, child));
          if (viewport3Delement != null && viewport3Delement.Camera == null)
          {
            ModelVisual3DElement modelVisual3Delement = element as ModelVisual3DElement;
            Camera camera;
            if (modelVisual3Delement != null)
            {
              Rect computedTightBounds = viewport3Delement.GetComputedTightBounds();
              camera = Helper3D.CreateEnclosingPerspectiveCamera(45.0, computedTightBounds.Width / computedTightBounds.Height, modelVisual3Delement.DesignTimeBounds, 0.8);
            }
            else
              camera = (Camera) new PerspectiveCamera(new Point3D(0.0, 0.0, -5.0), new Vector3D(0.0, 0.0, 1.0), new Vector3D(0.0, 1.0, 0.0), 45.0);
            viewport3Delement.Camera = (CameraElement) sceneViewModel.CreateSceneNode((object) camera);
          }
        }
      }
      else
      {
        ModelVisual3DElement modelVisual3D = element as ModelVisual3DElement;
        if (modelVisual3D != null)
        {
          Viewport3DElement forModelVisual3D = GeometryCreationHelper3D.GetEnclosingViewportForModelVisual3D(sceneViewModel, modelVisual3D);
          list.Add((SceneElement) forModelVisual3D);
          insertionPoint.Insert((SceneNode) forModelVisual3D);
        }
        else if (insertionPoint.CanInsert((ITypeId) element.Type))
        {
          list.Add(element);
          insertionPoint.Insert((SceneNode) element);
        }
      }
      return (IList<SceneElement>) list;
    }

    protected abstract bool DoesNodeReferenceUrl(DocumentNode node, string url);

    protected abstract void RefreshInstance(SceneElement element, SceneDocument sceneDocument, string url);

    public override void RefreshAllInstances(DocumentReference documentReference, IDocument document)
    {
      SceneDocument sceneDocument = document as SceneDocument;
      if (sceneDocument == null)
        return;
      SceneViewModel sceneViewModel = this.GetSceneViewModel(sceneDocument);
      if (sceneViewModel == null)
        return;
      bool flag = false;
      SceneElement rootElement = sceneViewModel.RootNode as SceneElement;
      if (rootElement == null)
        return;
      using (SceneEditTransaction editTransaction = sceneDocument.CreateEditTransaction(StringTable.AssetFileUpdatedUndo, true))
      {
        foreach (SceneElement element in SceneElementHelper.GetLogicalTree(rootElement))
        {
          if (this.DoesNodeReferenceUrl(element.DocumentNode, documentReference.Path))
          {
            this.RefreshInstance(element, sceneDocument, documentReference.Path);
            flag = true;
          }
        }
        if (flag)
          editTransaction.Commit();
        else
          editTransaction.Cancel();
      }
    }

    protected SceneViewModel GetSceneViewModel(SceneDocument sceneDocument)
    {
      SceneViewModel sceneViewModel = (SceneViewModel) null;
      foreach (IView view in (IEnumerable<IView>) this.designerContext.ViewService.Views)
      {
        SceneView sceneView = view as SceneView;
        if (sceneView != null && sceneView.Document.DocumentRoot == sceneDocument.DocumentRoot)
        {
          sceneViewModel = sceneView.ViewModel;
          break;
        }
      }
      return sceneViewModel;
    }

    protected override void LoadImageIcon(string path)
    {
      this.CachedImage = Microsoft.Expression.DesignSurface.FileTable.GetImageSource(path);
    }
  }
}
