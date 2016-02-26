// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.ClrObjectDataSourceNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.SampleData;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Documents;
using System;
using System.Globalization;
using System.IO;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class ClrObjectDataSourceNode : DataSourceNode
  {
    private ISchema schema;
    private Type sourceType;

    public Type Type
    {
      get
      {
        return this.sourceType;
      }
    }

    public string TypeName
    {
      get
      {
        return this.Type.Name;
      }
    }

    public string Namespace
    {
      get
      {
        if (string.IsNullOrEmpty(this.Type.Namespace))
          return StringTable.DefaultNamespace;
        return this.Type.Namespace;
      }
    }

    public override ISchema Schema
    {
      get
      {
        return this.schema;
      }
    }

    public override string ErrorMessage
    {
      get
      {
        string format = (string) null;
        string path = (string) null;
        SampleDataSet sampleData = this.SampleData;
        if (sampleData != null)
        {
          if (!sampleData.IsValid || sampleData.IsSaveFailed || sampleData.ValidRootNodeFromXamlDocument == null)
          {
            format = StringTable.XamlErrorsInSampleOrDesignDataFile;
            path = sampleData.XamlFilePath;
          }
        }
        else
        {
          path = this.CorruptSampleMetadataFile;
          if (!string.IsNullOrEmpty(path))
            format = StringTable.DataSourceErrorBecauseOfFile;
        }
        if (string.IsNullOrEmpty(format))
          return (string) null;
        if (!string.IsNullOrEmpty(path))
          path = DocumentReference.Create(this.DocumentNode.TypeResolver.ProjectPath).GetRelativePath(DocumentReference.Create(path));
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, new object[1]
        {
          (object) path
        });
      }
    }

    private string CorruptSampleMetadataFile
    {
      get
      {
        IProjectContext projectContext = (IProjectContext) this.DocumentNode.TypeResolver;
        string fullName = this.DocumentNode.Type.FullName;
        DataSetContext dataSetContext = DataSetContext.SampleData;
        string clrNamespacePrefix = DataSetContext.GetClrNamespacePrefix(dataSetContext.DataSetType, projectContext.RootNamespace);
        if (!fullName.StartsWith(clrNamespacePrefix, StringComparison.Ordinal))
        {
          dataSetContext = DataSetContext.DataStore;
          clrNamespacePrefix = DataSetContext.GetClrNamespacePrefix(dataSetContext.DataSetType, projectContext.RootNamespace);
          if (!fullName.StartsWith(clrNamespacePrefix, StringComparison.Ordinal))
            return (string) null;
        }
        string[] strArray = fullName.Substring(clrNamespacePrefix.Length).Split('.');
        if (strArray == null || strArray.Length != 2 || strArray[0] != strArray[1])
          return (string) null;
        string path2 = strArray[0];
        string str = Path.Combine(Path.Combine(Path.GetDirectoryName(projectContext.ProjectPath), dataSetContext.DataRootFolder), path2);
        if (!Directory.Exists(str))
          return (string) null;
        return Path.Combine(str, path2 + ".xsd");
      }
    }

    public ClrObjectDataSourceNode(ClrObjectSchema schema, DocumentNode entryNode)
      : base(entryNode)
    {
      this.schema = (ISchema) schema;
      this.sourceType = schema.Root.Type;
    }

    public override SceneNode CreateBindingOrData(SceneViewModel viewModel, string bindingPath, SceneNode targetNode, IProperty targetProperty)
    {
      SceneNode sceneNode = (SceneNode) null;
      DataSchemaNodePath nodePathFromPath = this.Schema.GetNodePathFromPath(bindingPath);
      if (nodePathFromPath != null)
        sceneNode = viewModel.BindingEditor.CreateAndSetBindingOrData(targetNode, (IPropertyId) targetProperty, nodePathFromPath);
      if (sceneNode == null)
        sceneNode = (SceneNode) this.CreateFallbackBind(viewModel, bindingPath, targetNode, targetProperty);
      return sceneNode;
    }

    private BindingSceneNode CreateFallbackBind(SceneViewModel viewModel, string bindingPath, SceneNode targetNode, IProperty targetProperty)
    {
      if (this.ResourceKey == null)
        return (BindingSceneNode) null;
      BindingSceneNode binding = BindingSceneNode.Factory.Instantiate(viewModel);
      DocumentNode keyNode = this.ResourceKey.Clone(viewModel.Document.DocumentContext);
      binding.Source = (DocumentNode) DocumentNodeUtilities.NewStaticResourceNode(viewModel.Document.DocumentContext, keyNode);
      if (bindingPath.Length > 0)
        binding = binding.SetPath(bindingPath);
      using (targetNode.ViewModel.AnimationEditor.DeferKeyFraming())
        targetNode.SetBinding((IPropertyId) targetProperty, binding);
      return binding;
    }
  }
}
