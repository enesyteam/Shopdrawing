// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.DataBindingToolBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Timeline.DragDrop;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal class DataBindingToolBehavior : DragToolBehavior
  {
    private InsertionPointHighlighter previewHighlighter;
    private Cursor dragCursor;
    private IDragSource dragSource;
    private Brush masterBackground;
    private Brush detailsBackground;

    public DataBindingToolBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
      this.previewHighlighter = new InsertionPointHighlighter(toolContext);
      this.masterBackground = this.GetResourceBrush(SystemColors.HighlightColorKey);
      this.detailsBackground = this.GetResourceBrush(SystemColors.ControlDarkDarkColorKey);
    }

    protected override bool OnDrop(DragEventArgs args)
    {
      this.previewHighlighter.Option = HighlightOption.Default;
      this.previewHighlighter.InsertionPointPreview = (ISceneInsertionPoint) null;
      this.FeedbackAdorner.CloseAdorner();
      DataBindingDragDropModel dragFeedback = this.GetDragFeedback(args);
      bool flag = false;
      if (dragFeedback != null && dragFeedback.CheckDropFlags(DataBindingDragDropFlags.SetBinding | DataBindingDragDropFlags.CreateElement, false))
      {
        Point position = args.GetPosition((IInputElement) this.ActiveView.ViewRootContainer);
        this.ToolBehaviorContext.SnappingEngine.Start(this.ToolBehaviorContext, (BaseFrameworkElement) null, (IList<BaseFrameworkElement>) null);
        Point artboardSnappedDropPoint = this.ToolBehaviorContext.SnappingEngine.SnapPoint(position, EdgeFlags.TopLeft) * this.ActiveView.GetComputedTransformFromRoot((SceneElement) dragFeedback.TargetNode);
        this.ToolBehaviorContext.SnappingEngine.Stop();
        flag = DataBindingDragDropManager.Drop(dragFeedback, artboardSnappedDropPoint);
      }
      if (!this.IsSuspended)
        this.PopSelf();
      return flag;
    }

    protected override bool OnDragOver(DragEventArgs args)
    {
      if (this.dragSource == null)
      {
        this.dragSource = Enumerable.First<IDragSource>(DragSourceHelper.DataOfType<IDragSource>(args.Data));
        this.dragSource.GiveFeedback += new GiveFeedbackEventHandler(this.DragSourceGiveFeedback);
      }
      this.dragCursor = (Cursor) null;
      args.Effects = DragDropEffects.None;
      DataBindingDragDropModel dragFeedback = this.GetDragFeedback(args);
      if (dragFeedback == null || !dragFeedback.CheckDropFlags(DataBindingDragDropFlags.SetBinding | DataBindingDragDropFlags.CreateElement, false))
      {
        this.previewHighlighter.Option = HighlightOption.Default;
        this.previewHighlighter.InsertionPointPreview = (ISceneInsertionPoint) null;
        this.FeedbackAdorner.Text = string.Empty;
        this.FeedbackAdorner.CloseAdorner();
        base.OnDragOver(args);
        return true;
      }
      if (dragFeedback.CheckDropFlags(DataBindingDragDropFlags.CreateElement))
      {
        if (!((SceneElement) dragFeedback.TargetNode).IsEffectiveRoot)
        {
          this.previewHighlighter.Option = HighlightOption.Insert;
          this.previewHighlighter.InsertionPointPreview = (ISceneInsertionPoint) dragFeedback.InsertionPoint;
        }
        else
        {
          this.previewHighlighter.Option = HighlightOption.Default;
          this.previewHighlighter.InsertionPointPreview = (ISceneInsertionPoint) null;
        }
        args.Effects = DragDropEffects.Copy;
        this.dragCursor = DataBindingModeModel.Instance.NormalizedMode != DataBindingMode.Details ? ToolCursors.DataBindingMasterAddCursor : ToolCursors.DataBindingDetailsAddCursor;
      }
      else
      {
        this.previewHighlighter.Option = HighlightOption.Preview;
        this.previewHighlighter.InsertionPointPreview = (ISceneInsertionPoint) dragFeedback.InsertionPoint;
        args.Effects = DragDropEffects.Move;
        this.dragCursor = DataBindingModeModel.Instance.NormalizedMode != DataBindingMode.Details ? ToolCursors.DataBindingMasterCursor : ToolCursors.DataBindingDetailsCursor;
      }
      if (DataBindingModeModel.Instance.NormalizedMode == DataBindingMode.Details)
        this.FeedbackAdorner.Background = this.detailsBackground;
      else
        this.FeedbackAdorner.Background = this.masterBackground;
      this.FeedbackAdorner.Text = dragFeedback.Tooltip;
      this.SetTextCuePosition(args.GetPosition((IInputElement) this.ActiveView.Artboard.ContentArea));
      this.FeedbackAdorner.DrawAdorner(this.OpenFeedback());
      this.CloseFeedback();
      DataBindingToolBehavior.PokeWpfToRefresh();
      base.OnDragOver(args);
      return true;
    }

    private static void PokeWpfToRefresh()
    {
      Application.Current.Dispatcher.Invoke(DispatcherPriority.Render, (Delegate) (() => {}));
    }

    protected override bool OnDragLeave(DragEventArgs args)
    {
      this.previewHighlighter.InsertionPointPreview = (ISceneInsertionPoint) null;
      if (!this.IsSuspended)
        this.PopSelf();
      return base.OnDragLeave(args);
    }

    private void DragSourceGiveFeedback(object sender, GiveFeedbackEventArgs e)
    {
      if (this.dragCursor == null)
      {
        e.UseDefaultCursors = true;
      }
      else
      {
        e.UseDefaultCursors = false;
        Mouse.SetCursor(this.dragCursor);
      }
      e.Handled = true;
    }

    protected override void OnDetach()
    {
      this.previewHighlighter.InsertionPointPreview = (ISceneInsertionPoint) null;
      this.FeedbackAdorner.CloseAdorner();
      if (this.dragSource != null)
        this.dragSource.GiveFeedback -= new GiveFeedbackEventHandler(this.DragSourceGiveFeedback);
      base.OnDetach();
    }

    protected override void OnSuspend()
    {
      this.previewHighlighter.InsertionPointPreview = (ISceneInsertionPoint) null;
      base.OnSuspend();
    }

    protected override bool OnHover(Point pointerPosition)
    {
      if (!this.IsSuspended)
        this.PopSelf();
      return false;
    }

    protected override bool OnButtonDown(Point pointerPosition)
    {
      if (!this.IsSuspended)
        this.PopSelf();
      return false;
    }

    private DataBindingDragDropModel GetDragFeedback(DragEventArgs args)
    {
      DataSchemaNodePathCollection result = (DataSchemaNodePathCollection) null;
      if (!DragSourceHelper.FirstDataOfType<DataSchemaNodePathCollection>(args.Data, ref result) || result == null)
        return (DataBindingDragDropModel) null;
      SceneNode sceneNode = (SceneNode) this.ActiveView.GetSelectableElementAtPoint(args.GetPosition((IInputElement) this.ActiveView.ViewRootContainer), SelectionFor3D.None, false, false);
      if (sceneNode == null)
      {
        sceneNode = (SceneNode) this.ActiveSceneViewModel.FindPanelClosestToActiveEditingContainer();
        if (sceneNode == null)
          return (DataBindingDragDropModel) null;
      }
      if (PlatformTypes.FrameworkTemplate.IsAssignableFrom((ITypeId) sceneNode.Type))
      {
        sceneNode = sceneNode.GetLocalValueAsSceneNode(FrameworkTemplateElement.VisualTreeProperty);
        if (sceneNode == null)
          return (DataBindingDragDropModel) null;
      }
      BindingSceneInsertionPoint insertionPoint = new BindingSceneInsertionPoint(sceneNode, (IProperty) null, -1);
      return DataBindingDragDropManager.GetDragFeedback(result, insertionPoint, DataBindingDragDropFlags.ArtboardDefault, Keyboard.Modifiers);
    }

    private Brush GetResourceBrush(ResourceKey colorKey)
    {
      Color color = (Color) this.ToolBehaviorContext.View.FeedbackLayer.FindResource((object) colorKey);
      color.A = (byte) 216;
      SolidColorBrush solidColorBrush = new SolidColorBrush(color);
      solidColorBrush.Freeze();
      return (Brush) solidColorBrush;
    }

    private delegate void Nop();
  }
}
