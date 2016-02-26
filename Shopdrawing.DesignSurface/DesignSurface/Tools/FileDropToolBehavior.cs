// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.FileDropToolBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.UserInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal class FileDropToolBehavior : DragDropInsertBehavior
  {
    private FileDropUtility dropUtility;

    private ISceneInsertionPoint DragDropInsertionPoint
    {
      get
      {
        if (this.PreviewInsertionPoint != null)
          return this.PreviewInsertionPoint;
        if (this.ActiveSceneViewModel != null)
          return this.ActiveSceneViewModel.ActiveSceneInsertionPoint;
        return this.ActiveSceneInsertionPoint;
      }
    }

    public FileDropToolBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
      this.dropUtility = FileDropToolBehavior.CreateImageOrMediaDrop(this.ActiveSceneViewModel.DesignerContext);
    }

    internal static FileDropUtility CreateImageOrMediaDrop(DesignerContext context)
    {
      List<IDocumentType> list = new List<IDocumentType>();
      list.Add(context.DocumentTypeManager.DocumentTypes[DocumentTypeNamesHelper.Image]);
      list.Add(context.DocumentTypeManager.DocumentTypes[DocumentTypeNamesHelper.SilverlightAndWpfMedia]);
      if (context.ActiveView.Document.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf))
        list.Add(context.DocumentTypeManager.DocumentTypes[DocumentTypeNamesHelper.WpfMedia]);
      return new FileDropUtility(context.ProjectManager, (FrameworkElement) null, list.ToArray());
    }

    protected override bool OnDragLeave(DragEventArgs args)
    {
      if (!this.IsSuspended)
        this.PopSelf();
      return base.OnDragLeave(args);
    }

    protected override bool OnDragOver(DragEventArgs args)
    {
      base.OnDragOver(args);
      bool flag = true;
      SafeDataObject safeDataObject = new SafeDataObject(args.Data);
      if (safeDataObject.GetDataPresent(DataFormats.FileDrop))
      {
        if (this.dropUtility.GetSupportedFiles(args.Data).Length > 0)
          flag = false;
      }
      else if (safeDataObject.GetDataPresent("BlendProjectItem"))
      {
        foreach (IProjectItem projectItem in ((ItemSelectionSet) safeDataObject.GetData("BlendProjectItem")).Selection)
        {
          if (projectItem.DocumentType.CanInsertTo(projectItem, (IView) this.ActiveView))
          {
            flag = false;
            break;
          }
        }
      }
      if (flag)
        args.Effects = DragDropEffects.None;
      args.Handled = true;
      return true;
    }

    protected override bool OnButtonDown(Point pointerPosition)
    {
      if (!this.IsSuspended)
        this.PopSelf();
      return false;
    }

    protected override bool OnDrop(DragEventArgs args)
    {
      ISceneInsertionPoint dropInsertionPoint = this.DragDropInsertionPoint;
      Point position = args.GetPosition((IInputElement) this.ActiveView.ViewRootContainer);
      this.ToolBehaviorContext.SnappingEngine.Start(this.ToolBehaviorContext, (BaseFrameworkElement) null, (IList<BaseFrameworkElement>) null);
      Point point = this.ToolBehaviorContext.SnappingEngine.SnapPoint(position, EdgeFlags.All);
      Point dropPoint = this.ActiveView.TransformPointFromRoot(dropInsertionPoint.SceneElement, point);
      this.ToolBehaviorContext.SnappingEngine.Stop();
      SafeDataObject safeDataObject = new SafeDataObject(args.Data);
      using (TemporaryCursor.SetWaitCursor())
      {
        if (safeDataObject.GetDataPresent(DataFormats.FileDrop))
          FileDropToolBehavior.AddItemsToDocument(this.ActiveView, this.ActiveSceneViewModel.DesignerContext.ActiveProject.AddItems(Enumerable.Select<string, DocumentCreationInfo>((IEnumerable<string>) this.dropUtility.GetSupportedFiles(args.Data), (Func<string, DocumentCreationInfo>) (file => new DocumentCreationInfo()
          {
            SourcePath = file
          }))), dropPoint, dropInsertionPoint);
        else if (safeDataObject.GetDataPresent("BlendProjectItem"))
        {
          ItemSelectionSet itemSelectionSet = (ItemSelectionSet) safeDataObject.GetData("BlendProjectItem");
          using (SceneEditTransaction editTransaction = this.ActiveSceneViewModel.CreateEditTransaction(StringTable.DropProjectItemIntoSceneUndo))
          {
            foreach (IProjectItem projectItem in itemSelectionSet.Selection)
              FileDropToolBehavior.AddToDocument(this.ActiveView, projectItem, dropPoint, dropInsertionPoint);
            editTransaction.Commit();
          }
        }
      }
      if (!this.IsSuspended)
        this.PopSelf();
      return true;
    }

    internal static bool CanInsertTo(SceneView view, IProjectItem projectItem, ISceneInsertionPoint insertionPoint)
    {
      AssetDocumentType assetDocumentType = projectItem.DocumentType as AssetDocumentType;
      if (assetDocumentType != null)
        return assetDocumentType.CanInsertTo(projectItem, (IView) view, insertionPoint);
      return projectItem.DocumentType.CanInsertTo(projectItem, (IView) view);
    }

    internal static bool AddToDocument(SceneView view, IProjectItem projectItem, Point dropPoint, ISceneInsertionPoint insertionPoint)
    {
      AssetDocumentType assetDocumentType = projectItem.DocumentType as AssetDocumentType;
      if (assetDocumentType != null)
        return assetDocumentType.AddToDocument(projectItem, (IView) view, insertionPoint, new Rect(dropPoint, new Vector(double.PositiveInfinity, double.PositiveInfinity)));
      return projectItem.DocumentType.AddToDocument(projectItem, (IView) view);
    }

    internal static void AddItemsToDocument(SceneView view, IEnumerable<IProjectItem> importedItems, Point dropPoint, ISceneInsertionPoint insertionPoint)
    {
      bool flag = false;
      if (importedItems == null || !Enumerable.Any<IProjectItem>(importedItems))
        return;
      using (SceneEditTransaction editTransaction = view.ViewModel.CreateEditTransaction(StringTable.DropElementsIntoSceneUndo))
      {
        foreach (IProjectItem projectItem in importedItems)
        {
          if (FileDropToolBehavior.CanInsertTo(view, projectItem, insertionPoint))
            FileDropToolBehavior.AddToDocument(view, projectItem, dropPoint, insertionPoint);
          else
            flag = true;
        }
        editTransaction.Commit();
      }
      if (!flag)
        return;
      MessageBoxArgs args = new MessageBoxArgs()
      {
        Message = StringTable.DragInsertionContainedIllegalItemsMessage,
        Button = MessageBoxButton.OK,
        Image = MessageBoxImage.Exclamation
      };
      int num = (int) view.ViewModel.DesignerContext.MessageDisplayService.ShowMessage(args);
    }
  }
}
