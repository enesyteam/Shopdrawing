// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.XmlDataSourceNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class XmlDataSourceNode : DataSourceNode
  {
    private XmlSchema schema;

    public override ISchema Schema
    {
      get
      {
        return (ISchema) this.schema;
      }
    }

    public XmlDataSourceNode(XmlSchema schema, DocumentNode entryNode)
      : base(entryNode)
    {
      this.schema = schema;
    }

    public override SceneNode CreateBindingOrData(SceneViewModel viewModel, string bindingPath, SceneNode targetNode, IProperty targetProperty)
    {
      SceneNode sceneNode = (SceneNode) null;
      DataSchemaNodePath nodePathFromPath = this.Schema.GetNodePathFromPath(bindingPath);
      if (nodePathFromPath != null)
        sceneNode = (SceneNode) (viewModel.BindingEditor.CreateAndSetBindingOrData(targetNode, (IPropertyId) targetProperty, nodePathFromPath) as BindingSceneNode);
      if (sceneNode == null)
        sceneNode = (SceneNode) this.CreateFallbackBind(viewModel, bindingPath, targetNode, targetProperty);
      return sceneNode;
    }

    private BindingSceneNode CreateFallbackBind(SceneViewModel viewModel, string bindingPath, SceneNode targetNode, IProperty targetProperty)
    {
      BindingSceneNode binding = BindingSceneNode.Factory.Instantiate(viewModel);
      if (this.ResourceKey != null)
      {
        DocumentNode keyNode = this.ResourceKey.Clone(viewModel.Document.DocumentContext);
        binding.Source = (DocumentNode) DocumentNodeUtilities.NewStaticResourceNode(viewModel.Document.DocumentContext, keyNode);
      }
      if (bindingPath.Length > 0)
        binding.XPath = bindingPath;
      using (targetNode.ViewModel.AnimationEditor.DeferKeyFraming())
        targetNode.SetBinding((IPropertyId) targetProperty, binding);
      return binding;
    }
  }
}
