// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.LayoutSynchronizer
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.VisualStudio.PlatformUI
{
  public static class LayoutSynchronizer
  {
    private static HashSet<PresentationSource> elementsToUpdate = new HashSet<PresentationSource>();
    private static int layoutSynchronizationRefCount;
    private static bool isUpdatingLayout;

    public static bool IsSynchronizing
    {
      get
      {
        return LayoutSynchronizer.layoutSynchronizationRefCount > 0;
      }
    }

    public static IDisposable BeginLayoutSynchronization()
    {
      return (IDisposable) new LayoutSynchronizer.LayoutSynchronizationScope();
    }

    public static void Update(Visual element)
    {
      if (!LayoutSynchronizer.IsSynchronizing || LayoutSynchronizer.isUpdatingLayout)
        return;
      PresentationSource presentationSource = PresentationSource.FromVisual(element);
      if (presentationSource == null)
        return;
      LayoutSynchronizer.elementsToUpdate.Add(presentationSource);
    }

    private static void Synchronize()
    {
      if (LayoutSynchronizer.isUpdatingLayout)
        return;
      LayoutSynchronizer.isUpdatingLayout = true;
      try
      {
        foreach (PresentationSource presentationSource in LayoutSynchronizer.elementsToUpdate)
        {
          UIElement uiElement = presentationSource.RootVisual as UIElement;
          if (uiElement != null)
          {
            try
            {
              uiElement.UpdateLayout();
            }
            catch (Exception ex)
            {
            }
          }
        }
        LayoutSynchronizer.elementsToUpdate.Clear();
      }
      finally
      {
        LayoutSynchronizer.isUpdatingLayout = false;
      }
    }

    private class LayoutSynchronizationScope : DisposableObject
    {
      public LayoutSynchronizationScope()
      {
        ++LayoutSynchronizer.layoutSynchronizationRefCount;
      }

      protected override void DisposeManagedResources()
      {
        --LayoutSynchronizer.layoutSynchronizationRefCount;
        if (LayoutSynchronizer.layoutSynchronizationRefCount != 0)
          return;
        LayoutSynchronizer.Synchronize();
      }
    }
  }
}
