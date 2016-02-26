// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.WindowProfileValidator
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.PlatformUI.Shell
{
  internal class WindowProfileValidator
  {
    public void Validate(WindowProfile profile)
    {
      List<MainSite> mainSites = new List<MainSite>();
      for (int index = 0; index < profile.Children.Count; ++index)
      {
        ViewElement element = profile.Children[index];
        if (element is MainSite)
          mainSites.Add(element as MainSite);
        if (!(!(element is ViewGroup) ? this.ValidateElement(element) : this.ValidateGroup(element as ViewGroup)))
        {
          profile.Children.Remove(element);
          --index;
        }
      }
      if (mainSites.Count == 0)
      {
        MainSite mainSite = MainSite.Create();
        mainSite.Child = WindowProfile.CreateDefaultMainSiteContent();
        profile.Children.Add((ViewElement) mainSite);
      }
      else
      {
        if (mainSites.Count > 1)
          this.DeleteExtraMainSites(mainSites, profile);
        foreach (MainSite site in mainSites)
          this.PostValidation(site);
      }
    }

    private bool ValidateGroup(ViewGroup group)
    {
      bool flag = false;
      DockOperations.TryCollapse((ViewElement) group);
      if (group.Parent != null || group is ViewSite)
      {
        flag = this.ValidateElement((ViewElement) group);
        if (flag)
        {
          for (int index = 0; index < group.Children.Count; ++index)
          {
            ViewElement element = group.Children[index];
            if (element is AutoHideGroup)
              ((ViewGroup) element).SelectedElement = (ViewElement) null;
            if (!(!(element is ViewGroup) ? this.ValidateElement(element) : this.ValidateGroup(element as ViewGroup)))
            {
              group.Children.Remove(element);
              --index;
            }
          }
        }
      }
      return flag;
    }

    private bool ValidateElement(ViewElement element)
    {
      if (!this.IsFiniteSizeInValidRange(element.MinimumHeight))
        element.MinimumHeight = (double) ViewElement.MinimumHeightProperty.DefaultMetadata.DefaultValue;
      if (!this.IsFiniteSizeInValidRange(element.MinimumWidth))
        element.MinimumWidth = (double) ViewElement.MinimumWidthProperty.DefaultMetadata.DefaultValue;
      if (!this.IsFiniteSizeInValidRange(element.AutoHideHeight))
        element.AutoHideHeight = (double) ViewElement.AutoHideHeightProperty.DefaultMetadata.DefaultValue;
      if (!this.IsFiniteSizeInValidRange(element.AutoHideWidth))
        element.AutoHideWidth = (double) ViewElement.AutoHideWidthProperty.DefaultMetadata.DefaultValue;
      if (!this.IsFiniteSizeInValidRange(element.FloatingHeight))
        element.FloatingHeight = (double) ViewElement.FloatingHeightProperty.DefaultMetadata.DefaultValue;
      if (!this.IsFiniteSizeInValidRange(element.FloatingWidth))
        element.FloatingWidth = (double) ViewElement.FloatingWidthProperty.DefaultMetadata.DefaultValue;
      if (!this.IsValueInValidRange(element.FloatingLeft, double.NegativeInfinity, double.PositiveInfinity, false, false))
        element.FloatingLeft = (double) ViewElement.FloatingLeftProperty.DefaultMetadata.DefaultValue;
      if (!this.IsValueInValidRange(element.FloatingTop, double.NegativeInfinity, double.PositiveInfinity, false, false))
        element.FloatingTop = (double) ViewElement.FloatingTopProperty.DefaultMetadata.DefaultValue;
      MainSite site1 = element as MainSite;
      if (site1 != null)
        return this.ValidateElement(site1);
      FloatSite site2 = element as FloatSite;
      if (site2 != null)
        return this.ValidateElement(site2);
      DocumentGroupContainer container = element as DocumentGroupContainer;
      if (container != null)
        return this.ValidateElement(container);
      return true;
    }

    private bool ValidateElement(MainSite site)
    {
      bool flag = true;
      if (site.Parent != null)
        flag = false;
      return flag;
    }

    private bool ValidateElement(FloatSite site)
    {
      bool flag = true;
      if (site.Children.Count == 0)
        flag = false;
      if (site.Parent != null)
        flag = false;
      return flag;
    }

    private bool ValidateElement(DocumentGroupContainer container)
    {
      bool flag = true;
      if (container.Children.Count == 0)
        container.Children.Add((ViewElement) DocumentGroup.Create());
      return flag;
    }

    private bool IsFiniteSizeInValidRange(double size)
    {
      return this.IsValueInValidRange(size, 1.0, double.PositiveInfinity, false, false);
    }

    private bool IsInfiniteSizeInValidRange(double size)
    {
      return this.IsValueInValidRange(size, 1.0, double.PositiveInfinity, false, true);
    }

    private bool IsValueInValidRange(double value, double minValue, double maxValue, bool allowedNaN, bool allowedInfinity)
    {
      return (allowedNaN || !double.IsNaN(value)) && (allowedInfinity || !double.IsInfinity(value)) && (value >= minValue && value <= maxValue);
    }

    private void PostValidation(MainSite site)
    {
      if (site.Find((Predicate<ViewElement>) (v =>
      {
        if (!(v is DocumentGroup))
          return v is NakedView;
        return true;
      })) != null)
        return;
      site.Child = WindowProfile.CreateDefaultMainSiteContent();
    }

    private void DeleteExtraMainSites(List<MainSite> mainSites, WindowProfile profile)
    {
      MainSite mainSite1 = (MainSite) null;
      int num = 0;
      foreach (MainSite mainSite2 in mainSites)
      {
        List<ViewElement> list = new List<ViewElement>(mainSite2.FindAll((Predicate<ViewElement>) (v => v != null)));
        if (mainSite1 == null || list.Count > num)
        {
          mainSite1 = mainSite2;
          num = list.Count;
        }
      }
      mainSites.Remove(mainSite1);
      foreach (MainSite mainSite2 in mainSites)
        profile.Children.Remove((ViewElement) mainSite2);
      mainSites.Clear();
      mainSites.Add(mainSite1);
    }
  }
}
