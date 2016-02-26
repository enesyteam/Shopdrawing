// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.BehaviorAsset
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  internal class BehaviorAsset : TypeAsset
  {
    public BehaviorAsset(IType type, string displayName, ExampleAssetInfo exampleInfo, string onDemandAssemblyFileName, AssemblyNameAndLocation[] resolvableAssemblyReferences)
      : base(type, displayName, exampleInfo, onDemandAssemblyFileName, resolvableAssemblyReferences)
    {
    }

    protected override IEnumerable<ISceneInsertionPoint> InternalFindInsertionPoints(SceneViewModel viewModel)
    {
      return Enumerable.Select<SceneElement, ISceneInsertionPoint>((IEnumerable<SceneElement>) viewModel.ElementSelectionSet.Selection, (Func<SceneElement, ISceneInsertionPoint>) (element => new BehaviorInsertionPointCreator(element).Create((object) this)));
    }

    protected override bool InternalCanCreateInstance(ISceneInsertionPoint insertionPoint)
    {
      if (insertionPoint.Property == null || !insertionPoint.Property.Equals((object) BehaviorHelper.BehaviorsProperty))
        return false;
      return insertionPoint.CanInsert((ITypeId) this.Type);
    }

    protected override SceneNode InternalCreateInstance(ISceneInsertionPoint insertionPoint, Rect rect, OnCreateInstanceAction action)
    {
      ProjectContext projectContext = ProjectContext.GetProjectContext(insertionPoint.SceneNode.ProjectContext);
      if (!this.EnsureTypeReferenced(projectContext))
        return (SceneNode) null;
      BehaviorHelper.EnsureSystemWindowsInteractivityReferenced((ITypeResolver) projectContext);
      insertionPoint.SceneNode.DesignerContext.ViewUpdateManager.RebuildPostponedViews();
      SceneViewModel viewModel = insertionPoint.SceneNode.ViewModel;
      using (SceneEditTransaction editTransaction = viewModel.CreateEditTransaction(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.UndoUnitCreateControlFormat, new object[1]
      {
        (object) this.Name
      })))
      {
        viewModel.BehaviorSelectionSet.Clear();
        BehaviorBaseNode selectionToSet = (BehaviorBaseNode) base.InternalCreateInstance(insertionPoint, rect, action);
        ISceneNodeCollection<SceneNode> collectionForProperty = selectionToSet.GetCollectionForProperty(BehaviorHelper.BehaviorTriggersProperty);
        foreach (IProperty property in selectionToSet.GetProperties())
        {
          if (PlatformTypes.ICommand.IsAssignableFrom((ITypeId) property.PropertyType))
          {
            object triggerAttribute = BehaviorHelper.CreateTriggerFromDefaultTriggerAttribute((IEnumerable) property.Attributes, insertionPoint.SceneNode.TargetType);
            if (triggerAttribute != null)
            {
              BehaviorTriggerBaseNode behaviorTriggerBaseNode = (BehaviorTriggerBaseNode) viewModel.CreateSceneNode(triggerAttribute);
              InvokeCommandActionNode commandActionNode = (InvokeCommandActionNode) viewModel.CreateSceneNode(ProjectNeutralTypes.InvokeCommandAction);
              commandActionNode.CommandName = property.Name;
              behaviorTriggerBaseNode.Actions.Add((SceneNode) commandActionNode);
              if (ProjectNeutralTypes.BehaviorEventTriggerBase.IsAssignableFrom((ITypeId) behaviorTriggerBaseNode.Type))
                BehaviorHelper.CreateAndSetElementNameBinding(BehaviorEventTriggerBaseNode.BehaviorSourceObjectProperty, (SceneNode) behaviorTriggerBaseNode, insertionPoint.SceneNode);
              collectionForProperty.Add((SceneNode) behaviorTriggerBaseNode);
            }
          }
        }
        viewModel.BehaviorSelectionSet.SetSelection(selectionToSet);
        editTransaction.Commit();
        return (SceneNode) selectionToSet;
      }
    }
  }
}
