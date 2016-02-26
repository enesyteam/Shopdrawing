// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.AddItemCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.Tools.Assets;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal class AddItemCommand : SingleTargetCommandBase
  {
    private DefaultTypeInstantiator TypeInstantiator { get; set; }

    protected StyleAsset Asset
    {
      get
      {
        StyleAsset styleAsset1 = (StyleAsset) null;
        if (this.TargetElement != null)
        {
          DocumentNodePath valueAsDocumentNode = this.TargetElement.GetLocalValueAsDocumentNode(BaseFrameworkElement.StyleProperty);
          DocumentNode documentNode1 = valueAsDocumentNode != null ? valueAsDocumentNode.Node : (DocumentNode) null;
          if (documentNode1 != null && documentNode1.Parent != null && DictionaryEntryNode.ValueProperty.Equals((object) documentNode1.SitePropertyKey))
          {
            DocumentNode documentNode2 = (DocumentNode) documentNode1.Parent.Parent;
            if (documentNode2 != null && documentNode2.Parent == null && PlatformTypes.ResourceDictionary.IsAssignableFrom((ITypeId) documentNode2.Type))
            {
              foreach (ResourceDictionaryAssetProvider dictionaryAssetProvider in ((AssetLibrary) this.DesignerContext.AssetLibrary).FindAssetProviders<ResourceDictionaryAssetProvider>())
              {
                ResourceDictionaryContentProvider contentProvider = dictionaryAssetProvider.ContentProvider;
                if (contentProvider != null && contentProvider.Document == documentNode2.DocumentRoot && contentProvider.Document.RootNode == documentNode2)
                {
                  using (IEnumerator<Microsoft.Expression.DesignSurface.Tools.Assets.Asset> enumerator = dictionaryAssetProvider.Assets.GetEnumerator())
                  {
                    while (enumerator.MoveNext())
                    {
                      StyleAsset styleAsset2 = enumerator.Current as StyleAsset;
                      if (styleAsset2 != null && styleAsset2.StyleType.Equals((object) this.ItemType))
                      {
                        styleAsset1 = styleAsset2;
                        break;
                      }
                    }
                    break;
                  }
                }
              }
            }
          }
        }
        if (styleAsset1 == null)
          styleAsset1 = this.DesignerContext.AssetLibrary.FindActiveUserThemeAsset(this.ItemType);
        return styleAsset1;
      }
    }

    public override bool IsEnabled
    {
      get
      {
        if (!base.IsEnabled || !(this.TargetElement is ItemsControlElement))
          return false;
        IType type = this.ItemType as IType;
        if (type != null && type.Access == MemberAccessType.Public)
          return !type.Equals((object) PlatformTypes.ContentPresenter);
        return false;
      }
    }

    protected virtual ITypeId ItemType
    {
      get
      {
        ITypeId typeId = (ITypeId) null;
        ItemsControlElement itemsControlElement = this.TargetElement as ItemsControlElement;
        if (itemsControlElement != null)
          typeId = (ITypeId) itemsControlElement.ItemType;
        return typeId;
      }
    }

    private string UndoString
    {
      get
      {
        Microsoft.Expression.DesignSurface.Tools.Assets.Asset asset = (Microsoft.Expression.DesignSurface.Tools.Assets.Asset) this.Asset;
        if (asset != null)
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.UndoUnitAddItem, new object[1]
          {
            (object) asset.Name
          });
        if (this.ItemType == null)
          return StringTable.UndoUnitAddItemDefault;
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.UndoUnitAddItem, new object[1]
        {
          (object) this.ItemType.Name
        });
      }
    }

    public AddItemCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
      this.TypeInstantiator = new DefaultTypeInstantiator(viewModel.DefaultView);
    }

    public override void Execute()
    {
      using (SceneEditTransaction editTransaction = this.SceneViewModel.CreateEditTransaction(this.UndoString))
      {
        ItemsControlElement itemsControlElement = this.TargetElement as ItemsControlElement;
        if (itemsControlElement != null)
        {
          ITypeId itemType = this.ItemType;
          StyleAsset asset = this.Asset;
          IProperty targetProperty = this.SceneViewModel.ProjectContext.ResolveProperty(ItemsControlElement.ItemsProperty);
          ISceneInsertionPoint insertionPoint = (ISceneInsertionPoint) new PropertySceneInsertionPoint((SceneElement) itemsControlElement, targetProperty);
          if (asset != null && asset.CanCreateInstance(insertionPoint))
          {
            IExpandable expandable = this.TargetElement as IExpandable;
            if (expandable != null)
            {
              using (this.SceneViewModel.ForceBaseValue())
                this.TargetElement.SetValue(expandable.ExpansionProperty, (object) true);
            }
            asset.CreateInstance(this.DesignerContext.LicenseManager, insertionPoint, Rect.Empty, (OnCreateInstanceAction) null);
          }
          else
            this.TypeInstantiator.CreateInstance(itemType, insertionPoint, Rect.Empty, (OnCreateInstanceAction) null);
        }
        editTransaction.Commit();
      }
    }

    public override object GetProperty(string propertyName)
    {
      if (propertyName == "Text")
        return (object) this.UndoString;
      return base.GetProperty(propertyName);
    }
  }
}
