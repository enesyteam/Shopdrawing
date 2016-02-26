// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourcePane.TypedResourceItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.ResourcePane
{
  public abstract class TypedResourceItem : ResourceEntryItem
  {
    public ICommand EditCommand
    {
      get
      {
        DelegateCommand delegateCommand = new DelegateCommand((DelegateCommand.SimpleEventHandler) (() =>
        {
          IType type = this.Type;
          if (type != null && !PlatformTypes.DataTemplate.IsAssignableFrom((ITypeId) this.Resource.Type) && (!PlatformTypes.FrameworkElement.IsAssignableFrom((ITypeId) type) || !TypeUtilities.CanCreateTypeInXaml(this.Resource.ResourceNode.TypeResolver, type.RuntimeType) && this.Container.ViewModel.DefaultView.InstanceBuilderContext.ViewNodeManager.GetInstantiatableType((ITypeResolver) this.Container.ProjectContext, type.RuntimeType).Equals((object) type)))
            return;
          SceneViewModel viewModel = this.Container.ViewModel;
          if (!viewModel.XamlDocument.IsEditable)
            return;
          SceneNode sceneNode = viewModel.GetSceneNode(this.Resource.ValueNode);
          viewModel.SetViewRoot((SceneView) null, (SceneElement) null, (IPropertyId) null, sceneNode.DocumentNode, Size.Empty);
          viewModel.DefaultView.EnsureDesignSurfaceVisible();
          ProjectXamlContext.FromProjectContext(viewModel.Document.ProjectContext).OpenView(viewModel.DocumentRoot, true);
          SceneElement selectionToSet = sceneNode as SceneElement;
          if (selectionToSet == null)
            return;
          viewModel.ElementSelectionSet.SetSelection(selectionToSet);
        }));
        IType type1 = this.Type;
        delegateCommand.IsEnabled = type1 == null || PlatformTypes.DataTemplate.IsAssignableFrom((ITypeId) this.Resource.Type) || PlatformTypes.FrameworkElement.IsAssignableFrom((ITypeId) type1);
        return (ICommand) delegateCommand;
      }
    }

    protected virtual IPropertyId TargetTypeProperty
    {
      get
      {
        return (IPropertyId) null;
      }
    }

    public IType Type
    {
      get
      {
        if (this.TargetTypeProperty != null)
        {
          DocumentCompositeNode documentCompositeNode = this.Resource.ValueNode as DocumentCompositeNode;
          if (documentCompositeNode != null)
          {
            IProperty property = documentCompositeNode.TypeResolver.ResolveProperty(this.TargetTypeProperty);
            if (property != null)
            {
              DocumentPrimitiveNode documentPrimitiveNode = documentCompositeNode.Properties[(IPropertyId) property] as DocumentPrimitiveNode;
              if (documentPrimitiveNode != null)
                return DocumentPrimitiveNode.GetValueAsType((DocumentNode) documentPrimitiveNode);
            }
          }
        }
        return (IType) null;
      }
    }

    protected TypedResourceItem(ResourceManager resourceManager, ResourceContainer resourceContainer, ResourceModel resource)
      : base(resourceManager, resourceContainer, resource)
    {
    }

    protected string GetKeyOrFormattedTypeName(string typeName)
    {
      string name = this.Resource.Name;
      if (!string.IsNullOrEmpty(name) || this.Type == null)
        return name;
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ResourceItemTypedResource, new object[1]
      {
        (object) typeName
      });
    }
  }
}
