// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Designers.MoveStrategyContext
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Designers
{
  internal sealed class MoveStrategyContext
  {
    public ToolBehaviorContext ToolBehaviorContext { get; private set; }

    public IToolBehaviorTransaction Transaction { get; private set; }

    public MouseDevice MouseDevice { get; private set; }

    public SceneElement PrimarySelection { get; private set; }

    public ReadOnlyCollection<SceneElement> SelectedElements { get; private set; }

    public ReadOnlyCollection<BaseFrameworkElement> DraggedElements { get; private set; }

    public ReadOnlyCollection<Point> Offsets { get; private set; }

    public ReadOnlyCollection<CanonicalTransform> BaseRenderTransforms { get; private set; }

    public ReadOnlyCollection<LayoutCacheRecord> LayoutCacheRecords { get; private set; }

    public Rect BoundsOfAllElements { get; private set; }

    public Vector DuplicationOffset { get; private set; }

    public SceneView ActiveView
    {
      get
      {
        return this.ToolBehaviorContext.View;
      }
    }

    public SceneNode RootNode
    {
      get
      {
        return this.ActiveView.ViewModel.ActiveEditingContainer;
      }
    }

    public bool IsRecordingKeyframes
    {
      get
      {
        return this.ActiveView.ViewModel.UsingEffectDesigner;
      }
    }

    public static MoveStrategyContext FromSelection(ToolBehavior toolBehavior, SceneElement primarySelection, IList<SceneElement> selectedElements, IList<BaseFrameworkElement> draggedElements, Vector duplicationOffset, Point referencePoint)
    {
      MoveStrategyContext moveStrategyContext = new MoveStrategyContext();
      moveStrategyContext.ToolBehaviorContext = toolBehavior.ToolBehaviorContext;
      moveStrategyContext.Transaction = (IToolBehaviorTransaction) toolBehavior;
      moveStrategyContext.MouseDevice = toolBehavior.MouseDevice;
      moveStrategyContext.PrimarySelection = primarySelection;
      moveStrategyContext.SelectedElements = new ReadOnlyCollection<SceneElement>(selectedElements ?? (IList<SceneElement>) new List<SceneElement>());
      moveStrategyContext.DraggedElements = new ReadOnlyCollection<BaseFrameworkElement>(draggedElements ?? (IList<BaseFrameworkElement>) new List<BaseFrameworkElement>());
      moveStrategyContext.DuplicationOffset = duplicationOffset;
      moveStrategyContext.SnapshotDraggedElements(moveStrategyContext.ActiveView, referencePoint);
      return moveStrategyContext;
    }

    private void SnapshotDraggedElements(SceneView view, Point referencePoint)
    {
      List<Point> list1 = new List<Point>();
      List<CanonicalTransform> list2 = new List<CanonicalTransform>();
      List<LayoutCacheRecord> list3 = new List<LayoutCacheRecord>();
      Rect empty = Rect.Empty;
      foreach (BaseFrameworkElement element in this.DraggedElements)
      {
        element.GetComputedTightBounds();
        Point point = view.TransformPoint((IViewObject) view.HitTestRoot, element.Visual, referencePoint);
        list1.Add(point);
        list2.Add(new CanonicalTransform((Transform) element.GetComputedValueAsWpf(Base2DElement.RenderTransformProperty))
        {
          Center = element.RenderTransformOriginInElementCoordinates
        });
        if (element.ViewObject != null)
        {
          LayoutCacheRecord layoutCacheRecord = this.ActiveView.ViewModel.GetLayoutDesignerForChild((SceneElement) element, true).CacheLayout(element);
          list3.Add(layoutCacheRecord);
          empty.Union(layoutCacheRecord.Rect);
        }
      }
      this.Offsets = list1.AsReadOnly();
      this.BaseRenderTransforms = list2.AsReadOnly();
      this.LayoutCacheRecords = list3.AsReadOnly();
      this.BoundsOfAllElements = empty;
    }
  }
}
