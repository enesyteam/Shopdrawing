// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.LayoutSynchronizedItemsControl
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.VisualStudio.PlatformUI
{
  public class LayoutSynchronizedItemsControl : ItemsControl
  {
    protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
    {
      base.OnItemsChanged(e);
      LayoutSynchronizer.Update((Visual) this);
    }
  }
}
