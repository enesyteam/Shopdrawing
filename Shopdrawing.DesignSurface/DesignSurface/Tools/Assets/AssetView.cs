// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.AssetView
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Project;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Project;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class AssetView : Control, INotifyPropertyChanged, ITreeViewItemProvider<AssetCategory>, IComponentConnector
  {
    private ObservableCollectionWorkaround<Asset> primarySearchResult = new ObservableCollectionWorkaround<Asset>();
    private ObservableCollectionWorkaround<Asset> secondarySearchResult = new ObservableCollectionWorkaround<Asset>();
    private AssetCategory RootCategory = AssetCategory.CreateRootCategory();
    private List<Asset> assets = new List<Asset>();
    private AssetLibraryDamages assetLibraryDamages = AssetLibraryDamages.Categories;
    private GridLength? categoryColumnWidth = new GridLength?();
    private string searchString;
    private VirtualizingTreeItemFlattener<AssetCategory> TreeFlattener;
    private bool setFocusToSearchBoxOnLoaded;
    private static IMultiValueConverter userThemeConverter;
    private bool? useListView;
    private AssetView.AsyncCall updateAssetAsyncCall;
    private AssetView.AsyncCall updateSearchAsyncCall;
    private AssetView.AsyncCall countCategoriesAsyncCall;
    private bool _contentLoaded;

    private AssetLibrary Library
    {
      get
      {
        return this.DataContext as AssetLibrary;
      }
    }

    private IProject ActiveProject
    {
      get
      {
        if (this.Library == null || this.Library.DesignerContext == null)
          return (IProject) null;
        return this.Library.DesignerContext.ActiveProject;
      }
    }

    private SceneViewModel ActiveSceneViewModel
    {
      get
      {
        if (this.Library == null || this.Library.DesignerContext == null)
          return (SceneViewModel) null;
        return this.Library.DesignerContext.ActiveSceneViewModel;
      }
    }

    public ReadOnlyObservableCollection<AssetCategory> Categories
    {
      get
      {
        if (this.TreeFlattener == null)
          return (ReadOnlyObservableCollection<AssetCategory>) null;
        return this.TreeFlattener.ItemList;
      }
    }

    public IList PrimarySearchResult
    {
      get
      {
        return (IList) this.primarySearchResult;
      }
    }

    public IList SecondarySearchResult
    {
      get
      {
        return (IList) this.secondarySearchResult;
      }
    }

    public bool SearchBoxHasFocus
    {
      get
      {
        object name = this.Template.FindName("SearchBox", (FrameworkElement) this);
        if (name != null)
          return ((UIElement) name).IsFocused;
        return false;
      }
    }

    public bool SetFocusToSearchBoxOnLoaded
    {
      get
      {
        return this.setFocusToSearchBoxOnLoaded;
      }
      set
      {
        if (this.setFocusToSearchBoxOnLoaded == value)
          return;
        this.setFocusToSearchBoxOnLoaded = value;
        this.OnPropertyChanged("SetFocusToSearchBoxOnLoaded");
      }
    }

    public string SearchString
    {
      get
      {
        return this.searchString;
      }
      set
      {
        if (!(this.searchString != value))
          return;
        this.searchString = value;
        this.OnPropertyChanged("SearchString");
        this.assetLibraryDamages |= AssetLibraryDamages.CategoryCounts;
        this.UpdateSearch();
      }
    }

    public AssetCategory SelectedCategory { get; private set; }

    public ICommand SelectCategoryCommand
    {
      get
      {
        return (ICommand) new ArgumentDelegateCommand((ArgumentDelegateCommand.ArgumentEventHandler) (obj => this.SelectCategory(obj as AssetCategory)));
      }
    }

    public ICommand DoubleClickCategoryCommand
    {
      get
      {
        return (ICommand) new ArgumentDelegateCommand((ArgumentDelegateCommand.ArgumentEventHandler) (obj => this.DoubleClickCategory(obj as AssetCategory)));
      }
    }

    public ICommand RightClickCategoryCommand
    {
      get
      {
        return (ICommand) new ArgumentDelegateCommand((ArgumentDelegateCommand.ArgumentEventHandler) (obj => this.OpenCategoryContextMenu(obj as AssetCategory)));
      }
    }

    public Asset SelectedAsset { get; private set; }

    public ICommand StartDragAssetCommand
    {
      get
      {
        return (ICommand) new ArgumentDelegateCommand((ArgumentDelegateCommand.ArgumentEventHandler) (obj => this.OnStartDragAsset(obj as Asset)));
      }
    }

    public ICommand SingleClickAssetCommand
    {
      get
      {
        return (ICommand) new ArgumentDelegateCommand((ArgumentDelegateCommand.ArgumentEventHandler) (obj => this.OnSingleClickAsset(obj as Asset)));
      }
    }

    public ICommand DoubleClickAssetCommand
    {
      get
      {
        return (ICommand) new ArgumentDelegateCommand((ArgumentDelegateCommand.ArgumentEventHandler) (obj => this.OnDoubleClickAsset(obj as Asset)));
      }
    }

    internal IAssetFilter AssetFilter { get; set; }

    internal ICategoryFilter CategoryFilter { get; set; }

    public static IMultiValueConverter UserThemeConverter
    {
      get
      {
        return AssetView.userThemeConverter ?? (AssetView.userThemeConverter = (IMultiValueConverter) new AssetView.UserThemeConverterImplementation());
      }
    }

    public GridLength CategoryColumnWidth
    {
      get
      {
        if (!this.categoryColumnWidth.HasValue)
          this.categoryColumnWidth = new GridLength?(this.GetProperty<GridLength>("CategoryColumnWidth", new GridLength(130.0)));
        return this.categoryColumnWidth.Value;
      }
      set
      {
        GridLength? nullable = this.categoryColumnWidth;
        GridLength gridLength = value;
        if ((!nullable.HasValue ? 1 : (nullable.GetValueOrDefault() != gridLength ? true : false)) == 0)
          return;
        this.categoryColumnWidth = new GridLength?(value);
        this.SetConfigurationProperty(this.PropertyName("CategoryColumnWidth"), (object) value);
        this.OnPropertyChanged("CategoryColumnWidth");
      }
    }

    public bool UseListView
    {
      get
      {
        if (!this.useListView.HasValue)
          this.useListView = new bool?(this.GetProperty<bool>("UseListView", this.DefaultUseListView));
        return this.useListView.Value;
      }
      set
      {
        bool? nullable = this.useListView;
        bool flag = value;
        if ((nullable.GetValueOrDefault() != flag ? 1 : (!nullable.HasValue ? true : false)) == 0)
          return;
        this.useListView = new bool?(value);
        this.SetConfigurationProperty(this.PropertyName("UseListView"), (object) (bool) (value ? true : false));
        this.OnPropertyChanged("UseListView");
      }
    }

    public string ConfigurationPrefix { get; set; }

    public bool DefaultUseListView { get; set; }

    private bool NeedRebuildCategories
    {
      get
      {
        return (this.assetLibraryDamages & AssetLibraryDamages.Categories) != AssetLibraryDamages.None;
      }
    }

    private bool NeedRebuildAssets
    {
      get
      {
        return (this.assetLibraryDamages & AssetLibraryDamages.Assets) != AssetLibraryDamages.None;
      }
    }

    private bool NeedRebuildAssetCategories
    {
      get
      {
        return (this.assetLibraryDamages & AssetLibraryDamages.AssetCategories) != AssetLibraryDamages.None;
      }
    }

    private bool NeedRebuildCategoryCounts
    {
      get
      {
        return (this.assetLibraryDamages & AssetLibraryDamages.CategoryCounts) != AssetLibraryDamages.None;
      }
    }

    public AssetCategory RootItem
    {
      get
      {
        return this.RootCategory;
      }
    }

    internal event EventHandler<AssetEventArgs> SelectedAssetChanged;

    internal event EventHandler<AssetEventArgs> AssetSingleClicked;

    internal event EventHandler<AssetEventArgs> AssetDoubleClicked;

    public event PropertyChangedEventHandler PropertyChanged;

    public AssetView()
    {
      this.TreeFlattener = new VirtualizingTreeItemFlattener<AssetCategory>((ITreeViewItemProvider<AssetCategory>) this);
      this.DataContextChanged += new DependencyPropertyChangedEventHandler(this.AssetView_DataContextChanged);
      this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(this.AssetView_IsVisibleChanged);
      this.InitializeComponent();
      this.AssetFilter = (IAssetFilter) new DefaultAssetViewFilter();
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      FrameworkElement categoryTree = (FrameworkElement) this.Template.FindName("CategoryTree", (FrameworkElement) this);
      categoryTree.LayoutUpdated += (EventHandler) ((s, a) => this.CategoryColumnWidth = new GridLength(categoryTree.ActualWidth));
    }

    private void AssetView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (this.IsVisible)
      {
        this.Library.OnOpen();
        this.UpdateAssets(AssetLibraryDamages.None);
      }
      else
        this.Library.OnClose();
    }

    private IEnumerable<AssetCategory> AllCategories()
    {
      Queue<AssetCategory> categories = new Queue<AssetCategory>();
      categories.Enqueue(this.RootCategory);
      while (categories.Count > 0)
      {
        AssetCategory current = categories.Dequeue();
        foreach (AssetCategory assetCategory in (Collection<AssetCategory>) current.Children)
          categories.Enqueue(assetCategory);
        yield return current;
      }
    }

    private void SetConfigurationProperty(string propertyName, object value)
    {
      if (this.Library == null || this.Library.DesignerContext == null)
        return;
      this.Library.DesignerContext.Configuration.SetProperty(propertyName, value);
    }

    private T GetProperty<T>(string propertyName, T defaultValue)
    {
      if (this.Library != null && this.Library.DesignerContext != null)
        return (T) this.Library.DesignerContext.Configuration.GetProperty(this.PropertyName(propertyName), (object) defaultValue);
      return defaultValue;
    }

    private string PropertyName(string propertyName)
    {
      if (this.ConfigurationPrefix == null)
        return propertyName;
      return this.ConfigurationPrefix + propertyName;
    }

    private static bool IsUserThemeCategory(AssetCategoryPath path)
    {
      if (PresetAssetCategoryPath.StylesRoot != path && PresetAssetCategoryPath.StylesRoot.Contains(path))
        return true;
      if (PresetAssetCategoryPath.PrototypeStyles != path)
        return PresetAssetCategoryPath.PrototypeStyles.Contains(path);
      return false;
    }

    private void AssetView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      AssetLibrary assetLibrary1 = e.OldValue as AssetLibrary;
      if (assetLibrary1 != null)
      {
        assetLibrary1.AssetLibraryChanged -= new Action<AssetLibraryDamages>(this.OnAssetLibraryChanged);
        assetLibrary1.ActiveUserThemeProviderChanged -= new Action(this.Library_ActiveUserThemeProviderChanged);
        assetLibrary1.NeedsUpdateChanged -= new Action(this.OnAssetLibraryNeedsUpdateChanged);
        assetLibrary1.DesignerContext.SelectionManager.LateActiveSceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.SelectionManager_LateActiveSceneUpdatePhase);
      }
      AssetLibrary assetLibrary2 = e.NewValue as AssetLibrary;
      if (assetLibrary2 != null)
      {
        assetLibrary2.ActiveUserThemeProviderChanged += new Action(this.Library_ActiveUserThemeProviderChanged);
        assetLibrary2.AssetLibraryChanged += new Action<AssetLibraryDamages>(this.OnAssetLibraryChanged);
        assetLibrary2.NeedsUpdateChanged += new Action(this.OnAssetLibraryNeedsUpdateChanged);
        assetLibrary2.DesignerContext.SelectionManager.LateActiveSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.SelectionManager_LateActiveSceneUpdatePhase);
      }
      this.UpdateAssets(AssetLibraryDamages.All);
      this.OnAssetLibraryNeedsUpdateChanged();
    }

    private void SelectionManager_LateActiveSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      this.IsEnabled = this.ActiveProject != null && this.ActiveSceneViewModel != null && this.ActiveSceneViewModel.DefaultView.IsDesignSurfaceVisible && this.ActiveSceneViewModel.ActiveSceneInsertionPoint != null;
    }

    private void Library_ActiveUserThemeProviderChanged()
    {
      foreach (AssetCategory assetCategory in (ReadOnlyCollection<AssetCategory>) this.Categories)
      {
        if (PresetAssetCategoryPath.StylesRoot.Contains(assetCategory.Path) || PresetAssetCategoryPath.PrototypeStyles.Contains(assetCategory.Path))
          assetCategory.UpdateSelfBinding();
      }
    }

    private void OnAssetLibraryNeedsUpdateChanged()
    {
      if (this.Library != null && this.Library.IsOpened && (this.Library.NeedsUpdate && this.IsEnabled))
        this.Cursor = Cursors.AppStarting;
      else
        this.Cursor = Cursors.Arrow;
    }

    private void OnAssetLibraryChanged(AssetLibraryDamages damages)
    {
      this.UpdateAssets(damages);
    }

    private void SelectCategory(AssetCategory category)
    {
      if (this.SelectedCategory == category)
        return;
      this.SelectedCategory = category;
      this.OnPropertyChanged("SelectedCategory");
      this.UpdateSearch();
    }

    private void DoubleClickCategory(AssetCategory category)
    {
      if (category.Children.Count <= 0)
        return;
      category.IsExpanded = !category.IsExpanded;
    }

    private void OpenCategoryContextMenu(AssetCategory category)
    {
      ContextMenu contextMenu = (ContextMenu) null;
      if (AssetView.IsUserThemeCategory(category.Path))
      {
        this.SelectCategory(category);
        contextMenu = UserThemeCommandHelper.GetThemeContextMenu(this.Library, category);
      }
      if (contextMenu == null)
        return;
      contextMenu.Placement = PlacementMode.MousePoint;
      contextMenu.IsOpen = true;
    }

    private void OnStartDragAsset(Asset asset)
    {
      this.SelectAsset(asset);
    }

    private void OnSingleClickAsset(Asset asset)
    {
      this.SelectAsset(asset);
      if (this.AssetSingleClicked == null)
        return;
      this.AssetSingleClicked((object) this, new AssetEventArgs(this.SelectedAsset));
    }

    private void SelectAsset(Asset asset)
    {
      if (this.SelectedAsset != asset)
      {
        this.SelectedAsset = asset;
        this.OnPropertyChanged("SelectedAsset");
      }
      if (this.SelectedAssetChanged == null)
        return;
      this.SelectedAssetChanged((object) this, new AssetEventArgs(this.SelectedAsset));
    }

    private void OnDoubleClickAsset(Asset asset)
    {
      if (this.AssetDoubleClicked == null)
        return;
      this.AssetDoubleClicked((object) this, new AssetEventArgs(asset));
    }

    private void RebuildCategories()
    {
      this.RootCategory = AssetCategory.CreateRootCategory();
      IXamlProject xamlProject = this.ActiveProject as IXamlProject;
      if (xamlProject != null && xamlProject.ProjectContext != null)
      {
        AssetTypeHelper typeHelper = new AssetTypeHelper(xamlProject.ProjectContext, this.Library.DesignerContext.PrototypingService);
        foreach (PresetAssetCategory category in Enum.GetValues(typeof (PresetAssetCategory)))
        {
          AssetCategoryPath path = (AssetCategoryPath) PresetAssetCategoryPath.FromPreset(category);
          if (this.FilterCategory(path, typeHelper))
            this.RootCategory.CreateCategory(path);
        }
      }
      this.TreeFlattener.RebuildList(false);
      if (this.Categories.Count == 1)
        this.Categories[0].IsExpanded = true;
      this.OnPropertyChanged("Categories");
      this.OnPropertyChanged("RootItem");
      this.assetLibraryDamages &= ~AssetLibraryDamages.Categories;
      this.assetLibraryDamages |= AssetLibraryDamages.Assets;
      this.assetLibraryDamages |= AssetLibraryDamages.AssetCategories;
      this.assetLibraryDamages |= AssetLibraryDamages.CategoryCounts;
    }

    internal void UpdateAssets(AssetLibraryDamages damages)
    {
      this.assetLibraryDamages |= damages;
      if (!this.IsVisible && !AssetLibrary.DisableAsyncUpdate)
        return;
      if ((this.assetLibraryDamages & AssetLibraryDamages.ProjectChanged) != AssetLibraryDamages.None)
      {
        if (this.assets.Count > 0)
        {
          this.assets.Clear();
          this.primarySearchResult.Clear();
          this.secondarySearchResult.Clear();
          this.OnPropertyChanged("PrimarySearchResult");
          this.OnPropertyChanged("SecondarySearchResult");
        }
        this.assetLibraryDamages &= ~AssetLibraryDamages.ProjectChanged;
      }
      AssetView.AsyncCall.Invoke(ref this.updateAssetAsyncCall, new Action(this.UpdateAssetsWorker), this.Library.NeedsUpdate);
    }

    private void UpdateAssetsWorker()
    {
      if (this.Library == null || this.Library.DesignerContext == null)
        return;
      using (this.NeedRebuildCategories ? new AssetView.PreserveSelectionToken(this) : (AssetView.PreserveSelectionToken) null)
      {
        using (PerformanceUtility.PerformanceSequence(PerformanceEvent.AssetViewRebuildAssets))
        {
          if (this.NeedRebuildCategories)
            this.RebuildCategories();
          if (!this.NeedRebuildAssets && !this.NeedRebuildAssetCategories)
            return;
          this.RebuildAssets();
          this.UpdateSearch();
        }
      }
    }

    private void RebuildAssets()
    {
      this.assets.Clear();
      IXamlProject xamlProject = this.ActiveProject as IXamlProject;
      if (xamlProject != null)
      {
        AssetTypeHelper typeHelper = new AssetTypeHelper(xamlProject.ProjectContext, this.Library.DesignerContext.PrototypingService);
        foreach (Asset asset in this.Library.Assets)
        {
          if (this.PrefilterAsset(asset, typeHelper))
          {
            if (asset.Categories == null || this.NeedRebuildAssetCategories)
              asset.UpdateOnProject(this.ActiveProject, typeHelper);
            if (this.FilterAsset(asset, typeHelper))
            {
              bool flag = false;
              foreach (AssetCategoryPath path in (IEnumerable<AssetCategoryPath>) asset.Categories)
              {
                if (this.FilterCategory(path, typeHelper))
                {
                  flag = true;
                  this.RootCategory.CreateCategory(path);
                }
              }
              if (flag)
                this.assets.Add(asset);
            }
          }
        }
      }
      this.assetLibraryDamages &= ~AssetLibraryDamages.Assets;
      this.assetLibraryDamages &= ~AssetLibraryDamages.AssetCategories;
      this.assetLibraryDamages |= AssetLibraryDamages.CategoryCounts;
    }

    private bool PrefilterAsset(Asset asset, AssetTypeHelper typeHelper)
    {
      if (asset == null || !asset.IsValid)
        return false;
      IType targetType = asset.TargetType;
      return targetType == null || typeHelper.IsTypeSupported(targetType, true);
    }

    private bool FilterAsset(Asset asset, AssetTypeHelper typeHelper)
    {
      if (asset.Categories == null || asset.Categories.Count == 0 || !asset.IsValid || this.AssetFilter != null && !this.AssetFilter.IsValid(asset, typeHelper))
        return false;
      if (!typeHelper.IsPrototypingProject)
      {
        TypeAsset typeAsset = asset as TypeAsset;
        if (typeAsset != null && typeAsset.IsSketchShapeType)
          return false;
      }
      return true;
    }

    private bool FilterCategory(AssetCategoryPath path, AssetTypeHelper typeHelper)
    {
      return (typeHelper.IsPrototypingProject || !PresetAssetCategoryPath.PrototypeRoot.Contains(path) && !PresetAssetCategoryPath.BehaviorsPrototype.Contains(path) && !PresetAssetCategoryPath.SketchShapes.Contains(path)) && ((typeHelper.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsAssetLibraryBehaviorsItems) || !PresetAssetCategoryPath.BehaviorsRoot.Contains(path)) && (typeHelper.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsUIElementEffectProperty) || !PresetAssetCategoryPath.EffectsRoot.Contains(path)) && (this.CategoryFilter == null || this.CategoryFilter.IsValid(path, typeHelper)));
    }

    private void UpdateSearch()
    {
      AssetView.AsyncCall.Invoke(ref this.updateSearchAsyncCall, new Action(this.UpdateSearchWorker), this.Library.NeedsUpdate);
    }

    private void UpdateSearchWorker()
    {
      if (this.Library == null || this.Library.DesignerContext == null)
        return;
      using (PerformanceUtility.PerformanceSequence(PerformanceEvent.AssetViewUpdateSearch))
      {
        List<Asset> list1 = new List<Asset>();
        List<Asset> list2 = new List<Asset>();
        bool flag1 = false;
        bool flag2 = false;
        if (this.Library != null)
        {
          foreach (Asset asset in this.SearchAsset(this.SearchString, this.SelectedCategory, true))
          {
            flag1 |= asset == this.SelectedAsset;
            list1.Add(asset);
          }
          foreach (Asset asset in this.SearchAsset(this.SearchString, this.SelectedCategory, false))
          {
            flag2 |= asset == this.SelectedAsset;
            list2.Add(asset);
          }
        }
        this.IncrementallyChangeList<Asset>(this.primarySearchResult, (IList<Asset>) list1, Asset.AlphabeticComparer);
        this.IncrementallyChangeList<Asset>(this.secondarySearchResult, (IList<Asset>) list2, Asset.AlphabeticComparer);
        if (this.SelectedAsset != null && (!flag1 && !flag2 || flag1 && !this.BringAssetItemIntoView("PrimarySearchResultList") || flag2 && !this.BringAssetItemIntoView("SecondarySearchResultList")))
          this.SelectedAsset = (Asset) null;
        if (!this.NeedRebuildCategoryCounts)
          return;
        this.CountAssetCategories();
      }
    }

    private void IncrementallyChangeList<T>(ObservableCollectionWorkaround<T> oldList, IList<T> newList, IComparer<T> comparer)
    {
      if (newList.Count == 0)
      {
        oldList.Clear();
      }
      else
      {
        IList<T> list;
        if (oldList.Count == 0)
        {
          list = newList;
        }
        else
        {
          HashSet<T> oldItemsSet = new HashSet<T>((IEnumerable<T>) oldList);
          list = (IList<T>) Enumerable.ToList<T>(Enumerable.Where<T>((IEnumerable<T>) newList, (Func<T, bool>) (item => !oldItemsSet.Contains(item))));
          oldItemsSet.ExceptWith((IEnumerable<T>) newList);
          EnumerableExtensions.ForEach<T>((IEnumerable<T>) oldItemsSet, (Action<T>) (item => ((Collection<T>) oldList).Remove(item)));
        }
        foreach (T obj in (IEnumerable<T>) list)
        {
          int index = oldList.BinarySearch(obj, comparer);
          if (index < 0)
            index = ~index;
          oldList.Insert(index, obj);
        }
      }
    }

    private void CountAssetCategories()
    {
      AssetView.AsyncCall.Invoke(ref this.countCategoriesAsyncCall, new Action(this.CountAssetCategoriesWorker), this.Library.NeedsUpdate);
    }

    private void CountAssetCategoriesWorker()
    {
      if (this.Library == null || this.Library.DesignerContext == null || !this.NeedRebuildCategoryCounts)
        return;
      using (PerformanceUtility.PerformanceSequence(PerformanceEvent.AssetViewRecountCategories))
      {
        EnumerableExtensions.ForEach<AssetCategory>(this.AllCategories(), (Action<AssetCategory>) (category => category.AssetCount = 0));
        foreach (Asset asset in this.GetAssetsForCategoryCounting())
        {
          HashSet<AssetCategoryPath> hashSet = new HashSet<AssetCategoryPath>();
          using (IEnumerator<AssetCategoryPath> enumerator = asset.Categories.GetEnumerator())
          {
label_9:
            while (enumerator.MoveNext())
            {
              AssetCategoryPath current = enumerator.Current;
              hashSet.Add(current);
              AssetCategoryPath assetCategoryPath = current;
              while (true)
              {
                if (!assetCategoryPath.IsRoot && assetCategoryPath.AlwaysShow)
                {
                  hashSet.Add(assetCategoryPath);
                  assetCategoryPath = assetCategoryPath.Parent;
                }
                else
                  goto label_9;
              }
            }
          }
          foreach (AssetCategoryPath index in hashSet)
          {
            AssetCategory assetCategory = this.RootCategory[index];
            if (assetCategory != null)
              ++assetCategory.AssetCount;
          }
        }
        EnumerableExtensions.ForEach<AssetCategory>(this.AllCategories(), (Action<AssetCategory>) (category => category.NotifyAssetCountChanged()));
        this.assetLibraryDamages &= ~AssetLibraryDamages.CategoryCounts;
      }
    }

    private IEnumerable<Asset> GetAssetsForCategoryCounting()
    {
      if (string.IsNullOrEmpty(this.SearchString))
        return (IEnumerable<Asset>) this.assets;
      return Enumerable.Concat<Asset>((IEnumerable<Asset>) this.primarySearchResult, (IEnumerable<Asset>) this.secondarySearchResult);
    }

    private bool BringAssetItemIntoView(string itemsControlName)
    {
      if (this.Template != null)
      {
        object obj = (object) this.SelectedAsset;
        FrameworkElement frameworkElement = (this.Template.FindName(itemsControlName, (FrameworkElement) this) as ItemsControl).ItemContainerGenerator.ContainerFromItem(obj) as FrameworkElement;
        if (frameworkElement != null)
        {
          frameworkElement.BringIntoView();
          return true;
        }
      }
      return false;
    }

    private IEnumerable<Asset> SearchAsset(string searchString, AssetCategory category, bool withinCategory)
    {
      if (string.IsNullOrEmpty(searchString))
      {
        if (category != null && withinCategory)
        {
          foreach (Asset asset in this.assets)
          {
            if (category.Contains(asset))
              yield return asset;
          }
        }
      }
      else
      {
        foreach (Asset asset in this.assets)
        {
          if (asset.Name.IndexOf(searchString, StringComparison.CurrentCultureIgnoreCase) >= 0 && ((!withinCategory ? true : false) ^ (category == null ? 0 : (category.Contains(asset) ? true : false))) != 0)
            yield return asset;
        }
      }
    }

    private void Hyperlink_Click(object sender, EventArgs e)
    {
      WebPageLauncher.Navigate(((Hyperlink) sender).NavigateUri, (IMessageDisplayService) null);
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/tools/assettool/assetlibrary/assetview.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          ((Hyperlink) target).Click += new RoutedEventHandler(this.Hyperlink_Click);
          break;
        case 2:
          ((Hyperlink) target).Click += new RoutedEventHandler(this.Hyperlink_Click);
          break;
        case 3:
          ((Hyperlink) target).Click += new RoutedEventHandler(this.Hyperlink_Click);
          break;
        case 4:
          ((Hyperlink) target).Click += new RoutedEventHandler(this.Hyperlink_Click);
          break;
        case 5:
          ((Hyperlink) target).Click += new RoutedEventHandler(this.Hyperlink_Click);
          break;
        case 6:
          ((Hyperlink) target).Click += new RoutedEventHandler(this.Hyperlink_Click);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }

    private class UserThemeConverterImplementation : IMultiValueConverter
    {
      public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
      {
        AssetCategory category = (AssetCategory) values[0];
        return (object) (Visibility) (this.ShouldCheckCategory(((AssetView) values[1]).Library, category) ? 0 : 2);
      }

      public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
      {
        throw new NotImplementedException();
      }

      private bool ShouldCheckCategory(AssetLibrary library, AssetCategory category)
      {
        if (AssetView.IsUserThemeCategory(category.Path))
          return AssetLibrary.GetUserThemeName(library.ActiveUserThemeProvider) == category.Path.LastStep;
        return false;
      }
    }

    private class PreserveSelectionToken : IDisposable
    {
      private AssetCategoryPath SelectedCategory = (AssetCategoryPath) PresetAssetCategoryPath.ControlsRoot;
      private IList<AssetCategoryPath> ExpandedCategories = (IList<AssetCategoryPath>) new List<AssetCategoryPath>();
      private AssetView Host;

      public PreserveSelectionToken(AssetView assetView)
      {
        this.Host = assetView;
        if (this.Host.SelectedCategory != null)
          this.SelectedCategory = this.Host.SelectedCategory.Path;
        foreach (AssetCategory assetCategory in (ReadOnlyCollection<AssetCategory>) this.Host.Categories)
        {
          if (assetCategory.IsExpanded)
            this.ExpandedCategories.Add(assetCategory.Path);
        }
      }

      public void Dispose()
      {
        foreach (AssetCategoryPath index in (IEnumerable<AssetCategoryPath>) this.ExpandedCategories)
        {
          AssetCategory assetCategory = this.Host.RootCategory[index];
          if (assetCategory != null)
            assetCategory.IsExpanded = true;
        }
        this.Host.SelectCategory(this.Host.RootCategory[this.SelectedCategory] ?? this.Host.RootCategory[(AssetCategoryPath) PresetAssetCategoryPath.ControlsRoot]);
      }
    }

    private class AsyncCall
    {
      private Action workerAction;
      private DispatcherTimer timer;

      private AsyncCall(Action workerAction)
      {
        this.workerAction = workerAction;
      }

      public static void Invoke(ref AssetView.AsyncCall asyncCall, Action workerAction, bool lowPriority)
      {
        if (asyncCall == null)
          asyncCall = new AssetView.AsyncCall(workerAction);
        asyncCall.Invoke(lowPriority);
      }

      private void Invoke(bool lowPriority)
      {
        if (AssetLibrary.DisableAsyncUpdate)
        {
          this.workerAction();
        }
        else
        {
          if (this.timer == null)
          {
            this.timer = new DispatcherTimer(DispatcherPriority.Background);
            this.timer.Tick += (EventHandler) ((s, e) => this.InvokeWorker());
          }
          this.timer.Stop();
          this.timer.Interval = TimeSpan.FromSeconds(lowPriority ? 0.5 : 0.1);
          this.timer.Start();
        }
      }

      private void InvokeWorker()
      {
        this.workerAction();
        this.timer.Stop();
      }
    }
  }
}
