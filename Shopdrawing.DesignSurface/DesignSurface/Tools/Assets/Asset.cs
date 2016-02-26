// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.Asset
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  public abstract class Asset : INotifyPropertyChanged
  {
    private static readonly object licenseManagerLock = new object();
    public static readonly DependencyProperty StartDragCommandProperty = DependencyProperty.RegisterAttached("StartDragCommand", typeof (ICommand), typeof (Asset));
    private bool isValid = true;
    private DrawingBrush icon;
    private static IComparer<Asset> defaultComparer;
    private static IComparer<Asset> alphabeticComparer;

    public abstract string Name { get; }

    public abstract string TypeName { get; }

    public abstract string Location { get; }

    public AssetInfoModel AssetInfo { get; private set; }

    public bool IsValid
    {
      get
      {
        return this.isValid;
      }
      set
      {
        if (this.isValid == value)
          return;
        this.isValid = value;
      }
    }

    public virtual IDragDropHandler DragDropHandler
    {
      get
      {
        return (IDragDropHandler) new Microsoft.Expression.DesignSurface.DragDropHandler()
        {
          DragAction = new Action<UIElement>(this.DoDragDrop)
        };
      }
    }

    public virtual Size SourcePixelSize
    {
      get
      {
        return Size.Empty;
      }
    }

    private DrawingBrush IconSourceBrush
    {
      get
      {
        if (this.icon == null)
          this.icon = this.CreateIconSourceBrush();
        return this.icon;
      }
    }

    public virtual DrawingBrush SmallIcon
    {
      get
      {
        return this.IconSourceBrush;
      }
    }

    public virtual DrawingBrush LargeIcon
    {
      get
      {
        return this.IconSourceBrush;
      }
    }

    internal IList<AssetCategoryPath> Categories { get; private set; }

    internal virtual IType TargetType
    {
      get
      {
        return (IType) null;
      }
    }

    public static IComparer<Asset> DefaultComparer
    {
      get
      {
        return Asset.defaultComparer ?? (Asset.defaultComparer = (IComparer<Asset>) new Asset.AssetComparer());
      }
    }

    public static IComparer<Asset> AlphabeticComparer
    {
      get
      {
        return Asset.alphabeticComparer ?? (Asset.alphabeticComparer = (IComparer<Asset>) new Asset.AssetAlphabeticComparer());
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public static ICommand GetStartDragCommand(DependencyObject dependencyObject)
    {
      return (ICommand) dependencyObject.GetValue(Asset.StartDragCommandProperty);
    }

    public static void SetStartDragCommand(DependencyObject dependencyObject, ICommand value)
    {
      dependencyObject.SetValue(Asset.StartDragCommandProperty, (object) value);
    }

    public virtual bool SupportsTextEditing(IProjectContext projectContext)
    {
      return false;
    }

    public IEnumerable<ISceneInsertionPoint> FindInsertionPoints(SceneViewModel viewModel)
    {
      IEnumerable<ISceneInsertionPoint> insertionPoints = this.InternalFindInsertionPoints(viewModel);
      if (insertionPoints != null)
        return insertionPoints;
      return (IEnumerable<ISceneInsertionPoint>) new ISceneInsertionPoint[1]
      {
        viewModel.ActiveSceneInsertionPoint
      };
    }

    public bool CanCreateInstance(ISceneInsertionPoint insertionPoint)
    {
      if (insertionPoint != null && insertionPoint.SceneElement != null && (!insertionPoint.SceneElement.IsLocked && this.IsValid))
        return this.InternalCanCreateInstance(insertionPoint);
      return false;
    }

    internal SceneNode CreateInstance(ILicenseFileManager licenseManager, ISceneInsertionPoint insertionPoint, Rect rect, OnCreateInstanceAction action)
    {
      if (LicenseManager.CurrentContext == null || !typeof (DesigntimeLicenseContext).IsAssignableFrom(LicenseManager.CurrentContext.GetType()))
        LicenseManager.CurrentContext = (LicenseContext) new DesigntimeLicenseContext();
      LicenseManager.LockContext(Asset.licenseManagerLock);
      try
      {
        SceneNode instance = this.InternalCreateInstance(insertionPoint, rect, action);
        if (instance != null && instance.IsViewObjectValid)
        {
          TypeAsset typeAsset = this as TypeAsset;
          if (typeAsset != null && typeAsset.IsLicensed && licenseManager != null)
          {
            string projectPath = instance.ProjectContext.ProjectPath;
            licenseManager.AddLicensedItem(projectPath, typeAsset.Type.FullName, typeAsset.Type.RuntimeAssembly.FullName);
          }
        }
        return instance;
      }
      finally
      {
        LicenseManager.UnlockContext(Asset.licenseManagerLock);
      }
    }

    protected abstract bool InternalCanCreateInstance(ISceneInsertionPoint insertionPoint);

    protected abstract SceneNode InternalCreateInstance(ISceneInsertionPoint insertionPoint, Rect rect, OnCreateInstanceAction action);

    protected virtual IEnumerable<ISceneInsertionPoint> InternalFindInsertionPoints(SceneViewModel viewModel)
    {
      return (IEnumerable<ISceneInsertionPoint>) null;
    }

    public abstract SceneNode CreatePrototypeInstance(ISceneInsertionPoint insertionPoint);

    protected abstract DrawingBrush CreateIconSourceBrush();

    internal void UpdateOnProject(IProject project, AssetTypeHelper typeHelper)
    {
      this.AssetInfo = this.CreateAssetInfoModel(project);
      this.OnPropertyChanged("AssetInfo");
      this.Categories = (IList<AssetCategoryPath>) new List<AssetCategoryPath>();
      this.ComputeCategories(this.Categories, typeHelper);
    }

    protected abstract void ComputeCategories(IList<AssetCategoryPath> categoryPaths, AssetTypeHelper typeHelper);

    protected abstract AssetInfoModel CreateAssetInfoModel(IProject project);

    private void DoDragDrop(UIElement sourceElement)
    {
      if (!this.isValid)
        return;
      ICommand startDragCommand = Asset.GetStartDragCommand((DependencyObject) sourceElement);
      if (startDragCommand != null)
        startDragCommand.Execute((object) this);
      int num = (int) DragSourceHelper.DoDragDrop(sourceElement, (object) this, DragDropEffects.All);
    }

    protected void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    public virtual int CompareTo(Asset other)
    {
      return this.GetType().GetHashCode().CompareTo(other.GetType().GetHashCode());
    }

    protected static int CompareITypes(IType thisType, IType otherType)
    {
      if (thisType == otherType)
        return 0;
      if (thisType == null)
        return -1;
      if (otherType == null)
        return 1;
      int num = string.CompareOrdinal(thisType.FullName, otherType.FullName);
      if (num != 0)
        return num;
      IUnreferencedType unreferencedType1 = thisType as IUnreferencedType;
      IUnreferencedType unreferencedType2 = otherType as IUnreferencedType;
      if (unreferencedType1 != null && unreferencedType2 != null)
        return string.Compare(unreferencedType1.AssemblyLocation, unreferencedType2.AssemblyLocation, StringComparison.OrdinalIgnoreCase);
      return 0;
    }

    protected static int ComparePlatforms(IPlatformMetadata thisPlatform, IPlatformMetadata otherPlatform)
    {
      if (thisPlatform == otherPlatform)
        return 0;
      if (thisPlatform == null)
        return -1;
      if (otherPlatform == null)
        return 1;
      return string.CompareOrdinal((string) thisPlatform.GetCapabilityValue(PlatformCapability.PlatformSimpleName), (string) otherPlatform.GetCapabilityValue(PlatformCapability.PlatformSimpleName));
    }

    private class AssetComparer : IComparer<Asset>
    {
      public int Compare(Asset assetA, Asset assetB)
      {
        if (assetA == assetB)
          return 0;
        if (assetA == null)
          return -1;
        if (assetB == null)
          return 1;
        return assetA.CompareTo(assetB);
      }
    }

    private class AssetAlphabeticComparer : IComparer<Asset>
    {
      public int Compare(Asset x, Asset y)
      {
        return string.Compare(x.Name, y.Name, StringComparison.CurrentCulture);
      }
    }
  }
}
