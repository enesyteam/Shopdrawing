// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.ExplicitDataContextBindingSourceModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class ExplicitDataContextBindingSourceModel : IBindingSourceModel, INotifyPropertyChanged
  {
    private ISchema schema;
    private SchemaItem schemaItem;

    public string DisplayName
    {
      get
      {
        return StringTable.DataBindingDialogExplicitContextDescription;
      }
    }

    public bool IsEnabled
    {
      get
      {
        return true;
      }
    }

    public string AutomationName
    {
      get
      {
        return "ExplicitDataContextSourceBindingSource";
      }
    }

    public ISchema Schema
    {
      get
      {
        return this.schema;
      }
    }

    public SchemaItem SchemaItem
    {
      get
      {
        return this.schemaItem;
      }
    }

    public string Path
    {
      get
      {
        return this.schemaItem.SelectionPath;
      }
    }

    public string PathDescription
    {
      get
      {
        return this.Schema.PathDescription;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    internal ExplicitDataContextBindingSourceModel(SceneNode targetElement, ReferenceStep targetProperty, DataSchemaItemFilter dataSchemaFilter)
    {
      this.schema = SchemaManager.GetSchemaForDataSourceInfo(new DataContextEvaluator().Evaluate(targetElement, (IPropertyId) targetProperty, true).DataSource);
      this.schemaItem = new SchemaItem(this.schema);
      DataSchemaNodePath nodePathFromPath = this.schema.GetNodePathFromPath((string) null);
      if (nodePathFromPath != null && nodePathFromPath.Node != null)
        this.schemaItem.SelectedItem = this.schemaItem.FindDataSchemaItemForNode(nodePathFromPath);
      this.schemaItem.SetDataSchemaItemFilter(dataSchemaFilter);
      this.schemaItem.PropertyChanged += new PropertyChangedEventHandler(this.schemaItem_PropertyChanged);
      if (this.schema is EmptySchema || this.schemaItem.Root == null)
        return;
      this.schemaItem.Root.IsExpanded = true;
    }

    public SceneNode CreateBindingOrData(SceneViewModel viewModel, SceneNode targetNode, IProperty targetProperty)
    {
      return this.CreateBindingOrData(viewModel, this.Path, targetNode, targetProperty);
    }

    public SceneNode CreateBindingOrData(SceneViewModel viewModel, string bindingPath, SceneNode targetNode, IProperty targetProperty)
    {
      BindingSceneNode bindingSceneNode = BindingSceneNode.Factory.Instantiate(viewModel);
      if (bindingPath.Length > 0)
      {
        if (this.Schema is ClrObjectSchema)
          bindingSceneNode = bindingSceneNode.SetPath(bindingPath);
        else if (this.Schema is XmlSchema)
          bindingSceneNode.XPath = bindingPath;
        else
          bindingSceneNode = bindingSceneNode.SetPath(bindingPath);
      }
      return (SceneNode) bindingSceneNode;
    }

    public void Unhook()
    {
      if (this.schemaItem == null)
        return;
      this.schemaItem.PropertyChanged -= new PropertyChangedEventHandler(this.schemaItem_PropertyChanged);
      this.schemaItem = (SchemaItem) null;
    }

    private void schemaItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "SelectionPath"))
        return;
      this.OnPropertyChanged("Path");
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
