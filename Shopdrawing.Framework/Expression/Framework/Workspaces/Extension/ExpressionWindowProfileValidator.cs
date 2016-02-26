// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Workspaces.Extension.ExpressionWindowProfileValidator
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI.Shell;
using System;

namespace Microsoft.Expression.Framework.Workspaces.Extension
{
  public class ExpressionWindowProfileValidator : WindowProfileValidator
  {
    protected override bool ValidateOrReplaceView(ref View view, WindowProfileValidationContext context)
    {
      if (!(view is ExpressionView))
      {
        View view1 = ViewElementFactory.Current.CreateView(typeof (ExpressionView));
        if (view1 == null || !(view1 is ExpressionView))
          return false;
        this.CopyViewProperties(view, view1);
        view = view1;
      }
      return true;
    }

    protected override bool ValidateOrReplaceDockGroup(ref DockGroup dockGroup, WindowProfileValidationContext context)
    {
      if (!(dockGroup is ExpressionDockGroup) && typeof (DockGroup) == dockGroup.GetType())
      {
        DockGroup dockGroup1 = ViewElementFactory.Current.CreateDockGroup();
        if (dockGroup1 == null || !(dockGroup1 is ExpressionDockGroup))
          return false;
        this.CopyDockGroupProperties(dockGroup, dockGroup1);
        dockGroup = dockGroup1;
      }
      return true;
    }

    protected override bool ValidateOrReplaceDockRoot(ref DockRoot dockRoot, WindowProfileValidationContext context)
    {
      if (!(dockRoot is ExpressionDockRoot))
      {
        DockRoot dockRoot1 = ViewElementFactory.Current.CreateDockRoot();
        if (dockRoot1 == null || !(dockRoot1 is ExpressionDockRoot))
          return false;
        this.CopyDockRootProperties(dockRoot, dockRoot1);
        dockRoot = dockRoot1;
      }
      return true;
    }

    protected override bool ValidateOrReplaceDocumentGroup(ref DocumentGroup documentGroup, WindowProfileValidationContext context)
    {
      if (!(documentGroup is ExpressionDocumentGroup))
      {
        DocumentGroup documentGroup1 = ViewElementFactory.Current.CreateDocumentGroup();
        if (documentGroup1 == null || !(documentGroup1 is ExpressionDocumentGroup))
          return false;
        this.CopyDocumentGroupProperties(documentGroup, documentGroup1);
        documentGroup = documentGroup1;
      }
      return true;
    }

    protected override bool ValidateOrReplaceCustomViewElement(ref ViewElement element, WindowProfileValidationContext context)
    {
      if (element is NakedView)
        return true;
      return base.ValidateOrReplaceCustomViewElement(ref element, context);
    }

    private void CopyViewElementProperties(ViewElement from, ViewElement to)
    {
      to.IsVisible = from.IsVisible;
      to.IsSelected = from.IsSelected;
      to.DockedWidth = from.DockedWidth;
      to.DockedHeight = from.DockedHeight;
      to.AutoHideWidth = from.AutoHideWidth;
      to.AutoHideHeight = from.AutoHideHeight;
      to.FloatingWidth = from.FloatingWidth;
      to.FloatingHeight = from.FloatingHeight;
      to.FloatingLeft = from.FloatingLeft;
      to.FloatingTop = from.FloatingTop;
      to.DockRestriction = from.DockRestriction;
      to.AreDockTargetsEnabled = from.AreDockTargetsEnabled;
      to.MinimumWidth = from.MinimumWidth;
      to.MinimumHeight = from.MinimumHeight;
      to.FloatingWindowState = from.FloatingWindowState;
    }

    private void CopyViewProperties(View from, View to)
    {
      this.CopyViewElementProperties((ViewElement) from, (ViewElement) to);
      to.Name = from.Name;
    }

    private void CopyViewGroupProperties(ViewGroup from, ViewGroup to)
    {
      this.CopyViewElementProperties((ViewElement) from, (ViewElement) to);
      to.Children.Clear();
      while (from.Children.Count > 0)
      {
        ViewElement viewElement = from.Children[0];
        to.Children.Add(viewElement);
      }
      to.SelectedElement = from.SelectedElement;
    }

    private void CopyDockRootProperties(DockRoot from, DockRoot to)
    {
      this.CopyViewGroupProperties((ViewGroup) from, (ViewGroup) to);
    }

    private void CopyDockGroupProperties(DockGroup from, DockGroup to)
    {
      this.CopyViewGroupProperties((ViewGroup) from, (ViewGroup) to);
      to.Orientation = from.Orientation;
    }

    private void CopyNestedGroupProperties(NestedGroup from, NestedGroup to)
    {
      this.CopyViewGroupProperties((ViewGroup) from, (ViewGroup) to);
    }

    private void CopyDocumentGroupProperties(DocumentGroup from, DocumentGroup to)
    {
      this.CopyNestedGroupProperties((NestedGroup) from, (NestedGroup) to);
    }

    protected override void PostValidateMainSite(MainSite site)
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
  }
}
