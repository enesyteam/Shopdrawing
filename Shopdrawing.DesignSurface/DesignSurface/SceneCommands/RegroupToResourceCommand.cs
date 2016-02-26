// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.RegroupToResourceCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal abstract class RegroupToResourceCommand : SceneCommandBase
  {
    protected DesignerContext designerContext;

    public override bool IsAvailable
    {
      get
      {
        if (this.designerContext.ActiveDocument.ProjectContext.PlatformMetadata.ResolveType(PlatformTypes.DrawingBrush) != null)
          return base.IsAvailable;
        return false;
      }
    }

    public override bool IsEnabled
    {
      get
      {
        if (base.IsEnabled)
          return this.FindClosestValidCanvas(this.designerContext.SelectionManager.ElementSelectionSet.Selection) != null;
        return false;
      }
    }

    protected abstract string UndoUnitString { get; }

    internal RegroupToResourceCommand(SceneViewModel sceneView)
      : base(sceneView)
    {
      this.designerContext = sceneView.DesignerContext;
    }

    public override void Execute()
    {
      SceneElement closestValidCanvas = this.FindClosestValidCanvas(this.designerContext.SelectionManager.ElementSelectionSet.Selection);
      if (closestValidCanvas == null)
        return;
      RegroupToResourceCommand.ConvertibleBrushReference brushReference = new RegroupToResourceCommand.ConvertibleBrushReference(closestValidCanvas);
      ResourceContainer resourceContainer = this.designerContext.ResourceManager.FindResourceContainer(brushReference.DocumentUrl);
      using (SceneEditTransaction editTransaction1 = this.designerContext.ActiveSceneViewModel.CreateEditTransaction(this.UndoUnitString))
      {
        using (SceneEditTransaction editTransaction2 = resourceContainer.ViewModel.CreateEditTransaction(this.UndoUnitString))
        {
          DocumentCompositeNode resourceNode = this.GetResourceNode(brushReference);
          DocumentNode documentNode = resourceNode.Properties[DictionaryEntryNode.ValueProperty];
          this.designerContext.SelectionManager.ElementSelectionSet.SetSelection(closestValidCanvas);
          if (!typeof (DrawingBrush).IsAssignableFrom(documentNode.TargetType))
            throw new NotSupportedException("Regroup to resource not enabled on non-DrawingBrush types.");
          new RegroupToResourceCommand.ReplaceDrawingBrushCommand(this.designerContext.ActiveSceneViewModel, resourceNode).Execute();
          this.PostProcess(closestValidCanvas);
          editTransaction2.Commit();
          editTransaction1.Commit();
        }
      }
    }

    public static bool IsTypeSupported(Type type)
    {
      return typeof (DrawingBrush).IsAssignableFrom(type);
    }

    private bool CanUseBrushReference(RegroupToResourceCommand.ConvertibleBrushReference brushReference)
    {
      if (!brushReference.HasReferenceInfo)
        return false;
      ResourceEntryItem resourceEntryItem = this.FindResourceItem(brushReference) as ResourceEntryItem;
      if (resourceEntryItem != null)
        return RegroupToResourceCommand.IsTypeSupported(resourceEntryItem.Resource.ValueNode.TargetType);
      return this.designerContext.ResourceManager.FindResourceContainer(brushReference.DocumentUrl) != null;
    }

    private ResourceItem FindResourceItem(RegroupToResourceCommand.ConvertibleBrushReference brushReference)
    {
      ResourceContainer resourceContainer = this.designerContext.ResourceManager.FindResourceContainer(brushReference.DocumentUrl);
      if (resourceContainer != null)
      {
        foreach (ResourceItem resourceItem in (Collection<ResourceItem>) resourceContainer.ResourceItems)
        {
          ResourceEntryItem resourceEntryItem = resourceItem as ResourceEntryItem;
          if (resourceEntryItem != null && DocumentPrimitiveNode.GetValueAsString(resourceEntryItem.Resource.KeyNode) == brushReference.Key)
            return (ResourceItem) resourceEntryItem;
        }
      }
      return (ResourceItem) null;
    }

    private DocumentCompositeNode GetResourceNode(RegroupToResourceCommand.ConvertibleBrushReference brushReference)
    {
      ResourceItem resourceItem = this.FindResourceItem(brushReference);
      if (resourceItem != null)
        return (DocumentCompositeNode) resourceItem.DocumentNode;
      ResourceContainer resourceContainer = this.designerContext.ResourceManager.FindResourceContainer(brushReference.DocumentUrl);
      resourceContainer.EnsureEditable();
      CreateResourceModel createResourceModel = new CreateResourceModel(resourceContainer.ViewModel, resourceContainer.ViewModel.DesignerContext.ResourceManager, typeof (DrawingBrush), (Type) null, (string) null, (SceneElement) null, (SceneNode) null, CreateResourceModel.ContextFlags.None);
      createResourceModel.SelectedLocation = (object) resourceContainer.ViewModel.Document;
      createResourceModel.KeyString = brushReference.Key;
      DocumentCompositeNode documentCompositeNode = (DocumentCompositeNode) null;
      createResourceModel.SelectedExternalResourceDictionaryFile = resourceContainer;
      using (SceneEditTransaction editTransaction = resourceContainer.ViewModel.CreateEditTransaction(StringTable.CreateResourceDialogTitle))
      {
        documentCompositeNode = resourceContainer.ViewModel.Document.DocumentContext.CreateNode(typeof (DrawingBrush));
        documentCompositeNode = createResourceModel.CreateResource((DocumentNode) documentCompositeNode, (IPropertyId) null, -1);
        editTransaction.Commit();
      }
      return documentCompositeNode;
    }

    protected virtual void PostProcess(SceneElement sceneElement)
    {
    }

    private SceneElement FindClosestValidCanvas(ReadOnlyCollection<SceneElement> selection)
    {
      if (selection.Count == 0)
        return (SceneElement) null;
      SceneNode sceneNode = (SceneNode) selection[0];
      for (int index = 1; index < selection.Count; ++index)
        sceneNode = sceneNode.GetCommonAncestor((SceneNode) selection[index]);
      for (; sceneNode != null; sceneNode = sceneNode.Parent)
      {
        if (typeof (Canvas).IsAssignableFrom(sceneNode.TargetType))
        {
          SceneElement sceneElement = (SceneElement) sceneNode;
          if (this.CanUseBrushReference(new RegroupToResourceCommand.ConvertibleBrushReference(sceneElement)))
            return sceneElement;
        }
      }
      return (SceneElement) null;
    }

    private class ConvertibleBrushReference
    {
      public string DocumentUrl { get; private set; }

      public string Key { get; private set; }

      public bool HasReferenceInfo
      {
        get
        {
          if (this.Key != null)
            return this.DocumentUrl != null;
          return false;
        }
      }

      private ConvertibleBrushReference(string key, string documentUrl)
      {
        this.Key = key;
        this.DocumentUrl = documentUrl;
      }

      public ConvertibleBrushReference(SceneElement sceneElement)
        : this(sceneElement.GetLocalOrDefaultValue(DesignTimeProperties.BrushKeyProperty) as string, sceneElement.GetLocalOrDefaultValue(DesignTimeProperties.BrushDocumentReferenceProperty) as string)
      {
      }
    }

    private sealed class ReplaceDrawingBrushCommand : MakeDrawingBrushCommand
    {
      private DocumentCompositeNode resourceNode;

      public ReplaceDrawingBrushCommand(SceneViewModel viewModel, DocumentCompositeNode resourceNode)
        : base(viewModel)
      {
        this.ShowUI = false;
        this.resourceNode = resourceNode;
      }

      protected override DocumentNode ProcessAsResource(BaseFrameworkElement originalElement, DocumentNode newValue)
      {
        this.resourceNode.Properties[DictionaryEntryNode.ValueProperty] = newValue;
        return (DocumentNode) this.resourceNode;
      }
    }
  }
}
