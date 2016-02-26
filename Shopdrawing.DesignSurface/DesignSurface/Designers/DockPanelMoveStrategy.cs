// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Designers.DockPanelMoveStrategy
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools.Layout;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.Designers
{
  internal class DockPanelMoveStrategy : InplaceMoveStrategy
  {
    private DockPanelMoveStrategy.InsertionPoint insertionPoint = new DockPanelMoveStrategy.InsertionPoint();
    private DockAdorner oldDockAdorner;
    private DockPanelLayoutAdornerSet dockPanelAdornerSet;

    public DockPanelMoveStrategy(MoveStrategyContext context)
      : base(context)
    {
    }

    protected override void OnBeginDrag()
    {
      this.PrepareDraggedElementsForDragging();
    }

    protected override void OnContinueDrag(BaseFrameworkElement hitElement)
    {
      this.UpdateAdorners();
      this.SetInsertionPoint();
      this.MoveDraggedElementsWithTempTransform();
    }

    protected override bool OnEndDrag(bool commit)
    {
      this.ClearAdorners();
      this.RestoreDraggedElements();
      if (!commit || this.insertionPoint.IsEmpty || this.DraggedElements == null)
        return false;
      this.AdjustIndexBeforeRemovingFromSceneView();
      HashSet<BaseFrameworkElement> hashSet = new HashSet<BaseFrameworkElement>();
      foreach (BaseFrameworkElement frameworkElement in this.DraggedElements)
      {
        hashSet.Add(frameworkElement);
        frameworkElement.Remove();
        this.insertionPoint.DockPanel.Children.Insert(this.insertionPoint.Index, (SceneNode) frameworkElement);
        ++this.insertionPoint.Index;
        frameworkElement.SetValue(DockPanelElement.DockProperty, (object) this.insertionPoint.Dock);
      }
      this.Context.Transaction.UpdateEditTransaction();
      this.AdjustLayoutAfterReparenting((ICollection<BaseFrameworkElement>) hashSet);
      return true;
    }

    protected override void AdjustIndexBeforeRemovingFromSceneView()
    {
      int offsetBeforeRemove = this.ComputeIndexOffsetBeforeRemove(this.insertionPoint.Index);
      if (offsetBeforeRemove == 0)
        return;
      this.insertionPoint.Index += offsetBeforeRemove;
    }

    private void ClearAdorners()
    {
      if (this.dockPanelAdornerSet == null)
        return;
      this.ActiveView.AdornerLayer.Remove((IAdornerSet) this.dockPanelAdornerSet);
      this.dockPanelAdornerSet = (DockPanelLayoutAdornerSet) null;
    }

    private void UpdateAdorners()
    {
      if (this.dockPanelAdornerSet != null)
        return;
      this.dockPanelAdornerSet = new DockPanelLayoutAdornerSet(this.ToolContext, this.LayoutContainer);
      this.ActiveView.AdornerLayer.Add((IAdornerSet) this.dockPanelAdornerSet);
    }

    private void SetInsertionPoint()
    {
      bool flag = false;
      if (this.oldDockAdorner != null)
        this.oldDockAdorner.IsActive = false;
      DockAdorner dockAdorner = this.ActiveView.AdornerService.GetHitAdorner(this.Pointer, new Type[1]
      {
        typeof (DockPanelAdorner)
      }) as DockAdorner;
      this.oldDockAdorner = dockAdorner;
      if (dockAdorner != null)
      {
        this.oldDockAdorner.IsActive = true;
        if (dockAdorner is DockPanelAdorner)
        {
          DockPanelElement dockPanel = (DockPanelElement) dockAdorner.Element;
          DockPanelAdorner dockPanelAdorner = (DockPanelAdorner) dockAdorner;
          this.insertionPoint.DockPanel = dockPanel;
          this.insertionPoint.Dock = dockPanelAdorner.Dock;
          this.insertionPoint.Index = DockPanelLayoutUtilities.GetFillRegionInsertionIndex(dockPanel);
          flag = true;
        }
      }
      if (flag || this.insertionPoint.IsEmpty)
        return;
      this.insertionPoint.Clear();
    }

    private class InsertionPoint
    {
      private DockPanelElement dockPanel;
      private Dock dock;
      private int index;

      public bool IsEmpty
      {
        get
        {
          if (this.DockPanel != null)
            return this.Index < 0;
          return true;
        }
      }

      public DockPanelElement DockPanel
      {
        get
        {
          return this.dockPanel;
        }
        set
        {
          this.dockPanel = value;
        }
      }

      public Dock Dock
      {
        get
        {
          return this.dock;
        }
        set
        {
          this.dock = value;
        }
      }

      public int Index
      {
        get
        {
          return this.index;
        }
        set
        {
          this.index = value;
        }
      }

      public void Clear()
      {
        this.DockPanel = (DockPanelElement) null;
        this.Index = -1;
      }
    }
  }
}
