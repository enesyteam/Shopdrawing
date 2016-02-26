// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Properties.AnimationEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI;
using Microsoft.Expression.DesignSurface.UserInterface.Triggers;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.UserInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.Properties
{
  public sealed class AnimationEditor : IDisposable
  {
    private Collection<StoryboardTimelineSceneNode> storyboardNodes = new Collection<StoryboardTimelineSceneNode>();
    private AnimationSelectionSet animationSelectionSet;
    private KeyFrameSelectionSet keyFrameSelectionSet;
    private int deferKeyFrameCount;
    private double seekedTime;
    private bool isRecording;
    private AutoDialogType pendingAutoDialogType;
    private SceneViewModel viewModel;
    private SceneNodeSubscription<object, object> storyboardsSubscription;
    private IViewStoryboard cachedActiveViewStoryboard;
    private bool isActiveStoryboardMediaDirty;

    public double AnimationTime
    {
      get
      {
        return this.seekedTime;
      }
    }

    public bool CanAnimateLayout
    {
      get
      {
        VisualStateSceneNode stateEditTarget = this.viewModel.EditContextManager.StateEditTarget;
        if (stateEditTarget == null)
          return false;
        VisualStateGroupSceneNode stateGroupSceneNode = stateEditTarget.Parent as VisualStateGroupSceneNode;
        IProperty property = this.viewModel.ProjectContext.ResolveProperty(VisualStateManagerSceneNode.UseFluidLayoutProperty);
        if (property == null)
          return false;
        return (bool) stateGroupSceneNode.GetLocalOrDefaultValue((IPropertyId) property);
      }
    }

    public bool CanKeyFrame
    {
      get
      {
        return this.ActiveStoryboardTimeline != null;
      }
    }

    public bool IsKeyFraming
    {
      get
      {
        if (this.CanKeyFrame)
          return this.deferKeyFrameCount == 0;
        return false;
      }
    }

    public bool IsRecording
    {
      get
      {
        return this.isRecording;
      }
      set
      {
        if (this.isRecording == value)
          return;
        this.isRecording = value;
        if (this.RecordModeChanged != null)
          this.RecordModeChanged((object) this, EventArgs.Empty);
        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (Delegate) (arg =>
        {
          if (this.viewModel != null)
            this.viewModel.RefreshSelection();
          return (object) null;
        }), (object) null);
      }
    }

    public bool CanRecord
    {
      get
      {
        if (this.ActiveStoryboardTimeline == null)
          return this.ActiveVisualTrigger != null;
        return true;
      }
    }

    public IStoryboardContainer ActiveStoryboardContainer
    {
      get
      {
        return this.viewModel.ActiveStoryboardContainer;
      }
    }

    public StoryboardTimelineSceneNode ActiveStoryboardTimeline
    {
      get
      {
        return this.viewModel.ActiveStoryboardTimeline;
      }
    }

    public IViewStoryboard ActiveViewStoryboard
    {
      get
      {
        if (this.cachedActiveViewStoryboard != null)
          return this.cachedActiveViewStoryboard;
        return this.CreateViewStoryboard();
      }
    }

    public bool IsActiveStoryboardMediaDirty
    {
      get
      {
        return this.isActiveStoryboardMediaDirty;
      }
    }

    public TriggerBaseNode ActiveVisualTrigger
    {
      get
      {
        return this.viewModel.ActiveVisualTrigger;
      }
    }

    private SceneDocument SceneDocument
    {
      get
      {
        return this.viewModel.Document;
      }
    }

    public StoryboardTimelineSceneNode FirstActiveStoryboard
    {
      get
      {
        return this.GetFirstActiveStoryboard(true);
      }
    }

    public IViewObject ActiveStoryboardTarget
    {
      get
      {
        SceneElement sceneElement = this.ActiveStoryboardContainer as SceneElement;
        if (sceneElement == null)
          return (IViewObject) null;
        return sceneElement.ViewTargetElement;
      }
    }

    public event EventHandler RecordModeChanged;

    internal AnimationEditor(KeyFrameSelectionSet keyFrameSelectionSet, AnimationSelectionSet animationSelectionSet, SceneViewModel viewModel)
    {
      this.viewModel = viewModel;
      this.keyFrameSelectionSet = keyFrameSelectionSet;
      this.animationSelectionSet = animationSelectionSet;
      this.storyboardsSubscription = new SceneNodeSubscription<object, object>();
      this.storyboardsSubscription.Path = new SearchPath(new SearchStep[1]
      {
        new SearchStep(SearchAxis.DocumentDescendant, (ISearchPredicate) new SceneNodeTypePredicate(typeof (StoryboardTimelineSceneNode)), (ISearchPredicate) null)
      });
      this.storyboardsSubscription.SetPathNodeInsertedHandler(new SceneNodeSubscription<object, object>.PathNodeInsertedHandler(this.StoryboardsSubscription_TimelineInserted));
      this.storyboardsSubscription.PathNodeRemoved += new SceneNodeSubscription<object, object>.PathNodeRemovedListener(this.StoryboardsSubscription_TimelineRemoved);
      if (this.viewModel == null)
        return;
      this.storyboardsSubscription.SetSceneRootNodeAsTheBasisNode(this.viewModel);
    }

    public void Dispose()
    {
      if (this.viewModel != null)
      {
        if (this.cachedActiveViewStoryboard != null)
          this.cachedActiveViewStoryboard.CurrentTimeChanged -= new EventHandler<EventArgs>(this.ActiveTimeline_Seeked);
        this.viewModel = (SceneViewModel) null;
      }
      if (this.storyboardsSubscription.CurrentViewModel != null)
        this.storyboardsSubscription.CurrentViewModel = (SceneViewModel) null;
      this.storyboardsSubscription = (SceneNodeSubscription<object, object>) null;
      this.storyboardNodes = (Collection<StoryboardTimelineSceneNode>) null;
    }

    private IViewStoryboard CreateViewStoryboard()
    {
      if (this.ActiveStoryboardTimeline != null)
        return this.ActiveStoryboardTimeline.CreateViewStoryboard();
      return (IViewStoryboard) null;
    }

    public void UpdateStoryboardList(DocumentNodeChangeList damage)
    {
      if (this.viewModel == null)
        return;
      this.storyboardsSubscription.SetSceneRootNodeAsTheBasisNode(this.viewModel);
      this.storyboardsSubscription.Update(this.viewModel, damage, this.viewModel.XamlDocument.ChangeStamp);
    }

    public StoryboardTimelineSceneNode GetFirstActiveStoryboard(bool allowInlineStoryboards)
    {
      foreach (StoryboardTimelineSceneNode timelineSceneNode in this.storyboardNodes)
      {
        if (timelineSceneNode.StoryboardContainer == this.ActiveStoryboardContainer && (allowInlineStoryboards || timelineSceneNode.IsInResourceDictionary))
          return timelineSceneNode;
      }
      return (StoryboardTimelineSceneNode) null;
    }

    public IEnumerable<StoryboardTimelineSceneNode> EnumerateStoryboardsForContainer(IStoryboardContainer container)
    {
      return (IEnumerable<StoryboardTimelineSceneNode>) new AnimationEditor.ChildStoryboardEnumerable(this, container);
    }

    public StoryboardTimelineSceneNode FindStoryboardInContainer(IStoryboardContainer container, string nameToFind)
    {
      if (container == null || nameToFind == null)
        return (StoryboardTimelineSceneNode) null;
      foreach (StoryboardTimelineSceneNode timelineSceneNode in this.EnumerateStoryboardsForContainer(container))
      {
        if (timelineSceneNode.Name == nameToFind)
          return timelineSceneNode;
      }
      return (StoryboardTimelineSceneNode) null;
    }

    private bool GetIsCurrentTimeValid()
    {
      if (this.ActiveViewStoryboard == null || this.ActiveStoryboardTimeline == null)
        return true;
      if (this.AnimationTime >= this.ActiveStoryboardTimeline.PlayDuration)
        return this.ActiveViewStoryboard.CurrentState == ViewStoryboardState.Filling;
      double currentTime = this.ActiveViewStoryboard.CurrentTime;
      if (!double.IsNaN(currentTime))
        return currentTime == this.AnimationTime;
      return false;
    }

    public void SeekTo(double seconds)
    {
      if (this.ActiveViewStoryboard != null)
      {
        if (this.ActiveViewStoryboard == this.cachedActiveViewStoryboard)
        {
          try
          {
            this.ActiveViewStoryboard.Seek(seconds);
          }
          catch (Exception ex)
          {
            SceneView defaultView = this.viewModel.DefaultView;
            if (defaultView != null)
            {
              defaultView.SetViewException(ex);
              defaultView.ViewModel.RefreshCurrentValues();
              ExceptionHandler.SafelyForceLayoutArrange();
            }
          }
        }
      }
      this.seekedTime = seconds;
    }

    public string GenerateNewTimelineName(IStoryboardContainer storyboardContainer)
    {
      return this.GenerateNewTimelineName(storyboardContainer, "Storyboard");
    }

    public string GenerateNewTimelineName(IStoryboardContainer storyboardContainer, string suggestion)
    {
      SceneNodeIDHelper sceneNodeIdHelper = new SceneNodeIDHelper(this.viewModel, (SceneNode) storyboardContainer);
      List<string> list = new List<string>();
      foreach (StoryboardTimelineSceneNode timelineSceneNode in this.EnumerateStoryboardsForContainer(storyboardContainer))
        list.Add(timelineSceneNode.Name);
      suggestion = SceneNodeIDHelper.ToCSharpID(suggestion);
      int length = 512 - int.MaxValue.ToString((IFormatProvider) CultureInfo.InvariantCulture).Length;
      if (suggestion.Length > length)
        suggestion = suggestion.Substring(0, length);
      int num = 1;
      string candidateID;
      do
      {
        candidateID = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", new object[2]
        {
          (object) suggestion,
          (object) num
        });
        ++num;
      }
      while (list.Contains(candidateID) || !sceneNodeIdHelper.IsValidElementID((SceneNode) null, candidateID));
      return candidateID;
    }

    public StoryboardTimelineSceneNode CreateNewTimeline(TriggerCreateBehavior createTrigger)
    {
      return this.CreateNewTimeline(this.ActiveStoryboardContainer, this.GenerateNewTimelineName(this.ActiveStoryboardContainer), createTrigger, true);
    }

    public StoryboardTimelineSceneNode CreateNewTimeline(IStoryboardContainer storyboardContainer, string timelineName)
    {
      return this.CreateNewTimeline(storyboardContainer, timelineName, TriggerCreateBehavior.DoNotCreate, true);
    }

    public StoryboardTimelineSceneNode CreateNewTimeline(IStoryboardContainer storyboardContainer, string timelineName, TriggerCreateBehavior createTrigger, bool createAsResource)
    {
      bool flag;
      switch (createTrigger)
      {
        case TriggerCreateBehavior.Create:
          flag = true;
          break;
        case TriggerCreateBehavior.DoNotCreate:
          flag = false;
          break;
        case TriggerCreateBehavior.Default:
          SceneNode sceneNode = this.ActiveStoryboardContainer as SceneNode;
          flag = sceneNode != null && PlatformTypes.FrameworkElement.IsAssignableFrom((ITypeId) sceneNode.Type);
          break;
        default:
          flag = false;
          break;
      }
      StoryboardTimelineSceneNode timelineSceneNode1 = (StoryboardTimelineSceneNode) null;
      SceneNode root = (SceneNode) storyboardContainer;
      using (SceneEditTransaction editTransaction = this.viewModel.CreateEditTransaction(StringTable.UndoUnitCreateTimeline))
      {
        timelineSceneNode1 = StoryboardTimelineSceneNode.Factory.Instantiate(root.ViewModel);
        if (createAsResource)
        {
          foreach (StoryboardTimelineSceneNode timelineSceneNode2 in this.EnumerateStoryboardsForContainer(storyboardContainer))
          {
            if (timelineSceneNode2.Name == timelineName)
              timelineName = this.GenerateNewTimelineName(storyboardContainer, timelineName);
          }
        }
        if (!createAsResource || this.viewModel.ProjectContext.IsCapabilitySet(PlatformCapability.NameSupportedAsKey))
          timelineName = new SceneNodeIDHelper(this.viewModel, root).GetValidElementID((SceneNode) timelineSceneNode1, timelineName);
        if (createAsResource)
          this.AddResourceStoryboard(storyboardContainer, timelineName, timelineSceneNode1);
        if (timelineSceneNode1 != null && flag)
          TriggersHelper.CreateDefaultTrigger(timelineSceneNode1);
        if (!createAsResource)
          timelineSceneNode1.Name = timelineName;
        this.SetActiveStoryboardTimeline(storyboardContainer, timelineSceneNode1);
        editTransaction.Commit();
      }
      return timelineSceneNode1;
    }

    public void AddResourceStoryboard(string storyboardName, StoryboardTimelineSceneNode storyboard)
    {
      this.AddResourceStoryboard(this.ActiveStoryboardContainer, storyboardName, storyboard);
    }

    private void AddResourceStoryboard(IStoryboardContainer storyboardContainer, string storyboardName, StoryboardTimelineSceneNode storyboard)
    {
      storyboardContainer.AddResourceStoryboard(storyboardName, storyboard);
      if (!this.viewModel.ProjectContext.IsCapabilitySet(PlatformCapability.NameSupportedAsKey))
        return;
      storyboard.DocumentNode.Name = storyboardName;
      storyboard.DocumentNode.Parent.ClearValue(DictionaryEntryNode.KeyProperty);
    }

    public string RenameStoryboardWithValidation(StoryboardTimelineSceneNode storyboard, string newName)
    {
      CreateResourceModel createResourceModel = new CreateResourceModel(storyboard.ViewModel, storyboard.ViewModel.DesignerContext.ResourceManager, typeof (Storyboard), (Type) null, (string) null, storyboard.ViewModel.ActiveEditingContainer as SceneElement, storyboard.ViewModel.ActiveEditingContainer, CreateResourceModel.ContextFlags.CanOnlyDefineKey);
      createResourceModel.CreateAsResource = storyboard.Parent is DictionaryEntryNode;
      if (storyboard.ProjectContext.IsCapabilitySet(PlatformCapability.NameSupportedAsKey))
      {
        createResourceModel.ValidateKeyAsName = true;
        createResourceModel.NamedElement = (SceneNode) storyboard;
        createResourceModel.NameScope = storyboard.DocumentNode.FindNameScopeForChildren();
      }
      createResourceModel.KeyString = newName;
      if (storyboard.Name != createResourceModel.KeyString)
      {
        if (!createResourceModel.KeyStringHasIssues)
          return this.RenameTimeline(storyboard.ViewModel.AnimationEditor.ActiveStoryboardContainer, storyboard, createResourceModel.KeyString);
        storyboard.DesignerContext.MessageDisplayService.ShowError(createResourceModel.KeyStringWarningText);
      }
      return newName;
    }

    public string RenameTimeline(IStoryboardContainer storyboardContainer, StoryboardTimelineSceneNode timeline, string timelineName)
    {
      List<TimelineActionNode> list = new List<TimelineActionNode>();
      foreach (TriggerBaseNode triggerBaseNode in (IEnumerable<TriggerBaseNode>) storyboardContainer.VisualTriggers)
      {
        foreach (SceneNode sceneNode in triggerBaseNode.GetActions())
        {
          TimelineActionNode timelineActionNode = sceneNode as TimelineActionNode;
          if (timelineActionNode != null && timelineActionNode.TargetTimeline == timeline)
            list.Add(timelineActionNode);
        }
      }
      using (SceneEditTransaction editTransaction = this.viewModel.CreateEditTransaction(StringTable.UndoUnitRenameStoryboard))
      {
        timeline.Name = timelineName;
        foreach (TimelineActionNode timelineActionNode in list)
        {
          if (timeline.IsInResourceDictionary)
            timelineActionNode.TargetTimeline = timeline;
          timelineActionNode.Name = (string) null;
        }
        timeline.UpdateActionNames();
        editTransaction.Commit();
      }
      return timelineName;
    }

    public void DeleteTimeline(StoryboardTimelineSceneNode timeline)
    {
      IStoryboardContainer storyboardContainer = timeline.StoryboardContainer;
      using (SceneEditTransaction editTransaction = this.viewModel.CreateEditTransaction(StringTable.UndoUnitDeleteTimeline))
      {
        if (storyboardContainer == this.ActiveStoryboardContainer && timeline == this.ActiveStoryboardTimeline)
        {
          if (timeline.ControllingState != null || timeline.ControllingTransition != null && timeline.Children != null)
          {
            timeline.Children.Clear();
            timeline.ShouldSerialize = false;
          }
          else
            this.SetActiveStoryboardTimeline(storyboardContainer, (StoryboardTimelineSceneNode) null);
        }
        List<TimelineActionNode> list = new List<TimelineActionNode>();
        foreach (TimelineActionNode timelineActionNode in timeline.ControllingActions)
          list.Add(timelineActionNode);
        foreach (TimelineActionNode timelineActionNode in list)
        {
          EventTriggerNode eventTriggerNode = timelineActionNode.Parent as EventTriggerNode;
          if (eventTriggerNode != null && eventTriggerNode.Actions.Count == 1)
            eventTriggerNode.Remove();
          else
            timelineActionNode.Remove();
        }
        if (timeline.IsInResourceDictionary && storyboardContainer != null)
          storyboardContainer.RemoveResourceStoryboard(timeline);
        editTransaction.Commit();
      }
    }

    public void SetActiveStoryboardTimeline(IStoryboardContainer storyboardContainer, StoryboardTimelineSceneNode timeline)
    {
      this.viewModel.SetActiveStoryboardTimeline(storyboardContainer, timeline, (TriggerBaseNode) null);
    }

    internal PropertyReference GetAnimationProperty(SceneNode node, PropertyReference propertyReference)
    {
      if (node.Type.Metadata.NameProperty == propertyReference.LastStep)
        return (PropertyReference) null;
      ReferenceStep property = propertyReference[propertyReference.Count - 1];
      Type targetType = property.TargetType;
      if (propertyReference.Count > 1)
      {
        object computedValue = node.GetComputedValue(propertyReference.Subreference(0, propertyReference.Count - 2));
        if (computedValue != null)
          targetType = computedValue.GetType();
      }
      if (!TypeHelper.IsPropertyWritable((ITypeResolver) this.viewModel.ProjectContext, (IProperty) property, node.IsSubclassDefinition))
        return (PropertyReference) null;
      if (!this.IsPropertyAnimatable(targetType, property))
        return (PropertyReference) null;
      if (PlatformTypes.IsExpressionInteractiveType(PlatformTypeHelper.GetDeclaringType((IMember) property)))
        return (PropertyReference) null;
      if (KeyFrameAnimationSceneNode.CanAnimateType(propertyReference.ValueTypeId, node.ProjectContext))
        return propertyReference;
      return (PropertyReference) null;
    }

    private bool IsPropertyAnimatable(Type targetType, ReferenceStep property)
    {
      DependencyPropertyReferenceStep propertyReferenceStep = property as DependencyPropertyReferenceStep;
      return propertyReferenceStep != null && JoltHelper.TypeAnimationSupported(this.viewModel.ProjectContext, PlatformTypeHelper.GetPropertyType((IProperty) propertyReferenceStep)) && (!propertyReferenceStep.IsAnimationProhibited(targetType) && !BaseFrameworkElement.DataContextProperty.Equals((object) propertyReferenceStep)) && (this.viewModel.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf) || !property.DeclaringTypeId.Equals((object) PlatformTypes.ToolTipService) || !(property.Name == "ToolTip"));
    }

    public bool IsCurrentlyAnimated(SceneNode component, PropertyReference propertyReference, int maxDepth)
    {
      if (this.ActiveStoryboardTimeline == null)
        return false;
      maxDepth = Math.Min(maxDepth, propertyReference.Count);
      StoryboardTimelineSceneNode storyboardTimeline = this.ActiveStoryboardTimeline;
      for (int index1 = 0; index1 < storyboardTimeline.Children.Count; ++index1)
      {
        AnimationSceneNode animationSceneNode = storyboardTimeline.Children[index1] as AnimationSceneNode;
        if (animationSceneNode != null)
        {
          TimelineSceneNode.PropertyNodePair elementAndProperty = animationSceneNode.TargetElementAndProperty;
          if (elementAndProperty.SceneNode == component && elementAndProperty.PropertyReference != null && elementAndProperty.PropertyReference.Count >= maxDepth)
          {
            bool flag = true;
            for (int index2 = 0; index2 < maxDepth; ++index2)
            {
              if (elementAndProperty.PropertyReference[index2] != propertyReference[index2])
              {
                flag = false;
                break;
              }
            }
            if (flag)
              return true;
          }
        }
      }
      return false;
    }

    public IDisposable DeferKeyFraming()
    {
      return (IDisposable) new AnimationEditor.DeferKeyFramingToken(this);
    }

    public void SetValue(SceneElement element, PropertyReference propertyReference, object value)
    {
      this.SetValue(element, propertyReference, value, this.AnimationTime);
    }

    public void SetValue(SceneElement element, PropertyReference propertyReference, object value, double time)
    {
      IViewObject viewObject = element.ViewTargetElement;
      if (viewObject.PlatformSpecificObject is Viewport3D)
        viewObject = element.ViewObject;
      if (this.IsKeyFraming)
      {
        bool flag = false;
        AnimationSceneNode animation = this.ActiveStoryboardTimeline.GetAnimation((SceneNode) element, propertyReference);
        if (animation != null)
        {
          if (animation is PathAnimationSceneNode)
            return;
          if (animation is KeyFrameAnimationSceneNode)
            this.DeleteAnimationsInTimeline(this.ActiveStoryboardTimeline, element, propertyReference, animation);
          else if (animation is FromToAnimationSceneNode)
          {
            this.DeleteAnimationsInTimeline(this.ActiveStoryboardTimeline, element, propertyReference, animation);
            flag = true;
          }
          else
            this.DeleteAnimationsInTimeline(this.ActiveStoryboardTimeline, element, propertyReference, (AnimationSceneNode) null);
        }
        PropertyReference propertyReference1 = DesignTimeProperties.GetAppliedShadowPropertyReference(propertyReference, (ITypeId) element.Type);
        if (value == DependencyProperty.UnsetValue)
        {
          object baseValue = viewObject.GetBaseValue(propertyReference1);
          if (baseValue is double && (double.IsNaN((double) baseValue) || double.IsInfinity((double) baseValue)) && !this.CanAnimateLayout)
            this.PostAutoErrorDialog(AutoDialogType.BaseValue);
          else if (!flag)
            this.AddPropertyAutoKeyframe(element, propertyReference, time, baseValue, baseValue);
          else
            this.SetFromOrToValue(animation as FromToAnimationSceneNode, time, (object) null);
        }
        else
        {
          if (viewObject == null)
            return;
          object oldValue = viewObject.GetCurrentValue(propertyReference1);
          if (oldValue is double && value is double)
          {
            if ((double.IsNaN((double) oldValue) || double.IsInfinity((double) oldValue)) && !this.CanAnimateLayout)
            {
              this.PostAutoErrorDialog(AutoDialogType.BaseValue);
              return;
            }
            if ((double.IsNaN((double) value) || double.IsInfinity((double) value)) && !this.CanAnimateLayout)
            {
              this.PostAutoErrorDialog(AutoDialogType.KeyFrame);
              return;
            }
            oldValue = (object) JoltHelper.RoundDouble(element.ProjectContext, (double) oldValue);
          }
          if (!flag)
            this.AddPropertyAutoKeyframe(element, propertyReference, time, oldValue, value);
          else
            this.SetFromOrToValue(animation as FromToAnimationSceneNode, time, value);
        }
      }
      else
      {
        this.DeleteAllAnimations((SceneNode) element, propertyReference.Path);
        using (this.DeferKeyFraming())
        {
          if (value == DependencyProperty.UnsetValue)
            element.ClearValue(propertyReference);
          else
            element.SetValue(propertyReference, value);
        }
      }
    }

    public void PostAutoErrorDialog(AutoDialogType type)
    {
      if (this.pendingAutoDialogType != AutoDialogType.None || type == AutoDialogType.None)
        return;
      this.pendingAutoDialogType = type;
      if (this.SceneDocument.HasOpenTransaction)
      {
        this.SceneDocument.EditTransactionCompleted += new EventHandler(this.SceneDocument_EditTransactionEnd);
        this.SceneDocument.EditTransactionCanceled += new EventHandler(this.SceneDocument_EditTransactionEnd);
      }
      else
        this.ShowPendingAutoDialog();
    }

    private void ShowPendingAutoDialog()
    {
      switch (this.pendingAutoDialogType)
      {
        case AutoDialogType.KeyFrame:
          Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, (Delegate) (obj =>
          {
            this.viewModel.DesignerContext.MessageDisplayService.ShowError(StringTable.NanKeyFrameError);
            return (object) null;
          }), (object) null);
          break;
        case AutoDialogType.BaseValue:
          Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, (Delegate) (obj =>
          {
            this.viewModel.DesignerContext.MessageDisplayService.ShowError(StringTable.NanBaseValueAnimationError);
            return (object) null;
          }), (object) null);
          break;
      }
      this.pendingAutoDialogType = AutoDialogType.None;
    }

    private void SceneDocument_EditTransactionEnd(object sender, EventArgs e)
    {
      SceneDocument sceneDocument = sender as SceneDocument;
      if (sceneDocument == null)
        return;
      this.ShowPendingAutoDialog();
      sceneDocument.EditTransactionCompleted -= new EventHandler(this.SceneDocument_EditTransactionEnd);
      sceneDocument.EditTransactionCanceled -= new EventHandler(this.SceneDocument_EditTransactionEnd);
    }

    public void RecordAutoKeyframe(SceneElement element, double time)
    {
      bool flag = false;
      using (SceneEditTransaction editTransaction = this.viewModel.CreateEditTransaction(StringTable.UndoUnitRecordAutoKeyFrame))
      {
        ArrayList arrayList = new ArrayList();
        foreach (TimelineSceneNode timelineSceneNode in (IEnumerable<TimelineSceneNode>) this.ActiveStoryboardTimeline.Children)
        {
          KeyFrameAnimationSceneNode animationSceneNode = timelineSceneNode as KeyFrameAnimationSceneNode;
          if (animationSceneNode != null)
          {
            TimelineSceneNode.PropertyNodePair elementAndProperty = animationSceneNode.TargetElementAndProperty;
            if (elementAndProperty.SceneNode == element && elementAndProperty.PropertyReference != null && !DesignTimeProperties.ExplicitAnimationProperty.Equals((object) elementAndProperty.PropertyReference[0]))
            {
              object obj = element.GetComputedValue(animationSceneNode.TargetProperty);
              if (obj is double)
                obj = (object) JoltHelper.RoundDouble(element.ProjectContext, (double) obj);
              arrayList.Add((object) new DictionaryEntry((object) animationSceneNode.TargetProperty, obj));
              flag = true;
            }
          }
        }
        foreach (DictionaryEntry dictionaryEntry in arrayList)
          this.SetValue(element, (PropertyReference) dictionaryEntry.Key, dictionaryEntry.Value, time);
        if (!flag)
          this.AddCompoundKeyframe((SceneNode) element, time);
        editTransaction.Commit();
      }
    }

    public void OffsetSelectedKeyframes(double offset)
    {
      ICollection<KeyFrameSceneNode> collection = (ICollection<KeyFrameSceneNode>) this.keyFrameSelectionSet.Selection;
      List<KeyframeData> list = new List<KeyframeData>();
      foreach (KeyFrameSceneNode keyFrameSceneNode in (IEnumerable<KeyFrameSceneNode>) collection)
        list.Add(new KeyframeData(keyFrameSceneNode.KeyFrameAnimation, keyFrameSceneNode.Time, keyFrameSceneNode.TargetElement));
      list.Sort();
      if (offset > 0.0)
        list.Reverse();
      this.keyFrameSelectionSet.Clear();
      foreach (KeyframeData keyframeData in list)
      {
        double second = keyframeData.Second;
        this.ChangeKeyFrameTime(keyframeData.First, second, TimelineView.SnapSeconds(second + offset, false));
        this.RemoveCompoundKeyframeIfExists(this.ActiveStoryboardTimeline, keyframeData.Third, second);
      }
      foreach (KeyframeData keyframeData in list)
      {
        KeyFrameSceneNode keyFrameAtTime = keyframeData.First.GetKeyFrameAtTime(TimelineView.SnapSeconds(keyframeData.Second + offset, false));
        if (keyFrameAtTime != null)
          this.keyFrameSelectionSet.ExtendSelection(keyFrameAtTime);
      }
    }

    public void MoveKeyframe(SceneNode targetNode, PropertyReference prop, double fromTime, double toTime)
    {
      if (fromTime == toTime)
        return;
      KeyFrameAnimationSceneNode animation = (KeyFrameAnimationSceneNode) this.ActiveStoryboardTimeline.GetAnimation(targetNode, prop);
      ICollection<KeyFrameSceneNode> collection = (ICollection<KeyFrameSceneNode>) this.keyFrameSelectionSet.Selection;
      List<double> list = new List<double>();
      using (SceneEditTransaction editTransaction = this.viewModel.CreateEditTransaction(StringTable.UndoUnitMoveKeyFrame))
      {
        foreach (KeyFrameSceneNode selectionToRemove in (IEnumerable<KeyFrameSceneNode>) collection)
        {
          if (selectionToRemove.KeyFrameAnimation == animation)
          {
            list.Add(selectionToRemove.Time);
            this.keyFrameSelectionSet.RemoveSelection(selectionToRemove);
          }
        }
        this.ChangeKeyFrameTime(animation, fromTime, toTime);
        this.RemoveCompoundKeyframeIfExists(this.ActiveStoryboardTimeline, targetNode, fromTime);
        foreach (double num in list)
        {
          double time = num;
          if (num == fromTime)
            time = toTime;
          this.keyFrameSelectionSet.ExtendSelection(animation.GetKeyFrameAtTime(time));
        }
        editTransaction.Commit();
      }
    }

    public void MoveScheduledProperties(TimelineSceneNode timeline, double begin, double clipBegin, double clipEnd)
    {
      if (timeline.Begin == begin && timeline.ClipBegin == clipBegin && timeline.ClipEnd == clipEnd)
        return;
      using (SceneEditTransaction editTransaction = this.viewModel.CreateEditTransaction(StringTable.UndoUnitMoveScheduledProperties))
      {
        timeline.Begin = begin;
        timeline.ClipBegin = clipBegin;
        timeline.ClipEnd = clipEnd;
        editTransaction.Commit();
      }
    }

    public void SetRepeatCount(TimelineSceneNode timeline, double repeatCount)
    {
      using (SceneEditTransaction editTransaction = this.viewModel.CreateEditTransaction(StringTable.UndoUnitRepeatCount))
      {
        this.SetRepeatCountPrivate(timeline, repeatCount);
        editTransaction.Commit();
      }
    }

    public void UpdateStoryboardOnElementRename(IStoryboardContainer storyboardContainer, Dictionary<string, string> renameTable)
    {
      foreach (StoryboardTimelineSceneNode timelineSceneNode1 in this.EnumerateStoryboardsForContainer(storyboardContainer))
      {
        foreach (TimelineSceneNode timelineSceneNode2 in (IEnumerable<TimelineSceneNode>) timelineSceneNode1.Children)
        {
          if (timelineSceneNode2.TargetName != null && renameTable.ContainsKey(timelineSceneNode2.TargetName))
            timelineSceneNode2.TargetName = renameTable[timelineSceneNode2.TargetName];
        }
      }
      foreach (TriggerBaseNode triggerBaseNode in (IEnumerable<TriggerBaseNode>) storyboardContainer.VisualTriggers)
      {
        BaseTriggerNode baseTriggerNode = triggerBaseNode as BaseTriggerNode;
        if (baseTriggerNode != null)
        {
          foreach (SetterSceneNode setterSceneNode in (IEnumerable<SceneNode>) baseTriggerNode.Setters)
          {
            string target = setterSceneNode.Target;
            if (target != null && renameTable.ContainsKey(target))
              setterSceneNode.Target = renameTable[target];
          }
          ITriggerConditionNode triggerConditionNode = baseTriggerNode as ITriggerConditionNode;
          if (triggerConditionNode != null)
          {
            string sourceName = triggerConditionNode.SourceName;
            if (!string.IsNullOrEmpty(sourceName) && renameTable.ContainsKey(sourceName))
              triggerConditionNode.SourceName = renameTable[sourceName];
          }
          MultiTriggerNode multiTriggerNode = baseTriggerNode as MultiTriggerNode;
          if (multiTriggerNode != null)
          {
            foreach (ConditionNode conditionNode in (IEnumerable<ConditionNode>) multiTriggerNode.Conditions)
            {
              string sourceName = conditionNode.SourceName;
              if (!string.IsNullOrEmpty(sourceName) && renameTable.ContainsKey(sourceName))
                conditionNode.SourceName = renameTable[sourceName];
            }
          }
        }
        else
        {
          EventTriggerNode eventTriggerNode = triggerBaseNode as EventTriggerNode;
          if (eventTriggerNode != null)
          {
            string sourceId = eventTriggerNode.SourceID;
            if (sourceId != null && renameTable.ContainsKey(sourceId))
              eventTriggerNode.SourceID = renameTable[sourceId];
          }
        }
      }
    }

    private void RemoveNamedValueAnimations(SceneNode node, PropertyReference propertyReference)
    {
      if (!this.ShouldScanForNamedValues((ITypeId) this.viewModel.ProjectContext.ResolveType((ITypeId) propertyReference.ValueTypeId)))
        return;
      DocumentNodePath valueAsDocumentNode = node.GetLocalValueAsDocumentNode(propertyReference, false);
      DocumentNode node1 = valueAsDocumentNode != null ? valueAsDocumentNode.Node : (DocumentNode) null;
      if (node1 == null || this.viewModel.IsExternal(node1))
        return;
      List<SceneNode> list = new List<SceneNode>();
      this.AddNamedValuesToList(this.viewModel, node1, (IList<SceneNode>) list);
      this.FindOrDeleteAllAnimations((ICollection<SceneNode>) list, "", false, false);
    }

    public void ValidateAnimations(SceneNode node, PropertyReference propertyReference, object newValue)
    {
      if (!node.IsAttached)
        return;
      ITypeId type = (ITypeId) this.viewModel.ProjectContext.ResolveType((ITypeId) propertyReference.ValueTypeId);
      this.RemoveNamedValueAnimations(node, propertyReference);
      if (newValue == DependencyProperty.UnsetValue)
      {
        DependencyPropertyReferenceStep propertyReferenceStep = propertyReference.LastStep as DependencyPropertyReferenceStep;
        if (propertyReferenceStep != null)
        {
          object defaultValue = propertyReferenceStep.GetDefaultValue(node.TargetType);
          if (defaultValue is double && (double.IsNaN((double) defaultValue) || double.IsInfinity((double) defaultValue)) && !this.CanAnimateLayout)
          {
            this.DeleteAllAnimations(node, propertyReference.Path, true);
            return;
          }
        }
        if (PlatformTypes.DependencyObject.IsAssignableFrom(type) && !PlatformTypes.TextDecorationCollection.IsAssignableFrom(type))
        {
          this.DeleteAllAnimations(node, propertyReference.Path);
          return;
        }
      }
      if (PlatformTypes.Brush.IsAssignableFrom(type))
      {
        object computedValue = node.GetComputedValue(propertyReference);
        PropertyReference propertyReference1 = new PropertyReference((ReferenceStep) this.viewModel.ProjectContext.Platform.Metadata.ResolveProperty(GradientBrushNode.GradientStopsProperty)).Append(GradientStopCollectionNode.CountProperty);
        if (computedValue == null || newValue == null || !PlatformTypes.IsInstance(computedValue, PlatformTypes.Brush, (ITypeResolver) this.viewModel.ProjectContext))
        {
          if (computedValue == newValue)
            return;
          this.DeleteAllAnimations(node, propertyReference.Path);
        }
        else
        {
          if (!(computedValue.GetType() != newValue.GetType()) && (!PlatformTypes.IsInstance(computedValue, PlatformTypes.GradientBrush, (ITypeResolver) this.viewModel.ProjectContext) || !PlatformTypes.IsInstance(newValue, PlatformTypes.GradientBrush, (ITypeResolver) this.viewModel.ProjectContext) || (int) propertyReference1.GetValue(computedValue) == (int) propertyReference1.GetValue(newValue)))
            return;
          this.DeleteAllAnimations(node, propertyReference.Path);
        }
      }
      else if (PlatformTypes.GradientStopCollection.Equals((object) type))
        this.DeleteAllAnimations(node, new PropertyReference(propertyReference[0]).Path);
      else if (PlatformTypes.Double.Equals((object) type))
      {
        if (!(newValue is double) || !double.IsNaN((double) newValue) && !double.IsInfinity((double) newValue) || this.CanAnimateLayout)
          return;
        this.DeleteAllAnimations(node, propertyReference.Path, true);
      }
      else
      {
        if (!PlatformTypes.Material.IsAssignableFrom(type))
          return;
        Material material = node.GetLocalValueAsWpf(propertyReference) as Material;
        if (material == null || newValue == null)
        {
          if (material == newValue)
            return;
          this.DeleteAllAnimations(node, propertyReference.Path);
        }
        else
        {
          if (!(material.GetType() != newValue.GetType()))
            return;
          this.DeleteAllAnimations(node, propertyReference.Path);
        }
      }
    }

    public void ValidateAnimations(SceneNode node, PropertyReference propertyReference, int index, bool isAdding)
    {
      if (!node.IsAttached || index == -1)
        return;
      if (!isAdding)
      {
        IProjectContext projectContext = node.ProjectContext;
        List<ReferenceStep> steps = new List<ReferenceStep>((IEnumerable<ReferenceStep>) propertyReference.ReferenceSteps);
        IndexedClrPropertyReferenceStep referenceStep = IndexedClrPropertyReferenceStep.GetReferenceStep((ITypeResolver) projectContext, propertyReference.ValueType, index, false);
        if (referenceStep != null)
        {
          steps.Add((ReferenceStep) referenceStep);
          PropertyReference propertyReference1 = new PropertyReference(steps);
          this.RemoveNamedValueAnimations(node, propertyReference1);
        }
      }
      this.UpdateAnimationIndices(node, propertyReference, index, isAdding);
    }

    private void UpdateAnimationIndices(SceneNode node, PropertyReference propertyReference, int index, bool isAdding)
    {
      foreach (StoryboardTimelineSceneNode timeline in this.EnumerateStoryboardsForContainer(node.StoryboardContainer))
      {
        for (int index1 = timeline.Children.Count - 1; index1 >= 0; --index1)
        {
          AnimationSceneNode animationSceneNode = timeline.Children[index1] as AnimationSceneNode;
          if (animationSceneNode != null && animationSceneNode.TargetElement == node)
          {
            PropertyReference targetProperty = animationSceneNode.TargetProperty;
            if (targetProperty.Count > propertyReference.Count)
            {
              bool flag = true;
              for (int index2 = 0; index2 < propertyReference.ReferenceSteps.Count; ++index2)
              {
                DependencyPropertyReferenceStep propertyReferenceStep1 = propertyReference.ReferenceSteps[index2] as DependencyPropertyReferenceStep;
                DependencyPropertyReferenceStep propertyReferenceStep2 = targetProperty.ReferenceSteps[index2] as DependencyPropertyReferenceStep;
                if (propertyReferenceStep1 != null && propertyReferenceStep2 != null)
                {
                  if (!object.ReferenceEquals(propertyReferenceStep1.DependencyProperty, propertyReferenceStep2.DependencyProperty))
                  {
                    flag = false;
                    break;
                  }
                }
                else
                {
                  IndexedClrPropertyReferenceStep propertyReferenceStep3 = propertyReference.ReferenceSteps[index2] as IndexedClrPropertyReferenceStep;
                  IndexedClrPropertyReferenceStep propertyReferenceStep4 = targetProperty.ReferenceSteps[index2] as IndexedClrPropertyReferenceStep;
                  if (propertyReferenceStep3 != null && propertyReferenceStep4 != null)
                  {
                    if (propertyReferenceStep3.Index != propertyReferenceStep4.Index)
                    {
                      flag = false;
                      break;
                    }
                  }
                  else
                  {
                    flag = targetProperty.Path.StartsWith(propertyReference.Path, StringComparison.Ordinal);
                    break;
                  }
                }
              }
              if (flag)
              {
                IndexedClrPropertyReferenceStep propertyReferenceStep = targetProperty.ReferenceSteps[propertyReference.Count] as IndexedClrPropertyReferenceStep;
                if (propertyReferenceStep != null && propertyReferenceStep.Index >= index)
                {
                  if (!isAdding && propertyReferenceStep.Index == index)
                  {
                    this.RemoveAnimation(timeline, (TimelineSceneNode) animationSceneNode);
                  }
                  else
                  {
                    int index2 = propertyReferenceStep.Index;
                    int index3 = !isAdding ? index2 - 1 : index2 + 1;
                    IProjectContext projectContext = animationSceneNode.ProjectContext;
                    List<ReferenceStep> steps = new List<ReferenceStep>((IEnumerable<ReferenceStep>) targetProperty.ReferenceSteps);
                    steps[propertyReference.Count] = (ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep((ITypeResolver) projectContext, PlatformTypeHelper.GetDeclaringType((IMember) propertyReferenceStep), index3);
                    animationSceneNode.TargetProperty = new PropertyReference(steps);
                  }
                }
              }
            }
          }
        }
      }
    }

    public void DeleteAllAnimations(SceneNode node)
    {
      this.DeleteAllAnimations(node, "");
    }

    public void DeleteAllAnimationsInSubtree(SceneElement element)
    {
      List<SceneNode> list = new List<SceneNode>();
      foreach (SceneElement sceneElement in SceneElementHelper.GetLogicalTree(element))
      {
        if (sceneElement.StoryboardContainer == element.StoryboardContainer)
        {
          list.Add((SceneNode) sceneElement);
          this.AddNamedValuesForAllPropertiesToList((SceneNode) sceneElement, (IList<SceneNode>) list);
        }
      }
      this.FindOrDeleteAllAnimations((ICollection<SceneNode>) list, "", false, false);
    }

    public void DeleteAllAnimationsInPropertySubtree(SceneNode node, PropertyReference property)
    {
      SceneNode valueAsSceneNode = node.GetLocalValueAsSceneNode(property);
      if (valueAsSceneNode == null || node.StoryboardContainer != valueAsSceneNode.StoryboardContainer)
        return;
      SceneElement element = valueAsSceneNode as SceneElement;
      if (element != null)
      {
        this.DeleteAllAnimationsInSubtree(element);
      }
      else
      {
        if (!node.ShouldClearAnimation)
          return;
        this.DeleteAllAnimations(node, property.ToString() + "/");
      }
    }

    public void DeleteAllAnimations(SceneNode node, string referencePrefix)
    {
      this.DeleteAllAnimations(node, referencePrefix, false);
    }

    public void DeleteAllAnimations(SceneNode node, string referencePrefix, bool preserveObjectAnimations)
    {
      List<SceneNode> list = new List<SceneNode>();
      list.Add(node);
      this.AddNamedValuesForAllPropertiesToList(node, (IList<SceneNode>) list);
      this.FindOrDeleteAllAnimations((ICollection<SceneNode>) list, referencePrefix, false, preserveObjectAnimations);
    }

    public bool HasAnimations(SceneNode node, string referencePrefix)
    {
      List<SceneNode> list = new List<SceneNode>();
      list.Add(node);
      this.AddNamedValuesForAllPropertiesToList(node, (IList<SceneNode>) list);
      return this.FindOrDeleteAllAnimations((ICollection<SceneNode>) list, referencePrefix, true, false);
    }

    public void UpdateTransformReferences(SceneNode node, string referencePrefix)
    {
      if (node == null || string.IsNullOrEmpty(referencePrefix))
        return;
      IStoryboardContainer storyboardContainer = node.StoryboardContainer;
      if (storyboardContainer == null)
        return;
      bool flag = false;
      foreach (StoryboardTimelineSceneNode timeline in this.EnumerateStoryboardsForContainer(storyboardContainer))
      {
        for (int index = timeline.Children.Count - 1; index >= 0; --index)
        {
          TimelineSceneNode animation = timeline.Children[index];
          if (animation != null)
          {
            TimelineSceneNode.PropertyNodePair elementAndProperty = animation.TargetElementAndProperty;
            if (node == elementAndProperty.SceneNode && elementAndProperty.PropertyReference != null && elementAndProperty.PropertyReference.Path.StartsWith(referencePrefix, StringComparison.Ordinal))
            {
              PropertyReference propertyReference = TransformPropertyLookup.ConvertTransformPropertyToComposite(elementAndProperty.PropertyReference);
              if (propertyReference != null)
              {
                animation.TargetProperty = propertyReference;
              }
              else
              {
                this.RemoveAnimation(timeline, animation);
                flag = true;
              }
            }
          }
        }
      }
      if (!flag)
        return;
      this.viewModel.DefaultView.ShowBubble(StringTable.AnimationAutoDeletedWarningMessage, MessageBubbleType.Warning);
    }

    private bool FindOrDeleteAllAnimations(ICollection<SceneNode> nodes, string referencePrefix, bool findOnly, bool preserveObjectAnimations)
    {
      if (nodes == null)
        return false;
      List<SceneNode> list1 = new List<SceneNode>((IEnumerable<SceneNode>) nodes);
      bool flag1 = false;
      bool flag2 = false;
      while (list1.Count > 0)
      {
        IStoryboardContainer storyboardContainer = list1[0].StoryboardContainer;
        HybridDictionary hybridDictionary = new HybridDictionary();
        for (int index = list1.Count - 1; index >= 0; --index)
        {
          SceneNode sceneNode = list1[index];
          if (sceneNode.StoryboardContainer == storyboardContainer)
          {
            list1.RemoveAt(index);
            hybridDictionary[(object) sceneNode] = (object) true;
          }
        }
        if (storyboardContainer != null)
        {
          foreach (StoryboardTimelineSceneNode timeline in this.EnumerateStoryboardsForContainer(storyboardContainer))
          {
            for (int index = timeline.Children.Count - 1; index >= 0; --index)
            {
              AnimationSceneNode animationSceneNode = timeline.Children[index] as AnimationSceneNode;
              MediaTimelineSceneNode timelineSceneNode = timeline.Children[index] as MediaTimelineSceneNode;
              if (animationSceneNode != null && (!preserveObjectAnimations || !PlatformTypes.ObjectAnimationUsingKeyFrames.IsAssignableFrom((ITypeId) animationSceneNode.Type)))
              {
                TimelineSceneNode.PropertyNodePair elementAndProperty = animationSceneNode.TargetElementAndProperty;
                if (elementAndProperty.SceneNode != null && hybridDictionary.Contains((object) elementAndProperty.SceneNode))
                {
                  bool flag3 = true;
                  if (!string.IsNullOrEmpty(referencePrefix) && (elementAndProperty.PropertyReference == null || !elementAndProperty.PropertyReference.Path.StartsWith(referencePrefix, StringComparison.Ordinal)))
                    flag3 = false;
                  if (flag3)
                  {
                    if (findOnly)
                      return true;
                    this.RemoveAnimation(timeline, (TimelineSceneNode) animationSceneNode);
                    flag1 = true;
                  }
                }
              }
              else if (timelineSceneNode != null && timelineSceneNode.TargetElement != null && (hybridDictionary.Contains((object) timelineSceneNode.TargetElement) && string.IsNullOrEmpty(referencePrefix)))
              {
                if (findOnly)
                  return true;
                this.RemoveAnimation(timeline, (TimelineSceneNode) timelineSceneNode);
                flag1 = true;
              }
            }
          }
          if (storyboardContainer.CanEditTriggers)
          {
            List<SceneNode> list2 = new List<SceneNode>();
            foreach (TriggerBaseNode triggerBaseNode in (IEnumerable<TriggerBaseNode>) storyboardContainer.VisualTriggers)
            {
              BaseTriggerNode baseTriggerNode = triggerBaseNode as BaseTriggerNode;
              EventTriggerNode eventTriggerNode = triggerBaseNode as EventTriggerNode;
              TriggerNode triggerNode = triggerBaseNode as TriggerNode;
              MultiTriggerNode multiTriggerNode = triggerBaseNode as MultiTriggerNode;
              if (baseTriggerNode != null)
              {
                ISceneNodeCollection<SceneNode> setters = baseTriggerNode.Setters;
                int index = 0;
                while (index < setters.Count)
                {
                  SetterSceneNode setterSceneNode = setters[index] as SetterSceneNode;
                  if (setterSceneNode != null)
                  {
                    SceneNode sceneNode = setterSceneNode.StoryboardContainer.ResolveTargetName(setterSceneNode.Target);
                    if (sceneNode != null && hybridDictionary.Contains((object) sceneNode) && setterSceneNode.Property.Name.StartsWith(referencePrefix, StringComparison.Ordinal))
                    {
                      if (findOnly)
                        return true;
                      setterSceneNode.Remove();
                      flag2 = true;
                    }
                    else
                      ++index;
                  }
                }
              }
              if (string.IsNullOrEmpty(referencePrefix))
              {
                if (eventTriggerNode != null && eventTriggerNode.SourceID != null)
                {
                  SceneNode sceneNode = eventTriggerNode.StoryboardContainer.ResolveTargetName(eventTriggerNode.SourceID);
                  if (sceneNode != null && hybridDictionary.Contains((object) sceneNode))
                  {
                    if (findOnly)
                      return true;
                    list2.Add((SceneNode) eventTriggerNode);
                    flag2 = true;
                  }
                }
                else if (triggerNode != null)
                {
                  SceneNode source = ((ITriggerConditionNode) triggerNode).Source;
                  if (source != null && hybridDictionary.Contains((object) source))
                  {
                    if (findOnly)
                      return true;
                    list2.Add((SceneNode) triggerNode);
                    flag2 = true;
                  }
                }
                else if (multiTriggerNode != null)
                {
                  bool flag3 = false;
                  foreach (ITriggerConditionNode triggerConditionNode in (IEnumerable<ConditionNode>) multiTriggerNode.Conditions)
                  {
                    SceneNode source = triggerConditionNode.Source;
                    if (source != null && hybridDictionary.Contains((object) source))
                    {
                      if (findOnly)
                        return true;
                      list2.Add((SceneNode) triggerConditionNode);
                      flag2 = true;
                    }
                    else
                      flag3 = true;
                  }
                  if (!flag3)
                  {
                    if (findOnly)
                      return true;
                    list2.Add((SceneNode) multiTriggerNode);
                    flag2 = true;
                  }
                }
              }
            }
            foreach (SceneNode sceneNode in list2)
              sceneNode.Remove();
          }
        }
      }
      if (flag1)
        this.viewModel.DefaultView.ShowBubble(StringTable.AnimationAutoDeletedWarningMessage, MessageBubbleType.Warning);
      if (!flag1)
        return flag2;
      return true;
    }

    public bool IsTargetedByName(SceneElement element)
    {
      IStoryboardContainer storyboardContainer = element.StoryboardContainer;
      if (storyboardContainer == null)
        return false;
      foreach (StoryboardTimelineSceneNode timelineSceneNode1 in this.EnumerateStoryboardsForContainer(storyboardContainer))
      {
        foreach (TimelineSceneNode timelineSceneNode2 in (IEnumerable<TimelineSceneNode>) timelineSceneNode1.Children)
        {
          AnimationSceneNode animationSceneNode = timelineSceneNode2 as AnimationSceneNode;
          if (animationSceneNode != null && animationSceneNode.TargetElement == element)
            return true;
        }
      }
      foreach (TriggerBaseNode triggerBaseNode in (IEnumerable<TriggerBaseNode>) storyboardContainer.VisualTriggers)
      {
        BaseTriggerNode baseTriggerNode = triggerBaseNode as BaseTriggerNode;
        EventTriggerNode eventTriggerNode = triggerBaseNode as EventTriggerNode;
        if (baseTriggerNode != null)
        {
          foreach (SetterSceneNode setterSceneNode in (IEnumerable<SceneNode>) baseTriggerNode.Setters)
          {
            if (setterSceneNode.Target == element.Name)
              return true;
          }
        }
        else if (eventTriggerNode != null && eventTriggerNode.SourceID == element.Name)
          return true;
      }
      return false;
    }

    private void DeleteAnimationsInTimeline(StoryboardTimelineSceneNode timeline, SceneElement targetElement, PropertyReference targetProperty, AnimationSceneNode animationToIgnore)
    {
      List<AnimationSceneNode> list = new List<AnimationSceneNode>();
      foreach (TimelineSceneNode timelineSceneNode in (IEnumerable<TimelineSceneNode>) timeline.Children)
      {
        AnimationSceneNode animationSceneNode = timelineSceneNode as AnimationSceneNode;
        if (animationSceneNode != null && animationSceneNode != animationToIgnore && !AnimationProxyManager.IsOptimizedAnimation((TimelineSceneNode) animationSceneNode))
        {
          TimelineSceneNode.PropertyNodePair elementAndProperty = animationSceneNode.TargetElementAndProperty;
          if (elementAndProperty.SceneNode == targetElement && object.Equals((object) elementAndProperty.PropertyReference, (object) targetProperty))
            list.Add(animationSceneNode);
        }
      }
      foreach (AnimationSceneNode animationSceneNode in list)
        this.RemoveAnimation(timeline, (TimelineSceneNode) animationSceneNode);
    }

    public void DeleteKeyframe(SceneNode targetElement, PropertyReference propertyReference, double time)
    {
      using (SceneEditTransaction editTransaction = this.viewModel.CreateEditTransaction(StringTable.UndoUnitDeleteKeyFrame))
      {
        if (!DesignTimeProperties.ExplicitAnimationProperty.Equals((object) propertyReference[0]))
        {
          this.DeleteKeyframeCore((KeyFrameAnimationSceneNode) this.ActiveStoryboardTimeline.GetAnimation(targetElement, propertyReference), time);
          bool flag = false;
          foreach (TimelineSceneNode timelineSceneNode in (IEnumerable<TimelineSceneNode>) this.ActiveStoryboardTimeline.Children)
          {
            if (timelineSceneNode is AnimationSceneNode && timelineSceneNode.TargetElement == targetElement)
            {
              flag = true;
              break;
            }
          }
          if (!flag)
            this.RemoveCompoundKeyframeIfExists(this.ActiveStoryboardTimeline, targetElement, time);
        }
        else
          this.RemoveCompoundKeyframeIfExists(this.ActiveStoryboardTimeline, targetElement, time);
        editTransaction.Commit();
      }
    }

    public void DeleteAnimation(AnimationSceneNode animationNode)
    {
      this.animationSelectionSet.RemoveSelection(animationNode);
      this.RemoveAnimation(this.ActiveStoryboardTimeline, (TimelineSceneNode) animationNode);
    }

    public void SetEaseOfSelection(Point? easeInControlPoint, Point? easeOutControlPoint)
    {
      List<KeyFrameSceneNode> list1 = new List<KeyFrameSceneNode>((IEnumerable<KeyFrameSceneNode>) this.keyFrameSelectionSet.Selection);
      list1.Sort();
      list1.Reverse();
      List<KeyFrameSceneNode> list2 = new List<KeyFrameSceneNode>();
      using (SceneEditTransaction editTransaction = this.viewModel.CreateEditTransaction(StringTable.UndoUnitKeyframeEase))
      {
        this.keyFrameSelectionSet.Clear();
        foreach (KeyFrameSceneNode keyFrameSceneNode1 in list1)
        {
          if (easeInControlPoint.HasValue)
          {
            KeyFrameSceneNode keyFrameSceneNode2 = KeyFrameSceneNode.EnsureCanSetEaseInControlPoint(keyFrameSceneNode1);
            if (keyFrameSceneNode2 != null)
            {
              keyFrameSceneNode2.EaseInControlPoint = easeInControlPoint.Value;
              list2.Add(keyFrameSceneNode2);
            }
          }
          if (easeOutControlPoint.HasValue)
          {
            int index = -1;
            if (keyFrameSceneNode1.KeyFrameAnimation != null)
            {
              KeyFrameSceneNode nextKeyFrame = keyFrameSceneNode1.KeyFrameAnimation.GetNextKeyFrame(keyFrameSceneNode1);
              index = list2.IndexOf(nextKeyFrame);
            }
            if (KeyFrameSceneNode.SetEaseOutControlPoint(keyFrameSceneNode1, easeOutControlPoint.Value) && index != -1)
              list2[index] = keyFrameSceneNode1.KeyFrameAnimation.GetNextKeyFrame(keyFrameSceneNode1);
            list2.Add(keyFrameSceneNode1);
          }
        }
        this.keyFrameSelectionSet.SetSelection((ICollection<KeyFrameSceneNode>) list2, (KeyFrameSceneNode) null);
        editTransaction.Commit();
      }
    }

    public void SetMotionPath(SceneElement targetElement, PathGeometry path, double? begin, double? end)
    {
      using (SceneEditTransaction editTransaction = this.viewModel.CreateEditTransaction(StringTable.EditMotionPathUndo))
      {
        bool flag1 = false;
        bool removedAnimation = false;
        BaseFrameworkElement frameworkElement = targetElement as BaseFrameworkElement;
        if (frameworkElement != null)
          frameworkElement.EnsureRenderTransform();
        this.SetPathAnimation(targetElement, targetElement.Platform.Metadata.CommonProperties.RenderTransformTranslationX, path, PathAnimationSource.X, true, begin, end, out removedAnimation);
        bool flag2 = flag1 | removedAnimation;
        this.SetPathAnimation(targetElement, targetElement.Platform.Metadata.CommonProperties.RenderTransformTranslationY, path, PathAnimationSource.Y, true, begin, end, out removedAnimation);
        bool flag3 = flag2 | removedAnimation;
        this.SetPathAnimation(targetElement, targetElement.Platform.Metadata.CommonProperties.RenderTransformRotationAngle, path, PathAnimationSource.Angle, false, begin, end, out removedAnimation);
        bool flag4 = flag3 | removedAnimation;
        editTransaction.Commit();
        if (!flag4)
          return;
        this.viewModel.DefaultView.ShowBubble(StringTable.AnimationAutoDeletedWarningMessage, MessageBubbleType.Warning);
      }
    }

    public void DeleteMotionPath(SceneNode targetElement)
    {
      using (SceneEditTransaction editTransaction = this.viewModel.CreateEditTransaction(StringTable.DeleteMotionPathUndo))
      {
        ReadOnlyCollection<SceneElement> selection = this.viewModel.ElementSelectionSet.Selection;
        SceneElement primarySelection = this.viewModel.ElementSelectionSet.PrimarySelection;
        this.viewModel.ElementSelectionSet.Clear();
        AnimationSceneNode animation1 = this.ActiveStoryboardTimeline.GetAnimation(targetElement, targetElement.Platform.Metadata.CommonProperties.RenderTransformTranslationX);
        if (animation1 != null)
          this.RemoveAnimation(this.ActiveStoryboardTimeline, (TimelineSceneNode) animation1);
        AnimationSceneNode animation2 = this.ActiveStoryboardTimeline.GetAnimation(targetElement, targetElement.Platform.Metadata.CommonProperties.RenderTransformTranslationY);
        if (animation2 != null)
          this.RemoveAnimation(this.ActiveStoryboardTimeline, (TimelineSceneNode) animation2);
        AnimationSceneNode animation3 = this.ActiveStoryboardTimeline.GetAnimation(targetElement, targetElement.Platform.Metadata.CommonProperties.RenderTransformRotationAngle);
        if (animation3 != null && animation3 is PathAnimationSceneNode)
          this.RemoveAnimation(this.ActiveStoryboardTimeline, (TimelineSceneNode) animation3);
        this.viewModel.ElementSelectionSet.SetSelection((ICollection<SceneElement>) selection, primarySelection);
        editTransaction.Commit();
      }
    }

    public bool IsMotionPathOrientedToPath(SceneNode targetNode)
    {
      return this.ActiveStoryboardTimeline.GetAnimation(targetNode, targetNode.Platform.Metadata.CommonProperties.RenderTransformRotationAngle) is PathAnimationSceneNode;
    }

    public void SetMotionPathOrientToPath(SceneElement targetElement, bool orientToPath)
    {
      using (SceneEditTransaction editTransaction = this.viewModel.CreateEditTransaction(StringTable.EditMotionPathUndo))
      {
        bool removedAnimation = false;
        if (!orientToPath)
        {
          AnimationSceneNode animation = this.ActiveStoryboardTimeline.GetAnimation((SceneNode) targetElement, targetElement.Platform.Metadata.CommonProperties.RenderTransformRotationAngle);
          if (animation != null)
            this.RemoveAnimation(this.ActiveStoryboardTimeline, (TimelineSceneNode) animation);
        }
        else
        {
          PathAnimationSceneNode animationSceneNode = this.ActiveStoryboardTimeline.GetAnimation((SceneNode) targetElement, targetElement.Platform.Metadata.CommonProperties.RenderTransformTranslationX) as PathAnimationSceneNode;
          if (animationSceneNode != null)
            this.SetPathAnimation(targetElement, targetElement.Platform.Metadata.CommonProperties.RenderTransformRotationAngle, animationSceneNode.Path, PathAnimationSource.Angle, true, new double?(animationSceneNode.Begin), new double?(animationSceneNode.ClipEnd), out removedAnimation);
        }
        editTransaction.Commit();
        if (!removedAnimation)
          return;
        this.viewModel.DefaultView.ShowBubble(StringTable.AnimationAutoDeletedWarningMessage, MessageBubbleType.Warning);
      }
    }

    private bool DoesPropertyUseNonAnimatableValues(IPropertyId property)
    {
      if (!BaseFrameworkElement.WidthProperty.Equals((object) property) && !BaseFrameworkElement.HeightProperty.Equals((object) property) && (!BaseFrameworkElement.MaxWidthProperty.Equals((object) property) && !BaseFrameworkElement.MaxHeightProperty.Equals((object) property)) && !CanvasElement.LeftProperty.Equals((object) property))
        return CanvasElement.TopProperty.Equals((object) property);
      return true;
    }

    public void EnsureNonAnimatablePropertiesAreObjectAnimations(StoryboardTimelineSceneNode storyboard)
    {
      for (int index1 = 0; index1 < storyboard.Children.Count; ++index1)
      {
        TimelineSceneNode timeline = storyboard.Children[index1];
        AnimationSceneNode animationSceneNode1 = timeline as AnimationSceneNode;
        if (animationSceneNode1 != null && !AnimationProxyManager.IsAnimationProxy(timeline) && (PlatformTypes.Double.Equals((object) animationSceneNode1.AnimatedType) && this.DoesPropertyUseNonAnimatableValues((IPropertyId) timeline.TargetProperty.FirstStep)))
        {
          List<double> list1 = new List<double>();
          List<double> list2 = new List<double>();
          FromToAnimationSceneNode animationSceneNode2 = animationSceneNode1 as FromToAnimationSceneNode;
          if (animationSceneNode2 != null)
          {
            if (((double?) animationSceneNode2.From).HasValue)
            {
              list1.Add(0.0);
              list2.Add(((double?) animationSceneNode2.From).Value);
            }
            if (((double?) animationSceneNode2.To).HasValue)
            {
              list1.Add(animationSceneNode2.Duration);
              list2.Add(((double?) animationSceneNode2.To).Value);
            }
          }
          else
          {
            KeyFrameAnimationSceneNode animationSceneNode3 = animationSceneNode1 as KeyFrameAnimationSceneNode;
            if (animationSceneNode3 != null)
            {
              foreach (KeyFrameSceneNode keyFrameSceneNode in animationSceneNode3.KeyFrames)
              {
                list1.Add(keyFrameSceneNode.Time);
                list2.Add((double) keyFrameSceneNode.Value);
              }
            }
          }
          KeyFrameAnimationSceneNode animationSceneNode4 = KeyFrameAnimationSceneNode.Factory.InstantiateWithTarget(storyboard.ViewModel, animationSceneNode1.TargetElement, animationSceneNode1.TargetProperty, storyboard.StoryboardContainer, PlatformTypes.ObjectAnimationUsingKeyFrames);
          for (int index2 = 0; index2 < list1.Count; ++index2)
            animationSceneNode4.AddKeyFrame(list1[index2], (object) list2[index2]);
          animationSceneNode4.Begin = animationSceneNode1.Begin;
          storyboard.Children.RemoveAt(index1);
          storyboard.Children.Insert(index1, (TimelineSceneNode) animationSceneNode4);
        }
      }
    }

    public void AddPropertyAutoKeyframe(SceneElement element, PropertyReference property, double time, object oldValue, object newValue)
    {
      AnimationSceneNode animation1 = this.ActiveStoryboardTimeline.GetAnimation((SceneNode) element, property);
      if (animation1 != null && !(animation1 is KeyFrameAnimationSceneNode))
        return;
      KeyFrameAnimationSceneNode animation2 = animation1 as KeyFrameAnimationSceneNode;
      if (animation2 == null)
      {
        element.EnsureNamed();
        ITypeId type = !this.CanAnimateLayout || !this.DoesPropertyUseNonAnimatableValues((IPropertyId) property.FirstStep) ? KeyFrameAnimationSceneNode.GetKeyFrameAnimationForType((ITypeId) property.ValueTypeId, element.ProjectContext) : PlatformTypes.ObjectAnimationUsingKeyFrames;
        animation2 = KeyFrameAnimationSceneNode.Factory.InstantiateWithTarget(element.ViewModel, (SceneNode) element, property, this.ActiveStoryboardContainer, type);
        animation2.Begin = 0.0;
        this.InsertAnimation(this.ActiveStoryboardTimeline, (TimelineSceneNode) animation2);
      }
      double naturalDuration = animation2.NaturalDuration;
      double clipEnd = animation2.ClipEnd;
      KeyFrameSceneNode keyFrameAtTime = animation2.GetKeyFrameAtTime(time);
      if (keyFrameAtTime != null)
      {
        keyFrameAtTime.Value = newValue;
      }
      else
      {
        double nextTime;
        if (this.HasCompoundKeyframeAfterTime((SceneNode) element, time, out nextTime) && !animation2.HasKeyFrameAtTime(nextTime))
        {
          this.AddKeyFrame(animation2, nextTime, oldValue);
          this.RemoveCompoundKeyframeIfExists(this.ActiveStoryboardTimeline, (SceneNode) element, nextTime);
        }
        double previousTime;
        if (this.HasCompoundKeyframeBeforeTime((SceneNode) element, time, out previousTime) && !animation2.HasKeyFrameAtTime(previousTime))
        {
          this.AddKeyFrame(animation2, previousTime, oldValue);
          this.RemoveCompoundKeyframeIfExists(this.ActiveStoryboardTimeline, (SceneNode) element, previousTime);
        }
        this.AddKeyFrame(animation2, time, newValue);
        this.RemoveCompoundKeyframeIfExists(this.ActiveStoryboardTimeline, (SceneNode) element, time);
      }
    }

    private void SetFromOrToValue(FromToAnimationSceneNode animation, double time, object value)
    {
      if (animation.Duration == 0.0 && time == animation.Begin)
        animation.To = value;
      else if (time <= animation.Begin)
      {
        animation.From = value;
        double num = animation.Begin - time;
        animation.Begin = time;
        animation.Duration += num;
      }
      else
      {
        animation.To = value;
        animation.Duration = time - animation.Begin;
      }
    }

    private object StoryboardsSubscription_TimelineInserted(object sender, SceneNode basisNode, object basisContent, SceneNode newPathNode)
    {
      if (newPathNode.DocumentNode is DocumentCompositeNode)
        this.storyboardNodes.Add((StoryboardTimelineSceneNode) newPathNode);
      return (object) null;
    }

    private void StoryboardsSubscription_TimelineRemoved(object sender, SceneNode basisNode, object basisContent, SceneNode oldPathNode, object oldContent)
    {
      this.storyboardNodes.Remove((StoryboardTimelineSceneNode) oldPathNode);
    }

    private void DeleteKeyframeCore(KeyFrameAnimationSceneNode animation, double time)
    {
      double naturalDuration = animation.NaturalDuration;
      double clipEnd = animation.ClipEnd;
      KeyFrameSceneNode keyFrameAtTime = animation.GetKeyFrameAtTime(time);
      if (this.keyFrameSelectionSet.IsSelected(keyFrameAtTime))
        this.keyFrameSelectionSet.RemoveSelection(keyFrameAtTime);
      if (animation.KeyFrameCount == 1)
      {
        this.RemoveKeyFrame(animation, time);
        this.RemoveAnimation(this.ActiveStoryboardTimeline, (TimelineSceneNode) animation);
      }
      else
        this.RemoveKeyFrame(animation, time);
    }

    private void InsertAnimation(StoryboardTimelineSceneNode timeline, TimelineSceneNode animation)
    {
      timeline.Children.Add(animation);
    }

    public void RemoveAnimation(StoryboardTimelineSceneNode timeline, TimelineSceneNode animation)
    {
      TimelineSceneNode timelineSceneNode1 = (TimelineSceneNode) null;
      foreach (TimelineSceneNode timelineSceneNode2 in (IEnumerable<TimelineSceneNode>) timeline.Children)
      {
        if (timelineSceneNode2 == animation)
        {
          timelineSceneNode1 = animation;
          break;
        }
      }
      List<KeyFrameSceneNode> list = new List<KeyFrameSceneNode>();
      foreach (KeyFrameSceneNode keyFrameSceneNode in this.viewModel.KeyFrameSelectionSet.Selection)
      {
        if (keyFrameSceneNode.KeyFrameAnimation != null && keyFrameSceneNode.KeyFrameAnimation == timelineSceneNode1)
          list.Add(keyFrameSceneNode);
      }
      this.viewModel.KeyFrameSelectionSet.RemoveSelection((ICollection<KeyFrameSceneNode>) list);
      timeline.Children.Remove(timelineSceneNode1);
    }

    private void AddKeyFrame(KeyFrameAnimationSceneNode animation, double time, object value)
    {
      animation.AddKeyFrame(time, value);
    }

    private void RemoveKeyFrame(KeyFrameAnimationSceneNode animation, double time)
    {
      animation.RemoveKeyFrame(time);
    }

    private void ChangeKeyFrameTime(KeyFrameAnimationSceneNode animation, double fromTime, double toTime)
    {
      animation.MoveKeyFrame(fromTime, toTime);
    }

    private void SetRepeatCountPrivate(TimelineSceneNode animation, double repeatCount)
    {
      animation.RepeatCount = repeatCount;
    }

    private PathAnimationSceneNode SetPathAnimation(SceneElement targetElement, PropertyReference targetProperty, PathGeometry path, PathAnimationSource source, bool createAnimation, double? begin, double? end, out bool removedAnimation)
    {
      AnimationSceneNode animation = this.ActiveStoryboardTimeline.GetAnimation((SceneNode) targetElement, targetProperty);
      PathAnimationSceneNode animationSceneNode = animation as PathAnimationSceneNode;
      removedAnimation = false;
      if (animationSceneNode != null)
      {
        animationSceneNode.Path = path;
        animationSceneNode.Source = source;
        if (begin.HasValue)
          animationSceneNode.Begin = begin.Value;
        if (end.HasValue)
          animationSceneNode.ClipEnd = end.Value;
      }
      else if (createAnimation)
      {
        if (animation != null)
        {
          this.RemoveAnimation(this.ActiveStoryboardTimeline, (TimelineSceneNode) animation);
          removedAnimation = true;
        }
        targetElement.EnsureNamed();
        animationSceneNode = PathAnimationSceneNode.Factory.InstantiateWithTarget(targetElement.ViewModel, (SceneNode) targetElement, targetProperty, this.ActiveStoryboardContainer, PathAnimationSceneNode.GetPathAnimationForType((ITypeId) targetProperty.ValueTypeId));
        animationSceneNode.Path = path;
        animationSceneNode.Source = source;
        animationSceneNode.Begin = begin.Value;
        animationSceneNode.ClipEnd = end.Value;
        this.InsertAnimation(this.ActiveStoryboardTimeline, (TimelineSceneNode) animationSceneNode);
      }
      return animationSceneNode;
    }

    private void AddCompoundKeyframe(SceneNode node, double time)
    {
      KeyFrameAnimationSceneNode animation = (KeyFrameAnimationSceneNode) this.ActiveStoryboardTimeline.GetAnimation(node, DesignTimeProperties.ExplicitAnimationProperty);
      if (animation == null)
      {
        node.EnsureNamed();
        PropertyReference targetProperty = new PropertyReference(this.viewModel.ProjectContext.ResolveProperty(DesignTimeProperties.ExplicitAnimationProperty) as ReferenceStep);
        animation = KeyFrameAnimationSceneNode.Factory.InstantiateWithTarget(node.ViewModel, node, targetProperty, this.ActiveStoryboardContainer, PlatformTypes.DoubleAnimationUsingKeyFrames);
        animation.Begin = 0.0;
        this.InsertAnimation(this.ActiveStoryboardTimeline, (TimelineSceneNode) animation);
      }
      if (animation.HasKeyFrameAtTime(time))
        return;
      this.AddKeyFrame(animation, time, (object) 0.0);
    }

    private void RemoveCompoundKeyframeIfExists(StoryboardTimelineSceneNode timeline, SceneNode targetElement, double time)
    {
      KeyFrameAnimationSceneNode animation = timeline.GetAnimation(targetElement, DesignTimeProperties.ExplicitAnimationProperty) as KeyFrameAnimationSceneNode;
      if (animation == null || !animation.HasKeyFrameAtTime(time))
        return;
      this.DeleteKeyframeCore(animation, time);
    }

    private bool HasCompoundKeyframeAfterTime(SceneNode targetElement, double time, out double nextTime)
    {
      double[] compoundKeyTimes = this.ActiveStoryboardTimeline.GetCompoundKeyTimes(targetElement);
      nextTime = double.NaN;
      for (int index = 0; index < compoundKeyTimes.Length; ++index)
      {
        double num = compoundKeyTimes[index];
        if (num > time && (double.IsNaN(nextTime) || num < nextTime))
          nextTime = num;
      }
      return !double.IsNaN(nextTime);
    }

    private bool HasCompoundKeyframeBeforeTime(SceneNode targetElement, double time, out double previousTime)
    {
      double[] compoundKeyTimes = this.ActiveStoryboardTimeline.GetCompoundKeyTimes(targetElement);
      previousTime = double.NaN;
      for (int index = compoundKeyTimes.Length - 1; index >= 0; --index)
      {
        double num = compoundKeyTimes[index];
        if (num < time && (double.IsNaN(previousTime) || num > previousTime))
          previousTime = num;
      }
      return !double.IsNaN(previousTime);
    }

    public void OnViewModelActiveTimelineChanged()
    {
      this.IsRecording = this.ActiveStoryboardTimeline != null || this.ActiveVisualTrigger != null;
    }

    internal void UpdateActiveTimeline()
    {
      this.UpdateActiveTimeline(true);
    }

    public void Invalidate()
    {
      if (this.cachedActiveViewStoryboard == null)
        return;
      this.cachedActiveViewStoryboard.Remove();
      this.cachedActiveViewStoryboard = (IViewStoryboard) null;
    }

    internal void UpdateActiveTimeline(bool disableMedia)
    {
      this.isActiveStoryboardMediaDirty = false;
      IViewStoryboard viewStoryboard = this.CreateViewStoryboard();
      IViewObject storyboardTarget = this.ActiveStoryboardTarget;
      if (this.cachedActiveViewStoryboard != null && this.cachedActiveViewStoryboard.Target != null)
      {
        this.cachedActiveViewStoryboard.CurrentTimeChanged -= new EventHandler<EventArgs>(this.ActiveTimeline_Seeked);
        this.cachedActiveViewStoryboard.Remove();
      }
      this.cachedActiveViewStoryboard = viewStoryboard;
      if (disableMedia)
        this.isActiveStoryboardMediaDirty = true;
      if (this.cachedActiveViewStoryboard == null || storyboardTarget == null)
        return;
      this.cachedActiveViewStoryboard.CurrentTimeChanged += new EventHandler<EventArgs>(this.ActiveTimeline_Seeked);
      IViewObject context = (IViewObject) null;
      FrameworkTemplateElement frameworkTemplateElement = this.ActiveStoryboardContainer as FrameworkTemplateElement;
      if (frameworkTemplateElement != null)
        context = frameworkTemplateElement.ViewObject;
      try
      {
        ViewStoryboardApplyOptions flags = (disableMedia ? ViewStoryboardApplyOptions.RemoveMedia : ViewStoryboardApplyOptions.None) | this.viewModel.DefaultView.StoryboardApplyOptions;
        this.cachedActiveViewStoryboard.Apply(context, storyboardTarget, flags);
        this.cachedActiveViewStoryboard.Play();
        this.cachedActiveViewStoryboard.Pause();
        this.cachedActiveViewStoryboard.Seek(this.AnimationTime);
        this.viewModel.DefaultView.OnActiveStoryboardTimelineInstanceUpdated(this.cachedActiveViewStoryboard);
      }
      catch (Exception ex)
      {
        SceneView defaultView = this.viewModel.DefaultView;
        if (defaultView != null)
          defaultView.SetViewException(ex);
        this.Invalidate();
      }
    }

    private void ActiveTimeline_Seeked(object sender, EventArgs e)
    {
      if (this.viewModel == null)
        return;
      if (!this.viewModel.IsEditable)
      {
        this.Invalidate();
        this.viewModel.Document.OnUpdatedEditTransaction();
      }
      else
      {
        if (!this.GetIsCurrentTimeValid())
          return;
        this.viewModel.Document.OnUpdatedEditTransaction();
      }
    }

    public void ApplyAnimation(SceneElement element, TimelineSceneNode animation, double baseKeyframeOffset, IList<SceneNode> outKeyframes)
    {
      if (this.ActiveStoryboardTimeline == null)
        return;
      StoryboardTimelineSceneNode storyboardTimeline = this.ActiveStoryboardTimeline;
      IStoryboardContainer storyboardContainer = element.StoryboardContainer;
      PropertyReference targetProperty = animation.TargetProperty;
      if (targetProperty == null || !this.CoercePropertyIntoExistence(element, targetProperty))
        return;
      bool flag = true;
      TimelineSceneNode timelineSceneNode = (TimelineSceneNode) storyboardTimeline.GetAnimation((SceneNode) element, targetProperty);
      if (timelineSceneNode != null)
      {
        if (this.MergeAnimation(timelineSceneNode, animation, baseKeyframeOffset, outKeyframes))
          flag = false;
        else
          this.RemoveAnimation(this.ActiveStoryboardTimeline, timelineSceneNode);
      }
      if (!flag)
        return;
      TimelineSceneNode animation1 = (TimelineSceneNode) this.viewModel.GetSceneNode(animation.DocumentNode.Clone(this.ActiveStoryboardTimeline.DocumentContext));
      animation1.TargetElement = (SceneNode) element;
      KeyFrameAnimationSceneNode animationSceneNode = animation1 as KeyFrameAnimationSceneNode;
      if (animationSceneNode != null)
      {
        animationSceneNode.ClearKeyFrames();
        this.MergeAnimation((TimelineSceneNode) animationSceneNode, animation, baseKeyframeOffset, outKeyframes);
      }
      this.InsertAnimation(this.ActiveStoryboardTimeline, animation1);
    }

    private bool CoercePropertyIntoExistence(SceneElement element, PropertyReference property)
    {
      if (element.ViewObject == null)
        return false;
      if (property.IsValidPath(element.ViewObject, (ITypeResolver) this.viewModel.ProjectContext))
        return true;
      Base2DElement base2Delement = element as Base2DElement;
      Base3DElement base3Delement = element as Base3DElement;
      IPlatform platform = element.Platform;
      if (base2Delement != null && property.Count > 1)
      {
        if (Base2DElement.RenderTransformProperty.Equals((object) property[0]) && property.Subreference(1).IsValidPath(platform.ViewObjectFactory.Instantiate(new CanonicalTransform().GetPlatformTransform(platform.GeometryHelper)), platform.Metadata.DefaultTypeResolver))
        {
          base2Delement.EnsureRenderTransform();
          return true;
        }
      }
      else if (base3Delement != null && property.Count > 1)
      {
        ReferenceStep referenceStep = property[0];
        if ((Visual3DElement.TransformProperty.Equals((object) referenceStep) || ModelVisual3DElement.TransformProperty.Equals((object) referenceStep)) && (Visual3DElement.TransformProperty.Equals((object) base3Delement.TransformPropertyId) || ModelVisual3DElement.TransformProperty.Equals((object) base3Delement.TransformPropertyId)) || Model3DElement.TransformProperty.Equals((object) referenceStep) && Model3DElement.TransformProperty.Equals((object) base3Delement.TransformPropertyId))
        {
          PropertyReference propertyReference = property.Subreference(1);
          CanonicalTransform3D canonicalTransform3D = new CanonicalTransform3D();
          if (propertyReference.IsValidPath(platform.ViewObjectFactory.Instantiate((object) canonicalTransform3D.ToTransform()), platform.Metadata.DefaultTypeResolver))
          {
            base3Delement.SetValue(base3Delement.TransformPropertyId, (object) canonicalTransform3D.ToTransform());
            return true;
          }
        }
      }
      return false;
    }

    private bool MergeAnimation(TimelineSceneNode existingAnimation, TimelineSceneNode newAnimation, double baseKeyframeOffset, IList<SceneNode> outKeyframes)
    {
      KeyFrameAnimationSceneNode animationSceneNode1 = existingAnimation as KeyFrameAnimationSceneNode;
      KeyFrameAnimationSceneNode animationSceneNode2 = newAnimation as KeyFrameAnimationSceneNode;
      if (animationSceneNode1 == null || animationSceneNode2 == null)
        return false;
      foreach (SceneNode sceneNode in animationSceneNode2.KeyFrames)
      {
        KeyFrameSceneNode keyFrameSceneNode = (KeyFrameSceneNode) this.viewModel.GetSceneNode(sceneNode.DocumentNode.Clone(existingAnimation.DocumentContext));
        keyFrameSceneNode.Time = keyFrameSceneNode.Time - baseKeyframeOffset + this.seekedTime;
        animationSceneNode1.AddKeyFrame(keyFrameSceneNode);
        KeyFrameSceneNode keyFrameAtTime = animationSceneNode1.GetKeyFrameAtTime(keyFrameSceneNode.Time);
        if (keyFrameAtTime != null)
          outKeyframes.Add((SceneNode) keyFrameAtTime);
      }
      return true;
    }

    private bool ShouldScanForNamedValues(ITypeId valueType)
    {
      if (!PlatformTypes.Style.IsAssignableFrom(valueType) && !PlatformTypes.FrameworkTemplate.IsAssignableFrom(valueType))
        return !PlatformTypes.UIElementCollection.IsAssignableFrom(valueType);
      return false;
    }

    private bool ShouldCheckForName(ITypeId type)
    {
      return !PlatformTypes.UIElement.IsAssignableFrom(type);
    }

    private void AddNamedValuesForAllPropertiesToList(SceneNode node, IList<SceneNode> list)
    {
      if (!this.ShouldScanForNamedValues((ITypeId) node.Type))
        return;
      DocumentCompositeNode documentCompositeNode = node.DocumentNode as DocumentCompositeNode;
      if (documentCompositeNode == null)
        return;
      foreach (KeyValuePair<IProperty, DocumentNode> keyValuePair in (IEnumerable<KeyValuePair<IProperty, DocumentNode>>) documentCompositeNode.Properties)
      {
        DocumentNode node1 = keyValuePair.Value;
        if (this.ShouldScanForNamedValues((ITypeId) keyValuePair.Key.PropertyType))
          this.AddNamedValuesToList(node.ViewModel, node1, list);
      }
    }

    private void AddNamedValuesToList(SceneViewModel viewModel, DocumentNode node, IList<SceneNode> list)
    {
      if (node == null)
        return;
      DocumentNode node1 = node;
      DocumentCompositeNode documentCompositeNode = node as DocumentCompositeNode;
      using (IEnumerator<DocumentNode> enumerator = documentCompositeNode != null ? documentCompositeNode.DescendantNodes.GetEnumerator() : (IEnumerator<DocumentNode>) null)
      {
        for (; node1 != null; node1 = enumerator == null || !enumerator.MoveNext() ? (DocumentNode) null : enumerator.Current)
        {
          if (this.ShouldCheckForName((ITypeId) node1.Type) && node1.Name != null)
          {
            SceneNode sceneNode = viewModel.GetSceneNode(node1);
            list.Add(sceneNode);
          }
        }
      }
    }

    private static bool SetAnimationValueToKeyFrame(IDocumentContext context, DocumentCompositeNode animationNode, IPropertyId sourceProperty, DocumentCompositeNode keyFrameNode, IPropertyId valueProperty)
    {
      if (animationNode == null || sourceProperty == null || (keyFrameNode == null || valueProperty == null))
        return false;
      DocumentNode documentNode = animationNode.Properties[sourceProperty];
      if (documentNode != null && documentNode.Type.NullableType != null)
        documentNode = (DocumentNode) context.CreateNode((ITypeId) documentNode.Type.NullableType, ((DocumentPrimitiveNode) documentNode).Value);
      if (documentNode == null)
        return false;
      keyFrameNode.Properties[valueProperty] = documentNode.Clone(context);
      return true;
    }

    public static void ConvertFromToAnimations(DocumentNode rootNode)
    {
      List<DocumentNode> list = new List<DocumentNode>();
      DurationConverter durationConverter = new DurationConverter();
      foreach (DocumentNode documentNode1 in rootNode.DescendantNodes)
      {
        ITypeId typeId1 = (ITypeId) null;
        ITypeId typeId2 = (ITypeId) null;
        if (PlatformTypes.Timeline.IsAssignableFrom((ITypeId) documentNode1.Type))
        {
          if (PlatformTypes.DoubleAnimation.Equals((object) documentNode1.Type))
          {
            typeId1 = PlatformTypes.SplineDoubleKeyFrame;
            typeId2 = PlatformTypes.DoubleAnimationUsingKeyFrames;
          }
          if (PlatformTypes.ColorAnimation.Equals((object) documentNode1.Type))
          {
            typeId1 = PlatformTypes.SplineColorKeyFrame;
            typeId2 = PlatformTypes.ColorAnimationUsingKeyFrames;
          }
          if (PlatformTypes.PointAnimation.Equals((object) documentNode1.Type))
          {
            typeId1 = PlatformTypes.SplinePointKeyFrame;
            typeId2 = PlatformTypes.PointAnimationUsingKeyFrames;
          }
        }
        if (typeId2 != null)
        {
          IDocumentContext context = documentNode1.Context;
          ITypeId typeId3 = (ITypeId) context.TypeResolver.ResolveType(typeId2);
          if (typeId3 != null)
          {
            DocumentCompositeNode animationNode = (DocumentCompositeNode) documentNode1;
            if (typeId2 != null && typeId1 != null)
            {
              DocumentCompositeNode node1 = context.CreateNode(typeId2);
              node1.Properties[StoryboardTimelineSceneNode.TargetNameProperty] = animationNode.Properties[StoryboardTimelineSceneNode.TargetNameProperty].Clone(context);
              node1.Properties[StoryboardTimelineSceneNode.TargetPropertyProperty] = animationNode.Properties[StoryboardTimelineSceneNode.TargetPropertyProperty].Clone(context);
              foreach (KeyValuePair<IProperty, DocumentNode> keyValuePair in (IEnumerable<KeyValuePair<IProperty, DocumentNode>>) animationNode.Properties)
              {
                if (PlatformTypes.Timeline.Equals((object) keyValuePair.Key.DeclaringTypeId) && !TimelineSceneNode.DurationProperty.Equals((object) keyValuePair.Key))
                  node1.Properties[(IPropertyId) keyValuePair.Key] = keyValuePair.Value.Clone(context);
              }
              DocumentCompositeNode keyFrameNode = (DocumentCompositeNode) null;
              IPropertyId valueProperty = (IPropertyId) typeId1.GetMember(MemberType.LocalProperty, "Value", MemberAccessTypes.Public);
              IPropertyId index = (IPropertyId) typeId1.GetMember(MemberType.LocalProperty, "KeyTime", MemberAccessTypes.Public);
              IPropertyId sourceProperty1 = (IPropertyId) animationNode.Type.GetMember(MemberType.LocalProperty, "From", MemberAccessTypes.Public);
              if (animationNode.Properties[sourceProperty1] != null)
              {
                keyFrameNode = context.CreateNode(typeId1);
                if (AnimationEditor.SetAnimationValueToKeyFrame(context, animationNode, sourceProperty1, keyFrameNode, valueProperty))
                {
                  DocumentNode documentNode2 = (DocumentNode) context.CreateNode(PlatformTypes.KeyTime, (IDocumentNodeValue) new DocumentNodeStringValue("0"));
                  keyFrameNode.Properties[index] = documentNode2;
                }
                else
                  continue;
              }
              DocumentCompositeNode node2 = context.CreateNode(typeId1);
              IPropertyId sourceProperty2 = (IPropertyId) animationNode.Type.GetMember(MemberType.LocalProperty, "To", MemberAccessTypes.Public);
              if (AnimationEditor.SetAnimationValueToKeyFrame(context, animationNode, sourceProperty2, node2, valueProperty))
              {
                DocumentNode node3 = animationNode.Properties[TimelineSceneNode.DurationProperty];
                if (node3 != null)
                {
                  bool flag = false;
                  DocumentPrimitiveNode documentPrimitiveNode = node3 as DocumentPrimitiveNode;
                  if (documentPrimitiveNode != null && !DocumentNodeUtilities.IsBinding(node3) && !DocumentNodeUtilities.IsDynamicResource(node3))
                  {
                    if (!DocumentNodeUtilities.IsStaticResource(node3))
                    {
                      try
                      {
                        string text = documentPrimitiveNode.GetValue<string>();
                        if (((Duration) durationConverter.ConvertFromString(text)).HasTimeSpan)
                        {
                          DocumentNode documentNode2 = (DocumentNode) context.CreateNode(PlatformTypes.KeyTime, (IDocumentNodeValue) new DocumentNodeStringValue(text));
                          node2.Properties[index] = documentNode2;
                          flag = true;
                        }
                      }
                      catch
                      {
                      }
                    }
                  }
                  if (!flag)
                    continue;
                }
                else if (keyFrameNode == null)
                {
                  DocumentNode documentNode2 = (DocumentNode) context.CreateNode(PlatformTypes.KeyTime, (IDocumentNodeValue) new DocumentNodeStringValue("0"));
                  node2.Properties[index] = documentNode2;
                }
                else
                  continue;
                IProperty property = (IProperty) typeId3.GetMember(MemberType.LocalProperty, "KeyFrames", MemberAccessTypes.Public);
                Type propertyType = PlatformTypeHelper.GetPropertyType(property);
                DocumentCompositeNode node4 = context.CreateNode(propertyType);
                node1.Properties[(IPropertyId) property] = (DocumentNode) node4;
                if (keyFrameNode != null)
                  node4.Children.Add((DocumentNode) keyFrameNode);
                node4.Children.Add((DocumentNode) node2);
                documentNode1.Parent.Children.Add((DocumentNode) node1);
                list.Add(documentNode1);
              }
            }
          }
        }
      }
      foreach (DocumentNode documentNode in list)
        documentNode.Parent.Children.Remove(documentNode);
      list.Clear();
    }

    private sealed class DeferKeyFramingToken : IDisposable
    {
      private AnimationEditor animationEditor;
      private bool disposed;

      public DeferKeyFramingToken(AnimationEditor animationEditor)
      {
        this.animationEditor = animationEditor;
        ++this.animationEditor.deferKeyFrameCount;
      }

      public void Dispose()
      {
        if (this.disposed)
          return;
        --this.animationEditor.deferKeyFrameCount;
        this.disposed = true;
      }
    }

    private class ChildStoryboardEnumerable : IEnumerable<StoryboardTimelineSceneNode>, IEnumerable
    {
      private AnimationEditor editor;
      private IStoryboardContainer container;

      public ChildStoryboardEnumerable(AnimationEditor editor, IStoryboardContainer container)
      {
        this.editor = editor;
        this.container = container;
      }

      public IEnumerator<StoryboardTimelineSceneNode> GetEnumerator()
      {
        return (IEnumerator<StoryboardTimelineSceneNode>) new AnimationEditor.ChildStoryboardEnumerable.ChildStoryboardEnumerator(this);
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) new AnimationEditor.ChildStoryboardEnumerable.ChildStoryboardEnumerator(this);
      }

      private class ChildStoryboardEnumerator : IEnumerator<StoryboardTimelineSceneNode>, IDisposable, IEnumerator
      {
        private int curIndex = -1;
        private AnimationEditor.ChildStoryboardEnumerable parent;

        public StoryboardTimelineSceneNode Current
        {
          get
          {
            if (this.curIndex < 0 || this.curIndex > this.parent.editor.storyboardNodes.Count)
              throw new InvalidOperationException();
            return this.parent.editor.storyboardNodes[this.curIndex];
          }
        }

        object IEnumerator.Current
        {
          get
          {
            return (object) this.Current;
          }
        }

        public ChildStoryboardEnumerator(AnimationEditor.ChildStoryboardEnumerable parent)
        {
          this.parent = parent;
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
          ++this.curIndex;
          while (this.curIndex < this.parent.editor.storyboardNodes.Count && this.parent.editor.storyboardNodes[this.curIndex].StoryboardContainer != this.parent.container)
            ++this.curIndex;
          return this.curIndex < this.parent.editor.storyboardNodes.Count;
        }

        public void Reset()
        {
          this.curIndex = -1;
        }
      }
    }
  }
}
