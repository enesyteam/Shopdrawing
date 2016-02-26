// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Diagnostics.VisualTreeWalker
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.Diagnostics
{
  public class VisualTreeWalker
  {
    private Visual root;
    private int visualCount;

    public event VisualVisitor VisualVisited;

    public VisualTreeWalker(Visual root)
    {
      this.root = root;
    }

    public int Walk()
    {
      this.visualCount = 0;
      this.RecurseVisuals(this.root, 1);
      return this.visualCount;
    }

    private void RecurseVisuals(Visual visual, int currentDepth)
    {
      ++this.visualCount;
      this.OnVisualVisit(visual, currentDepth);
      for (int childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount((DependencyObject) visual); ++childIndex)
      {
        Visual visual1 = VisualTreeHelper.GetChild((DependencyObject) visual, childIndex) as Visual;
        if (visual1 != null)
          this.RecurseVisuals(visual1, currentDepth + 1);
      }
    }

    private void OnVisualVisit(Visual v, int currentDepth)
    {
      if (this.VisualVisited == null)
        return;
      this.VisualVisited(v, currentDepth);
    }
  }
}
