// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.StyleAsset
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Project;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  public class StyleAsset : ResourceAsset
  {
    internal StyleAssetProvider Provider { get; private set; }

    public override string Name
    {
      get
      {
        string name = base.Name;
        if (name.Length == 0)
        {
          ITypeId typeId = (ITypeId) this.StyleType;
          if (typeId != null)
            name = typeId.Name;
        }
        return name;
      }
    }

    public override string TypeName
    {
      get
      {
        return StringTable.AssetToolStyleTypeName;
      }
    }

    internal override IType TargetType
    {
      get
      {
        return this.StyleType;
      }
    }

    public IType StyleType
    {
      get
      {
        DocumentCompositeNode valueNode = (DocumentCompositeNode) this.ResourceModel.ValueNode;
        if (valueNode != null)
          return valueNode.TypeResolver.ResolveType((ITypeId) StyleAsset.GetStyleType(valueNode));
        return (IType) null;
      }
    }

    internal StyleAsset(StyleAssetProvider provider, ResourceModel resourceModel)
      : base(resourceModel)
    {
      this.Provider = provider;
    }

    protected override void ComputeCategories(IList<AssetCategoryPath> categoryPaths, AssetTypeHelper typeHelper)
    {
      string userThemeName = AssetLibrary.GetUserThemeName(this.Provider as IUserThemeProvider);
      if (!string.IsNullOrEmpty(userThemeName))
      {
        categoryPaths.Add(PresetAssetCategoryPath.StylesRoot.Append(userThemeName, false));
        if (!typeHelper.IsPrototypingStyle(this))
          return;
        categoryPaths.Add(PresetAssetCategoryPath.PrototypeStyles.Append(userThemeName, false));
      }
      else
      {
        if (!typeHelper.IsStyleLocal(this))
          return;
        categoryPaths.Add((AssetCategoryPath) PresetAssetCategoryPath.StylesRoot);
        categoryPaths.Add((AssetCategoryPath) PresetAssetCategoryPath.ProjectRoot);
      }
    }

    protected override AssetInfoModel CreateAssetInfoModel(IProject project)
    {
      return (AssetInfoModel) StyleAssetInfoModel.Create(this.StyleType, this.ResourceModel);
    }

    public override bool SupportsTextEditing(IProjectContext projectContext)
    {
      return Enumerable.Any<ITypeId>(TextTool.TextToolTypes, (Func<ITypeId, bool>) (textType => textType.IsAssignableFrom((ITypeId) this.StyleType)));
    }

    public static IType GetStyleType(DocumentCompositeNode valueNode)
    {
      DocumentPrimitiveNode documentPrimitiveNode = valueNode.Properties[StyleNode.TargetTypeProperty] as DocumentPrimitiveNode;
      if (documentPrimitiveNode != null)
        return DocumentPrimitiveNode.GetValueAsType((DocumentNode) documentPrimitiveNode);
      return (IType) null;
    }

    public static StyleAsset Find(IEnumerable assets, ITypeId unresolvedType)
    {
      IType type = (IType) null;
      foreach (object obj in assets)
      {
        StyleAsset styleAsset = obj as StyleAsset;
        if (styleAsset != null)
        {
          if (type == null)
            type = styleAsset.StyleType.PlatformMetadata.ResolveType(unresolvedType);
          if (styleAsset.StyleType.Equals((object) type))
            return styleAsset;
        }
      }
      return (StyleAsset) null;
    }

    public bool ApplyStyle(SceneNode node)
    {
      BaseFrameworkElement frameworkElement = node as BaseFrameworkElement;
      if (frameworkElement == null || !this.StyleType.IsAssignableFrom((ITypeId) frameworkElement.Type))
        return false;
      ResourceDictionaryAssetProvider dictionaryAssetProvider = this.Provider as ResourceDictionaryAssetProvider;
      if (dictionaryAssetProvider != null && dictionaryAssetProvider.ContentProvider != null && !dictionaryAssetProvider.ContentProvider.EnsureLinked(frameworkElement.ViewModel) || frameworkElement.DocumentNodePath.Contains(this.ResourceModel.ValueNode))
        return false;
      IDocumentContext context = frameworkElement.DocumentNode.Context;
      DocumentNode valueNode = !JoltHelper.TypeSupported((ITypeResolver) frameworkElement.ProjectContext, PlatformTypes.DynamicResource) ? (DocumentNode) DocumentNodeUtilities.NewStaticResourceNode(context, this.ResourceModel.KeyNode.Clone(context)) : (DocumentNode) DocumentNodeUtilities.NewDynamicResourceNode(context, this.ResourceModel.KeyNode.Clone(context));
      frameworkElement.SetLocalValue(BaseFrameworkElement.StyleProperty, valueNode);
      return true;
    }

    protected override SceneNode InternalCreateInstance(ISceneInsertionPoint insertionPoint, Rect rect, OnCreateInstanceAction action)
    {
      SceneViewModel viewModel = insertionPoint.SceneNode.ViewModel;
      using (viewModel.DesignerContext.AmbientPropertyManager.SuppressApplyAmbientProperties())
      {
        ITypeId instanceType = (ITypeId) this.StyleType;
        if (instanceType != null)
          return this.GetInstantiator(viewModel.DefaultView).CreateInstance(instanceType, insertionPoint, rect, action);
        return (SceneNode) null;
      }
    }

    public override SceneNode CreatePrototypeInstance(ISceneInsertionPoint insertionPoint)
    {
      SceneViewModel viewModel = insertionPoint.SceneNode.ViewModel;
      using (viewModel.DesignerContext.AmbientPropertyManager.SuppressApplyAmbientProperties())
      {
        ITypeId instanceType = (ITypeId) this.StyleType;
        if (instanceType != null)
          return this.GetInstantiator(viewModel.DefaultView).CreatePrototypeInstance(instanceType);
        return (SceneNode) null;
      }
    }

    protected override bool InternalCanCreateInstance(ISceneInsertionPoint insertionPoint)
    {
      if (!this.IsInsertionPointValid(insertionPoint))
        return false;
      SceneNode sceneNode = insertionPoint.SceneNode;
      DocumentNodePath documentNodePath = sceneNode.DocumentNodePath;
      DocumentNode documentNode = new ExpressionEvaluator(sceneNode.ViewModel.DocumentRootResolver).EvaluateResource(documentNodePath, this.ResourceModel.KeyNode);
      DocumentNode valueNode = this.ResourceModel.ValueNode;
      if (documentNode != null)
        return !documentNodePath.Contains(valueNode);
      return false;
    }

    protected virtual DefaultTypeInstantiator GetInstantiator(SceneView sceneView)
    {
      return (DefaultTypeInstantiator) new StyleAssetInstantiator(sceneView, this.Provider, this);
    }

    protected virtual bool IsInsertionPointValid(ISceneInsertionPoint insertionPoint)
    {
      if (insertionPoint == null || insertionPoint.SceneElement == null)
        return false;
      SceneView defaultView = insertionPoint.SceneNode.ViewModel.DefaultView;
      if (defaultView == null || !defaultView.IsEditable)
        return false;
      ITypeId typeToInsert = (ITypeId) this.StyleType;
      return insertionPoint.CanInsert(typeToInsert);
    }

    protected override DrawingBrush CreateIconSourceBrush()
    {
      ITypeId type = (ITypeId) this.StyleType;
      if (type != null)
        return IconMapper.GetDrawingBrushForType(type, true, 24, 24);
      return (DrawingBrush) null;
    }
  }
}
