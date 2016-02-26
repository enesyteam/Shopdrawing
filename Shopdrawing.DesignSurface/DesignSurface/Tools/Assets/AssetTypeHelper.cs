// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.AssetTypeHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.Project;
using Microsoft.Windows.Design;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  public class AssetTypeHelper
  {
    private static readonly HashSet<string> commonControls = new HashSet<string>()
    {
      ProjectNeutralTypes.GridSplitter.FullName,
      PlatformTypes.Border.FullName,
      PlatformTypes.UniformGrid.FullName,
      PlatformTypes.BulletDecorator.FullName,
      PlatformTypes.Visual3D.FullName,
      PlatformTypes.Button.FullName,
      PlatformTypes.CheckBox.FullName,
      PlatformTypes.ComboBox.FullName,
      PlatformTypes.GroupBox.FullName,
      ProjectNeutralTypes.Label.FullName,
      PlatformTypes.ListView.FullName,
      PlatformTypes.Menu.FullName,
      PlatformTypes.PasswordBox.FullName,
      PlatformTypes.ScrollBar.FullName,
      PlatformTypes.TextBoxBase.FullName,
      PlatformTypes.ProgressBar.FullName,
      PlatformTypes.RadioButton.FullName,
      PlatformTypes.RichTextBox.FullName,
      ProjectNeutralTypes.TabControl.FullName,
      PlatformTypes.TextBlock.FullName,
      PlatformTypes.TextBox.FullName,
      PlatformTypes.ToolBar.FullName,
      ProjectNeutralTypes.TreeView.FullName,
      PlatformTypes.Popup.FullName,
      PlatformTypes.Slider.FullName,
      PlatformTypes.AmbientLight.FullName,
      PlatformTypes.DirectionalLight.FullName,
      ProjectNeutralTypes.Expander.FullName,
      PlatformTypes.Model3D.FullName,
      PlatformTypes.OrthographicCamera.FullName,
      PlatformTypes.Path.FullName,
      PlatformTypes.PerspectiveCamera.FullName,
      PlatformTypes.PointLight.FullName,
      PlatformTypes.ResizeGrip.FullName,
      PlatformTypes.ScrollViewer.FullName,
      PlatformTypes.SpotLight.FullName,
      PlatformTypes.Style.FullName,
      PlatformTypes.ListBox.FullName,
      PlatformTypes.MenuItem.FullName,
      PlatformTypes.ContentPresenter.FullName,
      ProjectNeutralTypes.Viewbox.FullName,
      PlatformTypes.InkCanvas.FullName,
      PlatformTypes.BlurEffect.FullName,
      PlatformTypes.DropShadowEffect.FullName,
      ProjectNeutralTypes.PathListBox.FullName
    };
    private static readonly string[] ThemeNames = new string[4]
    {
      "Luna",
      "Royale",
      "Aero",
      "Classic"
    };
    private static readonly byte[][] KnownPublicKeyTokens = new byte[2][]
    {
      new byte[8]
      {
        (byte) 49,
        (byte) 191,
        (byte) 56,
        (byte) 86,
        (byte) 173,
        (byte) 54,
        (byte) 78,
        (byte) 53
      },
      new byte[8]
      {
        (byte) 124,
        (byte) 236,
        (byte) 133,
        (byte) 215,
        (byte) 190,
        (byte) 167,
        (byte) 121,
        (byte) 142
      }
    };
    private IType visualType;
    private IType effectType;
    private IType shapeType;
    private IType frameworkElementType;
    private IType userControlType;
    private bool? isPrototypingProject;

    public IType VisualType
    {
      get
      {
        return this.visualType ?? (this.visualType = this.ProjectContext.ResolveType(PlatformTypes.Visual));
      }
    }

    public IType EffectType
    {
      get
      {
        return this.effectType ?? (this.effectType = this.ProjectContext.ResolveType(PlatformTypes.Effect));
      }
    }

    public IType ShapeType
    {
      get
      {
        return this.shapeType ?? (this.shapeType = this.ProjectContext.ResolveType(PlatformTypes.Shape));
      }
    }

    public IType FrameworkElementType
    {
      get
      {
        return this.frameworkElementType ?? (this.frameworkElementType = this.ProjectContext.ResolveType(PlatformTypes.FrameworkElement));
      }
    }

    public IType UserControlType
    {
      get
      {
        return this.userControlType ?? (this.userControlType = this.ProjectContext.ResolveType(PlatformTypes.UserControl));
      }
    }

    public bool IsPrototypingProject
    {
      get
      {
        bool? nullable = this.isPrototypingProject;
        return (nullable.HasValue ? new bool?(nullable.GetValueOrDefault()) : (this.isPrototypingProject = new bool?(this.ProjectContext.IsCapabilitySet(PlatformCapability.SupportPrototyping)))).Value;
      }
    }

    public IProjectContext ProjectContext { get; private set; }

    public IPrototypingService PrototypingService { get; private set; }

    public AssetTypeHelper(IProjectContext projectContext, IPrototypingService prototypingService)
    {
      this.ProjectContext = projectContext;
      this.PrototypingService = prototypingService;
    }

    public void EnforceTypeCaches()
    {
      this.visualType = this.VisualType;
      this.effectType = this.EffectType;
      this.shapeType = this.ShapeType;
      this.frameworkElementType = this.FrameworkElementType;
      this.userControlType = this.UserControlType;
      this.isPrototypingProject = new bool?(this.IsPrototypingProject);
    }

    public bool IsVisualType(ITypeId type)
    {
      return this.VisualType.IsAssignableFrom(type);
    }

    public bool IsControlType(ITypeId type)
    {
      return this.FrameworkElementType.IsAssignableFrom(type);
    }

    public bool IsUserControlType(ITypeId type)
    {
      return this.UserControlType.IsAssignableFrom(type);
    }

    public static bool IsBehaviorType(ITypeId type)
    {
      return ProjectNeutralTypes.Behavior.IsAssignableFrom(type);
    }

    public static bool IsTriggerActionType(ITypeId type)
    {
      return ProjectNeutralTypes.BehaviorTriggerAction.IsAssignableFrom(type);
    }

    public bool IsEffectType(ITypeId type)
    {
      return this.EffectType.IsAssignableFrom(type);
    }

    public bool IsShapeType(ITypeId type)
    {
      if (!this.ShapeType.IsAssignableFrom(type) && !ProjectNeutralTypes.PrimitiveShape.IsAssignableFrom(type) && !ProjectNeutralTypes.CompositeShape.IsAssignableFrom(type))
        return ProjectNeutralTypes.CompositeContentShape.IsAssignableFrom(type);
      return true;
    }

    public bool IsCommonControlType(ITypeId type)
    {
      return AssetTypeHelper.commonControls.Contains(type.FullName);
    }

    public bool IsPrototypingType(IType type)
    {
      return type.RuntimeAssembly.Name.StartsWith("Microsoft.Expression.Prototyping", StringComparison.Ordinal);
    }

    public bool IsMockupType(IType type)
    {
      return type.RuntimeAssembly.Name.StartsWith("Microsoft.Expression.Prototyping.Mockups", StringComparison.Ordinal);
    }

    public bool IsTypeInProject(IType type)
    {
      return this.ProjectContext.IsTypeInSolution(type);
    }

    public static bool IsTypeCreatable(Type type, bool isMainAssembly)
    {
      return TypeUtilities.HasDefaultConstructor(type, isMainAssembly) && !type.IsNested;
    }

    public static bool IsTypeBrowsable(Type type)
    {
      ToolboxBrowsableAttribute browsableAttribute = (ToolboxBrowsableAttribute) null;
      try
      {
        browsableAttribute = TypeUtilities.GetAttribute<ToolboxBrowsableAttribute>(type);
      }
      catch
      {
      }
      return browsableAttribute == null || browsableAttribute.Browsable;
    }

    public static string GetDisplayName(Type type)
    {
      DisplayNameAttribute attribute = TypeUtilities.GetAttribute<DisplayNameAttribute>(type);
      if (attribute != null && !string.IsNullOrEmpty(attribute.DisplayName))
        return attribute.DisplayName;
      if (type != (Type) null)
        return type.Name;
      return (string) null;
    }

    public PrototypeScreenType GetPrototypeScreenType(IType type)
    {
      if (this.PrototypingService != null && this.IsUserControlType((ITypeId) type))
        return this.PrototypingService.GetScreenType(type);
      return PrototypeScreenType.None;
    }

    public static IEnumerable<CustomAssetCategoryPath> GetCustomAssetCategoryPaths(Type type)
    {
      return CustomAssetCategoryPath.Convert(Enumerable.OfType<ToolboxCategoryAttribute>((IEnumerable) TypeUtilities.GetAttributes(type)));
    }

    internal IEnumerable<CustomAssetCategoryPath> GetCustomAssetCategoryPaths(IType type)
    {
      IUnreferencedType unreferencedType = type as IUnreferencedType;
      if (unreferencedType == null)
        return AssetTypeHelper.GetCustomAssetCategoryPaths(type.RuntimeType);
      return unreferencedType.Categories;
    }

    public bool IsSketchShapesCategory(AssetCategoryPath rootPath, CustomAssetCategoryPath customPath)
    {
      return rootPath == PresetAssetCategoryPath.ShapesRoot && customPath.CategoryPath == PresetAssetCategoryPath.SketchShapes.LastStep;
    }

    public bool IsAssemblySupported(string assemblyName, bool onDemand)
    {
      return !string.IsNullOrEmpty(assemblyName) && (!onDemand || !assemblyName.StartsWith("PresentationFramework.", StringComparison.Ordinal) || !Enumerable.Any<string>((IEnumerable<string>) AssetTypeHelper.ThemeNames, (Func<string, bool>) (themeName => assemblyName.Contains(themeName)))) && !assemblyName.StartsWith("Microsoft.Expression.Prototyping.Runtime", StringComparison.Ordinal);
    }

    public bool IsTypeSupported(IType type, bool filterWithProject = true)
    {
      if (type == null || type.RuntimeAssembly == null)
        return false;
      IUnreferencedType unreferencedType = type as IUnreferencedType;
      if (!this.IsAssemblySupported(type.RuntimeAssembly.Name, unreferencedType != null) || filterWithProject && !this.ProjectContext.IsTypeSupported((ITypeId) type))
        return false;
      if (AssetTypeHelper.IsBehaviorType((ITypeId) type) || AssetTypeHelper.IsTriggerActionType((ITypeId) type))
      {
        if (filterWithProject)
        {
          if (!this.IsPrototypingProject && this.IsPrototypingType(type) || !this.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsAssetLibraryBehaviorsItems))
            return false;
        }
        else if (unreferencedType != null && this.IsPrototypingType(type))
          return false;
      }
      return true;
    }

    public bool IsRuntimeTypeSupportedByAssetTool(Type type, bool allowNotPublic)
    {
      if (!type.IsGenericType && !type.IsAbstract && !type.IsValueType && (allowNotPublic || !type.IsNotPublic))
        return !PlatformTypeHelper.HasUnboundTypeArguments(type);
      return false;
    }

    internal bool IsStyleLocal(StyleAsset styleAsset)
    {
      foreach (IProjectDocument projectDocument in (IEnumerable<IProjectDocument>) this.ProjectContext.Documents)
      {
        if (projectDocument.DocumentType != ProjectDocumentType.ResourceDictionary && projectDocument.DocumentRoot == styleAsset.ResourceModel.ValueNode.DocumentRoot)
          return true;
      }
      return false;
    }

    internal bool IsStyleControlParts(StyleAsset styleAsset)
    {
      DocumentPrimitiveNode documentPrimitiveNode = ((DocumentCompositeNode) styleAsset.ResourceModel.ValueNode).Properties[DesignTimeProperties.IsControlPartProperty] as DocumentPrimitiveNode;
      if (documentPrimitiveNode != null)
        return documentPrimitiveNode.GetValue<bool>();
      return false;
    }

    internal bool HasKnownPublicKey(IType type)
    {
      IUnreferencedType unreferencedType = type as IUnreferencedType;
      byte[] publicKeyToken;
      if (unreferencedType != null)
      {
        publicKeyToken = unreferencedType.AssemblyPublicKey;
      }
      else
      {
        if (!type.RuntimeAssembly.IsLoaded)
          return false;
        publicKeyToken = type.RuntimeAssembly.GetPublicKeyToken();
      }
      return Enumerable.Any<byte[]>((IEnumerable<byte[]>) AssetTypeHelper.KnownPublicKeyTokens, (Func<byte[], bool>) (knownToken =>
      {
        if (publicKeyToken == null || publicKeyToken.Length != 8)
          return false;
        return ProjectAssemblyHelper.ComparePublicKeyTokens(knownToken, publicKeyToken);
      }));
    }

    internal static ResourceDictionaryUsage GetResourceDictionaryUsage(ResourceDictionaryContentProvider contentProvider)
    {
      ResourceDictionaryUsage resourceDictionaryUsage = ResourceDictionaryUsage.Unknown;
      if (contentProvider != null && contentProvider.Document != null && contentProvider.Document.RootNode != null)
      {
        DocumentCompositeNode documentCompositeNode = contentProvider.Document.RootNode as DocumentCompositeNode;
        if (documentCompositeNode != null)
        {
          if (documentCompositeNode.GetValue<bool>(DesignTimeProperties.IsSketchFlowStyleProperty))
            resourceDictionaryUsage |= ResourceDictionaryUsage.PrototypingStyles;
          if (documentCompositeNode.GetValue<bool>(DesignTimeProperties.IsSketchFlowDefaultStyleProperty))
            resourceDictionaryUsage |= ResourceDictionaryUsage.PrototypingDefaultStyles;
        }
      }
      return resourceDictionaryUsage;
    }

    internal bool IsPrototypingStyle(StyleAsset styleAsset)
    {
      ResourceDictionaryAssetProvider dictionaryAssetProvider = styleAsset.Provider as ResourceDictionaryAssetProvider;
      if (dictionaryAssetProvider != null)
        return (dictionaryAssetProvider.ResourceDictionaryUsage & ResourceDictionaryUsage.PrototypingStyles) != ResourceDictionaryUsage.Unknown;
      return false;
    }
  }
}
