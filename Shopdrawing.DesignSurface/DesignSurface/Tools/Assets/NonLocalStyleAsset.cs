// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.NonLocalStyleAsset
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  internal class NonLocalStyleAsset : StyleAsset
  {
    private ResourceDictionaryAssetProvider provider;
    private string name;

    public override string Name
    {
      get
      {
        this.name = base.Name;
        return this.name;
      }
    }

    private string CachedName
    {
      get
      {
        return this.name;
      }
    }

    public NonLocalStyleAsset(ResourceDictionaryAssetProvider provider, ResourceModel resourceModel)
      : base((StyleAssetProvider) provider, resourceModel)
    {
      this.provider = provider;
      this.name = resourceModel.Name;
    }

    protected override bool InternalCanCreateInstance(ISceneInsertionPoint insertionPoint)
    {
      if (!this.IsInsertionPointValid(insertionPoint))
        return false;
      SceneNode sceneNode = insertionPoint.SceneNode;
      DocumentNodePath documentNodePath = sceneNode.DocumentNodePath;
      DocumentNode nodeBeingLookedFor = new ExpressionEvaluator(sceneNode.ViewModel.DocumentRootResolver).EvaluateResource(documentNodePath, this.ResourceModel.KeyNode);
      return !documentNodePath.Contains(nodeBeingLookedFor);
    }

    protected override DefaultTypeInstantiator GetInstantiator(SceneView sceneView)
    {
      return (DefaultTypeInstantiator) new NonLocalStyleAssetInstantiator(sceneView, this.provider, this);
    }

    public override int CompareTo(Asset other)
    {
      NonLocalStyleAsset nonLocalStyleAsset = other as NonLocalStyleAsset;
      if (nonLocalStyleAsset == null)
        return base.CompareTo(other);
      int num = Asset.CompareITypes(this.StyleType, nonLocalStyleAsset.StyleType);
      if (num != 0)
        return num;
      return this.CachedName.CompareTo(nonLocalStyleAsset.CachedName);
    }
  }
}
