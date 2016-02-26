// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.ToolManager
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.Tools
{
  public class ToolManager : IDisposable
  {
    private List<Tool> tools = new List<Tool>();
    private Dictionary<ToolCategory, ToolCategoryGroup> toolCategoryGroups = new Dictionary<ToolCategory, ToolCategoryGroup>();
    private bool? showBoundaries = new bool?();
    private bool? showAlignmentAdorners = new bool?();
    private bool? showSelectionPreview = new bool?();
    private bool? showActiveContainer = new bool?();
    private Tool activeTool;
    private Tool pausedTool;
    private Tool overrideTool;
    private Tool candidateOverrideTool;
    private DispatcherTimer dispatcherTimer;
    private IViewService viewService;
    private IConfigurationObject configuration;

    public List<Tool> Tools
    {
      get
      {
        return this.tools;
      }
    }

    public Tool LastSelectionTool { get; private set; }

    internal Tool OverrideTool
    {
      get
      {
        return this.overrideTool;
      }
      set
      {
        if (this.candidateOverrideTool == value)
          return;
        this.candidateOverrideTool = value;
        if (this.dispatcherTimer == null)
        {
          this.dispatcherTimer = new DispatcherTimer(DispatcherPriority.Input);
          this.dispatcherTimer.Interval = TimeSpan.FromSeconds(0.05);
          this.dispatcherTimer.Tick += (EventHandler) ((sender, e) =>
          {
            if (InputManager.Current.PrimaryMouseDevice.LeftButton == MouseButtonState.Pressed)
              this.candidateOverrideTool = (Tool) null;
            this.RealizeOverrideTool();
            this.dispatcherTimer.Stop();
          });
        }
        this.dispatcherTimer.Start();
      }
    }

    public Tool ActiveTool
    {
      get
      {
        return this.activeTool;
      }
      set
      {
        if (this.pausedTool != null && value != null)
        {
          this.pausedTool = value;
        }
        else
        {
          bool flag = this.activeTool != value;
          if (this.activeTool != null)
          {
            if (this.activeTool is ISelectionTool)
              this.LastSelectionTool = this.activeTool;
            this.activeTool.Deactivate();
          }
          this.activeTool = value;
          if (this.activeTool != null)
            this.activeTool.Activate();
          if (!flag)
            return;
          this.OnActiveToolChanged(new ToolEventArgs(value));
        }
      }
    }

    public IEnumerable ToolCategoryGroups
    {
      get
      {
        return (IEnumerable) this.toolCategoryGroups.Values;
      }
    }

    public bool ShowBoundaries
    {
      get
      {
        if (this.configuration != null && !this.showBoundaries.HasValue)
          this.showBoundaries = new bool?((bool) this.configuration.GetProperty("ShowBoundaries", (object) false));
        bool? nullable = this.showBoundaries;
        if (nullable.GetValueOrDefault())
          return nullable.HasValue;
        return false;
      }
      set
      {
        this.showBoundaries = new bool?(value);
        if (this.configuration != null)
          this.configuration.SetProperty("ShowBoundaries", (object) (bool) (value ? true : false));
        this.activeTool.RebuildAdornerSets();
      }
    }

    public bool ShowAlignmentAdorners
    {
      get
      {
        if (this.configuration != null && !this.showAlignmentAdorners.HasValue)
          this.showAlignmentAdorners = new bool?((bool) this.configuration.GetProperty("ShowAlignmentAdorners", (object) true));
        bool? nullable = this.showAlignmentAdorners;
        if (nullable.GetValueOrDefault())
          return nullable.HasValue;
        return false;
      }
      set
      {
        this.showAlignmentAdorners = new bool?(value);
        if (this.configuration != null)
          this.configuration.SetProperty("ShowAlignmentAdorners", (object) (bool) (value ? true : false));
        this.activeTool.RebuildAdornerSets();
      }
    }

    public bool ShowSelectionPreview
    {
      get
      {
        if (this.configuration != null && !this.showSelectionPreview.HasValue)
          this.showSelectionPreview = new bool?((bool) this.configuration.GetProperty("ShowSelectionPreview", (object) true));
        bool? nullable = this.showSelectionPreview;
        if (nullable.GetValueOrDefault())
          return nullable.HasValue;
        return false;
      }
      set
      {
        this.showSelectionPreview = new bool?(value);
        if (this.configuration != null)
          this.configuration.SetProperty("ShowSelectionPreview", (object) (bool) (value ? true : false));
        this.activeTool.RebuildAdornerSets();
      }
    }

    public bool ShowActiveContainer
    {
      get
      {
        if (this.configuration != null && !this.showActiveContainer.HasValue)
          this.showActiveContainer = new bool?((bool) this.configuration.GetProperty("ShowActiveContainer", (object) true));
        bool? nullable = this.showActiveContainer;
        if (nullable.GetValueOrDefault())
          return nullable.HasValue;
        return false;
      }
      set
      {
        this.showActiveContainer = new bool?(value);
        if (this.configuration != null)
          this.configuration.SetProperty("ShowActiveContainer", (object) (bool) (value ? true : false));
        this.activeTool.RebuildAdornerSets();
      }
    }

    public event ToolEventHandler ActiveToolChanged;

    public event ToolEventHandler OverrideToolChanged;

    public event ToolEventHandler ToolAdded;

    public event ToolEventHandler ToolRemoved;

    public ToolManager(IViewService viewService, IConfigurationObject configuration)
    {
      this.viewService = viewService;
      this.configuration = configuration;
      if (this.viewService != null)
      {
        this.viewService.ActiveViewChanging += new ViewChangedEventHandler(this.ViewService_ActiveViewChanging);
        this.viewService.ActiveViewChanged += new ViewChangedEventHandler(this.ViewService_ActiveViewChanged);
      }
      this.toolCategoryGroups[ToolCategory.Text] = (ToolCategoryGroup) new TextToolCategoryGroup(ToolCategory.Text);
    }

    public void Dispose()
    {
      if (this.viewService != null)
      {
        this.viewService.ActiveViewChanging -= new ViewChangedEventHandler(this.ViewService_ActiveViewChanging);
        this.viewService.ActiveViewChanged -= new ViewChangedEventHandler(this.ViewService_ActiveViewChanged);
      }
      this.viewService = (IViewService) null;
    }

    private void RealizeOverrideTool()
    {
      if (this.overrideTool == this.candidateOverrideTool)
        return;
      if (this.ActiveTool != null)
        this.ActiveTool.ClearAdornerSets();
      this.overrideTool = this.candidateOverrideTool;
      this.OnOverrideToolChanged(new ToolEventArgs(this.overrideTool));
      if (this.ActiveTool == null)
        return;
      this.ActiveTool.RebuildAdornerSets();
    }

    public void Add(Tool tool)
    {
      this.tools.Add(tool);
      this.OnToolAdded(new ToolEventArgs(tool));
      if (this.tools.Count != 1)
        return;
      this.ActiveTool = tool;
    }

    public void Remove(Tool tool)
    {
      bool flag1 = false;
      bool flag2 = false;
      if (this.activeTool == tool)
      {
        this.ActiveTool = (Tool) null;
        flag1 = true;
      }
      if (this.pausedTool == tool)
      {
        this.pausedTool = (Tool) null;
        flag2 = true;
      }
      this.tools.Remove(tool);
      if (this.tools.Count > 0)
      {
        if (flag1)
          this.ActiveTool = this.tools[0];
        if (flag2)
          this.pausedTool = this.tools[0];
      }
      this.OnToolRemoved(new ToolEventArgs(tool));
    }

    public Tool FindTool(Type type)
    {
      foreach (Tool tool in this.tools)
      {
        if (tool.GetType() == type)
          return tool;
      }
      return (Tool) null;
    }

    public void SetCategoryActiveTool(ToolCategory category, Tool tool)
    {
      ToolCategoryGroup toolCategoryGroup;
      if (!this.toolCategoryGroups.TryGetValue(category, out toolCategoryGroup))
        return;
      toolCategoryGroup.ActiveTool = tool;
    }

    public Key GetToolCategoryKey(ToolCategory category)
    {
      ToolCategoryGroup toolCategoryGroup;
      if (this.toolCategoryGroups.TryGetValue(category, out toolCategoryGroup))
        return toolCategoryGroup.Key;
      return Key.None;
    }

    private void OnActiveToolChanged(ToolEventArgs e)
    {
      if (this.ActiveToolChanged == null)
        return;
      this.ActiveToolChanged((object) this, e);
    }

    private void OnOverrideToolChanged(ToolEventArgs e)
    {
      if (this.OverrideToolChanged == null)
        return;
      this.OverrideToolChanged((object) this, e);
    }

    private void OnToolAdded(ToolEventArgs e)
    {
      if (this.ToolAdded == null)
        return;
      this.ToolAdded((object) this, e);
    }

    private void OnToolRemoved(ToolEventArgs e)
    {
      if (this.ToolRemoved == null)
        return;
      this.ToolRemoved((object) this, e);
    }

    private void ViewService_ActiveViewChanging(object sender, ViewChangedEventArgs args)
    {
      Tool activeTool = this.ActiveTool;
      this.ActiveTool = (Tool) null;
      this.pausedTool = activeTool;
      if (args.NewView != null)
        return;
      this.overrideTool = (Tool) null;
      this.candidateOverrideTool = (Tool) null;
      if (this.dispatcherTimer == null)
        return;
      this.dispatcherTimer.Stop();
    }

    private void ViewService_ActiveViewChanged(object sender, ViewChangedEventArgs args)
    {
      if (this.pausedTool == null)
        return;
      Tool tool = this.pausedTool;
      this.pausedTool = (Tool) null;
      this.ActiveTool = tool;
    }
  }
}
