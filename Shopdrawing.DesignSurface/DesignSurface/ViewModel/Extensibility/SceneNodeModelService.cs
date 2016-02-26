// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.Extensibility.SceneNodeModelService
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.ViewModel.Extensibility
{
  public class SceneNodeModelService : ModelService
  {
    private SceneViewModel viewModel;

    public override ModelItem Root
    {
      get
      {
        if (this.viewModel.RootNode != null)
          return (ModelItem) this.viewModel.RootNode.ModelItem;
        return (ModelItem) this.viewModel.CreateSceneNode(PlatformTypes.UserControl).ModelItem;
      }
    }

    public override event EventHandler<ModelChangedEventArgs> ModelChanged;

    public SceneNodeModelService(SceneViewModel viewModel)
    {
      this.viewModel = viewModel;
    }

    internal void OnModelChanged(DocumentNodeChange change)
    {
      if (this.ModelChanged == null)
        return;
      this.ModelChanged((object) this, (ModelChangedEventArgs) new SceneNodeModelChangedEventArgs(this.viewModel, change));
    }

    protected override ModelItem CreateItem(object item)
    {
      return (ModelItem) this.viewModel.CreateSceneNode(item).ModelItem;
    }

    protected override ModelItem CreateItem(Type itemType, CreateOptions options, params object[] arguments)
    {
      ModelItem modelItem = (ModelItem) null;
      if (arguments == null || arguments.Length == 0)
      {
        modelItem = (ModelItem) this.viewModel.CreateSceneNode(itemType).ModelItem;
      }
      else
      {
        object obj = (object) null;
        try
        {
          obj = Activator.CreateInstance(itemType, arguments);
        }
        catch (Exception ex)
        {
        }
        if (obj != null)
          modelItem = this.CreateItem(obj);
      }
      if (modelItem != null && options == CreateOptions.InitializeDefaults)
      {
        foreach (DefaultInitializer defaultInitializer in Enumerable.Reverse<FeatureProvider>(FeatureExtensions.CreateFeatureProviders(this.viewModel.ExtensibilityManager.FeatureManager, typeof (DefaultInitializer), modelItem)))
          defaultInitializer.InitializeDefaults(modelItem);
      }
      return modelItem;
    }

    public override IEnumerable<ModelItem> Find(ModelItem startingItem, Predicate<Type> match)
    {
      ISceneNodeModelItem root = startingItem as ISceneNodeModelItem;
      if (root != null)
      {
        SearchPath path = new SearchPath(new SearchStep[1]
        {
          new SearchStep(SearchAxis.DocumentSelfAndDescendant, (ISearchPredicate) new DelegatePredicate((Predicate<SceneNode>) (node => match(node.TargetType)), SearchScope.NodeTreeSelf))
        });
        foreach (SceneNode sceneNode in path.Query(root.SceneNode))
          yield return (ModelItem) sceneNode.ModelItem;
      }
    }

    public override IEnumerable<ModelItem> Find(ModelItem startingItem, Type type)
    {
      return this.Find(startingItem, (Predicate<Type>) (targetType => targetType == type));
    }

    public override IEnumerable<ModelItem> Find(ModelItem startingItem, TypeIdentifier typeIdentifier)
    {
      Type runtimeType = this.ResolveType(typeIdentifier);
      return this.Find(startingItem, (Predicate<Type>) (targetType => targetType == runtimeType));
    }

    public override ModelItem FromName(ModelItem scope, string name, StringComparison comparison)
    {
      ISceneNodeModelItem sceneNodeModelItem = (scope ?? this.Root) as ISceneNodeModelItem;
      if (sceneNodeModelItem != null)
      {
        SceneNode sceneNode = new SearchPath(new SearchStep[1]
        {
          new SearchStep(SearchAxis.DocumentSelfAndDescendant, (ISearchPredicate) new DelegatePredicate((Predicate<SceneNode>) (node => string.Compare(name, node.Name, comparison) == 0), SearchScope.NodeTreeSelf))
        }).QueryFirst(sceneNodeModelItem.SceneNode);
        if (sceneNode != null)
          return (ModelItem) sceneNode.ModelItem;
      }
      return (ModelItem) null;
    }

    public override ModelItem ConvertItem(ModelItem item)
    {
      SceneNodeModelItem sceneNodeModelItem = item as SceneNodeModelItem;
      if (sceneNodeModelItem != null && sceneNodeModelItem.SceneNode.ViewModel == this.viewModel)
        return (ModelItem) sceneNodeModelItem;
      throw new InvalidOperationException();
    }

    protected override Type ResolveType(TypeIdentifier typeIdentifier)
    {
      return this.viewModel.ExtensibilityManager.ResolveType(typeIdentifier);
    }

    protected override ModelItem CreateStaticMemberItem(Type type, string memberName)
    {
      IType type1 = this.viewModel.ProjectContext.GetType(type);
      if (type1 != null)
      {
        MemberAccessTypes allowableMemberAccess = TypeHelper.GetAllowableMemberAccess((ITypeResolver) this.viewModel.ProjectContext, type1);
        IMember memberId = type1.GetMember(MemberType.LocalProperty | MemberType.Field, memberName, allowableMemberAccess) as IMember;
        if (memberId != null)
          return (ModelItem) this.viewModel.GetSceneNode((DocumentNode) DocumentNodeHelper.NewStaticNode(this.viewModel.Document.DocumentContext, memberId)).ModelItem;
      }
      return (ModelItem) null;
    }
  }
}
