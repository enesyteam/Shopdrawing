// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Triggers.NoneTriggerModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface.Triggers
{
  public class NoneTriggerModel : PropertyTriggerModel
  {
    private SceneNode activeEditingContainer;
    private SceneNodeSubscription<object, SetterModel> setterSubscription;
    private SelectionManager selectionManager;

    public override IList<object> Contents
    {
      get
      {
        IList<object> list = (IList<object>) new List<object>();
        list.Add((object) this.Setters);
        return list;
      }
    }

    public SceneNode ActiveEditingContainer
    {
      get
      {
        return this.activeEditingContainer;
      }
    }

    public SceneViewModel ViewModel
    {
      get
      {
        return this.activeEditingContainer.ViewModel;
      }
    }

    public override bool IsActive
    {
      get
      {
        return this.ViewModel.ActiveVisualTrigger == null;
      }
    }

    public bool CanContainSetters
    {
      get
      {
        return this.activeEditingContainer is StyleNode;
      }
    }

    public NoneTriggerModel(SceneNode activeEditingContainer)
      : base((BaseTriggerNode) null, (TriggerModelManager) null)
    {
      this.activeEditingContainer = activeEditingContainer;
    }

    public override void Initialize()
    {
      this.selectionManager = this.ViewModel.DesignerContext.SelectionManager;
      this.selectionManager.LateActiveSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.SelectionManager_LateActiveSceneUpdatePhase);
      this.setterSubscription = new SceneNodeSubscription<object, SetterModel>();
      this.setterSubscription.Path = new SearchPath(new SearchStep[1]
      {
        new SearchStep(SearchAxis.DocumentDescendant, (ISearchPredicate) new SceneNodeTypePredicate(typeof (SetterSceneNode)), (ISearchPredicate) new DelegatePredicate((Predicate<SceneNode>) (obj =>
        {
          if (!typeof (TriggerBase).IsAssignableFrom(obj.TargetType))
            return TriggerModelManager.IsNotStyleOrTemplate(obj);
          return false;
        }), SearchScope.NodeTreeSelf))
      });
      this.setterSubscription.SetPathNodeInsertedHandler(new SceneNodeSubscription<object, SetterModel>.PathNodeInsertedHandler(this.SetterSubscription_Inserted));
      this.setterSubscription.PathNodeRemoved += new SceneNodeSubscription<object, SetterModel>.PathNodeRemovedListener(this.SetterSubscription_Removed);
      this.setterSubscription.PathNodeContentChanged += new SceneNodeSubscription<object, SetterModel>.PathNodeContentChangedListener(this.SetterSubscription_ContentChanged);
      this.setterSubscription.SetBasisNodes(this.ViewModel, (IEnumerable<SceneNode>) new List<SceneNode>(1)
      {
        this.activeEditingContainer
      });
      this.SortAfterUpdate();
    }

    public override void Detach()
    {
      base.Detach();
      if (this.selectionManager != null)
      {
        this.selectionManager.LateActiveSceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.SelectionManager_LateActiveSceneUpdatePhase);
        this.selectionManager = (SelectionManager) null;
      }
      if (this.setterSubscription == null)
        return;
      this.setterSubscription.PathNodeRemoved -= new SceneNodeSubscription<object, SetterModel>.PathNodeRemovedListener(this.SetterSubscription_Removed);
      this.setterSubscription.PathNodeContentChanged -= new SceneNodeSubscription<object, SetterModel>.PathNodeContentChangedListener(this.SetterSubscription_ContentChanged);
      this.setterSubscription.CurrentViewModel = (SceneViewModel) null;
      this.setterSubscription = (SceneNodeSubscription<object, SetterModel>) null;
    }

    public override void Update()
    {
    }

    public override void Activate()
    {
      this.activeEditingContainer.ViewModel.SetActiveTrigger((TriggerBaseNode) null);
    }

    private void SelectionManager_LateActiveSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      this.setterSubscription.Update(args.ViewModel, args.DocumentChanges, args.DocumentChangeStamp);
      this.SortAfterUpdate();
    }

    private SetterModel SetterSubscription_Inserted(object sender, SceneNode basisNode, object basisContent, SceneNode newPathNode)
    {
      return this.AddSetter((SetterSceneNode) newPathNode);
    }

    private void SetterSubscription_Removed(object sender, SceneNode basisNode, object basisContent, SceneNode oldPathNode, SetterModel oldContent)
    {
      this.RemoveSetter(oldContent.Setter);
    }

    private void SetterSubscription_ContentChanged(object sender, SceneNode pathNode, SetterModel content, DocumentNodeMarker damageMarker, DocumentNodeChange damage)
    {
      content.Update();
    }
  }
}
