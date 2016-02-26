// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Workspaces.Extension.WindowProfileValidator
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI.Shell;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Workspaces.Extension
{
  public class WindowProfileValidator
  {
    public virtual bool Validate(WindowProfile profile)
    {
      WindowProfileValidationContext validationContext = this.CreateValidationContext();
      if (!this.ValidateOrReplaceViewElementCollection(profile.Children, validationContext))
        return false;
      this.PostValidation(profile, validationContext);
      return true;
    }

    protected virtual WindowProfileValidationContext CreateValidationContext()
    {
      return new WindowProfileValidationContext();
    }

    protected virtual bool ValidateOrReplaceViewElementCollection(IObservableCollection<ViewElement> children, WindowProfileValidationContext context)
    {
      for (int index = 0; index < children.Count; ++index)
      {
        ViewElement element = children[index];
        using (element.PreventCollapse())
        {
          bool flag = this.ValidateOrReplaceViewElement(ref element, context);
          if (element != children[index])
            children[index] = element;
          else if (!flag)
          {
            children.Remove(element);
            --index;
          }
        }
      }
      return true;
    }

    protected bool ValidateOrReplaceViewElement(ref ViewElement element, WindowProfileValidationContext context)
    {
      this.ValidateOrReplaceViewElementSizes(element);
      this.ValidateOrReplaceViewElementPosition(element);
      View view = element as View;
      if (view != null)
      {
        bool flag = this.ValidateOrReplaceView(ref view, context);
        element = (ViewElement) view;
        return flag;
      }
      ViewGroup viewGroup = element as ViewGroup;
      if (viewGroup != null)
      {
        bool flag = this.ValidateOrReplaceViewGroup(ref viewGroup, context);
        element = (ViewElement) viewGroup;
        return flag;
      }
      ViewBookmark viewBookmark = element as ViewBookmark;
      if (viewBookmark == null)
        return this.ValidateOrReplaceCustomViewElement(ref element, context);
      bool flag1 = this.ValidateOrReplaceViewBookmark(ref viewBookmark, context);
      element = (ViewElement) viewBookmark;
      return flag1;
    }

    protected void ValidateOrReplaceViewElementSizes(ViewElement element)
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
      if (this.IsFiniteSizeInValidRange(element.FloatingWidth))
        return;
      element.FloatingWidth = (double) ViewElement.FloatingWidthProperty.DefaultMetadata.DefaultValue;
    }

    protected void ValidateOrReplaceViewElementPosition(ViewElement element)
    {
      if (!this.IsValueInValidRange(element.FloatingLeft, double.NegativeInfinity, double.PositiveInfinity, false, false))
        element.FloatingLeft = (double) ViewElement.FloatingLeftProperty.DefaultMetadata.DefaultValue;
      if (this.IsValueInValidRange(element.FloatingTop, double.NegativeInfinity, double.PositiveInfinity, false, false))
        return;
      element.FloatingTop = (double) ViewElement.FloatingTopProperty.DefaultMetadata.DefaultValue;
    }

    protected virtual bool ValidateOrReplaceView(ref View view, WindowProfileValidationContext context)
    {
      return true;
    }

    protected bool ValidateOrReplaceViewGroup(ref ViewGroup viewGroup, WindowProfileValidationContext context)
    {
      DockOperations.TryCollapse((ViewElement) viewGroup);
      DockGroup dockGroup = viewGroup as DockGroup;
      if (dockGroup != null)
      {
        bool flag = this.ValidateOrReplaceDockGroup(ref dockGroup, context);
        viewGroup = (ViewGroup) dockGroup;
        this.ValidateOrReplaceViewElementCollection(viewGroup.Children, context);
        return flag;
      }
      NestedGroup nestedGroup = viewGroup as NestedGroup;
      if (nestedGroup != null)
      {
        bool flag = this.ValidateOrReplaceNestedGroup(ref nestedGroup, context);
        viewGroup = (ViewGroup) nestedGroup;
        this.ValidateOrReplaceViewElementCollection(viewGroup.Children, context);
        return flag;
      }
      AutoHideGroup autoHideGroup = viewGroup as AutoHideGroup;
      if (autoHideGroup != null)
      {
        autoHideGroup.SelectedElement = (ViewElement) null;
        bool flag = this.ValidateOrReplaceAutoHideGroup(ref autoHideGroup, context);
        viewGroup = (ViewGroup) autoHideGroup;
        this.ValidateOrReplaceViewElementCollection(viewGroup.Children, context);
        return flag;
      }
      AutoHideChannel autoHideChannel = viewGroup as AutoHideChannel;
      if (autoHideChannel != null)
      {
        bool flag = this.ValidateOrReplaceAutoHideChannel(ref autoHideChannel, context);
        viewGroup = (ViewGroup) autoHideChannel;
        this.ValidateOrReplaceViewElementCollection(viewGroup.Children, context);
        return flag;
      }
      ViewSite viewSite = viewGroup as ViewSite;
      if (viewSite != null)
      {
        bool flag = this.ValidateOrReplaceViewSite(ref viewSite, context);
        viewGroup = (ViewGroup) viewSite;
        this.ValidateOrReplaceViewElementCollection(viewGroup.Children, context);
        return flag;
      }
      AutoHideRoot autoHideRoot = viewGroup as AutoHideRoot;
      if (autoHideRoot != null)
      {
        bool flag = this.ValidateOrReplaceAutoHideRoot(ref autoHideRoot, context);
        viewGroup = (ViewGroup) autoHideRoot;
        this.ValidateOrReplaceViewElementCollection(viewGroup.Children, context);
        return flag;
      }
      DockRoot dockRoot = viewGroup as DockRoot;
      if (dockRoot == null)
        return this.ValidateOrReplaceCustomViewGroup(ref viewGroup, context);
      bool flag1 = this.ValidateOrReplaceDockRoot(ref dockRoot, context);
      viewGroup = (ViewGroup) dockRoot;
      this.ValidateOrReplaceViewElementCollection(viewGroup.Children, context);
      return flag1;
    }

    protected virtual bool ValidateOrReplaceDockGroup(ref DockGroup dockGroup, WindowProfileValidationContext context)
    {
      DocumentGroupContainer documentGroupContainer = dockGroup as DocumentGroupContainer;
      if (documentGroupContainer == null)
        return true;
      if (documentGroupContainer.Children.Count == 0)
        documentGroupContainer.Children.Add((ViewElement) DocumentGroup.Create());
      bool flag = this.ValidateOrReplaceDocumentGroupContainer(ref documentGroupContainer, context);
      dockGroup = (DockGroup) documentGroupContainer;
      return flag;
    }

    protected virtual bool ValidateOrReplaceDocumentGroupContainer(ref DocumentGroupContainer documentGroupContainer, WindowProfileValidationContext context)
    {
      return true;
    }

    protected bool ValidateOrReplaceNestedGroup(ref NestedGroup nestedGroup, WindowProfileValidationContext context)
    {
      TabGroup tabGroup = nestedGroup as TabGroup;
      if (tabGroup != null)
      {
        bool flag = this.ValidateOrReplaceTabGroup(ref tabGroup, context);
        nestedGroup = (NestedGroup) tabGroup;
        return flag;
      }
      DocumentGroup documentGroup = nestedGroup as DocumentGroup;
      if (documentGroup == null)
        return this.ValidateOrReplaceCustomNestedGroup(ref nestedGroup, context);
      bool flag1 = this.ValidateOrReplaceDocumentGroup(ref documentGroup, context);
      nestedGroup = (NestedGroup) documentGroup;
      return flag1;
    }

    protected virtual bool ValidateOrReplaceTabGroup(ref TabGroup tabGroup, WindowProfileValidationContext context)
    {
      return true;
    }

    protected virtual bool ValidateOrReplaceDocumentGroup(ref DocumentGroup documentGroup, WindowProfileValidationContext context)
    {
      return true;
    }

    protected virtual bool ValidateOrReplaceCustomNestedGroup(ref NestedGroup nestedGroup, WindowProfileValidationContext context)
    {
      return false;
    }

    protected virtual bool ValidateOrReplaceAutoHideGroup(ref AutoHideGroup autoHideGroup, WindowProfileValidationContext context)
    {
      return true;
    }

    protected virtual bool ValidateOrReplaceAutoHideChannel(ref AutoHideChannel autoHideChannel, WindowProfileValidationContext context)
    {
      return true;
    }

    protected bool ValidateOrReplaceViewSite(ref ViewSite viewSite, WindowProfileValidationContext context)
    {
      if (viewSite.Parent != null)
        return false;
      FloatSite floatSite = viewSite as FloatSite;
      if (floatSite != null)
      {
        if (floatSite.Children.Count == 0)
          return false;
        bool flag = this.ValidateOrReplaceFloatSite(ref floatSite, context);
        viewSite = (ViewSite) floatSite;
        return flag;
      }
      MainSite mainSite = viewSite as MainSite;
      if (mainSite == null)
        return this.ValidateOrReplaceCustomViewSite(ref viewSite, context);
      context.MainSites.Add(mainSite);
      bool flag1 = this.ValidateOrReplaceMainSite(ref mainSite, context);
      viewSite = (ViewSite) mainSite;
      return flag1;
    }

    protected virtual bool ValidateOrReplaceFloatSite(ref FloatSite floatSite, WindowProfileValidationContext context)
    {
      return true;
    }

    protected virtual bool ValidateOrReplaceMainSite(ref MainSite mainSite, WindowProfileValidationContext context)
    {
      return true;
    }

    protected virtual bool ValidateOrReplaceCustomViewSite(ref ViewSite viewSite, WindowProfileValidationContext context)
    {
      return false;
    }

    protected virtual bool ValidateOrReplaceAutoHideRoot(ref AutoHideRoot autoHideRoot, WindowProfileValidationContext context)
    {
      return true;
    }

    protected virtual bool ValidateOrReplaceDockRoot(ref DockRoot dockRoot, WindowProfileValidationContext context)
    {
      return true;
    }

    protected virtual bool ValidateOrReplaceCustomViewGroup(ref ViewGroup viewGroup, WindowProfileValidationContext context)
    {
      return false;
    }

    protected virtual bool ValidateOrReplaceViewBookmark(ref ViewBookmark viewBookmark, WindowProfileValidationContext context)
    {
      return true;
    }

    protected virtual bool ValidateOrReplaceCustomViewElement(ref ViewElement element, WindowProfileValidationContext context)
    {
      return false;
    }

    protected bool IsFiniteSizeInValidRange(double size)
    {
      return this.IsValueInValidRange(size, 1.0, double.PositiveInfinity, false, false);
    }

    protected bool IsInfiniteSizeInValidRange(double size)
    {
      return this.IsValueInValidRange(size, 1.0, double.PositiveInfinity, false, true);
    }

    protected bool IsValueInValidRange(double value, double minValue, double maxValue, bool allowedNaN, bool allowedInfinity)
    {
      return (allowedNaN || !double.IsNaN(value)) && (allowedInfinity || !double.IsInfinity(value)) && (value >= minValue && value <= maxValue);
    }

    protected virtual void PostValidation(WindowProfile profile, WindowProfileValidationContext context)
    {
      if (context.MainSites.Count == 0)
      {
        MainSite mainSite = MainSite.Create();
        mainSite.Child = WindowProfile.CreateDefaultMainSiteContent();
        profile.Children.Add((ViewElement) mainSite);
      }
      else
      {
        if (context.MainSites.Count > 1)
          this.DeleteExtraMainSites(context.MainSites, profile);
        foreach (MainSite site in context.MainSites)
          this.PostValidateMainSite(site);
      }
    }

    protected virtual void PostValidateMainSite(MainSite site)
    {
      if (site.Find((Predicate<ViewElement>) (v => v is DocumentGroup)) != null)
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
