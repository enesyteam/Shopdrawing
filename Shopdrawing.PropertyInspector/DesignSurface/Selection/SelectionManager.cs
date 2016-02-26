// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Selection.SelectionManager
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.Text;
using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.Selection
{
  public class SelectionManager : IDisposable
  {
    private SceneViewModel viewModel;
    private DesignerContext designerContext;
    private uint elementSelectionSetChangeStamp;
    private uint dependencyObjectSelectionSetChangeStamp;
    private uint textSelectionSetChangeStamp;
    private uint gridRowSelectionSetChangeStamp;
    private uint gridColumnSelectionSetChangeStamp;
    private uint keyFrameSelectionSetChangeStamp;
    private uint animationSelectionSetChangeStamp;
    private uint storyboardSelectionSetChangeStamp;
    private uint behaviorSelectionSetChangeStamp;
    private uint annotationSelectionSetChangeStamp;
    private uint childPropertySelectionChangeStamp;
    private uint transitionSelectionChangeStamp;
    private bool isTemplateSelectedCache;
    private bool isEditingTriggerCache;
    private SceneNode[] selectedNodes;
    private SceneNode[] secondarySelectedNodes;

    public SceneElementSelectionSet ElementSelectionSet
    {
      get
      {
        if (this.viewModel == null)
          return (SceneElementSelectionSet) null;
        return this.viewModel.ElementSelectionSet;
      }
    }

    public DependencyObjectSelectionSet DependencyObjectSelectionSet
    {
      get
      {
        if (this.viewModel == null)
          return (DependencyObjectSelectionSet) null;
        return this.viewModel.DependencyObjectSelectionSet;
      }
    }

    public TextSelectionSet TextSelectionSet
    {
      get
      {
        if (this.viewModel == null)
          return (TextSelectionSet) null;
        return this.viewModel.TextSelectionSet;
      }
    }

    public PathPartSelectionSet PathPartSelectionSet
    {
      get
      {
        if (this.viewModel == null)
          return (PathPartSelectionSet) null;
        return this.viewModel.PathPartSelectionSet;
      }
    }

    public KeyFrameSelectionSet KeyFrameSelectionSet
    {
      get
      {
        if (this.viewModel == null)
          return (KeyFrameSelectionSet) null;
        return this.viewModel.KeyFrameSelectionSet;
      }
    }

    public AnimationSelectionSet AnimationSelectionSet
    {
      get
      {
        if (this.viewModel == null)
          return (AnimationSelectionSet) null;
        return this.viewModel.AnimationSelectionSet;
      }
    }

    public StoryboardSelectionSet StoryboardSelectionSet
    {
      get
      {
        if (this.viewModel == null)
          return (StoryboardSelectionSet) null;
        return this.viewModel.StoryboardSelectionSet;
      }
    }

    public GridColumnSelectionSet GridColumnSelectionSet
    {
      get
      {
        if (this.viewModel == null)
          return (GridColumnSelectionSet) null;
        return this.viewModel.GridColumnSelectionSet;
      }
    }

    public GridRowSelectionSet GridRowSelectionSet
    {
      get
      {
        if (this.viewModel == null)
          return (GridRowSelectionSet) null;
        return this.viewModel.GridRowSelectionSet;
      }
    }

    public PropertySelectionSet PropertySelectionSet
    {
      get
      {
        if (this.viewModel == null)
          return (PropertySelectionSet) null;
        return this.viewModel.PropertySelectionSet;
      }
    }

    public BehaviorSelectionSet BehaviorSelectionSet
    {
      get
      {
        if (this.viewModel == null)
          return (BehaviorSelectionSet) null;
        return this.viewModel.BehaviorSelectionSet;
      }
    }

    public AnnotationSelectionSet AnnotationSelectionSet
    {
      get
      {
        if (this.viewModel == null)
          return (AnnotationSelectionSet) null;
        return this.viewModel.AnnotationSelectionSet;
      }
    }

    public ChildPropertySelectionSet ChildPropertySelectionSet
    {
      get
      {
        if (this.viewModel == null)
          return (ChildPropertySelectionSet) null;
        return this.viewModel.ChildPropertySelectionSet;
      }
    }

    public SceneElementSubpartSelectionSet<SetterSceneNode> SetterSelectionSet
    {
      get
      {
        if (this.viewModel == null)
          return (SceneElementSubpartSelectionSet<SetterSceneNode>) null;
        return this.viewModel.SetterSelectionSet;
      }
    }

    public TransitionSelectionSet TransitionSelectionSet
    {
      get
      {
        if (this.viewModel == null)
          return (TransitionSelectionSet) null;
        return this.viewModel.TransitionSelectionSet;
      }
    }

    public IEnumerable<SceneNode> SecondarySelectedNodes
    {
      get
      {
        if (this.viewModel == null)
        {
          this.secondarySelectedNodes = (SceneNode[]) null;
          return (IEnumerable<SceneNode>) new List<SceneNode>();
        }
        if (this.transitionSelectionChangeStamp < this.TransitionSelectionSet.ChangeStamp || this.secondarySelectedNodes == null)
        {
          int length = 0;
          if (this.TransitionSelectionSet.Count != 0)
            length += this.TransitionSelectionSet.Count;
          this.secondarySelectedNodes = new SceneNode[length];
          int num = 0;
          foreach (VisualStateTransitionSceneNode transitionSceneNode in this.TransitionSelectionSet.Selection)
            this.secondarySelectedNodes[num++] = (SceneNode) transitionSceneNode;
          this.transitionSelectionChangeStamp = this.TransitionSelectionSet.ChangeStamp;
        }
        return (IEnumerable<SceneNode>) new List<SceneNode>((IEnumerable<SceneNode>) this.secondarySelectedNodes);
      }
    }

    public SceneNode[] SelectedNodes
    {
      get
      {
        if (this.viewModel == null)
        {
          this.selectedNodes = (SceneNode[]) null;
          return this.selectedNodes;
        }
        bool flag1 = this.ElementSelectionSet.Count == 1 && this.ElementSelectionSet.PrimarySelection is FrameworkTemplateElement;
        bool flag2 = this.viewModel.ActiveVisualTrigger != null && this.viewModel.AnimationEditor.IsRecording;
        if (this.elementSelectionSetChangeStamp < this.ElementSelectionSet.ChangeStamp || this.dependencyObjectSelectionSetChangeStamp < this.DependencyObjectSelectionSet.ChangeStamp || (this.textSelectionSetChangeStamp < this.TextSelectionSet.ChangeStamp || this.gridRowSelectionSetChangeStamp < this.GridRowSelectionSet.ChangeStamp) || (this.gridColumnSelectionSetChangeStamp < this.GridColumnSelectionSet.ChangeStamp || this.keyFrameSelectionSetChangeStamp < this.KeyFrameSelectionSet.ChangeStamp || (this.animationSelectionSetChangeStamp < this.AnimationSelectionSet.ChangeStamp || this.storyboardSelectionSetChangeStamp < this.StoryboardSelectionSet.ChangeStamp)) || (this.behaviorSelectionSetChangeStamp < this.BehaviorSelectionSet.ChangeStamp || this.annotationSelectionSetChangeStamp < this.AnnotationSelectionSet.ChangeStamp || (this.childPropertySelectionChangeStamp < this.ChildPropertySelectionSet.ChangeStamp || this.isEditingTriggerCache != flag2) || (this.isTemplateSelectedCache != flag1 || this.selectedNodes == null)))
        {
          TextSelectionSet textSelectionSet = this.TextSelectionSet;
          SceneElementSelectionSet elementSelectionSet = this.ElementSelectionSet;
          GridRowSelectionSet gridRowSelectionSet = this.GridRowSelectionSet;
          GridColumnSelectionSet columnSelectionSet = this.GridColumnSelectionSet;
          this.selectedNodes = (SceneNode[]) null;
          if (textSelectionSet != null && textSelectionSet.IsActive)
          {
            TextRangeElement elementFromProxy = TextRangeElement.CreateTextRangeElementFromProxy(textSelectionSet.TextEditProxy);
            this.selectedNodes = new SceneNode[1];
            this.selectedNodes[0] = (SceneNode) elementFromProxy;
          }
          else if (this.KeyFrameSelectionSet != null && this.KeyFrameSelectionSet.Count != 0)
          {
            this.selectedNodes = new SceneNode[this.KeyFrameSelectionSet.FilteredSelectionCount];
            int num = 0;
            foreach (KeyFrameSceneNode node in this.KeyFrameSelectionSet.Selection)
            {
              if (!this.KeyFrameSelectionSet.IsHiddenKeyFrame(node))
                this.selectedNodes[num++] = (SceneNode) node;
            }
          }
          else if (this.AnimationSelectionSet != null && this.AnimationSelectionSet.Count != 0)
          {
            this.selectedNodes = new SceneNode[this.AnimationSelectionSet.Count];
            int num = 0;
            foreach (SceneNode sceneNode in this.AnimationSelectionSet.Selection)
              this.selectedNodes[num++] = sceneNode;
          }
          else if (this.StoryboardSelectionSet != null && this.StoryboardSelectionSet.Count != 0)
          {
            this.selectedNodes = new SceneNode[this.StoryboardSelectionSet.Count];
            int num = 0;
            foreach (SceneNode sceneNode in this.StoryboardSelectionSet.Selection)
              this.selectedNodes[num++] = sceneNode;
          }
          else if (this.BehaviorSelectionSet != null && this.BehaviorSelectionSet.Count != 0)
          {
            this.selectedNodes = new SceneNode[this.BehaviorSelectionSet.Count];
            for (int index = 0; index < this.BehaviorSelectionSet.Count; ++index)
              this.selectedNodes[index] = (SceneNode) this.BehaviorSelectionSet.Selection[index];
          }
          else if (this.ChildPropertySelectionSet != null && this.ChildPropertySelectionSet.Count != 0)
          {
            this.selectedNodes = new SceneNode[this.ChildPropertySelectionSet.Count];
            int num = 0;
            foreach (SceneNode sceneNode in this.ChildPropertySelectionSet.Selection)
              this.selectedNodes[num++] = sceneNode;
          }
          else if (this.DependencyObjectSelectionSet != null && this.DependencyObjectSelectionSet.Count != 0)
          {
            this.selectedNodes = new SceneNode[this.DependencyObjectSelectionSet.Count];
            int num = 0;
            foreach (SceneNode sceneNode in this.DependencyObjectSelectionSet.Selection)
              this.selectedNodes[num++] = sceneNode;
          }
          else
          {
            int num = 0;
            int length = 0;
            if (elementSelectionSet != null)
              length += elementSelectionSet.Count;
            if (gridRowSelectionSet != null)
              length += gridRowSelectionSet.Count;
            if (columnSelectionSet != null)
              length += columnSelectionSet.Count;
            this.selectedNodes = new SceneNode[length];
            if (elementSelectionSet != null)
            {
              foreach (SceneElement sceneElement in elementSelectionSet.Selection)
                this.selectedNodes[num++] = (SceneNode) sceneElement;
            }
            if (gridRowSelectionSet != null)
            {
              foreach (SceneElement sceneElement in gridRowSelectionSet.Selection)
                this.selectedNodes[num++] = (SceneNode) sceneElement;
            }
            if (columnSelectionSet != null)
            {
              foreach (SceneElement sceneElement in columnSelectionSet.Selection)
                this.selectedNodes[num++] = (SceneNode) sceneElement;
            }
          }
          if (flag1 && flag2)
          {
            FrameworkTemplateElement frameworkTemplateElement = this.designerContext.ActiveSceneViewModel.ActiveVisualTrigger.TriggerContainer as FrameworkTemplateElement;
            if (frameworkTemplateElement != null)
            {
              SceneElement parentElement = frameworkTemplateElement.ParentElement;
              while (parentElement is StyleNode)
                parentElement = parentElement.ParentElement;
              if (parentElement != null)
                this.selectedNodes[0] = (SceneNode) parentElement;
            }
          }
          this.elementSelectionSetChangeStamp = this.ElementSelectionSet.ChangeStamp;
          this.dependencyObjectSelectionSetChangeStamp = this.DependencyObjectSelectionSet.ChangeStamp;
          this.textSelectionSetChangeStamp = this.TextSelectionSet.ChangeStamp;
          this.gridRowSelectionSetChangeStamp = this.GridRowSelectionSet.ChangeStamp;
          this.gridColumnSelectionSetChangeStamp = this.GridColumnSelectionSet.ChangeStamp;
          this.keyFrameSelectionSetChangeStamp = this.KeyFrameSelectionSet.ChangeStamp;
          this.animationSelectionSetChangeStamp = this.AnimationSelectionSet.ChangeStamp;
          this.storyboardSelectionSetChangeStamp = this.StoryboardSelectionSet.ChangeStamp;
          this.behaviorSelectionSetChangeStamp = this.BehaviorSelectionSet.ChangeStamp;
          this.annotationSelectionSetChangeStamp = this.AnnotationSelectionSet.ChangeStamp;
          this.childPropertySelectionChangeStamp = this.ChildPropertySelectionSet.ChangeStamp;
          this.isTemplateSelectedCache = flag1;
          this.isEditingTriggerCache = flag2;
        }
        return this.selectedNodes;
      }
    }

    public event SceneUpdatePhaseEventHandler EarlyActiveSceneUpdatePhase;

    public event SceneUpdatePhaseEventHandler LateActiveSceneUpdatePhase;

    public event EventHandler ActiveSceneSwitched;

    public event EventHandler ActiveSceneSwitching;

    public event EventHandler PostSceneUpdatePhase;

    internal SelectionManager(DesignerContext designerContext)
    {
      this.designerContext = designerContext;
      this.designerContext.ViewService.ActiveViewChanged += new ViewChangedEventHandler(this.ViewService_ActiveViewChanged);
      this.designerContext.ViewService.ActiveViewChanging += new ViewChangedEventHandler(this.ViewService_ActiveViewChanging);
      this.viewModel = designerContext.ActiveSceneViewModel;
    }

    public void Dispose()
    {
      this.designerContext.ViewService.ActiveViewChanged -= new ViewChangedEventHandler(this.ViewService_ActiveViewChanged);
    }

    internal void FireEarlyActiveSceneUpdatePhase(SceneUpdatePhaseEventArgs args)
    {
      if (this.EarlyActiveSceneUpdatePhase == null)
        return;
      args.Refresh(false, false);
      this.EarlyActiveSceneUpdatePhase((object) this, args);
    }

    internal void FireLateActiveSceneUpdatePhase(SceneUpdatePhaseEventArgs args)
    {
      if (this.LateActiveSceneUpdatePhase == null)
        return;
      args.Refresh(false, false);
      this.LateActiveSceneUpdatePhase((object) this, args);
    }

    internal void FirePostSceneUpdatePhase()
    {
      if (this.PostSceneUpdatePhase == null)
        return;
      this.PostSceneUpdatePhase((object) this, EventArgs.Empty);
    }

    private void ViewService_ActiveViewChanged(object sender, ViewChangedEventArgs e)
    {
      this.elementSelectionSetChangeStamp = 0U;
      this.textSelectionSetChangeStamp = 0U;
      this.selectedNodes = (SceneNode[]) null;
      this.secondarySelectedNodes = (SceneNode[]) null;
      SceneView sceneView = e.NewView as SceneView;
      this.viewModel = sceneView == null || !sceneView.IsDesignSurfaceVisible ? (SceneViewModel) null : sceneView.ViewModel;
      if (this.ActiveSceneSwitched != null)
        this.ActiveSceneSwitched((object) this, EventArgs.Empty);
      if (this.viewModel == null)
      {
        this.FireEarlyActiveSceneUpdatePhase(new SceneUpdatePhaseEventArgs((SceneViewModel) null, true, false));
        this.FireLateActiveSceneUpdatePhase(new SceneUpdatePhaseEventArgs((SceneViewModel) null, true, false));
      }
      else
        this.viewModel.SchedulePipelineTasks(true, false);
    }

    private void ViewService_ActiveViewChanging(object sender, ViewChangedEventArgs e)
    {
      if (this.ActiveSceneSwitching == null)
        return;
      this.ActiveSceneSwitching((object) this, EventArgs.Empty);
    }
  }
}
