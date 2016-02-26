// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.AdornerProvider
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Model;
using MS.Internal.Features;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Microsoft.Windows.Design.Interaction
{
  [FeatureConnector(typeof (AdornerProviderFeatureConnector))]
  public abstract class AdornerProvider : FeatureProvider
  {
    private bool _adornersVisible = true;
    private Collection<UIElement> _adorners;
    private EditingContext _context;
    private bool _firstActivate;
    private bool _canClearAdorners;

    public Collection<UIElement> Adorners
    {
      get
      {
        if (this._adorners == null)
          this._adorners = new Collection<UIElement>();
        return this._adorners;
      }
    }

    public bool AdornersVisible
    {
      get
      {
        return this._adornersVisible;
      }
      set
      {
        if (this._adornersVisible == value)
          return;
        this._adornersVisible = value;
        if (this._context == null)
          return;
        DesignerView designerView = DesignerView.FromContext(this._context);
        if (designerView == null)
          return;
        ICollection<UIElement> adorners = designerView.Adorners;
        if (this._adornersVisible)
        {
          foreach (UIElement uiElement in this.Adorners)
          {
            if (LogicalTreeHelper.GetParent((DependencyObject) uiElement) == null)
              adorners.Add(uiElement);
          }
        }
        else
        {
          foreach (UIElement uiElement in this.Adorners)
          {
            if (LogicalTreeHelper.GetParent((DependencyObject) uiElement) != null)
              adorners.Remove(uiElement);
          }
        }
      }
    }

    protected EditingContext Context
    {
      get
      {
        return this._context;
      }
    }

    public virtual bool IsToolSupported(Tool tool)
    {
      return tool is SelectionTool;
    }

    protected virtual void Activate(ModelItem item)
    {
    }

    protected virtual void Deactivate()
    {
    }

    internal void InvokeActivate(EditingContext context, ModelItem item)
    {
      this._context = context;
      if (this._firstActivate && this._canClearAdorners && this.Adorners.Count > 0)
        this.Adorners.Clear();
      if (!this._firstActivate)
      {
        this._firstActivate = true;
        this._canClearAdorners = this.Adorners.Count == 0;
      }
      this.Activate(item);
      foreach (UIElement uiElement in this.Adorners)
      {
        if (uiElement.ReadLocalValue(AdornerProperties.ModelProperty) == DependencyProperty.UnsetValue)
          AdornerProperties.SetModel((DependencyObject) uiElement, item);
      }
    }

    internal void InvokeDeactivate()
    {
      if (this._context == null)
        return;
      this.Deactivate();
      this._context = (EditingContext) null;
    }
  }
}
