// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.MakeUserControlCommandBase
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Designers;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Tools.Layout;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.Templates;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal abstract class MakeUserControlCommandBase : SceneCommandBase
  {
    public bool ShowUI { get; set; }

    public override bool IsAvailable
    {
      get
      {
        return JoltHelper.TypeSupported((ITypeResolver) this.SceneViewModel.ProjectContext, PlatformTypes.Control);
      }
    }

    public virtual bool AddToApplicationFlow
    {
      get
      {
        return false;
      }
    }

    public override bool IsEnabled
    {
      get
      {
        if (!base.IsEnabled || this.SceneViewModel.ElementSelectionSet.Count <= 0)
          return false;
        ReadOnlyCollection<SceneElement> selection = this.SceneViewModel.ElementSelectionSet.Selection;
        bool flag = false;
        SceneNode sceneNode = (SceneNode) null;
        SceneElement sceneElement1 = this.SceneViewModel.RootNode as SceneElement;
        if (sceneElement1 != null && selection.Contains(sceneElement1))
          return false;
        foreach (SceneElement sceneElement2 in selection)
        {
          if (!(sceneElement2 is BaseFrameworkElement) || !sceneElement2.IsViewObjectValid)
            return false;
          if (!flag)
          {
            sceneNode = sceneElement2.Parent;
            flag = true;
          }
          else if (sceneElement2.Parent != sceneNode)
            return false;
        }
        return true;
      }
    }

    protected bool IsPrototypingEnabled
    {
      get
      {
        if (this.SceneViewModel.ProjectContext.IsCapabilitySet(PlatformCapability.SupportPrototyping))
          return this.DesignerContext.PrototypingService != null;
        return false;
      }
    }

    protected abstract string DialogTitle { get; }

    protected MakeUserControlCommandBase(SceneViewModel viewModel)
      : base(viewModel)
    {
      this.ShowUI = true;
    }

    public override void Execute()
    {
      string fileName = "test";
      IProject activeProject = this.DesignerContext.ActiveProject;
      TemplateItemHelper templateItemHelper = new TemplateItemHelper(activeProject, (IList<string>) null, (IServiceProvider) this.DesignerContext.Services);
      IProjectItemTemplate templateItem = templateItemHelper.FindTemplateItem("UserControl");
      if (templateItem == null)
      {
        this.DesignerContext.MessageDisplayService.ShowError(StringTable.MakeUserControlTemplateNotFound);
      }
      else
      {
        SceneViewModel activeSceneViewModel = this.DesignerContext.ActiveSceneViewModel;
        List<SceneElement> elements = new List<SceneElement>();
        elements.AddRange((IEnumerable<SceneElement>) activeSceneViewModel.ElementSelectionSet.Selection);
        elements.Sort((IComparer<SceneElement>) new ZOrderComparer<SceneElement>(activeSceneViewModel.RootNode));
        if (this.ShowUI)
        {
          string recommendedName = this.GetRecommendedName((IEnumerable<SceneElement>) elements);
          MakeUserControlDialog userControlDialog = new MakeUserControlDialog(this.DesignerContext, this.DialogTitle, templateItemHelper, recommendedName);
          bool? nullable = userControlDialog.ShowDialog();
          if ((!nullable.GetValueOrDefault() ? 1 : (!nullable.HasValue ? true : false)) != 0)
            return;
          fileName = userControlDialog.ControlName;
        }
        List<IProjectItem> itemsToOpen = (List<IProjectItem>) null;
        IProjectItem projectItem1 = (IProjectItem) null;
        IEnumerable<IProjectItem> source = (IEnumerable<IProjectItem>) null;
        try
        {
          source = templateItemHelper.AddProjectItemsForTemplateItem(templateItem, fileName, this.DesignerContext.ProjectManager.TargetFolderForProject(activeProject), CreationOptions.DoNotAllowOverwrites | CreationOptions.DoNotSelectCreatedItems | CreationOptions.DoNotSetDefaultImportPath, out itemsToOpen);
        }
        catch (Exception ex)
        {
          if (ex is NotSupportedException || ErrorHandling.ShouldHandleExceptions(ex))
            this.DesignerContext.MessageDisplayService.ShowError(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ProjectNewFileErrorDialogMessage, new object[2]
            {
              (object) fileName,
              (object) ex.Message
            }));
          else
            throw;
        }
        if (source == null || EnumerableExtensions.CountIsLessThan<IProjectItem>(source, 1))
          return;
        if (itemsToOpen != null && itemsToOpen.Count > 0)
        {
          projectItem1 = Enumerable.FirstOrDefault<IProjectItem>((IEnumerable<IProjectItem>) itemsToOpen);
          projectItem1.OpenDocument(false, true);
        }
        if (projectItem1 != null && projectItem1.IsOpen && projectItem1.DocumentType.CanView)
        {
          Rect empty = Rect.Empty;
          for (int index = 0; index < elements.Count; ++index)
          {
            BaseFrameworkElement child = elements[index] as BaseFrameworkElement;
            if (child != null)
            {
              Rect childRect = child.ViewModel.GetLayoutDesignerForChild((SceneElement) child, true).GetChildRect(child);
              empty.Union(childRect);
            }
          }
          Rect rect = RoundingHelper.RoundRect(empty);
          SceneElement parentElement = elements[0].ParentElement;
          bool useLayoutRounding = LayoutRoundingHelper.GetUseLayoutRounding(parentElement);
          DataObject dataObject = (DataObject) null;
          using (activeSceneViewModel.ForceBaseValue())
          {
            PastePackage pastePackage = new PastePackage(activeSceneViewModel);
            pastePackage.CopyStoryboardsReferencingElements = true;
            pastePackage.AddElements(elements);
            dataObject = pastePackage.GetPasteDataObject();
          }
          SceneView sceneView = projectItem1.OpenView(true) as SceneView;
          if (sceneView != null)
          {
            SceneViewModel viewModel = sceneView.ViewModel;
            ProjectXamlContext projectXamlContext = ProjectXamlContext.FromProjectContext(viewModel.ViewRoot.ProjectContext);
            ClassAttributes rootClassAttributes = viewModel.DocumentRoot.RootClassAttributes;
            ITypeId typeId = (ITypeId) null;
            if (projectXamlContext != null && rootClassAttributes != null)
            {
              projectXamlContext.RefreshUnbuiltTypeDescriptions();
              if (rootClassAttributes != null)
                typeId = (ITypeId) projectXamlContext.GetType(projectXamlContext.ProjectAssembly.Name, rootClassAttributes.QualifiedClassName);
            }
            if (typeId != null && this.CheckForCircularReference((IEnumerable<SceneElement>) elements, typeId))
            {
              this.DesignerContext.MessageDisplayService.ShowError(StringTable.MakeUserControlCircularReferenceFound);
              this.CleanupAfterCancel(projectItem1);
              return;
            }
            using (SceneEditTransaction editTransaction = viewModel.CreateEditTransaction(StringTable.UndoUnitMakeUserControl))
            {
              if (!rect.IsEmpty)
              {
                viewModel.RootNode.SetValue(DesignTimeProperties.DesignWidthProperty, (object) rect.Width);
                viewModel.RootNode.SetValue(DesignTimeProperties.DesignHeightProperty, (object) rect.Height);
                if (this.AddToApplicationFlow)
                {
                  viewModel.RootNode.SetValue(BaseFrameworkElement.WidthProperty, (object) rect.Width);
                  viewModel.RootNode.SetValue(BaseFrameworkElement.HeightProperty, (object) rect.Height);
                }
              }
              IProperty property = LayoutRoundingHelper.ResolveUseLayoutRoundingProperty(viewModel.RootNode);
              if (property != null)
                viewModel.RootNode.SetValue((IPropertyId) property, (object) (bool) (useLayoutRounding ? true : false));
              ILayoutDesigner designerForParent = viewModel.GetLayoutDesignerForParent(viewModel.ActiveSceneInsertionPoint.SceneElement, true);
              bool canceledPasteOperation;
              ICollection<SceneNode> nodes = PasteCommand.PasteData(viewModel, new SafeDataObject((IDataObject) dataObject), viewModel.ActiveSceneInsertionPoint, out canceledPasteOperation);
              if (canceledPasteOperation)
              {
                editTransaction.Cancel();
                this.CleanupAfterCancel(projectItem1);
                return;
              }
              editTransaction.Update();
              if (nodes.Count > 0)
              {
                viewModel.DefaultView.UpdateLayout();
                viewModel.SelectNodes(nodes);
                if (designerForParent != null)
                {
                  foreach (SceneNode sceneNode in (IEnumerable<SceneNode>) nodes)
                  {
                    BaseFrameworkElement child = sceneNode as BaseFrameworkElement;
                    if (child != null && child.IsViewObjectValid)
                    {
                      Rect childRect = child.ViewModel.GetLayoutDesignerForChild((SceneElement) child, true).GetChildRect(child);
                      childRect.Location = (Point) (childRect.Location - rect.Location);
                      designerForParent.SetChildRect(child, childRect);
                    }
                  }
                }
              }
              editTransaction.Commit();
            }
            if (this.AddToApplicationFlow && this.DesignerContext.PrototypingService != null)
              this.DesignerContext.PrototypingService.PromoteToCompositionScreen(projectItem1);
            if (typeId != null)
            {
              using (activeSceneViewModel.ForceBaseValue())
              {
                using (activeSceneViewModel.DisableDrawIntoState())
                {
                  using (SceneEditTransaction editTransaction = activeSceneViewModel.CreateEditTransaction(StringTable.UndoUnitMakeUserControl))
                  {
                    using (activeSceneViewModel.DisableUpdateChildrenOnAddAndRemove())
                    {
                      SceneElement primarySelection = activeSceneViewModel.ElementSelectionSet.PrimarySelection;
                      IProperty propertyForChild = parentElement.GetPropertyForChild((SceneNode) primarySelection);
                      PropertySceneInsertionPoint sceneInsertionPoint = new PropertySceneInsertionPoint(parentElement, propertyForChild);
                      SceneNode sceneNode = (SceneNode) null;
                      if (sceneInsertionPoint.CanInsert(typeId))
                      {
                        foreach (SceneElement element in elements)
                        {
                          if (element != primarySelection)
                          {
                            activeSceneViewModel.AnimationEditor.DeleteAllAnimationsInSubtree(element);
                            element.Remove();
                          }
                        }
                        ISceneNodeCollection<SceneNode> collectionForProperty = parentElement.GetCollectionForProperty((IPropertyId) propertyForChild);
                        int index = collectionForProperty.IndexOf((SceneNode) primarySelection);
                        activeSceneViewModel.AnimationEditor.DeleteAllAnimationsInSubtree(primarySelection);
                        primarySelection.Remove();
                        sceneNode = activeSceneViewModel.CreateSceneNode(typeId);
                        collectionForProperty.Insert(index, sceneNode);
                        this.DesignerContext.ViewService.ActiveView = (IView) activeSceneViewModel.DefaultView;
                        editTransaction.Update();
                        activeSceneViewModel.DefaultView.UpdateLayout();
                        BaseFrameworkElement child = sceneNode as BaseFrameworkElement;
                        if (child != null && child.IsViewObjectValid)
                          activeSceneViewModel.GetLayoutDesignerForParent(parentElement, true).SetChildRect(child, rect);
                      }
                      if (this.AddToApplicationFlow)
                      {
                        if (sceneNode != null)
                          sceneNode.SetValue(DesignTimeProperties.IsPrototypingCompositionProperty, (object) true);
                      }
                    }
                    editTransaction.Commit();
                  }
                }
              }
              this.DesignerContext.ViewService.ActiveView = (IView) viewModel.DefaultView;
            }
          }
        }
        if (itemsToOpen == null || itemsToOpen.Count <= 1)
          return;
        foreach (IProjectItem projectItem2 in itemsToOpen)
        {
          if (projectItem1 != projectItem2)
            projectItem2.OpenView(true);
        }
      }
    }

    private void CleanupAfterCancel(IProjectItem itemToOpen)
    {
      IProject project = itemToOpen.Project;
      itemToOpen.CloseDocument();
      project.RemoveItems(1 != 0, itemToOpen);
    }

    protected virtual string GetRecommendedName(IEnumerable<SceneElement> elementsToCopy)
    {
      foreach (SceneElement sceneElement in elementsToCopy)
      {
        if (!string.IsNullOrEmpty(sceneElement.Name))
          return sceneElement.Name;
      }
      return string.Empty;
    }

    private bool CheckForCircularReference(IEnumerable<SceneElement> elementsToCopy, ITypeId newTypeId)
    {
      foreach (SceneElement rootElement in elementsToCopy)
      {
        foreach (SceneNode sceneNode in SceneElementHelper.GetLogicalTree(rootElement))
        {
          if (sceneNode.Type == newTypeId)
            return true;
        }
      }
      return false;
    }
  }
}
