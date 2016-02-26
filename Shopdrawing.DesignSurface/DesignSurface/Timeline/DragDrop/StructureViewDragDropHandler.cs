// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Timeline.DragDrop.StructureViewDragDropHandler
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface.Timeline;
using Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Timeline.DragDrop
{
  public class StructureViewDragDropHandler : DragDropHandler
  {
    private StructureViewDragDropHandler.HittestResult hittestResult;
    private ModifierKeys lastModifierKeys;
    private DragDropEffects lastAllowedEffects;

    public StructureView StructureView { get; private set; }

    public DragDropContext DragDropContext { get; private set; }

    private TimelineItem TargetItem
    {
      get
      {
        if (this.hittestResult != null)
          return this.hittestResult.candidateItem;
        return (TimelineItem) null;
      }
    }

    private bool ShouldTryDropInsideFirst
    {
      get
      {
        if (this.hittestResult != null)
          return this.hittestResult.allowDropInside;
        return false;
      }
    }

    public StructureViewDragDropHandler(StructureView structureView)
    {
      this.StructureView = structureView;
    }

    public override void OnDragOver(DragEventArgs e)
    {
      base.OnDragOver(e);
      try
      {
        e.Effects = DragDropEffects.None;
        StructureViewDragDropHandler.HittestResult hittestResult = this.Hittest(e.GetPosition((IInputElement) this.StructureView));
        ModifierKeys modifiers = Keyboard.Modifiers;
        if (this.lastModifierKeys != modifiers || this.lastAllowedEffects != e.AllowedEffects || !object.Equals((object) this.hittestResult, (object) hittestResult))
        {
          this.lastModifierKeys = modifiers;
          this.lastAllowedEffects = e.AllowedEffects;
          this.hittestResult = hittestResult;
          e.Effects = this.ComputeDropEffects(e);
        }
        else
        {
          if (this.DragDropContext == null)
            return;
          e.Effects = this.DragDropContext.Descriptor.ResultEffects;
        }
      }
      finally
      {
        this.UpdateFeedback(true, e);
        e.Handled = true;
      }
    }

    public override void OnDragEnter(DragEventArgs e)
    {
      base.OnDragEnter(e);
      try
      {
        e.Effects = DragDropEffects.None;
        this.lastModifierKeys = Keyboard.Modifiers;
        this.lastAllowedEffects = e.AllowedEffects;
        this.hittestResult = this.Hittest(e.GetPosition((IInputElement) this.StructureView));
        e.Effects = this.ComputeDropEffects(e);
      }
      finally
      {
        this.UpdateFeedback(true, e);
        e.Handled = true;
      }
    }

    public override void OnDragLeave(DragEventArgs e)
    {
      this.hittestResult = (StructureViewDragDropHandler.HittestResult) null;
      this.UpdateFeedback(false, (DragEventArgs) null);
      e.Handled = true;
      base.OnDragLeave(e);
    }

    public override void OnDrop(DragEventArgs e)
    {
      try
      {
        e.Effects = DragDropEffects.None;
        this.UpdateFeedback(false, (DragEventArgs) null);
        if (this.TargetItem != null)
        {
          if (this.DragDropContext != null)
          {
            IDropAction instance = DropActionFactory.CreateInstance(this.DragDropContext);
            if (instance != null)
              e.Effects = instance.HandleDrop(this.DragDropContext.Descriptor.ResultEffects);
          }
        }
      }
      finally
      {
        this.hittestResult = (StructureViewDragDropHandler.HittestResult) null;
        e.Handled = true;
      }
      base.OnDrop(e);
    }

    protected override void OnDragSourceGiveFeedback(UIElement dragSource, GiveFeedbackEventArgs e)
    {
      e.UseDefaultCursors = false;
      Mouse.SetCursor(Cursors.Arrow);
      e.Handled = true;
    }

    protected override void OnDragSourceDragFinished(UIElement dragSource, EventArgs e)
    {
      this.UpdateFeedback(false, (DragEventArgs) null);
    }

    private StructureViewDragDropHandler.HittestResult Hittest(Point position)
    {
      StructureViewDragDropHandler.HittestResult hittestResult = new StructureViewDragDropHandler.HittestResult();
      if (this.StructureView.TimelineItemManager != null)
      {
        double indentMultiplier = this.StructureView.IndentMultiplier;
        int num = (int) Math.Floor((position.X - indentMultiplier) / this.StructureView.IndentMultiplier);
        IList<TimelineItem> list = (IList<TimelineItem>) this.StructureView.TimelineItemManager.ItemList;
        if (list != null)
        {
          for (int index = list.Count - 1; index >= 0; --index)
          {
            TimelineItem timelineItem = list[index];
            if (timelineItem != null && timelineItem.Depth <= num)
            {
              ElementTimelineItem elementTimelineItem = timelineItem as ElementTimelineItem;
              bool flag = elementTimelineItem != null && elementTimelineItem.Element.IsContainer;
              hittestResult.allowDropInside = flag || timelineItem.Depth <= num - 1;
              hittestResult.candidateItem = timelineItem;
              break;
            }
          }
        }
      }
      return hittestResult;
    }

    private void UpdateFeedback(bool showFeedback, DragEventArgs e)
    {
      if (this.StructureView == null || this.DragDropContext == null)
        return;
      Point mouseOffset = e == null ? new Point() : e.GetPosition((IInputElement) this.StructureView);
      this.StructureView.ShowDragFeedback(showFeedback, mouseOffset, this.DragDropContext.Descriptor);
      this.StructureView.ShowSplitAdorner(showFeedback, this.DragDropContext.Descriptor);
    }

    private DragDropEffects ComputeDropEffects(DragEventArgs e)
    {
      if (this.TargetItem != null)
      {
        TimelineDropEffects requestedEffects1 = TimelineDropEffects.None;
        if ((e.AllowedEffects & DragDropEffects.Copy) != DragDropEffects.None && (e.KeyStates & DragDropKeyStates.ControlKey) != DragDropKeyStates.None)
          requestedEffects1 |= TimelineDropEffects.Copy;
        if ((e.AllowedEffects & DragDropEffects.Move) != DragDropEffects.None)
          requestedEffects1 |= TimelineDropEffects.Move;
        if (this.ShouldTryDropInsideFirst && this.TryDrop(e, requestedEffects1))
          return this.DragDropContext.Descriptor.ResultEffects;
        TimelineDropEffects requestedEffects2 = requestedEffects1 | (this.StructureView.TimelineItemManager.SortByZOrder ? TimelineDropEffects.After : TimelineDropEffects.Before);
        if (this.TryDrop(e, requestedEffects2))
          return this.DragDropContext.Descriptor.ResultEffects;
      }
      if (this.DragDropContext == null)
        return DragDropEffects.None;
      this.DragDropContext.Descriptor.DisableDrop();
      return this.DragDropContext.Descriptor.ResultEffects;
    }

    private bool TryDrop(DragEventArgs e, TimelineDropEffects requestedEffects)
    {
      this.DragDropContext = new DragDropContext(e.Data, this.TargetItem, requestedEffects);
      IDropAction instance = DropActionFactory.CreateInstance(this.DragDropContext);
      if (instance != null)
        return instance.CanDrop(this.DragDropContext.Descriptor);
      return false;
    }

    private class HittestResult
    {
      public TimelineItem candidateItem;
      public bool allowDropInside;

      public override bool Equals(object obj)
      {
        StructureViewDragDropHandler.HittestResult hittestResult = obj as StructureViewDragDropHandler.HittestResult;
        if (hittestResult != null && hittestResult.candidateItem == this.candidateItem)
          return hittestResult.allowDropInside == this.allowDropInside;
        return false;
      }

      public override int GetHashCode()
      {
        return this.allowDropInside.GetHashCode() ^ (this.candidateItem == null ? 0 : this.candidateItem.GetHashCode());
      }
    }
  }
}
