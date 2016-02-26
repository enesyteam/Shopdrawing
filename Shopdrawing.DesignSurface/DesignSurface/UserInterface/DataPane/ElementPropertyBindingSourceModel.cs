// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.ElementPropertyBindingSourceModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class ElementPropertyBindingSourceModel : IBindingSourceModel, INotifyPropertyChanged
  {
    private ElementBindingSourceNode root;
    private ElementBindingSourceNode selectedNode;
    private SelectionContext<ElementNode> selectionContext;
    private DataSchemaItemFilter dataSchemaFilter;

    public string DisplayName
    {
      get
      {
        return StringTable.DataBindingDialogElementPropertyDescription;
      }
    }

    public bool IsEnabled
    {
      get
      {
        return BindingSceneNode.IsElementNameSupported(this.root.Element.ViewModel);
      }
    }

    public string AutomationName
    {
      get
      {
        return "ElementSourceBindingSource";
      }
    }

    public ElementBindingSourceNode Root
    {
      get
      {
        return this.root;
      }
    }

    public ElementBindingSourceNode SelectedNode
    {
      get
      {
        return (ElementBindingSourceNode) this.selectionContext.PrimarySelection;
      }
      set
      {
        this.selectionContext.SetSelection((ElementNode) value);
      }
    }

    public string Source
    {
      get
      {
        if (this.SelectedNode != null)
          return this.SelectedNode.Element.Name;
        return string.Empty;
      }
    }

    public string Path
    {
      get
      {
        if (this.SelectedNode != null)
          return this.SelectedNode.Path;
        return string.Empty;
      }
    }

    public string PathDescription
    {
      get
      {
        if (this.SelectedNode != null)
          return this.SelectedNode.Schema.PathDescription;
        return StringTable.UseCustomPropertyPathDescription;
      }
    }

    public ISchema Schema
    {
      get
      {
        if (this.SelectedNode != null)
          return this.SelectedNode.Schema;
        return (ISchema) null;
      }
    }

    public SchemaItem SchemaItem
    {
      get
      {
        if (this.SelectedNode != null)
          return this.SelectedNode.SchemaItem;
        return (SchemaItem) null;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public ElementPropertyBindingSourceModel(SceneNode targetElement, DataSchemaItemFilter dataSchemaFilter)
    {
      this.selectionContext = (SelectionContext<ElementNode>) new SingleSelectionContext<ElementNode>();
      this.selectionContext.PropertyChanged += new PropertyChangedEventHandler(this.selectionContext_PropertyChanged);
      this.root = this.BuildTree((SceneElement) targetElement.ViewModel.ActiveEditingContainer, targetElement.ViewModel.BindingEditor);
      this.root.IsExpanded = true;
      this.dataSchemaFilter = dataSchemaFilter;
      ElementNode elementNode = this.root.FindDescendantSceneNode(targetElement) ?? (ElementNode) this.root;
      if (!elementNode.IsSelectable)
        return;
      elementNode.ExpandAncestors();
      this.selectionContext.SetSelection(elementNode);
    }

    public SceneNode CreateBindingOrData(SceneViewModel viewModel, SceneNode targetNode, IProperty targetProperty)
    {
      return (SceneNode) this.CreateBinding(viewModel, this.Path);
    }

    public SceneNode CreateBindingOrData(SceneViewModel viewModel, string bindingPath, SceneNode targetNode, IProperty targetProperty)
    {
      return (SceneNode) this.CreateBinding(viewModel, bindingPath);
    }

    private BindingSceneNode CreateBinding(SceneViewModel viewModel, string bindingPath)
    {
      BindingSceneNode bindingSceneNode = (BindingSceneNode) null;
      if (this.SelectedNode != null)
      {
        bindingSceneNode = BindingSceneNode.Factory.Instantiate(viewModel);
        if (!string.IsNullOrEmpty(this.Source))
          bindingSceneNode.ElementName = this.Source;
        else
          bindingSceneNode.ElementBindingTarget = (SceneNode) this.SelectedNode.Element;
        if (bindingPath.Length > 0)
          bindingSceneNode = bindingSceneNode.SetPath(bindingPath);
      }
      return bindingSceneNode;
    }

    public ElementBindingSourceNode FindElementNodeByName(string name)
    {
      return (ElementBindingSourceNode) this.Root.FindDescendantByName(name);
    }

    public void Unhook()
    {
      if (this.selectionContext != null)
      {
        this.selectionContext.PropertyChanged -= new PropertyChangedEventHandler(this.selectionContext_PropertyChanged);
        this.selectionContext = (SelectionContext<ElementNode>) null;
      }
      this.UnhookSelectedNode();
    }

    private ElementBindingSourceNode BuildTree(SceneElement element, BindingEditor bindingEditor)
    {
      ElementBindingSourceNode bindingSourceNode1 = new ElementBindingSourceNode(element, this.selectionContext);
      foreach (SceneNode sceneNode in element.GetAllContent())
      {
        BaseFrameworkElement frameworkElement = sceneNode as BaseFrameworkElement;
        if (frameworkElement != null)
        {
          ElementBindingSourceNode bindingSourceNode2 = this.BuildTree((SceneElement) frameworkElement, bindingEditor);
          if (bindingSourceNode2 != null)
            bindingSourceNode1.AddChild((ElementNode) bindingSourceNode2);
        }
      }
      if (element.ViewModel.TimelineItemManager.SortByZOrder)
        bindingSourceNode1.Children.Reverse();
      if (bindingSourceNode1.Element is StyleNode)
        bindingSourceNode1.IsSelectable = false;
      return bindingSourceNode1;
    }

    private void selectedNode_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "Path"))
        return;
      this.OnPropertyChanged("Path");
    }

    private void UnhookSelectedNode()
    {
      if (this.selectedNode == null)
        return;
      this.selectedNode.PropertyChanged -= new PropertyChangedEventHandler(this.selectedNode_PropertyChanged);
      if (this.dataSchemaFilter != null)
        this.selectedNode.SchemaItem.ClearDataSchemaItemFilter();
      this.selectedNode = (ElementBindingSourceNode) null;
    }

    private void selectionContext_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      this.UnhookSelectedNode();
      this.selectedNode = (ElementBindingSourceNode) this.selectionContext.PrimarySelection;
      if (this.selectedNode != null)
      {
        this.selectedNode.PropertyChanged += new PropertyChangedEventHandler(this.selectedNode_PropertyChanged);
        if (this.dataSchemaFilter != null)
          this.selectedNode.SchemaItem.SetDataSchemaItemFilter(this.dataSchemaFilter);
      }
      this.OnPropertyChanged("SelectedNode");
      this.OnPropertyChanged("Source");
      this.OnPropertyChanged("Path");
      this.OnPropertyChanged("Schema");
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
