// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Timeline.DragDrop.TreeRowDragDropHandler
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface.Timeline;
using Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI;
using Microsoft.Expression.Framework.Controls;
using System;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Timeline.DragDrop
{
  public class TreeRowDragDropHandler : ListItemDragDropHandler
  {
    private ModifierKeys lastModifierKeys;
    private DragDropEffects lastAllowedEffects;

    public TimelineTreeRow TimelineTreeRow
    {
      get
      {
        return this.DropTarget as TimelineTreeRow;
      }
    }

    public TimelineItem TimelineItem
    {
      get
      {
        if (this.TimelineTreeRow != null)
          return this.TimelineTreeRow.TimelineItem;
        return (TimelineItem) null;
      }
    }

    public StructureView StructureView
    {
      get
      {
        if (this.TimelineItem != null)
          return this.TimelineItem.TimelinePane.StructureView;
        return (StructureView) null;
      }
    }

    public DragDropContext DragDropContext { get; private set; }

    public TreeRowDragDropHandler(TimelineTreeRow treeRow)
      : base((FrameworkElement) treeRow)
    {
    }

    public override void OnDragBegin(DragBeginEventArgs e)
    {
      if (!e.Handled && this.TimelineTreeRow != null && (!this.TimelineTreeRow.IsEditingTitle && this.TimelineItem != null) && this.TimelineItem.CanDrag())
      {
        object dragData = this.TimelineItem.DragData;
        if (dragData != null)
        {
          int num = (int) DragSourceHelper.DoDragDrop((UIElement) this.TimelineTreeRow, dragData, DragDropEffects.All);
        }
        e.Handled = true;
      }
      base.OnDragBegin(e);
    }

    public override void OnDragOver(DragEventArgs e)
    {
      base.OnDragOver(e);
      try
      {
        e.Effects = DragDropEffects.None;
        ModifierKeys modifiers = Keyboard.Modifiers;
        if (this.HitTestResult != this.LastHitTestResult || this.lastModifierKeys != modifiers || this.lastAllowedEffects != e.AllowedEffects)
        {
          this.lastModifierKeys = modifiers;
          this.lastAllowedEffects = e.AllowedEffects;
          e.Effects = this.ComputeDropEffects(e);
        }
        else if (this.DragDropContext != null)
          e.Effects = this.DragDropContext.Descriptor.ResultEffects;
      }
      finally
      {
        this.UpdateFeedback(true, e);
        e.Handled = true;
      }
      if (this.StructureView == null)
        return;
      this.StructureView.ScrollIfNeeded((FrameworkElement) this.TimelineTreeRow);
    }

    public override void OnDragEnter(DragEventArgs e)
    {
      base.OnDragEnter(e);
      try
      {
        e.Effects = DragDropEffects.None;
        this.lastModifierKeys = Keyboard.Modifiers;
        this.lastAllowedEffects = e.AllowedEffects;
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
        if (this.DragDropContext != null)
        {
          IDropAction instance = DropActionFactory.CreateInstance(this.DragDropContext);
          if (instance != null)
            e.Effects = instance.HandleDrop(this.DragDropContext.Descriptor.ResultEffects);
        }
      }
      finally
      {
        e.Handled = true;
      }
      base.OnDrop(e);
    }

    protected override void OnHoverEnter()
    {
      if (this.TimelineItem != null)
        this.TimelineItem.IsExpanded = true;
      base.OnHoverEnter();
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

    private void UpdateFeedback(bool showFeedback, DragEventArgs e)
    {
      if (this.StructureView == null || this.DragDropContext == null)
        return;
      Point mouseOffset = e == null ? new Point() : e.GetPosition((IInputElement) this.StructureView);
      this.StructureView.ShowDragFeedback(showFeedback, mouseOffset, this.DragDropContext.Descriptor);
      this.StructureView.ShowSplitAdorner(showFeedback, this.DragDropContext.Descriptor);
    }

    private bool IsCopyKeyState(DragDropKeyStates keyStates)
    {
      return (keyStates & (DragDropKeyStates.ControlKey | DragDropKeyStates.AltKey)) != DragDropKeyStates.None;
    }

    private DragDropEffects ComputeDropEffects(DragEventArgs e)
    {
      TimelineDropEffects allowedEffects1 = TimelineDropEffects.None;
      if ((this.HitTestResult & DropHitTestResults.UpperHalf) != DropHitTestResults.None)
        allowedEffects1 = !this.TimelineItem.TimelineItemManager.SortByZOrder ? TimelineDropEffects.After : TimelineDropEffects.Before;
      else if ((this.HitTestResult & DropHitTestResults.LowerHalf) != DropHitTestResults.None)
        allowedEffects1 = !this.TimelineItem.TimelineItemManager.SortByZOrder ? TimelineDropEffects.Before : TimelineDropEffects.After;
      if ((e.AllowedEffects & DragDropEffects.Copy) != DragDropEffects.None && this.IsCopyKeyState(e.KeyStates))
        allowedEffects1 |= TimelineDropEffects.Copy;
      if ((e.AllowedEffects & DragDropEffects.Move) != DragDropEffects.None)
        allowedEffects1 |= TimelineDropEffects.Move;
      if ((this.HitTestResult & DropHitTestResults.CenterHalf) != DropHitTestResults.None)
      {
        TimelineDropEffects allowedEffects2 = allowedEffects1 & ~(TimelineDropEffects.Before | TimelineDropEffects.After);
        this.DragDropContext = new DragDropContext(e.Data, this.TimelineItem, allowedEffects2);
        IDropAction instance = DropActionFactory.CreateInstance(this.DragDropContext);
        if (instance != null && instance.CanDrop(this.DragDropContext.Descriptor))
          return this.DragDropContext.Descriptor.ResultEffects;
      }
      this.DragDropContext = new DragDropContext(e.Data, this.TimelineItem, allowedEffects1);
      IDropAction instance1 = DropActionFactory.CreateInstance(this.DragDropContext);
      if (instance1 != null && instance1.CanDrop(this.DragDropContext.Descriptor))
        return this.DragDropContext.Descriptor.ResultEffects;
      this.DragDropContext.Descriptor.DisableDrop();
      return this.DragDropContext.Descriptor.ResultEffects;
    }
  }
}
