// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.ViewBridge
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Workspaces.Extension;
using Microsoft.VisualStudio.PlatformUI.Shell;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Expression.Framework.UserInterface
{
  internal sealed class ViewBridge
  {
    private IWorkspaceService workspaceService;
    private IViewService viewService;
    private IMessageDisplayService messageDisplayService;
    private DocumentGroup documentGroup;
    private List<ViewBridge.LinkedView> viewShelter;
    private ViewBridge.LinkedView viewShelterActive;
    private bool silenced;

    internal IViewCollection OrderedViews
    {
      get
      {
        ViewBridge.ViewCollection viewCollection = new ViewBridge.ViewCollection();
        foreach (ViewBridge.LinkedView linkedView in (IEnumerable<ViewElement>) this.documentGroup.Children)
          viewCollection.Add(linkedView.ViewReference);
        return (IViewCollection) viewCollection;
      }
    }

    private WindowProfile ActiveWindowProfile
    {
      get
      {
        if (this.workspaceService.ActiveWorkspace == null)
          return (WindowProfile) null;
        return ((Workspace) this.workspaceService.ActiveWorkspace).WindowProfile;
      }
    }

    public ViewBridge(IWorkspaceService workspaceService, IViewService viewService, IMessageDisplayService messageDisplayService)
    {
      this.workspaceService = workspaceService;
      this.viewService = viewService;
      this.messageDisplayService = messageDisplayService;
      this.viewService.ViewOpened += new ViewEventHandler(this.ViewService_ViewOpened);
      this.viewService.ViewClosed += new ViewEventHandler(this.ViewService_ViewClosed);
      this.viewService.ActiveViewChanged += new ViewChangedEventHandler(this.ViewService_ActiveViewChanged);
      this.workspaceService.ActiveWorkspaceChanging += new CancelEventHandler(this.WorkspaceService_ActiveWorkspaceChanging);
      this.workspaceService.ActiveWorkspaceChangingCanceled += new EventHandler(this.WorkspaceService_ActiveWorkspaceChangingCanceled);
      this.workspaceService.ActiveWorkspaceChanged += new EventHandler(this.WorkspaceService_ActiveWorkspaceChanged);
      DockOperations.DockPositionChanging += new EventHandler<DockEventArgs>(this.DockOperations_DockPositionChanging);
      DockOperations.DockPositionChanged += new EventHandler<DockEventArgs>(this.DockOperations_DockPositionChanged);
    }

    private void UpdateDocumentGroup()
    {
      this.documentGroup = (DocumentGroup) this.ActiveWindowProfile.Find((Predicate<ViewElement>) (i => i is DocumentGroup));
    }

    private void ViewService_ViewOpened(object sender, ViewEventArgs e)
    {
      ViewBridge.LinkedView linkedView = (ViewBridge.LinkedView) Microsoft.VisualStudio.PlatformUI.Shell.View.Create(this.ActiveWindowProfile, "Document", typeof (ViewBridge.LinkedView));
      linkedView.ViewReference = e.View;
      linkedView.Initialize(e.View);
      linkedView.Title = (object) e.View.Caption;
      linkedView.Content = (object) ((IElementProvider) e.View).Element;
      linkedView.Hiding += new CancelEventHandler(this.View_Hiding);
      linkedView.IsSelectedChanged += new EventHandler(this.View_IsSelectedChanged);
      DockOperations.DockAt((ViewElement) this.documentGroup, (ViewElement) linkedView, -1);
      linkedView.Show();
    }

    private void ViewService_ViewClosed(object sender, ViewEventArgs e)
    {
      ViewBridge.LinkedView linkedView = this.FindLinkedView(e.View);
      linkedView.Hiding -= new CancelEventHandler(this.View_Hiding);
      linkedView.IsSelectedChanged -= new EventHandler(this.View_IsSelectedChanged);
      linkedView.Content = (object) null;
      this.documentGroup.Children.Remove((ViewElement) linkedView);
      linkedView.Dispose();
    }

    private void View_Hiding(object sender, CancelEventArgs e)
    {
      if (this.silenced)
        return;
      ViewBridge.LinkedView linkedView = (ViewBridge.LinkedView) sender;
      IDocumentView documentView = linkedView.ViewReference as IDocumentView;
      if (documentView != null && this.GetViewCount(documentView.Document) <= 1 && !this.viewService.PromptToSaveAssociatedDocument(documentView.Document, this.messageDisplayService))
        e.Cancel = true;
      if (e.Cancel)
        return;
      this.viewService.CloseView(linkedView.ViewReference);
    }

    private void ViewService_ActiveViewChanged(object sender, ViewChangedEventArgs e)
    {
      if (e.NewView == null)
        return;
      ViewBridge.LinkedView linkedView = this.FindLinkedView(e.NewView);
      if (linkedView.IsSelected)
        return;
      linkedView.ShowInFront();
    }

    private void View_IsSelectedChanged(object sender, EventArgs e)
    {
      if (this.silenced)
        return;
      ViewBridge.LinkedView linkedView = (ViewBridge.LinkedView) sender;
      if (!linkedView.IsSelected)
        return;
      this.viewService.ActiveView = linkedView.ViewReference;
    }

    private int GetViewCount(IDocument document)
    {
      int num = 0;
      foreach (IView view in (IEnumerable<IView>) this.viewService.Views)
      {
        IDocumentView documentView = view as IDocumentView;
        if (documentView != null && documentView.Document == document)
          ++num;
      }
      return num;
    }

    private void WorkspaceService_ActiveWorkspaceChanging(object sender, CancelEventArgs e)
    {
      if (this.documentGroup == null)
        return;
      this.silenced = true;
      this.viewShelter = new List<ViewBridge.LinkedView>(this.documentGroup.Children.Count);
      foreach (ViewBridge.LinkedView linkedView in (IEnumerable<ViewElement>) this.documentGroup.Children)
        this.viewShelter.Add(linkedView);
      this.viewShelterActive = (ViewBridge.LinkedView) this.documentGroup.SelectedElement;
    }

    private void WorkspaceService_ActiveWorkspaceChangingCanceled(object sender, EventArgs e)
    {
      this.viewShelter = (List<ViewBridge.LinkedView>) null;
      this.viewShelterActive = (ViewBridge.LinkedView) null;
      this.silenced = false;
    }

    private void WorkspaceService_ActiveWorkspaceChanged(object sender, EventArgs e)
    {
      this.UpdateDocumentGroup();
      if (this.viewShelter != null)
      {
        foreach (ViewElement viewElement in this.viewShelter)
          this.documentGroup.Children.Add(viewElement);
        this.documentGroup.SelectedElement = (ViewElement) this.viewShelterActive;
      }
      this.viewShelter = (List<ViewBridge.LinkedView>) null;
      this.viewShelterActive = (ViewBridge.LinkedView) null;
      this.silenced = false;
    }

    private void DockOperations_DockPositionChanged(object sender, DockEventArgs e)
    {
      this.silenced = false;
    }

    private void DockOperations_DockPositionChanging(object sender, DockEventArgs e)
    {
      this.silenced = true;
    }

    private ViewBridge.LinkedView FindLinkedView(IView view)
    {
      return (ViewBridge.LinkedView) this.documentGroup.Find((Predicate<ViewElement>) (viewElement =>
      {
        ViewBridge.LinkedView linkedView = viewElement as ViewBridge.LinkedView;
        if (linkedView != null)
          return linkedView.ViewReference == view;
        return false;
      }));
    }

    private sealed class ViewCollection : List<IView>, IViewCollection, IReadOnlyList<IView>, IReadOnlyCollection<IView>, IEnumerable<IView>, ICollection, IEnumerable
    {
    }

    [NonXamlSerialized]
    private sealed class LinkedView : ExpressionView, IDisposable
    {
      private int maxTabTitleLength = 24;

      public IView ViewReference { get; set; }

      public string TabTitle
      {
        get
        {
          IDocumentView documentView = this.ViewReference as IDocumentView;
          string str;
          if (documentView != null && !string.IsNullOrEmpty(documentView.Document.DocumentReference.EditableDisplayName))
          {
            str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.CustomTabDisplayNameFormat, new object[2]
            {
              (object) documentView.Document.DocumentReference.EditableDisplayName,
              (object) this.ViewReference.Caption
            });
            if (str.Length > this.maxTabTitleLength)
              str = documentView.Document.DocumentReference.EditableDisplayName;
            if (str.Length > this.maxTabTitleLength)
              str = str.Substring(0, this.maxTabTitleLength) + "...";
          }
          else
          {
            str = this.ViewReference.Caption;
            if (str.Length > this.maxTabTitleLength)
              str = str.Substring(0, this.maxTabTitleLength / 2) + "..." + str.Substring(str.Length - this.maxTabTitleLength / 2, this.maxTabTitleLength / 2);
          }
          if (this.ViewReference.IsDirty)
            str += "*";
          return str;
        }
      }

      public string TabToolTip
      {
        get
        {
          return this.ViewReference.TabToolTip;
        }
      }

      public string AutomationIdentifier
      {
        get
        {
          IDocumentView documentView = this.ViewReference as IDocumentView;
          if (documentView != null)
            return "Tab:" + documentView.Document.DocumentReference.Path;
          return string.Empty;
        }
      }

      public void Initialize(IView viewReference)
      {
        this.ViewReference = viewReference;
        viewReference.PropertyChanged += new PropertyChangedEventHandler(this.ViewReference_PropertyChanged);
        IDocumentView documentView = this.ViewReference as IDocumentView;
        if (documentView == null)
          return;
        documentView.Document.IsDirtyChanged += new EventHandler(this.Document_IsDirtyChanged);
        documentView.Document.Renamed += new EventHandler(this.Document_Renamed);
      }

      public void Dispose()
      {
        this.Dispose(true);
        GC.SuppressFinalize((object) this);
      }

      private void Dispose(bool disposing)
      {
        if (!disposing)
          return;
        this.ViewReference.PropertyChanged -= new PropertyChangedEventHandler(this.ViewReference_PropertyChanged);
        IDocumentView documentView = this.ViewReference as IDocumentView;
        if (documentView != null)
        {
          documentView.Document.IsDirtyChanged -= new EventHandler(this.Document_IsDirtyChanged);
          documentView.Document.Renamed -= new EventHandler(this.Document_Renamed);
        }
        this.ViewReference = (IView) null;
      }

      private void Document_Renamed(object sender, EventArgs e)
      {
        this.Title = (object) this.ViewReference.Caption;
        this.NotifyPropertyChanged("TabTitle");
        this.NotifyPropertyChanged("TabToolTip");
        this.NotifyPropertyChanged("AutomationIdentifier");
      }

      private void Document_IsDirtyChanged(object sender, EventArgs e)
      {
        this.NotifyPropertyChanged("TabTitle");
      }

      private void ViewReference_PropertyChanged(object sender, PropertyChangedEventArgs e)
      {
        if (!(e.PropertyName == "Caption"))
          return;
        this.NotifyPropertyChanged("TabTitle");
        this.NotifyPropertyChanged("AutomationIdentifier");
      }
    }
  }
}
