// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Documents.MediaDocumentType
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Documents
{
  internal abstract class MediaDocumentType : AssetDocumentType
  {
    private const string MediaDocumentBuildTask = "Content";

    protected override string ImagePath
    {
      get
      {
        return "Resources\\Documents\\Wmv.png";
      }
    }

    public override string Name
    {
      get
      {
        return "MediaElement";
      }
    }

    public override string Description
    {
      get
      {
        return StringTable.MediaDocumentTypeDescription;
      }
    }

    protected override string FileNameBase
    {
      get
      {
        return StringTable.MediaDocumentTypeFileNameBase;
      }
    }

    protected override string DefaultBuildTask
    {
      get
      {
        return "Content";
      }
    }

    protected override IDictionary<string, string> MetadataInformation
    {
      get
      {
        return (IDictionary<string, string>) new Dictionary<string, string>()
        {
          {
            "CopyToOutputDirectory",
            "PreserveNewest"
          }
        };
      }
    }

    public MediaDocumentType(DesignerContext designerContext)
      : base(designerContext)
    {
    }

    public override bool CanInsertTo(IProjectItem projectItem, IView view, ISceneInsertionPoint insertionPoint)
    {
      if (!base.CanInsertTo(projectItem, view, insertionPoint))
        return false;
      bool flag = false;
      SceneView sceneView = view as SceneView;
      if (sceneView != null)
      {
        SceneViewModel viewModel = sceneView.ViewModel;
        if (insertionPoint != null)
        {
          SceneElement sceneElement = insertionPoint.SceneElement;
          if (!(sceneElement is Viewport3DElement) && !(sceneElement is Base3DElement))
          {
            string resourceReference = viewModel.Document.DocumentContext.MakeResourceReference(projectItem.DocumentReference.Path);
            flag = !projectItem.IsComponentUri(resourceReference);
          }
        }
      }
      return flag;
    }

    protected override IList<SceneElement> AddToDocumentInternal(string importedFilePath, SceneElement element, ISceneInsertionPoint insertionPoint, SceneViewModel sceneViewModel, SceneEditTransaction editTransaction)
    {
      MediaSceneElement mediaSceneElement = element as MediaSceneElement;
      IList<SceneElement> list = (IList<SceneElement>) new List<SceneElement>();
      if (mediaSceneElement != null)
      {
        double num = (double) mediaSceneElement.GetLocalOrDefaultValueAsWpf(DesignTimeProperties.DesignTimeNaturalDurationProperty);
        mediaSceneElement.ClearLocalValue(DesignTimeProperties.DesignTimeNaturalDurationProperty);
        if (insertionPoint.CanInsert((ITypeId) mediaSceneElement.Type))
        {
          if (!JoltHelper.TypeSupported((ITypeResolver) element.ProjectContext, PlatformTypes.MediaTimeline))
          {
            Uri uri = new Uri(importedFilePath, UriKind.RelativeOrAbsolute);
            mediaSceneElement.SetLocalValueAsWpf(MediaSceneElement.SourceProperty, (object) uri);
          }
          else
          {
            IStoryboardContainer mediaStoryboardContainer = insertionPoint.SceneNode.StoryboardContainer;
            StoryboardTimelineSceneNode targetStoryboard = (StoryboardTimelineSceneNode) null;
            sceneViewModel.EditContextManager.SingleViewModelEditContextWalker.Walk(false, (SingleHistoryCallback) ((context, isGhosted) =>
            {
              if (context.StoryboardContainer != mediaStoryboardContainer)
                return false;
              targetStoryboard = context.Timeline;
              return true;
            }));
            if (targetStoryboard == null)
            {
              foreach (TriggerBaseNode triggerBaseNode in (IEnumerable<TriggerBaseNode>) mediaStoryboardContainer.VisualTriggers)
              {
                EventTriggerNode eventTriggerNode = triggerBaseNode as EventTriggerNode;
                if (eventTriggerNode != null && eventTriggerNode.RoutedEvent == FrameworkElement.LoadedEvent)
                {
                  foreach (SceneNode sceneNode in (IEnumerable<SceneNode>) eventTriggerNode.Actions)
                  {
                    TimelineActionNode timelineActionNode = sceneNode as TimelineActionNode;
                    if (timelineActionNode != null && timelineActionNode.TimelineOperation == TimelineOperation.Begin)
                    {
                      targetStoryboard = timelineActionNode.TargetTimeline;
                      if (!TimelinePane.ShouldExposeStoryboardToUser((SceneNode) targetStoryboard))
                        targetStoryboard = (StoryboardTimelineSceneNode) null;
                      else
                        break;
                    }
                  }
                }
                if (targetStoryboard != null)
                  break;
              }
              if (targetStoryboard != null)
                sceneViewModel.SetActiveStoryboardTimeline(mediaStoryboardContainer, targetStoryboard, (TriggerBaseNode) null);
            }
            if (targetStoryboard == null)
            {
              targetStoryboard = sceneViewModel.AnimationEditor.CreateNewTimeline(sceneViewModel.ActiveStoryboardContainer, element.Name, TriggerCreateBehavior.Default, true);
              sceneViewModel.AnimationEditor.IsRecording = false;
            }
            IType type = this.DesignerContext.ActiveDocument.ProjectContext.ResolveType(PlatformTypes.MediaTimeline);
            MediaTimelineSceneNode timelineSceneNode = (MediaTimelineSceneNode) sceneViewModel.CreateSceneNode(type.RuntimeType);
            timelineSceneNode.SetLocalValue(DesignTimeProperties.DesignTimeNaturalDurationProperty, (object) num);
            Uri uri = new Uri(importedFilePath, UriKind.RelativeOrAbsolute);
            timelineSceneNode.Source = uri;
            timelineSceneNode.TargetName = element.Name;
            double animationTime = sceneViewModel.AnimationEditor.AnimationTime;
            timelineSceneNode.Begin = animationTime;
            targetStoryboard.Children.Add((TimelineSceneNode) timelineSceneNode);
            editTransaction.Update();
          }
          list.Add(element);
          insertionPoint.Insert((SceneNode) mediaSceneElement);
        }
      }
      return list;
    }

    protected override SceneElement CreateElement(SceneViewModel viewModel, ISceneInsertionPoint insertionPoint, string importedFilePath)
    {
      BaseFrameworkElement frameworkElement = (BaseFrameworkElement) null;
      Uri mediaUri = viewModel.XamlDocument.DocumentContext.MakeDesignTimeUri(new Uri(importedFilePath, UriKind.RelativeOrAbsolute));
      MediaOpener mediaOpener = new MediaOpener(importedFilePath, mediaUri, this.DesignerContext.MessageDisplayService);
      bool? nullable = mediaOpener.OpenMedia();
      bool flag = false;
      if (nullable.HasValue && nullable.Value)
      {
        IType type = this.DesignerContext.ActiveDocument.ProjectContext.ResolveType(PlatformTypes.MediaElement);
        frameworkElement = (BaseFrameworkElement) (viewModel.CreateSceneNode(type.RuntimeType) as MediaSceneElement);
        new SceneNodeIDHelper(viewModel, insertionPoint.SceneNode ?? viewModel.ViewRoot).SetValidName((SceneNode) frameworkElement, Path.GetFileName(importedFilePath));
        if (mediaOpener.Player.HasVideo)
        {
          frameworkElement.Height = (double) mediaOpener.Player.NaturalVideoHeight;
          frameworkElement.Width = (double) mediaOpener.Player.NaturalVideoWidth;
        }
        else if (mediaOpener.Player.HasAudio)
        {
          frameworkElement.Height = 0.0;
          frameworkElement.Width = 0.0;
        }
        Duration naturalDuration = mediaOpener.Player.NaturalDuration;
        if (naturalDuration.HasTimeSpan)
          frameworkElement.SetLocalValue(DesignTimeProperties.DesignTimeNaturalDurationProperty, (object) naturalDuration.TimeSpan.TotalSeconds);
        else
          flag = true;
        frameworkElement.SetValueAsWpf(MediaSceneElement.StretchProperty, (object) Stretch.Fill);
      }
      else
        flag = true;
      if (frameworkElement != null && flag)
        frameworkElement.SetLocalValue(DesignTimeProperties.DesignTimeNaturalDurationProperty, (object) 0.0);
      return (SceneElement) frameworkElement;
    }

    protected override bool DoesNodeReferenceUrl(DocumentNode node, string url)
    {
      return false;
    }

    protected override double GetNativeWidth(SceneElement element)
    {
      MediaSceneElement mediaSceneElement = element as MediaSceneElement;
      if (mediaSceneElement != null && mediaSceneElement.IsViewObjectValid)
        return mediaSceneElement.Width;
      return base.GetNativeWidth(element);
    }

    protected override double GetNativeHeight(SceneElement element)
    {
      MediaSceneElement mediaSceneElement = element as MediaSceneElement;
      if (mediaSceneElement != null && mediaSceneElement.IsViewObjectValid)
        return mediaSceneElement.Height;
      return base.GetNativeWidth(element);
    }

    protected override void RefreshInstance(SceneElement element, SceneDocument sceneDocument, string url)
    {
    }
  }
}
