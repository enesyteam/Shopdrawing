// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.PickWhipEngine
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.Tools.Transforms;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.UserInterface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  internal sealed class PickWhipEngine
  {
    private PickWhipEngine.PickState state;
    private IPickWhipHost host;

    public bool IsActive
    {
      get
      {
        return this.state != null;
      }
    }

    public PickWhipEngine(IPickWhipHost host)
    {
      this.host = host;
    }

    public PickWhipEngine(FrameworkElement editor, SceneNodeProperty editingProperty, IElementSelectionStrategy strategy, Cursor cursor)
      : this((IPickWhipHost) new PickWhipHost(editor, editingProperty, strategy, cursor))
    {
    }

    private void Owner_LostMouseCapture(object sender, MouseEventArgs e)
    {
      this.CancelEditing();
    }

    private void ViewModel_Closing(object sender, EventArgs e)
    {
      this.CancelEditing();
    }

    private void Owner_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (!this.IsActive)
        return;
      if (e.ChangedButton == MouseButton.Left)
      {
        this.CommitEditing();
        e.Handled = true;
      }
      else
      {
        if (e.ChangedButton != MouseButton.Right)
          return;
        this.CancelEditing();
        e.Handled = true;
      }
    }

    public void BeginEditing()
    {
      if (!this.host.PropertyEditor.CaptureMouse())
        return;
      this.host.PropertyEditor.LostMouseCapture += new MouseEventHandler(this.Owner_LostMouseCapture);
      this.host.PropertyEditor.MouseDown += new MouseButtonEventHandler(this.Owner_MouseDown);
      this.state = new PickWhipEngine.PickState(this.host);
      InputManager.Current.PostNotifyInput += new NotifyInputEventHandler(this.Current_PostNotifyInput);
      if (this.state.View != null && this.state.View.ViewModel != null)
        this.state.View.ViewModel.Closing += new EventHandler(this.ViewModel_Closing);
      this.state.OnMouseMovement();
    }

    public void CancelEditing()
    {
      if (this.state == null)
        return;
      this.FinishEditing();
    }

    public void CommitEditing()
    {
      if (this.state == null)
        return;
      this.SelectHoveredElement();
      this.FinishEditing();
    }

    public void FinishEditing()
    {
      if (this.state != null)
      {
        if (this.state.View != null && this.state.View.ViewModel != null)
          this.state.View.ViewModel.Closing -= new EventHandler(this.ViewModel_Closing);
        this.state.ClearState();
        this.state = (PickWhipEngine.PickState) null;
      }
      InputManager.Current.PostNotifyInput -= new NotifyInputEventHandler(this.Current_PostNotifyInput);
      this.host.PropertyEditor.ReleaseMouseCapture();
      this.host.PropertyEditor.LostMouseCapture -= new MouseEventHandler(this.Owner_LostMouseCapture);
      this.host.PropertyEditor.MouseDown -= new MouseButtonEventHandler(this.Owner_MouseDown);
    }

    private void Current_PostNotifyInput(object sender, NotifyInputEventArgs e)
    {
      KeyEventArgs keyEventArgs = e.StagingItem.Input as KeyEventArgs;
      if (keyEventArgs != null && keyEventArgs.Key == Key.Escape && this.state != null)
      {
        this.CancelEditing();
      }
      else
      {
        if (!(e.StagingItem.Input is MouseEventArgs))
          return;
        this.state.OnMouseMovement();
      }
    }

    private void SelectHoveredElement()
    {
      if (this.state.TopElement == null)
        return;
      Point position = Mouse.GetPosition((IInputElement) this.state.View.SceneScrollViewer);
      if (Keyboard.Modifiers == ModifierKeys.Control && this.state.View.Artboard.ArtboardBounds.Contains(position))
      {
        this.CreatePickWhipContextMenu(position);
      }
      else
      {
        if (!this.host.ElementSelectionStrategy.CanSelectElement(this.state.TopElement))
          return;
        this.host.ElementSelectionStrategy.SelectElement(this.state.TopElement, this.host.EditingProperty);
      }
    }

    private void CreatePickWhipContextMenu(Point location)
    {
      ContextMenu contextMenu = new ContextMenu();
      contextMenu.MinWidth = 150.0;
      foreach (SceneElement element in (IEnumerable<SceneElement>) this.state.GetAllElementsUnderMouse())
      {
        if (this.host.ElementSelectionStrategy.CanSelectElement(element))
          contextMenu.Items.Add(this.CreateMenuItem(element));
      }
      if (contextMenu.Items.Count <= 0)
        return;
      contextMenu.Placement = PlacementMode.RelativePoint;
      contextMenu.PlacementTarget = (UIElement) this.state.View.SceneScrollViewer;
      contextMenu.PlacementRectangle = new Rect()
      {
        Location = location
      };
      contextMenu.IsOpen = true;
    }

    private MenuItem CreateMenuItem(SceneElement element)
    {
      if (element == null)
        return (MenuItem) null;
      MenuItem menuItem = new MenuItem();
      string displayName = element.DisplayName;
      menuItem.Header = (object) displayName;
      menuItem.SetValue(AutomationElement.IdProperty, (object) displayName);
      menuItem.Command = (ICommand) new DelegateCommand((DelegateCommand.SimpleEventHandler) (() => this.host.ElementSelectionStrategy.SelectElement(element, this.host.EditingProperty)));
      return menuItem;
    }

    private class PickState
    {
      private Point lastArtboardPoint = new Point();
      private SceneView view;
      private bool haveLeftPropertyInspector;
      private PreviewElementHighlighter previewHighlighter;
      private SceneElement topElement;
      private TimelineTreeRow hoveredTreeRow;
      private FrameworkElement editor;
      private IElementSelectionStrategy elementSelectionStrategy;
      private TextFeedbackAdorner feedbackAdorner;
      private Cursor cursor;

      internal SceneElement TopElement
      {
        get
        {
          return this.topElement;
        }
      }

      internal SceneView View
      {
        get
        {
          return this.view;
        }
      }

      private TimelineTreeRow HoveredTreeRow
      {
        get
        {
          return this.hoveredTreeRow;
        }
        set
        {
          if (this.hoveredTreeRow == value)
            return;
          if (this.hoveredTreeRow != null)
          {
            this.hoveredTreeRow.BackgroundHighlighted = false;
            this.hoveredTreeRow.TimelineItem.ExtendedTooltip = string.Empty;
          }
          this.hoveredTreeRow = value;
          if (this.hoveredTreeRow == null)
            return;
          this.hoveredTreeRow.BackgroundHighlighted = true;
        }
      }

      public PickState(IPickWhipHost pickWhipHost)
      {
        if (pickWhipHost == null)
          throw new ArgumentNullException("pickWhipHost");
        this.editor = pickWhipHost.PropertyEditor;
        this.elementSelectionStrategy = pickWhipHost.ElementSelectionStrategy;
        this.view = pickWhipHost.EditingProperty.SceneNodeObjectSet.ViewModel.DefaultView;
        this.cursor = pickWhipHost.PickWhipCursor;
        this.feedbackAdorner = new TextFeedbackAdorner(this.view);
        this.feedbackAdorner.Offset = new Vector(18.0, 27.0);
        ToolBehaviorContext toolContext = new ToolBehaviorContext(this.view.DesignerContext.ToolContext, this.view.DesignerContext.ToolManager.ActiveTool, this.view);
        this.previewHighlighter = new PreviewElementHighlighter(this.view.AdornerLayer, (PreviewElementHighlighter.CreateAdornerSet) (adornedElement => (AnimatableAdornerSet) new SelectionPreviewBoundingBoxAdornerSet(toolContext, adornedElement)), (PreviewElementHighlighter.VerifyIsEnabled) (() => true));
      }

      public IList<SceneElement> GetAllElementsUnderMouse()
      {
        Point position = Mouse.GetPosition((IInputElement) this.view.Artboard.ContentArea);
        return this.view.GetElementsInRectangle(new Rect(position.X - 0.5, position.Y - 0.5, 1.0, 1.0), new HitTestModifier(this.view.GetSelectableElement), (InvisibleObjectHitTestModifier) null, false);
      }

      public void ClearState()
      {
        this.editor.Cursor = (Cursor) null;
        this.previewHighlighter.PreviewElement = (SceneElement) null;
        this.topElement = (SceneElement) null;
        this.HoveredTreeRow = (TimelineTreeRow) null;
        this.feedbackAdorner.CloseAdorner();
      }

      public void OnMouseMovement()
      {
        if (this.view == null)
          return;
        Point position = Mouse.GetPosition((IInputElement) this.view.Artboard.ContentArea);
        if (!(position != this.lastArtboardPoint))
          return;
        this.lastArtboardPoint = position;
        this.feedbackAdorner.SetPosition(position);
        if (this.IsOverEditor())
        {
          if (this.haveLeftPropertyInspector)
            this.SetNoDrop();
          else
            this.editor.Cursor = this.cursor;
        }
        else
        {
          this.haveLeftPropertyInspector = true;
          if (this.DoHitTestOverArtboard(position) || this.DoHitTestOverTimeline())
            return;
          this.SetNoDrop();
        }
      }

      private void SetNoDrop()
      {
        this.editor.Cursor = ToolCursors.NoDropCursor;
        this.topElement = (SceneElement) null;
      }

      private bool DoHitTestOverTimeline()
      {
        this.HoveredTreeRow = this.GetTimelineTreeRowUnderMouse();
        if (this.HoveredTreeRow != null)
        {
          SceneElement element = (SceneElement) this.HoveredTreeRow.TimelineItem.SceneNode;
          if (element != null)
          {
            this.previewHighlighter.PreviewElement = element;
            if (this.elementSelectionStrategy.CanSelectElement(element))
            {
              this.editor.Cursor = this.cursor;
              this.topElement = element;
              if (!this.topElement.IsNamed)
                this.HoveredTreeRow.TimelineItem.ExtendedTooltip = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ObjectTreeNamingNotification, new object[1]
                {
                  this.GetCandidateName(this.topElement)
                });
              return true;
            }
          }
          else
            this.previewHighlighter.PreviewElement = (SceneElement) null;
        }
        return false;
      }

      private bool DoHitTestOverArtboard(Point artboardPoint)
      {
        bool flag = false;
        if (this.view.Artboard.ArtboardBounds.Contains(artboardPoint))
        {
          this.topElement = this.view.GetSelectableElementAtPoint(artboardPoint, SelectionFor3D.Deep, false);
          if (this.topElement == null)
            this.topElement = this.view.GetElementAtPoint(artboardPoint, new HitTestModifier(this.view.GetSelectableElement), (InvisibleObjectHitTestModifier) null, (ICollection<BaseFrameworkElement>) null);
          if (this.topElement != null)
          {
            if (this.elementSelectionStrategy.CanSelectElement(this.topElement))
            {
              this.editor.Cursor = this.cursor;
              flag = true;
            }
            if (this.previewHighlighter.PreviewElement != this.topElement)
            {
              if (this.topElement.IsNamed)
                this.feedbackAdorner.Text = this.topElement.Name;
              else if (flag)
                this.feedbackAdorner.Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.NamingNotification, new object[2]
                {
                  this.topElement.DisplayNameNoTextContent,
                  this.GetCandidateName(this.topElement)
                });
              else
                this.feedbackAdorner.Text = this.topElement.DisplayNameNoTextContent;
              this.feedbackAdorner.DrawAdorner();
              this.previewHighlighter.PreviewElement = this.topElement;
            }
          }
          else
          {
            this.feedbackAdorner.CloseAdorner();
            this.previewHighlighter.PreviewElement = (SceneElement) null;
          }
        }
        else
          this.previewHighlighter.PreviewElement = (SceneElement) null;
        return flag;
      }

      private bool IsOverEditor()
      {
        return VisualTreeHelper.HitTest((Visual) this.editor, Mouse.GetPosition((IInputElement) this.editor)) != null;
      }

      private TimelineTreeRow GetTimelineTreeRowUnderMouse()
      {
        if (this.view == null)
          return (TimelineTreeRow) null;
        PaletteRegistryEntry paletteRegistryEntry = this.view.DesignerContext.WindowService.PaletteRegistry["Designer_TimelinePane"];
        if (paletteRegistryEntry != null && paletteRegistryEntry.IsVisible)
        {
          FrameworkElement content = paletteRegistryEntry.Content;
          HitTestResult hitTestResult = VisualTreeHelper.HitTest((Visual) content, Mouse.GetPosition((IInputElement) content));
          if (hitTestResult != null)
            return (TimelineTreeRow) ElementUtilities.GetVisualTreeAncestorOfType(hitTestResult.VisualHit, typeof (TimelineTreeRow));
        }
        return (TimelineTreeRow) null;
      }

      private string GetCandidateName(SceneElement element)
      {
        if (element == null)
          return string.Empty;
        SceneNode root = element.StoryboardContainer as SceneNode ?? this.view.ViewModel.RootNode;
        SceneNodeIDHelper sceneNodeIdHelper = new SceneNodeIDHelper(element.ViewModel, root);
        string namePrefix = SceneNodeIDHelper.DefaultNamePrefixForType((ITypeId) element.Type);
        return sceneNodeIdHelper.GetValidElementID((SceneNode) element, namePrefix);
      }
    }
  }
}
