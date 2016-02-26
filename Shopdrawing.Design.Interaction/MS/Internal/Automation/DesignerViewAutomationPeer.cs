// Decompiled with JetBrains decompiler
// Type: MS.Internal.Automation.DesignerViewAutomationPeer
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Services;
using MS.Internal.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;

namespace MS.Internal.Automation
{
  internal class DesignerViewAutomationPeer : UIElementAutomationPeer, ISelectionProvider, IDisposable
  {
    private DesignerView _owner;
    private ModelService _modelService;
    private Hashtable _dataChildren;
    private List<DesignerItemAutomationPeer> _oldSelectionList;
    private bool _fromContext;
    private ModelItem _root;
    private bool _disposed;
    private EditingContext _context;

    public DesignerView View
    {
      get
      {
        return this._owner;
      }
    }

    public EditingContext Context
    {
      get
      {
        return this._context;
      }
      set
      {
        if (this._context != null)
        {
          this._context.Items.Unsubscribe<Selection>(new SubscribeContextCallback<Selection>(this.OnSelectionChanged));
          if (this.ModelService != null)
            this.ModelService.ModelChanged -= new EventHandler<ModelChangedEventArgs>(this.OnModelChanged);
          this._context.Disposing -= new EventHandler(this.OnContextDisposing);
        }
        this._context = value;
        if (this._context == null)
          return;
        this._context.Disposing += new EventHandler(this.OnContextDisposing);
        if (this._dataChildren == null)
          this.PopulateDataChildren();
        try
        {
          this._fromContext = true;
          this._context.Items.Subscribe<Selection>(new SubscribeContextCallback<Selection>(this.OnSelectionChanged));
        }
        finally
        {
          this._fromContext = false;
        }
        this._oldSelectionList = new List<DesignerItemAutomationPeer>((IEnumerable<DesignerItemAutomationPeer>) this.GetSelectedAutomationPeers(this._context.Items.GetValue<Selection>()));
        if (this.ModelService == null)
          return;
        this._root = this.ModelService.Root;
        this.ModelService.ModelChanged += new EventHandler<ModelChangedEventArgs>(this.OnModelChanged);
      }
    }

    private ModelService ModelService
    {
      get
      {
        if (this._modelService == null && this.Context != null)
          this._modelService = this.Context.Services.GetService<ModelService>();
        return this._modelService;
      }
    }

    public bool CanSelectMultiple
    {
      get
      {
        return true;
      }
    }

    public bool IsSelectionRequired
    {
      get
      {
        return false;
      }
    }

    public DesignerViewAutomationPeer(DesignerView view)
      : base((UIElement) view)
    {
      if (view == null)
        return;
      this._owner = view;
      this.Context = this._owner.Context;
    }

    private void OnModelChanged(object sender, ModelChangedEventArgs e)
    {
      ModelItem root = this.ModelService.Root;
      if (root == this._root)
        return;
      this._root = root;
      this.PopulateDataChildren();
    }

    private void OnContextDisposing(object sender, EventArgs e)
    {
      this.Dispose();
    }

    protected override List<AutomationPeer> GetChildrenCore()
    {
      if (this._disposed)
        return (List<AutomationPeer>) null;
      List<AutomationPeer> list = new List<AutomationPeer>();
      if (this._dataChildren == null)
        this.PopulateDataChildren();
      foreach (DictionaryEntry dictionaryEntry in this._dataChildren)
        list.Add(dictionaryEntry.Value as AutomationPeer);
      return list;
    }

    protected override string GetAutomationIdCore()
    {
      return "WPFDesignerView";
    }

    protected override string GetNameCore()
    {
        return MS.Internal.Properties.Resources.DesignerViewAutomationPeer_Name;
    }

    protected override string GetHelpTextCore()
    {
        return MS.Internal.Properties.Resources.DesignerViewAutomationPeer_HelpText;
    }

    protected override string GetItemTypeCore()
    {
        return MS.Internal.Properties.Resources.DesignerViewAutomationPeer_ItemType;
    }

    protected override string GetClassNameCore()
    {
      return this._owner.GetType().Name;
    }

    public override object GetPattern(PatternInterface patternInterface)
    {
      if (patternInterface == PatternInterface.Selection)
        return (object) this;
      return (object) null;
    }

    protected override bool IsEnabledCore()
    {
      return this._owner.IsEnabled;
    }

    private void OnSelectionChanged(Selection newSelection)
    {
      if (this._fromContext)
        return;
      List<DesignerItemAutomationPeer> list = new List<DesignerItemAutomationPeer>((IEnumerable<DesignerItemAutomationPeer>) this.GetSelectedAutomationPeers(newSelection));
      if (list.Count == 1)
      {
        DesignerItemAutomationPeer itemAutomationPeer = list[0];
        if (itemAutomationPeer != null)
        {
          itemAutomationPeer.RaiseAutomationEvent(AutomationEvents.AutomationFocusChanged);
          itemAutomationPeer.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementSelected);
        }
      }
      else
      {
        foreach (DesignerItemAutomationPeer itemAutomationPeer in this._oldSelectionList)
        {
          if (!list.Contains(itemAutomationPeer))
          {
            itemAutomationPeer.RaiseAutomationEvent(AutomationEvents.AutomationFocusChanged);
            itemAutomationPeer.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection);
          }
        }
        foreach (DesignerItemAutomationPeer itemAutomationPeer in list)
        {
          if (!this._oldSelectionList.Contains(itemAutomationPeer))
          {
            itemAutomationPeer.RaiseAutomationEvent(AutomationEvents.AutomationFocusChanged);
            itemAutomationPeer.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementAddedToSelection);
          }
        }
      }
      this._oldSelectionList = list;
    }

    private void PopulateDataChildren()
    {
      Hashtable hashtable = this._dataChildren;
      if (this.ModelService == null)
        return;
      ModelItem root = this.ModelService.Root;
      if (root == null)
        return;
      this._dataChildren = new Hashtable();
      Canvas canvas = this._owner.Child as Canvas;
      if (canvas != null && canvas.Children != null)
      {
        foreach (object key in canvas.Children)
        {
          Image owner = key as Image;
          if (owner != null)
            this._dataChildren.Add(key, (object) new ImageAutomationPeer(owner));
        }
      }
      DesignerItemAutomationPeer itemAutomationPeer = (hashtable == null ? (DesignerItemAutomationPeer) null : (DesignerItemAutomationPeer) hashtable[(object) root]) ?? new DesignerItemAutomationPeer(root, this);
      if (this._dataChildren[(object) root] != null)
        return;
      this._dataChildren.Add((object) root, (object) itemAutomationPeer);
    }

    public IRawElementProviderSimple GetProviderFromPeer(AutomationPeer peer)
    {
      return this.ProviderFromPeer(peer);
    }

    public IRawElementProviderSimple[] GetSelection()
    {
      if (this.Context == null)
        return new IRawElementProviderSimple[0];
      Selection selection1 = this.Context.Items.GetValue<Selection>();
      List<IRawElementProviderSimple> list = new List<IRawElementProviderSimple>(selection1.SelectionCount);
      if (selection1.SelectionCount == 0)
        return list.ToArray();
      if (this._dataChildren == null)
        this.PopulateDataChildren();
      DesignerItemAutomationPeer itemAutomationPeer = (DesignerItemAutomationPeer) null;
      ModelItem root = this.ModelService.Root;
      if (root != null)
        itemAutomationPeer = (DesignerItemAutomationPeer) this._dataChildren[(object) root];
      foreach (ModelItem modelItem in selection1.SelectedObjects)
      {
        if (this._dataChildren.ContainsKey((object) modelItem) && itemAutomationPeer != null)
          list.Add(itemAutomationPeer.GetProviderFromPeer((AutomationPeer) itemAutomationPeer));
        List<IRawElementProviderSimple> selection2 = itemAutomationPeer.GetSelection((object) modelItem);
        list.AddRange((IEnumerable<IRawElementProviderSimple>) selection2);
      }
      return list.ToArray();
    }

    public List<DesignerItemAutomationPeer> GetSelectedAutomationPeers(Selection newSel)
    {
      List<DesignerItemAutomationPeer> list = new List<DesignerItemAutomationPeer>(newSel.SelectionCount);
      if (newSel.SelectionCount == 0)
        return list;
      DesignerItemAutomationPeer itemAutomationPeer = (DesignerItemAutomationPeer) null;
      ModelItem root = this.ModelService.Root;
      if (root != this._root)
      {
        this._root = root;
        this.PopulateDataChildren();
      }
      if (root != null)
        itemAutomationPeer = (DesignerItemAutomationPeer) this._dataChildren[(object) root];
      foreach (ModelItem modelItem in newSel.SelectedObjects)
      {
        if (this._dataChildren.ContainsKey((object) modelItem) && itemAutomationPeer != null)
          list.Add(itemAutomationPeer);
        List<DesignerItemAutomationPeer> selectedAutomationPeers = itemAutomationPeer.GetSelectedAutomationPeers((object) modelItem);
        list.AddRange((IEnumerable<DesignerItemAutomationPeer>) selectedAutomationPeers);
      }
      return list;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this._disposed)
        return;
      if (disposing)
      {
        this.Context = (EditingContext) null;
        if (this._root != null)
          ((DesignerItemAutomationPeer) this._dataChildren[(object) this._root]).Dispose();
      }
      this._owner = (DesignerView) null;
      this._modelService = (ModelService) null;
      this._dataChildren = (Hashtable) null;
      this._oldSelectionList = (List<DesignerItemAutomationPeer>) null;
      this._root = (ModelItem) null;
      this._disposed = true;
    }
  }
}
