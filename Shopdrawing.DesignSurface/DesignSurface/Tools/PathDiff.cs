// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.PathDiff
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools
{
  public class PathDiff
  {
    private ReferenceStep pathProperty;
    private Base2DElement targetElement;
    private PathDiffChangeList changeList;
    private IPlatformTypes platformMetadata;

    public PathDiff(Base2DElement targetElement, IPropertyId pathProperty, PathDiffChangeList changeList)
    {
      this.targetElement = targetElement;
      this.platformMetadata = targetElement.Platform.Metadata;
      this.pathProperty = this.platformMetadata.ResolveProperty(pathProperty) as ReferenceStep;
      this.changeList = changeList;
    }

    private static DependencyPropertyReferenceStep GetReferenceStepFromWpf(DesignerContext designerContext, IPlatform targetPlatform, Type targetType, DependencyProperty dp)
    {
      IPlatform defaultPlatform = designerContext.DesignerDefaultPlatformService.DefaultPlatform;
      return designerContext.PlatformConverter.ConvertFromWpfPropertyReference(new PropertyReference((ReferenceStep) DependencyPropertyReferenceStep.GetReferenceStep(defaultPlatform.Metadata.DefaultTypeResolver, targetType, dp)), (IPlatformMetadata) targetPlatform.Metadata).FirstStep as DependencyPropertyReferenceStep;
    }

    public void SetPathUsingMinimalDiff(PathGeometry path)
    {
      PathGeometryUtilities.EnsureOnlySingleSegmentsInGeometry(path);
      PathGeometry pathGeometry = this.targetElement.GetComputedValueAsWpf((IPropertyId) this.pathProperty) as PathGeometry;
      if (pathGeometry != null && path != null)
      {
        PathGeometryUtilities.EnsureOnlySingleSegmentsInGeometry(pathGeometry);
        if (this.changeList.Changes.Count == 0 && this.DoesStructureMatch(path, pathGeometry))
          this.SetPathGeometryUsingMinimalDiff(path, pathGeometry);
        else
          this.SetPathUsingMapping(path);
      }
      else
        this.targetElement.SetValueAsWpf((IPropertyId) this.pathProperty, (object) path);
    }

    private void SetPathUsingMapping(PathGeometry path)
    {
      SceneViewModel viewModel = this.targetElement.ViewModel;
      using (viewModel.AnimationProxyManager != null ? viewModel.AnimationProxyManager.ExpandAllProxiesInActiveContainer() : (IDisposable) null)
      {
        foreach (PathAction action in this.changeList.Changes)
        {
            System.Windows.Media.Geometry geometry = this.targetElement.GetLocalOrDefaultValueAsWpf((IPropertyId)this.pathProperty) as System.Windows.Media.Geometry;
          if (geometry == null && this.targetElement.IsValueExpression((IPropertyId) this.pathProperty))
          {
              geometry = this.targetElement.ViewModel.DefaultView.ConvertToWpfValue(this.targetElement.ViewModel.CreateInstance(this.targetElement.GetLocalValueAsSceneNode((IPropertyId)this.pathProperty).DocumentNodePath)) as System.Windows.Media.Geometry;
            if (geometry == null)
            {
                geometry = this.targetElement.GetComputedValueAsWpf((IPropertyId)this.pathProperty) as System.Windows.Media.Geometry;
              if (geometry == null)
                return;
            }
          }
          PathGeometry oldGeometry = new PathGeometry();
          oldGeometry.AddGeometry(geometry.Clone());
          PathGeometry pathGeometry = new PathGeometry();
          pathGeometry.AddGeometry(geometry);
          PathFigureEditor pathFigureEditor = new PathFigureEditor(pathGeometry.Figures[action.Figure]);
          PathGeometryEditor pathGeometryEditor = new PathGeometryEditor(pathGeometry);
          switch (action.Action)
          {
            case PathActionType.InsertPoint:
              pathFigureEditor.SubdivideSegment(action.PointIndex, action.Parameter);
              break;
            case PathActionType.DeletePoint:
              pathFigureEditor.RemovePoint(action.PointIndex);
              break;
            case PathActionType.DeleteSegment:
              if (action.Segment == 0 && pathFigureEditor.PathFigure.Segments.Count > 1)
              {
                pathFigureEditor.RemoveFirstSegment();
                break;
              }
              pathFigureEditor.RemoveLastSegment();
              break;
            case PathActionType.PromoteSegment:
              pathFigureEditor.PromoteSegment(action.PointIndex);
              break;
            case PathActionType.Rotate:
              pathFigureEditor.Rotate();
              break;
            case PathActionType.Open:
              pathFigureEditor.Open(action.PointIndex);
              break;
            case PathActionType.Split:
              pathFigureEditor.Split(action.PointIndex);
              break;
            case PathActionType.SplitAndAdd:
              pathGeometryEditor.SplitFigure(action.Figure, action.PointIndex);
              break;
            case PathActionType.RemoveFigure:
              pathGeometryEditor.RemoveFigure(action.Figure);
              break;
            case PathActionType.AppendSegment:
              PathAppendLineAction appendLineAction;
              if ((appendLineAction = action as PathAppendLineAction) != null)
              {
                Point point = appendLineAction.Point;
                if (action.Figure < path.Figures.Count && action.Segment < path.Figures[action.Figure].Segments.Count)
                {
                  LineSegment lineSegment = path.Figures[action.Figure].Segments[action.Segment] as LineSegment;
                  if (lineSegment != null)
                    point = lineSegment.Point;
                }
                pathFigureEditor.LineTo(point);
                break;
              }
              PathAppendQuadraticBezierAction quadraticBezierAction;
              if ((quadraticBezierAction = action as PathAppendQuadraticBezierAction) != null)
              {
                Point point1 = quadraticBezierAction.Point1;
                Point point2 = quadraticBezierAction.Point2;
                if (action.Figure < path.Figures.Count && action.Segment < path.Figures[action.Figure].Segments.Count)
                {
                  QuadraticBezierSegment quadraticBezierSegment = path.Figures[action.Figure].Segments[action.Segment] as QuadraticBezierSegment;
                  if (quadraticBezierSegment != null)
                  {
                    point1 = quadraticBezierSegment.Point1;
                    point2 = quadraticBezierSegment.Point2;
                  }
                }
                pathFigureEditor.QuadraticCurveTo(point1, point2);
                break;
              }
              PathAppendBezierAction appendBezierAction;
              if ((appendBezierAction = action as PathAppendBezierAction) != null)
              {
                Point point1 = appendBezierAction.Point1;
                Point point2 = appendBezierAction.Point2;
                Point point3 = appendBezierAction.Point3;
                if (action.Figure < path.Figures.Count && action.Segment < path.Figures[action.Figure].Segments.Count)
                {
                  BezierSegment bezierSegment = path.Figures[action.Figure].Segments[action.Segment] as BezierSegment;
                  if (bezierSegment != null)
                  {
                    point1 = bezierSegment.Point1;
                    point2 = bezierSegment.Point2;
                    point3 = bezierSegment.Point3;
                  }
                }
                pathFigureEditor.CubicCurveTo(point1, point2, point3);
                break;
              }
              break;
            case PathActionType.Close:
              pathFigureEditor.CloseWithLineSegment();
              break;
            case PathActionType.Join:
              pathGeometryEditor.JoinFigure(action.Figure, action.PointIndex);
              break;
            case PathActionType.Reverse:
              pathFigureEditor.Reverse();
              break;
          }
          this.targetElement.SetLocalValueAsWpf((IPropertyId) this.pathProperty, (object) pathGeometry);
          this.ApplyAnimationChanges(oldGeometry, pathGeometry, action);
          if (action.Action == PathActionType.PromoteSegment && action.Segment < pathGeometry.Figures[action.Figure].Segments.Count)
          {
            this.targetElement.ViewModel.Document.OnUpdatedEditTransaction();
            this.SetKeyframesForSegment(path, action.Figure, action.Segment);
          }
        }
        this.RemoveInvalidAnimations(PathGeometryUtilities.GetPathGeometryFromGeometry((System.Windows.Media.Geometry)this.targetElement.GetLocalOrDefaultValueAsWpf((IPropertyId)this.pathProperty)));
      }
    }

    private void SubdivideSegment(PathGeometry oldGeometry, int figure, int segment, int pointIndex, double parameter, Dictionary<TimelineSceneNode, StoryboardTimelineSceneNode> subdivisionProperties)
    {
      PropertyReference propertyReference1 = new PropertyReference(this.pathProperty).Append(PathElement.FiguresProperty).Append((ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep((IPlatformMetadata) this.platformMetadata, PlatformTypes.PathFigureCollection, figure));
      PropertyReference propertyReference2 = propertyReference1.Append(PathElement.PathFigureSegmentsProperty);
      PropertyReference propertyReference3 = segment != oldGeometry.Figures[figure].Segments.Count ? propertyReference2.Append((ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep((IPlatformMetadata) this.platformMetadata, PlatformTypes.PathSegmentCollection, segment)) : propertyReference1.Append(PathElement.PathFigureStartPointProperty);
      PropertyReference propertyReference4;
      if (segment == 0)
      {
        propertyReference4 = propertyReference1.Append(PathElement.PathFigureStartPointProperty);
      }
      else
      {
        ReferenceStep step = (ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep((IPlatformMetadata) this.platformMetadata, PlatformTypes.PathSegmentCollection, segment - 1);
        propertyReference4 = propertyReference2.Append(step).Append(this.LastPointProperty(oldGeometry.Figures[figure].Segments[segment - 1]));
      }
      PropertyReference propertyReference5 = (PropertyReference) null;
      if (segment == oldGeometry.Figures[figure].Segments.Count - 1 && PathFigureUtilities.IsClosed(oldGeometry.Figures[figure]) && PathFigureUtilities.IsCloseSegmentDegenerate(oldGeometry.Figures[figure]))
        propertyReference5 = propertyReference1.Append(PathElement.PathFigureStartPointProperty);
      foreach (StoryboardTimelineSceneNode timelineSceneNode1 in this.targetElement.ViewModel.AnimationEditor.EnumerateStoryboardsForContainer(this.targetElement.StoryboardContainer))
      {
        Dictionary<double, List<KeyFrameSceneNode>> keyFrames1 = new Dictionary<double, List<KeyFrameSceneNode>>();
        foreach (KeyValuePair<TimelineSceneNode, StoryboardTimelineSceneNode> keyValuePair in subdivisionProperties)
        {
          if (keyValuePair.Value == timelineSceneNode1)
          {
            KeyFrameAnimationSceneNode keyFrameNode = keyValuePair.Key as KeyFrameAnimationSceneNode;
            if (keyFrameNode != null)
              this.RecordKeyFrames(keyFrames1, keyFrameNode);
          }
        }
        Dictionary<double, List<KeyFrameSceneNode>> keyFrames2 = new Dictionary<double, List<KeyFrameSceneNode>>();
        foreach (TimelineSceneNode timelineSceneNode2 in (IEnumerable<TimelineSceneNode>) timelineSceneNode1.Children)
        {
          if (timelineSceneNode2.TargetElement == this.targetElement && timelineSceneNode2.TargetProperty != null && (propertyReference3.Equals((object) timelineSceneNode2.TargetProperty) || propertyReference3.IsPrefixOf(timelineSceneNode2.TargetProperty) || propertyReference4.Equals((object) timelineSceneNode2.TargetProperty) || propertyReference5 != null && propertyReference5.Equals((object) timelineSceneNode2.TargetProperty)))
          {
            KeyFrameAnimationSceneNode keyFrameNode = timelineSceneNode2 as KeyFrameAnimationSceneNode;
            if (keyFrameNode != null)
              this.RecordKeyFrames(keyFrames2, keyFrameNode);
          }
        }
        foreach (KeyValuePair<double, List<KeyFrameSceneNode>> keyValuePair in keyFrames2)
        {
          PathGeometry path = oldGeometry.Clone();
          PathGeometryEditor pathGeometryEditor = new PathGeometryEditor(path);
          foreach (KeyFrameSceneNode keyFrameSceneNode in keyValuePair.Value)
          {
            PropertyReference propertyReference6 = this.targetElement.ViewModel.DefaultView.ConvertToWpfPropertyReference(keyFrameSceneNode.TargetProperty.Subreference(1));
            object valueToSet = this.targetElement.ViewModel.DefaultView.ConvertToWpfValue(keyFrameSceneNode.Value);
            propertyReference6.SetValue((object) path, valueToSet);
          }
          pathGeometryEditor.SubdivideSegment(figure, pointIndex, parameter);
          List<KeyFrameSceneNode> list;
          if (keyFrames1.TryGetValue(keyValuePair.Key, out list))
          {
            foreach (KeyFrameSceneNode keyFrameSceneNode in list)
            {
              object obj = this.targetElement.ViewModel.DefaultView.ConvertFromWpfValue(this.targetElement.ViewModel.DefaultView.ConvertToWpfPropertyReference(keyFrameSceneNode.TargetProperty.Subreference(1)).GetCurrentValue((object) path));
              keyFrameSceneNode.Value = obj;
            }
          }
        }
      }
    }

    private void RecordKeyFrames(Dictionary<double, List<KeyFrameSceneNode>> keyFrames, KeyFrameAnimationSceneNode keyFrameNode)
    {
      PropertyReference targetProperty = keyFrameNode.TargetProperty;
      if (!PlatformTypes.Point.IsAssignableFrom((ITypeId) targetProperty.ValueTypeId))
        return;
      foreach (KeyFrameSceneNode keyFrameSceneNode in keyFrameNode.KeyFrames)
      {
        List<KeyFrameSceneNode> list;
        if (!keyFrames.TryGetValue(keyFrameSceneNode.Time, out list))
        {
          list = new List<KeyFrameSceneNode>();
          keyFrames[keyFrameSceneNode.Time] = list;
        }
        list.Add(keyFrameSceneNode);
      }
    }

    private void SetKeyframesForSegment(PathGeometry pathGeometry, int figure, int segment)
    {
      PathSegment pathSegment = pathGeometry.Figures[figure].Segments[segment];
      PathSegment currentSegment = (PathSegment) Activator.CreateInstance(pathSegment.GetType());
      this.SetSegmentUsingMinimalDiff(pathSegment, currentSegment, this.CreatePropertyReference(figure, segment));
    }

    private PropertyReference CreatePropertyReference(int figureIndex, int segmentIndex)
    {
      PropertyReference propertyReference = new PropertyReference((ReferenceStep) this.platformMetadata.ResolveProperty((IPropertyId) this.pathProperty)).Append(PathElement.FiguresProperty).Append((ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep((IPlatformMetadata) this.platformMetadata, PlatformTypes.PathFigureCollection, figureIndex));
      if (segmentIndex == PathStructureChange.StartPointIndex)
        return propertyReference.Append(PathElement.PathFigureStartPointProperty);
      return propertyReference.Append(PathElement.PathFigureSegmentsProperty).Append((ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep((IPlatformMetadata) this.platformMetadata, PlatformTypes.PathSegmentCollection, segmentIndex));
    }

    private IPropertyId LastPointProperty(PathSegment segment)
    {
      if (segment is LineSegment)
        return PathElement.LineSegmentPointProperty;
      if (segment is BezierSegment)
        return PathElement.BezierSegmentPoint3Property;
      if (segment is QuadraticBezierSegment)
        return PathElement.QuadraticBezierSegmentPoint2Property;
      if (segment is ArcSegment)
        return PathElement.ArcSegmentPointProperty;
      return (IPropertyId) null;
    }

    private void ApplyAnimationChanges(PathGeometry oldGeometry, PathGeometry newGeometry, PathAction action)
    {
      PropertyReference propertyReference1 = new PropertyReference(this.platformMetadata.ResolveProperty((IPropertyId) this.pathProperty) as ReferenceStep).Append(PathElement.FiguresProperty);
      ReferenceStep step1 = (ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep((IPlatformMetadata) this.platformMetadata, PlatformTypes.PathFigureCollection, action.Figure);
      PropertyReference propertyReference2 = propertyReference1.Append(step1);
      propertyReference2.Append(PathElement.PathFigureSegmentsProperty);
      Dictionary<TimelineSceneNode, StoryboardTimelineSceneNode> toRemove1 = new Dictionary<TimelineSceneNode, StoryboardTimelineSceneNode>();
      Dictionary<TimelineSceneNode, StoryboardTimelineSceneNode> dictionary1 = new Dictionary<TimelineSceneNode, StoryboardTimelineSceneNode>();
      Dictionary<TimelineSceneNode, StoryboardTimelineSceneNode> subdivisionProperties = new Dictionary<TimelineSceneNode, StoryboardTimelineSceneNode>();
      foreach (PathStructureChange pathStructureChange in action.PathStructureChanges)
      {
        int index1 = action.Figure;
        int index2 = action.Figure;
        PropertyReference propertyReference3 = propertyReference2;
        PropertyReference propertyReference4 = propertyReference2;
        if (pathStructureChange.OldFigureIndex != -1)
        {
          ReferenceStep step2 = (ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep((IPlatformMetadata) this.platformMetadata, PlatformTypes.PathFigureCollection, pathStructureChange.OldFigureIndex);
          propertyReference3 = propertyReference1.Append(step2);
          index1 = pathStructureChange.OldFigureIndex;
        }
        if (pathStructureChange.NewFigureIndex != -1)
        {
          ReferenceStep step2 = (ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep((IPlatformMetadata) this.platformMetadata, PlatformTypes.PathFigureCollection, pathStructureChange.NewFigureIndex);
          propertyReference4 = propertyReference1.Append(step2);
          index2 = pathStructureChange.NewFigureIndex;
        }
        PropertyReference propertyReference5 = propertyReference3.Append(PathElement.PathFigureSegmentsProperty);
        PropertyReference propertyReference6 = propertyReference4.Append(PathElement.PathFigureSegmentsProperty);
        PropertyReference propertyReference7 = (PropertyReference) null;
        bool flag1 = false;
        if (pathStructureChange.OldSegmentIndex == PathStructureChange.StartPointIndex)
        {
          propertyReference7 = propertyReference3.Append(PathElement.PathFigureStartPointProperty);
          flag1 = true;
        }
        else if (pathStructureChange.OldSegmentIndex != PathStructureChange.DeletedPointIndex)
        {
          ReferenceStep step2 = (ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep((IPlatformMetadata) this.platformMetadata, PlatformTypes.PathSegmentCollection, pathStructureChange.OldSegmentIndex);
          propertyReference7 = propertyReference5.Append(step2);
          if (pathStructureChange.OldPointProperty != null)
          {
            ReferenceStep step3 = (ReferenceStep) PathDiff.GetReferenceStepFromWpf(this.targetElement.DesignerContext, this.targetElement.Platform, pathStructureChange.OldPointProperty.OwnerType, pathStructureChange.OldPointProperty);
            propertyReference7 = propertyReference7.Append(step3);
            flag1 = true;
          }
          else if (pathStructureChange.NewSegmentIndex == PathStructureChange.StartPointIndex)
          {
            if (pathStructureChange.OldSegmentIndex < oldGeometry.Figures[index1].Segments.Count)
            {
              propertyReference7 = propertyReference7.Append(this.LastPointProperty(oldGeometry.Figures[index1].Segments[pathStructureChange.OldSegmentIndex]));
              flag1 = true;
            }
            else
              continue;
          }
        }
        PropertyReference propertyReference8 = (PropertyReference) null;
        bool flag2 = false;
        if (pathStructureChange.NewSegmentIndex == PathStructureChange.StartPointIndex)
        {
          propertyReference8 = propertyReference4.Append(PathElement.PathFigureStartPointProperty);
          flag2 = true;
        }
        else if (pathStructureChange.NewSegmentIndex != PathStructureChange.DeletedPointIndex)
        {
          ReferenceStep step2 = (ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep((IPlatformMetadata) this.platformMetadata, PlatformTypes.PathSegmentCollection, pathStructureChange.NewSegmentIndex);
          propertyReference8 = propertyReference6.Append(step2);
          if (pathStructureChange.NewPointProperty != null)
          {
            ReferenceStep step3 = (ReferenceStep) PathDiff.GetReferenceStepFromWpf(this.targetElement.DesignerContext, this.targetElement.Platform, pathStructureChange.NewPointProperty.OwnerType, pathStructureChange.NewPointProperty);
            propertyReference8 = propertyReference8.Append(step3);
            flag2 = true;
          }
          else if (pathStructureChange.OldSegmentIndex == PathStructureChange.StartPointIndex)
          {
            if (pathStructureChange.NewSegmentIndex < newGeometry.Figures[index2].Segments.Count)
            {
              propertyReference8 = propertyReference8.Append(this.LastPointProperty(newGeometry.Figures[index2].Segments[pathStructureChange.NewSegmentIndex]));
              flag2 = true;
            }
            else
              continue;
          }
        }
        Dictionary<TimelineSceneNode, StoryboardTimelineSceneNode> toRemove2 = pathStructureChange.PathChangeType == PathChangeType.Move ? toRemove1 : (Dictionary<TimelineSceneNode, StoryboardTimelineSceneNode>) null;
        Dictionary<TimelineSceneNode, StoryboardTimelineSceneNode> toAdd = pathStructureChange.PathChangeType == PathChangeType.InferSubdivision ? subdivisionProperties : dictionary1;
        if (pathStructureChange.NewSegmentIndex == PathStructureChange.DeletedPointIndex)
          this.RemoveAnimation(propertyReference7, toRemove1);
        else if (pathStructureChange.NewSegmentIndex == PathStructureChange.StartPointIndex || pathStructureChange.OldSegmentIndex == PathStructureChange.StartPointIndex)
        {
          this.ChangeAnimationProperty(propertyReference7, propertyReference8, toAdd, toRemove2);
          if (pathStructureChange.NewSegmentIndex == PathStructureChange.StartPointIndex && pathStructureChange.NewSegmentIndex != pathStructureChange.OldSegmentIndex)
            this.RemoveAnimation(propertyReference8, toRemove1);
        }
        else if (pathStructureChange.OldSegmentIndex >= 0 && pathStructureChange.OldSegmentIndex < oldGeometry.Figures[index1].Segments.Count && (pathStructureChange.NewSegmentIndex >= 0 && pathStructureChange.NewSegmentIndex < newGeometry.Figures[index2].Segments.Count))
        {
          if (oldGeometry.Figures[index1].Segments[pathStructureChange.OldSegmentIndex].GetType() != newGeometry.Figures[index2].Segments[pathStructureChange.NewSegmentIndex].GetType())
          {
            if (!flag1)
              propertyReference7 = propertyReference7.Append(this.LastPointProperty(oldGeometry.Figures[index1].Segments[pathStructureChange.OldSegmentIndex]));
            if (!flag2)
              propertyReference8 = propertyReference8.Append(this.LastPointProperty(newGeometry.Figures[index2].Segments[pathStructureChange.NewSegmentIndex]));
            this.ChangeAnimationProperty(propertyReference7, propertyReference8, toAdd, toRemove2);
            if (pathStructureChange.OldPointProperty == null && pathStructureChange.PathChangeType == PathChangeType.Move)
            {
              ReferenceStep step2 = (ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep((IPlatformMetadata) this.platformMetadata, PlatformTypes.PathSegmentCollection, pathStructureChange.OldSegmentIndex);
              this.RemoveAnimation(propertyReference5.Append(step2), toRemove1);
            }
          }
          else if (pathStructureChange.OldPointProperty == null && pathStructureChange.NewPointProperty == null)
          {
            this.ChangeAnimationProperty(propertyReference7, propertyReference8, toAdd, toRemove2, false);
          }
          else
          {
            if (!flag1)
              propertyReference7 = propertyReference7.Append(this.LastPointProperty(oldGeometry.Figures[index1].Segments[pathStructureChange.OldSegmentIndex]));
            if (!flag2)
              propertyReference8 = propertyReference8.Append(this.LastPointProperty(newGeometry.Figures[index2].Segments[pathStructureChange.NewSegmentIndex]));
            this.ChangeAnimationProperty(propertyReference7, propertyReference8, toAdd, toRemove2);
          }
        }
      }
      if (action.Action == PathActionType.InsertPoint)
        this.SubdivideSegment(oldGeometry, action.Figure, action.Segment, action.PointIndex, action.Parameter, subdivisionProperties);
      foreach (KeyValuePair<TimelineSceneNode, StoryboardTimelineSceneNode> keyValuePair in toRemove1)
        keyValuePair.Value.Children.Remove(keyValuePair.Key);
      foreach (KeyValuePair<TimelineSceneNode, StoryboardTimelineSceneNode> keyValuePair in dictionary1)
        keyValuePair.Value.Children.Add(keyValuePair.Key);
      Dictionary<StoryboardTimelineSceneNode, HashSet<TimelineSceneNode.PropertyNodePair>> dictionary2 = new Dictionary<StoryboardTimelineSceneNode, HashSet<TimelineSceneNode.PropertyNodePair>>();
      foreach (KeyValuePair<TimelineSceneNode, StoryboardTimelineSceneNode> keyValuePair in subdivisionProperties)
      {
        HashSet<TimelineSceneNode.PropertyNodePair> hashSet = (HashSet<TimelineSceneNode.PropertyNodePair>) null;
        if (!dictionary2.TryGetValue(keyValuePair.Value, out hashSet))
        {
          hashSet = new HashSet<TimelineSceneNode.PropertyNodePair>();
          dictionary2[keyValuePair.Value] = hashSet;
        }
        if (!hashSet.Contains(keyValuePair.Key.TargetElementAndProperty))
        {
          hashSet.Add(keyValuePair.Key.TargetElementAndProperty);
          keyValuePair.Value.Children.Add(keyValuePair.Key);
        }
      }
    }

    private void RemoveInvalidAnimations(PathGeometry path)
    {
      IViewObject target = (IViewObject) null;
      PropertyReference propertyReference = (PropertyReference) null;
      IPlatform platform = this.targetElement.ProjectContext.Platform;
      foreach (StoryboardTimelineSceneNode timelineSceneNode1 in this.targetElement.ViewModel.AnimationEditor.EnumerateStoryboardsForContainer(this.targetElement.StoryboardContainer))
      {
        List<TimelineSceneNode> list = new List<TimelineSceneNode>();
        foreach (TimelineSceneNode timelineSceneNode2 in (IEnumerable<TimelineSceneNode>) timelineSceneNode1.Children)
        {
          if (timelineSceneNode2.TargetElement == this.targetElement && timelineSceneNode2.TargetProperty != null)
          {
            if (propertyReference == null)
              propertyReference = new PropertyReference(platform.Metadata.ResolveProperty((IPropertyId) this.pathProperty) as ReferenceStep);
            if (propertyReference.IsPrefixOf(timelineSceneNode2.TargetProperty))
            {
              if (target == null)
              {
                object platformObject = this.targetElement.ViewModel.DefaultView.ConvertFromWpfValue((object) path);
                target = platform.ViewObjectFactory.Instantiate(platformObject);
              }
              if (!timelineSceneNode2.TargetProperty.Subreference(1).IsValidPath(target, platform.Metadata.DefaultTypeResolver))
                list.Add(timelineSceneNode2);
            }
          }
        }
        foreach (TimelineSceneNode timelineSceneNode2 in list)
          timelineSceneNode1.Children.Remove(timelineSceneNode2);
      }
    }

    private void ChangeAnimationProperty(PropertyReference fromProperty, PropertyReference toProperty, Dictionary<TimelineSceneNode, StoryboardTimelineSceneNode> toAdd, Dictionary<TimelineSceneNode, StoryboardTimelineSceneNode> toRemove)
    {
      this.ChangeAnimationProperty(fromProperty, toProperty, toAdd, toRemove, true);
    }

    private void ChangeAnimationProperty(PropertyReference fromProperty, PropertyReference toProperty, Dictionary<TimelineSceneNode, StoryboardTimelineSceneNode> toAdd, Dictionary<TimelineSceneNode, StoryboardTimelineSceneNode> toRemove, bool exactMatch)
    {
      foreach (StoryboardTimelineSceneNode timelineSceneNode in this.targetElement.ViewModel.AnimationEditor.EnumerateStoryboardsForContainer(this.targetElement.StoryboardContainer))
      {
        foreach (TimelineSceneNode timeline in (IEnumerable<TimelineSceneNode>) timelineSceneNode.Children)
        {
          if (timeline.TargetElement == this.targetElement && timeline.TargetProperty != null)
          {
            PropertyReference propertyReference = toProperty;
            bool flag1 = AnimationProxyManager.IsOptimizedAnimation(timeline);
            bool flag2 = !flag1 && fromProperty.Equals((object) timeline.TargetProperty);
            if (!flag1 && !exactMatch && !flag2)
            {
              flag2 = fromProperty.IsPrefixOf(timeline.TargetProperty);
              if (flag2)
              {
                for (int count = toProperty.Count; count < timeline.TargetProperty.Count; ++count)
                  propertyReference = propertyReference.Append(timeline.TargetProperty[count]);
              }
            }
            if (flag2)
            {
              if (toRemove != null)
                toRemove[timeline] = timelineSceneNode;
              TimelineSceneNode index = (TimelineSceneNode) this.targetElement.ViewModel.GetSceneNode(timeline.DocumentNode.Clone(timeline.DocumentContext));
              index.TargetProperty = propertyReference;
              index.ShouldSerialize = true;
              KeyFrameAnimationSceneNode animationSceneNode = index as KeyFrameAnimationSceneNode;
              if (animationSceneNode != null)
                animationSceneNode.IsAnimationProxy = false;
              toAdd[index] = timelineSceneNode;
            }
          }
        }
      }
    }

    private void RemoveAnimation(PropertyReference animationPrefix, Dictionary<TimelineSceneNode, StoryboardTimelineSceneNode> toRemove)
    {
      foreach (StoryboardTimelineSceneNode timelineSceneNode in this.targetElement.ViewModel.AnimationEditor.EnumerateStoryboardsForContainer(this.targetElement.StoryboardContainer))
      {
        foreach (TimelineSceneNode timeline in (IEnumerable<TimelineSceneNode>) timelineSceneNode.Children)
        {
          if (timeline.TargetElement == this.targetElement && timeline.TargetProperty != null && !AnimationProxyManager.IsOptimizedAnimation(timeline) && (animationPrefix.Equals((object) timeline.TargetProperty) || animationPrefix.IsPrefixOf(timeline.TargetProperty)))
            toRemove[timeline] = timelineSceneNode;
        }
      }
    }

    private bool DoesStructureMatch(PathSegment segment1, PathSegment segment2)
    {
      if (segment1.GetType() != segment2.GetType())
        return false;
      PolyBezierSegment polyBezierSegment1;
      if ((polyBezierSegment1 = segment1 as PolyBezierSegment) != null)
      {
        PolyBezierSegment polyBezierSegment2 = (PolyBezierSegment) segment2;
        if (polyBezierSegment1.Points.Count != polyBezierSegment2.Points.Count)
          return false;
      }
      else
      {
        PolyQuadraticBezierSegment quadraticBezierSegment1;
        if ((quadraticBezierSegment1 = segment1 as PolyQuadraticBezierSegment) != null)
        {
          PolyQuadraticBezierSegment quadraticBezierSegment2 = (PolyQuadraticBezierSegment) segment2;
          if (quadraticBezierSegment1.Points.Count != quadraticBezierSegment2.Points.Count)
            return false;
        }
        else
        {
          PolyLineSegment polyLineSegment1;
          if ((polyLineSegment1 = segment1 as PolyLineSegment) != null)
          {
            PolyLineSegment polyLineSegment2 = (PolyLineSegment) segment2;
            if (polyLineSegment1.Points.Count != polyLineSegment2.Points.Count)
              return false;
          }
        }
      }
      return true;
    }

    private bool DoesStructureMatch(PathFigure figure1, PathFigure figure2)
    {
      if (figure1.IsClosed != figure2.IsClosed || figure1.Segments.Count != figure2.Segments.Count)
        return false;
      for (int index = 0; index < figure1.Segments.Count; ++index)
      {
        if (!this.DoesStructureMatch(figure1.Segments[index], figure2.Segments[index]))
          return false;
      }
      return true;
    }

    private bool DoesStructureMatch(PathGeometry geometry1, PathGeometry geometry2)
    {
      if (geometry1.Figures.Count != geometry2.Figures.Count)
        return false;
      for (int index = 0; index < geometry1.Figures.Count; ++index)
      {
        if (!this.DoesStructureMatch(geometry1.Figures[index], geometry2.Figures[index]))
          return false;
      }
      return true;
    }

    private void SetPathGeometryUsingMinimalDiff(PathGeometry path, PathGeometry currentGeometry)
    {
      PropertyReference propertyReference = new PropertyReference(this.platformMetadata.ResolveProperty((IPropertyId) this.pathProperty) as ReferenceStep).Append(PathElement.FiguresProperty);
      for (int index = 0; index < path.Figures.Count; ++index)
      {
        ReferenceStep step = (ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep((IPlatformMetadata) this.platformMetadata, PlatformTypes.PathFigureCollection, index);
        PropertyReference figureReference = propertyReference.Append(step);
        PathFigure currentFigure = currentGeometry.Figures[index];
        this.SetFigureUsingMinimalDiff(path.Figures[index], currentFigure, figureReference);
      }
    }

    private void SetFigureUsingMinimalDiff(PathFigure pathFigure, PathFigure currentFigure, PropertyReference figureReference)
    {
      if (!VectorUtilities.ArePathPointsVeryClose(currentFigure.StartPoint, pathFigure.StartPoint))
        this.targetElement.SetValueAsWpf(figureReference.Append(PathElement.PathFigureStartPointProperty), (object) pathFigure.StartPoint);
      for (int index = 0; index < pathFigure.Segments.Count; ++index)
      {
        PathSegment currentSegment = currentFigure.Segments[index];
        PathSegment pathSegment = pathFigure.Segments[index];
        PropertyReference segmentsReference = figureReference.Append(PathElement.PathFigureSegmentsProperty).Append((ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep((IPlatformMetadata) this.platformMetadata, PlatformTypes.PathSegmentCollection, index));
        this.SetSegmentUsingMinimalDiff(pathSegment, currentSegment, segmentsReference);
      }
    }

    private void SetSegmentUsingMinimalDiff(PathSegment pathSegment, PathSegment currentSegment, PropertyReference segmentsReference)
    {
      PolyBezierSegment currentSegment1;
      if ((currentSegment1 = currentSegment as PolyBezierSegment) != null)
      {
        this.SetPolyBezierSegmentUsingMinimalDiff(currentSegment1, (PolyBezierSegment) pathSegment, segmentsReference);
      }
      else
      {
        BezierSegment currentSegment2;
        if ((currentSegment2 = currentSegment as BezierSegment) != null)
        {
          this.SetBezierSegmentUsingMinimalDiff(currentSegment2, (BezierSegment) pathSegment, segmentsReference);
        }
        else
        {
          PolyLineSegment currentSegment3;
          if ((currentSegment3 = currentSegment as PolyLineSegment) != null)
          {
            this.SetPolyLineSegmentUsingMinimalDiff(currentSegment3, (PolyLineSegment) pathSegment, segmentsReference);
          }
          else
          {
            LineSegment currentSegment4;
            if ((currentSegment4 = currentSegment as LineSegment) != null)
            {
              this.SetLineSegmentUsingMinimalDiff(currentSegment4, (LineSegment) pathSegment, segmentsReference);
            }
            else
            {
              QuadraticBezierSegment currentSegment5;
              if ((currentSegment5 = currentSegment as QuadraticBezierSegment) != null)
              {
                this.SetQuadraticBezierSegmentUsingMinimalDiff(currentSegment5, (QuadraticBezierSegment) pathSegment, segmentsReference);
              }
              else
              {
                PolyQuadraticBezierSegment currentSegment6;
                if ((currentSegment6 = currentSegment as PolyQuadraticBezierSegment) != null)
                  this.SetPolyQuadraticBezierSegmentUsingMinimalDiff(currentSegment6, (PolyQuadraticBezierSegment) pathSegment, segmentsReference);
                else
                  this.targetElement.SetValueAsWpf(segmentsReference, (object) pathSegment);
              }
            }
          }
        }
      }
    }

    private void SetPolyBezierSegmentUsingMinimalDiff(PolyBezierSegment currentSegment, PolyBezierSegment pathSegment, PropertyReference segmentReference)
    {
      PropertyReference propertyReference1 = segmentReference.Append(PathElement.PolyBezierSegmentPointsProperty);
      for (int index = 0; index < currentSegment.Points.Count; ++index)
      {
        ReferenceStep step = (ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep((IPlatformMetadata) this.platformMetadata, PlatformTypes.PointCollection, index);
        PropertyReference propertyReference2 = propertyReference1.Append(step);
        if (!VectorUtilities.ArePathPointsVeryClose(currentSegment.Points[index], pathSegment.Points[index]))
          this.targetElement.SetValueAsWpf(propertyReference2, (object) pathSegment.Points[index]);
      }
    }

    private void SetPolyQuadraticBezierSegmentUsingMinimalDiff(PolyQuadraticBezierSegment currentSegment, PolyQuadraticBezierSegment pathSegment, PropertyReference segmentReference)
    {
      PropertyReference propertyReference1 = segmentReference.Append(PathElement.PolyQuadraticBezierSegmentPointsProperty);
      for (int index = 0; index < currentSegment.Points.Count; ++index)
      {
        ReferenceStep step = (ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep((IPlatformMetadata) this.platformMetadata, PlatformTypes.PointCollection, index);
        PropertyReference propertyReference2 = propertyReference1.Append(step);
        if (!VectorUtilities.ArePathPointsVeryClose(currentSegment.Points[index], pathSegment.Points[index]))
          this.targetElement.SetValueAsWpf(propertyReference2, (object) pathSegment.Points[index]);
      }
    }

    private void SetPolyLineSegmentUsingMinimalDiff(PolyLineSegment currentSegment, PolyLineSegment pathSegment, PropertyReference segmentReference)
    {
      PropertyReference propertyReference1 = segmentReference.Append(PathElement.PolyLineSegmentPointsProperty);
      for (int index = 0; index < currentSegment.Points.Count; ++index)
      {
        ReferenceStep step = (ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep((IPlatformMetadata) this.platformMetadata, PlatformTypes.PointCollection, index);
        PropertyReference propertyReference2 = propertyReference1.Append(step);
        if (!VectorUtilities.ArePathPointsVeryClose(currentSegment.Points[index], pathSegment.Points[index]))
          this.targetElement.SetValueAsWpf(propertyReference2, (object) pathSegment.Points[index]);
      }
    }

    private void SetBezierSegmentUsingMinimalDiff(BezierSegment currentSegment, BezierSegment pathSegment, PropertyReference segmentReference)
    {
      PropertyReference propertyReference1 = segmentReference.Append(PathElement.BezierSegmentPoint1Property);
      PropertyReference propertyReference2 = segmentReference.Append(PathElement.BezierSegmentPoint2Property);
      PropertyReference propertyReference3 = segmentReference.Append(PathElement.BezierSegmentPoint3Property);
      if (!VectorUtilities.ArePathPointsVeryClose(currentSegment.Point1, pathSegment.Point1))
        this.targetElement.SetValueAsWpf(propertyReference1, (object) pathSegment.Point1);
      if (!VectorUtilities.ArePathPointsVeryClose(currentSegment.Point2, pathSegment.Point2))
        this.targetElement.SetValueAsWpf(propertyReference2, (object) pathSegment.Point2);
      if (VectorUtilities.ArePathPointsVeryClose(currentSegment.Point3, pathSegment.Point3))
        return;
      this.targetElement.SetValueAsWpf(propertyReference3, (object) pathSegment.Point3);
    }

    private void SetQuadraticBezierSegmentUsingMinimalDiff(QuadraticBezierSegment currentSegment, QuadraticBezierSegment pathSegment, PropertyReference segmentReference)
    {
      PropertyReference propertyReference1 = segmentReference.Append(PathElement.QuadraticBezierSegmentPoint1Property);
      PropertyReference propertyReference2 = segmentReference.Append(PathElement.QuadraticBezierSegmentPoint2Property);
      if (!VectorUtilities.ArePathPointsVeryClose(currentSegment.Point1, pathSegment.Point1))
        this.targetElement.SetValueAsWpf(propertyReference1, (object) pathSegment.Point1);
      if (VectorUtilities.ArePathPointsVeryClose(currentSegment.Point2, pathSegment.Point2))
        return;
      this.targetElement.SetValueAsWpf(propertyReference2, (object) pathSegment.Point2);
    }

    private void SetLineSegmentUsingMinimalDiff(LineSegment currentSegment, LineSegment pathSegment, PropertyReference segmentReference)
    {
      PropertyReference propertyReference = segmentReference.Append(PathElement.LineSegmentPointProperty);
      if (VectorUtilities.ArePathPointsVeryClose(currentSegment.Point, pathSegment.Point))
        return;
      this.targetElement.SetValueAsWpf(propertyReference, (object) pathSegment.Point);
    }
  }
}
