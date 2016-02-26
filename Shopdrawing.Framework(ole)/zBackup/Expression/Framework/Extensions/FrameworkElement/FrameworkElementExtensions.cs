// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Extensions.FrameworkElement.FrameworkElementExtensions
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.Framework.Extensions.FrameworkElement
{
  public static class FrameworkElementExtensions
  {
      public static IEnumerable<System.Windows.FrameworkElement> SelfAndAncestors(this System.Windows.FrameworkElement element)
    {
        for (; element != null; element = (element.TemplatedParent ?? element.Parent) as System.Windows.FrameworkElement)
        yield return element;
    }
  }
}
