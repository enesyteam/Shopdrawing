// Decompiled with JetBrains decompiler
// Type: MS.Internal.Features.AdornerProviderFeatureConnector
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Services;
using System.Collections.Generic;
using System.Windows;

namespace MS.Internal.Features
{
  [RequiresContextItem(typeof (CurrentDesignerView))]
  [RequiresService(typeof (ViewService))]
  internal class AdornerProviderFeatureConnector : PolicyDrivenToolFeatureConnector<AdornerProvider>
  {
    private static DependencyProperty OwningProviderProperty = DependencyProperty.RegisterAttached("OwningProvider", typeof (AdornerProvider), typeof (AdornerProviderFeatureConnector));
    private ICollection<UIElement> _adorners;
    private ViewService _viewService;

    private ICollection<UIElement> Adorners
    {
      get
      {
        if (this._adorners == null)
          this._adorners = this.Context.Items.GetValue<CurrentDesignerView>().View.Adorners;
        return this._adorners;
      }
    }

    private ViewService ViewService
    {
      get
      {
        if (this._viewService == null)
          this._viewService = this.Context.Services.GetRequiredService<ViewService>();
        return this._viewService;
      }
    }

    public AdornerProviderFeatureConnector(FeatureManager manager)
      : base(manager)
    {
    }

    protected override bool IsValidProvider(FeatureProvider featureProvider)
    {
      AdornerProvider adornerProvider = featureProvider as AdornerProvider;
      if (adornerProvider != null && this.CurrentTool != null)
        return adornerProvider.IsToolSupported(this.CurrentTool);
      return false;
    }

    private bool IsItemDesignable(ModelItem item)
    {
      for (; item != null && item != item.Root; item = item.Parent)
      {
        ModelProperty source = item.Source;
        if (source != (ModelProperty) null && source.Parent is ModelItemDictionary)
          return false;
      }
      return true;
    }

    protected override void FeatureProvidersAdded(ModelItem item, IEnumerable<AdornerProvider> extensions)
    {
      if (!(item.View != (ViewItem) null) || !this.IsItemDesignable(item))
        return;
      ICollection<UIElement> adorners = this.Adorners;
      foreach (AdornerProvider adornerProvider in extensions)
      {
        if (new RequirementValidator(this.Manager, adornerProvider.GetType()).MeetsRequirements)
        {
          adornerProvider.InvokeActivate(this.Context, item);
          foreach (UIElement uiElement in adornerProvider.Adorners)
          {
            uiElement.SetValue(AdornerProviderFeatureConnector.OwningProviderProperty, (object) adornerProvider);
            if (adornerProvider.AdornersVisible && LogicalTreeHelper.GetParent((DependencyObject) uiElement) == null)
              adorners.Add(uiElement);
          }
        }
      }
    }

    protected override void FeatureProvidersRemoved(ModelItem item, IEnumerable<AdornerProvider> extensions)
    {
      ICollection<UIElement> adorners = this.Adorners;
      foreach (AdornerProvider adornerProvider in extensions)
      {
        foreach (UIElement uiElement in adornerProvider.Adorners)
        {
          if (LogicalTreeHelper.GetParent((DependencyObject) uiElement) != null && uiElement.GetValue(AdornerProviderFeatureConnector.OwningProviderProperty) == adornerProvider)
            adorners.Remove(uiElement);
        }
        adornerProvider.InvokeDeactivate();
      }
    }
  }
}
