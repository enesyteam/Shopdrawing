// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Workspaces.Extension.ExpressionViewManager
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.UserInterface;
using Microsoft.VisualStudio.PlatformUI.Shell;
using Microsoft.VisualStudio.PlatformUI.Shell.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Expression.Framework.Workspaces.Extension
{
  internal class ExpressionViewManager : ViewManager
  {
    private int suppressViewActivationCount;

    public ExpressionViewManager()
    {
      EventManager.RegisterClassHandler(typeof (NakedViewControl), UIElement.PreviewMouseDownEvent, (Delegate) new MouseButtonEventHandler(this.OnViewMouseDown));
    }

    public IDisposable SuppressViewActivationOnGotFocus()
    {
      return (IDisposable) new ExpressionViewManager.SuppressViewActivationOnGotFocusToken(this);
    }

    protected override void OnAutoHideViewCore(ViewElement autoHidingElement, bool autoHideOnlyActiveView)
    {
      TabGroup tabGroup = autoHidingElement as TabGroup;
      if (tabGroup != null)
      {
        if (autoHideOnlyActiveView)
        {
          ExpressionView expressionView = tabGroup.SelectedElement as ExpressionView;
          if (expressionView != null)
            expressionView.WasSelectedBeforeAutoHide = false;
        }
        else
        {
          foreach (ViewElement viewElement in (IEnumerable<ViewElement>) tabGroup.Children)
          {
            ExpressionView expressionView = viewElement as ExpressionView;
            if (expressionView != null)
              expressionView.WasSelectedBeforeAutoHide = expressionView.IsSelected;
          }
        }
      }
      base.OnAutoHideViewCore(autoHidingElement, autoHideOnlyActiveView);
    }

    private void OnViewMouseDown(object sender, MouseButtonEventArgs args)
    {
      this.ActivateViewFromPresenter(sender as ViewPresenter);
    }

    protected override FloatingWindowManager CreateFloatingWindowManager()
    {
      return (FloatingWindowManager) new ExpressionFloatingWindowManager();
    }

    protected bool ShouldActivateFromFocusChange(RoutedEventArgs args)
    {
      if (this.suppressViewActivationCount == 0)
        ViewManager.ShouldActivateFromFocusChange(args);
      return false;
    }

    internal override void OnViewPreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs args)
    {
      FocusScopeManager.HandlePreviewGotKeyboardFocus(sender, args);
    }

    protected override void ActivateViewFromPresenter(ViewPresenter presenter)
    {
      if (presenter != null && presenter.DataContext is NakedView)
        ViewManager.Instance.ActiveView = (View) null;
      else
        base.ActivateViewFromPresenter(presenter);
    }

    private sealed class SuppressViewActivationOnGotFocusToken : IDisposable
    {
      private ExpressionViewManager viewManager;
      private bool disposed;

      public SuppressViewActivationOnGotFocusToken(ExpressionViewManager viewManager)
      {
        this.viewManager = viewManager;
        ++this.viewManager.suppressViewActivationCount;
      }

      public void Dispose()
      {
        if (this.disposed)
          return;
        --this.viewManager.suppressViewActivationCount;
        this.disposed = true;
      }
    }
  }
}
