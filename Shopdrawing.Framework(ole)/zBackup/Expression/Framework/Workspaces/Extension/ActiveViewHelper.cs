// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Workspaces.Extension.ActiveViewHelper
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI.Shell;
using Microsoft.VisualStudio.PlatformUI.Shell.Controls;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.Workspaces.Extension
{
  public static class ActiveViewHelper
  {
    public static void SetActiveView(DependencyObject obj)
    {
      if (obj == null)
        return;
      ViewPresenter viewPresenter = (ViewPresenter) null;
      for (; obj != null; obj = VisualTreeHelper.GetParent(obj))
      {
        viewPresenter = obj as ViewPresenter;
        if (viewPresenter != null)
          break;
      }
      if (viewPresenter == null)
        return;
      if (viewPresenter.View != null)
      {
        ViewManager.Instance.ActiveView = viewPresenter.View;
      }
      else
      {
        if (!(viewPresenter.DataContext is NakedView))
          return;
        ViewManager.Instance.ActiveView = (View) null;
      }
    }

    public static void ClearActiveView()
    {
      ViewManager.Instance.ActiveView = (View) null;
    }
  }
}
