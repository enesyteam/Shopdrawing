// Decompiled with JetBrains decompiler
// Type: MS.Internal.Automation.DesignerItemAutomationPeer
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Services;
using MS.Internal;
using MS.Internal.Properties;
using MS.Internal.Transforms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace MS.Internal.Automation
{
  internal class DesignerItemAutomationPeer : AutomationPeer, ISelectionItemProvider, IDisposable
  {
    private static readonly TypeIdentifier Type = new TypeIdentifier("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "AutomationProperties");
    private static readonly PropertyIdentifier AutomationIdProperty = new PropertyIdentifier(DesignerItemAutomationPeer.Type, "AutomationId");
    private static readonly PropertyIdentifier HelpTextProperty = new PropertyIdentifier(DesignerItemAutomationPeer.Type, "HelpText");
    private static readonly PropertyIdentifier NameProperty = new PropertyIdentifier(DesignerItemAutomationPeer.Type, "Name");
    private const int MAX_CLICK_POINTS_TESTS = 1000;
    private const string _xmlns = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
    private ModelItem _item;
    private DesignerView _view;
    private ModelService _modelService;
    private Hashtable _dataChildren;
    private DesignerViewAutomationPeer _viewPeer;
    private bool _disposed;

    private ModelService ModelService
    {
      get
      {
        if (this._modelService == null && this._view.Context != null)
          this._modelService = this._view.Context.Services.GetService<ModelService>();
        return this._modelService;
      }
    }

    private ViewItem ViewItem
    {
      get
      {
        return this._item.View;
      }
    }

    public bool IsSelected
    {
      get
      {
        foreach (ModelItem modelItem in this._view.Context.Items.GetValue<Selection>().SelectedObjects)
        {
          if (this._item == modelItem)
            return true;
        }
        return false;
      }
    }

    public IRawElementProviderSimple SelectionContainer
    {
      get
      {
        return this._viewPeer.GetProviderFromPeer((AutomationPeer) this._viewPeer);
      }
    }

    public DesignerItemAutomationPeer(ModelItem item, DesignerViewAutomationPeer peer)
    {
      this._item = item;
      this._view = peer.View;
      this._viewPeer = peer;
    }

    private string GetModelProperty(PropertyIdentifier property)
    {
      ModelProperty modelProperty = this._item.Properties.Find(property);
      if (modelProperty != (ModelProperty) null)
        return modelProperty.ComputedValue as string;
      return (string) null;
    }

    protected override List<AutomationPeer> GetChildrenCore()
    {
      List<AutomationPeer> list1 = new List<AutomationPeer>();
      List<UIElement> list2 = new List<UIElement>();
      ICollection<UIElement> adorners = this._view.Adorners;
      if (adorners != null)
      {
        foreach (UIElement uiElement1 in new List<UIElement>((IEnumerable<UIElement>) adorners))
        {
          ModelItem model = AdornerProperties.GetModel((DependencyObject) uiElement1);
          if (model == this._item)
          {
            Panel panel = uiElement1 as Panel;
            if (panel != null)
            {
              foreach (UIElement uiElement2 in panel.Children)
                list2.Add(uiElement2);
            }
            else
              list2.Add(uiElement1);
          }
          else if (model == null)
          {
            Panel panel = uiElement1 as Panel;
            if (panel != null)
            {
              foreach (UIElement uiElement2 in panel.Children)
              {
                if (AdornerProperties.GetModel((DependencyObject) uiElement2) == this._item)
                  list2.Add(uiElement2);
              }
            }
          }
        }
      }
      if (list2.Count > 0)
        list1.Add((AutomationPeer) new AdornerNodeAutomationPeer(list2));
      this.PopulateDataChildren(list1);
      return list1;
    }

    private IEnumerable<ModelItem> GetChildren(ModelItem parent)
    {
      if (!parent.ItemType.IsSubclassOf(typeof (UserControl)))
      {
        ModelProperty content = parent.Content;
        if (!(content == (ModelProperty) null))
        {
          if (content.IsCollection)
          {
            foreach (ModelItem modelItem in content.Collection)
            {
              if (modelItem.View != (ViewItem) null)
                yield return modelItem;
            }
          }
          else if (content.Value != null && content.Value.View != (ViewItem) null && content.IsSet)
            yield return content.Value;
        }
      }
    }

    private void PopulateDataChildren(List<AutomationPeer> list)
    {
      Hashtable hashtable = this._dataChildren;
      List<ModelItem> list1 = new List<ModelItem>(this.GetChildren(this._item));
      this._dataChildren = new Hashtable();
      foreach (ModelItem modelItem in list1)
      {
        DesignerItemAutomationPeer itemAutomationPeer = (hashtable == null ? (DesignerItemAutomationPeer) null : (DesignerItemAutomationPeer) hashtable[(object) modelItem]) ?? new DesignerItemAutomationPeer(modelItem, this._viewPeer);
        if (this._dataChildren[(object) modelItem] == null)
          this._dataChildren.Add((object) modelItem, (object) itemAutomationPeer);
      }
      if (list1 == null)
        return;
      for (int index1 = list1.Count - 1; index1 >= 0; --index1)
      {
        object index2 = (object) list1[index1];
        list.Add(this._dataChildren[index2] as AutomationPeer);
      }
    }

    public IRawElementProviderSimple GetProviderFromPeer(AutomationPeer peer)
    {
      return this.ProviderFromPeer(peer);
    }

    public List<IRawElementProviderSimple> GetSelection(object selectedObj)
    {
      if (this._dataChildren == null)
        this.GetChildren();
      List<IRawElementProviderSimple> list = new List<IRawElementProviderSimple>();
      if (this._dataChildren.ContainsKey(selectedObj))
      {
        DesignerItemAutomationPeer itemAutomationPeer = this._dataChildren[selectedObj] as DesignerItemAutomationPeer;
        if (itemAutomationPeer != null)
          list.Add(itemAutomationPeer.ProviderFromPeer((AutomationPeer) itemAutomationPeer));
      }
      else
      {
        foreach (DictionaryEntry dictionaryEntry in this._dataChildren)
        {
          DesignerItemAutomationPeer itemAutomationPeer = dictionaryEntry.Value as DesignerItemAutomationPeer;
          if (itemAutomationPeer != null)
            list.AddRange((IEnumerable<IRawElementProviderSimple>) itemAutomationPeer.GetSelection(selectedObj));
        }
      }
      return list;
    }

    protected override AutomationControlType GetAutomationControlTypeCore()
    {
      return AutomationControlType.Custom;
    }

    protected override string GetLocalizedControlTypeCore()
    {
        return MS.Internal.Properties.Resources.DesignerItemAutomationPeer_LocalizedControlType;
    }

    protected override string GetAutomationIdCore()
    {
      string str = this._item.Name;
      if (string.IsNullOrEmpty(str))
        str = this.GetModelProperty(DesignerItemAutomationPeer.AutomationIdProperty);
      return str ?? string.Empty;
    }

    protected override Rect GetBoundingRectangleCore()
    {
      if (this._item.Parent == null && this._item != this._item.Root && this._item.Name != null)
      {
        ModelItem modelItem = this.ModelService.FromName(this.ModelService.Root, this._item.Name, StringComparison.OrdinalIgnoreCase);
        if (modelItem != null)
          this._item = modelItem;
      }
      PresentationSource presentationSource = PresentationSource.FromVisual((Visual) DesignerView.FromContext(this._item.Context));
      if (presentationSource == null || presentationSource.RootVisual == null || !this.ViewItem.IsVisible)
        return new Rect();
      Rect selectionFrameBounds = ElementUtilities.GetSelectionFrameBounds(this.ViewItem);
      if (selectionFrameBounds.Location == new Point(0.0, 0.0) && selectionFrameBounds.Size == new Size(0.0, 0.0))
        return new Rect();
      Rect rect1 = new Rect(selectionFrameBounds.Size);
      Rect rect2 = TransformUtil.GetSelectionFrameTransformToParentVisual(this.ViewItem, presentationSource.RootVisual).TransformBounds(rect1);
      return new Rect(presentationSource.RootVisual.PointToScreen(rect2.Location), rect2.Size);
    }

    protected override string GetClassNameCore()
    {
      return this._item.ItemType.Name;
    }

    protected override string GetAcceleratorKeyCore()
    {
      return string.Empty;
    }

    protected override string GetAccessKeyCore()
    {
      return string.Empty;
    }

    protected override Point GetClickablePointCore()
    {
      if (this._item.Parent == null && this._item != this._item.Root && (this.ModelService != null && this._item.Name != null))
      {
        ModelItem root = this.ModelService.Root;
        if (root != null)
        {
          ModelItem modelItem = this.ModelService.FromName(root, this._item.Name, StringComparison.OrdinalIgnoreCase);
          if (modelItem != null)
            this._item = modelItem;
        }
      }
      ViewItem viewItem = this.ViewItem;
      UIElement uiElement = (UIElement) this._view;
      ViewItem ancestorView = this.ModelService == null || this.ModelService.Root == null ? (ViewItem) null : this.ModelService.Root.View;
      if (uiElement == null || viewItem == (ViewItem) null || PresentationSource.FromVisual((Visual) uiElement) == null)
        return new Point(0.0, 0.0);
      Rect selectionFrameBounds = viewItem.SelectionFrameBounds;
      double width = selectionFrameBounds.Width;
      double height = selectionFrameBounds.Height;
      double num1 = 2.0;
      int num2 = 0;
      Point point1 = new Point();
      Transform transformToDesignerView = TransformUtil.GetSelectionFrameTransformToDesignerView(this._item.Context, viewItem);
      Transform transformToParentView = TransformUtil.GetSelectionFrameTransformToParentView(viewItem, ancestorView);
      while (num1 <= width || num1 <= height)
      {
        int num3 = 1;
        double x = width / num1;
        for (; (double) num3 < num1; ++num3)
        {
          int num4 = 1;
          double y = height / num1;
          for (; (double) num4 < num1; ++num4)
          {
            if (num3 % 2 != 0 || num4 % 2 != 0)
            {
              if (++num2 > 1000)
                return new Point();
              Point point2 = transformToDesignerView.Transform(new Point(x, y));
              HitTestResult hitTestResult = HitTestHelper.HitTest((Visual) uiElement, point2, false, (HitTestFilterCallback) null);
              if (hitTestResult != null && hitTestResult.VisualHit is DesignerView.OpaqueElement)
              {
                Point point3 = transformToParentView.Transform(new Point(x, y));
                ViewHitTestResult viewHitTestResult = ancestorView.HitTest((ViewHitTestFilterCallback) null, (ViewHitTestResultCallback) null, (HitTestParameters) new PointHitTestParameters(point3));
                if (viewHitTestResult != null && viewHitTestResult.ViewHit != (ViewItem) null)
                {
                  ViewItem viewHit = viewHitTestResult.ViewHit;
                  bool flag1 = false;
                  if (viewItem != viewHit)
                    flag1 = DesignerItemAutomationPeer.IsPartOfTemplate(viewItem, viewHitTestResult.ViewHit);
                  bool flag2 = false;
                  if (viewItem.ItemType.IsSubclassOf(typeof (UserControl)))
                    flag2 = viewHitTestResult.ViewHit.IsDescendantOf(viewItem);
                  if (viewItem == viewHitTestResult.ViewHit || flag1 || flag2)
                    return ancestorView.PointToScreen(point3);
                }
              }
            }
            y += height / num1;
          }
          x += width / num1;
        }
        num1 *= 2.0;
      }
      return point1;
    }

    protected override string GetHelpTextCore()
    {
      string str = this.GetModelProperty(DesignerItemAutomationPeer.HelpTextProperty);
      if (string.IsNullOrEmpty(str))
      {
        DescriptionAttribute descriptionAttribute = TypeDescriptor.GetAttributes((object) this._item)[typeof (DescriptionAttribute)] as DescriptionAttribute;
        if (descriptionAttribute != null)
          str = descriptionAttribute.Description;
      }
      return str;
    }

    protected override string GetItemStatusCore()
    {
      return string.Empty;
    }

    protected override string GetItemTypeCore()
    {
      return this._item.ItemType.Name;
    }

    protected override AutomationPeer GetLabeledByCore()
    {
      return (AutomationPeer) null;
    }

    protected override AutomationOrientation GetOrientationCore()
    {
      return AutomationOrientation.None;
    }

    protected override bool HasKeyboardFocusCore()
    {
      return true;
    }

    protected override bool IsEnabledCore()
    {
      return this._viewPeer.IsEnabled();
    }

    protected override bool IsKeyboardFocusableCore()
    {
      return true;
    }

    protected override bool IsOffscreenCore()
    {
      if (this.ViewItem != (ViewItem) null)
        return this.ViewItem.IsOffscreen;
      return false;
    }

    protected override bool IsPasswordCore()
    {
      return false;
    }

    protected override bool IsRequiredForFormCore()
    {
      return false;
    }

    protected override void SetFocusCore()
    {
      SelectionOperations.SelectOnly(this._view.Context, this._item);
    }

    protected override string GetNameCore()
    {
      string str = this.GetModelProperty(DesignerItemAutomationPeer.NameProperty);
      if (string.IsNullOrEmpty(str))
        str = this._item.ItemType.Name;
      return str;
    }

    public override object GetPattern(PatternInterface patternInterface)
    {
      if (patternInterface == PatternInterface.SelectionItem)
        return (object) this;
      return (object) null;
    }

    protected override bool IsContentElementCore()
    {
      using (IEnumerator<object> enumerator = this._item.GetAttributes(typeof (ContentPropertyAttribute)).GetEnumerator())
      {
        if (enumerator.MoveNext())
        {
          ContentPropertyAttribute propertyAttribute = (ContentPropertyAttribute) enumerator.Current;
          return true;
        }
      }
      return false;
    }

    protected override bool IsControlElementCore()
    {
      return true;
    }

    private static bool IsPartOfTemplate(ViewItem parent, ViewItem childToCheck)
    {
      if (parent == (ViewItem) null || childToCheck == (ViewItem) null || !childToCheck.IsDescendantOf(parent))
        return false;
      foreach (ViewItem ancestor in parent.LogicalChildren)
      {
        if (ancestor == childToCheck || childToCheck.IsDescendantOf(ancestor))
          return false;
      }
      return true;
    }

    public List<DesignerItemAutomationPeer> GetSelectedAutomationPeers(object selectedObj)
    {
      if (this._dataChildren == null)
        this.GetChildren();
      List<DesignerItemAutomationPeer> list = new List<DesignerItemAutomationPeer>();
      if (this._dataChildren.ContainsKey(selectedObj))
      {
        DesignerItemAutomationPeer itemAutomationPeer = this._dataChildren[selectedObj] as DesignerItemAutomationPeer;
        if (itemAutomationPeer != null)
          list.Add(itemAutomationPeer);
      }
      else
      {
        foreach (DictionaryEntry dictionaryEntry in this._dataChildren)
        {
          DesignerItemAutomationPeer itemAutomationPeer = dictionaryEntry.Value as DesignerItemAutomationPeer;
          if (itemAutomationPeer != null)
            list.AddRange((IEnumerable<DesignerItemAutomationPeer>) itemAutomationPeer.GetSelectedAutomationPeers(selectedObj));
        }
      }
      return list;
    }

    public void AddToSelection()
    {
      SelectionOperations.Union(this._view.Context, this._item);
      this.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementAddedToSelection);
      this.RaiseAutomationEvent(AutomationEvents.AutomationFocusChanged);
    }

    public void RemoveFromSelection()
    {
      if (!this.IsSelected)
        return;
      SelectionOperations.Toggle(this._view.Context, this._item);
      this.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection);
      this.RaiseAutomationEvent(AutomationEvents.AutomationFocusChanged);
    }

    public void Select()
    {
      SelectionOperations.SelectOnly(this._view.Context, this._item);
      this.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementSelected);
      this.RaiseAutomationEvent(AutomationEvents.AutomationFocusChanged);
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
      if (disposing && this._dataChildren != null)
      {
        foreach (DictionaryEntry dictionaryEntry in this._dataChildren)
        {
          DesignerItemAutomationPeer itemAutomationPeer = dictionaryEntry.Value as DesignerItemAutomationPeer;
          if (itemAutomationPeer != null)
            itemAutomationPeer.Dispose();
        }
      }
      this._item = (ModelItem) null;
      this._view = (DesignerView) null;
      this._dataChildren = (Hashtable) null;
      this._viewPeer = (DesignerViewAutomationPeer) null;
      this._modelService = (ModelService) null;
      this._disposed = true;
    }
  }
}
