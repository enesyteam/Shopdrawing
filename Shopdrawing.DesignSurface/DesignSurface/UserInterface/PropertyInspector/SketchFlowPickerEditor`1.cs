// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SketchFlowPickerEditor`1
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  internal abstract class SketchFlowPickerEditor<TItemNode> : SketchFlowPickerEditor where TItemNode : SceneNode
  {
    private SceneNodeSubscription<TItemNode, TItemNode> subscription;

    protected override void Subscribe(SceneNodeProperty editingProperty)
    {
      this.editingProperty = editingProperty;
      this.viewModel = editingProperty.SceneNodeObjectSet.ViewModel;
      this.viewModel.LateSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.ViewModel_LateSceneUpdatePhase);
      this.subscription = this.CreateSubscription();
      this.subscription.PathNodeInserted += new SceneNodeSubscription<TItemNode, TItemNode>.PathNodeInsertedListener(((SketchFlowPickerEditor) this).Subscription_PathNodesChanged);
      this.subscription.PathNodeRemoved += new SceneNodeSubscription<TItemNode, TItemNode>.PathNodeRemovedListener(((SketchFlowPickerEditor) this).Subscription_PathNodesChanged);
      this.subscription.PathNodeContentChanged += new SceneNodeSubscription<TItemNode, TItemNode>.PathNodeContentChangedListener(((SketchFlowPickerEditor) this).Subscription_PathNodeContentChanged);
      this.subscription.SetSceneRootNodeAsTheBasisNode(editingProperty.SceneNodeObjectSet.ViewModel);
      ReferenceStep singleStep = (ReferenceStep) editingProperty.SceneNodeObjectSet.ProjectContext.ResolveProperty(this.TargetScreenProperty);
      this.targetScreenProperty = (SceneNodeProperty) editingProperty.SceneNodeObjectSet.CreateProperty(new PropertyReference(singleStep), (AttributeCollection) null);
      this.targetScreenProperty.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(((SketchFlowPickerEditor) this).TargetScreenProperty_PropertyReferenceChanged);
      this.editingProperty.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(((SketchFlowPickerEditor) this).EditingProperty_PropertyReferenceChanged);
    }

    protected override void Unsubscribe()
    {
      if (this.editingProperty != null)
        this.editingProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(((SketchFlowPickerEditor) this).EditingProperty_PropertyReferenceChanged);
      if (this.targetScreenProperty != null)
      {
        this.targetScreenProperty.OnRemoveFromCategory();
        this.targetScreenProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(((SketchFlowPickerEditor) this).TargetScreenProperty_PropertyReferenceChanged);
      }
      if (this.subscription != null)
      {
        this.subscription.PathNodeInserted -= new SceneNodeSubscription<TItemNode, TItemNode>.PathNodeInsertedListener(((SketchFlowPickerEditor) this).Subscription_PathNodesChanged);
        this.subscription.PathNodeRemoved -= new SceneNodeSubscription<TItemNode, TItemNode>.PathNodeRemovedListener(((SketchFlowPickerEditor) this).Subscription_PathNodesChanged);
        this.subscription.PathNodeContentChanged -= new SceneNodeSubscription<TItemNode, TItemNode>.PathNodeContentChangedListener(((SketchFlowPickerEditor) this).Subscription_PathNodeContentChanged);
        this.subscription.Path = (SearchPath) null;
      }
      if (this.viewModel != null)
        this.viewModel.LateSceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.ViewModel_LateSceneUpdatePhase);
      this.editingProperty = (SceneNodeProperty) null;
      this.viewModel = (SceneViewModel) null;
      this.subscription = (SceneNodeSubscription<TItemNode, TItemNode>) null;
      this.targetScreenProperty = (SceneNodeProperty) null;
    }

    private void ViewModel_LateSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      if (this.subscription == null)
        return;
      this.subscription.Update(args.ViewModel, args.DocumentChanges, args.DocumentChangeStamp);
    }

    protected abstract SceneNodeSubscription<TItemNode, TItemNode> CreateSubscription();
  }
}
