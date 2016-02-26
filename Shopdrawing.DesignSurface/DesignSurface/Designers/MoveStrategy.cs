// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Designers.MoveStrategy
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Designers
{
  internal abstract class MoveStrategy
  {
    private bool isConstraining;
    private Point dragStartPosition;
    private Point dragCurrentPosition;

    protected MoveStrategyContext Context { get; private set; }

    public BaseFrameworkElement LayoutContainer { get; internal set; }

    protected ToolBehaviorContext ToolContext
    {
      get
      {
        return this.Context.ToolBehaviorContext;
      }
    }

    protected SceneView ActiveView
    {
      get
      {
        return this.ToolContext.View;
      }
    }

    protected SceneViewModel ActiveSceneViewModel
    {
      get
      {
        return this.ActiveView.ViewModel;
      }
    }

    protected Point DragStartPosition
    {
      get
      {
        return this.dragStartPosition;
      }
    }

    protected Point DragCurrentPosition
    {
      get
      {
        return this.dragCurrentPosition;
      }
    }

    protected MouseDevice Pointer
    {
      get
      {
        return this.Context.MouseDevice;
      }
    }

    protected ReadOnlyCollection<BaseFrameworkElement> DraggedElements
    {
      get
      {
        return this.Context.DraggedElements;
      }
    }

    public bool IsConstraining
    {
      get
      {
        return this.isConstraining;
      }
      set
      {
        if (this.isConstraining == value)
          return;
        this.isConstraining = value;
        this.OnIsConstrainingChanged();
      }
    }

    public MoveStrategy(MoveStrategyContext context)
    {
      this.Context = context;
    }

    internal static int GetMaxChildrenForElement(SceneElement element, IProperty property = null)
    {
      if (property == null)
        property = element.DefaultContentProperty;
      if (!element.IsContainer && property == element.DefaultContentProperty)
        return 0;
      return element.GetCollectionForProperty((IPropertyId) property).FixedCapacity ?? int.MaxValue;
    }

    internal static IViewObject GetContainerHost(SceneElement container)
    {
      ItemsControlElement itemsControlElement = container as ItemsControlElement;
      if (itemsControlElement != null)
        return (IViewObject) itemsControlElement.ItemsHost;
      if (container != null)
        return container.ViewObject;
      return (IViewObject) null;
    }

    internal void BeginDrag(Point dragStartPosition)
    {
      this.dragStartPosition = dragStartPosition;
      this.OnBeginDrag();
      if (this.Context.PrimarySelection.IsAttached)
        this.ActiveSceneViewModel.ElementSelectionSet.SetSelection((ICollection<SceneElement>) this.Context.SelectedElements, this.Context.PrimarySelection);
      this.Context.Transaction.CreateSubTransaction();
    }

    internal void ContinueDrag(Point dragCurrentPosition, BaseFrameworkElement hitElement)
    {
      this.dragCurrentPosition = dragCurrentPosition;
      if (!this.ActiveSceneViewModel.AnimationEditor.IsRecording)
        this.Context.Transaction.ReplaceSubTransaction();
      this.OnContinueDrag(hitElement);
    }

    internal bool EndDrag(bool commit)
    {
      bool flag = this.OnEndDrag(commit);
      if (commit)
      {
        this.Context.Transaction.CommitSubTransaction();
        return flag;
      }
      this.Context.Transaction.CancelSubTransaction();
      return true;
    }

    protected abstract void OnBeginDrag();

    protected abstract void OnContinueDrag(BaseFrameworkElement hitElement);

    protected abstract bool OnEndDrag(bool commit);

    protected virtual void OnIsConstrainingChanged()
    {
    }

    protected void AdjustLayoutAfterReparenting(ICollection<BaseFrameworkElement> reparentedElements)
    {
      ILayoutDesigner designerForParent = this.LayoutContainer.ViewModel.GetLayoutDesignerForParent((SceneElement) this.LayoutContainer, true);
      for (int index = 0; index < this.Context.DraggedElements.Count; ++index)
      {
        BaseFrameworkElement element = this.Context.DraggedElements[index];
        if (element.IsAttached && element.IsViewObjectValid && reparentedElements.Contains(element))
          designerForParent.SetLayoutFromCache(element, this.Context.LayoutCacheRecords[index], this.Context.BoundsOfAllElements);
      }
    }

    protected Point ConstrainPointToAxis(Point dragStartPosition, Point dragCurrentPosition)
    {
      Vector vector = dragCurrentPosition - dragStartPosition;
      if (Math.Abs(vector.X) > Math.Abs(vector.Y))
        vector.Y = 0.0;
      else
        vector.X = 0.0;
      return new Point(dragStartPosition.X + vector.X, dragStartPosition.Y + vector.Y);
    }
  }
}
