// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.TypeAsset
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.Extensibility;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Project;
using Microsoft.Windows.Design;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  internal class TypeAsset : Asset
  {
    private IType type;
    private IType onDemandType;
    private string onDemandAssemblyFileName;
    private DrawingBrush smallIcon;
    private DrawingBrush largeIcon;
    private ExampleAssetInfo exampleInfo;
    private string displayName;
    private AssemblyNameAndLocation[] resolvableAssemblyReferences;

    internal override IType TargetType
    {
      get
      {
        return this.Type;
      }
    }

    public bool IsSketchShapeType { get; private set; }

    public override string Name
    {
      get
      {
        if (this.exampleInfo != null && !string.IsNullOrEmpty(this.exampleInfo.DisplayName))
          return this.exampleInfo.DisplayName;
        if (!string.IsNullOrEmpty(this.displayName))
          return this.displayName;
        return this.type.Name;
      }
    }

    public override string TypeName
    {
      get
      {
        return this.type.Name;
      }
    }

    public override string Location
    {
      get
      {
        return this.type.RuntimeAssembly.Name;
      }
    }

    public IType Type
    {
      get
      {
        return this.type;
      }
    }

    public bool IsLicensed
    {
      get
      {
        object[] objArray = (object[]) null;
        try
        {
          objArray = this.type.RuntimeType.GetCustomAttributes(true);
        }
        catch (Exception ex)
        {
        }
        if (objArray != null)
        {
          foreach (object obj in objArray)
          {
            if (obj is LicenseProviderAttribute)
              return true;
          }
        }
        return false;
      }
    }

    public override DrawingBrush SmallIcon
    {
      get
      {
        return this.smallIcon ?? (this.smallIcon = this.CacheSmallIcon());
      }
    }

    public override DrawingBrush LargeIcon
    {
      get
      {
        return this.largeIcon ?? (this.largeIcon = this.CacheLargeIcon());
      }
    }

    public TypeAsset(IType type)
      : this(type, (string) null, (ExampleAssetInfo) null)
    {
    }

    public TypeAsset(IType type, string displayName, ExampleAssetInfo exampleInfo)
    {
      if (type == null)
        throw new ArgumentNullException("type");
      this.type = type;
      this.exampleInfo = exampleInfo;
      this.displayName = displayName;
    }

    public TypeAsset(IType type, string displayName, ExampleAssetInfo exampleInfo, string onDemandAssemblyFileName, AssemblyNameAndLocation[] resolvableAssemblyReferences)
      : this(type, displayName, exampleInfo)
    {
      this.onDemandAssemblyFileName = onDemandAssemblyFileName;
      this.resolvableAssemblyReferences = resolvableAssemblyReferences;
      this.onDemandType = type;
    }

    protected override void ComputeCategories(IList<AssetCategoryPath> categoryPaths, AssetTypeHelper typeHelper)
    {
      if (!string.IsNullOrEmpty(this.AssetInfo.Location))
      {
        string relativePath = this.AssetInfo.Location + (this.Type.Namespace != null ? "/" + this.Type.Namespace.Replace('.', '/') : string.Empty);
        categoryPaths.Add(PresetAssetCategoryPath.LocationsRoot.Append(relativePath, true));
      }
      if (typeHelper.IsTypeInProject(this.Type))
        categoryPaths.Add((AssetCategoryPath) PresetAssetCategoryPath.ProjectRoot);
      bool flag1 = typeHelper.IsMockupType(this.type);
      bool flag2 = typeHelper.IsControlType((ITypeId) this.Type) && !flag1;
      bool flag3 = AssetTypeHelper.IsBehaviorType((ITypeId) this.Type) || AssetTypeHelper.IsTriggerActionType((ITypeId) this.Type);
      bool flag4 = typeHelper.IsEffectType((ITypeId) this.Type);
      bool isShapeType = typeHelper.IsShapeType((ITypeId) this.Type);
      this.IsSketchShapeType = false;
      AssetCategoryPath rootPath = flag1 ? (AssetCategoryPath) PresetAssetCategoryPath.PrototypeRoot : (isShapeType ? (AssetCategoryPath) PresetAssetCategoryPath.ShapesRoot : (flag2 ? (AssetCategoryPath) PresetAssetCategoryPath.ControlsRoot : (flag3 ? (AssetCategoryPath) PresetAssetCategoryPath.BehaviorsRoot : (flag4 ? (AssetCategoryPath) PresetAssetCategoryPath.EffectsRoot : (AssetCategoryPath) null))));
      if (rootPath != null)
      {
        bool flag5 = false;
        bool hasCustomCategory = false;
        foreach (CustomAssetCategoryPath customPath in typeHelper.GetCustomAssetCategoryPaths(this.type))
        {
          hasCustomCategory = true;
          if (!string.IsNullOrEmpty(customPath.CategoryPath))
          {
            if (isShapeType && typeHelper.IsSketchShapesCategory(rootPath, customPath))
            {
              categoryPaths.Add(PresetAssetCategoryPath.PrototypeRoot.Append(customPath));
              this.IsSketchShapeType = true;
            }
            categoryPaths.Add(rootPath.Append(customPath));
          }
          flag5 = ((flag5 ? true : false) | (customPath.AlwaysShows ? 1 : (string.IsNullOrEmpty(customPath.CategoryPath) ? true : false))) != 0;
          categoryPaths.Add(PresetAssetCategoryPath.CategoriesRoot.Append(customPath));
        }
        if (this.exampleInfo != null && this.exampleInfo.Categories != null)
          EnumerableExtensions.ForEach<CustomAssetCategoryPath>((IEnumerable<CustomAssetCategoryPath>) this.exampleInfo.Categories, (Action<CustomAssetCategoryPath>) (customPath =>
          {
            hasCustomCategory = true;
            if (string.IsNullOrEmpty(customPath.CategoryPath))
              return;
            if (isShapeType && typeHelper.IsSketchShapesCategory(rootPath, customPath))
            {
              categoryPaths.Add(PresetAssetCategoryPath.PrototypeRoot.Append(customPath));
              this.IsSketchShapeType = true;
            }
            categoryPaths.Add(rootPath.Append(customPath));
          }));
        if (rootPath == PresetAssetCategoryPath.ControlsRoot)
          flag5 = flag5 & typeHelper.HasKnownPublicKey(this.Type) | typeHelper.IsCommonControlType((ITypeId) this.Type);
        else if (flag3 || flag4 || (isShapeType || flag1))
          flag5 |= !hasCustomCategory;
        if (flag5)
          categoryPaths.Add(rootPath);
      }
      if (flag2)
      {
        categoryPaths.Add((AssetCategoryPath) PresetAssetCategoryPath.ControlsAll);
        switch (typeHelper.GetPrototypeScreenType(this.Type))
        {
          case PrototypeScreenType.NavigationScreen:
            categoryPaths.Add((AssetCategoryPath) PresetAssetCategoryPath.PrototypeNavigations);
            break;
          case PrototypeScreenType.CompositionScreen:
            categoryPaths.Add((AssetCategoryPath) PresetAssetCategoryPath.PrototypeCompositions);
            break;
        }
      }
      else
      {
        if (!flag3 || !typeHelper.IsPrototypingType(this.Type))
          return;
        categoryPaths.Add((AssetCategoryPath) PresetAssetCategoryPath.PrototypeBehaviors);
        categoryPaths.Add((AssetCategoryPath) PresetAssetCategoryPath.BehaviorsPrototype);
      }
    }

    protected override AssetInfoModel CreateAssetInfoModel(IProject project)
    {
      return (AssetInfoModel) TypeAssetInfoModel.Create(this.Type, project, this.exampleInfo);
    }

    public override bool SupportsTextEditing(IProjectContext projectContext)
    {
      IType resolvedType = projectContext.ResolveType((ITypeId) this.Type);
      return Enumerable.Any<ITypeId>(TextTool.TextToolTypes, (Func<ITypeId, bool>) (textType => textType.IsAssignableFrom((ITypeId) resolvedType)));
    }

    protected override SceneNode InternalCreateInstance(ISceneInsertionPoint insertionPoint, Rect rect, OnCreateInstanceAction action)
    {
      if (!this.EnsureTypeReferenced(ProjectContext.GetProjectContext(insertionPoint.SceneNode.ProjectContext)))
        return (SceneNode) null;
      if (insertionPoint.SceneNode != null)
        insertionPoint.SceneNode.DesignerContext.ViewUpdateManager.RebuildPostponedViews();
      SceneView defaultView = insertionPoint.SceneNode.ViewModel.DefaultView;
      DefaultTypeInstantiator typeInstantiator = this.exampleInfo == null ? new DefaultTypeInstantiator(defaultView) : (DefaultTypeInstantiator) new TypeAsset.ExampleTypeInstantiator(defaultView, this.exampleInfo);
      using (this.IsSketchShapeType ? defaultView.DesignerContext.AmbientPropertyManager.SuppressApplyAmbientProperties() : (IDisposable) null)
        return typeInstantiator.CreateInstance((ITypeId) this.type, insertionPoint, rect, action);
    }

    public override SceneNode CreatePrototypeInstance(ISceneInsertionPoint insertionPoint)
    {
      IProjectContext projectContext = insertionPoint.SceneNode.ProjectContext;
      IType type1 = projectContext.ResolveType((ITypeId) this.Type);
      IType type2 = projectContext.GetType(type1.RuntimeAssembly.Name, type1.FullName) ?? projectContext.Platform.Metadata.GetType(type1.RuntimeType);
      if (type2 != null)
        return new DefaultTypeInstantiator(insertionPoint.SceneNode.ViewModel.DefaultView).CreatePrototypeInstance((ITypeId) type2);
      return (SceneNode) null;
    }

    protected override bool InternalCanCreateInstance(ISceneInsertionPoint insertionPoint)
    {
      if (insertionPoint == null)
        return false;
      bool flag = insertionPoint.CanInsert((ITypeId) this.type);
      if (flag)
      {
        ITypeId typeId = (ITypeId) insertionPoint.SceneNode.ViewModel.DocumentRoot.CodeBehindClass;
        if (typeId != null)
          flag = !this.type.Equals((object) typeId);
      }
      return flag;
    }

    public void OnProjectChanged()
    {
      if (this.onDemandAssemblyFileName == null || this.onDemandType == null)
        return;
      this.type = this.onDemandType;
    }

    public bool EnsureTypeReferenced(ProjectContext projectContext)
    {
      if (this.onDemandAssemblyFileName != null)
      {
        foreach (AssemblyNameAndLocation assemblyNameAndLocation in this.resolvableAssemblyReferences)
        {
          if (projectContext.GetAssembly(assemblyNameAndLocation.AssemblyName) == null)
            projectContext.EnsureAssemblyReferenced(assemblyNameAndLocation.Location);
        }
        if (!projectContext.EnsureAssemblyReferenced(this.onDemandAssemblyFileName))
          return false;
        IType type = projectContext.GetType(this.type.RuntimeAssembly.Name, this.type.FullName);
        if (type == null)
          return false;
        this.type = type;
      }
      return true;
    }

    public override int CompareTo(Asset other)
    {
      TypeAsset typeAsset = other as TypeAsset;
      if (typeAsset == null)
        return base.CompareTo(other);
      int num = Asset.CompareITypes(this.onDemandType ?? this.type, typeAsset.onDemandType ?? typeAsset.type);
      if (num != 0)
        return num;
      if (this.exampleInfo == null && typeAsset.exampleInfo == null)
        return 0;
      if (this.exampleInfo == null)
        return -1;
      if (typeAsset.exampleInfo == null)
        return 1;
      return this.exampleInfo.Index.CompareTo(typeAsset.exampleInfo.Index);
    }

    private DrawingBrush CacheSmallIcon()
    {
      IUnreferencedType unreferencedType = this.type as IUnreferencedType;
      if (unreferencedType != null && unreferencedType.SmallIcon != null)
        return unreferencedType.SmallIcon;
      if (this.exampleInfo != null && this.exampleInfo.SmallIcon != null)
      {
        DrawingBrush drawingBrushFromStream = IconMapper.CreateDrawingBrushFromStream((Stream) new MemoryStream(this.exampleInfo.SmallIcon), this.TypeName + this.exampleInfo.DisplayName + "12x12");
        if (drawingBrushFromStream != null)
          return drawingBrushFromStream;
      }
      return IconMapper.GetDrawingBrushForType((ITypeId) this.type, true, 12, 12);
    }

    private DrawingBrush CacheLargeIcon()
    {
      IUnreferencedType unreferencedType = this.type as IUnreferencedType;
      if (unreferencedType != null && unreferencedType.LargeIcon != null)
        return unreferencedType.LargeIcon;
      if (this.exampleInfo != null && this.exampleInfo.LargeIcon != null)
      {
        DrawingBrush drawingBrushFromStream = IconMapper.CreateDrawingBrushFromStream((Stream) new MemoryStream(this.exampleInfo.LargeIcon), this.TypeName + this.exampleInfo.DisplayName + "24x24");
        if (drawingBrushFromStream != null)
          return drawingBrushFromStream;
      }
      return IconMapper.GetDrawingBrushForType((ITypeId) this.type, true, 24, 24);
    }

    protected override DrawingBrush CreateIconSourceBrush()
    {
      return (DrawingBrush) null;
    }

    private class ExampleTypeInstantiator : DefaultTypeInstantiator
    {
      private ExampleAssetInfo exampleInfo;

      public ExampleTypeInstantiator(SceneView sceneView, ExampleAssetInfo exampleInfo)
        : base(sceneView)
      {
        this.exampleInfo = exampleInfo;
      }

      protected override SceneNode InternalCreateRawInstance(ITypeId instanceType)
      {
        IType type = this.ViewModel.ProjectContext.ResolveType(instanceType);
        if (type != null && type.RuntimeType != (System.Type) null)
        {
          ToolboxExampleAttribute attribute = TypeUtilities.GetAttribute<ToolboxExampleAttribute>(type.RuntimeType);
          if (attribute != null)
          {
            IToolboxExampleFactory toolboxExampleFactory = Activator.CreateInstance(attribute.ToolboxExampleFactoryType) as IToolboxExampleFactory;
            if (toolboxExampleFactory != null && toolboxExampleFactory.Examples != null)
            {
              IToolboxExample[] toolboxExampleArray = Enumerable.ToArray<IToolboxExample>(toolboxExampleFactory.Examples);
              if (this.exampleInfo.Index < toolboxExampleArray.Length)
              {
                IToolboxExample toolboxExample = toolboxExampleArray[this.exampleInfo.Index];
                if (toolboxExample != null)
                {
                  ISceneNodeModelItem sceneNodeModelItem = toolboxExample.CreateExample(this.ViewModel.ExtensibilityManager.EditingContext) as ISceneNodeModelItem;
                  if (sceneNodeModelItem != null && sceneNodeModelItem.SceneNode != null)
                    return sceneNodeModelItem.SceneNode;
                }
              }
            }
          }
        }
        return base.InternalCreateRawInstance(instanceType);
      }
    }
  }
}
