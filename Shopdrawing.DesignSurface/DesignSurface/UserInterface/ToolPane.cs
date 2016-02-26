// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ToolPane
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.Tools.Assets;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Workspaces.Extension;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  internal class ToolPane : Border, IAdvancedLayoutPanel, IComponentConnector
  {
    public static readonly DependencyProperty ParentOrientationProperty = DependencyProperty.RegisterAttached("ParentOrientation", typeof (Orientation), typeof (ToolPane), (PropertyMetadata) new FrameworkPropertyMetadata((object) Orientation.Horizontal));
    private Dictionary<ToolCategory, ToolGroupButton> toolGroupButtons = new Dictionary<ToolCategory, ToolGroupButton>();
    private Dictionary<Tool, ToolGroupItem> toolToWrapperMapping = new Dictionary<Tool, ToolGroupItem>();
    private ToolManager toolManager;
    private DesignerContext designerContext;
    private StackPanel currentToolGroupPanel;
    private Orientation? dockingOrientation;
    internal Border ToolsRootBorder;
    internal StackPanel ToolsRoot;
    private bool _contentLoaded;

    public Orientation? DockingOrientation
    {
      get
      {
        return this.dockingOrientation;
      }
      set
      {
        this.dockingOrientation = value;
        Orientation? nullable = value;
        if ((nullable.GetValueOrDefault() != Orientation.Vertical ? 0 : (nullable.HasValue ? true : false)) != 0)
        {
          this.ToolsRoot.Orientation = Orientation.Horizontal;
          this.ToolsRootBorder.HorizontalAlignment = HorizontalAlignment.Left;
          this.ToolsRootBorder.VerticalAlignment = VerticalAlignment.Stretch;
          this.ToolsRootBorder.SetResourceReference(Border.CornerRadiusProperty, (object) "GripperCornerRoundingRight");
        }
        else
        {
          this.ToolsRoot.Orientation = Orientation.Vertical;
          this.ToolsRootBorder.HorizontalAlignment = HorizontalAlignment.Stretch;
          this.ToolsRootBorder.VerticalAlignment = VerticalAlignment.Top;
          this.ToolsRootBorder.SetResourceReference(Border.CornerRadiusProperty, (object) "GripperCornerRoundingBottom");
        }
      }
    }

    public ToolPane(DesignerContext designerContext, ToolContext toolContext)
    {
      this.InitializeComponent();
      this.toolManager = designerContext.ToolManager;
      this.designerContext = designerContext;
      this.toolManager.ActiveToolChanged += new ToolEventHandler(this.ToolManager_ActiveToolChanged);
      this.toolManager.ToolAdded += new ToolEventHandler(this.ToolManager_ToolAdded);
      this.toolManager.ToolRemoved += new ToolEventHandler(this.ToolManager_ToolRemoved);
      this.designerContext.SelectionManager.LateActiveSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.SelectionManager_LateActiveSceneUpdatePhase);
      this.AddToolGroupButton(ToolCategory.Selection);
      this.AddToolGroupButton(ToolCategory.Subselection);
      this.AddToolSeparator();
      this.AddToolGroupButton(ToolCategory.Pan);
      this.AddToolGroupButton(ToolCategory.Zoom);
      this.AddToolGroupButton(ToolCategory.ThreeD);
      this.AddToolSeparator();
      this.AddToolGroupButton(ToolCategory.Eyedropper);
      this.AddToolGroupButton(ToolCategory.PaintBucket);
      this.AddToolGroupButton(ToolCategory.BrushTransform);
      this.AddToolSeparator();
      this.AddToolGroupButton(ToolCategory.Drawing);
      this.AddToolGroupButton(ToolCategory.ShapePrimitives);
      this.AddToolGroupButton(ToolCategory.LayoutPanels);
      this.AddToolGroupButton(ToolCategory.Text);
      this.AddToolGroupButton(ToolCategory.CommonControls);
      this.CreateAssetToolGroup();
      this.CreateAssetPopupAndButton(designerContext, toolContext);
      this.AddToolGroupButton(ToolCategory.Asset);
      foreach (Tool tool in this.toolManager.Tools)
        this.Add(tool);
      this.IsEnabled = false;
    }

    public static Orientation GetParentOrientation(DependencyObject target)
    {
      return (Orientation) target.GetValue(ToolPane.ParentOrientationProperty);
    }

    public static void SetParentOrientation(DependencyObject target, Orientation value)
    {
      target.SetValue(ToolPane.ParentOrientationProperty, (object) value);
    }

    private void Add(Tool tool)
    {
      ToolGroupButton toolGroupButton = (ToolGroupButton) null;
      if (!this.toolGroupButtons.TryGetValue(tool.Category, out toolGroupButton))
        return;
      ToolGroupItem tool1 = new ToolGroupItem(tool);
      toolGroupButton.AddTool(tool1);
      if (tool == this.toolManager.ActiveTool)
        toolGroupButton.SetActive(tool1);
      this.toolToWrapperMapping[tool] = tool1;
    }

    private void AddToolGroupButton(ToolCategory category)
    {
      ToolGroupButton toolGroupButton = new ToolGroupButton(category.ToString(), category == ToolCategory.Asset);
      this.toolGroupButtons[category] = toolGroupButton;
      if (this.currentToolGroupPanel == null)
        this.currentToolGroupPanel = this.ToolsRoot;
      toolGroupButton.SetBinding(ToolGroupButton.ToolSelectionPlacementProperty, (BindingBase) new Binding()
      {
        Source = (object) this.ToolsRoot,
        Path = new PropertyPath("Orientation", new object[0]),
        Converter = (IValueConverter) new InverseOrientationToPopupPlacementConverter(),
        Mode = BindingMode.OneWay
      });
      toolGroupButton.SetBinding(ToolPane.ParentOrientationProperty, (BindingBase) new Binding()
      {
        Source = (object) this.ToolsRoot,
        Path = new PropertyPath("Orientation", new object[0])
      });
      this.currentToolGroupPanel.Children.Add((UIElement) toolGroupButton);
      toolGroupButton.ToolGroupSelectionChanged += new EventHandler(this.ToolGroupButton_ToolGroupSelectionChanged);
      toolGroupButton.ToolGroupActiveChanged += new EventHandler(this.ToolGroupButton_ToolGroupActiveChanged);
      toolGroupButton.Focusable = false;
    }

    private void AddToolSeparator()
    {
      ToolSeparator toolSeparator = new ToolSeparator();
      toolSeparator.SetBinding(ToolPane.ParentOrientationProperty, (BindingBase) new Binding()
      {
        Source = (object) this.ToolsRoot,
        Path = new PropertyPath("Orientation", new object[0])
      });
      if (this.currentToolGroupPanel == null)
        this.currentToolGroupPanel = this.ToolsRoot;
      this.currentToolGroupPanel.Children.Add((UIElement) toolSeparator);
    }

    private void CreateAssetToolGroup()
    {
      Border border = new Border();
      border.SetResourceReference(FrameworkElement.StyleProperty, (object) "ToolGroupStyle");
      this.currentToolGroupPanel = new StackPanel();
      border.Child = (UIElement) this.currentToolGroupPanel;
      Binding binding = new Binding();
      binding.Source = (object) this.ToolsRoot;
      binding.Path = new PropertyPath("Orientation", new object[0]);
      border.SetBinding(ToolPane.ParentOrientationProperty, (BindingBase) binding);
      this.currentToolGroupPanel.SetBinding(StackPanel.OrientationProperty, (BindingBase) binding);
      this.ToolsRoot.Children.Add((UIElement) border);
    }

    private void CreateAssetPopupAndButton(DesignerContext designerContext, ToolContext toolContext)
    {
      ToggleButton button = new ToggleButton();
      button.Name = "AssetToolMoreButton";
      button.ToolTip = (object) StringTable.AssetToolMoreButtonToolTip;
      button.SetResourceReference(FrameworkElement.StyleProperty, (object) "MoreButtonStyle");
      this.currentToolGroupPanel.Children.Add((UIElement) button);
      AssetPopup assetPopup = new AssetPopup(designerContext.AssetLibrary, designerContext.Configuration);
      assetPopup.PlacementTarget = (UIElement) button;
      button.ClickMode = ClickMode.Press;
      button.Checked += (RoutedEventHandler) ((s, e) => assetPopup.IsOpen = true);
      assetPopup.Closed += (EventHandler) ((s, e) => button.IsChecked = new bool?(false));
      assetPopup.SetBinding(Popup.PlacementProperty, (BindingBase) new Binding()
      {
        Source = (object) this.ToolsRoot,
        Path = new PropertyPath("Orientation", new object[0]),
        Converter = (IValueConverter) new InverseOrientationToPopupPlacementConverter(),
        Mode = BindingMode.OneWay
      });
      ICommandService commandService = this.designerContext.CommandService;
      commandService.AddTarget((ICommandTarget) new ToolPane.ToolPaneCommandTarget(commandService, assetPopup));
      assetPopup.AssetView.SelectedAssetChanged += (EventHandler<AssetEventArgs>) ((s, e) => toolContext.AssetMruList.OnSelectAsset(e.Asset));
      assetPopup.AssetView.AssetSingleClicked += (EventHandler<AssetEventArgs>) ((s, e) => assetPopup.IsOpen = false);
      assetPopup.Closed += (EventHandler) ((s, e) =>
      {
        toolContext.AssetMruList.ActivateAssetTool();
        if (designerContext.ActiveView == null)
          return;
        designerContext.ActiveView.ReturnFocus();
      });
    }

    private void ToolGroupButton_ToolGroupSelectionChanged(object sender, EventArgs e)
    {
      ToolGroupButton toolGroupButton = sender as ToolGroupButton;
      if (toolGroupButton == null || toolGroupButton.ActiveTool == null)
        return;
      this.toolManager.ActiveTool = toolGroupButton.ActiveTool.Tool;
    }

    private void ToolGroupButton_ToolGroupActiveChanged(object sender, EventArgs e)
    {
      ToolGroupButton toolGroupButton = sender as ToolGroupButton;
      if (toolGroupButton == null || toolGroupButton.ActiveTool == null)
        return;
      this.toolManager.SetCategoryActiveTool(toolGroupButton.ActiveTool.Tool.Category, toolGroupButton.ActiveTool.Tool);
    }

    private void ToolManager_ActiveToolChanged(object sender, ToolEventArgs args)
    {
      ToolGroupItem tool = (ToolGroupItem) null;
      if (args.Tool != null)
        this.toolToWrapperMapping.TryGetValue(args.Tool, out tool);
      foreach (ToolGroupButton toolGroupButton in this.toolGroupButtons.Values)
        toolGroupButton.SetActive(tool);
    }

    private void ToolManager_ToolAdded(object sender, ToolEventArgs args)
    {
      this.Add(args.Tool);
    }

    private void ToolManager_ToolRemoved(object sender, ToolEventArgs args)
    {
      ToolGroupItem tool = this.toolToWrapperMapping[args.Tool];
      foreach (ToolGroupButton toolGroupButton in this.toolGroupButtons.Values)
      {
        if (toolGroupButton.RemoveTool(tool))
          break;
      }
      this.toolToWrapperMapping.Remove(args.Tool);
    }

    private void SelectionManager_LateActiveSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      if (!args.IsDirtyViewState(SceneViewModel.ViewStateBits.IsEditable | SceneViewModel.ViewStateBits.ElementSelection | SceneViewModel.ViewStateBits.ActiveSceneInsertionPoint | SceneViewModel.ViewStateBits.LockedInsertionPoint))
        return;
      this.RevalidateToolEnabledState();
    }

    private void RevalidateToolEnabledState()
    {
      SceneView activeView = this.designerContext.ActiveView;
      this.IsEnabled = activeView != null && activeView.IsDesignSurfaceEnabled;
      foreach (ToolGroupButton toolGroupButton in this.toolGroupButtons.Values)
      {
        ToolGroupItem activeTool = toolGroupButton.ActiveTool;
        toolGroupButton.UpdateIsEnabled();
        if (toolGroupButton.IsChecked.HasValue && toolGroupButton.IsChecked.Value)
        {
          if (!toolGroupButton.IsEnabled || toolGroupButton.ActiveTool == null || !toolGroupButton.ActiveTool.IsVisible)
            this.toolManager.ActiveTool = this.toolManager.Tools[0];
          else if (activeTool != toolGroupButton.ActiveTool)
            this.toolManager.ActiveTool = toolGroupButton.ActiveTool.Tool;
        }
      }
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/toolpane/toolpane.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.ToolsRootBorder = (Border) target;
          break;
        case 2:
          this.ToolsRoot = (StackPanel) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }

    private class ToolPaneCommandTarget : CommandTarget
    {
      public ToolPaneCommandTarget(ICommandService commandManager, AssetPopup assetPopup)
      {
        this.AddCommand("ToolPane_OpenAssetToolPopup", (Microsoft.Expression.Framework.Commands.ICommand) new ToolPane.ToolPaneCommandTarget.OpenAssetToolPopupCommand(assetPopup));
        commandManager.SetCommandPropertyDefault("ToolPane_OpenAssetToolPopup", "Shortcuts", (object) new KeyBinding[1]
        {
          new KeyBinding()
          {
            Modifiers = ModifierKeys.Control,
            Key = Key.OemPeriod
          }
        });
      }

      private class OpenAssetToolPopupCommand : Command
      {
        private AssetPopup assetPopup;

        public OpenAssetToolPopupCommand(AssetPopup assetPopup)
        {
          this.assetPopup = assetPopup;
        }

        public override void Execute()
        {
          this.assetPopup.IsOpen = !this.assetPopup.IsOpen;
        }
      }
    }
  }
}
