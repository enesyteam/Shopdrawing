// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.ConvertToElementsCommand
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
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  public class ConvertToElementsCommand : ICommand
  {
    private DesignerContext designerContext;
    private ResourceManager resourceManager;
    private bool isEnabled;
    private bool isVisible;
    private Point point;

    public bool IsVisible
    {
      get
      {
        return this.isVisible;
      }
    }

    public bool IsEnabled
    {
      get
      {
        return this.isEnabled;
      }
      private set
      {
        if (this.isEnabled == value)
          return;
        this.isEnabled = value;
        if (this.CanExecuteChanged == null)
          return;
        this.CanExecuteChanged((object) this, EventArgs.Empty);
      }
    }

    public string ResourceType
    {
      get
      {
        if (this.IsVisible && this.resourceManager.SelectedItems.Count == 1)
          return ((ResourceEntryItem) this.resourceManager.SelectedItems.Selection[0]).Resource.ValueNode.TargetType.Name;
        return (string) null;
      }
    }

    public string CommandName
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ConvertDrawingBrushToElementsCommandName, new object[1]
        {
          (object) this.ResourceType
        });
      }
    }

    public event EventHandler CanExecuteChanged;

    internal ConvertToElementsCommand(DesignerContext designerContext, ResourceManager resourceManager, Point point)
    {
      this.designerContext = designerContext;
      this.resourceManager = resourceManager;
      this.point = point;
      this.UpdateIsEnabled();
    }

    public bool CanExecute(object parameter)
    {
      return this.IsEnabled;
    }

    public void Execute(object parameter)
    {
      this.Execute();
    }

    public void Execute()
    {
      if (!this.IsEnabled)
        return;
      ResourceEntryItem resourceEntryItem = (ResourceEntryItem) this.resourceManager.SelectedItems.Selection[0];
      SceneNode sceneNode = resourceEntryItem.Container.ViewModel.GetSceneNode(resourceEntryItem.Resource.ValueNode);
      ConvertibleDrawing convertibleDrawing = ConvertibleDrawing.CreateConvertibleDrawing((object) (Brush) this.designerContext.ActiveSceneViewModel.CreateInstance(sceneNode.DocumentNodePath), sceneNode.ProjectContext.ProjectPath);
      if (convertibleDrawing == null)
        return;
      DocumentCompositeNode documentCompositeNode = (DocumentCompositeNode) sceneNode.DocumentNode;
      string valueAsString = DocumentPrimitiveNode.GetValueAsString(resourceEntryItem.Resource.KeyNode);
      string documentUrl = documentCompositeNode.DocumentRoot.DocumentContext.DocumentUrl;
      SceneViewModel activeSceneViewModel = this.designerContext.ActiveSceneViewModel;
      using (SceneEditTransaction editTransaction = activeSceneViewModel.CreateEditTransaction(this.CommandName))
      {
        BaseFrameworkElement child = (BaseFrameworkElement) activeSceneViewModel.CreateSceneNode((object) convertibleDrawing.Convert());
        activeSceneViewModel.ActiveSceneInsertionPoint.Insert((SceneNode) child);
        child.SetLocalValue(DesignTimeProperties.BrushDocumentReferenceProperty, (object) documentUrl);
        child.SetLocalValue(DesignTimeProperties.BrushKeyProperty, (object) valueAsString);
        child.Name = valueAsString + "_1";
        this.point *= ElementUtilities.GetInverseMatrix(activeSceneViewModel.DefaultView.GetComputedTransformToRoot(activeSceneViewModel.ActiveSceneInsertionPoint.SceneElement));
        Rect rect = new Rect(this.point, new Size(child.Width, child.Height));
        activeSceneViewModel.GetLayoutDesignerForChild((SceneElement) child, true).SetChildRect(child, rect, LayoutOverrides.None, LayoutOverrides.Width | LayoutOverrides.Height, LayoutOverrides.None);
        this.designerContext.SelectionManager.ElementSelectionSet.SetSelection((SceneElement) child);
        editTransaction.Commit();
      }
    }

    public void UpdateIsEnabled()
    {
      bool flag = false;
      if (this.designerContext.ActiveSceneViewModel != null && this.designerContext.ActiveSceneViewModel.ActiveSceneInsertionPoint != null && (this.resourceManager.SelectedItems.Count == 1 && this.designerContext.ActiveSceneViewModel.ActiveSceneInsertionPoint != null))
      {
        ResourceEntryItem resourceEntryItem = this.resourceManager.SelectedItems.Selection[0] as ResourceEntryItem;
        if (resourceEntryItem != null)
        {
          DocumentNode valueNode = resourceEntryItem.Resource.ValueNode;
          this.isVisible = ConvertibleDrawing.IsResourceTypeSupported(valueNode.Type);
          flag = ConvertibleDrawing.CanCreateConvertibleDrawing(valueNode);
        }
      }
      else
      {
        flag = false;
        this.isVisible = false;
      }
      this.IsEnabled = flag;
    }
  }
}
