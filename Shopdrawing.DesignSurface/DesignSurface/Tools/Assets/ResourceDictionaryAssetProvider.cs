// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.ResourceDictionaryAssetProvider
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Project;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  internal class ResourceDictionaryAssetProvider : StyleAssetProvider, IUserThemeProvider
  {
    private ResourceDictionaryContentProvider contentProvider;
    private bool isUpdating;

    public ResourceDictionaryContentProvider ContentProvider
    {
      get
      {
        return this.contentProvider;
      }
      set
      {
        if (this.contentProvider == value)
          return;
        if (this.contentProvider != null)
          this.contentProvider.ItemsChanged -= new CollectionChangeEventHandler(this.OnItemsChanged);
        this.contentProvider = value;
        if (this.contentProvider != null)
          this.contentProvider.ItemsChanged += new CollectionChangeEventHandler(this.OnItemsChanged);
        this.Assets.Clear();
        this.NeedsUpdate = true;
      }
    }

    public ResourceDictionaryUsage ResourceDictionaryUsage { get; private set; }

    protected virtual IEnumerable<DocumentNode> Items
    {
      get
      {
        if (this.contentProvider == null)
          return (IEnumerable<DocumentNode>) null;
        return this.contentProvider.Items;
      }
    }

    public IProjectItem ProjectItem
    {
      get
      {
        if (this.contentProvider == null)
          return (IProjectItem) null;
        return this.contentProvider.ProjectItem;
      }
    }

    string IUserThemeProvider.ThemeName
    {
      get
      {
        return this.GetThemeName();
      }
    }

    IEnumerable<Asset> IUserThemeProvider.ThemeAssets
    {
      get
      {
        return (IEnumerable<Asset>) this.Assets;
      }
    }

    public virtual bool IsCustomized
    {
      get
      {
        return false;
      }
    }

    public ResourceDictionaryAssetProvider(ResourceDictionaryContentProvider provider)
    {
      this.ContentProvider = provider;
    }

    protected ResourceDictionaryAssetProvider()
    {
    }

    protected override void InternalDispose(bool disposing)
    {
      base.InternalDispose(disposing);
      if (!disposing)
        return;
      this.ContentProvider = (ResourceDictionaryContentProvider) null;
    }

    public override ResourceAsset CreateAsset(ResourceModel ResourceModel)
    {
      return (ResourceAsset) new NonLocalStyleAsset(this, ResourceModel);
    }

    public virtual bool DoesProjectItemMatch(IProjectItem projectItem)
    {
      return this.ProjectItem == projectItem;
    }

    protected override bool UpdateAssets()
    {
      this.Assets.Clear();
      this.ResourceDictionaryUsage = AssetTypeHelper.GetResourceDictionaryUsage(this.contentProvider);
      foreach (DocumentNode documentNode in this.Items)
      {
        DocumentCompositeNode resourceNode = documentNode as DocumentCompositeNode;
        if (resourceNode != null)
        {
          ResourceModel resourceModel = new ResourceModel(resourceNode);
          if (this.IsResourceValid(resourceModel))
            this.Assets.Add((Asset) this.CreateAsset(resourceModel));
        }
      }
      this.Assets.Sort(Asset.DefaultComparer);
      if (!this.isUpdating)
        this.NotifyAssetsChanged();
      this.NeedsUpdate = false;
      return true;
    }

    private void OnItemsChanged(object sender, CollectionChangeEventArgs args)
    {
      this.NeedsUpdate = true;
    }

    protected virtual string GetThemeName()
    {
      if (this.ProjectItem == null || !(this.ProjectItem.DocumentReference != (DocumentReference) null))
        return (string) null;
      string str = this.ProjectItem.DocumentReference.DisplayNameShort;
      for (IProjectItem parent = this.ProjectItem.Parent; parent != null; parent = parent.Parent)
      {
        if (parent.IsDirectory)
          str = parent.DocumentReference.DisplayName + "\\" + str;
      }
      return str;
    }

    bool IUserThemeProvider.CanInsert(IProject project)
    {
      IXamlProject xamlProject = project as IXamlProject;
      if (xamlProject != null)
      {
        if (this.NeedsUpdate)
          this.Update();
        Asset asset = Enumerable.FirstOrDefault<Asset>((IEnumerable<Asset>) this.Assets);
        if (asset != null)
        {
          AssetTypeHelper assetTypeHelper = new AssetTypeHelper(xamlProject.ProjectContext, (IPrototypingService) null);
          if (asset.TargetType == null || assetTypeHelper.IsTypeSupported(asset.TargetType, true))
            return true;
        }
      }
      return false;
    }

    public virtual void ResetToDefault()
    {
    }
  }
}
