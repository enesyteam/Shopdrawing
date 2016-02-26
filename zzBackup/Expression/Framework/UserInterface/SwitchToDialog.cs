// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.SwitchToDialog
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Microsoft.Expression.Framework.UserInterface
{
  internal class SwitchToDialog : Dialog
  {
    private IWindowService windowService;
    private IViewService viewService;
    private ObservableCollection<DocumentViewReference> documentViewReferences;
    private ICollectionView documentViewReferencesView;
    private ListBox documentListBox;
    private bool cancel;
    private static SwitchToDialog currentlyActiveDialog;

    public static bool IsInUse
    {
      get
      {
        return SwitchToDialog.currentlyActiveDialog != null;
      }
    }

    public override bool IsOverridingWindowsChrome
    {
      get
      {
        return false;
      }
    }

    public ObservableCollection<DocumentViewReference> DocumentViewReferences
    {
      get
      {
        return this.documentViewReferences;
      }
    }

    public SwitchToDialog(IWindowService windowService, IViewService viewService)
    {
      FrameworkElement element = FileTable.GetElement("Resources\\UserInterface\\SwitchToDialog.xaml");
      element.DataContext = (object) this;
      this.DialogContent = (UIElement) element;
      this.Title = StringTable.SwitchToDialogTitle;
      this.SizeToContent = SizeToContent.WidthAndHeight;
      this.WindowStyle = WindowStyle.None;
      this.documentListBox = element.FindName("DocumentList") as ListBox;
      this.windowService = windowService;
      this.viewService = viewService;
      this.documentViewReferences = new ObservableCollection<DocumentViewReference>();
      foreach (IView view in (IEnumerable<IView>) this.viewService.Views)
      {
        IDocumentView documentView = view as IDocumentView;
        if (documentView != null)
          this.documentViewReferences.Add(new DocumentViewReference(documentView));
      }
      this.documentViewReferencesView = CollectionViewSource.GetDefaultView((object) this.documentViewReferences);
      this.documentViewReferencesView.MoveCurrentToFirst();
      this.MoveSelectionByOne(true);
    }

    public static void FocusCurrentInstance()
    {
      if (SwitchToDialog.currentlyActiveDialog == null)
        return;
      SwitchToDialog.currentlyActiveDialog.Focus();
    }

    protected override void OnInitialized(EventArgs e)
    {
      SwitchToDialog.currentlyActiveDialog = this;
      base.OnInitialized(e);
    }

    protected override void OnClosed(EventArgs e)
    {
      if (!this.cancel)
      {
        DocumentViewReference documentViewReference = (DocumentViewReference) this.documentViewReferencesView.CurrentItem;
        if (documentViewReference != null)
          this.viewService.ActiveView = (IView) documentViewReference.DocumentView;
      }
      this.windowService.ReturnFocus();
      SwitchToDialog.currentlyActiveDialog = (SwitchToDialog) null;
      base.OnClosed(e);
    }

    private void MoveSelectionByOne(bool forwards)
    {
      ICollectionView collectionView = this.documentViewReferencesView;
      if (collectionView == null)
        return;
      if (!forwards)
      {
        collectionView.MoveCurrentToPrevious();
        if (collectionView.CurrentItem == null)
          collectionView.MoveCurrentToLast();
      }
      else
      {
        collectionView.MoveCurrentToNext();
        if (collectionView.CurrentItem == null)
          collectionView.MoveCurrentToFirst();
      }
      this.documentListBox.ScrollIntoView(collectionView.CurrentItem);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      if (e.Key == Key.Tab)
      {
        if (!e.IsDown)
          return;
        this.MoveSelectionByOne(!e.KeyboardDevice.IsKeyDown(Key.LeftShift) && !e.KeyboardDevice.IsKeyDown(Key.RightShift));
      }
      else if (e.Key == Key.Up)
      {
        if (!e.IsDown)
          return;
        this.MoveSelectionByOne(false);
      }
      else if (e.Key == Key.Down)
      {
        if (!e.IsDown)
          return;
        this.MoveSelectionByOne(true);
      }
      else
      {
        if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
          return;
        this.cancel = true;
        this.Close();
      }
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
      if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
        this.Close();
      else
        base.OnKeyUp(e);
    }
  }
}
