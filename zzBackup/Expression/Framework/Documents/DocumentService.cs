// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Documents.DocumentService
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.UserInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Expression.Framework.Documents
{
  public class DocumentService : CommandTarget, IDocumentService, IViewService
  {
    private DocumentService.ItemCollection items = new DocumentService.ItemCollection();
    private DocumentService.DocumentCollection documents = new DocumentService.DocumentCollection();
    private DocumentService.ViewCollection views = new DocumentService.ViewCollection();
    private ICommandService commandService;
    private DocumentService.Item activeItem;
    private IView activeView;

    public IDocument ActiveDocument
    {
      get
      {
        if (this.activeItem != null)
          return this.activeItem.Document;
        return (IDocument) null;
      }
      set
      {
        DocumentService.Item obj = (DocumentService.Item) null;
        if (value != null)
        {
          obj = this.items[value];
          if (obj == null)
            throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.DocumentNotIncludedInOpenDocumentList, new object[1]
            {
              (object) value.DocumentReference.Path
            }));
        }
        this.SetActiveItem(obj);
      }
    }

    public IDocumentCollection Documents
    {
      get
      {
        return (IDocumentCollection) this.documents;
      }
    }

    public IView ActiveView
    {
      get
      {
        return this.activeView;
      }
      set
      {
        this.SetActiveView(value);
      }
    }

    public IViewCollection Views
    {
      get
      {
        return (IViewCollection) this.views;
      }
    }

    public event DocumentChangedEventHandler ActiveDocumentChanging;

    public event DocumentChangedEventHandler ActiveDocumentChanged;

    public event DocumentEventHandler DocumentOpened;

    public event DocumentEventHandler DocumentClosed;

    public event ViewChangedEventHandler ActiveViewChanging;

    public event ViewChangedEventHandler ActiveViewChanged;

    public event ViewEventHandler ViewOpened;

    public event ViewEventHandler ViewClosed;

    public DocumentService(ICommandService commandService, IMessageDisplayService messageDisplayService)
    {
      this.commandService = commandService;
      this.AddCommand("Application_FileSave", (Microsoft.Expression.Framework.Commands.ICommand) new DocumentService.FileSaveCommand((IDocumentService) this, messageDisplayService));
      this.AddCommand("Application_FileClose", (Microsoft.Expression.Framework.Commands.ICommand) new DocumentService.FileCloseCommand((IDocumentService) this, messageDisplayService));
      this.AddCommand("Windows_Empty", (Microsoft.Expression.Framework.Commands.ICommand) new DocumentService.EmptyActivateViewCommand((IViewService) this));
      for (int index = 0; index < 10; ++index)
        this.AddCommand("Windows_View_" + index.ToString((IFormatProvider) CultureInfo.InvariantCulture), (Microsoft.Expression.Framework.Commands.ICommand) new DocumentService.ActivateViewCommand((IViewService) this, index));
      this.AddCommand("Windows_More", (Microsoft.Expression.Framework.Commands.ICommand) new DocumentService.ViewServiceDialogCommand((IViewService) this));
    }

    protected IReadOnlyList<IView> GetDocumentViews(IDocument document)
    {
      DocumentService.Item obj;
      try
      {
        obj = this.items[document];
      }
      catch (KeyNotFoundException ex)
      {
        return (IReadOnlyList<IView>) null;
      }
      return (IReadOnlyList<IView>) obj.Views;
    }

    public void OpenDocument(IDocument document)
    {
      if (this.documents.Contains(document))
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.DocumentAlreadyOpen, new object[1]
        {
          (object) document.DocumentReference.Path
        }));
      this.documents.Add(document);
      this.items.Add(new DocumentService.Item(document));
      this.OnDocumentOpened(new DocumentEventArgs(document));
    }

    public void CloseDocument(IDocument document)
    {
      if (document == null)
        throw new ArgumentNullException("document");
      DocumentService.Item obj;
      try
      {
        obj = this.items[document];
      }
      catch (KeyNotFoundException ex)
      {
        return;
      }
      if (obj == null)
        return;
      List<IView> list = new List<IView>();
      while (obj.Views.Count != 0)
      {
        IView view = obj.Views[0];
        obj.RemoveView(view);
        this.views.Remove(view);
        list.Add(view);
      }
      if (obj == this.activeItem)
      {
        IDocument index = (IDocument) null;
        foreach (IView view in (List<IView>) this.views)
        {
          IDocumentView documentView = view as IDocumentView;
          if (documentView != null)
          {
            index = documentView.Document;
            break;
          }
        }
        this.SetActiveItem(index != null ? this.items[index] : (DocumentService.Item) null);
      }
      foreach (IView view in list)
      {
        try
        {
          this.OnViewClosed(new ViewEventArgs(view));
        }
        finally
        {
          view.Dispose();
        }
      }
      ((Collection<DocumentService.Item>) this.items).Remove(obj);
      this.documents.Remove(document);
      try
      {
        this.OnDocumentClosed(new DocumentEventArgs(document));
      }
      finally
      {
        document.Dispose();
      }
    }

    public void OpenView(IView view)
    {
      if (this.views.Contains(view))
        throw new InvalidOperationException(ExceptionStringTable.ViewAlreadyOpen);
      this.views.Add(view);
      IDocumentView documentView = view as IDocumentView;
      if (documentView != null)
        this.items[documentView.Document].AddView((IView) documentView);
      view.Initialize();
      this.OnViewOpened(new ViewEventArgs(view));
    }

    public void CloseView(IView view)
    {
      if (view == null)
        throw new ArgumentNullException("view");
      if (view == this.ActiveView)
      {
        IView view1 = (IView) null;
        if (this.views.Count > 0)
        {
          if (view != this.views[0])
            view1 = this.views[0];
          else if (this.views.Count > 1)
            view1 = this.views[1];
        }
        this.SetActiveView(view1);
      }
      this.views.Remove(view);
      IDocument document = (IDocument) null;
      try
      {
        IDocumentView documentView = view as IDocumentView;
        DocumentService.Item obj1 = (DocumentService.Item) null;
        if (documentView != null)
        {
          DocumentService.Item obj2 = this.items[documentView.Document];
          obj2.RemoveView((IView) documentView);
          if (obj2.Views.Count == 0)
            obj1 = obj2;
        }
        bool flag = this.RecycleView(view);
        this.OnViewClosed(new ViewEventArgs(view));
        if (flag)
        {
          view = (IView) null;
        }
        else
        {
          if (obj1 == null)
            return;
          ((Collection<DocumentService.Item>) this.items).Remove(obj1);
          document = obj1.Document;
          this.documents.Remove(document);
          this.OnDocumentClosed(new DocumentEventArgs(document));
        }
      }
      finally
      {
        if (view != null)
          view.Dispose();
        if (document != null)
          document.Dispose();
      }
    }

    protected virtual bool RecycleView(IView view)
    {
      return false;
    }

    public bool PromptToSaveAssociatedDocument(IDocument document, IMessageDisplayService messageDisplayService)
    {
      return DocumentUtilities.PromptUserAndSaveDocument(document, true, messageDisplayService);
    }

    protected virtual void OnDocumentOpened(DocumentEventArgs e)
    {
      if (this.DocumentOpened == null)
        return;
      this.DocumentOpened((object) this, e);
    }

    protected virtual void OnDocumentClosed(DocumentEventArgs e)
    {
      if (this.DocumentClosed == null)
        return;
      this.DocumentClosed((object) this, e);
    }

    protected virtual void OnActiveDocumentChanging(DocumentChangedEventArgs e)
    {
      if (this.ActiveDocumentChanging == null)
        return;
      this.ActiveDocumentChanging((object) this, e);
    }

    protected virtual void OnActiveDocumentChanged(DocumentChangedEventArgs e)
    {
      if (this.ActiveDocumentChanged == null)
        return;
      this.ActiveDocumentChanged((object) this, e);
    }

    protected virtual void OnViewOpened(ViewEventArgs e)
    {
      if (this.ViewOpened == null)
        return;
      this.ViewOpened((object) this, e);
    }

    protected virtual void OnViewClosed(ViewEventArgs e)
    {
      if (this.ViewClosed == null)
        return;
      this.ViewClosed((object) this, e);
    }

    protected virtual void OnActiveViewChanging(ViewChangedEventArgs e)
    {
      if (this.ActiveViewChanging == null)
        return;
      this.ActiveViewChanging((object) this, e);
    }

    protected virtual void OnActiveViewChanged(ViewChangedEventArgs e)
    {
      if (this.ActiveViewChanged == null)
        return;
      this.ActiveViewChanged((object) this, e);
    }

    private void SetActiveItem(DocumentService.Item item)
    {
      PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.SwitchView);
      this.SetActiveDocumentAndView(item == null || item.Views.Count <= 0 ? (IView) null : item.Views[0], item);
    }

    private void SetActiveView(IView view)
    {
      PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.SwitchView);
      IDocumentView documentView = view as IDocumentView;
      DocumentService.Item obj = documentView != null ? this.items[documentView.Document] : (DocumentService.Item) null;
      this.SetActiveDocumentAndView(view, obj);
    }

    private void SetActiveDocumentAndView(IView view, DocumentService.Item item)
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.SetActiveDocumentAndView);
      ViewChangedEventArgs e1 = view != this.activeView ? new ViewChangedEventArgs(this.activeView, view) : (ViewChangedEventArgs) null;
      IDocument oldDocument = this.activeItem != null ? this.activeItem.Document : (IDocument) null;
      IDocument newDocument = item != null ? item.Document : (IDocument) null;
      DocumentChangedEventArgs e2 = item != this.activeItem ? new DocumentChangedEventArgs(oldDocument, newDocument) : (DocumentChangedEventArgs) null;
      if (e1 != null || e2 != null)
      {
        if (e1 != null)
        {
          PerformanceUtility.MarkInterimStep(PerformanceEvent.SetActiveDocumentAndView, "OnActiveViewChanging");
          if (e1.OldView != null)
            e1.OldView.Deactivating();
          this.OnActiveViewChanging(e1);
        }
        if (e2 != null)
        {
          PerformanceUtility.MarkInterimStep(PerformanceEvent.SetActiveDocumentAndView, "OnActiveDocumentChanging");
          this.OnActiveDocumentChanging(e2);
        }
        this.activeItem = item;
        this.activeView = view;
        if (this.activeView != null)
        {
          PerformanceUtility.MarkInterimStep(PerformanceEvent.SetActiveDocumentAndView, "RecordViewUse");
          this.RecordViewUse(this.activeView);
          this.activeView.ReturnFocus();
        }
        else
        {
          try
          {
            if (Application.Current != null)
            {
              if (Application.Current.MainWindow != null)
                FocusManager.SetFocusedElement((DependencyObject) Application.Current.MainWindow, (IInputElement) Application.Current.MainWindow);
            }
          }
          catch
          {
          }
        }
        if (e2 != null)
        {
          PerformanceUtility.MarkInterimStep(PerformanceEvent.SetActiveDocumentAndView, "OnActiveDocumentChanged");
          this.OnActiveDocumentChanged(e2);
        }
        if (e1 != null)
        {
          PerformanceUtility.MarkInterimStep(PerformanceEvent.SetActiveDocumentAndView, "OnActiveViewChanged");
          if (e1.OldView != null)
          {
            e1.OldView.Deactivated();
            ICommandTarget target = e1.OldView as ICommandTarget;
            if (target != null)
              this.commandService.RemoveTarget(target);
          }
          if (e1.NewView != null)
          {
            e1.NewView.Activated();
            ICommandTarget target = e1.NewView as ICommandTarget;
            if (target != null)
              this.commandService.AddTarget(target);
          }
          this.OnActiveViewChanged(e1);
        }
        if (e2 != null)
        {
          PerformanceUtility.MarkInterimStep(PerformanceEvent.SetActiveDocumentAndView, "UpdateTargets");
          ICommandTarget target1;
          if ((target1 = oldDocument as ICommandTarget) != null)
            this.commandService.RemoveTarget(target1);
          ICommandTarget target2;
          if ((target2 = newDocument as ICommandTarget) != null)
            this.commandService.AddTarget(target2);
        }
      }
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.SetActiveDocumentAndView);
    }

    private void RecordViewUse(IView view)
    {
      int index = this.views.IndexOf(view);
      if (index >= 0)
        this.views.RemoveAt(index);
      this.views.Insert(0, view);
    }

    private sealed class DocumentCollection : List<IDocument>, IDocumentCollection, IReadOnlyList<IDocument>, IReadOnlyCollection<IDocument>, IEnumerable<IDocument>, ICollection, IEnumerable
    {
    }

    private sealed class ViewCollection : List<IView>, IViewCollection, IReadOnlyList<IView>, IReadOnlyCollection<IView>, IEnumerable<IView>, ICollection, IEnumerable
    {
    }

    private sealed class Item
    {
      private DocumentService.ViewCollection views = new DocumentService.ViewCollection();
      private IDocument document;

      public IDocument Document
      {
        get
        {
          return this.document;
        }
      }

      public IViewCollection Views
      {
        get
        {
          return (IViewCollection) this.views;
        }
      }

      public Item(IDocument document)
      {
        if (document == null)
          throw new ArgumentNullException("document");
        this.document = document;
      }

      public void AddView(IView view)
      {
        if (this.views.Contains(view) || view == null)
          return;
        IDocumentView documentView = view as IDocumentView;
        if (documentView != null && documentView.Document != this.document)
          throw new InvalidOperationException();
        this.views.Add(view);
      }

      public void RemoveView(IView view)
      {
        this.views.Remove(view);
      }
    }

    private sealed class ItemCollection : KeyedCollection<IDocument, DocumentService.Item>
    {
      protected override IDocument GetKeyForItem(DocumentService.Item item)
      {
        return item.Document;
      }
    }

    private sealed class FileSaveCommand : Command
    {
      private IDocumentService documentService;
      private IMessageDisplayService messageDisplayService;

      public override bool IsEnabled
      {
        get
        {
          return this.documentService.ActiveDocument != null;
        }
      }

      public FileSaveCommand(IDocumentService documentService, IMessageDisplayService messageDisplayService)
      {
        this.documentService = documentService;
        this.messageDisplayService = messageDisplayService;
      }

      public override void Execute()
      {
        DocumentUtilities.SaveDocument(this.documentService.ActiveDocument, true, true, this.messageDisplayService);
      }
    }

    private sealed class FileCloseCommand : Command
    {
      private IDocumentService documentService;
      private IMessageDisplayService messageDisplayService;

      public override bool IsEnabled
      {
        get
        {
          return this.documentService.ActiveDocument != null;
        }
      }

      public FileCloseCommand(IDocumentService documentService, IMessageDisplayService messageDisplayService)
      {
        this.documentService = documentService;
        this.messageDisplayService = messageDisplayService;
      }

      public override void Execute()
      {
        IDocument activeDocument = this.documentService.ActiveDocument;
        if (activeDocument == null || !DocumentUtilities.PromptUserAndSaveDocument(activeDocument, true, this.messageDisplayService))
          return;
        this.documentService.CloseDocument(activeDocument);
      }
    }

    private sealed class EmptyActivateViewCommand : Command
    {
      private IViewService viewService;

      public EmptyActivateViewCommand(IViewService viewService)
      {
        this.viewService = viewService;
      }

      public override void Execute()
      {
      }

      public override void SetProperty(string propertyName, object propertyValue)
      {
      }

      public override object GetProperty(string propertyName)
      {
        switch (propertyName)
        {
          case "IsVisible":
            return (object) (bool) (this.viewService.Views.Count == 0 ? true : false);
          case "IsEnabled":
            return (object) false;
          default:
            return base.GetProperty(propertyName);
        }
      }
    }

    private sealed class ViewServiceDialogCommand : Command
    {
      private IViewService viewService;

      public ViewServiceDialogCommand(IViewService viewService)
      {
        this.viewService = viewService;
      }

      public override void Execute()
      {
        ViewServiceDialog viewServiceDialog = new ViewServiceDialog(this.viewService);
        bool? nullable = viewServiceDialog.ShowDialog();
        if ((!nullable.GetValueOrDefault() ? false : (nullable.HasValue ? true : false)) == false)
          return;
        this.viewService.ActiveView = viewServiceDialog.ActiveView;
      }

      public override object GetProperty(string propertyName)
      {
        switch (propertyName)
        {
          case "IsEnabled":
          case "IsVisible":
            return (object) (bool) (this.viewService.Views.Count > 10 ? true : false);
          default:
            return base.GetProperty(propertyName);
        }
      }
    }

    private sealed class ActivateViewCommand : Command
    {
      private IViewService viewService;
      private int index;

      public ActivateViewCommand(IViewService viewService, int index)
      {
        this.viewService = viewService;
        this.index = index;
      }

      public override void Execute()
      {
      }

      public override object GetProperty(string propertyName)
      {
        switch (propertyName)
        {
          case "Text":
            if (this.index >= this.viewService.Views.Count)
              return (object) string.Empty;
            if (this.index >= 9)
              return (object) ("1_0 " + this.viewService.Views[this.index].Caption);
            return (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "_{0} {1}", new object[2]
            {
              (object) (this.index + 1),
              (object) this.viewService.Views[this.index].Caption
            });
          case "IsEnabled":
          case "IsVisible":
            return (object) (bool) (this.index < this.viewService.Views.Count ? true : false);
          case "IsChecked":
            return (object) (bool) (this.index >= this.viewService.Views.Count ? false : (this.viewService.ActiveView == this.viewService.Views[this.index] ? true : false));
          default:
            return base.GetProperty(propertyName);
        }
      }

      public override void SetProperty(string propertyName, object propertyValue)
      {
        switch (propertyName)
        {
          case "IsChecked":
            if (this.index >= this.viewService.Views.Count || !(bool) propertyValue)
              break;
            this.viewService.ActiveView = this.viewService.Views[this.index];
            break;
          default:
            base.SetProperty(propertyName, propertyValue);
            break;
        }
      }
    }
  }
}
